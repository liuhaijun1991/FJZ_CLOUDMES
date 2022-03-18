layui.config({
    base: '../../lib/'
}).extend({
    Config: 'Config',
    common: 'common',
    IWebSocket: 'IWebSocket',
    bootstrapTable: 'bootstrap-table/bootstrap-table',
    jQueryUI: 'Plugins/jQueryUI',
    TouchPunch: 'Plugins/TouchPunch',
    bootstrap: 'Bootstrap/js/bootstrap',
    htmlClean: 'Plugins/htmlClean',
    echarts: 'echarts/echarts'
}).define(['common', 'IWebSocket', 'jquery', 'form', 'layer', 'jQueryUI', 'TouchPunch', 'bootstrap', 'bootstrapTable', 'htmlClean', 'echarts', 'ChartDefaultOption'], function (exports) {
    var $ = layui.$;
    var currentDocument = null;
    var timerSave = 2e3;
    var demoHtml = $(".demo").html();
    var charts = {};
    var BackUp = {};
    ReportOptions = {
        Layout: "",
        DataSource: [],
        Widgets: [],
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

    layui.IWebSocket.init({
        url: 'ws://' + layui.Config.SOCKET_SERVERIP + ':' + layui.Config.SOCKET_SERVERPORT + '/' + layui.Config.SOCKET_SERVICE
    }, function (e) {
        layui.IWebSocket.Login('TEST', 'TEST', function (d) {

        });
    });

    function ResetChart(id) {
        var wg = ReportOptions.Widgets.GetChild('id', id);
        var option = SetCharts(wg);
        charts[id].charts.clear();
        charts[id].charts.setOption(option);
        charts[id].charts.resize({ width: $('.demo [for-elem=' + id + ']').width(), height: wg.height });
        charts[id].option = option;
    };
    function SetCharts(wg) {
        var option = wg.option;
        var o = {};
        var ds = [];
        var Datasource = {};
        for (var n = 0; n < wg.DataSource.length; n++) {
            for (var i = 0; i < ReportOptions.DataSource.length; i++) {
                if (wg.DataSource[n] == ReportOptions.DataSource[i].id) {
                    switch (ReportOptions.DataSource[i].type) {
                        case 'HTTP':
                            Datasource[ReportOptions.DataSource[i].id] = common.RunAJAX(ReportOptions.DataSource[i].data, {});
                            break;
                        case 'JSON':
                            Datasource[ReportOptions.DataSource[i].id] = common.ParseJSON(ReportOptions.DataSource[i].data);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        for (var item in Datasource) {
            if (ds.length == 0) {
                ds = Datasource[item];
            } else if (ds.length != 0 && ds[0].length == Datasource[item][0].length) {
                var dt = Datasource[item];
                dt.splice(0, 1);
                ds = ds.concat(dt);
            }
        }
        if (!o.dataset)
            o['dataset'] = {};
        if (!o.dataset.source)
            o.dataset['source'] = ds;

        if (option.title)
            o['title'] = option.title;
        if (option.legend) {
            o['legend'] = { data: [] };
            if (option.legend.orient)
                o['legend']['orient'] = option.legend.orient;
            if (option.legend.DataType === 'row') {
                for (var i = 1; i < ds[option.legend.DataIndex].length; i++) {
                    o['legend'].data.push(ds[option.legend.DataIndex][i]);
                }
            }
            else if (option.legend.DataType === 'column') {
                for (var i = 1; i < ds.length; i++) {
                    o['legend'].data.push(ds[i][option.legend.DataIndex]);
                }
            } else {
                o['legend'].data = option.legend.data;
            }
        }
        if (option.xAxis)
            o['xAxis'] = option.xAxis;
        if (option.yAxis)
            o['yAxis'] = option.yAxis;
        if (option.visualMap)
            o['visualMap'] = option.visualMap;
        if (option.series) {
            o['series'] = [];
            for (var i = 0; i < option.series.length; i++) {
                if (option.series[i].type === 'line' || option.series[i].type === 'bar') {
                    if (Datasource[option.series[i].SourceID] && option.series[i].seriesLayoutBy) {
                        var dss = Datasource[option.series[i].SourceID];
                        var serItem = {};
                        if (option.series[i].type)
                            serItem['type'] = option.series[i].type;
                        if (option.series[i].seriesLayoutBy)
                            serItem['seriesLayoutBy'] = option.series[i].seriesLayoutBy;
                        if (option.series[i].stack)
                            serItem['stack'] = option.series[i].stack;
                        if (option.series[i].yAxisIndex)
                            serItem['yAxisIndex'] = option.series[i].yAxisIndex;
                        if (option.series[i].seriesLayoutBy) {
                            if (option.series[i].seriesLayoutBy === 'row') {
                                for (var n = 1; n < dss.length; n++) {
                                    var SItem = $.extend({}, serItem);
                                    SItem['name'] = dss[n][option.series[i].columnIndex];
                                    o['series'].push(SItem);
                                }
                            } else {
                                for (var n = 1; n < dss[0].length; n++) {
                                    var SItem = $.extend({}, serItem);
                                    SItem['name'] = dss[0][n];
                                    o['series'].push(SItem);
                                }
                            }
                        } else if (option.series[i].encode) {
                            serItem['encode'] = option.series[i].encode;
                            o['series'].push(serItem);
                        }
                    }
                    else {
                        o['series'].push(option.series[i]);
                    }
                }
                else {
                    var ser = $.extend(true, {}, option.series[i]);
                    delete ser['SourceID'];
                    delete ser['columnIndex'];
                    o['series'].push(ser);
                }
            }
        }

        return o;
    }
    function ChartsRezise() {
        for (var item in charts) {
            charts[item].charts.resize({ width: charts[item].charts._dom.parentElement.offsetWidth, height: (charts[item].charts._dom.parentElement.offsetHeight - 30) });
        }
    };


    function OpenWindow(option) {
        var options = {
            type: 1,
            area: ['70vw', '80vh'],
            offset: ['10vh', '15vw'],
            content: ''
        };
        for (var o in option) {
            options[o] = option[o];
        }
        if (options.type === 1) {
            $.ajax({
                type: 'get',
                url: options.url,
                async: false,
                success: function (data) {
                    options.content = data;
                },
                error: function (e) {
                    var page = '';
                    switch (e.status) {
                        case 404:
                            page = '404.html';
                            break;
                        case 500:
                            page = '500.html';
                            break;
                        default:
                            options.content = "打开窗口失败";
                    }
                    $.ajax({
                        type: 'get',
                        url: '../error/' + page,
                        async: false,
                        success: function (data) {
                            options.content = data;
                        },
                        error: function () {
                            layer.close(load);
                        }
                    });
                }
            });
        }
        else {
            options.content = options.url;
        }
        delete options['url'];
        layui.layer.open(options);
    }
    function handleSaveLayout() {
        var e = $(".demo").html();
        if (e !== window.demoHtml) {
            //saveLayout();
            window.demoHtml = e
        }
    }

    function randomNumber() {
        return randomFromInterval(1, 1e6)
    }
    function randomFromInterval(e, t) {
        return Math.floor(Math.random() * (t - e + 1) + e)
    }
    function gridSystemGenerator() {
        $(".lyrow .preview input").bind("keyup",
            function () {
                var e = 0;
                var t = "";
                var n = false;
                var r = $(this).val().split(" ", 12);
                $.each(r,
                    function (r, i) {
                        if (!n) {
                            if (parseInt(i) <= 0) n = true;
                            e = e + parseInt(i);
                            t += '<div class="col-md-' + i + ' column"></div>'
                        }
                    });
                if (e === 12 && !n) {
                    $(this).parent().next().children().html(t);
                    $(this).parent().prev().show()
                } else {
                    $(this).parent().prev().hide()
                }
            })
    }
    function configurationElm(e, t) {
        $(".demo").delegate(".configuration > a", "click",
            function (e) {
                e.preventDefault();
                var t = $(this).parent().next().next().children();
                $(this).toggleClass("active");
                t.toggleClass($(this).attr("rel"))
            });
        $(".demo").delegate(".configuration .dropdown-menu a", "click",
            function (e) {
                e.preventDefault();
                var t = $(this).parent().parent();
                var n = t.parent().parent().next().children();
                t.find("li").removeClass("active");
                $(this).parent().addClass("active");
                var r = "";
                t.find("a").each(function () {
                    r += $(this).attr("rel") + " "
                });
                t.parent().removeClass("open");
                n.removeClass(r);
                n.addClass($(this).attr("rel"))
            })
    }
    function OpenConfigWindow() {
        $(".dsource").delegate(".config", "click",
            function (e) {
                var elid = $(this).parent().attr('for-elem');
                OpenWindow({
                    type: 2,
                    title: 'DataSourceConfig',
                    url: 'template/DataBaseConfig.html?elid=' + elid + '&type=Data',
                    btn: ['Save', 'Cancel'],
                    yes: function (index, layero) {
                        var body = layui.layer.getChildFrame('body', index);
                        var id = body.find('[lay-filter=ConfigObjectID]').val();
                        var name = body.find('[lay-filter=sourceName]').val();
                        var data = body.find('[lay-filter=sourceData]').val();
                        var type = body.find('[lay-filter=sourceType]').val();
                        var item = ReportOptions.DataSource.GetChild('id', id);
                        if (item.name != name) {
                            if (ReportOptions.DataSource.GetChild('name', name)) {
                                layer.msg('数据源重名', { icon: 2 });
                                return false;
                            }
                        }
                        if (item) {
                            $('#' + id).find('.sourcename').text(name);
                            $('#' + id).attr('sourceName', name);
                            $('#' + id).attr('sourceData', data);
                            $('#' + id).attr('sourceType', type);
                            item.name = name;
                            item.type = type;
                            item.data = data;
                        }
                        else {
                            var dt = {
                                id: id,
                                name: name,
                                type: type,
                                data: url
                            }
                            ReportOptions.DataSource.push(dt);
                        }
                        layui.layer.close(index);
                    },
                    btn2: function (index, layero) {
                        //return false 开启该代码可禁止点击该按钮关闭
                    }
                });
            });
        $(".demo").delegate(".config", "click",
            function (e) {
                var elid = $(this).parent().attr('for-elem');
                var type = $(this).parent().attr('rel');
                var url = 'template/';
                var title = '';
                if (type === 'Table') {
                    title = 'TableConfig';
                    url += 'TableConfig.html?elid' + elid + '&type=' + type;
                }
                else if (type === 'Echarts') {
                    title = 'Echart Config';
                    url += 'EchartConfig.html?elid=' + elid + '&type=' + type;
                }
                OpenWindow({
                    type: 2,
                    title: title,
                    url: url,
                    maxmin: true,
                    closeBtn: 0,
                    btn: ['Save', 'Cancel'],
                    success: function (layero, index) {
                        BackUp = $.extend(true, {}, ReportOptions);
                    },
                    yes: function (index, layero) {
                        var body = layui.layer.getChildFrame('body', index);
                        var id = body.find('[lay-filter=ConfigObjectID]').val();
                        var type = body.find('[lay-filter=ConfigObjectType]').val();
                        switch (type) {
                            case 'Echarts':
                                ResetChart(id);
                                break;
                            case 'Table':
                                break;
                            default:
                        }
                        layui.layer.close(index);
                    },
                    btn2: function (index, layero) {
                        ReportOptions = $.extend(true, {}, BackUp);
                    },
                    end: function () {
                        localStorage.removeItem(elid)
                    }
                });
            });
    }
    function removeElm() {
        $(".demo,.dsource").delegate(".remove", "click",
            function (e) {
                e.preventDefault();

                $(this).parent().remove();
                if (!$(".dsource .data").length > 0) {
                    clearSource()
                }
                if (!$(".demo .column").length > 0) {
                    clearDemo()
                }
            })
    }
    function clearDemo() {
        $(".demo").empty()
    }
    function clearSource() {
        $(".dsource").empty()
    }
    function removeMenuClasses() {
        $("#menu-layoutit li button").removeClass("active")
    }
    function changeStructure(e, t) {
        $("#download-layout ." + e).removeClass(e).addClass(t)
    }
    function cleanHtml(e) {
        $(e).parent().append($(e).children().html())
    }
    function downloadLayoutSrc() {
        var e = "";
        $("#download-layout").children().html($(".demo").html());
        var t = $("#download-layout").children();
        t.find(".preview, .configuration, .drag, .remove").remove();
        t.find(".lyrow").addClass("removeClean");
        t.find(".box-element").addClass("removeClean");
        t.find(".lyrow .lyrow .lyrow .lyrow .lyrow .removeClean").each(function () {
            cleanHtml(this)
        });
        t.find(".lyrow .lyrow .lyrow .lyrow .removeClean").each(function () {
            cleanHtml(this)
        });
        t.find(".lyrow .lyrow .lyrow .removeClean").each(function () {
            cleanHtml(this)
        });
        t.find(".lyrow .lyrow .removeClean").each(function () {
            cleanHtml(this)
        });
        t.find(".lyrow .removeClean").each(function () {
            cleanHtml(this)
        });
        t.find(".removeClean").each(function () {
            cleanHtml(this)
        });
        t.find(".removeClean").remove();
        $("#download-layout .column").removeClass("ui-sortable");
        $("#download-layout .row-fluid").removeClass("clearfix").children().removeClass("column");
        if ($("#download-layout .container").length > 0) {
            changeStructure("row-fluid", "row")
        }
        formatSrc = $.htmlClean($("#download-layout").html(), {
            format: true,
            allowedAttributes: [["id"], ["class"], ["data-toggle"], ["data-target"], ["data-parent"], ["role"], ["data-dismiss"], ["aria-labelledby"], ["aria-hidden"], ["data-slide-to"], ["data-slide"]]
        });
        $("#download-layout").html(formatSrc);
        //$("#downloadModal textarea").empty();
        //$("#downloadModal textarea").val(formatSrc)
    }

    function ClearElem(e) {
        if (e.hasClass('view') || e.hasClass('lyrow')) {
            var c = e.children();
            e.parent().append(c);
            if (c.has('.view') || c.has('.lyrow')) {
                c.each(function () {
                    ClearElem($(this));
                });
            }
            e.remove();
        } else if (e.has('.view').length > 0 || e.has('.lyrow').length > 0) {
            e.children().each(function () {
                ClearElem($(this));
            });
        }
    }

    $(window).resize(function () {
        $("body").css("min-height", $(window).height() - 90);
        $(".demo").css("min-height", $(window).height() - 160)
    });
    $(document).ready(function () {
        $("body").css("min-height", $(window).height() - 90);
        $(".demo").css("min-height", $(window).height() - 220);
        $(".demo, .demo .column").sortable({
            connectWith: ".column",
            opacity: .35,
            handle: ".drag"
        });
        $(".dsource").sortable({
            connectWith: ".data",
            opacity: .35,
            handle: ".drag",
            deactivate: function (e, t) {
                var id = 'data' + randomNumber();
                t.item.attr('id', id);
                t.item.attr('for-elem', id);
                OpenWindow({
                    type: 2,
                    title: 'DataSource Config',
                    url: 'template/DataBaseConfig.html?elid=' + id + '&type=Data',
                    closeBtn: 0,
                    btn: ['Save', 'Cancel'],
                    yes: function (index, layero) {
                        var body = layui.layer.getChildFrame('body', index);
                        var id = body.find('[lay-filter=ConfigObjectID]').val();
                        var name = body.find('[lay-filter=sourceName]').val();
                        var type = body.find('[lay-filter=sourceType]').val();
                        var url = body.find('[lay-filter=sourceData]').val();
                        if (ReportOptions.DataSource.GetChild('name', name)) {
                            layer.msg('数据源重名', { icon: 2 });
                            return false;
                        }
                        var dt = {
                            id: id,
                            name: name,
                            type: type,
                            data: url
                        }
                        ReportOptions.DataSource.push(dt);

                        t.item.append($('<div class="sourcename">' + name + '</div>'));
                        t.item.attr('sourceName', name);
                        t.item.attr('sourceData', url);
                        t.item.attr('sourceType', type);

                        layui.layer.close(index);
                    },
                    btn2: function (index, layero) {
                        t.item.remove();
                        //return false
                    }
                });
            }
        });
        $(".sidebar-nav .lyrow").draggable({
            connectToSortable: ".demo",
            helper: "clone",
            handle: ".drag",
            drag: function (e, t) {
                t.helper.width(400)
            },
            stop: function (e, t) {
                $(".demo .column").sortable({
                    opacity: .35,
                    connectWith: ".column"
                })
            }
        });
        $(".sidebar-nav .box").draggable({
            connectToSortable: ".column",
            helper: "clone",
            handle: ".drag",
            start: function (e, t) {
                var id = '';
                if (t.helper.context.outerHTML.indexOf('echartDemo') > 0) {
                    id = 'Charts' + randomNumber();
                    t.helper.id = id
                    t.helper.context.children[4].children[0].setAttribute('id', id);
                } else {
                    id = 'Table' + randomNumber();
                    t.helper.id = id
                    t.helper.context.children[4].children[0].setAttribute('id', id);
                }
                t.helper.context.setAttribute('for-elem', id);
            },
            drag: function (e, t) {
                t.helper.width(400)
            },
            stop: function (e, t) {
                var id = t.helper.id + "";
                $('.sidebar-nav #' + id).attr('id', '');
                if (id.startsWith('Charts')) {
                    var selector = '.demo #' + id;
                    var container = $(selector);
                    if (container.length > 0) {
                        var width = container.parent().width();
                        var wg = $.extend(true, {}, layui.ChartDefaultOption);
                        wg.id = id;
                        var option = SetCharts(wg);
                        charts[id] = {};
                        charts[id]['charts'] = layui.echarts.init(container[0], layui.EchartsTheme, { width: width, height: 400 });
                        charts[id]['option'] = option;
                        charts[id].charts.clear();
                        charts[id].charts.resize();
                        charts[id].charts.setOption(option);
                        ReportOptions.Widgets.push(wg);
                    }
                }
            }
        });
        $(".sidebar-nav .data").draggable({
            connectToSortable: ".dsource",
            helper: "clone",
            handle: ".drag",
            drag: function (e, t) {
                t.helper.width(100)
            },
            stop: function (e, t) {
                //handleJsIds()                
            }
        });
        $("#button-download").click(function (e) {
            //e.preventDefault();
            //downloadLayoutSrc();
            OpenWindow({
                title: '查看/编辑源代码',
                rul: 'template/download.html'
            });
        });
        $('#button-save').click(function (e) {
            var demo = $('#HideDemo');
            demo.html($('.demo').html());
            demo.find('[for-elem]').empty();
            demo.find('.remove').remove();
            demo.find('.drag').remove();
            demo.find('.preview').remove();
            demo.find('.config').remove();
            demo.children().each(function () {
                ClearElem($(this));
            });
            var html = demo.html();

            ReportOptions.Layout = html;
            layui.layer.prompt({
                formType: 0,
                value: '',
                title: '請輸入報表名稱',
                area: ['300px', '300px']
            }, function (value, index, elem) {
                layui.IWebSocket.Call('MesReportCenter.ReportCenterBaseAPI', 'AddData', {
                    ID: '',
                    CONFIGTYPE: 'REPORT',
                    KEY: value,
                    DATATYPE: 'json',
                    PARENTKEY: '',
                    DATA: ReportOptions
                }, function (e) {
                    if (e.Status == 'Pass') {
                        layui.layer.msg('', { icon: 1 });
                    } else {
                        layui.layer.msg(e.Message, { icon: 2 });
                    }
                });
                layui.layer.close(index);
            });
        });
        $("#download").click(function () {
            //downloadLayout();
            //return false
        });
        $("#downloadhtml").click(function () {
            //downloadHtmlLayout();
            //return false
        });
        $("#edit").click(function () {
            $("body").removeClass("devpreview sourcepreview");
            $("body").addClass("edit");
            removeMenuClasses();
            $(this).addClass("active");
            return false
        });
        $("#clear").click(function (e) {
            e.preventDefault();
            clearDemo()
        });
        $("#devpreview").click(function () {
            $("body").removeClass("edit sourcepreview");
            $("body").addClass("devpreview");
            removeMenuClasses();
            $(this).addClass("active");
            return false
        });
        $("#sourcepreview").click(function () {
            $("body").removeClass("edit");
            $("body").addClass("devpreview sourcepreview");
            removeMenuClasses();
            $(this).addClass("active");
            return false
        });
        $(".nav-header").click(function () {
            $(".sidebar-nav .boxes, .sidebar-nav .rows").hide();
            $(this).next().slideDown()
        });
        window.onresize = ChartsRezise;
        removeElm();
        OpenConfigWindow();
        configurationElm();
        gridSystemGenerator();
        setInterval(function () {
            handleSaveLayout()
        },
            timerSave)
    });

    exports('index', {});
}).addcss('../../../view/Admin/css/layoutit.css', 'layoutit');
