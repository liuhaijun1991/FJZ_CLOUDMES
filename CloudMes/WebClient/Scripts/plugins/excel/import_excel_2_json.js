///-----------USE INSTRUCTION--
//  INCLUDE THESE LIBRARIES TO YOUR WEBPAGE 
            //<link href="../css/plugins/bootstrap/bootstrap.min.css" rel="stylesheet" />
            //<link href="../css/plugins/bootstrapTable/bootstrap-table.min.css" rel="stylesheet" />
            //<script src="../Scripts/jquery-1.10.2.min.js"></script>
            //<script src="../Scripts/plugins/excel/xlsx.full.min.js"></script>
            //<script src="../Scripts/plugins/bootstrap/bootstrap.min.js"></script>
            //<script src="../Scripts/plugins/bootstrapTable/bootstrap-table.min.js"></script>
            //<script src="../Scripts/plugins/bootstrapTable/bootstrap-table-zh-TW.min.js"></script>

//----- EXAMPLE: USE THIS STRUCT  IN YOUR WEBPAGE 
    //  <div>
    //    <span><strong> Excel file</strong>  </span> <input type="file" accept=".xlsx,.xlsm,.xlsb,.xls,.xltx,.xltm,.xlt,.xlam,.xla"  class="input_excel" style="display:inline-block;" />
    //    <span class="form-control"><span class="ntf"></span></span>
    //    <div  class="jumbotron div_js" style="color:red; display:none;">
    //        <!--data of excel will display here with json string format-->

    //    </div>
    //    <div id="table_box1" style="display:none">   </div>
    //    <div class="table_box" style="overflow:scroll; display:none;">
    //        <!--data of excel will display here with  table format-->
    //    </div>
    //    <table  class="boostraptable_dvt">
    //        <!--data of excel will display here with  boostraptable format  : YOU HAVE TO DEFINE STRUCT OF YOUR BOOSTRAP  TABLE BEFORE
    //    </table>

    //</div>
    
var JS_array = [];
  

function reset() {
    $(".table_box").html('');
    $("#table_box1").html('');
    $(".div_js").html('');
    $(".ntf").html('');
}
$(document).ready(function () {
    var HTMLOUT = document.getElementById("table_box1");
    $('.input_excel').change(function (e) {

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
                var count = 0;
                $(first_sheet).children('tr').eq(0).children('td').each(function () {
                    if ($(this).text().trim() == "") {
                        count++;
                    }
                       
                });
                if (count > 0) {
                    $(".ntf").text("Data import fail");
                    $(".table_box").html('');
                    alert("Warning > False format inside content of excel file > Title of columns includes one or some cell empty ");
                }
                else {
                    var arrray1 = [];
                    var arrray2 = [];
                    var arrray3 = [];
                    var arrray4 = [];
                    var row_modal = "";
                    var other_rows = "";
                    var first_row = "";
                    var cell = "";
                    $(first_sheet).children('tr').eq(0).children('td').each(function () {
                        var zz = $(this).text().trim().toUpperCase();
                        var zz1 = "";
                        for (var i = 0; i < zz.length; i++) {
                            if (zz[i].trim() != "") { zz1 += zz[i]; }
                        }
                        cell += '<td>' + zz1 + '</td>';

                    });
                    first_row = '<tr>' + cell + '</tr>';
                    for (var i = 0; i < colum_qty; i++)
                        row_modal += '<td></td>';

                    console.log(row_modal);
                    $(first_sheet).children('tr').each(function () {
                        arrray1.push($(this).html());

                    });

                    for (var i = 1; i < arrray1.length; i++) {
                        if (arrray1[i] != row_modal) { arrray2.push(arrray1[i]); }
                    }

                    for (var i = 0; i < arrray2.length; i++) {
                        arrray2[i] = '<tr>' + arrray2[i] + '</tr>';

                    }

                    /////////////////////////////////////
                    for (var i = 0; i < colum_qty; i++) {
                        arrray3.push($(first_row).children('td').eq(i).text());
                    }

                    var substr1 = '[';
                    for (var i = 0; i < arrray2.length; i++) {
                        var count3 = -1;
                        var substr = '{';
                        $(arrray2[i]).children('td').each(function () {
                            count3++;
                            if (count3 == colum_qty - 1) {
                                var xx = $(this).text().trim();
                                //if (xx == "") { substr += '\"' + arrray3[count3] + '\":\"' + xx + '\"}'; }
                                //else
                                //    if (isFinite(String(xx))) { substr += '\"' + arrray3[count3] + '\":' + xx + '}'; }
                                //    else
                                        substr += '\"' + arrray3[count3] + '\":\"' + xx + '\"}';
                            }
                            else {
                                var xx1 = $(this).text().trim();
                                //if (xx1 == "") { substr += '\"' + arrray3[count3] + '\":\"' + xx1 + '\",'; }
                                //else
                                //    if (isFinite(String(xx1))) { substr += '\"' + arrray3[count3] + '\":' + xx1 + ','; }
                                //    else
                                        substr += '\"' + arrray3[count3] + '\":\"' + xx1 + '\",';
                            }

                              
                        });
                        if (i == arrray2.length - 1) { substr1 += substr + ']'; break; }
                        else substr1 += substr + ',';    
                         
                    }
                     JS_array = JSON.parse(substr1);
                     console.log(JSON.parse(substr1));
                    //////////////////////AUTO GENERAL NAME OF COLUMNS IN BOOSTRAP TABLE////////////////////
                     $(".div_js").text(substr1);
                     //var columns_list = [
                     //                      {
                     //                          field: 'select',
                     //                          title: 'select',
                     //                          checkbox: true
                     //                      }
                     //                   ];
                     
                     //for (var aa = 0; aa < arrray3.length; aa++)
                     //    {
                     //            var test_obj = {
                     //                                field: arrray3[aa],
                     //                                title: arrray3[aa],
                     //                                align: 'center',
                     //                                sortable: true
                     //                            };
                     //            columns_list.push(test_obj);
                     //    }
 
                     //$(".boostraptable_dvt").bootstrapTable({
                     //    showColumns: true,
                     //    showRefresh: true,
                     //    striped: true,
                     //    cache: false,
                     //    pagination: true,
                     //    sidePagination: "client",
                     //    pageNumber: 1,
                     //    pageSize: 5,
                     //    pageList: [5, 15, 30, 70],
                     //    search: true,
                     //    strictSearch: true,
                     //    searchOnEnterKey: false,
                     //    clickToSelect: true,
                     //    minimumCountColumns: 3,
                     //    showToggle: true,
                     //    cardView: false,
                     //    detailView: false,
                     //    dataType: "json",
                     //    method: "post",
                     //    searchAlign: "right",
                     //    buttonsAlign: "left",
                     //    toolbarAlign: "right",
                     //    columns: columns_list,
                     //    locale: "zh-CN"
                     //});   

                   


                      

                     $(".boostraptable_dvt").bootstrapTable('load', JSON.parse(substr1));     // load data from excel to bootstrapTable here
                    //////////////////////////////////////////////
                  
                    for (var i = 0; i < arrray2.length; i++) {
                        first_row += arrray2[i];
                    }
                    console.log(first_row);
                    $(".table_box").html('<table border="1" class="table table-responsive" style="text-align:center;">' + first_row + '</table>');

                    $(".ntf").text("Data import success");
                }
            }
        }
        else {
            reset();
            alert('Please select excel file with xlsx/xlsm/xlsb/xls/xltx/xltm/xlt/xlam/xla formats');
        }
    });


});