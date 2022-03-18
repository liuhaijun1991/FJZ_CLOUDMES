var LabelList = [];
var Skuno = "";
var ListName = "";
var DataList = [];
var filename = "";
var UseType = "";
var Bas64File = null;
SubTable = {};

var GetPreview = function (LabelName) {
    layer.load(2, { shade: [0.8, '#393D49'] });
    parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabFilePreview", { NAME: LabelName }, function (e) {
        layer.closeAll('loading');
        if (e.Status == "Pass") {
            $("#uImg").attr('src', '../../' + e.Data + "?ver=" + Date.now());
            setTimeout(function (data) {
                var w = $("#uImg").width();
                var h = $("#uImg").height();
                layer.open({
                    type: 1,
                    shadeClose: true,
                    area: [((w / 2) + 60) + 'px', ((h / 2) + 60)+'px'],
                    offset: "5vh",
                    shade: [0.8, '#393D49'],
                    title: false,
                    content: '<div style="background-image:url(../../img/Imgboxbg.jpg);background-repeat:repeat"><img style="margin:30px;width:' + (w / 2) + 'px;height:' + (h / 2) + 'px;" src="../../' + data + '" /></div>'
                });
            }, 100, e.Data);
        } else {
            layer.msg(e.Message, {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });
};

window.operateEvents = {
    'click .preview': function (e, value, row, index) {
        GetPreview(row["LABELNAME"]);
        return false;
    }
};

function operateFormatter(value, row, index) {
    if (row["R_FILE_NAME"].toUpperCase().endsWith(".LAB")) {
        return [
            '<button class="btn btn-primary preview">',
            'Preview',
            '</button>'
        ].join('')
    } else {
        return [
            '<p title="Only the file that expands the name is \'lab\' can be previewed">----</p>'
        ].join('')

    }
};

$(document).ready(function () {

    $("#DownloadLabel").attr("disabled", "disabled");
    var reset = function () {
        Bas64File = null;
        $("#LabelFileUpload").val("");
    };

    var Get_data = function () {
        parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabelList", {}, function (e) {
            if (e.Status == "Pass") {
                LabelList = e.Data;
                $('#Label_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                    $("#LabelFileUpload").val("");
                });
            }
        });
    }

    var Get_File = function () {
        parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetFileList", {}, function (e) {
            if (e.Status == "Pass") {
                LabelList = e.Data;
                $('#Label_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                    $("#LabelFileUpload").val("");
                });
            }
        });
    }

    var b64toBlob = function (b64Data, sliceSize) {
        sliceSize = sliceSize || 512;
        var byteCharacters = atob(b64Data);
        var byteArrays = [];
        for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
            var slice = byteCharacters.slice(offset, offset + sliceSize);
            var byteNumbers = new Array(slice.length);
            for (var i = 0; i < slice.length; i++) {
                byteNumbers[i] = slice.charCodeAt(i);
            }
            var byteArray = new Uint8Array(byteNumbers);
            byteArrays.push(byteArray);
        }
        var blob = new Blob(byteArrays);
        return blob;
    };

    var ShowNewLabel = {
        id: "ShowNewLabel",
        type: 1,
        shade: 0.8,
        shadeClose: false,
        title: "New Label",
        area: ['600px', '400px'],
        content: $("#NewLabelPanel"),
        btn: ["Upload", "cancel"],
        success: function (layero, index) {
            layer.closeAll('loading');
            $("#NewLabelPanel").removeClass("hidden");
        },
        end: function () {
            $("#NewLabelPanel").addClass("hidden");
        },
        yes: function (index) {
            if ($("#txt_LabelName").val() == "") {
                layer.msg("Label Name Can't be null", {
                    icon: 2,
                    time: 2000
                }, function () {
                    $("#txt_LabelName").focus();
                });
                return false;
            }
            if ($("#txt_ArryLength").val() == "") {
                layer.msg("Label Max Length Can't be null", {
                    icon: 2,
                    time: 2000
                }, function () {
                    $("#txt_ArryLength").focus();
                });
                return false;
            } else {
                var reg = new RegExp("^[0-9]*[1-9][0-9]*$");
                if (!reg.test($("#txt_ArryLength").val())) {
                    layer.msg("Label Max Length is an invalid number", {
                        icon: 2,
                        time: 2000
                    }, function () {
                        $("#txt_ArryLength").focus();
                    });
                    return false;
                }
            }
            layer.load(3);
            parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "UpLoadLabelFile", { Name: $("#txt_LabelName").val(), FileName: filename, MD5: "", UseType: "LABEL", LabelName: $("#txt_LabelName").val(), PrintType: $("#txt_PrintType").val(), ArryLength: $("#txt_ArryLength").val(), Bas64File: Bas64File }, function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    layer.close(index);
                    layer.msg("Success", {
                        icon: 1,
                        time: 3000
                    }, function () {
                        $("#LabelFileUpload").val("");
                        Get_data();
                    });
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            });
        },
        cancel: function (index) {
            $("#LabelFileUpload").val("");
            layer.close(index);
        }
    };

    $('#DeleteLabel').on('click', function () {
        var selRows = $('#Label_List_Table').bootstrapTable('getSelections');
        if (selRows.length > 0) {
            layer.open({
                title: 'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
                yes: function (index) {
                    layer.close(index);
                    var Names = [];
                    for (var i = 0; i < selRows.length; i++) {
                        Names.push(selRows[i].LISTNAME);
                    }
                    parent.client.CallFunction("MESStation.KeyPart.KPScan", "RemoveKPList", { ListNames: Names }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("Success", {
                                icon: 1,
                                time: 3000
                            }, function () {
                                Get_data();
                            });
                        }
                        else {
                            layer.msg(e.Message, {
                                icon: 2,
                                time: 3000
                            }, function () {
                            });
                        }
                    });
                }
            });
        } else {
            layer.msg("no records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });

    $('#LabelFileUpload').change(function (e) {
        var w = new Worker("../../Scripts/Setting/BigFileReader.js");
        w.onmessage = function (e) {
            layer.closeAll("loading");
            if (e.data.Status == "Pass") {
                Bas64File = e.data.Bas64File;
                layer.open(ShowNewLabel);
            } else {
                $('#LabelFileUpload').val("");
                layer.msg(e.data.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        };
        w.onerror = function () {
            $('#LabelFileUpload').val("");
            layer.msg("Worker Error!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
        layer.load(3);
        filename = $(this).val();
        filename = filename.substring(filename.lastIndexOf("\\") + 1);
        UseType = filename.substring(filename.lastIndexOf(".") + 1).toUpperCase();
        w.postMessage({ file: e.target.files[0], filename: filename, UseType: UseType, ExtName: ".LAB,.TXT,.XLS,.XLSX" });
    });


    $('#Label_List_Table').bootstrapTable({
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        singleSelect: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#Table_Toolbar",
        onCheck: function (row) {
            var rows = $('#Label_List_Table').bootstrapTable('getSelections');
            if (rows.length == 1) {
                $("#DownloadLabel").removeAttr("disabled");
            } else {
                $("#DownloadLabel").attr("disabled", "disabled");
            }
        },
        onUncheck: function (row) {
            var rows = $('#Label_List_Table').bootstrapTable('getSelections');
            $("#DownloadLabel").attr("disabled", "disabled");
        },
        onRefresh: function (param) {
            Get_data();
        },
        columns: [{
            checkbox: true
        }, {
            field: 'LABELNAME',
            title: 'Label Name'
        }, {
            field: 'R_FILE_NAME',
            title: 'File Name'
        }, {
            field: 'PRINTTYPE',
            title: 'Use Printer Index'
        }, {
            field: 'ARRYLENGTH',
            title: 'Max Length'
        }, {
            field: 'EDIT_TIME',
            title: 'Edit Time'
        }, {
            field: 'EDIT_EMP',
            title: 'Edit By'
        }, {
            field: 'BB',
            title: 'Operation',
            events: window.operateEvents,
            formatter: operateFormatter
        }],
        data: LabelList
    });

    $("#DownloadLabel").on("click", function () {
        var rows = $('#Label_List_Table').bootstrapTable('getSelections');
        if (rows.length != 1) {
            layer.msg("Please select one record", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }

        self.parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "FileDownLoadByBrowser", { "ID": rows[0].ID }, function (e) {
            if (e.Status == "Pass") {
                var blob = b64toBlob(e.Data.Content);
                if (window.navigator.msSaveOrOpenBlob) {
                    navigator.msSaveBlob(blob, e.Data.FileName);
                } else {
                    var link = document.createElement('a');
                    link.href = window.URL.createObjectURL(blob);
                    link.download = e.Data.FileName;
                    link.click();
                    window.URL.revokeObjectURL(link.href);
                }
            }
            else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () { });
                return;
            }
        });
    });
    $("#DeleteLabel").on("click", function () {
        var rows = $('#Label_List_Table').bootstrapTable('getSelections');
        if (rows.length != 1) {
            layer.msg("Please select one record", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }

        self.parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "FilDelete", { "ID": rows[0].ID }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("Success", {
                    icon: 1,
                    time: 3000
                }, function () {
                 Get_data();
                });
            }
            else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    });

    Get_data();


});