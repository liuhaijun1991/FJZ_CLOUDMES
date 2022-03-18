var NewFlag = true;//true為新增修改刪除權限、fail為Approve權限
var SkuListData = [];
var tableLocale = "";//Table支持的語言
var SKU_EditRow = null;//用于存儲記錄
var mesUI;
var lan;
var client = null;
$(document).ready(function () {
    client = self.parent.client;
    mesUI = new MesClientUI(client);
    LoginUserEmp = client.UserInfo.EMP_NO;
    lan = $.cookie($.MES.CK_LAN_NAME);
    if (lan == "CHINESE") {
        tableLocale = "zh-CN"
    }
    else if (lan == "CHINESE_TW") {
        tableLocale = "zh-TW"
    }
    else {
        tableLocale = "en"
    };

    //將Rows對象中字段的值賦值給標簽對象，key為標簽對象,data為row對象
    var FindValueByKey = function (key, data) {
        for (var d in data) {
            if (key.name == d) {
                key.value = data[d];
                break;
            }
            else if (Object.prototype.toString.call(data[d]) === '[object Object]') {
                FindValueByKey(key, data[d]);
            }
        }
    };

    //獲取所有料號稱重信息
    var Get_SkuList = function () {
        $("#Edit").attr("disabled", "disabled");
        $("#Delete").attr("disabled", "disabled");
        $("#Approve").attr("disabled", "disabled");
        //layer彈出框,3是彈出框的一種等待風格；但是load方法不會自動關閉,因此需要在下面的回調函數中關閉
        layer.load(3, { shade: 0.5 });
        client.CallFunction("MESStation.Config.WTConfig", "GetAllWTList", {}, function (e) {
            //在回調函數中，即數據返回來后,關閉layer彈出框
            layer.closeAll('loading');
            if (e.Status == "Pass") {
                SkuListData = e.Data;
                $('#Sku_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };

    //獲取新增修改或Approve權限
    var Get_WTPrivilege = function () {
        $("#Add").addClass("hidden");
        $("#Edit").addClass("hidden")
        $("#Delete").addClass("hidden");
        $("#Approve").addClass("hidden");
        client.CallFunction("MESStation.Config.WTConfig", "Get_WTPrivilege", {}, function (e) {
            if (e.Status = "Pass") {
                var List = [];
                List = e.Data;
                for (var i = 0; i < List.length ; i++) {
                    if (List[i] == "WTConfigModify") {
                        $("#Add").removeClass("hidden");
                        $("#Edit").removeClass("hidden");
                        $("#Delete").removeClass("hidden");
                    } else if (List[i] == "WTConfigApprove") {
                        $("#Approve").removeClass("hidden");
                    }
                }
            } else {
                layer.msg("Get_WTPrivilege Fail:" + e.Data, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });
    };
    //表格操作初使化
    $('#Sku_List_Table').bootstrapTable({
        data: SkuListData,
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        //maintainSelected: false,
        clickToSelect: true,
        detailView: false,
        uniqueId: "ID",
        selectItemName: "ID",
        toolbar: "#Table_Toolbar",
        //選擇某行記錄
        onCheck: function (row) {
            var rows = $('#Sku_List_Table').bootstrapTable('getSelections');
            //如果選擇超過1行或沒選擇,則置Edit為disabled
            if (rows.length > 1 || rows.length <= 0) {
                $("#Edit").attr("disabled", "disabled");
            } else {
                $("#Edit").removeAttr("disabled")
            }
            if (rows.length <= 0) {
                $("#Delete").attr("disabled", "disabled");
                $("#Approve").attr("disabled", "disabled");
            } else {
                $("#Delete").removeAttr("disabled");
                //如果選擇的行中存在已Approve的記錄，則置Approve為disabled
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].APPROVE_EMP != "") {
                        $("#Approve").attr("disabled", "disabled");
                        break;
                    }
                    if (i == rows.length - 1) {
                        $("#Approve").removeAttr("disabled");
                    }
                }
            }
        },
        //取消行選擇
        onUncheck: function (row) {
            var rows = $('#Sku_List_Table').bootstrapTable('getSelections');
            //如果選擇超過1行或沒選擇,則置Edit為disabled
            if (rows.length > 1 || rows.length <= 0) {
                $("#Edit").attr("disabled", "disabled");
            } else {
                $("#Edit").removeAttr("disabled");
            }
            if (rows.length <= 0) {
                $("#Delete").attr("disabled", "disabled");
                $("#Approve").attr("disabled", "disabled");
            } else {
                $("#Delete").removeAttr("disabled");
                //如果選擇的行中存在已Approve的記錄，則置Approve為disabled
                for (var i = 0; i < rows.length; i++) {
                    if (rows[i].APPROVE_EMP != "") {
                        $("#Approve").attr("disabled", "disabled");
                        break;
                    }
                    if (i == rows.length - 1) {
                        $("#Approve").removeAttr("disabled");
                    }
                }
            }
        },
        //設置表頭
        columns: [{
            title: '<label set-lan="html:SELECT">SELECT</label>',
            checkbox: true
        }, {
            field: 'SKUNO',
            title: '<label set-lan="html:SkuNo">SkuNo</label>'
        }, {
            field: 'BMINVALUE',
            title: '<label set-lan="html:BMinValue">BMinValue</label>'
        }, {
            field: 'BMAXVALUE',
            title: '<label set-lan="html:BMaxValue">BMaxValue</label>'
        }, {
            field: 'CMINVALUE',
            title: '<label set-lan="html:CMinValue">CMinValue</label>'
        },{
            field: 'CMAXVALUE',
            title: '<label set-lan="html:CMaxValue">CMaxValue</label>'
        },{
            field: "ONLINE_FLAG",
            title: '<label set-lan="html:OnLine_Flag">OnLine_Flag</label>'
        },{
            field: 'EDIT_EMP',
            title: '<label set-lan="html:EDIT_EMP">EDIT_EMP</label>'
        }, {
            field: 'EDIT_TIME',
            title: '<label set-lan="html:EDIT_TIME">EDIT_TIME</label>'
        }, {
            field: 'APPROVE_EMP',
            title: '<label set-lan="html:Approve_Emp">Approve_Emp</label>'
        }, {
            field: 'APPROVE_TIME',
            title: '<label set-lan="html:Approve_Time">Approve_Time</label>'
        }],
        locale: tableLocale//中文支持,
    });
    //新增
    $("#New").on("click", function () {
        SKU_EditRow = {};
        //找出所有帶form-control類的標簽,即將所有input重置清空
        $("#ModifyModelDetail .form-control").each(function () {
            this.value = "";
        });
        //取消勾選項
        $('#Sku_List_Table').bootstrapTable("uncheckAll");
        //通過彈出框,加載新增頁面#ModifyList
        layer.open({
            type: 1,
            title: 'New WT Params',
            area: ["80%", "90%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
                Get_SkuList();
            }
        });
    });
    //修改
    $("#Edit").on("click", function () {
        var selRows = $('#Sku_List_Table').bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("Please select one record first!", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        } else if (selRows.length > 1) {
            layer.msg("Too many records selected!", {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }
        SKU_EditRow = selRows[0];
        layer.open({
            type: 1,
            title: 'Edit WT Params',
            area: ["60%", "60%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                //將Row數據增值給相應的input標簽
                for (var i = 0; i < $('#Modify input[type=text]').length; i++) {
                    FindValueByKey($('#Modify input[type=text]')[i], SKU_EditRow);
                }
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
                SKU_EditRow = null;
                Get_SkuList();
            }
        });
    })
    //刪除
    $("#Delete").on("click", function () {
        var selRows = $('#Sku_List_Table').bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("Please select one record first!", {
                icon: 2,
                time: 3000
            }, function () {
            });
        } else {
            layer.open({
                title:'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back!</br>  Are you sure you want to delete these records?",
                yes: function (index) {
                    //關閉該彈出框
                    layer.close(index);
                    var IDList = [];
                    for (i = 0; i < selRows.length; i++) {
                        //向數組添加一個或多個元素
                        IDList.push(selRows[i].ID);
                    }
                    parent.client.CallFunction("MESStation.Config.WTConfig", "DeleteWTByID", { IDList: IDList }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("Success", {
                                icon: 1,
                                time:3000
                            }, function () {
                                Get_SkuList();//重新加載數據
                            });
                        } else {
                            layer.msg(e.Message + ":" + e.Data, {
                                icon: 2,
                                time: 5000
                            }, function () {
                            });
                        }
                    })
                }
            });
        }
    });
    //Approve
    $("#Approve").on("click", function () {
        var selRows = $("#Sku_List_Table").bootstrapTable('getSelections');
        if (selRows.length <= 0) {
            layer.msg("Please select one record first!", {
                icon: 2,
                time:3000
            }, function () {
            });
        } else {
            layer.open({
                title: "Attention",
                btn: ["Confirm", "Cancel"],
                //content: "該操作將強制包裝工站必須稱重才能過站!</br>你確定要繼續嗎?",
                content: "This operation will force PACKING Station to be Weight before pass the station!</br>Are you sure to continute?",
                yes: function (index) {
                    layer.close(index);
                    var IDList = [];
                    for (i = 0; i < selRows.length; i++) {
                        IDList.push(selRows[i].ID);
                    }
                    parent.client.CallFunction("MESStation.Config.WTConfig", "ApproveWTByID", { IDList: IDList, LoginUserEmp: LoginUserEmp }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("Approve Successful!", {
                                icon: 1,
                                time:3000
                            }, function () {
                                Get_SkuList();
                            });
                        } else {
                            layer.msg(e.Message + ":" + e.Data, {
                                icon: 2,
                                time: 5000
                            }, function () {
                            });
                        }
                })
                }
            });
        }
    });
    //當修改數據時，同時更新SKU_EditRow的值
    $("#ModifyModelDetail .form-control").on("change", function () {
        if (this.name == "ONLINE_FLAG") {
            if (this.value == "NO") {
                SKU_EditRow[this.name] = "0";
            } else {
                SKU_EditRow[this.name] = "1";
            }
        }else{
            SKU_EditRow[this.name] = this.value;
        }
    });
    //保存
    $("#Save").on("click", function () {
        var ClassName, FunctionName;
        var id = $("#ModifyModelDetail input[name=ID]").val();
        //新增
        if (id == "" || id == undefined) {
            ClassName = "MESStation.Config.WTConfig";
            FunctionName = "AddWT";
        } else {//修改
            ClassName = "MESStation.Config.WTConfig";
            FunctionName = "UpdateWT";
        }
        //判斷輸入的值是否為數字類型
        var strTmp = $("#ModifyModelDetail input[name=BMINVALUE]").val();
        if(isNaN(strTmp)==true&&strTmp!=""){
            layer.msg("Please enter the correct BMinValue:" + strTmp, {
                icon: 2,
                time:3000
            }, function () {
            });
            return;
        }

        strTmp = $("#ModifyModelDetail input[name=BMAXVALUE]").val();
        if (isNaN(strTmp) == true && strTmp != "") {
            layer.msg("Please enter the correct BMaxValue:" + strTmp, {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }
        strTmp = $("#ModifyModelDetail input[name=CMINVALUE]").val();
        if (isNaN(strTmp) == true && strTmp != "") {
            layer.msg("Please enter the correct CMinValue:" + strTmp, {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }
        strTmp = $("#ModifyModelDetail input[name=CMAXVALUE]").val();
        if (isNaN(strTmp) == true && strTmp != "") {
            layer.msg("Please enter the correct CMaxValue:" + strTmp, {
                icon: 2,
                time: 3000
            }, function () {
            });
            return;
        }

        //同步SKU_EditRow的值
        $("#ModifyModelDetail .form-control").each(function () {
            //轉換YES/NO為數字類型
            if (this.name == "ONLINE_FLAG") {
                if (this.value == "NO") {
                    SKU_EditRow[this.name] = "0";
                } else {
                    SKU_EditRow[this.name] = "1";
                }
            } else {
                SKU_EditRow[this.name] = this.value;
            }
        });
        //調用API更新數據
        client.CallFunction(ClassName,FunctionName,{SKU_EditRow:SKU_EditRow},function(e){
            if (e.Status == "Pass") {
                layer.msg("OK," + e.Message, {
                    icon:1,
                    time:3000
                }, function () {
                });
                SKU_EditRow["ID"] = e.Data;
                $("#ModifyModelDetail input[name=ID]").val(e.Data);
            } else {
                layer.msg(e.Message+":"+e.Data, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        });

    });

    Get_SkuList();
    Get_WTPrivilege();
    mesUI.SetLanguage("WT Config");
});



