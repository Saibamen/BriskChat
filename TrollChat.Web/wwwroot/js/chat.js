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
 *  SignalR
 */

$(function () {
    $.connection.hub.url = "http://localhost:52283/signalr";
    var myHub = $.connection.channelHub;

    var testName = "TestName";

    myHub.client.broadcastMessage = function (username, message) {
        console.log(testName + ": " + message);
    }

    // Start the connection
    $.connection.hub.start()
        .done(function () {
            console.log("Connected :)");
            console.log("connection ID: " + $.connection.hub.id);

            $("#msg_form").keypress(function (e) {
                if (e.which == 13) {
                    message = $("#msg_input").val(); 
                    myHub.server.send(testName, message);
                    $("#msg_input").val("");
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
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            console.log("The server is online");
        } 
    });

    $.connection.hub.reconnected(function() {
        console.log("Reconnected");
    });
});