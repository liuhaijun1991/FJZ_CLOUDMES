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
    var RepairClass="";
    if (IsRepair)
    {
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

//删除选中
function DeleteSelect() {
    var arr = getSelectedRegions();
    for (var i = 0; i < arr.length; i++) {
        jsPlumb.remove(arr[i], true);
    }
    jsPlumb.repaintEverything();

}

function GetNextId(ID)
{
    var list = [];//全部的连接点列表
    list = jsPlumb.getAllConnections();//获取所有的链接
    var IDList = Array();
    
        for (var i in list) {
            for (var j = 0; j < list[i].length; j++) {
                if (list[i][j]['targetId'] == ID) {
                    IDList[0] = list[i][j]['sourceId'];
                }
                if (list[i][j]['sourceId'] == ID) {
                    if ($('#' + list[i][j]['targetId'] + ' h5').text() != "Repair") {
                        IDList[1] = list[i][j]['targetId'];
                    }
                }
            }
        }
    
    return IDList;
}

//序列化路由数据,json格式
function save() {
    var list = [];//全部的连接点列表
    var RETURN_FLAG = "N";
    var IsReturnStation = false;
    var ReturnItemss = "";
    var index = 0;
    var RepairIdArray = new Array();
    var RepairIdArray2 = new Array();
    var ReturnRepairIdArray = new Array();
    var RepairFlage = true;
    list = jsPlumb.getAllConnections();//获取所有的链接  
    var Detail1 = [];
    var Detail = [];
    var TopIdArray = new Array();
    var SEQ_ID = 10;
    var LastIndex = 0;
    var RepairIdsIndex = 0;

    var ROUTE_NAME = $("#ROUTE_NAME").val();
    var ROUTE_TYPE = $("#ROUTE_TYPE").val();
    var DEFAULT_SKUNO = $("#DEFAULT_SKUNO").val();
    if ($.trim(ROUTE_NAME) == "")
    {
        swal("請填寫路由名稱！", "warning");
        return;
    }
    if ($.trim(ROUTE_TYPE) == "") {
        swal("請選擇路由類型！", "", "warning");
        return;
    }
   
    //記錄進維修的ID
    for (var i in list) {
        for (var j = 0; j < list[i].length; j++)
        {
            if (list[i][j].endpoints[0].anchor.type == "BottomCenter") {
                        RepairIdArray[index] = list[i][j]['targetId'];
                        RepairIdArray2[index] = list[i][j]['sourceId'];
                        index++;
                    }
        }
    }

    var IsRepair = false;
    var IsSkip = false;
    var FirstId = "";
    var LastId = "";
    var IsOneFirstId = false;
    var FirstIdNum = 0;
    var IdListIndex = 0;
    var SourceIdList = new Array();
    var TargetIdList = new Array();
    var StationName = "";

    var SampleteList = [];
    index = 0;
    SourceIdList.splice(0, SourceIdList.length);
    TargetIdList.splice(0, TargetIdList.length);
    
    //查找第一工站和最後工站
    for (var i in list) {
        for (var j = 0; j < list[i].length; j++)
        {
            var FirstDot = list[i][j]["endpoints"][0]["anchor"]["type"];
            var NextDot = list[i][j]["endpoints"][1]["anchor"]["type"];

            var R_Name=$('#' + list[i][j]["sourceId"]+' h5').text();
            //判斷維修連線規則
            if (R_Name == "Repair") {
                if (FirstDot == "LeftMiddle" && NextDot != "BottomCenter") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
                if (FirstDot == "TopCenter") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
                if (FirstDot == "BottomCenter") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
                if (FirstDot == "RightMiddle") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
            } else {
                //判斷連線規則
                if (FirstDot == "TopCenter" && NextDot != "TopCenter") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("跳站只能連接頂部端點！工站名：" + StationName, "錯誤", "warning");
                    return;
                }
                if (FirstDot == "RightMiddle" && NextDot != "LeftMiddle") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
                if (FirstDot == "LeftMiddle") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
                if (FirstDot == "BottomCenter" && NextDot != "TopCenter") {
                    StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                    swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                    return;
                }
            }
            if (list[i][j]["sourceId"] == list[i][j]["targetId"]) {
                StationName = $("#" + list[i][j]["sourceId"] + " h5").text();
                swal("工站：" + StationName + " 連接錯誤！", "錯誤", "warning");
                return;
            }

            
            for (x in RepairIdArray) {
                if (RepairIdArray[x] == list[i][j]['targetId'] || RepairIdArray[x] == list[i][j]['sourceId']) {
                    IsRepair = true;
                }
            }
            if (list[i][j].endpoints[0].anchor.type == "TopCenter") {
                IsSkip = true;
            }
            //記錄正常流程上一工站ID和下一工站ID，IsRepair判斷是否維修工站IsSkip判斷是否跳站
            if (IsRepair == false && IsSkip == false) {
                SourceIdList[IdListIndex] = list[i][j]['sourceId'];
                if (list[i][j]['targetId'] != "")
                {
                    TargetIdList[IdListIndex] = list[i][j]['targetId'];
                }
                IdListIndex++;
            }
            IsRepair = false;
            IsSkip = false;           
        }
    }

    //查找第一個工站ID，FirstId
    for (var j in SourceIdList) {
        if (SourceIdList.hasOwnProperty(j)) {
            for (var i in TargetIdList) {
                if (TargetIdList.hasOwnProperty(i)) {
                    if (SourceIdList[j] == TargetIdList[i]) {
                        IsOneFirstId = true;
                    }
                }
            }
            if (IsOneFirstId == false) {
                if (FirstIdNum > 0) {
                    swal("路由配置错误，请确认！", "", "warning");
                    return;
                }
                FirstIdNum++;
                FirstId = SourceIdList[j];
                IsOneFirstId = false;
            }
            IsOneFirstId = false;
        }
    }
    IsOneFirstId = false;
    FirstIdNum = 0;
    //查找最後一個工站ID，LastId
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
                if (FirstIdNum > 0) {
                    swal("路由配置错误，请确认！", "", "warning");
                    return;
                }
                FirstIdNum++;
                LastId = TargetIdList[j];
                IsOneFirstId = false;
            }
            IsOneFirstId = false;
        }
    }

    if (FirstId == "" || LastId == "")
    {
        swal("路由配置错误，请确认！", "", "warning");
        return;
    }

    //生成Json
        for (var k in SourceIdList) {
            if (SourceIdList.hasOwnProperty(k)) {
                for (var i in list) {
                    for (var j = 0; j < list[i].length; j++) {
                        for (x in RepairIdArray) {
                            if (RepairIdArray[x] == list[i][j]['targetId'] || RepairIdArray[x] == list[i][j]['sourceId']) {
                                IsRepair = true;
                            }
                        }
                        if (list[i][j].endpoints[0].anchor.type == "TopCenter") {
                            IsSkip = true;
                        }
                        if (IsRepair == false && IsSkip == false) {
                            //如果是第一個工站，SEQ_ID序號為初始10，下一工站加10
                            if (list[i][j]['sourceId'] == FirstId) {
                                var ids = $('#' + list[i][j]['sourceId'] + ' h5').text();
                                var type = $('#' + list[i][j]['sourceId'] + ' a').text();

                                Detail.push({
                                    ID: list[i][j]['sourceId'],
                                    SEQ_NO: SEQ_ID,
                                    ROUTE_ID: $('#ROUTE_ID').val(),
                                    STATION_NAME: $('#' + list[i][j]['sourceId'] + ' h5').text(),
                                    STATION_TYPE: $('#' + list[i][j]['sourceId'] + ' a').text(),
                                    RETURN_FLAG: RETURN_FLAG,
                                    ReturnItems: [],
                                    NextStation: {
                                        ID: list[i][j]['targetId'],
                                        SEQ_NO: SEQ_ID + 10,
                                        ROUTE_ID: $('#ROUTE_ID').val(),
                                        STATION_NAME: $('#' + list[i][j]['targetId'] + ' h5').text(),
                                        STATION_TYPE: $('#' + list[i][j]['targetId'] + ' a').text(),
                                        RETURN_FLAG: "",
                                    },
                                    DirectLinkStations: []
                                });
                                SEQ_ID += 10;
                                FirstId = list[i][j]['targetId'];
                                if (list[i][j]['targetId'] == LastId) {
                                    Detail.push({
                                        ID: list[i][j]['targetId'],
                                        SEQ_NO: SEQ_ID,
                                        ROUTE_ID: $('#ROUTE_ID').val(),
                                        STATION_NAME: $('#' + list[i][j]['targetId'] + ' h5').text(),
                                        STATION_TYPE: $('#' + list[i][j]['targetId'] + ' a').text(),
                                        RETURN_FLAG: RETURN_FLAG,
                                        ReturnItems: [],
                                        NextStation: null,
                                        DirectLinkStations: []
                                    });
                                }

                                var samplete = $('#' + list[i][j]["sourceId"] + ' samplete').text();
                                if (samplete != "" && samplete != undefined) {
                                    if (samplete == "YES") {
                                        SampleteList.push({
                                            DETIAL_ID: list[i][j]["sourceId"],
                                            IsSampleteStation: "YES"
                                        });
                                    }
                                    else {
                                        SampleteList.push({
                                            DETIAL_ID: list[i][j]["sourceId"],
                                            isSampleteStation: "NO"
                                        });
                                    }
                                } 
                                break;
                            }
                        }
                        IsRepair = false;
                        IsSkip = false;
                    }
                }
            }
        }
        var RepairIds = new Array();
        
        for (var i in list) {
            for (var x = 0; x < RepairIdArray.length; x++) {
                for (var j = 0; j < list[i].length; j++) {
                    if (list[i][j]['sourceId'] == RepairIdArray[index]) {
                        RepairIds[RepairIdsIndex] = RepairIdArray2[index] + "|" + '{"ID":"' + list[i][j]['targetId'] + '",' + '"SEQ_NO":' + GetSEQ_NO(list[i][j]['targetId'], Detail) + ',"STATION_NAME":"' + $('#' + list[i][j]['targetId'] + ' h5').text() + '",' + '"STATION_TYPE":"","RETURN_FLAG":"","ROUTE_ID":""' + "}";
                        RepairIdsIndex++;
                    }
                }
                index++;
            }
        }
        index = 0;
        RepairIdsIndex = 0;
        var MainMessage = new Object();
        MainMessage.ID = $('#ROUTE_ID').val();
        MainMessage.ROUTE_NAME = $('#ROUTE_NAME').val();
        MainMessage.DEFAULT_SKUNO = $('#DEFAULT_SKUNO').val();
        MainMessage.ROUTE_TYPE = $('#ROUTE_TYPE').val();
        MainMessage.EDIT_TIME = "/Date(0)/";
        MainMessage.EDIT_EMP = "";

        var serliza1 = ' { "MainMessage":' + JSON.stringify(MainMessage) + ',"Detail":' + JSON.stringify(Detail) + "}";
        var unpack = JSON.parse(serliza1);
    //在Json加入維修返回工站信息
        for (x in RepairIds) {
            if (RepairIds.hasOwnProperty(x)) {
                var RepairId = RepairIds[x].substring(0, RepairIds[x].lastIndexOf('|'));
                for (i = 0; i < unpack['Detail'].length; i++) {
                    if (RepairId == unpack['Detail'][i]['ID']) {
                        var str = RepairIds[x].substring(RepairIds[x].lastIndexOf('|') + 1);
                        unpack['Detail'][i]['ReturnItems'].push(JSON.parse(str));
                        unpack['Detail'][i]['RETURN_FLAG'] = "Y";
                    }
                }
            }
        }

        for (var i in list) {
            for (var j = 0; j < list[i].length; j++) {
                if (list[i][j].endpoints[0].anchor.type == "TopCenter") {
                    TopIdArray[index] = list[i][j]['sourceId'] + "|" + '{"ID":"' + list[i][j]['targetId'] + '",' + '"SEQ_NO":' + GetSEQ_NO(list[i][j]['targetId'], Detail) + ',"STATION_NAME":"' + $('#' + list[i][j]['targetId'] + ' h5').text() + '",' + '"STATION_TYPE":"","RETURN_FLAG":"","ROUTE_ID":""}';
                    index++;
                }
            }
        }
    //在Json加入跳站信息
        for (x in TopIdArray) {
            if (TopIdArray.hasOwnProperty(x)) {
                var TopId = TopIdArray[x].substring(0, TopIdArray[x].lastIndexOf('|'));
                for (i = 0; i < unpack['Detail'].length; i++) {
                    if (TopId == unpack['Detail'][i]['ID']) {
                        var str = TopIdArray[x].substring(TopIdArray[x].lastIndexOf('|') + 1);
                        unpack['Detail'][i]['DirectLinkStations'].push(JSON.parse(str));
                    }
                }
            }
        }

    var JsonData =  JSON.stringify(unpack); //序列化成json格式

    //判斷是新增還是修改
    if ($("#ROUTE_ID").val() == "") {
        FunctionName = "AddRoute";
      
    } else {
        FunctionName = "UpdateRoute";
    }
    ClassName = "MESStation.Config.RouteConfig";
    //保存

    self.parent.client.CallFunction(ClassName, FunctionName, { RouteJsonString: JsonData }, function (e) {
        if (e.Status == "Pass") {
            loadChartByJSON(e);
            if (SampleteList.length > 0) {
                self.parent.client.CallFunction("MESStation.Config.RouteConfig", "SaveSampleteStation", { SampleteList: JSON.stringify(SampleteList) }, function (e) {
                    if (e.Status != "Pass") {
                        swal("保存SAMPLETESTLOT失敗！" + e.Message, "", "warning");
                    }
                });
            }
            swal("保存成功！", "", "success");
        } else {
            swal("保存失敗！" + e.Message, "", "warning");
        }

    });

}

//獲取序號
function GetSEQ_NO(id, data) {
    var serliza = "{" + '"Detail":' + JSON.stringify(data) + "}";
    serliza = JSON.parse(serliza);
    for (i = 0; i < serliza['Detail'].length; i++) {
        if (id == serliza['Detail'][i]['ID']) {
            return serliza['Detail'][i]['SEQ_NO'];
        }

    }
}
//清空所有連接
function RemoveAllConnect() {
    var list = null;
    list = jsPlumb.getAllConnections();//获取所有的链接 ;
   
    if (list != null) {
        for (var i in list) {
            var j = list[i].length;
            for (var x = 0; x < j; x++) {
                if (list[i].length != 0) {
                    jsPlumb.removeAllEndpoints(list[i][0]['sourceId']);
                }

                // jsPlumb.removeAllEndpoints(list[i][j]['targetId']);
            }
        }
    }
    $('.chart-design').empty();
}

var BlocksX = 100;
var BlocksY = 120;
//通过JSON导入路由
function loadChartByJSON(data) {
    //var data = $('#EDIT_TIME').val();
    var unpack = data;// JSON.parse(data);

    ImportFlage = true;
    if (!unpack) {
        return false;
    }
    BlocksX = 100;
    BlocksY = 120;
    RemoveAllConnect();//清空所有連接
    var DEFAULT_SKUNO = unpack['Data']['MainMessage']['DEFAULT_SKUNO'];
    $('#ROUTE_ID').val(unpack['Data']['MainMessage']['ID']);
    $('#ROUTE_NAME').val(unpack['Data']['MainMessage']['ROUTE_NAME']);
    $('#ROUTE_TYPE').val(unpack['Data']['MainMessage']['ROUTE_TYPE']);
    $('#DEFAULT_SKUNO').val(DEFAULT_SKUNO);
    $('#EDIT_EMP').val(unpack['Data']['MainMessage']['EDIT_EMP']);
    $('#EDIT_TIME').val(unpack['Data']['MainMessage']['EDIT_TIME']);
   

   //Josn載入工站自動佈局
    for (var i = 0; i < unpack['Data']['Detail'].length; i++) {
        var BlockId = unpack['Data']['Detail'][i]['ID'];
        var BlockContent = unpack['Data']['Detail'][i]['STATION_NAME'];
        var StationType = unpack['Data']['Detail'][i]['STATION_TYPE'];
        var BlockX = BlocksX;
        var BlockY = BlocksY;
        var blockAttr = BlockId.split('-')[0];
        $('.chart-design').append("<div class=\"item text-center draggable " + blockAttr + " new-" + blockAttr + "\" id=\"" + BlockId + "\"><h5>" + BlockContent + "</h5><a>" + StationType + "</a></div>");
        $("#" + BlockId)
          .css("left", BlockX)
          .css("top", BlockY);
        $("#" + BlockId + " a").css("display", "none");

        if (unpack['Data']['Detail'][i]['RETURN_FLAG'] == "Y") {
            BlockId = "R_" + unpack['Data']['Detail'][i]['ID'];
            BlockContent = "Repair"; //+ unpack['Data']['Detail'][i]['STATION_NAME'];
            BlockX = BlocksX;
            BlockY = BlocksY + 250;
            blockAttr = BlockId.split('-')[0];
            $('.chart-design').append("<div class=\"item text-center draggable Repair-class " + blockAttr + " new-" + blockAttr + "\" id=\"" + BlockId + "\"><h5>" + BlockContent + "</h5><a>" + StationType + "</a></div>");
            $("#" + BlockId)
              .css("left", BlockX)
              .css("top", BlockY);
            $("#" + BlockId + " a").css("display", "none");
        }
        BlocksX += 170;
    }

    //跳站連線
    for (i = 0; i < unpack['Data']['Detail'].length; i++) {
        if (unpack['Data']['Detail'][i]['DirectLinkStations'][0] != null) {
            for (var DirectLinkIndex = 0; DirectLinkIndex < unpack['Data']['Detail'][i]['DirectLinkStations'].length; DirectLinkIndex++) {
                PageSourceId = unpack['Data']['Detail'][i]['ID'];
                PageTargetId = unpack['Data']['Detail'][i]['DirectLinkStations'][DirectLinkIndex]['ID'];
                Dot1 = "TopCenter";
                Dot2 = "TopCenter";
                //添加連接點
                addEndpoint(PageSourceId, PageTargetId);
                //連接工站
                connect(PageSourceId, PageTargetId, Dot1, Dot2, "Bezier");
            }
        }
    }

    //維修連線
    for (i = 0; i < unpack['Data']['Detail'].length; i++) {
        if (unpack['Data']['Detail'][i]['RETURN_FLAG'] == "Y") {
            PageTargetId = "R_" + unpack['Data']['Detail'][i]['ID'];
            Dot1 = "BottomCenter";
            Dot2 = "TopCenter";
            PageSourceId = unpack['Data']['Detail'][i]['ID'];
            //添加連接點
            addEndpoint(PageSourceId, PageTargetId);
            //連接工站
            connect(PageSourceId, PageTargetId, Dot1, Dot2, "Flowchart");
        }
    }

    //正常連線
    for (i = 0; i < unpack['Data']['Detail'].length - 1; i++) {
        var PageSourceId;
        var PageTargetId;
        PageTargetId = unpack['Data']['Detail'][i]['NextStation']['ID'];
        Dot1 = "RightMiddle";
        Dot2 = "LeftMiddle";
        PageSourceId = unpack['Data']['Detail'][i]['ID'];

        //添加連接點
        addEndpoint(PageSourceId, PageTargetId);
        NewConnect = PageTargetId;
        //連接工站
        connect(PageSourceId, PageTargetId, Dot1, Dot2, "StateMachine");
    }

    //維修返回連線
    var nums = 0;
    var RepairStation = new Array();
    for (var x = 0; x < unpack['Data']['Detail'].length; x++) {
        if (unpack['Data']['Detail'][x]['RETURN_FLAG'] == "Y") {
            RepairStation[nums] = "R_" + unpack['Data']['Detail'][x]['ID'] + "|" + unpack['Data']['Detail'][x]['ReturnItems'][0]['ID'];
            nums++;

            for (var num in RepairStation) {
                if (RepairStation.hasOwnProperty(num)) {
                    for (var ReturnItemsIndex = 0; ReturnItemsIndex < unpack['Data']['Detail'][x]['ReturnItems'].length; ReturnItemsIndex++)
                    {
                        PageSourceId = "R_" + unpack['Data']['Detail'][x]['ID'];
                        PageTargetId = unpack['Data']['Detail'][x]['ReturnItems'][ReturnItemsIndex]['ID'];
                        //添加連接點
                        addEndpoint(PageSourceId, PageTargetId);
                        //連接工站
                        connect(PageSourceId, PageTargetId, "LeftMiddle", "BottomCenter", "Bezier");
                    }
                   
                }
            }
        }
    }
    loadAllStation();
    loadAllSKU();
    loadAllStationType();
    ClassName = "MESStation.Config.SkuConfig";
    FunctionName = "GetSkuByRouteId";
    $("#SKUNO").empty();
    self.parent.client.CallFunction(ClassName, FunctionName, { RouteId: $('#ROUTE_ID').val() }, function (e) {
        if (e.Status == "Pass") {
            for (var i in e.Data) {
                if (e.Data.hasOwnProperty(i)) {
                    $("#SKUNO").append("<option value='" + e.Data[i].SKUNO + "'>" + e.Data[i].SKUNO + "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" + e.Data[i].VERSION + "</option>");
                }
            }

        } else {
            swal(e.Message, "", "warning");
        }
    });
    return true;
}

var StationX =  "100px";
var StationY =  "120px";
var RepairStationX = "100px";
var RepairStationY = "300px";
//新增工站
function newStation() {
    //var LastID= GetLastID();
   
    if (NextId != "" && NextId != null && LastId != null && LastId != "" )
    {
        var NextName = $('#' + NextId + ' h5').text();
        var LastName = $('#' + LastId + ' h5').text();
        if (NextName != "" && LastName != "")
        {
            NewConnect = LastId;
            StationX = $('#' + NewConnect).css('left');
            StationX = (parseInt(StationX) + 170).toString() + "px";
        }
        
    } else if (GetLastID() != "") {
        NextId = "";
        LastId = "";
        NewConnect = GetLastID();
        StationX = $('#' + NewConnect).css('left');
        StationX = (parseInt(StationX) + 170).toString() + "px";
    }
    //else {
    //    AddRoute();
    //}
    if (NewConnect == "")
    {
        AddRoute();
    }
    
    if (StationX == "100px") {
        StationX = BlocksX.toString() + "px";
        StationY = BlocksY.toString() + "px";
    }
    var uid = new Date().getTime();
    if ($('#checkbox-id').is(':checked')) {
        var node = addNode('flow-panel', 'node' + uid, 'Repair', { x: RepairStationX, y: RepairStationY }, $('#StationType').val(), true);   //設置新建工站樣式
        //設置工站連接節點
        jsPlumb.addEndpoint('node' + uid, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.addEndpoint('node' + uid, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.draggable('node' + uid);
        //$("#" + 'node' + uid).draggable({ containment: "parent" });//保证拖动不跨界
        RepairStationX = (parseInt(RepairStationX.substring(0, RepairStationX.lastIndexOf('p'))) + 170).toString() + "px";
    } else {

        var node = addNode('flow-panel', 'node' + uid, $('#Station option:selected').text(), { x: StationX, y: StationY }, $('#StationType').val(), false);   //設置新建工站樣式
        jsPlumb.addEndpoint('node' + uid, { anchor: "RightMiddle" }, SetConnectorStyle("#61B7CF", "StateMachine")); //設置工站連接節點
        jsPlumb.addEndpoint('node' + uid, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.addEndpoint('node' + uid, { anchor: "BottomCenter" }, SetConnectorStyle("#61B7CF", "Flowchart"));
        jsPlumb.addEndpoint('node' + uid, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.draggable('node' + uid);
        //$("#" + 'node' + uid).draggable({ containment: "parent" });//保证拖动不跨界
        StationX = (parseInt(StationX.substring(0, StationX.lastIndexOf('p'))) + 170).toString() + "px";
        if (NewConnect == "") {
            NewConnect = 'node' + uid;
        } else {
            connect(NewConnect, 'node' + uid, "RightMiddle", "LeftMiddle", "StateMachine");
            NewConnect = 'node' + uid;
            if (NextId != "")
            {
                connect('node' + uid, NextId, "RightMiddle", "LeftMiddle", "StateMachine");
                NextId = "";
                LastId = "";
            }
        }
    }
}

//連接工站
function connect(PageSourceId, PageTargetId, Dot1, Dot2, ConnectStyle) {
    var clor = "#61B7CF";
    switch (Dot1)
    {
        case "LeftMiddle": clor = "#61B7CF";
    }
    jsPlumb.connect({
        source: PageSourceId,
        target: PageTargetId,
        //锚点位置
        anchors: [Dot1, Dot2],//獲取連接點位置
        endpoint: ["Dot", { radius: 1 }],  //端点的外形
        connectorStyle: connectorPaintStyle,//连接线的色彩,大小样式 
        connectorHoverStyle: connectorHoverStyle,
        paintStyle: {
            strokeStyle: clor,
            opacity: 0.5,
            radius: 2,
            lineWidth: 2
        },//端点的色彩样式
        isSource: true,    //是否可以拖动(作为连线出发点)
        connector: [ConnectStyle, { stub: [40, 60], gap: 10, cornerRadius: 5, alwaysRespectStubs: true }],  //连接线的样式种类有[Bezier],[Flowchart],[StateMachine ],[Straight ]
        isTarget: true,    //是否可以放置(连线终点)
        maxConnections: -1,    // 设置连接点最多可以连接几条线
        //覆盖物
        overlays: [
          ["Arrow", {//箭头的样式
              location: 1,
              visible: true,
              width: 11,
              length: 11,
              id: "ARROW",
          }]
        ]
    });
};

//添加連接點
function addEndpoint(PageSourceId, PageTargetId) {
    if (PageSourceId.substring(0, 2) == "R_") {
        jsPlumb.addEndpoint(PageSourceId, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.addEndpoint(PageSourceId, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
    }
    else {
        jsPlumb.addEndpoint(PageSourceId, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.addEndpoint(PageSourceId, { anchor: "RightMiddle" }, SetConnectorStyle("#61B7CF", "StateMachine"));
        jsPlumb.addEndpoint(PageSourceId, { anchor: "BottomCenter" }, SetConnectorStyle("#61B7CF", "Flowchart"));
        jsPlumb.addEndpoint(PageSourceId, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
    }
    
    if (PageTargetId.substring(0, 2) == "R_") {
        jsPlumb.addEndpoint(PageTargetId, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.addEndpoint(PageTargetId, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
    } else {
        jsPlumb.addEndpoint(PageTargetId, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
        jsPlumb.addEndpoint(PageTargetId, { anchor: "RightMiddle" }, SetConnectorStyle("#61B7CF", "StateMachine"));
        jsPlumb.addEndpoint(PageTargetId, { anchor: "BottomCenter" }, SetConnectorStyle("#61B7CF", "Flowchart"));
        jsPlumb.addEndpoint(PageTargetId, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
    }
    

    jsPlumb.draggable(PageSourceId);
    jsPlumb.draggable(PageTargetId);

    $("#" + PageSourceId).draggable({ containment: "parent" });//保证拖动不跨界
    $("#" + PageTargetId).draggable({ containment: "parent" });//保证拖动不跨界
};

//加載所有工站
function loadAllStation() {
    var StationNameData = [];
    ClassName = "MESStation.Config.CStationConfig";
    FunctionName = "ShowAllData";

        self.parent.client.CallFunction(ClassName, FunctionName, {}, function (e) {
            if (e.Status == "Pass") {
                $(".Station").empty();
                for (var i in e.Data) {
                    if (e.Data.hasOwnProperty(i)) {
                        $(".Station").append("<option value='" + e.Data[i].Station_Name + "'>" + e.Data[i].Station_Name + "</option>");
                        //StationNameData.push({ StationName: e.Data[i].Station_Name });
                    }
                }
               // SelectSuggest("Station", StationNameData);
               
            } else {
                swal( e.Message,"" , "warning");
            }

        })

};

//加載所有工站類型
function loadAllStationType()
{
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "GetAllStationType";
        self.parent.client.CallFunction(ClassName, FunctionName, {}, function (e) {
            if (e.Status == "Pass") {
                $(".StationType").empty();
                for (var i in e.Data) {
                    if (e.Data.hasOwnProperty(i)) {
                        $(".StationType").append("<option value='" + e.Data[i] + "'>" + e.Data[i] + "</option>");
                    }
                }
            } else {
                swal(e.Message, "", "warning");
            }

        })
}
//載入所有幾種
function loadAllSKU()
{
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
                swal( e.Message, "", "warning");
            }

        })
}

//設置搜索自動補全
function SelectSuggest(SelectID,DataList)
{
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
function SearchRoute()
{
    SearchName = $('#S_RouteName').val();
    loadTable(SearchName, "1", true);
   // PageList(SearchName, "1")
}

//新增路由
function AddRouteMain() {
    $('#RouteConfig').show();
    $('#RoutMain').hide();
    $('#RouteTitle').hide();
    $("#SKUNO").empty();
    BlocksX = 100;
    BlocksY = 120;
    NewConnect = "";
    StationX = "100px";
    StationY = "120px";
    RepairStationX = "100px";
    RepairStationY = "300px";
    RemoveAllConnect();//清空所有連接
    
    $("#ROUTE_ID").val("");
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "GetNewRouteName";
        self.parent.client.CallFunction(ClassName, FunctionName, {}, function (e) {
            if (e.Status == "Pass") {
                $('#ROUTE_NAME').val(e.Data);
                loadAllStation();
                loadAllSKU();
                loadAllStationType();
            } else {
                swal(e.Message, "", "warning");
            }

        })
}

//新增路由
function AddRoute()
{
    NextId = "";
    LastId = "";
    BlocksX = 100;
    BlocksY = 120;
    NewConnect = "";
    StationX = "100px";
    StationY = "120px";
    RepairStationX = "100px";
    RepairStationY = "300px";
    RemoveAllConnect();//清空所有連接
    $("#ROUTE_ID").val("");
    $("#SKUNO").empty();
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "GetNewRouteName";
        self.parent.client.CallFunction(ClassName, FunctionName, {}, function (e) {
            if (e.Status == "Pass") {
                $('#ROUTE_NAME').val(e.Data);
            } else {
                swal(e.Message, "", "warning");
            }

        })
}

//獲取最後工站ID
function GetLastID()
{
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
   var  list = jsPlumb.getAllConnections();//获取所有的链接  
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

//設置默認連線樣式
function SetConnectorStyle(Color, ConnectorStyle) {
    //连接线样式
    connectorPaintStyle = {
        lineWidth: 2,
        strokeStyle: Color,
        joinstyle: "round",
        outlineColor: "rgb(251,251,251)",
        outlineWidth: 2
    };

    // 鼠标悬浮在连接线上的样式
    connectorHoverStyle = {
        lineWidth: 2,
        strokeStyle: "#216477",
        outlineWidth: 2,
        outlineColor: "rgb(251,251,251)"
    };

    exampleEndpoint = {
        endpoint: "Dot",  //端点的外形
        paintStyle: { fillStyle: "#1C84C6", radius: 6 },//端点的色彩样式
        connectorStyle: connectorPaintStyle,//连接线的色彩,大小样式 
        connectorHoverStyle: connectorHoverStyle,
        //anchor: "AutoDefault",
        isSource: true,    //是否可以拖动(作为连线出发点)
        connector: [ConnectorStyle, { stub: [40, 60], gap: 10, cornerRadius: 5, alwaysRespectStubs: true }],  //连接线的样式种类有[Bezier],[Flowchart],[StateMachine ],[Straight ]
        isTarget: true,    //是否可以放置(连线终点)
        maxConnections: -1,    // 设置连接点最多可以连接几条线
        connectorOverlays: [["Arrow", { width: 10, length: 10, location: 1 }]]
    };
    return exampleEndpoint;
}

//function PageList(RouteName, PageNumber)
//{
//    ClassName = "MESStation.Config.RouteConfig";
//    FunctionName = "GetRouteMainMessage"
//    self.parent.client.CallFunction(ClassName, FunctionName, { RouteName: RouteName, PageNumber: PageNumber, PageSize: PageSiz }, function (e) {
//        if (e.Status == "Pass") {
//            $("#pagenum").empty();
//            PageMax = e.Data.CountPage;
//            for (var i = 1; i <= e.Data.CountPage; i++) {
//                $("#pagenum").append("<option value='" + i.toString() + "'>" + i.toString() + "</option>");
//            }
//        }
//    });   
//}

//加載表數據
function loadTable(RouteName, PageNumber, isPosBack) {
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "GetRouteMainMessage"
    if($("#page-size").text()!="")
    {
        PageSizs = $("#page-size").text();
    }
   
    self.parent.client.CallFunction(ClassName, FunctionName, { RouteName: RouteName, PageNumber: PageNumber, PageSize: PageSizs }, function (e) {
        if (e.Status == "Pass") {
            
            if (isPosBack)
            {
                //$('#Table').bootstrapTable('load', e.Data.MainData);
                $('#Table').bootstrapTable('load', e.Data);
            }
            var c = [];

                 c.push({ field: 'ROUTE_NAME', title: '<label set-lan="html:RouteName">ROUTE_NAME</label>' }, { field: 'STATION', title: '<label set-lan="html:DefaultSTATION">STATION</label>' }, { field: 'DEFAULT_SKUNO', title: '<label set-lan="html:DefaultSku">DEFAULT_SKUNO</label>' }, { field: 'ROUTE_TYPE', title: '<label set-lan="html:RouteType">ROUTE_TYPE</label>' }, { field: 'EDIT_TIME', title: '<label set-lan="html:ModifyDate">EDIT_TIME</label>' }, { field: 'EDIT_EMP', title: '<label set-lan="html:ModifyMan">EDIT_EMP</label>' }, { field: 'SKUCOUNT', title: '<label set-lan="html:SKUCOUNT">SKUCOUNT</label>' });
                $('#Table').bootstrapTable({
                //data: e.Data.MainData,
                    data: e.data,
                    striped: true,
                    cache: false,
                    //pagination: true,
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
                PageMax = e.Data.CountPage;//設置分頁欄頁面數量
                Pagination(PageMax, PageNumber)
        }
        else
        {
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

//function loadTable(RouteName, PageNumber, isPosBack) {
//    ClassName = "MESStation.Config.RouteConfig";
//    FunctionName = "GetRouteMainMessage"
//    if ($("#page-size").text() != "") {
//        PageSizs = $("#page-size").text();
//    }

//    self.parent.client.CallFunction(ClassName, FunctionName, { RouteName: RouteName, PageNumber: PageNumber, PageSize: PageSizs }, function (e) {
//        if (e.Status == "Pass") {

//            if (isPosBack) {
//                //$('#Table').bootstrapTable('load', e.Data.MainData);
//                $('#Table').bootstrapTable('load', e.Data);
//            }
//            var c = [];
//            //var checkbox = " title:'checkall',field:'select',checkbox: false, width: 30, align: 'center',valign: 'middle' ";
//            //c.push({ checkbox });
//            //for (var item in e.Data.MainData[0]) {
//            //    if (item != 'ID')
//            //    {
//            c.push({ field: 'ROUTE_NAME', title: '<label set-lan="html:RouteName">ROUTE_NAME</label>' }, { field: 'DEFAULT_SKUNO', title: '<label set-lan="html:DefaultSku">DEFAULT_SKUNO</label>' }, { field: 'ROUTE_TYPE', title: '<label set-lan="html:RouteType">ROUTE_TYPE</label>' }, { field: 'EDIT_TIME', title: '<label set-lan="html:ModifyDate">EDIT_TIME</label>' }, { field: 'EDIT_EMP', title: '<label set-lan="html:ModifyMan">EDIT_EMP</label>' }, { field: 'SKUCOUNT', title: '<label set-lan="html:SKUCOUNT">SKUCOUNT</label>' });
//            $('#Table').bootstrapTable({
//                //data: e.Data.MainData,
//                data: e.data,
//                striped: true,
//                cache: false,
//                //pagination: true,
//                sortable: false,
//                sortOrder: "asc",
//                //sidePagination: "client",
//                //pageNumber: 1,
//                //pageSize: 10,
//                //pageList: [8, 15, 30, 70],
//                search: false,
//                strictSearch: true,
//                searchOnEnterKey: false,
//                showColumns: true,
//                showRefresh: true,
//                minimumCountColumns: 2,
//                clickToSelect: true,
//                showToggle: true,
//                cardView: false,
//                detailView: false,
//                dataType: "json",
//                method: "post",
//                searchAlign: "left",
//                buttonsAlign: "left",
//                toolbar: "#toolbar",
//                toolbarAlign: "right",
//                columns: c,
//                locale: "zh-CN"
//                //onpagechange: function (n, s) {
//                //    loadtable(n, s);
//                //}
//            });
//            PageMax = e.Data.CountPage;//設置分頁欄頁面數量
//            Pagination(PageMax, PageNumber)
//        }
//        else {
//            swal({
//                title: "提示",
//                text: e.Message,
//                timer: 2000,
//                type: "warning",
//                showConfirmButton: false
//            });
//        }
//        mesUI.SetLanguage("RouteConfig");
//    })
//};

function CopyRoute() {
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "CopyRouteByRouteName";
    var FromRouteName = $('#ROUTE_NAME').val();
    var ToRouteName = $('#NewRouteName').val()
   
    self.parent.client.CallFunction(ClassName, FunctionName, { FromRouteName: FromRouteName, ToRouteName: ToRouteName }, function (e) {
        if (e.Status == "Pass") {
            loadChartByJSON(e);
            swal("複製成功！", "", "success");
        } else {
            swal(e.Message, "", "warning");
        }
    });

}

function DeleteRoute() {
    $('#DeleteRouteModal').modal('hide');
    var RouteId = $('#ROUTE_ID').val();
    ClassName = "MESStation.Config.RouteConfig";
    FunctionName = "DeleteRouteByRouteId";
    self.parent.client.CallFunction(ClassName, FunctionName, { RouteId: RouteId}, function (e) {
        if (e.Status == "Pass") {
            $('#RouteConfig').hide();
            $('#RoutMain').show();
            $('#RouteTitle').show();
            loadTable("","1",true);
            swal("刪除成功！", "", "success");
        } else {
            swal(e.Message, "", "warning");
        }
    })
};

function getQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = window.location.search.substr(1).match(reg);
    if (r != null)
    {
        alert(unescape(r[2]));
    }

    //$('#RouteConfig').show();
    //$('#RoutMain').hide();
    ////$(element).css({"color":"blue","font-size":"16px;"});  
    //ClassName = "MESStation.Config.RouteConfig";
    //FunctionName = "GetRouteByRouteId";

    //self.parent.client.CallFunction(ClassName, FunctionName, { RouteId: row.ID }, function (e) {
    //    if (e.Status == "Pass") {
    //        loadChartByJSON(e);
    //        loadAllStation();
    //        loadAllSKU();
    //        loadAllStationType();
    //    } else {
    //        swal(e.Message, "", "warning");
    //    }
    //});
}

function NextPage(page)
{
    if (parseInt($('#pagenum').val()) < PageMax && PageMax > 0) {
       // var page = parseInt($('#pagenum').val()) + 1;
        loadTable(SearchName, page, true);
    } else {
        loadTable(SearchName, 1, true);
    }
    
}
function PreviousPage(page)
{
    if (parseInt($('#pagenum').val()) >1) {
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
        }, onPageClicked: function (event, originalEvent, type, page) {//给每个页眉绑定一个事件，其实就是ajax请求，其中page变量为当前点击的页上的数字。
            loadTable(SearchName, page, true);
        }
    });
}


jsPlumb.ready(function () {
    
    var StationStr; //工站名
    var StationID;
    $('#RouteConfig').hide();
   
    //設置默認參數
    jsPlumb.importDefaults({
        DragOptions: { cursor: 'pointer', zIndex: 2000 },    //拖动时鼠标停留在该元素上显示指针，通过css控制  
        PaintStyle: { strokeStyle: '#4B8CF5' },                //元素的默认颜色  
        EndpointStyle: { width: 20, height: 16, strokeStyle: '#4B8CF5' },      //连接点的默认颜色 
        Endpoint: "Dot",                //连接点的默认形状  
        Anchors: ["TopCenter"]                //连接点的默认位置  
    });

   
    //變更工站名
    $('#Station').change(function () {
        var SelectText = $('#Station option:selected').text();
        $('#list-group-item').html(SelectText);
        newStation();
    });

    ////拖動
    //$('#list-group-item').attr('draggable', 'true').on('dragstart', function (ev) {

    //    ev.originalEvent.dataTransfer.setData('text', ev.target.textContent);
    //    StationStr = $('#list-group-item').text();
    //});

    //$('#flow-panel').on('drop', function (ev) {

    //    if (ev.target.className.indexOf('_jsPlumb') >= 0) {
    //        return;
    //    }
    //    ev.preventDefault();
    //    var mx = '' + ev.originalEvent.offsetX + 'px';
    //    var my = '' + ev.originalEvent.offsetY + 'px';

    //    var uid = new Date().getTime();
    //    var node = addNode('flow-panel', 'node' + uid, StationStr, { x: mx, y: my }, $('#StationType').val());   //設置新建工站樣式
    //    jsPlumb.addEndpoint('node' + uid, { anchor: "RightMiddle" }, SetConnectorStyle("#61B7CF", "StateMachine")); //設置工站連接節點
    //    jsPlumb.addEndpoint('node' + uid, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
    //    jsPlumb.addEndpoint('node' + uid, { anchor: "BottomCenter" }, SetConnectorStyle("#61B7CF", "Flowchart"));
    //    jsPlumb.addEndpoint('node' + uid, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
    //    jsPlumb.draggable($(node));
    //}).on('dragover', function (ev) {
    //    ev.preventDefault();
    //    console.log('on drag over');
    //});

    //删除连接线
    jsPlumb.bind("click", function (conn, originalEvent) {
        if (confirm("确定删除吗?")) {
            jsPlumb.detach(conn);
        }
    });

    //$(document).delegate('.item', 'dblclick', function () {
    //    var parentID = $(this).attr('id');
    //    var parentDOM = $(this);
    //    if (confirm("确定要删除吗?")) {
    //        jsPlumb.removeAllEndpoints(parentID);
    //        parentDOM.remove();
    //        NewConnect = GetLastID();
            
    //    }
    //});

    $("#DEFAULT_SKUNO").blur(function () {
        var rgb = $("#DEFAULT_SKUNO").css("background");
        var index = rgb.indexOf(")");
        rgb = rgb.substring(0,index+1);
        if (rgb == "rgba(255, 0, 0, 0.1)")
        {
            $("#DEFAULT_SKUNO").val("");
            swal("請選擇正確的幾種", "", "warning");
        }
    });

    $("#SaveStationChange").click(function () {
        var Station_M = $("#Station_M").val();
        var StationType = $("#StationType_M").val();
        $("#" + StationID + " h5").text(Station_M);
        $("#" + StationID + " a").text(StationType);


        var isSampletestlot = $("#divIsSampletestlot").find("input:radio[name='Sampletestlot']:checked").val();
        $("#" + StationID + " samplete").text(isSampletestlot);       

        $('#exampleModal').modal('hide');
        swal("修改成功！", "", "success");
    });
    $.contextMenu({
        selector: 'svg',
        callback: function (key, options) {
            jsPlumb.bind( function (conn, originalEvent) {
                if (confirm("确定删除吗?")) {
                    jsPlumb.detach(conn);
                }
            });
        },
        items: {
            "delete": { name: "删除", }
        }
    });

    $.contextMenu({
        selector: '.Repair-class',
        callback: function (key, options) {
                jsPlumb.removeAllEndpoints(options.$trigger.context.id);
                options.$trigger.context.remove();
        },
        items: {
            "delete": { name: "删除", }
        }
    });
   
    //添加右鍵菜單
    $.contextMenu({
        selector: '.item',
        callback: function (key, options) {
            if (key == "edit")
            {
                StationID = options.$trigger.context.id;
                var Station_M = $("#" + StationID + " h5").text();
                var StationType_M = $("#" + StationID + " a").text();
                 //顯示是否是SampleteStation begin
                $("#" + StationID).find("samplete").remove();
                $("#divIsSampletestlot").hide();
                self.parent.client.CallFunction("MESStation.Config.RouteConfig", "CheckSampleteStation", { DetailID: StationID }, function (e) {
                    if (e.Status == "Pass") {                       
                        if (e.Data.ShowSampleteStation == "YES" && e.Data.IsSampleteStation == "YES") {
                            $("#divIsSampletestlot").show();
                            $("#" + StationID).append("<samplete class='hidden' >YES</samplete>");
                            $("#divIsSampletestlot").find("input:radio[name='Sampletestlot'][value='YES']").prop("checked", "checked");
                        }
                        else if (e.Data.ShowSampleteStation == "YES" && e.Data.IsSampleteStation == "NO") {
                            $("#divIsSampletestlot").show();
                            $("#" + StationID).append("<samplete class='hidden' >NO</samplete>");
                            $("#divIsSampletestlot").find("input:radio[name='Sampletestlot'][value='NO']").prop("checked", "checked");
                        } else if (e.Data.ShowSampleteStation == "NO" ) {
                            $("#divIsSampletestlot").hide();
                        }
                    }
                    else {
                        swal("Check Samplete Station Fail！" + e.Message, "", "warning");
                    }
                });
                 //顯示是否是SampleteStation end
                $('#Station_M').val(Station_M);
                $('#StationType_M').val(StationType_M);
                $("#exampleModalLabel").text("编辑");
                $('#exampleModal').modal('show');
            }else
                if (key == "delete") {
                    var Repair = $('#' + options.$trigger.context.id + ' h5').text();
                    if (Repair != "Repair")
                    {
                        var GetID = GetNextId(options.$trigger.context.id);
                        if (GetID[1] != null) {
                            NextId = GetID[1];
                        }
                        if (GetID[0] != null) {
                            LastId = GetID[0];
                        }
                        else {
                            LastId = "";
                        }

                    }
                    jsPlumb.removeAllEndpoints(options.$trigger.context.id);
                    options.$trigger.context.remove();
                    //var MaxNum = 0;
                    //var h5 = $('#' + options.$trigger.context.id + ' h5').text();
                    //while ($('#' + options.$trigger.context.id + ' h5').text() != null && $('#' + options.$trigger.context.id + ' h5').text() != "" && MaxNum < 10) {
                    //    MaxNum++;
                    //    try {
                    //        jsPlumb.removeAllEndpoints(options.$trigger.context.id);
                    //        options.$trigger.context.remove();
                    //    } catch (err) {

                    //    }

                    //}

                    NewConnect = GetLastID();
                    StationX = $('#' + NewConnect).css('left');
                    StationX = (parseInt(StationX) + 170).toString() + "px";
                } else if (key == "Repair") {
                    var uid = new Date().getTime();
                    var RepairX = $('#' + options.$trigger.context.id).offset().left-45+"px";
                    addNode('flow-panel', 'node' + uid, 'Repair', { x: RepairX, y: "370px" }, $('#StationType').val(),true);
                    jsPlumb.addEndpoint('node' + uid, { anchor: "TopCenter" }, SetConnectorStyle("#61B7CF", "Bezier"));
                    jsPlumb.addEndpoint('node' + uid, { anchor: "LeftMiddle" }, SetConnectorStyle("#61B7CF", "Bezier"));
                    jsPlumb.draggable('node' + uid);
                    connect(options.$trigger.context.id, 'node' + uid, "BottomCenter", "TopCenter", "Flowchart");
                }
        },
        items: {
            "edit": { name: "编辑" },
            "sep1": "---------",
            "delete": { name: "删除", },
             "sep2": "---------",
             "Repair": { name: "維修", }
        }
    });
    loadTable("", "1", false);
    Pagination(PageMax);
   // PageList("", "1");
    $('#pagenum').change(function () {
        var SelectPage = $('#pagenum option:selected').text();
        $('#pagenumList').text(SelectPage);
        loadTable(SearchName, SelectPage, true);
    });

    //點擊數據跳轉到路由界面
    $('#Table').on('click-row.bs.table', function (e, row, element) {
        $('#RouteConfig').show();
        $('#RoutMain').hide();
        $('#RouteTitle').hide();
        //$(element).css({"color":"blue","font-size":"16px;"});  
        ClassName = "MESStation.Config.RouteConfig";
        FunctionName = "GetRouteByRouteId";

            self.parent.client.CallFunction(ClassName, FunctionName, { RouteId: row.ID }, function (e) {
                if (e.Status == "Pass") {
                    loadChartByJSON(e);
                    loadAllStation();
                    loadAllSKU();
                    loadAllStationType();
                } else {
                    swal(e.Message, "", "warning");
                }
            });
    });
   
    $('#BackRoute').click(function () {
        $('#RouteConfig').hide();
        $('#RoutMain').show();
        $('#RouteTitle').show();
    });

    $('#CopyRouteButten').click(function () {
        var ROUTE_NAME = $('#ROUTE_NAME').val();
        $('#NowRouteName').val(ROUTE_NAME);
        ClassName = "MESStation.Config.RouteConfig";
        FunctionName = "GetNewRouteName";
        self.parent.client.CallFunction(ClassName, FunctionName, {}, function (e) {
            if (e.Status == "Pass") {
                $('#NewRouteName').val(e.Data);
            } else {
                swal(e.Message, "", "warning");
            }
        })
        
        $('#NowRouteName').val(ROUTE_NAME);
        $("#CopyRouteModalLabel").text("複製");
        $('#CopyRouteModal').modal('show');
    });
    $('#SaveCopyRoute').click(function () {
        CopyRoute();
    });

    //確認刪除路由
    $('#DeleteRouteButten').click(function () {
        $("#DeleteRouteModalLabel").text("刪除路由");
        $('#DeleteRouteModal').modal('show');
    });
   
    $("#dropdown-ul").on("click", "li", function () {      
        $("#dropdown-ul li").removeClass("active");
        $(this).addClass("active");
        $("#page-size").text($(this).text());
        loadTable(SearchName, "1", true);
    });
    mesUI.SetLanguage("RouteConfig");
});
