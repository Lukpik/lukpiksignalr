
function Startup() {
    var uname = readCookie("lukpikretailer_usename");
    if (typeof (uname) == typeof(undefined) || uname == "") {
        logout();
    }
    var fname=localStorage.getItem("fname");
    if (typeof (fname != typeof (undefined) && fname != null)) {
        $('#txtFirstname').text(fname);
    }

    var image = localStorage.getItem("profilepic");
    if (typeof (image != typeof (undefined) && image != null)) {
        document.getElementById("imgStoreTop").src = image;
    }
    $('input').addClass('your-class');
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

function AddAlert() {
    //alert('hi');
}



function CheckConfirmationPwd() {
    var retval = false;
    var pwd = $('#txtPwd').val();
    var cpwd = $('#txtCPwd').val();
    if (pwd == cpwd) {
        $('#errorMessage').hide();
        retval = true;
    }
    else {
        $('#errorMessage').show();
        $('#errorMessage').text("Password Mismatch");
        retval = false;
    }
    return retval;
}

function ChangePassword() {
    AddProgressBarLoader();
    var currPwd = $('#txtCurrentPwd').val();
    var pwd = $('#txtPwd').val();
    var cpwd = $('#txtCPwd').val();
    if (currPwd != "" && pwd != "" && cpwd != "") {
        if (currPwd.length >= 6 && pwd.length >= 6 && cpwd.length >= 6) {
            if (CheckConfirmationPwd()) {
                hubEngine.server.changeStorePassword(readCookie("lukpikretailer_usename"), currPwd, pwd, $.connection.hub.id);
            }
            else {
                RemoveProgressBarLoader();
                $('#errorMessage').show();
                $('#errorMessage').text("Password Mismatch");
            }
        }
        else {
            RemoveProgressBarLoader();
            $('#errorMessage').show();
            $('#errorMessage').text("Minimum of 6 charecters required to set password.");
        }

    }
    else {
        RemoveProgressBarLoader();
        $('#errorMessage').show();
        $('#errorMessage').text("Please fill all the fields.");
    }
}

function ChangedPassword(msg) {
    if (msg == "1") {
        $('#errorMessage').removeClass("label-danger");
        $('#errorMessage').addClass("label-success");
        $('#errorMessage').show();
        $('#errorMessage').text("Password is changed successfully.");
        logout();
    }
    else if (msg == "2") {
        $('#errorMessage').show();
        $('#errorMessage').text("You have entered wrong password.");
    }
    else if (msg == "0") {
        $('#errorMessage').show();
        $('#errorMessage').text("Something went wrong, please try again later.");
    }
}