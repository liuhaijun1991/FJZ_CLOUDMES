var MesStationConfig = function (o) {
    this.Type = o.Type == undefined ? "Add" : o.Type;
    this.Name = o.Name;
    this.Station = null;
    this.Client = o.Client;
    this.IContainer = o.IContainer;
    this.OContainer = o.OContainer;
    this.AContainer = o.AContainer;
    this.OnInit = o.Init;
    MesStationConfig.prototype.constructor = MesStationConfig;
    MesStationConfig.prototype.ObjList = {};
    MesStationConfig.prototype.Init = function () {
        if (this.Type == "Add") {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            this.ListenStationData(MessageID);
            this.Client.CallFunction("MESStation.Stations.StationConfig.StationConfig", "GetID", {}, this.NewIDCallBack, MessageID);
        } else {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            this.ListenStationData(MessageID);
            this.Client.CallFunction("MESStation.Stations.CallStation", "InitCreateStation", { DisplayStationName: this.Name }, this.InitCallBack, MessageID);
        }
    };
    MesStationConfig.prototype.ListenStationData = function (k) {
        MesStationConfig.prototype.ObjList[k] = this;
    };
    MesStationConfig.prototype.NewIDCallBack = function (d) {
        var s = MesStationConfig.prototype.ObjList[d.MessageID];
        delete MesStationConfig.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.Station = new Station({ ID: d.Data });
        }
        if (s.OnInit != undefined) {
            s.OnInit(d);
        }
    };
    MesStationConfig.prototype.InitCallBack = function (d) {
        var s = MesStationConfig.prototype.ObjList[d.MessageID];
        delete MesStationConfig.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.Station = new Station(d.Data.Station);
        }
        if (s.OnInit != undefined) {
            s.OnInit(d);
        }
    };
    MesStationConfig.prototype.ReSort = function (arr) {
        for (var i = 0; i < arr.length; i++) {
            arr[i].SEQ_NO = i;
        }
    };
    MesStationConfig.prototype.GetInputList = function (CallBack) {
        this.Client.CallFunction("MESStation.Stations.StationConfig.InputConfig", "QueryInput", { ID: "", WO: "", DisplayType: "" }, CallBack);
    };
    MesStationConfig.prototype.GetActionList = function (CallBack) {
        this.Client.CallFunction("MESStation.Stations.StationConfig.CStationActionConfig", "QueryCStationAcation", { ID: "", ActionType: "" }, CallBack);
    };
    this.Init();
};
var Station = function (obj) {
    this.ID = obj.ID == undefined ? "" : obj.ID;
    this.DISPLAY_STATION_NAME = obj.DISPLAY_STATION_NAME == undefined ? "" : obj.DISPLAY_STATION_NAME;
    this.STATION_NAME = obj.STATION_NAME == undefined ? "" : obj.STATION_NAME;
    this.FAIL_STATION_ID = obj.FAIL_STATION_ID == undefined ? "" : obj.FAIL_STATION_ID;
    this.FAIL_STATION_FLAG = obj.FAIL_STATION_FLAG == undefined ? "0" : obj.FAIL_STATION_FLAG;
    this.EDIT_TIME = obj.EDIT_TIME == undefined ? "" : obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP == undefined ? "" : obj.EDIT_EMP;
    this.InputList = [];
    this.OutputList = [];
    Station.prototype.constructor = Station;
    Station.prototype.ObjList = {};
    Station.prototype.Init = function (obj) {
        if (obj.ID) {
            this.InputList.splice(0, this.InputList.length);
            this.OutputList.splice(0, this.OutputList.length);
            if (obj.InputList) {
                for (var i = 0; i < obj.InputList.length; i++) {
                    var input = new StationInput(obj.InputList[i]);
                    this.InputList.push(input);
                }
            }
            if (obj.OutputList) {
                for (var i = 0; i < obj.OutputList.length; i++) {
                    var output = new StationOutput(obj.OutputList[i]);
                    this.OutputList.push(output);
                }
            }
        } else {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            Station.prototype.ObjList[MessageID] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.StationConfig", "GetID", {}, this.NewIDCallBack, MessageID);
        }
    };
    Station.prototype.NewIDCallBack = function (d) {
        var s = Station.prototype.ObjList[d.MessageID];
        delete Station.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.ID = d.Data;
        }
    };
    Station.prototype.ShowInputs = function (obj) {
        obj.Container.empty();
        for (var i = 0; i < this.InputList.length; i++) {
            this.InputList[i].Show(obj.Type, obj.Container);
        }
    };
    Station.prototype.ShowOutputs = function (c) {
        c.empty();
        for (var i = 0; i < this.OutputList.length; i++) {
            this.OutputList[i].Show(c);
        }
    };
    Station.prototype.GetInput = function (ID) {
        for (var i = 0; i < this.InputList.length; i++) {
            if (this.InputList[i].ID == ID) {
                return this.InputList[i];
            }
        }
    };
    Station.prototype.AddInput = function (Input, SEQ) {
        this.InputList.splice(SEQ, 0, Input);
        MesStationConfig.prototype.ReSort(this.InputList);
    };
    Station.prototype.SortInput = function (ID, SEQ) {
        for (var i = 0; i < this.InputList.length; i++) {
            if (this.InputList[i].ID == ID) {
                var temp = this.InputList[i];
                this.InputList.splice(i, 1);
                this.InputList.splice(SEQ, 0, temp);
                break;
            }
        }
        MesStationConfig.prototype.ReSort(this.InputList);
    };
    Station.prototype.RemoveInput = function (ID) {
        for (var i = 0; i < this.InputList.length; i++) {
            if (this.InputList[i].ID == ID) {
                this.InputList[i].Remove();
                this.InputList.splice(i, 1);
                break;
            }
        }
        MesStationConfig.prototype.ReSort(this.InputList);
    };
    Station.prototype.GetOutput = function (ID) {
        for (var i = 0; i < this.OutputList.length; i++) {
            if (this.OutputList[i].ID == ID) {
                return this.OutputList[i];
            }
        }
    };
    Station.prototype.AddOutput = function (Output, SEQ) {
        this.OutputList.splice(SEQ, 0, Output);
        MesStationConfig.prototype.ReSort(this.OutputList);
    };
    Station.prototype.SortOutput = function (ID, SEQ) {
        for (var i = 0; i < this.OutputList.length; i++) {
            if (this.OutputList[i].ID == ID) {
                var temp = this.OutputList[i];
                this.OutputList.splice(i, 1);
                this.OutputList.splice(SEQ, 0, temp);
                break;
            }
        }
        MesStationConfig.prototype.ReSort(this.OutputList);
    };
    Station.prototype.RemoveOutput = function (ID) {
        for (var i = 0; i < this.OutputList.length; i++) {
            if (this.OutputList[i].ID == ID) {
                this.OutputList[i].Remove();
                this.OutputList.splice(i, 1);
                break;
            }
        }
        MesStationConfig.prototype.ReSort(this.OutputList);
    };
    this.Init(obj);
};
var StationInput = function (obj) {
    this.ID = obj.ID == undefined ? "" : obj.ID;
    this.STATION_ID = obj.STATION_ID == undefined ? "" : obj.STATION_ID;
    this.INPUT_ID = obj.INPUT_ID == undefined ? "" : obj.INPUT_ID;
    this.C_Input = obj.C_Input;
    this.SEQ_NO = obj.SEQ_NO == undefined ? "" : obj.SEQ_NO;
    this.SCAN_FLAG = (obj.SCAN_FLAG == null || obj.SCAN_FLAG == undefined) ? "0" : obj.SCAN_FLAG;
    this.DISPLAY_NAME = obj.DISPLAY_NAME == undefined ? "" : obj.DISPLAY_NAME;
    this.REMEMBER_LAST_INPUT = (obj.REMEMBER_LAST_INPUT == null || obj.REMEMBER_LAST_INPUT == undefined) ? "0" : obj.REMEMBER_LAST_INPUT;
    this.EDIT_TIME = obj.EDIT_TIME == undefined ? "" : obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP == undefined ? "" : obj.EDIT_EMP;
    this.InputActionList = [];
    this.StationActionList = [];
    StationInput.prototype.constructor = StationInput;
    StationInput.prototype.UList = {};
    StationInput.prototype.ObjList = {};
    StationInput.prototype.Init = function (obj) {
        if (obj.ID) {
            this.InputActionList.splice(0, this.InputActionList.length);
            this.StationActionList.splice(0, this.StationActionList.length);
            if (obj.InputActionList) {
                for (var i = 0; i < obj.InputActionList.length; i++) {
                    var IA = new IAction(obj.InputActionList[i]);
                    this.InputActionList.push(IA);
                }
            }
            if (obj.StationActionList) {
                for (var i = 0; i < obj.StationActionList.length; i++) {
                    var SA = new SAction(obj.StationActionList[i]);
                    this.StationActionList.push(SA);
                }
            }
        } else {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            StationInput.prototype.UList[MessageID] = obj.U;
            StationInput.prototype.ObjList[MessageID] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.StationInputConfig", "GetID", {}, this.NewIDCallBack, MessageID);
        }
    };
    StationInput.prototype.NewIDCallBack = function (d) {
        var u = StationInput.prototype.UList[d.MessageID];
        var s = StationInput.prototype.ObjList[d.MessageID];
        delete StationInput.prototype.UList[d.MessageID];
        delete StationInput.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.ID = d.Data;
            u.attr("data-objid", d.Data);
        }
    };
    StationInput.prototype.Show = function (Type, c) {
        var E = new InputElements();
        switch (Type) {
            case "modify":
                E.modify({ Input: this, Container: c });
                break;
            case "readonly":
                E.readonly({ Input: this, Container: c });
                break;
            default:
                break;
        }
    };
    StationInput.prototype.ShowActions = function (IC, SC) {
        IC.empty();
        SC.empty();
        for (var i = 0; i < this.InputActionList.length; i++) {
            this.InputActionList[i].Show(IC);
        }
        for (var i = 0; i < this.StationActionList.length; i++) {
            this.StationActionList[i].Show(SC);
        }
    };
    StationInput.prototype.Remove = function () {
        var selector = ".Inputs li[data-objid=" + this.ID + "]";
        $(selector).remove();
    };
    StationInput.prototype.GetAction = function (ID) {
        for (var i = 0; i < this.StationActionList.length; i++) {
            if (this.StationActionList[i].ID == ID) {
                return this.StationActionList[i];
            }
        }
    };
    StationInput.prototype.AddAction = function (Action, SEQ) {
        this.StationActionList.splice(SEQ, 0, Action);
        MesStationConfig.prototype.ReSort(this.StationActionList);
    };
    StationInput.prototype.SortAction = function (ID, SEQ) {
        for (var i = 0; i < this.StationActionList.length; i++) {
            if (this.StationActionList[i].ID == ID) {
                var temp = this.StationActionList[i];
                this.StationActionList.splice(i, 1);
                this.StationActionList.splice(SEQ, 0, temp);
                break;
            }
        }
        MesStationConfig.prototype.ReSort(this.StationActionList);
    };
    StationInput.prototype.RemoveAction = function (ID) {
        for (var i = 0; i < this.StationActionList.length; i++) {
            if (this.StationActionList[i].ID == ID) {
                this.StationActionList[i].Remove();
                this.StationActionList.splice(i, 1);
                break;
            }
        }
        MesStationConfig.prototype.ReSort(this.StationActionList);
    };
    this.Init(obj);
};
var IAction = function (obj) {
    this.ID = obj.ID == undefined ? "" : obj.ID;
    this.INPUT_ID = obj.INPUT_ID == undefined ? "" : obj.INPUT_ID;
    this.C_STATION_ACTION_ID = obj.C_STATION_ACTION_ID == undefined ? "" : obj.C_STATION_ACTION_ID;
    this.SEQ_NO = obj.SEQ_NO == undefined ? "" : obj.SEQ_NO;
    this.CONFIG_TYPE = obj.CONFIG_TYPE == undefined ? "" : obj.CONFIG_TYPE;
    this.CONFIG_VALUE = obj.CONFIG_VALUE == undefined ? "" : obj.CONFIG_VALUE;
    this.ADD_FLAG = obj.ADD_FLAG == undefined ? "" : obj.ADD_FLAG;
    this.EDIT_TIME = obj.EDIT_TIME == undefined ? "" : obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP == undefined ? "" : obj.EDIT_EMP;
    this.C_STATION_ACTION = {};
    this.ParaSA = [];
    this.ParaSB = [];
    IAction.prototype.constructor = IAction;
    IAction.prototype.Init = function (obj) {
        this.ParaSA.splice(0, this.ParaSA.length);
        this.ParaSB.splice(0, this.ParaSB.length);
        this.C_STATION_ACTION = new C_STATION_ACTION(obj.C_STATION_ACTION);
        for (var i = 0; i < obj.ParaSB.length; i++) {
            var pc = new ActionParaSB(obj);
            this.ParaSB.push(pc);
        }
        for (var i = 0; i < obj.ParaSA.length; i++) {
            var p = new ActionParaSA(obj);
            this.ParaSA.push(p);
        }
    };
    IAction.prototype.Show = function (c) {
        var E = new ActionElements();
        switch (this.C_STATION_ACTION.ACTION_TYPE) {
            case "DataLoader":
                E.Loader(c, this.ID, this.C_STATION_ACTION.ACTION_NAME);
                break;
            case "DataChecker":
                E.Checker(c, this.ID, this.C_STATION_ACTION.ACTION_NAME);
                break;
            case "ActionRunner":
                E.Runner(c, this.ID, this.C_STATION_ACTION.ACTION_NAME);
                break;
            default:
                break;
        }
    };
    this.Init(obj);
};
var SAction = function (obj) {
    this.ID = obj.ID == undefined ? "" : obj.ID;
    this.R_STATION_INPUT_ID = obj.R_STATION_INPUT_ID == undefined ? "" : obj.R_STATION_INPUT_ID;
    this.C_STATION_ACTION_ID = obj.C_STATION_ACTION_ID == undefined ? "" : obj.C_STATION_ACTION_ID;
    this.SEQ_NO = obj.SEQ_NO == undefined ? "" : obj.SEQ_NO;
    this.CONFIG_TYPE = obj.CONFIG_TYPE == undefined ? "Default" : obj.CONFIG_TYPE;
    this.CONFIG_VALUE = obj.CONFIG_VALUE == undefined ? "" : obj.CONFIG_VALUE;
    this.ADD_FLAG = obj.ADD_FLAG == undefined ? "1" : obj.ADD_FLAG;
    this.EDIT_TIME = obj.EDIT_TIME == undefined ? "" : obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP == undefined ? "" : obj.EDIT_EMP;
    this.C_STATION_ACTION = {};
    this.ParaSA = [];
    this.ParaSB = [];
    SAction.prototype.constructor = SAction;
    SAction.prototype.UList = {};
    SAction.prototype.ObjList = {};
    SAction.prototype.Init = function (obj) {
        if (obj.ID) {
            this.ParaSA.splice(0, this.ParaSA.length);
            this.ParaSB.splice(0, this.ParaSB.length);
            this.C_STATION_ACTION = new C_STATION_ACTION(obj.C_STATION_ACTION);
            for (var i = 0; i < obj.ParaSB.length; i++) {
                var pc = new ActionParaSB(obj.ParaSB[i]);
                this.ParaSB.push(pc);
            }
            for (var i = 0; i < obj.ParaSA.length; i++) {
                var p = new ActionParaSA(obj.ParaSA[i]);
                this.ParaSA.push(p);
            }
        } else {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            SAction.prototype.UList[MessageID] = obj.U;
            SAction.prototype.ObjList[MessageID] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.RStationActionConfig", "GetID", {}, this.NewIDCallBack, MessageID);
            this.C_STATION_ACTION = new C_STATION_ACTION({ ID: this.C_STATION_ACTION_ID, Client: obj.Client });
            /*Get ParaSB*/
            var MessageID2 = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            SAction.prototype.ObjList[MessageID2] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.ActionParaConfig", "QueryActionPara", { ID: "", StationActionID: obj.C_STATION_ACTION_ID, Name: "" }, this.GetParaSBCallBack, MessageID2);
        }
    };
    SAction.prototype.NewIDCallBack = function (d) {
        var u = SAction.prototype.UList[d.MessageID];
        var s = SAction.prototype.ObjList[d.MessageID];
        delete SAction.prototype.UList[d.MessageID];
        delete SAction.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.ID = d.Data;
            u.attr("data-objid", d.Data);
        }
    };
    SAction.prototype.GetParaSBCallBack = function (d) {
        var s = SAction.prototype.ObjList[d.MessageID];
        delete SAction.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            for (var i = 0; i < d.Data.length; i++) {
                var parasb = new ActionParaSB(d.Data[i]);
                s.ParaSB.push(parasb);
            }
        }
    };
    SAction.prototype.Show = function (c) {
        var E = new ActionElements();
        switch (this.C_STATION_ACTION.ACTION_TYPE) {
            case "DataLoader":
                E.Loader(c, this.ID, this.C_STATION_ACTION.ACTION_NAME);
                break;
            case "DataChecker":
                E.Checker(c, this.ID, this.C_STATION_ACTION.ACTION_NAME);
                break;
            case "ActionRunner":
                E.Runner(c, this.ID, this.C_STATION_ACTION.ACTION_NAME);
                break;
            default:
                break;
        }
    };
    SAction.prototype.Remove = function () {
        var selector = ".SAction li[data-objid=" + this.ID + "]";
        $(selector).remove();
    };
    SAction.prototype.GetParam = function (ID) {
        for (var i = 0; i < this.ParaSA.length; i++) {
            if (this.ParaSA[i].ID == ID) {
                return this.ParaSA[i];
            }
        }
    };
    SAction.prototype.ShowParam = function (AC, BC) {
        AC.empty();
        BC.empty();
        for (var i = 0; i < this.ParaSB.length; i++) {
            this.ParaSB[i].Show(BC);
        }
        for (var i = 0; i < this.ParaSA.length; i++) {
            this.ParaSA[i].Show(AC);
        }
    };
    SAction.prototype.AddParam = function (Param) {
        this.ParaSA.push(Param);
        MesStationConfig.prototype.ReSort(this.ParaSA);
    };
    SAction.prototype.RemoveParam = function (ID) {
        for (var i = 0; i < this.ParaSA.length; i++) {
            if (this.ParaSA[i].ID == ID) {
                this.ParaSA[i].Remove();
                this.ParaSA.splice(i, 1);
            }
        }
        MesStationConfig.prototype.ReSort(this.ParaSA);
    };
    this.Init(obj);
};
var C_STATION_ACTION = function (obj) {
    this.ID = obj.ID;
    this.ACTION_TYPE = obj.ACTION_TYPE;
    this.ACTION_NAME = obj.ACTION_NAME;
    this.DLL_NAME = obj.DLL_NAME;
    this.CLASS_NAME = obj.CLASS_NAME;
    this.FUNCTION_NAME = obj.FUNCTION_NAME;
    this.DESCRIPTION = obj.DESCRIPTION;
    this.BU = obj.BU;
    this.SORTING = obj.SORTING;
    this.USE_STATION = obj.USE_STATION;
    this.EDIT_TIME = obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP;
    C_STATION_ACTION.prototype.ObjList = {};
    C_STATION_ACTION.prototype.Init = function (obj) {
        if (obj.Client) {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            C_STATION_ACTION.prototype.ObjList[MessageID] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.CStationActionConfig", "QueryCStationAcation", { ID: obj.ID, ActionType: "" }, this.CallBack, MessageID);
        }
    };
    C_STATION_ACTION.prototype.CallBack = function (d) {
        var s = C_STATION_ACTION.prototype.ObjList[d.MessageID];
        delete C_STATION_ACTION.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.ID = d.Data[0].ID;
            s.ACTION_TYPE = d.Data[0].ACTION_TYPE;
            s.ACTION_NAME = d.Data[0].ACTION_NAME;
            s.DLL_NAME = d.Data[0].DLL_NAME;
            s.CLASS_NAME = d.Data[0].CLASS_NAME;
            s.FUNCTION_NAME = d.Data[0].FUNCTION_NAME;
            s.DESCRIPTION = d.Data[0].DESCRIPTION;
            s.BU = d.Data[0].BU;
            s.SORTING = d.Data[0].SORTING;
            s.USE_STATION = d.Data[0].USE_STATION;
            s.EDIT_TIME = d.Data[0].EDIT_TIME;
            s.EDIT_EMP = d.Data[0].EDIT_EMP;
        }
    };
    this.Init(obj);
};
var ActionParaSB = function (obj) {
    this.ID = obj.ID;
    this.C_STATION_ACTION_ID = obj.C_STATION_ACTION_ID;
    this.SEQ_NO = obj.SEQ_NO;
    this.NAME = obj.NAME;
    this.DESCRIPTION = obj.DESCRIPTION;
    this.EDIT_EMP = obj.EDIT_EMP;
    this.EDIT_TIME = obj.EDIT_TIME;
    ActionParaSB.prototype.constructor = ActionParaSB;
    ActionParaSB.prototype.Show = function (c) {
        var row = $('<tr><td>' + this.SEQ_NO + '</td><td>' + this.NAME + '</td><td>' + this.DESCRIPTION + '</td></tr>');
        c.append(row);
    };
};
var ActionParaSA = function (obj) {
    this.ID = obj.ID == undefined ? "" : obj.ID;
    this.R_STATION_ACTION_ID = obj.R_STATION_ACTION_ID == undefined ? "" : obj.R_STATION_ACTION_ID;
    this.R_INPUT_ACTION_ID = obj.R_INPUT_ACTION_ID == undefined ? "" : obj.R_INPUT_ACTION_ID;
    this.SEQ_NO = obj.SEQ_NO == undefined ? "" : obj.SEQ_NO;
    this.SESSION_TYPE = obj.SESSION_TYPE == undefined ? "" : obj.SESSION_TYPE;
    this.SESSION_KEY = obj.SESSION_KEY == undefined ? "" : obj.SESSION_KEY;
    this.VALUE = obj.VALUE == undefined ? "" : obj.VALUE;
    this.EDIT_TIME = obj.EDIT_TIME == undefined ? "" : obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP == undefined ? "" : obj.EDIT_EMP;
    ActionParaSA.prototype.constructor = ActionParaSA;
    ActionParaSA.prototype.UList = {};
    ActionParaSA.prototype.ObjList = {};
    ActionParaSA.prototype.Init = function (obj) {
        if (!obj.ID) {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            ActionParaSA.prototype.UList[MessageID] = obj.U;
            ActionParaSA.prototype.ObjList[MessageID] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.StationActionParaConfig", "GetID", {}, this.NewIDCallBack, MessageID);
        }
    };
    ActionParaSA.prototype.NewIDCallBack = function (d) {
        var u = ActionParaSA.prototype.UList[d.MessageID];
        var s = ActionParaSA.prototype.ObjList[d.MessageID];
        delete ActionParaSA.prototype.UList[d.MessageID];
        delete ActionParaSA.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.ID = d.Data;
            u.attr("data-objid", d.Data);
        }
    };
    ActionParaSA.prototype.Show = function (c) {
        var selector = "tr[data-objid=" + this.ID + "]";
        if (c.find(selector).length > 0) {
            var tds = c.find(selector).find("td");
            tds[0].innerText = this.SEQ_NO;
            tds[1].innerText = this.SESSION_TYPE;
            tds[2].innerText = this.SESSION_KEY;
            tds[3].innerText = this.VALUE;
        }
        else {
            var row = $('<tr data-objid="' + this.ID + '"><td>' + this.SEQ_NO + '</td><td>' + this.SESSION_TYPE + '</td><td>' + this.SESSION_KEY + '</td><td>' + this.VALUE + '</td></tr>');
            c.append(row);
        }
    };
    ActionParaSA.prototype.Remove = function () {
        var selector = ".ParaSABox  tr[data-objid=" + this.ID + "]";
        $(selector).remove();
    };
    this.Init(obj);
};
var StationOutput = function (obj) {
    this.ID = obj.ID == undefined ? "" : obj.ID;
    this.R_STATION_ID = obj.R_STATION_ID == undefined ? "" : obj.R_STATION_ID;
    this.NAME = obj.NAME == undefined ? "" : obj.NAME;
    this.SEQ_NO = obj.SEQ_NO == undefined ? "" : obj.SEQ_NO;
    this.DISPLAY_TYPE = obj.DISPLAY_TYPE == undefined ? "TXT" : obj.DISPLAY_TYPE;
    this.SESSION_TYPE = obj.SESSION_TYPE == undefined ? "" : obj.SESSION_TYPE;
    this.SESSION_KEY = obj.SESSION_KEY == undefined ? "" : obj.SESSION_KEY;
    this.EDIT_TIME = obj.EDIT_TIME == undefined ? "" : obj.EDIT_TIME;
    this.EDIT_EMP = obj.EDIT_EMP == undefined ? "" : obj.EDIT_EMP;
    StationOutput.prototype.constructor = StationOutput;
    StationOutput.prototype.UList = {};
    StationOutput.prototype.ObjList = {};
    StationOutput.prototype.Init = function (obj) {
        if (!obj.ID) {
            var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
            StationOutput.prototype.UList[MessageID] = obj.U;
            StationOutput.prototype.ObjList[MessageID] = this;
            obj.Client.CallFunction("MESStation.Stations.StationConfig.StationOutputConfig", "GetID", {}, this.NewIDCallBack, MessageID);
        }
    };
    StationOutput.prototype.NewIDCallBack = function (d) {
        var u = StationOutput.prototype.UList[d.MessageID];
        var s = StationOutput.prototype.ObjList[d.MessageID];
        delete StationOutput.prototype.UList[d.MessageID];
        delete StationOutput.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            s.ID = d.Data;
            u.attr("data-objid", d.Data);
        }
    };
    StationOutput.prototype.Show = function (c) {
        var e = new OutputElements();
        e.modify(c, this.ID, this.NAME);
    };
    StationOutput.prototype.Remove = function () {
        var selector = ".Outputs li[data-objid=" + this.ID + "]";
        $(selector).remove();
    };
    this.Init(obj);
};

var InputElements = function () {
    InputElements.prototype.constructor = InputElements;
    InputElements.prototype.readonly = function (obj) {
        var li = $("<li data-objid=\"" + obj.Input.ID + "\"><em>" + obj.Input.DISPLAY_NAME + "</em></li>");
        obj.Container.append(li);
    };
    InputElements.prototype.modify = function (obj) {
        var li = $("<li data-objid=\"" + obj.Input.ID + "\"><em>"+ obj.Input.DISPLAY_NAME + "</em><span class=\"close\"></span></li>");
        obj.Container.append(li);
    };
};
var OutputElements = function () {
    OutputElements.prototype.constructor = OutputElements;
    OutputElements.prototype.modify = function (c, ID, Name) {
        var li = $("<li data-objid=\"" + ID + "\"><em>" + Name + "</em><span class=\"close\"></span></li>");
        c.append(li);
        return li;
    };
};
var ActionElements = function () {
    ActionElements.prototype.constructor = ActionElements;
    ActionElements.prototype.Loader = function (c, ID, Name, Type) {
        var li = $("<li class=\"Loader\" data-objid=\"" + ID + "\"><em>" + Name + "</em>" + (Type == "readonly" ? "" : "<span class=\"close\"><span>") + "</li>");
        c.append(li);
    };
    ActionElements.prototype.Checker = function (c, ID, Name, Type) {
        var li = $("<li class=\"Checker\" data-objid=\"" + ID + "\"><em>" + Name + "</em>" + (Type == "readonly" ? "" : "<span class=\"close\"><span>") + "</li>");
        c.append(li);
    };
    ActionElements.prototype.Runner = function (c, ID, Name, Type) {
        var li = $("<li class=\"Runner\" data-objid=\"" + ID + "\"><em>" + Name + "</em>" + (Type == "readonly" ? "" : "<span class=\"close\"><span>") + "</li>");
        c.append(li);
    };
};




