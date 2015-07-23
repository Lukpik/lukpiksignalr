﻿var hubEngine;
var _latitude;
var _longitude;
var uname;
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
                //banner details
                $('#lblStorename').text(obj[0].store_name);
                var address = CheckNull(obj[0].store_street_addressline1) + ", " + CheckNull(obj[0].store_street_addressline2) + ", " + CheckNull(obj[0].store_city) + ", " + CheckNull(obj[0].store_state) + ", " + CheckNull(obj[0].store_country);
                $('#lblStoreAddress').text(address);
                $('#lblShortDesc').text(CheckNull(obj[0].StoreTagLine));
                $('#ancFb').prop("href", CheckNull(obj[0].StoreFacebookPage,"anc"));
                $('#ancTwitter').prop("href", CheckNull(obj[0].StoreTwitterPage,"anc"));
                $('#ancGoogle').prop("href", CheckNull(obj[0].StoreGooglePage,"anc"));

                //Store details
                $('#txtStorename').val(CheckNull(obj[0].store_name));
                $('#txtShortDesc').val(CheckNull(obj[0].StoreTagLine));
                $('#txtStoreDescription').val(CheckNull(obj[0].StoreDescription));

                //Store owner details

                $('#txtOwnerFirstname').val(CheckNull(obj[0].StoreOwnerFirstName));
                $('#txtOwnerLastname').val(CheckNull(obj[0].StoreOwnerLastName));
                $('#txtEmail').val(CheckNull(obj[0].Email));

                $('#txtPhone').val(CheckNull(obj[0].store_phone));


                //address details
                $('#txtAddressLine1').val(CheckNull(obj[0].store_street_addressline1));
                $('#txtAddressLine2').val(CheckNull(obj[0].store_street_addressline2));
                $('#txtCity').val(CheckNull(obj[0].store_city));
                $('#txtPincode').val(CheckNull(obj[0].store_postal_code));


                //Social pages
                $('#txtWebsiteURL').val(CheckNull(obj[0].SocialWebsite));
                $('#txtFbURL').val(CheckNull(obj[0].SocialFacebookPage));
                $('#txtTwitterURL').val(CheckNull(obj[0].SocialTwitterPage));
                $('#txtGooglePlusURL').val(CheckNull(obj[0].SocialGooglePage));

            }
            if (obj[0].Latitude != "" && obj[0].Latitude != null && obj[0].Longitude != "" && obj[0].Longitude != null)
                InitializeMap(CheckNull(obj[0].Latitude) * 1, CheckNull(obj[0].Longitude) * 1)
            else
                IdentifyLocation();
        }
    };

    hubEngine.client.updatedStores = function (msg) {
        if (msg == "1")
            AddAlert("", "Updated successfully.");
        else if (msg == "0")
            AddAlert("error", "Something went wrong, please try again later.");
    };

    hubEngine.client.changedPassword = function (msg) {
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
    };
    $('#modalChangePassword').modal('show');
});

function UpdateStoredetails() {
    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    $('#alertID').empty();
    var storename = $('#txtStorename').val();
    var storetype = document.getElementById("ddStoreTYpe").value;
    var shortDesc = $('#txtShortDesc').val();
    var storedesc = $('#txtStoreDescription').val();
    var brands = $('#tags_Brands').val();
    var othercategories = "";
    var isCreditCard = $('input[name=isCreditcard]:checked').val();
    var isHomedelivery = $('input[name=isHomedelivery]:checked').val();
    var isTrailfacility = $('input[name=isTrail]:checked').val();

    var ownerFname = $('#txtOwnerFirstname').val();
    var ownerLname = $('#txtOwnerLastname').val();
    var mobile = $('#txtPhone').val();
    var email = $('#txtEmail').val();

    //address
    var addLine1 = $('#txtAddressLine1').val();
    var addLine2 = $('#txtAddressLine2').val();
    var city = $('#txtCity').val();
    var state = document.getElementById("ddState").value;
    var country = $('#txtCountry').val();
    var pincode = $('#txtPincode').val();

    //social urls
    var website = $('#txtWebsiteURL').val();
    var fburl = $('#txtFbURL').val();
    var twitterurl = $('#txtTwitterURL').val();
    var googleurl = $('#txtGooglePlusURL').val();
    var latitude = _latitude;
    var longitude = _longitude

    //var uname = $('#txtUname').val();
    //var pwd = $('#txtPassword').val();
    if (storename != "" && storetype != "" && brands != "" && ownerFname != "" && ownerLname != "" && mobile != "" && email != "") {
        hubEngine.server.updateStores(storename, storetype, shortDesc, storedesc, brands, othercategories, isCreditCard, isTrailfacility, isHomedelivery, ownerFname, ownerLname, mobile, email, addLine1, addLine2, city, state, country, pincode, latitude, longitude, website, fburl, twitterurl, googleurl, $.connection.hub.id);
    }
    else {
        AddAlert("error", "Please fill all the fields.");
    }
}

function AddAlert(typeMSG, message) {
    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    var alertType = "alert-success";
    if (typeMSG == "error")
        alertType = "alert-danger";
    var str = '<div id="alertID" class="alert ' + alertType + ' alert-dismissible fade in" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">×</span></button><label id="lblmsg">' + message + '</label></div>';
    $('#alertIDBottom').append(str);
    $('#alertIDTop').append(str);
    $.fn.dpToast(message);
}

function CheckNull(value, fromType) {
    var valIFNull = "";
    if (fromType == "anc") {
        valIFNull = "#";
    }
    
    if (value == null)
        return valIFNull;
    else if(value=="")
        return valIFNull;
    else
        return value;
}

function Test() {
    document.getElementById("rbtnCCYes").checked = false;
    document.getElementById("rbtnCCNo").checked = true;
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
    var currPwd = $('#txtCurrentPwd').val();
    var pwd = $('#txtPwd').val();
    var cpwd = $('#txtCPwd').val();
    if (currPwd != "" && pwd != "" && cpwd != "") {
        if (CheckConfirmationPwd()) {
            hubEngine.server.changeStorePassword(readCookie("lukpikretailer_usename"), currPwd, pwd, $.connection.hub.id);
        }
        else {
            $('#errorMessage').show();
            $('#errorMessage').text("Password Mismatch");
        }
    }
    else {
        $('#errorMessage').show();
        $('#errorMessage').text("Please fill all the fields.");
    }
}