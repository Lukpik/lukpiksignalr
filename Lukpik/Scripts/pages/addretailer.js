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
        alert(msg);
    };



    //Retailers Page




});


//Website Home Page
function AddStore() {
    //var textemail = $('#txtSubscribeEmail').val();

    hubEngine.server.testInsert("val1","val2", $.connection.hub.id);
}





//Retailers Page