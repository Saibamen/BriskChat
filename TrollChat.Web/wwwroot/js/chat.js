$(".ui.sidebar").sidebar("setting", {
    transition: "overlay"
});

$(".ts_tip").popup({
    variation: "inverted"
});

$(".item > .btn_unstyle.right").popup({
    variation: "inverted"
});

/*
 *  Resizing
 */

$(window).resize(function() {
    // #chat_messages
    var chat_additional_height = 117;

    if ($(".ui.message").length) {
        if (! $(".ui.message").hasClass("hidden")) {
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

$(function () {
    $.connection.hub.url = "http://localhost:52284/signalr";
    var myHub = $.connection.channelHub;

    var testName = "TestName";

    myHub.client.broadcastMessage = function (userName, message, chatTime) {
        $("#chat_messages").append('<div class="ts-message"><div class="message_gutter"><div class="message_icon"><a href="/team/malgosia" target="/team/malgosia" class="member_image" data-member-id="U42KXAW07" style="background-image: url(\'https://ca.slack-edge.com/T0MBAPD9S-U42KXAW07-991dd9964bd3-48\')" aria-hidden="true" tabindex="-1"> </a></div></div><div class="message_content"><div class="message_content_header"><a href="#" class="message_sender">' + userName + '</a><a href="#" class="timestamp">' + chatTime + '</a></div><span class="message_body">' + message + '</span></div></div>');
    }

    // Start the connection
    $.connection.hub.start()
        .done(function () {
            console.log("Connected :)");
            console.log("connection ID: " + $.connection.hub.id);

            $("#msg_form").keypress(function (e) {
                if (e.which == 13) {
                    if (! e.shiftKey) {
                        if(message = $("#msg_input").val().trim()) {
                            console.log("Wysyłam: " + message);
                            myHub.server.send(testName, message);
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

    $.connection.hub.received = function(data) {
        console.log(data);
    };

    $.connection.hub.error = function(error) {
        console.warn(error);
    };

    $.connection.hub.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.reconnecting) {
            console.log("Re-connecting");
            $(".ui.main.container").css("display", "none");
            $(".ui.dimmer").addClass("active");
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            console.log("The server is online");
            $(".ui.main.container").css("display", "block");
            $(".ui.dimmer").removeClass("active");
        } 
    });

    $.connection.hub.reconnected(function() {
        console.log("Reconnected");
    });
});