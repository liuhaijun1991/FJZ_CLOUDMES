layui.config({
    base: '../../lib/'
}).define(['jquery', 'layer', 'common', 'Config'], function (exports) {
    "use strict";
    var $ = layui.jquery;
    var layer = layui.layer;
    var IWebSocket = function (options) {
        options = options || {
            url: 'ws://' + layui.Config.SOCKET_SERVERIP + ':' + layui.Config.SOCKET_SERVERPORT + '/' + layui.Config.SOCKET_SERVICE
        };
        for (var key in options) {
            this[key] = options[key];
        }
    };

    IWebSocket.prototype.ThisClient = {};
    IWebSocket.prototype.SetToken = function (Token) {
        this.Token = Token;
    };
    IWebSocket.prototype.SetUserInfo = function (Data) {
        if (Data.UserInfo) {
            for (var key in Data.UserInfo) {
                this.UserInfo[key] = Data.UserInfo[key];
            }
            if (Data.Roles) {
                this.UserInfo['Roles'] = $.extend([], Data.Roles);
            }
            if (Data.RoleLeaders) {
                this.UserInfo['RoleLeaders'] = $.extend([], Data.RoleLeaders);
            } else {
                this.UserInfo['RoleLeaders'] = [];
            }
        }
        else {
            this.UserInfo = {};
        }
    };
    IWebSocket.prototype.Call = function (ClassName, FunctionName, Data, CallBack, MessageID) {
        MessageID = MessageID ? MessageID : ("MSGID" + parseInt(Math.random() * 99).toString() + Date.now().toString());
        if (CallBack != null && CallBack != undefined) {
            layui.common.subscribe(MessageID, function (e, d) {
                CallBack(d);
            });
        }
        var data = { Token: this.Token, ClientID: this.ClientID, MessageID: MessageID, Class: ClassName, Function: FunctionName, Data: Data };
        if (this.ws.readyState == 1) {
            var jsonStr = JSON.stringify(data);
            this.ws.send(jsonStr);
            if (layui.Config.DEBUG) {
                console.log("Send>_" + jsonStr);
            } else {
                console.log("Send Trigger");
            }
        } else {
            console.log("Error>_ WebSocket not ready,State:" + this.ws.readyState);
        }
    };
    IWebSocket.prototype.Login = function (userName, Password, CallBack) {
        this.Call("MESStation.MESUserManager.UserManager", "Login", {
            EMP_NO: userName, Password: Password, Language: layui.Config.DEFAULT_LAN, BU_NAME: "HWD"
        }, function (e) {
            var tc = IWebSocket.prototype.ThisClient[e.ClientID];
            if (e.Status == "Pass") {
                tc.SetToken(e.Data.Token);
                tc.SetUserInfo(e.Data);
            }
            CallBack(e);
        });
    };
    IWebSocket.prototype.CheckToken = function (Token, CallBack) {
        this.Call("MESStation.Common", "CheckToken", {
            TOKEN: Token
        }, CallBack);
    };
    IWebSocket.prototype.Logout = function (CallBack) {
        this.CallFunction("MESStation.UserManager", "Logout", { TOKEN: this.Token }, function (e) {
            var tc = IWebSocket.prototype.ThisClient[e.ClientID];
            if (e.Status == "Pass") {
                tc.SetToken(null);
                tc.SetUserInfo(null);
            }
            CallBack(e);
        });
    };

    var IWS = new IWebSocket();

    IWS.config = function (options) {
        options = options || {};
        for (var key in options) {
            this[key] = options[key];
        }
        return this;
    };

    //初始化
    IWS.init = function (options, callback) {
        this.IsOpen = false;
        this.Token = null;//登錄令牌
        this.UserInfo = {};
        if (typeof options === 'object') {
            this.config(options);
        } else if (typeof options == 'function') {
            callback = options;
        }

        this.ClientID = "CID" + parseInt(Math.random() * 99).toString() + Date.now().toString();
        this.ws = new WebSocket(this.url);
        this.ws.ClientID = this.ClientID;
        IWebSocket.prototype.ThisClient[this.ws.ClientID] = this;
        if (this.OnMessage) {
            this.ws.onmessage = this.OnMessage;
        }
        else {
            this.ws.onmessage = function (e) {
                var d = JSON.parse(e.data);
                layui.common.publish(d.MessageID, d);
                if (layui.Config.DEBUG) {
                    console.log("Get>_" + e.data);
                } else {
                    console.log("OnMessage Trigger");
                }
            };
        }
        if (callback) {
            this.ws.onopen = function (e) {
                var tc = IWebSocket.prototype.ThisClient[this.ClientID];
                tc.IsOpen = true;
                callback(e);
                console.log("onOpen Trigger");
            }
        }
        else {
            this.ws.onopen = function (e) {
                var tc = IWebSocket.prototype.ThisClient[this.ClientID];
                tc.IsOpen = true;
                console.log("onOpen Trigger");
            };
        }
        if (this.OnClose) {
            this.ws.onclose = function (e) {
                var tc = IWebSocket.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                this.OnClose();
                console.log("onClose Trigger");
                layer("连接断开，{time}秒后重新连接，或刷新页面", {
                    time: 30 * 1000,
                    shade: 0.6,
                    success: function (layero, index) {
                        var msg = layero.text();
                        var i = 30;
                        var timer = null;
                        var fn = function () {
                            layero.find(".layui-layer-content").text(msg.replace(/{time}/, i));
                            if (!i) {
                                layer.close(index);
                                clearInterval(timer);
                            }
                            i--;
                        };
                        timer = setInterval(fn, 1000);
                        fn();
                    },
                }, function () {
                    window.location.reload();
                });
            };
        }
        else {
            this.ws.onclose = function (e) {
                var tc = IWebSocket.prototype.ThisClient[this.ClientID];
                tc.IsOpen = false;
                console.log("onClose Trigger");
                layer("连接断开，{time}秒后重新连接，或刷新页面", {
                    time: 30 * 1000,
                    shade: 0.6,
                    success: function (layero, index) {
                        var msg = layero.text();
                        var i = 30;
                        var timer = null;
                        var fn = function () {
                            layero.find(".layui-layer-content").text(msg.replace(/{time}/, i));
                            if (!i) {
                                layer.close(index);
                                clearInterval(timer);
                            }
                            i--;
                        };
                        timer = setInterval(fn, 1000);
                        fn();
                    },
                }, function () {
                    window.location.reload();
                });
            };
        }
        if (this.OnError) {
            this.ws.onerror = this.OnError;
            layer("通讯错误，请检查网络状态",
                {
                    icon: 2
                },
                function () {

                });
        }
        else {
            this.ws.onerror = function (e) {
                console.log("onError: connection Fail!" + e.target.url);
                layer("通讯错误，请检查网络状态",
                    {
                        icon: 2
                    },
                    function () {

                    });
            };
        }
    };

    exports('IWebSocket', IWS);
});