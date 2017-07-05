$(".ui.sidebar.left").sidebar("setting", {
    transition: "overlay"
});

$(".ui.sidebar.right").first().sidebar();

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

var currentTheme;
var descriptionInDatabase;
var roomNameInDatabase;

function loadingStart() {
    $(".ui.main.container").css("display", "none");
    $(".ui.dimmer").addClass("active");
}

function loadingStop() {
    $(".ui.main.container").css("display", "block");
    $(".ui.dimmer").removeClass("active");
    addActionsToMessages();
    console.log("Loader stops");
}

function addActionsToMessages() {
    $(".ts-message").not(":has(> .action_hover_container), #message_edit_container").each(function () {
        // This is our message?
        if ($(this).find(".message_sender").text() !== globalUserName || $(this).find(".member_image").data("member-id") !== globalUserId) {
            return true;
        }

        $(this).prepend('<div class="action_hover_container stretch_btn_heights narrow_buttons" data-js="action_hover_container" data-show_rxn_action="true">\
            <button type="button" data-action="edit" class="btn_unstyle btn_msg_action ts_tip" data-content="Edit message" data-position="top center">\
            <i class="edit icon"></i>\
            </button>\
            <button type="button" data-action="delete" class="btn_unstyle btn_msg_action ts_tip danger" data-content="Delete message" data-position="top center">\
            <i class="delete icon"></i>\
            </button>\
            </div>');
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

currentRoomId = 0;

$.connection.hub.url = "http://localhost:52284/signalr";
var myHub = $.connection.channelHub;

$("#channelsCount").click(function () {
    console.log("Click na licznik kanałów");
    myHub.server.getDomainPublicRooms();
});

// Change room
$(".menu").on("click", ".menu > a.item", function (e) {
    if (currentRoomId !== $(e.target).data("id")) {
        // Leave current room
        loadingStart();
        // Clear messages
        $("#chat_messages").empty();
        myHub.server.leaveRoom(currentRoomId);
        $(".menu > a.item.active").removeClass("active");

        $.when(myHub.server.joinRoom($(e.target).data("id"))).then(function () {
            currentRoomId = $(e.target).data("id");
            $(e.target).addClass("active");
            $("#channel_title").html($(e.target).html());

            getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

            $.when(getRoomInformation).then(function () {
                loadingStop();
            });
        });
    }
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
    //range.setStart($(".ql-editor").get(0).childNodes[0], 0);
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
                if (oldMessageText !== $(".ql-editor").text().trim()) {
                    var editedMessage;

                    if (editedMessage === $(".ql-editor").text().trim()) {
                        console.log("Edytuję: " + editedMessage + " do pokoju " + currentRoomId);
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
                console.log("Edytuję: " + editedMessage + " do pokoju " + currentRoomId);
                myHub.server.editMessage(currentRoomId, messageId, editedMessage);
            }
        }

        document.removeEventListener("click", clickOutsideEditContainer, true);
        message.show();
        $(x.target).closest("#message_edit_container").remove();
    });

    // TODO: Need more testing?
    function clickOutsideEditContainer(event) {
        console.log("Click listener");
        // Do only if #message_edit_container exists
        if ($(document).find("#message_edit_container").length) {
            // Check for x.target and parents
            if (!$(event.target).parents().addBack().is("#message_edit_container")) {
                console.log("Kliknięto poza. Usuwam #MEC");
                document.removeEventListener("click", clickOutsideEditContainer, true);
                $(document).find("#message_edit_container").prev().show();
                $(document).find("#message_edit_container").remove();
            }
        }
    }
});

$(".grid").on("click", ".private-conversation-row", function (e) {
    var item;

    if ($(e.target).hasClass(".private-conversation-row")) {
        item = e.target;
    } else {
        item = $(e.target).closest(".private-conversation-row");
    }

    var append = $(item).data("name");
    var id = $(item).data("id");
    //var input = $("#createPrivateConversationForm :input[name='Name']");
    var input = $("#inputtext");
    input.before('<span class="private-conversation-tag" data-id="' + id + '">' + append + "</span>");
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
    messageBodies.first().html(Autolinker.link(messageText));

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

function getMessageHtml(userName, userId, messageId, messageText, timestamp) {
    var messageHtml = '<div class="ts-message" data-id="' + messageId + '"><div class="message_gutter"><div class="message_icon"><a href="/team/malgosia" target="/team/malgosia" class="member_image" data-member-id="' + userId + '" style="background-image: url(\'../images/troll.png\')" aria-hidden="true" tabindex="-1"> </a></div></div><div class="message_content"><div class="message_content_header"><a href="#" class="message_sender">' + userName + '</a><a href="#" class="timestamp">' + timestamp + '</a></div><span class="message_body">' + Autolinker.link(messageText);

    return messageHtml;
}

myHub.client.broadcastMessage = function (userName, userId, messageId, messageText, timestamp) {
    var messageHtml = getMessageHtml(userName, userId, messageId, messageText, timestamp);

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

    console.log("Klient usuwa wiadomosc o ID: " + message.data("id"));

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
    var setActive = true;

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

    var rooms = {};

    $("#channelsMenu > .item").filter(function () {
        var id = $(this).data("id");

        if (rooms[id]) {
            // Duplikat pokoju
            $(this).remove();
            return false;
        } else {
            // Pierwszy pokój z tym ID
            rooms[id] = true;
            return true;
        }
    });

    $("#channelsCount").text("CHANNELS (" + $("#channelsMenu > .item").length + ")");
};

myHub.client.loadPrivateConversations = function (result) {
    $.each(result, function (index, value) {
        var divToAppend = '<a class="item" data-id="' + value.Id + '"><i class="icon left">#</i>' + value.Name + "</a>";

        $("#privateConversationsMenu").append(divToAppend);
    });
};

myHub.client.parseLastMessages = function (result) {
    $.each(result, function (index, value) {
        var messageHtml = getMessageHtml(value.UserName, value.UserId, value.Id, value.Text, value.CreatedOn);

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
    loadingStop();
};

myHub.client.privateConversationAddedAction = function (value) {
    var divToAppend = '<a class="item" data-id="' + value.Id + '"><i class="icon left">#</i>' + value.Name + "</a>";

    $("#privateConversationsMenu").append(divToAppend);
    loadingStop();
};

myHub.client.privateConversationsUsersLoadedAction = function (result) {
    $("#privateConversationsUserList").empty();
    $.each(result, function (index, value) {
        var divToAppend = '<div class="row private-conversation-row" data-id="' + value.Id + '" data-name="' + value.Name + '">';
        divToAppend += '<div class="eight wide column"><b>' + value.UserName + "</b> ";

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
};

myHub.client.updateRoomUsersCount = function (roomUsersCount) {
    $("#channel_members_toggle_count_count").text(roomUsersCount);
    console.log("Updated roomUsersCount");
};

/*
 * Start the connection
 */
$.connection.hub.start()
    .done(function () {
        console.log("Connected to Hub. Getting rooms");
        // Getting rooms
        getRooms = myHub.server.getRooms();
        getPrivateConversations = myHub.server.getPrivateConversations();

        $.when(getRooms).then(function () {
            // Joining to room
            $.when(myHub.server.joinRoom(currentRoomId)).then(function () {
                console.log("Connected to room");
                var firstChannelTitle = $(".menu > a.item.active");
                $("#channel_title").html($(firstChannelTitle).html());
                
                getRoomInformation = myHub.server.getRoomInformation(currentRoomId);

                $.when(getRoomInformation).then(function () {
                    loadingStop();

                    $("#msg_form").keypress(function (e) {
                        if (e.which === 13) {
                            if (!e.shiftKey) {
                                var message = $("#msg_input").val().trim();

                                if (message) {
                                    console.log("Wysyłam: " + message + " do pokoju " + currentRoomId);
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
        console.log("Not connected! " + a);
    });

$.connection.hub.error = function (error) {
    console.warn(error);
};

$.connection.hub.stateChanged(function (change) {
    if (change.newState === $.signalR.connectionState.reconnecting) {
        console.log("Re-connecting");
        loadingStart();
    } else if (change.newState === $.signalR.connectionState.connected) {
        console.log("The server is online");
    }
});

$.connection.hub.reconnected(function () {
    console.log("Reconnected");
    loadingStop();
});

window.onbeforeunload = function () {
    $.connection.hub.stop();
};

$("#createNewChannel").click(function () {
    $("#createChanelForm")[0].reset();

    var item = $("input[name*='IsPublic']")[1];
    item.value = false;

    thisModal = $(".ui.basic.create-room.modal");

    $(thisModal).modal({
        closable: false,
        onDeny: function () {
            $(thisModal).parent().css("background-color", "");
        },
        onApprove: function () {
            $(thisModal).parent().css("background-color", "");
        }
    }).modal("show");

    $(thisModal).parent().css("background-color", "#fff");
});

$("#createNewPrivateConversation").click(function () {
    $("#createPrivateConversationForm")[0].reset();

    // Remove all added tags
    $("#createPrivateConversationForm").find(".private-conversation-tag").each(function (index, element) {
        console.log(element);
        $(element).remove();
    });

    myHub.server.getUsersFromDomain();

    var thisModal = $(".ui.basic.create-private-conversation.modal");

    $(thisModal).modal({
        closable: false,
        onDeny: function () {
            $(thisModal).parent().css("background-color", "");
        },
        onApprove: function () {
            $(thisModal).parent().css("background-color", "");
        }
    }).modal("show");

    $(thisModal).parent().css("background-color", "#fff");
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
    data = serializeForm($(this));
    myHub.server.createNewChannel(data);
    $(".ui.basic.create-room.modal").modal("hide");
});

$("#createPrivateConversationForm").submit(function (e) {
    loadingStart();
    e.preventDefault();
    var list = [];

    $("#createPrivateConversationForm").find(".private-conversation-tag").each(function (index, element) {
        var item = $(element);
        var id = $(item).data("id");
        list.push(id);
    });

    myHub.server.createNewPrivateConversation(list);
    $(".ui.basic.create-private-conversation.modal").modal("hide");
});

function updateChannelsCount(diff) {
    var number = $("#channelsCount").text().match(/\d+/);
    number = parseInt(number) + diff;
    $("#channelsCount").text("CHANNELS (" + number + ")");
}

function serializeForm(form) {
    data = $(form).serializeArray();
    var obj = {};

    $.each(data, function (key, value) {
        obj[value.name] = value.value;
    });

    return obj;
}

function customization() {
    myHub.server.getRoomInformation(currentRoomId);

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

    var divToAppend2 = '<div class="bottom attached centered ui item">\
                            <div class="ui buttons" tabindex="0"><button class="ui button" onclick="closeRightBarCallback();">Cancel</button>\
                                <div class="or"></div>\
                                <button class="ui positive button" onclick="changeSettingsValue(currentTheme);">Save</button>\
                            </div>\
                        </div>';

    $("#Rrightbar").append(divToAppend);
    $("#rightsidebarblock").append(divToAppend2);
    $(".ui.styled.accordion").accordion();
}

function membersInRoom() {
    $("#Rrightbar").html("");

    $("#rightbar_Title").html("Channel Details");
    $("#Rrightbar").append('<div class="ui styled accordion">' +
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
}

myHub.client.broadcastEditedRoomDescription = function (value) {
    descriptionInDatabase = value;
    $("#channel_topic_text").html(value);
};

myHub.client.broadcastEditedRoomCustomization = function (value) {
    themeInDatabase = value.toString();
    changeTheme(themeInDatabase);
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
    var descriptionNow = $("#infoDescription").val();
    var roomNameNow = $("#infoName").val();

    // Room name
    if (roomNameInDatabase !== roomNameNow) {
        myHub.server.editRoomName(currentRoomId, roomNameNow);
    }

    // Room topic
    if (descriptionInDatabase !== descriptionNow) {
        myHub.server.editRoomDescription(currentRoomId, descriptionNow);
    }

    // Room customization
    if (themeInDatabase !== val) {
        myHub.server.editRoomCustomization(currentRoomId, parseval);
    }

    $(".ui.sidebar.right").removeClass("visible");
}

function changeTheme(value) {
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

$("#channel_actions_toggle").click(function () {
    var sidebar = $(".ui.sidebar.right");

    if (sidebar.hasClass("visible")) {
        if ($("#rightbar_Title").text() === "Channel Settings") {
            sidebar.removeClass("visible");
        } else {
            $("#Rrightbar").html("");
            sidebar.addClass("visible");
            $("#rightbar_Title").html("Channel Settings");
            customization();
        }
    } else {
        $("#Rrightbar").html("");
        sidebar.addClass("visible");
        $("#rightbar_Title").html("Channel Settings");
        customization();
    }
});

var closeRightBarCallback = function () {
    $(".ui.sidebar.right").removeClass("visible");
    changeTheme(themeInDatabase);
};

$("#closerightbar").click(function () {
    closeRightBarCallback();
});

// Escape key
$(document).keydown(function (x) {
    if (x.keyCode === 27 && $(".ui.sidebar.right").hasClass("visible")) {
        console.log("Escape dla prawego sidebaru");
        closeRightBarCallback();
    }
});

$("#details_toggle").click(function () {
    var sidebar = $(".ui.sidebar.right");
    myHub.server.getRoomInformation(currentRoomId);

    if (sidebar.hasClass("visible")) {
        if ($("#rightbar_Title").html() === "Channel Details") {
            sidebar.removeClass("visible");
        } else {
            sidebar.addClass("visible");
            membersInRoom();
        }
    } else {
        membersInRoom();
        sidebar.addClass("visible");
    }
});

$(document).on("click", ".private-conversation-tag", function () {
    $(this).remove();
});

myHub.client.usersInRoom = function (result) {
    $("#MembersInRoom").empty();

    $("#MembersInRoom").prev().contents().last().replaceWith(result.length + " Members");

    $.each(result, function (index, value) {
        var divToAppend = '<div class="row MembersInRoom-row" data-id="' + value.Id + '" data-name="' + value.Name + '">';
        divToAppend += '<div class="eight wide column"><i class="user icon"></i>' + value.Name + "</div>";
        divToAppend += "</div>";

        $("#MembersInRoom").append(divToAppend);
    });
};

myHub.client.roomInfo = function (result, resultTime) {
    console.log("Pobranie informacji z funkcji roomInfo()");
    $("#aboutInfo").empty();
    var divToAppend = "<div>Created by " + result.OwnerName + " on " + resultTime + "</div>";
    $("#aboutInfo").append(divToAppend);
    $("#infoDescription").val(result.Description);
    themeInDatabase = String(result.Customization);
    currentTheme = themeInDatabase;
    descriptionInDatabase = result.Description;
    roomNameInDatabase = result.Name;
    $("#channel_topic_text").html(descriptionInDatabase);
    $("#infoName").val(roomNameInDatabase);
    $("#msg_input").attr("placeholder", "Message " + roomNameInDatabase);

    changeTheme(themeInDatabase);

    // Change selected theme in drop-down list
    if ($(".ui.sidebar.right").hasClass("visible")) {
        $("#selecttheme").val(themeInDatabase);
    }
};