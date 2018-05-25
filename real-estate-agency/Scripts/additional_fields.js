var j = 1;
function add_phone_field() {
    var name = "Phone" + j;
    var button = '<input class="form-control add-edit-phone-photo-remove-button" type="button" value="Х" id="delete' + j + '" onclick="$(\'#' + name + '\').remove(); $(\'#delete' + j+'\').remove();" />';
    var input = '<input class="form-control add-edit-phone-photo-field" type="tel" name="' + name + '" id="' + name +'"/>';

    $("#phonediv").append('<div class="but form-group">' + input + button + '</div>');
    j++;
}

var i = 1;
function add_photo_field() {
    var name = "Images" + i;
    var input_file = '<input class="form-control add-edit-phone-photo-field" type="file" name="' + name + '" id="' + name + '" />';
    var input_button = '<input class="form-control add-edit-phone-photo-remove-button" type="button" value="Х" id="remove' + i + '" onclick="$(\'#' + name + '\').remove(); $(\'#remove' + i + '\').remove();" />';

    $("#photodiv").append('<div class="but form-group">' + input_file + input_button + '</div>');
    i++;
}
