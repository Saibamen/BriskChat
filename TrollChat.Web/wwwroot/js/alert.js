$(".ui.message").on("click", function () {
    $(this).closest(".ui.message").transition("fade");

    // Resize #chat_messages on close
    $("#chat_messages").height($(window).height() - 117);
});