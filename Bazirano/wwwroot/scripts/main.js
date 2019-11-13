


window.onload = function () {
    var $recaptcha = document.querySelector('#g-recaptcha-response');
    if ($recaptcha) {
        $recaptcha.setAttribute("required", "required");
};

function quote(id) {
    var oldText = $("#textbox").val();
    var newText = oldText + "#" + id + "\n"

    scrollToAnchor("textbox");

    $("#textbox").val(newText);
}

function jumpToAnchor(id) {
    var tag = "#" + id;
    $(document).scrollTop($(tag).offset().top);
}

function scrollToAnchor(id) {
    var element = $("#" + id);
    $('html,body').animate({ scrollTop: element.offset().top }, 'slow');
}