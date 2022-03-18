var OptionData = {
    AxisType: ['category', 'value', 'time', 'log'],
    ChartsType: [
        {
            label: '直線圖',
            value: 'line'
        },
        {
            label: '柱狀圖',
            value: 'bar'
        }
        //,
        //{
        //    label: '元餅圖',
        //    value: 'pie'
        //},
        //{
        //    label: '散點圖',
        //    value: 'scatter'
        //},
        //{
        //    label: '雷達圖',
        //    value: 'radar'
        //},
        //{
        //    label: '樹形圖',
        //    value: 'tree'
        //},
        //{
        //    label: '樹形圖',
        //    value: 'tree'
        //},
        //{
        //    label: '曲線圖',
        //    value: 'graph'
        //},
        //{
        //    label: '漏斗圖',
        //    value: 'funnel'
        //},
        //{
        //    label: '儀錶圖',
        //    value: 'gauge'
        //}
    ],
    seriesLayoutBy: ['column','row', 'none']
};

layui.define(function (exports) {
    exports('ChartDefaultOption', {
        id: "",
        type: "Echarts",
        DataSource: [],
        width: 900,
        height: 350,
        option: {
            tooltip: {
                trigger: "axis",
                axisPointer: {
                    type: "shadow" 
                }
            },
            title: {
                text: '',
                left: 'center',
                top: 10,
                textStyle: {
                    color: '#000'
                }
            },
            toolbox: {
                feature: {
                    magicType: {
                        type: ["line", "bar", "stack", "tiled"]
                    },
                    dataView: {}
                }
            },
            legend: {
                DataType: "column",
                DataIndex: 0
            },
            xAxis: [],
            yAxis: [],
            grid: {
                top:"60",
                left: "3%",
                right: "4%",
                bottom: "3%",
                containLabel: true
            },
            series:[]
        }
    });
});