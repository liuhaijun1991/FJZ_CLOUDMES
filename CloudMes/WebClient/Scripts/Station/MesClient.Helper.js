var Helper = function (o) {
    this.ClientID = "";
    this.websocket = null;
    this.OnOpen = o.OnOpen;
    this.IsOpen = false;
    this.MContainer = o.MContainer;
    Helper.prototype.Init = function (obj) {
        if (this.ClientID == "") {
            this.ClientID = "CID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        }
        if (obj.ServerIP && obj.Port && obj.ServiceName) {
            this.websocket = new WebSocket("ws://" + obj.ServerIP + ":" + obj.Port + "/" + obj.ServiceName);
            this.websocket.ClientID = this.ClientID;
        }
        if (obj.OnMessage) {
            this.websocket.onmessage = obj.OnMessage;
        }
        else {
            this.websocket.onmessage = function (e) {
                if ($.MES.DEBUG) {
                    console.log("Get>_" + e.data);
                }
                var d = JSON.parse(e.data);
                $.publish(d.MessageID, d);
                var tc = Helper.prototype.ThisClient[this.ClientID];
                var m;
                if (d.Status == "Pass") {
                    m = new StationMessage({ State: 1, Message: d.Data });
                }
                else {
                    m = new StationMessage({ State: 0, Message: d.Message });
                }
                m.Show("", tc.MContainer);
            };
        }
        if (obj.OnOpen) {
            this.websocket.onopen = function (e) {
                if ($.MES.DEBUG) {
                    console.log("onOpen:MesHelper connection open");
                }
                var tc = Helper.prototype.ThisClient[this.ClientID];
                tc.IsOpen = true;
                obj.OnOpen(e);
                var m = new StationMessage({ State: 1, Message: "Connect to MesHelper ok!" });
                m.Show("", tc.MContainer);
            }
        }
        else {
            this.websocket.onopen = function (e) {
                if ($.MES.DEBUG) {
                    console.log("onOpen:MesHelper connection open");
                }
                var tc = Helper.prototype.ThisClient[this.ClientID];
                tc.IsOpen = true;
                var m = new StationMessage({ State: 1, Message: "Connect to MesHelper ok!" });
                m.Show("", tc.MContainer);
            };
        }
        if (obj.OnClose) {
            this.websocket.onclose = function (e) {
                var tc = Helper.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                var m = new StationMessage({ State: 0, Message: "MesHelper connection close!" });
                m.Show("", tc.MContainer);
                if ($.MES.DEBUG) {
                    console.log("onClose:connection close");
                }
                obj.OnClose();
            };
        }
        else {
            this.websocket.onclose = function (e) {
                var tc = Helper.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                var m = new StationMessage({ State: 0, Message: "MesHelper connection close!" });
                m.Show("", tc.MContainer);
                if ($.MES.DEBUG) {
                    console.log("onClose:MesHelper connection close");
                }
            };
        }
        if (obj.OnError) {
            this.websocket.onerror = obj.OnError;
        }
        else {
            this.websocket.onerror = function (e) {
                if ($.MES.DEBUG) {
                    console.log("onError: MesHelper connection Fail!");
                }
                var tc = Helper.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                var m = new StationMessage({ State: 0, Message: "MesHelper Error!" });
                m.Show("", tc.MContainer);
            };
        }
        Helper.prototype.ThisClient[this.ClientID] = this;
    }
    Helper.prototype.ThisClient = {};
    Helper.prototype.CallFunction = function (TCode, Data, CallBack, MessageID) {
        MessageID = MessageID ? MessageID : ("MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString());
        if (CallBack != null && CallBack != undefined) {
            $.subscribe(MessageID, function (e, d) {
                CallBack(d);
            });
        }
        var data = { ClientID: this.ClientID, MessageID: MessageID, TCode: TCode, Data: Data };
        if (this.websocket.readyState == 1) {
            var jsonStr = JSON.stringify(data);
            if ($.MES.DEBUG) {
                console.log("Send>_" + jsonStr);
            }
            this.websocket.send(jsonStr);
        } else {
            console.log("Error>_ WebSocket not ready,State:" + this.websocket.readyState);
        }
    };
    Helper.prototype.Print = function (Data, CallBack) {
        this.CallFunction("PRINT", Data, CallBack);
    };
    Helper.prototype.StillPrint = function (Data, CallBack) {
        this.CallFunction("STILLPRINT", Data, CallBack);
    };
    Helper.prototype.Prints = function (Data, CallBack) {
        this.CallFunction("PRINTS", Data, CallBack);
    };
    Helper.prototype.ReadData = function (CallBack) {
        this.CallFunction("GETCOMDATA", null, CallBack);
    };
    this.Init({ ServerIP: "localhost", Port: $.MES.PRINTER_PORT, ServiceName: "MESHelper", OnOpen: this.OnOpen });
}


