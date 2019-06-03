

function quote(id) {
    var oldText = $("#textbox").val();
    var newText = oldText + "#" + id + "\n"

    scrollToAnchor("textbox");

    $("#textbox").val(newText);

   
}