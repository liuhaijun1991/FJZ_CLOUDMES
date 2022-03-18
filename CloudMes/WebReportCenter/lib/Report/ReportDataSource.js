layui.define(['jquery'], function (exports) {
    "use strict";

    var $ = layui.jquery;

    //页面构造函数
    var DataSource = function (options) {
        this.options = options || {};
    };

    //渲染
    DataSource.prototype.render = function (callback) {
        if (this.data === null) return;

        //调用渲染完毕的回调函数
        if (typeof callback == 'function')
            callback.call(this);
    };

    //设置数据
    DataSource.prototype.setData = function (callback) {
        this['DataSet'] = {};
        var data = this.options.data;
        for (var i = 0; i < data.length; i++) {
            switch (data[i].type) {
                case 'SOCKET':
                    this['DataSet'][data[i].id] = common.RunSql(data[i].data);
                    break;
                case 'SQL':
                    this['DataSet'][data[i].id] = common.RunSql(data[i].data);
                    break;
                case 'HTTP':
                    this['DataSet'][data[i].id] = common.RunAJAX(data[i].data, {});
                    break;
                case 'JSON':
                    this['DataSet'][data[i].id] = common.ParseJSON(data[i].data);
                    break;
                default:
            }
        }
        if (typeof callback == 'function')
            callback.call(this);
    };

    var common = {
        RunSql: function (sql) {
            var res = [];
            return res;
        },
        RunAJAX: function (url, data) {
            var res = null;
            $.get({
                async: false,
                url: url,
                dataType: 'text',
                data: data,
                success: function (r) {
                    res = eval(r);
                },
                error: function (e) {
                    res = 'error';
                }
            });
            return res;
        },
        ParseJSON: function (JsonStr) {
            var obj;
            try {
                obj = JSON.parse(JsonStr);
            } catch (e) {
                obj = e;
            }
            return obj;
        }
    }

    var datasource = new DataSource();

    //配置
    datasource.config = function (options) {
        options = options || {};
        for (var key in options) {
            this.options[key] = options[key];
        }
        return this;
    };

    //初始化
    datasource.init = function (options, callback) {
        if (typeof options === 'object') {
            this.config(options);
        } else if (typeof options == 'function') {
            callback = options;
        }
        //缓存回调函数
        this.done = callback = callback || this.done;

        this.setData(function () {
            this.render(callback);
        });
    };

    exports('ReportDataSource', datasource);
});