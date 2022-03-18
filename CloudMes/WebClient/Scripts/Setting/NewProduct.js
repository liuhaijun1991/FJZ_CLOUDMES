var NewFlag = false;
var SkuListData = [];
var TPointList = [];
var RouteList = [];
var SkuAqlData = [];
var OtherSetData = [];
var AqlData = [];
var tableLocale = "";
var SKU_EditRow = null;
var SKU_OtherSet = null;
var mesUI;
var lan;
var client = null;
$(document).ready(function () {
    client = self.parent.client;
    mesUI = new MesClientUI(client);
    lan = $.cookie($.MES.CK_LAN_NAME);
    if (lan == "CHINESE") {
        tableLocale = "zh-CN"
    }
    else if (lan == "CHINESE_TW") {
        tableLocale = "zh-TW"
    }
    else {
        tableLocale = "en"
    };

    var FindValueByKey = function (key, data) {
        for (var d in data) {
            if (key.name == d) {
                key.value = data[d];
                break;
            }
            else if (Object.prototype.toString.call(data[d]) === '[object Object]') {
                FindValueByKey(key, data[d]);
            }
        }
    };

    var Get_Route = function (SkuId) {
        client.CallFunction("MESStation.Config.SkuRouteMappingConfig", "GetAvailableRoutesForSku", { SkuId: SkuId }, function (e) {
            try {
                $("#SKUAddRouteName").autocomplete("destroy");
            } catch (e) {

            }
            if (e.Status == "Pass") {
                var data = [];
                for (var i = 0 ; i < e.Data.length; i++) {
                    data.push(e.Data[i].ROUTE_NAME);
                }
                $("#SKUAddRouteName").autocomplete({
                    minLength: 3,
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

    var Get_Serial = function () {
        client.CallFunction("MESStation.Config.SkuSeries", "FetchCurrentSeries", { Field: "", Value: "" }, function (e) {
            var sel = $("#C_SERIES_ID");
            sel.empty();
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    sel.append($("<option value='" + e.Data[a].ID + "'>" + e.Data[a].SERIES_NAME + "</option>"));
                }
            }
        });
    };

    var Get_PackType = function () {
        client.CallFunction("MESStation.Packing.PackConfigAPI", "GetPackType", {}, function (e) {
            var sel = $("#PackConfigForm select[name=PACK_TYPE]");
            var isel = $("#PackConfigForm select[name=INSIDE_PACK_TYPE]");
            sel.empty();
            if (e.Status == "Pass") {
                isel.append($("<option value='SN'>SN</option>"));
                for (var a = 0; a < e.Data.length; a++) {
                    sel.append($("<option value='" + e.Data[a].PACK_TYPE + "'>" + e.Data[a].PACK_TYPE + "</option>"));
                    isel.append($("<option value='" + e.Data[a].PACK_TYPE + "'>" + e.Data[a].PACK_TYPE + "</option>"));
                }
            }
        });
    };

    var Get_TransportType = function () {
        client.CallFunction("MESStation.Packing.PackConfigAPI", "GetTransportType", {}, function (e) {
            var sel = $("#PackConfigForm select[name=TRANSPORT_TYPE]");
            sel.empty();
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    sel.append($("<option value='" + e.Data[a].TRANSPORT_TYPE + "'>" + e.Data[a].TRANSPORT_TYPE + "</option>"));
                }
            }
        });
    };

    var Get_SNRule = function () {
        client.CallFunction("MESPubLab.MESStation.SNMaker.SNRulerConfig", "GetSNRulers", {}, function (e) {
            var sel = $("#PackConfigForm select[name=SN_RULE]");
            sel.empty();
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    sel.append($("<option value='" + e.Data[a].NAME + "'>" + e.Data[a].NAME + "</option>"));
                }
            }
        });
    };

    var Get_LabelList = function () {
        client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabelList", {}, function (e) {
            try {
                $("#LabelList input[name=LABELNAME]").autocomplete("destroy");
            } catch (e) {

            }
            if (e.Status == "Pass") {
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
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    sel.append($("<option value='" + e.Data[a].NAME + "'>" + e.Data[a].NAME + "</option>"));
                }
            }
        });
    };

    var Get_SkuList = function () {
        $("#EditSku").attr("disabled", "disabled");
        $("#DeleteSku").attr("disabled", "disabled");
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetAllSku", {}, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
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

    var Get_RoutList = function (SkuID) {
        $("#DeleteRoute").attr("disabled", "disabled");
        client.CallFunction("MESStation.Config.SkuRouteMappingConfig", "GetRoutesBySkuId", { SkuId: SkuID }, function (e) {
            if (e.Status == "Pass") {
                $('#RouteListTable').bootstrapTable("load", e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        })
    };

    var Get_BFPoint = function (Sku) {
        $("#EditPoint").attr("disabled", "disabled");
        $("#DeletePoint").attr("disabled", "disabled");
        client.CallFunction("MESStation.Config.SAPStationMapConfig", "GetAllSapStationMapBySku", { SkuNo: Sku }, function (e) {
            if (e.Status == "Pass") {
                $('#BFPoint_List_Table').bootstrapTable("load", e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };

    var Get_PackConfig = function (Skuno) {
        $("#DeletePack").attr("disabled", "disabled");
        client.CallFunction("MESStation.Packing.PackConfigAPI", "GetPackConfigBySKUNO", { SkuNo: Skuno }, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                $('#PackListTable').bootstrapTable('load', e.Data);
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
            if (e.Status == "Pass") {
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

    var Get_AqlLevel = function(AqlType) {
        client.CallFunction("MESStation.Config.CAqltypeConfig", "GetAqlLevel", { AQLTYPE: AqlType }, function (e) {
            var selLevel = $("#AqlList select[name=DefaultLevel]");
            selLevel.empty();
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    selLevel.append($("<option value='" + e.Data[a] + "'>" + e.Data[a] + "</option>"));
                }
            }
        });
    }

    var Get_AqlType = function(Skuno) {
        client.CallFunction("MESStation.Config.CAqltypeConfig", "GetAllAqlName", {}, function (e) {
            var selAql = $("#AqlList select[name=AqlType]");
            selAql.empty();
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    selAql.append($("<option value='" + e.Data[a] + "'>" + e.Data[a] + "</option>"));
                }
                if (e.Data.length > 0) {
                    Get_AqlLevel(selAql.val());
                    selAql.unbind("change");
                    selAql.bind("change", function () {
                        Get_AqlLevel(selAql.val());
                    });
                }
            }
        });
    }

    var Get_SkuAqlTypeData = function (skuId) {
        client.CallFunction("MESStation.Config.CAqltypeConfig", "GetSkuAqlData", { SkuId: skuId }, function (e) {
            if (e.Status == "Pass") {
                $('#AqlListTable').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
        client.CallFunction("MESStation.Config.CAqltypeConfig", "GetSkuAql", { SkuId: skuId }, function (e) {
            if (e.Status == "Pass") {
                $('#SkuAqlListTable').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    }

    var Get_SkuOtherSet = function () {
        client.CallFunction("MESStation.Config.NewProduct", "GetSkuOtherSet", { SkuObject: SKU_EditRow }, function (e) {
            if (e.Status == "Pass") {
                for (var i = 0; i < $("#EditModelOtherSet .form-control").length; i++) {
                    FindValueByKey($("#EditModelOtherSet .form-control")[i], e.Data);
                }
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        })
    }

    var Get_OtherSetting = function () {
        client.CallFunction("MESStation.Config.NewProduct", "GetSkuOtherSetting", { SkuObject: SKU_EditRow }, function (e) {
            if (e.Status == "Pass") {
                for (var i = 0; i < $("#OtherSettings .form-control").length; i++) {
                    FindValueByKey($("#OtherSettings .form-control")[i], e.Data);
                }
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        })
    }

    $('#Sku_List_Table').bootstrapTable({
        data: SkuListData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
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
                $("#EditSku").removeAttr("disabled")
            }
            if (rows.length <= 0) {
                $("#DeleteSku").attr("disabled", "disabled");
            } else {
                $("#DeleteSku").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#Sku_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditSku").attr("disabled", "disabled");
            } else {
                $("#EditSku").removeAttr("disabled")
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
            field: 'LAST_EDIT_TIME',
            title: '<label set-lan="html:tableEditTime">LastEditTime</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $("#NewSku").on("click", function () {
        NewFlag = true;
        SKU_EditRow = {};
        SKU_OtherSet = {};
        OtherSetData = {};
        $("#EditModelDetail .form-control").each(function () {
            this.value = "";
        });
        $("#EditModelOtherSet .form-control").each(function () {
            this.value = "";
        });
        $("#OtherSettings .form-control").each(function () {
            this.value = "";
        });

        $("#EditModelDetail .form-control[name=BU]").val(client.UserInfo.BU);
        $('#Sku_List_Table').bootstrapTable("uncheckAll");
        $('#BFPoint_List_Table').bootstrapTable("load", []);
        $('#RouteListTable').bootstrapTable("load", []);
        $('#PackListTable').bootstrapTable("load", []);
        $('#LabelListTable').bootstrapTable("load", []);
        layer.open({
            type: 1,
            title: 'New SKU',
            area: ["80%", "90%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                Get_Route("");
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
            }
        });
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
        SKU_OtherSet = {};
        OtherSetData = {};
        layer.open({
            type: 1,
            title: 'Edit SKU',
            area: ["80%", "90%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                Get_SkuOtherSet();
                Get_RoutList(SKU_EditRow.ID);
                Get_BFPoint(SKU_EditRow.SKUNO);
                Get_Route(SKU_EditRow.ID);
                Get_PackConfig(SKU_EditRow.SKUNO);
                Get_LabelConfig(SKU_EditRow.SKUNO);
                Get_SkuAqlTypeData(SKU_EditRow.ID);
                Get_OtherSetting();
                for (var i = 0; i < $("#ModifySkuno input[type=text]").length; i++) {
                    FindValueByKey($("#ModifySkuno input[type=text]")[i], SKU_EditRow);
                }
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
            }
        });
    });
    $("#DeleteSku").on("click", function () {
        var selRows = $('#Sku_List_Table').bootstrapTable('getSelections');
        if (selRows.length > 0) {
            layer.open({
                title: 'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
                yes: function (index) {
                    layer.close(index);
                    var SkuID = [];
                    for (var i = 0; i < selRows.length; i++) {
                        SkuID.push(selRows[i].ID);
                    }
                    parent.client.CallFunction("MESStation.Config.SkuConfig", "DeleteSkuById", { SkuID: SkuID }, function (e) {
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

    $("#EditModelDetail .form-control").on("change", function () {
        SKU_EditRow[this.name] = this.value;
    });
    $("#EditModelOtherSet .form-control").on("change", function () {
        SKU_OtherSet[this.name] = this.value;
    });

    $("#SaveSku").on("click", function () {
        var ClassName, FunctionName;
        var id = $("#EditModelDetail input[name=ID]").val();
        if (id == ""||id == undefined) {//新增
            ClassName = "MESStation.Config.NewProduct";
            FunctionName = "AddSku";
        } else {//修改
            ClassName = "MESStation.Config.NewProduct";
            FunctionName = "UpdateSku";
        }

        $("#EditModelDetail .form-control").each(function () {
            SKU_EditRow[this.name] = this.value;
        });

        $("#EditModelOtherSet .form-control").each(function () {
            SKU_OtherSet[this.name] = this.value;
        });

        if (SKU_EditRow.SKU_TYPE == "PCBA")
        {
            if (SKU_OtherSet.RADISOSP == "" || SKU_OtherSet.RADHIGHTEMP == "") {
                layer.open({
                    title: 'Warning',
                    btn: ['ok'],
                    content: "Save Fail!</br>PCBA Side must select ISOSP&HIGHTEMP !",
                });
                return;
            }
        }
        //保存數據並清空所有input
        client.CallFunction(ClassName, FunctionName, { SkuObject: SKU_EditRow, SkuDetail: SKU_OtherSet }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon: 1,
                    time: 3000
                }, function () {
                });
                SKU_EditRow["ID"] = e.Data;
                $("#EditModelDetail input[name=ID]").val(e.Data);
            }
            else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
        $('#myTab li:eq(1) a').tab('show');
    });

    $("#SaveOtherSettings").on("click", function () {
        var ClassName, FunctionName;

        $("#OtherSettings .form-control").each(function () {
            OtherSetData[this.name] = this.value;
        });
        $("#EditModelDetail .form-control").each(function () {
            SKU_EditRow[this.name] = this.value;
        });

        client.CallFunction("MESStation.Config.NewProduct", "SaveOtherSetting", { SkuObject: SKU_EditRow, OtherSetting: OtherSetData }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon: 1,
                    time: 3000
                }, function () {
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

    $('#BFPoint_List_Table').bootstrapTable({
        data: TPointList,
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
        toolbar: "#BFPoint_Toolbar",
        onCheck: function (row) {
            var rows = $('#BFPoint_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditPoint").attr("disabled", "disabled");
            } else {
                $("#EditPoint").removeAttr("disabled")
            }
            if (rows.length <= 0) {
                $("#DeletePoint").attr("disabled", "disabled");
            } else {
                $("#DeletePoint").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#Sku_List_Table').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditPoint").attr("disabled", "disabled");
            } else {
                $("#EditPoint").removeAttr("disabled")
            }
            if (rows.length <= 0) {
                $("#DeletePoint").attr("disabled", "disabled");
            } else {
                $("#DeletePoint").removeAttr("disabled")
            }
        },
        columns: [{
            checkbox: true
        }, {
            field: 'SKUNO',
            title: '<label set-lan="html:SkuNo">SkuNo</label>'
        }, {
            field: 'STATION_NAME',
            title: '<label set-lan="html:STATION_NAME">STATION_NAME</label>'
        }, {
            field: 'SAP_STATION_CODE',
            title: '<label set-lan="html:SAP_STATION_CODE">SAP_STATION_CODE</label>'
        }, {
            field: 'WORKORDER_TYPE',
            title: '<label set-lan="html:WORKORDER_TYPE">WORKORDER_TYPE</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:tableEditEmp">EDIT_EMP</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:tableEditTime">EDIT_TIME</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $('#NewPoint').on("click", function () {
        var MapObject = {};
        $("#ThrowPointForm .form-control").each(function () {
            MapObject[$(this)[0].name] = $(this).val();
        });
        MapObject.SKUNO = SKU_EditRow.SKUNO;
        client.CallFunction("MESStation.Config.SAPStationMapConfig", "UpdateSapStationMap", { Operation: "ADD", MapObject: MapObject }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon: 1,
                    time: 3000
                }, function () {
                    Get_BFPoint(SKU_EditRow.SKUNO);
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
    $('#DeletePoint').on("click", function () {
        var selRows = $('#BFPoint_List_Table').bootstrapTable('getSelections');
        if (selRows.length > 0) {
            layer.open({
                title: 'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
                yes: function (index) {
                    layer.close(index);
                    var IDList = [];
                    for (var i = 0; i < selRows.length; i++) {
                        IDList.push(selRows[i].ID);
                    }
                    parent.client.CallFunction("MESStation.Config.SAPStationMapConfig", "DeleteSapStationMap", { IDList: IDList }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("OK," + e.Message, {
                                icon: 1,
                                time: 3000
                            }, function () {
                                Get_BFPoint(SKU_EditRow.SKUNO);
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

    $('#RouteListTable').bootstrapTable({
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
        toolbar: "#RouteList_Toolbar",
        onCheck: function (row) {
            var rows = $('#RouteListTable').bootstrapTable('getSelections');
            if (rows.length <= 0) {
                $("#DeleteRoute").attr("disabled", "disabled");
            } else {
                $("#DeleteRoute").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#RouteListTable').bootstrapTable('getSelections');
            if (rows.length <= 0) {
                $("#DeleteRoute").attr("disabled", "disabled");
            } else {
                $("#DeleteRoute").removeAttr("disabled")
            }
        },
        columns: [{
            checkbox: true
        }, {
            field: 'DEFAULT_FLAG',
            title: '<label set-lan="html:DEFAULT_FLAG">DEFAULT_FLAG</label>'
        }, {
            field: 'ROUTE_NAME',
            title: '<label set-lan="html:ROUTE_NAME">ROUTE_NAME</label>'
        }, {
            field: 'DEFAULT_SKUNO',
            title: '<label set-lan="html:DEFAULT_SKUNO">DEFAULT_SKUNO</label>'
        }, {
            field: 'ROUTE_TYPE',
            title: '<label set-lan="html:ROUTE_TYPE">ROUTE_TYPE</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $('#SKU_Route').on("click", function () {
        var DEFAULT_FLAG = "";
        if ($('#checkbox-id').is(':checked')) {
            DEFAULT_FLAG = "Y";
        } else {
            DEFAULT_FLAG = "N";
        }
        client.CallFunction("MESStation.Config.SkuRouteMappingConfig", "AddSKuRouteMapping", { MappingObject: { DEFAULT_FLAG: DEFAULT_FLAG, SKU_ID: SKU_EditRow.ID, ROUTE_ID: $("#SKUAddRouteName").val() } }, function (e) {
            if (e.Status == "Pass") {
                Get_RoutList(SKU_EditRow.ID);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    });
    $('#DeleteRoute').on("click", function () {
        var selRows = $('#RouteListTable').bootstrapTable('getSelections');
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
                    parent.client.CallFunction("MESStation.Config.SkuRouteMappingConfig", "DeleteSkuRouteMapping", { MappingID: RowsID },
                        function (e) {
                            if (e.Status == "Pass") {
                                layer.msg("OK，" + e.Message, {
                                    icon: 1,
                                    time: 3000
                                }, function () {
                                    Get_RoutList(SKU_EditRow.ID);
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

    $('#PackListTable').bootstrapTable({
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
        toolbar: "#PackList_Toolbar",
        onCheck: function (row) {
            var rows = $('#PackListTable').bootstrapTable('getSelections');
            if (rows.length <= 0) {
                $("#DeletePack").attr("disabled", "disabled");
            } else {
                $("#DeletePack").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#PackListTable').bootstrapTable('getSelections');
            if (rows.length <= 0) {
                $("#DeletePack").attr("disabled", "disabled");
            } else {
                $("#DeletePack").removeAttr("disabled")
            }
        },
        columns: [{
            checkbox: true
        }, {
            field: 'PACK_TYPE',
            title: '<label set-lan="html:PACK_TYPE">TYPE</label>'
        }, {
            field: 'TRANSPORT_TYPE',
            title: '<label set-lan="html:TRANSPORT_TYPE">TRANSPORT</label>'
        }, {
            field: 'INSIDE_PACK_TYPE',
            title: '<label set-lan="html:INSIDE_PACK_TYPE">INSIDE</label>'
        }, {
            field: 'MAX_QTY',
            title: '<label set-lan="html:MAX_QTY">QTY</label>'
        }, {
            field: 'SN_RULE',
            title: '<label set-lan="html:SN_RULE">RULE</label>'
        }, {
            field: 'DESCRIPTION',
            title: '<label set-lan="html:DESCRIPTION">EDIT EMP</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $('#AddPack').on("click", function () {
        var PackObj = {};
        $("#PackConfigForm .form-control").each(function () {
            PackObj[$(this)[0].name] = $(this).val();
        });
        PackObj.SKUNO = SKU_EditRow.SKUNO;
        PackObj.ID = "";
        client.CallFunction("MESStation.Packing.PackConfigAPI", "AlertPackConfig", { PackObj: PackObj }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon: 1,
                    time: 3000
                }, function () {
                    Get_PackConfig(SKU_EditRow.SKUNO);
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
    $('#DeletePack').on("click", function () {
        var selRows = $('#PackListTable').bootstrapTable('getSelections');
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
                    parent.client.CallFunction("MESStation.Packing.PackConfigAPI", "RemovePackConfig", { ID_LIST: RowsID },
                        function (e) {
                            if (e.Status == "Pass") {
                                layer.msg("OK，" + e.Message, {
                                    icon: 1,
                                    time: 3000
                                }, function () {
                                    Get_PackConfig(SKU_EditRow.SKUNO);
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
            title: '<label set-lan="html:PACK_TYPE">STATION</label>'
        }, {
            field: 'SEQ',
            title: '<label set-lan="html:TRANSPORT_TYPE">SEQ</label>'
        }, {
            field: 'QTY',
            title: '<label set-lan="html:INSIDE_PACK_TYPE">QTY</label>'
        }, {
            field: 'LABELNAME',
            title: '<label set-lan="html:MAX_QTY">LABELNAME</label>'
        }, {
            field: 'LABELTYPE',
            title: '<label set-lan="html:SN_RULE">LABELTYPE</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:DESCRIPTION">EDIT_EMP</label>'
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

    $("#ConfirmAql").on("click", function () {
        var id = SKU_EditRow.ID;
        var aqlType = $("#AqlList select[name=AqlType]").val();
        var aqlLevel = $("#AqlList select[name=DefaultLevel]").val();
        if (id == undefined)
            layer.msg("Sku Don't Save!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        else
            client.CallFunction("MESStation.Config.CAqltypeConfig", "AddSkuAql", { SkuId: id, AqlType: aqlType, aqlLevel: aqlLevel }, function (e) {
                if (e.Status == "Pass") {
                    layer.msg("OK," + e.Message, {
                        icon: 1,
                        time: 3000
                    }, function () {
                    });
                    Get_SkuAqlTypeData(id);
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
    $('#AqlListTable').bootstrapTable({
        data: AqlData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: false,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        columns: [{
            checkbox: true
        }, {
            field: 'AQL_TYPE',
            title: '<label set-lan="html:AQL_TYPE">AQL_TYPE</label>'
        }, {
            field: 'LOT_QTY',
            title: '<label set-lan="html:LOT_QTY">LOT_QTY</label>'
        }, {
            field: 'GL_LEVEL',
            title: '<label set-lan="html:GL_LEVEL">GL_LEVEL</label>'
        }, {
            field: 'SAMPLE_QTY',
            title: '<label set-lan="html:SAMPLE_QTY">SAMPLE_QTY</label>'
        }, {
            field: 'REJECT_QTY',
            title: '<label set-lan="html:REJECT_QTY">REJECT_QTY</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:EDIT_TIME">EDIT_TIME</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $('#SkuAqlListTable').bootstrapTable({
        data: SkuAqlData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: false,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        columns: [{
            checkbox: true
        }, {
            field: 'AQLTYPE',
            title: '<label set-lan="html:AQL_TYPE">AQLTYPE</label>'
        }, {
            field: 'SKUNO',
            title: '<label set-lan="html:LOT_QTY">SKUNO</label>'
        }, {
            field: 'DEFAULLEVEL',
            title: '<label set-lan="html:SAMPLE_QTY">DEFAULLEVEL</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:GL_LEVEL">EDIT_EMP</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:SAMPLE_QTY">EDIT_TIME</label>'
        }],
        locale: tableLocale//中文支持,
    });
    Get_SkuList();
    Get_Serial();
    Get_PackType();
    Get_TransportType();
    Get_SNRule();
    Get_LabelList();
    Get_LabelType();
    Get_AqlType();
    mesUI.SetLanguage("NewProduct");
});



