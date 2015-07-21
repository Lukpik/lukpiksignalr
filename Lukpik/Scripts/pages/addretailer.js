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
    hubEngine.client.testJS = function (msg) {
        $('#lblmsg').show();
        if (msg == "1")
            $('#lblmsg').text("You have successfully registered with us, please check you mail for password. <a href='retailers/lohin.html'>Click here to login</a>");
        else if (msg == "2")
            $('#lblmsg').text("Email already exists. Please try with different email.");
        else if (msg == "0")
            $('#lblmsg').text("Something went wrong, please try again later.");
    };



    //Retailers Page




});


//Website Home Page
function AddStore() {
    $('#lblmsg').hide();
    //var textemail = $('#txtSubscribeEmail').val();
    var firstname = $('#txtFirstname').val();
    var lastname = $('#txtLastname').val();
    var email = $('#txtEmail').val();
    var storesname = $('#txtStorename').val();
    var phonenum = $('#txtPhonenum').val();
    var city = $('#txtCity').val();
    if (firstname != "" && lastname != "" && email != "" && storesname != "" && phonenum != "" && city != "")
        hubEngine.server.addStore(firstname, lastname, email, storesname, phonenum, city, $.connection.hub.id);
    else {
        $('#lblmsg').show();
        $('#lblmsg').text("Please fill all the fields.");
    }
}





//Retailers Page