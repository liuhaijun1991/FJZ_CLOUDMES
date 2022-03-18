using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using MESStation.LogicObject;
using MESPubLab.MESStation.MESReturnView.Station;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using System.Data;
using MESPubLab.MESStation.SNMaker;
using MESDataObject.Module.BPD;
using MESDataObject.Common;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class SNLoader
    {
        public static void KpsnRepalceDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception("Error Load Wo: Can't finded WOSession!");
            }
            
            try
            {
                var strSN = SNSession.Value.ToString();
                OleExec SFCDB = Station.SFCDB;
                var kp1 = SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == strSN && t.SCANTYPE == "SN" && t.VALID_FLAG == 1).First();
                if (kp1 != null)
                {
                    if (kp1.R_SN_ID != null)
                    {
                        var psn = SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == kp1.R_SN_ID).First();
                        if (psn != null)
                        {
                            var config = SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where
                                (t => t.SKUNO == psn.SKUNO && t.CATEGORY == "PASSSTATION" && t.CATEGORY_NAME == "REPLACE_SN" && t.EXTEND == Station.StationName).First();
                            if (config != null)
                            {
                                SNSession.Value = kp1.SN;
                                Input.Value = kp1.SN;
                            }

                        }

                    }
                }



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 按配置自动生成新SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SiAutoSNMakerDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new Exception("Error Load Wo: Can't finded WOSession!");
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error Load Wo: Can't finded SkuSession!");
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception("Error Load Wo: Can't finded SNSession!");
            }

            MESStationSession KPSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (KPSNSession == null)
            {
                KPSNSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(KPSNSession);
            }
            KPSNSession.Value = null;
            try
            {
                OleExec sfcdb = Station.SFCDB;
                WorkOrder WO = (WorkOrder)WOSession.Value;
                SKU Sku = (SKU)SkuSession.Value;
                //获取KP配置
                C_KP_List_Item_Detail oo = null;
                string kpSkuno;
                var kps = sfcdb.ORM.Queryable<C_KP_List_Item>().Where(t => t.LIST_ID == WO.KP_LIST_ID && t.STATION == Station.StationName).OrderBy(t => t.SEQ).ToList();

                if (kps.Count > 0)
                {
                    kpSkuno = kps[0].KP_PARTNO;
                    oo = sfcdb.ORM.Queryable<C_KP_List_Item_Detail>().Where(t => t.ITEM_ID == kps[0].ID && (t.SCANTYPE == "PCBA S/N" || t.SCANTYPE == "PPM S/N")).First();
                }
                else
                {
                    throw new Exception("Kplist config err: WO.Kp_List_Id is Null Or Station Not SILoading!");
                }

                //如果没有配置
                if (oo == null)
                {
                    throw new Exception("Kplist config err: ScanType Not In [PCBA S/N , PPM S/N]!");
                }
                var strSN = SNSession.Value.ToString();
                SN sn = new SN(strSN, sfcdb, DB_TYPE_ENUM.Oracle);
                if (sn.SkuNo != kpSkuno)
                {
                    throw new Exception($@"KP Err:'{sn.SkuNo}' need input '{kpSkuno}'");
                }

                var newSN = SNmaker.GetNextSN(Sku.SkuBase.SN_RULE, Station.DBS["SFCDB"]);
                Input.Value = newSN;
                SNSession.Value = newSN;
                KPSNSession.Value = sn;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                // throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }

            if (Input.Value == null || (Input.Value != null && Input.Value.ToString().Equals("")))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { "SN" }));
            }
            string inputValue = Input.Value.ToString();
            SN SNObj = null;
            try
            {
                SNObj = new SN(inputValue, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(SNObj.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + inputValue }));
                }
                SNSession.Value = SNObj;
                SNSession.InputValue = inputValue;
                SNSession.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { "SN:", inputValue }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 從輸入加載并轉換主板條碼SN對象重新寫入Session和Input.Value.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadSystemSnObjectFromInputDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strInputSn = "", strSystemSn = "", strModelType = "";
            if (Paras.Count != 2)
            {
                // throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
                strInputSn = Input.Value.ToString();
            }
            else if (SNSession.Value != null)
            {
                //strInputSn = SNSession.Value.ToString();//不能直接轉換Session裡面的條碼,因為Loading工站加B條碼
                strInputSn = Input.Value.ToString();
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { "SN" }));
            }

            MESStationSession sessionModelType = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionModelType == null)
            {
            }
            else if (sessionModelType.Value != null)
            {
                strModelType = sessionModelType.Value.ToString();
            }

            if (Station.StationName.EndsWith("LOADING"))
            {
                //SILOADING因為條碼沒有LOADING進系統,所以只能加載SN(String),如果用戶有配置ModelType:022(主板自動加B LOADING)則將輸入的主板條碼加B進行後面的Loading&過站.
                if (strModelType.IndexOf("022") >= 0)
                {
                    strSystemSn = strInputSn + "B";
                }
                else
                {
                    strSystemSn = strInputSn;
                }
                SNSession.Value = strSystemSn;
                SNSession.InputValue = strSystemSn;
                SNSession.ResetInput = Input;
            }
            else
            {
                //先從KEYPART找SILOADING記錄(默認用戶掃的是前段的PCBA條碼,在SILOADING會寫R_SN_KP一筆記錄關聯前後段條碼),找到記錄後取主板SN再從R_SN取SN對象
                R_SN_KP rsnkp = new R_SN_KP();
                T_R_SN_KP tsnkp = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_SN_KP> listkp = tsnkp.GetBYSNSCANTYPE(strInputSn, 1, Station.SFCDB).Where(p => p.STATION.EndsWith("LOADING")).ToList();
                if (listkp.Count > 0)
                {
                    strSystemSn = listkp[0].SN;
                }
                //沒有前後段PCBA LINK關係的則默認是R_SN.BOXSN或R_SN.SN
                if (strSystemSn == "")
                {
                    T_R_SN trsn = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    R_SN rsn = trsn.GetSNByBoxSN(strInputSn, Station.SFCDB);
                    if (rsn == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { strInputSn }));
                    }
                    strSystemSn = rsn.SN;
                }
                SN sn = new SN(strSystemSn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                SNSession.Value = sn;
                SNSession.InputValue = strSystemSn;
                SNSession.ResetInput = Input;
            }
        }

        public static void InputCSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }

            if (Input.Value == null || (Input.Value != null && Input.Value.ToString().Equals("")))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { "SN" }));
            }
            string inputValue = Input.Value.ToString();
            string Strsql = $@"select*From r_Sn_link where csn='{inputValue}' and validflag='1'";
            DataTable dt = Station.SFCDB.RunSelect(Strsql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                inputValue = dt.Rows[0]["SN"].ToString();
            }
            SN SNObj = null;
            try
            {
                SNObj = new SN(inputValue, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(SNObj.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + inputValue }));
                }
                SNSession.Value = SNObj;
                SNSession.InputValue = inputValue;
                SNSession.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { "SN:", inputValue }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// 加載 SN 的返修站位
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnReturnStationDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_C_ROUTE_DETAIL RD = new T_C_ROUTE_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (Paras.Count < 2)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                }
                // throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }

            MESStationSession ReturnStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ReturnStationSession == null)
            {
                ReturnStationSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(ReturnStationSession);
            }


            string inputValue = Input.Value.ToString();
            SN SNObj = null;
            try
            {
                SNObj = (SN)SNSession.Value;
                if (SNObj == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + inputValue }));
                }
                ReturnStationSession.Value = RD.GetReturnStation(SNObj.RouteID, SNObj.NextStation, Station.SFCDB);
                Station.AddMessage("MES00000029", new string[] { "SN:", inputValue }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                }
                throw ex;
            }


        }

        public static void RepairInputSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                }
                // throw new Exception("參數數量不正確!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }

            string inputValue = Input.Value.ToString();
            SN SNObj = null;
            try
            {
                SNObj = new SN(inputValue, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (string.IsNullOrEmpty(SNObj.SerialNo))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + inputValue }));
                }
                SNSession.Value = SNObj;
                SNSession.InputValue = inputValue;
                SNSession.ResetInput = Input;
                Station.AddMessage("MES00000029", new string[] { "SN:", inputValue }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                foreach (R_Station_Output output in Station.StationOutputs)
                {
                    try
                    {
                        Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                    }
                    catch
                    {
                    }
                }
                throw ex;
            }
        }

        public static void IStaionTest(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string inputValue = Input.Value.ToString();
            int para = Paras.Count;
        }

        //加載SN待過工站
        public static void SNNextStationDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //Modify by LLF 2018-01-26 通過配置獲取
            //MESStationSession WipStationSave = new MESStationSession() {MESDataType= "WIPSTATION", InputValue=Input.Value.ToString(),SessionKey="1",ResetInput=Input };

            string StrNextStation = "";
            MESStationSession NextStationSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (NextStationSave == null)
            {
                NextStationSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NextStationSave);
            }
            string strSn = Input.Value.ToString();
            SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == sn.CurrentStation).FirstOrDefault();

            //Modify by LLF 2018-01-29
            //string nextStation1 = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO + 10).FirstOrDefault().STATION_NAME;

            string nextStation1 = sn.NextStation;
            if (!sn.CurrentStation.Equals("REWORK") && !sn.CurrentStation.Equals("MRB"))
            {
                if (R == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000156", new string[] { strSn, sn.CurrentStation }));
                }

                if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null && !sn.CurrentStation.Equals("REWORK"))
                {
                    if (!string.IsNullOrEmpty(sn.CurrentStation))
                    {
                        StrNextStation = sn.NextStation;
                    }
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000200", new string[] { strSn, StrNextStation }));
                }
                nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(nextStation1);
            if (R != null && R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            NextStationSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "NextStationList", "NextStation" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 通過CSN加載SN待過工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNNextStationByCSNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StrNextStation = "";
            MESStationSession NextStationSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (NextStationSave == null)
            {
                NextStationSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NextStationSave);
            }
            string strCsn = Input.Value.ToString();
            string strSn = string.Empty;
            R_SN_LINK objRSL = Station.SFCDB.ORM.Queryable<R_SN_LINK>().Where(t => t.CSN == strCsn && t.VALIDFLAG == "1").First();
            if (objRSL != null)
            {
                strSn = objRSL.SN;
            }
            else
            {
                //throw new Exception("輸入CSN在R_SN_LINK表記錄!");
                strSn = strCsn;
            }
            SN sn = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == sn.CurrentStation).FirstOrDefault();

            string nextStation1 = sn.NextStation;
            if (!sn.CurrentStation.Equals("REWORK"))
            {
                if (R == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000156", new string[] { strSn, sn.CurrentStation }));
                }

                if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null && !sn.CurrentStation.Equals("REWORK"))
                {
                    if (!string.IsNullOrEmpty(sn.CurrentStation))
                    {
                        StrNextStation = sn.NextStation;
                    }
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000200", new string[] { strSn, StrNextStation }));
                }
                nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(nextStation1);
            if (R != null && R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            NextStationSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "NextStationList", "NextStation" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        //add by LLF 2018-01-26
        //加載Panel待過工站
        //modify by ZGJ 2018-03-07
        public static void PanelSNNextStationDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //MESStationSession WipStationSave = new MESStationSession() { MESDataType = "WIPSTATION", InputValue = Input.Value.ToString(), SessionKey = "1", ResetInput = Input };
            MESStationSession NextStationSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (NextStationSave == null)
            {
                NextStationSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NextStationSave);
            }
            string PanelSn = Input.Value.ToString();
            T_R_PANEL_SN table = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            NextStationSave.Value = new List<string>() { table.GetPanelNextStation(PanelSn, Station.SFCDB) };
            //SN sn = new SN();

            //sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            //Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //List<string> snStationList = new List<string>();
            //List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            //RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == sn.CurrentStation).FirstOrDefault();
            ////Modify By LLF 2018-01-29
            ////string nextStation1 = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO + 10).FirstOrDefault().STATION_NAME;
            //string nextStation1 = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            //snStationList.Add(nextStation1);
            //if (R.DIRECTLINKLIST != null)
            //{
            //    foreach (var item in R.DIRECTLINKLIST)
            //    {
            //        snStationList.Add(item.STATION_NAME);
            //    }
            //}
            Station.AddMessage("MES00000029", new string[] { "NextStationList", "NextStation" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 加載工站Pass下一站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// add by LLF 2018-01-29
        public static void StationNextDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string NextStation = "";
            MESStationSession StationNextSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StationNextSave == null)
            {
                StationNextSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationNextSave);
            }
            string strSn = Input.Value.ToString();
            SN sn = new SN();

            //Marked by LLF 2018-02-22 begin
            //sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            try
            {
                sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            catch
            {

                sn.Load(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            //Marked by LLF 2018-02-22 end

            Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();

            if (R == null)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180724154541", new string[] { Station.StationName }));

            if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null)//當前工站為最後一個工站時
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO).FirstOrDefault().STATION_TYPE;
            }
            else
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(NextStation);
            if (R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            StationNextSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "StationNext", "StationNextList" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 通過CSN加載工站Pass下一站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// add by ZHB 20200718
        public static void StationNextByCSNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string NextStation = "";
            MESStationSession StationNextSave = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StationNextSave == null)
            {
                StationNextSave = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationNextSave);
            }
            string strCsn = Input.Value.ToString();
            string strSn = string.Empty;
            R_SN_LINK objRSL = Station.SFCDB.ORM.Queryable<R_SN_LINK>().Where(t => t.CSN == strCsn && t.VALIDFLAG == "1").First();
            if (objRSL != null)
            {
                strSn = objRSL.SN;
            }
            else
            {
                //throw new Exception("輸入CSN在R_SN_LINK表記錄!");
                strSn = strCsn;
            }
            SN sn = new SN();

            //Marked by LLF 2018-02-22 begin
            //sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            try
            {
                sn.PanelSN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            catch
            {

                sn.Load(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            //Marked by LLF 2018-02-22 end

            Route routeDetail = new Route(sn.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();

            if (R == null)
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180724154541", new string[] { Station.StationName }));

            if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null)//當前工站為最後一個工站時
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO).FirstOrDefault().STATION_TYPE;
            }
            else
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(NextStation);
            if (R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            StationNextSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "StationNext", "StationNextList" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// By工單Route獲取下一工站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void StationNextByWODataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string NextStation = "";
            WorkOrder WoObj = new WorkOrder();

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession StationNextSave = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNextSave == null)
            {
                StationNextSave = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationNextSave);
            }

            WoObj = (WorkOrder)WOSession.Value;
            Route routeDetail = new Route(WoObj.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            List<string> snStationList = new List<string>();
            List<RouteDetail> routeDetailList = routeDetail.DETAIL;
            RouteDetail R = routeDetailList.Where(r => r.STATION_NAME == Station.StationName).FirstOrDefault();

            if (routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault() == null)//當前工站為最後一個工站時
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO == R.SEQ_NO).FirstOrDefault().STATION_TYPE;
            }
            else
            {
                NextStation = routeDetailList.Where(r => r.SEQ_NO > R.SEQ_NO).FirstOrDefault().STATION_NAME;
            }

            snStationList.Add(NextStation);

            if (R.DIRECTLINKLIST != null)
            {
                foreach (var item in R.DIRECTLINKLIST)
                {
                    snStationList.Add(item.STATION_NAME);
                }
            }
            StationNextSave.Value = snStationList;
            Station.AddMessage("MES00000029", new string[] { "StationNext", "StationNextList" }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }


        /// <summary>
        /// 從SN對象LOADER REPAIR BACK STATION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetBackStationDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession stationListSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (stationListSession == null)
            {
                stationListSession = new MESStationSession { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, InputValue = "", ResetInput = Input };
                Station.StationSession.Add(stationListSession);
            }
            SN snObj = (SN)snSession.Value;
            List<string> stationList = new List<string>();
            List<string> list = new List<string>();
            list = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(route => route.ROUTE_ID == snObj.RouteID && SqlSugar.SqlFunc.Subqueryable<C_TEMES_STATION_MAPPING>()
                        .Where(map => map.MES_STATION == route.STATION_NAME).Any()).OrderBy(route => route.SEQ_NO).Select(route => route.STATION_NAME).ToList();
            stationList.Add("");
            stationList.AddRange(list);
            stationListSession.Value = stationList;
        }

        /// <summary>
        /// 從第一個輸入框加載PanelSN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// Add by LLF 2018-02-01
        //public static void SNInputDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        //{
        //    string StrSN = Input.Value.ToString();

        //    MESStationSession SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
        //    Station.StationSession.Add(SNSession);

        //    SNSession.InputValue = Input.Value.ToString();
        //    SNSession.Value = StrSN;
        //}

        public static void SNLinkKeypartDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<Dictionary<string, string>> KPList = new List<Dictionary<string, string>>();
            int SeqNo = 0;
            KPList = null;
            if (Paras.Count != 5)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            string Sn = Input.Value.ToString();
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                SNSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNSession);
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                WOSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WOSession);
            }
            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SubKPSession == null)
            {
                SubKPSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubKPSession);
            }
            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (MainKPSession == null)
            {
                MainKPSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainKPSession);
            }
            MESStationSession KPListSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (KPListSession == null)
            {
                KPListSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(KPListSession);
            }

            KPListSession.Value = KPList;
            //SN sn = null;
            //WorkOrder wo = null;
            SN sn = new SN();
            WorkOrder wo = new WorkOrder();
            wo = (WorkOrder)WOSession.Value;
            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //sn.Load(Sn,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            //wo.Init(sn.WorkorderNo,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> keyparts = tck.GetKeypartListByWOAndStation(Station.SFCDB, wo.WorkorderNo, Station.StationName);
            if (keyparts.Count > 0)
            {
                SeqNo = (int)((C_KEYPART)keyparts[0]).SEQ_NO;
                SubKPSession.Value = keyparts.Where(s => s.SEQ_NO == SeqNo).ToList();
                MainKPSession.Value = keyparts.Where(s => s.SEQ_NO > SeqNo).ToList();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000190");
                throw new MESReturnMessage(errMsg);
            }
        }
        public static void MainKPCheck(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int SeqNo = 0;
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                WOSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WOSession);
            }
            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (MainKPSession == null)
            {
                MainKPSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainKPSession);
            }
            WorkOrder wo = new WorkOrder();
            wo = (WorkOrder)WOSession.Value;
            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //sn.Load(Sn,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            //wo.Init(sn.WorkorderNo,Station.SFCDB,DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> keyparts = tck.GetKeypartListByWOAndStation(Station.SFCDB, wo.WorkorderNo, Station.StationName);
            if (keyparts.Count > 0)
            {
                SeqNo = (int)((C_KEYPART)keyparts[0]).SEQ_NO;
                MainKPSession.Value = keyparts.Where(s => s.SEQ_NO > SeqNo).ToList();
            }


            if (((List<C_KEYPART>)MainKPSession.Value).Count == 0)
            {
                // throw new Exception("因產品需要上料，请去LIKE_PTH工站扫描!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814160913"));
            }

        }
        public static void LinkKeypartDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            string Sn = Input.Value.ToString();
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                WOSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WOSession);
            }

            MESStationSession SubKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SubKPSession == null)
            {
                SubKPSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SubKPSession);
            }
            MESStationSession MainKPSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (MainKPSession == null)
            {
                MainKPSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(MainKPSession);
            }
            SN sn = null;
            WorkOrder wo = null;
            T_C_KEYPART tck = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            sn.Load(Sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            wo.Init(sn.WorkorderNo, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_KEYPART> keyparts = tck.GetKeypartListByWOAndStation(Station.SFCDB, wo.WorkorderNo, Station.StationName);
            SubKPSession.Value = keyparts.Where(s => s.SEQ_NO == 10).ToList();
            MainKPSession.Value = keyparts.Where(s => s.SEQ_NO != 10).ToList().OrderBy(s => s.SEQ_NO);
        }

        public static void PanelVitualSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string PanelSN = "";
            SN SNObj = new SN();
            R_PANEL_SN PanelObj = null;

            MESStationSession PANEL = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PANEL == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession VirtualSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (VirtualSN == null)
            {
                VirtualSN = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(VirtualSN);
            }

            VirtualSN.InputValue = Input.Value.ToString();
            VirtualSN.ResetInput = Input;
            PanelSN = PANEL.InputValue.ToString();
            PanelObj = SNObj.GetPanelVirtualSN(PanelSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            VirtualSN.Value = PanelObj;
        }


        /// <summary>
        /// 通過SN獲取r_sn_keypart_detail表的LINK信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BySnObjectGetLinkKeypartDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession LinkSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LinkSnSession == null)
            {
                LinkSnSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(LinkSnSession);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null)
            {
                throw new Exception("Can Not Find " + Paras[1].SESSION_TYPE + " 'Information ' !");
            }
            else
            {
                SN Objsn = (SN)SNSession.Value;
                T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_SN_KEYPART_DETAIL> KeyPartList = stk.GetKeypartBySN(Station.SFCDB, Objsn.SerialNo);

                LinkSnSession.Value = KeyPartList;

                Station.AddMessage("MES00000029", new string[] { "KeyPartList", KeyPartList.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 從輸入加載SN對象集合
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetSnObjectListDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionInputType = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionInputType.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            MESStationSession sessionInputString = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputString == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            if (sessionInputString.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            MESStationSession snObjectList = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_KEY && t.SessionKey == Paras[3].SESSION_KEY);
            if (snObjectList == null)
            {
                snObjectList = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(snObjectList);
            }
            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                string inputType = sessionInputType.Value.ToString();
                string inputString = sessionInputString.Value.ToString();
                List<R_SN> snList = new List<R_SN>();
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                if (inputType.Equals("SN"))
                {
                    T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                    snList.Add(t_r_sn.LoadSN(inputString, Station.SFCDB));
                }
                else if (inputType.Equals("PANEL"))
                {
                    T_R_PANEL_SN t_r_panel_sn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
                    snList = t_r_panel_sn.GetValidSnByPanel(inputString, Station.SFCDB);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));
                }
                snObjectList.Value = snList;
                Station.AddMessage("MES00000001", new string[] { sessionInputString.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 從Sn加載 stationName List
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetStationBySnDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionStation = new MESStationSession();
            SN SNObj = (SN)sessionSN.Value;
            List<string> stationList = new List<string>();
            T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            //List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(SNObj.RouteID, Station.SFCDB);
            List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(SNObj.RouteID, SNObj.NextStation, Station.SFCDB);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "ReturnStation");
            if (RouteDetails.Count > 0)
            {

                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (C_ROUTE_DETAIL c in RouteDetails)
                {
                    //退站方法並未對SILOADING做特殊處理，就直接UPDATE當前站下一站為SILOADING，不僅沒用反而是錯的，這裡增加條件排除掉，要退SILOADING暫時只能找IT手動退 Edit By ZHB 20200924
                    if (!c.STATION_NAME.Equals("BIP") && !c.STATION_NAME.Equals("SMTLOADING") && !c.STATION_NAME.Equals("SILOADING"))
                    {
                        stationInput.DataForUse.Add(c.STATION_NAME);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { SNObj.WorkorderNo }));
            }


            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);
        }

        /// <summary>
        /// 從Pallet加載 stationName List
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetStationByPalletDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Pallet == null || Pallet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            List<R_SN> listSN = new List<R_SN>();
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            R_PACKING objPack = TRP.GetPackingByPackNo(Pallet.InputValue, Station.SFCDB);
            if (objPack == null) throw new MESReturnMessage("Pallet " + Pallet.InputValue + " not found");
            listSN = TRP.GetSnListByPalletIDShippedNotyet(objPack.ID, Station.SFCDB);//Lấy danh sách SN theo PackNo
            if (listSN.Count == 0)
                throw new MESReturnMessage("SN not found in PackID " + objPack.ID);
            T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(listSN.FirstOrDefault().ROUTE_ID, Station.SFCDB);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "ReturnStation");
            if (RouteDetails.Count > 0)
            {

                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (C_ROUTE_DETAIL c in RouteDetails)
                {
                    //退站方法並未對SILOADING做特殊處理，就直接UPDATE當前站下一站為SILOADING，不僅沒用反而是錯的，這裡增加條件排除掉，要退SILOADING暫時只能找IT手動退 Edit By ZHB 20200924
                    if (c.STATION_NAME.Equals("CBS") || c.STATION_NAME.Equals("SHIPOUT"))
                    {
                        stationInput.DataForUse.Add(c.STATION_NAME);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { listSN.FirstOrDefault().WORKORDERNO }));
            }


            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);


        }

        /// <summary>
        /// GET Station by List SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetStationByListSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            List<R_SN> snList = new List<R_SN>();
            string lstSN = Input.Value.ToString();
            lstSN = $@"'{lstSN.Replace("\n", "',\n'")}'";
            if (lstSN.Length == 0)
            {
                throw new MESReturnMessage("Please Input list SN!");
            }
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            snList = t_r_sn.GetSnListByListSN(lstSN, Station.SFCDB);
            var result = snList.Select(p => p.ROUTE_ID).Distinct();
            if (result.Count() > 1)
            {
                throw new MESReturnMessage("Dont have multiple Route!");
            }
            var nextStation = snList.Select(p => p.NEXT_STATION).Distinct();
            if (nextStation.Count() > 1)
            {
                throw new MESReturnMessage("Dont have multiple NextStation!");
            }
            var currentStation = snList.Select(p => p.CURRENT_STATION).Distinct();
            if (currentStation.Count() > 1)
            {
                throw new MESReturnMessage("Dont have multiple Current Station!");
            }
            var currentWO = snList.Select(p => p.WORKORDERNO).Distinct();
            if (currentWO.Count() > 1)
            {
                throw new MESReturnMessage("Dont have multiple WorkOrder!");
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }
            WorkOrder ObjWorkorder = new WorkOrder();
            String SNLoadPoint = Input.Value.ToString();
            string WOSavePoint = null;

            try
            {
                WOSavePoint = currentWO.FirstOrDefault().ToString();
                ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Swo.Value = ObjWorkorder;
                Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw new MESReturnMessage("Not found WO infomation");

            }
            string next_group = snList[0].NEXT_STATION.ToString();
            T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(snList.FirstOrDefault().ROUTE_ID, next_group, Station.SFCDB);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "ReturnStation");
            if (RouteDetails.Count > 0)
            {

                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (C_ROUTE_DETAIL c in RouteDetails)
                {
                    //退站方法並未對SILOADING做特殊處理，就直接UPDATE當前站下一站為SILOADING，不僅沒用反而是錯的，這裡增加條件排除掉，要退SILOADING暫時只能找IT手動退 Edit By ZHB 20200924
                    if (!c.STATION_NAME.Equals("BIP") && !c.STATION_NAME.Equals("SMTLOADING") && !c.STATION_NAME.Equals("SILOADING") && !c.STATION_NAME.Equals("SHIPOUT"))
                    {
                        stationInput.DataForUse.Add(c.STATION_NAME);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { snList.FirstOrDefault().WORKORDERNO }));
            }


            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);


        }

        /// <summary>
        /// Valid status DN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckDNStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Pallet == null || Pallet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            List<R_SN> listSN = new List<R_SN>();
            List<C_ROUTE_DETAIL> listRoute = new List<C_ROUTE_DETAIL>();
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            R_PACKING objPack = TRP.GetPackingByPackNo(Pallet.InputValue, Station.SFCDB);
            T_R_DN_STATUS TRDS = new T_R_DN_STATUS(Station.SFCDB, Station.DBType);
            T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(Station.SFCDB, Station.DBType);

            listSN = TRP.GetSnListByPalletIDShippedNotyet(objPack.ID, Station.SFCDB);
            if (listSN.Count == 0)
            {
                throw new MESReturnMessage($@"The Pallet {Pallet.Value} Is Empty!");
            }
            R_SHIP_DETAIL objShipDetail;
            R_DN_STATUS objDNStatus;
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            string current_station = "";
            DateTime sysDateTime = TRS.GetDBDateTime(Station.SFCDB);
            listRoute = TCRD.GetLastStations(listSN.FirstOrDefault().ROUTE_ID, "SHIPOUT", Station.SFCDB).OrderByDescending(r => r.SEQ_NO).ToList();
            current_station = listRoute.FirstOrDefault().STATION_NAME;
            foreach (R_SN sn in listSN)
            {
                objShipDetail = TRSD.GetShipDetailBySN(Station.SFCDB, sn.SN);
                if (objShipDetail == null)
                {
                    throw new MESReturnMessage(sn.SN + " No Shipping Record!");
                }
                if (sn.NEXT_STATION != "SHIPFINISH" && sn.CURRENT_STATION != "SHIPOUT")
                {
                    throw new MESReturnMessage(sn.SN + " Hasn't Been Shipped Yet!");
                }

                objDNStatus = TRDS.GetStatusByNOAndLine(Station.SFCDB, objShipDetail.DN_NO, objShipDetail.DN_LINE);
                if (objDNStatus.DN_FLAG == "3")
                {
                    //做完GT不給退SHIPPING
                    throw new Exception("This " + objShipDetail.DN_NO + "," + objShipDetail.DN_LINE + " Has Done GT!");
                }
                else if (objDNStatus.DN_FLAG == "2")
                {
                    throw new Exception("This " + objShipDetail.DN_NO + "," + objShipDetail.DN_LINE + " Has Done CQA!. Already send data shipping to customer");
                }
            }
        }

        public static void GetStationByPalletToOBADataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Pallet == null || Pallet.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            List<R_SN> listSN = new List<R_SN>();
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
            R_PACKING objPack = TRP.GetPackingByPackNo(Pallet.InputValue, Station.SFCDB);
            if (objPack == null) throw new MESReturnMessage("Pallet " + Pallet.InputValue + " not found");
            listSN = TRP.GetSnListByPalletIDShippedNotyet(objPack.ID, Station.SFCDB);//Lấy danh sách SN theo PackNo
            if (listSN.Count == 0)
                throw new MESReturnMessage("SN not found in PackID " + objPack.ID);
            T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(listSN.FirstOrDefault().ROUTE_ID, Station.SFCDB);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "ReturnStation");
            if (RouteDetails.Count > 0 && stationInput != null)
            {

                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (C_ROUTE_DETAIL c in RouteDetails)
                {
                    //退站方法並未對SILOADING做特殊處理，就直接UPDATE當前站下一站為SILOADING，不僅沒用反而是錯的，這裡增加條件排除掉，要退SILOADING暫時只能找IT手動退 Edit By ZHB 20200924
                    if (c.STATION_NAME.Equals("OBA") || c.STATION_NAME.Equals("CBS"))
                    {
                        stationInput.DataForUse.Add(c.STATION_NAME);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { listSN.FirstOrDefault().WORKORDERNO }));
            }


            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);



        }

        /// <summary>
        /// 20190222 Patty added for FTX Oracle: only show available stations (before and includ current station)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ORAGetStationBySnDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }


            MESStationSession sessionStation = new MESStationSession();
            SN SNObj = (SN)sessionSN.Value;
            List<string> stationList = new List<string>();
            T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);

            //2019/06/17 Patty added: JOBFINISH (backflush done) cannot do reverse for now.
            if (SNObj.CompletedFlag == "1" && SNObj.StockStatus == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104922", new string[] { sessionSN.Value.ToString(), SNObj.CurrentStation }));
            }
            if (SNObj.ShippedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { sessionSN.Value.ToString() }));
            }
            if (SNObj.RepairFailedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104007", new string[] { sessionSN.Value.ToString() }));
            }
            //2019/03/29 Patty fixed bug for reverse, checking next station instead of current station

            List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(SNObj.RouteID, SNObj.NextStation, Station.SFCDB);
            MESStationInput stationInput = Station.Inputs.Find(t => t.DisplayName == "ReturnStation");

            if (RouteDetails == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000267", new string[] { sessionSN.Value.ToString(), SNObj.NextStation }));
            }

            if (RouteDetails.Count > 0)
            {

                stationInput.DataForUse.Clear();
                stationInput.DataForUse.Add("");
                foreach (C_ROUTE_DETAIL c in RouteDetails)
                {
                    if (!c.STATION_NAME.Equals("BIP") && c.STATION_NAME.ToUpper().IndexOf("LOADING") < 0)
                    {
                        stationInput.DataForUse.Add(c.STATION_NAME);
                    }
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000194", new string[] { SNObj.WorkorderNo }));
            }


            Station.AddMessage("MES00000001", new string[] { }, StationMessageState.Message);
        }

        public static void StationNoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            Dictionary<string, string> dic = (Dictionary<string, string>)Input.Value;
            string station_no = dic["Station_No"];
            MESStationSession StationNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_KEY && t.SessionKey == Paras[0].SESSION_KEY);
            if (StationNoSession == null)
            {
                StationNoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationNoSession);
            }
            T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_AP_TEMP ap = tap.GetMaxByStation_no(Station.SFCDB, station_no);
            StationNoSession.Value = ap;
        }

        /// <summary>
        /// 加载 SN 在这一站 要 link的keypart 资料
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SnKeypartsLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            SN Sn = (SN)SnSession.Value;

            T_R_SN_KEYPART_DETAIL TRSKD = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_KEYPART TCK = new T_C_KEYPART(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KEYPART_DETAIL> ScanedKeyparts = TRSKD.GetKeypartsBySN(Sn.SerialNo, Station.StationName, Station.SFCDB);

            List<C_KEYPART> ConfigedKeyparts = TCK.GetKeyPartBywo(Sn.WorkorderNo, Station.SFCDB).FindAll(t => t.STATION_NAME == Station.StationName);

            MESStationSession NextKeypartSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NextKeypartSession == null)
            {
                NextKeypartSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(NextKeypartSession);
            }


            if (ConfigedKeyparts.Count > 0)
            {
                if (ScanedKeyparts.Count > 0)
                {
                    string CategoryName = ScanedKeyparts.Last().CATEGORY_NAME;
                    C_KEYPART ConfigedKeypart = ConfigedKeyparts.Find(t => t.CATEGORY_NAME == CategoryName);
                    //如果 Keypart 数量不够的话，就继续 Link 当前 Keypart
                    if (ConfigedKeypart.QTY > ScanedKeyparts.FindAll(t => t.CATEGORY_NAME == CategoryName).Count)
                    {
                        NextKeypartSession.Value = CategoryName;
                    }
                    else
                    {
                        if (ConfigedKeyparts.Find(t => t.SEQ_NO > ConfigedKeypart.SEQ_NO) != null)
                        {
                            NextKeypartSession.Value = ConfigedKeyparts.Find(t => t.SEQ_NO > ConfigedKeypart.SEQ_NO).CATEGORY_NAME;
                        }
                        else
                        {
                            NextKeypartSession.Value = ConfigedKeyparts.First().CATEGORY_NAME;
                            Station.NextInput = Station.FindInputByName("MainSN");
                        }
                    }
                }
                else
                {
                    NextKeypartSession.Value = ConfigedKeyparts.First().CATEGORY_NAME;
                    //Station.NextInput = Station.FindInputByName("MainSN");
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000190"));
            }

        }


        /// <summary>
        /// 更新r_sn_kp资料
        /// add by hgb 2019.07.28
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void R_sn_kpUpdate(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            OleExec sfcdb = Station.SFCDB;
            sfcdb.BeginTrain();
            try
            {

                WorkOrder objWorkorder = new WorkOrder();
                DB_TYPE_ENUM sfcdbType = Station.DBType;

                objWorkorder = (WorkOrder)sessionWO.Value;
                //獲取後段產品的工單號，即上一槍輸入的工單

                T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                R_SN r_sn = new R_SN();
                r_sn = t_r_sn.LoadData(sessionSn.Value.ToString(), sfcdb);

                #region  寫入 r_sn_kp 


                OleExec SFCDB = Station.SFCDB;

                T_R_SN_KP TRKP = new T_R_SN_KP(sfcdb, DB_TYPE_ENUM.Oracle);
                List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(r_sn.ID, Station.StationName, sfcdb);
                List<R_SN_KP> kpwait = snkp.FindAll(T => T.VALUE == "" || T.VALUE == null);

                //還有KEYPART未掃描才更新r_sn_kp
                if (kpwait.Count > 0 || snkp.Count == 0)//snkp==0是沒有KP，需要更新
                {
                    //開始更新r_sn_kp
                    T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                    if (objWorkorder.KP_LIST_ID != null && objWorkorder.KP_LIST_ID.ToString() != "")
                    {
                        if (!c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { objWorkorder.WorkorderNo + " KP_LIST_ID" }));
                        }
                        SN snObject = new SN();
                        snObject.Load(sessionSn.Value.ToString(), sfcdb, sfcdbType);
                        //按工站刪除，再重新INSERT

                        if (snObject.DeleteR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType))
                        {
                            snObject.InsertR_SN_KP_bySTATION(objWorkorder, r_sn, sfcdb, Station, sfcdbType);

                        }

                    }
                    else
                    {
                        if (c_kp_list.GetListIDBySkuno(objWorkorder.SkuNO, sfcdb).Count != 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181101091946", new string[] { objWorkorder.SkuNO, objWorkorder.WorkorderNo }));
                        }
                    }
                }
                //else
                //{
                //   // throw new MESReturnMessage($@"本工站{Station.StationName}KP已掃描，如需重新過站，請退站打散綁定關係");
                //}
                sfcdb.CommitTrain();
                #endregion
            }
            catch (Exception ex)
            {
                sfcdb.RollbackTrain();
                throw ex;
            }
        }


        /// <summary>
        /// 產品從 B29 出來之後的站位
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void NextStationForB29Loader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            Dictionary<string, object> NextStations = new Dictionary<string, object>();
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NextStationSession == null)
            {
                NextStationSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY };
                Station.StationSession.Add(NextStationSession);
            }
            SN Sn = (SN)SnSession.Value;
            //if (Sn.CurrentStation.Equals("REWORK"))
            //{
            //    //如果是從 Rework 工站掃出的 B29，那麼就從 R_MRB 中找最近的
            //    R_SN rsn = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == Sn.SerialNo && t.VALID_FLAG == "0" && t.CURRENT_STATION != "REWORK").OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            //    if (rsn != null)
            //    {
            //        NextStations.Add("NextStations", new List<string>() { rsn.CURRENT_STATION });
            //    }
            //}
            //else
            //{
            //    NextStations = TCRD.GetNextStations(Sn.RouteID, Sn.CurrentStation, Station.SFCDB);
            //}
            //從 R_MRB 中找最近的一筆記錄的 NextStation 作為B29出去的站位
            T_R_MRB TRM = new T_R_MRB(Station.SFCDB, Station.DBType);
            R_MRB mrb = TRM.GetMrbBySN(Sn.SerialNo, Station.SFCDB).OrderByDescending(t => t.EDIT_TIME).FirstOrDefault();
            if (mrb == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181022144317", new string[] { Sn.SerialNo }));
            }
            NextStationSession.Value = mrb.NEXT_STATION;
            //if (NextStations.ContainsKey("NextStations"))
            //{
            //    List<string> Stations = (List<string>)NextStations["NextStations"];
            //    if (Stations.Count > 0)
            //    {
            //        NextStationSession.Value = Stations[0];
            //    }
            //    else
            //    {
            //        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102112716", new string[] { Sn.SerialNo, Sn.CurrentStation }));
            //    }
            //}
            //else
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190102112716", new string[] { Sn.SerialNo, Sn.CurrentStation }));
            //}



        }

        /// <summary>
        /// Use SN to check if need to print label 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// add by patty 2019/02/09
        public static void IsRunbySNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession StationIsRun = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StationIsRun == null)
            {
                StationIsRun = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StationIsRun);
            }
            string strSn = Input.Value.ToString();
            SN SN = new SN(strSn, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            string strWO = SN.WorkorderNo;
            WorkOrder WO = new WorkOrder();
            WO.Init(strWO, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (WO.SKU_SERIES.Contains("2C") || WO.SKU_SERIES.Contains("-8")) //20190306 patty added: for X8-8 product
            {
                StationIsRun.Value = "TRUE";
            }
            else
            {
                StationIsRun.Value = "FALSE";
            }


            Station.AddMessage("MES00000029", new string[] { "StationIsRun", StationIsRun.Value.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
        }

        /// <summary>
        /// 根據BOXSN 或SN 取得真實SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetActualSNDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            T_R_SN TRS = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_SN sn = TRS.LoadData(sessionSN.Value.ToString(), Station.SFCDB);
            if (sn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "SN:" + sessionSN.Value.ToString() }));
            }
            else
            {
                sessionSN.Value = sn.SN;
            }
        }
        /// <summary>
        /// 獲取SN鎖定信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetSNLockInfoDataloader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionLockInfo = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionLockInfo == null)
            {
                sessionLockInfo = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionLockInfo);
            }
            SN objSN = (SN)sessionSN.Value;
            T_R_SN_LOCK TRSL = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
            sessionLockInfo.Value = TRSL.GetLockObject("", "", objSN.SerialNo, "", "", "", Station.SFCDB);
        }
        /// <summary>
        /// HWT OBA 工站掃描SN記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTOBASNScanListLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSNList = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSNList == null)
            {
                sessionSNList = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = "", SessionKey = Paras[0].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionSNList);
            }
            MESStationSession sessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPassQty == null)
            {
                sessionPassQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = "", SessionKey = Paras[1].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionPassQty);
            }

            MESStationSession sessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionFailQty == null)
            {
                sessionFailQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = "", SessionKey = Paras[2].SESSION_KEY, ResetInput = null };
                Station.StationSession.Add(sessionFailQty);
            }

            MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(Station.SFCDB, Station.DBType);
            DataTable dt = TROT.GetTableByTypeAndIP(Station.SFCDB, "SN", Station.IP);

            List<MESDataObject.Module.HWT.R_OBA_TEMP> listSN = TROT.GetListByTypeAndIP(Station.SFCDB, "SN", Station.IP);
            sessionPassQty.Value = listSN.Where(r => r.STATUS == "PASS").ToList().Count;
            sessionFailQty.Value = listSN.Where(r => r.STATUS == "FAIL").ToList().Count;
            sessionSNList.Value = dt;
        }

        public static void RMAReasonLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SnSession = Station.GetStationSession(Paras[0]);
            MESStationSession RMAReasonCodeSession = Station.GetOrAddStationSession(Paras[1]);
            MESStationSession RMAReasonSession = Station.GetOrAddStationSession(Paras[2]);

            string Sn = SnSession.Value.ToString();
            T_R_RMA_CHECKIN TRRC = new T_R_RMA_CHECKIN(Station.SFCDB, Station.DBType);
            R_RMA_CHECKIN CheckIn = TRRC.GetBySn(Sn, Station.SFCDB).FirstOrDefault();
            if (CheckIn != null)
            {

                RMAReasonCodeSession.Value = CheckIn.CheckinReason;
                T_C_REASON_CODE TCRC = new T_C_REASON_CODE(Station.SFCDB, Station.DBType);
                C_REASON_CODE ReasonCode = TCRC.GetObjByReasonCode(Station.SFCDB, CheckIn.CheckinReason);
                RMAReasonSession.Value = ReasonCode == null ? "" : ReasonCode.CHINESE_DESCRIPTION;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200401144053", new string[] { Sn }));
            }
        }

        public static void MRBNextStationLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionNextStation = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionNextStation == null)
            {
                sessionNextStation = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = null, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionNextStation);
            }
            MESStationSession sessionWO = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            List<R_Station_Action_Para> listException = Paras.FindAll(t => t.SESSION_TYPE.Equals("ExceptionStation"));
            List<string> exceptionStation = listException.Select(r => r.VALUE.Trim()).ToList<string>();

            SN snObject = (SN)sessionSN.Value;
            WorkOrder woObject = (WorkOrder)sessionWO.Value;
            R_MRB mrbObj = Station.SFCDB.ORM.Queryable<R_MRB>().Where(r => r.SN == snObject.SerialNo && SqlSugar.SqlFunc.IsNullOrEmpty(r.REWORK_WO))
                       .OrderBy(r => r.CREATE_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (mrbObj == null)
            {
                throw new Exception(snObject.SerialNo + " Not In MRB!");
            }
            if (woObject.START_STATION.IndexOf("LOADING") < 0)
            {
                //VT 如果生管在手動轉重工工單時配置了起始工站，且起始工站不是LOADING工站，則掃入這個工單的SN從生管指定的工站開始重工                    
                sessionNextStation.Value = woObject.START_STATION;
            }
            else
            {
                if (!exceptionStation.Contains(mrbObj.NEXT_STATION))
                {
                    sessionNextStation.Value = mrbObj.NEXT_STATION;
                }
            }
        }

        /// <summary>
        /// 從Packing對象加載SNList
        /// </summary>
        public static void SNListFromPackingDataLoader(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionPacking = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionPacking == null || sessionPacking.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionSNList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSNList == null)
            {
                sessionSNList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(sessionSNList);
            }
            List<SN> snListObject = new List<SN>();
            LogicObject.Packing packingObject = (LogicObject.Packing)sessionPacking.Value;
            for (int i = 0; i < packingObject.SNList.Count; i++)
            {
                SN snObject = new SN(packingObject.SNList[i].ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
                snListObject.Add(snObject);
            }
            sessionSNList.Value = snListObject==null?throw new Exception("Get SNList Object From PackNo Error!"): snListObject;
        }

        /// <summary>
        /// ARUBA有個神邏輯：MES系統SN不帶PN後綴,Label和測試記錄帶PN後綴,加個邏輯：如果是ARUBA的就Like查詢
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ArubaSubStr(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strInput = Input.Value.ToString();
            int position = strInput.IndexOf(' ');
            if (position <= 0) { return; }
            strInput = strInput.Substring(0, position);

            var customer = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE, C_SKU, C_SERIES, C_CUSTOMER>((r, wo, sku, se, cus) => r.WORKORDERNO == wo.WORKORDERNO && wo.SKUNO == sku.SKUNO && sku.C_SERIES_ID == se.ID && se.CUSTOMER_ID == cus.ID)
                                                .Where((r, wo, sku, se, cus) => r.SN == strInput && r.VALID_FLAG == "1" && cus.CUSTOMER_NAME.ToUpper() == MESDataObject.Constants.Customer.ARUBA.Ext<EnumExtensions.EnumValueAttribute>().Description)
                                                .Select((r, wo, sku, se, cus) => cus).ToList();
            if (customer != null && customer.Count > 0)
            {
                Input.Value = strInput;
            }
        }
    }
}
