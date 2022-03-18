var KeyPartData=[];
var Skuno = "";
var ListName = "";
var DataList = [];
SubTable = {};
$(document).ready(function () {

    var reset = function () {
        $(".table_box").html('');
        $("#table_box1").html('');
    }

    var Get_data = function () {
        parent.client.CallFunction("MESStation.KeyPart.KPListImport", "GetAllKPList", { ListName: ListName }, function (e) {
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
                field: 'STATION',
                title: 'STATION'
            }, {
                field: 'PARTNO_SEQ',
                title: 'PART SEQ'
            },{
                field: 'PARTNO',
                title: 'PARTNO'
            }, {
                field: 'QTY',
                title: 'QTY'
            }, {
                field: 'MPN',
                title: 'MPN'
            },{
                field: 'SCANTYPE',
                title: 'SCANTYPE'
            },  {
                field: 'REGEX',
                title: 'REGEX'
            }, {
                field: 'FUNCTIONNAME',
                title: 'FUNCTIONNAME',
                formatter: function (value, row, index) {
                    var taglist = '';
                    if (value.length > 1) {
                        taglist = '<select style="height:100%;width:100%;">';
                        for (var i = 0; i < value.length; i++) {
                            taglist += '<option>' + value[i].FUNCTION + '</option>';
                        }
                        taglist += '</select>';
                    } else if (value.length == 1) {
                        taglist = '<label>' + value[0].FUNCTION + '</label>';
                    } else {
                        taglist = '<label></label>';
                    }
                    return taglist;
                }
            }]
        });

        var MessageID = "TABLE" + parseInt(Math.random() * 99).toString() + Date.now().toString()
        SubTable[MessageID] = cur_table;
        parent.client.CallFunction("MESStation.KeyPart.KPListImport", "GetKPListByListName", { ListName: row.LISTNAME }, function (e) {
            if (e.Status == "Pass") {
                var stb = SubTable[MessageID];
                delete SubTable[MessageID];
                $(stb).bootstrapTable('load', e.Data);
            } else {
                layer.msg(e.Message, {
                    icon: 2,
                    time: 3000
                }, function () {

                });
            }
        });
    };

    

    var Myfunc = function mergeCells(data, fieldName, colspan, target) {
        if (data.length == 0) {
            return;
        }
        var numArr = [];
        var value = data[0][fieldName];
        var num = 0;
        for (var i = 0; i < data.length; i++) {
            if (value != data[i][fieldName]) {
                numArr.push(num);
                value = data[i][fieldName];
                num = 1;
                continue;
            }
            num++;
        }
        if (typeof (value) != "undefined" && value != "") {
            numArr.push(num);
        }
        var merindex = 0;
        for (var i = 0; i < numArr.length; i++) {
            $(target).bootstrapTable('mergeCells', { index: merindex, field: fieldName, colspan: colspan, rowspan: numArr[i] });
            merindex += numArr[i];
        }
    }

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
            parent.client.CallFunction("MESStation.KeyPart.KPListImport", "UpLoadKPList", { SkuNo: Skuno, ListName: ListName, ListData: DataList }, function (e) {
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
                    parent.client.CallFunction("MESStation.KeyPart.KPListImport", "RemoveKPList", { ListNames: Names }, function (e) {
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
                                icon: 2,
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
        window.open("../../File/KPLIST_IMPORT_TMP.xlsx");
    });

    $('#KPFileUpload').change(function (e) {
        reset();
        var filename = $(this).val();
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

                first_row = '<tr>' + cell + '</tr>';
                for (var i = 0; i < colum_qty; i++)
                    row_modal += '<td></td>';

                $(first_sheet).children('tr').each(function () {
                    arrray1.push($(this).html());

                });

                for (var i = 1; i < arrray1.length; i++) {
                    if (arrray1[i] != row_modal) { arrDataAll.push(arrray1[i]); }
                }

                for (var i = 1; i < arrDataAll.length; i++) {
                    arrDataAll[i] = '<tr>' + arrDataAll[i] + '</tr>';

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
                            substr += '\"' + arrData[count3] + '\":\"' + xx + '\"}';
                        }
                        else {
                            var xx1 = $(this).text().trim();
                            substr += '\"' + arrData[count3] + '\":\"' + xx1 + '\",';
                        }


                    });
                    if (i == arrDataAll.length - 1) { substr1 += substr + ']'; break; }
                    else substr1 += substr + ',';

                }

                DataList = JSON.parse(substr1);

                //Excel Data Check Start
                var retmes = '';
                for (var i = 0; i < DataList.length; i++) {
                    if (DataList[i]['Rule'] == '') {
                        retmes = 'Rule not be null! </br> Please check your file!';
                    }
                    if (DataList[i]['PartNO'] == '') {
                        retmes = 'PartNO not be null! </br> Please check your file!'
                    }
                    if (DataList[i]['Item'] == '') {
                        retmes = 'Item not be null! </br> Please check your file!'
                    }
                    if (DataList[i]['Station'] == '') {
                        retmes = 'Station not be null! </br> Please check your file!'
                    }
                    if (DataList[i]['ScanType'] == '') {
                        retmes = 'ScanType not be null! </br> Please check your file!'
                    }

                    if (retmes != '') {
                        layer.msg(retmes, {
                            icon: 2,
                            time: 3000
                        }, function () {
                            $("#KPFileUpload").val("");
                            DataList = [];
                        });
                        return false;
                    }
                }

                //Excel Data Check End

                for (var i = 0; i < arrDataAll.length; i++) {
                    first_row += arrDataAll[i];
                }
                $(".table_box").html('<table border="1" class="table table-responsive" style="text-align:center;">' + first_row + '</table>');
                parent.client.CallFunction("MESStation.KeyPart.KPListImport", "CheckKPListName", { ListName: ListName }, function (e) {
                    if (e.Status == "Pass") {
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
            title: 'List Name'
        }, {
            field: 'SKUNO',
            title: 'SkuNo'
        }, {
            field: 'EDIT_TIME',
            title: 'Upload Time'
        }, {
            field: 'EDIT_EMP',
            title: 'Upload By'
        }],
        data: KeyPartData
    });

    Get_data();
});