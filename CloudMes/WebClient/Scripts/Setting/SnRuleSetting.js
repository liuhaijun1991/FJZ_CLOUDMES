var client = null;
var PageIndex = null;
var RuleId = null;
var mesUI;
var lan;
var tableLocale = "";
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
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetSNRuleDetailById", { "RuleId": RuleId}, function (e) {
            if (e.Status == "Pass") {
                $("#SNRuleDetailTable").bootstrapTable('load', e.Data);
            } else {
                $("#SNRuleDetailTable").bootstrapTable('load', []);
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    }

    var Get_CodeType = function () {
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetCodeType", {}, function (e) {
            var CodeOptions = $("#SNRuleDetailForm select[name=CodeType]");
            CodeOptions.empty();
            if (e.Status == "Pass") {
                for (var a = 0; a < e.Data.length; a++) {
                    CodeOptions.append($("<option value='" + e.Data[a] + "'>" + e.Data[a] + "</option>"));
                }
            }
        });
    }

    $('#SNRuleListTable').bootstrapTable({
        pagination: true,
        pageSize: 5,
        pageList: [5, 10, 20, 50],
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
        toolbar: "#SnRuleTool",
        locale: tableLocale,
        columns: [{
            title: '<label set-lan="html:SELECT">SELECT</label>',
            checkbox: true
        }, {
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
                RuleId = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
                if (typeof (RuleId) == 'undefined') {
                    return false;
                } else {
                    Get_SnRuleDetailData(RuleId);
                }
            }

            if (rows.length == 0) {
                $("#UpdateSnRule").attr("disabled", "disabled");
                $("#AddSnRule").removeAttr("disabled");
                $("#AddSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRule").attr("disabled", "disabled");
            } else {
                $("#UpdateSnRule").removeAttr("disabled");
                $("#AddSnRule").attr("disabled","disabled");
                $("#AddSnRuleDetail").removeAttr("disabled");
                $("#DeleteSnRule").removeAttr("disabled")
            }
        },
        onUncheck: function (row) {
            var rows = $('#SNRuleListTable').bootstrapTable('getSelections');

            if (rows.length == 0) {
                $('#SNRuleDetailTable').bootstrapTable('load', []);
                $("#UpdateSnRule").attr("disabled", "disabled");
                $("#AddSnRuleDetail").attr("disabled", "disabled");
                $("#UpdateSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRule").attr("disabled", "disabled");
                $("#AddSnRule").removeAttr("disabled");
            } else {
                $("#UpdateSnRule").removeAttr("disabled")
                $("#DeleteSnRule").removeAttr("disabled")
                

            }
        }

    });
    $('#SNRuleDetailTable').bootstrapTable({
        pagination: true,
        pageSize: 10,
        pageList: [5, 10, 15, 20],
        search: true,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        //maintainSelected: true,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#SnRuleDetailTool",
        locale: tableLocale,
        columns: [{
            title: '<label set-lan="html:SELECT">SELECT</label>',
            checkbox: true
        }, {
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
                field: 'RESETVALUE',
                title: '<label set-lan="html:RESETVALUE_FLAG">RESETVALUE</label>'
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
        onCheck: function (row) {
            var rows = $('#SNRuleDetailTable').bootstrapTable('getSelections');

            if (rows.length > 0) {
                $("#AddSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRuleDetail").removeAttr("disabled");
                $("#UpdateSnRuleDetail").removeAttr("disabled");
                if (rows.length > 1) {
                    $("#UpdateSnRuleDetail").attr("disabled", "disabled");
                }
            }
            else {
                $("#AddSnRuleDetail").removeAttr("disabled");
                $("#UpdateSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRuleDetail").attr("disabled", "disabled");
            }

        },
        onUncheck: function (row) {
            var rows = $('#SNRuleDetailTable').bootstrapTable('getSelections');

            if (rows.length > 0) {
                $("#AddSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRuleDetail").removeAttr("disabled");
                $("#UpdateSnRuleDetail").removeAttr("disabled");
                if (rows.length > 1) {
                    $("#UpdateSnRuleDetail").attr("disabled", "disabled");
                }
            }
            else {
                $("#AddSnRuleDetail").removeAttr("disabled");
                $("#UpdateSnRuleDetail").attr("disabled", "disabled");
                $("#DeleteSnRuleDetail").attr("disabled", "disabled");
            }
        }
    });

    $("#SubmitSnRule").on("click", function () {
        SubmitRule();
        layer.close(PageIndex);
    });

    $("#SubmitSnRuleDetail").on("click", function () {
        SubmitRuleDetail();
        layer.close(PageIndex);
    });

    $("#AddSnRule").on("click", function () {
        $("#CRuleName").val("");
        $("#CPrefix").val("");
        $("#CSkuNo").val("");
        layer.open({
            type: 1,
            skin: 'layui-layer-rim', //加上边框
            area: ['30%', '40%'], //宽高
            content: $("#SnRuleCopyForm"),
            title:"Copy Sn Rule",
            success: function (layero, index) {
                PageIndex = index;
                $("#SnRuleCopyForm").removeClass("hidden");
            }
        });
    });

    $("#SubmitCopy").on("click", function () {
        if ($("#CRuleName").val().length == 0) {
            layer.msg("Please input a new RuleName!", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }
        if ($("#CPrefix").val().length == 0) {
            layer.msg("Please input the Prefix!", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }
        if ($("#CSkuNo").val().length == 0) {
            layer.msg("Please input the SKUNO which you want to Copy!", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }
        client.CallFunction("MESStation.Config.CSnRuleConfig", "AddRuleByCopySku", {
            "CRuleName": $("#CRuleName").val(), "CPrefix": $("#CPrefix").val(), "CSkuNo": $("#CSkuNo").val()
        }, function (e) {
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon: 1,
                    time: 3000
                }, function () { });
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () { });
            }
        });
    });

    $("#UpdateSnRule").on("click", function () {
        //var RuleId = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetRuleById", { "RuleId": RuleId }, function (e) {
            if (e.Status == "Pass") {
                $("#RuleName").val(e.Data.NAME);
            }
            else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 2000
                }, function () {
                });
               
            }
        });

        layer.open({
            type: 1,
            skin: 'layui-layer-rim', //加上边框
            area: ['30%', '20%'], //宽高
            content: $("#SnRuleEditForm"),
            title: "Modify Rule Name",
            success: function (layero, index) {
                PageIndex = index;
                $("#SnRuleEditForm").removeClass("hidden");
            }
        });
    });

    $("#DeleteSnRule").on("click", function () {
        //var RuleId = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
        layer.open({
            title: 'Warning',
            btn: ['Delete', 'Cancel'],
            content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
            yes: function (index) {
                layer.close(index);
                client.CallFunction("MESStation.Config.CSnRuleConfig", "DeleteRule", { "RuleId": RuleId }, function (e) {
                    if (e.Status == "Pass") {
                        layer.msg("OK" + e.Message, {
                            icon: 1,
                            time: 2000
                        }, function () {
                        });
                        RefreshRule();
                    } else {
                        layer.msg(e.Message, {
                            icon: 2,
                            time: 2000
                        }, function () {
                        });
                    }
                });
            }
        });        
    });

    $("#AddSnRuleDetail").on("click", function () {
            layer.open({
                type: 1,
                skin: 'layui-layer-rim', //加上边框
                area: ['30%', '45%'], //宽高
                content: $("#SNRuleDetailForm"),
                title: "Add Sn Rule Detail",
                success: function (layero, index) {
                    PageIndex = index;
                    $("#SNRuleDetailForm").removeClass("hidden");
                }
            });
    });

    $("#UpdateSnRuleDetail").on("click", function () {
        var RuleDetailId = $('#SNRuleDetailTable').bootstrapTable('getSelections')[0]["ID"];
        client.CallFunction("MESStation.Config.CSnRuleConfig", "GetRuleDetailByDetailId", { "RuleDetailId": RuleDetailId }, function (e) {
            if (e.Status == "Pass") {
                $('#InputType').val(e.Data.INPUTTYPE);
                $("#CodeType").val(e.Data.CODETYPE);
                if (e.Data.RESETSN_FLAG == "1")
                    $("#ResetFlag").prop("checked",true);
                if (e.Data.CHECK_FLAG == "1")
                    $("#CheckFlag").prop("checked", true);
                $("#ResetValue").val(e.Data.RESETVALUE);
                $("#CurrentValue").val(e.Data.CURVALUE);
                $("#SeqNo").val(e.Data.SEQ);
            }
            else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 2000
                }, function () {
                });

            }
        });

        layer.open({
            type: 1,
            skin: 'layui-layer-rim', //加上边框
            area: ['30%', '50%'], //宽高
            content: $("#SNRuleDetailForm"),
            title: "Update Sn Rule Detail",
            success: function (layero, index) {
                PageIndex = index;
                $("#SNRuleDetailForm").removeClass("hidden");
            }
        });
    });

    $("#DeleteSnRuleDetail").on("click", function () {
        var RuleDetailId = $('#SNRuleDetailTable').bootstrapTable('getSelections')[0]["ID"];        
        layer.open({
            title: 'Warning',
            btn: ['Delete', 'Cancel'],
            content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
            yes: function (index) {
                layer.close(index);
                client.CallFunction("MESStation.Config.CSnRuleConfig", "DeleteRuleDetail", { "RuleDetailId": RuleDetailId }, function (e) {
                    if (e.Status == "Pass") {
                        RefreshSnRuleDetail();
                    } else {
                        layer.msg(e.Message, {
                            icon: 2,
                            time: 2000
                        }, function () {
                        });
                    }

                });
            }
        }); 
    });

    function SubmitRule()
    {
        var RuleName = $("#RuleName").val();
        if (RuleName == "") {
            layer.msg("RuleName Can't Be Empty!", {
                icon: 2,
                time: 2000
            }, function () { });
            return;
        }
        var rows = $('#SNRuleListTable').bootstrapTable('getSelections');        
        if (rows.length > 0) {
            var RuleId = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
            client.CallFunction("MESStation.Config.CSnRuleConfig", "UpdateRule", { "RuleId": RuleId, "RuleName": RuleName }, function (e) {
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
        }
        else {
            client.CallFunction("MESStation.Config.CSnRuleConfig", "AddRule", { "RuleName": RuleName }, function (e) {
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
        }
        RefreshRule();
    }

    function SubmitRuleDetail() {
        var rows = $('#SNRuleDetailTable').bootstrapTable('getSelections');
        //var row = $("#SNRuleListTable").bootstrapTable("getSelections")[0];
        //var RuleId = $("#SNRuleListTable").bootstrapTable("getSelections")[0]["ID"];
        var RuleDetail = {};

        if (rows.length > 0) {
            RuleDetail = rows[0];
        }

        RuleDetail["C_SN_RULE_ID"] = RuleId;
        RuleDetail["INPUTTYPE"] = $('#InputType').val();
        RuleDetail["CODETYPE"] = $("#CodeType").val();
        RuleDetail["RESETSN_FLAG"] = $("#ResetFlag").prop("checked") ? "1" : "0";
        RuleDetail["RESETVALUE"] = $("#ResetValue").val();
        RuleDetail["CHECK_FLAG"] = $("#CheckFlag").prop("checked") ? "1" : "0";
        RuleDetail["CURVALUE"] = $("#CurrentValue").val();
        RuleDetail["VALUE10"] = $("#CurrentValue").val();
        RuleDetail["SEQ"] = $("#SeqNo").val();

        if (rows.length > 0) {
            RuleDetail["ID"] = $('#SNRuleDetailTable').bootstrapTable('getSelections')[0]["ID"];
            client.CallFunction("MESStation.Config.CSnRuleConfig", "UpdateRuleDetail", { "RuleDetail":RuleDetail }, function (e) {
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
            client.CallFunction("MESStation.Config.CSnRuleConfig", "AddRuleDetail", { "RuleDetail": RuleDetail }, function (e) {
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
        }
        RefreshSnRuleDetail();
        RefreshRule();
        //row.checked = true;
    }

    function RefreshSnRuleDetail() {
        var rows = $('#SNRuleListTable').bootstrapTable('getSelections');
        if (rows.length > 0) {
            var RuleId = $('#SNRuleListTable').bootstrapTable('getSelections')[0]["ID"];
            Get_SnRuleDetailData(RuleId);
            mesUI.SetLanguage("SNRuleSetting");
        }
    }

    function RefreshRule()
    {
        Get_AllSnRuleData();
        $("#AddSnRule").removeAttr("disabled");
        $("#UpdateSnRule").attr("disabled", "disabled");
        $("#DeleteSnRule").attr("disabled", "disabled");
        $("#AddSnRuleDetail").attr("disabled", "disabled");
        $("#UpdateSnRuleDetail").attr("disabled", "disabled");
        $("#DeleteSnRuleDetail").attr("disabled", "disabled");
        $('#SNRuleDetailTable').bootstrapTable('load', []);
        mesUI.SetLanguage("SNRuleSetting");
    }

    Get_AllSnRuleData();
    Get_CodeType();
    mesUI.SetLanguage("SNRuleSetting");
});