//var $ = jQuery = {};
//importScripts('../../Scripts/plugins/jquery-base64/jquery.base64.js');
var filename;
var file;
var reader;
var UseType;
var ExtName = "";
var BigFileReader = function () {
    if (ExtName.toUpperCase().indexOf(UseType) >= 0 || ExtName == "") {
        reader = new FileReader();
        reader.onload = function (e) {
            try {
                postMessage({ Status: "Pass", Bas64File: this.result, Message: "" });
            } catch (ex) {
                postMessage({ Status: "Fail", Bas64File: "", Message: ex.message });
            }
        }
        reader.readAsDataURL(file);
    }
    else {
        postMessage({ Status: "Fail", Bas64File: "", Message: "Please select lab file with " + ExtName + " formats" });
    }
}

onmessage = function (event) {
    file = event.data.file;
    filename = event.data.filename;
    ExtName = event.data.ExtName;
    UseType = event.data.UseType;
    BigFileReader();
}
