using System;
using System.Data;
using MESDBHelper;
using SqlSugar;
using MESDataObject.Module;
using System.Collections.Generic;

namespace MESReport.Juniper
{
    
    class CheckAPUnitTraceability : ReportBase
    {
        ReportInput WO = new ReportInput { Name = "WO", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = true };
        ReportInput SIDE = new ReportInput { Name = "SIDE", InputType = "Select", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "BOTTOM", "TOP" }, EnterSubmit = true };

        public CheckAPUnitTraceability()
        {
            Inputs.Add(WO);
            Inputs.Add(SIDE);
        }

        public override void Run()
        {
            OleExec apdb = DBPools["APDB"].Borrow();
            OleExec sfcdb = DBPools["SFCDB"].Borrow();
            string strProcess = null;
            string strWO = WO.Value.ToString();
            string strSide = SIDE.Value.ToString();
            List<string> listSN = new List<string>();

            if (strSide == "BOTTOM")
            {
                strProcess = "AOI2";
            }
            else
            {
                strProcess = "AOI4";
            }
            if (string.IsNullOrEmpty(strWO))
            {
                throw new Exception("WO cannot be null");
            }
            try
            {
                #region Get all SN from WO
                listSN = sfcdb.ORM.Queryable<R_SN>().Where(s => s.WORKORDERNO == strWO)
                    .Select(s => s.SN).ToList();
                #endregion

                if(listSN.Count == 0)
                {
                    throw new Exception($@"NO Serial Numbers Associated to WO {strWO}.");
                }

                DataTable statusTable = new DataTable();
                ReportTable repTab = new ReportTable();
                statusTable.Columns.Add("SN");
                statusTable.Columns.Add("AUTOSCANNER STATUS");
                statusTable.Columns.Add("SIDE");

                #region All Parts Stored Procedure
                foreach (string strSN in listSN)
                {
                    var G_PSN = new SugarParameter("@G_PSN", strSN);
                    var G_EVENT = new SugarParameter("@G_EVENT", strProcess); //isOutput=true
                    var RES = new SugarParameter("@RES", null, true);
                    var excDt = apdb.ORM.Ado.UseStoredProcedure().GetDataTable("MES1.CHECK_PSN_MATERIAL_TOP", G_PSN, G_EVENT, RES);
                    string strStatus = RES.Value.ToString();
                    var newRow = statusTable.NewRow();
                    newRow["SN"] = strSN;
                    newRow["AUTOSCANNER STATUS"] = strStatus;
                    newRow["SIDE"] = strSide;
                    statusTable.Rows.Add(newRow);
                }
                #endregion
                repTab.LoadData(statusTable);
                repTab.Tittle = "Autoscanner Unit Status for WO "+ strWO;
                Outputs.Add(repTab);
            }
            catch (Exception ee)
            {

            }
            finally
            {
                DBPools["SFCDB"].Return(apdb);


            }
        }
    }
}
