$(".ui.sidebar").sidebar("setting", {
    transition: "overlay"
});

$(".ts_tip").popup({
    variation: "inverted"
});

$(".item > .btn_unstyle.right").popup({
    variation: "inverted"
});

$.connection.hub.url = 'http://localhost:52284/signalr';
var myHub = $.connection.channelHub;

myHub.client.broadcastMessage = function (username, message) {
    console.log(message);
}
newConnection();

function newConnection() {
    $.connection.hub.start().done(function () {
        
    });
}

$("#msg_form").keypress(function(e) {
    if (e.which == 13) {
        varmessage = $("#msg_input").val(); 
        myHub.server.send(varmessage, varmessage);
    }
});