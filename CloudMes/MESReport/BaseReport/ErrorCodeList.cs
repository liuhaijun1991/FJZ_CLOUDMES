using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    public class ErrorCodeList : ReportBase
    {
        ReportInput inputErrorCode = new ReportInput() { Name = "Code", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputStation = new ReportInput() { Name = "Station", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };
        
        public ErrorCodeList()
        {
            Inputs.Add(inputErrorCode);
            Inputs.Add(inputStation);

            string sqlGetStation = $@"SELECT DISTINCT C.NEXT_STATION AS STATION_NAME FROM SFCRUNTIME.R_SN_STATION_DETAIL C WHERE EXISTS (
                SELECT 1 FROM SFCRUNTIME.R_STATION WHERE (FAIL_STATION_FLAG = 1 OR FAIL_STATION_ID IS NOT NULL) AND C.NEXT_STATION = STATION_NAME) ORDER BY STATION_NAME";
            Sqls.Add("GetStation", sqlGetStation);
        }

        public override void Init()
        {
            base.Init();

            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            DataTable dtStation = SFCDB.RunSelect(Sqls["GetStation"]).Tables[0];

            if (SFCDB != null)
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
            List<string> stationList = new List<string>();
            stationList.Add("ALL");
            foreach (DataRow row in dtStation.Rows)
            {
                stationList.Add(row["STATION_NAME"].ToString());
            }
            inputStation.ValueForUse = stationList;
        }

        public override void Run()
        {
            try
            {
                base.Run();
                string errorCode = inputErrorCode.Value == null ? inputErrorCode.Value.ToString() : inputErrorCode.Value.ToString().Trim().ToUpper();
                string station = inputStation.Value.ToString();

                string runSql = $@"SELECT ERROR_CODE, ENGLISH_DESCRIPTION, CHINESE_DESCRIPTION AS SPANISH_DESCRIPTION FROM SFCBASE.C_ERROR_CODE WHERE 1 = 1";

                switch (station)
                {
                    case "ALL":
                        break;
                    case "SMT1":
                    case "SMT2":
                        runSql += $@" AND ERROR_CODE LIKE '%AOI%' ";
                        break;
                    case "5DX":
                    case "5DX2":
                        runSql += $@" AND ERROR_CODE LIKE '%5DX%' ";
                        break;
                    case "PTH":
                    case "ICT":
                        runSql += $@" AND ERROR_CODE LIKE '%{station}%' ";
                        break;
                    default:
                        runSql += $@" AND ID LIKE '%MIDI10%' ";
                        break;
                }

                if (!String.IsNullOrEmpty(errorCode))
                {
                    runSql += $@" AND ERROR_CODE LIKE '%{errorCode}%'";
                }

                OleExec SFCDB = DBPools["SFCDB"].Borrow();
                try
                {
                    RunSqls.Add(runSql);
                    DataTable dt = SFCDB.RunSelect(runSql).Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("No Data!");
                    }
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    ReportTable retTab = new ReportTable();
                    retTab.LoadData(dt, null);
                    retTab.Tittle = $@"{errorCode} {station} Error Code List";
                    Outputs.Add(retTab);
                }
                catch (Exception exception)
                {
                    if (SFCDB != null)
                    {
                        DBPools["SFCDB"].Return(SFCDB);
                    }
                    Outputs.Add(new ReportAlart(exception.Message));
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