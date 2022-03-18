var LabelTypeData = [];
var LabelTYpeName = "";
var ConfigData = {
    Keys: {},
    Functions: []
};
var JsonCode = "";
var Inputs = [];
var NotOutputKeys = [];
var Keys = [];
var Functions = [];
var GroupList = [];
var FunctionList = [];
var CurrentFunction = {};

var EditFlag = false;
var EditIndex = null;
var EditItem = null;

var operateFormatter = function (value, row, index) {
    return [
        '<button type="button" class="EditRow btn btn-primary btn-sm" style="margin-right:15px;">Edit</button>',
        '<button type="button" class="DeleteRow btn btn-danger btn-sm" style="margin-right:15px;">Delete</button>',
    ].join('');
};

window.operateEvents = {
    'click .EditRow': function (e, value, row, index) {
        EditFlag = true;
        EditIndex = index;
        EditItem = row;
        if (row.hasOwnProperty("StationSessionKey")) {
            layer.open(ShowNewInput);
        } else if (row.hasOwnProperty("Key")) {
            layer.open(ShowNewKey);
        } else {
            layer.open(ShowNewFunction);
        }
    },
    'click .DeleteRow': function (e, value, row, index) {
        if (row.hasOwnProperty("StationSessionKey")) {
            Inputs.splice(index, 1);
            $("#Inputs_Table").bootstrapTable('refreshOptions', {
                data: Inputs,
            });
        } else if (row.hasOwnProperty("Key")) {
            Keys.splice(index, 1);
            $("#Keys_Table").bootstrapTable('refreshOptions', {
                data: Keys,
            });
        } else {
            Functions.splice(index, 1);
            $("#FunctionList_Table").bootstrapTable('refreshOptions', {
                data: Functions,
            });
        }
    },
    'click .NotOutput': function (e, value, row, index) {
        var v = "";
        if (row.hasOwnProperty("StationSessionKey")) {
            v = row["Name"];
            if (e.target.checked) {
                NotOutputKeys.push(v);
            } else {
                NotOutputKeys.splice(NotOutputKeys.indexOf(v), 1);
            }
            $("#Inputs_Table").bootstrapTable('refreshOptions', {
                data: Inputs,
            });
        } else if (row.hasOwnProperty("Key")) {
            v = row["Key"];
            if (e.target.checked) {
                NotOutputKeys.push(v);
            } else {
                NotOutputKeys.splice(NotOutputKeys.indexOf(v), 1);
            }
            $("#Keys_Table").bootstrapTable('refreshOptions', {
                data: Keys,
            });
        } else {
            v = row["OutPutKey"];
            if (e.target.checked) {
                NotOutputKeys.push(v);
            } else {
                NotOutputKeys.splice(NotOutputKeys.indexOf(v), 1);
            }
            $("#FunctionList_Table").bootstrapTable('refreshOptions', {
                data: Functions,
            });
        }
    }
};

var GetLabelTypeData = function () {
    layer.load(3);
    parent.client.CallFunction("MESStation.Label.API.ConfigLabelAPI", "GetConfigLabelTypeList", {}, function (e) {
        layer.closeAll('loading');
        if (e.Status == "Pass") {
            LabelTypeData = e.Data;
            $('#LabelType_Table').bootstrapTable('load', LabelTypeData);
        } else {
            layer.msg(e.Message, {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });
}

var GetGroupList = function () {
    $('#cmb_Group').empty();
    layer.load(3);
    parent.client.CallFunction("MESStation.Label.API.ConfigLabelAPI", "GetLabelValueGroupNameList", {}, function (e) {
        layer.closeAll('loading');
        if (e.Status == "Pass") {
            GroupList = e.Data;
            var g = $('#cmb_Group');
            for (var i = 0; i < e.Data.length; i++) {
                g.append($("<option>" + e.Data[i] + "</option>"));
            }
            GetFunctionList(GroupList[0]);
        } else {
            layer.msg(e.Message, {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });
};

var GetFunctionList = function (GroupName, EditFunction) {
    $('#cmb_Function').empty();
    layer.load(3);
    var EditF = EditFunction;
    parent.client.CallFunction("MESStation.Label.API.ConfigLabelAPI", "GetLabelValueGroup", { GroupName: GroupName }, function (e) {
        layer.closeAll('loading');
        if (e.Status == "Pass") {
            var f = $('#cmb_Function');
            FunctionList = e.Data;
            for (var i = 0; i < e.Data.length; i++) {
                f.append($("<option>" + e.Data[i].FunctionName + "-" + e.Data[i].Description + "</option>"));
            }
            if (EditF) {
                var n = FunctionList.filter(function (item) { return item.FunctionName == EditF.FunctionName });
                if (n.length > 0) {
                    CurrentFunction = n[0];
                    f.val(CurrentFunction.FunctionName + "-" + CurrentFunction.Description);
                } else {
                    CurrentFunction = FunctionList[0];
                }
            } else {
                CurrentFunction = FunctionList[0];
            }
            $('#Group_Params').empty();
            for (var i = 0; i < CurrentFunction.Paras.length; i++) {
                $('#Group_Params').append('<label for="Params_' + CurrentFunction.Paras[i] + '" class="col-xs-4">' + CurrentFunction.Paras[i] + ':</label><div class="col-xs-8" id="Group_Params"><input class="form-control" id="Params_' + CurrentFunction.Paras[i] + '" value="' + (EditF ? EditF.ParaNames[i] : CurrentFunction.Paras[i]) + '" /></div>');
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

var Get_LabelList = function () {
    parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabelList", {}, function (e) {
        try {
            $("#LabelName").autocomplete("destroy");
        } catch (e) {

        }
        if (e.Status == "Pass") {
            var data = [];
            for (var i = 0; i < e.Data.length; i++) {
                data.push(e.Data[i].LABELNAME);
            }
            $("#LabelName").autocomplete({
                minLength: 0,
                source: data,
                select: function (event, ui) {
                    $(this).val(ui.item.value);
                    $("#loadingImg").css("display", "");
                    parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabFilePreview", { NAME: ui.item.value }, function (e) {
                        if (e.Status == "Pass") {
                            $("#loadingImg").css("display", "none");
                            $("#uImg").attr('src', '../../' + e.Data + "?ver=" + Date.now());
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

var ShowNewLabel = {
    id: "ShowNewLabel",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "New Label Type",
    area: ['95vw', '95vh'],
    content: $("#NewLabelTypePanel"),
    btn: ["Submit", "cancel"],
    success: function (layero, index) {
        layer.closeAll('loading');
        $('#txt_TypeName').val(LabelTYpeName);
        $("#Keys_Table").bootstrapTable('refreshOptions', {
            data: Keys,
        });
        $("#Inputs_Table").bootstrapTable('refreshOptions', {
            data: Inputs,
        });
        $("#FunctionList_Table").bootstrapTable('refreshOptions', {
            data: Functions,
        });
        $("#NewLabelTypePanel").removeClass("hidden");
        GetGroupList();
    },
    end: function () {
        $("#NewLabelTypePanel").addClass("hidden");
    },
    yes: function (index) {
        layer.load(3);
        ConfigData.Inputs = $.extend(true, [], Inputs);
        ConfigData.Functions = $.extend(true, [], Functions);
        ConfigData.NotOutputKeys = $.extend(true, [], NotOutputKeys);
        var ks = {};
        for (var i = 0; i < Keys.length; i++) {
            ks[Keys[i].Key] = Keys[i].Value;
        }
        ConfigData.Keys = $.extend(true, {}, ks);
        parent.client.CallFunction("MESStation.Label.API.ConfigLabelAPI", "UpdateConfigLabelType", { TypeName: $('#txt_TypeName').val(), JsonData: ConfigData }, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                layer.close(index);
                layer.msg("Success", {
                    icon: 1,
                    time: 3000
                }, function () {
                    GetLabelTypeData();
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
        layer.close(index);
    }
};

var ShowJsonCode = {
    id: "ShowJsonCode",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "Json Code",
    area: ['95vw', '95vh'],
    content: $("#ShowJsonPanel"),
    btn: ["Copy", "Close"],
    success: function (layero, index) {
        layer.closeAll('loading');
        $("#ShowJsonPanel").removeClass("hidden");
    },
    end: function () {
        $("#ShowJsonPanel").addClass("hidden");
    },
    yes: function (index) {
        try {
            window.clipboardData.setData("Text", JsonCode);
        } catch (e) {
            $("#JsonText").select();
            document.execCommand("copy");
            window.getSelection().empty();
        }
        layer.msg("Copy success!", { time: 2000 });
    },
    cancel: function (index) {
        layer.close(index);
    }
};

var ShowPasteJsonCode = {
    id: "ShowPasteJsonCode",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "Json Code",
    area: ['95vw', '95vh'],
    content: $("#ShowJsonPanel"),
    btn: ["Submit", "Close"],
    success: function (layero, index) {
        layer.closeAll('loading');
        $("#ShowJsonPanel").removeClass("hidden");
        $("#JsonText").val("");
    },
    end: function () {
        $("#ShowJsonPanel").addClass("hidden");
    },
    yes: function (index, layero) {
        var jsonstr = $("#JsonText").val();
        ConfigData = JSON.parse(jsonstr);
        Functions = $.extend(true, [], ConfigData.Functions);
        Inputs = $.extend(true, [], ConfigData.Inputs);
        Keys = [];
        for (var item in ConfigData.Keys) {
            Keys.push({ Key: item, Value: ConfigData.Keys[item] });
        }
        $("#Keys_Table").bootstrapTable('refreshOptions', {
            data: Keys,
        });
        $("#Inputs_Table").bootstrapTable('refreshOptions', {
            data: Inputs,
        });
        $("#FunctionList_Table").bootstrapTable('refreshOptions', {
            data: Functions,
        });
        GetGroupList();
        layer.close(index);
    },
    cancel: function (index) {
        $("#JsonText").val("");
        layer.close(index);
    }
};

var ShowNewInput = {
    id: "NewInputPanel",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "New Input",
    area: ['400px', '350px'],
    content: $("#NewInputPanel"),
    btn: ["Submit", "cancel"],
    success: function (layero, index) {
        if (EditFlag) {
            $('#txt_IName').val(EditItem.Name);
            $('#txt_IKey').val(EditItem.StationSessionKey);
            $('#txt_IType').val(EditItem.StationSessionType);
            $('#txt_IValue').val(EditItem.StationSessionValue);
        }
        $("#NewInputPanel").removeClass("hidden");
    },
    end: function () {
        $("#NewInputPanel").addClass("hidden");
        EditFlag = false;
        EditIndex = null;
        EditItem = null;
    },
    yes: function (index) {
        if ($('#txt_IName').val().trim().length == 0) {
            layer.msg('Input Name Cannot be empty!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#txt_IName').focus();
            });
            return;
        } else if ($('#txt_IKey').val().trim().length == 0) {
            layer.msg('Session Key Cannot be empty!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#txt_IKey').focus();
            });
            return;
        } else if ($('#txt_IType').val().trim().length == 0) {
            layer.msg('Session Type Cannot be empty!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#txt_IType').focus();
            });
            return;
        } else {
            var ips = $.extend(true, [], Inputs);
            if (EditFlag) {
                ips.splice(EditIndex, 1);
            }
            var fs = ips.filter(function (item) { return item.Name == $('#txt_IName').val().trim() });
            if (fs.length > 0) {
                layer.msg('Duplicate Input Name!', {
                    icon: 2,
                    time: 3000
                }, function () {
                    $('#txt_IName').focus();
                });
                return;
            }
        }

        var ip = {
            Name: $('#txt_IName').val().trim(),
            StationSessionKey: $('#txt_IKey').val().trim(),
            StationSessionType: $('#txt_IType').val().trim(),
            StationSessionValue: $('#txt_IValue').val().trim(),
            Type: ''
        };
        if (EditFlag) {
            Inputs.splice(EditIndex, 1, ip);
        } else {
            Inputs.push(ip);
        }
        $("#Inputs_Table").bootstrapTable('refreshOptions', {
            data: Inputs,
        });
        layer.close(index);
    },
    cancel: function (index) {
        layer.close(index);
    }
};

var ShowNewKey = {
    id: "NewKeyPanel",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "New Data Key",
    area: ['400px', '250px'],
    content: $("#NewKeyPanel"),
    btn: ["Submit", "cancel"],
    success: function (layero, index) {
        if (EditFlag) {
            $('#txt_key').val(EditItem.Key);
            $('#txt_value').val(EditItem.Value);
        }
        $("#NewKeyPanel").removeClass("hidden");
    },
    end: function () {
        $("#NewKeyPanel").addClass("hidden");
        EditFlag = false;
        EditIndex = null;
        EditItem = null;
    },
    yes: function (index) {
        if ($('#txt_key').val().trim().length == 0) {
            layer.msg('Key Name Cannot be empty!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#txt_key').focus();
            });
            return;
        }
        var ks = Keys.filter(function (item) { return item.Key == $('#txt_key').val().trim() });
        if (ks.length == 0) {
            var key = { Key: $('#txt_key').val().trim(), Value: $('#txt_value').val().trim() };
            if (EditFlag) {
                Keys.splice(EditIndex, 1, key);
            } else {
                Keys.push(key);
            }
        } else {
            ks[0].Value = $('#txt_value').val().trim();
        }
        $("#Keys_Table").bootstrapTable('refreshOptions', {
            data: Keys,
        });
        layer.close(index);
    },
    cancel: function (index) {
        layer.close(index);
    }
};

var ShowNewFunction = {
    id: "NewFunctionPanel",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "New Output",
    area: ['650px', '450px'],
    content: $("#NewFunctionPanel"),
    btn: ["Submit", "cancel"],
    success: function (layero, index) {
        $("#NewFunctionPanel").removeClass("hidden");
        $('#Group_Params').empty();
        for (var i = 0; i < CurrentFunction.Paras.length; i++) {
            $('#Group_Params').append('<label for="Params_' + CurrentFunction.Paras[i] + '" class="col-xs-4">' + CurrentFunction.Paras[i] + ':</label><div class="col-xs-8" id="Group_Params"><input class="form-control" id="Params_' + CurrentFunction.Paras[i] + '" value="' + CurrentFunction.Paras[i] + '" /></div>');
        }
        if (EditFlag) {
            $('#txt_OutputName').val(EditItem.OutPutKey);
            $('#cmb_Group').val(EditItem.Class.substring(EditItem.Class.lastIndexOf('.') + 1));
            GetFunctionList(EditItem.Class.substring(EditItem.Class.lastIndexOf('.') + 1), EditItem);
        }
    },
    end: function () {
        $("#NewFunctionPanel").addClass("hidden");
        EditFlag = false;
        EditIndex = null;
        EditItem = null;
    },
    yes: function (index) {
        if ($('#txt_OutputName').val().trim().length == 0) {
            layer.msg('Output Key Cannot be empty!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#txt_OutputName').focus();
            });
            return;
        } else if ($('#cmb_Group').val().length == 0) {
            layer.msg('You need to select an Group option!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#cmb_Group').focus();
            });
            return;
        } else if ($('#cmb_Function').val().length == 0) {
            layer.msg('You need to select an Function option!', {
                icon: 2,
                time: 3000
            }, function () {
                $('#cmb_Function').focus();
            });
            return;
        } else {
            var fl = $.extend(true, [], Functions);
            if (EditFlag) {
                fl.splice(EditIndex, 1);
            }
            //var fs = fl.filter(function (item) { return item.OutPutKey == $('#txt_OutputName').val().trim() });
            //if (fs.length > 0) {
            //    layer.msg('Duplicate output key!', {
            //        icon: 2,
            //        time: 3000
            //    }, function () {
            //        $('#txt_OutputName').focus();
            //    });
            //    return;
            //}
        }
        var params = [];
        for (var i = 0; i < CurrentFunction.Paras.length; i++) {
            var pid = $("#Params_" + CurrentFunction.Paras[i]);
            if ($(pid).val().trim().length == 0) {
                layer.msg('Params ' + CurrentFunction.Paras[i] + ' cannot be empty!', {
                    icon: 2,
                    time: 3000
                }, function () {
                    $(pid).focus();
                });
                return;
            }
            params.push($(pid).val());
        }
        for (var i = 0; i < params.length; i++) {
            var item = params[i];
            var ip = Inputs.filter(function (it) {
                return it.Name == item;
            });
            var k = Keys.filter(function (it) {
                return it.Key == item;
            });
            var f = Functions.filter(function (it) {
                return it.OutPutKey == item;
            });
            if (k < 0 && f.length == 0 && ip.length == 0) {
                layer.alert('This function need parameters of ' + item + ' ! You need to first create an Input or Key with the name ' + item + '!', {
                    icon: 2
                }, function (index) {
                    layer.close(index);
                    $('#cmb_Function').focus();
                });
                return;
            }
        }
        var fun = {
            Class: CurrentFunction.ClassName,
            FunctionName: CurrentFunction.FunctionName,
            ParaNames: params,
            OutPutKey: $('#txt_OutputName').val().trim()
        };
        if (EditFlag) {
            Functions.splice(EditIndex, 1, fun);
        } else {
            Functions.push(fun);
        }
        $("#FunctionList_Table").bootstrapTable('refreshOptions', {
            data: Functions,
        });
        layer.close(index);
    },
    cancel: function (index) {
        layer.close(index);
    }
};

var ShowTestLabel = {
    id: "ShowTestLabel",
    type: 1,
    shade: 0.8,
    shadeClose: false,
    title: "Test Label Function",
    area: ['600px', '100vh'],
    content: $("#TestLabelPanel"),
    btn: ["Test", "cancel"],
    success: function (layero, index) {
        $("#TestLabelPanel").removeClass("hidden");
    },
    end: function () {
        $("#TestLabelPanel").addClass("hidden");
    },
    yes: function (index, layero) {
        var LabelName = layero.find("#LabelName").val();
        ConfigData.Inputs = $.extend(true, [], Inputs);
        ConfigData.Functions = $.extend(true, [], Functions);
        var ks = {};
        for (var i = 0; i < Keys.length; i++) {
            ks[Keys[i].Key] = Keys[i].Value;
        }
        ConfigData.Keys = $.extend(true, {}, ks);
        $("#loadingImg").css("display", "");
        $("#uImg").attr('src', '');
        $("#Preview").attr('src', '');
        parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetLabValuePreview", { JsonData: ConfigData, NAME: LabelName }, function (e) {
            if (e.Status == "Pass") {
                $("#loadingImg").css("display", "none");
                $("#uImg").attr('src', '../../' + e.Data + "?ver=" + Date.now());
                setTimeout(function (data) {
                    var img = $("#Preview");
                    var w = $("#uImg").width();
                    var h = $("#uImg").height();
                    img.attr("src", "../../" + e.Data + "?ver=" + Date.now());
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
    cancel: function (index) {
        $("#LabelName").val('');
        $("#uImg").attr('src', '');
        $("#Preview").attr('src', '');
        layer.close(index);
    }
};

$(document).ready(function () {
    $('#DeleteLabelType').on('click', function () {
        var selRows = $('#LabelType_Table').bootstrapTable('getSelections');
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
                    parent.client.CallFunction("*", "Remove", { ListNames: Names }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("Success", {
                                icon: 1,
                                time: 3000
                            }, function () {
                                GetLabelTypeData();
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

    $('#AddLabelType').on('click', function () {
        LabelTYpeName = "";
        ConfigData = {
            Keys: {},
            Functions: []
        };
        Inputs = [];
        NotOutputKeys = [];
        Keys = [];
        Functions = [];
        GroupList = [];
        FunctionList = [];
        CurrentFunction = {};
        layer.open(ShowNewLabel);
    });

    $('#EditLabelType').on('click', function () {
        var selRows = $('#LabelType_Table').bootstrapTable('getSelections');
        if (selRows.length == 1) {
            layer.load(3);
            LabelTYpeName = selRows[0].NAME;
            parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetJsonDataById", { ID: selRows[0].CLASS }, function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    ConfigData = JSON.parse(e.Data);
                    Functions = $.extend(true, [], ConfigData.Functions);
                    Inputs = $.extend(true, [], ConfigData.Inputs);
                    NotOutputKeys = $.extend(true, [], ConfigData.NotOutputKeys);
                    Keys = [];
                    for (var item in ConfigData.Keys) {
                        Keys.push({ Key: item, Value: ConfigData.Keys[item] });
                    }
                    layer.open(ShowNewLabel);
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            });
        } else {
            layer.msg("Please select a record!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });

    $('#CopyLabelType').on('click', function () {
        var selRows = $('#LabelType_Table').bootstrapTable('getSelections');
        if (selRows.length == 1) {
            layer.load(3);
            LabelTYpeName = selRows[0].NAME + "_Copy";
            parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetJsonDataById", { ID: selRows[0].CLASS }, function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    ConfigData = JSON.parse(e.Data);
                    Functions = $.extend(true, [], ConfigData.Functions);
                    Inputs = $.extend(true, [], ConfigData.Inputs);
                    Keys = [];
                    for (var item in ConfigData.Keys) {
                        Keys.push({ Key: item, Value: ConfigData.Keys[item] });
                    }
                    layer.open(ShowNewLabel);
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            });
        } else {
            layer.msg("Please select a record!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });

    $('#CopyLabelTypeJson').on('click', function () {
        var selRows = $('#LabelType_Table').bootstrapTable('getSelections');
        if (selRows.length == 1) {
            layer.load(3);
            parent.client.CallFunction("MESStation.FileUpdate.FileUpload", "GetJsonDataById", { ID: selRows[0].CLASS }, function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    JsonCode = e.Data;
                    $("#JsonText").text(e.Data);
                    layer.open(ShowJsonCode);
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            });
        } else {
            layer.msg("Please select a record!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });

    $('#JsonPaste').on('click', function () {
        layer.open(ShowPasteJsonCode);
    });

    $('#LabelType_Table').bootstrapTable({
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
        toolbar: "#LabelType_Toolbar",
        onCheck: function (row) {
            $("#EditLabelType").removeAttr("disabled");
            $("#CopyLabelType").removeAttr("disabled");
            $("#CopyLabelTypeJson").removeAttr("disabled");
        },
        onUncheck: function (row) {
            $("#EditLabelType").attr("disabled", "disabled");
            $("#CopyLabelType").attr("disabled", "disabled");
            $("#CopyLabelTypeJson").attr("disabled", "disabled");
        },
        onRefresh: function (param) {
            GetLabelTypeData();
        },
        columns: [{
            checkbox: true
        }, {
            field: 'NAME',
            title: 'Type Name'
        }, {
            field: 'CLASS',
            title: 'JSON ID'
        }, {
            field: 'EDIT_TIME',
            title: 'Edit Time'
        }, {
            field: 'EDIT_EMP',
            title: 'Edit By'
        }],
        data: LabelTypeData
    });

    $('#Inputs_Table').bootstrapTable({
        striped: true,
        showRefresh: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#InputsList_Toolbar",
        onRefresh: function (param) {
            $("#Inputs_Table").bootstrapTable('refreshOptions', {
                data: Inputs,
            });
        },
        columns: [{
            checkbox: true
        }, {
            field: 'Name',
            title: 'Input Name'
        }, {
            field: 'StationSessionKey',
            title: 'Session Key'
        }, {
            field: 'StationSessionType',
            title: 'Session Type'
        }, {
            field: 'StationSessionValue',
            title: 'Session Value'
        }, {
            field: 'NotOutput',
            title: 'Not Output',
            events: "operateEvents",
            formatter: function (value, row, index) {
                if (NotOutputKeys.Contain(row["Name"])) {
                    return ['<input class="NotOutput checkbox" type="checkbox" checked />'].join('');
                } else {
                    return ['<input class="NotOutput checkbox" type="checkbox" />'].join('');
                }
            }
        }, {
            field: 'operate',
            title: 'Operation',
            align: 'center',
            width: '154',
            events: "operateEvents",
            formatter: operateFormatter
        }],
        data: Inputs
    });

    $('#Keys_Table').bootstrapTable({
        striped: true,
        showRefresh: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#KeysList_Toolbar",
        onRefresh: function (param) {
            $("#Keys_Table").bootstrapTable('refreshOptions', {
                data: Keys,
            });
        },
        columns: [{
            checkbox: true
        }, {
            field: 'Key',
            title: 'Key'
        }, {
            field: 'Value',
            title: 'Value'
        }, {
            field: 'NotOutput',
            title: 'Not Output',
            events: "operateEvents",
            formatter: function (value, row, index) {
                if (NotOutputKeys.Contain(row["Key"])) {
                    return ['<input class="NotOutput checkbox" type="checkbox" checked />'].join('');
                } else {
                    return ['<input class="NotOutput checkbox" type="checkbox" />'].join('');
                }
            }
        }, {
            field: 'operate',
            title: 'Operation',
            align: 'center',
            width: '154',
            events: "operateEvents",
            formatter: operateFormatter
        }],
        data: Keys
    });

    $('#FunctionList_Table').bootstrapTable({
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        striped: true,
        showRefresh: true,
        detailView: false,
        clickEdit: true,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#FuntionsList_Toolbar",
        onRefresh: function (param) {
            $("#FunctionList_Table").bootstrapTable('refreshOptions', {
                data: Functions,
            });
        },
        columns: [{
            checkbox: true
        }, {
            field: 'OutPutKey',
            title: 'Output Key'
        }, {
            field: 'Class',
            title: 'Class Name'
        }, {
            field: 'FunctionName',
            title: 'Function Name'
        }, {
            field: 'NotOutput',
            title: 'Not Output',
            events: "operateEvents",
            formatter: function (value, row, index) {
                if (NotOutputKeys.Contain(row["OutPutKey"])) {
                    return ['<input class="NotOutput checkbox" type="checkbox" checked />'].join('');
                } else {
                    return ['<input class="NotOutput checkbox" type="checkbox" />'].join('');
                }
            }
        }, {
            field: 'operate',
            title: 'Operation',
            align: 'center',
            width: '154',
            events: "operateEvents",
            formatter: operateFormatter
        }],
        data: Functions
    });


    GetLabelTypeData();

    Get_LabelList();

    $('#cmb_Group').on('change', function (e) {
        GetFunctionList(this.value);
    });

    $('#cmb_Function').on('change', function (e) {
        CurrentFunction = FunctionList[this.selectedIndex];
        $('#Group_Params').empty();
        for (var i = 0; i < CurrentFunction.Paras.length; i++) {
            $('#Group_Params').append('<label for="lab_Params" class="col-xs-4">' + CurrentFunction.Paras[i] + ':</label><div class="col-xs-8" id="Group_Params"><input class="form-control" id="Params_' + CurrentFunction.Paras[i] + '" value="' + CurrentFunction.Paras[i] + '" /></div>');
        }
    });

    $('#AddInput').on('click', function (e) {
        layer.open(ShowNewInput);
    });

    $('#AddFunction').on('click', function (e) {
        layer.open(ShowNewFunction);
    });

    $('#AddKey').on('click', function (e) {
        layer.open(ShowNewKey);
    });

    $('#TestLabel').on('click', function (e) {
        layer.open(ShowTestLabel);
    });

    $('#TestResult').on('click', function (e) {
        ConfigData.Inputs = $.extend(true, [], Inputs);
        ConfigData.Functions = $.extend(true, [], Functions);
        var ks = {};
        for (var i = 0; i < Keys.length; i++) {
            ks[Keys[i].Key] = Keys[i].Value;
        }
        ConfigData.Keys = $.extend(true, {}, ks);
        layer.load(3);
        parent.client.CallFunction("MESStation.Label.API.ConfigLabelAPI", "TestRunConfigLabel", { JsonData: ConfigData }, function (e) {
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                layer.open({
                    id: 'TestRes',
                    type: 0,
                    shade: 0.8,
                    title: "Test",
                    content: '<textarea class="form-control" style="height: 100%">' + JSON.stringify(e.Data.Outputs, null, 4) + '</textarea>',
                    area: ['60vw', '85vh'],
                    cancel: function (index) {
                        layer.close(index);
                    }
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
});