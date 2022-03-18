console.log("123456");
var KeyPartScan = function (o) {
    this.Client = o.Client;
    this.InitClassName = o.InitClassName ? o.InitClassName : "MESStation.KeyPart.KPScan";
    this.InitFunctionName = o.InitFunctionName ? o.InitFunctionName : "GetSNStationKPList";
    this.InputClassName = o.InputClassName ? o.InputClassName : "MESStation.KeyPart.KPScan";
    this.InputFunctionName = o.InputFunctionName ? o.InputFunctionName : "ScanKPItem";
    this.CallBackClass = o.CallBackClass ? o.CallBackClass : "MESStation.GlobalConfig.CallUIFunctionAPI";
    this.CallBackFunctionName = o.CallBackFunctionName ? o.CallBackFunctionName : "UIFunctionCallBack";
    this.SN = o.SN;
    this.ServerMessageID = o.ServerMessageID,
        this.KPInputData = o.KPInputData,
        this.WO = o.WO;
    this.StationName = o.StationName;
    this.KPListTable = o.KPListTable;
    this.ScanInput = o.ScanInput;
    //this.MPNSeletor = o.MPNSeletor; //20190108 patty commented out MPN selector
    this.PartNoSeletor = o.PartNoSeletor;
    this.ScanType = o.ScanType;
    this.KPS = [];
    this.KPSChange = [];
    this.ScanItem = [];
    //this.MPNs = []; //20190108 patty commented out MPN selector
    this.KPTrim = [];
    this.Repalce = {};
    this.KPRule = [];
    this.CurrentKeyPart = {};
    KeyPartScan.prototype.constructor = KeyPartScan;
    KeyPartScan.prototype.List = {};
    KeyPartScan.prototype.Init = function () {
        var MessageID = "KPINIT" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.List[MessageID] = this;
        this.Client.CallFunction(this.InitClassName, this.InitFunctionName, { SN: this.SN, WO: this.WO, STATION: this.StationName }, this.InitCallBack, MessageID);
    };//初始化Keypart数据
    KeyPartScan.prototype.InitCallBack = function (d) {
        var KPScan = KeyPartScan.prototype.List[d.MessageID];
        delete KeyPartScan.prototype.List[d.MessageID];
        if (d.Status == "Pass") {
            KPScan.KPS = d.Data.KPS;
            KPScan.KPSChange = $.extend(true, [], d.Data.KPS);
            KPScan.MPNs = d.Data.MPNs; //20190108 patty commented out MPN selector
            KPScan.Repalce = d.Data.Repalce;
            KPScan.KPRule = d.Data.KPRule;

            KPScan.KPTrim = d.Data.KPTrims;

            KPScan.InitKpList();
            KPScan.SetCurrentKP(true);
            KPScan.ScanInput.focus();
        } else {
            parent.layer.msg(d.Message, {
                icon: 2,
                time: 3000
            }, function () {
            });
            var lindex = parent.layer.getFrameIndex(window.name);
            parent.layer.close(lindex);
        }
    };//初始化Keypart数据回调函数
    KeyPartScan.prototype.SetCurrentKP = function (ItemOK) {
        var KeyParts = [];
        if (ItemOK) {
            KeyParts = this.KPS;
        }
        else {
            KeyParts = this.KPSChange;
        }
        for (var i = 0; i < KeyParts.length; i++) {
            if (KeyParts[i].VALUE == null || KeyParts[i].VALUE == "") {
                this.CurrentKeyPart = $.extend(true, {}, KeyParts[i]);
                break;
            }
        }
        this.InitKpList("load", KeyParts);
        if (!this.CheckOK()) {
            this.InitInput();
            this.ScollToCurrnetKP();
        }
        else {
            this.ScanInput.attr("disabled", "disabled");
            this.PartNoSeletor.attr("disabled", "disabled");
            //this.MPNSeletor.attr("disabled", "disabled"); //20190108 patty commented out MPN selector
        }
    };//查找并设置当前需要扫描KP内容
    KeyPartScan.prototype.ScollToCurrnetKP = function () {
        this.KPListTable.find("tr").each(function () { $(this).css("background-color", ""); });
        var objTr = this.KPListTable.find("tr[data-ITEMSEQ='" + this.CurrentKeyPart.ITEMSEQ + "'][data-SCANSEQ='" + this.CurrentKeyPart.SCANSEQ + "'][data-DETAILSEQ='" + this.CurrentKeyPart.DETAILSEQ + "']");
        objTr.css("background-color", "#0FF");
        //20190123 patty modified scrolltop offset to scroll to middle instead of top
        //$("#TableConten").animate({ scrollTop: objTr[0].offsetTop }, "fast");
        var elOffset = objTr.offset().top;
        var elHeight = objTr.height();
        var windowHeight = $(window).height();
        var offset;

        //if (elHeight < windowHeight) {
        //    offset = elOffset - ((windowHeight / 2) - (elHeight / 2));
        //}
        //else {
        //    offset = elOffset;
        //}

        offset = objTr[0].offsetTop - ((windowHeight / 2) - (elHeight / 2));
        var speed = 700;
        $("#TableConten").animate({ scrollTop: offset }, speed);
    };//KP列表跳轉到當前需要掃描項并高亮标注
    KeyPartScan.prototype.InitInput = function () {
        this.ScanInput.unbind("keypress");
        this.ScanInput.val("");
        this.ScanInput.bind("keypress", { KPScan: this }, function (event) {
            if (event.keyCode == 13) {
                var scandata = "";
                var tempdata = "";
                var KPScan = event.data.KPScan;
                //add condition here for mac KP scan vince_20191015
                var KPScanType = KPScan.CurrentKeyPart.SCANTYPE;

                // var KPTrims = event.data.KPtrim;
                for (var i = 0; i < KPScan.KPTrim.length; i++) {
                    if (KPScan.CurrentKeyPart.PARTNO == KPScan.KPTrim[i].PARTNO && KPScan.CurrentKeyPart.SCANTYPE == KPScan.KPTrim[i].SCANTYPE && this.value.slice(0, KPScan.KPTrim[i].PREFIXLEN) == KPScan.KPTrim[i].PREFIX) {
                        tempdata = this.value.substr(KPScan.KPTrim[i].LSTART, KPScan.KPTrim[i].LLEN);
                        break;
                    }
                }

                if (String(KPScanType).includes("MAC")) {
                    // scandata = this.value.slice(-12);
                    scandata = this.value.replace(/[:\s]/g, '').slice(-12); //changed by James Zhu 04/03/2020
                }
                else if (tempdata.length > 1) {
                    scandata = tempdata;
                }
                else {
                    scandata = this.value;
                }
                if (KPScan.RegExp(scandata)) { //change this.value to scandata for mac KP scan vince_20191015
                    KPScan.SetValue(this.id, scandata);
                    if (KPScan.CheckItemOK()) {
                        KPScan.SendData();
                    }
                    else {
                        KPScan.SetCurrentKP(false);
                    }
                }
                else {
                    parent.layer.msg("Input not match!", {
                        icon: 2,
                        time: 10000,
                        title: 'Error!',
                        btn: ['ok']

                    }, function () {
                        KPScan.HighlightScanValue();
                    });
                    //parent.layer.alert('Input not match! Please check KP rule setting!', { title: 'Error!', icon: 2, btn: ['OK'] }, function () {
                    //        KPScan.HighlightScanValue();
                    //    });

                }
            }
        });
        this.ScanType.val(this.CurrentKeyPart.SCANTYPE);
        this.InitPartNo();
        //this.InitMPN(); 20190108 patty commented out MPN selector
    };//初始化输入框
    KeyPartScan.prototype.InitKpList = function (load, data) {
        data = data ? data : this.KPS;
        if (load == "load") {
            this.KPListTable.bootstrapTable(load, data);
        }
        else {
            this.KPListTable.bootstrapTable({
                pagination: false,
                showHeader: false,
                rowStyle: function (row, index) {
                    if (row.VALUE != "" && row.VALUE != null) {
                        return { classes: "success" };
                    } else {
                        return { classes: "" };
                    }
                },
                rowAttributes: function (row, index) {
                    return {
                        "data-ITEMSEQ": row.ITEMSEQ,
                        "data-SCANSEQ": row.SCANSEQ,
                        "data-DETAILSEQ": row.DETAILSEQ
                    }
                },
                data: data,
                columns: [
                    {
                        field: 'PARTNO',
                        title: 'Part No',
                        class: 'vertical-align-middle'
                    }, {
                        field: 'KP_NAME',
                        title: 'KP Name',
                        class: 'vertical-align-middle'
                    }, {
                        field: 'ITEMSEQ',
                        title: 'ITEMSEQ',
                        class: 'vertical-align-middle'
                    },
                    {
                        field: 'MPN',
                        title: 'MPN',
                        class: 'vertical-align-middle'
                    },
                    {
                        field: 'LOCATION',
                        title: 'LOCATION',
                        class: 'vertical-align-middle'
                    },
                    {
                        field: 'SCANTYPE',
                        title: 'SCANTYPE'
                    }, {
                        field: 'VALUE',
                        title: 'KP Value'
                    }
                ]
            });
        }
        $.MES.mergeCells(data, "PARTNO", 1, this.KPListTable);
        $.MES.mergeCells(data, "KP_NAME", 1, this.KPListTable);
        $.MES.mergeCells(data, "ITEMSEQ", 1, this.KPListTable);
        $.MES.mergeCells(data, "MPN", 1, this.KPListTable); //patty added 20181226
    };//初始化KP显示列表
    KeyPartScan.prototype.InitPartNo = function () {
        this.PartNoSeletor.unbind("change");
        var key = this.CurrentKeyPart.PARTNO;
        this.PartNoSeletor.empty();
        if (this.Repalce.hasOwnProperty(key)) {
            for (var i = 0; i < this.Repalce[key].length; i++) {
                this.PartNoSeletor.append($("<option>" + this.Repalce[key][i].REPALCEPARTNO + "</option>"))
            }
        }
        if (key) {
            this.PartNoSeletor.append($("<option>" + key + "</option>"));
        }
        this.PartNoSeletor.bind("change", { KPScan: this }, function (event) {
            event.data.KPScan.SetValue(this.name, this.value);
            //event.data.KPScan.InitMPN();  //20190108 patty commented out MPN selector
        });
        this.CurrentKeyPart.PARTNO = this.PartNoSeletor.val();
    };//初始化PartNo选择框
    KeyPartScan.prototype.Cancel = function () {
        this.Client.CallFunction(this.CallBackClass, this.CallBackFunctionName, { StationLayerReturnType: StationLayerReturnType.Cancel, ServerMessageID: this.ServerMessageID, StationLayerData: this.KPInputData.ErrMessage }, null, this.ServerMessageID);
    };
    KeyPartScan.prototype.SetValue = function (Name, Value) {
        switch (Name) {
            case "ScanValue":
                this.CurrentKeyPart.VALUE = Value;
                break;
            case "MPN":
                this.CurrentKeyPart.MPN = Value;
                break;
            case "PartNo":
                this.CurrentKeyPart.PARTNO = Value;
                break;
            default:
                break;
        }
        for (var i = 0; i < this.KPSChange.length; i++) {
            if (this.KPSChange[i].ID == this.CurrentKeyPart.ID) {
                this.KPSChange.splice(i, 1, this.CurrentKeyPart);
                this.ScanItem.push(this.CurrentKeyPart);
            }
        }

    };//输入设置键值
    KeyPartScan.prototype.RegExp = function (value) {
        var flag = false;
        var REGEXArr = [];
        var currentMPN = this.MPNs.filter(t => t.PARTNO == this.CurrentKeyPart.PARTNO);
        REGEXArr = this.KPRule.filter(t => t.PARTNO == this.CurrentKeyPart.PARTNO && currentMPN.findIndex(element => element.MPN == t.MPN) >= 0 && t.SCANTYPE == this.CurrentKeyPart.SCANTYPE);

        for (var i = 0; i < REGEXArr.length; i++) {
            var reg = new RegExp(REGEXArr[i].REGEX);
            if (reg.test(value)) {
                flag = true;
                this.CurrentKeyPart.REGEX = REGEXArr[i].REGEX;
                this.CurrentKeyPart.MPN = REGEXArr[i].MPN;
            }
        }
        return flag;
    };//检查输入值与正则表达式是否匹配
    KeyPartScan.prototype.HighlightScanValue = function () {
        $("#ScanValue").select();

    };//20190123 patty added for: highligh scanvalue if value is not valid
    KeyPartScan.prototype.CheckItemOK = function () {
        var ItemKPS = [];
        for (var i = 0; i < this.KPSChange.length; i++) {
            if (this.KPSChange[i].ITEMSEQ == this.CurrentKeyPart.ITEMSEQ && this.KPSChange[i].SCANSEQ == this.CurrentKeyPart.SCANSEQ) {
                ItemKPS.push(this.KPSChange[i]);
            }
        }
        var flag = true;
        for (var i = 0; i < ItemKPS.length; i++) {
            if (ItemKPS[i].VALUE == "" || ItemKPS[i].VALUE == null) {
                flag = false;
                break;
            }
        }
        return flag;
    };//检查当前扫描Item是否已经扫描完全
    KeyPartScan.prototype.CheckOK = function () {
        var flag = true;
        for (var i = 0; i < this.KPS.length; i++) {
            if (this.KPS[i].VALUE == null || this.KPS[i].VALUE == "") {
                flag = false;
                break;
            }
        }
        return flag;
    };
    KeyPartScan.prototype.SendData = function () {
        var MessageID = "KPINPUT" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.List[MessageID] = this;
        this.Client.CallFunction(this.InputClassName, this.InputFunctionName, { SN: this.SN, STATION: this.StationName, KPITEM: this.ScanItem }, this.SendDataCallBack, MessageID);
    };//提交数据
    KeyPartScan.prototype.SendDataCallBack = function (d) {
        var KPScan = KeyPartScan.prototype.List[d.MessageID];
        delete KeyPartScan.prototype.List[d.MessageID];
        if (d.Status == "Pass") {
            KPScan.KPS = KPScan.KPSChange;
            if (KPScan.CheckOK()) {
                $("#Flag_IsCancel").val("0");
                parent.$("a.layui-layer-ico.layui-layer-close.layui-layer-close1").trigger("click");
            } else {
                KPScan.SetCurrentKP(true);
            }
        }
        else {
            parent.layer.msg(d.Message, {
                icon: 2,
                time: 10000,
                title: 'Error!',
                btn: ['ok']

            }, function () {
                KPScan.HighlightScanValue();
            });
            //20190108 patty commented set currentKP function, if sending fail, do not go to next input!
            //KPScan.SetCurrentKP(true);
        }
        KPScan.ScanItem.splice(0, KPScan.ScanItem.length);
    };//提交数据回调方法
    KeyPartScan.prototype.CompleteEvent = function () {
        this.Client.CallFunction(this.CallBackClass, this.CallBackFunctionName, { StationLayerReturnType: StationLayerReturnType.Close, ServerMessageID: this.ServerMessageID, StationLayerData: null }, null, this.ServerMessageID);
    };
    //this.Init();
};
