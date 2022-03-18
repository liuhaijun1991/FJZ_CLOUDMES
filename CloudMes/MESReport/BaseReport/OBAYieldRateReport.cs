using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.OleDb;


namespace MESReport.BaseReport
{
    public class OBAYieldRateReport : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();


        private APIInfo _GetList = new APIInfo()
        {

            FunctionName = "GetList",
            Description = "日報表",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>()
            { }
        };


      


        public OBAYieldRateReport()
        {
            Apis.Add(_GetList.FunctionName, _GetList);
          
        }

  
        public void GetList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            
            OleExec DB = null;
            string OBAType = Data["OBAType"].ToString().Trim();
            string QueryType = Data["QueryType"].ToString().Trim();
            string inputDate = Data["inputDate"].ToString().Trim();
            string WeekYearFrom = Data["WeekYearFrom"].ToString().Trim();
            string WeekYearTo = Data["WeekYearTo"].ToString().Trim();
            string WeekFrom = Data["WeekFrom"].ToString().Trim();
            string WeekTo = Data["WeekTo"].ToString().Trim();
            string skuFlag = Data["skuFlag"].ToString().Trim();
            string TimeFrom = string.Empty;
            string TimeTo = string.Empty;
 

            try
            {
                DB = this.DBPools["SFCDB"].Borrow();
                T_R_LOT_STATUS TRLS = new T_R_LOT_STATUS(DB, DB_TYPE_ENUM.Oracle);
                if (QueryType == "1")//by day
                {
                    TimeFrom = inputDate.Substring(0, 10);
                    TimeTo = inputDate.Substring(13, 10);
                    StationReturn.Data = TRLS.GetDataByDays(TimeFrom, TimeTo, OBAType, skuFlag, DB);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "";
                }
                else if (QueryType == "2")//by week
                {
                    List<OBA_Yield_Data> OBAList = new List<OBA_Yield_Data>();
                    string strFirstDateOfWeek = string.Empty;
                    string strLastDateOfWeek = string.Empty;

                    int intYearFrom = Convert.ToInt32(WeekYearFrom);
                    int intYearTo = Convert.ToInt32(WeekYearTo);

                    int intWeekFrom= Convert.ToInt32(WeekFrom);
                    int intWeekTo = Convert.ToInt32(WeekTo);

                    if(intYearFrom==intYearTo && intWeekFrom<= intWeekTo)
                    {
                        for(int CountWeek= intWeekFrom; CountWeek <= intWeekTo; CountWeek++)
                        {
                            strFirstDateOfWeek = GetFirstDateByWeek(intYearFrom, CountWeek).ToString("yyyy/MM/dd");
                            strLastDateOfWeek = GetLastDateByWeek(intYearFrom, CountWeek).ToString("yyyy/MM/dd");
                            OBAList.AddRange(TRLS.GetDataByWeeks(strFirstDateOfWeek, strLastDateOfWeek, WeekYearFrom, CountWeek.ToString(), OBAType, skuFlag, DB));
                        }
                        StationReturn.Data = OBAList;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = "";
                    }
                    else if(intYearFrom < intYearTo)
                    {             
                        for (int CountYear = intYearFrom; CountYear <= intYearTo; CountYear++)
                        {
                            if(CountYear== intYearFrom) { 
                                for (int CountWeek = intWeekFrom; CountWeek <= 53; CountWeek++)
                                {
                                    strFirstDateOfWeek = GetFirstDateByWeek(CountYear, CountWeek).ToString("yyyy/MM/dd");
                                    strLastDateOfWeek = GetLastDateByWeek(CountYear, CountWeek).ToString("yyyy/MM/dd");
                                    OBAList.AddRange(TRLS.GetDataByWeeks(strFirstDateOfWeek, strLastDateOfWeek, CountYear.ToString(), CountWeek.ToString(), OBAType, skuFlag, DB));
                                }
                            }
                            else if (CountYear== intYearTo)
                            {
                                for (int CountWeek = 1; CountWeek <= intWeekTo; CountWeek++)
                                {
                                    strFirstDateOfWeek = GetFirstDateByWeek(CountYear, CountWeek).ToString("yyyy/MM/dd");
                                    strLastDateOfWeek = GetLastDateByWeek(CountYear, CountWeek).ToString("yyyy/MM/dd");
                                    OBAList.AddRange(TRLS.GetDataByWeeks(strFirstDateOfWeek, strLastDateOfWeek, CountYear.ToString(), CountWeek.ToString(), OBAType, skuFlag, DB));
                                }
                            }
                            else
                            {
                                for (int CountWeek = 1; CountWeek <= 53;CountWeek++)
                                {
                                    strFirstDateOfWeek = GetFirstDateByWeek(CountYear, CountWeek).ToString("yyyy/MM/dd");
                                    strLastDateOfWeek = GetLastDateByWeek(CountYear, CountWeek).ToString("yyyy/MM/dd");
                                    OBAList.AddRange(TRLS.GetDataByWeeks(strFirstDateOfWeek, strLastDateOfWeek, CountYear.ToString(), CountWeek.ToString(), OBAType, skuFlag, DB));
                                }
                            }
                        }
                        StationReturn.Data = OBAList;
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = "";

                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MSGCODE20200522104611";
                    }

                    
                }
                else if (QueryType == "3")//by week
                {
                    TimeFrom = inputDate.Substring(0, 7);
                    TimeTo = inputDate.Substring(10, 7);
                    StationReturn.Data = TRLS.GetDataByMonth(TimeFrom, TimeTo, OBAType, skuFlag, DB);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000083";
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                throw e;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(DB);
            }
        }

        public DateTime GetFirstDateByWeek(int Year, int Week) {
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
                if(lastDate> calcLast)
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
