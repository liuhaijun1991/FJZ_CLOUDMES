var FAIListData = [];
var mesUI;
var lan;
var client = null;
var FAI_EditRow = null;
var FAI_DML;
var filename = "";
var UseType = "";
var Bas64File = null;
var postData={};
var chooselan,successlan,Faillan,errormes0,errormes1,
    Deletemes, submitmes, cancelmes, warningmes, samefile, errorComplete;

$(document).ready(function () {
    client = self.parent.client;
    mesUI = new MesClientUI(client);
    lan = $.cookie($.MES.CK_LAN_NAME);
    if (lan == "CHINESE") {
        tableLocale = "zh-CN";
        chooselan="--请选择--";
        successlan="操作成功";
        Faillan="操作失败"
        errormes0="请选择上传文件";
        errormes1="请选择机种";
        Deletemes="确认删除？";
        submitmes="确认";
        cancelmes="取消";
        warningmes = "警告"
        samefile = "File must be the same name.";
        errorComplete = "You cannot modify registers that are already completed."

    }
    else if (lan == "CHINESE_TW") {
        tableLocale = "zh-TW"
        chooselan="--請選擇--";
        successlan="操作成功";
        Faillan="操作失敗";
        errormes0="請選擇上傳文件";
        errormes1="請選擇機種";
        Deletemes="確認刪除？"
        submitmes="確認";
        cancelmes="取消";
        warningmes = "警告"
        samefile = "File must be the same name."
        errorComplete = "You cannot modify registers that are already completed."
    }
    else {
        tableLocale = "en"
        chooselan="--Pls Choose--";
        successlan="Operation successful";
        Faillan="Operation failed";
        errormes0="Pls Choose a file to upload";
        errormes1="Pls choose skuno";
        Deletemes="Sure to Delete?"
        submitmes="Submit";
        cancelmes="Cancel";
        warningmes = "Warining"
        samefile = "File must be the same name."
        errorComplete = "You cannot modify registers that are already completed."
    };

    mesUI.SetLanguage("FAIREPORT");
    GetMenuList();
    Get_Sku();

  


    $("#NewFAI").on("click", function () {
        $('#FAI_List_Table').bootstrapTable("uncheckAll");
        layer.open({
            type: 1,
            title: 'New PRR',
            area: ["60%", "80%"],
            scrollbar: false,
            content: $("#ModifyList"),
            success: function (layero, index) {
                $("#ModifyList").removeClass("hidden");
                $("#SelectFAIType").attr('disabled', false);
                $('#SelectFAIType option:first').prop('selected', 'selected');
                $("#InputTextWorkorderno").attr('disabled', true);
                $("#FileUpload").attr("disabled", false);
                $("#FileUpload").val("");
                $("#InputTextWorkorderno").val("");
                $('#SelectProductType option:first').prop('selected', 'selected');
                $("#SelectSku").attr('disabled', false);
                $('#SelectSku option:first').prop('selected', 'selected');
                $("#InputTextSkuVer").val("");
                $('#SelectStation option:first').prop('selected', 'selected');
                $("#SelectStation").attr('disabled', true);
                $("#Remark").val("");
            },
            end: function () {
                $("#ModifyList").addClass("hidden");
            }
        });
    })

    $("#EditFAI").on("click", function () {
        var rows = $('#FAI_List_Table').bootstrapTable('getSelections');
        if (rows[0].REMARK == 'COMPLETE') {
            layer.msg(errorComplete, {
                icon: 2,
                time: 5000
            })
        }
        else {
            //$('#FAI_List_Table').bootstrapTable("uncheckAll");
            layer.open({
                type: 1,
                title: 'Edit PRR',
                area: ["60%", "80%"],
                scrollbar: false,
                content: $("#editMRR"),
                success: function (layero, index) {
                    $("#editMRR").removeClass("hidden");
                    $("#SelectFAIType").attr('disabled', false);
                    $('#SelectFAIType option:first').prop('selected', 'selected');
                    $("#InputTextWorkorderno").attr('disabled', true);
                    $("#FileUpload").attr("disabled", false);
                    $("#FileUpload").val("");
                    $("#InputTextWorkorderno").val("");
                    $('#SelectProductType option:first').prop('selected', 'selected');
                    $("#SelectSku").attr('disabled', false);
                    $('#SelectSku option:first').prop('selected', 'selected');
                    $("#InputTextSkuVer").val("");
                    $('#SelectStation option:first').prop('selected', 'selected');
                    $("#SelectStation").attr('disabled', true);
                    $("#Remark").val("");
                    var rows = $('#FAI_List_Table').bootstrapTable('getSelections');
                    $("#nonStaticStatus").val(rows[0].REMARK);
                    $("#staticSku").val(rows[0].SKUNO);
                    $("#staticSkuVer").val(rows[0].SKU_VER);
                    $("#staticType").val(rows[0].FAITYPE);
                    $("#staticProduct").val(rows[0].PRODUCTTYPE);
                    $("#staticFile").val(rows[0].FILENAME);//staticFile

                },
                end: function () {
                    $("#ModifyList").addClass("hidden");
                }
            });
        }
    })

    $("#SelectSku").change(function () {//選擇機種自動獲取版本
        if ($("#SelectSku").val() != "") {
            $("#InputTextSkuVer").val();
            client.CallFunction("MESReport.DCN.FAIReportAPI", "GetSkuVerBySkuno", { Skuno: $("#SelectSku").val() }, function (e) {
                if (e.Status == "Pass") {
                    $("#InputTextSkuVer").val(e.Data[0]);
                }
            })
        }
    }) 

    //btnChangeFile

    $("#btnChangeFile").on("click", function () {
        $("#editPanel").removeAttr("hidden");
    }) 

    $("#btnCancel").on("click", function () {
        $("#editPanel").attr("hidden","hidden");
        layer.closeAll();
    }) 

    $("#DeleteFAI").on("click", function () {
        var rows = $('#FAI_List_Table').bootstrapTable('getSelections');
        if (rows.length > 0) {
            layer.open({
                title: warningmes,
                btn: [submitmes, cancelmes],
                content: Deletemes,
                yes: function (index) {
                    layer.close(index);
                    parent.client.CallFunction("MESReport.DCN.FAIReportAPI", "DeleteOBAFAI", { ID: rows[0].ID }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg(successlan, {
                                icon: 1,
                                time: 1000
                            }, function () {
                                GetMenuList();
                            });
                        }
                        else {
                            layer.msg(e.Message, {
                                icon: 2,
                                time: 2000
                            }, function () {
                            });
                        }
                    });
                }
            });
        } 
    });
    $('#FileUpload').change(function (e) {
        var w = new Worker("../../Scripts/Setting/BigFileReader.js");
        if($(this).val()!=""){
        w.onmessage = function (e) {
            layer.closeAll("loading");
            if (e.data.Status == "Pass") {
                Bas64File = e.data.Bas64File;
            } else {
                $('#FileUpload').val("");
                layer.msg(e.data.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
        };
        w.onerror = function () {
            $('#FileUpload').val("");
            layer.msg(Faillan, {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
        filename = $(this).val();
        filename = filename.substring(filename.lastIndexOf("\\") + 1);
        UseType = filename.substring(filename.lastIndexOf(".") + 1).toUpperCase();
        w.postMessage({ file: e.target.files[0], filename: filename, UseType: UseType, ExtName: ".DOC,.DOCX,.TXT,.XLS,.XLSX,.PDF,.LAB,.JPG,.PNG,.PPT,.PPTX,.ZIP,.ODT,.CSV" });
     }
    });


    //-----------------------------------------------------------------------------------------------------------

    $('#FileUpload2').change(function (e) {
        var w = new Worker("../../Scripts/Setting/BigFileReader.js");
        if ($(this).val() != "") {
            w.onmessage = function (e) {
                layer.closeAll("loading");
                if (e.data.Status == "Pass") {
                    Bas64File = e.data.Bas64File;
                } else {
                    $('#FileUpload2').val("");
                    layer.msg(e.data.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            };
            w.onerror = function () {
                $('#FileUpload2').val("");
                layer.msg(Faillan, {
                    icon: 2,
                    time: 3000
                }, function () {
                });
            }
            filename = $(this).val();
            filename = filename.substring(filename.lastIndexOf("\\") + 1);
            UseType = filename.substring(filename.lastIndexOf(".") + 1).toUpperCase();
            w.postMessage({ file: e.target.files[0], filename: filename, UseType: UseType, ExtName: ".DOC,.DOCX,.TXT,.XLS,.XLSX,.PDF,.LAB,.JPG,.PNG,.PPT,.PPTX,.ZIP,.ODT,.CSV" });
        }
    });

     //-----------------------------------------------------------------------------------------------------------

    $("#Btn_Save").on("click", function (e) {
        if($("#SelectSku").val()==""){
            layer.msg(errormes1, {
                icon: 2,
                time: 3000
            })
        }
        else if (filename==""){
            layer.msg(errormes0, {
                icon: 2,
                time: 3000
            })
        }
        else{
           // var id = $("#FAIConfigForm input[name=ID]").val();
            var ClassName, FunctionName;
            var NAME=$("#FileUpload").val();
            NAME=NAME.substring(NAME.lastIndexOf("\\") + 1,NAME.lastIndexOf("."));

            if ($('#SelectFAIType').val() == "SKUNO") {//機種類型
            postData={Name:NAME,FileName:filename,Bas64File:Bas64File};
            //postData['ID']=id;
            postData['FAITYPE']=$("#SelectFAIType").val();
            postData['SKUNO']=$("#SelectSku").val();
            postData['SKU_VER']=$("#InputTextSkuVer").val();
            postData['PRODUCTTYPE']=$("#SelectProductType").val();
            postData['STATUS'] = $("#selectStatus").val();
                client.CallFunction("MESJuniper.Api.MRRAPI","NewMRRbySkuno",postData,function (e) {
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    layer.msg(successlan, {
                        icon: 1,
                        time: 1000
                    }, function () {
                        $("#FileUpload").val("");
                        GetMenuList();
                        layer.closeAll();
                    });
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 2000
                    }, function () {
                    });
                }
            }); 
            }
        }
        
    });
    $("#RefreshFAI").on("click", function () { GetMenuList(); });
});

$("#btnEdit").on("click", function (e) {
    
    if (filename == "") {
        layer.msg(errormes0, {
            icon: 2,
            time: 5000
        })
    }
    else if ($("#staticFile").val() != filename) {
        layer.msg(samefile, {
            icon: 2,
            time: 5000
         })
    }
    else {
        var rows = $('#FAI_List_Table').bootstrapTable('getSelections');
        var id = $("#FAIConfigForm input[name=ID]").val();
        var ClassName, FunctionName;
        var NAME = $("#FileUpload2").val();
        NAME = NAME.substring(NAME.lastIndexOf("\\") + 1, NAME.lastIndexOf("."));

        if ($('#SelectFAIType').val() == "SKUNO") {//機種類型
            postData = { Name: NAME, FileName: filename, Bas64File: Bas64File };
            postData['ID'] = rows[0].ID;
            /*postData['FAITYPE'] = $("#SelectFAIType").val();*/
            postData['SKUNO'] = $("#staticSku").val();
            /*postData['SKU_VER'] = $("#InputTextSkuVer").val();*/
            postData['STATUS'] = $("#nonStaticStatus").val();
            client.CallFunction("MESJuniper.Api.MRRAPI", "UpdateMRRbySkuno", postData, function (e){
                layer.closeAll('loading');
                if (e.Status == "Pass") {
                    layer.msg(successlan, {
                        icon: 1,
                        time: 3000
                    }, function () {
                        $("#FileUpload2").val("");
                        GetMenuList();
                        layer.closeAll();
                    });
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 2000
                    }, function () {
                    });
                }
            });
        }
    }
        $("#RefreshFAI").on("click", function () { GetMenuList(); });
});
//    $("#RefreshFAI").on("click", function () { GetMenuList(); });
//});


function DownLoadFile(FAIID){
    client.CallFunction("MESJuniper.Api.MRRAPI", "FileDownLoad", { "ID": FAIID }, function (e) {
        if (e.Status == "Pass") {
            var aaa=e.Data.Content;
            var blob = b64toBlob(e.Data.Content);
            if (window.navigator.msSaveOrOpenBlob) {
                navigator.msSaveBlob(blob, e.Data.FileName);
            } else {
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = e.Data.FileName;
                link.click();
                window.URL.revokeObjectURL(link.href);
            }
        }
        else {
            layer.msg(e.Message, {
                icon: 2,
                time: 3000
            }, function () { });
            return;
        }
    });
};
function b64toBlob(b64Data, sliceSize) {
    sliceSize = sliceSize || 512;
    var byteCharacters = atob(b64Data);
    var byteArrays = [];
    for (var offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        var slice = byteCharacters.slice(offset, offset + sliceSize);
        var byteNumbers = new Array(slice.length);
        for (var i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }
        var byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }
    var blob = new Blob(byteArrays);
    return blob;
};
window.operateEvents = {
    'click #bind': function(e, value, row, index) {
        if(row.FILENAME!=null){
        DownLoadFile(row.ID);}
    }
};

function GetMenuList(){
    $("#DeleteFAI").attr("disabled", "disabled");
    $("#EditFAI").attr("disabled", "disabled");
    $("#FAI_List_Table").html("");
    $("#FAI_List_Table").bootstrapTable('destroy');
    client.CallFunction("MESJuniper.Api.MRRAPI", "GetOBAFAIMenuList", {}, function (e) {
        if (e.Status == "Pass") {
            $('#FAI_List_Table').bootstrapTable({//主頁面報表
                data: e.Data,
                striped: true,                    //是否显示行间隔色
                cache: false,                      //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
                sortable: false,                   //是否启用排序
                sortOrder: "desc",                  //排序方式
                pagination: true,                  //是否显示分页（*）
                sidePagination: "client",          //分页方式：client客户端分页，server服务端分页（*）
                pageNumber: 1,                     //初始化加载第一页，默认第一页
                pageSize: 10,                       //每页的记录行数（*）
                pageList: [10, 20, 60, 100],        //可供选择的每页的行数（*）
                showColumns: false,                 //是否显示 内容列下拉框
                showRefresh: false,                 //是否显示刷新按钮
                minimumCountColumns: 2,            //最少允许的列数
                clickToSelect: false,               //是否启用点击选中行
                singleSelect: true,                //单选checkbox
                showToggle: false,                  //是否显示详细视图和列表视图的切换按钮
                cardView: false,                   //是否显示详细视图
                detailView: false,                 //是否显示父子表
                search: true,                      //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，
                strictSearch: false,               //设置为 true启用 全匹配搜索，否则为模糊搜索
                searchOnEnterKey: false,            //回车搜索
                searchTimeOut: 500,                //设置搜索超时时间
                trimOnSearch: true,                //设置为 true 将允许空字符搜索
                searchAlign: "right",              //查询框对齐方式
                toolbarAlign: "left",              //工具栏对齐方式
                buttonsAlign: "right",             //按钮对齐方式
                uniqueId: "ID",
                selectItemName: "ID",
                toolbar: "#Table_Toolbar",
                locale: tableLocale,//語言選項
                onCheck: function (row) {
                    var rows = $('#FAI_List_Table').bootstrapTable('getSelections');
                    if (rows.length <= 0) {
                        $("#DeleteFAI").attr("disabled", "disabled");
                        $("#EditFAI").attr("disabled", "disabled");
                    } else {
                        $("#DeleteFAI").removeAttr("disabled") //EditFAI
                        $("#EditFAI").removeAttr("disabled") 
                    }
        
                },
                onUncheck: function (row) {
                    var rows = $('#FAI_List_Table').bootstrapTable('getSelections');
                    if (rows.length <= 0) {
                        $("#DeleteFAI").attr("disabled", "disabled");
                        $("#EditFAI").attr("disabled", "disabled");
                    } else {
                        $("#DeleteFAI").removeAttr("disabled")
                        $("#EditFAI").removeAttr("disabled")
                    }
                },
                columns: [{
                    title: '<label set-lan="html:SELECT">SELECT</label>',
                    checkbox: true
                }, {
                    field: 'ID',
                    title: '<label set-lan="html:FAI_ID">FAI_ID</label>'
                }, {
                    field: 'FAITYPE',
                    title: '<label set-lan="html:FAIType">FAIType</label>'
                }, {
                    field: 'SKUNO',
                    title: '<label set-lan="html:Skuno">Skuno</label>'
                }, {
                    field: 'SKU_VER',
                    title: '<label set-lan="html:SKU_VER">SKU_VER</label>'
                }, {
                    field: 'WORKORDERNO',
                    title: '<label set-lan="html:Wo">Wo</label>',
                    visible:false
                },
                {
                    field: 'ECONO',
                    title: '<label set-lan="html:Eco">Eco</label>',
                    visible:false
                },
                {
                    field: "LINE",
                    title: '<label set-lan="html:Line">Line</label>',
                    visible:false
                },
                {
                    field: 'PRODUCTTYPE',
                    title: '<label set-lan="html:ProductType">ProductType</label>'
                },
                {
                    field: 'FAISTATION',
                    title: '<label set-lan="html:Station">Station</label>',
                    visible:false
                },
                {
                    field: 'STATUS',
                    title: '<label set-lan="html:STATUS">STATUS</label>',
                    visible:false
                }, {
                    field: 'FILENAME',
                    title: '<label set-lan="html:FileName">FileName</label>',
                    formatter: FileFormatter,
                    events: operateEvents,
                }, {
                    field: 'REMARK',
                    title: '<label set-lan="html:Remark">Remark</label>'
                }, {
                    field: 'EDITBY',
                    title: '<label set-lan="html:LastEditBy">LastEditBy</label>'
                }, {
                    field: 'EDITTIME',
                    title: '<label set-lan="html:EditTime">EditTime</label>'
                }],
            });
            mesUI.SetLanguage("FAIREPORT");
        } else {
            layer.msg(e.Message, {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });
   }

function FileFormatter(value, row, index) {
    var actions = [];
    actions.push('<button id="bind" type="button" class="btn btn-link">'+row.FILENAME+'</button>');
    return actions.join('');
}

function Get_Sku() {//獲取選擇機種
    client.CallFunction("MESReport.DCN.FAIReportAPI", "GetAllFAISkuno", {}, function (e) {
        var sel = $("#SelectSku");
        sel.empty();
        sel.append($("<option value=''>--"+chooselan+"--</option>"));
        if (e.Status == "Pass") {
            for (var a = 0; a < e.Data.length; a++) {
                sel.append($("<option value='" + e.Data[a] + "'>" + e.Data[a] + "</option>"));
            }
        }
    });
};