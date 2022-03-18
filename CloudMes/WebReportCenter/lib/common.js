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
Array.prototype.Contain = function (s) {
    if (this.indexOf && typeof (this.indexOf) == 'function') {
        var index = this.indexOf(s);
        if (index >= 0) {
            return true;
        }
    }
    return false;
};
Array.prototype.GetChild = function (k, v) {
    for (var i = 0; i < this.length; i++) {
        if (this[i].hasOwnProperty(k) && this[i][k] == v) {
            return this[i];
            break;
        } else if (k.indexOf('.') > 0) {
            var kArr = k.split('.');
            var t = this[i];
            for (var x = 0; x < kArr.length - 1; x++) {
                if (t.hasOwnProperty(kArr[x])) {
                    t = t[kArr[x]];
                }
            }
            if (t.hasOwnProperty(kArr[kArr.length - 1]) && t[kArr[kArr.length - 1]] == v) {
                return t;
            }
        }
        if (this[i].hasOwnProperty("Childs") && this[i]["Childs"]) {
            if (this[i]["Childs"].length > 0) {
                for (var n = 0; n < this[i]["Childs"].length; n++) {
                    if (this[i]["Childs"][n].hasOwnProperty(k) && this[i]["Childs"][n][k] == v) {
                        return this[i]["Childs"][n];
                    } else if (k.indexOf('.') > 0) {
                        var kArr = k.split('.');
                        var t = this[i]["Childs"][n];
                        for (var x = 0; x < kArr.length - 1; x++) {
                            if (t.hasOwnProperty(kArr[x])) {
                                t = t[kArr[x]];
                            }
                        }
                        if (t.hasOwnProperty(kArr[kArr.length - 1]) && t[kArr[kArr.length - 1]] == v) {
                            return t;
                        }
                    }
                }
            }
        }
    }
    return null;
};
Array.prototype.Delete = function (k, v) {
    var index = -1;
    for (var i = 0; i < this.length; i++) {
        if (this[i].hasOwnProperty(k) && this[i][k] == v) {
            index = i;
            break;
        }
    }
    if (index >= 0) {
        this.splice(index, 1);
    }
};
Array.prototype.Distinct = function (keys) {
    var ResArr = [];
    var KeyArr = [];
    keys = keys || [];
    if (keys.constructor === Array) {
        KeyArr = keys;
    }
    if (keys.constructor === String) {
        if (keys.trim() != "") {
            KeyArr.push(keys);
        }
    }
    if (KeyArr.length == 0) {
        for (var i = 0; i < this.length; i++) {
            if (!ResArr.Contain(this[i])) {
                ResArr.push(this[i]);
            }
        }
    } else {
        for (var i = 0; i < this.length; i++) {
            if (ResArr.length == 0) {
                ResArr.push(this[i]);
                continue;
            }

            var Temp = null;
            for (var x = 0; x < ResArr.length; x++) {
                var p = 0;
                for (var n = 0; n < KeyArr.length; n++) {
                    if (this[i].hasOwnProperty(KeyArr[n]) && ResArr[x].hasOwnProperty(KeyArr[n])) {
                        if (this[i][KeyArr[n]] == ResArr[x][KeyArr[n]]) {
                            p++;
                        }
                    }
                }
                if (p == KeyArr.length) {
                    Temp = null;
                    break;
                }
                if (p < KeyArr.length && x == (ResArr.length - 1)) {
                    Temp = this[i];
                }
            }
            if (Temp != null) {
                ResArr.push(Temp);
            }
        }
    }
    return ResArr;
};

layui.define(['jquery', 'layer'], function (exports) {
    "use strict";
    var $ = layui.jquery;
    var so = $({});

    exports('common', {
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
                    if (p == undefined && d.indexOf(v.toUpperCase()) >= 0) {
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
            for (var i = 0; i < data.length; i++) {
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
        subscribe: function () {
            so.on.apply(so, arguments);
        },
        unsubscribe: function () {
            so.off.apply(so, arguments);
        },
        publish: function () {
            so.trigger.apply(so, arguments);
            so.off.apply(so, arguments);
        },
        publishMoreTime: function () {
            so.trigger.apply(so, arguments);
        }
    });
});