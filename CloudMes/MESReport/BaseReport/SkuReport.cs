using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
namespace MESReport.BaseReport
{
    public class SkuReport : ReportBase
    {
        ReportInput Skuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput CloseFlag = new ReportInput { Name = "CloseFlag", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        ReportInput Series = new ReportInput { Name = "Series", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        public SkuReport()
        {

            Inputs.Add(Skuno);
            Inputs.Add(CloseFlag);
            Inputs.Add(Series);
        
            //  string strGetSn = @"SELECT * FROM R_SN WHERE SN='{0}' OR BOXSN='{0}'";
            //   Sqls.Add("strGetSN", strGetSn);
        }
        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                InitSeries(SFCDB);
                //DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public override void Run()
        {
   
            DataRow linkDataRow = null;
            string closeflag = CloseFlag.Value.ToString();
            string skuno = Skuno.Value.ToString();
            string series = Series.Value.ToString();
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string _Sqlsku = $@"SELECT SKUNO FROM C_SKU WHERE C_SERIES_ID IN (
                                SELECT ID FROM C_SERIES  WHERE CUSTOMER_ID IN(
                                SELECT ID FROM C_CUSTOMER WHERE CUSTOMER_NAME = '{series}'))";


            try
            {
                string Sqlsku = $@"select workorderno,RELEASE_DATE,wo_type,skuno,sku_ver,workorder_qty from r_wo_base a,r_wo_type b where 1=1 and substr(a.workorderno,0,6) =b.PREFIX  ";
                if (skuno != "ALL" && skuno !="") {

                    Sqlsku = Sqlsku + $@" and a.skuno = '{skuno}' ";
                }
                if (closeflag == "Y")
                {
                    Sqlsku = Sqlsku + " and a.CLOSED_FLAG = 1";
                }
                else if (closeflag == "N")
                {
                    Sqlsku = Sqlsku + " and a.CLOSED_FLAG = 0";
                }
               
                if (series != "ALL" && series != "")
                {
                    Sqlsku = Sqlsku + $@" and a.SKUNO  IN ({_Sqlsku})";
                }

                Sqlsku = Sqlsku + " order by a.skuno asc,a.sku_ver asc,a.RELEASE_DATE desc ";

                DataTable dtsku = SFCDB.RunSelect(Sqlsku).Tables[0];
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                if (dtsku.Rows.Count == 0)
                {
                    ReportAlart alart = new ReportAlart("No Data!");
                    Outputs.Add(alart);
                    return;
                }

                DataTable linkTable = new DataTable();

                linkTable.Columns.Add("workorderno");
                linkTable.Columns.Add("RELEASE_DATE");
                linkTable.Columns.Add("wo_type");
                linkTable.Columns.Add("skuno");
                linkTable.Columns.Add("sku_ver");
                linkTable.Columns.Add("workorder_qty");



                for (int i = 0; i < dtsku.Rows.Count; i++)
                {
                    linkDataRow = linkTable.NewRow();
                    //跳轉的頁面鏈接
                    linkDataRow["workorderno"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.WoReport&RunFlag=1&WO=" + dtsku.Rows[i]["workorderno"].ToString() + "&EventName=";
                    linkDataRow["wo_type"] = "";
                    linkDataRow["skuno"] = "";
                    linkDataRow["sku_ver"] = "";
                    linkDataRow["workorder_qty"] = "";
                    linkTable.Rows.Add(linkDataRow);

                }
                ReportTable retTab = new ReportTable();
                retTab.LoadData(dtsku, linkTable);
                retTab.Tittle = "Skuno Report";
                Outputs.Add(retTab);
            }
            catch (Exception exception)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                throw exception;
            }
        }

        public void InitSeries(OleExec db)
        {

            
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            dt = GetALLSeries(db);
            station.Add("ALL");
            foreach (DataRow dr in dt.Rows)
            {
                station.Add(dr["CUSTOMER_NAME"].ToString());

            }
            Series.ValueForUse = station;

        
        }

        public DataTable GetALLSeries(OleExec db)
        {
            List<string> station = new List<string>();
            DataTable dt = new DataTable();
            string sql = $@"SELECT CUSTOMER_NAME  FROM C_CUSTOMER";
            dt = db.ExecSelect(sql).Tables[0];
            return dt;
        }
    }
}
