var hubEngine;
$(document).ready(function () {
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        //alert('success');
        var uname = readCookie("lukpikretailer_usename");
        hubEngine.server.getRetailerDetails(uname, $.connection.hub.id);
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
    hubEngine.client.gotDetails = function (jsObj, msg) {
        if (msg == "0") {
            alert('Something went wrong.')
        }
        else if (msg == "1") {
            var obj = jQuery.parseJSON(jsObj);

            if (typeof (obj) != typeof (undefined)) {
                //Store details
                $('#txtStorename').val(obj[0].store_name);
                $('#txtShortDesc').val(obj[0].store_name);
                $('#txtStoreDescription').val(obj[0].StoreDescription);

                //Store owner details
                
                $('#txtOwnerFirstname').val(obj[0].StoreOwnerFirstName);
                $('#txtOwnerLastname').val(obj[0].StoreOwnerLastName);
                $('#txtEmail').val(obj[0].Email);

                $('#txtPhone').val(obj[0].store_phone);
                $('#txtCity').val(obj[0].store_city);


                //Social pages
                $('#txtWebsiteURL').val(obj[0].SocialWebsite);
                $('#txtFbURL').val(obj[0].SocialFacebookPage);
                $('#txtTwitterURL').val(obj[0].SocialTwitterPage);
                $('#txtGooglePlusURL').val(obj[0].SocialGooglePage);
            }
            IdentifyLocation();
        }
    };

});

function Login() {
    $('#lblmsg').hide();
    var uname = $('#txtUname').val();
    var pwd = $('#txtPassword').val();
    if (uname != "" && pwd != "") {
        hubEngine.server.login(uname, pwd, $.connection.hub.id);
    }
    else {
        $('#lblmsg').show();
        $('#lblmsg').text("Please fill all fields.");
    }
}