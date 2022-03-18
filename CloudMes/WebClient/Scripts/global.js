(function ($) {
    var o = $({});
    $.MES = {
        DEBUG: false,
        VERSION: "1.0",
        SOCKET_SERVERIP:window.location.hostname ,//"localhost",
        SOCKET_SERVERPORT: "2130", 
        SOCKET_SERVICE: "ReportService",
        PRINTER_PORT: "2600",
        DEFAULT_LAN: "ENGLISH", 
        CK_LAN_NAME: "MES_LAN",
        CK_TOKEN_NAME: "Token",
        CK_BU_NAME: "BU_NAME",
        CK_LINE_LIST: "LINE_LIST",
        CK_LINE_NAME: "CURRENT_LINE",
        getQueryString: function (p) {
            var reg = new RegExp('(^|&)' + p + '=([^&]*)(&|$)', 'i');
            var r = self.window.location.search.substr(1).match(reg);
            if (r != null) {
                return unescape(r[2]);
            }
            return null;
        },
        getQueryObj: function () {
            var p = self.window.location.search.substr(1).split("&");
            var robj = [];
            for (var i = 0; i < p.length; i++) {
                var po = p[i].split("=");
                if (po.length > 1)
                    robj[i] = [po[0], po[1]];
            }
            return robj;
        },
        filterInvisibleChar: function (s) {
            var res = s;
            for (var i = 0; i < res.length; i++) {
                var ascchar = res.charAt(i).charCodeAt();
                if (ascchar < 32 || ascchar > 126) {
                    s=s.replace(res.charAt(i), "").trim();
                }
            }
            return s;
        },
        getPathName: function () {
            var ph = self.window.location.pathname.substr(1);
            var p = ph.substr(0, ph.lastIndexOf('/') + 1);
            return p;
        },
        getQuickStart: function (o) {
            var arr = [];
            var search = function (r, d, v, p) {
                if (typeof (d) == "object") {
                    if (d.hasOwnProperty("length")) {
                        for (var i = 0; i < d.length; i++) {
                            search(r, d[i], v, p);
                        }
                    }
                    else {
                        OutHere:
                            for (var n in d) {
                                switch (typeof (d[n])) {
                                    case "string":
                                        if (p == undefined) {
                                            var t = d[n].toUpperCase();
                                            if (t.indexOf(v.toUpperCase()) >= 0) {
                                                r.push(d);
                                                break OutHere;/*当这里匹配时跳出循环，不再循环子级*/
                                            }
                                        }
                                        else if (p.Contain(n)) {
                                            var t = d[n].toUpperCase();
                                            if (t.indexOf(v.toUpperCase()) >= 0) {
                                                r.push(d);
                                                break OutHere;/*当这里匹配时跳出循环，不再循环子级*/
                                            }
                                        }
                                        break;
                                    case "object":
                                        search(r, d[n], v, p);
                                        break;
                                    default:
                                        break;
                                }
                            }
                    }
                }
                else if (typeof (d) == "string") {
                    if (p == undefined && d.toUpperCase().indexOf(v.toUpperCase()) >= 0) {
                        r.push(d);
                    }
                }
            };
            search(arr, o.Data, o.Value, o.Fields);
            return arr;
        },
        mergeCells: function (data, fieldName, colspan, target) {
            var sortMap = {};
            var ii = 0;
            for (var i = 0 ; i < data.length ; i++) {
                for (var prop in data[i]) {
                    if (prop == fieldName) {
                        if (i > 0) {
                            if (sortMap.hasOwnProperty(ii) && data[i][prop] == data[i - 1][prop]) {
                                sortMap[ii] = sortMap[ii] * 1 + 1;
                            } else {
                                ii++;
                                sortMap[ii] = 1;
                            }
                        } else {
                            sortMap[ii] = 1;
                        }
                        break;
                    }
                }
            }
            var index = 0;
            for (var prop in sortMap) {
                var count = sortMap[prop] * 1;
                target.bootstrapTable('mergeCells', { index: index, field: fieldName, colspan: colspan, rowspan: count });
                index += count;
            }
        },
        sleep: function (Millis) {
            var now = new Date();
            var exitTime = now.getTime() + Millis;
            while (true) {
                now = new Date();
                if (now.getTime() > exitTime)
                    return;
            }
        },
        changeURLArg: function (url, arg, val) {
            var pattern = arg + '=([^&]*)';
            var replaceText = arg + '=' + val;
            return url.match(pattern) ? url.replace(eval('/(' + arg + '=)([^&]*)/gi'), replaceText) : (url.match('[\?]') ? url + '&' + replaceText : url + '?' + replaceText);
        }
    };
    $.getScripts = function (urls, CallBack) {
        for (var i in urls) {
            $.ajax({
                url: urls[i] + "?V=" + $.MES.VERSION,
                dataType: "script",
                cache: true,/*开启缓存*/
                async: false,/*同步执行*/
                success: function (e) {
                    if ($.MES.DEBUG) {
                        console.log("loadScript:" + this.url);
                    }
                },
                error: function (e, err) {
                    console.log("loadScript error>>url:" + this.url + ",Error:" + err);
                }
            });
        }
        CallBack();
    };
    $.subscribe = function () {
        o.on.apply(o, arguments);
    };
    $.unsubscribe = function () {
        o.off.apply(o, arguments);
    };
    $.publish = function () {
        o.trigger.apply(o, arguments);
        o.off.apply(o, arguments);
    };
    $.publishMoreTime = function () {
        o.trigger.apply(o, arguments);
    };
}(jQuery));

(function (a, b) {
    "use strict";
    var c = function () {
        var a = [["requestFullscreen", "exitFullscreen", "fullscreenchange", "fullscreen", "fullscreenElement"], ["webkitRequestFullScreen", "webkitCancelFullScreen", "webkitfullscreenchange", "webkitIsFullScreen", "webkitCurrentFullScreenElement"], ["mozRequestFullScreen", "mozCancelFullScreen", "mozfullscreenchange", "mozFullScreen", "mozFullScreenElement"]];
        for (var c = 0,
        d = a.length; c < d; c++) {
            var e = a[c];
            if (e[1] in b) return e
        }
    }();
    if (!c) return a.screenfull = !1;
    var d = "ALLOW_KEYBOARD_INPUT" in Element,
    e = {
        init: function () {
            return b.addEventListener(c[2],
            function (a) {
                e.isFullscreen = b[c[3]],
                e.element = b[c[4]],
                e.onchange(a)
            }),
            this
        },
        isFullscreen: b[c[3]],
        element: b[c[4]],
        request: function (a) {

            a = a || b.documentElement,
            a[c[0]](d && Element.ALLOW_KEYBOARD_INPUT),
            b.isFullscreen || a[c[0]]();
            //alert("dd");
        },
        exit: function () {
            b[c[1]]()
        },
        toggle: function (a) {
            this.isFullscreen ? this.exit() : this.request(a)
        },
        onchange: function () { }
    };
    a.screenfull = e.init()
})(window, document);

Date.prototype.format = function (format) {
    /*
    * eg:format="yyyy-MM-dd hh:mm:ss";
    */
    var o = {
        "M+": this.getMonth() + 1, // month
        "d+": this.getDate(), // day
        "h+": this.getHours(), // hour
        "m+": this.getMinutes(), // minute
        "s+": this.getSeconds(), // second
        "q+": Math.floor((this.getMonth() + 3) / 3), // quarter
        "S": this.getMilliseconds()
        // millisecond
    }

    if (/(y+)/.test(format)) {
        format = format.replace(RegExp.$1, (this.getFullYear() + "").substr(4
						- RegExp.$1.length));
    }

    for (var k in o) {
        if (new RegExp("(" + k + ")").test(format)) {
            format = format.replace(RegExp.$1, RegExp.$1.length == 1
							? o[k]
							: ("00" + o[k]).substr(("" + o[k]).length));
        }
    }
    return format;
};
Date.prototype.GetTimeString = function (timestamp) {
    return new Date(parseInt(timestamp)).format("yyyy-MM-dd hh:mm:ss");
};
String.prototype.GetQueryString = function (p) {
    var reg = new RegExp('(^|&)' + p + '=([^&]*)(&|$)', 'i');
    var par = this.substr(this.indexOf("?") + 1);
    var r = par.match(reg);
    if (r != null) {
        return unescape(r[2]);
    }
    return null;
};
Array.prototype.Contain = function (s) {
    if (this.indexOf && typeof (this.indexOf) == 'function') {
        var index = this.indexOf(s);
        if (index >= 0) {
            return true;
        }
    }
    return false;
};
$.HtmlControl = {
    MustScan: function () {
        var MustScanInputTime = 0;
        $("input[data-scan='true']").unbind("keydown");
        $("input[data-scan='true']").attr("autocomplete", "off").bind("contextmenu", function () {
            return false;
        }).bind("keydown", function () {
            if (event.keyCode == 13) {
                MustScanInputTime = 0;
            }
            else if (event.keyCode != 93 && event.keyCode < 112 && event.keyCode > 47) {
                if (MustScanInputTime == 0) {
                    MustScanInputTime = event.timeStamp;
                }
                else {
                    var Timing = event.timeStamp - MustScanInputTime;
                    if (Timing > 150) {
                        alert("please scan for input!");
                        MustScanInputTime = 0;
                        this.value = "";
                        event.returnvalue = "";
                        return false;
                    }
                    else {
                        MustScanInputTime = event.timeStamp;
                    }
                }
            }
        }).bind("paste", function () {
            return false;
        }).bind("dragenter", function () {
            $("#" + this.id).attr("readonly", "readonly");
        }).bind("dragleave", function () {
            $("#" + this.id).removeAttr("readonly");
        });
    }
};

var StringToDateUTC = function (strData) {
    var dataTime = new Date(strData);
    var year = dataTime.getFullYear();
    var month = dataTime.getMonth() + 1;
    var day = dataTime.getDate();
    var hour = dataTime.getHours();
    var minute = dataTime.getMinutes();
    var second = dataTime.getSeconds();
    return Date.UTC(year, month, day, hour, minute, second);
}

String.prototype.replaceAll = function (s1, s2) {
    return this.replace(new RegExp(s1, "gm"), s2);
}