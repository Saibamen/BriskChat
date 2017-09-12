$(".ui.sidebar.left, .ui.sidebar.right").sidebar();

$("#directMessagesTitle, #channelsCount, #closerightbar, #deleteRoomButton").popup({
    variation: "inverted"
});

$(document).on("reloadPopups", function () {
    $(".ts_tip").popup({
        variation: "inverted"
    });

    $(".item > .btn_unstyle.right").popup({
        variation: "inverted"
    });
});

var Autolinker = new Autolinker({
    stripPrefix: false,
    stripTrailingSlash: false,
    phone: false
});

// Globals
var globalDomainName;
var globalUserName;
var globalUserId;
var loading = true;

var currentTheme;
var topicInDatabase;
var roomNameInDatabase;
var descriptionInDatabase;
var themeInDatabase;
var currentRoomId = 0;
var isCurrentRoomPrivateConversation;

var clippyShow = false;

var GravatarUrl = "https://www.gravatar.com/avatar/";
var GravatarOptions = "d=retro";

function loadingStart() {
    loading = true;
    $(".ui.main.container").css("display", "none");
    $(".ui.dimmer").addClass("active");
}

function loadingStop() {
    loading = false;

    if ($(".ui.sidebar.right").hasClass("visible")) {
        $(".ui.main.container").css("cssText", "margin-left: 260px !important");
    } else {
        $(".ui.main.container").removeAttr("style");
    }

    $(".ui.dimmer").removeClass("active");
    addActionsToMessages();
    printLog("Loader stops");
}

function addActionsToMessages() {
    $(".ts-message").not(":has(> .action_hover_container), #message_edit_container").each(function () {
        // This is our message?
        if ($(this).find(".message_sender").text() !== globalUserName || $(this).find(".member_image").data("member-id") !== globalUserId) {
            return true;
        }

        $(this).prepend('<div class="action_hover_container stretch_btn_heights narrow_buttons" data-js="action_hover_container" data-show_rxn_action="true">\
            <button type="button" data-action="edit" class="btn_unstyle btn_msg_action ts_tip" data-content="Edit message" data-position="bottom center">\
            <i class="edit icon"></i>\
            </button>\
            <button type="button" data-action="delete" class="btn_unstyle btn_msg_action ts_tip danger" data-content="Delete message" data-position="bottom right">\
            <i class="delete icon"></i>\
            </button>\
            </div>');

        return true;
    });
}

/*
 *  Resizing
 */

$(window).resize(function () {
    // #chat_messages
    var chatAdditionalHeight = 117;
    var messages = $(".ui.message");

    if (messages.length) {
        if (!messages.hasClass("hidden")) {
            chatAdditionalHeight += 51;
        }
    }

    $("#chat_messages").height($(window).height() - chatAdditionalHeight);

    // .sidebar_channels_messages
    var sidebarAdditionalHeight = 165;

    $(".sidebar_channels_messages").height($(window).height() - sidebarAdditionalHeight);
});

$(window).trigger("resize");

/*
 *  SignalR
 */

$.connection.hub.url = "http://localhost:52284/signalr";
var myHub = $.connection.channelHub;

$("#channelsCount").click(function () {
    if (loading) {
        return;
    }

    $("#browseChannelsForm")[0].reset();

    var domainRooms = myHub.server.getDomainPublicAndUserRooms();
    var thisModal = $(".ui.basic.browse-channels.modal");

    $.when(domainRooms).then(function () {
        $(thisModal).modal({
            closable: false,
            onShow: function () {
                $(".ui.dimmer.modals").css("background-color", "#fff");
            },
            onHide: function () {
                $(".ui.dimmer.modals").css("background-color", "");
            }
        }).modal("show");
    });
});

function changeRoom(newRoom) {
    var newRoomId;

    if ((newRoomId = newRoom.data("id")) && currentRoomId !== newRoomId) {
        // Leave current room
        loadingStart();
        // Clear messages
        $("#chat_messages").empty();
        myHub.server.leaveRoom(currentRoomId);
        $(".menu > a.item.active").removeClass("active");

        var joinRoom = myHub.server.joinRoom(newRoomId);

        $.when(joinRoom).then(function () {
            currentRoomId = newRoomId;

            if (newRoom.data("name") && !$("#channelsMenu").find("a:contains(" + newRoom.data("name") + ")").length) {
                printLog("Add channel to sidebar from changeRoom()");
                addRoomToSidebar(newRoom.data("name"), newRoomId, newRoom.data("ispublic"));
                newRoom = $("#channelsMenu").find("a[data-id='" + newRoomId + "']");
            } else if (newRoom.is("div")) {
                // If this is "a href" - skip this, because we change room from sidebar
                newRoom = $("#channelsMenu").find("a[data-id='" + newRoomId + "']");
            }

            newRoom.addClass("active");
            $("#channel_title").html(newRoom.html());

            var getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

            $.when(getRoomInformation).then(function () {
                loadingStop();
            });
        });
    }
}

// Change room
$(".menu").on("click", ".menu > a.item", function (e) {
    if (loading) {
        return;
    }

    changeRoom($(e.currentTarget));
});

function addEditContainer(id, oldMessageText, messageIcon) {
    return '<div class="ts-message message highlight" id="message_edit_container">\
            <div class="message_gutter">\
                <div class="message_icon">\
                    ' + messageIcon + '\
                </div>\
            </div>\
            <form id="message_edit_form" data-id="' + id + '">\
                <button class="btn_unstyle emo_menu" aria-label="Emoji menu" type="button">\
                    <i class="meh icon"></i>\
                </button>\
                <div id="msg_text" class="message_input ql-container focus">\
                    <div class="ql-editor" role="textbox" tabindex="0" aria-multiline="true" aria-haspopup="true" spellcheck="true" contenteditable="true"><p>' + oldMessageText + '</p></div>\
                    <div class="ql-clipboard" tabindex="-1" aria-hidden="true" role="presentation" spellcheck="true" contenteditable="true"></div>\
                </div>\
                <a id="cancel_edit" role="button" class="ui button mini">Cancel</a>\
                <a id="commit_edit" role="button" class="ui button mini positive"><i class="icon edit"></i>Save Changes</a>\
                <span id="message_editing_info" class="mini" style="display: none;">Finish editing this message first! Or press <strong>escape</strong> if youve changed your mind.</span>\
            </form>\
        </div>';
}

// Editing message
$("#chat_messages").on("click", ".ts-message .btn_msg_action[data-action='edit']", function (e) {
    // Only one edit container on site
    if ($(document).find("#message_edit_container").length) {
        $(document).find("#message_edit_container").prev().show();
        $(document).find("#message_edit_container").remove();
    }

    var message = $(e.target).closest(".ts-message");
    var messageId = message.data("id");
    var oldMessageText = message.find(".message_body").text();

    // Adding edit container and hide message
    message.hide();
    message.after(addEditContainer(messageId, oldMessageText, message.find(".message_icon").html()));

    // Focus on textbox
    var ql_editor = $(".ql-editor");
    ql_editor.focus();

    var range = document.createRange();
    range.selectNodeContents(ql_editor.get(0));
    range.collapse(false);
    var sel = window.getSelection();
    sel.removeAllRanges();
    sel.addRange(range);

    if ($(document).find("#message_edit_container").length) {
        document.addEventListener("click", clickOutsideEditContainer, true);
    }

    // Cancel
    $(".ts-message").on("click", "#cancel_edit", function (x) {
        document.removeEventListener("click", clickOutsideEditContainer, true);
        message.show();
        $(x.target).closest("#message_edit_container").remove();
    });

    // Escape key
    $("#message_edit_container").keydown(function (x) {
        if (x.keyCode === 27) {
            document.removeEventListener("click", clickOutsideEditContainer, true);
            $(document).find("#message_edit_container").prev().show();
            $(document).find("#message_edit_container").remove();
        }
    });

    $("#message_edit_form").keypress(function (x) {
        // Enter key
        if (x.which === 13) {
            if (!x.shiftKey) {
                var editedMessage = $(".ql-editor").text().trim();

                if (oldMessageText !== editedMessage) {
                    if (editedMessage) {
                        printLog("Editing: " + editedMessage + " to room " + currentRoomId);
                        myHub.server.editMessage(currentRoomId, messageId, editedMessage);
                    }
                }

                document.removeEventListener("click", clickOutsideEditContainer, true);
                message.show();
                $(x.target).closest("#message_edit_container").remove();
            }
        }
    });

    // Save
    $(".ts-message").on("click", "#commit_edit", function (x) {
        // Send to server when text is changed
        var editedMessage = $(".ql-editor").text().trim();

        if (oldMessageText !== editedMessage) {
            if (editedMessage) {
                printLog("Editing: " + editedMessage + " to room " + currentRoomId);
                myHub.server.editMessage(currentRoomId, messageId, editedMessage);
            }
        }

        document.removeEventListener("click", clickOutsideEditContainer, true);
        message.show();
        $(x.target).closest("#message_edit_container").remove();
    });

    function clickOutsideEditContainer(event) {
        // Do only if #message_edit_container exists
        if ($(document).find("#message_edit_container").length) {
            // Check for x.target and parents
            if (!$(event.target).parents().addBack().is("#message_edit_container")) {
                document.removeEventListener("click", clickOutsideEditContainer, true);
                $(document).find("#message_edit_container").prev().show();
                $(document).find("#message_edit_container").remove();
            }
        }
    }
});

$(".grid").on("click", ".private-conversation-row", function (e) {
    var item = e.currentTarget;
    var append = $(item).data("name");
    var id = $(item).data("id");
    var input = $("#private_inputtext");

    $(item).attr("data-is-selected", "true");
    $(item).hide();

    $(input).attr("placeholder", "");

    $("#createPrivateConversationForm").find(":submit").removeClass("disabled").addClass("positive");

    $(input).before('<span class="private-conversation-tag" data-id="' + id + '">' + append + ' <i class="remove icon"></i></span>');
});

$(".grid").on("click", ".invite-users-row", function (e) {
    var item = e.currentTarget;
    var append = $(item).data("name");
    var id = $(item).data("id");
    var input = $("#invite_inputtext");

    $(item).attr("data-is-selected", "true");
    $(item).hide();

    $(input).attr("placeholder", "");

    $("#inviteUsersForm").find(":submit").removeClass("disabled").addClass("positive");

    $(input).before('<span class="invite-users-tag" data-id="' + id + '">' + append + ' <i class="remove icon"></i></span>');
});

// This deletes the message
$("#chat_messages").on("click", ".ts-message .btn_msg_action[data-action='delete']", function (e) {
    $(".ui.delete.modal").modal({
        onApprove: function () {
            // Deleting message
            var messageId = $(e.target).closest(".ts-message").data("id");
            myHub.server.deleteMessage(currentRoomId, messageId);
        }
    })
    .modal("show");
});

myHub.client.broadcastEditedMessage = function (messageId, messageText) {
    var message = $(".ts-message[data-id='" + messageId + "']");
    var messageBodies = message.find(".message_body");

    // Change text
    messageBodies.first().html(Autolinker.link(parseEmoticons(messageText)));

    var youTubeMatch = messageText.match(/watch\?v=([a-zA-Z0-9\-_]+)/);

    if (youTubeMatch) {
        var youtubeIframeHtml = '<iframe src="https://www.youtube.com/embed/' +
            youTubeMatch[1] +
            '?feature=oembed&amp;autoplay=0&amp;iv_load_policy=3" allowfullscreen="" height="300" frameborder="0" width="400"></iframe>';

        if (messageBodies.last().hasClass("youtube_iframe")) {
            messageBodies.last().html(youtubeIframeHtml);
        } else {
            messageBodies.last().after(youtubeIframeHtml);
        }
    } else if (messageBodies.last().hasClass("youtube_iframe")) {
        messageBodies.last().hide("slow", function () {
            messageBodies.last().remove();
        });
    }
};

// Add new emoticon names here:
var additionalEmoticonNames = ["poop", "party"];

var emoticonRegex = "(:smile:|\\B:-?[)]\\B)|(:smile_big:|\\B:-?[D]\\b)|(:sad:|\\B:-?[(]\\B)|(:tongue:|\\B:-?[P]\\b)|(:crying:|\\B;-?[(]\\B)|(:wink:|\\B;-?[)]\\B)";

// Add emoticon names to regex string for emoReplacer() function
for (var i = 0; i < additionalEmoticonNames.length; i++) {
    emoticonRegex += "|(:" + additionalEmoticonNames[i] + ":)";
}

var emoticonsGroup = {
    1: "smile", 2: "smile_big", 3: "sad", 4: "tongue", 5: "crying", 6: "wink"
};

// Add emoticon names to object for parseEmoticons(text) function
for (var i = Object.keys(emoticonsGroup).length + 1, j = 0; j < additionalEmoticonNames.length; i++, j++) {
    emoticonsGroup[i] = additionalEmoticonNames[j];
}

function emoReplacer() {
    var args = Array.prototype.slice.call(arguments);

    // Arguments for emoReplacer() are: match, regexGroup1, ..., regexGroupN, offset, string
    for (var i = 1; i < args.length - 2; i++) {
        if (args[i]) {
            return '<span class="emoji-outer emoji-sizer" style="background: url(/images/emoticons/' + emoticonsGroup[i] + '.png)" title="' + emoticonsGroup[i] + '">:' + emoticonsGroup[i] + ":</span>";
        }
    }
}

function parseEmoticons(text) {
    // https://regex101.com/r/zJ9XXL/11
    var regex = new RegExp(emoticonRegex, "gi");
    text = text.replace(regex, emoReplacer);

    return text;
}

var loadingMessageOffset = false;

// Load previous messages when we scroll up
$("#chat_messages").scroll(function () {
    if (!$(".ui.dimmer").hasClass("active") && !loadingMessageOffset) {
        var height = $("#chat_messages").scrollTop();

        if (height < 100) {
            loadingMessageOffset = true;
            var firstMessage = $("#chat_messages").children().first();
            $(".ui.dimmer").addClass("active");

            var getPreviousMessages = myHub.server.getPreviousMessages(currentRoomId, firstMessage.data("id"));

            $.when(getPreviousMessages).then(function () {
                var previousMessages = firstMessage.prevAll();

                if (previousMessages.length > 0) {
                    var prevHeight = 0;

                    previousMessages.each(function () {
                        prevHeight += $(this).outerHeight();
                    });

                    $("#chat_messages").scrollTop(prevHeight);
                }

                $(".ui.dimmer").removeClass("active");
                loadingMessageOffset = false;
            });
        }
    }
});

function getMessageHtml(userName, userId, messageId, messageText, timestamp, emailHash) {
    return '<div class="ts-message target" data-id="' + messageId + '"><div class="message_gutter"><div class="message_icon"><a href="/team/malgosia" target="/team/malgosia" class="member_image" data-member-id="' + userId + '" style="background-image: url(' + GravatarUrl + emailHash + '?s=36&' + GravatarOptions + ')" aria-hidden="true" tabindex="-1"> </a></div></div><div class="message_content"><div class="message_content_header"><a href="#" class="message_sender">' + userName + '</a><a href="#" class="timestamp">' + timestamp + '</a></div><span class="message_body">' + Autolinker.link(parseEmoticons(messageText));
}

myHub.client.broadcastMessage = function (userName, userId, messageId, messageText, timestamp, emailHash) {
    var messageHtml = getMessageHtml(userName, userId, messageId, messageText, timestamp, emailHash);

    var youTubeMatch = messageText.match(/watch\?v=([a-zA-Z0-9\-_]+)/);

    if (youTubeMatch) {
        messageHtml += '</span><span class="message_body youtube_iframe"><iframe src="https://www.youtube.com/embed/' +
            youTubeMatch[1] +
            '?feature=oembed&amp;autoplay=0&amp;iv_load_policy=3" allowfullscreen="" height="300" frameborder="0" width="400"></iframe></span></div></div>';
    } else {
        messageHtml += "</span></div></div>";
    }

    var chat_messages = $("#chat_messages");
    chat_messages.append(messageHtml);

    // Scroll #chat_messages
    chat_messages.clearQueue();
    chat_messages.animate({ scrollTop: chat_messages[0].scrollHeight }, "slow");
    addActionsToMessages();
    $(document).trigger("reloadPopups");
};

myHub.client.deleteMessage = function (messageId) {
    var message = $(".ts-message[data-id='" + messageId + "']");

    printLog("Deleting message with ID: " + message.data("id"));

    message.hide("slow", function () {
        message.remove();
    });
};

myHub.client.loadRooms = function (result) {
    var setActive = true;
    $("#channelsCount").text("CHANNELS (" + result.length + ")");

    $.each(result, function (index, value) {
        var divToAppend = '<a class="item';

        if (setActive) {
            divToAppend += " active";
            setActive = false;
            currentRoomId = value.Id;
        }

        divToAppend += '"data-id="' + value.Id + '" data-ispublic="' + value.IsPublic + '">';

        if (value.IsPublic) {
            divToAppend += '<i class="icon left">#</i>';
        } else {
            divToAppend += '<i class="lock icon left"></i>';
        }

        divToAppend += value.Name + "</a>";
        $("#channelsMenu").append(divToAppend);
    });
};

myHub.client.loadDomainPublicAndUserRooms = function (result) {
    $("#browseChannelsList").empty();

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row browse-room-row" data-id="' + value.Id + '" data-name="' + value.Name + '" data-ispublic="' + value.IsPublic + '">';
        divToAppend += '<div class="eight wide column">';

        if (value.IsPublic) {
            divToAppend += "# ";
        } else {
            divToAppend += '<i class="lock icon"></i> ';
        }

        divToAppend += "<strong>" + value.Name + "</strong>";

        // Mark joined channels
        if ($("#channelsMenu").find("a:contains(" + value.Name + ")").length) {
            divToAppend += "  <small>JOINED</small>";
        }

        divToAppend += "<br><i>Created by <strong>" + value.Owner + "</strong> on " + value.CreatedOn + "</i>";

        divToAppend += "<br>" + value.Description + "</div>";

        divToAppend += '<div class="six wide column"></div>';
        divToAppend += '<div class="two wide column mycheckmark"><i class="level down rotated big blue icon"></i></div>';
        divToAppend += "</div>";

        $("#browseChannelsList").append(divToAppend);
    });
};

$(".grid").on("click", ".browse-room-row", function (e) {
    var item = $(e.currentTarget);

    changeRoom(item);
    $(".ui.basic.browse-channels.modal").modal("hide");
});

myHub.client.loadPrivateConversations = function (result) {
    $.each(result, function (index, value) {
        // TODO: Status icon
        var divToAppend = '<a class="item" data-id="' + value.Id + '"><i class="lock icon left"></i>' + value.Name + "</a>";

        $("#privateConversationsMenu").append(divToAppend);
    });
};

myHub.client.parseLastMessages = function (result) {
    $.each(result, function (index, value) {
        var messageHtml = getMessageHtml(value.UserName, value.UserId, value.Id, value.Text, value.CreatedOn, value.EmailHash);

        var youTubeMatch = value.Text.match(/watch\?v=([a-zA-Z0-9\-_]+)/);

        if (youTubeMatch) {
            messageHtml +=
                '</span><span class="message_body youtube_iframe"><iframe src="https://www.youtube.com/embed/' +
                youTubeMatch[1] +
                '?feature=oembed&amp;autoplay=0&amp;iv_load_policy=3" allowfullscreen="" height="300" frameborder="0" width="400"></iframe></span></div></div>';
        } else {
            messageHtml += "</span></div></div>";
        }

        $("#chat_messages").append(messageHtml);
    });

    // Scroll #chat_messages
    var chat_messages = $("#chat_messages");
    $(".ui.main.container").css("display", "block");
    chat_messages.clearQueue();
    chat_messages.animate({ scrollTop: chat_messages[0].scrollHeight }, "slow");
    addActionsToMessages();
    $(document).trigger("reloadPopups");
};

myHub.client.parseOffsetMessages = function (result) {
    $.each(result, function (index, value) {
        var messageHtml = getMessageHtml(value.UserName, value.UserId, value.Id, value.Text, value.CreatedOn, value.EmailHash);

        var youTubeMatch = value.Text.match(/watch\?v=([a-zA-Z0-9\-_]+)/);

        if (youTubeMatch) {
            messageHtml +=
                '</span><span class="message_body youtube_iframe"><iframe src="https://www.youtube.com/embed/' +
                youTubeMatch[1] +
                '?feature=oembed&amp;autoplay=0&amp;iv_load_policy=3" allowfullscreen="" height="300" frameborder="0" width="400"></iframe></span></div></div>';
        } else {
            messageHtml += "</span></div></div>";
        }

        $("#chat_messages").prepend(messageHtml);
    });

    addActionsToMessages();
    $(document).trigger("reloadPopups");
};

function addRoomToSidebar(channelName, roomId, isPublic) {
    var divToAppend = '<a class="item" data-id="' + roomId + '" data-ispublic="' + isPublic + '">';

    if (isPublic) {
        divToAppend += '<i class="icon left">#</i>';
    } else {
        divToAppend += '<i class="lock icon left"></i>';
    }

    divToAppend += channelName + "</a>";
    $("#channelsMenu").append(divToAppend);
    updateChannelsCount(1);
}

myHub.client.channelAddedAction = function (channelName, roomId, isPublic) {
    addRoomToSidebar(channelName, roomId, isPublic);
    changeRoom($("#channelsMenu").children().last());
};

myHub.client.privateConversationAddedAction = function (value) {
    // TODO: Status icon
    var divToAppend = '<a class="item" data-id="' + value.Id + '"><i class="lock icon left"></i>' + value.Name + "</a>";

    var menu = $("#privateConversationsMenu");
    menu.append(divToAppend);
    changeRoom(menu.children().last());
};

myHub.client.inviteUsersAddedAction = function () {
    printLog("inviteUsersAddedAction");
    loadingStop();
};

myHub.client.privateConversationsUsersLoadedAction = function (result) {
    $("#privateConversationsUserList").empty();

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row private-conversation-row" data-id="' + value.Id + '" data-name="' + value.Name + '" data-is-selected="false">';
        divToAppend += '<div class="eight wide column"><img class="gravatarMembersList" src="' + GravatarUrl + value.EmailHash + "?s=20&" + GravatarOptions + '" alt="avatar"><strong>' + value.Name + "</strong> ";

        if (value.IsOnline) {
            divToAppend += '<i class="circle icon green"></i>';
        } else {
            divToAppend += '<i class="circle thin icon"></i>';
        }

        divToAppend += "</div>";
        divToAppend += '<div class="six wide column"></div>';
        divToAppend += '<div class="two wide column mycheckmark"><i class="level down rotated big blue icon"></i></div>';
        divToAppend += "</div>";

        $("#privateConversationsUserList").append(divToAppend);
    });
};

myHub.client.notInvitedUsersLoadedAction = function (result) {
    $("#inviteUsersList").empty();

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row invite-users-row" data-id="' + value.Id + '" data-name="' + value.Name + '" data-is-selected="false">';
        divToAppend += '<div class="eight wide column"><img class="gravatarMembersList" src="' + GravatarUrl + value.EmailHash + "?s=20&" + GravatarOptions + '" alt="avatar"><strong>' + value.Name + "</strong> ";

        if (value.IsOnline) {
            divToAppend += '<i class="circle icon green"></i>';
        } else {
            divToAppend += '<i class="circle thin icon"></i>';
        }

        divToAppend += "</div>";
        divToAppend += '<div class="six wide column"></div>';
        divToAppend += '<div class="two wide column mycheckmark"><i class="level down rotated big blue icon"></i></div>';
        divToAppend += "</div>";

        $("#inviteUsersList").append(divToAppend);
    });
};

myHub.client.setDomainInformation = function (domainName, userName, userId) {
    globalDomainName = domainName;
    globalUserName = userName;
    globalUserId = userId;

    $("#team_name").text(globalDomainName);
    $("#team_menu_user_name").text(globalUserName);
    // Add domain name to HTML <title> tag
    document.title = globalDomainName + " " + document.title;
};

myHub.client.updateRoomUsersCount = function (roomUsersCount) {
    $("#channel_members_toggle_count_count").text(roomUsersCount);
};

/*
 * Start the connection
 */
$.connection.hub.start()
    .done(function () {
        printLog("Connected to Hub. Connection ID: "+ $.connection.hub.id +". Getting rooms");
        // Getting rooms
        var getRooms = myHub.server.getRooms();
        var getPrivateConversations = myHub.server.getPrivateConversations();

        $.when(getRooms && getPrivateConversations).then(function () {
            var bootTime = ($.now() - startTime) / 1000;
            printLog("Finished first boot " + bootTime + " seconds after DOM ready");

            // Joining to room
            var joinRoom = myHub.server.joinRoom(currentRoomId);
            $.when(joinRoom).then(function () {
                printLog("Connected to room after boot");
                var firstChannelTitle = $(".menu > a.item.active");
                $("#channel_title").html($(firstChannelTitle).html());

                var getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

                $.when(getRoomInformation).then(function () {
                    loadingStop();

                    $("#msg_form").keypress(function (e) {
                        if (e.which === 13) {
                            if (!e.shiftKey) {
                                var message = $("#msg_input").val().trim();

                                if (message) {
                                    if (message === "zerg") {
                                        printLog("You Must Construct Additional Pylons!");
                                        new ZergRush(100);
                                    } else if (message === "clippy") {
                                        if (!clippyShow) {
                                            printLog("Best Word users friend");
                                            clippyShow = true;
                                            clippy.load("Clippy", function(agent) {
                                                agent.show();
                                                agent.play("Greeting");
                                                agent.speak("Hi! Remember me?");

                                                setInterval(function() {
                                                    agent.animate();
                                                }, 20000);
                                            });
                                        } else {
                                            $(".clippy").hide("slow", function () {
                                                $(".clippy").remove();
                                                $(".clippy-balloon").remove();
                                            });
                                            clippyShow = false;
                                        }
                                    } else {
                                        printLog("Sending: " + message + " to room " + currentRoomId);
                                        myHub.server.sendMessage(currentRoomId, message);
                                    }
                                }

                                e.preventDefault();
                                $("#msg_input").val("");
                            }
                        }
                    });
                });
            });
        });
    })
    .fail(function (a) {
        printLog("Not connected! " + a);
    });

$.connection.hub.error = function (error) {
    console.error(error);
    printLog(error);
};

$.connection.hub.stateChanged(function (change) {
    if (change.newState === $.signalR.connectionState.reconnecting) {
        printLog("Re-connecting");
        loadingStart();
    }
});

$.connection.hub.reconnected(function () {
    printLog("Reconnected");
    loadingStop();
});

window.onbeforeunload = function () {
    $.connection.hub.stop();
};

$("#createNewChannel, #newChannel").click(function () {
    if (loading) {
        return;
    }

    $("#createChanelForm")[0].reset();

    var item = $("input[name*='IsPublic']")[1];
    var thisModal = $(".ui.basic.create-room.modal");
    item.value = false;

    $(thisModal).modal({
        closable: false,
        onShow: function () {
            $(".ui.dimmer.modals").css("background-color", "#fff");
        },
        onHide: function () {
            $(".ui.dimmer.modals").css("background-color", "");
        }
    }).modal("show");
});

$("#createNewPrivateConversation, #directMessagesTitle").click(function () {
    if (loading) {
        return;
    }

    var form = $("#createPrivateConversationForm");
    form[0].reset();
    // Remove all added tags
    form.find(".private-conversation-tag").remove();

    var domainUsers = myHub.server.getUsersFromDomain();
    var thisModal = $(".ui.basic.create-private-conversation.modal");

    $.when(domainUsers).then(function () {
        $(thisModal).modal({
            closable: false,
            onShow: function () {
                $(".ui.dimmer.modals").css("background-color", "#fff");
            },
            onHide: function () {
                $(".ui.dimmer.modals").css("background-color", "");
            }
        }).modal("show");
    });
});

function inviteUsers() {
    if (loading) {
        return;
    }

    var form = $("#inviteUsersForm");
    form[0].reset();
    // Remove all added tags
    form.find(".invite-users-tag").remove();

    var notInvitedUsers = myHub.server.getNotInvitedUsers(currentRoomId);
    var thisModal = $(".ui.basic.invite-users.modal");

    $.when(notInvitedUsers).then(function () {
        $(thisModal).modal({
            closable: false,
            onShow: function () {
                $(".ui.dimmer.modals").css("background-color", "#fff");
            },
            onHide: function () {
                $(".ui.dimmer.modals").css("background-color", "");
            }
        }).modal("show");
    });
};

$("#private_inputtext").keyup(function() {
    searchPrivateModal($(this).val());
});

function searchPrivateModal(value) {
    var items = $(".private-conversation-row[data-is-selected='false']");
    
    modalListSearch(value, items);
}

$("#invite_inputtext").keyup(function() {
    searchInviteModal($(this).val());
});

function searchInviteModal(value) {
    var items = $(".invite-users-row[data-is-selected='false']");
    
    modalListSearch(value, items);
}

$("#findChannelName").keyup(function() {
    var value = $(this).val();
    var items = $(".browse-room-row");
    
    modalListSearch(value, items);
});

function modalListSearch(value, items) {
    if (!value.length) {
        items.show();
        return;
    }
    
    var exp = new RegExp(value, "i");

    items.each(function() {
        var isMatch = exp.test($(this).data("name"));
        $(this).toggle(isMatch);
    });
}

$("#myCheckBox").click(function () {
    if ($(this).is(":checked")) {
        $("#createNewChannelHeader").html("Create a channel");
        $("#createNewChannelLabel").html("Anyone on your team can view and join this channel");
        $("input[name*='IsPublic']")[1].value = true;
    } else {
        $("#createNewChannelHeader").html("Create a private channel");
        $("#createNewChannelLabel").html("This channel can only be joined by invite");
        $("input[name*='IsPublic']")[1].value = false;
    }
});

$("#createChanelForm").submit(function (e) {
    loadingStart();
    e.preventDefault();
    var data = serializeForm($(this));
    myHub.server.createNewChannel(data);
    $(".ui.basic.create-room.modal").modal("hide");
});

$("#createPrivateConversationForm").submit(function (e) {
    printLog("Begin submit #createPrivateConversationForm");
    e.preventDefault();
    var usersIdList = [];
    var usersNameList = [];

    var conversationTags = $("#createPrivateConversationForm").find(".private-conversation-tag");

    conversationTags.each(function (index, element) {
        usersIdList.push($(element).data("id"));
        usersNameList.push($.trim($(element).text()));
    });

    if (usersIdList.length > 0) {
        var allPrivs = $("#privateConversationsMenu a");
        var searchedPriv = {};
        var selectedRoomUsersList = [];

        allPrivs.each(function (i, val) {
            selectedRoomUsersList = $(val).text().split(", ");

            // Search only on equal size
            if (selectedRoomUsersList.length !== usersNameList.length) {
                return true;
            }

            var matchingNames = 0;

            $(usersNameList).each(function(userNameIndex, userNameElement) {
                $(selectedRoomUsersList).each(function(selectedRoomIndex, selectedRoomElement) {
                    if (selectedRoomElement === userNameElement) {
                        matchingNames += 1;

                        return false;
                    }
                });
            });

            if (matchingNames === usersNameList.length) {
                // TODO: Bug with adding priv conv in backend :(
                searchedPriv = $(val);

                return false;
            }
        });

        if (searchedPriv.length) {
            printLog("Change room");
            changeRoom(searchedPriv);
        } else {
            printLog("Add new private conversation channel");
            loadingStart();
            myHub.server.createNewPrivateConversation(usersIdList);
        }

        $(".ui.basic.create-private-conversation.modal").modal("hide");
    }
});

$("#inviteUsersForm").submit(function (e) {
    printLog("Begin submit #inviteUsersForm");
    e.preventDefault();
    var usersIdList = [];

    var conversationTags = $("#inviteUsersForm").find(".invite-users-tag");

    conversationTags.each(function (index, element) {
        usersIdList.push($(element).data("id"));
    });

    loadingStart();
    printLog("Inviting new users");
    myHub.server.inviteUsersToPrivateRoom(currentRoomId, usersIdList);

    $(".ui.basic.invite-users.modal").modal("hide");
});

function updateChannelsCount(diff) {
    var counter = $("#channelsCount");
    var number = counter.text().match(/\d+/);
    number = parseInt(number) + diff;
    counter.text("CHANNELS (" + number + ")");
}

function serializeForm(form) {
    var data = $(form).serializeArray();
    var obj = {};

    $.each(data, function (key, value) {
        obj[value.name] = value.value;
    });

    return obj;
}

function channelSettings() {
    var rightbar = $("#Rightbar");
    rightbar.html("");

    // TODO: Implement deleting room
    $("#rightbar_Title").html("Channel Settings");

    var divToAppend = '<div class="item">\
                            <div class="ui styled accordion">\
                                    <div class="accordion">\
                                        <div class="title"><i class="theme icon"></i>Customization</div>\
                                        <div class="content" id="customization">\
                                            <div class="ui form">\
                                                <div class="field">\
                                                    <select id="selecttheme" class="ui dropdown" onchange="selectTheme(this);">\
                                                        <option value="0">Black</option>\
                                                        <option value="1">Blue</option>\
                                                        <option value="2">Green</option>\
                                                        <option value="3">Orange</option>\
                                                        <option value="4">Pink</option>\
                                                    </select>\
                                                </div>\
                                            </div>\
                                        </div>\
                                        <div class="title"><i class="wpforms icon"></i>Topic</div>\
                                        <div class="content" id="Topic">\
                                            <div class="ui form">\
                                                <div class="field">\
                                                <input type="text" id="infoTopic">\
                                                </div>\
                                            </div>\
                                        </div>\
                                        <div class="title"><i class="wpforms icon"></i>Description</div>\
                                        <div class="content" id="roomDescription">\
                                            <div class="ui form">\
                                                <div class="field">\
                                                <input type="text" id="infoDescription">\
                                                </div>\
                                            </div>\
                                        </div>\
                                        <div class="title"><i class="wpforms icon"></i>Room Name</div>\
                                        <div class="content" id="roomName">\
                                            <div class="ui form">\
                                                <div class="field">\
                                                <input type="text" id="infoName">\
                                                </div>\
                                            </div>\
                                        </div>\
                                        <div class="title"><i class="delete icon"></i>Delete Room</div>\
                                        <div class="content" id="deleteRoom">\
                                            <div class="ui two column centered grid">\
                                                    <div class="ui form ">\
                                                        <div class="field">\
                                                            <button class="negative ui button" id="deleteRoomButton" data-tooltip="Irrevocable removal of the room" data-position="bottom center" onclick="deleteRoom();">Delete Room</button>\
                                                        </div>\
                                                    </div>\
                                                </div>\
                                         </div>\
                                    </div>\
                            </div>\
                        </div>';

    var divToAppend2 = '<div class="bottom attached centered ui item" id="right_bar_save_cancel_buttons">\
                            <div class="ui buttons" tabindex="0"><button class="ui button" onclick="closeRightBarCallback();">Cancel</button>\
                                <div class="or"></div>\
                                <button class="ui positive button" onclick="changeSettingsValue(currentTheme);">Save</button>\
                            </div>\
                        </div>';

    rightbar.append(divToAppend);

    var getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

    $.when(getRoomInformation).then(function () {
        $("#rightsidebarblock").append(divToAppend2);
        $(".ui.styled.accordion").accordion();
    });
}

function channelDetails() {
    var rightbar = $("#Rightbar");
    rightbar.html("");

    // todo use this?
    if ($("#right_bar_save_cancel_buttons").length) {
        $("#right_bar_save_cancel_buttons").remove();
    }

    $("#rightbar_Title").html("Channel Details");
    rightbar.append('<div class="ui styled accordion">' +
                                '<div class="active content">' +
                                    '<div class="accordion">' +
                                        '<div class="title"><i class="dropdown icon"></i><i class="info icon"></i>About</div>' +
                                        '<div class="content" id="aboutInfo"></div>' +
                                        '<div class="title"><i class="dropdown icon"></i><i class="users icon"></i>Members</div>' +
                                        '<div class="content" id="MembersInRoom"></div>' +
                                    "</div>" +
                                "</div>" +
                            "</div>");

    myHub.server.getRoomUsers(currentRoomId);
    var getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

    $.when(getRoomInformation).then(function () {
        $(".ui.styled.accordion").accordion();
    });
}

myHub.client.broadcastEditedRoomTopic = function (value) {
    topicInDatabase = value;
    $("#channel_topic_text").html(value);

    if ($(".ui.sidebar.right").hasClass("visible")) {
        if ($("#rightbar_Title").text() === "Channel Settings") {
            $("#infoTopic").val(topicInDatabase);
        }
    };
};

myHub.client.broadcastEditedRoomDescription = function (value) {
    descriptionInDatabase = value;

    if ($(".ui.sidebar.right").hasClass("visible")) {
        if ($("#rightbar_Title").text() === "Channel Settings") {
            $("#infoDescription").val(descriptionInDatabase);
        } else {
            $("#channelDetailsDescription").text(descriptionInDatabase);
        }
    };
};

myHub.client.broadcastEditedRoomCustomization = function (value) {
    themeInDatabase = value.toString();
    changeTheme(themeInDatabase);

    if ($(".ui.sidebar.right").hasClass("visible")) {
        if ($("#rightbar_Title").text() === "Channel Settings") {
            $("#selecttheme").val(value);
        }
    };
};

// For active room only
myHub.client.broadcastEditedActiveRoomName = function (value) {
    roomNameInDatabase = value;

    // Room name in header
    $("#channel_title").contents().last().replaceWith(roomNameInDatabase);
    // Active room name in sidebar
    $("#channelsMenu").find("> a.item.active").contents().last().replaceWith(roomNameInDatabase);
    // Room name in #msg_input
    $("#msg_input").attr("placeholder", "Message " + roomNameInDatabase);

    if ($(".ui.sidebar.right").hasClass("visible")) {
        if ($("#rightbar_Title").text() === "Channel Settings") {
            $("#infoName").val(roomNameInDatabase);
        }
    }
};

// For all clients connected to Domain
myHub.client.broadcastDomainEditedRoomName = function (roomId, roomName) {
    var channelsMenu = $("#channelsMenu");
    // Don't search DOM if changed room is our active room
    if (channelsMenu.find("> a.item.active").data("id") !== roomId) {
        // Room name in sidebar
        channelsMenu.find("a[data-id='" + roomId + "']").contents().last().replaceWith(roomName);
    }
};

function changeSettingsValue(val) {
    var parseval = parseInt(currentTheme);
    var topicNow = $("#infoTopic").val();
    var roomNameNow = $("#infoName").val();
    var descriptionNow = $("#infoDescription").val();

    // Room name
    if (roomNameInDatabase !== roomNameNow) {
        myHub.server.editRoomName(currentRoomId, roomNameNow);
    }

    // Room topic
    if (topicInDatabase !== topicNow) {
        myHub.server.editRoomTopic(currentRoomId, topicNow);
    }

    // Room description
    if (descriptionInDatabase !== descriptionNow) {
        myHub.server.editRoomDescription(currentRoomId, descriptionNow);
    }

    // Room customization
    if (themeInDatabase !== val) {
        myHub.server.editRoomCustomization(currentRoomId, parseval);
    }

    $(".ui.sidebar.right").removeClass("visible");
    $(".ui.main.container").removeAttr("style");
}

function changeTheme(value) {
    var leftbar;

    switch (value) {
        case "0":
            {
                leftbar = "#1B1C1D";
                break;
            }
        case "1":
            {
                leftbar = "#0080ff";
                break;
            }
        case "2":
            {
                leftbar = "#33CC00";
                break;
            }
        case "3":
            {
                leftbar = "#FF9900";
                break;
            }
        case "4":
            {
                leftbar = "#D15CC1";
                break;
            }
        default:
            leftbar = "#1B1C1D";
    }

    $(".ui.sidebar.left").css("background-color", leftbar);
}

function selectTheme(sel) {
    currentTheme = sel.value;
    changeTheme(currentTheme);
}

function changeChannelSettingsPopup(tip) {
    $("#channel_settings_toggle").attr({ 
        "data-content": tip + " Channel Settings",
        "aria-label": tip + " Channel Settings"
    });
}

function changeChannelDetailsPopup(tip) {
    $("#channel_details_toggle").attr({ 
        "data-content": tip + " Channel Details",
        "aria-label": tip + " Channel Details"
    });
}

$("#channel_settings_toggle").click(function () {
    var sidebar = $(".ui.sidebar.right");

    if (sidebar.hasClass("visible")) {
        if ($("#rightbar_Title").text() === "Channel Settings") {
            changeChannelSettingsPopup("Show");
            sidebar.removeClass("visible");
            $(".ui.main.container").removeAttr("style");
        } else {
            channelSettings();
            changeChannelSettingsPopup("Hide");
            changeChannelDetailsPopup("Show");
        }
    } else {
        channelSettings();
        changeChannelSettingsPopup("Hide");
        sidebar.addClass("visible");
        $(".ui.main.container").css("cssText", "margin-left: 260px !important");
    }
});

var closeRightBarCallback = function () {
    $(".ui.sidebar.right").removeClass("visible");
    $(".ui.main.container").removeAttr("style");
    changeTheme(themeInDatabase);
};

$("#closerightbar").click(function () {
    closeRightBarCallback();
});

$(document).keydown(function (x) {
    // Escape key
    if (x.keyCode === 27) {
        if ($(".ui.sidebar.right").hasClass("visible")) {
            closeRightBarCallback();
        }

        $(".ui.basic.modal").modal("hide");
    }
});

$("#channel_details_toggle, #channel_members_toggle_count").click(function () {
    var sidebar = $(".ui.sidebar.right");

    if (sidebar.hasClass("visible")) {
        if ($("#rightbar_Title").html() === "Channel Details") {
            changeChannelDetailsPopup("Show");
            sidebar.removeClass("visible");
            $(".ui.main.container").removeAttr("style");
        } else {
            channelDetails();
            changeChannelDetailsPopup("Hide");
            changeChannelSettingsPopup("Show");
        }
    } else {
        channelDetails();
        changeChannelDetailsPopup("Hide");
        sidebar.addClass("visible");
        $(".ui.main.container").css("cssText", "margin-left: 260px !important");
    }
});

// Click on added users in input when creating new private conversation
$(document).on("click", ".private-conversation-tag", function (e) {
    var userId = $(e.currentTarget).data("id");

    var row = $('.row.private-conversation-row[data-id="' + userId + '"]');
    row.attr("data-is-selected", "false");
    row.show();

    $(e.currentTarget).remove();

    var input = $("#private_inputtext");
    var form = $("#createPrivateConversationForm");
    // Search again if we have text in #private_inputtext
    searchPrivateModal(input.val());

    // Disable submit when users input list are empty
    if (!form.find(".private-conversation-tag").length) {
        form.find(":submit").addClass("disabled").removeClass("positive");
        input.attr("placeholder", "Find or start a conversation");
    }
});

// Click on added users in input when inviting to private room
$(document).on("click", ".invite-users-tag", function (e) {
    var userId = $(e.currentTarget).data("id");

    var row = $('.row.invite-users-row[data-id="' + userId + '"]');
    row.attr("data-is-selected", "false");
    row.show();

    $(e.currentTarget).remove();

    var input = $("#invite_inputtext");
    var form = $("#inviteUsersForm");
    // Search again if we have text in #invite_inputtext
    searchInviteModal(input.val());

    // Disable submit when users input list are empty
    if (!form.find(".invite-users-tag").length) {
        form.find(":submit").addClass("disabled").removeClass("positive");
        input.attr("placeholder", "Search by name");
    }
});

myHub.client.usersInRoom = function (result) {
    $("#MembersInRoom").empty();

    $("#MembersInRoom").prev().contents().last().replaceWith(result.length + " Members");

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row MembersInRoom-row" data-id="' + value.Id + '" data-name="' + value.Name + '">';
        divToAppend += '<div class="eight wide column"><img class="gravatarMembersList" src="' + GravatarUrl + value.EmailHash + "?s=20&" + GravatarOptions + '" alt="avatar">' + value.Name;

        if (globalUserName === value.Name) {
            divToAppend += "(you)";
        }

        if (value.IsOnline) {
            divToAppend += '<i class="small circle icon green"></i>';
        } else {
            divToAppend += '<i class="small circle thin icon"></i>';
        }

        divToAppend += "</div></div>";

        $("#MembersInRoom").append(divToAppend);
    });
};

myHub.client.roomInfo = function (result, createdOn) {
    themeInDatabase = String(result.Customization);
    currentTheme = themeInDatabase;
    topicInDatabase = result.Topic;
    roomNameInDatabase = result.Name;
    descriptionInDatabase = result.Description;
    isCurrentRoomPrivateConversation = result.IsPrivateConversation;

    changeTheme(themeInDatabase);

    if ($(".ui.sidebar.right").hasClass("visible")) {
        if ($("#rightbar_Title").html() === "Channel Details") {
            var divToAppend = "<div>Created by <strong>" + result.OwnerName + "</strong> on " + createdOn + '<h5>Description</h5><span id="channelDetailsDescription">' + result.Description + "</span></div>";

            $("#aboutInfo").empty().append(divToAppend);

            // Add invite button to non public rooms
            if (!result.IsPublic && !result.IsPrivateConversation) {
                var inviteButton = '<div class="ui two column centered grid" id="inviteButtonContainer">\
                    <button class="primary ui button" id="inviteButton" onclick="inviteUsers();">Invite users</button>\
                    </div>';
                
                $(".ui.styled.accordion").append(inviteButton);
            } else {
                $("#inviteButtonContainer").remove();
            }
        } else {
            $("#selecttheme").val(themeInDatabase);
            $("#infoTopic").val(result.Topic);
            $("#infoDescription").val(result.Description);

            if (isCurrentRoomPrivateConversation) {
                $("#roomName").prev().fadeOut("fast", function() {
                    $("#roomName").removeClass("active");
                    $(this).removeClass("active");
                });
            } else {
                $("#infoName").val(roomNameInDatabase);

                $("#roomName").prev().fadeIn("fast", function() {
                    $(this).show();
                });
            }
        }
    }

    $("#channel_topic_text").html(topicInDatabase);

    var inputPlaceholder;

    if (isCurrentRoomPrivateConversation) {
        var nameArray = roomNameInDatabase.split(", ");
        inputPlaceholder = roomNameInDatabase;

        // Remove current userName
        $(nameArray).each(function(index, element) {
            if (element === globalUserName) {
                nameArray.splice(index, 1);
                inputPlaceholder = nameArray;

                return false;
            }
        });
    } else {
        inputPlaceholder = roomNameInDatabase;
    }

    $("#msg_input").attr("placeholder", "Message " + inputPlaceholder);
};