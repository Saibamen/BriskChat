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
    var connection = $.hubConnection("http://localhost:52283/signalr");

    var testName = "TestName";

    /*connection.broadcastMessage = function (userName, message) {
        console.log("broadcastMessage  function");
        var encodedName = $.text(userName).html();
        var encodedMessage = $.text(message).html();

        // TODO: add message to chat
    };*/

    // Start the connection
    connection.start()
        .done(function () {
            console.log("Connected :)");
            console.log("connection ID: " + connection.id);

            $("#msg_form").keypress(function (e) {
                if (e.which == 13) {
                    console.log("Sending message " + $("#msg_input").val());
                    connection.send(testName, $("#msg_input").val());
                    $("#msg_input").val("");
                }
            });
        })

        .fail(function (a) {
            console.log("Not connected! " + a);
        });

    connection.receive(function(data) {
        console.log(data);
    });

    connection.error(function(error) {
        console.warn(error);
    });

    connection.stateChanged(function (change) {
        if (change.newState === $.signalR.connectionState.reconnecting) {
            console.log("Re-connecting");
        }
        else if (change.newState === $.signalR.connectionState.connected) {
            console.log("The server is online");
        } 
    });

    connection.reconnected(function() {
        console.log("Reconnected");
    });
});