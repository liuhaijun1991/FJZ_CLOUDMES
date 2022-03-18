using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace MESReport.BaseReport
{
    public class SkuWipByStation : ReportBase
    {
        ReportInput inputStations = new ReportInput { Name = "Stations", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputType = new ReportInput { Name = "TYPE", InputType = "Select", Value = "PCBA", Enable = true, SendChangeEvent = false, ValueForUse = new string[] { "PCBA", "MODEL", "CTO", "OPTICS", "DOF", "VIRTUAL" } };
        [JsonIgnore]
        [ScriptIgnore]
        public OleExec SFCDB = null;
        public SkuWipByStation()
        {
            this.Inputs.Add(inputStations);
            this.Inputs.Add(inputType);
        }
        public override void Run()
        {
            string sql = "";
            if (inputStations.Value == null || inputStations.Value.ToString() == "")
            {
                sql = "SELECT *\n" +
                "  FROM (select SKUNO, NEXT_STATION, COUNT(1) QTY\n" +
                "          from r_sn A\n" +
                "         where next_station in ('PRESS-FIT', '5DX', 'FQA', 'ICT', 'STOCKIN')\n" +
                "           AND A.VALID_FLAG = 1\n" +
                "           AND A.REPAIR_FAILED_FLAG = 0\n" +
                "           AND A.SKUNO IN\n" +
                "               (SELECT SKUNO FROM C_SKU WHERE SKU_TYPE IN ('{0}'))\n" +
                "         GROUP BY SKUNO, NEXT_STATION) B\n" +
                "PIVOT(SUM(QTY)\n" +
                "   FOR NEXT_STATION IN('PRESS-FIT' as \"PRESS-FIT\",\n" +
                "                       '5DX' as \"5DX\",\n" +
                "                       'FQA' as \"FQA\",\n" +
                "                       'ICT' as \"ICT\",\n" +
                "                       'STOCKIN' as \"STOCKIN\"))";

            }
            else
            {
                var stationArr = inputStations.Value.ToString().Split(',');
                var stations = "";
                var StationPrivot = "";
                for (int i = 0; i < stationArr.Length; i++)
                {
                    stations += "'" + stationArr[i] + "',";
                    StationPrivot += "'" + stationArr[i] + "' as \"" + stationArr[i] + "\",";
                }
                stations = stations.Substring(0, stations.Length - 1);
                StationPrivot = StationPrivot.Substring(0, StationPrivot.Length - 1);
                sql = "SELECT *\n" +
                "  FROM (select SKUNO, NEXT_STATION, COUNT(1) QTY\n" +
                "          from r_sn A\n" +
                "         where next_station in (" + stations + ")\n" +
                "           AND A.VALID_FLAG = 1\n" +
                "           AND A.REPAIR_FAILED_FLAG = 0\n" +
                "           AND A.SKUNO IN\n" +
                "               (SELECT SKUNO FROM C_SKU WHERE SKU_TYPE IN ('" + inputType.Value + "'))\n" +
                "         GROUP BY SKUNO, NEXT_STATION) B\n" +
                "PIVOT(SUM(QTY)\n" +
                "   FOR NEXT_STATION IN(" + StationPrivot + "))";
            }

            bool isborrow = false;
            if (SFCDB == null)
            {
                SFCDB = DBPools["SFCDB"].Borrow();
                isborrow = true;
            }
            try
            {
                RunSqls.Add(sql);
                DataTable res = SFCDB.RunSelect(sql).Tables[0];
                ReportTable report = new ReportTable();
                report.LoadData(res);
                report.pagination = false;
                report.FixedHeader = true;
                report.Tittle = "Station WIP";
                Outputs.Add(report);

            }
            catch (Exception e)
            {

                ReportAlart alart = new ReportAlart(e.ToString());
                Outputs.Add(alart);
                return;
            }
            finally
            {
                if (isborrow)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
            }

        }
    }
}
