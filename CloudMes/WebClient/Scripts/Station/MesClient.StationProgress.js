var StationProgress = function (o) {
    this.ID = o.ID;
    this.ServerMessageID = o.ServerMessageID;
    this.Station = o.Station;
    this.Client = o.Client;
    this.CallBackClass = o.FunctionName ? o.FunctionName : "MESStation.GlobalConfig.CallUIFunctionAPI";
    this.CallBackFunctionName = o.FunctionName ? o.FunctionName : "UIFunctionCallBack";
    this.OutputContainer = o.OutputContainer;
    this.MsgContainer = o.MsgContainer;
    this.OutputObj = o.OutputObj;
    this.Outputs = [];
    this.player = parent.layer;
    StationProgress.prototype.Init = function () {
        this.InitOutputContainer();
    };
    StationProgress.prototype.InitOutputContainer = function () {
        this.OutputContainer.empty();
        for (var i = 0; i < this.OutputObj.length; i++) {
            var outputitem = new StationProgressOutput(this.OutputObj[i]);
            outputitem.Show(this.OutputContainer);
        }
        this.Reply();
    };
    StationProgress.prototype.Reply = function () {
        this.Client.CallFunction(this.CallBackClass,
            this.CallBackFunctionName,
            {
                StationLayerReturnType: 2,//Reply,
                ServerMessageID: this.ServerMessageID,
                StationLayerData: "Reply"
            },
            null,
            this.ServerMessageID);
    };
};
var StationProgressOutput = function (obj) {
    this.ID = obj.ID;
    this.Name = obj.Name;
    this.DisplayType = obj.DisplayType;
    this.Value = obj.Value;
    StationProgressOutput.prototype.constructor = StationProgressOutput;
    StationProgressOutput.prototype.Show = function (c) {
        var E = new LayerOutputElements();
        switch (this.DisplayType) {
            case UIOutputType.Text:
                E.Text(c, this.Name, this.Value);
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
var LayerOutputElements = function () {
    LayerOutputElements.prototype.constructor = LayerOutputElements;
    LayerOutputElements.prototype.Text = function (c, ID, value) {
        var label = $("<dt>" + ID + ":" + "</dt>");
        var input = $("<dd>" + value + "</dd>");
        c.append(label, input);
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



