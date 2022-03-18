var MesClient = function (OnOpen,OnMessage,OnClose,OnError) {
    this.ClientID = "";
    this.UserInfo = {
        ID: "",
        EMP_NO: "",
        EMP_NAME: "",
        EMP_LEVEL: "",
        Department: "",
        FACTORY: "",
        BU: ""
    };
    this.Token = null;//登錄令牌
    this.Permission = null;//權限 
    this.websocket = null;
    this.OnOpen = OnOpen;
    this.OnMessage = OnMessage;
    this.OnClose = OnClose;
    this.OnError = OnError;
    this.IsOpen = false;
    MesClient.prototype.Init = function (obj) {
        if (this.ClientID == "") {
            this.ClientID = "CID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        }
        if (obj.ServerIP && obj.Port && obj.ServiceName) {
            this.websocket = new WebSocket((window.location.protocol == "https:" ? "wss://" : "ws://") + obj.ServerIP + ":" + obj.Port + "/" + obj.ServiceName);
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
                var tc = MesClient.prototype.ThisClient[this.ClientID];
                tc.IsOpen = true;
                obj.OnOpen(e);
            }
        }
        else {
            this.websocket.onopen = function (e) {
                var tc = MesClient.prototype.ThisClient[this.ClientID];
                tc.IsOpen = true;
                if ($.MES.DEBUG) {
                    console.log("onOpen:connection open");
                }
            };
        }
        if (obj.OnClose) {
            this.websocket.onclose = function (e) {
                var tc = MesClient.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                if ($.MES.DEBUG) {
                        console.log("onClose:connection close;" + e.reason);
                }
                obj.OnClose();
            };
        }
        else {
            this.websocket.onclose = function (e) {
                var tc = MesClient.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                if ($.MES.DEBUG) {
                    console.log("onClose:connection close");
                }
                swal({
                    title: "Connection close",
                    text: "The server connection close;" + e.reason,
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
                swal("Connection Fail", "Can Not Connect to Server! URL:" + e.target.url, "error");
            };
        }
        MesClient.prototype.ThisClient[this.ClientID] = this;
    }
    MesClient.prototype.ThisClient = {};
    MesClient.prototype.CallFunction = function (ClassName, FunctionName, Data, CallBack, MessageID) {
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
    MesClient.prototype.SetUserInfo = function (UserInfo) {
        if (UserInfo) {
            this.UserInfo.ID = UserInfo.ID;
            this.UserInfo.EMP_NO = UserInfo.EMP_NO;
            this.UserInfo.EMP_NAME = UserInfo.EMP_NAME;
            this.UserInfo.EMP_LEVEL = UserInfo.EMP_LEVEL;
            this.UserInfo.FACTORY = UserInfo.FACTORY;
            this.UserInfo.Department = UserInfo.DPT_NAME;
            this.UserInfo.BU = UserInfo.BU_NAME;
        }
        else {
            this.UserInfo.ID = null;
            this.UserInfo.EMP_NO = null;
            this.UserInfo.EMP_NAME = null;
            this.UserInfo.EMP_LEVEL = null;
            this.UserInfo.FACTORY = null;
            this.UserInfo.Department = null;
            this.UserInfo.BU = null;
        }
    };
    MesClient.prototype.SetToken = function (Token) {
        this.Token = Token;
        $.cookie($.MES.CK_TOKEN_NAME, Token, { expires: 1, path: '/' });
    };
    MesClient.prototype.GetBUList = function (CallBack) {
        this.CallFunction("MESStation.GlobalConfig.GetCommonConfig", "GetBU", {}, CallBack);
    };
    MesClient.prototype.GetStationLine = function (CallBack) {
        this.CallFunction("MESStation.Config.LineConfig", "GetAllLine", {}, CallBack);
    };
    MesClient.prototype.GetLanguageList = function (CallBack) {
        this.CallFunction("MESStation.GlobalConfig.GetCommonConfig", "GetLanguageType", {}, CallBack);
    };
    MesClient.prototype.GetLanguage = function (Language, PageName, CallBack) {
        var cookieName = Language + "_" + PageName;
        var cLan = localStorage.getItem(cookieName);
        if (!cLan && cLan != "null") {
            this.CallFunction("MESStation.GlobalConfig.LanguagePageConfig", "GetPageLanguage", { PageName: PageName, LanguageValue: Language }, function (event) {
                if (event.Status == "Pass") {
                    localStorage.setItem(cookieName, JSON.stringify(event.Data));
                    CallBack(event);
                }
                else {
                    CallBack(event);
                }
            });
        }
        else {
            try {
                var data = JSON.parse(cLan);
                var ev = { Status: "Pass", Message: "", Data: data };
                CallBack(ev);
            } catch (e) {
                this.CallFunction("MESStation.Test.LanguagePageTest", "GetPageLanguage", { PageName: PageName, LanguageValue: Language }, function (event) {
                    localStorage.setItem(cookieName, JSON.stringify(event.data));
                    CallBack(event);
                });
            }
        }
    };
    MesClient.prototype.GetMesApiClass = function (CallBack) {
        this.CallFunction("MESStation.ApiHelper", "GetApiClassList", {}, CallBack);
    };
    MesClient.prototype.GetMesFunctionList = function (ClassName, CallBack) {
        this.CallFunction("MESStation.ApiHelper", "GetApiFunctionsList", { CLASSNAME: ClassName }, CallBack);
    };
    MesClient.prototype.Login = function (userName, Password, CallBack) {
        this.CallFunction("MESStation.MESUserManager.UserManager", "Login", { EMP_NO: userName, Password: Password, Language: $.cookie($.MES.CK_LAN_NAME), BU_NAME: $.cookie($.MES.CK_BU_NAME) }, function (e) {
            var tc = MesClient.prototype.ThisClient[e.ClientID];
            if (e.Status == "Pass") {
                tc.SetToken(e.Data.Token);
                tc.SetUserInfo(e.Data.UserInfo);
            }
            CallBack(e);
        });
    };
    MesClient.prototype.Logout = function (CallBack) {
        this.CallFunction("MESStation.UserManager", "Logout", { TOKEN: this.Token }, function (e) {
            var tc = MesClient.prototype.ThisClient[e.ClientID];
            if (e.Status == "Pass") {
                tc.SetToken(null);
                tc.SetUserInfo(null);
            }
            CallBack(e);
        });
    };
    MesClient.prototype.CheckToken = function (CallBack) {
        var CKTK = $.cookie($.MES.CK_TOKEN_NAME);
        this.CallFunction("MESStation.MESUserManager.UserManager", "CheckToken", { Token: CKTK }, function (e) {
            var tc = MesClient.prototype.ThisClient[e.ClientID];
            if (e.Status == "Pass") {
                tc.SetToken(CKTK);
                tc.SetUserInfo(e.Data);
            }
            else {
                tc.SetToken(null);
                tc.SetUserInfo(null);
            }
            CallBack(e);
        });
    };
    MesClient.prototype.GetPermission = function (CallBack) {
        this.CallFunction("GETPERMISSION", "GETPERMISSION", {}, CallBack);
    };
    MesClient.prototype.GetMenu = function (CallBack) {
        this.CallFunction("MESStation.GlobalConfig.SystemMenuConfig", "GetMenu", { EMP_NO: this.UserInfo.EMP_NO }, CallBack);
    };
    this.Init({ ServerIP: $.MES.SOCKET_SERVERIP, Port: $.MES.SOCKET_SERVERPORT, ServiceName: $.MES.SOCKET_SERVICE, OnOpen: this.OnOpen, OnMessage: this.OnMessage, OnClose: this.OnClose, OnError: this.OnError });
}


