$(".ui.sidebar").sidebar("setting", {
    transition: "overlay"
});

$(document).on("reloadPopups", function () {
    $(".ts_tip").popup({
    variation: "inverted"
    });

    $(".item > .btn_unstyle.right").popup({
        variation: "inverted"
    });
});

function loadingStart() {
    $(".ui.main.container").css("display", "none");
    $(".ui.dimmer").addClass("active");
}

function loadingStop() {
    $(".ui.main.container").css("display", "block");
    $(".ui.dimmer").removeClass("active");
    console.log("loader stops");
    addActionsToMessages();
}

function addActionsToMessages() {
    $(".ts-message").not(":has(> .action_hover_container)").each(function() {
        $(this).prepend('<div class="action_hover_container stretch_btn_heights narrow_buttons" data-js="action_hover_container" data-show_rxn_action="true"> \
            <button type="button" data-action="edit" class="btn_unstyle btn_msg_action ts_tip" data-content="Edit message" data-position="top center"> \
            <i class="edit icon"></i> \
            </button> \
            <button type="button" data-action="delete" class="btn_unstyle btn_msg_action ts_tip danger" data-content="Delete message" data-position="top center"> \
            <i class="delete icon"></i> \
            </button> \
            </div>');
    });
}

/*
 *  Resizing
 */

$(window).resize(function () {
    // #chat_messages
    var chat_additional_height = 117;

    if ($(".ui.message").length) {
        if (!$(".ui.message").hasClass("hidden")) {
            chat_additional_height += 51;
        }
    }

    $("#chat_messages").height($(window).height() - chat_additional_height);

    // .sidebar_channels_messages
    var sidebar_additional_height = 165;

    $(".sidebar_channels_messages").height($(window).height() - sidebar_additional_height);
});

$(window).trigger("resize");

/*
 *  SignalR
 */

currentRoomId = 0;

$.connection.hub.url = "http://localhost:52284/signalr";
var myHub = $.connection.channelHub;

// Change room
$(".menu").on("click", ".menu > a.item", function (e) {
    if (currentRoomId != $(e.target).data("id")) {
        // Leave current room
        loadingStart();
        myHub.server.leaveRoom(currentRoomId);
        $(".menu > a.item.active").removeClass("active");

        currentRoomId = $(e.target).data("id");

        $.when(myHub.server.joinRoom(currentRoomId)).then(function () {
            $(e.target).addClass("active");
            $("#channel_title").html($(e.target).html());
            loadingStop();
            console.log("Current room ID: " + currentRoomId);
        });
    }
});

// This deletes the message
$("#chat_messages").on("click", ".ts-message .btn_msg_action[data-action='delete']", function (e) {
    $(".ui.delete.modal")
        .modal({
            onApprove: function () {
                // Deleting message
                var messageId = $(e.target).closest(".ts-message").data("id");
                myHub.server.deleteMessage(currentRoomId, messageId);
            }
        })
        .modal("show");
});

myHub.client.broadcastMessage = function (userName, message, timestamp) {
    $("#chat_messages").append('<div class="ts-message" data-id="-----TODO-----"><div class="message_gutter"><div class="message_icon"><a href="/team/malgosia" target="/team/malgosia" class="member_image" data-member-id="U42KXAW07" style="background-image: url(\'../images/troll.png\')" aria-hidden="true" tabindex="-1"> </a></div></div><div class="message_content"><div class="message_content_header"><a href="#" class="message_sender">' + userName + '</a><a href="#" class="timestamp">' + timestamp + '</a></div><span class="message_body">' + message + '</span></div></div>');

    // Scroll #chat_messages
    $("#chat_messages").clearQueue();
    $("#chat_messages").animate({ scrollTop: $("#chat_messages")[0].scrollHeight }, "slow");
    addActionsToMessages();
    $(document).trigger("reloadPopups");
}

myHub.client.deleteMessage = function (messageId) {
    var message = $(".ts-message[data-id='" + messageId + "']");

    console.log("Klient usuwa wiadomosc o ID: " + message.data("id"));

    message.hide("slow", function () {
        message.remove();
    });
}

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

        divToAppend += '"data-id="' + value.Id + '" > ';

        if (value.IsPublic) {
            divToAppend += '<i class="icon left">#</i>';
        } else {
            divToAppend += '<i class="lock icon left"></i>'
        }

        divToAppend += value.Name + "</a>";
        $("#channelsMenu").append(divToAppend);
    });
}

myHub.client.loadPrivateConversations = function (result) {
    $.each(result,
        function (index, value) {
            var divToAppend = '<a class="item';

            if (setActive) {
                divToAppend += " active";
            }
            divToAppend += '"data-id="' + value.Id + '" > ';
            divToAppend += '<i class="icon left">#</i>';
            divToAppend += value.Name + "</a>";

            $("#privateConversationsMenu").append(divToAppend);
        });
}

myHub.client.channelAddedAction = function (channelName, roomId, isPublic) {
    var divToAppend = '<a class="item" data-id="' + roomId + '">';

    if (isPublic) {
        divToAppend += '<i class="icon left">#</i>';
    } else {
        divToAppend += '<i class="lock icon left"></i>'
    }

    divToAppend += channelName + "</a>";
    $("#channelsMenu").append(divToAppend);
    updateChannelsCount(1);
    loadingStop();
}

myHub.client.privateConversationAddedAction = function (channelName, roomId) {
    var divToAppend = '<a class="item" data-id="' + roomId + '">';
    divToAppend += '<i class="icon left">#</i>';
    divToAppend += channelName + "</a>";
    $("#privateConversationsMenu").append(divToAppend);

    loadingStop();
}

// Start the connection
$.connection.hub.start()
    .done(function () {
        console.log("Connected to Hub. Getting rooms");
        // Getting rooms

        getRooms = myHub.server.getRooms();
        //  getPrivateConversations = myHub.Server.getPrivateConversations();

        $.when(getRooms).then(function () {
            // Joining to room
            $.when(myHub.server.joinRoom(currentRoomId)).then(function () {
                console.log("Connected to room :)");
                loadingStop();

                $("#msg_form").keypress(function (e) {
                    if (e.which == 13) {
                        if (!e.shiftKey) {
                            if (message = $("#msg_input").val().trim()) {
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
    }
    else if (change.newState === $.signalR.connectionState.connected) {
        console.log("The server is online");
    }
});

$.connection.hub.reconnected(function () {
    console.log("Reconnected");
    loadingStop();
});

$("#createNewChannel").click(function () {
    $("#createChanelForm")[0].reset();

    var item = $("input[name*='IsPublic']")[1];
    item.value = false;

    thisModal = $(".ui.basic.create-room.modal");

    $(thisModal)
        .modal({
            closable: false,
            onDeny: function () {
                $(thisModal).parent().css("background-color", "");
            },
            onApprove: function () {
                $(thisModal).parent().css("background-color", "");
            }
        })
        .modal("show");

    $(thisModal).parent().css("background-color", "#fff");
});

$("#createNewPrivateConversation").click(function () {
    $("#createPrivateConversationForm")[0].reset();

    thisModal = $(".ui.basic.create-private-conversation.modal");

    $(thisModal)
        .modal({
            closable: false,
            onDeny: function () {
                $(thisModal).parent().css("background-color", "");
            },
            onApprove: function () {
                $(thisModal).parent().css("background-color", "");
            }
        })
        .modal("show");

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

function updateChannelsCount(diff) {
    var number = $("#channelsCount").text().match(/\d+/);
    number = parseInt(number) + diff;
    $("#channelsCount").text("CHANNELS (" + number + ")");
}

function serializeForm(_form) {
    data = $(_form).serializeArray();
    var obj = {};

    $.each(data,
        function (key, value) {
            obj[value.name] = value.value;
        });

    return obj;
}