using MESDataObject;
using MESDataObject.Module;
using MESPubLab.MESStation;
using System.Collections.Generic;
using System.Data;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class StationDataLoader
    {
        /// <summary>
        /// 从Session或者直接配置数据到控件的DataForUse属性
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSourceDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationInput StationInput = Station.Inputs.Find(t => t.DisplayName == Paras[0].VALUE);
            if (StationInput == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            var SourceType = Paras[1];
            if (SourceType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            if (SourceType.VALUE.Equals("SESSION"))
            {
                var data = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                StationInput.DataForUse = new List<object>();
                if (data.Value != null)
                {
                    if (data.Value.GetType().Name == "String")
                    {
                        StationInput.DataForUse.Add(data.Value);
                    }
                    else
                    {
                        IEnumerable<object> list = data.Value as IEnumerable<object>;
                        foreach (var item in list)
                        {
                            StationInput.DataForUse.Add(item);
                        }
                    }
                }
            }
            else if (SourceType.VALUE.Equals("DATA"))
            {
                StationInput.DataForUse = new List<object>();
                for (int i = 2; i < Paras.Count; i++)
                {
                    StationInput.DataForUse.Add(Paras[i].VALUE);
                }
            }
        }

        /// <summary>
        /// 将机种列表数据加载到SESSION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SKUListDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var skus = Station.SFCDB.ORM.Queryable<C_SKU>().Select(t => t.SKUNO).ToList();
            MESStationSession SKUList = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SKUList == null)
            {
                SKUList = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = skus };
                Station.StationSession.Add(SKUList);
            }
            else
            {
                SKUList.Value = skus;
            }
        }

        /// <summary>
        /// 手动设置Session数据
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SessionDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationSession session = Station.StationSession.Find(t => t.MESDataType == Paras[i].SESSION_TYPE && t.SessionKey == Paras[i].SESSION_KEY);
                if (session == null)
                {
                    Station.StationSession.Add(new MESStationSession() { MESDataType = Paras[i].SESSION_TYPE, SessionKey = Paras[i].SESSION_KEY, Value = Paras[i].VALUE });
                }
                else
                {
                    session.Value = Paras[i].VALUE;
                }
            }
        }

        public static void TableDataToSessionDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (session == null)
            {
                session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = "" };
                Station.StationSession.Add(session);
            }
            var TB = Paras.Find(t => t.SESSION_TYPE.ToUpper() == "TABLENAME");
            if (TB == null)
            {
                throw new MESReturnMessage("Missing Params That Session_TYPE Equals 'TableName'");
            }
            else if (TB.VALUE == null || TB.VALUE == "")
            {
                throw new MESReturnMessage("Missing Params,The Value Of Session_TYPE Equals 'TableName' Cann't be null");
            }
            var COL = Paras.Find(t => t.SESSION_TYPE.ToUpper() == "COLUMN");
            if (COL == null)
            {
                throw new MESReturnMessage("Missing Session_TYPE Witch Equals 'Column'");
            }
            else if (COL.VALUE == null || TB.VALUE == "")
            {
                throw new MESReturnMessage("Missing Params,The Value Of Session_TYPE Equals 'Column' Cann't be null");
            }
            var Whr = Paras.FindAll(t => t.SESSION_TYPE.ToUpper() == "WHERE");

            string sql = "SELECT {0} FROM {1} WHERE 1=1 {2}";
            string whrStr = "";
            for (int i = 0; i < Whr.Count; i++)
            {
                if (Whr[i].VALUE.ToString().ToUpper() == "SESSION")
                {
                    var p = Paras[Paras.IndexOf(Whr[i]) + 1];
                    var value = Station.StationSession.Find(t => t.SessionKey == p.SESSION_KEY && t.MESDataType == p.SESSION_TYPE).Value.ToString();
                    whrStr += " AND " + Whr[i].SESSION_KEY + "='" + value + "' ";
                }
                else
                {
                    if (Whr[i].VALUE.Contains(","))
                    {
                        var valarray = Whr[i].VALUE.Split(',');
                        var vals = "";
                        for (int x = 0; x < valarray.Length; x++)
                        {
                            vals += "'" + valarray[x] + "',";
                        }
                        vals = vals.Substring(0, vals.Length - 1);
                        whrStr += " AND " + Whr[i].SESSION_KEY + " in(" + vals + ") ";
                    }
                    else
                    {
                        whrStr += " AND " + Whr[i].SESSION_KEY + "='" + Whr[i].VALUE + "' ";
                    }
                }
            }
            sql = string.Format(sql, COL.SESSION_KEY + " " + COL.VALUE, TB.VALUE, whrStr);
            var res = Station.SFCDB.ORM.Ado.SqlQuery<string>(sql);
            session.Value = res;
        }

        /// <summary>
        /// 加載登錄用戶到SESSION
        /// </summary>
        public static void LoginEmpSessionLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionEmpNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionEmpNo is null)
            {
                Station.StationSession.Add(new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = Station.LoginUser.EMP_NO, ResetInput = Input });
            }
        }
    }
}
