$(".ui.sidebar.left").sidebar();
$(".ui.sidebar.right").sidebar();

$("#directMessagesTitle, #channelsCount, #closerightbar").popup({
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
    stripTrailingSlash: false
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
var loadedMessagesIteration = 1;

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

    if ($(".ui.message").length) {
        if (!$(".ui.message").hasClass("hidden")) {
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
    // TODO
    if (loading) {
        return;
    }

    printLog("Click na licznik kanałów");

    $("#browseChannelsForm")[0].reset();

    var domainRooms = myHub.server.getDomainPublicRooms();

    var thisModal = $(".ui.basic.browse-channels.modal");

    $.when(domainRooms).then(function () {
        $(thisModal).modal({
            closable: false,
            onShow: function () {
                $(".ui.dimmer.modals").css("background-color", "#fff");
            },
            onHide: function () {
                $(".ui.dimmer.modals").css("background-color", "");
            },
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
            newRoom.addClass("active");
            $("#channel_title").html(newRoom.html());

            var getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

            $.when(getRoomInformation).then(function () {
                loadingStop();
                loadedMessagesIteration = 1;
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
    var divToAppend = '<div class="ts-message message highlight" id="message_edit_container">\
            <div class="message_gutter">\
                <div class="message_icon">\
                    '+ messageIcon + '\
                </div>\
            </div>\
            <form id="message_edit_form" data-id="'+ id + '">\
                <button class="btn_unstyle emo_menu" aria-label="Emoji menu" type="button">\
                    <i class="meh icon"></i>\
                </button>\
                <div id="msg_text" class="message_input ql-container focus">\
                    <div class="ql-editor" role="textbox" tabindex="0" aria-multiline="true" aria-haspopup="true" spellcheck="true" contenteditable="true"><p>'+ oldMessageText + '</p></div>\
                    <div class="ql-clipboard" tabindex="-1" aria-hidden="true" role="presentation" spellcheck="true" contenteditable="true"></div>\
                </div>\
                <a id="cancel_edit" role="button" class="ui button mini">Cancel</a>\
                <a id="commit_edit" role="button" class="ui button mini positive"><i class="icon edit"></i>Save Changes</a>\
                <span id="message_editing_info" class="mini" style="display: none;">Finish editing this message first! Or press <strong>escape</strong> if youve changed your mind.</span>\
            </form>\
        </div>';

    return divToAppend;
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
    $(".ql-editor").focus();

    var range = document.createRange();
    range.selectNodeContents($(".ql-editor").get(0));
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
    // FIXME: move to message_edit_form?
    $("#message_edit_container").keydown(function (x) {
        printLog("keydown dla #message_edit_container");
        if (x.keyCode === 27) {
            printLog("Escape key dla #message_edit_container");
            document.removeEventListener("click", clickOutsideEditContainer, true);
            $(document).find("#message_edit_container").prev().show();
            $(document).find("#message_edit_container").remove();
        }
    });

    $("#message_edit_form").keypress(function (x) {
        printLog("keypress dla #message_edit_form");
        // Enter key
        if (x.which === 13) {
            if (!x.shiftKey) {
                printLog("Enter key dla #message_edit_form");
                if (oldMessageText !== $(".ql-editor").text().trim()) {
                    printLog("Wiadomosci sie roznia");
                    var editedMessage;

                    if (editedMessage === $(".ql-editor").text().trim()) {
                        printLog("Edytuję: " + editedMessage + " do pokoju " + currentRoomId);
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
        if (oldMessageText !== $(".ql-editor").text().trim()) {
            var editedMessage = $(".ql-editor").text().trim();

            if (editedMessage) {
                printLog("Edytuję: " + editedMessage + " do pokoju " + currentRoomId);
                myHub.server.editMessage(currentRoomId, messageId, editedMessage);
            }
        }

        document.removeEventListener("click", clickOutsideEditContainer, true);
        message.show();
        $(x.target).closest("#message_edit_container").remove();
    });

    // TODO: Need more testing?
    function clickOutsideEditContainer(event) {
        printLog("Click listener: clickOutsideEditContainer()");
        // Do only if #message_edit_container exists
        if ($(document).find("#message_edit_container").length) {
            // Check for x.target and parents
            if (!$(event.target).parents().addBack().is("#message_edit_container")) {
                printLog("Kliknięto poza. Usuwam #MEC");
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
    var input = $("#inputtext");
    $(item).hide();

    $(input).attr("placeholder", "");

    $("#createPrivateConversationForm").find(":submit").removeClass("disabled").addClass("positive");

    $(input).before('<span class="private-conversation-tag" data-id="' + id + '">' + append + ' <i class="remove icon"></i></span>');
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
                loadedMessagesIteration++;
            });
        }
    }
});

function getMessageHtml(userName, userId, messageId, messageText, timestamp, emailHash) {
    var messageHtml = '<div class="ts-message" data-id="' + messageId + '"><div class="message_gutter"><div class="message_icon"><a href="/team/malgosia" target="/team/malgosia" class="member_image" data-member-id="' + userId + '" style="background-image: url(' + GravatarUrl + emailHash + '?s=36&' + GravatarOptions + ')" aria-hidden="true" tabindex="-1"> </a></div></div><div class="message_content"><div class="message_content_header"><a href="#" class="message_sender">' + userName + '</a><a href="#" class="timestamp">' + timestamp + '</a></div><span class="message_body">' + Autolinker.link(parseEmoticons(messageText));

    return messageHtml;
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

    $("#chat_messages").append(messageHtml);

    // Scroll #chat_messages
    $("#chat_messages").clearQueue();
    $("#chat_messages").animate({ scrollTop: $("#chat_messages")[0].scrollHeight }, "slow");
    addActionsToMessages();
    $(document).trigger("reloadPopups");
};

myHub.client.deleteMessage = function (messageId) {
    var message = $(".ts-message[data-id='" + messageId + "']");

    printLog("Klient usuwa wiadomosc o ID: " + message.data("id"));

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

myHub.client.loadDomainPublicRooms = function (result) {
    $("#browseChannelsList").empty();

    console.log(result);

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row browse-room-row" data-id="' + value.Id + '" data-name="' + value.Name + '">';
        divToAppend += '<div class="eight wide column">';

        if (value.IsPublic) {
            divToAppend += "# ";
        } else {
            divToAppend += '<i class="lock icon"></i> ';
        }

        divToAppend += "<strong>" + value.Name + "</strong>";

        // Mark joined channels
        if ($("#channelsMenu").find("a:contains(" + value.Name + ")").length) {
            divToAppend += '  <small>JOINED</small>';
        }

        divToAppend += "<br><i>Created by <strong>" + value.Owner.Name + "</strong> on </i>";

        divToAppend += "<br>" + value.Description + "</div>";

        divToAppend += '<div class="four wide column"></div>';
        divToAppend += '<div class="four wide column mycheckmark"><i class="level down rotated big icon"></i></div>';
        divToAppend += "</div>";

        $("#browseChannelsList").append(divToAppend);
    });
};

myHub.client.loadPrivateConversations = function (result) {
    $.each(result, function (index, value) {
        var divToAppend = '<a class="item" data-id="' + value.Id + '"><i class="icon left">#</i>' + value.Name + "</a>";

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
    $(".ui.main.container").css("display", "block");
    $("#chat_messages").clearQueue();
    $("#chat_messages").animate({ scrollTop: $("#chat_messages")[0].scrollHeight }, "slow");
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

myHub.client.channelAddedAction = function (channelName, roomId, isPublic) {
    var divToAppend = '<a class="item" data-id="' + roomId + '" data-ispublic="' + isPublic + '">';

    if (isPublic) {
        divToAppend += '<i class="icon left">#</i>';
    } else {
        divToAppend += '<i class="lock icon left"></i>';
    }

    divToAppend += channelName + "</a>";
    $("#channelsMenu").append(divToAppend);
    updateChannelsCount(1);
    changeRoom($("#channelsMenu").children().last());
};

myHub.client.privateConversationAddedAction = function (value) {
    var divToAppend = '<a class="item" data-id="' + value.Id + '"><i class="icon left">#</i>' + value.Name + "</a>";

    $("#privateConversationsMenu").append(divToAppend);
    changeRoom($("#privateConversationsMenu").children().last());
};

myHub.client.privateConversationsUsersLoadedAction = function (result) {
    $("#privateConversationsUserList").empty();

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row private-conversation-row" data-id="' + value.Id + '" data-name="' + value.Name + '">';
        divToAppend += '<div class="eight wide column"><strong>' + value.Email + "</strong> ";

        if (value.IsOnline) {
            divToAppend += '<i class="circle icon green"></i>';
        } else {
            divToAppend += '<i class="circle thin icon"></i>';
        }

        divToAppend += value.Name + "</div>";
        divToAppend += '<div class="four wide column"></div>';
        divToAppend += '<div class="four wide column mycheckmark"><i class="checkmark icon"></i></div>';
        divToAppend += "</div>";

        $("#privateConversationsUserList").append(divToAppend);
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
        printLog("Connected to Hub. Getting rooms");
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
                                    printLog("Wysyłam: " + message + " do pokoju " + currentRoomId);
                                    myHub.server.sendMessage(currentRoomId, message);
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

$("#createNewChannel").click(function () {
    if (loading) {
        return;
    }

    $("#createChanelForm")[0].reset();

    var item = $("input[name*='IsPublic']")[1];
    item.value = false;

    var thisModal = $(".ui.basic.create-room.modal");

    $(thisModal).modal({
        closable: false,
        onShow: function () {
            $(".ui.dimmer.modals").css("background-color", "#fff");
        },
        onHide: function () {
            $(".ui.dimmer.modals").css("background-color", "");
        },
    }).modal("show");
});

$("#createNewPrivateConversation, #directMessagesTitle").click(function () {
    if (loading) {
        return;
    }

    $("#createPrivateConversationForm")[0].reset();
    // Remove all added tags
    $("#createPrivateConversationForm").find(".private-conversation-tag").remove();

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
            },
        }).modal("show");
    });
});

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
    e.preventDefault();
    var list = [];
    var users = [];

    $("#createPrivateConversationForm").find(".private-conversation-tag").each(function (index, element) {
        list.push($(element).data("id"));
        users.push($.trim($(element).text()));
        printLog($.trim($(element).text()));
    });

    printLog(users[0]);
    // TODO: search multiple users in one private conversation

    if (list.length > 0) {
        var searchedPriv = $("#privateConversationsMenu a:contains('" + users[0] + "')");
        printLog(searchedPriv);

        if (searchedPriv.length) {
            changeRoom(searchedPriv);
        } else {
            loadingStart();
            myHub.server.createNewPrivateConversation(list);
        }

        $(".ui.basic.create-private-conversation.modal").modal("hide");
    }
});

function updateChannelsCount(diff) {
    var number = $("#channelsCount").text().match(/\d+/);
    number = parseInt(number) + diff;
    $("#channelsCount").text("CHANNELS (" + number + ")");
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
    $("#Rightbar").html("");

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
                                        <div class="content" id="Description">\
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
                                                            <button class="negative ui button" id="deleteRoomButton" data-inverted="" data-tooltip="Irrevocable removal of the room" data-position="bottom center" onclick="deleteRoom();">Delete Room</button>\
                                                        </div>\
                                                    </div>\
                                                </div>\
                                         </div>\
                                    </div>\
                            </div>\
                        </div>';

    $("#deleteRoomButton").popup({
        variation: "inverted"
    });

    var divToAppend2 = '<div class="bottom attached centered ui item" id="right_bar_save_cancel_buttons">\
                            <div class="ui buttons" tabindex="0"><button class="ui button" onclick="closeRightBarCallback();">Cancel</button>\
                                <div class="or"></div>\
                                <button class="ui positive button" onclick="changeSettingsValue(currentTheme);">Save</button>\
                            </div>\
                        </div>';

    $("#Rightbar").append(divToAppend);

    var getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

    $.when(getRoomInformation).then(function () {
        $("#rightsidebarblock").append(divToAppend2);
        $(".ui.styled.accordion").accordion();
    });
}

function channelDetails() {
    $("#Rightbar").html("");

    if ($("#right_bar_save_cancel_buttons").length) {
        $("#right_bar_save_cancel_buttons").remove();
    }

    $("#rightbar_Title").html("Channel Details");
    $("#Rightbar").append('<div class="ui styled accordion">' +
                                '<div class="active content">' +
                                    '<div class="accordion">' +
                                        '<div class="title"><i class="dropdown icon"></i><i class="info icon"></i>About</div>' +
                                        '<div class="content" id="aboutInfo"></div>' +
                                        '<div class="title"><i class="dropdown icon"></i><i class="users icon"></i>Members</div>' +
                                        '<div class="content" id="MembersInRoom"></div>' +
                                    "</div>" +
                                "</div>" +
                            "</div>");

    $(".ui.styled.accordion").accordion();
    myHub.server.getRoomUsers(currentRoomId);
    myHub.server.getRoomInformation(currentRoomId);
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
    $("#channelsMenu > a.item.active").contents().last().replaceWith(roomNameInDatabase);
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
    // Don't search DOM if changed room is our active room
    if ($("#channelsMenu > a.item.active").data("id") !== roomId) {
        // Room name in sidebar
        $("[data-id='" + roomId + "']").contents().last().replaceWith(roomName);
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

// Escape key
$(document).keydown(function (x) {
    if (x.keyCode === 27) {
        if ($(".ui.sidebar.right").hasClass("visible")) {
            printLog("Escape dla prawego sidebaru");
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

    $('.row.private-conversation-row[data-id="' + userId + '"]').show();
    $(e.currentTarget).remove();

    // Disable submit when users input list are empty
    if (!$("#createPrivateConversationForm").find(".private-conversation-tag").length) {
        $("#createPrivateConversationForm").find(":submit").addClass("disabled").removeClass("positive");
        $("#inputtext").attr("placeholder", "Find or start a conversation");
    }
});

myHub.client.usersInRoom = function (result) {
    $("#MembersInRoom").empty();

    $("#MembersInRoom").prev().contents().last().replaceWith(result.length + " Members");

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row MembersInRoom-row" data-id="' + value.Id + '" data-name="' + value.Name + '">';
        divToAppend += '<div class="eight wide column"><img class="gravatarMembersList" src="' + GravatarUrl + value.EmailHash + "?s=20&" + GravatarOptions + '">' + value.Name;

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

    changeTheme(themeInDatabase);

    if ($(".ui.sidebar.right").hasClass("visible")) {
        if ($("#rightbar_Title").html() === "Channel Details") {
            var divToAppend = "<div>Created by <strong>" + result.OwnerName + "</strong> on " + createdOn + '<h5>Description</h5><span id="channelDetailsDescription">' + result.Description + "</span></div>";

            $("#aboutInfo").empty();
            $("#aboutInfo").append(divToAppend);
        } else {
            $("#selecttheme").val(themeInDatabase);
            $("#infoTopic").val(result.Topic);
            $("#infoName").val(roomNameInDatabase);
            $("#infoDescription").val(result.Description);
        }
    }

    $("#channel_topic_text").html(topicInDatabase);
    $("#msg_input").attr("placeholder", "Message " + roomNameInDatabase);
};