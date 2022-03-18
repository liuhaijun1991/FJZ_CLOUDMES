var btn = [{ "INPUT_NAME": "Start", "Display_name": "Button", "value": "開始" }, { "INPUT_NAME": "Stop", "Display_name": "Button", "value": "停止" }, { "INPUT_NAME": "Exe", "Display_name": "Button", "value": "執行" }, { "INPUT_NAME": "lbl_datetime", "Display_name": "Label", "value": "日期" }, { "INPUT_NAME": "datetime", "Display_name": "Input", "value": "datetime" }];

var MesInterface = function (i) {
    this.InterfaceJson = null;
    this.Client = i.Client;
    this.Data = i.Data;
    this.IContainer = i.IContainer;
    this.OContainer = i.OContainer;
    this.MContainer = i.MContainer;
    this.MessageShowType = i.MessageShowType;
    this.Inputs = [];
    this.Outputs = [];
    this.Message = [];
    this.Interfaces = [];
    this.OnInit = i.Init;
    this.clientip = null;
    MesInterface.prototype.constructor = MesInterface;
    MesInterface.prototype.InterfaceList = {};
    MesInterface.prototype.Init = function () {
        this.InterfaceJson = this.Data;
        this.Message.splice(0, this.Message.length);
        this.Outputs.splice(0, this.Outputs.length);
        this.Inputs.splice(0, this.Inputs.length);
        for (var i = 0; i < btn.length; i++) {
            var n = new InterfaceInput(btn[i]);
            this.Inputs.push(n);
        }
        if (this.Data.INTERFACE.length > 0) {
            var face = new InterfaceOutput(this.Data.INTERFACE);
            this.Outputs.push(face);
        }
        for (var i = 0; i < this.Data.INTERFACE_SERVER.length; i++) {
            this.clientip = this.Data.INTERFACE_SERVER[i]["SERVER_IP"];
        }
        if (this.MessageID != null || this.MessageID != "") {
            var m = new InterfaceMessage(this.MessageID);
            this.Message.push(m);
        }
        if (this.IContainer) {
            this.ShowInputs(this.IContainer, this.OContainer, this.Client, this.MContainer, this.clientip);
        }
        if (this.OContainer) {
            this.ShowOutputs(this.OContainer);
        }
        if (this.MContainer) {
            this.ShowMessage(this.MContainer);
        }
        if (this.OnInit != undefined) {
            this.OnInit({ Data: this.Data });
        }
    };
    MesInterface.prototype.ShowMessage = function (Container) {
        Container = Container ? Container : this.MContainer;
        for (var i = 0; i < this.Message.length; i++) {
            this.Message[i].Show(Container);
        }
    };
    MesInterface.prototype.ShowInputs = function (Container, OContainer, InterfaceClient, MContainer, clientip) {
        for (var i = 0; i < this.Inputs.length; i++) {
            this.Inputs[i].Remove();
            this.Inputs[i].Show({ Client: this.Client, Container: Container });
            if (this.Inputs[i].Display_Name == "Button") {
                $("#" + this.Inputs[i].INPUT_NAME).unbind('click');
                if (this.Inputs[i].INPUT_NAME == "Start") {
                    $("#" + this.Inputs[i].INPUT_NAME).bind('click', { Interface: this.Inputs[i], OContainer: OContainer, InterfaceClient: InterfaceClient, MContainer: MContainer, ClientIp: clientip }, function (e) {
                        e.data.Interface.Start(e.data.OContainer, e.data.InterfaceClient, e.data.MContainer, e.data.ClientIp);
                    });
                }
                else if (this.Inputs[i].INPUT_NAME == "Stop") {
                    $("#" + this.Inputs[i].INPUT_NAME).bind('click', { Interface: this.Inputs[i], OContainer: OContainer, InterfaceClient: InterfaceClient, MContainer: MContainer, ClientIp: clientip }, function (e) {
                        e.data.Interface.Stop(e.data.OContainer, e.data.InterfaceClient, e.data.MContainer, e.data.ClientIp);
                    });
                } else {
                    $("#" + this.Inputs[i].INPUT_NAME).bind('click', { Interface: this.Inputs[i], OContainer: OContainer, InterfaceClient: InterfaceClient, MContainer: MContainer, ClientIp: clientip }, function (e) {
                        e.data.Interface.Exe(e.data.OContainer, e.data.InterfaceClient, e.data.MContainer, e.data.ClientIp);
                    });
                }
            }
        }
    };
    MesInterface.prototype.ShowOutputs = function (Container) {
        for (var i = 0; i < this.Outputs.length; i++) {
            this.Outputs[i].Remove();
            this.Outputs[i].Show(Container);
        }
    };
    this.Init();
};
var InterfaceMessage = function (obj) {
    InterfaceMessage.prototype.Show = function (container) {
        container.append(obj);
    }
};
var InterfaceInput = function (obj) {
    this.INPUT_NAME = obj.INPUT_NAME;
    this.Display_Name = obj.Display_name;
    this.Value = obj.value;
    InterfaceInput.prototype.constructor = InterfaceInput;
    InterfaceInput.prototype.Show = function (obj) {
        var K = new InputElements(obj.Client);
        var container = obj.Container;
        switch (this.Display_Name) {
            case "Button":
                K.Button(container, this.INPUT_NAME, this.Value);
                break;
            case "Label":
                K.Label(container, this.INPUT_NAME, this.Value);
                break;
            case "Input":
                K.Input(container, this.INPUT_NAME, this.Value);
                break;
            default:
                container.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;
        }
    };
    InterfaceInput.prototype.Remove = function () {
    };
    InterfaceInput.prototype.Exe = function ($container, InterfaceClient, MContainer,ClientIp) {
        var arrselections = $container.find("table").bootstrapTable("getSelections");
        if (arrselections <= 0 || arrselections > 1) {
            swal({
                title: "警告",
                text: "請選擇一條信息!",
                type: "warning",
                timer: 2000,
                showConfirmButton: false
            });
            return;
        }
        InterfaceClient.CallFunction(arrselections[0].CLASS_NAME, arrselections[0].FUNCTION_NAME, { PROGRAM_NAME: arrselections[0].PROGRAM_NAME, STATUS: "Execute", DATE_TIME: $("#datetime").val(), ITEM_NAME: arrselections[0].ITEM_NAME, IP: ClientIp }, function (e) {
            MContainer.append("\n" + e.Message);
            swal({
                title: "",
                text: e.Message,
                type: "",
                timer: 2000,
                showConfirmButton: false
            });
            return;
        });
    };
    InterfaceInput.prototype.Stop = function ($container, InterfaceClient, MContainer, ClientIp) {
        //var arrselections = $("#" + this.Name).bootstrapTable("getSelections");
        var arrselections = $container.find("table").bootstrapTable("getSelections");
        if (arrselections <= 0) {
            swal({
                title: "警告",
                text: "請選擇一條信息!",
                type: "warning",
                timer: 2000,
                showConfirmButton: false
            });
            return;
        }
        var data = new Array();
        for (var i = 0; i < arrselections.length; i++) {
            data.push(arrselections[i].ITEM_NAME);
        };
        InterfaceClient.CallFunction(arrselections[0].CLASS_NAME, arrselections[0].FUNCTION_NAME, { PROGRAM_NAME: arrselections[0].PROGRAM_NAME, STATUS: "Stop", DATE_TIME: $("#datetime").val(), ITEM_NAME: arrselections[0].ITEM_NAME, IP: ClientIp }, function (e) {
            MContainer.append("\n" + e.Message);
            swal({
                title: "",
                text: e.Message,
                type: "",
                timer: 2000,
                showConfirmButton: false
            });
            return;
        });
    };
    InterfaceInput.prototype.Start = function ($container, InterfaceClient, MContainer, ClientIp) {
        var arrselections = $container.find("table").bootstrapTable("getSelections");
        if (arrselections <= 0) {
            swal({
                title: "警告",
                text: "請選擇一條信息!",
                type: "warning",
                timer: 2000,
                showConfirmButton: false
            });
            return;
        }
        var data = new Array();
        for (var i = 0; i < arrselections.length; i++) {
            data.push(arrselections[i].ITEM_NAME);
        };
        InterfaceClient.CallFunction(arrselections[0].CLASS_NAME, arrselections[0].FUNCTION_NAME, { PROGRAM_NAME: arrselections[0].PROGRAM_NAME, STATUS: "Start", DATE_TIME: $("#datetime").val(), ITEM_NAME: arrselections[0].ITEM_NAME, IP: ClientIp }, function (e) {
            MContainer.append("\n" + e.Message);
            swal({
                title: "",
                text: e.Message,
                type: "",
                timer: 2000,
                showConfirmButton: false
            });
            return;
        });
    };
};
var InterfaceOutput = function (obj) {
    InterfaceOutput.prototype.constructor = InterfaceOutput;
    InterfaceOutput.prototype.Show = function (container) {
        var O = new OutputElements();
        O.Table(container, this.ID, obj);
    };
    InterfaceOutput.prototype.Remove = function (container) {


    };

};
var InputElements = function (client) {
    this.client = client;
    InputElements.prototype.constructor = InputElements;
    InputElements.prototype.Button = function (container, ID, value) {
        var div = $("<div class=\"col-xs-3 col-sm-1 col-md-1 col-lg-1 \"></div>");
        var button = $("<button class=\"form-group btn btn-primary \" view-group=\"" + ID + "\"  id=\"" + ID + "\"><lan set-lan=\"html:btn_show" + ID + "\">" + value + "</lan></button>");
        div.append(button);
        container.append(div);
    };
    InputElements.prototype.Label = function (container, ID, value) {
        var div = $("<label class=\"control-label col-xs-2 col-sm-1 col-md-1 col-lg-1\" ><lan set-lan=\"val:lbl_datetime\">" + value + "</lan></label>");
        container.append(div);
    };
    InputElements.prototype.Input = function (container, ID, value) {
        var div = $("<input id=\"" + ID + "\" class=\"laydate-icon form-control layer-date col-xs-1 col-sm-1 col-md-1 col-lg-1 \"  data-option=\"laydate({elem: '#datetime', event: 'focus' });\">");
        container.append(div);
    };
};
var OutputElements = function () {
    OutputElements.prototype.constructor = OutputElements;
    OutputElements.prototype.Table = function (c, ID, value) {
        var tb = $("<table id=\"" + ID + "\" view-group=\"" + ID + "\"></table>");
        c.append(tb);
        var ck = [];
        var checkbox = " title:'checkall',field:'select',checkbox: true, width: 30, align: 'center',valign: 'middle' ";
        ck.push({ checkbox });
        for (var item in value[0]) {
            if (item == "LAST_RUN_DATE") {
                ck.push({
                    title: item, formatter: function (value, row, index) {
                        return new Date(row.LAST_RUN_DATE).format("yyyy-MM-dd hh:mm:ss")
                    }
                });
            } else if (item == "NEXT_RUN_DATE") {
                ck.push({
                    title: item, formatter: function (value, row, index) {
                        return new Date(row.NEXT_RUN_DATE).format("yyyy-MM-dd hh:mm:ss")
                    }
                });
            } else if (item == "ID" || item == "EDIT_EMP" || item == "EDIT_TIME") {
            
            } else {
                ck.push({ field: item, title: item });
            }
        }
        tb.bootstrapTable({
            data: value,
            striped: true,
            cache: false,
            sortable: false,
            sortOrder: "asc",
            search: true,
            strictSearch: true,
            searchOnEnterKey: false,
            showColumns: false,
            showRefresh: false,
            minimumCountColumns: 2,
            singleSelect: false,
            clickToSelect: true,
            showToggle: false,
            cardView: false,
            detailView: false,
            dataType: "json",
            method: "post",
            searchAlign: "left",
            buttonsAlign: "left",
            columns: ck,
            locale: "zh-CN"
        });
    };
};


