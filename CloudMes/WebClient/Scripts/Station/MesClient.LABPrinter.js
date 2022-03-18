var LABPrinter = function (OnOpen) {
	this.ClientID = "";	
	this.websocket = null;
	this.OnOpen = OnOpen;
	this.IsOpen = false;
	LABPrinter.prototype.Init = function (obj) {
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
			};
		}
		if (obj.OnOpen) {
			this.websocket.onopen = function (e) {
				if ($.MES.DEBUG) {
					console.log("onOpen:connection open");
				}
				var tc = LABPrinter.prototype.ThisClient[this.ClientID];
				tc.IsOpen = true;
				obj.OnOpen(e);
			}
		}
		else {
			this.websocket.onopen = function (e) {
				var tc = LABPrinter.prototype.ThisClient[this.ClientID];
				tc.IsOpen = true;
				if ($.MES.DEBUG) {
					console.log("onOpen:connection open");
				}
			};
		}
		if (obj.OnClose) {
			this.websocket.onclose = function (e) {
				var tc = LABPrinter.prototype.ThisClient[this.ClientID];
				tc.IsOpen = false;
				if ($.MES.DEBUG) {
					console.log("onClose:connection close");
				}
				obj.OnClose();
			};
		}
		else {
			this.websocket.onclose = function (e) {
				var tc = LABPrinter.prototype.ThisClient[this.ClientID];
				tc.IsOpen = false;
				if ($.MES.DEBUG) {
					console.log("onClose:connection close");
				}
				swal({
					title: "Connetion close",
					text: "The server connection close",
					type: "error"
				},
                function () {
                	parent.window.location.reload();
                });
			};
		}
		if (obj.OnError) {
			this.websocket.onerror = obj.OnError;
		}
		else {
			this.websocket.onerror = function (e) {
				if ($.MES.DEBUG) {
					console.log("onError: connection Fail!");
				}
				swal("Connetion Fail", "Can Not Connet to Server! URL:" + e.target.url, "error");
			};
		}
		LABPrinter.prototype.ThisClient[this.ClientID] = this;
	}
	LABPrinter.prototype.ThisClient = {};
	LABPrinter.prototype.CallFunction = function (ClassName, FunctionName, Data, CallBack, MessageID) {
		MessageID = MessageID ? MessageID : ("MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString());
		if (CallBack != null && CallBack != undefined) {
			$.subscribe(MessageID, function (e, d) {
				CallBack(d);
			});
		}
		var data = { Token: this.Token, ClientID: this.ClientID, MessageID: MessageID, Class: ClassName, Function: FunctionName, Data: Data };
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
	LABPrinter.prototype.Print = function () {

	};
	this.Init({ ServerIP: "localhost", Port: $.MES.PRINTER_PORT, ServiceName: "PRINTER", OnOpen: this.OnOpen });
}


