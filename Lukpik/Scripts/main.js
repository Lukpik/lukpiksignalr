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
    hubEngine.client.testJS = function (msg) {
        //alert(msg);
    };

    hubEngine.client.mailSent = function (msg) {
        //alert(msg);
        RemoveProgressBarLoader();
        
        if (msg == "1") {
            $('#lblmsg').show();
            $('#lblmsg').text("Thank you for contacting us.");
        }
        else {
            $('#lblmsg').show();
            $('#lblmsg').text("Something went wrong, please try again later.");
        }
    };



    //Retailers Page




});


//Website Home Page
function Subscribe() {
    var textemail = $('#txtSubscribeEmail').val();
    hubEngine.server.testMethod(textemail, $.connection.hub.id);
}

function ContactUs() {
    AddProgressBarLoader();
    var name = $('#txtName').val();
    var phone = $('#txtPhone').val();
    var email = $('#txtEmail').val();
    var message = $('#txtMessage').val();
    if (name != "" && phone != "" && email != "" && message != "") {
        if (validEmail) {
            hubEngine.server.contactUs(name,email,phone,message, $.connection.hub.id)
        }
        else {
            RemoveProgressBarLoader();
        }

    }
    else {
        RemoveProgressBarLoader();
        $('#lblmsg').show();
        $('#lblmsg').text("Please fill all fields!");
    }
}

function ValidateEmail() {
    //Checks from Retailer registration and login pages
    var email = $("#txtEmail").val();
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

//Retailers Page