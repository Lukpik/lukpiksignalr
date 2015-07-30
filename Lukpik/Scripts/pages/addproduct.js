
var hubEngine;
var _cnt = 0;
$(document).ready(function () {
    AddSpecification();
    Startup();
    
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        //alert('success');
        $('#divImgPreview').hide();
        hubEngine.server.getProductFamily($.connection.hub.id);
        //hubEngine.server.getProductTypes($.connection.hub.id);
        hubEngine.server.getAllBrands($.connection.hub.id);
        var productID=readCookie("productID");
        if (productID != null && productID != "") {
            AddProgressBarLoader();
            hubEngine.server.getProductDetails(readCookie("lukpikretailer_usename"), productID, $.connection.hub.id);
        }
        
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
    hubEngine.client.gotProductFamily = function (msg) {
        if (msg != "") {
            var ddObj = jQuery.parseJSON(msg);
            var strProductFamily = "";
            if (ddObj.length > 0) {
                for (var i = 0; i < ddObj.length; i++) {
                    strProductFamily = strProductFamily + "<option value='" + ddObj[i].ProductFamilyID + "'>" + ddObj[i].Name + "</option>";
                }
            }
            $('#ddProductFamily').empty();
            $('#ddProductFamily').append(strProductFamily);
        }
        onChangeProductFamily();

    };

    //hubEngine.client.gotProductTypes = function (msg) {
    //    if (msg != "") {
    //        var ddObj = jQuery.parseJSON(msg);
    //        var strProductType = "";
    //        if (ddObj.length > 0) {
    //            for (var i = 0; i < ddObj.length; i++) {
    //                strProductType = strProductType + "<option value='" + ddObj[i].ProductTypeID + "'>" + ddObj[i].Name + "</option>";
    //            }
    //        }
    //        $('#ddProducttype').empty();
    //        $('#ddProducttype').append(strProductType);
    //    }
    //};

    hubEngine.client.gotAllBrands = function (msg) {
        if (msg != "") {
            var ddObj = jQuery.parseJSON(msg);
            var strBrand = "";
            if (ddObj.length > 0) {
                for (var i = 0; i < ddObj.length; i++) {
                    strBrand = strBrand + "<option value='" + ddObj[i].BrandID + "'>" + ddObj[i].brandname + "</option>";
                }
            }
            $('#ddBrands').empty();
            $('#ddBrands').append(strBrand);
        }

    };

    hubEngine.client.addedProduct = function (msg) {
        //praveen
        RemoveProgressBarLoader();
        //ProductImageNames = [];
        //ProductImages = [];
        if (msg == "1") {
            //location.href = "addproduct.html";
            AddAlert("", "Updated successfully.");
            $('#modalProductAdded').modal('show');
        }
        else if (msg == "0")
            AddAlert("error", "Something went wrong, please try again later.");
    };

    hubEngine.client.changedOptions = function (msg) {
        //praveen
        if (msg != "") {
            var ddObj = jQuery.parseJSON(msg);
            //onChangeProductFamily();
            FormProductCategory(ddObj);
        }
    };

    

    hubEngine.client.productDetails = function (json1, json2) {
        RemoveProgressBarLoader();
        if (json1 != "") {
            var obj1 = jQuery.parseJSON(json1);
            var obj2 = jQuery.parseJSON(json2);
            $('#lblHeading').text("Edit Product");
            $('#btnSaveTop').text("Update");
            $('#btnSaveBottom').text("Update");
            FillValues(obj1, obj2);
        }
        else {
            AddAlert("error", "Something went wrong, please try again later.");
        }
    };

    hubEngine.client.changedPassword = function (msg) {
        RemoveProgressBarLoader();
        ChangedPassword(msg);
    };

});

function Login() {

}

function AddProduct() {
    //FileSelected();

    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    $('#alertID').empty();
    AddProgressBarLoader();
    var productname = $('#txtFullName').val();
    var gender = $('input[name=gender]:checked').val();
    var productFamilyID = document.getElementById("ddProductFamily").value;
    var price = $('#txtPrice').val();
    var productDesc = $('#txtProductDescription').val();
    var visibility = $('input[name=isvisibe]:checked').val();
    var productCategorySubCategoryID = document.getElementById("selectProduct").value;
    var brandID = document.getElementById("ddBrands").value;
    //var brandID = $('#txtBrand').val();
    var tags = $('#tags_Collection').val();
    var sizes = $('#txtSizes').val();
    var colors = $('#txtColors').val();
    var ecommercelink = $('#txtECommerceLink').val();

  var ProductImages = []; var ProductImageNames = [];
  if (file5 != null && file5 != "")
  {
      ProductImageNames.push(file5Name); ProductImages.push(file5);
  }
  if (file6 != null && file6 != "") {
      ProductImageNames.push(file6Name); ProductImages.push(file6);
  }
    if (productname != "" && gender != "" && productFamilyID != "" && price != "" && productCategorySubCategoryID != "" && brandID != "") {
        // productname,  gender,  productFamily,  productdescription, price,  quantity,  size,  color,  visibility, prodyctype,  brand, collection,  images, clientID
        var productID = readCookie("productID");
        if (ProductImages.length > 0) {
            if (productID != null && productID != "") {
                hubEngine.server.updateProduct(productID, productname, gender, productFamilyID, productDesc, price, "", "", "", visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink);
            }
            else {
                hubEngine.server.addProduct(productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink);
                //AddProduct();
            }
        }
        else {
            RemoveProgressBarLoader();
            AddAlert("error", "Please add image(s) to proceed.");
        }
    }
    else {
        RemoveProgressBarLoader();
        AddAlert("error", "Please fill mandatory fields.");
    }
   
}

function GetAllImages(id) {
    //$('#file-5').fileinput('clear');
    //ProductImages = []; ProductImageNames = [];
    var j = 1;
    var totalFiles = []; var totalFilesID = [];
    if ($("#file-5")[0].files.length != 0) {
        totalFiles.push($("#file-5")[0].files[0]);
        totalFilesID.push("file-5");
    }
    if ($("#file-6")[0].files.length != 0) {
        totalFiles.push($("#file-6")[0].files[0]);
        totalFilesID.push("file-6");
    }
    if (totalFiles.length != 0) {
        for (var i = 0; i < totalFiles.length; i++) {
            currFile = i;
            var sFileName = totalFiles[i].name;
            var sFileSize = totalFiles[i].size;
            var sFileExtension = sFileName.split('.')[sFileName.split('.').length - 1].toLowerCase();
            if (sFileExtension == 'jpg' || sFileExtension == 'jpeg' || sFileExtension == 'png' || sFileExtension == 'gif') {
                //if (sFileSize < 10000000) {
                //$("#divProgress").attr("style", "width:10%");
                //$("#uploadfilePercentage").text("10%");
                totalFileCount = totalFiles.length;
                //$('#dropRegion').attr("style", "display:none;");
                //S('#progressBar').attr("style", "display:block;");
                //$('#progressBarETL').attr('style', 'display:block;');
                //            for (j = 0; j < totalFileCount; j++) {
                //                var progControl = $("#dvProgess").append($("#fileInput")[0].files[j].name + "<div id='progress" + j + "'></div><div class='progress progress-striped' id='ProgressComp" + j + "' style='width: 400px; height: 15px; '> <div class='bar' id='progbarWidth" + j + "' style='width: 0%; height: 15px;'>&nbsp;</div></div>");
                //            }
                //uploadFile();
                sendFile(totalFiles[i], j, totalFiles.length, totalFilesID[i]);
                //}
                //else {
                //     $("#fileInput").val("");
                //     $("#filesizeAlertModal").modal("show");
                // }
                j++;
            }
            else
                j++;
        }
    }
    else {
        AddProduct();
    }

}

//var ProductImages = []; var ProductImageNames = [];
var currFile = 0;
var totalFileCount = 0;
var filecount = 0;
var file5Name, file6Name, file5, file6;
function FileSelected() {
    var j = 1;
    for (var i = 0; i < $("#file-5")[0].files.length; i++) {
        currFile = i;
        var sFileName = $("#file-5")[0].files[i].name;
        var sFileSize = $("#file-5")[0].files[i].size;
        var sFileExtension = sFileName.split('.')[sFileName.split('.').length - 1].toLowerCase();
        if (sFileExtension == 'jpg' || sFileExtension == 'jpeg' || sFileExtension == 'png' || sFileExtension == 'gif') {
            //if (sFileSize < 10000000) {
            //$("#divProgress").attr("style", "width:10%");
            //$("#uploadfilePercentage").text("10%");
            totalFileCount = $("#file-5")[0].files.length;
            //$('#dropRegion').attr("style", "display:none;");
            //S('#progressBar').attr("style", "display:block;");
            //$('#progressBarETL').attr('style', 'display:block;');
            //            for (j = 0; j < totalFileCount; j++) {
            //                var progControl = $("#dvProgess").append($("#fileInput")[0].files[j].name + "<div id='progress" + j + "'></div><div class='progress progress-striped' id='ProgressComp" + j + "' style='width: 400px; height: 15px; '> <div class='bar' id='progbarWidth" + j + "' style='width: 0%; height: 15px;'>&nbsp;</div></div>");
            //            }
            //uploadFile();
            sendFile($("#file-5")[0].files[i], j, $("#file-5")[0].files.length);
            //}
            //else {
            //     $("#fileInput").val("");
            //     $("#filesizeAlertModal").modal("show");
            // }
            j++;
        }
        else
            j++;

    }

}

//File Upload and other function Script
function sendFile(file, j, k,id) {
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
                    var now = new Date();
                    now = now.getTime();


                    if (file.name != "" && file.name != null && result != "" && result != null && id=="file-5") {
                        file5Name = now + file.name; file5 = result;
                        //ProductImageNames.push(now + file.name); ProductImages.push(result);
                    }
                    if (file.name != "" && file.name != null && result != "" && result != null && id == "file-6") {
                        file6Name = now + file.name; file6 = result;
                        //ProductImageNames.push(now + file.name); ProductImages.push(result);
                    }

                    if (k === j)
                        AddProduct();
                        
                        //BeginProcess(ProductImages, ProductImageNames.name, 0);
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
                    triggerNextFileUpload(id);
                }
            }
        }
    });
}

function triggerNextFileUpload(id) {
    if (currFile < totalFileCount - 1) {
        currFile = currFile + 1;
        sendFile($("#"+id)[0].files[currFile]);
    }
    else {
        $("#"+id).replaceWith($("#"+id).clone());
    }
}

function AddSpecification() {
    var divID = "div" + _cnt;
    var str = '<div class="row" id="' + divID + '" style="background-color:#eaeaea;padding-top:10px;margin-bottom:2px;"> <div class="col-md-4 cssmarginbotom col-sm-12 col-xs-12"> <label class="control-label">Size(s)</label> <div class=""> <input id="tags_Sizes" type="text" class="tags form-control" value="" placeholder="small, medium, large"/> <div id="suggestions-container1" style="position: relative; float: left; width: 250px; margin: 10px;"></div></div></div><div class="col-md-4 cssmarginbotom col-sm-12 col-xs-12"><label class="control-label">Color(s)</label> <div class=""> <input id="tags_Colors" type="text" class="tags form-control" value=""/> <div id="suggestions-container2" style="position: relative; float: left; width: 250px; margin: 10px;"></div></div></div><div class="col-md-3 col-sm-12 col-xs-12">  <label for="">Quantity :</label><input type="text" class="form-control" name="fullname" placeholder="10"/> </div><div class="col-md-1 col-sm-12 col-xs-12"><label class="control-label">Delete</label> <button class="btn btn-danger btn-sm" onclick="RemoveDiv(\'' + divID + '\');"><span class="fa fa-remove"></span></button></div></div>';
    $('#divSpecification').append(str);
    _cnt++;

}

function RemoveDiv(divID) {
    if (_cnt != 1) {
        $('#' + divID).remove();
        _cnt--;
    }

}

function RemoveSpecification() {
    $('#divSpecification').empty();
    AddSpecification();
    _cnt = 1;
}

function FillValues(myobj1, myobj2) {
    $('#divImgPreview').show();
    $('fileUpdloadDiv').hide();

    $('#txtFullName').val(myobj1[0].ProductName);
    $('#txtPrice').val(myobj1[0].Price);
    $('#txtProductDescription').val(myobj1[0].ProductLongDescription);
    $('#txtECommerceLink').val(myobj1[0].ECommerceLink);

    $("input[name=gender][value=" + (myobj1[0].Gender)  + "]").prop("checked", true);

    $("input[name=isvisibe][value=" + (myobj1[0].IsVisible) * 1 + "]").prop("checked", true);

    $('[name=producttype] option').filter(function () {
        return ($(this).val() == myobj1[0].ProductTypeName);
    }).prop('selected', true);

    $('[name=brands] option').filter(function () {
        return ($(this).val() == myobj1[0].BrandName);
    }).prop('selected', true);

    $('[name=productfamily] option').filter(function () {
        return ($(this).val() == myobj1[0].ProductFamilyName);
    }).prop('selected', true);
    addImage(myobj2);
}

var cnt = 1;
function addImage(imageObj) {
    if (imageObj.ImageUrl.length > 0) {

        for (var i = 0; i < imageObj.ImageUrl.length; i++) {
            var imagePath = imageObj.ImageUrl[i];
            if (imagePath != "NoImage") {
                var imgSrc = imagePath == "NoImage" ? "images/previewnotavailable.png" : imagePath;

                var divId = "div" + cnt;
                var str = '<div class="col-md-3" id="' + divId + '" ><div class="well"><div style="text-align:center;"><span style="cursor:pointer;text-decoration:none;" onclick="RemoveImage(\'' + divId + '\');">X</span></div><img src="' + imgSrc + '" class="img-responsive"/></div></div>';
                //$('#blah').attr('src', e.target.result);
                $('#divImgPreview').append(str);
                cnt++;
            }

        }

    }
}
function RemoveImage(id) {
    $('#' + id).empty();
    $('#' + id).remove();
}

function Test() {
    var str = ' <optgroup label="Alaskan/Hawaiian Time Zone"> <option value="AK">Alaska</option> <option value="HI">Hawaii</option> </optgroup> <optgroup label="Pacific Time Zone"> <option value="CA">California</option> <option value="NV">Nevada</option> <option value="OR">Oregon</option> <option value="WA">Washington</option> </optgroup> <optgroup label="Mountain Time Zone"> <option value="AZ">Arizona</option> <option value="CO">Colorado</option> <option value="ID">Idaho</option> <option value="MT">Montana</option> <option value="NE">Nebraska</option> <option value="NM">New Mexico</option> <option value="ND">North Dakota</option> <option value="UT">Utah</option> <option value="WY">Wyoming</option> </optgroup> <optgroup label="Central Time Zone"> <option value="AL">Alabama</option> <option value="AR">Arkansas</option> <option value="IL">Illinois</option> <option value="IA">Iowa</option> <option value="KS">Kansas</option> <option value="KY">Kentucky</option> <option value="LA">Louisiana</option> <option value="MN">Minnesota</option> <option value="MS">Mississippi</option> <option value="MO">Missouri</option> <option value="OK">Oklahoma</option> <option value="SD">South Dakota</option> <option value="TX">Texas</option> <option value="TN">Tennessee</option> <option value="WI">Wisconsin</option> </optgroup> <optgroup label="Eastern Time Zone"> <option value="CT">Connecticut</option> <option value="DE">Delaware</option> <option value="FL">Florida</option> <option value="GA">Georgia</option> <option value="IN">Indiana</option> <option value="ME">Maine</option> <option value="MD">Maryland</option> <option value="MA">Massachusetts</option> <option value="MI">Michigan</option> <option value="NH">New Hampshire</option> <option value="NJ">New Jersey</option> <option value="NY">New York</option> <option value="NC">North Carolina</option> <option value="OH">Ohio</option> <option value="PA">Pennsylvania</option> <option value="RI">Rhode Island</option> <option value="SC">South Carolina</option> <option value="VT">Vermont</option> <option value="VA">Virginia</option> <option value="WV">West Virginia</option> </optgroup>';
    $('#selectProduct').append(str);
}


function onChangeProductFamily() {
    var gender = $('input[name=gender]:checked').val();
    var productFamilyID = document.getElementById("ddProductFamily").value;
    hubEngine.server.changeOptions(gender, productFamilyID, $.connection.hub.id);

}
$("input[name=gender]:radio").change(function () {
    onChangeProductFamily();
})
function FormProductCategory(myobj) {
    var str = '';
    $('#selectProduct').empty();
    for (var parent = 0; parent < myobj.length; parent++) {
        var parentText = myobj[parent].P;
        
        str = str + '<optgroup label="' + parentText + '">';
        for (var child = 0; child < myobj[parent].c.length; child++) {
            var selected = "";
            if (parent == 0 && child == 0) {
                selected = 'selected';
            }
            var childText = myobj[parent].c[child].split("_")[1];
            var value = myobj[parent].c[child].split("_")[0] + "_" + myobj[parent].c[child].split("_")[2];
            str = str + '<option value="' + value + '" ' + selected + '>' + childText + '</option>';
        }

        str = str + '</optgroup>';
        
    }
    $('#selectProduct').append(str);
    $(".select2_group").select2({});
    
}

function CloseModal() {
    $('#modalProductAdded').modal('hide');
    location.href = "addproduct.html";
}

function isDecimal(evt) {
    var charCode = (evt.which) ? evt.which : event.keyCode

    if (charCode == 46) {
        var inputValue = $("#txtPrice").val()
        if (inputValue.indexOf('.') < 1) {
            return true;
        }
        return false;
    }
    if (charCode != 46 && charCode > 31 && (charCode < 48 || charCode > 57)) {
        return false;
    }
    return true;
}

function ClearFileUpload() {
    //$('#file-5').fileinput('clear');
    //$('#w20').modal('show');

   var t1= $('.file-preview-image').title;
}