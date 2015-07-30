var hubEngine;
$(document).ready(function () {
    //Hub Connection and functions
    hubEngine = $.connection.allHubs;
    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        Startup();
        AddProgressBarLoader();
        //alert('success');
        var uname = readCookie("lukpikretailer_usename");
        if (typeof (uname) == typeof (undefined) || uname == null || uname == "")
            location.href = "../login.html";
        else
            hubEngine.server.getProductDetails(uname, "", $.connection.hub.id);
    });
    $.connection.hub.disconnected(function () {
        setTimeout(function () {
            $.connection.hub.start();
        }, 1000); // Re-start connection after 1 seconds
    });
    setInterval(function () {
        hubEngine.server.activecall();
    }, 15000);

    hubEngine.client.productDetails = function (json1, json2) {
        
        if (json1 != "") {
            var obj1 = jQuery.parseJSON(json1);
            var obj2 = jQuery.parseJSON(json2);
            CreateTableData(obj1, obj2);
        }
        else {
            var str = '<tr class="even pointer"> <td class="a-center " colspan="8"> No Products found. Please add your products.</td></tr>';
            $('#tableBody').empty();
            $('#tableBody').append(str);
            RemoveProgressBarLoader();
        }
        
    };

    hubEngine.client.removedProduct = function (result, trID) {
        RemoveProgressBarLoader();
        CloseModal();
        if (result == "0") {
            AddAlert("error", "Deletion failed. Please try again later.");
        }
        else {
            
            AddAlert("", "Successfully deleted from your product list.");
            $('#' + trID).hide();
            hubEngine.server.getProductDetails(readCookie("lukpikretailer_usename"), "", $.connection.hub.id);
        }
    };

    hubEngine.client.changedPassword = function (msg) {
        RemoveProgressBarLoader();
        ChangedPassword(msg);
    };
});

function CreateTableData(myobj, myobj2) {
    var str = "";
    if (myobj.length > 0) {
        for (var prodCount = 0; prodCount < myobj.length; prodCount++) {

            var imagePath = myobj2.ImageUrl[prodCount];
            var imgSrc = imagePath == "NoImage" ? "images/previewnotavailable.png" : imagePath;


            var productname = myobj[prodCount].ProductName;
            
            var brand = myobj[prodCount].BrandName;
            var price = myobj[prodCount].Price;
            var status = myobj[prodCount].IsVisible;
            var isVisible = "Hide";
            if (status == 1)
                isVisible = "Show";
            var trID = "trID" + prodCount;
            var selectID = "select" + prodCount;
            var category = myobj[prodCount].ProductCategoryName + " - " + myobj[prodCount].ProductSubCategoryName;
            str = str + '<tr class="even pointer" id="' + trID + '"> <td class="a-center "> <input type="checkbox" class="tableflat"></td><td class=" "><img src="' + imgSrc + '" class="img-responsive" width="80"/></td><td class=" " width="45%">' + productname + ' </td><td class=" "  width="20%"> ' + category + ' </td><td class="a-right a-right ">' + isVisible + '</td><td class=" last"> <select name="action" id="' + selectID + '" onchange="ChooseAction(this,\'' + myobj[prodCount].ProductID + '\',' + trID + ');"><option value="0">Choose </option><option value="2">Delete</option></select></td></tr>';
            //<option value="1">View / Edit</option><option value="3">See info</option>
            //<td class=" ">' + brand + '</td><td class=" ">' + price + '</td>
            // <a onclick="EditProduct(\'' + myobj[prodCount].ProductID + '\');">View/Edit</a>

            //<td class="a-right a-right "><span class="fa fa-remove" style="cursor:pointer;" onclick="RemoveProduct(\'' + myobj[prodCount].ProductID + '\',' + trID + ');"></span></td>
        }
    }
    else {
        str = str + '<tr class="even pointer"> <td class="a-center " colspan="8"> No Products found. Please add your products.</td></tr>';
    }

    $('#tableBody').empty();
    $('#tableBody').append(str);
    RemoveProgressBarLoader();
}

function ChooseAction(control, productID, trID) {
    //0 - Do nothing
    //1 - View/Edit
    //2 - Delete modal
    var e = document.getElementById(control.id);
    var strUser = e.options[e.selectedIndex].value;
    if (strUser == "1") {
        EditProduct(productID);
    }
    else if (strUser == "2") {
        DeleteAlertModal(productID, trID);
    }
}

function EditProduct(productID) {
    createCookie("productID", productID);
    location.href = "addproduct.html";
}

function DeleteAlertModal(productID, rID) {
    rID = rID.id;
    $('#modalDeleteAlertFooter').empty();
    $('#modalDeleteAlert').modal('show');
    var modalFooter = '<button type="button" class="btn btn-default" onclick="CloseModal();">Cancel</button><button type="button" class="btn btn-danger" onclick="RemoveProduct(\'' + productID + '\',\'' + rID + '\');">Delete</button>';
    $('#modalDeleteAlertFooter').append(modalFooter);
}
function RemoveProduct(productID, rID) {
    
    AddProgressBarLoader();
    hubEngine.server.removeProduct(productID, rID, $.connection.hub.id);

}

function CloseModal() {
    $('#modalDeleteAlert').modal('hide');
    $('[name=action] option').filter(function () {
        return ($(this).val() == "0");
    }).prop('selected', true);
}