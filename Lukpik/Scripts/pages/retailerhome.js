var hubEngine;
var _latitude;
var _longitude;
var uname;
var currFile = 0;
var totalFileCount = 0;
var filecount = 0;
var tempSrc = "";
$(document).ready(function () {
    AddProgressBarLoader();
    findStoreEmail();
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        //alert('success');
        var uname = readCookie("lukpikretailer_usename");
        hubEngine.server.checkFirstTimeLogin(uname, $.connection.hub.id);
        hubEngine.server.getAllSelectedCategories(uname, $.connection.hub.id);
        hubEngine.server.getAllSelectedBrands(uname, $.connection.hub.id);
        
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


    hubEngine.client.gotAllCatAndSelectedCat = function (json1, json2) {
        var allCategories = jQuery.parseJSON(json1);
        var selectedList = jQuery.parseJSON(json2);
        AddOtherCategories(allCategories, selectedList);
    };

    hubEngine.client.dpUploaded = function (img, msg) {
        if (msg == "0") {
            AddAlert("error","Failed to upload your profile picture, please try again later.");
        }
        else {
            //Image uploaded successfully
            AddAlert("", "Uploaded successfully.");
        }
    };

    hubEngine.client.gotAllSelectedBrands = function (list) {
        $('#tags_Brands').text(list);

        document.getElementById("tags_BrandsText").value = list;
    };

    hubEngine.client.isfirstTime = function (msg) {
        if (msg = "1") {
            $('#modalChangePassword').modal('show');
        }
    };
    //Website Home Page
    hubEngine.client.gotDetails = function (jsObj, msg) {
        if (msg == "0") {
            alert('Something went wrong.');
            RemoveProgressBarLoader();
        }
        else if (msg == "1") {
            var obj = jQuery.parseJSON(jsObj);

            if (typeof (obj) != typeof (undefined)) {
                //banner details
                if (obj[0].StoreImage != "" && obj[0].StoreImage != null) {
                    //document.getElementById("imgStore").src = obj[0].StoreImage;		
                    //document.getElementById("imgStoreTop").src = obj[0].StoreImage;		
                    hubEngine.server.updateImage(CheckNull(obj[0].Email), $.connection.hub.id, "refresh");
                }
                else {
                    document.getElementById("imgStore").src = "./images/default.png";
                    document.getElementById("imgStoreTop").src = "./images/default.png";
                    localStorage.setItem("profilepic", "./images/default.png");
                }
                $('#txtFirstname').text(CheckNull(obj[0].StoreOwnerFirstName));
                //Local Storage
                localStorage.setItem("fname", CheckNull(obj[0].StoreOwnerFirstName));
                $('#lblStorename').text(obj[0].store_name);
                var address = CheckNull(obj[0].store_street_addressline1) + ", " + CheckNull(obj[0].store_street_addressline2) + ", " + CheckNull(obj[0].store_city) + ", " + CheckNull(obj[0].store_state) + ", " + CheckNull(obj[0].store_country);
                $('#lblStoreAddress').text(address);
                $('#lblShortDesc').text(CheckNull(obj[0].StoreTagLine));
                $('#ancFb').prop("href", CheckNull(obj[0].StoreFacebookPage, "anc"));
                $('#ancTwitter').prop("href", CheckNull(obj[0].StoreTwitterPage, "anc"));
                $('#ancGoogle').prop("href", CheckNull(obj[0].StoreGooglePage, "anc"));
               
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
                $('[name=state] option').filter(function () {
                    return ($(this).text() == obj[0].store_state);
                }).prop('selected', true);


                //Social pages
                $('#txtWebsiteURL').val(CheckNull(obj[0].SocialWebsite));
                $('#txtFbURL').val(CheckNull(obj[0].SocialFacebookPage));
                $('#txtTwitterURL').val(CheckNull(obj[0].SocialTwitterPage));
                $('#txtGooglePlusURL').val(CheckNull(obj[0].SocialGooglePage));

                //checkboxes

                //Cards Accepted
                if (obj[0].Cardsaccepted == 1) {
                    document.getElementById("rbtnCCYes").checked = true;
                    document.getElementById("rbtnCCNo").checked = false;
                }
                else {
                    document.getElementById("rbtnCCYes").checked = false;
                    document.getElementById("rbtnCCNo").checked = true;
                }

                //Home delivery
                if (obj[0].homedeliveryflag == 1) {
                    document.getElementById("rbtnHDYes").checked = true;
                    document.getElementById("rbtnHDNo").checked = false;
                }
                else {
                    document.getElementById("rbtnHDYes").checked = false;
                    document.getElementById("rbtnHDNo").checked = true;
                }

                //Trail availability
                if (obj[0].trialroomflag == 1) {
                    document.getElementById("rbtnTrailYes").checked = true;
                    document.getElementById("rbtnTrailNo").checked = false;
                }
                else {
                    document.getElementById("rbtnTrailYes").checked = false;
                    document.getElementById("rbtnTrailNo").checked = true;
                }


            }
            if (obj[0].Latitude != "" && obj[0].Latitude != null && obj[0].Longitude != "" && obj[0].Longitude != null)
                InitializeMap(CheckNull(obj[0].Latitude) * 1, CheckNull(obj[0].Longitude) * 1)
            else
                IdentifyLocation();
            RemoveProgressBarLoader();
        }
    };

    hubEngine.client.updatedStores = function (msg) {
        
        if (msg == "1") {
            AddAlert("", "Updated successfully.");
            localStorage.setItem("fname", $('#txtOwnerFirstname').val());
        }
        else if (msg == "0")
            AddAlert("error", "Something went wrong, please try again later.");

        RemoveProgressBarLoader();
    };

    hubEngine.client.changedPassword = function (msg) {
        RemoveProgressBarLoader();
        ChangedPassword(msg);
    };
    //$('#modalChangePassword').modal('show');

    hubEngine.client.storeImgPath = function (img, msg,isfrm) {
        if (msg == "0") {
            AddAlert("error", "Failed to upload your profile picture, please try again later.");
        }
        else {
            //Image uploaded successfully
            var path = img;
            tempSrc = path;
            $('#avatar-modal').modal('hide');
            document.getElementById("imgStore").src = path;
            document.getElementById("imgStoreTop").src = path;
            localStorage.setItem("profilepic", path);
            if (isfrm != "refresh")
                AddAlert("", "Uploaded successfully.");
        }
    };
    RemoveProgressBarLoader();
});

function UpdateStoredetails() {
    AddProgressBarLoader();
    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    $('#alertID').empty();
    var storename = $('#txtStorename').val();
    var storetype = document.getElementById("ddStoreTYpe").value;
    var shortDesc = $('#txtShortDesc').val();
    var storedesc = $('#txtStoreDescription').val();
    var brands = $('#tags_BrandsText').val();
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
    var longitude = _longitude;
    var yourArray = [];
    $("input:checkbox[name=othercategories]:checked").each(function () {
        yourArray.push($(this).val());
    });
    othercategories = yourArray.toString();
    //var uname = $('#txtUname').val();
    //var pwd = $('#txtPassword').val();
    if (othercategories!="" && storename != "" && storetype != "" && addLine1 != "" && ownerFname != "" && ownerLname != "" && mobile != "" && email != "" && city != "" && pincode != "") {
        hubEngine.server.updateStores(storename, storetype, shortDesc, storedesc, brands, othercategories, isCreditCard, isTrailfacility, isHomedelivery, ownerFname, ownerLname, mobile, email, addLine1, addLine2, city, state, country, pincode, latitude, longitude, website, fburl, twitterurl, googleurl, $.connection.hub.id);
    }
    else {
        RemoveProgressBarLoader();
        AddAlert("error", "Please fill all the mandatory fields.");
    }
}



function CheckNull(value, fromType) {
    var valIFNull = "";
    if (fromType == "anc") {
        valIFNull = "#";
    }

    if (value == null)
        return valIFNull;
    else if (value == "")
        return valIFNull;
    else
        return value;
}

function Test() {
    //document.getElementById("rbtnCCYes").checked = false;
    //document.getElementById("rbtnCCNo").checked = true;
    $('#tags_Brands').value = "list,list2";
}


function FileSelected() {
    var sFileName = $("#avatarInput")[0].files[0].name;
    var sFileSize = $("#avatarInput")[0].files[0].size;
    var sFileExtension = sFileName.split('.')[sFileName.split('.').length - 1].toLowerCase();
    if (sFileExtension == 'jpg' || sFileExtension == 'jpeg' || sFileExtension == 'png' || sFileExtension == 'gif' || sFileExtension == 'xlsx') {
        //if (sFileSize < 10000000) {
        //$("#divProgress").attr("style", "width:10%");
        //$("#uploadfilePercentage").text("10%");
        totalFileCount = $("#avatarInput")[0].files.length;
        //$('#dropRegion').attr("style", "display:none;");
        //S('#progressBar').attr("style", "display:block;");
        //$('#progressBarETL').attr('style', 'display:block;');
        //            for (j = 0; j < totalFileCount; j++) {
        //                var progControl = $("#dvProgess").append($("#fileInput")[0].files[j].name + "<div id='progress" + j + "'></div><div class='progress progress-striped' id='ProgressComp" + j + "' style='width: 400px; height: 15px; '> <div class='bar' id='progbarWidth" + j + "' style='width: 0%; height: 15px;'>&nbsp;</div></div>");
        //            }
        //uploadFile();
        sendFile($("#avatarInput")[0].files[0]);
        //}
        //else {
        //     $("#fileInput").val("");
        //     $("#filesizeAlertModal").modal("show");
        // }
    }
    else {
        //alert('Currently infactum can not support this extension.');
        $("#avatarInput").val("");
        //$("#myExtensionAlertModal").modal("show");
    }
}

//File Upload and other function Script
function sendFile(file) {
    //var mainFileName = file.name.replace(/[&]/g, "([@-@-@])");
    //$("#divProgress").attr("style", "width:30%");
    //$("#uploadfilePercentage").text("30%");
    //$('#hdnFileName').val(file.name);
    //createCookie("fileName", file.name, 1);
    hubEngine.server.hubstart($.connection.hub.id).done(function (flag) {
        if (filecount < flag) {
            $.ajax({
                url: 'FileUploader/FileUploader.ashx', //+ '&Extention=' + file.name.substr(file.name.lastIndexOf(".")),//server script to process data
                type: 'POST',
                xhr: function () {
                    myXhr = $.ajaxSettings.xhr();
                    if (myXhr.upload) {
                        // $("#divProgress").attr("style", "width:40%");
                        // $("#uploadfilePercentage").text("40%");
                        myXhr.upload.addEventListener('progress', progressHandlingFunction, false);
                        // $("#divProgress").attr("style", "width:50%");
                        //$("#uploadfilePercentage").text("50%");
                    }
                    return myXhr;
                },
                success: function (result) {
                    //On success if you want to perform some tasks.
                    //$("#divProgress").attr("style", "width:100%");
                    //$("#uploadfilePercentage").text("100%");
                    //$("#uploadMsg").text('Upload Done')
                    //$("#divProgress").attr("display:none");
                    //document.getElementById("testmsg").innerHTML = "Uploading done";
                    BeginProcess(result, file.name, 1);
                },
                data: file,
                cache: false,
                contentType: false,
                processData: false
            });
        }
        //else {
        //    //if (window.confirm("Infactum can currently save only 5 Sessions. Please delete at least one session to continue.") == true) {
        //    //    $("#divProgress").width("100%");
        //    //    InvertScreen();
        //    //    $('#ancSessions').click();

        //    //}
        //    var control = $("#fileInput");
        //    control.replaceWith(control = control.clone(true));
        //    $("#mySessionAlertModal").modal("show");

        //}



        function progressHandlingFunction(e) {
            if (e.lengthComputable) {
                var s = parseInt((e.loaded / e.total) * 100);
                //$("#progress" + currFile).text(s + "%");
                //$("#divProgress").width(s + "%");
                //$("#uploadfilePercentage").text(s + "%");
                if (s == 100) {
                    triggerNextFileUpload();
                }
            }
        }
    });
}

function triggerNextFileUpload() {
    if (currFile < totalFileCount - 1) {
        currFile = currFile + 1;
        sendFile($("#avatarInput")[0].files[currFile]);
    }
    else {
        $("#avatarInput").replaceWith($("#avatarInput").clone());
    }
}


function BeginProcess(fileName, orgFileName, isFrom) {
    if (orgFileName != "") {
        var now = new Date();
        now = now.getTime();
        hubEngine.server.imgLocationSave(now + orgFileName, fileName, $.connection.hub.id, "Store", "").done(function (flag) {
            if (flag == "1") {
                var obj = jQuery.parseJSON(_cropJson);
                hubEngine.server.corpimg(Math.round(obj.x), Math.round(obj.y), Math.round(obj.height), Math.round(obj.width), obj.ortate, now + orgFileName, $.connection.hub.id, readCookie("lukpikretailer_usename"));
            }
        });

    }
}

function SaveCropImg() {
    //onchange = "FileSelected();"
    FileSelected();

}


function ChnageImage() {
    //document.getElementById("imgStore").src = tempSrc;// "../StoreImages/1437652439094TestScrn1.jpg";
//    $('#tags_Brands').value = "list,list2";
    //$('#divBrands').empty();
    //var str = '<input id="tags_Brands" type="text" class="tags form-control" value="levis" /><div id="suggestions-container3" style="position: relative; float: left; width: 250px; margin: 10px;"></div>';
    //$('#divBrands').append(str);

    setCheckedValue("isCreditcard", "rbtnCCNo");
}

function AddOtherCategories(all, selected) {
    var str = "";
    $('#divAddOtherCategories').empty();
    for (var i = 0; i < all.length; i++) {
        var checked = "";
        if (CheckExistence(all[i].CategoryID, selected)) {
            checked = "checked";
        }
        str = str + '<div class="col-md-3 col-xs-12 col-sm-12 cssmarginbotom"><label class="cssCheck"><input type="checkbox" name="othercategories" class="flat" data-role="none" value="' + all[i].CategoryID + '" ' + checked + ' /> ' + all[i].CategoryName + '&nbsp;&nbsp;&nbsp;&nbsp;</label></div>';
    }
    $('#divAddOtherCategories').append(str);
  

}

function isInArray(value, array) {
    return array.indexOf(value) > -1;
}

function CheckExistence(currCategoryID, selected) {
    var retVal = false;
    for(var j=0;j<selected.length;j++){
        //currCategoryID == selected[j]*1
        if (currCategoryID.toString() == selected[j]) {
            retVal = true;
            break;
        }
        else
            retVal = false;
       
    }
    return retVal;
    
}

function setCheckedValue(checkboxName, toBeChecked) {
    //$('input[name=' + checkboxName + ']').prop("checked", true);
    document.getElementById(toBeChecked).checked = true;
    document.getElementById("rbtnCCYes").checked = false;
}
