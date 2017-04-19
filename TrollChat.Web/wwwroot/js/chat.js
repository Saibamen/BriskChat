$(".ui.sidebar").sidebar("setting", {
    transition: "overlay"
});

$(".ts_tip").popup({
    variation: "inverted"
});

$(".item > .btn_unstyle.right").popup({
    variation: "inverted"
});

function loadingStart() {
    $(".ui.main.container").css("display", "none");
    $(".ui.dimmer").addClass("active");
}

function loadingStop() {
    $(".ui.main.container").css("display", "block");
    $(".ui.dimmer").removeClass("active");
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

var currentRoomId = $(".menu > a.item.active").data("id");

$.connection.hub.url = "http://localhost:52284/signalr";
var myHub = $.connection.channelHub;

// Change room
$(".menu").on("click", ".menu > a.item", function (e) {
    // Leave current room
    loadingStart();
    myHub.server.leaveRoom(currentRoomId);
    $(".menu > a.item.active").removeClass("active");

    currentRoomId = $(e.target).data("id");
    myHub.server.joinRoom(currentRoomId);
    $(e.target).addClass("active");
    loadingStop();
    console.log("Current room ID: " + currentRoomId);
});

myHub.client.broadcastMessage = function (userName, message, timestamp) {
    $("#chat_messages").append('<div class="ts-message"><div class="message_gutter"><div class="message_icon"><a href="/team/malgosia" target="/team/malgosia" class="member_image" data-member-id="U42KXAW07" style="background-image: url(\'../images/troll.png\')" aria-hidden="true" tabindex="-1"> </a></div></div><div class="message_content"><div class="message_content_header"><a href="#" class="message_sender">' + userName + '</a><a href="#" class="timestamp">' + timestamp + '</a></div><span class="message_body">' + message + '</span></div></div>');

    // Scroll #chat_messages
    $("#chat_messages").clearQueue();
    $("#chat_messages").animate({ scrollTop: $("#chat_messages")[0].scrollHeight }, "slow");
}

myHub.client.loadRooms = function (result) {
    console.log(result);
    $.each(result,
        function (index, value) {
            var divToAppend = '<a class="item" data-id="' + value.Id + '">';
            if (value.IsPublic) {
                divToAppend += '<i class="icon left">#</i>';
            } else {
                divToAppend += '<i class="lock icon left"></i>'
            }
            divToAppend += value.Name + '</a>';

            $("#channelsMenu").append(divToAppend);
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
    loadingStop();
}

// Start the connection
$.connection.hub.start()
    .done(function () {
        // Joining to room
        myHub.server.getRooms();
        myHub.server.joinRoom(currentRoomId);
        console.log("Connected :)");

        $("#msg_form").keypress(function (e) {
            if (e.which == 13) {
                if (!e.shiftKey) {
                    if (message = $("#msg_input").val().trim()) {
                        console.log("Wysyłam: " + message + " do pokoju " + currentRoomId);
                        myHub.server.send(currentRoomId, message);
                    }

                    e.preventDefault();
                    $("#msg_input").val("");
                }
            }
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
        loadingStop();
    }
});

$.connection.hub.reconnected(function () {
    console.log("Reconnected");
});

$("#createNewChannel").click(function () {
    $("#createChanelForm")[0].reset();

    var item = $("input[name*='IsPublic']")[1];
    item.value = false;

    $('.ui.modal')
        .modal('setting', 'closable', false)
        .modal('show');
});

$('#myCheckBox').click(function () {
    if ($(this).is(':checked')) {
        $("#createNewChannelHeader").html('Create a channel');
        $("#createNewChannelLabel").html('Anyone on your team can view and join this channel');
        var item = $("input[name*='IsPublic']")[1];
        item.value = true;
    } else {
        $("#createNewChannelHeader").html('Create a private channel');
        $("#createNewChannelLabel").html('This channel can only be joined by invite');
        var item = $("input[name*='IsPublic']")[1];
        item.value = false;
    }
});

$("#createChanelForm").submit(function (e) {
    loadingStart();
    e.preventDefault();
    data = serializeForm($(this));
    myHub.server.createNewChannel(data);
    $(".ui.modal").modal("hide");
});

function serializeForm(_form) {
    data = $(_form).serializeArray();
    var obj = {};

    $.each(data,
        function (key, value) {
            obj[value.name] = value.value;
        });

    return obj;
}