using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using MESDataObject;
using System.Data;

namespace MESStation.Stations.StationActions.DataLoaders
{
    public class WoFromAnyDataLoader
    {
        /// <summary>
        /// 從SNLoadPoint保存的SN對象加載工單對象到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,WO,SN保存的位置</param>
        public static void WoFromSNDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Ssn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Swo == null)
            {
                //throw new Exception("Can Not Fint " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            if (Ssn == null) 
            {
                //throw new Exception("请输入SN!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            else
            {
                SN ObjSN = (SN)Ssn.Value;
                WorkOrder ObjWorkorder = new WorkOrder();
                String SNLoadPoint = Input.Value.ToString();
                string WOSavePoint = null;

                try
                {
                    WOSavePoint = ObjSN.WorkorderNo.Trim();
                    ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    Swo.Value = ObjWorkorder;

                    Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    string msgCode = ex.Message;
                    throw ex;

                }
            }
        }

        /// <summary>
        /// 從棧板號加載工單對象到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,WO,SN保存的位置</param>
        public static void WoFromPalletDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Swo == null)
            {
                //throw new Exception("Can Not Fint " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            if (Pallet == null)
            {
                
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            else
            {
                var wo = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.PACK_NO == Pallet.Value
                   && a.ID == b.PARENT_PACK_ID && c.PACK_ID == b.ID && d.ID == c.SN_ID && d.VALID_FLAG == "1").Select((a, b, c, d) => d.WORKORDERNO).ToList().FirstOrDefault();

                WorkOrder ObjWorkorder = new WorkOrder();
                string WOSavePoint = null;

                try
                {
                    WOSavePoint = wo;
                    ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    Swo.Value = ObjWorkorder;

                    Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    string msgCode = ex.Message;
                    throw ex;

                }
            }
        }

        /// <summary>
        /// Load WO_BASE workorder QTY and Finished QTY
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1 parameter </param>
        public static void WOQtyFromWODataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {

            if (Paras.Count < 3)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));

            }
            
            
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
               
            }

            MESStationSession WOQTY = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOQTY == null)
            {
                WOQTY = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(WOQTY);
            }

            MESStationSession FINISHEDQTY = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (FINISHEDQTY == null)
            {
                FINISHEDQTY = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(FINISHEDQTY);
            }


            WorkOrder WOObject = (WorkOrder)WOSession.Value;
            T_R_SN_STATION_DETAIL T_R_SN_STATION_DETAIL = new T_R_SN_STATION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            try
            {
                
                WOQTY.Value = WOObject.WORKORDER_QTY;
                //patty 20190214 changed displayed qty to show qty passed the current station.
                FINISHEDQTY.Value = T_R_SN_STATION_DETAIL.GetCountByWOAndStation(WOObject.WorkorderNo, Station.StationName, Station.SFCDB).ToString();


            }
            catch (Exception e)
            {               
                throw e;
            }


        }
        /// <summary>
        /// 從PanelLoadPoint保存的Panel對象加載工單對象到指定位置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">2個參數,WO,Panle保存的位置</param>
        public static void WoFromPanelDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession Spanel = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            if (Spanel == null)
            {
                //throw new Exception("请输入PANEL!");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000171", new string[] { "PANEL" }));
            }
            else
            {
                Panel ObjPanel = (Panel)Spanel.Value;
                WorkOrder ObjWorkorder = new WorkOrder();
                string PanelLoadPoint = Input.Value.ToString();
                string WOSavePoint = null;

                try
                {
                    if (ObjPanel.PanelCollection.Count != 0)
                    {
                        WOSavePoint = ObjPanel.PanelCollection[0].WORKORDERNO.ToString();
                    }
                    else
                    {
                        //throw new Exception("Can Not Find " + PanelLoadPoint + " 'Information ' !");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { PanelLoadPoint }));
                    }
                    ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    if (ObjWorkorder == null)
                    {
                        //throw new Exception("Can Not Find " + WOSavePoint + " 'Information ' !");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { WOSavePoint }));
                    }
                    Swo.Value = ObjWorkorder;

                    Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    string msgCode = ex.Message;
                    throw ex;

                }
            }
        }

        /// <summary>
        /// 從輸入加載工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void WoDataloader(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //T_R_WO_DEVIATION TRWD = null;
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }

            WorkOrder ObjWorkorder = new WorkOrder();
            string WOSavePoint = Input.Value.ToString();

            try
            {
                //TRWD = new T_R_WO_DEVIATION(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                ObjWorkorder.Init(WOSavePoint, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (ObjWorkorder == null)
                {
                    //throw new Exception("Can Not Find " + WOSavePoint + " 'Information ' !");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { WOSavePoint }));
                }
                Swo.Value = ObjWorkorder;
                Station.AddMessage("MES00000029", new string[] { "Workorder", WOSavePoint }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);

                if (Paras.Exists(t => t.SESSION_TYPE == "WOQTY" && t.SESSION_KEY == "1"))
                {
                    MESStationSession WOQTYSession = Station.StationSession.Find(t => t.MESDataType == "WOQTY" && t.SessionKey == "1");
                    if (WOQTYSession != null)
                    {
                        WOQTYSession.Value = ObjWorkorder.WORKORDER_QTY;
                    }
                    else
                    {
                        Station.StationSession.Add(new MESStationSession() { MESDataType = "WOQTY", SessionKey = "1", Value = ObjWorkorder.WORKORDER_QTY });
                    }
                }

                //MESStationSession DeviationSession = Station.StationSession.Find(t => t.MESDataType == "DEVIATION" && t.SessionKey == "1");
                //if (DeviationSession == null)
                //{
                //    DeviationSession = new MESStationSession() { MESDataType = "DEVIATION", SessionKey = "1" };
                //    Station.StationSession.Add(DeviationSession);
                //}
                //R_WO_DEVIATION Deviation = TRWD.GetDeviationByWo(WOSavePoint, Station.SFCDB);
                //if (Deviation!=null)
                //{
                //    DeviationSession.Value = Deviation.DEVIATION;
                //}
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;
            }
        }

        /// <summary>
        /// 從輸入的對象加載工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WoDataLoadingFromInputObject(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionInputObject = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInputObject == null || sessionInputObject.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }           
            
            MESStationSession sessionWOObject = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWOObject == null)
            {
                sessionWOObject = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionWOObject);
            }
            try
            {               
                string wo = "";
                WorkOrder ObjWorkorder = new WorkOrder();
                if (sessionInputObject.Value.GetType() == typeof(SN))
                {
                    SN snObject = (SN)sessionInputObject.Value;
                    wo = snObject.WorkorderNo;
                }
                else if (sessionInputObject.Value.GetType() == typeof(Panel))
                {
                    Panel panelObject = (Panel)sessionInputObject.Value;
                    wo = panelObject.Workorderno;
                }
                else
                {
                    wo = sessionInputObject.Value.ToString();
                }

                ObjWorkorder.Init(wo, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (ObjWorkorder == null)
                {                    
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000189", new string[] { wo }));
                }
                sessionWOObject.Value = ObjWorkorder;
                Station.AddMessage("MES00000029", new string[] { "Workorder", wo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void WoDataloader_AP(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //T_R_WO_DEVIATION TRWD = null;
            //  if (Paras.Count != 3)
            //{
            //    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            //}
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }
            MESStationSession sessionWOQty = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_TYPE);
            if (sessionWOQty == null)
            {
                sessionWOQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionWOQty);
            }

            //MESStationSession sessionLine = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_TYPE);
            //if (sessionLine == null)
            //{
            //    sessionLine = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(sessionLine);
            //}
            WorkOrder ObjWorkorder = new WorkOrder();
            string WOSavePoint = Input.Value.ToString();
            DataTable dt, dt2;
            string SQL, SQL2;
            try
            {
                //var wo1= Station.APDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WOSavePoint ).ToList();
                SQL = $@"SELECT*FROM R_WO_BASE WHERE WORKORDERNO='{WOSavePoint}'";
                dt = new DataTable();
                dt = Station.SFCDB.ExecuteDataTable(SQL, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("WO NOT EXITS!");
                }
                else
                {
                    sessionWOQty.Value = dt.Rows[0]["WORKORDER_QTY"].ToString();
                    SQL = $@"SELECT*FROM R_WO_BASE WHERE WORKORDERNO='{WOSavePoint}'";
                    dt = new DataTable();
                    dt = Station.SFCDB.ExecuteDataTable(SQL, CommandType.Text, null);


                    if (dt.Rows.Count == 0)
                    {
                        throw new Exception("WO NOT EXITS!");
                    }
                    else if (dt.Rows[0]["closed_flag"].ToString() == "1")
                    {
                        throw new Exception("WO is closed!");
                    }
                }




                //              sessionLine.Value= dt.Rows[0]["WO_QTY"].ToString(); ;




                //MESStationSession DeviationSession = Station.StationSession.Find(t => t.MESDataType == "DEVIATION" && t.SessionKey == "1");
                //if (DeviationSession == null)
                //{
                //    DeviationSession = new MESStationSession() { MESDataType = "DEVIATION", SessionKey = "1" };
                //    Station.StationSession.Add(DeviationSession);
                //}
                //R_WO_DEVIATION Deviation = TRWD.GetDeviationByWo(WOSavePoint, Station.SFCDB);
                //if (Deviation!=null)
                //{
                //    DeviationSession.Value = Deviation.DEVIATION;
                //}
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;
            }
        }
        public static void SNDataloader_RESI(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //T_R_WO_DEVIATION TRWD = null;
            //  if (Paras.Count != 3)
            //{
            //    throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            //}
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                Swo = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(Swo);
            }
            MESStationSession sessionWOQty = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_TYPE);
            if (sessionWOQty == null)
            {
                sessionWOQty = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionWOQty);
            }

            MESStationSession sessionSLOT = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_TYPE);
            if (sessionSLOT == null)
            {
                sessionSLOT = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionSLOT);
            }

            MESStationSession sessionLINE = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_TYPE);
            if (sessionLINE == null)
            {
                sessionLINE = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionLINE);
            }

            //MESStationSession sessionLine = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_TYPE);
            //if (sessionLine == null)
            //{
            //    sessionLine = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
            //    Station.StationSession.Add(sessionLine);
            //}
            WorkOrder ObjWorkorder = new WorkOrder();
            string WOSavePoint = Input.Value.ToString();
            DataTable dt;
            string SQL;
            try
            {
                //var wo1= Station.APDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == WOSavePoint ).ToList();
                SQL = $@"SELECT*FROM mes4.R_TR_SN WHERE TR_SN='{WOSavePoint}'";

                dt = new DataTable();
                dt = Station.APDB.ExecuteDataTable(SQL, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("SN NOT EXITS!");
                }

                else
                {

                    SQL = $@"SELECT*FROM mes4.R_TR_SN_WIP WHERE TR_SN='{WOSavePoint}'";
                    dt = new DataTable();
                    dt.Clear();
                    dt = Station.APDB.ExecuteDataTable(SQL, CommandType.Text, null);

                    if (dt.Rows.Count > 0)
                    {
                        string work_flag;
                        work_flag = dt.Rows[0]["work_flag"].ToString();
                        Swo.Value = dt.Rows[0]["wo"].ToString();
                        sessionLINE.Value = dt.Rows[0]["STATION"].ToString();

                        if (work_flag == "1")
                        {
                            throw new Exception("TR SN is online, can't return!");
                        }





                    }

                    else
                    {
                        throw new Exception("NO TR SN in WIP!");
                    }
                }



                //              sessionLine.Value= dt.Rows[0]["WO_QTY"].ToString(); ;




                //MESStationSession DeviationSession = Station.StationSession.Find(t => t.MESDataType == "DEVIATION" && t.SessionKey == "1");
                //if (DeviationSession == null)
                //{
                //    DeviationSession = new MESStationSession() { MESDataType = "DEVIATION", SessionKey = "1" };
                //    Station.StationSession.Add(DeviationSession);
                //}
                //R_WO_DEVIATION Deviation = TRWD.GetDeviationByWo(WOSavePoint, Station.SFCDB);
                //if (Deviation!=null)
                //{
                //    DeviationSession.Value = Deviation.DEVIATION;
                //}
            }
            catch (Exception ex)
            {
                string msgCode = ex.Message;
                throw ex;
            }
        }
    }
}
