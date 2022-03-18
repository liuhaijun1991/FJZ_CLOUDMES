using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="OBAYieldLots.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>ZhangWenXiao</author>
    // <date> 2020-05-26 </date>
    /// <summary>
    /// OBAYieldLots
    /// </summary>
    public class OBAYieldLots : ReportBase
    {
        ReportInput inputOBAType = new ReportInput() { Name = "OBAType", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputTime = new ReportInput() { Name = "searchTime", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSkuno = new ReportInput() { Name = "Skuno", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputSeachType = new ReportInput() { Name = "seachType", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput inputLotID = new ReportInput() { Name = "LOT_ID", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };

        public OBAYieldLots()
        {

            Inputs.Add(inputOBAType);
            Inputs.Add(inputTime);
            Inputs.Add(inputSkuno);
            Inputs.Add(inputSeachType);
            Inputs.Add(inputLotID);

        }

        public override void Init()
        {
            //base.Init();
        }

        public override void Run()
        {

            string OBAType = inputOBAType.Value.ToString().ToUpper();
            string Time = inputTime.Value.ToString().ToUpper();
            string Skuno = inputSkuno.Value.ToString().ToUpper();
            string LOT_ID = inputLotID.Value.ToString().ToUpper();
            string TimeType = "Day";
            string sqlRun = string.Empty;
            string strSkunoFlag = "";
            string SeachType = inputSeachType.Value.ToString().ToUpper();
            string strsqlcdt = "";
            DataTable LotListTable = new DataTable();
            DataTable linkTable = new DataTable();
            DataRow linkRow = null;
            OleExec SFCDB = DBPools["SFCDB"].Borrow();


            switch (Time.Length)
            {
                case 10:
                    TimeType = "Day";
                    break;
                case 8:
                    TimeType = "Week";
                    break;
                case 9:
                    TimeType = "Week";
                    break;
                case 7:
                    TimeType = "Month";
                    break;
            }

            if (Skuno != "") { strSkunoFlag = "AND SKUNO='" + Skuno + "'"; }

            try
            {

                if (SeachType == "1"|| SeachType == "2"|| SeachType == "3") {

                    switch (SeachType)
                    {
                        case "1":
                            strsqlcdt = "";
                            break;
                        case "2":
                            strsqlcdt = "AND (FAIL_QTY<REJECT_QTY OR FAIL_QTY=0)";
                            break;
                        case "3":
                            strsqlcdt = "AND FAIL_QTY>=REJECT_QTY AND FAIL_QTY<>0";
                            break;
                        default:
                            break;
                    }



                    if (TimeType.Equals("Day"))
                    {
                        sqlRun = $@"SELECT ID,LOT_NO,SKUNO,AQL_TYPE,AQL_LEVEL,SAMPLE_STATION,
                                    LOT_QTY,REJECT_QTY,SAMPLE_QTY,PASS_QTY,FAIL_QTY
                                    FROM R_LOT_STATUS WHERE 
                                    EDIT_TIME 
                                    BETWEEN 
                                    TO_DATE('{Time} 00:00:00','YYYY/MM/DD HH24:MI:SS') AND
                                    TO_DATE('{Time} 23:59:59','YYYY/MM/DD HH24:MI:SS') 
                                    AND SAMPLE_STATION='{OBAType}'
                                    {strSkunoFlag}    
                                    {strsqlcdt}";
                    }
                    else if (TimeType.Equals("Week"))
                    {
                        string startTime = Time.Length==9?
                        GetFirstDateByWeek(Convert.ToInt32(Time.Substring(0, 4)), Convert.ToInt32(Time.Substring(7, 2))).ToString("yyyy/MM/dd"):
                        GetFirstDateByWeek(Convert.ToInt32(Time.Substring(0, 4)), Convert.ToInt32(Time.Substring(7, 1))).ToString("yyyy/MM/dd");

                        string endTime = Time.Length == 9 ? 
                        GetLastDateByWeek(Convert.ToInt32(Time.Substring(0, 4)), Convert.ToInt32(Time.Substring(7, 2))).ToString("yyyy/MM/dd"):
                        GetLastDateByWeek(Convert.ToInt32(Time.Substring(0, 4)), Convert.ToInt32(Time.Substring(7, 1))).ToString("yyyy/MM/dd");

                        sqlRun = $@"SELECT ID,LOT_NO,SKUNO,AQL_TYPE,AQL_LEVEL,SAMPLE_STATION,
                                    LOT_QTY,REJECT_QTY,SAMPLE_QTY,PASS_QTY,FAIL_QTY
                                    FROM R_LOT_STATUS WHERE 
                                    EDIT_TIME 
                                    BETWEEN 
                                    TO_DATE('{startTime} 00:00:00','YYYY/MM/DD HH24:MI:SS') AND
                                    TO_DATE('{endTime} 23:59:59','YYYY/MM/DD HH24:MI:SS') 
                                    AND SAMPLE_STATION='{OBAType}'
                                    {strSkunoFlag}
                                    {strsqlcdt}";
                    }
                    else if (TimeType.Equals("Month"))
                    {
                        string ToTime = Time.Substring(5, 2) == "12" ? (Convert.ToInt32(Time.Substring(0, 4)) + 1).ToString() + "/" + "01" : Time.Substring(0, 4) + "/" + (Convert.ToInt32(Time.Substring(5, 2)) + 1).ToString();
                        sqlRun = $@"SELECT ID,LOT_NO,SKUNO,AQL_TYPE,AQL_LEVEL,SAMPLE_STATION,
                                    LOT_QTY,REJECT_QTY,SAMPLE_QTY,PASS_QTY,FAIL_QTY
                                    FROM R_LOT_STATUS WHERE 
                                    EDIT_TIME 
                                    BETWEEN 
                                    TO_DATE('{Time}','YYYY/MM') AND
                                    TO_DATE('{ToTime}','YYYY/MM')
                                    AND SAMPLE_STATION='{OBAType}'
                                    {strSkunoFlag}
                                    {strsqlcdt}";
                    }


                    RunSqls.Add(sqlRun);

                    LotListTable = SFCDB.RunSelect(sqlRun).Tables[0];
    
                    linkTable.Columns.Add("ID");
                    linkTable.Columns.Add("LOT_NO");
                    linkTable.Columns.Add("SKUNO");
                    linkTable.Columns.Add("AQL_TYPE");
                    linkTable.Columns.Add("AQL_LEVEL");
                    linkTable.Columns.Add("SAMPLE_STATION");
                    linkTable.Columns.Add("LOT_QTY");
                    linkTable.Columns.Add("REJECT_QTY");
                    linkTable.Columns.Add("SAMPLE_QTY");
                    linkTable.Columns.Add("PASS_QTY");
                    linkTable.Columns.Add("FAIL_QTY");
                    for (int i = 0; i < LotListTable.Rows.Count; i++)
                    {
                        linkRow = linkTable.NewRow();
                        linkRow["ID"] = "";
                        linkRow["LOT_NO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.OBAYieldLots&RunFlag=1&seachType=4&LOT_ID=" + LotListTable.Rows[i]["ID"].ToString()+ "&OBAType=" + LotListTable.Rows[i]["SAMPLE_STATION"].ToString();
                        linkRow["SKUNO"] = "";
                        linkRow["AQL_TYPE"] = "";
                        linkRow["AQL_LEVEL"] = "";
                        linkRow["SAMPLE_STATION"] = "";
                        linkRow["LOT_QTY"] = "";
                        linkRow["REJECT_QTY"] = "";
                        linkRow["SAMPLE_QTY"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.OBAYieldLots&RunFlag=1&seachType=4&LOT_ID=" + LotListTable.Rows[i]["ID"].ToString() + "&OBAType=" + LotListTable.Rows[i]["SAMPLE_STATION"].ToString();
                        linkRow["PASS_QTY"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.OBAYieldLots&RunFlag=1&seachType=5&LOT_ID=" + LotListTable.Rows[i]["ID"].ToString() + "&OBAType=" + LotListTable.Rows[i]["SAMPLE_STATION"].ToString();
                        linkRow["FAIL_QTY"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.OBAYieldLots&RunFlag=1&seachType=6&LOT_ID=" + LotListTable.Rows[i]["ID"].ToString() + "&OBAType=" + LotListTable.Rows[i]["SAMPLE_STATION"].ToString();
                        linkTable.Rows.Add(linkRow);
                    }
                    ReportTable reportTable = new ReportTable();
                    reportTable.LoadData(LotListTable, linkTable);
                    reportTable.Tittle = "OBAYieldRateBySku";
                    Outputs.Add(reportTable);
                }
                else if(SeachType=="4"|| SeachType == "5"|| SeachType == "6")
                {
                    
                    switch (SeachType)
                    {
                        case "4":
                            strsqlcdt = "";
                            break;
                        case "5":
                            strsqlcdt = "AND A.STATUS='1'";
                            break;
                        case "6":
                            strsqlcdt = "AND A.STATUS='2'";
                            break;
                        default:
                            strsqlcdt = "";
                            break;

                    }
                    
                    sqlRun = $@"SELECT B.LOT_NO,A.SN,A.WORKORDERNO,
                                CASE WHEN A.STATUS='1' THEN 'PASS' WHEN A.STATUS='2' THEN 'FAIL' END AS STATUS,
                                A.EDIT_EMP,A.EDIT_TIME
                                FROM R_LOT_DETAIL A,R_LOT_STATUS B
                                WHERE LOT_ID='{LOT_ID}'
                                AND A.LOT_ID=B.ID
                                AND B.SAMPLE_STATION='{OBAType}'
                                {strsqlcdt}";

                    RunSqls.Add(sqlRun);

                    LotListTable = SFCDB.RunSelect(sqlRun).Tables[0];
                    linkTable.Columns.Add("LOT_NO");
                    linkTable.Columns.Add("SN");
                    linkTable.Columns.Add("WORKORDERNO");
                    linkTable.Columns.Add("STATUS");
                    linkTable.Columns.Add("EDIT_EMP");
                    linkTable.Columns.Add("EDIT_TIME");

                    for (int i = 0; i < LotListTable.Rows.Count; i++)
                    {
                        linkRow = linkTable.NewRow();
                        linkRow["LOT_NO"] = "";
                        linkRow["SN"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNReport&RunFlag=1&SN=" + LotListTable.Rows[i]["SN"].ToString();
                        linkRow["WORKORDERNO"] = "Link#/FunctionPage/Report/Report.html?ClassName=MESReport.BaseReport.SNListByWo&RunFlag=1&WO=" + LotListTable.Rows[i]["WORKORDERNO"].ToString() + "&EventName=";
                        linkRow["STATUS"] = "";
                        linkRow["EDIT_EMP"] = "";
                        linkRow["EDIT_TIME"] = "";
                        linkTable.Rows.Add(linkRow);
                    }
                    ReportTable reportTable = new ReportTable();
                    reportTable.LoadData(LotListTable);
                    reportTable.Tittle = "LotsSnList";
                    Outputs.Add(reportTable);

                }
            }
            catch (Exception exception)
            { 
                throw exception;
            }
            finally
            {
                DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public DateTime GetFirstDateByWeek(int Year, int Week)
        {
            DateTime firstDate = DateTime.MinValue;
            DateTime lastDate = DateTime.MinValue;


            DateTime calcFirst = new DateTime(Year, 1, 1);
            DateTime calcLast = new DateTime(Year, 12, 31);
            int startWeekDay = (int)calcFirst.DayOfWeek;

            if (Week == 1)
            {
                firstDate = calcFirst;
                lastDate = calcFirst.AddDays(6 - startWeekDay);
            }
            else
            {
                firstDate = calcFirst.AddDays((7 - startWeekDay) + (Week - 2) * 7);
                lastDate = firstDate.AddDays(6);
                if (lastDate > calcLast)
                {
                    lastDate = calcLast;
                }
            }
            return firstDate;
        }
        public DateTime GetLastDateByWeek(int Year, int Week)
        {
            DateTime firstDate = DateTime.MinValue;
            DateTime lastDate = DateTime.MinValue;


            DateTime calcFirst = new DateTime(Year, 1, 1);
            DateTime calcLast = new DateTime(Year, 12, 31);
            int startWeekDay = (int)calcFirst.DayOfWeek;

            if (Week == 1)
            {
                firstDate = calcFirst;
                lastDate = calcFirst.AddDays(6 - startWeekDay);
            }
            else
            {
                firstDate = calcFirst.AddDays((7 - startWeekDay) + (Week - 2) * 7);
                lastDate = firstDate.AddDays(6);
                if (lastDate > calcLast)
                {
                    lastDate = calcLast;
                }
            }
            return lastDate;
        }

    }
}
