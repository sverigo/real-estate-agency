var j = 1;
function add_phone_field() {
    var name = "Phone" + j;
    var button = '<input type="button" value="Х" id="delete' + j + '" onclick="$(\'#' + name + '\').remove(); $(\'#delete' + j+'\').remove();" />';
    var input = '<input type="tel" name="' + name + '" id="' + name +'"/>';

    $("#phonediv").append('<div class="but">' + button + input + '</div>');
    j++;
}

var i = 1;
function add_photo_field() {
    var name = "Images" + i;
    var input_file = '<input type="file" name="' + name + '" id="' + name + '" />';
    var input_button = '<input type="button" value="Х" id="remove' + i + '" onclick="$(\'#' + name + '\').remove(); $(\'#remove' + i + '\').remove();" />';

    $("#photodiv").append('<div class="but">' + input_button + input_file + '</div>');
    i++;
}
