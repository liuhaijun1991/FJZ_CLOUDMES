using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESStation.BaseClass;
using MESDBHelper;
using MESDataObject.Module;
using Newtonsoft.Json;
using MESDataObject;

namespace MESStation.Config
{
    public class SeriesConfig : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();

        private APIInfo AllSeries = new APIInfo()
        {
            FunctionName = "GetAllSeries",
            Description = "獲取所有機種",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo SeriesByName = new APIInfo()
        {
            FunctionName = "GetSeriesByName",
            Description = "根據機種名獲取機種",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="Series_Name",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _UpdateSeries = new APIInfo()
        {
            FunctionName = "UpdateSeries",
            Description = "修改機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SeriesObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _AddSeries = new APIInfo()
        {
            FunctionName = "AddSeries",
            Description = "添加機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SeriesObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSeries = new APIInfo()
        {
            FunctionName = "DeleteSeries",
            Description = "刪除機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SeriesObject",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        private APIInfo _DeleteSeriesById = new APIInfo()
        {
            FunctionName = "DeleteSeriesById",
            Description = "刪除機種信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SeriesID",InputType="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }
        };

        public SeriesConfig()
        {
            this.Apis.Add(AllSeries.FunctionName, AllSeries);
            this.Apis.Add( SeriesByName.FunctionName, SeriesByName);
            this.Apis.Add(_AddSeries.FunctionName, _AddSeries);
            this.Apis.Add(_UpdateSeries.FunctionName, _UpdateSeries);
            this.Apis.Add(_DeleteSeries.FunctionName, _DeleteSeries);
            this.Apis.Add(_DeleteSeriesById.FunctionName, _DeleteSeriesById);
        }

        public void GetAllSeries(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SERIES> SeriesList = new List<C_SERIES>();
            T_C_SERIES table = null;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_SERIES(sfcdb, DBTYPE);
                SeriesList = table.GetAllSeries(sfcdb);
                if (SeriesList.Count() == 0)
                {
                    //沒有獲取到數據
                    ConstructReturns(
                    ref StationReturn,
                    StationReturnStatusValue.Pass,
                    MESReturnMessage.GetMESReturnMessage("QueryNoData"),
                    new object());
                }
                else
                {
                    //獲取到數據
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("QueryOK", new string[] { SeriesList.Count().ToString() }),
                        SeriesList);
                }
            }
            catch (Exception e)
            {
                ConstructReturns(
                    ref StationReturn,
                    StationReturnStatusValue.Fail,
                    MESReturnMessage.GetMESReturnMessage("Exception", new string[] { e.Message }),
                    e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void GetSeriesByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_SERIES> SeriesList = new List<C_SERIES>();
            T_C_SERIES table = null;
            string SeriesName = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_SERIES(sfcdb, DBTYPE);
                SeriesName = Data["Series_Name"].ToString();
                if (string.IsNullOrEmpty(SeriesName))
                {
                    GetAllSeries(requestValue, Data, StationReturn);
                }
                else
                {
                    SeriesList = table.GetSeriesByName(SeriesName, sfcdb);
                    if (SeriesList.Count() == 0)
                    {
                        //沒有獲取到數據
                        ConstructReturns(
                            ref StationReturn,
                            StationReturnStatusValue.Pass,
                            MESReturnMessage.GetMESReturnMessage("QueryNoData"),
                            new object());
                    }
                    else
                    {
                        //獲取成功
                        ConstructReturns(
                            ref StationReturn,
                            StationReturnStatusValue.Pass,
                            MESReturnMessage.GetMESReturnMessage("QueryOK", new string[] { SeriesList.Count().ToString() }),
                            SeriesList);
                    }
                }
            }
            catch (Exception e)
            {
                ConstructReturns(
                    ref StationReturn,
                    StationReturnStatusValue.Fail,
                    MESReturnMessage.GetMESReturnMessage("Exception", new string[] { e.Message }),
                    e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }


        public void UpdateSeries(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SERIES table = null;
            string SeriesObject = string.Empty;
            C_SERIES series = null;
            string result = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_SERIES(sfcdb, DBTYPE);
                SeriesObject = Data["SeriesObject"].ToString();
                series = (C_SERIES)JsonConvert.Deserialize(SeriesObject, typeof(C_SERIES));
                series.EDIT_EMP = "A0225204";//LoginUser.EMP_NO;
                result = table.UpdateSeries(BU, series, "UPDATE", sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //更新成功
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateOK", new string[] { result }),
                        result);
                }
                else
                {
                    //更新失敗
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateNoData"),
                        result);
                }
            }
            catch (Exception e)
            {
                ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Fail,
                        MESReturnMessage.GetMESReturnMessage("Exception", new string[] { e.Message }),
                        e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }

        }

        public void AddSeries(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SERIES table = null;
            string SeriesObject = string.Empty;
            C_SERIES series = null;
            string result = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_SERIES(sfcdb, DBTYPE);
                SeriesObject = Data["SeriesObject"].ToString();
                series = (C_SERIES)JsonConvert.Deserialize(SeriesObject, typeof(C_SERIES));
                series.EDIT_EMP = "A0225204";// LoginUser.EMP_NO;
                series.EDIT_TIME = DateTime.Now;
                result = table.UpdateSeries(BU, series, "ADD", sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //添加成功
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateOK", new string[] { result }),
                        result);
                }
                else
                {
                    //沒有添加任何數據
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateNoData"),
                        result);
                }
            }
            catch (Exception e)
            {
                ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Fail,
                        MESReturnMessage.GetMESReturnMessage("Exception", new string[] { e.Message }),
                        e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DeleteSeries(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SERIES table = null;
            string SeriesObject = string.Empty;
            C_SERIES series = null;
            string result = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_SERIES(sfcdb, DBTYPE);
                SeriesObject = Data["SeriesObject"].ToString();
                series = (C_SERIES)JsonConvert.Deserialize(SeriesObject, typeof(C_SERIES));
                result = table.UpdateSeries(BU, series, "DELETE", sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateOK", new string[] { result }),
                        result);
                }
                else
                {
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateNoData"),
                        result);
                }
            }
            catch (Exception e)
            {
                ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("Exception", new string[] { e.Message }),
                        e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void DeleteSeriesById(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SERIES table = null;
            C_SERIES series = null;
            string result = string.Empty;
            string SeriesId = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                table = new T_C_SERIES(sfcdb, DBTYPE);
                SeriesId = Data["SeriesID"].ToString();
                series = new C_SERIES();
                series.ID = SeriesId;
                result = table.UpdateSeries(BU, series, "DELETE", sfcdb);

                if (Int32.Parse(result) > 0)
                {
                    //刪除成功
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateOK", new string[] { result }),
                        result);
                }
                else
                {
                    //沒有刪除任何數據
                    ConstructReturns(
                        ref StationReturn,
                        StationReturnStatusValue.Pass,
                        MESReturnMessage.GetMESReturnMessage("UpdateNoData"),
                        result);
                }
            }
            catch (Exception e)
            {
                ConstructReturns(
                    ref StationReturn,
                    StationReturnStatusValue.Fail,
                    MESReturnMessage.GetMESReturnMessage("Exception", new string[] { e.Message }),
                    e.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(sfcdb);
            }
        }

        public void ConstructReturns(ref MESStationReturn StationReturn, string Status, string Message, object Data)
        {
            StationReturn.Status = Status;
            StationReturn.Message = Message;
            StationReturn.data = Data;
        }
    }
}
