using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESStation.LogicObject;
using MESDBHelper;
using System.Data;
using MESDataObject.Module.Vertiv;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class SkuCheckers
    {
        /// <summary>
        /// 檢查輸入的料號與工單的料號是否一致
        /// 張官軍 2018/01/18
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSkuWoSkuChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string SkuNo = string.Empty;
            //marked by LLF 2018-02-24
            //MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            //if (WoSession == null)
            //{
            //    MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == "SN" && t.SessionKey == "1");
            //    if (SNSession != null)
            //    {
            //        SN Sn = ((SN)SNSession.Value);
            //        WorkOrder WoTemp = new WorkOrder();
            //        WoTemp.Init(Sn.WorkorderNo, Station.SFCDB);
            //        WoSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, SessionKey = Paras[0].SESSION_KEY, Value = WoTemp };
            //        Station.StationSession.Add(WoSession);
            //    }
            //    else
            //    {
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] {Paras[0].SESSION_TYPE+Paras[0].SESSION_KEY });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //}
            //WorkOrder Wo = ((WorkOrder)WoSession.Value);

            //SkuNo = Input.Value.ToString().ToUpper().Trim();
            //if (Wo != null)
            //{
            //    if (Wo.SkuNO.Equals(SkuNo))
            //    {
            //        Station.AddMessage("MES00000111", new string[] { SkuNo }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            //    }
            //    else
            //    {
            //        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000110", new string[] {SkuNo,Wo.WorkorderNo });
            //        throw new MESReturnMessage(ErrMessage);
            //    }
            //}

            MESStationSession InputSKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession SNSKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);

            if (InputSKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (SNSKUSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (InputSKUSession.Value.ToString() != SNSKUSession.Value.ToString() && InputSKUSession.InputValue.ToString() != SNSKUSession.Value.ToString())
            {
                //誰把這段防呆屏蔽了？Openned by LLF 2018-03-17
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000110", new string[] { InputSKUSession.Value.ToString(), SNSKUSession.Value.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
        }

        public static void CheckSkuIsExist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180904013101")); 
            }
            MESStationSession verSession= new MESStationSession { MESDataType = "VERSION", SessionKey = "1", InputValue = "", ResetInput = Input };
           Station.StationSession.Add(verSession);

            MESStationSession skuSession1 = new MESStationSession { MESDataType = "SKUNO", SessionKey = "1", InputValue = "", ResetInput = Input };
            Station.StationSession.Add(skuSession1);

            //判断逻辑
            var sku = Station.SFCDB.ORM.Queryable<C_SKU>().Where(c => c.SKUNO == skuSession.Value.ToString()).ToList().FirstOrDefault();

            if (sku == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180904013207", new string[] { skuSession.Value.ToString() }));
            }
            else
            {
                skuSession.Value = sku;
                skuSession1.Value = sku.SKUNO;
                verSession.Value = sku.VERSION;
            }

        }

        public static void CheckInputSkuExist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180904013101"));
            }
   
            var sku = Station.SFCDB.ORM.Queryable<C_SKU>().Where(c => c.SKUNO == skuSession.InputValue.ToString()).ToList().FirstOrDefault();
            if (sku == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180904013207", new string[] { skuSession.Value.ToString() }));
            }
            else
            {
                skuSession.Value = sku;
            }

        }

        /// <summary>
        /// 檢查軟件版本是否配置
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSkuSoft(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string Skuno="";
            string WO = "";
            int count;
            OleExec sfcdb = Station.SFCDB;
            DataTable dt = new DataTable();
            string strsql;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new Exception("skuSession missing,Call IT");
            }
            else
            {
                Skuno = skuSession.Value.ToString();
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null || WOSession.Value == null)
            {
                throw new Exception("WOSession missing,Call IT");
            }
            else
            {
                WO = WOSession.Value.ToString();
            }

            var softver = Station.SFCDB.ORM.Queryable<C_SKU_SOFT_CONFIG>().Where(t => t.P_NO == Skuno.ToString()).ToList().FirstOrDefault();
            if (softver == null)
            {
                //機種{0}未配置軟件版本，請聯繫PE！
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190820162652", new string[] { Skuno.ToString() }));               
            }

            try
            {
                if (Station.StationName.EndsWith("LOADING"))//SMTLOADING和SILOADING都要寫
                {
                    //strsql = $@"SELECT *
                    //             FROM c_sku_soft_config
                    //            WHERE soft_item_code <> 'NULL' AND p_no = '{Skuno}'";

                    //dt = sfcdb.ExecSelect(strsql).Tables[0];
                    //string a = "NULL";
                    //var SQL = Station.SFCDB.ORM.Queryable<C_SKU_SOFT_CONFIG>().Where(t => t.P_NO == Skuno && t.SOFT_ITEM_CODE != "NULL").ToSql();
                    count = Station.SFCDB.ORM.Queryable<C_SKU_SOFT_CONFIG>().Where(t => t.P_NO == Skuno).ToList().Count;//用ORM查詢的時候字符串NULL會被轉化為null來判斷

                    if (count > 0)
                    {
                        //每次都刪除并插入最新的
                      // Station.SFCDB.ORM.Deleteable<r_sn_soft>().Where(t => t.WO == WO).ExecuteCommand();

                        strsql = $@"INSERT INTO r_sn_soft
                                  (id,
                                   sn,
                                   skuno,
                                   wo,
                                   soft_item_code,
                                   soft_revision,
                                   soft_location,
                                   edit_emp)
                                  SELECT 'RSNSOFT' || to_char(SYSDATE, 'YYYYMMDDHH24MISS') || rownum,
                                         'BY_WO_CONTROL',
                                         '{Skuno}',
                                         '{WO}',
                                         soft_item_code,
                                         soft_revision,
                                         soft_location,
                                         '{Station.LoginUser.EMP_NO}'
                                    FROM c_sku_soft_config
                                   WHERE p_no = '{Skuno}'";
                        sfcdb.ThrowSqlExeception = true;
                        sfcdb.ExecSQL(strsql);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            finally
            {
                sfcdb.ThrowSqlExeception = false;
            }
            
        }

        /// <summary>
        /// 檢查機種是否配置Label
        /// </summary>
        public static void SkuPrintMacRuleChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSkuNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSkuNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionLabelType = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_TYPE);
            if (sessionLabelType == null)
            {
                sessionLabelType = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(sessionLabelType);
            }
            string skuNo = string.Empty;
            if (sessionSkuNo.Value.GetType() == typeof(SKU))
            {
                SKU skuObj = (SKU)sessionSkuNo.Value;
                skuNo = skuObj.SkuNo;
            }
            else
            {
                skuNo = sessionSkuNo.Value.ToString();
            }

            T_C_SKU_Label _Label = new T_C_SKU_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<C_SKU_Label> labelList = _Label.GetLabelConfigBySkuStation(skuNo, Station.StationName, Station.SFCDB);
            if (labelList == null || labelList.Count == 0)
            {
                //throw new Exception(skuNo + " 未配置工站Label![SKU Setting -> Label配置]");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814164854", new string[] { skuNo }));
            }
            sessionLabelType.Value = labelList[0].LABELTYPE;
        }
    }
}
