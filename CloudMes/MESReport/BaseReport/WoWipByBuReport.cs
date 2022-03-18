using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject;
using MESDataObject.Module;

namespace MESReport.BaseReport
{
    //工單WIPByBU

    public  class WoWipByBuReport: ReportBase
    {
        ReportInput Bu = new ReportInput { Name = "Bu", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "HWT", "DCN" } };
        ReportInput ModelType = new ReportInput { Name = "ModelType", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "PCBA", "MODEL" } };
        ReportInput Model = new ReportInput { Name = "Model", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "ALL", "N", "Y" } };
        public WoWipByBuReport()
        {
            Inputs.Add(Bu);
            Inputs.Add(ModelType);
            Inputs.Add(Model);
        }

        public override void Init()
        {
            OleExec SFCDB = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                InitBu(SFCDB);


                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
}

        public override void Run()
        {
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            string strbu = Bu.Value.ToString();
            string skunotype = ModelType.Value.ToString();
            string strskuno = Model.Value.ToString();
            
            try
            {
                string SQLSku = $@"select * from C_SKU where bu='{strbu}' ";
                if (skunotype != "ALL")
                {
                    SQLSku = SQLSku + $@" and sku_type = '{ skunotype}'";
                }
                if (strskuno != "ALL")
                {
                    SQLSku = SQLSku + $@"and and skuno = '{ strskuno}'";
                }
                DataTable dtskuo = SFCDB.RunSelect(SQLSku).Tables[0];

                foreach (DataRow dr in dtskuo.Rows)
                {
                    string Sqlwo = $@"SELECT workorderno ,WORKORDER_QTY , trunc ( sysdate - DOWNLOAD_DATE) DATS,INPUT_QTY,FINISHED_QTY FROM R_WO_BASE where 
                           CLOSED_FLAG=0 and skuno = '{dr["skuno"].ToString()}' ";

                    DataTable dtwo = SFCDB.RunSelect(Sqlwo).Tables[0];
                    if (dtwo.Rows.Count == 0)
                    {
                        continue;
                    }
                    //string SqlRoute = $@"select * from C_ROUTE_DETAIL where route_id='{dtwo.Rows[0]["route_id"].ToString()}' order by s.eventseqno";
                    //DataTable dtroute = SFCDB.RunSelect(SqlRoute).Tables[0];
                    DataTable resdt = new DataTable();
                    resdt.Columns.Add("WorkOrderNo");
                    resdt.Columns.Add("DATS");
                    resdt.Columns.Add("QTY");
                    //for (int i = 0; i < resdt.Rows.Count; i++)
                    //{
                    //    resdt.Columns.Add(resdt.Rows[i]["STATION_NAME"].ToString());

                    //}
                    resdt.Columns.Add("STOCKIN");
                    resdt.Columns.Add("RepairWip");
                    resdt.Columns.Add("MRB");
                    resdt.Columns.Add("REWORK");
                    resdt.Columns.Add("JOBFINISH");
                    foreach (DataRow wo in dtwo.Rows)
                    {
                        DataRow drd = resdt.NewRow();
                        drd["WorkOrderNo"] = wo["workorderno"].ToString();
                        drd["DATS"] = wo["DATS"].ToString();
                        drd["QTY"] = wo["WORKORDER_QTY"].ToString();
                        drd["STOCKIN"] = wo["FINISHED_QTY"].ToString();
                        string Sqlsncount = $@" select NEXT_STATION, count(NEXT_STATION)c from r_sn where REPAIR_FAILED_FLAG <> 1 and(COMPLETED_FLAG = 0 or NEXT_STATION = 'JOBFINISH') 
                                         and  workorderno = '{wo["workorderno"].ToString()}' group by NEXT_STATION";

                        DataTable dtsncont = SFCDB.RunSelect(Sqlsncount).Tables[0];

                        for (int i = 0; i < dtsncont.Rows.Count; i++)
                        {
                            resdt.Columns.Add(dtsncont.Rows[i]["NEXT_STATION"].ToString());
                            drd[dtsncont.Rows[i]["NEXT_STATION"].ToString()] = dtsncont.Rows[i]["c"].ToString();
                        }

                        string SqlRepairCount = $@" select count(1) repaircount from r_sn where REPAIR_FAILED_FLAG = 1 and workorderno = '{wo["workorderno"].ToString()}'";
                        DataTable dtrepaircont = SFCDB.RunSelect(SqlRepairCount).Tables[0];
                        drd["RepairWip"] = dtrepaircont.Rows[0]["repaircount"].ToString();

                        string SqlMrbCount = $@"select count(1) mrbcount from r_mrb where workorderno = '{wo["workorderno"].ToString()}' and rework_wo is null";
                        DataTable dtmrbcont = SFCDB.RunSelect(SqlMrbCount).Tables[0];
                        drd["MRB"] = dtmrbcont.Rows[0]["mrbcount"].ToString();
                        resdt.Rows.Add(drd);
                        ReportTable retTab = new ReportTable();
                        retTab.LoadData(resdt, null);
                        Outputs.Add(retTab);
                    }
                }
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
                if (SFCDB != null) DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void InitBu(OleExec db)
        {
            DataTable dt = new DataTable();
            List<string> allbu = new List<string>();
            T_C_BU bu = new T_C_BU(db, DB_TYPE_ENUM.Oracle);
            dt = bu.GetAllBu(db);
            foreach (DataRow dr in dt.Rows)
            {
                allbu.Add(dr["BU"].ToString());
            }
            Bu.ValueForUse = allbu;
        }
    }
}
