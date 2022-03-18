var exampleEndpoint;
var connectorPaintStyle;     //连接线样式
var connectorHoverStyle;    // 鼠标悬浮在连接线上的样式
var ImportFlage = false;
var MaxSEQ_ID;
var client;
var client1;
var ClassName = "MESStation.RouteConfig";
var FunctionName = "";
var NewConnect = "";
var PageSizs = "10";
var NextId = "";
var LastId = "";
var PageMax = 0;
var mesUI = new MesClientUI(self.parent.client);
//添加工站樣式
function addNode(parentId, nodeId, nodeLable, position, StationType, IsRepair) {
    var RepairClass = "";
    if (IsRepair) {
        RepairClass = "Repair-class";
    }
    var panel = d3.select("#" + parentId);
    panel.append('div').style('top', position.y).style('left', position.x)

        .attr('id', nodeId).classed('item text-center ' + RepairClass, true)
        //.text(nodeLable);
        .append('h5').text(nodeLable);

    panel = d3.select("#" + nodeId);
    panel.append('a').style('display', 'none').text(StationType);
    return jsPlumb.getSelector('#' + nodeId)[0];
}

//載入所有幾種
function loadAllSKU() {
    var SKUData = [];
    ClassName = "MESStation.Config.SkuConfig";
    FunctionName = "GetAllSku";
    self.parent.client.CallFunction(ClassName, FunctionName, {}, function (e) {
        if (e.Status == "Pass") {
            for (var i in e.Data) {
                if (e.Data.hasOwnProperty(i)) {
                    //$("#DEFAULT_SKUNO").append("<option value='" + e.Data[i].SKUNO + "'>" + e.Data[i].SKUNO + "</option>");
                    SKUData.push({ SKU: e.Data[i].SKUNO });
                }
            }

            SelectSuggest("DEFAULT_SKUNO", SKUData);

        } else {
            swal(e.Message, "", "warning");
        }

    })
}

//設置搜索自動補全
function SelectSuggest(SelectID, DataList) {
    $("#" + SelectID).bsSuggest({
        indexId: 0,  //data.value 的第几个数据，作为input输入框的内容
        indexKey: 0, //data.value 的第几个数据，作为input输入框的内容
        ignorecase: true,
        autoDropup: true,
        data: {
            "value": DataList
        }
    }).on('onDataRequestSuccess', function (e, result) {
        console.log('从 json参数中获取，不会触发 onDataRequestSuccess 事件', result);
    }).on('onSetSelectValue', function (e, keyword, data) {
        console.log('onSetSelectValue: ', keyword, data);
    }).on('onUnsetSelectValue', function () {
        console.log("onUnsetSelectValue");
    });
}

//工站類型自動補全，暫時不用
function StationType() {
    var StationTypeData = [
        {
            StationType: "JOBSTARTED"
        },
        {
            StationType: "NORMAL"
        },
        {
            StationType: "JOBFINISHED"
        }
    ]
    $("#StationType").bsSuggest({
        indexId: 0,  //data.value 的第几个数据，作为input输入框的内容
        indexKey: 0, //data.value 的第几个数据，作为input输入框的内容
        data: {
            "value": StationTypeData
        }
    }).on('onDataRequestSuccess', function (e, result) {
        console.log('从 json参数中获取，不会触发 onDataRequestSuccess 事件', result);
    }).on('onSetSelectValue', function (e, keyword, data) {
        console.log('onSetSelectValue: ', keyword, data);
    }).on('onUnsetSelectValue', function () {
        console.log("onUnsetSelectValue");
    });
}
var SearchName = "";
function SearchRoute() {
    SearchName = $('#S_RouteName').val();
    loadTable(SearchName, "1", true);
    // PageList(SearchName, "1")
}

//獲取最後工站ID
function GetLastID() {
    var IsSkip = false;
    var FirstId = "";
    var LastId = "";
    var IsOneFirstId = false;
    var FirstIdNum = 0;
    var IdListIndex = 0;
    var IsRepair = false;
    var RepairIdArray = new Array();
    var SourceIdList = new Array();
    var TargetIdList = new Array();
    var list = jsPlumb.getAllConnections();//获取所有的链接  
    for (var i in list) {
        for (var j = 0; j < list[i].length; j++) {
            for (x in RepairIdArray) {
                if (RepairIdArray[x] == list[i][j]['targetId'] || RepairIdArray[x] == list[i][j]['sourceId']) {
                    IsRepair = true;
                }
            }
            if (list[i][j].endpoints[1].anchor.type == "TopCenter" || list[i][j].endpoints[0].anchor.type == "TopCenter") {
                IsSkip = true;
            }
            if (IsRepair == false && IsSkip == false) {
                SourceIdList[IdListIndex] = list[i][j]['sourceId'];
                if (list[i][j]['targetId'] != "") {
                    TargetIdList[IdListIndex] = list[i][j]['targetId'];
                }
                IdListIndex++;
            }
            IsRepair = false;
            IsSkip = false;
        }
    }

    IsOneFirstId = false;
    FirstIdNum = 0;
    for (var j in TargetIdList) {
        if (TargetIdList.hasOwnProperty(j)) {
            for (var i in SourceIdList) {
                if (SourceIdList.hasOwnProperty(i)) {
                    if (TargetIdList[j] == SourceIdList[i]) {
                        IsOneFirstId = true;
                    }
                }
            }
            if (IsOneFirstId == false) {
                if (FirstIdNum == 0) {
                    //swal("路由配置错误，请确认！", "", "warning");
                    //return;
                    FirstIdNum++;
                    LastId = TargetIdList[j];
                    IsOneFirstId = false;
                }

            }
            IsOneFirstId = false;
        }
    }
    return LastId;
}

//加載表數據
function loadTable(RouteName, PageNumber, isPosBack) {
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "GetRouteMainMessage"
    if ($("#page-size").text() != "") {
        PageSizs = $("#page-size").text();
    }

    self.parent.client.CallFunction(ClassName, FunctionName, { RouteName: RouteName, PageNumber: PageNumber, PageSize: PageSizs }, function (e) {
        if (e.Status == "Pass") {

            if (isPosBack) {
                //$('#Table').bootstrapTable('load', e.Data.MainData);
                $('#Table').bootstrapTable('load', e.Data);
            }
            var c = [];

            c.push({ field: 'ROUTE_NAME', title: '<label set-lan="html:RouteName">ROUTE_NAME</label>' }, { field: 'STATION', title: '<label set-lan="html:DefaultSTATION">STATION</label>' }, { field: 'DEFAULT_SKUNO', title: '<label set-lan="html:DefaultSku">DEFAULT_SKUNO</label>' }, { field: 'ROUTE_TYPE', title: '<label set-lan="html:RouteType">ROUTE_TYPE</label>' }, { field: 'EDIT_TIME', title: '<label set-lan="html:ModifyDate">EDIT_TIME</label>' }, { field: 'EDIT_EMP', title: '<label set-lan="html:ModifyMan">EDIT_EMP</label>' }, { field: 'SKUCOUNT', title: '<label set-lan="html:SKUCOUNT">SKUCOUNT</label>' });
            $('#Table').bootstrapTable({
                //data: e.Data.MainData,
                data: e.Data,
                striped: true,
                cache: false,
                pagination: true,
                sortable: false,
                sortOrder: "asc",
                sidePagination: "client",
                pageNumber: 1,
                pageSize: 25,
                pageList: [25, 50, 75, 100],
                search: false,
                strictSearch: true,
                searchOnEnterKey: false,
                showColumns: true,
                showRefresh: true,
                minimumCountColumns: 2,
                clickToSelect: true,
                showToggle: true,
                cardView: false,
                detailView: false,
                dataType: "json",
                method: "post",
                searchAlign: "left",
                buttonsAlign: "left",
                toolbar: "#toolbar",
                toolbarAlign: "right",
                columns: c,
                locale: "zh-CN"
                //onpagechange: function (n, s) {
                //    loadtable(n, s);
                //}
            });
            //PageMax = e.Data.CountPage;//設置分頁欄頁面數量
            //PageMax = e.Data.length;//設置分頁欄頁面數量
            //Pagination(PageMax, PageNumber)
        }
        else {
            swal({
                title: "提示",
                text: e.Message,
                timer: 2000,
                type: "warning",
                showConfirmButton: false
            });
        }
        mesUI.SetLanguage("RouteConfig");
    })
};

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) {
        alert(unescape(r[2]));
    }
}

function NextPage(page) {
    if (parseInt($('#pagenum').val()) < PageMax && PageMax > 0) {
        // var page = parseInt($('#pagenum').val()) + 1;
        loadTable(SearchName, page, true);
    } else {
        loadTable(SearchName, 1, true);
    }

}
function PreviousPage(page) {
    if (parseInt($('#pagenum').val()) > 1) {
        //var page = parseInt($('#pagenum').val()) - 1;
        loadTable(SearchName, page, true);
    } else {
        loadTable(SearchName, PageMax, true);
    }

}

function Pagination(PageNum, currentPage) {
    $('#pageLimit').bootstrapPaginator({
        currentPage: currentPage,//当前的请求页面。
        totalPages: PageNum,//一共多少页。
        size: "normal",//应该是页眉的大小。
        bootstrapMajorVersion: 3,//bootstrap的版本要求。
        alignment: "right",
        numberOfPages: 5,//一页列出多少数据。
        itemTexts: function (type, page, current) {//如下的代码是将页眉显示的中文显示我们自定义的中文。
            switch (type) {
                case "first": return "<<";
                case "prev": return "<";
                case "next": return ">";
                case "last": return ">>";
                case "page": return page;
            }
        }
        //, onPageClicked: function (event, originalEvent, type, page) {//给每个页眉绑定一个事件，其实就是ajax请求，其中page变量为当前点击的页上的数字。
        //    loadTable(SearchName, page, true);
        //}
    });
}


jsPlumb.ready(function () {

    var StationStr; //工站名
    var StationID;
    $('#RouteConfig').hide();

    loadTable("", "1", false);
    Pagination(PageMax);
    // PageList("", "1");
    //$('#pagenum').change(function () {
    //    var SelectPage = $('#pagenum option:selected').text();
    //    $('#pagenumList').text(SelectPage);
    //    loadTable(SearchName, SelectPage, true);
    //});

    $("#dropdown-ul").on("click", "li", function () {
        $("#dropdown-ul li").removeClass("active");
        $(this).addClass("active");
        $("#page-size").text($(this).text());
        loadTable(SearchName, "1", true);
    });
    mesUI.SetLanguage("RouteConfig");
});
