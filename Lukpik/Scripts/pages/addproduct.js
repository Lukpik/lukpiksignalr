
var hubEngine;
var _cnt = 0;
var _isLimitReached = false;
$(document).ready(function () {
    AddProgressBarLoader();
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
        if (readCookie("isDuplicate") != null && readCookie("isDuplicate") != "") {
            AddProgressBarLoader();
            hubEngine.server.getProductDetails(readCookie("lukpikretailer_usename"), productID, "true", $.connection.hub.id);
        }
        
            //createCookie("isDuplicate", "Duplicate");
        else if (productID != null && productID != "") {

            AddProgressBarLoader();
            hubEngine.server.getProductDetails(readCookie("lukpikretailer_usename"), productID, "false", $.connection.hub.id);
        }
        else {  
            RemoveProgressBarLoader();
            hubEngine.server.getStoreMasterPassword(readCookie("lukpikretailer_usename"), $.connection.hub.id);
        }
        hubEngine.server.isLimitReached(readCookie("lukpikretailer_usename"), $.connection.hub.id);
        
        
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 1000); // Re-start connection after 1 seconds
    });
    setInterval(function () {
        hubEngine.server.activecall();
    }, 15000);
    
    hubEngine.client.setMasterAck = function (msg,pwd) {
        if (msg == "1") {
            $('#txtProductMasterPassword').val(pwd);
        }
        else {
            
        }
    };

    hubEngine.client.fillCollectionColorSizes = function (msg,colors,tags,sizes) {
        if (msg != "") {
            $('#txtColors').val(colors);
            $('#txtSizes').val(sizes);
            $('#tags_Collection').val(tags);
        }

    };
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

    hubEngine.client.limitReached = function (msg, limit) {
        // 1- Limit reached
        // 2- about to reach - nearby
        // 3- not reached limit
        if (msg != "0") {
            if (msg == "1") {
                _isLimitReached = true;
                $('#lblLimitReached').html("Sorry, you have reached the product limit ( " + limit + " ) for free plan. Please contact support for any assistance.");
                $('#modalLimitReached').modal('show');
                //$('#divButtonPanel_Top').hide();
                //$('#divButtonPanel_Bottom').hide();
                $('#btnSaveTop').hide();
                $('#btnSaveBottom').hide();

            }
            else if (msg == "2") {
                _isLimitReached = false;
                $('#lblLimitReached').html("Hey, you have almost reached the limit(" + limit + "), you can add only one more product.");
                $('#modalLimitReached').modal('show');
                $('#btnViewProducts').text("Add Product");
                $('#btnViewProducts').text("Add Product");
            }
            else {
                _isLimitReached = false;
            }
        }

    };

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
    hubEngine.client.productUpdated = function (msg) {
        //praveen
        RemoveProgressBarLoader();
        //ProductImageNames = [];
        //ProductImages = [];
        if (msg == "1") {
            //location.href = "addproduct.html";
            AddAlert("", "Updated successfully.");
            //$('#modalProductAdded').modal('show');
            $('#modalProductUpdated').modal('show');
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

    

    hubEngine.client.productDetails = function (json1, json2,json3) {
        RemoveProgressBarLoader();
        if (json1 != "") {
            var obj1 = jQuery.parseJSON(json1);
            var obj2 = "";
            var obj3 = "";
            if (json2 != "") {
                obj2 = jQuery.parseJSON(json2);
            }
            if(json3!="")
                obj3 = jQuery.parseJSON(json3);
            if ((readCookie("isDuplicate") != null && readCookie("isDuplicate") != "")) {
                $('#lblHeading').text("Duplicate Product");
                $('#btnSaveTop').text("Save");
                $('#btnSaveBottom').text("Save");
            }
            else if ((readCookie("productID") != null && readCookie("productID") != "")) {
                $('#lblHeading').text("Edit Product");
                $('#btnSaveTop').text("Update");
                $('#btnSaveBottom').text("Update");
            }
            FillValues(obj1, obj2,obj3);
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

    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    $('#alertID').empty();
    AddProgressBarLoader();
    var productSKU = $('#txtProductSKU').val();
    var productname = $('#txtFullName').val();
    var gender = $('input[name=gender]:checked').val();
    var productFamilyID = document.getElementById("ddProductFamily").value;
    var price = $('#txtPrice').val();
    var productDesc = $('#txtProductDescription').val();
    var visibility = $('input[name=isvisibe]:checked').val();
    var masterPwd = $('#txtProductMasterPassword').val();
    if (visibility != "3")
        masterPwd = "";
    var productCategorySubCategoryID = document.getElementById("selectProduct").value;
    var brandID = document.getElementById("ddBrands").value;
    //var brandID = $('#txtBrand').val();
    var tags = $('#tags_Collection').val();
    var sizes = $('#txtSizes').val();
    var colors = $('#txtColors').val();
    var ecommercelink = $('#txtECommerceLink').val();

  //var ProductImages = []; var ProductImageNames = [];
  //if (file5 != null && file5 != "")
  //{
  //    ProductImageNames.push(file5Name); ProductImages.push(file5);
  //}
  //if (file6 != null && file6 != "") {
  //    ProductImageNames.push(file6Name); ProductImages.push(file6);
  //}
    if (productname != "" && gender != "" && productFamilyID != "" && productCategorySubCategoryID != "") {
      // productname,  gender,  productFamily,  productdescription, price,  quantity,  size,  color,  visibility, prodyctype,  brand, collection,  images, clientID
      var productID = readCookie("productID");
      var isDuplicate = readCookie("isDuplicate");
      
      //if (ProductImages.length > 0) {
      if (isDuplicate != null && isDuplicate != "") {
          GetAllImages('add');
      }
      else if (productID != null && productID != "") {
          if (price == "")
              price = "-1";
          //hubEngine.server.addProduct(productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink);
          //Main
          //hubEngine.server.updateProduct(productID, productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, "", readCookie("lukpikretailer_usename"), $.connection.hub.id, "", ecommercelink);
          GetAllImages('update');

      }
      else {
          //if (ProductImages.length > 0) {
          //    hubEngine.server.addProduct(productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink);
          //}
          //else {
          //    RemoveProgressBarLoader();
          //    AddAlert("error", "Please add image(s) to proceed.");
          //}
          GetAllImages('add');
      }

  }
  else {
      RemoveProgressBarLoader();
      AddAlert("error", "Please fill mandatory fields.");
  }
   
}

function ProductData() {

    $('#alertIDTop').empty();
    $('#alertIDBottom').empty();
    $('#alertID').empty();
    AddProgressBarLoader();
    var productSKU = $('#txtProductSKU').val();
    var productname = $('#txtFullName').val();
    var gender = $('input[name=gender]:checked').val();
    
    var productFamilyID = document.getElementById("ddProductFamily").value;
    var price = $('#txtPrice').val();
    var productDesc = $('#txtProductDescription').val();

    var visibility = $('input[name=isvisibe]:checked').val();
    var masterPwd = $('#txtProductMasterPassword').val();
    if (visibility != "3")
        masterPwd = "";
    var productCategorySubCategoryID = document.getElementById("selectProduct").value;
    var brandID = document.getElementById("ddBrands").value;
    //var brandID = $('#txtBrand').val();
    var tags = $('#tags_Collection').val();
    var sizes = $('#txtSizes').val();
    var colors = $('#txtColors').val();
    var ecommercelink = $('#txtECommerceLink').val();
    var imageIDs = [];
    var ProductImages = []; var ProductImageNames = [];
    if (file5 != null && file5 != "") {
        ProductImageNames.push(file5Name); ProductImages.push(file5);
        imageIDs.push("image1");
    }
    if (file6 != null && file6 != "") {
        ProductImageNames.push(file6Name); ProductImages.push(file6);
        imageIDs.push("image2");
    }
    
    if (productname != "" && gender != "" && productFamilyID != "" && productCategorySubCategoryID != "") {
        var productID = readCookie("productID");
        var isDuplicate = readCookie("isDuplicate");
        if (isDuplicate != null && isDuplicate != "") {
            if (price == "")
                price = "-1";
            if (ProductImages.length > 0)
                hubEngine.server.addProduct(productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink, productSKU, masterPwd);
            else {
                RemoveProgressBarLoader();
                AddAlert("error", "Please add image(s) to proceed.");
            }
        }
        else if ((productID != null && productID != "") && (ProductImages.length > 0 || $('#divImgPreview')[0].childElementCount > 0)) {
            if (price == "")
                price = "-1";
            hubEngine.server.updateProduct(productID, productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink, imageIDs.toString(), productSKU, masterPwd);

        }
        else if (ProductImages.length > 0 && productID == null) {
            if (price == "")
                price = "-1";

            hubEngine.server.addProduct(productname, gender, productFamilyID, productDesc, price, "", sizes, colors, visibility, productCategorySubCategoryID, brandID, tags, ProductImages.toString(), readCookie("lukpikretailer_usename"), $.connection.hub.id, ProductImageNames.toString(), ecommercelink, productSKU, masterPwd);
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

function GetAllImages(pos) {
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
    else if (pos == 'update') {
        if ($('#divImgPreview')[0].childElementCount == 0) {
            RemoveProgressBarLoader();
            AddAlert("error", "Please add image(s) to proceed.");
        } else {
            ProductData();
        }

    }
    else {
        RemoveProgressBarLoader();
        AddAlert("error", "Please add image(s) to proceed.");
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
                        ProductData();
                        
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

function FillValues(myobj1, myobj2,myobj3) {
    var productID = readCookie("productID");
    $('#divImgPreview').show();
    if (readCookie("isDuplicate") != null && readCookie("isDuplicate") != "") {
        $('#divImgPreview').hide();
    }
    
    $('fileUpdloadDiv').hide();
    $('#txtProductMasterPassword').val(myobj1[0].ProductMasterPassword);
    $('#txtProductSKU').val(myobj1[0].ProductSKU);
    $('#txtFullName').val(myobj1[0].ProductName);
    if (myobj1[0].Price != "-1")
        $('#txtPrice').val(myobj1[0].Price);
    $('#txtProductDescription').val(myobj1[0].ProductLongDescription);
    $('#txtECommerceLink').val(myobj1[0].ECommerceLink);
    
    $("input[name=gender][value=" + (myobj1[0].Gender)  + "]").prop("checked", true);

    $('[name=productfamily] option').filter(function () {
        return ($(this).text() == myobj1[0].ProductFamilyName);
    }).prop('selected', true);
    $("input[name=isvisibe][value=" + (myobj1[0].IsVisible) * 1 + "]").prop("checked", true);
    if (myobj1[0].IsVisible == "3")
        $('#txtProductMasterPassword').prop("disabled", false);
    else
        hubEngine.server.getStoreMasterPassword(readCookie("lukpikretailer_usename"), $.connection.hub.id);
    $('[name=producttype] option').filter(function () {
        return ($(this).val() == myobj1[0].ProductTypeName);
    }).prop('selected', true);

    $('[name=brands] option').filter(function () {
        return ($(this).text() == myobj1[0].BrandName);
    }).prop('selected', true);
    _selectedProductCategory = myobj1[0].ProductSubCategoryID;
    onChangeProductFamily();
    if (myobj3 != "")
        addImage(myobj3);
    
    hubEngine.server.getCollectionColorSizes(productID, readCookie("lukpikretailer_usename"), $.connection.hub.id);
}

var cnt = 1;
var _selectedProductCategory = "";
function addImage(imageObj) {
    
    if (imageObj.ImageUrl.length > 0) {

        for (var i = 0; i < imageObj.ImageUrl.length; i++) {
            var imagePath = imageObj.ImageUrl[i];
            if (imagePath != "NoImage") {
                var imgSrc = imagePath == "NoImage" ? "images/previewnotavailable.png" : "../StoreImages/"+imagePath;
                
                var divId = "div" + cnt;
                var str = '<div class="col-md-3" id="' + divId + '" style="text-align:center;"><div class="cssWell"><div style="text-align:right;padding-bottom:5px;"><span style="cursor:pointer;text-decoration:none;font-weight:bold;" onclick="RemoveImage(\'' + divId + '\',\'' + i + '\');">X</span></div><img src="' + imgSrc + '" class="img-responsive" /></div></div>';
                switch (i) {
                    case 0:
                        $('#file-5').fileinput("disable");
                        break;
                    case 1:
                        $('#file-6').fileinput("disable");
                        break;
                }
                //$('#blah').attr('src', e.target.result);
                $('#divImgPreview').append(str);
                cnt++;
            }

        }

    }
}

function RemoveImage(id,index) {
    $('#' + id).empty();
    $('#' + id).remove();
    var fileInput = "";
    switch (index) {
        case "0":
            fileInput = "file-5";
            break;
        case "1":
            fileInput = "file-6";
            break;
    }
    $('#' + fileInput).fileinput("enable");
    
    //$('#' + fileInput).prop("disabled", false);
    //document.getElementById(fileInput).disabled = false;
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
});

$("input[name=isvisibe]:radio").change(function () {
    var isVisible = $('input[name=isvisibe]:checked').val();
    if (isVisible == "3") {
        $('#txtProductMasterPassword').prop("disabled", false);
    }
    else {
        $('#txtProductMasterPassword').prop("disabled", true);
        //hubEngine.server.getStoreMasterPassword(readCookie("lukpikretailer_usename"), $.connection.hub.id);
    }
});

function FormProductCategory(myobj) {
    var str = '';
    $('#selectProduct').empty();
    for (var parent = 0; parent < myobj.length; parent++) {
        var parentText = myobj[parent].P;
        
        str = str + '<optgroup label="' + parentText + '">';
        for (var child = 0; child < myobj[parent].c.length; child++) {
            var selected = "";
            var childText = myobj[parent].c[child].split("_")[1];
            var subCategoryID = myobj[parent].c[child].split("_")[0];
            var isNotNull = _selectedProductCategory != "" ? true : false;
            
            var productID = readCookie("productID");
            if (productID == null || productID == "") {
                if (parent == 0 && child == 0) {
                    selected = 'selected';
                }
            }
            else {
                if (_selectedProductCategory * 1 == subCategoryID * 1 && isNotNull) {
                    selected = 'selected';
                }
            }

            
            
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
    eraseCookie("productID");
    eraseCookie("isDuplicate");
    location.href = "addproduct.html";
}

function NavigatetoProducts() {
    if (_isLimitReached)
        location.href = "products.html";
    else
        $('#modalLimitReached').modal('hide');
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