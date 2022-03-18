using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESReport.Juniper
{
    //SN 信息報表
    public class R_JNP_PD_KIT_MAIN_Report : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true }; 

        public R_JNP_PD_KIT_MAIN_Report()
        { 
            Inputs.Add(WO); 
        }

        public override void Init()
        {
            base.Init(); 
        }

        public override void Run()
        {

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                
                //string sql = $@"select  SKUNO, WO, PARTNO,   REQUESTQTY, SCANQTY, EDIT_TIME, EDIT_BY 
                //       from     R_JNP_PD_KIT_MAIN WHERE WO ='{WO.Value}' and VALID_FLAG ='1' ";

                string sql = $@" select SKUNO, WO, PARTNO, REQUESTQTY,COUNT(SN) SCANQTY
                                      from R_JNP_PD_KIT_DETAIL
                                     WHERE WO = '{WO.Value}'
                                       and VALID_FLAG = '1'
                                     group by SKUNO, WO, PARTNO,REQUESTQTY ";

               


                DataSet res = SFCDB.RunSelect(sql);
                ReportTable retTab = new ReportTable();

                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string DetailURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.Juniper.R_JNP_PD_KIT_DETAIL_Report&RunFlag=1&WO=" + row["WO"].ToString()+ "&PARTNO=" + row["PARTNO"].ToString();

                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SCANQTY")
                        {
                            linkRow[dc.ColumnName] = DetailURL;
                        }                       
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }

                retTab.LoadData(res.Tables[0], linkTable);
                retTab.Tittle = "R_JNP_PD_KIT_MAIN";
                Outputs.Add(retTab);
            }
            catch (Exception e)
            {
                Outputs.Add(new ReportAlart(e.ToString()));
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }

        } 
    }
}
