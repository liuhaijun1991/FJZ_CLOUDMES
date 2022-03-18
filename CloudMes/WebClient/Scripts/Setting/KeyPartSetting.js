var KeyPartData=[];
var Skuno = "";
var ListName = "";
var CustVersion = "";
var DataList = [];
SubTable = {};
$(document).ready(function () {
    var reset = function () {
        $(".table_box").html('');
        $("#table_box1").html('');
    }

    var Get_data = function () {
        parent.client.CallFunction("MESStation.KeyPart.KPScan", "GetAllKPList", { ListName: ListName }, function (e) {
            if (e.Status == "Pass") {
                KeyPartData = e.Data;
                $('#KP_List_Table').bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {
                    $("#KPFileUpload").val("");
                });
            }
        });
    }

    var InitKPItem = function (index, row, $detail) {
        var cur_table = $detail.html('<table></table>').find('table');
        $(cur_table).bootstrapTable({
            pagination: false,
            data: [],
            columns: [{
                field: 'KPPartNo',
                title: 'KPPartNo'
            }, {
                field: 'KPName',
                title: 'KPName'
            }, {
                field: 'QTY',
                title: 'QTY'
            }, {
                field: 'SEQ',
                title: 'SEQ'
            }, {
                field: 'Station',
                title: 'Station'
            }, {
                field: 'Detail',
                title: 'Scan Detail',
                formatter: function (value, row, index) {
                    if (value.length > 0) {
                        var e = "<ol>";
                        for (var i = 0; i < value.length; i++) {
                            e += "<li>" + value[i].SCANTYPE + "</li>";
                        }
                        e += "</ol>";
                        return e;
                    }
                    return "";
                }
            }, {
                field: 'Location',
                    title: 'Location',
                formatter: function (value, row, index) {
                    if (row.Detail.length > 0) {
                        var e = "<ol>";
                        for (var i = 0; i < row.Detail.length; i++) {
                            e += "<li>" + row.Detail[i].LOCATION + "</li>";
                        }
                        e += "</ol>";
                        return e;
                    }
                    return "";
                }
            }]
        });
        var MessageID = "TABLE" + parseInt(Math.random() * 99).toString() + Date.now().toString()
        SubTable[MessageID] = cur_table;
        parent.client.CallFunction("MESStation.KeyPart.KPScan", "GetKPListByListName", { ListName: row.LISTNAME }, function (e) {
            if (e.Status == "Pass") {
                var stb = SubTable[MessageID];
                delete SubTable[MessageID];
                $(stb).bootstrapTable('load', e.Data.Item);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {

                });
            }
        });
    };

    var HTMLOUT = document.getElementById("table_box1");

    var ShowData = {
        id:"ShowData",
        type: 1,
        shade: 0.8,
        shadeClose:false,
        title: "Excel Data",
        area: ['80%', '80%'],
        content: $(".table_box"),
        btn: ["Upload", "cancel"],
        success: function (layero, index) {
            $(".table_box").removeClass("hidden");
        },
        end: function () {
            $(".table_box").addClass("hidden");
        },
        yes: function (index) {
            parent.client.CallFunction("MESStation.KeyPart.KPScan", "UpLoadKPList", { SkuNo: Skuno, ListName: ListName, ListData: DataList, CustVersion: CustVersion }, function (e) {
                if (e.Status == "Pass") {
                    layer.close(index);
                    layer.msg("Success", {
                        icon: 1,
                        time: 3000
                    }, function () {
                        $("#KPFileUpload").val("");
                        Get_data();
                    });
                } else {
                    layer.msg(e.Message, {
                        icon: 2,
                        time: 3000
                    }, function () {
                    });
                }
            });
        },
        cancel: function (index) {
            $("#KPFileUpload").val("");
            layer.close(index);
        }
    };

    var addFunctionAlty = function (value, row, index) {
        return [
            '<button id="bind" type="button" class="btn btn-link">' + row.LISTNAME + '</button>'
        ].join('');
    };

    window.operateEvents = {
        'click #bind': function (e, value, row, index) {
            DownLoadFile(row.LISTNAME, "KPFILE");
        }
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
    var DownLoadFile = function (name, usetype) {
        self.parent.client.CallFunction("MESStation.FileUpdate.FileUpload",
            "FileDownLoadWithNameAndUseTypeByBrowser",
            { "NAME": name, "USETYPE": usetype },
            function (e) {
                if (e.Status == "Pass") {
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
                } else {
                    layer.msg(e.Message,
                        {
                            icon: 2,
                            time: 3000
                        },
                        function () { });
                    return;
                }
            });
    }

    $('#DeleteKPList').on('click', function () {
        var selRows = $('#KP_List_Table').bootstrapTable('getSelections');
        if (selRows.length > 0) {
            layer.open({
                title: 'Warning',
                btn: ['Delete', 'Cancel'],
                content: "Delete operation cannot be rolled back! </br> Are you sure you want to delete these records?",
                yes: function (index) {
                    layer.close(index);
                    var Names = [];
                    for (var i = 0; i < selRows.length; i++) {
                        Names.push(selRows[i].LISTNAME);
                    }
                    parent.client.CallFunction("MESStation.KeyPart.KPScan", "RemoveKPList", { ListNames: Names }, function (e) {
                        if (e.Status == "Pass") {
                            layer.msg("Success", {
                                icon: 1,
                                time: 3000
                            }, function () {
                                Get_data();
                            });
                        }
                        else {
                            layer.msg(e.Message, {
                                icon: 5,
                                time: 3000
                            }, function () {
                            });
                        }
                    });
                }
            });            
        } else {
            layer.msg("no records selected", {
                icon: 2,
                time: 3000
            }, function () {
            });
        }
    });

    $('#DownloadTemplateFile').on('click', function () {
        window.open("../../File/KP_TEMP.xlsx");
    });

    var savefile = function (e, savefilename, c) {
        var w = new Worker("../../Scripts/Setting/BigFileReader.js");
        w.onmessage = function (e) {
            layer.closeAll("loading");
            if (e.data.Status == "Pass") {
                Bas64File = e.data.Bas64File;
                parent.client.CallFunction("MESStation.FileUpdate.FileUpload",
                    "UpLoadFile",
                    {
                        Name: savefilename,
                        FileName: filename,
                        MD5: "",
                        UseType: "KPFILE",
                        Bas64File: Bas64File
                    },
                    function (e) {
                        layer.closeAll('loading');
                        if (e.Status == "Pass") {
                            layer.msg("Success",
                                {
                                    icon: 1,
                                    time: 3000
                                },
                                function () {
                                    $("#btneditdownload").html(savefilename);
                                    c.val(savefilename);
                                });
                        } else {
                            layer.msg(e.Message,
                                {
                                    icon: 2,
                                    time: 3000
                                },
                                function () {
                                });
                        }
                    });
            } else {
                c.val("");
                layer.msg(e.data.Message,
                    {
                        icon: 2,
                        time: 3000
                    },
                    function () {
                    });
            }
        };
        w.onerror = function () {
            c.val("");
            layer.msg("Worker Error!",
                {
                    icon: 2,
                    time: 3000
                },
                function () {
                });
        }
        layer.load(3);
        filename = c.val();
        filename = filename.substring(filename.lastIndexOf("\\") + 1);
        UseType = filename.substring(filename.lastIndexOf(".") + 1).toUpperCase();
        w.postMessage({ file: e.target.files[0], filename: filename, UseType: UseType, ExtName: ".XLS,.XLSX" });
    }

    $('#KPFileUpload').change(function (e) {
        reset();
        var filename = $(this).val();
        var efile = e;
        var c = $('#KPFileUpload');
        if ((filename.indexOf(".xlsx") >= 0) || (filename.indexOf(".xlsm") >= 0) || (filename.indexOf(".xlsb") >= 0) || (filename.indexOf(".xls") >= 0) || (filename.indexOf(".xltx") >= 0) || (filename.indexOf(".xltm") >= 0) || (filename.indexOf(".xlt") >= 0) || (filename.indexOf(".xlam") >= 0) || (filename.indexOf(".xla") >= 0)) {
            var reader = new FileReader();
            reader.readAsArrayBuffer(e.target.files[0]);
            reader.onload = function (e) {
                var data = new Uint8Array(reader.result);
                var wb = XLSX.read(data, { type: 'array' });
                var shitname = wb.SheetNames;

                HTMLOUT.innerHTML = "";
                wb.SheetNames.forEach(function (sheetName) {
                    var htmlstr = XLSX.write(wb, { sheet: shitname[0], type: 'binary', bookType: 'html' });
                    HTMLOUT.innerHTML += htmlstr;
                });

                $("#table_box1").find("td").each(function () { $(this).text($(this).text().trim()); });

                var first_sheet = $("#table_box1").children("table").eq(0).html();
                var colum_qty = $(first_sheet).children('tr').eq(0).children('td').length;
                var arrray1 = [];
                var arrDataAll = [];
                var arrData = [];
                var row_modal = "";
                var other_rows = "";
                var first_row = "";
                var cell = "";
                $(first_sheet).children('tr').eq(0).children('td').each(function () {
                    var zz = $(this).text().trim();
                    var zz1 = "";
                    for (var i = 0; i < zz.length; i++) {
                        if (zz[i].trim() != "") { zz1 += zz[i]; }
                    }
                    cell += '<td>' + zz1 + '</td>';
                });
                Skuno = $(cell).eq(1).text();
                if (Skuno == "") {
                    layer.msg("Skuno is Invalid! </br> Please check your file!", {
                        icon: 2,
                        time: 3000
                    }, function () {
                        $("#KPFileUpload").val("");                        
                    });
                    return false;
                }
                ListName = $(cell).eq(4).text();
                if (ListName == "") {
                    layer.msg("ListName is Invalid! </br> Please check your file!", {
                        icon: 2,
                        time: 3000
                    }, function () {
                        $("#KPFileUpload").val("");
                    });
                    return false;
                }
                CustVersion = $(cell).eq(6).text();
                first_row = '<tr>' + cell + '</tr>';
                for (var i = 0; i < colum_qty; i++)
                    row_modal += '<td></td>';

                $(first_sheet).children('tr').each(function () {
                    arrray1.push($(this).html());

                });

                for (var i = 1; i < arrray1.length; i++) {;
                    if (arrray1[i] != row_modal) { arrDataAll.push(arrray1[i]); }
                }

                for (var i = 1; i < arrDataAll.length; i++) {
                    arrDataAll[i] = '<tr>' + $.MES.filterInvisibleChar(arrDataAll[i]) + '</tr>';

                }

                for (var i = 0; i < colum_qty; i++) {
                    arrData.push($(arrDataAll[0]).eq(i).text());
                }

                var substr1 = '[';
                for (var i = 1; i < arrDataAll.length; i++) {
                    var count3 = -1;
                    var substr = '{';
                    $(arrDataAll[i]).children('td').each(function () {
                        count3++;
                        if (count3 == colum_qty - 1) {
                            var xx = $(this).text().trim();
                            substr += '\"' + arrData[count3] + '\":\"' + $.MES.filterInvisibleChar(xx) + '\"}';
                        }
                        else {
                            var xx1 = $(this).text().trim();
                            substr += '\"' + arrData[count3] + '\":\"' + $.MES.filterInvisibleChar(xx1) + '\",';
                        }


                    });
                    if (i == arrDataAll.length - 1) { substr1 += substr + ']'; break; }
                    else substr1 += substr + ',';

                }
                DataList = JSON.parse(substr1);

                for (var i = 0; i < arrDataAll.length; i++) {
                    first_row += arrDataAll[i];
                }
                $(".table_box").html('<table border="1" class="table table-responsive" style="text-align:center;">' + first_row + '</table>');
                parent.client.CallFunction("MESStation.KeyPart.KPScan", "CheckKPListName", { ListName: ListName }, function (e) {
                    if (e.Status == "Pass") {
                        savefile(efile, ListName, c);
                        if (e.Data) {
                            layer.open({
                                title: 'Warning',
                                btn: ['Replace', 'Cancel'],
                                content: "KP List Name Exists! </br> Continue to cover [" + ListName + "] Keypart List!",
                                yes: function (index) {
                                    layer.close(index);
                                    layer.open(ShowData);
                                }
                            });
                        } else {
                            layer.open(ShowData);
                        }
                    } else {
                        layer.msg(e.Message, {
                            icon: 2,
                            time: 3000
                        }, function () {
                        });
                    }
                });
            }
        }
        else {
            reset();
            alert('Please select excel file with xlsx/xlsm/xlsb/xls/xltx/xltm/xlt/xlam/xla formats');
        }
    });

    $('#KP_List_Table').bootstrapTable({
        pagination: true,
        pageSize: 10,
        pageList: [10, 25, 50, 100],
        search: true,
        striped: true,
        showRefresh: true,
        showSelectTitle: true,
        maintainSelected: true,
        clickToSelect: true,
        detailView: true,
        sortable: true,                   //是否启用排序
        sortOrder: "asc",                  //排序方式
        uniqueId: "id",
        selectItemName: "id",
        toolbar: "#KP_List_Table_Toolbar",
        onExpandRow: function (index, row, $detail) {
            InitKPItem(index, row, $detail);
        },
        onRefresh: function (param) {
            Get_data();
        },
        columns: [{
            field: 'select',
            title: 'select',
            checkbox: true
        }, {
            field: 'LISTNAME',
                title: 'List Name',
                sortable: true,
        }, {
            field: 'SKUNO',
            title: 'SkuNo'
        }, {
            field: 'CUSTVERSION',
                title: 'CustVersion',
                sortable: true,
        }, {
            field: 'EDIT_TIME',
            title: 'Upload Time',
            sortable: true,
        }, {
            field: 'EDIT_EMP',
            title: 'Upload By'
        }, {
            field: 'operate',
            title: '<label set-lan="html:FILE">FILE (click to download)</label>',
            events: operateEvents, //给按钮注册事件
            formatter: addFunctionAlty //表格中增加按钮  
        }],
        data: KeyPartData
    });

    Get_data();
});