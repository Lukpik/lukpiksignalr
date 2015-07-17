var hubEngine;
$(document).ready(function () {
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        alert('success');
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 1000); // Re-start connection after 1 seconds
    });
    setInterval(function () {
        hubEngine.server.activecall();
    }, 15000);


    hubEngine.client.testJS = function (msg) {
        alert(msg);
    };
});



function Subscribe() {
    var textemail = $('#txtSubscribeEmail').val();
    hubEngine.server.testMethod(textemail, $.connection.hub.id);
}