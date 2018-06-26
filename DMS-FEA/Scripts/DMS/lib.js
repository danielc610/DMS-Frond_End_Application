function resetForm(id) {
    $(id)[0].reset();
    $(': button').prop("disabled", false);
}



var defineNew = function (btnId) {
    $(btnId).dialog({
        autoOpen: false,
        width: 400,
        height: 300,
        modal: true,
        Title: 'Create new',
        buttons: {
            'Save': function () {
            },
            'Cancel': function () {
                $(this).dialog('close');
            }
        }
    });
}

function resetList(list) {
    $(list).prop("disabled", true);
}

function showbox(div, list){
    $(div).prop("hidden", false);
    document.getElementById(list).disabled = true;
    
}

function capLock(e) {
    var kc = e.keyCode ? e.keyCode : e.which;
    var sk = e.shiftKey ? e.shiftKey : kc === 16;
    var visibility = ((kc >= 65 && kc <= 90) && !sk) ||
        ((kc => 97 && kc <= 122) && sk) ? 'visible' : 'hidden';
    document.getElementById('divMayus').style.visibility = visibility
}
