layui.extend({
    bootstrap: '../Bootstrap/js/bootstrap',
    bootstrapTable: '../bootstrap-table/bootstrap-table'
}).define(['jquery', 'layer', 'echarts','bootstrap', 'bootstrapTable'], function (exports) {
    var $ = layui.jquery;
    var Echarts = layui.echarts;
    //页面构造函数
    var Report = function (options) {
        this.options = options || { widget: null };
        this.elem = {};
    };

    //渲染
    Report.prototype.render = function (callback) {
        if (this.options.widget === null) return;
        widget = this.options.widget;
        for (var item in widget) {
            switch (widget[item].type) {
                case 'Table':
                    $('[for-elem=' + item + ']').append($('<table id="' + item + '"></table>'));
                    this.elem[item] = $('[for-elem=' + item + '] table').bootstrapTable(widget[item].option);
                    break;
                case 'Echarts':
                    var i = $('<div id="' + item + '"></div>');
                    $('[for-elem=' + item + ']').append(i);
                    this.elem[item] = Echarts.init($('[for-elem=' + item + '] #' + item)[0], layui.EchartsTheme, { width: $('[for-elem=' + item + ']').width(), height: widget[item].height });
                    this.elem[item].clear();
                    this.elem[item].resize();
                    this.elem[item].setOption(widget[item].option);
                    break;
                default:
            }
        }
        //调用渲染完毕的回调函数
        if (typeof callback == 'function')
            callback.call(this);
    };

    //设置数据
    Report.prototype.setData = function (callback) {
        if (this.options.data === null) return;
        var data = this.options.data;
        this.options['widget'] = {};
        this.elem = {};
        for (var i = 0; i < data.length; i++) {
            this.options['widget'][data[i].id] = {
                type: data[i].type,
                option: {}
            };
            if (data[i].width) {
                this.options['widget'][data[i].id]['width'] = data[i].width;
            }
            if (data[i].height) {
                this.options['widget'][data[i].id]['height'] = data[i].height;
            }
            var ds = null;
            for (var n = 0; n < data[i].DataSource.length; n++) {
                if (ds == null) {
                    ds = {};
                }
                if (this.options.DataSet[data[i].DataSource[n]]) {
                    ds[data[i].DataSource[n]] = this.options.DataSet[data[i].DataSource[n]];
                }
            }
            if (ds) {
                switch (data[i].type) {
                    case 'Table':
                        //序列化表頭
                        var col = [];
                        var dt = [];
                        for (var item in ds) {
                            dt = ds[item];
                            break;
                        }
                        if (dt.length > 0) {
                            for (var n = 0; n < dt[0].length; n++) {
                                col.push({
                                    field: dt[0][n],
                                    title: dt[0][n].toUpperCase()
                                });
                            }
                        }
                        this.options['widget'][data[i].id].option['columns'] = col;
                        this.options['widget'][data[i].id].option['data'] = common.ArrayToJson(dt);
                        break;
                    case 'Echarts':
                        this.options['widget'][data[i].id].option = common.EchartsInitial(data[i], ds);
                        break;
                    default:
                }
            }
        }

        //调用渲染完毕的回调函数
        if (typeof callback == 'function')
            callback.call(this);
    };

    var common = {
        EchartsInitial: function (opt, data) {
            var o = {};
            var ds = [];
            if (data) {
                o['dataset'] = {};
                for (var item in data) {
                    if (ds.length == 0) {
                        ds = data[item];
                    } else if (ds.length != 0 && ds[0].length == data[item][0].length) {
                        var dt = data[item];
                        dt.splice(0, 1);
                        ds = ds.concat(dt);
                    }
                }
                o['dataset']['source'] = ds;
            }
            if (opt.option.title) {
                o['title'] = opt.option.title;
            }
            if (opt.option.legend) {
                o['legend'] = { data: [] };
                if (opt.option.legend.orient) {
                    o['legend']['orient'] = opt.option.legend.orient;
                }
                if (opt.option.legend.DataType === 'row') {
                    for (var i = 1; i < ds[opt.option.legend.DataIndex].length; i++) {
                        o['legend'].data.push(ds[opt.option.legend.DataIndex][i]);
                    }
                }
                else if (opt.option.legend.DataType === 'column') {
                    for (var i = 1; i < ds.length; i++) {
                        o['legend'].data.push(ds[i][opt.option.legend.DataIndex]);
                    }
                } else {
                    o['legend'].data = opt.option.legend.data;
                }
            }
            if (opt.option.xAxis) {
                o['xAxis'] = opt.option.xAxis;
            }
            if (opt.option.yAxis) {
                o['yAxis'] = opt.option.yAxis;
            }
            if (opt.option.visualMap) {
                o['visualMap'] = opt.option.visualMap;
            }
            if (opt.option.series) {
                o['series'] = [];
                for (var i = 0; i < opt.option.series.length; i++) {
                    if (opt.option.series[i].type === 'line' || opt.option.series[i].type === 'bar') {
                        if (data[opt.option.series[i].SourceID] && opt.option.series[i].seriesLayoutBy) {
                            var ds = data[opt.option.series[i].SourceID];
                            var serItem = {};
                            if (opt.option.series[i].type) {
                                serItem['type'] = opt.option.series[i].type;
                            }
                            if (opt.option.series[i].seriesLayoutBy) {
                                serItem['seriesLayoutBy'] = opt.option.series[i].seriesLayoutBy;
                            }
                            if (opt.option.series[i].stack) {
                                serItem['stack'] = opt.option.series[i].stack;
                            }
                            if (opt.option.series[i].yAxisIndex) {
                                serItem['yAxisIndex'] = opt.option.series[i].yAxisIndex;
                            }
                            if (opt.option.series[i].seriesLayoutBy) {
                                if (opt.option.series[i].seriesLayoutBy === 'row') {
                                    for (var n = 1; n < ds.length; n++) {
                                        var SItem = $.extend({}, serItem);
                                        SItem['name'] = ds[n][opt.option.series[i].columnIndex];
                                        o['series'].push(SItem);
                                    }
                                } else {
                                    for (var n = 1; n < ds[0].length; n++) {
                                        var SItem = $.extend({}, serItem);
                                        SItem['name'] = ds[0][n];
                                        o['series'].push(SItem);
                                    }
                                }
                            } else if (opt.option.series[i].encode) {
                                serItem['encode'] = opt.option.series[i].encode;
                                o['series'].push(serItem);
                            }
                        }
                        else {
                            o['series'].push(opt.option.series[i]);
                        }
                    }
                    else {
                        var ser = $.extend(true, {}, opt.option.series[i]);
                        delete ser['SourceID'];
                        delete ser['columnIndex'];
                        o['series'].push(ser);
                    }
                }
            }
            return o;
        },
        ArrayToJson: function (ArrData) {
            var res = [];
            if (ArrData.length > 0) {
                var head = ArrData[0];
                for (var i = 1; i < ArrData.length; i++) {
                    var row = {};
                    for (var n = 0; n < ArrData[i].length; n++) {
                        row[head[n]] = ArrData[i][n];
                    }
                    res.push(row);
                }
            }
            return res;
        }
    }

    var report = new Report();

    //配置
    report.config = function (options) {
        options = options || {};
        for (var key in options) {
            this.options[key] = options[key];
        }
        return this;
    };

    //初始化
    report.init = function (options, callback) {
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

    exports('ReportBase', report);
});