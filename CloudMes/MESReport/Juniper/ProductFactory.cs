using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Juniper
{
    public class ProductFactory : ReportBase
    {
        ReportInput inputSn = new ReportInput() { Name = "SN", InputType = "TXT", Value = "", Enable = false, SendChangeEvent = false, ValueForUse = null };

        public ProductFactory()
        {
            Inputs.Add(inputSn);
        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {
            string SN = inputSn.Value.ToString();
            string sqlRun = string.Empty;
            DataTable snListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;

            OleExec SFCDB = DBPools["SFCDB"].Borrow();

            try
            {
                string[] ids = SN.Split(new char[] { ',' });
                SN = SN.Replace(",", "','");
                SN = SN.Replace(" ", "");

                sqlRun = $@"SELECT SS.PONO, SS.POLINE, SS.SKUNO, SS.MPN, SS.WORKORDERNO, SS.SN, SS.FINALASN, SS.FINALASNTIME, MAX(SS.RN) COUNTQTY FROM (
                                SELECT RSN.SKUNO, RSK.MPN, RSN.WORKORDERNO, RSN.SN, OOM.PONO, OOM.POLINE, OOM.FINALASN, OOM.FINALASNTIME, ROW_NUMBER()OVER(
                                    PARTITION BY RSN.SN ORDER BY RSN.WORKORDERNO
                                ) AS RN FROM R_SN RSN, R_SN_KP RSK, O_ORDER_MAIN OOM, R_I139 RI WHERE OOM.FINALASN = RI.ASNNUMBER AND RSN.WORKORDERNO = OOM.PREWO
                                AND ASNNUMBER LIKE 'ACT%' AND RSN.SN IN('{SN}') AND RSK.SN = RSN.SN
                                AND RSK.KP_NAME = 'AutoKP' GROUP BY RSN.SN, RSK.MPN, RSN.WORKORDERNO, RSN.SKUNO, OOM.PONO, OOM.POLINE, OOM.FINALASN, OOM.FINALASNTIME) SS
                            GROUP BY SS.SN, SS.WORKORDERNO, SS.SKUNO, SS.MPN, SS.PONO, SS.POLINE, SS.FINALASN, SS.FINALASNTIME, SS.RN
                            ORDER BY SS.PONO, SS.POLINE, SS.SKUNO, SS.WORKORDERNO, SS.SN";
                RunSqls.Add(sqlRun);
                snListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                linkTable.Columns.Add("PONO");
                linkTable.Columns.Add("POLINE");
                linkTable.Columns.Add("SKUNO");
                linkTable.Columns.Add("MPN");
                linkTable.Columns.Add("WORKORDERNO");
                linkTable.Columns.Add("SN");
                linkTable.Columns.Add("FINALASN");
                linkTable.Columns.Add("FINALASNTIME");
                linkTable.Columns.Add("COUNTQTY");
                for (int i = 0; i < snListTable.Rows.Count; i++)
                {
                    linkRow = linkTable.NewRow();
                    linkRow["PONO"] = "";
                    linkRow["POLINE"] = "";
                    linkRow["SKUNO"] = "";
                    linkRow["MPN"] = "";
                    linkRow["WORKORDERNO"] = "";
                    linkRow["SN"] = "";
                    linkRow["FINALASN"] = "";
                    linkRow["FINALASNTIME"] = "";
                    linkRow["COUNTQTY"] = "";
                    linkTable.Rows.Add(linkRow);
                }
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(snListTable, linkTable);
                reportTable.Tittle = "SNList";
                //reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);

            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
