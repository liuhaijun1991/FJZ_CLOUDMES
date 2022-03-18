var MesReport = function (o) {
    this.ReportClassName = o.ReportClassName ? o.ReportClassName : "MESReport.Test.TEST1";
    this.InitClassName = o.InitClassName ? o.InitClassName : "MESStation.Report.CallReport";
    this.InitFunctionName = o.InitFunctionName ? o.InitFunctionName : "GetReport";
    this.RunClassName = o.RunClassName ? o.RunClassName : "MESStation.Report.CallReport";
    this.RunFunctionName = o.RunFunctionName ? o.RunFunctionName : "RunReport";
    this.DownExcelFunctionName = o.DownExcelFunctionName ? o.DownExcelFunctionName : "DownFile";
    this.InputChangeFunctionName = o.InputChangeFunctionName ? o.InputChangeFunctionName : "InputChange";
    this.Client = o.Client;
    this.ReportJson = null;
    this.Report = null;
    this.OnInit = o.Init;
    this.IScale = o.IScale;
    this.TitleHeight = o.TitleHeight;
    this.ShowToolbar = o.ShowToolbar;
    this.IContainer = o.IContainer;
    this.OContainer = o.OContainer;
    MesReport.prototype.constructor = MesReport;
    MesReport.prototype.ObjList = {};
    MesReport.prototype.Init = function () {
        MesReport.prototype.StationList = {};
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.InitClassName, this.InitFunctionName, { ClassName: this.ReportClassName }, this.InitCallBack, MessageID);
    };
    MesReport.prototype.ListenStationData = function (k) {
        MesReport.prototype.ObjList[k] = this;
    };
    MesReport.prototype.InitCallBack = function (d) {
        var _Mes = MesReport.prototype.ObjList[d.MessageID];
        delete MesReport.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            _Mes.ReportJson = d.Data;
            _Mes.Report = new Report(d.Data, { Container: _Mes.OContainer, TitleHeight: _Mes.TitleHeight, ShowToolbar: _Mes.ShowToolbar });
            if (_Mes.IContainer) {
                _Mes.ShowInputs(_Mes.IContainer);
            }
            if (_Mes.OContainer) {
                _Mes.ShowOutputs(_Mes.OContainer);
            }
            if (_Mes.OnInit != undefined) {
                _Mes.OnInit(d);
            }
        }
        else {
            swal("error", d.Message, "error");
        }
    };
    MesReport.prototype.SetInputValue = function (Name, value) {
        for (var i = 0; i < this.Report.Inputs.length; i++) {
            if (this.Report.Inputs[i].Name == Name) {
                this.Report.Inputs[i].Value = value;
                this.ReportJson.Inputs[i].Value = value;
            }
        }
    };
    MesReport.prototype.ShowInput = function (obj) {
        for (var i = 0; i < this.Inputs.length; i++) {
            if (this.Report.Inputs[i].DisplayName == obj.InputName) {
                obj.Container.find("input.form-control").unbind("keyup");
                obj.Container.find("select.form-control").unbind("change");
                this.Report.Inputs[i].Remove();
                this.Report.Inputs[i].Show({ Client: this.Client, Container: obj.Container, Scale: (obj.Scale == undefined ? (this.IScale == undefined ? "3:9" : this.IScale) : obj.Scale) });
                this.Report.Inputs[i].SetEnable();
                obj.Container.find("input.form-control:not(.datepicker)").bind("keyup", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    if (this.getAttribute("data-AutoPostback") == "True") {
                        event.data.Report.Run();
                    } else if (this.getAttribute("data-SendChange") == "True") {
                        event.data.Report.InputChange();
                    }
                });
                obj.Container.find("input.form-control.datepicker").bind("change", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    if (this.getAttribute("data-AutoPostback") == "True") {
                        event.data.Report.InputChange();
                    } else if (this.getAttribute("data-SendChange") == "True") {
                        event.data.Report.InputChange();
                    }
                });
                obj.Container.find("select.form-control").bind("change", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    if (this.getAttribute("data-AutoPostback") == "True") {
                        event.data.Report.InputChange();
                    } else if (this.getAttribute("data-SendChange") == "True") {
                        event.data.Report.InputChange();
                    }
                });
                obj.Container.find("textarea.form-control").bind("keyup", { Report: this }, function (event) {
                    event.data.Report.SetInputValue(this.name, this.value);
                    if (this.getAttribute("data-AutoPostback") == "True") {
                        event.data.Report.InputChange();
                    } else if (this.getAttribute("data-SendChange") == "True") {
                        event.data.Report.InputChange();
                    }
                });
                break;
            }
        }
    };
    MesReport.prototype.ShowInputs = function (Container) {
        Container.find("input.form-control").unbind("keyup");
        Container.find("select.form-control").unbind("change");
        for (var i = 0; i < this.Report.Inputs.length; i++) {
            this.Report.Inputs[i].Remove();
            this.Report.Inputs[i].Show({ Client: this.Client, Container: Container, Scale: (this.IScale == undefined ? "3:9" : this.IScale) });
            this.Report.Inputs[i].SetEnable();
        }
        Container.find("input.form-control").bind("keyup", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            if (event.keyCode == 13) {
                if (this.getAttribute("data-AutoPostback") == "True") {
                    event.data.Report.Run();
                } else if (this.getAttribute("data-SendChange") == "True") {
                    event.data.Report.InputChange();
                }
            }
        });
        Container.find("input.form-control.datepicker").bind("change", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            if (this.getAttribute("data-AutoPostback") == "True") {
                event.data.Report.Run();
            } else if (this.getAttribute("data-SendChange") == "True") {
                event.data.Report.InputChange();
            }
        });
        Container.find("select.form-control").bind("change", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            if (this.getAttribute("data-AutoPostback") == "True") {
                event.data.Report.Run();
            } else if (this.getAttribute("data-SendChange") == "True") {
                event.data.Report.InputChange();
            }
        });
        Container.find("textarea.form-control").bind("keyup", { Report: this }, function (event) {
            event.data.Report.SetInputValue(this.name, this.value);
            if (this.getAttribute("data-AutoPostback") == "True") {
                event.data.Report.Run();
            } else if (this.getAttribute("data-SendChange") == "True") {
                event.data.Report.InputChange();
            }
        });
    };
    MesReport.prototype.ShowOutput = function (obj) {
        obj.Container.off("click");
        for (var i = 0; i < this.Report.Outputs.length; i++) {
            if (this.Report.Outputs[i].Name == obj.OutputName) {
                this.Report.Outputs[i].Remove();
                if (this.Report.Outputs[i].ContainerID) {
                    var thisContainer = $("#" + this.Report.Outputs[i].ContainerID);
                    this.Report.Outputs[i].Show(thisContainer);
                } else {
                    this.Report.Outputs[i].Show(obj.Container);
                }
                break;
            }
        }
        obj.Container.on("click", ".J_menuItem", OnLinkClick);
    };
    MesReport.prototype.ShowOutputs = function (Container) {
        Container.off("click");
        //Container.empty();
        for (var i = 0; i < this.Report.Outputs.length; i++) {
            if (this.Report.Outputs[i].OutputType != "ReportAlart") {
                this.Report.Outputs[i].Remove();
            }
            if (this.Report.Outputs[i].ContainerID) {
                var thisContainer = $("#" + this.Report.Outputs[i].ContainerID);
                this.Report.Outputs[i].Show(thisContainer, (this.OScale == undefined ? "3:9" : this.OScale));
            } else {
                this.Report.Outputs[i].Show(Container, (this.OScale == undefined ? "3:9" : this.OScale));
            }
        }
        Container.on("click", ".J_menuItem", OnLinkClick);
    };
    MesReport.prototype.InputChange = function () {
        layer.load(1, {
            shade: [0.8, '#ccc']
        });
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.RunClassName, this.InputChangeFunctionName, { ClassName: this.ReportClassName, Report: this.ReportJson }, this.CallBack, MessageID);
    };
    MesReport.prototype.Run = function () {
        layer.load(1, {
            shade: [0.2, '#00F']
        });
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.RunClassName, this.RunFunctionName, { ClassName: this.ReportClassName, Report: this.ReportJson }, this.CallBack, MessageID);
    };
    MesReport.prototype.DownFile = function () {
        layer.load(1, {
            shade: [0.8, '#ccc']
        });
        var MessageID = "MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ListenStationData(MessageID);
        this.Client.CallFunction(this.RunClassName, this.DownExcelFunctionName, { ClassName: this.ReportClassName, Report: this.ReportJson }, this.CallBack, MessageID);
    };
    MesReport.prototype.CallBack = function (d) {
        var _Mes = MesReport.prototype.ObjList[d.MessageID];
        delete MesReport.prototype.ObjList[d.MessageID];
        if (d.Status == "Pass") {
            _Mes.ReportJson = d.Data;
            _Mes.Report = new Report(d.Data, { Container: _Mes.OContainer, TitleHeight: _Mes.TitleHeight, ShowToolbar: _Mes.ShowToolbar });

            if (_Mes.IContainer) {
                _Mes.ShowInputs(_Mes.IContainer);
            }
            if (_Mes.OContainer) {
                _Mes.ShowOutputs(_Mes.OContainer);
            }
        }
        else {
            layer.msg(d.Message + ";" + d.Data, {
                icon: 2,
                time: 60000,
                title: 'ERROR',
                btn: ['OK']
            }, function () { });
        }
        _Mes.Resize();
        layer.closeAll("loading");
        $('[data-AutoPostback=True]').select();
    };
    MesReport.prototype.Resize = function () {
        this.Report.Outputs.forEach(function (t) {
            if (t.OutputType == "ReportTable" && t.FixedHeader) {
                var elid = t.Tittle.replace(" ", "");
                $("#" + elid).bootstrapTable('resetView', {
                    height: $(window).height() - 75
                });
                $('#' + elid).resize();
            }
        });
    };
    this.Init();
};

var Report = function (obj, ClientOptions) {
    this.Inputs = [];
    this.Outputs = [];
    this.Sqls = {};
    this.RunSqls = [];
    this.Layout = obj.Layout;
    this.Inputs.splice(0, this.Inputs.length);
    this.Outputs.splice(0, this.Outputs.length);
    if (ClientOptions.Container != undefined) {
        ClientOptions.Container.empty();
        for (var i = 0; i < this.Layout.length; i++) {
            var row = $("<div class='row'></div>");
            for (var x = 0; x < this.Layout[i].length; x++) {
                var col = $("<div id='" + this.Layout[i][x].ID + "'class='col-xs-" + this.Layout[i][x].Scale + "'></div>");
                row.append(col);
            }
            ClientOptions.Container.append(row);
        }
    }
    for (var i = 0; i < obj.Inputs.length; i++) {
        var ip = new ReportInput(obj.Inputs[i]);
        this.Inputs.push(ip);
    }
    for (var i = 0; i < obj.Outputs.length; i++) {
        var op = new ReportOutput(obj.Outputs[i], { ShowToolbar: ClientOptions.ShowToolbar, TitleHeight: ClientOptions.TitleHeight });
        this.Outputs.push(op);
    }
    Report.prototype.constructor = Report;
};
var ReportInput = function (obj) {
    obj = obj || {};
    this.Enable = true;
    this.SendChangeEvent = true;
    this.RememberLastInput = false;
    for (var key in obj) {
        this[key] = obj[key];
    }
    ReportInput.prototype.constructor = ReportInput;
    ReportInput.prototype.Show = function (obj) {
        var E = new InputElements(obj.Client, this.EnterSubmit, this.SendChangeEvent);
        var container = obj.Container;
        if (container == null) {
            return;
        }
        switch (this.InputType) {
            case "TXT":
                E.Text(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            case "Select":
                E.Select(container, this.Name, this.Name, this.Name, this.Value, this.ValueForUse, obj.Scale);
                break;
            case "DateTime":
                //E.Text(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                E.DataTimeText(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            case "TextArea":
                E.TextArea(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            case "Autocomplete":
                E.Autocomplete(container, this.Name, this.Name, this.Name, this.Value, this.ValueForUse, this.API, this.APIPara, this.RefershType, obj.Scale);
                break;
            case "DateTime2":
                E.DataTimeText2(container, this.Name, this.Name, this.Name, this.Value, this.RefershType, obj.Scale);
                break;
            default:

        }
    };
    ReportInput.prototype.SetFocus = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        $(selector).select();
        $(selector).focus();
    };
    ReportInput.prototype.SetEnable = function (flag) {
        var selector = "#" + this.Name + "_" + this.ID;
        var f = (flag == undefined ? this.Enable : flag);
        if (f) {
            $(selector).attr("disabled", false);
        }
        else {
            $(selector).attr("disabled", true);
        }
    };
    ReportInput.prototype.Remove = function () {
        var selector = "[view-group=" + this.Name + "]";
        $(selector).find("input.form-control,select.form-control").unbind("keypress");
        $(selector).remove();
    };
};
var ReportOutput = function (obj, ClientOption) {
    this.TitleHeight = (ClientOption.TitleHeight != null || ClientOption.TitleHeight != undefined) ? ClientOption.TitleHeight : 75;
    this.ContainerID = obj.ContainerID;
    this.OutputType = obj.OutputType || obj.Lib;
    //this.Tittle = obj.Tittle || (obj.title ? obj.title : (obj.title.text ? obj.title.text : this.OutputType + ' Undefine Title'));
    this.Tittle = obj.Tittle || (obj.title ? (obj.title.text ? obj.title.text : this.OutputType + ' Undefine Title') : "");
    this.TableRow = obj.Rows;
    this.TableColNames = obj.ColNames;
    this.TableHeaders = $.extend(true, [], obj.TableHeaders);
    this.ShowToolbar = obj.ShowToolbar != undefined ? obj.ShowToolbar : (ClientOption.ShowToolbar != undefined ? ClientOption.ShowToolbar : true);
    this.pagination = obj.pagination;
    this.SearchCol = obj.SearchCol;
    this.MergeCells = obj.MergeCells;
    this.FixedHeader = obj.FixedHeader;
    this.FixedCol = obj.FixedCol;
    this.ChartData = obj;
    this.ColCount = obj.ColCount;
    this.ColNum = obj.ColNum;
    this.RowNun = obj.RowNun;
    this.charts = {};
    this.PaginationServer = obj.PaginationServer;
    ReportOutput.prototype.constructor = ReportOutput;
    ReportOutput.prototype.Show = function (c, s) {
        var E = new OutputElements();
        switch (this.OutputType) {
            case "ReportChart":
                E.Chart(c, this.Tittle, this.ChartData, s);
                break;
            case "EChart":
                this.charts[this.Tittle] = E.EChart(c, this.Tittle, this.ChartData);
                break;
            case "ReportTable":
                E.Table(c, this.Tittle.replace(" ", ""), this.TableRow, this.TableColNames, this.TableHeaders, { SearchCol: this.SearchCol, FixedHeader: this.FixedHeader, FixedCol: this.FixedCol, pagination: this.pagination, MergeCells: this.MergeCells, TitleHeight: this.TitleHeight, ShowToolbar: this.ShowToolbar });
                break;
            case "ReportAlart":
                E.Alart(c, obj.Msg, obj.AlartType);
                break;
            case "ReportFile":
                E.File(this.ChartData.FileName, this.ChartData.FileContent);
                break;
            case "ReportColumns":
                break;
            default:
                c.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;

        }
    };
    ReportOutput.prototype.Remove = function () {
        this.Tittle == "" ? this.Tittle = "未定義Tittle" : this.Tittle;
        if (this.OutputType == "ReportTable") {
            $("#" + this.Tittle.replace(/\s/g, "")).bootstrapTable("destroy");
        }
        else if (this.OutputType == "EChart") {
            if (this.charts[this.Tittle]) {
                this.charts[this.Tittle].dispose();
            }
        }
        if (this.ContainerID) {
            $("#" + this.ContainerID).empty();
        } else {
            var selector = "[view-group=" + this.Tittle.replace(/\s/g, "") + "]";
            $(selector).remove();
        }
    };
};

var InputElements = function (client, AutoPostback, SendChange) {
    this.client = client;
    this.AutoPostback = AutoPostback;
    this.SendChange = SendChange;
    InputElements.prototype.constructor = InputElements;
    InputElements.prototype.GetData = function (API, APIPara, ID, CallBack) {
        var ClassName = API.substr(0, API.lastIndexOf("."));
        var FunctionName = API.substr(API.lastIndexOf(".") + 1);
        var Params = {};
        var ParamsKey = [];
        var ParamsValeu = [];
        if (APIPara.length != 0) {
            var PTemp1 = APIPara.split(',');
            for (var i = 0; i < PTemp1.length; i++) {
                var PTemp2 = PTemp1[i].split(':');
                ParamsKey.push(PTemp2[0]);
                ParamsValeu.push(PTemp2[1]);
            }
            for (var i = 0; i < ParamsKey.length; i++) {
                if (ParamsValeu[i].indexOf('[') >= 0) {
                    var selector = "[name=" + ParamsValeu[i].substr(1, ParamsValeu[i].length - 2) + "]";
                    Params[ParamsKey[i]] = $(selector).val();
                }
                else {
                    Params[ParamsKey[i]] = ParamsValeu[i];
                }
            }
        }
        this.client.CallFunction(ClassName, FunctionName, Params, CallBack, ID);
    };
    InputElements.prototype.Text = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\" " + (this.AutoPostback ? "data-AutoPostback='True'" : "data-AutoPostback='False'") + (this.SendChange ? "data-SendChange='True'" : "data-SendChange='False'") + ">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.DataTimeText = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        //var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\">");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control datepicker\" value=\"" + value + "\" data-date-format=\"yyyy-mm-dd hh:ii:ss\"" + (this.AutoPostback ? "data-AutoPostback='True'" : "data-AutoPostback='False'") + (this.SendChange ? "data-SendChange='True'" : "data-SendChange='False'") + ">");
        //var input = $("<input type=\"text\" value=\"2012-05-15 21:05\" id=\"datetimepicker\" data-date-format=\"yyyy-mm-dd hh:ii\">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        var datatimetext = $("#" + ID);
        datatimetext.datetimepicker().on('changeDate', function (ev) {
            //$("#"+ID). ev.date.format("yyyy-MM-dd hh:mm:ss");
        });
    };
    InputElements.prototype.DataTimeText2 = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control datepicker\" value=\"" + value + "\" data-date-format=\"yyyy-mm-dd\"" + (this.AutoPostback ? "data-AutoPostback='True'" : "data-AutoPostback='False'") + (this.SendChange ? "data-SendChange='True'" : "data-SendChange='False'") + ">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        var datatimetext = $("#" + ID);
        datatimetext.datetimepicker({ minView: 2, autoclose: "true" });
        //datatimetext.datetimepicker("setDate", new Date());
    };
    InputElements.prototype.Select = function (c, ID, Label, placeholder, value, DataForUse, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<select id=\"" + ID + "\" name=\"" + Label + "\" class=\"form-control\"  placeholder=\"" + placeholder + "\" aria-describedby=\"basic-addon1\" " + (this.AutoPostback ? "data-AutoPostback='True'" : "data-AutoPostback='False'") + (this.SendChange ? "data-SendChange='True'" : "data-SendChange='False'") + "></select>");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        var select = $("#" + ID);
        select.empty();
        for (var x = 0; x < DataForUse.length; x++) {
            var op = $(" <option value=\"" + DataForUse[x] + "\"" + (value == DataForUse[x] ? "selected" : "") + ">" + DataForUse[x] + "</option>");
            select.append(op);
        }
    };
    InputElements.prototype.Checkbox = function (c, ID, Label, placeholder, value, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"checkbox\" class=\"form-control\"" + (value ? "checked" : "") + (this.AutoPostback ? "data - AutoPostback='True'" : "data - AutoPostback='False'") + (this.SendChange ? "data - SendChange='True'" : "data - SendChange='False'") + ">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.Radio = function (c, ID, Label, placeholder, value, DataForUse, API, APIPara, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var DataForUse = [];//get data 
        for (var i = 0; i < DataForUse.length; i++) {
            var label = $("<label class=\"radio-inline\"></label>");
            var radio = $("<input type=\"radio\" name=\"" + Label + "\" id=\"" + ID + "_" + i + "\"" + (value == DataForUse[i] ? "checked" : "") + (this.AutoPostback ? "data - AutoPostback='True'" : "data - AutoPostback='False'") + (this.SendChange ? "data - SendChange='True'" : "data - SendChange='False'") + ">");
            label.append(radio, DataForUse[i]);
            inputD.append(label);
        }
        div.append(label, inputD);
        c.append(div);
    };
    InputElements.prototype.Autocomplete = function (c, ID, Label, placeholder, value, DataForUse, API, APIPara, RefershType, Scale) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input class=\"form-control\" name=\"" + Label + "\" id=\"" + ID + "\" value=\"" + value + "\" autocomplete=\"on\"" + (this.AutoPostback ? "data - AutoPostback='True'" : "data - AutoPostback='False'") + (this.SendChange ? "data - SendChange='True'" : "data - SendChange='False'") + ">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
        try {
            input.autocomplete("destroy");
        } catch (e) {

        }
        if (RefershType == "Default") {
            //input.attr("PreValue", value);
            this.GetData(API, APIPara, ID, function (e) {
                if (e.Status == "Pass") {
                    $("#" + e.MessageID).autocomplete({
                        minLength: 0,
                        source: e.Data,
                        appendTo: ".SearchBox",
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
                        scrollHeight: 10,
                        position: { my: "left top", at: "right top" }
                    });
                }
            });
        }
        else if (RefershType == "EveryTime") {
            $("#" + ID).autocomplete({
                minLength: 0,
                source: DataForUse,
                select: function (event, ui) {
                    var e = $.Event("keypress");
                    e.keyCode = 13;
                    $(this).val(ui.item.value);
                    $(this).trigger(e);
                },
                create: function (event, ui) {
                    $(this).bind("click", function () {
                        var active = $(this).attr("autocomplete"); //没有这一行，鼠标选择选项时，会触发输入的click事件，导致提示框不能关闭    
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
    };
    InputElements.prototype.LocalChecker = function (ID, Label, placeholder, value, RefershType, MessageID) {
        swal({
            title: Label,
            text: placeholder,
            type: "input",
            showCancelButton: true,
            closeOnConfirm: false,
            showLoaderOnConfirm: true,
            animation: "slide-from-top",
            inputPlaceholder: placeholder
        },
            function (inputValue) {
                if (inputValue === false) return false;
                if (inputValue === "") {
                    swal.showInputError(placeholder);
                    return false;
                }
                if (inputValue !== value) {
                    swal.showInputError("The value not match " + value + ",please input again!");
                    return false;
                }
                if (inputValue == value) {
                    this.client.CallFunction("", "", {}, function (e) { });
                }
            });
    };
    InputElements.prototype.TextArea = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<textarea id=\"" + ID + "\" name=\"" + Label + "\" rows=\"5\" cols=\"1\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\" " + (this.AutoPostback ? "data - AutoPostback='True'" : "data - AutoPostback='False'") + (this.SendChange ? "data - SendChange='True'" : "data - SendChange='False'") + "></textarea>");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
};

var pieChart = function (c, ID, ChartData) {
    c.highcharts({
        chart: {
            backgroundColor: 'rgba(0,0,0,0)'
        },
        title: {
            text: ChartData.Title
        },
        tooltip: {
            headerFormat: '{series.name}<br>',
            pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: {
            pie: {
                allowPointSelect: true,
                cursor: 'pointer',
                showInLegend: true,
                dataLabels: {
                    enabled: true,
                    format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                    style: {
                        color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                    }
                }
            }
        },
        subtitle: {
            text: ChartData.SubTitle
        },
        series: ChartData.ChartDatas
    });
}
var lineChart = function (c, ID, ChartData) {
    var plot = [];
    var xAxis = [];
    var seriesdata = ChartData.ChartDatas;
    var charttypes = ChartData.ChartDatas[0].type;
    //X軸非時間類型ChartData.Plot.type == 1
    if (ChartData.Plot.type == 1) {
        xAxis = {
            labels: {
                overflow: 'justify'
            },
            title: {
                text: ChartData.XAxis.Title
            },
            categories: ChartData.XAxis.Categories
        };
        plot = {
            series: {
                label: {
                    connectorAllowed: false
                },
                pointStart: ChartData.Plot.pointStartIntdata,
                pointInterval: ChartData.Plot.pointInterval
            }
        }
    }
    //X軸是時間類型
    else if (ChartData.Plot.type == 0) {
        xAxis = {
            type: 'datetime',
            labels: {
                overflow: 'justify'
            },
            title: {
                text: ChartData.XAxis.Title
            },
            categories: ChartData.XAxis.Categories
        };
        //給定初始值類型ChartData.Plot.pointStartDateTime != "0001-01-01 00:00:00"
        if (ChartData.Plot.pointStartDateTime != "0001-01-01 00:00:00") {
            plot = {
                charttypes: {
                    dataLabels: {
                        enabled: true          // 开启数据标签
                    },
                    lineWidth: 4,
                    states: {
                        hover: {
                            lineWidth: 5
                        }
                    },
                    marker: {
                        enabled: false
                    },
                    pointInterval: ChartData.Plot.pointInterval,
                    pointStart: new Date(ChartData.Plot.pointStartDateTime).getTime()
                }
            }
        }
        //鍵值類型;
        else {
            plot = {
                charttypes: {
                    dataLabels: {
                        enabled: true          // 开启数据标签
                    },
                    lineWidth: 4,
                    states: {
                        hover: {
                            lineWidth: 5
                        }
                    },
                    marker: {
                        enabled: false
                    },
                    pointInterval: ChartData.Plot.pointInterval,
                    pointStart: new Date(ChartData.Plot.pointStartDateTime).getTime()
                }
            }
            var datainfo = [];
            for (var i = 0; i < ChartData.ChartDatas[0].data.length; i++) {
                var dataitem = [new Date(ChartData.ChartDatas[0].data[i][0]).getTime(), ChartData.ChartDatas[0].data[i][1]];
                datainfo.push(dataitem);
            }
            seriesdata[0].data = datainfo;
        }
    };

    c.highcharts({
        chart: {
            backgroundColor: 'rgba(0,0,0,0)'
        },
        title: {
            text: ChartData.Title
        },
        xAxis: xAxis,
        yAxis: {
            title: {
                text: ChartData.YAxis.Title
            }
        },
        tooltip: {
            //valueSuffix: ChartData.Tooltip
            headerFormat: '<b>{series.name}</b><br>',
            pointFormat: '{point.x:%e. %b}: {point.y:.2f} m'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: plot,
        subtitle: {
            text: ChartData.SubTitle
        },
        series: seriesdata
    });
}
var columnChart = function (c, ID, ChartData) {
    c.highcharts({
        chart: {
            backgroundColor: 'rgba(0,0,0,0)'
        },
        title: {
            text: ChartData.Title
        },
        xAxis: {
            //labels: {
            //    overflow: 'justify'
            //},
            title: {
                text: ChartData.XAxis.Title
            },
            categories: ChartData.XAxis.Categories
        },
        yAxis: {
            title: {
                text: ChartData.YAxis.Title
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.2f}' + ChartData.Tooltip + '</b> of total<br/>'
        },
        credits: {
            enabled: false // 禁用版权信息
        },
        plotOptions: {
            series: {
                borderWidth: 0,
                dataLabels: {
                    enabled: true,
                    format: '{point.y:.1f}'
                }
            }
        },
        subtitle: {
            text: ChartData.SubTitle
        },
        series: ChartData.ChartDatas
    });
}


var OutputElements = function () {
    OutputElements.prototype.constructor = OutputElements;
    OutputElements.prototype.Chart = function (c, ID, ChartData) {
        var d = $("<div class=\"form-group panel panel-default\" view-group=\"" + ID + "\"></div>");
        var ctb_title = $("<div class=\"panel-heading\"><h3 class=\"panel-title\"><b>" + ID + "</b></h3></div>");
        var ctb_body = $("<div class=\"panel-body\"></div>");
        var p = $("<u id=\"" + ID + "\" class=\"form-control-static\"></u>");
        ctb_body.append(p);
        d.append(ctb_title, ctb_body);
        c.append(d);
        if (ChartData.Lib === "EChart") {
            var elem;
            if (ChartData.Zone_ID) {
                elem = document.getElementById(ChartData.Zone_ID);
            } else {
                elem = ctb_body[0];
            }
            var chart = echarts.init(elem);
            chart.setOption(ChartData);
            $(window).resize(chart.resize);
        }
        else {
            switch (ChartData.Type) {
                case "pieChart":
                    pieChart(ctb_body, ID, ChartData);
                    break;
                case "lineChart":
                    lineChart(ctb_body, ID, ChartData);
                    break;
                case "columnChart":
                    columnChart(ctb_body, ID, ChartData);
                    break;
                default: break;
            }
        }
    };
    OutputElements.prototype.EChart = function (c, ID, ChartData) {
        c.empty();
        c.width(c.width() - 15);
        if (c.height() <= 1) {
            c.height(c.parent().height());
        }
        var chartDom = document.getElementById(c.attr("ID"));
        var chart = echarts.init(chartDom);
        if (ChartData.Options) {
            chart.setOption(ChartData.Options);
        } else {
            chart.setOption(ChartData);
        }
        $(window).resize(chart.resize);
        return chart;
    };
    OutputElements.prototype.Alart = function (c, Msg, ObjType) {
        swal({
            title: "",
            text: Msg,
            type: ObjType
        }, function () { $(".SearchBox").toggle(); }
        );
    };
    OutputElements.prototype.Table = function (c, ID, RowsData, ColData, CHander, options) {
        options = options || { SearchCol: [], FixedHeader: false, FixedCol: [], ShowToolbar: true }
        var ctb = $("<div class=\"panel panel-default\"></div>");
        var ctb_title = $("<div class=\"panel-heading\"><h3 class=\"panel-title\"><b>" + (ID.length == 0 ? "查詢結果" : ID) + " :</b></h3></div>");
        var ctb_body = $("<div class=\"panel-body tablebox\"></div>");
        var tb = $("<table id=\"" + ID + "\" view-group=\"" + ID + "\" class=\"table table-condensed table-dark\"></table>");
        ctb_body.append(tb);
        ctb.append(ctb_title);
        ctb.append(ctb_body);
        c.append(ctb);
        var col = [];
        if (RowsData.length > 0) {
            var row = RowsData[0];
            if (CHander && CHander.length > 0) {
                for (var i = 0; i < CHander.length; i++) {
                    for (var x = 0; x < CHander[i].length; x++) {
                        if (CHander[i][x].rowspan == 0) {
                            delete CHander[i][x].rowspan;
                        }
                        if (CHander[i][x].colspan == 0) {
                            delete CHander[i][x].colspan;
                        }
                        CHander[i][x]["align"] = 'center';
                        CHander[i][x]["valign"] = 'middle';
                        if (CHander[i][x].field) {
                            CHander[i][x]["formatter"] = function (value, row, index) {
                                var res = "";
                                switch (value.LinkType) {
                                    case "Report":
                                        res = '<a class="J_menuItem" data-type="' + value.LinkType + '" data-index="' + value.Value + '" href="javascript:;" url="/FunctionPage/Report/Report.html?ClassName=' + value.LinkData + '&Data=' + value.Value + '&RunFlag=1">' + value.Value + '</a>';
                                        break;
                                    case "Link":
                                        res = '<a class="J_menuItem" data-type="' + value.LinkType + '" data-index="' + value.Value + '" href="javascript:;" url="' + value.LinkData + '">' + value.Value + '</a>';
                                        break;
                                    case "Attachment":
                                        res = '<a class="J_menuItem" data-type="' + value.LinkType + '" data-index="' + value.Value + '" href="javascript:;" url="' + value.LinkData + '">Attachment</a>';
                                        break;
                                    default:
                                        res = value.Value;
                                }
                                return res;
                            };
                            CHander[i][x]["cellStyle"] = function (value, row, index, field) {
                                /*{backgourdcolor:'',color:''}*/
                                var cssobj = { css: {} };
                                cssobj.css = $.extend({}, true, value.CellStyle);
                                if (value.RowSpan == 0 || value.ColSpan == 0) {
                                    cssobj.css['display'] = 'none';
                                }
                                return cssobj;
                            };
                        } else {
                            delete CHander[i][x].field;
                        }
                    }
                }
                if (CHander.length == 1) {
                    col = CHander[0];
                } else {
                    col = CHander;
                }
            } else {
                for (var item in row) {
                    if (ColData.Contain(item)) {
                        var cell = {
                            field: item,
                            title: item,
                            align: 'center',
                            valign: 'middle',
                            sortable: false,
                            formatter: function (value, row, index) {
                                var res = "";
                                switch (value.LinkType) {
                                    case "Report":
                                        res = '<a class="J_menuItem" data-type="' + value.LinkType + '" data-index="' + value.Value + '" href="javascript:;" url="/FunctionPage/Report/Report.html?ClassName=' + value.LinkData + '&Data=' + value.Value + '&RunFlag=1">' + value.Value + '</a>';
                                        break;
                                    case "Link":
                                        res = '<a class="J_menuItem" data-type="' + value.LinkType + '" data-index="' + value.Value + '" href="javascript:;" url="' + value.LinkData + '">' + value.Value + '</a>';
                                        break;
                                    case "Attachment":
                                        res = '<a class="J_menuItem" data-type="' + value.LinkType + '" data-index="' + value.Value + '" href="javascript:;" data-url="' + value.LinkData + '" url="#">Attachment</a>';
                                        break;
                                    default:
                                        res = value.Value;
                                }
                                return res;
                            },
                            cellStyle: function (value, row, index, field) {
                                /*{backgourdcolor:'',color:''}*/
                                var cssobj = { css: {} };
                                cssobj.css = $.extend({}, true, value.CellStyle);
                                if (value.RowSpan == 0 || value.ColSpan == 0) {
                                    cssobj.css['display'] = 'none';
                                }
                                return cssobj;
                            }
                        };
                        if (options.SearchCol && options.SearchCol.length > 0) {
                            if (options.SearchCol.Contain(item)) {
                                cell["searchable"] = true;
                            } else {
                                cell["searchable"] = false;
                            }
                        }
                        col.push(cell);
                    }
                };
            }
        }
        var tbOption = {
            striped: true,
            cache: false,
            columns: col,
            data: RowsData,
            rowStyle: function (row, index) {
                /*{backgourdcolor:'',color:''}*/
                var cssobj = { css: {} };
                cssobj.css = $.extend({}, true, row.RowStyle);
                return cssobj;
            },
            sidePagination: "client",
            onPageChange: function (number, size) {
                c.off('click');
                c.on("click", ".J_menuItem", OnLinkClick);
            },
            onToggle: function (cardView) {
                c.off('click');
                c.on("click", ".J_menuItem", OnLinkClick);
            },
            onSearch: function (e) {
                tb.bootstrapTable('resetView');
            },
            cardView: false,
            search: options.ShowToolbar ? (options.search == undefined ? true : options.search) : false,
            searchAlign: "right",
            searchTimeOut: 1000, //设置搜索超时时间
            showColumns: options.ShowToolbar ? true : options.ShowToolbar,
            showExport: options.ShowToolbar ? true : options.ShowToolbar,
            exportDataType: "all"
        };
        if (options.pagination) {
            tbOption["pagination"] = true;
            tbOption["pageSize"] = 10;
            tbOption["pageList"] = [10, 25, 50, 100, 200, 500];
        }
        if (options.FixedHeader && RowsData.length > 20) {
            tbOption["height"] = window.innerHeight - options.TitleHeight;
        }
        if (options.FixedCol > 0) {
            tbOption["fixedColumns"] = true;
            tbOption["fixedNumber"] = options.FixedCol;
        }
        if (options.MergeCells) {
            tbOption["onResetView"] = function (params) {
                for (var index = 0; index < RowsData.length; index++) {
                    var row = RowsData[index];
                    for (var item in row) {
                        if ((row[item].RowSpan && row[item].RowSpan > 1) || (row[item].ColSpan && row[item].ColSpan > 1)) {
                            tb.bootstrapTable('mergeCells', {
                                index: index,
                                field: item,
                                colspan: row[item].ColSpan,
                                rowspan: row[item].RowSpan
                            });
                        }
                    }
                }
            };
        }
        tb.bootstrapTable('destroy').bootstrapTable(tbOption);
        if (options.MergeCells) {
            tb.bootstrapTable('resetView');
        }
    };
    OutputElements.prototype.File = function (fileName, fileContent) {
        var blob = b64toBlob(fileContent);
        if (window.navigator.msSaveOrOpenBlob) {
            navigator.msSaveBlob(blob, fileName);
        } else {
            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = fileName;
            link.click();
            window.URL.revokeObjectURL(link.href);
        }
    };
};

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

var OnLinkClick = function (e) {
    // 获取标识数据
    var dataUrl = $(this).attr('url'),
        linkType = $(this).data('type'),
        dataIndex = $(this).data('index'),
        menuName = $.trim($(this).text() + "_Report"),
        flag = true;
    if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;
    switch (linkType) {
        case 'Report':
        case 'Link':
            // 选项卡菜单已存在
            $(self.parent.document).find(".J_menuTab").each(function () {
                if ($(this).data('id') == dataUrl) {
                    if (!$(this).hasClass('active')) {
                        $(this).addClass('active').siblings('.J_menuTab').removeClass('active');
                        self.parent.window.scrollToTab(this);
                        // 显示tab对应的内容区
                        $(self.parent.document).find('.J_mainContent .J_iframe').each(function () {
                            if ($(this).data('id') == dataUrl) {
                                $(this).show().siblings('.J_iframe').hide();
                                return false;
                            }
                        });
                    }
                    flag = false;
                    return false;
                }
            });

            // 选项卡菜单不存在
            if (flag) {
                $(self.parent.document).find('.J_iframe').hide();
                var str = '<a href="javascript:;" class="active J_menuTab" data-id="' + dataUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
                $(self.parent.document).find('.J_menuTab').removeClass('active');
                // 添加选项卡对应的iframe
                var str1 = '<iframe class="J_iframe" name="iframe' + dataIndex + '" width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" seamless></iframe>';
                $(self.parent.document).find('.J_mainContent').find('iframe.J_iframe').hide().parents('.J_mainContent').append(str1);
                // 添加选项卡
                $(self.parent.document).find('.J_menuTabs .page-tabs-content').append(str);
                self.parent.window.scrollToTab($(self.parent.document).find('.J_menuTab.active'));
            }
            break;
        case 'Attachment':
            var layerid = $.trim($(this).text() + "_" + dataIndex);
            var LinkObj = eval("[" + dataUrl + "]")[0];
            var className = LinkObj.Class,
                functionName = LinkObj.Function,
                params = LinkObj.Paras;
            layer.load(2, { shade: [0.2, '#393D49'] });
            self.parent.client.CallFunction(className, functionName, params, function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    var container = $("<div class='full-height' style='background-color: #02020296;'></div>");
                    var previewlist = $("<ul class='nav-tabs vertical-tab col-xs-2 full-height' style='background-color: #02020296;' role='tablist'></ul>");
                    container.append(previewlist);
                    var box = $("<div class='tab-content vertical-tab-content col-xs-10 full-height' style='background-color: #02020296;'></div>");
                    container.append(box);

                    for (var i = 0; i < e.Data.length; i++) {
                        previewlist.append($("<li role='presentation' " + (i == 0 ? "class='active'" : "") + " style='list-style:none;margin-top:10px'><a  style='border:none; padding:0;height:auto;display:block;' href='#IMG" + e.Data[i].name + "' role='tab' data-toggle='tab'><img width='100%' height='80px' src='../../" + e.Data[i].URL + "'/></a></li>"));
                        box.append($("<div role='tabpanel' class='tab-pane " + (i == 0 ? "active" : "") + "  full-height' style='overflow-y:scroll' id='IMG" + e.Data[i].name + "'><img width='100%' src='../../" + e.Data[i].URL + "'/></div>"));
                    }
                    self.parent.layer.open({
                        id: layerid,
                        title: false,
                        shade: [0.2, '#020202'],
                        area: ['60%', '90%'],
                        type: 1,
                        scroll: false,
                        content: container[0].outerHTML
                    });
                } else {

                }
            });
            break;
        default:
    }
    return false;
};

