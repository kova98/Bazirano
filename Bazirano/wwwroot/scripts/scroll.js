
function jumpToAnchor(id) {
    var tag = "#" + id;
    $(document).scrollTop($(tag).offset().top);
} 

function scrollToAnchor(id) {
    var element = $("#" + id);
    $('html,body').animate({ scrollTop: element.offset().top }, 'slow');
}