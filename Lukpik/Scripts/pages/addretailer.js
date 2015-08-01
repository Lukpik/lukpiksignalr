var hubEngine;
var validEmail;
var capt = "";
$(document).ready(function () {
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        //alert('success');
        hubEngine.server.fillCapctha($.connection.hub.id);
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 1000); // Re-start connection after 1 seconds
    });
    setInterval(function () {
        hubEngine.server.activecall();
    }, 15000);

    hubEngine.client.showCaptcha = function (msg, cap) {
        $("#imgcaptcha").attr('src', "StoreImages/captcha/" + msg);
        capt = cap;
    }

    hubEngine.client.testJS = function (msg) {
        $('#lblmsg').show();
        if (msg == "1") {
            RemoveProgressBarLoader();
            $('#lblmsg').hide();
            $('#divmsg').append("You have successfully registered with us, we send password to your phone.");
            $('#btnRegister').hide();
            //<a style='color:rgba(61, 201, 179, 1);' href='retailers/login.html'>Click here to login</a>
        }
        else if (msg == "2") {
            RemoveProgressBarLoader();
            //$('#lblmsg').text("Email already exists. Please try with different email.");
            $('#lblmsg').text("Phone number already exists. Please try with different number.");
            $('#btnRegister').prop("disabled", false);
        }
        else if (msg == "0") {
            RemoveProgressBarLoader();
            $('#lblmsg').text("Something went wrong, please try again later.");
            $('#btnRegister').prop("disabled", false);
        }
    };



    //Retailers Page




});


//Website Home Page
function AddStore() {
   
    AddProgressBarLoader();
    $('#lblmsg').hide();
    $('#btnRegister').prop("disabled",true);
    //var textemail = $('#txtSubscribeEmail').val();
    var firstname = $('#txtFirstname').val();
    var lastname = $('#txtLastname').val();
    var email = $('#txtEmail').val();
    var storesname = $('#txtStorename').val();
    var phonenum = $('#txtPhonenum').val();
    var phonenum2 = $('#txtPhonenum2').val();
    var city = $('#txtCity').val();
    var captchaCode = $("#txtCode").val().trim();
    var checkEmailnotNull = email == "" ? true : false;
    if (firstname != "" && lastname != "" && storesname != "" && phonenum != "" && city != "")
        if (!checkEmailnotNull && !validEmail) {
            RemoveProgressBarLoader();
            $('#btnRegister').prop("disabled", false);
            $('#lblmsg').show();
            $('#lblmsg').text("Incorrect Email Format!");
        }
        else {
            if (phonenum.length == 10) {

                if (capt == captchaCode) {
                    hubEngine.server.addStore(firstname, lastname, email, storesname, phonenum, phonenum2,city, $.connection.hub.id);
                }
                else {
                    RemoveProgressBarLoader();
                    $('#btnRegister').prop("disabled", false);
                    $('#lblmsg').show();
                    $('#lblmsg').text("Incorrect captcha code.");
                }
            }
            else {
                RemoveProgressBarLoader();
                $('#btnRegister').prop("disabled", false);
                $('#lblmsg').show();
                $('#lblmsg').text("Please enter a valid mobile number.");
            }
            
        }
    else {
        RemoveProgressBarLoader();
        $('#btnRegister').prop("disabled", false);
        $('#lblmsg').show();
        $('#lblmsg').text("Please fill all the fields.");
    }
}

function ValidateEmail() {
    var email = $("#txtEmail").val();
    if (!validateEmail(email)) {
        validEmail = false;
        RemoveProgressBarLoader();
        $('#btnRegister').prop("disabled", false);
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