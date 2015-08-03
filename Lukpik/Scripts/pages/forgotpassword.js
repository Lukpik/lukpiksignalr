var hubEngine;
var validEmail;
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
    hubEngine.client.gotPassword = function (msg) {
        if (msg == "1") {
            $('#lblmsg').show();
            $('#lblmsg').text("Success ! Please check your mobile.");
        }
        else if (msg == "2") {
            $('#lblmsg').show();
            $('#lblmsg').text("mobile number does not exists.");
        }
        else {
            $('#lblmsg').show();
            $('#lblmsg').text("Something went wrong, please try again later.");
        }
    };

});

function GetPassword() {
    $('#lblmsg').hide();
    var uname = $('#txtUname').val();
    if (uname != "") {
        if (uname.length==10)
            hubEngine.server.getRetailerPassword(uname, $.connection.hub.id);
        else {
            $('#lblmsg').show();
            $('#lblmsg').text("Incorrect Moile number!");
        }

    }
    else {
        $('#lblmsg').show();
        $('#lblmsg').text("Please enter your mobile number!");
    }

  
}

function ValidateEmail() {
    //Checks from Retailer registration and login pages
    var email = $("#txtUname").val();
    if (!validateEmail(email)) {
        validEmail = false;
        $('#lblmsg').show();
        $('#lblmsg').text("Incorrect Email format!");

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