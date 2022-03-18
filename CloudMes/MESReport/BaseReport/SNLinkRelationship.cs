using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MESDBHelper;

namespace MESReport.BaseReport
{
    class SNLinkRelationship : ReportBase
    {
        //ReportInput Wo = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput SN = new ReportInput { Name = "SN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public SNLinkRelationship()
        {
            //Inputs.Add(Wo);
            Inputs.Add(SN);
        }

        public override void Init()
        {
            base.Init();
        }
        public override void Run()
        {
            //base.Run();
            string sn = SN.Value.ToString();
            string sqlLink1 = "";
            string sqlLink = "";
            string sqlkp = "";
            //string sql1 = "";

            if (sn == "")
            {
                ReportAlart alart = new ReportAlart("Please input a sn ");
                Outputs.Add(alart);
                return;
            }

            sqlLink1 = $@" select sn,value,partno,kp_name,mpn,scantype,itemseq,
                                scanseq,detailseq,station,valid_flag,edit_time from  r_sn_kp 
                                 where 1=1 AND SN='{sn}'";

            sqlLink = $@"select SN,SKUNO,WORKORDERNO,PLANT,CLASS_NAME,ROUTE_ID,LINE,STARTED_FLAG,PACKED_FLAG,PACKED_TIME,COMPLETED_FLAG,SHIPPED_FLAG,
                        CURRENT_STATION,NEXT_STATION,REWORK_COUNT,EDIT_TIME
                        from r_sn_station_detail where 1=1 AND sn in ( 
                        select value from r_sn_kp where sn='{sn}' ) order by EDIT_TIME";

            sqlkp = $@"select  SN,SKUNO,WORKORDERNO,PLANT,CLASS_NAME,ROUTE_ID,LINE,STARTED_FLAG,PACKED_FLAG,PACKED_TIME,COMPLETED_FLAG,SHIPPED_FLAG,
                        CURRENT_STATION,NEXT_STATION,REWORK_COUNT,EDIT_TIME from r_sn_station_detail where 1=1 AND sn in ( 
                        SELECT VALUE FROM R_SN_KP WHERE SN IN (
                        SELECT VALUE FROM R_SN_KP WHERE SN='{sn}')) order by EDIT_TIME";


            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            DataTable dtLink = new DataTable();
            DataTable dtNoLnk = new DataTable();
            DataTable dtsqlkp = new DataTable();
            DataSet res = sfcdb.RunSelect(sqlLink1);
            //DataSet res1 = sfcdb.RunSelect(sqlLink);
            //DataSet res2 = sfcdb.RunSelect(sqlkp);
            try
            {
                dtNoLnk = sfcdb.RunSelect(sqlLink1).Tables[0];
                dtLink = sfcdb.RunSelect(sqlLink).Tables[0];
                dtsqlkp = sfcdb.RunSelect(sqlkp).Tables[0];
                //if (sfcdb != null)
                //{
                //    DBPools["SFCDB"].Return(sfcdb);
                //}
                DataTable linkTable = new DataTable();
                DataRow linkRow;
                foreach (DataColumn column in res.Tables[0].Columns)
                {
                    linkTable.Columns.Add(column.ColumnName);
                }
                foreach (DataRow row in res.Tables[0].Rows)
                {
                    string linkURL = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + row["SN"].ToString();
                    linkRow = linkTable.NewRow();
                    foreach (DataColumn dc in linkTable.Columns)
                    {
                        if (dc.ColumnName.ToString().ToUpper() == "SN")
                        {
                            linkRow[dc.ColumnName] = linkURL;
                        }
                        else
                        {
                            linkRow[dc.ColumnName] = "";
                        }
                    }
                    linkTable.Rows.Add(linkRow);
                }


                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dtNoLnk, linkTable);
                reportTable.Tittle = "SN  link detail";
                Outputs.Add(reportTable);

                ReportTable retTab2 = new ReportTable();
                retTab2.LoadData(dtLink, null);
                retTab2.Tittle = "PCBA 1 link detail";
                Outputs.Add(retTab2);

                ReportTable retTab3 = new ReportTable();
                retTab3.LoadData(dtsqlkp, null);
                retTab3.Tittle = "PCBA 2 link detail";
                Outputs.Add(retTab3);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
                return;
            }
            finally
            {
                if (sfcdb != null)
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }
    }
}
