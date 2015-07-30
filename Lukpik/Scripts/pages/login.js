var hubEngine;
$(document).ready(function () {
    
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        //alert('success');
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 1000); // Re-start connection after 1 seconds
    });
    setInterval(function () {
        hubEngine.server.activecall();
    }, 15000);

    //Website Home Page
    hubEngine.client.loginResult = function (username, msg) {
        $('#lblmsg').show();
        if (msg == "1") {
            createCookie("lukpikretailer_usename", username, 1);
            $('#lblmsg').text("Login success, please wait while we redirect you to your store.");
            location.href = "../retailers/home.html";
        }
        else if (msg == "2") {
            $('#lblmsg').text("Invalid username / password.");
            RemoveProgressBarLoader();
        }
        else {
            $('#lblmsg').text("Something went wrong, please try again later.");
            RemoveProgressBarLoader();
        }
    };

});

function Login() {
    AddProgressBarLoader();
    $('#lblmsg').hide();
    var uname = $('#txtUname').val();
    var pwd = $('#txtPassword').val();
    if (uname != "" && pwd != "") {
        if (validEmail)
            hubEngine.server.login(uname, pwd, $.connection.hub.id);
        else {
            RemoveProgressBarLoader();
            $('#lblmsg').show();
            $('#lblmsg').text("Incorrect Email Format.");
        }
    }
    else {
        RemoveProgressBarLoader();
        $('#lblmsg').show();
        $('#lblmsg').text("Please fill all fields.");
    }
}

function ValidateEmail() {
    //Checks from Retailer registration and login pages
    var email = $("#txtUname").val();
    if (!validateEmail(email)) {
        validEmail = false;
        $('#lblmsg').show();
        $('#lblmsg').text("Incorrect Email Format!");

    }
    else if (validateEmail(email)) {
        $('#lblmsg').hide();
        $('#lblmsg').text("");

        validEmail = true;
    }
}
function validateEmail(email) {
    var emailReg = new RegExp(/^(("[\w-\s]+")|([\w-]+(?:\.[\w-]+)*)|("[\w-\s]+")([\w-]+(?:\.[\w-]+)*))(@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$)|(@\[?((25[0-5]\.|2[0-4][0-9]\.|1[0-9]{2}\.|[0-9]{1,2}\.))((25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\.){2}(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[0-9]{1,2})\]?$)/i);
    var valid = emailReg.test(email);
    if (!valid) {
        return false;
    } else {
        return true;
    }
}
function fnPasswordEnter(e) {
    if (e.keyCode === 13) {
        Login();
    }

    return false;
}