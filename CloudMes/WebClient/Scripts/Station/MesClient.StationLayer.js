var StationLayer = function (o) {
    this.ID = o.ID;
    this.ServerMessageID = o.ServerMessageID;
    this.Station = o.Station;
    this.Client = o.Client;
    this.CallBackClass = o.FunctionName ? o.FunctionName : "MESStation.GlobalConfig.CallUIFunctionAPI";
    this.CallBackFunctionName = o.FunctionName ? o.FunctionName : "UIFunctionCallBack";
    this.OutputContainer = o.OutputContainer;
    this.InputContainer = o.InputContainer;
    this.MsgContainer = o.MsgContainer;
    this.OutputObj = o.OutputObj;
    this.InputObj = o.inputObj;
    this.Inputs = null;
    this.Outputs = [];
    this.player = parent.layer;
    StationLayer.prototype.Init = function () {
        this.InitOutputContainer();
        this.InitInputContainer();
        this.InitTitleContainer();
        this.InitMsgContainer();
        this.initLayerStyle();
    };
    StationLayer.prototype.InitOutputContainer = function () {
        for (var i = 0; i < this.OutputObj.length; i++) {
            var outputitem = new StationLayerOutput(this.OutputObj[i]);
            outputitem.Show(this.OutputContainer, "3:7");
            if (outputitem.DisplayType != UIOutputType.TextArea)
                $("#outputfield").show();
        }
    };
    StationLayer.prototype.InitTitleContainer = function () {
        if (this.InputObj.Tittle == "") $("#title").hide();
        var tittlehtml = "<i id=\"icon\" class=\"layui-icon layui-icon-notice\"></i> <b> " + this.InputObj.Tittle + "</b>";
        $("#title").html(tittlehtml);
    };
    StationLayer.prototype.InitInputContainer = function () {
        Inputs = new StationLayerInput({ ID: this.ID, DisplayName: this.InputObj.Message, Name: this.InputObj.Name, DisplayType: this.InputObj.Type, Container: this.InputContainer, Parentobj: this, Player: this.player, Station: this.Station, placeholder: this.InputObj.placeholder });
        if (Inputs.DisplayType != UIControlType.Alart) $("#inputfield").show();
        Inputs.Show();
        Inputs.InitInputEvent();
        Inputs.SetFocus();
    };
    StationLayer.prototype.InitMsgContainer = function () {
        if (o.inputObj.CBMessage != null) {
            var fieldset = $("<fieldset class=\"layui-elem-field\"></fieldset>");
            var legend =
                $("<legend class=\"legend\"><span class=\"layui-badge \" style=\"font-size:5px\">ErrMsg</span></legend>");
            var div = $("<div class=\"layui-field-box\" style=\"text-align:center;word-break:break-all;\"><i class=\"layui-icon layui-icon-close-fill\"  style=\"font-size:20px;color:#be002f;\"> </i> <span style=\"color:#be002f;\"> <b>" +
                o.inputObj.CBMessage +
                "</b></span></div>");
            fieldset.append(legend, div);
            this.MsgContainer.append(fieldset);
        }
    };
    StationLayer.prototype.Cancel = function () {
        this.Client.CallFunction(this.CallBackClass, this.CallBackFunctionName, { StationLayerReturnType: StationLayerReturnType.Cancel, ServerMessageID: this.ServerMessageID, StationLayerData: this.InputObj.ErrMessage }, null, this.ServerMessageID);
    };
    StationLayer.prototype.Close = function () {
        this.Client.CallFunction(this.CallBackClass, this.CallBackFunctionName, { StationLayerReturnType: StationLayerReturnType.Close, ServerMessageID: this.ServerMessageID, StationLayerData: this.InputObj.ErrMessage }, null, this.ServerMessageID);
    };
    StationLayer.prototype.initLayerStyle = function () {
        var IconType = this.InputObj.IconType;
        var style = "";
        switch (IconType) {
            case LayerIconType.None:
                style = "layui-bg-gray";
                break;
            case LayerIconType.Message:
                style = "layui-bg-blue";
                break;
            case LayerIconType.Alert:
                style = "layui-bg-red";
                break;
            case LayerIconType.Warning:
                style = "layui-bg-orange";
                break;
            default:
                style = " ";
        }
        $("#layerbody").addClass(style);
        $("#layercard").addClass(style);
        $("#icon").addClass(style);
    };
    //this.Init();
};
var StationLayerInput = function (obj) {
    this.ID = obj.ID;
    this.Name = obj.Name;
    this.Value = obj.Value;
    this.DisplayName = obj.DisplayName;
    this.DisplayType = obj.DisplayType;
    this.placeholder = obj.placeholder;
    this.Container = obj.Container;
    this.Station = obj.Station;
    this.ParenObj = obj.Parentobj;
    this.Client = obj.Parentobj.Client;
    this.player = obj.Player;
    StationLayerInput.prototype.constructor = StationLayerInput;
    StationLayerInput.prototype.Show = function () {
        var E = new LayerInputElements();
        var container = this.Container;
        switch (this.DisplayType) {
            case UIControlType.String:
                E.Text(container, this.Name, this.DisplayName, this.placeholder, "", "", "3:7", this.ID, true);
                break;
            case UIControlType.Password:
                E.Password(container, this.Name + "_" + this.ID, this.DisplayName, this.placeholder, "", "", "3:7", true);
                break;
            case UIControlType.Confirm:
                E.Confirm(container, this.Name + "_" + this.ID, this.DisplayName, this.placeholder, "", "", "3:7", true);
                break;
            case UIControlType.Alart:
                E.Alart(container, this.Name + "_" + this.ID, this.Name, this.Name, this.DisplayName, "", "3:7", true);
                break;
            case UIControlType.Table:
                E.Table(container, this.DisplayName, this.DataForUse);
                break;
            case UIControlType.Weight:
                E.Weight(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, "", "", "3:7", true);
                break;
            case UIControlType.YesNo:
                E.YesNo(container, this.Name + "_" + this.ID, this.DisplayName, this.Name, "", "", "3:7", true);
                break;
            default:
                container.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;
        }
    };
    StationLayerInput.prototype.InitInputEvent = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        var ScanInput = $(selector);
        ScanInput.unbind("keypress");
        ScanInput.val("");
        ScanInput.bind("keypress", { layerValue: this }, function (event) {
            if (event.keyCode == 13) {
                var a = event.data.layerValue;
                a.SendData(StationLayerReturnType.Reply);
            }
        });
        $("#InputContainer").find("button").bind("click", { layerValue: this }, function (event) {
            var a = event.data.layerValue;
            a.SendDataBtn(StationLayerReturnType.Close,$(this));
            try {
                $.publishMoreTime(this.name, this.value);
            } catch (e) {

            }
        });
        if (this.DisplayType == UIControlType.Weight) {
            var weightinput = $("#" + this.Name + "_" + this.ID);
            var weightinputobj = this;
            this.Station.MesHelper.ReadData(function (e) {
                weightinput.val(e.Data.replace("OK,", ""));
                weightinputobj.SendData(StationLayerReturnType.Reply);
            });
        };
    };
    StationLayerInput.prototype.SendData = function (stationLayerReturnType) {
        var selector = "#" + this.Name + "_" + this.ID;
        var ScanInput = $(selector);
        this.Client.CallFunction(this.ParenObj.CallBackClass, this.ParenObj.CallBackFunctionName, { StationLayerReturnType: stationLayerReturnType, ServerMessageID: this.ParenObj.ServerMessageID, StationLayerData: ScanInput.val() }, this.Callback(this.player), this.ParenObj.ServerMessageID);
    };
    StationLayerInput.prototype.SendDataBtn = function (stationLayerReturnType,sender) {
        //var selector = "#" + this.Name + "_" + this.ID;
        var ScanInput = sender;
        this.Client.CallFunction(this.ParenObj.CallBackClass, this.ParenObj.CallBackFunctionName, { StationLayerReturnType: stationLayerReturnType, ServerMessageID: this.ParenObj.ServerMessageID, StationLayerData: ScanInput.val() }, this.Callback(this.player), this.ParenObj.ServerMessageID);
    };
    StationLayerInput.prototype.ClearValue = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        var e = $(selector);
        e.val("");
    };
    StationLayerInput.prototype.SetFocus = function () {
        var selector = "#" + this.Name + "_" + this.ID;
        var e = $(selector);
        e.focus();
    };
    StationLayerInput.prototype.SetEnable = function (flag) {
        var selector = "#" + this.Name + "_" + this.ID;
        var f = (flag == undefined ? this.Enable : flag);
        if (f) {
            $(selector).attr("disabled", false);
        }
        else {
            $(selector).attr("disabled", true);
        }
    };
    StationLayerInput.prototype.SetVisable = function (flag) {
        var selector = "[view-group=" + this.Name + "_" + this.ID + "]";
        var f = (flag == undefined ? this.Visable : flag);
        if (f) {
            $(selector).show();
        }
        else {
            $(selector).hide();
        }
    };
    StationLayerInput.prototype.Callback = function (playerobj) {
        var index = playerobj.getFrameIndex(window.name); //眔?玡iframe?ま
        playerobj.close(index);
    };
    StationLayerInput.prototype.Remove = function () {
        var selector = "[view-group=" + this.Name + "_" + this.ID + "]";
        $(selector).find("input.form-control,select.form-control").unbind("keypress");
        $(selector).remove();
    };
};
var StationLayerOutput = function (obj) {
    this.ID = obj.ID;
    this.Name = obj.Name;
    this.DisplayType = obj.DisplayType;
    this.Value = obj.Value;
    StationLayerOutput.prototype.constructor = StationLayerOutput;
    StationLayerOutput.prototype.Show = function (c, s) {
        var E = new LayerOutputElements();
        switch (this.DisplayType) {
            case UIOutputType.Text:
                E.Text(c, this.Name, this.Value, s, "text");
                break;
            case UIOutputType.Table:
                E.Table(c, this.Name, this.Value);
                break;
            case UIOutputType.TextArea:
                E.TextArea(c, this.Name, this.Value);
                break;
            default:
                c.append("<span>DisplayType " + this.DisplayType + " undefined,input name " + this.Name + "</span>");
                break;

        }
    };
    StationLayerOutput.prototype.Remove = function () {
        var selector = "[view-group=" + this.Name + "]";
        if (this.DisplayType == "Table") {
            $("#" + this.ID).bootstrapTable("destroy");
        }
        $(selector).remove();
    };
};
var UIControlType = {
    String: 1,
    YesNo: 2,
    Password: 3,
    Select: 4,
    Table: 5,
    Confirm: 6,
    Alart: 7,
    Weight: 8
}
var UIOutputType = {
    Text: "Text",
    Table: "Table",
    Select: "Select",
    Password: "Password",
    TextArea: "TextArea"
}
var LayerIconType = {
    Message: 1,
    Alert: 2,
    Warning: 3,
    None: 4
}
var LayerInputElements = function () {
    LayerInputElements.prototype.constructor = LayerInputElements;
    LayerInputElements.prototype.Text = function (c, Name, Label, placeholder, value, RefershType, Scale,objID, ScanFlag) {
        var scales = Scale.split(':');
        var ID = Name + "_" + objID;
        if (placeholder == "" || placeholder == null)
            placeholder = Name;
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div><br />");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\" " + (ScanFlag ? "data-scan=\"true\"" : "") + " onfocus=\"this.removeAttribute('readonly'); \" onblur=\"this.setAttribute('readonly', true);\">");

        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    LayerInputElements.prototype.Password = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div><br />");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"password\" class=\"form-control\" placeholder=\"" + placeholder + "\" value=\"" + value + "\" " + (ScanFlag ? "data-scan=\"true\"" : "") + " onfocus=\"this.removeAttribute('readonly'); \" onblur=\"this.setAttribute('readonly', true);\">");

        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    LayerInputElements.prototype.Confirm = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        //var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\"></label>");
        var inputD = $("<div class=\"col-xs-12\" style=\"text-align:center\"></div>");
        var btn = $("<button class=\"btn btn-primary small\" id=\"" + ID + "\" name=\"" + Label + "\" value=\"" + value + "\">" + Label + "</button>");
        inputD.append(btn);
        div.append(inputD);
        c.append(div);
    };
    LayerInputElements.prototype.YesNo = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        //var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\"></label>");
        var inputD = $("<div class=\"row\" style=\"text-align:center\"></div>");
        var btn = $("<div class=\"col-xs-2 col-xs-offset-4\"><button class=\"btn btn-primary btn-sm  btn-block\" id=\"" + ID + "Yes\" name=\"" + Label + "\" value=\"Yes\">Yes</button></div>");
        inputD.append(btn);       
        var btn1 = $("<div class=\"col-xs-2\"><button class=\"btn btn-danger btn-sm  btn-block\" id=\"" + ID + "No\" name=\"" + Label + "\" value=\"No\">No</button></div>");
        inputD.append(btn1);
        div.append(inputD);
        c.append(div);
    };
    LayerInputElements.prototype.Alart = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) { };
    LayerInputElements.prototype.Weight = function (c, ID, Label, placeholder, value, RefershType, Scale, ScanFlag) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div><br />");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + Label + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + Label + "\"  type=\"text\" class=\"form-control weightflag\" placeholder=\"" + placeholder + "\"  disabled=\"true\" value=\"" + value + "\" " + (ScanFlag ? "data-scan=\"true\"" : "") + ">");

        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
};
var LayerOutputElements = function () {
    LayerOutputElements.prototype.constructor = LayerOutputElements;
    LayerOutputElements.prototype.Text = function (c, ID, value, Scale, inputType) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div>");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + ID + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + ID + "\"  type='" + inputType + "' class=\"form-control\" value=\"" + value + "\"  disabled=\"true\">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    LayerOutputElements.prototype.Table = function (c, ID, value) {
        var tb = $("<table id=\"" + ID + "\" view-group=\"" + ID + "\"></table>");
        c.append(tb);
        var col = [];
        if (value.length > 0) {
            for (var item in value[0]) {
                var cell = {
                    field: item,
                    title: item,
                    align: 'center',
                    valign: 'middle',
                    sortable: false
                };
                col.push(cell);
            }
        }
        tb.bootstrapTable({
            pagination: false,
            striped: true,
            cache: false,
            columns: col,
            data: value
        });
    };
    LayerOutputElements.prototype.Password = function (c, ID, value, Scale, inputType) {
        var scales = Scale.split(':');
        var div = $("<div class=\"form-group\" view-group=\"" + ID + "\"></div><br />");
        var label = $("<label for=\"" + ID + "\" class=\"col-xs-" + scales[0] + " control-label text-right\">" + ID + ":" + "</label>");
        var inputD = $("<div class=\"col-xs-" + scales[1] + "\"></div>");
        var input = $("<input id=\"" + ID + "\" name=\"" + ID + "\"  type=\"password\"  class=\"form-control\" value=\"" + value + "\"  disabled=\"true\">");
        inputD.append(input);
        div.append(label, inputD);
        c.append(div);
    };
    LayerOutputElements.prototype.TextArea = function (c, ID, value, Scale, inputType) {
        var fieldset = $("<fieldset class=\"layui-elem-field\"></fieldset>");
        var legend = $("<legend class=\"legend\"><span class=\"layui-badge layui-bg-cyan\" style=\"font-size:5px\">" + ID + "</span></legend>");
        var div = $("<div class=\"layui-field-box\" style=\"text-align:center;word-break:break-all\">" + value + "</div>");
        fieldset.append(legend, div);
        $("#OnputTextAreaContainer").append(fieldset);
        var fields = $("#OnputTextAreaContainer").find("fieldset");
        fields.removeClass(function (index, className) {
            var cls = className.split(' ');
            var res = '';
            for (var i = 0; i < cls.length; i++) {
                if (cls[i] && cls[i].startsWith("col-xs-")) {
                    res += ' ' + cls[i];
                }
            }
            return res;
        });
        fields.each(function (index, el) {
            if (fields.length < 3) {
                $(el).addClass('col-xs-' + (12 / fields.length));
            }
            else {
                $(el).addClass('col-xs-4');
            }
        });
    };

};



