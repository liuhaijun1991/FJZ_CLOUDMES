using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BPD
{
    public class StationWipReport:ReportBase
    {
        OleExec SFCDB = null;
        ReportInput Skuno = new ReportInput() { InputType = "Autocomplete", Name = "Skuno", Value = "", API = "MESStation.Config.SkuConfig.GetAllSkuno", APIPara = "", RefershType = RefershTypeEnum.Default.ToString() };
        ReportInput Wo = new ReportInput { Name = "WO", InputType = "TXT", Value = "002100017350", Enable = true, SendChangeEvent = false, ValueForUse = null, EnterSubmit = false };
        ReportInput Station = new ReportInput { Name = "StationName", InputType = "Select", Value = "ALL", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public StationWipReport()
        {
            this.Inputs.Add(Skuno);
            this.Inputs.Add(Wo);
            this.Inputs.Add(Station);
        }

        public override void Init()
        {
            SFCDB = DBPools["SFCDB"].Borrow();

            Station.ValueForUse = SFCDB.ORM.Queryable<C_STATION>().Select(t => t.STATION_NAME).ToList();
        }

        public string RecoverStationName(string OldStationName)
        {
            string NewStationName = OldStationName;
            if (OldStationName.Equals("S101"))
            {
                NewStationName = "STOCKIN";
            }
            else if (OldStationName.Contains("F101"))
            {
                NewStationName = "CBS";
            }
            else if (OldStationName.Contains("F106"))
            {
                NewStationName = "B29M";
            }
            return NewStationName;
        }

        public override void Run()
        {
            string _Skuno = Skuno.Value.ToString();
            string WorkOrder = Wo.Value.ToString();
            string StationName = RecoverStationName(Station.Value.ToString());
            string RepairLinkUrl = "/FunctionPage/Report/Report.html?ClassName=MESReport.BPD.SnRepairReport&RunFlag=1&SN=";

            DataTable SnData = null;
            DataTable SnStationData = null;
            if (StationName.Equals("Repair"))
            {
                SnData = SFCDB.ORM.Queryable<R_SN>()
                .Where(rs => rs.WORKORDERNO == WorkOrder && rs.SKUNO== _Skuno && rs.REPAIR_FAILED_FLAG == "1")
                .ToDataTable();

                SnStationData = SFCDB.ORM.Queryable<R_SN, R_SN_STATION_DETAIL>((rs, rssd) => rs.SN == rssd.SN)
                .Where((rs, rssd) => rs.WORKORDERNO == WorkOrder && rs.REPAIR_FAILED_FLAG == "1")
                .OrderBy((rs, rssd) => rs.SN).OrderBy((rs, rssd) => rs.EDIT_TIME)
                .Select((rs, rssd) => rssd).ToDataTable();
            }
            
            else
            {
                SnData = SFCDB.ORM.Queryable<R_SN>()
                    .Where(rs => rs.WORKORDERNO == WorkOrder && rs.SKUNO== _Skuno && rs.NEXT_STATION == StationName && rs.REPAIR_FAILED_FLAG != "1" && rs.SCRAPED_FLAG == "0" && rs.VALID_FLAG=="1")
                    .ToDataTable();

                SnStationData = SFCDB.ORM.Queryable<R_SN, R_SN_STATION_DETAIL>((rs, rssd) => rs.SN == rssd.SN)
                .Where((rs, rssd) => rs.WORKORDERNO == WorkOrder && rs.SKUNO== _Skuno && rs.NEXT_STATION == StationName)
                .OrderBy((rs, rssd) => rs.SN).OrderBy((rs, rssd) => rs.EDIT_TIME)
                .Select((rs, rssd) => rssd).ToDataTable();
            }

            


            ReportTable SnTable = new ReportTable();
            SnTable.Tittle = "SN 當前狀態";
            foreach (DataColumn dc in SnData.Columns)
            {
                if (!SnTable.ColNames.Contains(dc.ColumnName))
                {
                    SnTable.ColNames.Add(dc.ColumnName);
                }
            }

            if (StationName.Equals("Repair"))
            {
                foreach (DataRow dr in SnData.Rows)
                {
                    TableRowView RowView = new TableRowView();
                    foreach (DataColumn dc in SnData.Columns)
                    {
                        TableColView ColView = new TableColView()
                        {
                            ColName = dc.ColumnName,
                            Value = dr[dc.ColumnName].ToString()
                        };
                        if (dc.ColumnName.ToUpper().Equals("SN"))
                        {
                            ColView.LinkType = "Link";
                            ColView.LinkData = RepairLinkUrl + dr[dc.ColumnName];
                        }
                        RowView.RowData.Add(dc.ColumnName, ColView);

                    }
                    SnTable.Rows.Add(RowView);

                }
            }
            else
            {
                SnTable.LoadData(SnData);
            }
            Outputs.Add(SnTable);

            ReportTable SnStationTable = new ReportTable();
            SnStationTable.Tittle = "產品過站記錄";
            foreach (DataColumn dc in SnStationData.Columns)
            {
                if (!SnStationTable.ColNames.Contains(dc.ColumnName))
                {
                    SnStationTable.ColNames.Add(dc.ColumnName);
                }
            }
            SnStationTable.LoadData(SnStationData);
            Outputs.Add(SnStationTable);
        }

    }
}
