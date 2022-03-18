var NewFlag = false;
var SkuListData = [];
var SkuLabelListData = [];
var TPointList = [];
var RouteList = [];
var SkuAqlData = [];
var AqlData = [];
var tableLocale = "";
var SKU_EditRow = null;
var mesUI;
var lan;
var client = null;
var referenceSku = "";
$(document).ready(function () {
    client = self.parent.client;
    mesUI = new MesClientUI(client);
    lan = $.cookie($.MES.CK_LAN_NAME);
    if (lan === "CHINESE") {
        tableLocale = "zh-CN";
    }
    else if (lan === "CHINESE_TW") {
        tableLocale = "zh-TW";
    }
    else {
        tableLocale = "en";
    }

    var FindValueByKey = function (key, data) {
        for (var d in data) {
            if (key.name === d.toUpperCase()) {
                key.value = data[d];
                break;
            }
            else if (Object.prototype.toString.call(data[d]) === '[object Object]') {
                FindValueByKey(key, data[d]);
            }
        }
    };

    var Get_StationList = function (SkuId) {
        client.CallFunction("MESStation.FileUpdate.FileUpload", "GetStationList", { "SkuId": SkuId}, function (e) {
            try {
                $("#LabelList input[name=STATION]").autocomplete("destroy");
            } catch (e) {
                console.error(e);
            }
            if (e.Status == "Pass") {
                var data = [];
                data.push("SN_PRINT");
                for (var i = 0; i < e.Data.length; i++) {
                    data.push(e.Data[i]);
                }
                $("#LabelList input[name=STATION]").autocomplete({
                    minLength: 0,
                    source: data,
                    select: function (event, ui) {
                        $(this).val(ui.item.value);
                    },
                    create: function (event, ui) {
                        $(this).bind("click", function () {
                            var active = $(this).attr("autocomplete");
                            if (active == "off") {
                                $(this).autocomplete("search", "");
                            }
                        });
                    },
                    scroll: true,
                    scrollHeight: 180,
                    position: { my: "right top", at: "right bottom" }
                });
            }
        });
        
    };

    var Get_LabelList = function () {
        client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabelList", {}, function (e) {
            try {
                $("#LabelList input[name=LABELNAME]").autocomplete("destroy");
            } catch (e) {
                console.error(e);
            }
            if (e.Status === "Pass") {
                var data = [];
                for (var i = 0 ; i < e.Data.length; i++) {
                    data.push(e.Data[i].LABELNAME);
                }
                $("#LabelList input[name=LABELNAME]").autocomplete({
                    minLength: 0,
                    source: data,
                    select: function (event, ui) {
                        $(this).val(ui.item.value);
                    },
                    create: function (event, ui) {
                        $(this).bind("click", function () {
                            var active = $(this).attr("autocomplete");
                            if (active == "off") {
                                $(this).autocomplete("search", "");
                            }
                        });
                    },
                    scroll: true,
                    scrollHeight: 180,
                    position: { my: "right top", at: "right bottom" }
                });
            }
        });
    };

    var Get_LabelType = function () {
        client.CallFunction("MESStation.Label.LabelConfig", "GetLabelTypes", {}, function (e) {
            var sel = $("#LabelList select[name=LABELTYPE]");
            sel.empty();
            if (e.Status === "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    sel.append($("<option value='" + e.Data[a].NAME + "'>" + e.Data[a].NAME + "</option>"));
                }
            }
        });
    };

    var Get_SkuList = function () {
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetAllSku", {}, function (e) {
            layer.closeAll('loading');
            if (e.Status === "Pass") {
                ListData = e.Data;
                $('#Sku_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };

    var Get_SkuAllLabelList = function () {
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetAllSkuLabel", {}, function (e) {
            layer.closeAll('loading');
            if (e.Status === "Pass") {
                ListData = e.Data;
                $('#Sku_Label_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };
    
    var Get_LabelConfig = function (Skuno) {
        $("#DeleteLabel").attr("disabled", "disabled");
        client.CallFunction("MESStation.Label.LabelConfig", "GetLabelConfigBySkuno", { Skuno: Skuno }, function (e) {
            if (e.Status === "Pass") {
                $('#LabelListTable').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };

    $('#Sku_List_Table').bootstrapTable({
        data: SkuListData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: false,
        showSelectTitle: true,
        //maintainSelected: false,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#Table_Toolbar",
        onCheck: function (row) {
            var rows = $('#Sku_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditSku").attr("disabled", "disabled");
            } else {
                $("#EditSku").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#DeleteSku").attr("disabled", "disabled");
            } else {
                $("#DeleteSku").removeAttr("disabled");
            }
        },
        onUncheck: function (row) {
            var rows = $('#Sku_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditSku").attr("disabled", "disabled");
            } else {
                $("#EditSku").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#DeleteSku").attr("disabled", "disabled");
            } else {
                $("#DeleteSku").removeAttr("disabled")
            }
        },
        columns: [{
            title: '<label set-lan="html:SELECT">SELECT</label>',
            checkbox: true
        }, {
            field: 'SKUNO',
            title: '<label set-lan="html:SkuNo">SkuNo</label>'
        }, {
            field: 'BU',
            title: '<label set-lan="html:Bu">BU</label>'
        }, {
            field: 'VERSION',
            title: '<label set-lan="html:Version">Version</label>'
        }, {
            field: 'SKU_NAME',
            title: '<label set-lan="html:CodeName">SkuName</label>'
        },
        {
            field: 'SKU_TYPE',
            title: '<label set-lan="html:SkuType">SkuType</label>'
        },
        {
            field: "C_SERIES_ID",
            title: '<label set-lan="html:CSeriesId">CSeriesId</label>'
        },
        {
            field: 'CUST_PARTNO',
            title: '<label set-lan="html:CustPartNo">CustPartNo</label>'
        }, {
            field: 'CUST_SKU_CODE',
            title: '<label set-lan="html:CustSkuCode">CustSkuCode</label>'
        }, {
            field: 'SN_RULE',
            title: '<label set-lan="html:SnRule">SnRule</label>'
        }, {
            field: 'PANEL_RULE',
            title: '<label set-lan="html:PanelRule">PanelRule</label>'
        }, {
            field: 'DESCRIPTION',
            title: '<label set-lan="html:Description">Description</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:tableEditTime">LastEditTime</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $('#Sku_Label_List_Table').bootstrapTable({
        data: SkuLabelListData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: false,
        showSelectTitle: true,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#Table_Toolbar",
        onCheck: function (row) {
            var rows = $('#Sku_Label_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditSkuLabel").attr("disabled", "disabled");
            } else {
                $("#EditSkuLabel").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#DeleteSku").attr("disabled", "disabled");
            } else {
                $("#DeleteSku").removeAttr("disabled");
            }
        },
        onUncheck: function (row) {
            var rows = $('#Sku_Label_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditSku").attr("disabled", "disabled");
            } else {
                $("#EditSku").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#DeleteSku").attr("disabled", "disabled");
            } else {
                $("#DeleteSku").removeAttr("disabled")
            }
        },
        columns: [{
            title: '<label set-lan="html:SELECT">SELECT</label>',
            checkbox: true
        }, {
            field: 'SKUNO',
            title: '<label set-lan="html:SkuNo">SkuNo</label>'
        }, {
            field: 'STATION',
                title: '<label set-lan="html:Station">Station</label>'
        }, {
            field: 'SEQ',
            title: '<label set-lan="html:Seq">Seq</label>'
        }, {
            field: 'QTY',
            title: '<label set-lan="html:Qty">Qty</label>'
        },
        {
            field: 'LABELNAME',
            title: '<label set-lan="html:LabelName">LabelName</label>'
        },
        {
            field: "LABELTYPE",
            title: '<label set-lan="html:LabelType">LabelType</label>'
        },
        {
            field: 'R_FILE_NAME',
            title: '<label set-lan="html:FileName">FileName</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:EditEmp">EditEmp</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:EditTime">EditTime</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $("#EditSku").on("click", function () {
        var selRows = $('#Sku_List_Table').bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("no records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }
        else if (selRows.length > 1) {
            layer.msg("Too many records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }
        SKU_EditRow = selRows[0];
        layer.open({
            type: 1,
            title: 'SKU Label Setting',
            area: ["80%", "90%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                Get_LabelConfig(SKU_EditRow.SKUNO);

                for (var i = 0; i < $("#ModifySkuno input[type=text]").length; i++) {
                    FindValueByKey($("#ModifySkuno input[type=text]")[i], SKU_EditRow);
                }
                for (var i = 0; i < $("#ModifySkuno select").length; i++) {
                    FindValueByKey($("#ModifySkuno select")[i], SKU_EditRow);
                }
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
            }
        });
    });
    $("#EditSkuLabel").on("click", function () {
        var selRows = $('#Sku_Label_List_Table').bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("no records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }
        else if (selRows.length > 1) {
            layer.msg("Too many records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }
        SKU_EditRow = selRows[0];
        layer.open({
            type: 1,
            title: 'SKU Label Setting',
            area: ["80%", "90%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                Get_LabelConfig(SKU_EditRow.SKUNO);

                for (var i = 0; i < $("#ModifySkuno input[type=text]").length; i++) {
                    FindValueByKey($("#ModifySkuno input[type=text]")[i], SKU_EditRow);
                }
                for (var i = 0; i < $("#ModifySkuno select").length; i++) {
                    FindValueByKey($("#ModifySkuno select")[i], SKU_EditRow);
                }
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
            }
        });
    });
    $("#RefreshSku").on("click", function () {
        $("#EditSku").attr("disabled", "disabled");
        $("#DeleteSku").attr("disabled", "disabled");
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetAllSkuLabel", {}, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                SkuListData = e.Data;
                $('#Sku_Label_List_Table').bootstrapTable("load", e.Data);
                mesUI.SetLanguage("SKUSetting");
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    });
    $("#EditModelDetail .form-control").on("change", function () {
        SKU_EditRow[this.name] = this.value;
    });    
    $('#LabelListTable').bootstrapTable({
        data: RouteList,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#LabelList_Toolbar",
        onCheck: function (row) {
            var rows = $('#LabelListTable').bootstrapTable('getSelections');
            if (rows.length <= 0) {
                $("#DeleteLabel").attr("disabled", "disabled");
            } else {
                $("#DeleteLabel").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#LabelListTable').bootstrapTable('getSelections');
            if (rows.length <= 0) {
                $("#DeleteLabel").attr("disabled", "disabled");
            } else {
                $("#DeleteLabel").removeAttr("disabled")
            }
        },
        columns: [{
            checkbox: true
        }, {
            field: 'STATION',
                title: '<label set-lan="html:STATION">STATION</label>'
        }, {
            field: 'SEQ',
                title: '<label set-lan="html:SEQNO">SEQ</label>'
        }, {
            field: 'QTY',
                title: '<label set-lan="html:QTY">QTY</label>'
        }, {
            field: 'LABELNAME',
                title: '<label set-lan="html:LABELNAME">LABELNAME</label>'
        }, {
            field: 'LABELTYPE',
                title: '<label set-lan="html:LABELTYPE">LABELTYPE</label>'
        }, {
            field: 'EDIT_EMP',
                title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $('#AddLabel').on("click", function () {
        var LabelObject = {};
        $("#LabelList .form-control").each(function () {
            LabelObject[$(this)[0].name] = $(this).val();
        });
        LabelObject.SKUNO = SKU_EditRow.SKUNO;
        LabelObject.ID = "";
        if (LabelObject.STATION == "") {
            layer.msg("Station Can not be null", {
                icon: 2,
                time: 3000
            }, function () {
                $("#LabelList .form-control[name=STATION]").focus();
            });
            return;
        }
        if (LabelObject.QTY == "") {
            layer.msg("Print QTY Can not be null", {
                icon: 2,
                time: 3000
            }, function () {
                $("#LabelList .form-control[name=QTY]").focus();
            });
            return;
        }
        else {
            var r = RegExp("^[0-9]*[1-9][0-9]*$");
            if (!r.test(LabelObject.QTY)) {
                layer.msg("Print QTY is not a valid number", {
                    icon: 2,
                    time: 3000
                }, function () {
                    $("#LabelList .form-control[name=QTY]").focus();
                });
                return;
            }
        }
        if (LabelObject.SEQ == "") {
            layer.msg("Print SEQ Can not be null", {
                icon: 2,
                time: 3000
            }, function () {
                $("#LabelList .form-control[name=SEQ]").focus();
            });
            return;
        }
        else {
            var r = RegExp("^[0-9]*[1-9][0-9]*$");
            if (!r.test(LabelObject.SEQ)) {
                layer.msg("Print SEQ is not a valid number", {
                    icon: 2,
                    time: 3000
                }, function () {
                    $("#LabelList .form-control[name=SEQ]").focus();
                });
                return;
            }
        }
        client.CallFunction("MESStation.Label.LabelConfig", "AlertLabelConfig", { LabelObject: LabelObject }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon: 1,
                    time: 3000
                }, function () {
                    Get_LabelConfig(SKU_EditRow.SKUNO);
                });
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    });
    $('#DeleteLabel').on("click", function () {
        var selRows = $('#LabelListTable').bootstrapTable('getSelections');
        if (selRows.length > 0) {
            layer.open({
                title: 'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
                yes: function (index) {
                    layer.close(index);
                    var RowsID = [];
                    for (var i = 0; i < selRows.length; i++) {
                        RowsID.push(selRows[i].ID);
                    }
                    parent.client.CallFunction("MESStation.Label.LabelConfig", "RemoveLabelConfig", { ID_LIST: RowsID },
                        function (e) {
                            if (e.Status == "Pass") {
                                layer.msg("OK，" + e.Message, {
                                    icon: 1,
                                    time: 3000
                                }, function () {
                                    Get_LabelConfig(SKU_EditRow.SKUNO);
                                });
                            }
                            else {
                                layer.msg("Error," + e.Message, {
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

    Get_SkuList();
    Get_StationList();
    Get_LabelList();
    Get_LabelType();
    Get_SkuAllLabelList();

    mesUI.SetLanguage("SKUSetting");
});


