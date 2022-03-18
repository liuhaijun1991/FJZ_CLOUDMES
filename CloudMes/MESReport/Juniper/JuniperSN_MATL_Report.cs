using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class JuniperSN_MATL_Report : ReportBase
    {
        ReportInput inputSearch = new ReportInput() { Name = "Type", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "", "TRSN", "PN", "SN", "WO" } };
        ReportInput inputValue = new ReportInput() { Name = "Value", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        /*ReportInput inputTRSN = new ReportInput() { Name = "TRSN", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputWO = new ReportInput() { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };*/

        public JuniperSN_MATL_Report()
        {
            Inputs.Add(inputSearch);
            Inputs.Add(inputValue);
            /*Inputs.Add(inputTRSN);
            Inputs.Add(inputWO);*/
        }
        public override void Init()
        {
            base.Init();
        }
        public override void Run()
        {
            try
            {
                /*base.Run();
                DataTable dt = new DataTable();
                string runSql = "";
                DateTime preStartTime = DateTime.Now, preEndTime = DateTime.Now, finalStartTime = DateTime.Now, finalEndTime = DateTime.Now;

                string trsn = inputTRSN.Value.ToString();
                string wo = inputWO.Value.ToString();
                if (trsn != "")
                {

                    var t_trsn = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == trsn && t.KP_NAME == "TR_SN" && t.VALID_FLAG == 1).ToDataTable();

                    ReportTable reportTable = new ReportTable();
                    reportTable.LoadData(t_trsn, null);
                    reportTable.Tittle = "TRSN DATA";
                    Outputs.Add(reportTable);

                    var t_SN = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == trsn && t.KP_NAME == "LINK_TR_SN" && t.VALID_FLAG == 1).ToDataTable();
                    ReportTable reportTable1 = new ReportTable();
                    reportTable1.LoadData(t_SN, null);
                    reportTable1.Tittle = "SN DATA";
                    Outputs.Add(reportTable1);
                }
                if (wo != "")
                {
                    var sns = sfcdb.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == wo).Select(t => t.SN).ToList();
                    var trsns = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => sns.Contains(t.SN) && t.KP_NAME == "LINK_TR_SN" && t.VALID_FLAG == 1).Select(t => t.VALUE).Distinct().ToList();

                    var t_trsn = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => trsns.Contains(t.SN) && t.KP_NAME == "TR_SN" && t.VALID_FLAG == 1).ToDataTable();
                    ReportTable reportTable = new ReportTable();
                    reportTable.LoadData(t_trsn, null);
                    reportTable.Tittle = "TRSN DATA";
                    Outputs.Add(reportTable);

                    var t_SN = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => sns.Contains(t.SN) && t.KP_NAME == "LINK_TR_SN" && t.VALID_FLAG == 1).ToDataTable();
                    ReportTable reportTable1 = new ReportTable();
                    reportTable1.LoadData(t_SN, null);
                    reportTable1.Tittle = "SN DATA";
                    Outputs.Add(reportTable1);
                }*/

                OleExec sfcdb = DBPools["SFCDB"].Borrow();
                string type = inputSearch.Value.ToString();
                string value = inputValue.Value.ToString().ToUpper().Trim();
                List<string> sns = null, trsns = null;

                switch (type)
                {
                    case ("TRSN"):
                        break;
                    case ("PN"):
                        break;
                    case ("SN"):
                        trsns = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.SN == value && t.KP_NAME == "LINK_TR_SN" && t.VALID_FLAG == 1).Select(t => t.VALUE).Distinct().ToList();
                        break;
                    case ("WO"):
                        sns = sfcdb.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == value).Select(t => t.SN).ToList();
                        trsns = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => sns.Contains(t.SN) && t.KP_NAME == "LINK_TR_SN" && t.VALID_FLAG == 1).Select(t => t.VALUE).Distinct().ToList();
                        break;
                    default:
                        throw new Exception("Please select an input type!");
                }

                if (string.IsNullOrEmpty(value))
                {
                    throw new Exception("Please input a value!");
                }

                var t_trsn = sfcdb.ORM.Queryable<R_SN_KP>()
                    .WhereIF(type == "TRSN", t => t.SN == value && t.KP_NAME == "TR_SN")
                    .WhereIF(type == "PN", t => t.PARTNO == value && t.KP_NAME == "TR_SN")
                    .WhereIF(type == "SN", t => trsns.Contains(t.SN) && t.KP_NAME == "TR_SN")
                    .WhereIF(type == "WO", t => trsns.Contains(t.SN) && t.KP_NAME == "TR_SN").ToDataTable();
                

                var t_sn = sfcdb.ORM.Queryable<R_SN_KP>()
                    .WhereIF(type == "TRSN", t => t.VALUE == value && t.KP_NAME == "LINK_TR_SN")
                    .WhereIF(type == "PN", t => t.PARTNO == value && t.KP_NAME == "LINK_TR_SN")
                    .WhereIF(type == "SN", t => t.SN == value && t.KP_NAME == "LINK_TR_SN")
                    .WhereIF(type == "WO", t => sns.Contains(t.SN) && t.KP_NAME == "LINK_TR_SN").ToDataTable();
                

                try
                {
                    ReportTable reportTableTRSN = new ReportTable();
                    reportTableTRSN.LoadData(t_trsn, null);
                    reportTableTRSN.Tittle = "TRSN DATA";
                    Outputs.Add(reportTableTRSN);

                    ReportTable reportTableSN = new ReportTable();
                    reportTableSN.LoadData(t_sn, null);
                    reportTableSN.Tittle = "SN DATA";
                    Outputs.Add(reportTableSN);
                }
                catch(Exception ee)
                {
                    throw ee;
                }
                finally
                {
                    DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
        }
    }
}