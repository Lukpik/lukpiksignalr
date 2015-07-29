
function Startup() {
    var fname=localStorage.getItem("fname");
    if (typeof (fname != typeof (undefined) && fname != null)) {
        $('#txtFirstname').text(fname);
    }

    var image = localStorage.getItem("profilepic");
    if (typeof (image != typeof (undefined) && image != null)) {
        document.getElementById("imgStoreTop").src = image;
    }
}


function createCookie(name, value, days) {
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        var expires = "; expires=" + date.toGMTString();
    }
    else var expires = "";
    document.cookie = name + "=" + value + expires + "; path=/";
}

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function eraseCookie(name) {
    createCookie(name, "", -1);
}

function isNumberKey(control, evt, len) {
    if ($('#' + control.id).val().length < len) {
        var charCode = (evt.which) ? evt.which : event.keyCode;

        if (charCode > 31 && (charCode < 48 || charCode > 57))
            return false;


        return true;
    }
    else
        return false;
}

function logout() {
    eraseCookie('lukpikretailer_usename');
    location.href = '../login.html';
}

function findStoreEmail() {
    if (readCookie("lukpikretailer_usename") == null || readCookie("lukpikretailer_usename") == "") {
        location.href = '../login.html';
    }
}

function AddProgressBarLoader() {
    
    processBar = new ajaxLoader("body", { classOveride: 'blue-loader', bgColor: '#000' });// To start process bar
}
function RemoveProgressBarLoader() {
    if (typeof (processBar) != typeof (undefined))
        processBar.remove();
}

function AddAlert(typeMSG, message) {
    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    var alertType = "alert-success";
    if (typeMSG == "error")
        alertType = "alert-danger";
    var str = '<div id="alertID" class="alert ' + alertType + ' alert-dismissible fade in" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true" onclick="RemoveAlert();">×</span></button><label id="lblmsg">' + message + '</label></div>';
    $('#alertIDBottom').append(str);
    $('#alertIDTop').append(str);
    $.fn.dpToast(message);
}

function RemoveAlert() {
    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
}