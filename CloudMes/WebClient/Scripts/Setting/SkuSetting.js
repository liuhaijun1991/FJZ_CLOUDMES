var NewFlag = false;
var SkuListData = [];
var TPointList = [];
var RouteList = [];
var SkuAqlData = [];
var AqlData = [];
var SkuWeightData = [];
var ExtendConfigData = [];
var ExtendEasyOption = [];
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
    if (lan == "CHINESE") {
        tableLocale = "zh-CN"
    }
    else if (lan == "CHINESE_TW") {
        tableLocale = "zh-TW"
    }
    else {
        tableLocale = "en"
    };

    var showNewSku = function () {
        NewFlag = true;
        SKU_EditRow = {};
        referenceSku = $("#referenceSku").val();
        $("#EditModelDetail .form-control").each(function () {
            this.value = "";
        });
        $("#EditModelDetail .form-control[name=BU]").val(client.UserInfo.BU);
        $('#Sku_List_Table').bootstrapTable("uncheckAll");
        $('#BFPoint_List_Table').bootstrapTable("load", []);
        $('#RouteListTable').bootstrapTable("load", []);
        $('#PackListTable').bootstrapTable("load", []);
        $('#LabelListTable').bootstrapTable("load", []);
        $('#AqlListTable').bootstrapTable("load", []);
        $('#SkuAqlListTable').bootstrapTable("load", []);
        $('#SNRuleListTable').bootstrapTable("load", []);
        $('#SNRuleDetailTable').bootstrapTable("load", []);
        $('#WeightListTable').bootstrapTable("load", []);
        $('#ExtendConfigTable').bootstrapTable("load", []);
        layer.open({
            type: 1,
            title: 'New SKU',
            area: ["90%", "95%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                parent.client.CallFunction("MESStation.Config.SkuConfig", "GetSingleSkuByName", { Sku_Name: referenceSku }, function (e) {
                    if (e.Status == "Pass" && e.Data.length == 1) {
                        $("#ModifyList").removeClass("hidden");
                        Get_Route(e.Data[0].ID);
                        Get_StationList(e.Data[0].ID);
                        Get_RoutList(e.Data[0].ID);
                        Get_BFPoint(e.Data[0].SKUNO);
                        Get_PackConfig(e.Data[0].SKUNO);
                        Get_LabelConfig(e.Data[0].SKUNO);
                        Get_SkuAqlTypeData(e.Data[0].ID);
                        Get_ExtendConfig(e.Data[0].SKUNO);
                        Get_SNRule();
                        setModifySkuno(e.Data[0]);
                    } else {
                        $("#ModifyList").removeClass("hidden");
                        Get_Route("");
                    }
                });
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
            }
        });
    }

    var setModifySkuno = function (smdata) {
        for (var i = 0; i < $("#ModifySkuno input[type=text]").length; i++) {
            if ($("#ModifySkuno input[type=text]")[i].name == "ID")
                continue;
            FindValueByKey($("#ModifySkuno input[type=text]")[i], smdata);
        }
    }

    var clearsitevalue = function (siteid, unsetobj) {
        for (var i = 0; i < $("#" + siteid + " .form-control").length; i++) {
            if (unsetobj === undefined || unsetobj.indexOf($("#" + siteid + " .form-control")[i].name) === -1) {
                if ($("#" + siteid + " .form-control")[i].type === "text")
                    $("#" + siteid + " .form-control")[i].value = "";
                else if ($("#" + siteid + " .form-control")[i].type === "select-one")
                    $("#" + siteid + " .form-control")[i].value = $("#" + siteid + " .form-control")[i].options[0].value;
            }
        }
    }

    var getformobj = function (siteid) {
        var objres = {};
        for (var i = 0; i < $("#" + siteid + " .form-control").length; i++) {
            objres[$("#" + siteid + " .form-control")[i].name] = $("#" + siteid + " .form-control")[i].value;
        }
        return objres;
    }

    var setformobj = function (formid, objdata) {
        for (var i = 0; i < $("#" + formid + " .form-control").length; i++) {
            for (let k in objdata) {
                if ($("#" + formid + " .form-control")[i].name === k) {
                    if (typeof objdata[k] == 'object' && objdata[k] != null) {                        
                        $("#" + formid + " .form-control")[i].setAttribute("placeholder", objdata[k]["placeholder"]);
                        $("#" + formid + " .form-control")[i].value = objdata[k]["value"];
                    } else {
                        $("#" + formid + " .form-control")[i].value = objdata[k];
                        //if (objdata[k] != "") {
                        //    $("#" + formid + " .form-control")[i].setAttribute("disabled", "disabled");
                        //}                        
                    }
                }
            }
        }
    }


    var FindValueByKey = function (key, data) {
        for (var d in data) {
            if (key.name == d.toUpperCase()) {
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
                for (var i = 0; i < e.Data.length; i++) {
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
                //for (var a = 0; a < e.Data.length; a++) {
                //    sel.append($("<option value='" + e.Data[a].NAME + "'>" + e.Data[a].NAME + "</option>"));
                //}

                const transport_type = e.Data.filter((type) => type.NAME.toLowerCase().includes('carton') || type.NAME.toLowerCase().includes('pallet'));

                for (var a = 0; a < transport_type.length; a++) {
                    sel.append($("<option value='" + transport_type[a].NAME + "'>" + transport_type[a].NAME + "</option>"));
                }
            }
        });
    };

    var Get_StationList = function (SkuId) {
        client.CallFunction("MESStation.FileUpdate.FileUpload", "GetStationList", { "SkuId": SkuId }, function (e) {
            try {
                $("#LabelList input[name=STATION]").autocomplete("destroy");
                $("#EditThrowPoint input[name=STATION_NAME]").autocomplete("destory");
            } catch (e) {

            }
            if (e.Status == "Pass") {
                var data = [];
                data.push("PRINT_MAC");
                data.push("REPRINT_MAC");
                data.push("TGMES_SHIPOUT");
                data.push("KIT_PRINT");
                data.push("KIT_REPRINT");
                data.push("SKU_PRINT");
                data.push("SN_PRINT");
                data.push("PANEL_SN_PRINT");
                data.push("KIT_SKUPrint_SN");
                for (var i = 0; i < e.Data.length; i++) {
                    data.push(e.Data[i]);
                }
                $("#EditThrowPoint input[name=STATION_NAME]").autocomplete({
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
                $("#NewWeightsite input[name=STATION]").autocomplete({
                    minLength: 0,
                    source: data,
                    select: function (event, ui) {
                        $(this).val(ui.item.value);
                        $('#NewWeightsite').data("bootstrapValidator").updateStatus("STATION", "NOT_VALIDATED", null);
                        $('#NewWeightsite').data("bootstrapValidator").validateField('STATION');
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

            }
            if (e.Status == "Pass") {
                var data = [];
                for (var i = 0; i < e.Data.length; i++) {
                    data.push(e.Data[i].LABELNAME);
                }
                $("#LabelList input[name=LABELNAME]").autocomplete({
                    minLength: 0,
                    source: data,
                    select: function (event, ui) {
                        $(this).val(ui.item.value);
                        $("#loadingImg").css("display", "");
                        client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabFilePreview", { NAME: ui.item.value }, function (e) {
                            if (e.Status == "Pass") {
                                $("#loadingImg").css("display", "none");
                                $("#uImg").attr('src', '../../' + e.Data);
                                setTimeout(function (data) {
                                    var img = $("#Preview");
                                    var w = $("#uImg").width();
                                    var h = $("#uImg").height();
                                    img.attr("src", "../../" + e.Data);
                                    img.css("width", ((w / 2) + 60) + 'px');
                                    img.css("height", ((h / 2) + 60) + 'px');
                                }, 100, e.Data);
                            } else {
                                $("#Preview").removeAttr('src', '');
                                layer.msg(e.Message, {
                                    icon: 2,
                                    time: 3000
                                }, function () {
                                });
                            }
                        });
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
                SkuListData = e.Data;
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

    var Get_AqlLevel = function (AqlType) {
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

    var Get_AqlType = function (Skuno) {
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

    var Get_AllSnRuleData = function () {
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetAllSnRule", {}, function (e) {
            if (e.Status == "Pass") {
                $("#SNRuleListTable").bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }

        });
    }

    var Get_SnRuleDetailData = function (RuleId) {
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetSNRuleDetailById", { RuleId: RuleId }, function (e) {
            if (e.Status == "Pass") {
                $("#SNRuleDetailTable").bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    }

    var Get_CurrentSnRule = function (SkuId) {
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetSkuSnRule", { SkuId: SkuId }, function (e) {
            if (e.Status == "Pass") {
                $("#CurrentRule").val(e.Data.CURVALUE);
            }
        });
    }

    var Get_WeightList = function (Skuno) {
        $("#EditWeight").attr("disabled", "disabled");
        $("#DeleteWeight").attr("disabled", "disabled");
        $("#weight_sku").val(Skuno);
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetWeightConfigBySku", { Skuno: Skuno }, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                SkuWeightData = e.Data;
                $('#WeightListTable').bootstrapTable('removeAll');
                $('#WeightListTable').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };

    var Get_ExtendConfig = function (Skuno) {
        $("#EditExtendConfig").attr("disabled", "disabled");
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.CSkuDetailConfig", "SelectCSkuDetail", { SKUNO: Skuno }, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                ExtendConfigData = e.Data;
                $('#ExtendConfigTable').bootstrapTable('removeAll');
                $('#ExtendConfigTable').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };

    var Get_ExtendEasyOption = function (Skuno) {
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetSkuDetailConfigList", {}, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                ExtendEasyOption = e.Data;
                $("#easyOption").empty();
                for (var i = 0; i < ExtendEasyOption.length; i++) {
                    $("#easyOption").append('<button type="button" class="btn btn -default btn - xs" id="btn_' + ExtendEasyOption[i].ID + '">' + ExtendEasyOption[i].NAME + '</button>');
                }
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
        singleSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        showExport: true,
        exportDataType: "all",
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
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#EditSku").attr("disabled", "disabled");
            $("#DeleteSku").removeAttr("disabled");
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#EditSku").attr("disabled", "disabled");
            $("#DeleteSku").attr("disabled", "disabled");
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
            field: "SERIES_NAME",
            title: '<label set-lan="html:Series">Serial</label>'
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
    $("#NewSku").on("click", function () {
        var referenceHtml = "<div class=\"panel-body\"><div class=\"form-group\"><label class=\"col-xs-3 text-right\">From SKU:</label><div class=\"col-xs-7\"><input id=\"referenceSku\" name=\"referenceSku\"  type=\"text\" class=\"form-control\" /> </div><div class=\"col-xs-2\"><button type=\"button\" class=\"btn btn-outline btn-primary\" id=\"ReferenceBtn\"><lan set-lan=\"html:Commit\">Confirm</lan></button></div></div></div>";
        parent.client.CallFunction("MESStation.Config.SkuConfig", "GetAllCSku", {}, function (e) {
            if (e.Status == "Pass") {
                layer.open({
                    type: 1,
                    title: 'Copy SKU',
                    skin: 'layui-layer-rim', //加上边框
                    area: ['30%', '20%'], //宽高
                    content: referenceHtml,
                    success: function (layero, index) {
                        $("#ReferenceBtn").on("click", function () {
                            showNewSku();
                            layer.close(index);
                        });
                    }
                });
                var data = [];
                for (var i = 0; i < e.Data.length; i++) {
                    data.push(e.Data[i].SKUNO);
                }
                $("#referenceSku").autocomplete({
                    minLength: 2,
                    source: data,
                    select: function (event, ui) {
                        var e = $.Event("keypress");
                        e.keyCode = 13;
                        $(this).val(ui.item.value);
                        $(this).trigger(e);
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
            else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
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
        layer.open({
            type: 1,
            title: 'Edit SKU',
            area: ["90%", "95%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                Get_RoutList(SKU_EditRow.ID);
                Get_BFPoint(SKU_EditRow.SKUNO);
                Get_Route(SKU_EditRow.ID);
                Get_PackConfig(SKU_EditRow.SKUNO);
                Get_LabelConfig(SKU_EditRow.SKUNO);
                Get_SkuAqlTypeData(SKU_EditRow.ID);
                Get_StationList(SKU_EditRow.ID);
                Get_CurrentSnRule(SKU_EditRow.ID);//不知道取這個來有啥用,獲取的邏輯又寫錯,當獲取對象為NULL時,報錯信息又沒有加
                Get_WeightList(SKU_EditRow.SKUNO);
                Get_ExtendConfig(SKU_EditRow.SKUNO);
                //原邏輯確有不完善的地方，已更新。該功能的目的是獲取當前機種的 SN 編碼格式的具體格式
                for (var i = 0; i < $("#ModifySkuno input[type=text]").length; i++) {
                    FindValueByKey($("#ModifySkuno input[type=text]")[i], SKU_EditRow);
                }
                for (var i = 0; i < $("#ModifySkuno select").length; i++) {
                    FindValueByKey($("#ModifySkuno select")[i], SKU_EditRow);
                }


                //if (SKU_EditRow.SKU_TYPE != null && SKU_EditRow.SKU_TYPE != "") {
                //    $("#ModifySkuno select[name=SKU_TYPE]").find("option[value = '" + SKU_EditRow.SKU_TYPE + "']").prop("selected", "selected");
                //}



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
                                Get_SkuList();
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
    $("#RefreshSku").on("click", function () {
        $("#EditSku").attr("disabled", "disabled");
        $("#DeleteSku").attr("disabled", "disabled");
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.SkuConfig", "GetAllSku", {}, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                SkuListData = e.Data;
                $('#Sku_List_Table').bootstrapTable("load", e.Data);
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
    $("#SaveSku").on("click", function () {
        layer.open({
            title: 'Tips',
            btn: ['Submit', 'Cancel'],
            content: "Are you sure you want to submit these data?",
            yes: function (index) {
                //關閉該彈出框
                layer.close(index);
                var ClassName, FunctionName;
                var id = $("#EditModelDetail input[name=ID]").val();
                if (referenceSku == undefined || referenceSku == "") {
                    if (id == "" || id == undefined) {//新增
                        ClassName = "MESStation.Config.SkuConfig";
                        FunctionName = "AddSku";
                    } else {//修改
                        ClassName = "MESStation.Config.SkuConfig";
                        FunctionName = "UpdateSku";
                    }
                } else {
                    ClassName = "MESStation.Config.SkuConfig";
                    FunctionName = "CopySku";
                    SKU_EditRow["COPYFROMSKU"] = referenceSku;
                }
                $("#EditModelDetail .form-control").each(function () {
                    SKU_EditRow[this.name] = $.trim(this.value);
                });

                //保存數據並清空所有input
                client.CallFunction(ClassName, FunctionName, { SkuObject: SKU_EditRow }, function (e) {
                    if (e.Status == "Pass") {
                        layer.msg("OK," + e.Message, {
                            icon: 1,
                            time: 3000
                        }, function () {
                        });
                        SKU_EditRow["ID"] = e.Data;
                        $("#EditModelDetail input[name=SkuId]").val(e.Data);
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
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#EditPoint").attr("disabled", "disabled");
            $("#DeletePoint").removeAttr("disabled");
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#EditPoint").attr("disabled", "disabled");
            $("#DeletePoint").attr("disabled", "disabled");
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
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#DeleteRoute").removeAttr("disabled");
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#DeleteRoute").attr("disabled", "disabled");
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
        if ($('#checkFai-id').is(':checked')) {
            client.CallFunction("MESStation.Config.SkuRouteMappingConfig", "AddFaiToSku", { MappingObject: { SKU_ID: SKU_EditRow.ID, ROUTE_ID: $("#SKUAddRouteName").val() } }, function (e) {
            });
        }
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
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#DeletePack").removeAttr("disabled");
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#DeletePack").attr("disabled", "disabled");
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
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#DeleteLabel").removeAttr("disabled");
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#DeleteLabel").attr("disabled", "disabled");
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

    $('#SNRuleListTable').bootstrapTable({
        pagination: true,
        pageSize: 5,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        //maintainSelected: false,
        singleSelect: true,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#AddSnRuleTool",
        columns: [{
            title: '<label set-lan="html:SELECT">SELECT</label>',
            checkbox: true
        }, {
                field: 'ID',
                title: '<label set-lan="html:ID">ID</label>'
        },{
            field: 'NAME',
            title: '<label set-lan="html:NAME">NAME</label>'
        }, {
            field: 'CURVALUE',
            title: '<label set-lan="html:CURVALUE">CURVALUE</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:EDIT_TIME">EDIT_TIME</label>'
        }],
        onCheck: function (row) {
            var rows = $('#SNRuleListTable').bootstrapTable('getSelections');
            if (rows.length > 0) {
                var Id = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
                if (typeof (Id) == 'undefined') {
                    return false;
                } else {
                    Get_SnRuleDetailData(Id);
                }
            }
        },
        onUncheck: function (row) {
            var rows = $('#SNRuleListTable').bootstrapTable('getSelections');
            if (rows.length == 0) {
                $('#SNRuleDetailTable').bootstrapTable('load', []);
            }
        },
        locale: tableLocale//中文支持,
    });
    $('#SNRuleDetailTable').bootstrapTable({
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
            field: 'SEQ',
            title: '<label set-lan="html:SEQ">SEQ</label>'
        }, {
            field: 'INPUTTYPE',
            title: '<label set-lan="html:INPUTTYPE">INPUTTYPE</label>'
        }, {
            field: 'CODETYPE',
            title: '<label set-lan="html:CODETYPE">CODETYPE</label>'
        }, {
            field: 'CURVALUE',
            title: '<label set-lan="html:CURVALUE">CURVALUE</label>'
        }, {
            field: 'RESETSN_FLAG',
            title: '<label set-lan="html:RESETSN_FLAG">RESETSN_FLAG</label>'
        }, {
            field: 'RESETVALUE_FLAG',
            title: '<label set-lan="html:RESETVALUE_FLAG">RESETVALUE_FLAG</label>'
        }, {
            field: 'CHECK_FLAG',
            title: '<label set-lan="html:CHECK_FLAG">CHECK_FLAG</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:EDIT_TIME">EDIT_TIME</label>'
        }, {
            field: 'EDIT_EMP',
            title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>'
        }, {
            field: 'VALUE10',
            title: '<label set-lan="html:VALUE10">VALUE10</label>'
        }],
        locale: tableLocale//中文支持,
    });

    $("#AddSnRule").on("click", function () {
        var rows = $("#SNRuleListTable").bootstrapTable("getSelections");
        if (rows.length > 0) {
            var RuleId = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
            var SkuId = SKU_EditRow.ID;
            $("#EditModelDetail .form-control").each(function () {
                SKU_EditRow[this.name] = this.value;
            });

            client.CallFunction("MESStation.Config.CSnRuleConfig", "AddSkuSnRule", { SkuId: SkuId, RuleId: RuleId }, function (e) {
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
        } else {

        }
    });

    //weight start
    $('#WeightListTable').bootstrapTable({
        data: SkuWeightData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        toolbar: "#WeightTable_Toolbar",
        search: false,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        onCheck: function (row) {
            var rows = $('#WeightListTable').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditWeight").attr("disabled", "disabled");
            } else {
                $("#EditWeight").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#DeleteWeight").attr("disabled", "disabled");
            } else {
                $("#DeleteWeight").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#WeightListTable').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditWeight").attr("disabled", "disabled");
            } else {
                $("#EditWeight").removeAttr("disabled")
            }
            if (rows.length <= 0) {
                $("#DeleteWeight").attr("disabled", "disabled");
            } else {
                $("#DeleteWeight").removeAttr("disabled")
            }
        },
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#EditWeight").attr("disabled", "disabled");
            $("#DeleteWeight").removeAttr("disabled")
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#EditWeight").attr("disabled", "disabled");
            $("#DeleteWeight").attr("disabled", "disabled");
        },
        columns: [{
            checkbox: true
        }, {
            field: 'TYPE',
            title: '<label set-lan="html:TYPE">TYPE</label>'
        }, {
            field: 'SKUNO',
            title: '<label set-lan="html:LOT_QTY">SKUNO</label>'
        }, {
            field: 'PARTNO',
            title: '<label set-lan="html:PARTNO">PARTNO</label>'
        }, {
            field: 'MPN',
            title: '<label set-lan="html:MPN">MPN</label>'
        }, {
            field: 'MAXWEIGHT',
            title: '<label set-lan="html:MAXWEIGHT">MAXWEIGHT</label>'
        }, {
            field: 'AVGWEIGHT',
            title: '<label set-lan="html:AVGWEIGHT">AVGWEIGHT</label>'
        }, {
            field: 'MINWEIGHT',
            title: '<label set-lan="html:MINWEIGHT">MINWEIGHT</label>'
        }, {
            field: 'STATION',
            title: '<label set-lan="html:STATION">STATION</label>'
        }, {
            field: 'ENABLEFLAG',
            title: '<label set-lan="html:ENABLEFLAG">ENABLEFLAG</label>'
        }, {
            field: 'CREATETIME',
            title: '<label set-lan="html:CREATETIME">CREATETIME</label>'
        }, {
            field: 'CREATEBY',
            title: '<label set-lan="html:CREATEBY">CREATEBY</label>'
        }, {
            field: 'EDITTIME',
            title: '<label set-lan="html:EDITTIME">EDIT_TIME</label>'
        }, {
            field: 'EDITBY',
            title: '<label set-lan="html:EDITBY">EDITBY</label>'
        }],
        locale: tableLocale//中文支持,
    });
    $("#NewWeightsite").bootstrapValidator({
        excluded: [':disabled', ':hidden', ':not(:visible)'],
        feedbackIcons: {
            valid: 'glyphicon glyphicon-ok',
            invalid: 'glyphicon glyphicon-remove',
            validating: 'glyphicon glyphicon-refresh'
        },
        live: 'enabled',
        message: 'This value is not valid',
        //submitButtons: 'button[type="submit"]',
        //submitHandler: function (validator, form, submitButton) {
        //    client.CallFunction("MESStation.Config.SkuConfig", "RecordWeightConfig", getformobj("NewWeightsite"), function (e) {
        //        if (e.Status == "Pass") {
        //            layer.msg("OK," + e.Message, {
        //                icon: 1,
        //                time: 1500
        //            }, function () {
        //                //clearsitevalue("NewWeightsite");
        //                layer.close(layer.index - 1);
        //                Get_WeightList($("#weight_sku").val());
        //            });
        //        }
        //        else {
        //            layer.msg(e.Message, {
        //                icon: 2,
        //                time: 3000
        //            }, function () {
        //            });
        //        }
        //    });
        //},
        fields: {
            MAXWEIGHT: {
                validators: {
                    notEmpty: {},
                    regexp: {
                        regexp: /^\d+(\.\d+)?$/,
                        message: 'The value must be a number!'
                    }
                }
            },
            MINWEIGHT: {
                validators: {
                    notEmpty: {},
                    regexp: {
                        regexp: /^\d+(\.\d+)?$/,
                        message: 'The value must be a number!'
                    }
                }
            },
            AVGWEIGHT: {
                validators: {
                    notEmpty: {},
                    regexp: {
                        regexp: /^\d+(\.\d+)?$/,
                        message: 'The value must be a number!'
                    }
                }
            },
            STATION: {
                validators: {
                    notEmpty: {}
                }
            },
            PARTNO: {
                validators: {
                    notEmpty: {}
                }
                //validators: {
                //    callback: {
                //        callback: function (value, validator) {
                //            if ("PACKAGE,MPN".indexOf($("#NewWeightsite .form-control[name='TYPE']").val()) > -1 &&
                //                value === "")
                //                return false;
                //            else
                //                return true;
                //        }
                //    }
                //}
            },
            MPN: {
                validators: {
                    notEmpty: {}
                }
            }
        }

    })

    $("#SaveWeight").on("click", function () {
        $("#NewWeightsite").data("bootstrapValidator").validate();//手动触发全部验证
        var flag = $("#NewWeightsite").data("bootstrapValidator").isValid();//获取当前表单验证状态
        if (flag) {//验证通过
            //提交表单数据
            client.CallFunction("MESStation.Config.SkuConfig", "RecordWeightConfig", getformobj("NewWeightsite"), function (e) {
                if (e.Status == "Pass") {
                    layer.msg("OK," + e.Message, {
                        icon: 1,
                        time: 1500
                    }, function () {
                        //clearsitevalue("NewWeightsite");
                        layer.close(layer.index - 1);
                        Get_WeightList($("#weight_sku").val());
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

    $("#weighttype").on("change", function () {
        if ("MPN,PACKAGE".indexOf($("#weighttype").val()) === -1) {
            $('#divweightpartno').hide();
            $('#divweightmpn').hide();
        }
        else {
            $('#divweightpartno').show();
            $('#divweightmpn').show();
        }
    });
    $("#NewWeight").on("click", function () {
        $('#divweightpartno').hide();
        $('#divweightmpn').hide();
        layer.open({
            type: 1,
            title: 'Add Weight',
            scrollar: false,
            skin: 'layui-layer-rim', //加上边框
            area: ['30%', '70%'], //宽高
            content: $("#NewWeightsite"),
            success: function (layero, index) {
                $('#NewWeightsite').data('bootstrapValidator').resetForm(true);
            },
            end: function () {
                clearsitevalue("NewWeightsite", ["SKUNO"]);
                $('#NewWeightsite').data('bootstrapValidator').resetForm(true);
            }
        });
    });
    $("#EditWeight").on("click", function () {
        var selRows = $('#WeightListTable').bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("no records selected", {
                icon: 2,
                time: 1500
            }, function () {
            });
            return;
        }
        else if (selRows.length > 1) {
            layer.msg("Too many records selected", {
                icon: 2,
                time: 1500
            }, function () {
            });
            return;
        }
        layer.open({
            type: 1,
            title: 'Edit Weight',
            scrollar: false,
            skin: 'layui-layer-rim', //加上边框
            area: ['30%', '70%'], //宽高
            content: $("#NewWeightsite"),
            success: function (layero, index) {
                $('#NewWeightsite').data('bootstrapValidator').resetForm(true);
                setformobj("NewWeightsite", selRows[0]);
                if ("MPN,PACKAGE".indexOf($("#weighttype").val()) === -1) {
                    $('#divweightpartno').hide();
                    $('#divweightmpn').hide();
                }
                else {
                    $('#divweightpartno').show();
                    $('#divweightmpn').show();
                }
            },
            end: function () {
                clearsitevalue("NewWeightsite", ["SKUNO"]);
                $('#NewWeightsite').data('bootstrapValidator').resetForm(true);
            }
        });
    });
    //weight end

    //Extend Config Begin
    $('#ExtendConfigTable').bootstrapTable({
        data: ExtendConfigData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        toolbar: "#ExtendConfigTable_Toolbar",
        maintainSelected: true,
        clickToSelect: true,
        onCheck: function (row) {
            var rows = $('#ExtendConfigTable').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditExtendConfig").attr("disabled", "disabled");
            } else {
                $("#EditExtendConfig").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#DeleteExtendConfig").attr("disabled", "disabled");
            } else {
                $("#DeleteExtendConfig").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#ExtendConfigTable').bootstrapTable('getSelections');
            if (rows.length > 1 || rows.length <= 0) {
                $("#EditExtendConfig").attr("disabled", "disabled");
            } else {
                $("#EditExtendConfig").removeAttr("disabled")
            }
            if (rows.length <= 0) {
                $("#DeleteExtendConfig").attr("disabled", "disabled");
            } else {
                $("#DeleteExtendConfig").removeAttr("disabled")
            }
        },
        onCheckAll: function (rowsAfter, rowsBefore) {
            $("#EditExtendConfig").attr("disabled", "disabled");
            $("#DeleteExtendConfig").removeAttr("disabled");
        },
        onUncheckAll: function (rowsAfter, rowsBefore) {
            $("#EditExtendConfig").attr("disabled", "disabled");
            $("#DeleteExtendConfig").attr("disabled", "disabled");
        },
        columns: [
            {
                checkbox: true
            }, {
                field: 'SKUNO',
                title: '<label set-lan="html:SKUNO">SKUNO</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true
            }, {
                field: 'CATEGORY',
                title: '<label set-lan="html:CATEGORY">CATEGORY</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true
            }, {
                field: 'CATEGORY_NAME',
                title: '<label set-lan="html:CATEGORY_NAME">CATEGORY_NAME</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true
            }, {
                field: 'VALUE',
                title: '<label set-lan="html:VALUE">VALUE</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true
            }, {
                field: "EXTEND",
                title: '<label set-lan="html:EXTEND">EXTEND</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true,
                visible: true
            }, {
                field: "VERSION",
                title: '<label set-lan="html:VERSION">VERSION</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true,
                visible: true
            }, {
                field: "BASETEMPLATE",
                title: '<label set-lan="html:BASETEMPLATE">BASETEMPLATE</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true,
                visible: true
            }, {
                field: "STATION_NAME",
                title: '<label set-lan="html:STATION_NAME">STATION_NAME</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true,
                visible: true
            }, {
                field: 'EDIT_EMP',
                title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true
            }, {
                field: 'EDIT_TIME',
                title: '<label set-lan="html:EDIT_TIME">EDIT_TIME</label>',
                rowspan: 1,
                align: 'center',
                valign: 'middle',
                sortable: true
            }],
        locale: tableLocale//中文支持,
    });
    $("#NewExtendConfig").on("click", function () {
        layer.open({
            type: 1,
            title: 'Edit Extend Config',
            scrollar: false,
            skin: 'layui-layer-rim', //加上边框
            area: ['35%', '75%'], //宽高
            content: $("#NewExtendConfigForm"),
            btn: ['Submit', 'Cancel'],
            success: function (layero, index) {
                $("#NewExtendConfigForm input").val("");
                $("#NewExtendConfigForm").removeClass("hidden");
            },
            yes: function (index, layero) {
                var postData = getformobj("NewExtendConfigForm");
                postData["SKUNO"] = SKU_EditRow.SKUNO;
                postData["VERSION"] = SKU_EditRow.VERSION;
                client.CallFunction("MESStation.Config.CSkuDetailConfig", "AddCSkuDetail", postData, function (e) {
                    if (e.Status == "Pass") {
                        layer.close(index);
                        layer.msg("OK," + e.Message, {
                            icon: 1,
                            time: 1500
                        }, function () {
                            Get_ExtendConfig(SKU_EditRow.SKUNO);
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
            },
            btn2: function (index, layero) {
                layer.close(index);
            },
            end: function () {
                $("#NewExtendConfigForm input").val("");
                $("#NewExtendConfigForm").addClass("hidden");
            }
        });
    });
    $("#EditExtendConfig").on("click", function () {
        var selRows = $('#ExtendConfigTable').bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("no records selected", {
                icon: 2,
                time: 1500
            }, function () {
            });
            return;
        }
        layer.open({
            type: 1,
            title: 'Edit Extend Config',
            scrollar: false,
            skin: 'layui-layer-rim', //加上边框
            area: ['35%', '75%'], //宽高
            content: $("#NewExtendConfigForm"),
            btn: ['Submit', 'Cancel'],
            success: function (layero, index) {
                $("#NewExtendConfigForm").removeClass("hidden");
                setformobj("NewExtendConfigForm", selRows[0]);
                $("#easyOption").hide();
            },
            yes: function (index, layero) {
                var postData = getformobj("NewExtendConfigForm");
                postData["SKUNO"] = SKU_EditRow.SKUNO;
                postData["VERSION"] = SKU_EditRow.VERSION;
                client.CallFunction("MESStation.Config.CSkuDetailConfig", "UdateCSkuDetail", postData, function (e) {
                    if (e.Status == "Pass") {
                        layer.close(index);
                        layer.msg("OK," + e.Message, {
                            icon: 1,
                            time: 1500
                        }, function () {
                            Get_ExtendConfig(SKU_EditRow.SKUNO);
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
            },
            btn2: function (index, layero) {
                layer.close(index);
            },
            end: function () {
                $("#NewExtendConfigForm input").val("");
                $("#NewExtendConfigForm").addClass("hidden");
                $("#easyOption").show();
            }
        });
    });
    $('#DeleteExtendConfig').on("click", function () {
        var selRows = $('#ExtendConfigTable').bootstrapTable('getSelections');
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
                    parent.client.CallFunction("MESStation.Config.CSkuDetailConfig", "DeleteCSkuDetailByIDList", { ID_List: RowsID },
                        function (e) {
                            if (e.Status == "Pass") {
                                layer.msg("OK，" + e.Message, {
                                    icon: 1,
                                    time: 3000
                                }, function () {
                                    Get_ExtendConfig(SKU_EditRow.SKUNO);
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

    $('#easyOption').on('click', 'button', function () {
        var extobj = $.extend(true, {}, ExtendEasyOption.find(t => t.ID == this.id.substr(4)));
        delete extobj.ID;
        delete extobj.NAME;
        setformobj("NewExtendConfigForm", extobj);
    });
    //Extend Config End

    Get_SkuList();
    Get_Serial();
    Get_PackType();
    Get_TransportType();
    Get_SNRule();
    Get_LabelList();
    Get_LabelType();
    Get_AqlType();
    Get_AllSnRuleData();
    Get_ExtendEasyOption();
    mesUI.SetLanguage("SKUSetting");
});


