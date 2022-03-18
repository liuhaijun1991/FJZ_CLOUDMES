using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using MESDataObject;
using MESDataObject.Common;
using MESStation.LogicObject;

namespace MESStation.Stations.StationActions.DataCheckers
{
    public class CheckKP
    {
        public static void SNStationKPDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            var ShareDB = Paras.Find(t => t.SESSION_TYPE.ToUpper() == "SHAREDB");

            OleExec SFCDB = Station.SFCDB;
            OleExec APDB = Station.APDB;
            int ScanTimeOut = 1800000;

            SN sn = (SN)SNSession.Value;
            T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(sn.ID, Station.StationName, SFCDB);

            List<R_SN_KP> kpwait = snkp.FindAll(T => T.VALUE == "" || T.VALUE == null);
            if (kpwait.Count > 0)
            {
               
                
                Station.AddKPScan(sn.SerialNo, sn.WorkorderNo, Station.StationName);
                //更新底層KP彈窗模式:KP不再以拋出異常的方式彈窗，變更為中斷彈窗，KP掃描默認作業時間為1200秒超時--add by Eden 2020/04/18
                UIInputData I = new UIInputData() { Timeout = ScanTimeOut, ErrMessage = $@" {sn.SerialNo} {Station.StationName} Keypart scan has not completed! ", ReturnData = Station };
                if (ShareDB != null)
                {
                    if (Station.SFCDB.PoolItem.DBPool.ShareDB.Keys.Contains(sn.baseSN.SN))
                    {
                        throw new Exception("SN Can't be scan in diff pc");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814135714"));
                    }
                    Station.SFCDB.PoolItem.DBPool.ShareDB.Add(sn.baseSN.SN, SFCDB);
                }
                try
                {


                    I.GetUiInput(Station.API, UIInput.KeyPart, Station);

                    var doublecheckkp = TRKP.GetKPRecordBySnIDStation(sn.ID, Station.StationName, SFCDB)
                        .FindAll(T => T.VALUE == "" || T.VALUE == null).Any();
                    if (doublecheckkp)
                        //throw new Exception($@" {sn.SerialNo} @{Station.StationName} Keypart scan has not completed!");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140154", new string[] { sn.SerialNo, Station.StationName }));
                }
                catch (Exception ee)
                {
                    throw ee;
                }
                finally
                {
                    if (ShareDB != null)
                    {
                        SFCDB.PoolItem.DBPool.ShareDB.Remove(sn.baseSN.SN);
                    }
                    //Station.SFCDB.PoolItem.DBPool.ShareDB.Remove(sn.baseSN.SN);
                }
            }
        }

        /// <summary>
        /// 檢查SN條碼或SN對象的KEYPART
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNStationKeypartDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            SN snObject = null;
            if (SNSession.Value is string)
            {
                snObject = new SN(SNSession.Value.ToString(), Station.SFCDB, Station.DBType);
            }
            else
            {
                snObject = (SN)SNSession.Value;
            }

            if (snObject == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { SNSession.Value.ToString() }));
            }

            T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(snObject.ID, Station.StationName, SFCDB);

            List<R_SN_KP> kpwait = snkp.FindAll(T => T.VALUE == "" || T.VALUE == null);
            if (kpwait.Count > 0)
            {
                //Station.AddKPScan(snObject.SerialNo, snObject.WorkorderNo, Station.StationName);
                //throw new Exception($@"{snObject.SerialNo} 缺少Keypart");

                Station.AddKPScan(snObject.SerialNo, snObject.WorkorderNo, Station.StationName);
                //更新底層KP彈窗模式:KP不再以拋出異常的方式彈窗，變更為中斷彈窗，KP掃描默認作業時間為1200秒超時--add by Eden 2020/04/18
                UIInputData I = new UIInputData() { Timeout = 1200000, ErrMessage = $@" {snObject.SerialNo} {Station.StationName} Keypart scan has not completed! ", ReturnData = Station };
                I.GetUiInput(Station.API, UIInput.KeyPart, Station);
                var doublecheckkp = TRKP.GetKPRecordBySnIDStation(snObject.ID, Station.StationName, SFCDB)
                    .FindAll(T => T.VALUE == "" || T.VALUE == null).Any();
                if (doublecheckkp)
                    //throw new Exception($@" {snObject.SerialNo} @{Station.StationName} Keypart scan has not completed!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814140154", new string[] { snObject.SerialNo, Station.StationName }));
            }
        }

        /// <summary>
        /// 檢查R_SN_KP的KP與機種KP是否一致
        /// add by hgb 2019.07.30
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckKpBySnAndSku(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM sfcdbType = Station.DBType;
            T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
            R_SN r_sn = new R_SN();
            r_sn = t_r_sn.LoadData(sessionSn.Value.ToString(), sfcdb);

            string strSql = $@"
                SELECT *
                 FROM c_ort_sampling_sku
                WHERE sampling_type IN
                      (SELECT TYPE FROM c_ort_sampling WHERE SAMPLING_QTY = 100)
                 AND skuno = '{r_sn.SKUNO}' ";

            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                strSql = $@"
                SELECT *
                  FROM r_ort_sampling_wo
                 WHERE WO =  '{r_sn.WORKORDERNO}'  ";

                dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    strSql = $@" SELECT SAMPLING_TOTAL / WOQTY * 100
                              FROM R_ORT_SAMPLING_WO
                             WHERE WO =  '{r_sn.WORKORDERNO}' ";

                    dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                    if (Convert.ToInt32(dt.Rows[0][0].ToString()) >= 100)//--說明為百分之百抽測
                    {
                        T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                        //除了RMA工單，其他工單都要卡是否有ort記錄
                        if (!WOType.IsTypeInput("RMA", r_sn.WORKORDERNO.Substring(0, 6), sfcdb))
                        {
                            T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                            bool test = t_r_test_record.CheckTestBySNAndStation2(r_sn.ID, "ORT", sfcdb);
                            if (!test)//沒有pass記錄報錯
                            {
                                //throw new MESReturnMessage($@"機種{r_sn.SKUNO}抽測比例為百分之百,此SN沒有ORT測試PASS記錄,請重測ORT");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163703", new string[] { r_sn.SKUNO }));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 檢查100%抽測ORT的PASS記錄   
        /// add by hgb 2019.07.30
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckOrtPassRecord(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM sfcdbType = Station.DBType;
            T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
            R_SN r_sn = new R_SN();
            r_sn = t_r_sn.LoadData(sessionSn.Value.ToString(), sfcdb);

            string strSql = $@"
                SELECT *
                 FROM c_ort_sampling_sku
                WHERE sampling_type IN
                      (SELECT TYPE FROM c_ort_sampling WHERE SAMPLING_QTY = 100)
                 AND skuno = '{r_sn.SKUNO}' ";

            DataTable dt = Station.SFCDB.RunSelect(strSql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                strSql = $@"
                SELECT *
                  FROM r_ort_sampling_wo
                 WHERE WO =  '{r_sn.WORKORDERNO}' ";

                dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    strSql = $@"
                SELECT round (sampling_total / WOQTY * 100,2)
                  FROM r_ort_sampling_wo
                 WHERE WO =  '{r_sn.WORKORDERNO}' ";

                    dt = Station.SFCDB.RunSelect(strSql).Tables[0];
                    string aa = dt.Rows[0][0].ToString();

                    if (Convert.ToDouble(dt.Rows[0][0].ToString()) >= 100)//--說明為百分之百抽測
                    {
                        T_R_WO_TYPE WOType = new T_R_WO_TYPE(sfcdb, DB_TYPE_ENUM.Oracle);

                        //除了RMA工單，其他工單都要卡是否有ort記錄
                        if (!WOType.IsTypeInput("RMA", r_sn.WORKORDERNO.Substring(0, 6), sfcdb))
                        {
                            T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(sfcdb, DB_TYPE_ENUM.Oracle);
                            bool test = t_r_test_record.CheckTestBySNAndStation2(r_sn.ID, "ORT", sfcdb);
                            if (!test)//沒有pass記錄報錯
                            {
                                //throw new MESReturnMessage($@"機種{r_sn.SKUNO}抽測比例為百分之百,此SN沒有ORT測試PASS記錄,請重測ORT");
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814163703", new string[] { r_sn.SKUNO }));
                            }

                        }
                    }



                }

            }


        }


    }
}
