(function (root, factory) {
    if (typeof define === 'function' && define.amd) {
        // AMD. Register as an anonymous module.
        define(['exports', 'echarts'], factory);
    } else if (typeof exports === 'object' && typeof exports.nodeName !== 'string') {
        // CommonJS
        factory(exports, require('echarts'));
    } else {
        // Browser globals
        factory({}, root.echarts);
    }
}(this, function (exports, echarts) {
    var log = function (msg) {
        if (typeof console !== 'undefined') {
            console && console.error && console.error(msg);
        }
    };
    if (!echarts) {
        log('ECharts is not Loaded');
        return;
    }
    var contrastColor = '#eee';
    var axisCommon = function () {
        return {            
            axisLine: {
                lineStyle: {
                    color: contrastColor
                }
            },
            axisTick: {
                lineStyle: {
                    color: contrastColor
                }
            },
            axisLabel: {
                textStyle: {
                    color: contrastColor
                }
            },
            splitLine: {
                lineStyle: {
                    type: 'dashed',
                    color: '#aaa'
                }
            },
            splitArea: {
                areaStyle: {
                    color: contrastColor
                }
            }
        };
    };

    var colorPalette = [
      '#009688', '#1E9FFF', '#5FB878', '#FFB980', '#D87A80',
      '#8d98b3', '#e5cf0d', '#97b552', '#95706d', '#dc69aa',
      '#07a2a4', '#9a7fd1', '#588dd5', '#f5994e', '#c05050',
      '#59678c', '#c9ab00', '#7eb00a', '#6f5553', '#c14089'
    ];
    var theme = {
        // 默认色板
        color: colorPalette,

        // 图表标题
        title: {
            textStyle: {
                fontWeight: 'normal',
                color: contrastColor      // 主标题文字颜色
            }
        },
        legend: {
            textStyle: {
                color: contrastColor
            }
        },
        // 值域
        dataRange: {
            itemWidth: 15,
            color: ['#009688', '#e0ffff']
        },

        // 工具箱
        toolbox: {
            color: ['#1e90ff', '#1e90ff', '#1e90ff', '#1e90ff'],
            effectiveColor: '#ff4500'
        },

        // 提示框
        tooltip: {
            backgroundColor: 'rgba(50,50,50,0.5)',   // 提示背景颜色，默认为透明度为0.7的黑色
            axisPointer: {      // 坐标轴指示器，坐标轴触发有效
                type: 'line',     // 默认为直线，可选为：'line' | 'shadow'
                lineStyle: {// 直线指示器样式设置
                    color: contrastColor
                },
                crossStyle: {
                    color: contrastColor
                },
                shadowStyle: {           // 阴影指示器样式设置
                    color: 'rgba(200,200,200,0.2)'
                }
            }
        },

        // 区域缩放控制器
        dataZoom: {
            dataBackgroundColor: '#efefff',      // 数据背景颜色
            fillerColor: 'rgba(182,162,222,0.2)',   // 填充颜色
            handleColor: '#008acd',  // 手柄颜色
            textStyle: {
                color: contrastColor
            }
        },

        // 网格
        grid: {
            borderColor: '#eee'
        },

        // 类目轴 - X轴
        categoryAxis: axisCommon(),

        // 数值型坐标轴默认参数 - Y轴
        valueAxis: axisCommon(),

        polar: {
            axisLine: {      // 坐标轴线
                lineStyle: {     // 属性lineStyle控制线条样式
                    color: '#ddd'
                }
            },
            splitArea: {
                show: true,
                areaStyle: {
                    color: ['rgba(250,250,250,0.2)', 'rgba(200,200,200,0.2)']
                }
            },
            splitLine: {
                lineStyle: {
                    color: '#ddd'
                }
            }
        },

        timeline: {
            lineStyle: {
                color: contrastColor
            },
            itemStyle: {
                normal: {
                    color: colorPalette[1]
                }
            },
            label: {
                normal: {
                    textStyle: {
                        color: contrastColor
                    }
                }
            },
            controlStyle: {
                normal: {
                    color: contrastColor,
                    borderColor: contrastColor
                }
            }
        },

        // 柱形图默认参数
        bar: {
            itemStyle: {
                normal: {
                    barBorderRadius: 2
                },
                emphasis: {
                    barBorderRadius: 2
                }
            }
        },

        // 折线图默认参数
        line: {
            smooth: true,
            symbol: 'emptyCircle',  // 拐点图形类型
            symbolSize: 3       // 拐点图形大小
        },

        // K线图默认参数
        k: {
            itemStyle: {
                normal: {
                    color: '#d87a80',     // 阳线填充颜色
                    color0: '#2ec7c9',    // 阴线填充颜色
                    lineStyle: {
                        color: '#d87a80',   // 阳线边框颜色
                        color0: '#2ec7c9'   // 阴线边框颜色
                    }
                }
            }
        },

        // 散点图默认参数
        scatter: {
            symbol: 'circle',  // 图形类型
            symbolSize: 4    // 图形大小，半宽（半径）参数，当图形为方向或菱形则总宽度为symbolSize * 2
        },

        // 雷达图默认参数
        radar: {
            symbol: 'emptyCircle',  // 图形类型
            symbolSize: 3
            //symbol: null,     // 拐点图形类型
            //symbolRotate : null,  // 图形旋转控制
        },

        map: {
            itemStyle: {
                normal: {
                    areaStyle: {
                        color: '#ddd'
                    },
                    label: {
                        textStyle: {
                            color: '#d87a80'
                        }
                    }
                },
                emphasis: {         // 也是选中样式
                    areaStyle: {
                        color: '#fe994e'
                    }
                }
            }
        },

        force: {
            itemStyle: {
                normal: {
                    linkStyle: {
                        color: '#1e90ff'
                    }
                }
            }
        },

        chord: {
            itemStyle: {
                normal: {
                    borderWidth: 1,
                    borderColor: 'rgba(128, 128, 128, 0.5)',
                    chordStyle: {
                        lineStyle: {
                            color: 'rgba(128, 128, 128, 0.5)'
                        }
                    }
                },
                emphasis: {
                    borderWidth: 1,
                    borderColor: 'rgba(128, 128, 128, 0.5)',
                    chordStyle: {
                        lineStyle: {
                            color: 'rgba(128, 128, 128, 0.5)'
                        }
                    }
                }
            }
        },

        gauge: {
            title: {
                show:true,
                color: colorPalette
            },
            axisLine: {      // 坐标轴线
                lineStyle: {     // 属性lineStyle控制线条样式
                    color: [[0.2, '#2ec7c9'], [0.8, '#5ab1ef'], [1, '#d87a80']],
                    width: 10
                }
            },
            axisLabel: {
                fontSize: 10,
                color: colorPalette,
                formatter: '{value}%'
            },
            axisTick: {      // 坐标轴小标记
                splitNumber: 10,   // 每份split细分多少段
                length: 10,    // 属性length控制线长
                lineStyle: {     // 属性lineStyle控制线条样式
                    color: 'auto'
                }
            },
            splitLine: {       // 分隔线
                length: 5,     // 属性length控制线长
                lineStyle: {     // 属性lineStyle（详见lineStyle）控制线条样式
                    color: colorPalette
                }
            },
            pointer: {
                length: '60%',
                width: 4
            },
            detail: {
                fontSize: 15,
                color: colorPalette,
                offsetCenter: ['0', '70%']
            }
        },

        textStyle: {
            fontFamily: '微软雅黑, Arial, Verdana, sans-serif'
        }
    };
    theme.categoryAxis.splitLine.show = false;
    echarts.registerTheme('dark', theme);
}));
