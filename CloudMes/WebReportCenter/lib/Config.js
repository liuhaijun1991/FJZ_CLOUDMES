layui.define(function (exports) {
    "use strict";
    var IConfig = {
        DEBUG: true,
        VERSION: "1.0",
        SOCKET_SERVERIP: "localhost",
        SOCKET_SERVERPORT: "2130",
        SOCKET_SERVICE: "ReportService",
        PRINTER_PORT: "2600",
        DEFAULT_LAN: "CHINESE",
        CK_LAN_NAME: "MES_LAN",
        CK_TOKEN_NAME: "Token",
        CK_BU_NAME: "BU_NAME",
        CK_LINE_LIST: "LINE_LIST",
        CK_LINE_NAME: "CURRENT_LINE"
    };
    
    exports('Config', IConfig);
});