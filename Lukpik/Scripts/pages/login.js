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
    hubEngine.client.loginResult = function (username,msg) {
        $('#lblmsg').show();
        if (msg == "1") {
            createCookie("lukpikretailer_usename", username, 1);
            $('#lblmsg').text("Login success, please wait, we redirect you to your space.");
            location.href = "../retailers/home.html";
        }
        else if (msg == "0")
            $('#lblmsg').text("Something went wrong, please try again later.");
    };

});

function Login() {
    $('#lblmsg').hide();
    var uname = $('#txtUname').val();
    var pwd = $('#txtPassword').val();
    if (uname != "" && pwd != "") {
        hubEngine.server.login(uname,pwd, $.connection.hub.id);
    }
    else {
        $('#lblmsg').show();
        $('#lblmsg').text("Please fill all fields.");
    }
}