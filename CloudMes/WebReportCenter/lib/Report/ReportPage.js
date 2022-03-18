layui.extend({
    echarts: '../lib/echarts/echarts',
    ReportDataSource: '../Report/ReportDataSource',
    ReportBase: '../Report/ReportBase',
    IWebSocket: '../IWebSocket'
}).define(['jquery', 'layer', 'echarts', 'ReportDataSource', 'ReportBase', 'IWebSocket'], function (exports) {
    "use strict";
    var Echarts = layui.echarts;
    var Report = layui.ReportBase;
    var DataSource = layui.ReportDataSource;
    var layer = layui.layer;
    var $ = layui.jquery;

    //页面构造函数
    var Page = function (options) {
        this.options = options || {
            InitClass: "MESStation.ReportCenter.Report",
            InitFunction: "GetConfig",
            Container: $('#ReportContainer')
        };
        this.data = null;
        this.ProFile = null;
    };

    //渲染
    Page.prototype.render = function (callback) {
        if (this.data === null) return;
        var ConfigData = this.data;
        //添加佈局
        var html = ConfigData.Layout;
        this.options.Container.html(html);

        //初始化數據源
        DataSource.init({
            data: ConfigData.DataSource
        }, function (e) {
            //初始化顯示組件
            Report.init({
                data: ConfigData.Widgets,
                DataSet: DataSource.DataSet
            }, function (e) {
                window.onresize = function () {
                    for (var i in Report.elem) {
                        if (Report.elem[i]) {
                            Report.elem[i].resize({ width: Report.elem[i]._dom.offsetWidth, height: Report.elem[i]._dom.offsetHeight });
                        }
                    }
                }
            });
        });

    };

    //设置数据
    Page.prototype.setData = function (callback) {
        var obj = this
            , currOptions = obj.options;
        if (!currOptions.InitClass || !currOptions.InitFunction)
            return
        layui.IWebSocket.Call(currOptions.InitClass, currOptions.InitFunction, { ID: this.options.ReportID }, function (e) {
            if (e.Status == "Pass") {
                obj.data = JSON.parse(e.Data.DATA);
                if (typeof callback == 'function') {
                    callback.call(obj);
                }
            } else {
                obj.data = null;
                layer.msg('初始化報表數據失敗:' + e.Message, {
                    offset: '40px',
                    zIndex: layer.zIndex
                });
            }
        });        
    };

    var page = new Page();

    //配置
    page.config = function (options) {
        options = options || {};
        for (var key in options) {
            this.options[key] = options[key];
        }
        return this;
    };

    //初始化
    page.init = function (options, callback) {
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

    exports('ReportPage', page);
});