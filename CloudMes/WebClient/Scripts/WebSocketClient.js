var WebSocketClient = function () {
    WebSocketClient.prototype.isopen = false;
    this.webSocket = null;
    this.connectUrl = function (url) {
        this.webSocket = new WebSocket(url);
        this.webSocket.onerror = function (event) {
            WebSocketClient.prototype.callerrorbak(event);
        }
        this.webSocket.onopen = function (event) {
            WebSocketClient.prototype.setConnFlag(true);
            WebSocketClient.prototype.callopenbak(event);
        }
        this.webSocket.onclose = function (event) {
            WebSocketClient.prototype.callclosebak(event);
            WebSocketClient.prototype.unInit();
        }
        this.webSocket.onmessage = function (event) {
            WebSocketClient.prototype.callmsgbak(event.data);
        }
        return true;
    };
    if (typeof WebSocket._callmsgbak == "undefined") {
        WebSocketClient.prototype.callmsgbak = function (data) {

        }
        WebSocketClient._callmsgbak = true;
    }
    if (typeof WebSocketClient._callopenbak == "undefined") {
        WebSocketClient.prototype.callopenbak = function (data) {

        }
        WebSocketClient._callopenbak = true;
    }
    if (typeof WebSocketClient._callerrorbak == "undefined") {
        WebSocketClient.prototype.callerrorbak = function (data) {

        }
        WebSocketClient._callerrorbak = true;
    }
    if (typeof WebSocketClient._callclosebak == "undefined") {
        WebSocketClient.prototype.callclosebak = function (data) {

        }
        WebSocketClient._callclosebak = true;
    }
    if (typeof WebSocketClient._isConn == "undefined") {
        WebSocketClient.prototype.isConn = function (data) {
            return WebSocketClient.prototype.isopen;
        }
        WebSocketClient._isConn = true;
    }
    if (typeof WebSocketClient._setConnFlag == "undefined") {
        WebSocketClient.prototype.setConnFlag = function (flag) {
            WebSocketClient.prototype.isopen = flag;
        }
        WebSocketClient._setConnFlag = true;
    }
    if (typeof WebSocketClient._connect == "undefined") {
        WebSocketClient.prototype.connect = function (url, msgbak, openbak, errorbak, closebak) {
            WebSocketClient.prototype.callmsgbak = msgbak;
            WebSocketClient.prototype.callopenbak = openbak;
            WebSocketClient.prototype.callerrorbak = errorbak;
            WebSocketClient.prototype.callclosebak = closebak;
            if (this.webSocket == null)//表示连接成功  
            {
                this.connectUrl(url);
            }
            else if (this.webSocket.readyState == 3) {
                this.webSocket = null;
                this.connectUrl(url);
            }
            if (this.webSocket.readyState != 1) {
                return false;
            }
            return true;
        }
        WebSocketClient._connect = true;
    }

    if (typeof WebSocketClient._sendData == "undefined") {
        WebSocketClient.prototype.sendData = function (data) {
            if (WebSocketClient.prototype.isConn() && this.webSocket != null) {
                this.webSocket.send(data);
                return true;
            }
            else {
                return false;
            }
        }
        WebSocketClient._sendData = true;
    }
    if (typeof WebSocketClient._close == "undefined") {
        WebSocketClient.prototype.close = function (data) {
            this.webSocket.close(1000, "Closeing");
            WebSocketClient.prototype.isopen = false;
            this.webSocket = null;
            return true;
        }
        WebSocketClient._close = true;
    }
    if (typeof WebSocketClient._uninit == "undefined") {
        WebSocketClient.prototype.unInit = function (data) {
            WebSocketClient.prototype.isopen = false;
            this.webSocket = null;
            return true;
        }
        WebSocketClient._uninit = true;
    }
}

