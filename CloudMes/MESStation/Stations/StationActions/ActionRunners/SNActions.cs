//using DcnSfcModel;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.ALLPART;
using MESDataObject.Module.BPD;
using MESDataObject.Module.DCN;
using MESDataObject.Module.HWT;
using MESDataObject.Module.Vertiv;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESStation.Label;
using MESPubLab.MESStation.MESReturnView.Station;
using MESPubLab.MESStation.SNMaker;
using MESStation.KeyPart;
using MESStation.LogicObject;
using Renci.SshNet;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using static MESDataObject.Common.EnumExtensions;
using MESDataObject.Module.OM;
using MES_DCN.Schneider;
using MESStation.Stations;
using MESPubLab.MesClient;
using Newtonsoft.Json.Linq;
using WebSocketSharp;
using MESDataObject.Module.Juniper;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class SNActions
    {

        public static void SNUnbondAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN sn = (SN)SNSession.Value;
            if (sn.baseSN.SCRAPED_FLAG == "1")
            {
                throw new Exception("This SN allready be SCRAPED");
            }
            sn.Unbond(Station.SFCDB, Station.LoginUser.EMP_NO, Station.BU, DB_TYPE_ENUM.Oracle);
        }

        public static void SNReworkReplaceAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            MESStationSession NewSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NewSNSession == null)
            {
                NewSNSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(NewSNSession);
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = (SN)SNSession.Value;
            List<R_SN_REPLACE> RSN = Station.SFCDB.ORM.Queryable<R_SN_REPLACE>().Where(t => t.LINKTYPE == "WaitReplace" && t.OLDSN == sn.baseSN.SN).ToList();
            if (RSN.Count > 0)
            {
                string OldSn = sn.baseSN.SN;
                bool isUsed = false;


                var NewSN = SNmaker.GetNextSN(SKU.SkuBase.SN_RULE, Station.DBS["SFCDB"]);


                //重碼檢查，若存在，則重新生成
                T_R_SN R_SN = new T_R_SN(Station.SFCDB, Station.DBType);
                isUsed = R_SN.IsUsed(NewSN, Station.SFCDB);
                if (isUsed)
                {
                    NewSN = SNmaker.GetNextSN(SKU.SkuBase.SN_RULE, Station.DBS["SFCDB"]);
                }

                string sqlupdatersn = $@" UPDATE  R_SN  set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterskp = $@" UPDATE  R_SN_KP  set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterslog = $@" UPDATE  R_SN_LOG  set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterssd = $@" UPDATE  R_SN_STATION_DETAIL  set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterm = $@" UPDATE  R_MRB set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterslink = $@" UPDATE  R_SN_LINK set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdatertb = $@" UPDATE  R_TEST_BRCD set SYSSERIALNO='{NewSN}' where SYSSERIALNO='{OldSn}'";
                string sqlupdatertr = $@" update R_TEST_RECORD set sn='{NewSN}' where sn='{OldSn}'";
                string sqlupdaterrm = $@" UPDATE  R_REPAIR_MAIN set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaternb = $@" UPDATE  R_NORMAL_BONEPILE set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterrt = $@" UPDATE  R_REPAIR_TRANSFER set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdatSNlock = $@" UPDATE  R_SN_LOCK set sn='{NewSN}' where sn='{OldSn}' and type = 'SN' ";
                string sqlupdate = $@" UPDATE  R_REPAIR_FAILCODE set sn='{NewSN}' where sn='{OldSn}' ";
                string sqlupdaterrs = $@" UPDATE  R_REPLACE_SN set NEW_SN='{NewSN}' where OLD_SN='{OldSn}' ";
                string sqlupdatewd1 = $@" UPDATE  WWN_DATASHARING set VSSN='{NewSN}',VSKU='{SKU.SkuBase.SKUNO}' WHERE VSSN='{OldSn}' ";
                string sqlupdatewd2 = $@" UPDATE  WWN_DATASHARING set CSSN='{NewSN}',CSKU='{SKU.SkuBase.SKUNO}' WHERE CSSN='{OldSn}' ";
                string sqlupdatersr = $@" UPDATE  R_SN_REPLACE set LINKTYPE='ReplaceOk', NEWSN='{NewSN}',STATION='{Station.StationName}',EDITTIME=sysdate,EDITBY='{Station.LoginUser.EMP_NO}' where  LINKTYPE='WaitReplace' and  OLDSN='{OldSn}' ";

                Station.SFCDB.ExecSQL(sqlupdatersn);
                Station.SFCDB.ExecSQL(sqlupdaterskp);
                Station.SFCDB.ExecSQL(sqlupdaterslog);
                Station.SFCDB.ExecSQL(sqlupdaterssd);
                Station.SFCDB.ExecSQL(sqlupdaterm);
                Station.SFCDB.ExecSQL(sqlupdaterslink);
                Station.SFCDB.ExecSQL(sqlupdatertb);
                Station.SFCDB.ExecSQL(sqlupdatertr);
                Station.SFCDB.ExecSQL(sqlupdaterrm);
                Station.SFCDB.ExecSQL(sqlupdaternb);
                Station.SFCDB.ExecSQL(sqlupdaterrt);
                Station.SFCDB.ExecSQL(sqlupdatSNlock);
                Station.SFCDB.ExecSQL(sqlupdate);
                Station.SFCDB.ExecSQL(sqlupdaterrs);
                Station.SFCDB.ExecSQL(sqlupdatewd1);
                Station.SFCDB.ExecSQL(sqlupdatewd2);
                Station.SFCDB.ExecSQL(sqlupdatersr);

                SN newSn = new SN(NewSN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                NewSNSession.Value = newSn;//SN對象，寫良率UPH這個Session取的是SN對象
                NewSNSession.InputValue = NewSN;//String類型
            }
            else
            {
                NewSNSession.Value = SNSession.Value;
            }
        }

        public static void MINI_LTT_SAMPLEAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = (SN)SNSession.Value;
            string Skuno = SKU.SkuBase.SKUNO;
            string Sn = sn.baseSN.SN;
            string Routeid = sn.baseSN.ROUTE_ID;
            T_R_FUNCTION_CONTROL _FUNCTION_CONTROL = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            List<R_FUNCTION_CONTROL_NewList> RFC = _FUNCTION_CONTROL.Get2ExListbyVarValue("MINI-LTT-SAMPLE", "MINI-LTT-SAMPLE", Skuno, Station.SFCDB);

            if (RFC.Count > 0)
            {
                int TotalQty = Convert.ToInt32(RFC[0].EXTVAL1);
                int SampleQty = Convert.ToInt32(RFC[0].EXTVAL2);
                T_R_SN_LOG _SN_LOG = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
                string SID = _SN_LOG.GetNewID(Station.BU, Station.SFCDB);

                var jumpSql = $@"SELECT B.STATION_NAME FROM C_ROUTE_DETAIL A, C_ROUTE_DETAIL B WHERE A.ROUTE_ID=B.ROUTE_ID
                                 AND A.ROUTE_ID='{Routeid}' AND A.STATION_NAME='MINI-LTT' AND B.SEQ_NO>A.SEQ_NO ORDER BY B.SEQ_NO";
                var jumpDT = Station.SFCDB.ExecuteDataTable(jumpSql, CommandType.Text, null);
                var JUMPEVENT = jumpDT.Rows[0][0];

                //報↓這個鬼錯，不懂是不是更新SqlSugar的坑，屏蔽換成SQL語句  Edit By ZHB 2020年8月22日14:47:57
                //English Message : Join C1 needs to be the same as OrderBy C2 Chinese Message : 多表查询存在别名不一致,请把OrderBy中的C2改成C1就可以了，特殊需求可以使用.Select((x,y)=>new{ id=x.id,name=y.name}).MergeTable().Orderby(xxx=>xxx.Id)功能将Select中的多表结果集变成单表，这样就可以不限制别名一样
                //var JUMPEVENT = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL, C_ROUTE_DETAIL>((C1, C2) => C1.ROUTE_ID == C2.ROUTE_ID).
                //    Where((C1, C2) => C1.ROUTE_ID == Routeid && C1.STATION_NAME == "MINI-LTT"
                //    && C2.SEQ_NO > C1.SEQ_NO).OrderBy(C2 => C2.SEQ_NO, OrderByType.Asc).Select(C2 => C2.STATION_NAME).First();
                List<R_SN_STATION_DETAIL> rssd = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().
                    Where(t => t.SKUNO == Skuno && t.STATION_NAME == "MINI-LTT" && t.REPAIR_FAILED_FLAG == "1" && t.VALID_FLAG == "1").
                    OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList();
                List<r_profile> rp600 = Station.SFCDB.ORM.Queryable<r_profile>().Where(t => t.PROFILENAME == Skuno && t.PROFILETYPE == "MINI-LTT-SAMPLE" && t.PROFILESORT == 600).ToList();
                List<r_profile> rp = Station.SFCDB.ORM.Queryable<r_profile>().Where(t => t.PROFILENAME == Skuno && t.PROFILETYPE == "MINI-LTT-SAMPLE").ToList();
                List<r_profile> rplevel = Station.SFCDB.ORM.Queryable<r_profile>().Where(t => t.PROFILENAME == Skuno && t.PROFILETYPE == "MINI-LTT-SAMPLE" && t.PROFILELEVEL == 1).ToList();
                if (rp.Count == 0)
                {
                    T_r_profile _Profile = new T_r_profile(Station.SFCDB, Station.DBType);
                    String PID = _Profile.GetNewID(Station.BU, Station.SFCDB);


                    string Strsql = $@" INSERT INTO r_profile
                                                            (id,profilename,profilecategory,profiletype,profilevalue,profiledesc,profilesort,profilelevel,edit_emp,edit_time)
                                                            values('{PID}','{Skuno}','','MINI-LTT-SAMPLE','1','','{TotalQty}','0','{Station.LoginUser.EMP_NO}',sysdate)";
                    Station.SFCDB.ExecSQL(Strsql);
                    string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                            VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','MINI-LTT-SAMPLE','{Station.StationName}','Y',sysdate,'{Station.LoginUser.EMP_NO}')";
                    Station.SFCDB.ExecSQL(Strsamplesn);
                }
                else
                {
                    //有掃FAIL之後要連續抽測600PCS
                    if (rssd.Count > 0 && rp600.Count == 0)
                    {
                        string StrUpdate = $@"update r_profile set profilevalue='1',profilesort='600',profilelevel='1', lasteditdt =sysdate where profilename='{Skuno}'";
                        Station.SFCDB.ExecSQL(StrUpdate);
                    }
                    if (rplevel.Count > 0)
                    {
                        int PASSQTY = Convert.ToInt32(rp[0].PROFILEVALUE) + 1; //此前掃過的+此時掃進來的才是用來判斷的數量
                        if (PASSQTY >= 600)
                        {
                            string StrUpdate = $@"update r_profile set profilevalue='1',profilesort='{TotalQty}',profilelevel='0', lasteditdt =sysdate where profilename='{Skuno}'";
                            Station.SFCDB.ExecSQL(StrUpdate);

                            string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                            VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','MINI-LTT-SAMPLE','{Station.StationName}','Y',sysdate,'{Station.LoginUser.EMP_NO}')";
                            Station.SFCDB.ExecSQL(Strsamplesn);
                        }
                        else
                        {
                            string StrUpdate = $@"update r_profile set profilevalue=TO_CHAR('{PASSQTY}'),EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='MINI-LTT-SAMPLE'";
                            Station.SFCDB.ExecSQL(StrUpdate);

                            string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                            VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','MINI-LTT-SAMPLE','{Station.StationName}','Y',sysdate,'{Station.LoginUser.EMP_NO}')";
                            Station.SFCDB.ExecSQL(Strsamplesn);
                        }
                    }
                    else
                    {
                        int PASSQTY = Convert.ToInt32(rp[0].PROFILEVALUE) + 1; //此前掃過的+此時掃進來的才是用來判斷的數量
                        if (SampleQty >= PASSQTY || PASSQTY > TotalQty)
                        {
                            if (PASSQTY > TotalQty)
                            {
                                string StrUpdate = $@"update r_profile set profilevalue='1',EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='MINI-LTT-SAMPLE'";
                                Station.SFCDB.ExecSQL(StrUpdate);
                            }
                            else
                            {
                                string StrUpdate = $@"update r_profile set profilevalue=TO_CHAR('{PASSQTY}'),EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='MINI-LTT-SAMPLE'";
                                Station.SFCDB.ExecSQL(StrUpdate);
                            }

                            string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                            VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','MINI-LTT-SAMPLE','{Station.StationName}','Y',sysdate,'{Station.LoginUser.EMP_NO}')";
                            Station.SFCDB.ExecSQL(Strsamplesn);
                        }
                        else
                        {
                            string StrUpdate = $@"update r_profile set profilevalue=TO_CHAR('{PASSQTY}'),EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='MINI-LTT-SAMPLE'";
                            Station.SFCDB.ExecSQL(StrUpdate);

                            string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                            VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','MINI-LTT-SAMPLE','{Station.StationName}','N',sysdate,'{Station.LoginUser.EMP_NO}')";
                            Station.SFCDB.ExecSQL(Strsamplesn);
                            string StrSn = $@"update r_sn set CURRENT_STATION='MINI-LTT', NEXT_STATION='{JUMPEVENT.ToString()}' where sn='{Sn}' and VALID_FLAG='1'";
                            Station.SFCDB.ExecSQL(StrSn);
                        }
                    }
                }
            }
        }
        public static void CHARGE_SAMPLEAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = null;

            if (SNSession.Value.GetType().Name.ToUpper() == "STRING")
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }

            string Skuno = SKU.SkuBase.SKUNO;
            string Sn = sn.baseSN.SN;
            string Routeid = sn.baseSN.ROUTE_ID;
            T_R_FUNCTION_CONTROL _FUNCTION_CONTROL = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            List<R_FUNCTION_CONTROL_NewList> RFC = _FUNCTION_CONTROL.Get2ExListbyVarValue("CHARGE-SAMPLE", "CHARGE-SAMPLE", Skuno, Station.SFCDB);

            if (RFC.Count > 0)
            {
                int TotalQty = Convert.ToInt32(RFC[0].EXTVAL1);
                int SampleQty = Convert.ToInt32(RFC[0].EXTVAL2);
                T_R_SN_LOG _SN_LOG = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
                string SID = _SN_LOG.GetNewID(Station.BU, Station.SFCDB);

                List<r_profile> rp = Station.SFCDB.ORM.Queryable<r_profile>().Where(t => t.PROFILENAME == Skuno && t.PROFILETYPE == "CHARGE-SAMPLE").ToList();
                if (rp.Count == 0)
                {
                    T_r_profile _Profile = new T_r_profile(Station.SFCDB, Station.DBType);
                    String PID = _Profile.GetNewID(Station.BU, Station.SFCDB);

                    string Strsql = $@" INSERT INTO r_profile
                                                            (id,profilename,profilecategory,profiletype,profilevalue,profiledesc,profilesort,profilelevel,edit_emp,edit_time)
                                                            values('{PID}','{Skuno}','','CHARGE-SAMPLE','1','','{TotalQty}','0','{Station.LoginUser.EMP_NO}',sysdate)";
                    Station.SFCDB.ExecSQL(Strsql);
                    string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                            VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','CHARGE-SAMPLE','{Station.StationName}','Y',sysdate,'{Station.LoginUser.EMP_NO}')";
                    Station.SFCDB.ExecSQL(Strsamplesn);
                }
                else
                {
                    int PASSQTY = Convert.ToInt32(rp[0].PROFILEVALUE) + 1;//此前掃過的+此時掃進來的才是用來判斷的數量
                    if (SampleQty >= PASSQTY || PASSQTY > TotalQty)
                    {
                        if (PASSQTY > TotalQty)
                        {
                            string StrUpdate = $@"update r_profile set profilevalue='1',EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='CHARGE-SAMPLE'";
                            Station.SFCDB.ExecSQL(StrUpdate);
                        }
                        else
                        {
                            string StrUpdate = $@"update r_profile set profilevalue=TO_CHAR('{PASSQTY}'),EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='CHARGE-SAMPLE'";
                            Station.SFCDB.ExecSQL(StrUpdate);
                        }

                        string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                        VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','CHARGE-SAMPLE','{Station.StationName}','Y',sysdate,'{Station.LoginUser.EMP_NO}')";
                        Station.SFCDB.ExecSQL(Strsamplesn);
                    }
                    else
                    {
                        string StrUpdate = $@"update r_profile set profilevalue=TO_CHAR('{PASSQTY}'),EDIT_TIME =sysdate where profilename='{Skuno}' and PROFILETYPE='CHARGE-SAMPLE'";
                        Station.SFCDB.ExecSQL(StrUpdate);

                        string Strsamplesn = $@" insert into r_sn_log (ID,SNID,Sn,Logtype,DATA1,Flag,Createtime,Createby)
                                                        VALUES ('{SID}','{sn.baseSN.ID}','{Sn}','CHARGE-SAMPLE','{Station.StationName}','N',sysdate,'{Station.LoginUser.EMP_NO}')";
                        Station.SFCDB.ExecSQL(Strsamplesn);
                    }
                }
            }
        }
        public static void ORT_SAMPLEAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = null;

            if (SNSession.Value.GetType().Name.ToUpper() == "STRING")
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }

            string Skuno = SKU.SkuNo;
            string Sn = sn.SerialNo;
            string Routeid = sn.RouteID;
            string Wo = sn.WorkorderNo;
            string StrSql;
            string Snid = sn.ID;

            T_R_FUNCTION_CONTROL _FUNCTION_CONTROL = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            List<R_F_CONTROL> RFC = _FUNCTION_CONTROL.GetListByFcv("ORT_SET", "ORT_SET", Sn.Substring(0, 3), Station.SFCDB);
            List<R_ORT> RO = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn).ToList();
            T_R_RMA_BONEPILE _RMA_BONEPILE = new T_R_RMA_BONEPILE(Station.SFCDB, Station.DBType);
            bool RB = _RMA_BONEPILE.IsInRmaBonepile(Station.SFCDB, Sn);
            T_R_WO_TYPE _R_WO_TYPE = new T_R_WO_TYPE(Station.SFCDB, Station.DBType);
            bool RW = _R_WO_TYPE.GetWOTypeByPREFIX("RMA", Wo.Substring(1, 6), Station.SFCDB);

            if (RFC.Count > 0 && RB == false && RW == false && RO.Count == 0)
            {
                StrSql = $@" SELECT distinct (a.sn)
                              FROM R_ORT a
                             WHERE substr(a.sn, 1, 3) = substr('{Sn}', 1, 3)
                               AND a.sn NOT LIKE '~%'
                               AND a.sn NOT LIKE '*%'
                               and NOT EXISTS
                             (select 1
                                      from R_ORT b
                                     where b.sn = a.sn
                                       and (b.ortevent = 'ORTOUT' OR b.ortevent = 'ORT10'))
                               AND a.mdstime > sysdate - 10";
                DataTable Otd = Station.SFCDB.RunSelect(StrSql).Tables[0];
                StrSql = $@" select distinct (a.sn)
                              From r_ort_alert a
                             where substr(a.sn, 1, 3) = substr('{Sn}', 1, 3)
                               and NOT EXISTS (select 1 from R_ORT b where b.sn = a.sn)
                               and not exists (SELECT 1
                                      FROM r_sn_pass c
                                     where a.sn = c.sn
                                       and c.pass_station = 'ORT'
                                       and c.TYPE = 'BYPASSORT'
                                       and c.status = '1') ";
                DataTable Ontd = Station.SFCDB.RunSelect(StrSql).Tables[0];

                int SampleQty = Convert.ToInt32(RFC[0].EXTVAL);
                int OrtQty = Otd.Rows.Count;
                int OrtINQty = Ontd.Rows.Count;
                if (OrtINQty + OrtQty < SampleQty)
                {
                    T_R_ORT_ALERT TAL = new T_R_ORT_ALERT(Station.SFCDB, Station.DBType);
                    Row_R_ORT_ALERT RAL = (Row_R_ORT_ALERT)TAL.NewRow();
                    RAL.ID = TAL.GetNewID(Station.BU, Station.SFCDB);
                    RAL.SKUNO = Skuno;
                    RAL.SN = Sn;
                    RAL.SNID = Snid;
                    //RAL.SCANREASON = "該產品已經標識為待掃描ORTIN.";
                    RAL.SCANREASON = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152456");
                    RAL.ALERT_FLAG = 1;
                    RAL.CONTROLBY = "SYSTEM";
                    RAL.CONTROLDT = Station.GetDBDateTime();
                    RAL.SCANBY = "SYSTEM";
                    RAL.SCANDT = Station.GetDBDateTime();
                    Station.SFCDB.ExecSQL(RAL.GetInsertString(DB_TYPE_ENUM.Oracle));
                }
            }

        }
        public static void OrtFail_Recount(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = null;

            if (SNSession.Value.GetType().Name.ToUpper() == "STRING")
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }

            string Skuno = SKU.SkuNo;
            string Sn = sn.SerialNo;
            string Routeid = sn.RouteID;
            string Wo = sn.WorkorderNo;

            string Snid = sn.ID;
            string Version = SKU.Version;

            List<R_ORT> RO = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn && t.ORTEVENT == "ORTIN").ToList();
            if (RO.Count > 0)
            {
                DateTime? OrtInTime = RO[0].WORKTIME;

                List<R_TEST_BRCD> r_s = Station.SFCDB.ORM.Queryable<R_TEST_BRCD>().Where(t => t.SYSSERIALNO == Sn
                  && t.EVENTNAME == "POST-ORT" && t.STATUS != "PASS" && t.TATIME > OrtInTime).ToList();

                if (r_s.Count > 0)
                {
                    R_ORT oRTs = null;
                    oRTs = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn && t.MDSTIME > OrtInTime).OrderBy(t => t.MDSTIME, OrderByType.Desc).ToList().FirstOrDefault();
                    if (oRTs != null)
                    {
                        DateTime? Counttime = oRTs.MDSTIME;
                        string StrSql = $@"UPDATE R_ORT SET SN='RT'|| SN WHERE SN='{Sn}' ";
                        Station.SFCDB.ExecSQL(StrSql);
                        T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
                        Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
                        RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                        RRO.ORTEVENT = "ORTIN";
                        RRO.WORKORDERNO = Wo;
                        RRO.SKUNO = Skuno;
                        RRO.VERSION = Version;
                        RRO.REASONCODE = "";
                        RRO.COUNTER = 0;
                        RRO.SENDFLAG = "0";
                        RRO.MDSTIME = Convert.ToDateTime(Counttime).AddDays(1);
                        RRO.WORKTIME = Station.GetDBDateTime();
                        Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));

                        var log = Station.SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.SN == Sn && t.FLAG == "1").ToList();
                        if (log.Count != 0)
                        {
                            var f = Station.SFCDB.ORM.Updateable<R_SN_LOG>().SetColumns(t => new R_SN_LOG { FLAG = "0", DATA8 = Station.GetDBDateTime().ToString(), DATA9 = Station.LoginUser.EMP_NO }).Where(t => t.SN == Sn && t.LOGTYPE == "RE_TEST_ORT" && t.DATA1 == "ORT" && t.FLAG == "1").ExecuteCommand();
                        }

                        T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, Station.DBType);
                        R_SN_LOG check_log = new R_SN_LOG();
                        check_log = new R_SN_LOG();
                        check_log.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB);
                        check_log.SNID = Sn;
                        check_log.SN = Snid;
                        check_log.LOGTYPE = "RE_TEST_ORT";
                        check_log.DATA1 = "ORT";
                        check_log.FLAG = "1";
                        check_log.CREATETIME = Station.GetDBDateTime();
                        check_log.CREATEBY = Station.LoginUser.EMP_NO;
                        int rs = Station.SFCDB.ORM.Insertable<R_SN_LOG>(check_log).ExecuteCommand();
                        if (rs == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_SN_LOG" }));
                        }
                    }
                }
            }
        }
        public static void OrtScandAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = null;

            if (SNSession.Value.GetType().Name.ToUpper() == "STRING")
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }
            string Skuno = SKU.SkuNo;
            string Sn = sn.SerialNo;
            string Routeid = sn.RouteID;
            string Wo = sn.WorkorderNo;

            string ORTEVENT = "";
            string Snid = sn.ID;
            string Version = SKU.Version;

            string StrSql = $@" SELECT
	                                b.VALUE
                                FROM
	                                R_FUNCTION_CONTROL a,
	                                R_FUNCTION_CONTROL_EX b
                                WHERE
	                                a.ID = b.DETAIL_ID
	                                AND a.FUNCTIONNAME = 'ORT_SET'
	                                AND a.CATEGORY = 'ORT_SET'
	                                AND a.VALUE = SUBSTR('{Sn}', 1, 3) AND b.VALUE IS NOT NULL";
            DataTable Otd = Station.SFCDB.RunSelect(StrSql).Tables[0];
            //List<R_F_CONTROL> listort = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "ORT_SET" && t.CATEGORY == "ORT_SET" && t.VALUE == Sn.Substring(0, 3)).ToList();

            List<R_ORT> RO = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn).ToList();
            if (RO.Count == 0)
            {
                ORTEVENT = "ORTIN";
            }
            if (RO.Count == 11)
            {
                ORTEVENT = "ORTOUT";
            }
            else if (Otd.Rows.Count != 0 && (RO.Count - 1).ToString() == Otd.Rows[0]["VALUE"].ToString())
            {
                ORTEVENT = "ORTOUT";
            }

            T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
            Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
            RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
            RRO.SN = Sn;
            RRO.SNID = Snid;
            RRO.ORTEVENT = ORTEVENT;
            RRO.WORKORDERNO = Wo;
            RRO.SKUNO = Skuno;
            RRO.VERSION = Version;
            RRO.REASONCODE = "";
            RRO.COUNTER = RO.Count;
            RRO.SENDFLAG = "0";
            RRO.MDSTIME = Station.GetDBDateTime();
            RRO.WORKTIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));

        }
        public static void OrtPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = null;

            if (SNSession.Value.GetType().Name.ToUpper() == "STRING")
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }
            string Skuno = SKU.SkuNo;
            string Sn = sn.SerialNo;
            string Routeid = sn.RouteID;
            string Wo = sn.WorkorderNo;
            string Snid = sn.ID;
            string Version = SKU.Version;
            double? Totalcounter = 0;
            DateTime? Foremdstime = null;



            R_ORT RO = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn && t.ORTEVENT == "ORTOUT").OrderBy(t => t.WORKTIME, OrderByType.Desc).ToList().FirstOrDefault();
            if (RO != null)
            {
                DateTime? OrtOutTime = RO.WORKTIME;
                R_ORT r_ORT = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn && t.MDSTIME > OrtOutTime).OrderBy(t => t.MDSTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (r_ORT != null)
                {
                    Foremdstime = r_ORT.MDSTIME;
                    Totalcounter = r_ORT.COUNTER;
                }
            }
            else
            {

                R_ORT r_ORT = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn).OrderBy(t => t.MDSTIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (r_ORT != null)
                {
                    Foremdstime = r_ORT.MDSTIME;
                    Totalcounter = r_ORT.COUNTER;
                }
            }

            var r_ort_in = Station.SFCDB.ORM.Queryable<R_ORT>().Where(t => t.SN == Sn && t.ORTEVENT == "ORTIN").ToList();

            int Counter = Convert.ToInt32(Totalcounter) + 1;
            DateTime Mdstime = Convert.ToDateTime(Foremdstime);
            TimeSpan duration = DateTime.Now - Mdstime;
            int MdsHour = Mdstime.Hour;
            //正常維護數據
            if (duration.TotalHours >= 18 && duration.TotalHours <= 30)
            {
                T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
                Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
                RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                RRO.SN = Sn;
                RRO.SNID = Snid;
                RRO.ORTEVENT = "ORT" + Convert.ToString(Counter);
                RRO.WORKORDERNO = Wo;
                RRO.SKUNO = Skuno;
                RRO.VERSION = Version;
                RRO.REASONCODE = "";
                RRO.COUNTER = Counter;
                RRO.SENDFLAG = "0";
                RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                RRO.WORKTIME = Station.GetDBDateTime();
                Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));

            }

            //判斷是否缺少一天的記錄,是,就補前一天的記錄,并將今天的記錄也補上
            if (duration.TotalHours > 48 - MdsHour && duration.TotalHours <= 72 - MdsHour)
            {
                if (Counter < 2)
                {
                    T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
                    Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
                    RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                    RRO.SN = Sn;
                    RRO.SNID = Snid;
                    RRO.ORTEVENT = "ORT1";
                    RRO.WORKORDERNO = Wo;
                    RRO.SKUNO = Skuno;
                    RRO.VERSION = Version;
                    RRO.REASONCODE = "";
                    RRO.COUNTER = Counter;
                    RRO.SENDFLAG = "0";
                    RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                    RRO.WORKTIME = Station.GetDBDateTime();
                    Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (duration.TotalHours >= 18 + 24 && duration.TotalHours <= 30 + 24)
                    {
                        RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                        RRO.SN = Sn;
                        RRO.SNID = Snid;
                        RRO.ORTEVENT = "ORT2";
                        RRO.WORKORDERNO = Wo;
                        RRO.SKUNO = Skuno;
                        RRO.VERSION = Version;
                        RRO.REASONCODE = "";
                        RRO.COUNTER = Counter + 1;
                        RRO.SENDFLAG = "0";
                        RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                        RRO.WORKTIME = Station.GetDBDateTime();
                        Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));

                    }
                    else if (duration.TotalHours > 30 + 24)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前一天漏掃已補,但今天掃描間隔超過30個小時!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152624"));
                    }
                    else if (duration.TotalHours < 18 + 24)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前一天漏掃已補,但今天掃描間隔未過18個小時,請稍后掃描.");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152655"));
                    }

                }
                else
                {
                    T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
                    Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
                    RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                    RRO.SN = Sn;
                    RRO.SNID = Snid;
                    RRO.ORTEVENT = "ORT" + Convert.ToString(Counter);
                    RRO.WORKORDERNO = Wo;
                    RRO.SKUNO = Skuno;
                    RRO.VERSION = Version;
                    RRO.REASONCODE = "";
                    RRO.COUNTER = Counter;
                    RRO.SENDFLAG = "0";
                    RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                    RRO.WORKTIME = Station.GetDBDateTime();
                    Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (duration.TotalHours >= 18 + 24 && duration.TotalHours <= 30 + 24 && Counter < 9)
                    {
                        RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                        RRO.SN = Sn;
                        RRO.SNID = Snid;
                        RRO.ORTEVENT = "ORT" + Convert.ToString(Counter + 1);
                        RRO.WORKORDERNO = Wo;
                        RRO.SKUNO = Skuno;
                        RRO.VERSION = Version;
                        RRO.REASONCODE = "";
                        RRO.COUNTER = Counter + 2;
                        RRO.SENDFLAG = "0";
                        RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                        RRO.WORKTIME = Station.GetDBDateTime();
                        Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));

                    }
                    else if (duration.TotalHours > 30 + 24)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前一天漏掃已補,但今天掃描間隔超過30個小時!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152624"));
                    }
                    else if (duration.TotalHours < 18 + 24)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前一天漏掃已補,但今天掃描間隔未過18個小時,請稍后掃描.");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152655"));
                    }
                }

            }
            //判斷是否缺少兩天的記錄,是,就補兩天的記錄,再補上當天的記錄
            if (duration.TotalHours > 72 - MdsHour && duration.TotalHours <= 96 - MdsHour)
            {
                if (Counter < 2)
                {
                    T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
                    Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
                    RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                    RRO.SN = Sn;
                    RRO.SNID = Snid;
                    RRO.ORTEVENT = "ORT1";
                    RRO.WORKORDERNO = Wo;
                    RRO.SKUNO = Skuno;
                    RRO.VERSION = Version;
                    RRO.REASONCODE = "";
                    RRO.COUNTER = Counter;
                    RRO.SENDFLAG = "0";
                    RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                    RRO.WORKTIME = Station.GetDBDateTime();
                    Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                    RRO.ORTEVENT = "ORT2";
                    RRO.WORKORDERNO = Wo;
                    RRO.SKUNO = Skuno;
                    RRO.VERSION = Version;
                    RRO.REASONCODE = "";
                    RRO.COUNTER = Counter + 1;
                    RRO.SENDFLAG = "0";
                    RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(2);
                    RRO.WORKTIME = Station.GetDBDateTime();
                    Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (duration.TotalHours >= 18 + 24 && duration.TotalHours <= 30 + 24)
                    {
                        RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                        RRO.SN = Sn;
                        RRO.SNID = Snid;
                        RRO.ORTEVENT = "ORT3";
                        RRO.WORKORDERNO = Wo;
                        RRO.SKUNO = Skuno;
                        RRO.VERSION = Version;
                        RRO.REASONCODE = "";
                        RRO.COUNTER = Counter + 2;
                        RRO.SENDFLAG = "0";
                        RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(3);
                        RRO.WORKTIME = Station.GetDBDateTime();
                        Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));

                    }
                    else if (duration.TotalHours > 30 + 48)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前兩天漏掃已補掃,但今天掃描間隔超過30個小時!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152922"));
                    }
                    else if (duration.TotalHours < 18 + 48)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前兩天漏掃已補掃,但今天掃描間隔未過18個小時,請稍后掃描.");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152929"));
                    }
                }
                else
                {
                    T_R_ORT TRO = new T_R_ORT(Station.SFCDB, Station.DBType);
                    Row_R_ORT RRO = (Row_R_ORT)TRO.NewRow();
                    RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                    RRO.SN = Sn;
                    RRO.SNID = Snid;
                    RRO.ORTEVENT = "ORT" + Convert.ToString(Counter);
                    RRO.WORKORDERNO = Wo;
                    RRO.SKUNO = Skuno;
                    RRO.VERSION = Version;
                    RRO.REASONCODE = "";
                    RRO.COUNTER = Counter;
                    RRO.SENDFLAG = "0";
                    RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(1);
                    RRO.WORKTIME = Station.GetDBDateTime();
                    Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    if (Counter <= 9)
                    {
                        RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                        RRO.SN = Sn;
                        RRO.SNID = Snid;
                        RRO.ORTEVENT = "ORT" + Convert.ToString(Counter + 1);
                        RRO.WORKORDERNO = Wo;
                        RRO.SKUNO = Skuno;
                        RRO.VERSION = Version;
                        RRO.REASONCODE = "";
                        RRO.COUNTER = Counter + 1;
                        RRO.SENDFLAG = "0";
                        RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(2);
                        RRO.WORKTIME = Station.GetDBDateTime();
                        Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                    if (duration.TotalHours >= 18 + 48 && duration.TotalHours <= 30 + 48 && Counter < 8)
                    {
                        RRO.ID = TRO.GetNewID(Station.BU, Station.SFCDB);
                        RRO.SN = Sn;
                        RRO.SNID = Snid;
                        RRO.ORTEVENT = "ORT" + Convert.ToString(Counter + 2);
                        RRO.WORKORDERNO = Wo;
                        RRO.SKUNO = Skuno;
                        RRO.VERSION = Version;
                        RRO.REASONCODE = "";
                        RRO.COUNTER = Counter + 2;
                        RRO.SENDFLAG = "0";
                        RRO.MDSTIME = Convert.ToDateTime(Mdstime).AddDays(3);
                        RRO.WORKTIME = Station.GetDBDateTime();
                        Station.SFCDB.ExecSQL(RRO.GetInsertString(DB_TYPE_ENUM.Oracle));
                    }
                    else if (duration.TotalHours > 30 + 48)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前兩天漏掃已補掃,但今天掃描間隔超過30個小時!");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152922"));
                    }
                    else if (duration.TotalHours < 18 + 48)
                    {
                        //throw new MESReturnMessage($@"ORT掃描異常﹕前兩天漏掃已補掃,但今天掃描間隔未過18個小時,請稍后掃描.");
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152929"));
                    }
                }
            }
            if (duration.TotalHours < 18)
            {
                //throw new MESReturnMessage($@"ORT掃描異常﹕ORT維護掃描間隔未過18個小時,請稍后掃描.");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814152943"));
            }
            if (duration.TotalHours > 30 && r_ort_in.Count != 0)
            {
                //throw new MESReturnMessage($@"ORT掃描異常﹕ORT維護掃描間隔超過30個小時,請稍后掃描.");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153036"));
            }

        }
        public static void SN_InputWWNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            SN sn = null;
            if (Station.StationName.ToUpper().IndexOf("LOADING") > -1)
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }

            List<string> CheckWsn = new List<string>();
            List<string> Checkvssn = new List<string>();
            List<C_ROUTE_DETAIL> CRD = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == sn.baseSN.ROUTE_ID && t.STATION_NAME.Contains("SMTLOADING")).ToList();
            if (CRD.Count > 0)
            {
                List<WWN_DATASHARING> wd = Station.SFCDB.ORM.Queryable<WWN_DATASHARING>().Where(t => t.WSN == sn.baseSN.SN).ToList();
                if (wd.Count == 0)
                {
                    string ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "WWN_DATASHARING");
                    string strSql = $@"  insert into WWN_datasharing(ID,WSN,SKU,VSSN,VSKU,CSSN,CSKU,MAC,WWN,MAC_block_size,WWN_block_size,lasteditby,lasteditdt)
								values('{ID}','{sn.baseSN.SN}','{sn.baseSN.SKUNO}','N/A','N/A','N/A','N/A','','',0,0,'{Station.LoginUser.EMP_NO}',SYSDATE)";
                    Station.SFCDB.ExecSQL(strSql);
                }
            }
        }
        public static void SiAutoSNMakerLinkKPAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession NewSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (NewSNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession KPSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (KPSNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            string NewSN = NewSNSession.Value.ToString();
            SN kpsn = (SN)KPSNSession.Value;
            SN newSN = new SN(NewSNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var kpitem = newSN.KeyPartList.Find(t => t.PART_NO == kpsn.SkuNo);

            var kp = Station.SFCDB.ORM.Queryable<R_SN_KP>().
                Where(t => t.SN == newSN.baseSN.SN
            && t.PARTNO == kpsn.SkuNo
            && t.STATION == Station.StationName && t.R_SN_ID == newSN.baseSN.ID).First();
            if (kp == null)
            {
                throw new Exception("R_SN_KP don't have row");
            }

            kp.VALUE = kpsn.baseSN.SN;
            kp.EDIT_TIME = DateTime.Now;
            kp.EDIT_EMP = Station.LoginUser.EMP_NO;

            Station.SFCDB.ORM.Updateable<R_SN_KP>(kp).Where(t => t.ID == kp.ID).ExecuteCommand();

            #region Add SILOADING Check Scan KP SN : Status/Has Benn Link?  By ZHB 20200709
            List<R_SN_KP> rSNKpList = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.ID == kp.ID).ToList();
            SN_KP _SNKp = new SN_KP(rSNKpList, newSN.WorkorderNo, newSN.SkuNo, Station.SFCDB, Station.BU);

            T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<Row_R_SN_KP> RowRKPList = new List<Row_R_SN_KP>();
            Row_R_SN_KP RowRKP = (Row_R_SN_KP)TRKP.GetObjByID(kp.ID, Station.SFCDB);
            RowRKP.VALUE = kp.VALUE;
            RowRKP.PARTNO = kp.PARTNO;
            Station.SFCDB.ExecSQL(RowRKP.GetUpdateString(DB_TYPE_ENUM.Oracle));
            RowRKP.AcceptChange();
            RowRKPList.Add(RowRKP);
            MesAPIBase api = new MesAPIBase();
            api.LoginUser = Station.LoginUser;
            //Start Check
            KP_ScanType_Check.PCBASN_Check(_SNKp, newSN, RowRKP, RowRKPList, api, Station.SFCDB, Station.APDB);
            #endregion

            #region Insert R_Sn_Link 
            T_R_SN_LINK T_RSL = new T_R_SN_LINK(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_SN_LINK R_RSL = (Row_R_SN_LINK)T_RSL.NewRow();
            R_RSL.ID = T_RSL.GetNewID(Station.BU, Station.SFCDB);
            R_RSL.LINKTYPE = Station.StationName;
            R_RSL.MODEL = newSN.baseSN.SKUNO;
            R_RSL.SN = newSN.baseSN.SN;
            R_RSL.CSN = kpsn.baseSN.SN;
            R_RSL.VALIDFLAG = "1";
            R_RSL.CREATETIME = Station.GetDBDateTime();
            R_RSL.CREATEBY = Station.LoginUser.EMP_NO;
            R_RSL.EDITTIME = Station.GetDBDateTime();
            R_RSL.EDITBY = Station.LoginUser.EMP_NO;
            Station.SFCDB.ExecSQL(R_RSL.GetInsertString(DB_TYPE_ENUM.Oracle));
            #endregion
        }

        /// <summary>
        /// SN 投入工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ORACLE_SNInputAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            T_R_WO_BOM t_r_wo_bom = new T_R_WO_BOM(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);

            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;
            WorkOrder WorkOrder = null;
            if (Paras.Count != 1)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession Wo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder = (WorkOrder)Wo.Value;
            bool Woexists = t_r_wo_bom.CheckWOExist(Wo.Value.ToString(), Station.SFCDB);
            //get Sku
            SKU sku = new SKU();
            sku.Init(WorkOrder.SkuNO, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //get SNRule name
            string SNRULE = sku.SnRule;
            //Make WO SNs
            List<string> SNS = new List<string>();
            for (int i = 0; i < WorkOrder.WORKORDER_QTY - WorkOrder.INPUT_QTY; i++)
            {
                SNS.Add(SNmaker.GetNextSN(SNRULE, Station.DBS["SFCDB"]));
            }
            if (SNS.Count == 0)
            {
                throw new Exception("Workorder is Full !");
            }

            //input SNsToWo
            List<R_SN> SNs = new List<R_SN>();
            for (int i = 0; i < SNS.Count; i++)
            {
                var SN = new R_SN();
                SN.SN = SNS[i];
                SN.SKUNO = WorkOrder.SkuNO;
                SN.WORKORDERNO = WorkOrder.WorkorderNo;
                SN.PLANT = WorkOrder.PLANT;
                SN.ROUTE_ID = WorkOrder.RouteID;
                SN.STARTED_FLAG = "1";
                SN.PACKED_FLAG = "0";
                SN.COMPLETED_FLAG = "0";
                SN.SHIPPED_FLAG = "0";
                SN.REPAIR_FAILED_FLAG = "0";
                SN.CURRENT_STATION = Station.StationName;
                SN.CUST_PN = WorkOrder.CUST_PN;
                SN.SCRAPED_FLAG = "0";
                SN.PRODUCT_STATUS = "FRESH";
                SN.REWORK_COUNT = 0d;
                SN.VALID_FLAG = "1";
                SN.EDIT_EMP = Station.LoginUser.EMP_NO;
                SNs.Add(SN);
            }
            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); // 插入到 R_SN 表中

            WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            WoTable.AddCountToWo(WorkOrder.WorkorderNo, SNs.Count, Station.SFCDB); //更新工單投入數量
            //KP Extent
            MESStation.LogicObject.SN SNO = new SN();
            for (int i = 0; i < SNs.Count; i++)
            {
                //20190314 Patty ATO testing

                if (WorkOrder.SKU_NAME == "X7-2C" || WorkOrder.SKU_NAME == "ODA_HA" || WorkOrder.SKU_NAME == "BDA_ATO" || WorkOrder.SKU_NAME == "ORACLE_RACK" || WorkOrder.SKU_NAME == "E1-2C" || WorkOrder.SKU_NAME == "E2-2C" || (WorkOrder.PRODUCTION_TYPE == "PTO" && WorkOrder.SKU_NAME == "X8-8") || (WorkOrder.PRODUCTION_TYPE == "PTO" && WorkOrder.SKU_NAME == "ODA_X8-2"))
                {
                    SNO.InsertR_SN_KP(WorkOrder, SNs[i], Station.SFCDB, Station, DB_TYPE_ENUM.Oracle);
                }
                else
                {
                    if (!Woexists)
                    {
                        throw new Exception("Workorder missing WO BOM !");
                    }
                    SNO.ORAInsertR_SN_KP(WorkOrder, SNs[i], Station.SFCDB, Station, DB_TYPE_ENUM.Oracle);
                }
            }
            //Print LABEL

            T_C_SKU_Label TCSL = new T_C_SKU_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //獲取該機種所設置的 label 配置
            List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(sku.SkuNo, Station.StationName, Station.SFCDB);

            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            //因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type
            for (int i = 0; i < labs.Count; i++)
            {
                //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labs[i].LABELNAME, Station.SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labs[i].LABELTYPE, Station.SFCDB);

                //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                System.Type APIType = assembly.GetType(RC.CLASS);
                for (int j = 0; j < SNS.Count; j++)
                {
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    LabelBase Lab = (LabelBase)API_CLASS;
                    for (int j1 = 0; j1 < Lab.Inputs.Count; j1++)
                    {
                        if (Lab.Inputs[j1].Name.ToUpper() == "STATION")
                        {
                            Lab.Inputs[j1].Value = Station.StationName;
                        }

                        MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j1].StationSessionType && T.SessionKey == Lab.Inputs[j1].StationSessionKey);
                        if (S != null)
                        {
                            Lab.Inputs[j1].Value = S.Value;
                        }
                        if (Lab.Inputs[j1].Name.ToUpper() == "SN")
                        {
                            Lab.Inputs[j1].Value = SNS[j];
                        }
                    }

                    Lab.LabelName = RL.LABELNAME;
                    Lab.FileName = RL.R_FILE_NAME;
                    Lab.PrintQTY = (int)labs[i].QTY;
                    Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                    Lab.MakeLabel(Station.SFCDB);
                    List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);
                    for (int k = 0; k < pages.Count; k++)
                    {
                        pages[k].ALLPAGE = pages.Count;
                        Station.LabelPrint.Add(pages[k]);
                    }
                }
            }
        }

        /// <summary>
        /// SN 投入工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNInputAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //int uuu = 2;
            R_SN SN = null;
            WorkOrder WorkOrder = null;
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            //MESStationSession SnSession = null;
            List<R_SN> SNs = new List<R_SN>();
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession NewSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewSn == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession Wo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder = (WorkOrder)Wo.Value;

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            SN = new R_SN();
            SN.SN = NewSn.Value.ToString();
            SN.SKUNO = WorkOrder.SkuNO;
            SN.WORKORDERNO = WorkOrder.WorkorderNo;
            SN.PLANT = WorkOrder.PLANT;
            SN.ROUTE_ID = WorkOrder.RouteID;
            SN.STARTED_FLAG = "1";
            SN.PACKED_FLAG = "0";
            SN.COMPLETED_FLAG = "0";
            SN.SHIPPED_FLAG = "0";
            SN.REPAIR_FAILED_FLAG = "0";
            SN.CURRENT_STATION = Station.StationName;
            SN.CUST_PN = WorkOrder.CUST_PN;
            SN.SCRAPED_FLAG = "0";
            SN.PRODUCT_STATUS = "FRESH";
            SN.REWORK_COUNT = 0d;
            SN.VALID_FLAG = "1";
            SN.EDIT_EMP = Station.LoginUser.EMP_NO;
            SNs.Add(SN);

            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); // 插入到 R_SN 表中

            WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            WoTable.AddCountToWo(WorkOrder.WorkorderNo, 1, Station.SFCDB); //更新工單投入數量

            Station.AddMessage("MES00000054", new string[] { NewSn.Value.ToString() }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 修改BPD Backflush 时间
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InsertDate(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_C_CONTROL backflush = new T_C_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string ErrMessage = string.Empty;
            string startdate = string.Empty;
            string EnDdate = string.Empty;
            string data = string.Empty;
            MESStationSession StartDate = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (StartDate == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            startdate = StartDate.Value.ToString();
            MESStationSession EndDate = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (EndDate == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            EnDdate = EndDate.Value.ToString();
            DateTime StartTime = new DateTime();
            DateTime EndTime = new DateTime();
            if (DateTime.TryParse(startdate, out StartTime) && DateTime.TryParse(EnDdate, out EndTime))
            {
                data = StartTime.ToString("yyyy-MM-dd HH:mm:ss") + '~' + EndTime.ToString("yyyy-MM-dd HH:mm:ss");
                backflush.UpdateControlValue(data, Station.SFCDB);
            }
            else
            {
                //ErrMessage = "你输入的时间格式错误，请按照以下'2020-01-01 00:00:00'格式填写";
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153058");
                throw new MESReturnMessage(ErrMessage);
            }
        }

        /// <summary>
        /// SMTLOADING 工站，多個不連續的 SN 投入工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void MultiSNInputAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            R_SN SN = null;
            WorkOrder WorkOrder = null;
            T_R_SN SnTable = null;
            T_R_WO_BASE WoTable = null;
            List<R_SN> SNs = new List<R_SN>();
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            MESStationSession SnsSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnsSession == null || SnsSession.Value == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            List<string> Sns = ((List<string>)SnsSession.Value);

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder = (WorkOrder)WoSession.Value;

            //Device1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else
            {
                DeviceName = Station.StationName;
            }

            foreach (string NewSn in Sns)
            {
                SN = new R_SN();
                SN.SN = NewSn;
                SN.SKUNO = WorkOrder.SkuNO;
                SN.WORKORDERNO = WorkOrder.WorkorderNo;
                SN.PLANT = WorkOrder.PLANT;
                SN.ROUTE_ID = WorkOrder.RouteID;
                SN.STARTED_FLAG = "1";
                SN.PACKED_FLAG = "0";
                SN.COMPLETED_FLAG = "0";
                SN.SHIPPED_FLAG = "0";
                SN.REPAIR_FAILED_FLAG = "0";
                SN.CURRENT_STATION = Station.StationName;
                SN.CUST_PN = WorkOrder.CUST_PN;
                SN.SCRAPED_FLAG = "0";
                SN.PRODUCT_STATUS = "FRESH";
                SN.REWORK_COUNT = 0d;
                SN.VALID_FLAG = "1";
                SN.EDIT_EMP = Station.LoginUser.EMP_NO;
                SNs.Add(SN);
            }
            SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
            SnTable.AddToRSn(SNs, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB); // 插入到 R_SN 表中

            WoTable = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            WoTable.AddCountToWo(WorkOrder.WorkorderNo, SNs.Count, Station.SFCDB); //更新工單投入數量

            //重置剩余扫描数
            MESStationSession ExtScanSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (ExtScanSession == null)
            {
                ExtScanSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = "" };
                Station.StationSession.Add(ExtScanSession);
            }

            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (ExtScanSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            ExtScanSession.Value = LinkQtySession.Value.ToString();

            //Sns.Clear();
        }

        /// <summary>
        /// SMTLoading工站，SN區間批次過站
        /// 2018/1/25 Rain
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LotSNInputAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string strStartSN = "";
            string strEndSN = "";
            string strTempSN = "";
            string strDecmialType = "";
            int intLotQty;
            WorkOrder Wo;

            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            //工單必須存在
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Wo_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else
                {
                    Wo = (WorkOrder)Wo_Session.Value;
                    if (Wo.WorkorderNo == null || Wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
            }
            //Start SN必須加載
            MESStationSession StartSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StartSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else
            {
                if (StartSN_Session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000115", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else
                {
                    strStartSN = StartSN_Session.Value.ToString();
                }
            }
            //End SN必須加載
            MESStationSession EndSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (EndSN_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                if (EndSN_Session.Value != null)
                {
                    strEndSN = EndSN_Session.Value.ToString();
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000116", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
            }
            //DecimalType必須加載
            MESStationSession DecimalType_Session = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (DecimalType_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000127", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            else
            {
                if (DecimalType_Session.Value != null)
                {
                    strDecmialType = DecimalType_Session.Value.ToString();
                    if (strDecmialType != "10H" && strDecmialType != "34H")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000129", new string[] { Paras[3].SESSION_TYPE + strDecmialType }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000127", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
            }
            //LotQty必須加載
            MESStationSession LotQty_Session = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (LotQty_Session == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000128", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
            else
            {
                if (LotQty_Session.Value != null)
                {
                    intLotQty = Convert.ToInt32(LotQty_Session.Value.ToString());
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000128", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
            }

            //批量投入SN，處理R_SN及過站記錄表
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            List<R_SN> R_SNs = new List<R_SN>(intLotQty);
            List<string> SNIds = null;

            //循環取得下一個序號
            T_R_WO_BASE TR_Wo_Base = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            string strSubSN = "";
            for (int i = 1; i <= intLotQty; i++)
            {
                R_SN OriginalSN = new R_SN();
                //初始化OriginalSN
                OriginalSN.SKUNO = Wo.SkuNO;
                OriginalSN.WORKORDERNO = Wo.WorkorderNo;
                OriginalSN.PLANT = Wo.PLANT;
                OriginalSN.ROUTE_ID = Wo.RouteID;
                OriginalSN.STARTED_FLAG = "1";
                OriginalSN.PACKED_FLAG = "0";
                //OriginalSN.PACKDATE
                OriginalSN.COMPLETED_FLAG = "0";
                //OriginalSN.COMPLETED_TIME
                OriginalSN.SHIPPED_FLAG = "0";
                //OriginalSN.SHIPDATE
                OriginalSN.REPAIR_FAILED_FLAG = "0";
                OriginalSN.CURRENT_STATION = Station.StationName;
                OriginalSN.CUST_PN = Wo.CUST_PN;
                OriginalSN.SCRAPED_FLAG = "0";
                //OriginalSN.SCRAPED_TIME
                OriginalSN.PRODUCT_STATUS = "FRESH";
                OriginalSN.REWORK_COUNT = 0d;
                OriginalSN.VALID_FLAG = "1";
                OriginalSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                if (i == 1)
                {
                    strTempSN = strStartSN;
                }
                else
                {
                    strSubSN = strTempSN.Substring(strTempSN.Length - 4, 4);
                    strSubSN = TR_Wo_Base.Get_NextSN(strSubSN, strDecmialType);
                    strSubSN = strSubSN.PadLeft(4, '0');
                    strTempSN = strTempSN.Substring(0, strTempSN.Length - 4) + strSubSN;
                }
                OriginalSN.SN = strTempSN;
                R_SNs.Add(OriginalSN);
            }
            //寫R_SN及R_SN_Station_Detail
            SNIds = TR_SN.AddToRSn(R_SNs, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
            // 更新 R_WO_BASE 中的投入數量
            TR_Wo_Base.AddCountToWo(Wo.WorkorderNo, intLotQty, Station.SFCDB);
            ////回饋消息到前台
            Station.AddMessage("MES00000130", new string[] { intLotQty.ToString() }, StationMessageState.Pass);
        }

        /// <summary>
        /// 記錄良率
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordYieldRateAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StationName = string.Empty;
            string WorkOrderNo = string.Empty;
            //string Sn = string.Empty;
            List<string> SNs = new List<string>();
            string Status = string.Empty;
            T_R_SN SnTable = null;
            SN SNObj = null;
            double LinkQty = 0d;
            string ErrMessage = string.Empty;

            if (Paras.Count < 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "4", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //NEWSN
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            //Modify By LLF 2018-01-27
            //Sn = SNSession.Value.ToString();

            if (SNSession.Value is SN && SNSession.Value != null)
            {
                //Sn = ((SN)SNSession.Value).SerialNo;
                SNs.Add(((SN)SNSession.Value).SerialNo);
            }
            else if (SNSession.Value is List<string>)
            {
                SNs.AddRange((List<string>)SNSession.Value);
            }
            else
            {
                //Sn = SNSession.InputValue.ToString();
                SNs.Add(SNSession.InputValue.ToString());
            }

            ////WO
            MESStationSession WoSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            ////STATUS
            MESStationSession StatusSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "Pass" };
                Station.StationSession.Add(StatusSession);
                //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                //throw new MESReturnMessage(ErrMessage);
            }
            Status = StatusSession.Value.ToString();

            //Modify by LLF 2018-01-28,LinqQty 取實際的連板數量，而不是配置的連板數量
            ////LINKQTY
            //MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (LinkQtySession == null)
            //{
            //    //LinkQtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = "1" };
            //    //Station.StationSession.Add(LinkQtySession);
            //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
            //    throw new MESReturnMessage(ErrMessage);
            //}

            foreach (string Sn in SNs)
            {
                SNObj = new SN();
                LinkQty = SNObj.GetLinkQty(Sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

                StationName = Station.StationName;
                WorkOrderNo = ((WorkOrder)WoSession.Value).WorkorderNo;

                SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
                SnTable.RecordYieldRate(WorkOrderNo, LinkQty, Sn, Status, Station.Line, StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                Station.AddMessage("MES00000150", new string[] { Sn, "Yield Rate" }, StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 記錄 UPH
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordUPHAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string StationName = string.Empty;
            string WorkOrderNo = string.Empty;
            //string Sn = string.Empty;
            List<string> SNs = new List<string>();
            string Status = string.Empty;
            T_R_SN SnTable = null;
            SN SNObj = null;
            double LinkQty = 0d;
            string ErrMessage = string.Empty;

            if (Paras.Count < 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "4", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //NEWSN
            MESStationSession SNSession = Station.StationSession.Find(
                                                    t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            if (SNSession.Value is SN && SNSession.Value != null)
            {
                //Sn = ((SN)SNSession.Value).SerialNo;
                SNs.Add(((SN)SNSession.Value).SerialNo);
            }
            else if (SNSession.Value is List<string>)
            {
                SNs.AddRange((List<string>)SNSession.Value);
            }
            else
            {
                //Sn = SNSession.InputValue.ToString();
                SNs.Add(SNSession.InputValue.ToString());
            }

            //WO
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            //STATUS
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                //throw new MESReturnMessage(ErrMessage);
                StatusSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "Pass" };
                Station.StationSession.Add(StatusSession);
            }
            Status = StatusSession.Value.ToString();

            //Modify by LLF 2018-01-27,LinqQty 取實際的連板數量，而不是配置的連板數量
            //LINKQTY
            //MESStationSession LinkQtySession = Station.StationSession.Find(
            //                                        t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            //if (LinkQtySession == null)
            //{
            //    LinkQtySession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = "1" };
            //    Station.StationSession.Add(LinkQtySession);
            //    //ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY });
            //    //throw new MESReturnMessage(ErrMessage);
            //}
            //LinkQty = double.Parse(LinkQtySession.Value.ToString());

            foreach (string Sn in SNs)
            {
                SNObj = new SN();
                LinkQty = SNObj.GetLinkQty(Sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

                StationName = Station.StationName;
                WorkOrderNo = ((WorkOrder)WoSession.Value).WorkorderNo;

                SnTable = new T_R_SN(Station.SFCDB, Station.DBType);
                SnTable.RecordUPH(WorkOrderNo, LinkQty, Sn, Status, Station.Line, StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                Station.AddMessage("MES00000150", new string[] { Sn, "UPH" }, StationMessageState.Pass);
            }
        }

        /// <summary>
        /// SN 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPassStationAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SnObject = (SN)SNSession.Value;

            //STATUS,方便寫良率和UPH使用
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == "STATUS" && t.SessionKey == "1");
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[1].VALUE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value == null ||
                    (StatusSession.Value != null && StatusSession.Value.ToString() == ""))
                {
                    StatusSession.Value = "PASS";
                }
            }

            //Device
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else //Add by LLF 2018-02-05
            {
                DeviceName = Station.StationName;
            }
            if (StatusSession.Value.ToString().ToUpper() == "PASS")
            {
                table.PassStation(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, StatusSession.Value.ToString(), Station.LoginUser.EMP_NO, Station.SFCDB);

                #region VNDCN Schneider 傳PCBA SN給客戶 自動線已休眠，等它活再開
                //if (SnObject.NextStation == "STOCKIN")
                //{
                //    SchneiderAction Schneider = new SchneiderAction();
                //    Schneider.SendPCBASNTOSE(Station.SFCDB, SnObject.SerialNo);
                //}
                #endregion
            }
            //table.PassStation("6131351adfdf", "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 通過CSN過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPassStationByCSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string ErrMessage = string.Empty;
            string DeviceName = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "3", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            string strCsn = Input.Value.ToString();
            MESDataObject.Module.R_SN_LINK objRSL = Station.SFCDB.ORM.Queryable<MESDataObject.Module.R_SN_LINK>().Where(t => t.CSN == strCsn && t.VALIDFLAG == "1").First();
            if (objRSL != null)
            {
                SnObject = new SN(objRSL.SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                //throw new Exception("輸入CSN在R_SN_LINK表記錄!");
                SnObject = new SN(strCsn, Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            //STATUS,方便寫良率和UPH使用
            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == "STATUS" && t.SessionKey == "1");
            if (StatusSession == null)
            {
                StatusSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, InputValue = Input.Value.ToString(), Value = Paras[1].VALUE, SessionKey = Paras[1].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(StatusSession);
                if (StatusSession.Value == null ||
                    (StatusSession.Value != null && StatusSession.Value.ToString() == ""))
                {
                    StatusSession.Value = "PASS";
                }
            }

            //Device
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }
            else //Add by LLF 2018-02-05
            {
                DeviceName = Station.StationName;
            }
            if (StatusSession.Value.ToString().ToUpper() == "PASS")
            {
                table.PassStation(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, StatusSession.Value.ToString(), Station.LoginUser.EMP_NO, Station.SFCDB);
            }
            //table.PassStation("6131351adfdf", "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordPassStationDetailAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string SerialNo = string.Empty;
            string DeviceName = Station.StationName;
            string ErrMessage = string.Empty;

            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
            //獲取 SN1
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SnSession.Value.ToString();
            //SerialNo = ((R_SN)SnSession.Value).SN;

            //獲取 DEVICE1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            var snobj = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SerialNo && t.VALID_FLAG == "1").ToList().FirstOrDefault();
            table.RecordPassStationDetail(snobj, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);
            //table.RecordPassStationDetail(Input.Value.ToString(), Station.Line, Station.StationName, DeviceName,Station.BU, Station.SFCDB);
        }

        /// <summary>
        /// HWD在BIP工站更新第一個SN的工單到R_FAI表中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InsertFirstSnintoFai(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_FAI RFAI = new T_R_FAI(Station.SFCDB, Station.DBType);
            Panel panel = new Panel();

            string PanelNo = string.Empty;
            string DeviceName = Station.StationName;
            string Wo = string.Empty;
            string ErrMessage = string.Empty;

            //獲取 SN1
            MESStationSession PanelSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PanelSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            panel = (Panel)PanelSession.Value;
            PanelNo = PanelSession.InputValue.ToString();
            Wo = panel.Workorderno;

            var route = Station.SFCDB.ORM.Queryable<R_SN, R_PANEL_SN, C_ROUTE>((rs, ps, cr) => rs.WORKORDERNO == ps.WORKORDERNO && rs.SN == ps.SN && rs.ROUTE_ID == cr.ID).Where((rs, ps, cr) => ps.PANEL == PanelNo)
                .Select((rs, ps, cr) => cr).ToList().FirstOrDefault();

            var ControlExist = Station.SFCDB.ORM.Queryable<C_CONTROL>().Any(t => t.CONTROL_NAME == "'FAI_CONTROL_ROUTE'" && t.CONTROL_VALUE == route.ROUTE_NAME);
            //增加開關，不卡此邏輯機種路由的直接關掉，與AOI1同步
            if (!ControlExist)
            {
                if (!RFAI.CheckWoHaveDoneFai(Wo, "AOI1", Station.SFCDB))
                {
                    //記錄時間，用於在AOI1工站計算時間
                    string emp_no = Station.LoginUser.EMP_NO;
                    string result2 = RFAI.UpdateFaidetail(Wo, emp_no, Station.SFCDB);
                }
            }
        }

        public static void RecordStationDetailAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string SerialNo = string.Empty;
            string DeviceName = Station.StationName;
            string ErrMessage = string.Empty;

            //獲取 SN1
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SnSession.Value.ToString();
            //SerialNo = ((R_SN)SnSession.Value).SN;

            //獲取 DEVICE1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            MESStationSession StatusSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StatusSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            if (StatusSession.Value.ToString() != "PASS")
            {
                var snobj = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == SerialNo && t.VALID_FLAG == "1")
                    .ToList().FirstOrDefault();
                table.RecordPassStationDetail(snobj, Station.Line, Station.StationName, DeviceName, Station.BU,
                    Station.SFCDB, StatusSession.Value.ToString().Substring(0, 1));
            }
            //table.RecordPassStationDetail(Input.Value.ToString(), Station.Line, Station.StationName, DeviceName,Station.BU, Station.SFCDB);
        }

        public static void PassRotationAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)

        {
            T_R_SILVER_ROTATION tabel = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            string Csn = string.Empty;
            string Pn = string.Empty;
            string ErrMessage = string.Empty;

            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "1", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession Csnsession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Csnsession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Csn = Csnsession.Value.ToString();
            MESStationSession Pnsession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Pnsession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Pn = Pnsession.Value.ToString();
            //tabel.PassRotationActionDetail(Csn, Pn, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
        }

        public static void PassInputWashPcbAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_IO_HEAD tabel = new T_R_IO_HEAD(Station.SFCDB, Station.DBType);
            string sn = string.Empty;
            string Location = string.Empty;
            string Reason = string.Empty;
            string ErrMessage = string.Empty;

            if (Paras.Count != 3)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "1", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession Csnsession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Csnsession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            sn = Csnsession.InputValue.ToString();
            MESStationSession Reasonsession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Reasonsession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Reason = Reasonsession.Value.ToString();
            MESStationSession Locationsession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (Locationsession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Location = Locationsession.Value.ToString();
            tabel.PassWashActionDetail(sn, Reason, Location, Station.Line, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
        }

        public static void PassStationExt(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string SerialNo = string.Empty;
            string Location = string.Empty;
            string Description = string.Empty;
            string ErrMessage = string.Empty;

            if (Paras.Count != 2)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "2", Paras.Count.ToString() });
                throw new MESReturnMessage(ErrMessage);
            }

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SerialNo = SnSession.Value.ToString();
            //SerialNo = ((R_SN)SnSession.Value).SN;

            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LocationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            Location = LocationSession.Value.ToString();
            Description = Input.Value.ToString();

            table.PassStationExtDetail(SerialNo, Description, Station.StationName, Location, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            //table.RecordPassStationDetail(Input.Value.ToString(), Station.Line, Station.StationName, DeviceName,Station.BU, Station.SFCDB);
        }

        /// <summary>
        /// VT產品抽测ORT,2019/01/10 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OrtSpotTets(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_LOT_STATUS Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL Lot_Detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            SN SNObj = new SN();

            R_SN SerialNo = new R_SN();
            string Wo = string.Empty;
            string RouteID = "";
            DataTable dt = null;
            string sql = null;
            string NewLotFlag = "";
            string LotSatusID = "";
            int woqty, Count;
            int X, E, A;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SNObj = (SN)SNSession.Value;
            Wo = SNObj.WorkorderNo;
            RouteID = SNObj.RouteID;
            SerialNo = SNObj.LoadSN(SNObj.SerialNo, Station.SFCDB);
            sql = " select*From r_wo_base where WORKORDERNO='" + Wo + "' and skuno in ( SELECT skuno fROM c_sku_detail where category='ORT_NOSAMPLE') ";
            dt = Station.SFCDB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                var Station_Name = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(c => c.ROUTE_ID == RouteID && c.STATION_NAME == "BURNIN").ToList().FirstOrDefault();
                if (Station_Name != null)
                {
                    var WO = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(c => c.WORKORDERNO == Wo).ToList().FirstOrDefault();
                    woqty = (int)WO.WORKORDER_QTY;
                    if (woqty < 600)
                    {
                        X = woqty / 7;
                        E = 6;
                        if (X == 0)
                        {
                            X = 1;
                        }
                    }
                    else
                    {
                        X = 90;
                        E = woqty / 100;
                    }
                    var LotStatus = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>().Where(C => C.SKUNO == Wo).ToList().FirstOrDefault();
                    if (LotStatus == null)
                    {
                        NewLotFlag = "1";
                    }
                    else
                    {
                        LotSatusID = LotStatus.ID;
                    }
                    var LotDetail = Station.SFCDB.ORM.Queryable<R_LOT_DETAIL>().Where(c => c.SN == SNObj.SerialNo && c.WORKORDERNO == Wo).ToList().FirstOrDefault();
                    if (LotDetail == null)
                    {
                        sql = " select DISTINCT SN From r_sn_station_detail where workorderno='" + Wo + "' and station_name='FT2' and valid_flag=1 ";
                        dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                        Count = dt.Rows.Count + 1;
                        if (Count % X == 0)
                        {
                            sql = " select DISTINCT SN From r_lot_detail where WORKORDERNO='" + Wo + "'";
                            dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                            A = dt.Rows.Count;
                            if (A < E)
                            {
                                Lot_Status.OrtInLotPass(NewLotFlag, SerialNo, woqty, E, LotSatusID, "ORT", Station.LoginUser.EMP_NO, Station.Line, Station.BU, Station.SFCDB);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Juniper Product sample ORT,2021.9.28
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OrtSampleFromSku(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_LOT_STATUS Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL Lot_Detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Rstatus = (Row_R_LOT_STATUS)Lot_Status.NewRow();
            Row_R_LOT_DETAIL Rdetail = (Row_R_LOT_DETAIL)Lot_Detail.NewRow();

            //SN SNObj = new SN();
            DataTable dt = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //SNObj = (SN)SNSession.Value;
            //Sku = SNObj.SkuNo;
            //SerialNo = SNObj.LoadSN(SNObj.SerialNo, Station.SFCDB);
            var _SN = (SN)SNSession.Value;
            var SNObj = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == _SN.SerialNo && r.VALID_FLAG == "1").First();
            var Skusample = Station.SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(c => c.SKUNO == SNObj.SKUNO && c.CATEGORY == "JUNIPER" && c.CATEGORY_NAME == "ORT_Sample" && c.EXTEND == "Y").ToList().FirstOrDefault();
            string sql = string.Empty;

            //不同機種路由製程不一樣,與其讓QE設定在哪個工站進行抽測,還不如統一設定在SILOADING抽測
            if (Skusample != null && Station.StationName == "SILOADING")
            {
                //Convert percentage to decimal
                Double SampleValue = double.Parse(Skusample.VALUE.Replace("%", "")) / 100;

                //Calculate the current SKU production quantity
                sql = $@"SELECT skuno,sum(WORKORDER_QTY) woqty FROM r_wo_base where skuno='{SNObj.SKUNO}'group by skuno";
                dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                int SkuProductQty = Convert.ToInt32(dt.Rows[0]["WOQTY"].ToString());

                //Check whether there is a random test for SKU
                //var InORT = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>().Where(r => r.SKUNO == SNObj.SKUNO && r.SAMPLE_STATION == "ORT").ToList();
                //排除掉R_LOT_DETAIL中CancelORT的數據
                var InORT = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((s, d) => s.LOT_NO == d.LOT_ID).Where((s, d) => s.SKUNO == SNObj.SKUNO && s.SAMPLE_STATION == "ORT" && d.STATUS != "2").Select((s, d) => d).ToList();
                //Calculate the current SKU of sampling tests required
                //Double DoSample = SkuProductQty *  SampleValue;
                Double DoSample = Math.Round(SkuProductQty * SampleValue);//四捨五入, 0.1-0.4則略,0.5-0.9算1  Asked By QE BiBi  2021-11-15
                if (DoSample >= 1 && InORT.Count == 0)
                {
                    Rstatus.ID = Lot_Detail.GetNewID(Station.BU, Station.SFCDB);
                    Rstatus.LOT_NO = "LOT-" + DateTime.Now.ToString("yyyyMMddFF");
                    Rstatus.SKUNO = SNObj.SKUNO;
                    Rstatus.LOT_QTY = SkuProductQty;
                    Rstatus.REJECT_QTY = 0;
                    Rstatus.SAMPLE_QTY = Math.Floor(DoSample);//Rounded up
                    Rstatus.PASS_QTY = 1;
                    Rstatus.FAIL_QTY = 0;
                    Rstatus.CLOSED_FLAG = "2";
                    Rstatus.LOT_STATUS_FLAG = "0";
                    Rstatus.SAMPLE_STATION = "ORT";
                    Rstatus.EDIT_EMP = Station.LoginUser.EMP_NO;
                    Rstatus.EDIT_TIME = DateTime.Now;
                    Station.SFCDB.ExecSQL(Rstatus.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                    Station.SFCDB.CommitTrain();

                    Rdetail.ID = Lot_Detail.GetNewID(Station.BU, Station.SFCDB);
                    Rdetail.LOT_ID = Rstatus.LOT_NO;
                    Rdetail.SN = SNObj.SN;
                    Rdetail.WORKORDERNO = SNObj.WORKORDERNO;
                    Rdetail.CREATE_DATE = DateTime.Now;
                    Rdetail.STATUS = "0";
                    Station.SFCDB.ExecSQL(Rdetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                    Station.SFCDB.CommitTrain();

                    //增加彈窗提示
                    UIInputData O = new UIInputData()
                    {
                        Timeout = 60000,
                        UIArea = new string[] { "25%", "28%" },
                        IconType = IconType.None,
                        MustConfirm = false,
                        Message = "OK",
                        Tittle = "",
                        Type = UIInputType.Confirm,
                        Name = "",
                        ErrMessage = ""
                    };
                    O.OutInputs.Add(new DisplayOutPut() { Name = "Alert", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"{SNObj.SN} has been chosen to ORT test !" });
                    O.GetUiInput(Station.API, UIInput.Normal, Station);
                }
                else if (DoSample >= 1 && InORT.Count > 0)
                {
                    var InORTDetail = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((rls, rld) => rls.LOT_NO == rld.LOT_ID)
                        .Where((rls, rld) => rls.SKUNO == SNObj.SKUNO && rls.SAMPLE_STATION == "ORT" && (rld.STATUS == "0" || rld.STATUS == "1"))
                        .OrderBy((rls, rld) => rld.CREATE_DATE, SqlSugar.OrderByType.Desc)
                        .Select((rls, rld) => rld)
                        .ToList().FirstOrDefault();
                    DateTime ORTtestTime = (DateTime)InORTDetail.CREATE_DATE;

                    var stationdetail = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.SKUNO == SNObj.SKUNO && r.STATION_NAME == Station.StationName && r.VALID_FLAG == "1" && r.EDIT_TIME > ORTtestTime).ToList();
                    //Calculate the number of draws from the previous one to the next
                    int SnStationCount = stationdetail.Count();
                    //Using 100 as the unit, calculate how many samples need to be tested for every 100 pieces
                    int SeTValue = int.Parse(Skusample.VALUE.Replace("%", ""));
                    int SamPle = 100 / SeTValue;//Smapling rate :one of  "SamPle"

                    //int SamPle = (int)Math.Floor(100 * SampleValue);
                    if (SnStationCount == SamPle)
                    {
                        Station.SFCDB.ORM.Updateable<R_LOT_STATUS>().SetColumns(r => new R_LOT_STATUS
                        {
                            LOT_QTY = SkuProductQty,
                            SAMPLE_QTY = Math.Floor(DoSample),//Rounded up
                            PASS_QTY = r.PASS_QTY + 1,
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                            EDIT_TIME = DateTime.Now
                        }).Where(r => r.SKUNO == SNObj.SKUNO && r.SAMPLE_STATION == "ORT").ExecuteCommand();

                        Rdetail.ID = Lot_Detail.GetNewID(Station.BU, Station.SFCDB);
                        Rdetail.LOT_ID = Rstatus.LOT_NO;
                        Rdetail.SN = SNObj.SN;
                        Rdetail.WORKORDERNO = SNObj.WORKORDERNO;
                        Rdetail.CREATE_DATE = DateTime.Now;
                        Rdetail.STATUS = "0";
                        Station.SFCDB.ExecSQL(Rdetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        Station.SFCDB.CommitTrain();
                        //No UPDATE EDIT_EMP and  EDIT_TIME ,It is used when reserved for CANCLE ORT

                        //增加彈窗提示
                        UIInputData O = new UIInputData()
                        {
                            Timeout = 60000,
                            UIArea = new string[] { "25%", "28%" },
                            IconType = IconType.None,
                            MustConfirm = false,
                            Message = "OK",
                            Tittle = "",
                            Type = UIInputType.Confirm,
                            Name = "",
                            ErrMessage = ""
                        };
                        //O.OutInputs.Add(new DisplayOutPut() { Name = "Alert", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"{SNObj.SerialNo} has been chosen to ORT test !" });
                        O.OutInputs.Add(new DisplayOutPut() { Name = "Alert", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"{SNObj.SN} has been chosen to ORT test !" });
                        O.GetUiInput(Station.API, UIInput.Normal, Station);
                    }
                }
            }
        }

        public static void OrtScanIn(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_LOT_STATUS Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL Lot_Detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            SN SNObj = new SN();

            R_SN SerialNo = new R_SN();
            string Wo = string.Empty;
            string RouteID = "";
            string NewLotFlag = "";
            string LotSatusID = "";
            int woqty;
            int E;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SNObj = (SN)SNSession.Value;
            Wo = SNObj.WorkorderNo;
            RouteID = SNObj.RouteID;
            SerialNo = SNObj.LoadSN(SNObj.SerialNo, Station.SFCDB);
            var Station_Name = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(c => c.ROUTE_ID == RouteID && c.STATION_NAME == "BURNIN").ToList().FirstOrDefault();
            if (Station_Name != null)
            {
                var LotStatus = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>().Where(C => C.SKUNO == Wo).ToList().FirstOrDefault();
                if (LotStatus == null)
                {
                    NewLotFlag = "1";
                }
                else
                {
                    LotSatusID = LotStatus.ID;
                }
                var WO = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(c => c.WORKORDERNO == Wo).ToList().FirstOrDefault();
                woqty = (int)WO.WORKORDER_QTY;
                if (woqty < 600)
                {
                    E = 6;
                }
                else
                {
                    E = woqty / 100;
                }
                //增加卡關，rework工單不需要進行ORT測試
                var boolCharge = Station.SFCDB.ORM.Queryable<R_WO_TYPE>().Where(t => t.WORKORDER_TYPE == "REWORK" && t.CATEGORY == "RMA" && t.PREFIX == Wo.Substring(1, 6)).Any();
                if (!boolCharge)
                {
                    Lot_Status.OrtInLotPass(NewLotFlag, SerialNo, woqty, E, LotSatusID, "ORT", Station.LoginUser.EMP_NO, Station.Line, Station.BU, Station.SFCDB);
                }

            }
        }

        /// <summary>
        /// 產品掃描MRB過站Action,2018/01/10 肖倫
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNMrbPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            string ZCPP_FLAG = "";//add by fgg 2018.04.10 用於標誌是組裝退料還是從工單入MRB
            SN SnObject = null;
            bool isSame = false;
            string UserEMP = Station.LoginUser.EMP_NO;
            string To_Storage = "";
            string From_Storage = "";
            string Confirmed_Flag = "";
            string DeviceName = "";
            R_SN NewSN = new R_SN();
            R_MRB New_R_MRB = new R_MRB();
            //Modify by LLF
            //H_MRB_GT HMRB_GT = new H_MRB_GT();
            R_MRB_GT HMRB_GT = new R_MRB_GT();
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            //Modify by LLF
            //T_H_MRB_GT TH_MRB_GT = new T_H_MRB_GT(Station.SFCDB, Station.DBType);
            T_R_MRB_GT TH_MRB_GT = new T_R_MRB_GT(Station.SFCDB, Station.DBType);
            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_SN_LOG snLog = new R_SN_LOG();
            if (Paras.Count < 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;
            //SNID必須存在
            if (SnObject.ID == null || SnObject.ID.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //SN如果已經完工，Confirmed_Flag=1，否則Confirmed_Flag=0
            if (SnObject.CompletedFlag != null && SnObject.CompletedFlag == "1")
            {
                Confirmed_Flag = "1";
            }
            else
            {
                Confirmed_Flag = "0";
            }
            //判斷MRBType，0是單板入MRB，1是退料
            //0則From_Storage放空
            //1則From_Storage則取前台傳的工單
            if (Paras[1].VALUE == "0")//0是單板入MRB
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                From_Storage = "";
                ZCPP_FLAG = "0";
            }
            else if (Paras[1].VALUE == "1")//1是退料
            {
                if (Paras.Count != 4)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (WoSession == null || WoSession.Value == null || WoSession.Value.ToString().Length <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else
                {
                    From_Storage = WoSession.Value.ToString();
                    ZCPP_FLAG = "1";
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { "MRBType", "0/1" }));
            }
            //獲取To_Storage
            MESStationSession ToStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ToStorageSession == null || ToStorageSession.Value == null || ToStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                To_Storage = ToStorageSession.Value.ToString();
            }
            //更新SN當前站，下一站，如果SN的compled=1了也就是Confirmed_Flag==1,則修改當前站和下一站即可
            //如果如果SN的compled!=1,則還需要修改sn的compled=1和SN對應工單的finishedQTY要加一
            if (Confirmed_Flag != "1")
            {
                result = TR_SN.SN_Mrb_Pass_action(SnObject.ID, UserEMP, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                }
                else
                {
                    if (SnObject.WorkorderNo == null || SnObject.WorkorderNo.Trim().Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000125", new string[] { SnObject.SerialNo }));
                    }
                    else
                    {
                        TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);//這裡如果工單不存在GetWo會報錯
                        result = TR_WO_BASE.UpdateFINISHEDQTYAddOne(SnObject.WorkorderNo, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + SnObject.SerialNo, "UPDATE" }));
                        }
                        TR_WO_BASE.UpdateWoCloseFlag(SnObject.WorkorderNo, Station.SFCDB);//是否需要關閉工單
                    }
                }
            }
            else
            {
                result = TR_SN.SN_Mrb_Pass_actionNotUpdateCompleted(SnObject.ID, UserEMP, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                }
            }

            //添加一筆MRB記錄
            //給new_r_mrb賦值
            New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
            New_R_MRB.SN = SnObject.SerialNo;
            New_R_MRB.WORKORDERNO = SnObject.WorkorderNo;
            New_R_MRB.NEXT_STATION = SnObject.NextStation;
            New_R_MRB.SKUNO = SnObject.SkuNo;
            New_R_MRB.FROM_STORAGE = From_Storage;
            New_R_MRB.TO_STORAGE = To_Storage;
            New_R_MRB.REWORK_WO = "";//空
            New_R_MRB.CREATE_EMP = UserEMP;
            New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
            New_R_MRB.MRB_FLAG = "1";
            New_R_MRB.SAP_FLAG = "0";
            New_R_MRB.EDIT_EMP = UserEMP;
            New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
            result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + SnObject.SerialNo, "ADD" }));
            }

            //存在R_MRB_GT WO =? And SAP_FLAG = 0,則檢查FROM_STORAGE，TO_STORAGE，CONFIRMED_FLAG是否一樣，一樣則累加1
            if (Paras[1].VALUE == "0")//0是單板入MRB
            {
                //HMRB_GT = TH_MRB_GT.GetByWOAndSAPFlageIsZero(SnObject.WorkorderNo, Station.SFCDB);
                HMRB_GT = Station.SFCDB.ORM.Queryable<R_MRB_GT>()
                    .Where(r => r.WORKORDERNO == SnObject.WorkorderNo && r.ZCPP_FLAG == ZCPP_FLAG && r.CONFIRMED_FLAG == Confirmed_Flag && r.SAP_FLAG == "0")
                    .ToList().FirstOrDefault();

            }
            else if (Paras[1].VALUE == "1")//1是退料
            {
                //HMRB_GT = TH_MRB_GT.GetByWOAndSAPFlageIsZero(From_Storage, Station.SFCDB);
                HMRB_GT = Station.SFCDB.ORM.Queryable<R_MRB_GT>()
                    .Where(r => r.WORKORDERNO == From_Storage && r.ZCPP_FLAG == ZCPP_FLAG && r.CONFIRMED_FLAG == Confirmed_Flag && r.SAP_FLAG == "0")
                    .ToList().FirstOrDefault();
            }

            isSame = false;
            if (HMRB_GT != null)
            {
                HMRB_GT.FROM_STORAGE = (HMRB_GT.FROM_STORAGE == null || HMRB_GT.FROM_STORAGE.Trim().Length <= 0) ? "" : HMRB_GT.FROM_STORAGE;
                HMRB_GT.TO_STORAGE = (HMRB_GT.TO_STORAGE == null || HMRB_GT.TO_STORAGE.Trim().Length <= 0) ? "" : HMRB_GT.TO_STORAGE;
                HMRB_GT.CONFIRMED_FLAG = (HMRB_GT.CONFIRMED_FLAG == null || HMRB_GT.CONFIRMED_FLAG.Trim().Length <= 0) ? "" : HMRB_GT.CONFIRMED_FLAG;
                if (HMRB_GT.FROM_STORAGE == New_R_MRB.FROM_STORAGE && HMRB_GT.TO_STORAGE == New_R_MRB.TO_STORAGE && HMRB_GT.CONFIRMED_FLAG == Confirmed_Flag)
                {
                    isSame = true;
                    if (Paras[1].VALUE == "0")//0是單板入MRB
                    {
                        //result = TH_MRB_GT.updateTotalQTYAddOne(SnObject.WorkorderNo, UserEMP, Station.SFCDB);
                        result = TH_MRB_GT.updateTotalQTYAddOne(SnObject.WorkorderNo, UserEMP, Confirmed_Flag, Station.SFCDB);
                    }
                    else if (Paras[1].VALUE == "1")//1是退料
                    {
                        //result = TH_MRB_GT.updateTotalQTYAddOne(From_Storage, UserEMP, Station.SFCDB);
                        result = TH_MRB_GT.updateTotalQTYAddOne(From_Storage, UserEMP, Confirmed_Flag, Station.SFCDB);
                    }

                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB_GT:" + SnObject.SerialNo, "UPDATE" }));
                    }
                }
            }
            if (!isSame)
            {
                //Modify by LLF
                //H_MRB_GT New_HMRB_GT = new H_MRB_GT();
                R_MRB_GT New_HMRB_GT = new R_MRB_GT();
                //賦值
                New_HMRB_GT.ID = TH_MRB_GT.GetNewID(Station.BU, Station.SFCDB);
                if (Paras[1].VALUE == "0")//0是單板入MRB
                {
                    New_HMRB_GT.WORKORDERNO = SnObject.WorkorderNo;
                }
                else if (Paras[1].VALUE == "1")//1是退料
                {
                    New_HMRB_GT.WORKORDERNO = From_Storage;
                }
                Row_R_WO_BASE RowWO_BASE = TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);
                //string sapStationCode = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySkuAndWorkorderType(SnObject.SkuNo, RowWO_BASE.WO_TYPE, Station.SFCDB);
                string sapStationCode = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySku(SnObject.SkuNo, Station.SFCDB);
                if (sapStationCode == "")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000224", new string[] { SnObject.SkuNo }));
                }
                New_HMRB_GT.SAP_STATION_CODE = sapStationCode;
                New_HMRB_GT.FROM_STORAGE = From_Storage;
                New_HMRB_GT.TO_STORAGE = To_Storage;
                New_HMRB_GT.TOTAL_QTY = 1;
                New_HMRB_GT.CONFIRMED_FLAG = Confirmed_Flag;
                New_HMRB_GT.ZCPP_FLAG = ZCPP_FLAG;//暫時預留
                New_HMRB_GT.SAP_FLAG = "0";//0待拋,1已拋,2待重拋
                New_HMRB_GT.SKUNO = SnObject.SkuNo;
                New_HMRB_GT.SAP_MESSAGE = "";
                New_HMRB_GT.EDIT_EMP = UserEMP;
                New_HMRB_GT.EDIT_TIME = Station.GetDBDateTime();
                result = TH_MRB_GT.Add(New_HMRB_GT, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "H_MRB_GT:" + SnObject.SerialNo, "ADD" }));
                }
            }
            //添加過站記錄
            var snobj = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == SnObject.ID)
                .ToList().FirstOrDefault();
            result = Convert.ToInt32(TR_SN.RecordPassStationDetail(snobj, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + SnObject.SerialNo, "ADD" }));
            }

        }
        /// <summary>
        /// Save IP to Log
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void MRBSaveLog(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string UserEMP = Station.LoginUser.EMP_NO;
            SN SnObject = null;
            T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_SN_LOG snLog = new R_SN_LOG();
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;
            if (SnObject.ID == null || SnObject.ID.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var snobj = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == SnObject.ID)
                .ToList().FirstOrDefault();
            snLog.ID = t_r_sn_log.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
            snLog.SN = snobj.SN;
            snLog.SNID = SnObject.ID;
            snLog.LOGTYPE = Station.StationName;
            snLog.DATA1 = SnObject.WorkorderNo;
            snLog.DATA8 = Station.IP;
            snLog.FLAG = "1";
            snLog.CREATETIME = t_r_sn_log.GetDBDateTime(Station.SFCDB);
            snLog.CREATEBY = UserEMP;
            t_r_sn_log.Save(Station.SFCDB, snLog);
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass);
        }
        public static void ReworkPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            SN SnObject = null;
            R_SN NewSN = new R_SN();
            string UserEMP = Station.LoginUser.EMP_NO;
            string SNValidFlag = "0";
            string DeviceName = "";
            string ReworkStation = "";
            WorkOrder WoObject = null;
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_H_MRB_GT TH_MRB_GT = new T_H_MRB_GT(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN_PACKING TRSP = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
            if (Paras.Count != 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;
            //SNID必須要存在
            if (SnObject.ID == null || SnObject.ID.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //獲取工單對象
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (WOSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            WoObject = (WorkOrder)WOSession.Value;
            if (WoObject.WorkorderNo == null || WoObject.WorkorderNo.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession NextStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (NextStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else if (NextStationSession.Value == null || NextStationSession.Value.ToString().Trim().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                ReworkStation = NextStationSession.Value.ToString().Trim();
            }

            //更新SN的Valid_Flag=0
            result = TR_SN.updateValid_Flag(SnObject.ID, SNValidFlag, UserEMP, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
            }

            //新增一筆SN 與重工工單記錄
            NewSN = TR_SN.GetById(SnObject.ID, Station.SFCDB);
            NewSN.ID = TR_SN.GetNewID(Station.BU, Station.SFCDB);
            NewSN.SKUNO = WoObject.SkuNO;
            NewSN.WORKORDERNO = WoObject.WorkorderNo;
            NewSN.PLANT = WoObject.PLANT;//更新廠別，防止外面來的SN重工後取到舊的廠別 Edit By ZHB 2020年8月11日08:40:25
            NewSN.CURRENT_STATION = "REWORK";
            NewSN.NEXT_STATION = ReworkStation;
            NewSN.ROUTE_ID = WoObject.RouteID;
            NewSN.VALID_FLAG = "1";
            NewSN.COMPLETED_FLAG = "0";
            NewSN.COMPLETED_TIME = null;
            if (Station.BU.Equals("BPD") || Station.BU.Equals("VNDCN") || Station.BU.Equals("VNJUNIPER"))
            {
                List<C_ROUTE_DETAIL> details = TCRD.GetByRouteIdOrderBySEQASC(WoObject.RouteID, Station.SFCDB);
                C_ROUTE_DETAIL PackStation = details.Find(t => t.STATION_NAME == "CARTON" || t.STATION_NAME == "PACKOUT");//VNJUNIPER Carton StationName=PACKOUT
                C_ROUTE_DETAIL ShipStation = details.Find(t => t.STATION_NAME == "SHIPOUT");
                //如果回到出貨狀態之前，就一定把 SHIPPED_FLAG 改成0
                if (ShipStation != null && details.Any(t => t.STATION_NAME == ReworkStation && t.SEQ_NO <= ShipStation.SEQ_NO))
                {
                    NewSN.SHIPPED_FLAG = "0";
                    NewSN.SHIPDATE = null;
                }
                //如果回到包裝之前，就一定把 PACKED_FLAG 改成0
                if (PackStation != null && details.Any(t => t.STATION_NAME == ReworkStation && t.SEQ_NO <= PackStation.SEQ_NO))
                {
                    NewSN.PACKED_FLAG = "0";
                    NewSN.PACKDATE = null;
                }
                //如果回到包裝之後，就需要用新的 SN_ID 替換掉 R_SN_PACKING 裡面舊的 SN_ID
                if (PackStation != null && details.Any(t => t.STATION_NAME == ReworkStation && t.SEQ_NO > PackStation.SEQ_NO))
                {
                    TRSP.ReplaceOldSnId(SnObject.ID, NewSN.ID, Station.SFCDB);
                }
            }

            #region FJZ keep the sn repair status after rework
            if (Station.BU.Equals("FJZ"))
            {
                NewSN.REPAIR_FAILED_FLAG = SnObject.RepairFailedFlag;
                var R_Main = Station.SFCDB.ORM.Queryable<R_REPAIR_MAIN>()
                    .Where(t => t.SN == SnObject.baseSN.SN && t.WORKORDERNO == SnObject.baseSN.WORKORDERNO && t.CLOSED_FLAG == "0")
                    .First();
                if (R_Main != null)
                {
                    R_Main.WORKORDERNO = WoObject.WorkorderNo;
                    Station.SFCDB.ORM.Updateable<R_REPAIR_MAIN>(R_Main).ExecuteCommand();

                    var R_Trans = Station.SFCDB.ORM.Queryable<R_REPAIR_TRANSFER>()
                        .Where(t => t.REPAIR_MAIN_ID == R_Main.ID).First();
                    if (R_Trans != null)
                    {
                        R_Trans.WORKORDERNO = WoObject.WorkorderNo;
                        Station.SFCDB.ORM.Updateable<R_REPAIR_TRANSFER>(R_Trans).ExecuteCommand();
                    }
                }
            }
            #endregion

            NewSN.PRODUCT_STATUS = "REWORK";
            if (NewSN.REWORK_COUNT == null)
            {
                NewSN.REWORK_COUNT = 1;
            }
            else
            {
                NewSN.REWORK_COUNT = NewSN.REWORK_COUNT + 1;
            }
            NewSN.EDIT_EMP = UserEMP;
            NewSN.EDIT_TIME = Station.GetDBDateTime();
            result = TR_SN.AddNewSN(NewSN, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "ADD" }));
            }
            //更新r_MRB
            result = TR_MRB.OutMrbUpdate(WoObject.WorkorderNo, UserEMP, SnObject.SerialNo, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + SnObject.SerialNo, "UPDATE" }));
            }
            //更新工單投入數量
            result = Convert.ToInt32(TR_WO_BASE.AddCountToWo(WoObject.WorkorderNo, 1, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + WoObject.WorkorderNo, "UPDATE" }));
            }
            //添加TR_SN_STATION_DETAIL
            result = Convert.ToInt32(TR_SN.RecordPassStationDetail(NewSN.SN, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + SnObject.SerialNo, "ADD" }));
            }

            //SN newSn = new SN(SnObject.SerialNo, Station.SFCDB, Station.DBType);
            //int updateResult = newSn.UpdateKPSNID(Station.SFCDB);
            //if (result <= 0)
            //{
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + SnObject.SerialNo, "UPDATE" }));
            //}
            //處理重工的keypart
            SnObject.ReworkUpateSNKP(NewSN, Station, ReworkStation);

            //if (Station.BU == "HWT")
            //{
            //    result= TR_SN.updateValid_Flag(SnObject.ID, SNValidFlag, UserEMP, Station.SFCDB);
            //}
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        ///產品單個維修動作完成Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSaveRepairAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            //Object ReplaceIDObject;
            string UpdateSql = null;
            string VRepairer = Station.LoginUser.EMP_NO;
            string VDuty = "";
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_ACTION RepairRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();
            T_C_REPAIR_ITEMS TTRepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RepairItemsRow;
            T_C_REPAIR_ITEMS_SON HHRepairItemSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS_SON RepairItemSonRow;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SnObject = (SN)SNSession.Value;

                MESStationInput RepairedBySession = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
                if (RepairedBySession.Value.ToString() != "")
                {
                    VRepairer = RepairedBySession.Value.ToString();
                }

                MESStationInput ActionCodeSession = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                string VActionCode = ActionCodeSession.Value.ToString();

                MESStationInput RootCauseSession = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE);
                if (RootCauseSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (RootCauseSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                string VRootCause = RootCauseSession.Value.ToString();

                MESStationInput DutySession = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE);
                if (DutySession.Value.ToString() != "")
                {
                    VDuty = DutySession.Value.ToString();
                }

                MESStationSession TR_SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
                if (TR_SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else if (TR_SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                string VTR_SN = TR_SNSession.Value.ToString();

                MESStationSession PartNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);
                if (PartNoSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                else if (PartNoSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
                }
                string VPartNo = PartNoSession.Value.ToString();

                MESStationSession MFRNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[7].SESSION_TYPE && t.SessionKey == Paras[7].SESSION_KEY);
                if (MFRNameSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                }
                else if (MFRNameSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[7].SESSION_TYPE + Paras[7].SESSION_KEY }));
                }
                string VMFRName = MFRNameSession.Value.ToString();

                MESStationSession DateCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[8].SESSION_TYPE && t.SessionKey == Paras[8].SESSION_KEY);
                if (DateCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                else if (DateCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                string VDateCode = DateCodeSession.Value.ToString();

                MESStationSession LotCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[9].SESSION_TYPE && t.SessionKey == Paras[9].SESSION_KEY);
                if (LotCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE + Paras[9].SESSION_KEY }));
                }
                else if (LotCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[9].SESSION_TYPE + Paras[9].SESSION_KEY }));
                }
                string VLotCode = LotCodeSession.Value.ToString();

                MESStationInput DescriptionSession = Station.Inputs.Find(t => t.DisplayName == Paras[10].SESSION_TYPE);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                }
                string VDescription = DescriptionSession.Value.ToString();

                MESStationInput FailCodeIDSession = Station.Inputs.Find(t => t.DisplayName == Paras[11].SESSION_TYPE);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                }
                string VFailCodeID = FailCodeIDSession.Value.ToString();

                if (VFailCodeID != null)
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(VFailCodeID, Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, VFailCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[11].SESSION_TYPE + Paras[11].SESSION_KEY }));
                }

                MESStationInput RepairItemSession = Station.Inputs.Find(t => t.DisplayName == Paras[12].SESSION_TYPE);

                string RepairItem = RepairItemSession.Value.ToString();

                if (string.IsNullOrEmpty(RepairItem))
                {
                    RepairItem = "";
                }
                else
                {
                    RepairItemsRow = TTRepairItems.GetIDByItemName(RepairItem, Station.SFCDB);
                    RepairItem = RepairItemsRow.ID;
                }

                MESStationInput RepairItemSonSession = Station.Inputs.Find(t => t.DisplayName == Paras[13].SESSION_TYPE);

                string RepairItemSon = RepairItemSonSession.Value.ToString();

                if (string.IsNullOrEmpty(RepairItemSon))
                {
                    RepairItemSon = "";
                }
                else
                {
                    RepairItemSonRow = HHRepairItemSon.GetIDByItemsSon(RepairItemSon, Station.SFCDB);
                    RepairItemSon = RepairItemSonRow.ID;
                }

                MESStationInput LocationSession = Station.Inputs.Find(t => t.DisplayName == Paras[14].SESSION_TYPE);
                if (LocationSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                }
                else if (LocationSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[14].SESSION_TYPE + Paras[14].SESSION_KEY }));
                }
                string Location = LocationSession.Value.ToString();

                MESStationInput ProcessSession = Station.Inputs.Find(t => t.DisplayName == Paras[15].SESSION_TYPE);
                if (ProcessSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                else if (ProcessSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                string Process = ProcessSession.Value.ToString();

                Station.SFCDB.BeginTrain();
                RepairRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                RepairRow.REPAIR_FAILCODE_ID = VFailCodeID;
                RepairRow.SN = SnObject.SerialNo;
                RepairRow.ACTION_CODE = VActionCode;
                RepairRow.SECTION_ID = VDuty;
                RepairRow.PROCESS = Process;//FailCodeRow.FAIL_PROCESS;
                RepairRow.ITEMS_ID = RepairItem;
                RepairRow.ITEMS_SON_ID = RepairItemSon;
                RepairRow.REASON_CODE = VRootCause;
                RepairRow.DESCRIPTION = VDescription;
                RepairRow.FAIL_LOCATION = Location;//FailCodeRow.FAIL_LOCATION;
                RepairRow.FAIL_CODE = FailCodeRow.FAIL_CODE;
                RepairRow.KEYPART_SN = VPartNo;
                RepairRow.NEW_KEYPART_SN = "";
                RepairRow.TR_SN = VTR_SN;
                RepairRow.MFR_NAME = VMFRName;
                RepairRow.DATE_CODE = VDateCode;
                RepairRow.LOT_CODE = VLotCode;
                RepairRow.REPAIR_EMP = VRepairer;
                RepairRow.REPAIR_TIME = Station.GetDBDateTime();
                RepairRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                RepairRow.EDIT_TIME = Station.GetDBDateTime();

                string StrRes = Station.SFCDB.ExecSQL(RepairRow.GetInsertString(Station.DBType));
                if (StrRes == "1")
                {
                    Row_R_REPAIR_FAILCODE FRow = (Row_R_REPAIR_FAILCODE)RepairFailcode.GetObjByID(VFailCodeID, Station.SFCDB);
                    FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    UpdateSql = FRow.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(UpdateSql);
                    Station.SFCDB.CommitTrain();
                    Station.AddMessage("MES00000105", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Pass);
                }
                else
                {
                    Station.SFCDB.RollbackTrain();
                    Station.AddMessage("MES00000083", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Fail);
                }
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.5.31
        ///產品單個維修動作完成Action New
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSaveRepairActionNew(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            string UpdateSql = null;
            string VRepairer = Station.LoginUser.EMP_NO;

            string VFailCodeID = "";
            string VActionCode = "";
            string RepairReseaonCode = "";
            string Process = "";
            string Location = "";
            string VSection = "";
            string RepairItem = "";
            string RepairItemSon = "";
            string RepairKeypartSN = "";

            string VTR_SN = "";
            string VKPNO = "";
            string VMFRName = "";
            string VDateCode = "";
            string VLotCode = "";

            string VDescription = "";

            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_ACTION RepairRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();
            T_C_REPAIR_ITEMS TTRepairItems = new T_C_REPAIR_ITEMS(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS RepairItemsRow;
            T_C_REPAIR_ITEMS_SON HHRepairItemSon = new T_C_REPAIR_ITEMS_SON(Station.SFCDB, Station.DBType);
            Row_C_REPAIR_ITEMS_SON RepairItemSonRow;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            try
            {
                //獲取到 SN 對象
                MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SNSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                else if (SNSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                SnObject = (SN)SNSession.Value;

                MESStationInput FailCodeIDSession = Station.Inputs.Find(t => t.DisplayName == Paras[1].SESSION_TYPE);
                if (FailCodeIDSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                else if (FailCodeIDSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                VFailCodeID = FailCodeIDSession.Value.ToString();

                if (VFailCodeID != null)
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(VFailCodeID, Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000191", new string[] { SnObject.SerialNo, VFailCodeID }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }

                MESStationInput ActionCodeSession = Station.Inputs.Find(t => t.DisplayName == Paras[2].SESSION_TYPE);
                if (ActionCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                else if (ActionCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                VActionCode = ActionCodeSession.Value.ToString();

                MESStationInput ReseaonCodeSession = Station.Inputs.Find(t => t.DisplayName == Paras[3].SESSION_TYPE);
                if (ReseaonCodeSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else if (ReseaonCodeSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                RepairReseaonCode = ReseaonCodeSession.Value.ToString();

                MESStationInput ProcessSession = Station.Inputs.Find(t => t.DisplayName == Paras[4].SESSION_TYPE);
                if (ProcessSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                else if (ProcessSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
                }
                Process = ProcessSession.Value.ToString();

                MESStationInput LocationSession = Station.Inputs.Find(t => t.DisplayName == Paras[5].SESSION_TYPE);
                if (LocationSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                else if (LocationSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
                }
                Location = LocationSession.Value.ToString();

                MESStationInput SectionSession = Station.Inputs.Find(t => t.DisplayName == Paras[6].SESSION_TYPE);
                if (SectionSession != null && SectionSession.Value != null)
                {
                    VSection = SectionSession.Value.ToString();
                }

                MESStationInput RepairItemSession = Station.Inputs.Find(t => t.DisplayName == Paras[7].SESSION_TYPE);
                if (RepairItemSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                RepairItem = RepairItemSession.Value.ToString();
                if (string.IsNullOrEmpty(RepairItem))
                {
                    RepairItem = "";
                }
                else
                {
                    RepairItemsRow = TTRepairItems.GetIDByItemName(RepairItem, Station.SFCDB);
                    RepairItem = RepairItemsRow.ID;
                }

                MESStationInput RepairItemSonSession = Station.Inputs.Find(t => t.DisplayName == Paras[8].SESSION_TYPE);
                if (RepairItemSonSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[8].SESSION_TYPE + Paras[8].SESSION_KEY }));
                }
                RepairItemSon = RepairItemSonSession.Value.ToString();
                if (string.IsNullOrEmpty(RepairItemSon))
                {
                    RepairItemSon = "";
                }
                else
                {
                    RepairItemSonRow = HHRepairItemSon.GetIDByItemsSon(RepairItemSon, Station.SFCDB);
                    RepairItemSon = RepairItemSonRow.ID;
                }

                MESStationInput KeypartSNSession = Station.Inputs.Find(t => t.DisplayName == Paras[9].SESSION_TYPE);
                if (KeypartSNSession != null && KeypartSNSession.Value != null)
                {
                    RepairKeypartSN = KeypartSNSession.Value.ToString();
                }

                MESStationSession TR_SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[10].SESSION_TYPE && t.SessionKey == Paras[10].SESSION_KEY);
                MESStationSession KPNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[11].SESSION_TYPE && t.SessionKey == Paras[11].SESSION_KEY);
                MESStationSession MFRNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[12].SESSION_TYPE && t.SessionKey == Paras[12].SESSION_KEY);
                MESStationSession DateCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[13].SESSION_TYPE && t.SessionKey == Paras[13].SESSION_KEY);
                MESStationSession LotCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[14].SESSION_TYPE && t.SessionKey == Paras[14].SESSION_KEY);

                //如果有輸入ALLPART條碼,則取ALLPART條碼對應的料號、廠商、DateCode、LotCode，沒有則取輸入的值
                if (TR_SNSession != null)
                {
                    if (TR_SNSession.Value == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[10].SESSION_TYPE + Paras[10].SESSION_KEY }));
                    }
                    DataRow tr_sn = (DataRow)TR_SNSession.Value;
                    VTR_SN = tr_sn["TR_SN"].ToString();
                    VKPNO = tr_sn["CUST_KP_NO"].ToString();
                    VMFRName = tr_sn["MFR_KP_NO"].ToString();
                    VDateCode = tr_sn["DATE_CODE"].ToString();
                    VLotCode = tr_sn["Lot_Code"].ToString();
                }
                else
                {
                    if (KPNOSession != null && KPNOSession.Value != null)
                    {
                        VKPNO = KPNOSession.Value.ToString();
                    }

                    if (MFRNameSession != null && MFRNameSession.Value != null)
                    {
                        VMFRName = MFRNameSession.Value.ToString();
                    }

                    if (DateCodeSession != null && DateCodeSession.Value != null)
                    {
                        VDateCode = DateCodeSession.Value.ToString();
                    }

                    if (LotCodeSession != null && LotCodeSession.Value != null)
                    {
                        VLotCode = LotCodeSession.Value.ToString();
                    }
                }

                MESStationInput DescriptionSession = Station.Inputs.Find(t => t.DisplayName == Paras[15].SESSION_TYPE);
                if (DescriptionSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                else if (DescriptionSession.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[15].SESSION_TYPE + Paras[15].SESSION_KEY }));
                }
                VDescription = DescriptionSession.Value.ToString();
                Station.SFCDB.BeginTrain();
                RepairRow.ID = RepairAction.GetNewID(Station.BU, Station.SFCDB);
                RepairRow.REPAIR_FAILCODE_ID = VFailCodeID;
                RepairRow.SN = SnObject.SerialNo;
                RepairRow.ACTION_CODE = VActionCode;
                RepairRow.SECTION_ID = VSection;
                RepairRow.PROCESS = Process;
                RepairRow.ITEMS_ID = RepairItem;
                RepairRow.ITEMS_SON_ID = RepairItemSon;
                RepairRow.REASON_CODE = RepairReseaonCode;
                RepairRow.DESCRIPTION = VDescription;
                RepairRow.FAIL_LOCATION = Location;
                RepairRow.FAIL_CODE = FailCodeRow.FAIL_CODE;
                RepairRow.KEYPART_SN = "";
                RepairRow.NEW_KEYPART_SN = RepairKeypartSN;
                RepairRow.TR_SN = VTR_SN;
                RepairRow.KP_NO = VKPNO;
                RepairRow.MFR_NAME = VMFRName;
                RepairRow.DATE_CODE = VDateCode;
                RepairRow.LOT_CODE = VLotCode;
                RepairRow.REPAIR_EMP = VRepairer;
                RepairRow.REPAIR_TIME = Station.GetDBDateTime();
                RepairRow.EDIT_EMP = Station.LoginUser.EMP_NO;
                RepairRow.EDIT_TIME = Station.GetDBDateTime();

                string StrRes = Station.SFCDB.ExecSQL(RepairRow.GetInsertString(Station.DBType));
                if (StrRes == "1")
                {
                    Row_R_REPAIR_FAILCODE FRow = (Row_R_REPAIR_FAILCODE)RepairFailcode.GetObjByID(VFailCodeID, Station.SFCDB);
                    FRow.REPAIR_FLAG = "1";  //執行完維修動作後更新R_REPAIR_FAILCODE   FLAG=1
                    FRow.EDIT_TIME = Station.GetDBDateTime();
                    UpdateSql = FRow.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(UpdateSql);
                    Station.SFCDB.CommitTrain();
                    Station.AddMessage("MES00000105", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Pass);
                }
                else
                {
                    Station.SFCDB.RollbackTrain();
                    Station.AddMessage("MES00000083", new string[] { SnObject.SerialNo, VFailCodeID }, StationMessageState.Fail);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        ///產品所有維修動作完成Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNRepairFinishAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            string UpdateSql = "";
            string result = "";
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            //Row_R_REPAIR_FAILCODE FailCodeRow;
            T_R_REPAIR_ACTION RepairAction = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_ACTION RepairRow = (Row_R_REPAIR_ACTION)RepairAction.NewRow();
            T_R_REPAIR_MAIN RMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            List<R_REPAIR_MAIN> RepairMainInfo = new List<R_REPAIR_MAIN>();
            List<R_REPAIR_FAILCODE> FailCodeInfo = new List<R_REPAIR_FAILCODE>();
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string DeviceName = Station.StationName;

            if (Paras.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SnObject = (SN)SNSession.Value;

            //獲取 DEVICE1
            MESStationSession DeviceSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (DeviceSession != null)
            {
                DeviceName = DeviceSession.Value.ToString();
            }

            try
            {
                RepairMainInfo = RMain.GetRepairMainBySN(Station.SFCDB, SnObject.SerialNo);
                if (RepairMainInfo != null && RepairMainInfo[0].CLOSED_FLAG == "0")
                {
                    FailCodeInfo = RepairFailcode.CheckSNRepairFinishAction(Station.SFCDB, SnObject.SerialNo, RepairMainInfo[0].ID);
                    if (FailCodeInfo.Count != 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000106", new string[] { SnObject.SerialNo, FailCodeInfo[0].ID })); ///未维修完成的无法update repair_main 表信息
                    }
                    else
                    {
                        table.RecordPassStationDetail(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);   //添加过站记录

                        //執行完所有的維修動作後才能更新R_REPAIR_MAIN  FLAG=1
                        Row_R_REPAIR_MAIN FRow = (Row_R_REPAIR_MAIN)RMain.GetObjByID(RepairMainInfo[0].ID, Station.SFCDB);
                        FRow.CLOSED_FLAG = "1";
                        FRow.EDIT_TIME = Station.GetDBDateTime();
                        UpdateSql = FRow.GetUpdateString(Station.DBType);
                        Station.SFCDB.ExecSQL(UpdateSql);

                        //執行完所有的維修動作後 更新R_SN  FLAG=0
                        Row_R_SN SnRow = (Row_R_SN)table.GetObjByID(SnObject.ID, Station.SFCDB);
                        SnRow.REPAIR_FAILED_FLAG = "0";
                        SnRow.EDIT_TIME = Station.GetDBDateTime();
                        UpdateSql = SnRow.GetUpdateString(Station.DBType);
                        result = Station.SFCDB.ExecSQL(UpdateSql);
                        if (Convert.ToInt32(result) > 0)
                        {
                            foreach (R_Station_Output output in Station.StationOutputs)
                            {
                                if (Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY) != null)
                                {
                                    Station.StationSession.Find(s => s.MESDataType == output.SESSION_TYPE && s.SessionKey == output.SESSION_KEY).Value = "";
                                }
                            }

                            foreach (MESStationInput input in Station.Inputs)
                            {
                                if (Station.StationSession.Find(s => s.MESDataType == input.DisplayName) != null)
                                {
                                    Station.StationSession.Find(s => s.MESDataType == input.DisplayName).Value = "";
                                }
                                input.Value = "";
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
        ///SN批次解鎖ACTION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFQCLotUnlockAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_LOT_DETAIL Ulotdetail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);

            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SessionSNorLotNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSNorLotNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SessionSNorLotNo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string SnOrLotno = SessionSNorLotNo.Value.ToString();

            try
            {
                //Station.SFCDB.BeginTrain();
                Ulotdetail.UnLockLotBySnOrLotNo(SnOrLotno, Station.LoginUser.EMP_NO, Station.SFCDB);
                Station.AddMessage("MES00000173", new string[] { SnOrLotno }, StationMessageState.Pass); //回饋消息到前台
                //Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                //Station.SFCDB.RollbackTrain();
                throw ex;
            }
        }

        public static void SNStockInPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string DeviceName = "";
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession SessionDevice = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SessionDevice == null)
            {
                DeviceName = Station.StationName;
            }
            SN SNObj = (SN)SessionSN.Value;

            T_R_SN T_R_Sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_SN Rows = (Row_R_SN)T_R_Sn.GetObjByID(SNObj.ID, Station.SFCDB);
            Rows.STOCK_STATUS = "1";
            Rows.STOCK_IN_TIME = Station.GetDBDateTime();
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            T_R_Sn.RecordPassStationDetail(SNObj.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);

            //R_SN R_Sn = T_R_Sn.LoadSN(SNObj.SerialNo, Station.SFCDB);
            Station.AddMessage("MES00000063", new string[] { SNObj.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// Vertiv SN Stockin 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void VertivSNStockInPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string DeviceName = "";
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession SessionDevice = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SessionDevice == null)
            {
                DeviceName = Station.StationName;
            }
            SN SNObj = (SN)SessionSN.Value;

            T_R_SN T_R_Sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_SN Rows = (Row_R_SN)T_R_Sn.GetObjByID(SNObj.ID, Station.SFCDB);

            Dictionary<string, object> dicNextStation;
            string nextStation = "";
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);

            dicNextStation = t_c_route_detail.GetNextStations(Rows.ROUTE_ID, Station.StationName, Station.SFCDB);
            nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

            Rows.CURRENT_STATION = Station.StationName;
            Rows.NEXT_STATION = nextStation;
            Rows.STOCK_STATUS = "1";
            Rows.STOCK_IN_TIME = Station.GetDBDateTime();
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            T_R_Sn.RecordPassStationDetail(SNObj.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);

            //R_SN R_Sn = T_R_Sn.LoadSN(SNObj.SerialNo, Station.SFCDB);
            Station.AddMessage("MES00000063", new string[] { SNObj.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        public static void SNPreSCRAPPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string DeviceName = "";
            MESStationSession SessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionSN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession SessionDevice = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SessionDevice == null)
            {
                DeviceName = Station.StationName;
            }
            SN SNObj = (SN)SessionSN.Value;

            T_R_SN T_R_Sn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_SN Rows = (Row_R_SN)T_R_Sn.GetObjByID(SNObj.ID, Station.SFCDB);
            Rows.CURRENT_STATION = Station.StationName;
            Rows.NEXT_STATION = "MRB";
            Rows.SCRAPED_FLAG = "1";
            Rows.SCRAPED_TIME = Station.GetDBDateTime();
            Rows.EDIT_TIME = Station.GetDBDateTime();
            Station.SFCDB.ExecSQL(Rows.GetUpdateString(Station.DBType));

            T_R_Sn.RecordPassStationDetail(SNObj.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB);

            Station.AddMessage("MES00000063", new string[] { SNObj.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        public static void CounterAddAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count == 0)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession CounterSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            MESStationSession AddSession = null;// = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            try
            {
                AddSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            }
            catch
            { }
            int add = 1;
            if (AddSession != null)
            {
                add = int.Parse(AddSession.Value.ToString());
            }

            try
            {
                AddSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            }
            catch
            { }

            if (CounterSession == null || CounterSession.Value == null)
            {
                CounterSession = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(CounterSession);
                CounterSession.Value = add;
            }
            else
            {
                CounterSession.Value = (int)CounterSession.Value + add;
            }
        }

        /// <summary>
        /// 解除SN的keyparts綁定關係,把R_SN_KEYPART_DETAIL表的VAIID改為0   0表示無效，1代表有效
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNUnlinkAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string SerialSn = SNSession.Value.ToString();

            List<R_SN_KEYPART_DETAIL> LinkSN = new List<R_SN_KEYPART_DETAIL>();
            T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string UpdateSql = "";
            string KeyPartSnID = null;
            MESStationSession LinksnSesssion = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LinksnSesssion == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            else if (LinksnSesssion.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            if (LinksnSesssion != null)
            {
                LinkSN = (List<R_SN_KEYPART_DETAIL>)LinksnSesssion.Value;
                try
                {
                    Station.SFCDB.BeginTrain();
                    for (int i = 0; i < LinkSN.Count; i++)
                    {
                        KeyPartSnID = LinkSN[i].ID;
                        Row_R_SN_KEYPART_DETAIL RowKeypartSNID = (Row_R_SN_KEYPART_DETAIL)stk.GetObjByID(KeyPartSnID, Station.SFCDB);
                        RowKeypartSNID.VALID = "0";
                        UpdateSql = RowKeypartSNID.GetUpdateString(Station.DBType);
                        Station.SFCDB.ExecSQL(UpdateSql);
                    }

                    Station.SFCDB.CommitTrain();
                }
                catch (Exception ex)
                {
                    Station.SFCDB.RollbackTrain();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 解除SN的keyparts綁定關係後，把R_SN最新的VALID_FLAG=0的數據恢復為VALID_FLAG=1,並把和該SN綁定的板子打入MRB
        ///解除綁定關係是一步完成的，中間并未執行其餘動作，VALID_FLAG=0只能起到控制該產品在此刻不能生產的作用
        /// 而實際上，UNLINK這個動作在很短時間内就完成了，所以R_SN表中的valid_flag並不需要改變。
        /// 另此處將valid改爲0，後續并未將其改回1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LaststageofunlinkAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            R_MRB New_R_MRB = new R_MRB();
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);

            List<R_SN_KEYPART_DETAIL> LinkSN = new List<R_SN_KEYPART_DETAIL>();
            T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN shk = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN> GetRsnList = new List<R_SN>();
            List<R_MRB> GetMRBList = new List<R_MRB>();

            string UpdateSql = "";
            string RSNID = null;

            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN SerialSn = (SN)SNSession.Value;
            try
            {
                //註釋掉R_SN  VALID_FLAG=1的數據，恢復之前VALID_FLAG=0 時間最晚的數據

                Station.SFCDB.BeginTrain();
                Row_R_SN RowSN = (Row_R_SN)shk.GetObjByID(SerialSn.ID, Station.SFCDB);
                RowSN.VALID_FLAG = "1";
                RowSN.NEXT_STATION = "LINK";
                UpdateSql = RowSN.GetUpdateString(Station.DBType);
                Station.SFCDB.ExecSQL(UpdateSql);

                //GetRsnList = shk.GetINVaildSN(SerialSn.SerialNo, SerialSn.ID, Station.SFCDB);

                //if (GetRsnList != null && GetRsnList.Count > 0)
                //{
                //    Row_R_SN RowInvailSN = (Row_R_SN)shk.GetObjByID(GetRsnList[0].ID, Station.SFCDB);
                //    RowInvailSN.VALID_FLAG = "1";
                //    UpdateSql = RowInvailSN.GetUpdateString(Station.DBType);
                //    Station.SFCDB.ExecSQL(UpdateSql);
                //}

                //Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw ex;
            }

            string FROM_STORAGE = "";
            string TO_STORAGE = "";

            GetMRBList = TR_MRB.GetMrbBySN(SerialSn.SerialNo, Station.SFCDB);
            if (GetMRBList != null && GetMRBList.Count > 0)
            {
                FROM_STORAGE = GetMRBList[0].FROM_STORAGE;
                TO_STORAGE = GetMRBList[0].TO_STORAGE;
            }
            //處理被LINK的數據
            MESStationSession LinksnSesssion = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LinksnSesssion != null)
            {
                LinkSN = (List<R_SN_KEYPART_DETAIL>)LinksnSesssion.Value;
                try
                {
                    Station.SFCDB.BeginTrain();

                    for (int i = 0; i < LinkSN.Count; i++)
                    {
                        RSNID = LinkSN[i].R_SN_ID;
                        Row_R_SN RowInvailSN = (Row_R_SN)shk.GetObjByID(RSNID, Station.SFCDB);

                        //添加一筆MRB記錄
                        //給new_r_mrb賦值
                        New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
                        New_R_MRB.SN = RowInvailSN.SN;
                        New_R_MRB.WORKORDERNO = RowInvailSN.WORKORDERNO;
                        New_R_MRB.NEXT_STATION = RowInvailSN.NEXT_STATION;
                        New_R_MRB.SKUNO = RowInvailSN.SKUNO;
                        New_R_MRB.FROM_STORAGE = FROM_STORAGE;
                        New_R_MRB.TO_STORAGE = TO_STORAGE;
                        New_R_MRB.REWORK_WO = "";//空
                        New_R_MRB.CREATE_EMP = Station.LoginUser.EMP_NO;
                        New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
                        New_R_MRB.MRB_FLAG = "0";
                        New_R_MRB.SAP_FLAG = "0";
                        New_R_MRB.EDIT_EMP = Station.LoginUser.EMP_NO;
                        New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
                        result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + RowInvailSN.SN, "ADD" }));
                        }
                    }

                    Station.SFCDB.CommitTrain();

                    Station.AddMessage("MES00000063", new string[] { SerialSn.SerialNo }, StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    Station.SFCDB.RollbackTrain();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 解除SN的keyparts綁定關係後，把R_SN最新的VALID_FLAG=0的數據恢復為VALID_FLAG=1,並把和該SN綁定的板子打入MRB
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNUnlinkFinishallAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            R_MRB New_R_MRB = new R_MRB();
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);

            List<R_SN_KEYPART_DETAIL> LinkSN = new List<R_SN_KEYPART_DETAIL>();
            T_R_SN_KEYPART_DETAIL stk = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_SN shk = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_SN> GetRsnList = new List<R_SN>();
            List<R_MRB> GetMRBList = new List<R_MRB>();

            string UpdateSql = "";
            string RSNID = null;

            if (Paras.Count != 2)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN SerialSn = (SN)SNSession.Value;
            try
            {
                //註釋掉R_SN  VALID_FLAG=1的數據，恢復之前VALID_FLAG=0 時間最晚的數據

                Station.SFCDB.BeginTrain();
                Row_R_SN RowSN = (Row_R_SN)shk.GetObjByID(SerialSn.ID, Station.SFCDB);
                RowSN.VALID_FLAG = "0";
                //RowSN.NEXT_STATION = "LINK";
                UpdateSql = RowSN.GetUpdateString(Station.DBType);
                Station.SFCDB.ExecSQL(UpdateSql);

                GetRsnList = shk.GetINVaildSN(SerialSn.SerialNo, SerialSn.ID, Station.SFCDB);

                if (GetRsnList != null && GetRsnList.Count > 0)
                {
                    Row_R_SN RowInvailSN = (Row_R_SN)shk.GetObjByID(GetRsnList[0].ID, Station.SFCDB);
                    RowInvailSN.VALID_FLAG = "1";
                    UpdateSql = RowInvailSN.GetUpdateString(Station.DBType);
                    Station.SFCDB.ExecSQL(UpdateSql);
                }

                Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw ex;
            }

            string FROM_STORAGE = "";
            string TO_STORAGE = "";

            GetMRBList = TR_MRB.GetMrbBySN(SerialSn.SerialNo, Station.SFCDB);
            if (GetMRBList != null && GetMRBList.Count > 0)
            {
                FROM_STORAGE = GetMRBList[0].FROM_STORAGE;
                TO_STORAGE = GetMRBList[0].TO_STORAGE;
            }
            //處理被LINK的數據
            MESStationSession LinksnSesssion = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LinksnSesssion != null)
            {
                LinkSN = (List<R_SN_KEYPART_DETAIL>)LinksnSesssion.Value;
                try
                {
                    Station.SFCDB.BeginTrain();

                    for (int i = 0; i < LinkSN.Count; i++)
                    {
                        RSNID = LinkSN[i].R_SN_ID;
                        Row_R_SN RowInvailSN = (Row_R_SN)shk.GetObjByID(RSNID, Station.SFCDB);

                        //添加一筆MRB記錄
                        //給new_r_mrb賦值
                        New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
                        New_R_MRB.SN = RowInvailSN.SN;
                        New_R_MRB.WORKORDERNO = RowInvailSN.WORKORDERNO;
                        New_R_MRB.NEXT_STATION = RowInvailSN.NEXT_STATION;
                        New_R_MRB.SKUNO = RowInvailSN.SKUNO;
                        New_R_MRB.FROM_STORAGE = FROM_STORAGE;
                        New_R_MRB.TO_STORAGE = TO_STORAGE;
                        New_R_MRB.REWORK_WO = "";//空
                        New_R_MRB.CREATE_EMP = Station.LoginUser.EMP_NO;
                        New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
                        New_R_MRB.MRB_FLAG = "0";
                        New_R_MRB.SAP_FLAG = "0";
                        New_R_MRB.EDIT_EMP = Station.LoginUser.EMP_NO;
                        New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
                        result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + RowInvailSN.SN, "ADD" }));
                        }
                    }

                    Station.SFCDB.CommitTrain();

                    Station.AddMessage("MES00000063", new string[] { SerialSn.SerialNo }, StationMessageState.Pass);
                }
                catch (Exception ex)
                {
                    Station.SFCDB.RollbackTrain();
                    throw ex;
                }
            }
        }

        /// <summary>
        /// add by ZGJ
        /// 替換 SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReplaceSnAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMessage = string.Empty;
            string NewSn = string.Empty;
            string OldSn = string.Empty;
            SN SnObj = null;
            SN OldSnObj = null;
            string sql = string.Empty;
            OleExecPool APDBPool = Station.DBS["APDB"];
            OleExec APDB = null;
            int result = 0;
            T_R_REPAIR_MAIN RepairMain = new T_R_REPAIR_MAIN(Station.SFCDB, Station.DBType);
            T_R_REPAIR_FAILCODE RepairFailCode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            T_R_SN RSn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_STATION_DETAIL RSnStationDetail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN_KEYPART_DETAIL RSnKeypartDetail = new T_R_SN_KEYPART_DETAIL(Station.SFCDB, Station.DBType);
            T_R_REPLACE_SN ReplaceSn = new T_R_REPLACE_SN(Station.SFCDB, Station.DBType);
            R_REPLACE_SN ReplaceSnObj = new R_REPLACE_SN();
            T_R_PANEL_SN RPanelSn = new T_R_PANEL_SN(Station.SFCDB, Station.DBType);
            T_R_MRB RMrb = new T_R_MRB(Station.SFCDB, Station.DBType);
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
            T_R_TEST_RECORD TestRecord = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
            T_R_SHIP_DETAIL ShipDetail = new T_R_SHIP_DETAIL(Station.SFCDB, Station.DBType);
            T_R_REPAIR_ACTION t_r_repair_action = new T_R_REPAIR_ACTION(Station.SFCDB, Station.DBType);
            T_WWN_DATASHARING t_wwn = new T_WWN_DATASHARING(Station.SFCDB, Station.DBType);//Add By ZHB 20200820
            T_R_REPAIR_TRANSFER t_transfer = new T_R_REPAIR_TRANSFER(Station.SFCDB, Station.DBType);

            MESStationSession OldSnSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (OldSnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            var OldObj = OldSnSession.Value;
            if (OldObj is string)
            {
                OldSn = OldSnSession.Value.ToString();
                OldSnObj = new SN(OldSn, Station.SFCDB, Station.DBType);
            }
            else
            {
                OldSnObj = (SN)OldSnSession.Value;
                OldSn = OldSnObj.SerialNo;
            }

            MESStationSession NewSnSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[1].SESSION_TYPE) && t.SessionKey.Equals(Paras[1].SESSION_KEY));
            if (NewSnSession == null)
            {
                NewSn = Input.Value.ToString();
                SnObj = new SN(NewSn, Station.SFCDB, Station.DBType);
                NewSnSession = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = SnObj };
            }
            var NewObj = NewSnSession.Value;
            if (NewObj is string)
            {
                NewSn = NewSnSession.Value.ToString();
            }
            else
            {
                NewSn = ((SN)NewSnSession.Value).SerialNo;
            }

            try
            {
                Station.SFCDB.ThrowSqlExeception = true;
                Station.APDB.ThrowSqlExeception = true;
                //Station.SFCDB.BeginTrain();
                APDB = Station.APDB;

                RSn.ReplaceSn(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RSn.RecordPassStationDetail(NewSn, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
                RSnStationDetail.ReplaceSnStationDetail(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RepairMain.ReplaceSnRepairFailMain(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RepairFailCode.ReplaceSnRepairFailCode(NewSn, OldSn, Station.SFCDB, Station.DBType);
                t_r_repair_action.ReplaceSnRepairAction(NewSn, OldSn, Station.SFCDB);
                t_transfer.ReplaceSn(Station.SFCDB, Station.DBType, OldSn, NewSn);
                RSnKeypartDetail.ReplaceSnKeypartDetail(NewSn, OldSn, Station.SFCDB, Station.DBType);
                //add update RPanelSn ReplaceRMrb by wuq 20180416
                RPanelSn.ReplaceRPanelSn(NewSn, OldSn, Station.SFCDB, Station.DBType);
                RMrb.ReplaceRMrb(NewSn, OldSn, Station.SFCDB, Station.DBType);
                TestRecord.ReplaceSnTestRecord(OldSn, NewSn, Station.SFCDB);
                ShipDetail.ReplaceSnShipDetail(OldSn, NewSn, Station.SFCDB);
                t_wwn.ReplaceSnWWN(NewSn, OldSn, Station.SFCDB);//Add By ZHB 20200820

                ReplaceSnObj.OLD_SN_ID = OldSnObj.ID;
                ReplaceSnObj.OLD_SN = OldSnObj.SerialNo;
                ReplaceSnObj.NEW_SN = NewSn;
                ReplaceSnObj.EDIT_TIME = Station.GetDBDateTime();
                ReplaceSnObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                ReplaceSn.AddReplaceSNRecord(ReplaceSnObj, Station.BU, Station.SFCDB, Station.DBType);
                t_r_sn_kp.ReplaceSnKP(NewSn, OldSn, Station.SFCDB);

                sql = $@"UPDATE MES4.R_SN_LINK R SET R.P_SN='{NewSn}' WHERE R.P_SN='{OldSn}'";
                result = APDB.ExecSqlNoReturn(sql, null);
                if (result == 0)
                {
                    //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000197",new string[] { OldSn, "MES4.R_SN_LINK" });
                    //    throw new MESReturnMessage(ErrMessage);
                }

                sql = $@"UPDATE MES4.R_TR_PRODUCT_DETAIL R SET R.P_SN='{NewSn}' WHERE R.P_SN='{OldSn}'";
                result = APDB.ExecSqlNoReturn(sql, null);
                //if (result == 0)
                //{
                //    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000197",new string[] { OldSn, "MES4.R_TR_PRODUCT_DETAIL" });
                //    throw new MESReturnMessage(ErrMessage);
                //}

                sql = $@"UPDATE MES4.R_TEMP_REPLACE R SET R.P_SN='{NewSn}' WHERE R.P_SN='{OldSn}'";
                try
                {
                    result = APDB.ExecSqlNoReturn(sql, null);
                }
                catch (Exception ee)
                {
                    throw new Exception("UPDATE APDB :" + ee.Message);
                }

                if (Paras.Count == 3)
                {
                    MESStationSession ClearFlagSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "true" };
                    Station.StationSession.Add(ClearFlagSession);
                }
                Station.AddMessage("MES00000198", new string[] { NewSn, OldSn }, StationMessageState.Pass);
            }
            catch (MESReturnMessage ex)
            {
                throw ex;
            }
            finally
            {
                //APDBPool.Return(APDB);
            }
        }

        /// <summary>
        /// for BPD
        /// 海外回貨或者根本不是廠內生產的產品，生成資料到 R_SN 中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ExternalRMAAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
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

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputQty == null)
            {
                sessionInputQty = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(sessionInputQty);
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionExtQty == null)
            {
                sessionExtQty = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY };
                Station.StationSession.Add(sessionExtQty);
            }

            MESStationSession SnObjSessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (SnObjSessionExtQty == null)
            {
                SnObjSessionExtQty = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY };
                Station.StationSession.Add(SnObjSessionExtQty);
            }

            try
            {
                OleExec sfcdb = Station.SFCDB;
                DB_TYPE_ENUM sfcdbType = Station.DBType;
                WorkOrder objWorkorder = new WorkOrder();
                //Dictionary<string, object> dicNextStation;
                //string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                objWorkorder = (WorkOrder)sessionWO.Value;
                //獲取後段產品的工單號，即上一槍輸入的工單
                //string WorkorderNo = objWorkorder.WorkorderNo;
                //WorkOrder tempWorkOrder = new WorkOrder();
                //dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                //nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();
                var LastStation = t_c_route_detail.GetByRouteIdOrderBySeqDesc(objWorkorder.RouteID, Station.SFCDB).FirstOrDefault();
                T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                R_SN r_sn = new R_SN();
                r_sn.ID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                r_sn.SN = sessionSn.Value.ToString();
                r_sn.SKUNO = objWorkorder.SkuNO;
                r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                r_sn.PLANT = objWorkorder.PLANT;
                r_sn.ROUTE_ID = objWorkorder.RouteID;
                r_sn.STARTED_FLAG = "1";
                r_sn.START_TIME = Station.GetDBDateTime();
                r_sn.PACKED_FLAG = "0";
                r_sn.COMPLETED_FLAG = "0";
                r_sn.SHIPPED_FLAG = "0";
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.CURRENT_STATION = "RMA_EX";
                r_sn.NEXT_STATION = LastStation == null ? "SHIPOUT" : LastStation.STATION_NAME;
                r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                r_sn.CUST_PN = objWorkorder.CUST_PN;
                r_sn.VALID_FLAG = "1";
                r_sn.STOCK_STATUS = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = Station.GetDBDateTime();

                //加載出來 SN 類型的對象，供後面使用
                result = t_r_sn.AddNewSN(r_sn, sfcdb);
                SN SnObj = new SN();
                SnObj.baseSN = r_sn;
                SnObjSessionExtQty.Value = SnObj;

                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + sessionSn.Value.ToString(), "ADD" }));
                }
                t_r_sn.RecordPassStationDetail(sessionSn.Value.ToString(), Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HWD RMA過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RMAPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN Table_RSn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_C_ROUTE Table_CRoute = new T_C_ROUTE(Station.SFCDB, Station.DBType);
            string StrSN = "";
            string nextStation = "";
            string Oldworkorderno = string.Empty;
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (SKUSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SKU SkuObj = (SKU)SKUSession.Value;
            //OldSn = OldSnObj.SerialNo;

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[1].SESSION_TYPE) && t.SessionKey.Equals(Paras[1].SESSION_KEY));
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SN SnObj = (SN)SNSession.Value;
            //StrSN = SNSession.Value.ToString();

            try
            {
                //R_SN r_sn = null;
                //SN snObj = new SN();
                //r_sn = Table_RSn.LoadData(SnObj.SerialNo, Station.SFCDB);
                //if (r_sn != null)
                //{
                //    Table_RSn.updateValid_Flag(r_sn.ID, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                //    nextStation = r_sn.NEXT_STATION;
                //    Oldworkorderno = r_sn.WORKORDERNO;
                //}
                //else
                //{
                //    nextStation = "SMT_FQC";
                //}

                Table_RSn.updateValid_Flag(SnObj.ID, "0", Station.LoginUser.EMP_NO, Station.SFCDB);

                StrSN = SnObj.SerialNo;
                Oldworkorderno = SnObj.WorkorderNo;
                nextStation = SnObj.NextStation;
                // add by fgg 2018.05.03 R_MRB 表加一筆記錄，以便可以掃REWORK
                T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_MRB rowMrb = (Row_R_MRB)t_r_mrb.NewRow();
                rowMrb.ID = t_r_mrb.GetNewID(Station.BU, Station.SFCDB);
                rowMrb.SN = StrSN;
                rowMrb.WORKORDERNO = Oldworkorderno;
                rowMrb.NEXT_STATION = nextStation;
                rowMrb.SKUNO = SkuObj.SkuNo;
                rowMrb.FROM_STORAGE = Oldworkorderno;   //  rowMrb.FROM_STORAGE = "";//在r_mrb中記錄舊工單，便於查詢該工單的幾種，機種版本和版次 //SHBPD REWORK 卡機種版本，版次
                rowMrb.TO_STORAGE = "";
                rowMrb.REWORK_WO = "";//空
                rowMrb.CREATE_EMP = Station.LoginUser.EMP_NO;
                rowMrb.CREATE_TIME = Station.GetDBDateTime();
                rowMrb.MRB_FLAG = "1";
                rowMrb.SAP_FLAG = "0";
                rowMrb.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowMrb.EDIT_TIME = Station.GetDBDateTime();
                Station.SFCDB.ExecSQL(rowMrb.GetInsertString(DB_TYPE_ENUM.Oracle));

                Row_C_ROUTE Row_C_Route = (Row_C_ROUTE)Table_CRoute.GetRouteBySkuno(SkuObj.SkuId, Station.SFCDB, Station.DBType);
                Table_RSn.InsertRMASN(StrSN, "RMA", SkuObj.SkuNo, Row_C_Route.ID, "", "RMA", "REWORK", Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB, Station.LoginUser.BU, "FRESH");
                Table_RSn.RecordPassStationDetail(StrSN, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
                Station.AddMessage("MES00000063", new string[] { StrSN }, StationMessageState.Pass); //回饋消息到前台
            }
            catch (Exception ex)
            {
                Station.AddMessage("MES00000233" + ";" + ex.Message, new string[] { StrSN }, StationMessageState.Fail);
            }
        }

        /// <summary>
        /// add by lhj 2019.7.19
        /// 線外工站 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>

        public static void SNOutLinePassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            R_SN R_Sn = null;
            SN SubSNObj = new SN();
            SN SnObj = new SN();
            T_R_SN Table_R_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            try
            {
                SubSNObj = (SN)sessionSn.Value;
                R_Sn = Table_R_SN.GetById(SubSNObj.ID, Station.SFCDB);
                Table_R_SN.LinkPassStationDetail(R_Sn, SubSNObj.WorkorderNo, SubSNObj.SkuNo, SubSNObj.RouteID, Station.Line, Station.StationName, Station.StationName, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.5.14
        /// SILOADING 過站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNSILoadingPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionExtQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                OleExec sfcdb = Station.SFCDB;
                DB_TYPE_ENUM sfcdbType = Station.DBType;
                Dictionary<string, object> dicNextStation;
                string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                objWorkorder = (WorkOrder)sessionWO.Value;
                //獲取後段產品的工單號，即上一槍輸入的工單
                string WorkorderNo = objWorkorder.WorkorderNo;
                //鎖住該工單記錄,然後再重新加載工單

                Station.SFCDB.ORM.Updateable<R_WO_BASE>().UpdateColumns(r => new R_WO_BASE { SKUNO = r.SKUNO })
                    .Where(r => r.WORKORDERNO == WorkorderNo).ExecuteCommand();
                objWorkorder.Init(WorkorderNo, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                sessionWO.Value = objWorkorder;

                WorkOrder tempWorkOrder = new WorkOrder();
                dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();
                T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                List<R_SN> list = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.WORKORDERNO == objWorkorder.WorkorderNo && (r.VALID_FLAG != "0" || (r.VALID_FLAG == "0" && r.NEXT_STATION == "REWORK"))).ToList();

                if (list.Count() >= objWorkorder.WORKORDER_QTY)
                {
                    throw new MESReturnMessage($@" {objWorkorder.WorkorderNo} Already Full!");
                }
                //if (t_r_sn.CheckSNExists(sessionSn.Value.ToString(), Station.SFCDB))
                //{
                //    throw new MESReturnMessage($@" {sessionSn.Value.ToString()} Already Exist!");
                //}
                string newSNID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                var oldSNObj = t_r_sn.LoadSN(sessionSn.Value.ToString(), Station.SFCDB);
                if (oldSNObj != null)
                {
                    var ObjSku = sfcdb.ORM.Queryable<C_SKU>().Where(r => r.SKUNO == objWorkorder.SkuNO).ToList().FirstOrDefault();
                    var Oldworkorder = sfcdb.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == oldSNObj.WORKORDERNO).ToList().FirstOrDefault();
                    T_R_LINK_CONTROL t_r_link_control = new T_R_LINK_CONTROL(Station.SFCDB, Station.DBType);
                    var linkObj = t_r_link_control.GetControlList("SKU", objWorkorder.SkuNO, null, oldSNObj.SKUNO, null, "LOADING_KEEP_SN", Station.SFCDB);
                    if (ObjSku.SKU_TYPE == "PCBA" || ObjSku.SKU_TYPE == "MODEL")
                    {
                        //屏蔽此段，將手動配置版本改為由BOM裏面取版本
                        //linkObj = t_r_link_control.GetControlList("SKU", objWorkorder.SkuNO, objWorkorder.SKU_VER, oldSNObj.SKUNO, Oldworkorder.SKU_VER, "LOADING_KEEP_SN", Station.SFCDB);
                        var BomList = sfcdb.ORM.Queryable<R_WO_ITEM>().Where(r => r.AUFNR == objWorkorder.WorkorderNo && r.MATNR == oldSNObj.SKUNO).ToList();
                        if (BomList != null)
                        {
                            if (BomList.FindAll(r => r.REVLV == Oldworkorder.SKU_VER).Count() == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143120"));
                            }
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141822"));
                        }

                    }
                    if (linkObj.Count > 0)
                    {
                        DateTime sysdate = Station.SFCDB.ORM.GetDate();
                        oldSNObj.SHIPPED_FLAG = "1";
                        oldSNObj.SHIPDATE = sysdate;
                        oldSNObj.VALID_FLAG = "2";
                        oldSNObj.EDIT_TIME = sysdate;
                        oldSNObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                        Station.SFCDB.ORM.Updateable<R_SN>(oldSNObj).ExecuteCommand();

                        T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                        R_SN_KP kpObj = new R_SN_KP();
                        kpObj.ID = t_r_sn_kp.GetNewID(Station.BU, Station.SFCDB);
                        kpObj.R_SN_ID = newSNID;
                        kpObj.SN = sessionSn.Value.ToString();
                        kpObj.VALUE = sessionSn.Value.ToString();
                        kpObj.PARTNO = oldSNObj.SKUNO;
                        kpObj.KP_NAME = "KEEP_SN";
                        kpObj.MPN = oldSNObj.SKUNO;
                        kpObj.SCANTYPE = "KEEP_SN";
                        kpObj.ITEMSEQ = 1;
                        kpObj.SCANSEQ = 1;
                        kpObj.DETAILSEQ = 1;
                        kpObj.STATION = Station.StationName;
                        kpObj.VALID_FLAG = 1;
                        kpObj.EXKEY1 = "OLD_SN_ID";
                        kpObj.EXVALUE1 = oldSNObj.ID;
                        kpObj.EDIT_EMP = Station.LoginUser.EMP_NO;
                        kpObj.EDIT_TIME = Station.SFCDB.ORM.GetDate();
                        result = Station.SFCDB.ORM.Insertable<R_SN_KP>(kpObj).ExecuteCommand();
                        if (result == 0)
                        {
                            throw new MESReturnMessage($"KEEP_SN save keypart fail.");
                        }
                        // Tag 2021.12.11
                        Station.SFCDB.ORM.Updateable<R_SN_KP>().SetColumns(t => new R_SN_KP() { EXKEY1 = "0", EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO })
                                        .Where(t => t.SN == sessionSn.Value.ToString() && t.STATION == "MATL_LINK").ExecuteCommand();
                    }
                    else
                    {
                        //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("Please check whether the sku and version of the linkcontrol configuration are correct"));
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814141821"));
                    }

                }

                #region 寫入r_sn,r_sn_station_detail             
                R_SN r_sn = new R_SN();
                T_C_SKU_Label TCSL = new T_C_SKU_Label(sfcdb, sfcdbType);
                r_sn.ID = newSNID;
                r_sn.SN = sessionSn.Value.ToString();
                if (Station.BU.Equals("BPD"))
                {
                    r_sn = t_r_sn.GetSN(sessionSn.Value.ToString(), sfcdb);
                    //因爲 BPD 要求 NWG 開頭的要轉換成 FOX開頭，并且同時打印出 NWG 的條碼和 FOX 的條碼
                    //NWG 的条码之后会打印，这里只需要额外打印一个 FOX 开头的条码即可
                    //针对有的 NWG 开头的不需要打印 FOX，因此这里判断如果没有配置打印模板就不打印
                    if (sessionSn.Value.ToString().StartsWith("NWG") && TCSL.GetLabelConfigBySkuStation(objWorkorder.SkuNO, Station.StationName, sfcdb).Count > 0)
                    {
                        MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == "SKU");
                        if (SkuSession != null)
                        {
                            sessionSn.Value = sessionSn.Value.ToString().Replace("NWG", "FOX");
                            Station.LabelStillPrint.AddRange(LabelPrintAction.DoPrint((SKU)SkuSession.Value, Station));
                            sessionSn.Value = sessionSn.Value.ToString().Replace("FOX", "NWG");
                            ////加載出該SN原先的工單號
                            //tempWorkOrder.Init(r_sn.WORKORDERNO, Station.SFCDB);
                            ////設置到Session中
                            //sessionWO.Value = tempWorkOrder;
                            ////打印原先的 NWG 開頭的條碼
                            //Station.LabelStillPrint.AddRange(LabelPrintAction.DoPrint((SKU)SkuSession.Value, Station));
                            //// NWG 開頭的SN 需要用 FOX 替換
                            //r_sn.SN = r_sn.SN.Replace("NWG", "FOX");
                            ////再次加載上一槍掃描的工單到 sessionWO 中以備後面使用
                            //tempWorkOrder = new WorkOrder();
                            //tempWorkOrder.Init(WorkorderNo, Station.SFCDB);
                            //sessionWO.Value = tempWorkOrder;
                            //objWorkorder = (WorkOrder)sessionWO.Value;
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { "SKU" }));
                        }
                    }
                }
                r_sn.SKUNO = objWorkorder.SkuNO;
                r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                r_sn.PLANT = objWorkorder.PLANT;
                r_sn.ROUTE_ID = objWorkorder.RouteID;
                r_sn.STARTED_FLAG = "1";
                r_sn.START_TIME = Station.GetDBDateTime();
                r_sn.PACKED_FLAG = "0";
                r_sn.COMPLETED_FLAG = "0";
                r_sn.SHIPPED_FLAG = "0";
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.CURRENT_STATION = Station.StationName;
                r_sn.NEXT_STATION = nextStation;
                r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                r_sn.CUST_PN = objWorkorder.CUST_PN;
                r_sn.VALID_FLAG = "1";
                r_sn.STOCK_STATUS = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = Station.GetDBDateTime();
                if (Station.BU.Equals("BPD"))
                {
                    result = sfcdb.ORM.Updateable<R_SN>(r_sn).Where(t => t.ID == r_sn.ID).ExecuteCommand();
                    ////當更新完 R_SN 表中的 NWG 為 FOX 之後，重新加載SN對象，并且更新到 SessionSn 中以備後續使用
                    //if (sessionSn.Value.ToString().StartsWith("NWG"))
                    //{
                    //    SN tempSn = new SN();
                    //    tempSn.Load(r_sn.SN, Station.SFCDB, Station.DBType);
                    //    sessionSn.Value = tempSn;
                    //}
                }
                else
                {
                    result = t_r_sn.AddNewSN(r_sn, sfcdb);
                }
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + sessionSn.Value.ToString(), "ADD" }));
                }
                t_r_sn.RecordPassStationDetail(sessionSn.Value.ToString(), Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);

                #endregion 寫入r_sn,r_sn_station_detail

                #region 寫入 r_sn_kp

                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                if (objWorkorder.KP_LIST_ID != null && objWorkorder.KP_LIST_ID.ToString() != "")
                {
                    if (!c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { objWorkorder.WorkorderNo + " KP_LIST_ID" }));
                    }
                    SN snObject = new SN();
                    snObject.InsertR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType);
                }
                else
                {
                    if (c_kp_list.GetListIDBySkuno(objWorkorder.SkuNO, sfcdb).Count != 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181101091946", new string[] { objWorkorder.SkuNO, objWorkorder.WorkorderNo }));
                    }
                }

                #endregion 寫入 r_sn_kp

                //更新工單投入數量

                result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                }

                Row_R_WO_BASE newRowWo = t_r_wo_base.LoadWorkorder(objWorkorder.WorkorderNo, sfcdb);
                sessionInputQty.Value = newRowWo.INPUT_QTY;
                sessionExtQty.Value = newRowWo.WORKORDER_QTY - newRowWo.INPUT_QTY;

                if (newRowWo.INPUT_QTY >= newRowWo.WORKORDER_QTY)
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                    Station.AddMessage("MES00000247", new string[] { newRowWo.WORKORDERNO }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更改未綁定的SN的Keypart信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UpdateRSNKPAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
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
            MESStationSession sessionInputString = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputString == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }
            if (sessionInputString.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
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
                foreach (R_SN r_sn in snList)
                {
                    bool a = t_r_sn_kp.CheckLinkBySNID(r_sn.ID, Station.SFCDB);
                    if (t_r_sn_kp.CheckLinkBySNID(r_sn.ID, Station.SFCDB))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094344", new string[] { r_sn.SN }));
                    }
                }
                SN snObject = new SN();
                snObject.UpdateSNKP(objWorkorder, snList, Station);
                Station.AddMessage("MES00000063", new string[] { sessionInputString.Value.ToString() }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// SN 出货
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordSnObaSampleInfo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string LotNo = Station.DisplayOutput.Find(t => t.Name == "LOTNO").Value.ToString();
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL tRLotDetail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
            Row_R_LOT_DETAIL rowRLotDetail = (Row_R_LOT_DETAIL)tRLotDetail.NewRow();
            R_SN rSn = tRSn.GetDetailBySN(snSession.Value.ToString(), Station.SFCDB);
            Row_R_LOT_STATUS rowRLotStatus = tRLotStatus.GetByLotNo(LotNo, Station.SFCDB);
            //Lot{0}不處於待抽檢狀態,請檢查!
            if (!rowRLotStatus.CLOSED_FLAG.Equals("1"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180530114417", new string[] { LotNo }));
            }

            rowRLotDetail.ID = tRLotDetail.GetNewID(Station.BU, Station.SFCDB);
            rowRLotDetail.LOT_ID = rowRLotStatus.ID;
            rowRLotDetail.SN = snSession.Value.ToString();
            rowRLotDetail.WORKORDERNO = rSn.WORKORDERNO;
            rowRLotDetail.CREATE_DATE = tRLotDetail.GetDBDateTime(Station.SFCDB);
            rowRLotDetail.STATUS = Paras[1].VALUE.Equals("PASS") ? "1" : "2";
            rowRLotDetail.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowRLotDetail.EDIT_TIME = rowRLotDetail.CREATE_DATE;

            //rowRLotDetail.FAIL_CODE = 

            if (Paras[1].VALUE.Equals("PASS"))
            {
                rowRLotStatus.PASS_QTY++;
            }
            else
            {
                rowRLotStatus.FAIL_QTY++;
            }

            rowRLotStatus.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowRLotStatus.EDIT_TIME = rowRLotDetail.CREATE_DATE;
            if (rowRLotStatus.REJECT_QTY <= rowRLotStatus.FAIL_QTY)
            {
                rowRLotStatus.CLOSED_FLAG = "2";
                rowRLotStatus.LOT_STATUS_FLAG = "2";
                //鎖定LOT所有SN
                T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
                tRSnLock.LockSnInOba(LotNo, Station.SFCDB);
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180615143705", new string[] { LotNo }), State = StationMessageState.Message });
            }
            else if (rowRLotStatus.SAMPLE_QTY <= rowRLotStatus.PASS_QTY + rowRLotStatus.FAIL_QTY)
            {
                rowRLotStatus.CLOSED_FLAG = "2";
                rowRLotStatus.LOT_STATUS_FLAG = "1";
                //批量過站;
                List<R_SN> rSnList = new List<R_SN>();
                rSnList = tRSn.GetObaSnListByLotNo(LotNo, Station.SFCDB);
                tRSn.LotsPassStation(rSnList, Station.Line, rSn.NEXT_STATION, rSn.NEXT_STATION, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站
                //記錄通過數 ,UPH
                foreach (var snobj in rSnList)
                {
                    tRSn.RecordYieldRate(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                    tRSn.RecordUPH(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                }
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180615144008", new string[] { LotNo }), State = StationMessageState.Message });
            }
            Station.SFCDB.ExecSQL(rowRLotDetail.GetInsertString(Station.DBType));
            Station.SFCDB.ExecSQL(rowRLotStatus.GetUpdateString(Station.DBType));

            #region 加載界面信息

            if (rowRLotStatus.CLOSED_FLAG == "2")//抽檢完清空界面信息
            {
                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[11].SESSION_TYPE);
                s.DataForUse.Clear();
                Station.StationSession.Clear();
                MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == "SN");
                MESStationInput packInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
                MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == "FailSn");
                MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == "ScanType");
                MESStationInput failCodeInput = Station.Inputs.Find(t => t.DisplayName == "FailCode");
                MESStationInput locationInput = Station.Inputs.Find(t => t.DisplayName == "Location");
                MESStationInput failDescInput = Station.Inputs.Find(t => t.DisplayName == "FailDesc");
                packInput.Visable = true;
                snInput.Visable = false;
                scanTypeInput.Visable = false;
                failCodeInput.Visable = false;
                locationInput.Visable = false;
                failDescInput.Visable = false;
                failSnInput.Visable = false;
            }
            else//未抽檢完=>更新界面信息,設置NextInput
            {
                Station.NextInput = Station.Inputs.Find(t => t.DisplayName.Equals(Paras[0].SESSION_TYPE));
                MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                MESStationSession rejectQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                MESStationSession sampleQtyWithAqlSession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                MESStationSession passQtySession = new MESStationSession() { MESDataType = Paras[9].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[9].SESSION_KEY, ResetInput = Input };
                MESStationSession failQtySession = new MESStationSession() { MESDataType = Paras[10].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[10].SESSION_KEY, ResetInput = Input };

                Station.StationSession.Clear();
                Station.StationSession.Add(lotNoSession);
                Station.StationSession.Add(skuNoSession);
                Station.StationSession.Add(aqlSession);
                Station.StationSession.Add(lotQtySession);
                Station.StationSession.Add(sampleQtySession);
                Station.StationSession.Add(rejectQtySession);
                Station.StationSession.Add(sampleQtyWithAqlSession);
                Station.StationSession.Add(passQtySession);
                Station.StationSession.Add(failQtySession);

                lotNoSession.Value = rowRLotStatus.LOT_NO;
                skuNoSession.Value = rowRLotStatus.SKUNO;
                aqlSession.Value = rowRLotStatus.AQL_TYPE;
                lotQtySession.Value = rowRLotStatus.LOT_QTY;
                sampleQtySession.Value = rowRLotStatus.SAMPLE_QTY;
                rejectQtySession.Value = rowRLotStatus.REJECT_QTY;
                sampleQtyWithAqlSession.Value = rowRLotStatus.PASS_QTY + rowRLotStatus.FAIL_QTY;
                passQtySession.Value = rowRLotStatus.PASS_QTY;
                failQtySession.Value = rowRLotStatus.FAIL_QTY;
            }

            #endregion 加載界面信息
        }

        /// <summary>
        /// JOBSTOCK STATION PASS ACTION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckINTeststation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            try
            {
                var log = Station.SFCDB.ORM.Queryable<R_MES_LOG>().Where(r => r.DATA1 == sessionStation.Value.ToString() && r.DATA2 == sessionSn.Value.ToString() && r.DATA3 != "").ToList();
                if (log.Count() > 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("This SN has not checkout ,Can not Check IN"));
                }
                else
                {
                    T_R_MES_LOG mesLog = new T_R_MES_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    string id = mesLog.GetNewID("FJZ", Station.SFCDB);
                    Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                    Station.SFCDB.BeginTrain();
                    id = mesLog.GetNewID("FJZ", Station.SFCDB);
                    rowMESLog.ID = id;
                    rowMESLog.PROGRAM_NAME = "CloudMES";
                    rowMESLog.CLASS_NAME = "SNAction";
                    rowMESLog.FUNCTION_NAME = "CheckINTesstation";
                    rowMESLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                    rowMESLog.EDIT_TIME = System.DateTime.Now;
                    rowMESLog.DATA1 = sessionStation.Value.ToString();
                    rowMESLog.DATA2 = sessionSn.Value.ToString();
                    Station.SFCDB.ThrowSqlExeception = true;
                    Station.SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                    Station.SFCDB.CommitTrain();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// JOBSTOCK STATION PASS ACTION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckOutTeststation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            try
            {
                var log1 = Station.SFCDB.ORM.Queryable<R_MES_LOG>().Where(r => r.DATA1 == sessionStation.Value.ToString() && r.DATA2 == sessionSn.Value.ToString() && r.DATA3 == null).ToList();
                if (log1.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("This SN has not checkIn ,Can not Check Out"));
                }
                else
                {
                    T_R_MES_LOG mesLog = new T_R_MES_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    string id = mesLog.GetNewID("FJZ", Station.SFCDB);
                    Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                    Station.SFCDB.ORM.Updateable<R_MES_LOG>().SetColumns(t => new R_MES_LOG { DATA3 = DateTime.Now.ToShortDateString().ToString(), DATA4 = Station.LoginUser.EMP_NO })
                        .Where(t => t.DATA2 == sessionSn.Value.ToString() && t.DATA1 == sessionStation.Value.ToString()).ExecuteCommand();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// JOBSTOCK STATION PASS ACTION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void JobStockInAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                objWorkorder = (WorkOrder)sessionWO.Value;
                SN objSN = new SN();
                objSN = (SN)sessionSn.Value;
                Station.SFCDB.ThrowSqlExeception = true;
                T_R_STOCK t_r_stock = new T_R_STOCK(Station.SFCDB, Station.DBType);
                if (t_r_stock.IsStockIn(objSN.SerialNo, objWorkorder.WorkorderNo, Station.SFCDB))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180619154230", new string[] { objSN.SerialNo }));
                }
                T_R_FUNCTION_CONTROL r_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
                List<R_F_CONTROL> listControl = r_control.GetListByFcv("JobstockNotBackflush", "WO", Station.SFCDB);
                R_F_CONTROL control = listControl.Find(r => objWorkorder.WorkorderNo.StartsWith(r.VALUE));

                if (string.IsNullOrEmpty(objWorkorder.STOCK_LOCATION) && control == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180803114047", new string[] { objWorkorder.WorkorderNo }));
                }

                if (!(objSN.StockStatus == "1" && objSN.StockTime != null))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180804105812", new string[] { objSN.SerialNo }));
                }
                objSN.JobStockPass(objWorkorder, objSN, Station, "0");
                Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ReturnSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            Double Station_SEQ = 0.0;

            Double SN_SEQ = 0.0;
            try
            {
                SN objSN = (SN)sessionSn.Value;
                if (objSN.ShippedFlag == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { Paras[0].SESSION_TYPE }));
                }
                if (objSN.RepairFailedFlag == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104007", new string[] { Paras[0].SESSION_TYPE }));
                }
                if (objSN.CompletedFlag == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104922", new string[] { Paras[0].SESSION_TYPE }));
                }
                if (objSN.CurrentStation == "REWORK")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181128201930", new string[] { Paras[0].SESSION_TYPE }));
                }
                string station_name = sessionStation.Value.ToString();
                string DeviceName = string.Empty;
                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(objSN.RouteID, Station.SFCDB);

                if (RouteDetails.Count > 0)
                {
                    foreach (C_ROUTE_DETAIL c in RouteDetails)
                    {
                        if (station_name == c.STATION_NAME)
                        {
                            Station_SEQ = (Double)c.SEQ_NO;
                        }
                        if (objSN.CurrentStation == c.STATION_NAME)
                        {
                            SN_SEQ = (Double)c.SEQ_NO;
                        }
                    }
                    if (Station_SEQ > SN_SEQ)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808112641", new string[] { Paras[0].SESSION_TYPE }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808113358", new string[] { Paras[1].SESSION_TYPE }));
                }

                var stationList = RouteDetails.FindAll(t => t.SEQ_NO >= Station_SEQ && t.SEQ_NO <= SN_SEQ).Select(t => t.STATION_NAME).ToArray();

                if (stationList.Contains("BIP"))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181025091108", new string[] { objSN.SerialNo }));
                }

                var seqList = RouteDetails.FindAll(t => t.SEQ_NO < Station_SEQ).Select(t => t.SEQ_NO).ToArray();
                if (seqList.Length == 0)
                {
                    DeviceName = RouteDetails.OrderBy(r => r.SEQ_NO).FirstOrDefault().STATION_NAME;
                }
                else
                {
                    Array.Sort(seqList);
                    SN_SEQ = (double)seqList[seqList.Length - 1];
                    var tt = RouteDetails.Find(t => t.SEQ_NO == SN_SEQ);
                    DeviceName = tt.STATION_NAME;
                }
                //T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                //List<R_SN_KP> snkp = TRKP.GetAllKPBySnIDStation(objSN.ID, Station.SFCDB, stationList);
                //List<R_SN> sysKpList = TRKP.GetSysKeypartValueList(objSN.ID, stationList, Station.SFCDB);
                //打散綁定關係轉移到  BreakUpSNLinkAction
                //if (snkp.Count > 0)
                //{
                //    result = TRKP.ReturnUpdateKPSNBySnId(objSN.ID, stationList, Station.LoginUser.EMP_NO, Station.SFCDB);
                //}
                //if (result < 0)
                //{
                //    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + objSN.SerialNo, "ADD" }));
                //}
                T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                table.ReturnPassStation(objSN.SerialNo, Station.Line, DeviceName, station_name, Station.BU, objSN.ProductStatus, Station.LoginUser.EMP_NO, Station.SFCDB);
                //table.UpdateShippingFlag(sysKpList, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                Station.SFCDB.ThrowSqlExeception = true;
                Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 通過Snlist退站更新記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnSNBySnlist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (Pallet == null)
            {

                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            WorkOrder objWO = (WorkOrder)sessionWo.Value;

            Double Station_SEQ = 0.0;

            Double SN_SEQ = 0.0;
            try
            {

                List<R_SN> snList = new List<R_SN>();
                //snList = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((sn, wo) => sn.WORKORDERNO == wo.WORKORDERNO)
                //            .Where((sn, wo) => wo.WORKORDERNO == objWO.WorkorderNo   && (sn.SCRAPED_FLAG != "1" || SqlSugar.SqlFunc.IsNullOrEmpty(sn.SCRAPED_FLAG)) && sn.VALID_FLAG == "1")
                //            .Select((sn, wo) => sn).ToList();  //按照工單取sn

                snList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.PACK_NO == Pallet.Value
                  && a.ID == b.PARENT_PACK_ID && c.PACK_ID == b.ID && d.ID == c.SN_ID && d.VALID_FLAG == "1").Select((a, b, c, d) => d).ToList(); //按照棧板取sn


                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(snList.FirstOrDefault().ROUTE_ID, Station.SFCDB);

                //SN objSN = (SN)sessionSn.Value;
                foreach (R_SN r_sn in snList)
                {
                    //if (r_sn.SHIPPED_FLAG == "1")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    //if (r_sn.REPAIR_FAILED_FLAG == "1")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104007", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    //if (r_sn.COMPLETED_FLAG == "1")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104922", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    //if (r_sn.CURRENT_STATION == "REWORK")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181128201930", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    string station_name = sessionStation.Value.ToString();
                    string DeviceName = string.Empty;


                    if (RouteDetails.Count > 0)
                    {
                        foreach (C_ROUTE_DETAIL c in RouteDetails)
                        {
                            if (station_name == c.STATION_NAME)
                            {
                                Station_SEQ = (Double)c.SEQ_NO;
                            }
                            if (r_sn.CURRENT_STATION == c.STATION_NAME)
                            {
                                SN_SEQ = (Double)c.SEQ_NO;
                            }
                        }
                        if (Station_SEQ > SN_SEQ)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808112641", new string[] { Paras[0].SESSION_TYPE }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808113358", new string[] { Paras[1].SESSION_TYPE }));
                    }

                    var stationList = RouteDetails.FindAll(t => t.SEQ_NO >= Station_SEQ && t.SEQ_NO <= SN_SEQ).Select(t => t.STATION_NAME).ToArray();

                    if (stationList.Contains("BIP"))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181025091108", new string[] { r_sn.SN }));
                    }

                    var seqList = RouteDetails.FindAll(t => t.SEQ_NO < Station_SEQ).Select(t => t.SEQ_NO).ToArray();
                    if (seqList.Length == 0)
                    {
                        DeviceName = RouteDetails.OrderBy(r => r.SEQ_NO).FirstOrDefault().STATION_NAME;
                    }
                    else
                    {
                        Array.Sort(seqList);
                        SN_SEQ = (double)seqList[seqList.Length - 1];
                        var tt = RouteDetails.Find(t => t.SEQ_NO == SN_SEQ);
                        DeviceName = tt.STATION_NAME;
                    }
                    T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                    table.ReturnPassStation(r_sn.SN, Station.Line, DeviceName, station_name, Station.BU, r_sn.PRODUCT_STATUS, Station.LoginUser.EMP_NO, Station.SFCDB);
                    //table.UpdateShippingFlag(sysKpList, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                    Station.SFCDB.ThrowSqlExeception = true;
                    //Station.AddMessage("MES00000063", new string[] { r_sn.SN.ToString() }, StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Return Station by List SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnListSNBySnlist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            WorkOrder objWO = (WorkOrder)sessionWo.Value;

            Double Station_SEQ = 0.0;

            Double SN_SEQ = 0.0;
            try
            {

                List<R_SN> snList = new List<R_SN>();
                string lstSN = sessionSN.Value.ToString();
                lstSN = $@"'{lstSN.Replace("\n", "',\n'")}'";
                if (lstSN.Length == 0)
                {
                    throw new MESReturnMessage("Please Input list SN!");
                }
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                snList = t_r_sn.GetSnListByListSN(lstSN, Station.SFCDB);


                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.GetByRouteIdOrderBySEQASC(snList.FirstOrDefault().ROUTE_ID, Station.SFCDB);

                //SN objSN = (SN)sessionSn.Value;
                foreach (R_SN r_sn in snList)
                {
                    //if (r_sn.SHIPPED_FLAG == "1")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    //if (r_sn.REPAIR_FAILED_FLAG == "1")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104007", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    //if (r_sn.COMPLETED_FLAG == "1")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104922", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    //if (r_sn.CURRENT_STATION == "REWORK")
                    //{
                    //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181128201930", new string[] { Paras[0].SESSION_TYPE }));
                    //}
                    string station_name = sessionStation.Value.ToString();
                    string DeviceName = string.Empty;


                    if (RouteDetails.Count > 0)
                    {
                        foreach (C_ROUTE_DETAIL c in RouteDetails)
                        {
                            if (station_name == c.STATION_NAME)
                            {
                                Station_SEQ = (Double)c.SEQ_NO;
                            }
                            if (r_sn.CURRENT_STATION == c.STATION_NAME)
                            {
                                SN_SEQ = (Double)c.SEQ_NO;
                            }
                        }
                        if (Station_SEQ > SN_SEQ)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808112641", new string[] { Paras[0].SESSION_TYPE }));
                        }
                    }
                    else
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808113358", new string[] { Paras[1].SESSION_TYPE }));
                    }

                    var stationList = RouteDetails.FindAll(t => t.SEQ_NO >= Station_SEQ && t.SEQ_NO <= SN_SEQ).Select(t => t.STATION_NAME).ToArray();

                    if (stationList.Contains("BIP"))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181025091108", new string[] { r_sn.SN }));
                    }

                    var seqList = RouteDetails.FindAll(t => t.SEQ_NO < Station_SEQ).Select(t => t.SEQ_NO).ToArray();
                    if (seqList.Length == 0)
                    {
                        DeviceName = RouteDetails.OrderBy(r => r.SEQ_NO).FirstOrDefault().STATION_NAME;
                    }
                    else
                    {
                        Array.Sort(seqList);
                        SN_SEQ = (double)seqList[seqList.Length - 1];
                        var tt = RouteDetails.Find(t => t.SEQ_NO == SN_SEQ);
                        DeviceName = tt.STATION_NAME;
                    }
                    T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                    table.ReturnPassStation(r_sn.SN, Station.Line, DeviceName, station_name, Station.BU, r_sn.PRODUCT_STATUS, Station.LoginUser.EMP_NO, Station.SFCDB);
                    //table.UpdateShippingFlag(sysKpList, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                    Station.SFCDB.ThrowSqlExeception = true;
                    //Station.AddMessage("MES00000063", new string[] { r_sn.SN.ToString() }, StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteFile(string host, int port, string username, string password, string sftpPath, string SNBOMSN)
        {
            try
            {
                using (SftpClient sftpClient = new SftpClient(host, port, username, password))
                {
                    sftpPath = sftpPath + "/" + SNBOMSN;
                    sftpClient.Connect();
                    sftpClient.DeleteFile(sftpPath);
                    sftpClient.Disconnect();
                }
            }
            catch (Exception er)
            {
                Console.WriteLine("An exception has been caught " + er.ToString());
            }
        }

        public static void ORAReturnSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            // 20190222 Patty added for FTX Oracle to reverse station
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession ClearFlagSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "true" };
            Station.StationSession.Add(ClearFlagSession);

            Double Station_SEQ = 0.0;

            Double SN_SEQ = 0.0;
            int result = 0;
            try
            {
                SN objSN = (SN)sessionSn.Value;
                if (objSN.ShippedFlag == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { Paras[0].SESSION_TYPE }));
                }
                if (objSN.RepairFailedFlag == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104007", new string[] { Paras[0].SESSION_TYPE }));
                }
                if (objSN.CompletedFlag == "1" && objSN.StockStatus == "1")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808104922", new string[] { Paras[0].SESSION_TYPE }));
                }
                if (objSN.CurrentStation == "REWORK")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181128201930", new string[] { Paras[0].SESSION_TYPE }));
                }
                string oldStation = objSN.CurrentStation;

                string station_name = sessionStation.Value.ToString();
                if (station_name == "SILOADING")
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000277"));
                }
                string DeviceName = string.Empty;
                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(objSN.RouteID, objSN.NextStation, Station.SFCDB);

                if (RouteDetails.Count > 0)
                {
                    foreach (C_ROUTE_DETAIL c in RouteDetails)
                    {
                        if (station_name == c.STATION_NAME)
                        {
                            Station_SEQ = (Double)c.SEQ_NO;
                        }
                        //2019/03/29 Patty fixed bug for reverse, checking next station instead of current station
                        if (objSN.NextStation == c.STATION_NAME)
                        {
                            SN_SEQ = (Double)c.SEQ_NO;
                        }
                    }
                    if (Station_SEQ > SN_SEQ)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808112641", new string[] { Paras[0].SESSION_TYPE }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808113358", new string[] { Paras[1].SESSION_TYPE }));
                }

                var stationList = RouteDetails.FindAll(t => t.SEQ_NO >= Station_SEQ && t.SEQ_NO <= SN_SEQ).Select(t => t.STATION_NAME).ToArray();

                var seqList = RouteDetails.FindAll(t => t.SEQ_NO < Station_SEQ).Select(t => t.SEQ_NO).ToArray();
                Array.Sort(seqList);
                SN_SEQ = (double)seqList[seqList.Length - 1];
                var tt = RouteDetails.Find(t => t.SEQ_NO == SN_SEQ);
                DeviceName = tt.STATION_NAME;
                T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                List<R_SN_KP> snkp = TRKP.GetAllKPBySnIDStation(objSN.ID, Station.SFCDB, stationList);
                List<R_SN> sysKpList = TRKP.GetSysKeypartValueList(objSN.ID, stationList, Station.SFCDB);
                T_C_SKU t_c_sku = new T_C_SKU(Station.SFCDB, Station.DBType);
                string PF = t_c_sku.GetSku(objSN.SkuNo, Station.SFCDB).SKU_NAME;

                //2019/06/17 Patty added logic to check and update HIPOT and tesing result ---start
                //added portion to remove test file from Oracle server vince_20191111
                string host = @"10.18.155.17";
                int port = 22;
                string username = "bomuser";
                string password = @"4bomuser!";
                string sftpPath = @"/var/apache2/htdocs/FOCSFC/BOM";
                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                R_SN R_SN_Data = t_r_sn.LoadSN(objSN.SerialNo, Station.SFCDB);
                string str_skuno = R_SN_Data.SKUNO;
                MESDataObject.Module.T_R_WO_BASE O_TWO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE row_r_wo_base = O_TWO.LoadWorkorder(R_SN_Data.WORKORDERNO, Station.SFCDB);
                string plantO = row_r_wo_base.PLANT;

                T_R_TEST_RECORD t_r_test_record = new T_R_TEST_RECORD(Station.SFCDB, Station.DBType);
                var SnRemark = "--" + objSN.SerialNo;
                if (stationList.Contains("HIPOT"))
                {
                    //if return to Hipot, reverse all including Hipot FSC, FST, UPGRADEKIT test record vince_20191008
                    Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();

                    //Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "HIPOT" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                    if (plantO == "TOGA")
                    {
                        //delete SNBOM file from TDMS server vince_20191205
                        string SNBOMSN = objSN.SerialNo.ToString() + ".BOM";
                        DeleteFile(host, port, username, password, sftpPath, SNBOMSN);
                    }
                }

                if (stationList.Contains("SFT") && stationList.Contains("HIPOT"))
                {
                    // if 2C, need to update both BBT for chassis and SFT for SM
                    //get SM SNs from 2C chassis
                    List<R_SN_KP> SMODSN = TRKP.GetSMODSN(objSN.SerialNo, Station.SFCDB);

                    //Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "SFT" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                    if (PF.Contains("2C")) //should alwasy get 2 SMODs
                    {
                        //Not need to count SMOD vince_20191008
                        //if (SMODSN.Count == 2)
                        //{
                        //remark SFT records for 2 SMODs
                        foreach (var a in SMODSN)
                        {
                            var SMSnRemark = "--" + a.VALUE;
                            Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SMSnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "SFT" && t.STATE.ToUpper() == "PASS" && t.SN == a.VALUE).ExecuteCommand();
                            if (plantO == "TOGA")
                            {
                                //delete SNBOM file from TDMS server vince_20191205
                                string SNBOMSN = a.VALUE.ToString() + ".BOM";
                                DeleteFile(host, port, username, password, sftpPath, SNBOMSN);
                            }
                        }
                        //for 2C chassis
                        Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "BBT" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                        //}
                        //else
                        //{
                        //    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000276", new string[] { objSN.SerialNo }));
                        //}
                    }
                    //else
                    //{
                    //    //for othere products
                    //    Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "SFT" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                    //}
                }
                //add FSC test record reverse vince_20191008
                //if (stationList.Contains("FSC"))
                //{
                //    Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "FSC" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                //}

                //if (stationList.Contains("FST"))
                //{
                //    Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "FST" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                //}

                //if (stationList.Contains("UPGRADEKIT"))
                //{
                //    Station.SFCDB.ORM.Updateable<R_TEST_RECORD>().UpdateColumns(t => new R_TEST_RECORD { SN = SnRemark, EDIT_TIME = DateTime.Now, EDIT_EMP = Station.LoginUser.EMP_NO }).Where(t => t.TESTATION.ToUpper() == "UPGRADEKIT" && t.STATE.ToUpper() == "PASS" && t.SN == objSN.SerialNo).ExecuteCommand();
                //}

                //write KP record into MV table before update
                //T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                Station.SFCDB.ORM.Insertable<R_SN_MV>(t_r_sn.LoadSN(objSN.SerialNo, Station.SFCDB)).ExecuteCommand();

                foreach (var kp in snkp)
                {
                    Station.SFCDB.ORM.Insertable<R_SN_KP_MV>(kp).ExecuteCommand();
                }
                //if unit is completed and it's not back-flush, update it back
                if (objSN.CompletedFlag == "1" && objSN.StockStatus == null)
                {
                    Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN { COMPLETED_FLAG = "0", COMPLETED_TIME = null }).Where(t => t.SN == objSN.SerialNo).ExecuteCommand();
                }

                //2019/06/17 Patty added logic to check and update HIPOT and tesing result ---end

                if (snkp.Count > 0)
                {
                    result = TRKP.ReturnUpdateKPSNBySnId(objSN.ID, stationList, Station.LoginUser.EMP_NO, Station.SFCDB);
                }
                if (result < 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + objSN.SerialNo, "ADD" }));
                }
                T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                table.ReturnPassStation(objSN.SerialNo, Station.Line, DeviceName, station_name, Station.BU, objSN.ProductStatus, Station.LoginUser.EMP_NO, Station.SFCDB);
                table.UpdateShippingFlag(sysKpList, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                Station.SFCDB.ThrowSqlExeception = true;
                Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);

                //Add log to check who reversed it
                T_R_MES_LOG mesLog = new T_R_MES_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                string id = mesLog.GetNewID("ORACLE", Station.SFCDB);
                Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                Station.SFCDB.BeginTrain();
                id = mesLog.GetNewID("ORACLE", Station.SFCDB);
                rowMESLog.ID = id;
                rowMESLog.PROGRAM_NAME = "CloudMES";
                rowMESLog.CLASS_NAME = "SNAction";
                rowMESLog.FUNCTION_NAME = "ORAReturnSNAction";
                rowMESLog.LOG_MESSAGE = sessionSn.Value.ToString() + " has been reversed to " + sessionStation.Value.ToString();
                rowMESLog.LOG_SQL = "";
                rowMESLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowMESLog.EDIT_TIME = System.DateTime.Now;
                rowMESLog.DATA1 = sessionSn.Value.ToString();
                rowMESLog.DATA2 = oldStation;
                rowMESLog.DATA3 = sessionStation.Value.ToString();
                Station.SFCDB.ThrowSqlExeception = true;
                Station.SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Add reopen order update record function vince_20200220 --- start
        public static void ORAReopenSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession ClearFlagSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = "true" };
            Station.StationSession.Add(ClearFlagSession);

            try
            {
                SN objSN = (SN)sessionSn.Value;

                string oldStation = objSN.CurrentStation;

                string station_name = sessionStation.Value.ToString();
                string SNID = objSN.ID;
                string newSNID = SNID + "-Reopen";
                //if (station_name == "SILOADING")
                //{
                //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000277"));
                //}
                string DeviceName = string.Empty;
                T_C_ROUTE_DETAIL RouteDetailTable = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                List<C_ROUTE_DETAIL> RouteDetails = RouteDetailTable.ORAGetPreviousByRouteId(objSN.RouteID, objSN.NextStation, Station.SFCDB);

                T_R_SN_KP TRKP = new T_R_SN_KP(Station.SFCDB, Station.DBType);
                T_C_SKU t_c_sku = new T_C_SKU(Station.SFCDB, Station.DBType);
                string PF = t_c_sku.GetSku(objSN.SkuNo, Station.SFCDB).SKU_NAME;

                T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                R_SN R_SN_Data = t_r_sn.LoadSN(objSN.SerialNo, Station.SFCDB);
                string str_skuno = R_SN_Data.SKUNO;
                MESDataObject.Module.T_R_WO_BASE O_TWO = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE row_r_wo_base = O_TWO.LoadWorkorder(R_SN_Data.WORKORDERNO, Station.SFCDB);
                string plantO = row_r_wo_base.PLANT;

                //write SN record into MV table before update
                //T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                Station.SFCDB.ORM.Insertable<R_SN_MV>(t_r_sn.LoadSN(objSN.SerialNo, Station.SFCDB)).ExecuteCommand();

                //if unit is completed and it's not back-flush, update it back
                if (objSN.CompletedFlag == "1")
                {
                    Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN
                    {
                        COMPLETED_FLAG = "0",
                        COMPLETED_TIME = null,
                        SHIPPED_FLAG = "0",
                        SHIPDATE = null,
                        STOCK_STATUS = "0",
                        STOCK_IN_TIME = null,
                        //CURRENT_STATION = "PACKOUT",
                        NEXT_STATION = oldStation,
                        PACKED_FLAG = "0",
                        PACKDATE = null
                    }).Where(t => t.SN == objSN.SerialNo).ExecuteCommand();
                }

                Station.SFCDB.ORM.Updateable<R_WO_BASE>().UpdateColumns(t => new R_WO_BASE { CLOSED_FLAG = "0", CLOSE_DATE = null, FINISHED_QTY = row_r_wo_base.FINISHED_QTY - 1 }).Where(t => t.WORKORDERNO == R_SN_Data.WORKORDERNO).ExecuteCommand();

                Station.SFCDB.ORM.Updateable<R_SN_PACKING>().UpdateColumns(t => new R_SN_PACKING { SN_ID = newSNID }).Where(t => t.SN_ID == SNID).ExecuteCommand();

                //T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
                //table.ReturnPassStation(objSN.SerialNo, Station.Line, station_name, station_name, Station.BU, objSN.ProductStatus, Station.LoginUser.EMP_NO, Station.SFCDB);
                //table.UpdateShippingFlag(sysKpList, "0", Station.LoginUser.EMP_NO, Station.SFCDB);
                //Station.SFCDB.ThrowSqlExeception = true;
                Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);

                //Add log to check who reversed it
                T_R_MES_LOG mesLog = new T_R_MES_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                string id = mesLog.GetNewID("ORACLE", Station.SFCDB);
                Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();

                Station.SFCDB.BeginTrain();
                id = mesLog.GetNewID("ORACLE", Station.SFCDB);
                rowMESLog.ID = id;
                rowMESLog.PROGRAM_NAME = "CloudMES";
                rowMESLog.CLASS_NAME = "SNAction";
                rowMESLog.FUNCTION_NAME = "ORAReopenSNAction";
                rowMESLog.LOG_MESSAGE = sessionSn.Value.ToString() + " has been reopen to " + sessionStation.Value.ToString();
                rowMESLog.LOG_SQL = "";
                rowMESLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowMESLog.EDIT_TIME = System.DateTime.Now;
                rowMESLog.DATA1 = sessionSn.Value.ToString();
                rowMESLog.DATA2 = oldStation;
                rowMESLog.DATA3 = sessionStation.Value.ToString();
                rowMESLog.MAILFLAG = "0";
                Station.SFCDB.ThrowSqlExeception = true;
                Station.SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
                Station.SFCDB.CommitTrain();

                Station.StationMessages.Add(new StationMessage() { Message = (objSN.SerialNo + " Reopen OK! Next Station is [" + oldStation + "], please continue to process in Reverse station and reverse B/F in SAP."), State = StationMessageState.Message });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //Add reopen order update record function vince_20200220 --- end

        /// <summary>
        /// AddSMTLoadingRecordsBySn
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddSMTLoadingRecordsBySn(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            string message = "";
            string strSN = "";
            string StrWO = "";
            string StrTrCode = "";
            Dictionary<string, DataRow> TrSnTable = null;
            string Process = string.Empty;
            double LinkQty = 0d;
            string MacAddress = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000051", new string[] { "5", Paras.Count.ToString() }));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            strSN = snSession.Value.ToString();
            //獲取 TRSN 對象
            MESStationSession TrSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (TrSnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            TrSnTable = (Dictionary<string, DataRow>)TrSnSession.Value;

            //獲取面別
            Process = Paras[2].VALUE.ToString();

            //獲取連板數
            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (LinkQtySession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            LinkQty = double.Parse(LinkQtySession.Value.ToString());
            MacAddress = Paras[4].VALUE.ToString();
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + Paras[5].SESSION_KEY }));
            }
            StrWO = WoSession.Value.ToString();

            MESStationSession TrCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[6].SESSION_TYPE && t.SessionKey == Paras[6].SESSION_KEY);

            if (TrCodeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[6].SESSION_TYPE + Paras[6].SESSION_KEY }));
            }
            StrTrCode = TrCodeSession.Value.ToString();
            try
            {
                apdb = Station.APDB;

                if (Station.BU.Equals("BPD"))
                {
                    AP_DLL ap_dll = new AP_DLL();
                    string PanelCode = ap_dll.GetNextPanelCode(apdb);
                    message = table.AddSMTLoadingRecords(StrWO, new List<string>() { strSN }, PanelCode, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, StrTrCode, apdb);
                }
                else
                {
                    message = table.AddSMTLoadingRecords(StrWO, strSN, TrSnTable["R_TR_SN"]["TR_SN"].ToString(), Process, LinkQty, Station.LoginUser.EMP_NO, MacAddress, StrTrCode, "N", apdb);
                }
                if (message.Equals("OK"))
                {
                    Station.AddMessage("MES00000053", new string[] { }, StationMessageState.Pass); //回饋消息到前台
                    //Station.DBS["APDB"].Return(apdb);
                }
                else
                {
                    //Station.DBS["APDB"].Return(apdb);
                    throw new MESReturnMessage(message);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HWT SMTLOADING過站檢查allpart信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        //public static void SMTLoadingPassBySN_CheckAP(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        //{
        //    //Dictionary<string, DataRow> APInfo = new Dictionary<string, DataRow>();
        //    string strTRSN = "", strWO = Station.StationSession[0].Value.ToString(), strIP = Station.IP.ToString(), strEMPNO = Station.LoginUser.EMP_NO.ToString();
        //    //string ErrMessage = "";
        //    OleExec apdb = null;
        //    if (Paras.Count != 1)
        //    {
        //        string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
        //        throw new MESReturnMessage(errMsg);
        //    }

        //    strTRSN = Input.Value.ToString();
        //    MESStationSession TRSN_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
        //    if (TRSN_Session == null)
        //    {
        //        TRSN_Session = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
        //        Station.StationSession.Add(TRSN_Session);
        //    }
        //    else
        //    {
        //        TRSN_Session.ResetInput = Input;
        //        TRSN_Session.InputValue = strTRSN;
        //        TRSN_Session.Value = strTRSN;
        //    }

        //    //獲取ALLPART數據
        //    //APInfo = new Dictionary<string, DataRow>();
        //    AP_DLL APDLL = new AP_DLL();
        //    try
        //    {
        //        apdb = Station.APDB;
        //        //string TRSN, string WO, string IP, string EMP_NO, string PROCESS,string SN,string LINKQTY, OleExec DB
        //        string result = APDLL.AP_z_insert_panel_snlink_new(strTRSN, strWO, strIP, strEMPNO, "", apdb);
        //        //TRSN_Session.Value = APInfo;
        //        //if (Station.StationSession[2].Value.ToString() == "1")
        //        //{
        //        //    Station.StationMessages.Add(new StationMessage { Message = "請掃描30位LOT_CODE條碼", State = StationMessageState.Message });
        //        //}
        //        //Station.AddMessage("MES00000001", new string[] { TRSN_Session.Value.ToString() }, MESReturnView.Station.StationMessageState.Pass);
        //    }
        //    catch (Exception ex)
        //    {
        //        if (apdb != null)
        //        {
        //        }
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// add by fgg 2018.8.29
        /// SMTLoadingPassBySN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTLoadingPassByMultiSN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            MESStationSession sessionSns = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSns == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            var SNs = (List<string>)sessionSns.Value;

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionExtQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                OleExec sfcdb = Station.SFCDB;
                DB_TYPE_ENUM sfcdbType = Station.DBType;
                Dictionary<string, object> dicNextStation;
                string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                objWorkorder = (WorkOrder)sessionWO.Value;
                dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

                #region 寫入r_sn,r_sn_station_detail




                for (int i = 0; i < SNs.Count; i++)
                {
                    T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                    R_SN r_sn = new R_SN();
                    r_sn.ID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                    r_sn.SN = SNs[i];
                    r_sn.SKUNO = objWorkorder.SkuNO;
                    r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                    r_sn.PLANT = objWorkorder.PLANT;
                    r_sn.ROUTE_ID = objWorkorder.RouteID;
                    r_sn.STARTED_FLAG = "1";
                    r_sn.START_TIME = Station.GetDBDateTime();
                    r_sn.PACKED_FLAG = "0";
                    r_sn.COMPLETED_FLAG = "0";
                    r_sn.SHIPPED_FLAG = "0";
                    r_sn.REPAIR_FAILED_FLAG = "0";
                    r_sn.CURRENT_STATION = Station.StationName;
                    r_sn.NEXT_STATION = nextStation;
                    r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                    r_sn.CUST_PN = objWorkorder.CUST_PN;
                    r_sn.VALID_FLAG = "1";
                    r_sn.STOCK_STATUS = "0";
                    r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                    r_sn.EDIT_TIME = Station.GetDBDateTime();
                    result = t_r_sn.AddNewSN(r_sn, sfcdb);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SNs[i], "ADD" }));
                    }
                    t_r_sn.RecordPassStationDetail(SNs[i], Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);

                    #endregion 寫入r_sn,r_sn_station_detail

                    #region 寫入 r_sn_kp

                    T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                    if (objWorkorder.KP_LIST_ID != "" && c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                    {
                        SN snObject = new SN();
                        snObject.InsertR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType);
                    }

                    #endregion 寫入 r_sn_kp

                    //更新工單投入數量

                    result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                    }

                    Row_R_WO_BASE newRowWo = t_r_wo_base.LoadWorkorder(objWorkorder.WorkorderNo, sfcdb);
                    sessionInputQty.Value = newRowWo.INPUT_QTY;
                    sessionExtQty.Value = newRowWo.WORKORDER_QTY - newRowWo.INPUT_QTY;

                    if (newRowWo.INPUT_QTY >= newRowWo.WORKORDER_QTY)
                    {
                        Station.AddMessage("MES00000063", new string[] { SNs[i] }, StationMessageState.Pass);
                        Station.AddMessage("MES00000247", new string[] { newRowWo.WORKORDERNO }, StationMessageState.Pass);
                    }
                    else
                    {
                        Station.AddMessage("MES00000063", new string[] { SNs[i] }, StationMessageState.Pass);
                    }


                }
                SNs.Clear();

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// add by fgg 2018.8.29
        /// SMTLoadingPassBySN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SMTLoadingPassBySN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
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

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            if (sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionExtQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                OleExec sfcdb = Station.SFCDB;
                DB_TYPE_ENUM sfcdbType = Station.DBType;
                Dictionary<string, object> dicNextStation;
                string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                objWorkorder = (WorkOrder)sessionWO.Value;
                dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

                #region 寫入r_sn,r_sn_station_detail

                T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                R_SN r_sn = new R_SN();
                r_sn.ID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                r_sn.SN = sessionSn.Value.ToString();
                r_sn.SKUNO = objWorkorder.SkuNO;
                r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                r_sn.PLANT = objWorkorder.PLANT;
                r_sn.ROUTE_ID = objWorkorder.RouteID;
                r_sn.STARTED_FLAG = "1";
                r_sn.START_TIME = Station.GetDBDateTime();
                r_sn.PACKED_FLAG = "0";
                r_sn.COMPLETED_FLAG = "0";
                r_sn.SHIPPED_FLAG = "0";
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.CURRENT_STATION = Station.StationName;
                r_sn.NEXT_STATION = nextStation;
                r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                r_sn.CUST_PN = objWorkorder.CUST_PN;
                r_sn.VALID_FLAG = "1";
                r_sn.STOCK_STATUS = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = Station.GetDBDateTime();
                result = t_r_sn.AddNewSN(r_sn, sfcdb);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + sessionSn.Value.ToString(), "ADD" }));
                }
                t_r_sn.RecordPassStationDetail(sessionSn.Value.ToString(), Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);

                #endregion 寫入r_sn,r_sn_station_detail

                #region 寫入 r_sn_kp

                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                if (objWorkorder.KP_LIST_ID != "" && c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                {
                    SN snObject = new SN();
                    snObject.InsertR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType);
                }

                #endregion 寫入 r_sn_kp

                //更新工單投入數量

                result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                }

                Row_R_WO_BASE newRowWo = t_r_wo_base.LoadWorkorder(objWorkorder.WorkorderNo, sfcdb);
                sessionInputQty.Value = newRowWo.INPUT_QTY;
                sessionExtQty.Value = newRowWo.WORKORDER_QTY - newRowWo.INPUT_QTY;

                if (newRowWo.INPUT_QTY >= newRowWo.WORKORDER_QTY)
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                    Station.AddMessage("MES00000247", new string[] { newRowWo.WORKORDERNO }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SMTLoadingPassBySN_HWT(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 6)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            OleExec sfcdb = Station.SFCDB;

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

            MESStationSession sessionInputQty = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (sessionInputQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            MESStationSession sessionExtQty = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (sessionExtQty == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE }));
            }

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            MESStationSession sessionPType = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (sessionPType == null)
            {
                throw new System.Exception("sessionPType miss ");
            }

            try
            {
                WorkOrder objWorkorder = new WorkOrder();
                DB_TYPE_ENUM sfcdbType = Station.DBType;
                Dictionary<string, object> dicNextStation;
                string nextStation = "";
                int result;
                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(sfcdb, sfcdbType);
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(sfcdb, sfcdbType);
                objWorkorder = (WorkOrder)sessionWO.Value;
                dicNextStation = t_c_route_detail.GetNextStations(objWorkorder.RouteID, Station.StationName, sfcdb);
                nextStation = ((List<string>)dicNextStation["NextStations"])[0].ToString();

                #region 寫入r_sn,r_sn_station_detail

                T_R_SN t_r_sn = new T_R_SN(sfcdb, sfcdbType);
                R_SN r_sn = new R_SN();
                r_sn.ID = t_r_sn.GetNewID(Station.BU, sfcdb, sfcdbType);
                r_sn.SN = sessionSn.Value.ToString();
                r_sn.SKUNO = objWorkorder.SkuNO;
                r_sn.WORKORDERNO = objWorkorder.WorkorderNo;
                r_sn.PLANT = objWorkorder.PLANT;
                r_sn.ROUTE_ID = objWorkorder.RouteID;
                r_sn.STARTED_FLAG = "1";
                r_sn.START_TIME = Station.GetDBDateTime();
                r_sn.PACKED_FLAG = "0";
                r_sn.COMPLETED_FLAG = "0";
                r_sn.SHIPPED_FLAG = "0";
                r_sn.REPAIR_FAILED_FLAG = "0";
                r_sn.CURRENT_STATION = Station.StationName;
                r_sn.NEXT_STATION = nextStation;
                r_sn.KP_LIST_ID = objWorkorder.KP_LIST_ID;
                r_sn.CUST_PN = objWorkorder.CUST_PN;
                r_sn.VALID_FLAG = "1";
                r_sn.STOCK_STATUS = "0";
                r_sn.EDIT_EMP = Station.LoginUser.EMP_NO;
                r_sn.EDIT_TIME = Station.GetDBDateTime();
                result = t_r_sn.AddNewSN(r_sn, sfcdb);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + sessionSn.Value.ToString(), "ADD" }));
                }
                t_r_sn.RecordPassStationDetail(sessionSn.Value.ToString(), Station.Line, Station.StationName, Station.StationName, Station.BU, sfcdb);

                #endregion 寫入r_sn,r_sn_station_detail

                #region 寫入 r_sn_kp

                T_C_KP_LIST c_kp_list = new T_C_KP_LIST(sfcdb, sfcdbType);
                if (objWorkorder.KP_LIST_ID != "" && c_kp_list.KpIDIsExist(objWorkorder.KP_LIST_ID, sfcdb))
                {
                    SN snObject = new SN();
                    snObject.InsertR_SN_KP(objWorkorder, r_sn, sfcdb, Station, sfcdbType);
                }

                #endregion 寫入 r_sn_kp

                //更新工單投入數量

                result = Convert.ToInt32(t_r_wo_base.AddCountToWo(objWorkorder.WorkorderNo, 1, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + objWorkorder.WorkorderNo, "UPDATE" }));
                }

                Row_R_WO_BASE newRowWo = t_r_wo_base.LoadWorkorder(objWorkorder.WorkorderNo, sfcdb);
                sessionInputQty.Value = newRowWo.INPUT_QTY;
                sessionExtQty.Value = newRowWo.WORKORDER_QTY - newRowWo.INPUT_QTY;

                if (newRowWo.INPUT_QTY >= newRowWo.WORKORDER_QTY)
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                    Station.AddMessage("MES00000247", new string[] { newRowWo.WORKORDERNO }, StationMessageState.Pass);
                }
                else
                {
                    Station.AddMessage("MES00000063", new string[] { sessionSn.Value.ToString() }, StationMessageState.Pass);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            try
            {
                //數據準備
                string ProductType;
                string WO = sessionWO.Value.ToString();
                WorkOrder objWO = (WorkOrder)sessionWO.Value;
                string WOType = objWO.WO_TYPE.ToString();
                string HWItem = objWO.CUST_PN.ToString();
                string SkuNo = objWO.SkuNO.ToString();
                string EMP = Station.LoginUser.EMP_NO;
                string BU = Station.BU.ToString();
                string strhwver = objWO.CUST_PN_VER.ToString();//取工單版本（包括版本管控機種的batch欄位）
                string SN = sessionSn.Value.ToString();
                SKU objSKU = (SKU)sessionSKU.Value;
                string strhwdesc = objSKU.Description.ToString();
                string strskuid = objSKU.SkuId.ToString();
                //製程類型RMA/NPI等
                ProductType = sessionPType.Value.ToString();
                string strsql = "", TaskNo = "", TaskStatus = "NG", strlst, FatherSN = "";
                string /*sku2nd = "",*/
                    strltype = "", str2dsn = "", strhwrohs = "";
                T_C_SKU tcsku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
                strhwrohs = tcsku.GetSkuRoHs(strskuid, "1", "ROHS", sfcdb);
                int TaskQty, LoadQty, TaskSeqno;
                DataTable dt = new DataTable();
                DataTable dtt = new DataTable();
                T_r_task_order_sn TRTOS = new T_r_task_order_sn(sfcdb, DB_TYPE_ENUM.Oracle);
                T_r_2d_sn_relation T_r_2d_sn_relation = new T_r_2d_sn_relation(sfcdb, DB_TYPE_ENUM.Oracle);
                //判斷機種類型
                T_C_ITEMCODE_MAPPING_EMS t_c_itemcode_mapping_ems = new T_C_ITEMCODE_MAPPING_EMS(sfcdb, DB_TYPE_ENUM.Oracle);
                string skutype = t_c_itemcode_mapping_ems.Get_Customer_Partno("SKUTYPE", SkuNo, sfcdb);

                //判斷是否存在父項，父項也要寫入R_2D_SN_RELATION
                T_C_SKU_DETAIL tcsd = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                if (tcsd.CheckExistsFatherSN(SkuNo, sfcdb))
                {
                    strlst = "Y";
                }
                else
                {
                    strlst = "N";
                }
                //T_R_WO_TYPE trwt = new T_R_WO_TYPE(sfcdb,DB_TYPE_ENUM.Oracle);
                //string ProductType = trwt.GetProductTypeByWO_HWT(sfcdb,WO);//獲取工單是RMA還是NPI等屬性

                //正常工單需要記錄任務令與SN的綁定關係
                //if (WOType == "REGULAR")
                //{
                strsql = $@"SELECT substr(ltxa1, 1, instr(ltxa1, ',') - 1) task_no,
                                       REPLACE(substr(ltxa1, instr(ltxa1, ',') + 1), ';', '') task_qty
                                  FROM r_wo_text
                                 WHERE aufnr IN ('{WO}')
                                 ORDER BY ltxa1";
                dt = sfcdb.RunSelect(strsql).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (TaskStatus == "OK")
                    {
                        //避免一個SN錄入多個任務令記錄
                        break;
                    }
                    TaskNo = dt.Rows[i]["task_no"].ToString();
                    TaskQty = Convert.ToInt32(dt.Rows[i]["task_qty"].ToString());

                    //strsql = $@"SELECT COUNT(*) loadqty FROM r_task_order_sn WHERE hw_task_no = '{TaskNo}'";
                    //dtt = sfcdb.RunSelect(strsql).Tables[0];
                    //LoadQty = Convert.ToInt32(dtt.Rows[0]["loadqty"].ToString());

                    LoadQty = sfcdb.ORM.Queryable<r_task_order_sn>().Where(t => t.HW_TASK_NO == TaskNo.ToString()).ToList().Count;//.FirstOrDefault();

                    TaskSeqno = LoadQty + 1;
                    if (LoadQty == TaskQty)
                    {
                        continue;
                    }
                    else
                    {
                        //還能loading就OK
                        TaskStatus = "OK";
                        //先判斷是否存在，存在就報錯
                        T_r_task_order_sn trtos = new T_r_task_order_sn(sfcdb, DB_TYPE_ENUM.Oracle);
                        if (trtos.CheckExists(SN, sfcdb))//返回true代表沒有數據
                        {
                            r_task_order_sn RTOS = new r_task_order_sn();
                            RTOS.ID = TRTOS.GetNewID(Station.BU, sfcdb);
                            RTOS.SN = SN.ToString();
                            RTOS.HW_TASK_NO = TaskNo;
                            RTOS.HW_TASK_ITEM = HWItem;
                            RTOS.SKUNO = SkuNo;
                            RTOS.WO = WO.ToString();
                            RTOS.TASK_QTY = TaskQty.ToString();
                            RTOS.TASK_SEQNO = TaskSeqno.ToString();
                            RTOS.EDIT_EMP = EMP;
                            RTOS.EDIT_TIME = TRTOS.GetDBDateTime(sfcdb);
                            TRTOS.Insert(RTOS, sfcdb);
                        }
                        else
                        {
                            //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("{0}已經存在任務令關係表中，請掃描其他條碼", new string[] { SN }));
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153255", new string[] { SN }));
                        }
                    }
                }

                //判斷是否二代標籤
                T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                if (t_c_control.ValueIsExist("2ND_2D_LAB", SkuNo, sfcdb))
                {
                    //sku2nd = "Y";
                    strltype = "2D";
                    if (strhwver.Substring(0, 1) == "0")//如果版本是999或者0開頭的就不用傳
                    {
                        strhwver = "";
                    }

                    str2dsn = sfcdb.ORM.Queryable<R_LABEL_PRINT_T>().Where(t => t.WO == WO.ToString() && t.SN == SN.ToString()).ToList().FirstOrDefault().SN_2D.ToString();
                }
                else
                {
                    //sku2nd = "N";
                    strltype = "1D";
                    strhwver = "";
                    str2dsn = "";
                }

                if (ProductType == "RMA")//RMA的需要重傳
                {
                    var listr2sr = sfcdb.ORM.Queryable<r_2d_sn_relation>().Where(t => t.SN == SN).ToList().FirstOrDefault();
                    if (listr2sr != null && listr2sr.UPLOAD_FLAG == 2)
                    {
                        sfcdb.ORM.Updateable<r_2d_sn_relation>().UpdateColumns(t => t.SN == DateTime.Now.ToString("yymmddhh24miss") + SN).Where(t => t.ID == listr2sr.ID && t.UPLOAD_FLAG == 2).ExecuteCommand();
                    }
                }

                r_2d_sn_relation r_2d_sn_relation = new r_2d_sn_relation();
                r_2d_sn_relation.ID = T_r_2d_sn_relation.GetNewID(Station.BU, sfcdb);
                r_2d_sn_relation.SN = SN.ToString();
                r_2d_sn_relation.BARCODE_2D = str2dsn.ToString();
                r_2d_sn_relation.ITEM_NUMBER = HWItem;
                r_2d_sn_relation.ITEM_VER = strhwver;
                r_2d_sn_relation.CREATED_BY = "SYSTEM";
                r_2d_sn_relation.WO = WO;
                r_2d_sn_relation.LABEL_TYPE = strltype;
                r_2d_sn_relation.DESCRIPT = strhwdesc;
                r_2d_sn_relation.ROHS_FLAG = strhwrohs;
                r_2d_sn_relation.TRANS_ID = "H" + System.DateTime.Now.ToString("YYMMDDHHMISS") + SN.Substring(-5).ToString() + "00000";
                r_2d_sn_relation.SN_TYPE = strltype;
                r_2d_sn_relation.PO = ProductType == "RMA" ? "EPWENH00008" : TaskNo;
                T_r_2d_sn_relation.Insert(r_2d_sn_relation, sfcdb);

                if (strlst == "Y" && ProductType != "RMA")//有父項,且為正常品
                {
                    if (skutype == "OLD" || SN.Length == 16)
                    {
                        FatherSN = SN.Substring(0, 10) + "6" + SN.Substring(-5);
                    }
                    else
                    {
                        FatherSN = t_c_itemcode_mapping_ems.Get_Customer_Partno("CONVERT_BOXSN", SN, sfcdb);
                    }
                    //拉手條寫入r_2d_sn_relation
                    r_2d_sn_relation.ID = T_r_2d_sn_relation.GetNewID(Station.BU, sfcdb);
                    r_2d_sn_relation.SN = FatherSN.ToString();
                    r_2d_sn_relation.BARCODE_2D = str2dsn.ToString();
                    r_2d_sn_relation.ITEM_NUMBER = HWItem;
                    r_2d_sn_relation.ITEM_VER = strhwver;
                    r_2d_sn_relation.CREATED_BY = "SYSTEM";
                    r_2d_sn_relation.WO = WO;
                    r_2d_sn_relation.LABEL_TYPE = strltype;
                    r_2d_sn_relation.DESCRIPT = strhwdesc;
                    r_2d_sn_relation.ROHS_FLAG = strhwrohs;
                    r_2d_sn_relation.TRANS_ID = "H" + System.DateTime.Now.ToString("YYMMDDHHMISS") + SN.Substring(-5).ToString() + "00000";
                    r_2d_sn_relation.SN_TYPE = strltype;
                    r_2d_sn_relation.PO = TaskNo;
                    T_r_2d_sn_relation.Insert(r_2d_sn_relation, sfcdb);
                }

                //外廠條碼寫入接口表向HW獲取父項
                if (strlst == "N" || (strlst == "Y" && ProductType == "RMA"))//沒有父項的或者有父項且為RMA的
                {
                    if ((SN.Length == 16 && SN.Substring(6, 2) != "DM") || (SN.Length == 20 && SN.Substring(10, 2) != "DM"))
                    {
                        T_R_RELATIONDATA_EXTERNAL trre = new T_R_RELATIONDATA_EXTERNAL(sfcdb, DB_TYPE_ENUM.Oracle);
                        R_RELATIONDATA_EXTERNAL rre = new R_RELATIONDATA_EXTERNAL();
                        rre.ID = trre.GetNewID(BU, sfcdb);
                        rre.SN = SN;
                        rre.PARENT = SN;
                        rre.RECEIVE_FLAG = ProductType == "RMA" ? "N" : "Y";//RMA製程設為N，其他為Y
                        rre.EDIT_EMP = EMP;
                        trre.Insert(rre, sfcdb);
                    }
                }

                T_R_SN_MAC trsm = new T_R_SN_MAC(sfcdb, DB_TYPE_ENUM.Oracle);
                R_SN_MAC rsm = new R_SN_MAC();
                T_C_SKU_Label TCSL = new T_C_SKU_Label(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(SkuNo, "ASSY", sfcdb);
                //在線列印的類型
                string OnLinePrint = "MAC,IMEI,GENCODE,SN,KPIMEI";
                for (int m = 0; m < labs.Count; m++)
                {
                    //if (OnLinePrint.IndexOf(labs[m].LABELTYPE.ToString()) >= 0)
                    if (OnLinePrint.Contains(labs[m].LABELTYPE.ToString()))
                    {
                        string subtype = labs[m].LABELTYPE.ToString();
                        rsm.ID = trsm.GetNewID(BU, sfcdb);
                        rsm.SN = SN;
                        rsm.WO = WO;
                        rsm.SUBSN_TYPE = subtype;
                        rsm.TASK_NO = TaskNo;
                        rsm.BOXSN = (strlst == "Y" ? FatherSN : SN);
                        trsm.Insert(rsm, sfcdb);
                    }
                }

                #region 縮位條碼機種

                //縮位條碼處理,該邏輯和機種在16年就不用了，先不做了，以後導入再說
                //if (t_c_control.ValueIsExist("TC0013", SkuNo, sfcdb))
                //{
                //    var listcsk = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SkuNo && t.CATEGORY_NAME == "PCB S/N").ToList().FirstOrDefault();
                //    if (listcsk != null)
                //    {
                //        string temprule = listcsk.BASETEMPLATE.ToString();
                //        strsql = $@"";
                //    }
                //    //if (tcsd.CheckNameExists(SkuNo, "PCB S/N", sfcdb))
                //    //{
                //    //}
                //}

                #endregion 縮位條碼機種

                if (TaskStatus == "NG")
                {
                    //throw new Exception("該工單對應的任務令已load滿！");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153610"));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                sfcdb.ThrowSqlExeception = false;
            }
        }

        /// <summary>
        /// HWT預鎖定處理
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void checkPlanLock(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數

            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new MESReturnMessage("sessionSKU miss ");
            }
            string Skuno = sessionSKU.Value.ToString();

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage("sessionWO miss ");
            }

            string WO = sessionWO.Value.ToString();

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage("sessionSN miss ");
            }

            string SN = sessionSN.Value.ToString();

            //string Type = Paras[2].VALUE.ToString();

            string Remark = Paras[3].VALUE.ToString();

            #endregion 獲取傳入參數

            try
            {
                T_R_PLAN_LOCK plan_lock = new T_R_PLAN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //正常的機種鎖定檢查
                //t_r_sn_lock.CheckSkuLock(SKUNO, "SKU", "CARTON", Station.SFCDB);

                //處理預鎖定工單
                //plan_lock.IsPlanLock(WO, SN, Type, Remark, Station.SFCDB);
                //判斷級別從大到小，如果配置了大的後面就不用看了
                if (!plan_lock.IsPlanLock(Skuno, SN, "SKUNO", "a", Station.SFCDB))
                {
                    if (!plan_lock.IsPlanLock(WO, SN, "WO", "a", Station.SFCDB))
                    {
                        plan_lock.IsPlanLock(SN, SN, "SN", "a", Station.SFCDB);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HWT by wo 預鎖定判斷
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void checkPlanLock_byWO(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數

            if (Paras.Count != 4)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new System.Exception("sessionWO miss ");
            }

            string WO = sessionWO.Value.ToString();

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            string SN = sessionSN.Value.ToString();

            string Type = Paras[2].VALUE.ToString();

            string Remark = Paras[3].VALUE.ToString();

            #endregion 獲取傳入參數

            try
            {
                T_R_PLAN_LOCK plan_lock = new T_R_PLAN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //正常的機種鎖定檢查
                //t_r_sn_lock.CheckSkuLock(SKUNO, "SKU", "CARTON", Station.SFCDB);

                //處理預鎖定工單
                plan_lock.IsPlanLock(WO, SN, Type, Remark, Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// HWT by Sku 預鎖定判斷
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void checkPlanLock_bySku(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數

            if (Paras.Count != 4)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new System.Exception("sessionSKU miss ");
            }

            string Skuno = sessionSKU.Value.ToString();

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new System.Exception("sessionSN miss ");
            }

            string SN = sessionSN.Value.ToString();

            string Type = Paras[2].VALUE.ToString();

            string Remark = Paras[3].VALUE.ToString();

            #endregion 獲取傳入參數

            try
            {
                T_R_PLAN_LOCK plan_lock = new T_R_PLAN_LOCK(Station.SFCDB, DB_TYPE_ENUM.Oracle);

                //正常的機種鎖定檢查
                //t_r_sn_lock.CheckSkuLock(SKUNO, "SKU", "CARTON", Station.SFCDB);

                //處理預鎖定工單
                plan_lock.IsPlanLock(Skuno, SN, Type, Remark, Station.SFCDB);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 記錄SN和任務令的關係r_task_order_sn
        /// 寫入條碼屬性回傳表r_2d_sn_relation
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void WriteSN_Task_Record(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            #region 獲取傳入參數

            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            string ProductType;
            OleExec sfcdb = Station.SFCDB;

            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null)
            {
                throw new MESReturnMessage("sessionWO miss ");
            }
            string WO = sessionWO.Value.ToString();
            WorkOrder objWO = (WorkOrder)sessionWO.Value;
            string WOType = objWO.WO_TYPE.ToString();
            string HWItem = objWO.CUST_PN.ToString();
            string SkuNo = objWO.SkuNO.ToString();
            string EMP = Station.LoginUser.EMP_NO;
            string BU = Station.BU.ToString();

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null)
            {
                throw new MESReturnMessage("sessionSN miss ");
            }
            string SN = sessionSN.Value.ToString();

            MESStationSession sessionSKU = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSKU == null)
            {
                throw new MESReturnMessage("sessionSN miss ");
            }
            SKU objSKU = (SKU)sessionSKU.Value;
            string strhwdesc = objSKU.Description.ToString();
            string strskuid = objSKU.SkuId.ToString();

            MESStationSession sessionPType = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionPType == null)
            {
                throw new MESReturnMessage("sessionPType miss ");
            }
            else
            {
                //製程類型RMA/NPI等
                ProductType = sessionPType.Value.ToString();
            }

            string strsql = "", TaskNo = "", TaskStatus = "NG", strlst, FatherSN = "";
            string /*sku2nd = "",*/ strltype = "", str2dsn = "", strhwrohs = "";
            string strhwver = objWO.CUST_PN_VER == null ? "" : objWO.CUST_PN_VER.ToString();//取工單版本（包括版本管控機種的batch欄位）
            T_C_SKU tcsku = new T_C_SKU(sfcdb, DB_TYPE_ENUM.Oracle);
            strhwrohs = tcsku.GetSkuRoHs(strskuid, "1", "ROHS", sfcdb);

            int TaskQty, LoadQty, TaskSeqno;
            DataTable dt = new DataTable();
            DataTable dtt = new DataTable();
            T_r_task_order_sn TRTOS = new T_r_task_order_sn(sfcdb, DB_TYPE_ENUM.Oracle);
            T_r_2d_sn_relation T_r_2d_sn_relation = new T_r_2d_sn_relation(sfcdb, DB_TYPE_ENUM.Oracle);

            #endregion 獲取傳入參數

            try
            {
                //數據準備
                //判斷機種類型
                T_C_ITEMCODE_MAPPING_EMS t_c_itemcode_mapping_ems = new T_C_ITEMCODE_MAPPING_EMS(sfcdb, DB_TYPE_ENUM.Oracle);
                string skutype = t_c_itemcode_mapping_ems.Get_Customer_Partno("SKUTYPE", SkuNo, sfcdb);

                //判斷是否存在父項，父項也要寫入R_2D_SN_RELATION
                T_C_SKU_DETAIL tcsd = new T_C_SKU_DETAIL(sfcdb, DB_TYPE_ENUM.Oracle);
                if (tcsd.CheckExistsFatherSN(SkuNo, sfcdb))
                {
                    strlst = "Y";
                }
                else
                {
                    strlst = "N";
                }
                //T_R_WO_TYPE trwt = new T_R_WO_TYPE(sfcdb,DB_TYPE_ENUM.Oracle);
                //string ProductType = trwt.GetProductTypeByWO_HWT(sfcdb,WO);//獲取工單是RMA還是NPI等屬性

                strsql = $@"SELECT substr(ltxa1, 1, instr(ltxa1, ',') - 1) task_no,
                                       REPLACE(substr(ltxa1, instr(ltxa1, ',') + 1), ';', '') task_qty
                                  FROM r_wo_text
                                 WHERE aufnr IN ('{WO}')
                                 ORDER BY ltxa1";
                dt = sfcdb.RunSelect(strsql).Tables[0];
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (TaskStatus == "OK")
                    {
                        //避免一個SN錄入多個任務令記錄
                        break;
                    }
                    TaskNo = dt.Rows[i]["task_no"].ToString();
                    TaskQty = Convert.ToInt32(dt.Rows[i]["task_qty"].ToString());

                    LoadQty = sfcdb.ORM.Queryable<r_task_order_sn>().Where(t => t.HW_TASK_NO == TaskNo.ToString()).ToList().Count;//.FirstOrDefault();

                    TaskSeqno = LoadQty + 1;
                    if (LoadQty == TaskQty)
                    {
                        continue;
                    }
                    else
                    {
                        //還能loading就OK
                        TaskStatus = "OK";
                        //先判斷是否存在，存在就報錯
                        T_r_task_order_sn trtos = new T_r_task_order_sn(sfcdb, DB_TYPE_ENUM.Oracle);
                        if (trtos.CheckExists(SN, sfcdb))//返回true代表沒有數據
                        {
                            r_task_order_sn RTOS = new r_task_order_sn();
                            RTOS.ID = TRTOS.GetNewID(Station.BU, sfcdb);
                            RTOS.SN = SN.ToString();
                            RTOS.HW_TASK_NO = TaskNo;
                            RTOS.HW_TASK_ITEM = HWItem;
                            RTOS.SKUNO = SkuNo;
                            RTOS.WO = WO.ToString();
                            RTOS.TASK_QTY = TaskQty.ToString();
                            RTOS.TASK_SEQNO = TaskSeqno.ToString();
                            RTOS.EDIT_EMP = EMP;
                            RTOS.EDIT_TIME = TRTOS.GetDBDateTime(sfcdb);
                            TRTOS.Insert(RTOS, sfcdb);
                        }
                        else
                        {
                            //{0} 已經存在任務令關係表中，請掃描其他條碼
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190828110754", new string[] { SN }));
                        }
                    }
                }

                if (TaskStatus == "NG")
                {
                    //該工單對應的任務令不存在或已load滿！
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190828110610"));
                }

                //判斷是否二代標籤
                T_C_CONTROL t_c_control = new T_C_CONTROL(sfcdb, DB_TYPE_ENUM.Oracle);
                if (t_c_control.ValueIsExist("2ND_2D_LAB", SkuNo, sfcdb))
                {
                    //sku2nd = "Y";
                    strltype = "2D";
                    if (strhwver.Substring(0, 1) == "0")//如果版本是999或者0開頭的就不用傳
                    {
                        strhwver = "";
                    }

                    str2dsn = sfcdb.ORM.Queryable<R_LABEL_PRINT_T>().Where(t => t.WO == WO.ToString() && t.SN == SN.ToString()).ToList().FirstOrDefault().SN_2D.ToString();
                }
                else
                {
                    //sku2nd = "N";
                    strltype = "1D";
                    strhwver = "";
                    str2dsn = "";
                }

                //string aa = DateTime.Now.ToString("yyMMddHHmmss");

                if (ProductType == "RMA")//RMA的需要重傳
                {
                    var listr2sr = sfcdb.ORM.Queryable<r_2d_sn_relation>().Where(t => t.SN == SN).ToList().FirstOrDefault();
                    if (listr2sr != null && listr2sr.UPLOAD_FLAG == 2)
                    {
                        sfcdb.ORM.Updateable<r_2d_sn_relation>().UpdateColumns(t => t.SN == DateTime.Now.ToString("yyMMddHHmmss") + SN).Where(t => t.ID == listr2sr.ID && t.UPLOAD_FLAG == 2).ExecuteCommand();
                    }
                }

                r_2d_sn_relation r_2d_sn_relation = new r_2d_sn_relation();
                r_2d_sn_relation.ID = T_r_2d_sn_relation.GetNewID(Station.BU, sfcdb);
                r_2d_sn_relation.SN = SN.ToString();
                r_2d_sn_relation.BARCODE_2D = str2dsn.ToString();
                r_2d_sn_relation.ITEM_NUMBER = HWItem;
                r_2d_sn_relation.ITEM_VER = strhwver;
                r_2d_sn_relation.CREATED_BY = "SYSTEM";
                r_2d_sn_relation.WO = WO;
                r_2d_sn_relation.LABEL_TYPE = strltype;
                r_2d_sn_relation.DESCRIPT = strhwdesc;
                r_2d_sn_relation.ROHS_FLAG = strhwrohs;
                r_2d_sn_relation.TRANS_ID = "H" + DateTime.Now.ToString("yyMMddHHmmss") + SN.Substring(SN.Length - 5).ToString() + "00000";
                r_2d_sn_relation.SN_TYPE = "2D_SN";
                r_2d_sn_relation.PO = ProductType == "RMA" ? "EPWENH00008" : TaskNo;
                T_r_2d_sn_relation.Insert(r_2d_sn_relation, sfcdb);

                if (strlst == "Y" && ProductType != "RMA")//有父項,且為正常品
                {
                    if (skutype == "OLD" || SN.Length == 16)
                    {
                        FatherSN = SN.Substring(0, 10) + "6" + SN.Substring(SN.Length - 5);
                    }
                    else
                    {
                        FatherSN = t_c_itemcode_mapping_ems.Get_Customer_Partno("CONVERT_BOXSN", SN, sfcdb);
                    }
                    //拉手條寫入r_2d_sn_relation
                    r_2d_sn_relation.ID = T_r_2d_sn_relation.GetNewID(Station.BU, sfcdb);
                    r_2d_sn_relation.SN = FatherSN.ToString();
                    r_2d_sn_relation.BARCODE_2D = str2dsn.ToString();
                    r_2d_sn_relation.ITEM_NUMBER = HWItem;
                    r_2d_sn_relation.ITEM_VER = strhwver;
                    r_2d_sn_relation.CREATED_BY = "SYSTEM";
                    r_2d_sn_relation.WO = WO;
                    r_2d_sn_relation.LABEL_TYPE = strltype;
                    r_2d_sn_relation.DESCRIPT = strhwdesc;
                    r_2d_sn_relation.ROHS_FLAG = strhwrohs;
                    r_2d_sn_relation.TRANS_ID = "H" + System.DateTime.Now.ToString("yyMMddHHmmss") + SN.Substring(SN.Length - 5).ToString() + "00000";
                    r_2d_sn_relation.SN_TYPE = "2D_SN";
                    r_2d_sn_relation.PO = TaskNo;
                    T_r_2d_sn_relation.Insert(r_2d_sn_relation, sfcdb);
                }

                //外廠條碼寫入接口表向HW獲取父項
                if (strlst == "N" || (strlst == "Y" && ProductType == "RMA"))//沒有父項的或者有父項且為RMA的
                {
                    if ((SN.Length == 16 && SN.Substring(6, 2) != "DM") || (SN.Length == 20 && SN.Substring(10, 2) != "DM"))
                    {
                        T_R_RELATIONDATA_EXTERNAL trre = new T_R_RELATIONDATA_EXTERNAL(sfcdb, DB_TYPE_ENUM.Oracle);
                        R_RELATIONDATA_EXTERNAL rre = new R_RELATIONDATA_EXTERNAL();
                        rre.ID = trre.GetNewID(BU, sfcdb);
                        rre.SN = SN;
                        rre.PARENT = SN;
                        rre.RECEIVE_FLAG = ProductType == "RMA" ? "N" : "Y";//RMA製程設為N，其他為Y
                        rre.EDIT_EMP = EMP;
                        trre.Insert(rre, sfcdb);
                    }
                }

                T_R_SN_MAC trsm = new T_R_SN_MAC(sfcdb, DB_TYPE_ENUM.Oracle);
                R_SN_MAC rsm = new R_SN_MAC();
                T_C_SKU_Label TCSL = new T_C_SKU_Label(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(SkuNo, "ASSY", sfcdb);
                //在線列印的類型
                string OnLinePrint = "MAC,IMEI,GENCODE,SN,KPIMEI";
                for (int m = 0; m < labs.Count; m++)
                {
                    //if (OnLinePrint.IndexOf(labs[m].LABELTYPE.ToString()) >= 0)
                    if (OnLinePrint.Contains(labs[m].LABELTYPE.ToString()))
                    {
                        string subtype = labs[m].LABELTYPE.ToString();
                        rsm.ID = trsm.GetNewID(BU, sfcdb);
                        rsm.SN = SN;
                        rsm.WO = WO;
                        rsm.SUBSN_TYPE = subtype;
                        rsm.TASK_NO = TaskNo;
                        rsm.BOXSN = (strlst == "Y" ? FatherSN : SN);
                        trsm.Insert(rsm, sfcdb);
                    }
                }

                #region 縮位條碼機種

                //縮位條碼處理,該邏輯和機種在16年就不用了，先不做了，以後導入再說
                //if (t_c_control.ValueIsExist("TC0013", SkuNo, sfcdb))
                //{
                //    var listcsk = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SkuNo && t.CATEGORY_NAME == "PCB S/N").ToList().FirstOrDefault();
                //    if (listcsk != null)
                //    {
                //        string temprule = listcsk.BASETEMPLATE.ToString();
                //        strsql = $@"";
                //    }
                //    //if (tcsd.CheckNameExists(SkuNo, "PCB S/N", sfcdb))
                //    //{
                //    //}
                //}

                #endregion 縮位條碼機種
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 該功能為SN過當前工站后鎖定在某工站用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNPassLockStationAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
        }

        /// <summary>
        /// 該功能為LLT SN過當CARTON工站后鎖定在SHIPOUT工站用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LLTSNPassLockStationAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 1)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMesg);
            }

            MESStationSession LTTSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LTTSnSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "Reprint Sn" });
                throw new MESReturnMessage(ErrMesg);
            }
            string _Sn = LTTSnSession.Value.ToString();

            T_R_LLT T_RL = new T_R_LLT(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            T_R_LLT_TEST rTestRecord = new T_R_LLT_TEST(Station.SFCDB, DB_TYPE_ENUM.Oracle);

            //Row_R_LLT_TEST _RTestRecord = (Row_R_LLT_TEST)T_RL.NewRow();

            Row_R_LLT_TEST _RTestRecord = rTestRecord.GetTestTime2(_Sn, Station.SFCDB);

            _RTestRecord = rTestRecord.GetTestTime2(_Sn, Station.SFCDB);

            if (_RTestRecord != null)
            {
                var res = Station.SFCDB.ORM.Insertable(new R_SN_LOCK()
                {
                    ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_SN_LOCK"),
                    TYPE = "SN",
                    SN = _Sn,
                    LOCK_STATION = "SHIPOUT",
                    LOCK_STATUS = "1", //鎖定
                    LOCK_REASON = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153645"),// "LLT SN過CARTON鎖定在SHIPOUT",
                    LOCK_EMP = "SYSTEM",
                    LOCK_TIME = Station.GetDBDateTime(),
                    UNLOCK_REASON = "",
                    UNLOCK_EMP = "",
                    UNLOCK_TIME = null
                }).ExecuteCommandAsync();
            }

        }

        public static void SNReprintAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string ErrMesg = string.Empty;
            if (Paras.Count < 1)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(ErrMesg);
            }

            MESStationSession ReprintSnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ReprintSnSession == null)
            {
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MES00000006", new string[] { "Reprint Sn" });
                throw new MESReturnMessage(ErrMesg);
            }
            string ReprintSn = ReprintSnSession.Value.ToString();

            T_R_Label T_RL = new T_R_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_Label RL = T_RL.GetLabelBySN(ReprintSn, Station.StationName, Station.SFCDB);

            T_R_WO_REGION_DETAIL T_RWRD = new T_R_WO_REGION_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_WO_REGION_DETAIL detail = T_RWRD.GetBySn(ReprintSn, Station.SFCDB);

            T_C_SKU_Label TCLS = new T_C_SKU_Label(Station.SFCDB, Station.DBType);
            C_SKU_Label CSL = TCLS.GetLabelConfigBySN(ReprintSn, Station.StationName, Station.SFCDB);

            string path = System.AppDomain.CurrentDomain.BaseDirectory;

            T_C_Label_Type T_CLT = new T_C_Label_Type(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_Label_Type LT = T_CLT.GetByName(CSL.LABELTYPE, Station.SFCDB);

            Assembly assembly = Assembly.LoadFile(path + LT.DLL);
            if (detail != null && !string.IsNullOrEmpty(RL.LABELNAME))
            {
                LabelBase lb = (LabelBase)assembly.CreateInstance(LT.CLASS);
                lb.PAGE = 1;
                lb.ALLPAGE = 1;
                lb.LabelName = RL.LABELNAME;
                lb.FileName = RL.R_FILE_NAME;
                lb.PrintQTY = 1;
                lb.PrinterIndex = int.Parse(RL.PRINTTYPE);
                lb.Inputs.Find(t => t.StationSessionType == "SN" && t.StationSessionKey == "1").Value = ReprintSn;
                lb.Inputs.Find(t => t.StationSessionType == "WO" && t.StationSessionKey == "1").Value = detail.WORKORDERNO;
                lb.MakeLabel(Station.SFCDB);

                Station.LabelPrint.Add(lb);
                detail.USE_FLAG = (Int32.Parse(detail.USE_FLAG) + 1).ToString();
                detail.EDIT_EMP = Station.LoginUser.EMP_NO;
                detail.EDIT_TIME = DateTime.Now;
                T_RWRD.DoAfterPrint(detail, Station.SFCDB);
                Station.AddMessage("MSGCODE20180905113814", new string[] { ReprintSn }, StationMessageState.Message);
            }
            else
            {
                T_R_WO_BASE T_RWB = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                string SkuNo = T_RWB.GetWoByWoNo(detail.WORKORDERNO, Station.SFCDB).SKUNO;
                ErrMesg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180905093344", new string[] { SkuNo, Station.StationName });
                throw new MESReturnMessage(ErrMesg);
            }
        }

        public static void SNpandingVIErrorCodeAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            Dictionary<string, string> dic = (Dictionary<string, string>)Input.Value;
            string strinput = dic["SN"];
            string station_no = dic["Station_No"];
            T_C_ERROR_CODE tcec = new T_C_ERROR_CODE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_AP_TEMP tap = new T_R_AP_TEMP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_AP_TEMP ap = tap.GetMaxByStation_no(Station.SFCDB, station_no);
            if (tcec.CheckErrorCodeByErrorCode(Station.SFCDB, strinput))
            {
                if (ap.DATA4 != Station.Inputs[1].DisplayName)
                {
                    tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[1].DisplayName, strinput, Station.Inputs[2].DisplayName);
                }
                else
                {
                    tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[2].DisplayName, strinput, Station.Inputs[2].DisplayName);
                }
            }
            else
            {
                tap.SetApNextInput(Station.SFCDB, station_no, ap.DATA3 + 1, Station.Inputs[2].DisplayName, strinput, Station.Inputs[2].DisplayName);
            }
        }

        /// <summary>
        /// 掃出陪測,更新陪測SN狀態
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UpdateRotationStatusAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN snObj = (SN)snSession.Value;
            Station.SFCDB.ThrowSqlExeception = true;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            int result = t_r_silver_rotation.UpdateRotationStatus(snObj.SerialNo, "1", Station.LoginUser.EMP_NO, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "Status" }));
            }
            result = Station.SFCDB.ORM.Updateable<R_SN>().UpdateColumns(t => new R_SN { NEXT_STATION = "JOBFINISH" }).Where(r => r.ID == snObj.ID).ExecuteCommand();
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "NEXT_STATION" }));
            }
        }

        public static void UpdateWashPcbStatusAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            Panel snObj = (Panel)snSession.Value;
            Station.SFCDB.ThrowSqlExeception = true;
            T_R_IO_HEAD t_R_IO_HEAD = new T_R_IO_HEAD(Station.SFCDB, Station.DBType);
            int result = t_R_IO_HEAD.UpdateWashpcbStatus(snObj.PanelNo, "1", Station.LoginUser.EMP_NO, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "Status" }));
            }
        }

        /// <summary>
        /// 針對多連板的 SMTLOADING，當掃入的 SN 數等於輸入的連板數，即可進行後面的過站,否則加入到 已掃入SN 的集合中繼續掃
        /// LINKQTY 1
        /// SN 1
        /// SNS 1
        /// EXTSCAN 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CollectInputSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession LinkQtySession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (LinkQtySession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string LinkQty = LinkQtySession.Value.ToString();

            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SnSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string Sn = SnSession.Value.ToString();

            MESStationSession SnCollectionSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (SnCollectionSession == null)
            {
                SnCollectionSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = new List<string>() };
                Station.StationSession.Add(SnCollectionSession);
            }

            MESStationSession ExtScanSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (ExtScanSession == null)
            {
                ExtScanSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, SessionKey = Paras[3].SESSION_KEY, Value = LinkQty };
                Station.StationSession.Add(ExtScanSession);
            }

            //檢查在這一批中是否已經掃過
            if (((List<string>)SnCollectionSession.Value).Any(t => t == Sn))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181013190913", new string[] { Sn }));
            }

            //如果掃的不夠的話，提示出異常，繼續掃

            ((List<string>)SnCollectionSession.Value).Add(Sn);
            ExtScanSession.Value = Int32.Parse(LinkQty) - ((List<string>)SnCollectionSession.Value).Count;
            if (((List<string>)SnCollectionSession.Value).Count < Int32.Parse(LinkQty))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181013191113", new string[] { ExtScanSession.Value.ToString() }));
            }
        }

        /// <summary>
        /// 产品入 B29
        /// WO 1
        /// SN 1
        ///      B29
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ToB29Action(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            T_R_SN TRS = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_MRB TRM = new T_R_MRB(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_R_MRB_GT TRMG = new T_R_MRB_GT(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_SAP_STATION_MAP TCSAM = new T_C_SAP_STATION_MAP(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string CompletedFlag = string.Empty;
            string OriginalNextStation = string.Empty;
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            WorkOrder Wo = null;
            if (WoSession.Value is WorkOrder)
            {
                Wo = (WorkOrder)WoSession.Value;
            }
            else
            {
                Wo = new WorkOrder();
                Wo.Init(WoSession.Value.ToString(), Station.SFCDB);
            }

            MESStationSession Snsession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Snsession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            SN Sn = null;
            if (Snsession.Value is SN)
            {
                Sn = (SN)Snsession.Value;
            }
            else
            {
                Sn = new SN();
                Sn.Load(Snsession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            string ToStorage = Paras[2].VALUE;

            //異常現象
            //入B29提示completeflag為1，不能作業，手動更改為0后，重新入B29,再出B29時在r_sn 表裏產生兩條記錄
            //分析
            //兩條記錄工單不同，原工單validflag為0，重工工單validflag為1
            //管控complete flag的原因，控制抛帳
            //預期目標
            //R_sn只能有一條SN記錄
            //入重工工單后，掃維修，將valid_flag為0記錄掃入維修，同時將該條記錄刪除
            R_SN RSN = TRS.GetSN(Sn.SerialNo, Station.SFCDB);
            CompletedFlag = RSN.COMPLETED_FLAG;
            RSN.COMPLETED_FLAG = "1";
            RSN.COMPLETED_TIME = TRS.GetDBDateTime(Station.SFCDB);
            OriginalNextStation = RSN.NEXT_STATION;
            RSN.NEXT_STATION = Station.StationName;
            RSN.EDIT_EMP = Station.LoginUser.EMP_NO;
            RSN.EDIT_TIME = TRS.GetDBDateTime(Station.SFCDB);
            TRS.Update(RSN, Station.SFCDB);
            TRS.RecordPassStationDetail(Sn.SerialNo, Station.Line, Station.StationName, Station.StationName, Station.BU, Station.SFCDB);
            if (RSN.COMPLETED_FLAG != "1")
            {
                //RSN.COMPLETED_FLAG = "1";
                //TRS.Update(RSN, Station.SFCDB);
                TRWB.UpdateFinishQty(Wo.WorkorderNo, 1, Station.SFCDB);//complete flag為1，不執行此步驟
            }
            C_SAP_STATION_MAP map = TCSAM.GetSAPStationMapBySkuOrderBySAPCodeASC(Wo.SkuNO, Station.SFCDB).LastOrDefault();
            if (map != null)
            {
                //if (RSN.COMPLETED_FLAG!="1")
                //{
                //    RSN.COMPLETED_FLAG = "1";
                //    TRS.Update(RSN, Station.SFCDB);
                //    TRWB.UpdateFinishQty(Wo.WorkorderNo, 1, Station.SFCDB);//complete flag為1，不執行此步驟
                //}

                R_MRB MRB = new R_MRB();
                MRB.ID = TRM.GetNewID(Station.BU, Station.SFCDB);
                MRB.SN = Sn.SerialNo;
                MRB.WORKORDERNO = Wo.WorkorderNo;
                MRB.NEXT_STATION = OriginalNextStation;
                MRB.SKUNO = Wo.SkuNO;
                MRB.FROM_STORAGE = Wo.WorkorderNo;
                MRB.TO_STORAGE = ToStorage;
                MRB.CREATE_EMP = Station.LoginUser.EMP_NO;
                MRB.CREATE_TIME = TRM.GetDBDateTime(Station.SFCDB);
                MRB.MRB_FLAG = "1";
                MRB.SAP_FLAG = "0";
                MRB.EDIT_EMP = Station.LoginUser.EMP_NO;
                MRB.EDIT_TIME = TRM.GetDBDateTime(Station.SFCDB);
                TRM.Insert(MRB, Station.SFCDB);

                R_MRB_GT RMG = null;

                List<R_MRB_GT> RMGS = TRMG.GetByWoAndSapCode(Wo.WorkorderNo, map.SAP_STATION_CODE, CompletedFlag, Station.SFCDB);
                //如果有還沒拋帳的記錄，那麼就直接更新數量
                if (RMGS.Any(t => t.TO_STORAGE == ToStorage && t.SAP_FLAG == "0" && t.ZCPP_FLAG == "0" && t.SAP_MESSAGE == null))
                {
                    RMG = RMGS.Find(t => t.TO_STORAGE == ToStorage);
                    RMG.TOTAL_QTY += 1;
                    RMG.EDIT_EMP = Station.LoginUser.EMP_NO;
                    RMG.EDIT_TIME = TRM.GetDBDateTime(Station.SFCDB);
                    TRMG.Update(RMG, Station.SFCDB);
                }
                else
                {
                    RMG = new R_MRB_GT();
                    RMG.ID = TRMG.GetNewID(Station.BU, Station.SFCDB);
                    RMG.WORKORDERNO = Wo.WorkorderNo;
                    RMG.SAP_STATION_CODE = map.SAP_STATION_CODE;
                    RMG.FROM_STORAGE = Wo.WorkorderNo;
                    RMG.TO_STORAGE = ToStorage;
                    RMG.TOTAL_QTY = 1;
                    RMG.CONFIRMED_FLAG = CompletedFlag;
                    RMG.ZCPP_FLAG = "0";
                    RMG.SAP_FLAG = "0";
                    RMG.SKUNO = Wo.SkuNO;
                    RMG.EDIT_EMP = Station.LoginUser.EMP_NO;
                    RMG.EDIT_TIME = TRMG.GetDBDateTime(Station.SFCDB);
                    TRMG.Insert(RMG, Station.SFCDB);
                }
            }
        }

        public static void ReworkAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            T_R_SN TRSN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);

            #region 讀取參數值

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            WorkOrder Wo = null;
            if (WoSession.Value is WorkOrder)
            {
                Wo = (WorkOrder)WoSession.Value;
            }
            else
            {
                Wo = new WorkOrder();
                Wo.Init(WoSession.Value.ToString(), Station.SFCDB);
            }

            MESStationSession ToStationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (ToStationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string ToStation = ToStationSession.Value.ToString();

            MESStationSession Snsession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (Snsession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            SN Sn = null;
            if (Snsession.Value is SN)
            {
                Sn = (SN)Snsession.Value;
            }
            else
            {
                Sn = new SN();
                Sn.Load(Snsession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            #endregion 讀取參數值

            #region 判断SN状态以及同一个包装的状态，如果有已经出货则不允许重工

            if (Sn.ShippedFlag == "1")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808103413", new string[] { Sn.SerialNo }));
            }

            R_PACKING packing = TRP.GetPackingObjectBySN(Sn.SerialNo, Station.SFCDB);
            if (packing != null)
            {
                List<R_PACKING> ChildPacks = new List<R_PACKING>();
                if (packing.PARENT_PACK_ID != null)
                {
                    R_PACKING ParentPacking = TRP.GetPackingByID(packing.PARENT_PACK_ID, Station.SFCDB);
                    if (ParentPacking != null)
                    {
                        ChildPacks = TRP.GetChildPacks(ParentPacking.PACK_NO, Station.SFCDB);
                    }
                    else
                    {
                        ChildPacks.Add(packing);
                    }
                }

                foreach (R_PACKING rp in ChildPacks)
                {
                    List<R_SN> SnList = new List<R_SN>();
                    TRP.GetSnListByPackNo(rp.PACK_NO, ref SnList, Station.SFCDB);
                    foreach (R_SN rs in SnList)
                    {
                        if (rs.SHIPPED_FLAG == "1")
                        {
                            if (rp.PACK_NO == packing.PACK_NO)
                            {
                                // 該 包裝單位 內有產品已經出貨，不允許打重工
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190228154806", new string[] { packing.PACK_NO }));
                            }
                            else
                            {
                                //最大包裝單位內有已經出貨的產品，請將此包裝單位退出當前最大包裝單位后再打重工，并記得重新打印最大包裝單位的label
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190228154948", new string[] { rp.PACK_NO }));
                            }
                        }
                    }
                }
            }

            #endregion 判断SN状态以及同一个包装的状态，如果有已经出货则不允许重工

            #region 判斷重工工單的機種與產品的原機種是否一致

            if (Sn.SkuNo != Wo.SkuNO)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190228155329", new string[] { Wo.SkuNO, Sn.SkuNo }));
            }

            #endregion 判斷重工工單的機種與產品的原機種是否一致

            #region 判斷返回站位與當前站位的順序，防止往後打

            List<C_ROUTE_DETAIL> RouteDetails = Station.SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == Sn.RouteID).OrderBy(t => t.SEQ_NO).ToList();
            C_ROUTE_DETAIL NextStationDetail = RouteDetails.Find(t => t.STATION_NAME == Sn.NextStation);
            C_ROUTE_DETAIL ToStationDetail = RouteDetails.Find(t => t.STATION_NAME == ToStation);
            if (NextStationDetail == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190228155715"));
            }
            if (ToStationDetail == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190228155824"));
            }
            if (NextStationDetail.SEQ_NO <= ToStationDetail.SEQ_NO)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190228155942"));
            }

            #endregion 判斷返回站位與當前站位的順序，防止往後打

            R_SN RSN = TRSN.GetSN(Sn.SerialNo, Station.SFCDB);
            string currentStation = RouteDetails.FindLast(t => t.SEQ_NO < ToStationDetail.SEQ_NO).STATION_NAME;

            #region 產品還沒有被包裝的

            if (Sn.PackedFlag == "0")
            {
                RSN.EDIT_TIME = TRSN.GetDBDateTime(Station.SFCDB);
                RSN.EDIT_EMP = Station.LoginUser.EMP_NO;
                RSN.CURRENT_STATION = currentStation;
                RSN.NEXT_STATION = ToStationDetail.STATION_NAME;
                RSN.PRODUCT_STATUS = "REWORK";
                RSN.REWORK_COUNT = RSN.REWORK_COUNT + 1;
                if (RSN.CURRENT_STATION.Equals("STOCKIN"))
                {
                    RSN.COMPLETED_FLAG = "0";
                    RSN.COMPLETED_TIME = null;
                }
                Station.SFCDB.ORM.Updateable<R_SN>(RSN).Where(t => t.ID == RSN.ID).ExecuteCommand();
            }

            #endregion 產品還沒有被包裝的

            #region 產品已經包裝過

            else if (Sn.PackedFlag == "1")
            {
                //先找到SN所在包裝單位的最大單位
                R_PACKING CurrentSnPacking = TRP.GetPackingObjectBySN(RSN.SN, Station.SFCDB);
                R_PACKING ParentPacking = TRP.GetPackingByID(CurrentSnPacking.PARENT_PACK_ID, Station.SFCDB);
                List<R_PACKING> ChildPacks = new List<R_PACKING>();
                if (ParentPacking == null)
                {
                    ChildPacks.Add(CurrentSnPacking);
                }
                else
                {
                    ChildPacks = TRP.GetChildPacks(ParentPacking.PACK_NO, Station.SFCDB);
                }
                //如果打到待入庫站位
                if (ToStationDetail.STATION_NAME.Equals("CBS"))
                {
                    List<R_SN> RSNS = new List<R_SN>();
                    foreach (R_PACKING rp in ChildPacks)
                    {
                        TRP.GetSnListByPackNo(rp.PACK_NO, ref RSNS, Station.SFCDB);
                        foreach (R_SN rs in RSNS)
                        {
                            rs.COMPLETED_FLAG = "0";
                            rs.COMPLETED_TIME = null;
                            rs.EDIT_TIME = TRSN.GetDBDateTime(Station.SFCDB);
                            rs.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rs.CURRENT_STATION = currentStation;
                            rs.NEXT_STATION = ToStationDetail.STATION_NAME;
                            rs.PRODUCT_STATUS = "REWORK";
                            rs.REWORK_COUNT = RSN.REWORK_COUNT + 1;
                            Station.SFCDB.ORM.Updateable<R_SN>(RSN).Where(t => t.ID == rs.ID).ExecuteCommand();
                            TRSN.RecordPassStationDetail(rs.SN, Station.Line, "REWORK", "REWORK", Station.BU, Station.SFCDB);
                        }
                    }
                }
                //如果打到待 Pallet 站位
                else if (ToStationDetail.STATION_NAME.Equals("PALLET"))
                {
                    List<R_SN> RSNS = new List<R_SN>();
                    foreach (R_PACKING rp in ChildPacks)
                    {
                        //解除Pallet與其下面的Carton的關聯
                        rp.PARENT_PACK_ID = null;
                        rp.EDIT_TIME = TRP.GetDBDateTime(Station.SFCDB);
                        rp.EDIT_EMP = Station.LoginUser.EMP_NO;
                        Station.SFCDB.ORM.Updateable<R_PACKING>(rp).Where(t => t.ID == rp.ID).ExecuteCommand();
                        TRP.GetSnListByPackNo(rp.PACK_NO, ref RSNS, Station.SFCDB);
                        //將所有SN打回到待Pallet狀態
                        foreach (R_SN rs in RSNS)
                        {
                            rs.COMPLETED_FLAG = "0";
                            rs.COMPLETED_TIME = null;
                            rs.EDIT_TIME = TRSN.GetDBDateTime(Station.SFCDB);
                            rs.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rs.CURRENT_STATION = currentStation;
                            rs.NEXT_STATION = ToStationDetail.STATION_NAME;
                            rs.PRODUCT_STATUS = "REWORK";
                            rs.REWORK_COUNT = RSN.REWORK_COUNT + 1;
                            Station.SFCDB.ORM.Updateable<R_SN>(RSN).Where(t => t.ID == rs.ID).ExecuteCommand();
                            TRSN.RecordPassStationDetail(rs.SN, Station.Line, "REWORK", "REWORK", Station.BU, Station.SFCDB);
                        }
                    }
                    if (ParentPacking != null)
                    {
                        //刪除Pallet信息
                        Station.SFCDB.ORM.Deleteable<R_PACKING>().Where(t => t.ID == ParentPacking.ID).ExecuteCommand();
                    }
                }
                //如果打到待Carton站位
                else if (ToStationDetail.STATION_NAME.Equals("CARTON"))
                {
                    //根據當前SN獲取所有在Pallet裡面的所有Carton
                    List<R_SN> RSNS = new List<R_SN>();
                    foreach (R_PACKING rp in ChildPacks)
                    {
                        TRP.GetSnListByPackNo(rp.PACK_NO, ref RSNS, Station.SFCDB);
                        //所有的SN打回到待Carton站位
                        foreach (R_SN rs in RSNS)
                        {
                            rs.COMPLETED_FLAG = "0";
                            rs.COMPLETED_TIME = null;
                            rs.PACKED_FLAG = "0";
                            rs.PACKDATE = null;
                            rs.EDIT_TIME = TRSN.GetDBDateTime(Station.SFCDB);
                            rs.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rs.CURRENT_STATION = currentStation;
                            rs.NEXT_STATION = ToStationDetail.STATION_NAME;
                            rs.PRODUCT_STATUS = "REWORK";
                            rs.REWORK_COUNT = RSN.REWORK_COUNT + 1;
                            Station.SFCDB.ORM.Updateable<R_SN>(RSN).Where(t => t.ID == rs.ID).ExecuteCommand();
                            TRSN.RecordPassStationDetail(rs.SN, Station.Line, "REWORK", "REWORK", Station.BU, Station.SFCDB);
                        }

                        //所有SN解除與Carton的關聯關係
                        Station.SFCDB.ORM.Deleteable<R_SN_PACKING>().Where(t => t.PACK_ID == rp.ID).ExecuteCommand();

                        //刪除所有的Carton信息
                        Station.SFCDB.ORM.Deleteable<R_PACKING>().Where(t => t.ID == rp.ID).ExecuteCommand();

                        if (ParentPacking != null)
                        {
                            //刪除Pallet信息
                            Station.SFCDB.ORM.Deleteable<R_PACKING>().Where(t => t.ID == ParentPacking.ID).ExecuteCommand();
                        }
                    }
                }
            }

            #endregion 產品已經包裝過

            #region 如果打回到Link站位及其之前的則清除相應的keypart資料

            C_ROUTE_DETAIL LinkStationDetail = RouteDetails.Find(t => t.STATION_NAME == "LINK");
            if (LinkStationDetail != null)
            {
                if (ToStationDetail.SEQ_NO <= LinkStationDetail.SEQ_NO)
                {
                    Station.SFCDB.ORM.Updateable<R_SN_KEYPART_DETAIL>().UpdateColumns(t => t.VALID == "0").Where(t => t.SN == Sn.SerialNo).ExecuteCommand();
                }
            }

            #endregion 如果打回到Link站位及其之前的則清除相應的keypart資料
        }

        public static void FindSNFromOldDB(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string sn = SNSession.Value.ToString();
            //if (sn.StartsWith("HSH"))
            //{
            //    sn = sn.Replace("HSH", "XSH");
            //}

            StringBuilder ConnectionString = new StringBuilder();
            ConnectionString.Append("Data Source=").Append("10.117.2.51").Append(",").Append("3000").Append(";")
                .Append("Initial Catalog=").Append("eerp").Append(";")
                .Append("User ID=").Append("sa").Append(";")
                .Append("Password=").Append("mirrortest").Append(";");
            SqlSugar.SqlSugarClient SQLDB = new SqlSugar.SqlSugarClient(new SqlSugar.ConnectionConfig()
            {
                ConnectionString = ConnectionString.ToString(),
                DbType = SqlSugar.DbType.SqlServer,
                IsAutoCloseConnection = false,
                InitKeyType = SqlSugar.InitKeyType.Attribute
            });

            OleExec SFCDB = Station.SFCDB;

            if (!SFCDB.ORM.Queryable<R_SN>().Any(t => t.SN == sn))
            {
                var sql = $@"SELECT '{sn}' id,'{sn}' sn,b.skuno,a.workorderno,'SH50' plant,b.routeid route_id,started STARTED_flag,
                a.startdate start_time,packed packed_flag,packdate packdate,completed completed_flag, completedate completed_time,shipped shipped_flag,
                shipdate shipdate,repairheld repair_failed_flag, currentevent current_station,nextevent next_station,'' kp_list_id,'' po_no,'' cust_order_no,
                b.skuno cust_pn,'' boxsn,'0' scraped_flag,'' scraped_time,productstatus product_status, Reworkcount rework_count,'1' valid_flag,'' stock_status,
                '' stock_in_time,a.lasteditby edit_emp, a.lasteditdt edit_time
            FROM dbo.mfworkstatus a, dbo.mfworkorder b WHERE sysserialno ='{sn.Replace("FXS", "XXS")}' AND a.workorderno = b.workorderno";
                var dt = SQLDB.SqlQueryable<object>(sql).ToDataTable();
                if (dt.Rows.Count == 0)
                {
                    sql = $@"SELECT '{sn}' id,'{sn}' sn,b.skuno,a.workorderno,'SH50' plant,b.routeid route_id,started STARTED_flag,
                a.startdate start_time,packed packed_flag,packdate packdate,completed completed_flag, completedate completed_time,shipped shipped_flag,
                shipdate shipdate,repairheld repair_failed_flag, currentevent current_station,nextevent next_station,'' kp_list_id,'' po_no,'' cust_order_no,
                b.skuno cust_pn,'' boxsn,'0' scraped_flag,'' scraped_time,productstatus product_status, Reworkcount rework_count,'1' valid_flag,'' stock_status,
                '' stock_in_time,a.lasteditby edit_emp, a.lasteditdt edit_time
            FROM dbo.mfworkstatus a, dbo.mfworkorder b WHERE (sysserialno LIKE '%{sn}' or sysserialno like '%{sn.Substring(3)}') AND a.workorderno = b.workorderno";

                    dt = SQLDB.SqlQueryable<object>(sql).OrderBy("edit_time desc").ToDataTable();
                }

                string NextStation = string.Empty;
                string CurrentStation = string.Empty;
                if (dt.Rows.Count == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { sn }));
                }
                foreach (DataRow dr in dt.Rows)
                {
                    string wo = dr["WORKORDERNO"].ToString().Trim();
                    string skuno = dr["SKUNO"].ToString().Trim();
                    R_WO_BASE RWB = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == wo).ToList().FirstOrDefault();
                    R_SKU_ROUTE RSR = SFCDB.ORM.Queryable<R_SKU_ROUTE, C_SKU>((rsr, cs) => rsr.SKU_ID == cs.ID).Where((rsr, cs) => cs.SKUNO == skuno).Select((rsr, cs) => rsr).ToList().FirstOrDefault();
                    if (RWB == null)
                    {
                        //搬工單信息
                        sql = $@"SELECT workorderno id,workorderno,factoryid plant,releaseddate release_date,workorderdate download_date,productiontype production_type,
                            workordertype wo_type,skuno,REPLACE(skuversion,'+','') as sku_ver,skuname sku_name,skudesc sku_desc,custpartno cust_pn,
                            closed closed_flag,closedate close_date,workorderqty workorder_qty,finishedqty finished_qty,lasteditby edit_emp,GETDATE() edit_time
                            FROM dbo.mfworkorder WHERE workorderno='{wo}'";
                        DataTable dt_temp = SQLDB.SqlQueryable<object>(sql).ToDataTable();
                        if (dt_temp.Rows.Count > 0)
                        {
                            DataRow dr_temp = dt_temp.Rows[0];
                            string skuno_temp = dr_temp["skuno"].ToString();
                            string skuver_temp = dr_temp["sku_ver"].ToString();
                            C_SKU CSKU = SFCDB.ORM.Queryable<C_SKU>().Where(t => t.SKUNO == skuno_temp && t.VERSION == skuver_temp).ToList().FirstOrDefault();
                            C_ROUTE_DETAIL FirstStation = null;
                            if (RSR != null)
                            {
                                FirstStation = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == RSR.ROUTE_ID).OrderBy(t => t.SEQ_NO).ToList().FirstOrDefault();
                            }

                            SFCDB.ORM.Insertable<R_WO_BASE>(new R_WO_BASE()
                            {
                                ID = dr_temp["ID"].ToString() + DateTime.Now.ToString("yyyyMMddhhmmss"),
                                WORKORDERNO = dr_temp["WORKORDERNO"].ToString().Trim(),
                                PLANT = dr_temp["PLANT"].ToString().Trim(),
                                RELEASE_DATE = dr_temp["RELEASE_DATE"].ToString().Length > 0 ? DateTime.Parse(dr_temp["RELEASE_DATE"].ToString()) : new DateTime?(),
                                DOWNLOAD_DATE = dr_temp["DOWNLOAD_DATE"].ToString().Length > 0 ? DateTime.Parse(dr_temp["DOWNLOAD_DATE"].ToString()) : new DateTime?(),
                                PRODUCTION_TYPE = dr_temp["PRODUCTION_TYPE"].ToString().Trim(),
                                WO_TYPE = dr_temp["WO_TYPE"].ToString().Trim(),
                                SKUNO = dr_temp["SKUNO"].ToString().Trim(),
                                SKU_VER = dr_temp["SKU_VER"].ToString().Trim(),
                                SKU_SERIES = CSKU == null ? "" : CSKU.C_SERIES_ID,
                                SKU_NAME = dr_temp["SKU_NAME"].ToString().Trim(),
                                SKU_DESC = dr_temp["SKU_DESC"].ToString().Trim(),
                                CUST_PN = dr_temp["CUST_PN"].ToString().Trim(),
                                CUST_PN_VER = "",
                                CUSTOMER_NAME = "",
                                ROUTE_ID = RSR == null ? "" : RSR.ROUTE_ID,
                                START_STATION = FirstStation == null ? "" : FirstStation.STATION_NAME,
                                KP_LIST_ID = "",
                                CLOSED_FLAG = dr_temp["CLOSED_FLAG"].ToString().Trim() == "False" ? "0" : "1",
                                CLOSE_DATE = dr_temp["CLOSE_DATE"].ToString().Length > 0 ? DateTime.Parse(dr_temp["CLOSE_DATE"].ToString()) : new DateTime?(),
                                WORKORDER_QTY = double.Parse(dr_temp["WORKORDER_QTY"].ToString()),
                                INPUT_QTY = 0,
                                FINISHED_QTY = double.Parse(dr_temp["FINISHED_QTY"].ToString().Trim()),
                                SCRAPED_QTY = 0,
                                STOCK_LOCATION = "F101",
                                PO_NO = "",
                                CUST_ORDER_NO = "",
                                ROHS = "R6",
                                EDIT_EMP = dr_temp["EDIT_EMP"].ToString().Trim(),
                                EDIT_TIME = dr_temp["EDIT_TIME"].ToString().Length > 0 ? DateTime.Parse(dr_temp["EDIT_TIME"].ToString()) : new DateTime?()
                            }).ExecuteCommand();
                        }
                    }

                    try
                    {
                        NextStation = dr["NEXT_STATION"].ToString().Trim();
                        CurrentStation = dr["CURRENT_STATION"].ToString().Trim();
                        if (NextStation.Equals("MCEBU-CBS")) { NextStation = "SHIPOUT"; }   //舊系統出貨對應新系統SHIPFINISH
                        if (NextStation.Equals("VI")) { NextStation = "PTH"; }
                        if (NextStation.Equals("PACKING") || NextStation.Equals("MODEL-PACKING")) { NextStation = "CARTON"; }
                        if (NextStation.Equals("RoBAT-S1")) { NextStation = "ICT_S1"; }
                        if (NextStation.Equals("PCB2C")) { NextStation = "STOCKIN"; }

                        string route_id = RWB == null ? (RSR == null ? dr["ROUTE_ID"].ToString().Trim() : RSR.ROUTE_ID) : RWB.ROUTE_ID;
                        C_ROUTE_DETAIL NextCrd = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == route_id && t.STATION_NAME == NextStation).ToList().FirstOrDefault();
                        if (NextCrd != null)
                        {
                            C_ROUTE_DETAIL CurrentCrd = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(t => t.ROUTE_ID == route_id && t.SEQ_NO < NextCrd.SEQ_NO).OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                            if (CurrentCrd != null)
                            {
                                CurrentStation = CurrentCrd.STATION_NAME;
                            }
                        }

                        SFCDB.ORM.Insertable<R_SN>(new R_SN()
                        {
                            ID = dr["ID"].ToString() + DateTime.Now.ToString("yyyyMMddhhmmss"),
                            SN = dr["SN"].ToString(),
                            SKUNO = dr["SKUNO"].ToString(),
                            WORKORDERNO = dr["WORKORDERNO"].ToString().Trim(),
                            PLANT = dr["PLANT"].ToString(),
                            ROUTE_ID = RWB == null ? (RSR == null ? dr["ROUTE_ID"].ToString().Trim() : RSR.ROUTE_ID) : RWB.ROUTE_ID,
                            STARTED_FLAG = dr["STARTED_FLAG"].ToString().Equals("True") ? "1" : "0",
                            START_TIME = dr["START_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["START_TIME"].ToString()) : new DateTime?(),
                            PACKED_FLAG = dr["PACKED_FLAG"].ToString().Equals("True") ? "1" : "0",
                            PACKDATE = dr["PACKDATE"].ToString().Length > 0 ? DateTime.Parse(dr["PACKDATE"].ToString()) : new DateTime?(),
                            COMPLETED_FLAG = dr["COMPLETED_FLAG"].ToString().Equals("True") ? "1" : "0",
                            COMPLETED_TIME = dr["COMPLETED_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["COMPLETED_TIME"].ToString()) : new DateTime?(),
                            SHIPPED_FLAG = dr["SHIPPED_FLAG"].ToString().Equals("True") ? "1" : "0",
                            SHIPDATE = dr["SHIPDATE"].ToString().Length > 0 ? DateTime.Parse(dr["SHIPDATE"].ToString()) : new DateTime?(),
                            REPAIR_FAILED_FLAG = dr["REPAIR_FAILED_FLAG"].ToString().Equals("True") ? "1" : "0",

                            CURRENT_STATION = CurrentStation,
                            NEXT_STATION = NextStation,
                            KP_LIST_ID = dr["KP_LIST_ID"].ToString(),
                            PO_NO = dr["PO_NO"].ToString(),
                            CUST_ORDER_NO = dr["CUST_ORDER_NO"].ToString(),
                            CUST_PN = dr["CUST_PN"].ToString(),
                            BOXSN = dr["BOXSN"].ToString(),
                            SCRAPED_FLAG = dr["SCRAPED_FLAG"].ToString(),
                            SCRAPED_TIME = dr["SCRAPED_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["SCRAPED_TIME"].ToString()) : new DateTime?(),
                            PRODUCT_STATUS = dr["PRODUCT_STATUS"].ToString(),
                            REWORK_COUNT = double.Parse(dr["REWORK_COUNT"].ToString()),
                            VALID_FLAG = dr["VALID_FLAG"].ToString(),
                            STOCK_STATUS = dr["STOCK_STATUS"].ToString().Equals("True") ? "1" : "0",
                            STOCK_IN_TIME = dr["STOCK_IN_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["STOCK_IN_TIME"].ToString()) : new DateTime?(),
                            EDIT_EMP = dr["EDIT_EMP"].ToString(),
                            EDIT_TIME = dr["EDIT_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["EDIT_TIME"].ToString()) : new DateTime?()
                        }).ExecuteCommand();

                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }
                //搬出貨資料
                sql = $@"SELECT distinct p_sn id,P_SN sn,b.skuno skuno,a.DN_NO,a.DN_ITEM_NO dn_line,a.Container_TIME shipdate,p_sn createby
                FROM dbo.H_SHIPPING_DETAIL a, dbo.mfsysevent b
                WHERE (P_SN = '{sn}' or p_sn like '{sn.Substring(3)}') AND a.p_sn = b.sysserialno ";
                dt = SQLDB.SqlQueryable<object>(sql).ToDataTable();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        SFCDB.ORM.Insertable<R_SHIP_DETAIL>(new R_SHIP_DETAIL()
                        {
                            ID = (dr["ID"].ToString().Trim() + DateTime.Now.ToString("yyyyMMddhhmmss")).Trim(),
                            SN = dr["SN"].ToString().Trim(),
                            SKUNO = dr["SKUNO"].ToString(),
                            DN_NO = dr["DN_NO"].ToString(),
                            DN_LINE = dr["DN_LINE"].ToString(),
                            SHIPDATE = dr["SHIPDATE"].ToString().Length > 0 ? DateTime.Parse(dr["SHIPDATE"].ToString()) : new DateTime?(),
                            CREATEBY = dr["CREATEBY"].ToString().Trim()
                        }).ExecuteCommand();
                        break;
                    }
                    catch (Exception)
                    {
                        break;
                    }
                }

                //搬DOM資料
                sql = $@"SELECT '{sn}' id,'{sn}' r_sn_id,'{sn}' sn,'PASS' 'STATE','' TEGROUP,station TESTATION,'DOM' MESSTATION,lasteditby device,
                            testdate starttime,testdate endtime,GETDATE() edit_time,'A0225204' edit_emp
                            FROM mfautotestdom WHERE sysserialno='{sn}' or sysserialno='{sn.Replace("FOX", "NWG")}' or sysserialno like '%{sn.Substring(3)}'";
                dt = SQLDB.SqlQueryable<object>(sql).ToDataTable();
                if (!SFCDB.ORM.Queryable<R_TEST_RECORD>().Any(t => t.SN == sn && t.MESSTATION == "DOM" && t.STATE == "PASS"))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        try
                        {
                            SFCDB.ORM.Insertable<R_TEST_RECORD>(new R_TEST_RECORD()
                            {
                                ID = dr["ID"].ToString().Trim() + DateTime.Now.ToString("yyyyMMddhhmmss"),
                                R_SN_ID = dr["R_SN_ID"].ToString().Trim(),
                                SN = dr["SN"].ToString().Trim(),
                                STATE = dr["STATE"].ToString(),
                                TEGROUP = "",
                                TESTATION = dr["TESTATION"].ToString(),
                                MESSTATION = "DOM",
                                DEVICE = dr["DEVICE"].ToString(),
                                STARTTIME = dr["STARTTIME"].ToString().Length > 0 ? DateTime.Parse(dr["STARTTIME"].ToString()) : new DateTime?(),
                                ENDTIME = dr["ENDTIME"].ToString().Length > 0 ? DateTime.Parse(dr["ENDTIME"].ToString()) : new DateTime?(),
                                DETAILTABLE = "move from old DB",
                                TESTINFO = "",
                                EDIT_TIME = dr["EDIT_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["EDIT_TIME"].ToString()) : new DateTime?(),
                                EDIT_EMP = "A0225204"
                            }).ExecuteCommand();

                            break;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                }

                //搬測試資料
                sql = $@"SELECT '{sn}' + CONVERT(varchar(100), testdate, 112) + REPLACE(CONVERT(varchar(100), testdate, 24), ':', '') ID,'{sn}' R_SN_ID,'{sn}' sn,
					CASE status WHEN 'P' THEN 'PASS' ELSE 'FAIL' END AS STATE,'' tegroup,station testation,station messtation,'' device,CONVERT(varchar(100), testdate, 20) starttime,
					CONVERT(varchar(100), testdate, 20) endtime,'move from old DB' detailtable,testinfo,CONVERT(varchar(100), lasteditdt, 20) edit_time,'A0225204' Edit_emp
					FROM dbo.mfautotestrecord WHERE sysserialno='{sn}' or sysserialno='{sn.Replace("FOX", "NWG")}' or sysserialno like '%{sn.Substring(3)}'";
                dt = SQLDB.SqlQueryable<object>(sql).ToDataTable();

                foreach (DataRow dr in dt.Rows)
                {
                    try
                    {
                        SFCDB.ORM.Insertable<R_TEST_RECORD>(new R_TEST_RECORD()
                        {
                            ID = dr["ID"].ToString().Trim(),
                            R_SN_ID = dr["R_SN_ID"].ToString().Trim(),
                            SN = dr["SN"].ToString().Trim(),
                            STATE = dr["STATE"].ToString(),
                            TEGROUP = dr["TEGROUP"].ToString(),
                            TESTATION = dr["TESTATION"].ToString(),
                            MESSTATION = dr["MESSTATION"].ToString(),
                            DEVICE = dr["DEVICE"].ToString(),
                            STARTTIME = dr["STARTTIME"].ToString().Length > 0 ? DateTime.Parse(dr["STARTTIME"].ToString()) : new DateTime?(),
                            ENDTIME = dr["ENDTIME"].ToString().Length > 0 ? DateTime.Parse(dr["ENDTIME"].ToString()) : new DateTime?(),
                            DETAILTABLE = dr["DETAILTABLE"].ToString(),
                            TESTINFO = dr["TESTINFO"].ToString(),
                            EDIT_TIME = dr["EDIT_TIME"].ToString().Length > 0 ? DateTime.Parse(dr["EDIT_TIME"].ToString()) : new DateTime?(),
                            EDIT_EMP = dr["EDIT_EMP"].ToString(),
                        }).ExecuteCommand();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            //防止作業員多次掃描導致數據表中的資料錯誤
            if (SFCDB.ORM.Queryable<R_SN>().Any(t => t.SN == sn && t.VALID_FLAG == "1" && t.WORKORDERNO == "RMA"))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000237", new string[] { sn }));
            }
        }

        /// <summary>
        /// 2DX/5DX/NORMAL_STATION抽測ACTION
        /// add by hgb 2019.06.0601
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void Sfc2dx5dxNormal_stationAction(MESStationBase Station, MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SnSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                string SN = SnSession.Value.ToString().Trim().ToUpper();

                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                string WO = WOSession.Value.ToString().Trim().ToUpper();

                MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (SkuSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                string SKUNO = SkuSession.Value.ToString().Trim().ToUpper();

                Station.SFCDB.BeginTrain();
                T_R_2DX5DX_SAMPLING_WO t_r_2dx5dx_sampling_wo = new T_R_2DX5DX_SAMPLING_WO(Station.SFCDB, Station.DBType);
                t_r_2dx5dx_sampling_wo.Sfc2dx5dxNormalStationSampling(SN, WO, SKUNO, Station.StationName, Station.Line, "2DX", Station.SFCDB);
                t_r_2dx5dx_sampling_wo.Sfc2dx5dxNormalStationSampling(SN, WO, SKUNO, Station.StationName, Station.Line, "5DX", Station.SFCDB);
                t_r_2dx5dx_sampling_wo.Sfc2dx5dxNormalStationSampling(SN, WO, SKUNO, Station.StationName, Station.Line, "NORMAL_STATION", Station.SFCDB);
                Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                Station.SFCDB.RollbackTrain();
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
            }
        }

        /// <summary>
        /// HWT OBA 掃描SN
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTOBAScanSNAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionStatus = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStatus == null || sessionStatus.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionRemark = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);

            string status = sessionStatus.Value.ToString().ToUpper();
            SN objSN = (SN)sessionSN.Value;
            OleExec sfcdb = Station.SFCDB;
            MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(sfcdb, Station.DBType);

            if (TROT.IsExistByTypeAndValue(sfcdb, "SN", objSN.SerialNo))
            {
                //throw new Exception(objSN.SerialNo + " 已經掃描，請不要重複掃描");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153744", new string[] { objSN.SerialNo }));
            }

            MESDataObject.Module.HWT.R_OBA_TEMP tempObj = new MESDataObject.Module.HWT.R_OBA_TEMP();
            tempObj.ID = TROT.GetNewID(Station.BU, sfcdb);
            tempObj.TYPE = "SN";
            tempObj.VALUE = objSN.SerialNo;
            tempObj.QTY = 1;
            tempObj.STATUS = status;
            tempObj.IP = Station.IP;
            tempObj.EDIT_EMP = Station.LoginUser.EMP_NO;
            tempObj.EDIT_TIME = Station.GetDBDateTime();
            if (status == "FAIL")
            {
                if (sessionRemark == null || sessionRemark.Value == null || sessionRemark.Value.ToString() == "")
                {
                    throw new Exception("OBA Fail,Please input remark");
                }
                tempObj.REMARK = sessionRemark.Value.ToString();
            }
            else
            {
                tempObj.REMARK = "";
            }
            TROT.SaveNewRecord(sfcdb, tempObj);
        }

        /// <summary>
        /// HWT OBA PASS ACTION
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTOBAPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 5)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSMPQty = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSMPQty == null || sessionSMPQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionPassQty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionPassQty == null || sessionPassQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionFailQty = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionFailQty == null || sessionFailQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession sessionSkuno = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (sessionSkuno == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            MESStationSession sessionType = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (sessionType == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }

            int passQty = Convert.ToInt32(sessionPassQty.Value);
            int failQty = Convert.ToInt32(sessionFailQty.Value);
            int samplingQty = Convert.ToInt32(sessionSMPQty.Value);

            if (samplingQty <= passQty + failQty)
            {
                string skuno = sessionSkuno.Value.ToString();
                string type = sessionType.Value.ToString();
                DB_TYPE_ENUM dbtype = Station.DBType;
                OleExec sfcdb = Station.SFCDB;
                string IP = Station.IP;
                string BU = Station.BU;
                string loginName = Station.LoginUser.EMP_NO;
                string stationNmae = Station.StationName;
                string line = Station.Line;
                string status = "1";

                MESDataObject.Module.HWT.T_R_OBA_TEMP TROT = new MESDataObject.Module.HWT.T_R_OBA_TEMP(sfcdb, dbtype);
                T_R_LOT_STATUS TRLS = new T_R_LOT_STATUS(sfcdb, dbtype);
                T_R_LOT_PACK TRLP = new T_R_LOT_PACK(sfcdb, dbtype);
                T_R_LOT_DETAIL TRLD = new T_R_LOT_DETAIL(sfcdb, dbtype);
                T_R_SN TRS = new T_R_SN(sfcdb, dbtype);

                List<MESDataObject.Module.HWT.R_OBA_TEMP> listOBASN = TROT.GetListByTypeAndIP(sfcdb, "SN", IP).OrderBy(r => r.EDIT_TIME).ToList();
                DateTime dt = Station.GetDBDateTime();
                if (failQty > 0)
                {
                    status = "0";
                }

                #region 1.寫LOT

                LotNo lot = new LotNo();
                string lotno = lot.GetNewLotNo("OBALOT", sfcdb);
                string lotID = TRLS.GetNewID(BU, sfcdb);
                R_LOT_STATUS lotStatus = new R_LOT_STATUS();
                lotStatus.ID = lotID;
                lotStatus.LOT_NO = lotno;
                lotStatus.SKUNO = skuno;
                lotStatus.AQL_TYPE = type;
                lotStatus.LOT_QTY = listOBASN.Count;
                lotStatus.REJECT_QTY = 0;
                lotStatus.SAMPLE_QTY = samplingQty;
                lotStatus.PASS_QTY = passQty;
                lotStatus.FAIL_QTY = failQty;
                lotStatus.CLOSED_FLAG = "1";
                lotStatus.LOT_STATUS_FLAG = status;
                lotStatus.SAMPLE_STATION = stationNmae;
                lotStatus.LINE = line;
                lotStatus.EDIT_EMP = loginName;
                lotStatus.EDIT_TIME = dt;
                sfcdb.ORM.Insertable<R_LOT_STATUS>(lotStatus).ExecuteCommand();

                #endregion 1.寫LOT

                #region 2.寫lot pack

                List<MESDataObject.Module.HWT.R_OBA_TEMP> listOBAPack = TROT.GetListByTypeAndIP(sfcdb, "PALLET", IP);
                R_LOT_PACK lotPack;
                foreach (MESDataObject.Module.HWT.R_OBA_TEMP l in listOBAPack)
                {
                    lotPack = new R_LOT_PACK();
                    lotPack.ID = TRLP.GetNewID(BU, sfcdb);
                    lotPack.LOTNO = lotno;
                    lotPack.PACKNO = l.VALUE;
                    lotPack.EDIT_EMP = loginName;
                    lotPack.EDIT_TIME = dt;
                    sfcdb.ORM.Insertable<R_LOT_PACK>(lotPack).ExecuteCommand();
                }

                #endregion 2.寫lot pack

                #region 3.寫lot detail

                R_LOT_DETAIL lotDetail;
                R_SN objSN;
                R_PACKING packingCarton;
                R_PACKING packingPallet;
                string routeid = "";
                foreach (MESDataObject.Module.HWT.R_OBA_TEMP s in listOBASN)
                {
                    objSN = TRS.LoadData(s.VALUE, sfcdb);
                    if (objSN == null)
                    {
                        throw new Exception(s.VALUE + " not exist!");
                    }
                    if (string.IsNullOrEmpty(routeid))
                    {
                        routeid = objSN.ROUTE_ID;
                    }
                    packingCarton = sfcdb.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((RS, RSP, RP) => RS.ID == RSP.SN_ID && RSP.PACK_ID == RP.ID)
                        .Where((RS, RSP, RP) => RS.ID == objSN.ID).Select((RS, RSP, RP) => RP).ToList().FirstOrDefault();
                    if (packingCarton == null)
                    {
                        throw new Exception(s.VALUE + "'s cartonno not exist!");
                    }
                    packingPallet = sfcdb.ORM.Queryable<R_PACKING, R_PACKING>((RPC, RPP) => RPC.PARENT_PACK_ID == RPP.ID)
                        .Where((RPC, RPP) => RPC.ID == packingCarton.ID).Select((RPC, RPP) => RPP).ToList().FirstOrDefault();
                    if (packingPallet == null)
                    {
                        throw new Exception(s.VALUE + "'s palletno not exist!");
                    }
                    lotDetail = new R_LOT_DETAIL();
                    lotDetail.ID = TRLD.GetNewID(BU, sfcdb);
                    lotDetail.LOT_ID = lotID;
                    lotDetail.SN = objSN.SN;
                    lotDetail.WORKORDERNO = objSN.WORKORDERNO;
                    lotDetail.CREATE_DATE = dt;
                    lotDetail.SAMPLING = "1";
                    lotDetail.STATUS = s.STATUS == "PASS" ? "1" : "0";
                    lotDetail.DESCRIPTION = s.REMARK;
                    lotDetail.CARTON_NO = packingCarton.PACK_NO;
                    lotDetail.PALLET_NO = packingPallet.PACK_NO;
                    lotDetail.EDIT_EMP = loginName;
                    lotDetail.EDIT_TIME = dt;
                    sfcdb.ORM.Insertable<R_LOT_DETAIL>(lotDetail).ExecuteCommand();
                }

                #endregion 3.寫lot detail

                #region 4.全部Pass過站,只要有一個fail則不過站

                if (status == "1")
                {
                    T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
                    Dictionary<string, object> nextSation = TCRD.GetNextStations(routeid, stationNmae, sfcdb);
                    List<R_SN> listSN = sfcdb.ORM.Queryable<MESDataObject.Module.HWT.R_OBA_TEMP, R_SN, R_SN_PACKING, R_PACKING, R_PACKING>
                        ((RO, RS, RSP, RP, RPG) => RO.VALUE == RPG.PACK_NO && RPG.ID == RP.PARENT_PACK_ID && RP.ID == RSP.PACK_ID && RSP.SN_ID == RS.ID)
                        .Where((RO, RS, RSP, RP, RPG) => RO.TYPE == "PALLET" && RS.VALID_FLAG == "1").Select((RO, RS, RSP, RP, RPG) => RS).ToList();

                    foreach (R_SN s in listSN)
                    {
                        TRS.PassStation(s.SN, line, stationNmae, stationNmae, BU, "PASS", loginName, sfcdb);
                    }
                }

                #endregion 4.全部Pass過站,只要有一個fail則不過站

                #region 5.清除工站SESSION值

                foreach (var o in Station.StationSession)
                {
                    o.Value = "";
                }
                Station.NextInput = Station.Inputs.Find(r => r.DisplayName == "PalletNo");
                Station.StationSession.Find(t => t.MESDataType == "Result" && t.SessionKey == "1").Value = "PASS";
                Station.StationSession.Find(t => t.MESDataType == "IsNewLot" && t.SessionKey == "1").Value = "YES";
                Station.Inputs.Find(r => r.DisplayName == "Result").Visable = false;
                Station.Inputs.Find(r => r.DisplayName == "SMPSN").Visable = false;

                TROT.DeleteByTypeAndIP(sfcdb, "PALLET", Station.IP);
                TROT.DeleteByTypeAndIP(sfcdb, "SN", Station.IP);

                #endregion 5.清除工站SESSION值
            }
        }

        /// <summary>
        /// HWT ASSY類生成父項綁定關係ACTION(R_RELATION_DATA)
        /// add by hgb 2019.08.27
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTMakeParentRelationAction(MESStationBase Station, MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                if (Paras.Count != 2)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SnSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                string SN = SnSession.Value.ToString().Trim().ToUpper();

                MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (SkuSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                string SKUNO = SkuSession.Value.ToString().Trim().ToUpper();

                Station.SFCDB.BeginTrain();

                T_R_RELATION_DATA t_r_relation_data = new T_R_RELATION_DATA(Station.SFCDB, Station.DBType);
                t_r_relation_data.HWTMakeParentRelation(SN, SKUNO, Station.StationName, Station.SFCDB);

                // Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                // Station.SFCDB.RollbackTrain();
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
            }
        }

        /// <summary>
        /// HWT_ASSY類生成傳送HW綁定關係R_RELATIONDATA_DETAIL
        /// add by hgb 2019.08.28
        ///  </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void HWTMakeSentHwRelationAction(MESStationBase Station, MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            try
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }

                MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
                if (SnSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                string SN = SnSession.Value.ToString().Trim().ToUpper();

                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
                }
                string WO = WOSession.Value.ToString().Trim().ToUpper();

                MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (SkuSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
                }
                string SKUNO = SkuSession.Value.ToString().Trim().ToUpper();

                Station.SFCDB.BeginTrain();

                T_R_RELATIONDATA_DETAIL t_r_relationdata_detail = new T_R_RELATIONDATA_DETAIL(Station.SFCDB, Station.DBType);
                t_r_relationdata_detail.HWTMakeSentHwRelation(SN, SKUNO, Station.StationName, WO, Station.SFCDB);

                //  Station.SFCDB.CommitTrain();
            }
            catch (Exception ex)
            {
                // Station.SFCDB.RollbackTrain();
                throw new Exception(ex.Message.ToString());
            }
            finally
            {
            }
        }

        /// <summary>
        /// VERTIV掃入重工工單打散RMA回來的SN綁定關係     請注意這個不用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void VTBreakUpRMASNLinkAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN objSN = (SN)sessionSN.Value;
            string nextStation = sessionStation.Value.ToString();
            WorkOrder objWorkorder = (WorkOrder)sessionWo.Value;
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;
            MESDataObject.Module.Vertiv.T_R_RMA_BONEPILE TRRB = new MESDataObject.Module.Vertiv.T_R_RMA_BONEPILE(sfcdb, dbtype);

            if (TRRB.RmaBonepileIsOpen(sfcdb, objSN.SerialNo))
            {
                T_R_SN TRS = new T_R_SN(sfcdb, dbtype);
                T_R_SN_KP TRSK = new T_R_SN_KP(sfcdb, dbtype);
                T_R_WO_BASE TRWB = new T_R_WO_BASE(sfcdb, dbtype);
                T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
                T_C_KP_List_Item TCKL = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
                T_C_KP_List_Item_Detail TCKLI = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
                T_C_SKU_MPN TCSM = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
                T_C_KP_Rule TCKR = new T_C_KP_Rule(Station.SFCDB, Station.DBType);

                R_SN r_sn = null;
                R_WO_BASE objWo = TRWB.GetWoByWoNo(objWorkorder.WorkorderNo, sfcdb);

                //List<C_ROUTE_DETAIL> listRoute = TCRD.GetLastStations(objWo.ROUTE_ID, nextStation, sfcdb);
                //string[] arryStation = listRoute.Select(r => r.STATION_NAME).ToArray();
                //List<R_SN_KP> listKP = TRSK.GetAllKPBySnIDStation(objSN.ID, sfcdb, arryStation);
                //List<R_SN> listSN = new List<R_SN>();
                //foreach (R_SN_KP snkp in listKP)
                //{
                //    r_sn = TRS.GetSN(snkp.VALUE, sfcdb);
                //    if (r_sn == null)
                //    {
                //        continue;
                //    }
                //    listSN.Add(r_sn);
                //}
                //TRS.UpdateShippingFlag(listSN, "0", Station.LoginUser.EMP_NO, sfcdb);
                //TRSK.ReturnUpdateKPSNBySnId(objSN.ID, arryStation, Station.LoginUser.EMP_NO, sfcdb);

                List<C_ROUTE_DETAIL> listRoute = TCRD.GetByRouteIdOrderBySEQASC(objWo.ROUTE_ID, sfcdb);
                List<C_ROUTE_DETAIL> listLastRoute = TCRD.GetAllNextStationsByCurrentStation(objWo.ROUTE_ID, nextStation, sfcdb);
                List<R_SN_KP> listKP = TRSK.GetKPRecordBySnID(objSN.ID, sfcdb);
                List<string> listStation = listRoute.Select(r => r.STATION_NAME).Distinct().ToList();

                //已被刪掉的工站的KP
                List<R_SN_KP> listNoStationKP = listKP.Where(r => !listStation.Contains(r.STATION)).ToList();
                //當前選擇工站及其往後的工站的KP
                string[] arryStation = listLastRoute.Select(r => r.STATION_NAME).ToArray();
                List<R_SN_KP> listLastStationKP = TRSK.GetAllKPBySnIDStation(objSN.ID, sfcdb, arryStation);

                List<R_SN_KP> listDelete = new List<R_SN_KP>();
                listDelete.AddRange(listNoStationKP);
                listDelete.AddRange(listLastStationKP);
                List<R_SN> listKPSN = new List<R_SN>();
                foreach (R_SN_KP snkp in listDelete)
                {
                    r_sn = TRS.GetSN(snkp.VALUE, sfcdb);
                    if (r_sn != null)
                    {
                        listKPSN.Add(r_sn);
                    }
                    TRSK.ReworkUpdateKP(snkp.ID, Station.LoginUser.EMP_NO, sfcdb);
                }
                TRS.UpdateShippingFlag(listKPSN, "0", Station.LoginUser.EMP_NO, sfcdb);

                //刪掉后重新獲取已經綁定的KP信息
                listKP = TRSK.GetKPRecordBySnID(objSN.ID, sfcdb);
                List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
                List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
                List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
                C_KP_Rule kpRule = new C_KP_Rule();
                double? scanseq = 0;
                int result;
                string skuMpn = "";
                Row_R_SN_KP rowSNKP;
                R_SN_KP newKP = null;
                double? kpMaxSeq = listKP.Max(r => r.SCANSEQ);
                kpMaxSeq = kpMaxSeq == null ? 0 : kpMaxSeq;

                kpItemList = TCKL.GetItemObjectByListId(objWo.KP_LIST_ID, Station.SFCDB);
                foreach (C_KP_List_Item kpItem in kpItemList)
                {
                    itemDetailList = TCKLI.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { objWo.SKUNO }));
                    }

                    skuMpnList = TCSM.GetMpnBySkuAndPartno(Station.SFCDB, objWo.SKUNO, kpItem.KP_PARTNO);
                    if (skuMpnList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                    }
                    skuMpn = skuMpnList[0].MPN;
                    //添加新KEY
                    foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                    {
                        newKP = listKP.Find(r => r.R_SN_ID == objSN.ID && r.KP_NAME == kpItem.KP_NAME && r.STATION == kpItem.STATION);
                        if (newKP != null)
                        {
                            continue;
                        }
                        scanseq = kpMaxSeq + scanseq + 1;
                        kpRule = TCKR.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                        if (kpRule == null)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        if (kpRule.REGEX == "")
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        rowSNKP = (Row_R_SN_KP)TRSK.NewRow();
                        rowSNKP.ID = TRSK.GetNewID(Station.BU, Station.SFCDB);
                        rowSNKP.R_SN_ID = objSN.ID;
                        rowSNKP.SN = objSN.SerialNo;
                        rowSNKP.VALUE = "";
                        rowSNKP.PARTNO = kpItem.KP_PARTNO;
                        rowSNKP.KP_NAME = kpItem.KP_NAME;
                        rowSNKP.MPN = skuMpn;
                        rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                        rowSNKP.ITEMSEQ = kpItem.SEQ;
                        rowSNKP.SCANSEQ = scanseq;
                        rowSNKP.DETAILSEQ = itemDetail.SEQ;
                        rowSNKP.STATION = kpItem.STATION;
                        rowSNKP.REGEX = kpRule.REGEX;
                        rowSNKP.VALID_FLAG = 1;
                        rowSNKP.EXKEY1 = "";
                        rowSNKP.EXVALUE1 = "";
                        rowSNKP.EXKEY2 = "";
                        rowSNKP.EXVALUE2 = "";
                        rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                        result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                        if (result <= 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 打散SN綁定關係
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BreakUpSNLinkAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN objSN = (SN)sessionSN.Value;
            string nextStation = sessionStation.Value.ToString();
            WorkOrder objWorkorder = (WorkOrder)sessionWo.Value;
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;

            T_R_SN TRS = new T_R_SN(sfcdb, dbtype);
            T_R_SN_KP TRSK = new T_R_SN_KP(sfcdb, dbtype);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(sfcdb, dbtype);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
            T_C_KP_List_Item TCKL = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail TCKLI = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN TCSM = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule TCKR = new T_C_KP_Rule(Station.SFCDB, Station.DBType);

            R_SN r_sn = null;
            R_WO_BASE objWo = TRWB.GetWoByWoNo(objWorkorder.WorkorderNo, sfcdb);

            List<C_ROUTE_DETAIL> listRoute = TCRD.GetByRouteIdOrderBySEQASC(objWo.ROUTE_ID, sfcdb);
            List<C_ROUTE_DETAIL> listLastRoute = TCRD.GetAllNextStationsByCurrentStation(objWo.ROUTE_ID, nextStation, sfcdb);
            List<R_SN_KP> listKP = TRSK.GetKPRecordBySnID(objSN.ID, sfcdb);
            List<string> listStation = listRoute.Select(r => r.STATION_NAME).Distinct().ToList();

            //已被刪掉的工站的KP
            List<R_SN_KP> listNoStationKP = listKP.Where(r => !listStation.Contains(r.STATION)).ToList();
            //當前選擇工站及其往後的工站的KP
            string[] arryStation = listLastRoute.Select(r => r.STATION_NAME).ToArray();
            List<R_SN_KP> listLastStationKP = TRSK.GetAllKPBySnIDStation(objSN.ID, sfcdb, arryStation);

            List<R_SN_KP> listDelete = new List<R_SN_KP>();
            listDelete.AddRange(listNoStationKP);
            listDelete.AddRange(listLastStationKP);
            List<R_SN> listKPSN = new List<R_SN>();
            foreach (R_SN_KP snkp in listDelete)
            {
                r_sn = TRS.GetSN(snkp.VALUE, sfcdb);
                if (r_sn != null)
                {
                    listKPSN.Add(r_sn);
                }
                TRSK.ReworkUpdateKP(snkp.ID, Station.LoginUser.EMP_NO, sfcdb);

                if (Station.BU.Equals("VNDCN"))
                {
                    //update wwn_datasharing
                    //upadte 3階/出貨階
                    Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                        .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { CSKU = "N/A", CSSN = "N/A" })
                        .Where(w => w.CSSN == snkp.SN && w.VSSN == snkp.VALUE).ExecuteCommand();
                    //upadte 2階
                    Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                        .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { VSSN = "N/A", VSKU = "N/A", CSKU = "N/A", CSSN = "N/A" })
                        .Where(w => w.VSSN == snkp.SN && w.WSN == snkp.VALUE).ExecuteCommand();
                }
            }
            TRS.UpdateShippingFlag(listKPSN, "0", Station.LoginUser.EMP_NO, sfcdb);

            //刪掉后重新獲取已經綁定的KP信息
            listKP = TRSK.GetKPRecordBySnID(objSN.ID, sfcdb);
            List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
            List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
            List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
            C_KP_Rule kpRule = new C_KP_Rule();
            double? scanseq = 0;
            int result;
            string skuMpn = "";
            Row_R_SN_KP rowSNKP;
            double? kpMaxSeq = listKP.Max(r => r.SCANSEQ);
            scanseq = kpMaxSeq == null ? 0 : kpMaxSeq;
            kpItemList = TCKL.GetItemObjectByListId(objWo.KP_LIST_ID, Station.SFCDB);
            var sncurentkpitemlist = kpItemList.FindAll(t => arryStation.Contains(t.STATION));
            var kpcatch = new List<Row_R_SN_KP>();
            foreach (C_KP_List_Item kpItem in sncurentkpitemlist)
            {
                itemDetailList = TCKLI.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                if (itemDetailList == null || itemDetailList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { objWo.SKUNO }));
                }

                skuMpnList = TCSM.GetMpnBySkuAndPartno(Station.SFCDB, objWo.SKUNO, kpItem.KP_PARTNO);
                if (skuMpnList.Count == 0)
                {
                    throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                }
                skuMpn = skuMpnList[0].MPN;
                //添加新KEY
                for (int i = 0; i < kpItem.QTY; i++)
                {
                    foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                    {
                        scanseq = scanseq + 1;
                        kpRule = TCKR.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                        if (kpRule == null)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        if (kpRule.REGEX == "")
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                        }
                        rowSNKP = (Row_R_SN_KP)TRSK.NewRow();
                        rowSNKP.ID = TRSK.GetNewID(Station.BU, Station.SFCDB);
                        rowSNKP.R_SN_ID = objSN.ID;
                        rowSNKP.SN = objSN.SerialNo;
                        rowSNKP.VALUE = "";
                        rowSNKP.PARTNO = kpItem.KP_PARTNO;
                        rowSNKP.KP_NAME = kpItem.KP_NAME;
                        rowSNKP.MPN = skuMpn;
                        rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                        rowSNKP.ITEMSEQ = kpItem.SEQ;
                        rowSNKP.SCANSEQ = scanseq;
                        rowSNKP.DETAILSEQ = itemDetail.SEQ;
                        rowSNKP.STATION = kpItem.STATION;
                        rowSNKP.REGEX = kpRule.REGEX;
                        rowSNKP.VALID_FLAG = 1;
                        rowSNKP.EXKEY1 = "";
                        rowSNKP.EXVALUE1 = "";
                        rowSNKP.EXKEY2 = "";
                        rowSNKP.EXVALUE2 = "";
                        rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                        rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                        //增加LOCATION add by Eden
                        rowSNKP.LOCATION = new Func<string>(() =>
                        {
                            if (itemDetail.LOCATION != null && itemDetail.LOCATION.Length > 0)
                            {
                                var locations = itemDetail.LOCATION.Split('|');
                                if (i < locations.Length)
                                    return $@"{locations[i]}-1";
                                else
                                    return $@"{locations.LastOrDefault()}-{i - locations.Length + 2}";
                                //return locations.Length <= i ? locations.LastOrDefault() : locations[i];
                            }
                            else
                            {
                                ///上傳KPNAME禁止以-符號結尾
                                var locationSeq = kpcatch.Count > 0 ? kpcatch.FindAll(t => t.LOCATION.StartsWith($@"{kpItem.KP_NAME}-") && t.LOCATION.Length < (kpItem.KP_NAME.Length + 4)).Count + 1 : 1;
                                return $@"{kpItem.KP_NAME}-{locationSeq}";
                            }
                        })();
                        result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                        if (result <= 0)
                        {
                            throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                        }
                        kpcatch.Add(rowSNKP);
                    }
                }
            }
        }

        /// <summary>
        /// 通過Snlist打散SN綁定關係
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BreakUpSNLinkBySnlist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (Pallet == null)
            {

                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            string nextStation = sessionStation.Value.ToString();
            WorkOrder objWorkorder = (WorkOrder)sessionWo.Value;
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;

            T_R_SN TRS = new T_R_SN(sfcdb, dbtype);
            T_R_SN_KP TRSK = new T_R_SN_KP(sfcdb, dbtype);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(sfcdb, dbtype);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
            T_C_KP_List_Item TCKL = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail TCKLI = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN TCSM = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule TCKR = new T_C_KP_Rule(Station.SFCDB, Station.DBType);

            R_SN r_sn = null;
            R_WO_BASE objWo = TRWB.GetWoByWoNo(objWorkorder.WorkorderNo, sfcdb);
            if (objWo == null) throw new MESDataObject.MESReturnMessage($@"WO: '{ objWorkorder.WorkorderNo }' not found");

            List<C_ROUTE_DETAIL> listRoute = TCRD.GetByRouteIdOrderBySEQASC(objWo.ROUTE_ID, sfcdb);
            if (listRoute.Count == 0) throw new MESDataObject.MESReturnMessage($@"ROUTE_ID: '{ objWo.ROUTE_ID }' not found");
            List<C_ROUTE_DETAIL> listLastRoute = TCRD.GetAllNextStationsByCurrentStation(objWo.ROUTE_ID, nextStation, sfcdb);

            List<R_SN> snList = new List<R_SN>();
            //snList = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((sn, wo) => sn.WORKORDERNO == wo.WORKORDERNO)
            //            .Where((sn, wo) => wo.WORKORDERNO == objWorkorder.WorkorderNo   && (sn.SCRAPED_FLAG != "1" || SqlSugar.SqlFunc.IsNullOrEmpty(sn.SCRAPED_FLAG)) && sn.VALID_FLAG == "1")
            //            .Select((sn, wo) => sn).ToList();

            snList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.PACK_NO == Pallet.Value
                   && a.ID == b.PARENT_PACK_ID && c.PACK_ID == b.ID && d.ID == c.SN_ID && d.VALID_FLAG == "1").Select((a, b, c, d) => d).ToList(); //按照棧板取sn

            foreach (R_SN _r_sn in snList)
            {
                List<R_SN_KP> listKP = TRSK.GetKPRecordBySnID(_r_sn.ID, sfcdb);
                List<string> listStation = listRoute.Select(r => r.STATION_NAME).Distinct().ToList();

                //已被刪掉的工站的KP
                List<R_SN_KP> listNoStationKP = listKP.Where(r => !listStation.Contains(r.STATION)).ToList();
                //當前選擇工站及其往後的工站的KP
                string[] arryStation = listLastRoute.Select(r => r.STATION_NAME).ToArray();
                List<R_SN_KP> listLastStationKP = TRSK.GetAllKPBySnIDStation(_r_sn.ID, sfcdb, arryStation);

                List<R_SN_KP> listDelete = new List<R_SN_KP>();
                listDelete.AddRange(listNoStationKP);
                listDelete.AddRange(listLastStationKP);
                List<R_SN> listKPSN = new List<R_SN>();
                foreach (R_SN_KP snkp in listDelete)
                {
                    r_sn = TRS.GetSN(snkp.VALUE, sfcdb);
                    if (r_sn != null)
                    {
                        listKPSN.Add(r_sn);
                    }
                    TRSK.ReworkUpdateKP(snkp.ID, Station.LoginUser.EMP_NO, sfcdb);

                    if (Station.BU.Equals("VNDCN"))
                    {
                        //update wwn_datasharing
                        //upadte 3階/出貨階
                        Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.CSSN == snkp.SN && w.VSSN == snkp.VALUE).ExecuteCommand();
                        //upadte 2階
                        Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { VSSN = "N/A", VSKU = "N/A", CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.VSSN == snkp.SN && w.WSN == snkp.VALUE).ExecuteCommand();
                    }
                }
                TRS.UpdateShippingFlag(listKPSN, "0", Station.LoginUser.EMP_NO, sfcdb);

                //刪掉后重新獲取已經綁定的KP信息
                listKP = TRSK.GetKPRecordBySnID(_r_sn.ID, sfcdb);
                List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
                List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
                List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
                C_KP_Rule kpRule = new C_KP_Rule();
                double? scanseq = 0;
                int result;
                string skuMpn = "";
                Row_R_SN_KP rowSNKP;
                double? kpMaxSeq = listKP.Max(r => r.SCANSEQ);
                scanseq = kpMaxSeq == null ? 0 : kpMaxSeq;
                kpItemList = TCKL.GetItemObjectByListId(objWo.KP_LIST_ID, Station.SFCDB);
                var sncurentkpitemlist = kpItemList.FindAll(t => arryStation.Contains(t.STATION));
                foreach (C_KP_List_Item kpItem in sncurentkpitemlist)
                {
                    itemDetailList = TCKLI.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { objWo.SKUNO }));
                    }

                    skuMpnList = TCSM.GetMpnBySkuAndPartno(Station.SFCDB, objWo.SKUNO, kpItem.KP_PARTNO);
                    if (skuMpnList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                    }
                    skuMpn = skuMpnList[0].MPN;
                    //添加新KEY
                    for (int i = 0; i < kpItem.QTY; i++)
                    {
                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            scanseq = scanseq + 1;
                            kpRule = TCKR.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                            if (kpRule == null)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                            }
                            if (kpRule.REGEX == "")
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                            }
                            rowSNKP = (Row_R_SN_KP)TRSK.NewRow();
                            rowSNKP.ID = TRSK.GetNewID(Station.BU, Station.SFCDB);
                            rowSNKP.R_SN_ID = _r_sn.ID;
                            rowSNKP.SN = _r_sn.SN;
                            rowSNKP.VALUE = "";
                            rowSNKP.PARTNO = kpItem.KP_PARTNO;
                            rowSNKP.KP_NAME = kpItem.KP_NAME;
                            rowSNKP.MPN = skuMpn;
                            rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                            rowSNKP.ITEMSEQ = kpItem.SEQ;
                            rowSNKP.SCANSEQ = scanseq;
                            rowSNKP.DETAILSEQ = itemDetail.SEQ;
                            rowSNKP.STATION = kpItem.STATION;
                            rowSNKP.REGEX = kpRule.REGEX;
                            rowSNKP.VALID_FLAG = 1;
                            rowSNKP.EXKEY1 = "";
                            rowSNKP.EXVALUE1 = "";
                            rowSNKP.EXKEY2 = "";
                            rowSNKP.EXVALUE2 = "";
                            rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                            result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                            if (result <= 0)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ReturnStationSNLinkBySnlist
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnStationSNLinkBySnlist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionSN == null)
            {

                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            string nextStation = sessionStation.Value.ToString();
            WorkOrder objWorkorder = (WorkOrder)sessionWo.Value;
            OleExec sfcdb = Station.SFCDB;
            DB_TYPE_ENUM dbtype = Station.DBType;

            T_R_SN TRS = new T_R_SN(sfcdb, dbtype);
            T_R_SN_KP TRSK = new T_R_SN_KP(sfcdb, dbtype);
            T_R_WO_BASE TRWB = new T_R_WO_BASE(sfcdb, dbtype);
            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(sfcdb, dbtype);
            T_C_KP_List_Item TCKL = new T_C_KP_List_Item(Station.SFCDB, Station.DBType);
            T_C_KP_List_Item_Detail TCKLI = new T_C_KP_List_Item_Detail(Station.SFCDB, Station.DBType);
            T_C_SKU_MPN TCSM = new T_C_SKU_MPN(Station.SFCDB, Station.DBType);
            T_C_KP_Rule TCKR = new T_C_KP_Rule(Station.SFCDB, Station.DBType);

            R_SN r_sn = null;
            R_WO_BASE objWo = TRWB.GetWoByWoNo(objWorkorder.WorkorderNo, sfcdb);
            if (objWo == null) throw new MESDataObject.MESReturnMessage($@"WO: '{ objWorkorder.WorkorderNo }' not found");

            List<C_ROUTE_DETAIL> listRoute = TCRD.GetByRouteIdOrderBySEQASC(objWo.ROUTE_ID, sfcdb);
            if (listRoute.Count == 0) throw new MESDataObject.MESReturnMessage($@"ROUTE_ID: '{ objWo.ROUTE_ID }' not found");
            List<C_ROUTE_DETAIL> listLastRoute = TCRD.GetAllNextStationsByCurrentStation(objWo.ROUTE_ID, nextStation, sfcdb);

            List<R_SN> snList = new List<R_SN>();
            string lstSN = sessionSN.Value.ToString();
            lstSN = $@"'{lstSN.Replace("\n", "',\n'")}'";
            if (lstSN.Length == 0)
            {
                throw new MESReturnMessage("Please Input list SN!");
            }
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            snList = t_r_sn.GetSnListByListSN(lstSN, Station.SFCDB);

            foreach (R_SN _r_sn in snList)
            {
                List<R_SN_KP> listKP = TRSK.GetKPRecordBySnID(_r_sn.ID, sfcdb);
                List<string> listStation = listRoute.Select(r => r.STATION_NAME).Distinct().ToList();

                //已被刪掉的工站的KP
                List<R_SN_KP> listNoStationKP = listKP.Where(r => !listStation.Contains(r.STATION)).ToList();
                //當前選擇工站及其往後的工站的KP
                string[] arryStation = listLastRoute.Select(r => r.STATION_NAME).ToArray();
                List<R_SN_KP> listLastStationKP = TRSK.GetAllKPBySnIDStation(_r_sn.ID, sfcdb, arryStation);

                List<R_SN_KP> listDelete = new List<R_SN_KP>();
                listDelete.AddRange(listNoStationKP);
                listDelete.AddRange(listLastStationKP);
                List<R_SN> listKPSN = new List<R_SN>();
                foreach (R_SN_KP snkp in listDelete)
                {
                    r_sn = TRS.GetSN(snkp.VALUE, sfcdb);
                    if (r_sn != null)
                    {
                        listKPSN.Add(r_sn);
                    }
                    TRSK.ReworkUpdateKP(snkp.ID, Station.LoginUser.EMP_NO, sfcdb);

                    if (Station.BU.Equals("VNDCN"))
                    {
                        //update wwn_datasharing
                        //upadte 3階/出貨階
                        Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.CSSN == snkp.SN && w.VSSN == snkp.VALUE).ExecuteCommand();
                        //upadte 2階
                        Station.SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing { VSSN = "N/A", VSKU = "N/A", CSKU = "N/A", CSSN = "N/A" })
                            .Where(w => w.VSSN == snkp.SN && w.WSN == snkp.VALUE).ExecuteCommand();
                    }
                }
                TRS.UpdateShippingFlag(listKPSN, "0", Station.LoginUser.EMP_NO, sfcdb);

                //刪掉后重新獲取已經綁定的KP信息
                listKP = TRSK.GetKPRecordBySnID(_r_sn.ID, sfcdb);
                List<C_KP_List_Item> kpItemList = new List<C_KP_List_Item>();
                List<C_SKU_MPN> skuMpnList = new List<C_SKU_MPN>();
                List<C_KP_List_Item_Detail> itemDetailList = new List<C_KP_List_Item_Detail>();
                C_KP_Rule kpRule = new C_KP_Rule();
                double? scanseq = 0;
                int result;
                string skuMpn = "";
                Row_R_SN_KP rowSNKP;
                double? kpMaxSeq = listKP.Max(r => r.SCANSEQ);
                scanseq = kpMaxSeq == null ? 0 : kpMaxSeq;
                kpItemList = TCKL.GetItemObjectByListId(objWo.KP_LIST_ID, Station.SFCDB);
                var sncurentkpitemlist = kpItemList.FindAll(t => arryStation.Contains(t.STATION));
                foreach (C_KP_List_Item kpItem in sncurentkpitemlist)
                {
                    itemDetailList = TCKLI.GetItemDetailObjectByItemId(kpItem.ID, Station.SFCDB);
                    if (itemDetailList == null || itemDetailList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000255", new string[] { objWo.SKUNO }));
                    }

                    skuMpnList = TCSM.GetMpnBySkuAndPartno(Station.SFCDB, objWo.SKUNO, kpItem.KP_PARTNO);
                    if (skuMpnList.Count == 0)
                    {
                        throw new MESDataObject.MESReturnMessage(kpItem.KP_PARTNO + ",MPN MAPPING NOT SETTING!");
                    }
                    skuMpn = skuMpnList[0].MPN;
                    //添加新KEY
                    for (int i = 0; i < kpItem.QTY; i++)
                    {
                        foreach (C_KP_List_Item_Detail itemDetail in itemDetailList)
                        {
                            scanseq = scanseq + 1;
                            kpRule = TCKR.GetKPRule(Station.SFCDB, kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE);
                            if (kpRule == null)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                            }
                            if (kpRule.REGEX == "")
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000256", new string[] { kpItem.KP_PARTNO, skuMpn, itemDetail.SCANTYPE }));
                            }
                            rowSNKP = (Row_R_SN_KP)TRSK.NewRow();
                            rowSNKP.ID = TRSK.GetNewID(Station.BU, Station.SFCDB);
                            rowSNKP.R_SN_ID = _r_sn.ID;
                            rowSNKP.SN = _r_sn.SN;
                            rowSNKP.VALUE = "";
                            rowSNKP.PARTNO = kpItem.KP_PARTNO;
                            rowSNKP.KP_NAME = kpItem.KP_NAME;
                            rowSNKP.MPN = skuMpn;
                            rowSNKP.SCANTYPE = itemDetail.SCANTYPE;
                            rowSNKP.ITEMSEQ = kpItem.SEQ;
                            rowSNKP.SCANSEQ = scanseq;
                            rowSNKP.DETAILSEQ = itemDetail.SEQ;
                            rowSNKP.STATION = kpItem.STATION;
                            rowSNKP.REGEX = kpRule.REGEX;
                            rowSNKP.VALID_FLAG = 1;
                            rowSNKP.EXKEY1 = "";
                            rowSNKP.EXVALUE1 = "";
                            rowSNKP.EXKEY2 = "";
                            rowSNKP.EXVALUE2 = "";
                            rowSNKP.EDIT_EMP = Station.LoginUser.EMP_NO;
                            rowSNKP.EDIT_TIME = Station.GetDBDateTime();
                            try
                            {
                                result = Convert.ToInt32(Station.SFCDB.ExecSQL(rowSNKP.GetInsertString(Station.DBType)));
                                if (result <= 0)
                                {
                                    throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                                }
                            }
                            catch (Exception exx)
                            {
                                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_KP:" + r_sn.SN, "ADD" }));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 清除包裝信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BreakUpPackingInfoAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession sessionReturnStation = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionReturnStation == null || sessionReturnStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            SN objSN = (SN)sessionSN.Value;
            WorkOrder objWO = (WorkOrder)sessionWo.Value;
            string returnStation = sessionReturnStation.Value.ToString();
            int result = 0;

            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN TRSN = new T_R_SN(Station.SFCDB, Station.DBType);

            R_SN r_sn = TRSN.LoadData(objSN.SerialNo, Station.SFCDB);
            List<C_ROUTE_DETAIL> details = TCRD.GetByRouteIdOrderBySEQASC(objWO.RouteID, Station.SFCDB);
            C_ROUTE_DETAIL PackStation = details.Find(t => t.STATION_NAME == "CARTON" || t.STATION_NAME == "PACKOUT");//Juniper's Packout StationName=PACKOUT, not CARTON
            C_ROUTE_DETAIL ShipStation = details.Find(t => t.STATION_NAME == "SHIPOUT");

            //如果回到出貨狀態之前，就一定把 SHIPPED_FLAG 改成0
            if (ShipStation != null && details.Any(t => t.STATION_NAME == returnStation && t.SEQ_NO <= ShipStation.SEQ_NO))
            {
                r_sn.SHIPPED_FLAG = "0";
                r_sn.SHIPDATE = null;
                result = TRSN.Update(r_sn, Station.SFCDB);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                }
            }
            //如果回到包裝之前，就一定把 PACKED_FLAG 改成0
            if (PackStation != null && details.Any(t => t.STATION_NAME == returnStation && t.SEQ_NO <= PackStation.SEQ_NO))
            {
                r_sn.PACKED_FLAG = "0";
                r_sn.PACKDATE = null;
                //處理包裝信息
                T_R_SN_PACKING TRSP = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
                T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
                Row_R_SN_PACKING rowPacking = TRSP.GetDataBySNID(objSN.ID, Station.SFCDB);
                R_SN_PACKING r_sn_packing = rowPacking == null ? null : rowPacking.GetDataObject();
                DateTime sysdataDT = TRSP.GetDBDateTime(Station.SFCDB);
                if (r_sn_packing != null)
                {
                    R_PACKING cartonPack = TRP.GetPackingByID(r_sn_packing.PACK_ID, Station.SFCDB);
                    if (cartonPack != null)
                    {
                        cartonPack.QTY = cartonPack.QTY - 1;
                        cartonPack.EDIT_EMP = Station.LoginUser.EMP_NO;
                        cartonPack.EDIT_TIME = sysdataDT;

                        bool bUpdatePallet = false;
                        string palletID = cartonPack.PARENT_PACK_ID;
                        if (cartonPack.QTY <= 0)
                        {
                            cartonPack.PARENT_PACK_ID = "RW" + cartonPack.PARENT_PACK_ID;
                            bUpdatePallet = true;
                        }

                        r_sn_packing.SN_ID = "RW" + r_sn_packing.SN_ID;
                        r_sn_packing.PACK_ID = "RW" + r_sn_packing.PACK_ID;
                        r_sn_packing.EDIT_EMP = Station.LoginUser.EMP_NO;
                        r_sn_packing.EDIT_TIME = sysdataDT;
                        result = TRSP.Update(r_sn_packing, Station.SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_PACKING" }));
                        }

                        result = TRP.UpdatePacking(cartonPack, Station.SFCDB);
                        if (result == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                        }

                        if (bUpdatePallet)
                        {
                            R_PACKING palletPack = TRP.GetPackingByID(palletID, Station.SFCDB);
                            if (palletPack != null)
                            {
                                palletPack.QTY = palletPack.QTY - 1;
                                palletPack.EDIT_EMP = Station.LoginUser.EMP_NO;
                                palletPack.EDIT_TIME = sysdataDT;
                                result = TRP.UpdatePacking(palletPack, Station.SFCDB);
                                if (result == 0)
                                {
                                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                                }
                            }
                        }
                    }
                }
                result = TRSN.Update(r_sn, Station.SFCDB);
                if (result == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                }
            }
        }

        /// <summary>
        /// 通過Snlist清除包裝信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void BreakUpPackingInfoBySnlist(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionReturnStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionReturnStation == null || sessionReturnStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (Pallet == null)
            {

                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE }));
            }

            // SN objSN = (SN)sessionSN.Value;
            WorkOrder objWO = (WorkOrder)sessionWo.Value;
            string returnStation = sessionReturnStation.Value.ToString();
            int result = 0;

            T_C_ROUTE_DETAIL TCRD = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SN TRSN = new T_R_SN(Station.SFCDB, Station.DBType);

            //R_SN r_sn = TRSN.LoadData(objSN.SerialNo, Station.SFCDB);
            List<C_ROUTE_DETAIL> details = TCRD.GetByRouteIdOrderBySEQASC(objWO.RouteID, Station.SFCDB);
            C_ROUTE_DETAIL PackStation = details.Find(t => t.STATION_NAME == "CARTON");
            C_ROUTE_DETAIL ShipStation = details.Find(t => t.STATION_NAME == "SHIPOUT");

            List<R_SN> snList = new List<R_SN>();
            //snList = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((sn, wo) => sn.WORKORDERNO == wo.WORKORDERNO)
            //            .Where((sn, wo) => wo.WORKORDERNO == objWO.WorkorderNo  && (sn.SCRAPED_FLAG != "1" || SqlSugar.SqlFunc.IsNullOrEmpty(sn.SCRAPED_FLAG)) && sn.VALID_FLAG == "1")
            //            .Select((sn, wo) => sn).ToList();

            snList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.PACK_NO == Pallet.Value
                 && a.ID == b.PARENT_PACK_ID && c.PACK_ID == b.ID && d.ID == c.SN_ID && d.VALID_FLAG == "1").Select((a, b, c, d) => d).ToList(); //按照棧板取sn

            foreach (R_SN r_sn in snList)
            {
                //如果回到出貨狀態之前，就一定把 SHIPPED_FLAG 改成0
                if (ShipStation != null && details.Any(t => t.STATION_NAME == returnStation && t.SEQ_NO <= ShipStation.SEQ_NO))
                {
                    r_sn.SHIPPED_FLAG = "0";
                    r_sn.SHIPDATE = null;
                    result = TRSN.Update(r_sn, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                    }
                }
                //如果回到包裝之前，就一定把 PACKED_FLAG 改成0
                if (PackStation != null && details.Any(t => t.STATION_NAME == returnStation && t.SEQ_NO <= PackStation.SEQ_NO))
                {
                    r_sn.PACKED_FLAG = "0";
                    r_sn.PACKDATE = null;
                    //處理包裝信息
                    T_R_SN_PACKING TRSP = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);
                    T_R_PACKING TRP = new T_R_PACKING(Station.SFCDB, Station.DBType);
                    Row_R_SN_PACKING rowPacking = TRSP.GetDataBySNID(r_sn.ID, Station.SFCDB);
                    R_SN_PACKING r_sn_packing = rowPacking == null ? null : rowPacking.GetDataObject();
                    DateTime sysdataDT = TRSP.GetDBDateTime(Station.SFCDB);
                    if (r_sn_packing != null)
                    {
                        R_PACKING cartonPack = TRP.GetPackingByID(r_sn_packing.PACK_ID, Station.SFCDB);
                        if (cartonPack != null)
                        {
                            cartonPack.QTY = cartonPack.QTY - 1;
                            cartonPack.EDIT_EMP = Station.LoginUser.EMP_NO;
                            cartonPack.EDIT_TIME = sysdataDT;

                            bool bUpdatePallet = false;
                            string palletID = cartonPack.PARENT_PACK_ID;
                            if (cartonPack.QTY <= 0)
                            {
                                cartonPack.PARENT_PACK_ID = "RW" + cartonPack.PARENT_PACK_ID;
                                bUpdatePallet = true;
                            }

                            r_sn_packing.SN_ID = "RW" + r_sn_packing.SN_ID;
                            r_sn_packing.PACK_ID = "RW" + r_sn_packing.PACK_ID;
                            r_sn_packing.EDIT_EMP = Station.LoginUser.EMP_NO;
                            r_sn_packing.EDIT_TIME = sysdataDT;
                            result = TRSP.Update(r_sn_packing, Station.SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN_PACKING" }));
                            }

                            result = TRP.UpdatePacking(cartonPack, Station.SFCDB);
                            if (result == 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                            }

                            if (bUpdatePallet)
                            {
                                R_PACKING palletPack = TRP.GetPackingByID(palletID, Station.SFCDB);
                                if (palletPack != null)
                                {
                                    palletPack.QTY = palletPack.QTY - 1;
                                    palletPack.EDIT_EMP = Station.LoginUser.EMP_NO;
                                    palletPack.EDIT_TIME = sysdataDT;
                                    result = TRP.UpdatePacking(palletPack, Station.SFCDB);
                                    if (result == 0)
                                    {
                                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
                                    }
                                }
                            }
                        }
                    }
                    result = TRSN.Update(r_sn, Station.SFCDB);
                    if (result == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_SN" }));
                    }
                }

                Station.AddMessage("MES00000063", new string[] { r_sn.SN.ToString() }, StationMessageState.Pass);
            }
        }

        /// <summary>
        /// 處理DN與出貨信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param> 
        public static void DealwithDnwithShippingInformation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESStationSession Pallet = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Pallet == null)
            {

                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            T_R_SN_STATION_DETAIL TRSSD = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            T_R_SHIP_DETAIL TRSD = new T_R_SHIP_DETAIL(Station.SFCDB, Station.DBType);
            R_SHIP_DETAIL objShipDetail;
            List<R_SN> snList = new List<R_SN>();
            int result = 0;

            snList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.PACK_NO == Pallet.Value
            && a.ID == b.PARENT_PACK_ID && c.PACK_ID == b.ID && d.ID == c.SN_ID && d.VALID_FLAG == "1").Select((a, b, c, d) => d).ToList(); //按照棧板取sn

            foreach (R_SN r_sn in snList)
            {

                objShipDetail = TRSD.GetShipDetailBySN(Station.SFCDB, r_sn.SN);

                R_SN_STATION_DETAIL objStationDetail_P = TRSSD.GetDetailBySnAndStation(r_sn.SN, Station.StationName, Station.SFCDB);
                if (objStationDetail_P != null)
                {
                    objStationDetail_P.SN = "RS_" + objStationDetail_P.SN;
                    result = TRSSD.Update(objStationDetail_P, Station.SFCDB); //取消R_SN_STATION_DETAIL SHIPOUT記錄
                }
                if (objShipDetail != null)
                    result = TRSD.CancelShip(Station.SFCDB, objShipDetail);   //取消出貨記錄

            }



        }

        public static void RecordFinishMRBLOG(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSn == null || sessionSn.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }

            SN objSN = (SN)sessionSn.Value;
            T_R_SN TRSN = new T_R_SN(Station.SFCDB, Station.DBType);
            R_SN r_sn = TRSN.LoadData(objSN.SerialNo, Station.SFCDB);
            try
            {
                T_R_MES_LOG mesLog = new T_R_MES_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                string id = mesLog.GetNewID("VERTIV", Station.SFCDB);
                Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
                rowMESLog.ID = id;
                rowMESLog.PROGRAM_NAME = "CloudMES";
                rowMESLog.CLASS_NAME = "SNAction";
                rowMESLog.FUNCTION_NAME = "RecordFinishMRBLOG";
                rowMESLog.LOG_MESSAGE = r_sn.CURRENT_STATION + "-->" + "MRB";
                rowMESLog.LOG_SQL = "";
                rowMESLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                rowMESLog.EDIT_TIME = System.DateTime.Now;
                rowMESLog.DATA1 = "";
                rowMESLog.DATA2 = r_sn.SN;
                rowMESLog.DATA3 = "";
                Station.SFCDB.ThrowSqlExeception = true;
                Station.SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// VERTIV產品完工掃描MRB過站Action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNFiniShMrbPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            SN SnObject = null;
            string UserEMP = Station.LoginUser.EMP_NO;
            string To_Storage = "";
            string From_Storage = "";
            string Confirmed_Flag = "";
            string DeviceName = "";
            R_SN NewSN = new R_SN();
            R_MRB New_R_MRB = new R_MRB();
            R_MRB_GT HMRB_GT = new R_MRB_GT();
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            T_R_SN_PACKING TRP = new T_R_SN_PACKING(Station.SFCDB, Station.DBType);

            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
            if (Paras.Count < 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SnObject = (SN)SNSession.Value;
            //SNID必須存在
            if (SnObject.ID == null || SnObject.ID.Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            if (SnObject.CurrentStation != "CBS")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200305142808", new string[] { SnObject.SerialNo, SnObject.CurrentStation }));
            }
            Row_R_SN_PACKING rowP = TRP.GetDataBySNID(SnObject.ID, Station.SFCDB);

            R_SN_PACKING RSP = rowP == null ? null : rowP.GetDataObject();
            if (RSP != null)
            {
                result = TRP.UpdateRSNPacking(SnObject.ID, RSP.PACK_ID, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_PACKING:" + SnObject.ID, "UPDATE" }));
                }
            }
            //SN如果已經完工，Confirmed_Flag=1，否則Confirmed_Flag=0
            if (SnObject.CompletedFlag != null && SnObject.CompletedFlag == "1")
            {
                Confirmed_Flag = "1";
            }
            else
            {
                Confirmed_Flag = "0";
            }
            //判斷MRBType，0是單板入MRB，1是退料
            //0則From_Storage放空
            //1則From_Storage則取前台傳的工單
            if (Paras[1].VALUE == "0")//0是單板入MRB
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                From_Storage = "";
            }
            else if (Paras[1].VALUE == "1")//1是退料
            {
                if (Paras.Count != 4)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (WoSession == null || WoSession.Value == null || WoSession.Value.ToString().Length <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else
                {
                    From_Storage = WoSession.Value.ToString();
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { "MRBType", "0/1" }));
            }
            //獲取To_Storage
            MESStationSession ToStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ToStorageSession == null || ToStorageSession.Value == null || ToStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                To_Storage = ToStorageSession.Value.ToString();
            }
            //更新SN當前站，下一站，如果SN的compled=1了也就是Confirmed_Flag==1,則修改當前站和下一站即可
            //如果如果SN的compled!=1,則還需要修改sn的compled=1和SN對應工單的finishedQTY要加一
            if (Confirmed_Flag != "1")
            {
                result = TR_SN.SN_Mrb_Pass_action(SnObject.ID, UserEMP, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                }
                else
                {
                    if (SnObject.WorkorderNo == null || SnObject.WorkorderNo.Trim().Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000125", new string[] { SnObject.SerialNo }));
                    }
                    else
                    {
                        TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);//這裡如果工單不存在GetWo會報錯
                        result = TR_WO_BASE.UpdateFINISHEDQTYAddOne(SnObject.WorkorderNo, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + SnObject.SerialNo, "UPDATE" }));
                        }
                        TR_WO_BASE.UpdateWoCloseFlag(SnObject.WorkorderNo, Station.SFCDB);//是否需要關閉工單
                    }
                }
            }
            else
            {
                result = TR_SN.SNCBS_Mrb_Pass_actionNotUpdateCompleted(SnObject.ID, UserEMP, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                }
            }

            //添加一筆MRB記錄
            //給new_r_mrb賦值
            New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
            New_R_MRB.SN = SnObject.SerialNo;
            New_R_MRB.WORKORDERNO = SnObject.WorkorderNo;
            New_R_MRB.NEXT_STATION = SnObject.NextStation;
            New_R_MRB.SKUNO = SnObject.SkuNo;
            New_R_MRB.FROM_STORAGE = From_Storage;
            New_R_MRB.TO_STORAGE = To_Storage;
            New_R_MRB.REWORK_WO = "";//空
            New_R_MRB.CREATE_EMP = UserEMP;
            New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
            New_R_MRB.MRB_FLAG = "1";
            New_R_MRB.SAP_FLAG = "0";
            New_R_MRB.EDIT_EMP = UserEMP;
            New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
            result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + SnObject.SerialNo, "ADD" }));
            }
            //添加過站記錄
            result = Convert.ToInt32(TR_SN.RecordPassStationDetail(SnObject.SerialNo, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB));
            if (result <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + SnObject.SerialNo, "ADD" }));
            }
            Station.AddMessage("MES00000063", new string[] { SnObject.SerialNo }, StationMessageState.Pass); //回饋消息到前台
        }

        /// <summary>
        /// 检查SN是否可入OBA開箱抽檢
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnOpenBoxObaSamplingAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN snobj = (SN)sessionSN.Value;
            //if (snobj.NextStation!="CARTON")
            //    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200304162231", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));

            if (Station.SFCDB.ORM.Queryable<R_LOT_DETAIL>().Where(t => t.SN == snobj.SerialNo && t.WORKORDERNO == snobj.WorkorderNo && t.LOT_ID == null
                                                                       && t.PALLET_NO == LotConstants.WaitOba.Ext<EnumNameAttribute>().Description).Any()
                ||
                Station.SFCDB.ORM.Queryable<R_LOT_DETAIL, R_LOT_STATUS>((rld, rls) => rld.LOT_ID == rls.ID && rld.SN == snobj.SerialNo
                                                                                                           && rls.LOT_STATUS_FLAG == LotConstants.LotWaitSampling.Ext<EnumValueAttribute>().Description && rls.SAMPLE_STATION == "OBA").Select((rld, rls) => rld).ToList().Any())
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000093", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
        }

        /// <summary>
        /// 記錄開箱抽檢信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void RecordOpenBoxObaSamplingAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            SN snobj = (SN)sessionSN.Value;

            var res = Station.SFCDB.ORM.Insertable(new R_LOT_DETAIL()
            {
                ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_LOT_DETAIL"),
                SN = snobj.SerialNo,
                WORKORDERNO = snobj.WorkorderNo,
                CREATE_DATE = DateTime.Now,
                STATUS = Paras[1].VALUE.Equals("PASS") ? "1" : "0",
                PALLET_NO = LotConstants.WaitOba.Ext<EnumNameAttribute>().Description,
                EDIT_EMP = Station.LoginUser.EMP_NO,
                EDIT_TIME = DateTime.Now
            }).ExecuteCommand();
            Station.StationMessages.Add(new StationMessage() { State = StationMessageState.UserDefined, Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20200309162330", new string[] { snobj.SerialNo, Paras[1].VALUE }) });
        }

        /// <summary>
        /// 自動匹配開箱抽檢記錄到批次
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LoadOpenBoxSamplingRecordToLot(MESStationBase Station, MESStationInput Input,
            List<R_Station_Action_Para> Paras)
        {
            string packNo = Input.Value.ToString();
            var rLotStatusList = Station.SFCDB.ORM
                .Queryable<R_LOT_PACK, R_LOT_STATUS>((rlp, rls) => rlp.LOTNO == rls.LOT_NO && rlp.PACKNO == packNo && rls.LOT_STATUS_FLAG == LotConstants.LotWaitSampling.Ext<EnumValueAttribute>().Description)
                .Select((rlp, rls) => rls).ToList();

            if (rLotStatusList.FindAll(t => t.CLOSED_FLAG == "1").Count > 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529105040", new string[] { packNo }));
            }
            else if (rLotStatusList.FindAll(t => t.CLOSED_FLAG == "0").Count > 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529111019", new string[] { packNo, rLotStatusList.Find(t => t.CLOSED_FLAG == "0").LOT_NO }));
            }
            else if (rLotStatusList.FindAll(t => t.CLOSED_FLAG != "2").Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529111245", new string[] { packNo }));
            }

            //取该栈板所在批次所有开箱抽检过的SN;
            var targerSnList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN, R_LOT_DETAIL, R_LOT_PACK>(
                    (rp1, rp2, rsp, rs, rld, rlp) => rp1.PARENT_PACK_ID == rp2.ID && rlp.PACKNO == rp2.PACK_NO && rlp.LOTNO == rLotStatusList.FirstOrDefault().LOT_NO
                                                && rp1.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID &&
                                                rs.SN == rld.SN && rs.WORKORDERNO == rld.WORKORDERNO && rld.LOT_ID == null
                                                && rld.PALLET_NO == LotConstants.WaitOba.Ext<EnumNameAttribute>().Description)
                .Select((rp1, rp2, rsp, rs, rld) => rld).ToList();
            if (targerSnList.Count > 0)
            {
                targerSnList.ToList().ForEach(t =>
                {
                    t.LOT_ID = rLotStatusList.FirstOrDefault().LOT_NO;
                    t.PALLET_NO = LotConstants.SnSamplingCompleted.Ext<EnumNameAttribute>().Description;
                    Station.SFCDB.ORM.Updateable(t).ExecuteCommand();
                });

                //Station.SFCDB.ORM.Updateable(targerSnList).ExecuteCommand();

                rLotStatusList.FirstOrDefault().PASS_QTY += targerSnList
                    .FindAll(t => t.STATUS == LotConstants.SnSamplingPass.Ext<EnumValueAttribute>().Description).Count;
                rLotStatusList.FirstOrDefault().FAIL_QTY += targerSnList
                    .FindAll(t => t.STATUS == LotConstants.SnSamplingFail.Ext<EnumValueAttribute>().Description).Count;
                rLotStatusList.FirstOrDefault().EDIT_TIME = DateTime.Now;
                rLotStatusList.FirstOrDefault().EDIT_EMP = Station.LoginUser.EMP_NO;
            }

            if (rLotStatusList.FirstOrDefault().REJECT_QTY <= rLotStatusList.FirstOrDefault().FAIL_QTY)
            {
                rLotStatusList.FirstOrDefault().CLOSED_FLAG = "2";
                rLotStatusList.FirstOrDefault().LOT_STATUS_FLAG = "2";
                //鎖定LOT所有SN
                T_R_SN_LOCK tRSnLock = new T_R_SN_LOCK(Station.SFCDB, Station.DBType);
                tRSnLock.LockSnInOba(rLotStatusList.FirstOrDefault().LOT_NO, Station.SFCDB);
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180615143705", new string[] { rLotStatusList.FirstOrDefault().LOT_NO }), State = StationMessageState.Message });
            }
            else if (rLotStatusList.FirstOrDefault().SAMPLE_QTY <= rLotStatusList.FirstOrDefault().PASS_QTY + rLotStatusList.FirstOrDefault().FAIL_QTY)
            {
                rLotStatusList.FirstOrDefault().CLOSED_FLAG = "2";
                rLotStatusList.FirstOrDefault().LOT_STATUS_FLAG = "1";
                //批量過站;
                List<R_SN> rSnList = new List<R_SN>();
                T_R_SN tRSn = new T_R_SN(Station.SFCDB, Station.DBType);
                R_SN rSn = tRSn.GetDetailBySN(targerSnList.FirstOrDefault().SN, Station.SFCDB);
                rSnList = tRSn.GetObaSnListByLotNo(rLotStatusList.FirstOrDefault().LOT_NO, Station.SFCDB);
                tRSn.LotsPassStation(rSnList, Station.Line, rSn.NEXT_STATION, rSn.NEXT_STATION, Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB); // 過站
                //記錄通過數 ,UPH
                foreach (var snobj in rSnList)
                {
                    tRSn.RecordYieldRate(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                    tRSn.RecordUPH(snobj.WORKORDERNO, 1, snobj.SN, "PASS", Station.Line, snobj.NEXT_STATION, Station.LoginUser.EMP_NO, Station.BU, Station.SFCDB);
                }
                //Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180615144008", new string[] { rLotStatusList.FirstOrDefault().LOT_NO }), State = StationMessageState.Message });
            }
            Station.SFCDB.ORM.Updateable(rLotStatusList.FirstOrDefault()).ExecuteCommand();

            #region 加載界面信息

            if (rLotStatusList.FirstOrDefault().CLOSED_FLAG == "2")//抽檢完清空界面信息
            {
                MESStationInput s = Station.Inputs.Find(t => t.DisplayName == Paras[11].SESSION_TYPE);
                s.DataForUse.Clear();
                Station.StationSession.Clear();
                MESStationInput snInput = Station.Inputs.Find(t => t.DisplayName == "SN");
                MESStationInput packInput = Station.Inputs.Find(t => t.DisplayName == "PACKNO");
                MESStationInput failSnInput = Station.Inputs.Find(t => t.DisplayName == "FailSn");
                MESStationInput scanTypeInput = Station.Inputs.Find(t => t.DisplayName == "ScanType");
                MESStationInput failCodeInput = Station.Inputs.Find(t => t.DisplayName == "FailCode");
                MESStationInput locationInput = Station.Inputs.Find(t => t.DisplayName == "Location");
                MESStationInput failDescInput = Station.Inputs.Find(t => t.DisplayName == "FailDesc");
                packInput.Visable = true;
                snInput.Visable = false;
                scanTypeInput.Visable = false;
                failCodeInput.Visable = false;
                locationInput.Visable = false;
                failDescInput.Visable = false;
                failSnInput.Visable = false;
                Station.StationMessages.Add(new StationMessage() { Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20180615144008", new string[] { rLotStatusList.FirstOrDefault().LOT_NO }), State = StationMessageState.UserDefined });
            }
            else//未抽檢完=>更新界面信息,設置NextInput
            {
                Station.NextInput = Station.Inputs.Find(t => t.DisplayName.Equals(Paras[0].SESSION_TYPE));
                MESStationSession lotNoSession = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[2].SESSION_KEY, ResetInput = Input };
                MESStationSession skuNoSession = new MESStationSession() { MESDataType = Paras[3].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[3].SESSION_KEY, ResetInput = Input };
                MESStationSession aqlSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[4].SESSION_KEY, ResetInput = Input };
                MESStationSession lotQtySession = new MESStationSession() { MESDataType = Paras[5].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[5].SESSION_KEY, ResetInput = Input };
                MESStationSession sampleQtySession = new MESStationSession() { MESDataType = Paras[6].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[6].SESSION_KEY, ResetInput = Input };
                MESStationSession rejectQtySession = new MESStationSession() { MESDataType = Paras[7].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[7].SESSION_KEY, ResetInput = Input };
                MESStationSession sampleQtyWithAqlSession = new MESStationSession() { MESDataType = Paras[8].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[8].SESSION_KEY, ResetInput = Input };
                MESStationSession passQtySession = new MESStationSession() { MESDataType = Paras[9].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[9].SESSION_KEY, ResetInput = Input };
                MESStationSession failQtySession = new MESStationSession() { MESDataType = Paras[10].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[10].SESSION_KEY, ResetInput = Input };

                Station.StationSession.Clear();
                Station.StationSession.Add(lotNoSession);
                Station.StationSession.Add(skuNoSession);
                Station.StationSession.Add(aqlSession);
                Station.StationSession.Add(lotQtySession);
                Station.StationSession.Add(sampleQtySession);
                Station.StationSession.Add(rejectQtySession);
                Station.StationSession.Add(sampleQtyWithAqlSession);
                Station.StationSession.Add(passQtySession);
                Station.StationSession.Add(failQtySession);

                lotNoSession.Value = rLotStatusList.FirstOrDefault().LOT_NO;
                skuNoSession.Value = rLotStatusList.FirstOrDefault().SKUNO;
                aqlSession.Value = rLotStatusList.FirstOrDefault().AQL_TYPE;
                lotQtySession.Value = rLotStatusList.FirstOrDefault().LOT_QTY;
                sampleQtySession.Value = rLotStatusList.FirstOrDefault().SAMPLE_QTY;
                rejectQtySession.Value = rLotStatusList.FirstOrDefault().REJECT_QTY;
                sampleQtyWithAqlSession.Value = rLotStatusList.FirstOrDefault().PASS_QTY + rLotStatusList.FirstOrDefault().FAIL_QTY;
                passQtySession.Value = rLotStatusList.FirstOrDefault().PASS_QTY;
                failQtySession.Value = rLotStatusList.FirstOrDefault().FAIL_QTY;
            }

            #endregion 加載界面信息
        }

        /// <summary>
        /// 增加 RMA 產品 CHECKIN 記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddRMACheckInAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            var SnSession = Station.GetStationSession(Paras[0]);
            var ErrorCodeSession = Station.GetStationSession(Paras[1]);
            var AttachFileSession = Station.GetStationSession(Paras[2]);

            var TRRC = new T_R_RMA_CHECKIN(Station.SFCDB, Station.DBType);
            var AffectedRows = TRRC.AddRmaCheckIn(new R_RMA_CHECKIN()
            {
                ID = TRRC.GetNewID(Station.BU, Station.SFCDB),
                Sn = SnSession.Value.ToString(),
                CheckinReason = ErrorCodeSession.Value.ToString(),
                EditEmp = Station.LoginUser.EMP_NO,
                AttachFile = AttachFileSession.Value.ToString()
            }, Station.SFCDB);

            if (AffectedRows == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_RMA_CHECKIN:" + SnSession.Value.ToString(), "Insert" }));
            }
        }

        /// <summary>
        /// 增加 RMA 產品移動記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddRMAMoveAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SnSession = Station.GetStationSession(Paras[0]);
            MESStationSession LocationSession = Station.GetStationSession(Paras[1]);
            MESStationSession RemarkSession = Station.GetStationSession(Paras[2]);

            T_R_RMA_MOVE TRRM = new T_R_RMA_MOVE(Station.SFCDB, Station.DBType);
            int AffectedRows = TRRM.AddRMAMove(new R_RMA_MOVE()
            {
                ID = TRRM.GetNewID(Station.BU, Station.SFCDB),
                Sn = SnSession.Value.ToString(),
                ToLocation = LocationSession.Value.ToString(),
                Remark = RemarkSession.Value.ToString(),
                EditEmp = Station.LoginUser.EMP_NO
            }, Station.SFCDB);

            if (AffectedRows == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_RMA_MOVE:" + SnSession.Value.ToString(), "Insert" }));
            }
        }

        /// <summary>
        /// 檢查Sn某個工站過站時間是否超過配置的值
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CheckSnStationTime(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionControlTime = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionControlTime == null || sessionControlTime.Value == null)
            {
                if (Paras[1].SESSION_TYPE == "CONTROLTIME" && Paras[1].VALUE != "")
                {
                    Station.StationSession.Add(new MESStationSession()
                    {
                        MESDataType = Paras[1].SESSION_TYPE,
                        SessionKey = Paras[1].SESSION_KEY,
                        Value = Paras[1].VALUE
                    });
                }
            }

            var sndetail = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL, R_SN>((rss, rs) => rss.R_SN_ID == rs.ID).Where(
                (rss, rs) => rs.SN == Input.Value.ToString().Trim() && rs.VALID_FLAG == "1").OrderBy((rss, rs) => rs.EDIT_TIME, OrderByType.Desc).Select((rss, rs) => rss).ToList();
            if (sndetail.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200601105126", new string[] { Input.Value.ToString().Trim(), sessionStation.Value.ToString() }));
            }

            if (sndetail[0].EDIT_TIME < DateTime.Now.AddHours(int.Parse(sessionControlTime.Value.ToString()) * -1))
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200601105607",
                    new string[]
                    {
                        Input.Value.ToString().Trim(), sessionStation.Value.ToString(),
                        sndetail[0].EDIT_TIME.ToString(), sessionControlTime.Value.ToString()
                    }));
            }
        }

        /// <summary>
        /// 增加 RMA 維修記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddRMARepairAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 7)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession SnSession = Station.GetStationSession(Paras[0]);
            MESStationSession ErrorCodeSession = Station.GetStationSession(Paras[1]);
            MESStationSession ReasonCodeSession = Station.GetStationSession(Paras[2]);
            MESStationSession ActionCodeSession = Station.GetStationSession(Paras[3]);
            MESStationSession RepairLocationSession = Station.GetStationSession(Paras[4]);
            MESStationSession RemarkSession = Station.GetStationSession(Paras[5]);
            MESStationSession FileSession = Station.GetStationSession(Paras[6]);

            T_R_RMA_REPAIR TRRR = new T_R_RMA_REPAIR(Station.SFCDB, Station.DBType);
            int AffectedRows = TRRR.AddRMARepair(new R_RMA_REPAIR()
            {
                ID = TRRR.GetNewID(Station.BU, Station.SFCDB),
                Sn = SnSession.Value.ToString(),
                ErrorCode = ErrorCodeSession.Value.ToString(),
                ReasonCode = ReasonCodeSession.Value.ToString(),
                RepairCode = ActionCodeSession.Value.ToString(),
                RepairLocation = RepairLocationSession.Value.ToString(),
                Remark = RemarkSession.Value.ToString(),
                FileLocation = FileSession.Value.ToString(),
                EditEmp = Station.LoginUser.EMP_NO
            }, Station.SFCDB);

            if (AffectedRows == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_RMA_REPAIR:" + SnSession.Value.ToString(), "Insert" }));
            }
        }

        /// <summary>
        /// 給工單或者SN添加標註信息，記錄在相應的擴展表中
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AddProductNotes(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string type = "Notes";
            int AffectedRows = 0;
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            T_R_WO_BASE TRWB = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_R_SN TRS = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_SN_EX TRSE = new T_R_SN_EX(Station.SFCDB, Station.DBType);
            T_R_WO_BASE_EX TRWBE = new T_R_WO_BASE_EX(Station.SFCDB, Station.DBType);

            MESStationSession ProductSession = Station.GetStationSession(Paras[0]);
            MESStationSession NotesSession = Station.GetStationSession(Paras[1]);

            R_SN RSN = TRS.GetSN(ProductSession.Value.ToString(), Station.SFCDB);
            if (RSN == null)
            {
                R_WO_BASE RWB = TRWB.GetWoByWoNo(ProductSession.Value.ToString(), Station.SFCDB);
                if (RWB != null)
                {
                    AffectedRows = TRWBE.InsertWoBaseEx(RWB.ID, type, NotesSession.Value.ToString(), Station.SFCDB);
                    if (AffectedRows == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE_EX:" + ProductSession.Value.ToString(), "Insert" }));
                    }
                }
            }
            else
            {
                AffectedRows = TRSE.InsertSnEx(RSN.ID, type, NotesSession.Value.ToString(), Station.SFCDB);
                if (AffectedRows == 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_EX:" + ProductSession.Value.ToString(), "Insert" }));
                }
            }
        }

        /// <summary>
        /// 退SILoading,請不要隨意給別添加權限
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReturnSILoadingAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionReturnStation = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (sessionReturnStation == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            List<string> listEmp = Paras[2].VALUE.ToString().Trim() == "" ? new List<string>() : Paras[2].VALUE.ToString().Trim().Split(',').ToList<string>();
            SN snObject = (SN)sessionSN.Value;
            string returnStation = sessionReturnStation.Value.ToString();
            int result;
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(Station.SFCDB, Station.DBType);
            //只處理SILOADING工站
            if (!returnStation.Equals("SILOADING"))
            {
                return;
            }
            if (!listEmp.Contains(Station.LoginUser.EMP_NO))
            {
                //沒有權限
                throw new Exception("No Permission!");
            }
            else
            {
                List<C_ROUTE_DETAIL> routeDetailList = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.RouteID, Station.SFCDB);
                C_ROUTE_DETAIL SItStation = routeDetailList.Where(r => r.STATION_NAME == "SILOADING").FirstOrDefault();
                if (SItStation == null)
                {
                    throw new Exception($@"SILOADING Not In {snObject.SerialNo}'s Route ");
                }
                C_ROUTE_DETAIL SINextStation = routeDetailList.Where(r => r.SEQ_NO > SItStation.SEQ_NO).OrderBy(r => r.SEQ_NO).FirstOrDefault();
                if (snObject.NextStation != SINextStation.STATION_NAME)
                {
                    throw new Exception($@"{snObject.NextStation} Can Not Return SILOADING");
                }

                //update r_sn VALID_FLAG=0
                string emp_no = Station.LoginUser.EMP_NO;
                string workorderno = "RSI_" + snObject.WorkorderNo;
                DateTime sysdt = t_r_sn.GetDBDateTime(Station.SFCDB);
                result = Station.SFCDB.ORM.Updateable<R_SN>()
                    .UpdateColumns(r => new R_SN { WORKORDERNO = workorderno, VALID_FLAG = "0", EDIT_EMP = emp_no, EDIT_TIME = sysdt })
                    .Where(r => r.ID == snObject.ID).ExecuteCommand();
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + snObject.SerialNo, "UPDATE" }));
                }

                //update workorder input qty
                result = Station.SFCDB.ORM.Updateable<R_WO_BASE>().UpdateColumns(r => new R_WO_BASE { INPUT_QTY = r.INPUT_QTY - 1 })
                    .Where(r => r.WORKORDERNO == snObject.WorkorderNo).ExecuteCommand();
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + snObject.SerialNo, "UPDATE" }));
                }
            }
        }

        /// <summary>
        /// R_FUNCTION_CONTROL的配置OLD_CARRIER_CHECK,載具使用次數+1
        /// add by zwx 2020-4-18
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void OldCarrierIncreaseCount(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            OleExec SFCDB = Station.SFCDB;
            SKU sku = (SKU)SNSession.Value;
            int UseTimes = 0;
            string TESTSTATE = string.Empty;
            T_R_FUNCTION_CONTROL_EX rFunctionControlex = null;
            T_R_FUNCTION_CONTROL rFunctionControl = null;
            T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_F_CONTROL> Lrfc = TRFC.GetListByFcv("OLD_CARRIER_CHECK", "SKUNO_CARRIER_LINK", sku.SkuNo, SFCDB);

            if (Lrfc.Count > 0)
            {
                List<R_FUNCTION_CONTROL_NewList> UsetimesList = TRFC.Get2ExListbyVarValue("OLD_CARRIER_CHECK", "CARRIER_USE_TIMES", Lrfc[0].EXTVAL, SFCDB);
                if (UsetimesList.Count == 1)
                {
                    try
                    {
                        rFunctionControlex = new T_R_FUNCTION_CONTROL_EX(SFCDB, DB_TYPE_ENUM.Oracle);
                        rFunctionControl = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);
                        R_F_CONTROL_EX e = rFunctionControlex.ByDETAIL_ID_SEQ(UsetimesList[0].ID, 1, SFCDB);
                        UseTimes = Convert.ToInt32(UsetimesList[0].EXTVAL2);
                        e.VALUE = (UseTimes + 1).ToString();
                        rFunctionControlex.AddOrUpdateRFCEX("UPDATE", e, SFCDB);

                        List<R_F_CONTROL> CumulativeC = TRFC.GetListByFcv("OLD_CARRIER_CHECK", "CARRIERUSE_CUMULATIVE_COUNT", Lrfc[0].EXTVAL, SFCDB);
                        if (CumulativeC.Count != 0)
                        {
                            CumulativeC[0].EXTVAL = (Convert.ToInt32(CumulativeC[0].EXTVAL) + 1).ToString();
                            CumulativeC[0].EDITBY = "SYSTEM";
                            CumulativeC[0].EDITTIME = DateTime.Now;
                            rFunctionControl.AddOrUpdateRFC("UPDATE", CumulativeC[0], SFCDB);
                        }
                        else
                        {
                            Row_R_FUNCTION_CONTROL enew = (Row_R_FUNCTION_CONTROL)rFunctionControl.NewRow();
                            enew.ID = rFunctionControl.GetNewID(Station.BU, SFCDB);
                            enew.FUNCTIONNAME = "OLD_CARRIER_CHECK";
                            enew.CATEGORY = "CARRIERUSE_CUMULATIVE_COUNT";
                            enew.VALUE = Lrfc[0].EXTVAL;
                            enew.EXTVAL = e.VALUE;
                            enew.CONTROLFLAG = "Y";
                            enew.CREATEBY = "SYSTEM";
                            enew.CREATETIME = DateTime.Now;
                            enew.EDITBY = "SYSTEM";
                            enew.EDITTIME = DateTime.Now;
                            enew.FUNCTIONTYPE = "NOSYSTEM";
                            SFCDB.ExecSQL(enew.GetInsertString(DB_TYPE_ENUM.Oracle));
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200417172400", new string[] { }));
                }
            }
        }

        /// <summary>
        /// 更改未綁定的SN的Keypart信息
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UpdateSNKPPAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession sessionInputType = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionInputType == null || sessionInputType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            MESStationSession sessionInputString = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionInputString == null || sessionInputString.Value == null || sessionInputString.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                //Station.SFCDB.CMD_TIME_OUT
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
                else if (inputType.Equals("WO"))
                {
                    snList = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE>((sn, wo) => sn.WORKORDERNO == wo.WORKORDERNO)
                        .Where((sn, wo) => wo.WORKORDERNO == inputString && sn.COMPLETED_FLAG != "1" && sn.SHIPPED_FLAG != "1" && (sn.SCRAPED_FLAG != "1" || SqlSugar.SqlFunc.IsNullOrEmpty(sn.SCRAPED_FLAG)) && sn.VALID_FLAG == "1")
                        .Select((sn, wo) => sn).ToList();
                    if (snList.Count == 0)
                    {
                        var haveSn = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == inputString && t.VALID_FLAG == "1").Select(t => t.SN).Any();
                        if (haveSn)
                        {
                            throw new MESReturnMessage($@"WO:{inputString} All Sn has completed or shipped, Don't need to update keypart!");
                        }
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180529094259"));
                }
                SN snObject = new SN();
                if (inputType.Equals("WO"))
                {
                    snObject.UpdateSNKP(inputString, snList, Station);
                }
                else
                {
                    snObject.UpdateSNKP("", snList, Station);
                }

                //更新完R_SN_KP表後根據條件自動生成PPIDSN並綁定(只有UpdateKeypart需要在這寫,其他如Loading、Rework可調用工站Action)
                if (Station.BU.Equals("VNDCN"))
                {
                    for (int i = 0; i < snList.Count; i++)
                    {
                        //獲取SN對象
                        SN _SN = new SN(snList[i].SN, Station.SFCDB, DB_TYPE_ENUM.Oracle);
                        //獲取R_SN_KP List對象
                        List<R_SN_KP> _SNKPList = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == _SN.SerialNo && t.VALID_FLAG == 1).ToList();
                        //獲取R_SN_KP ppid對象
                        R_SN_KP _PPIDKP = _SNKPList.Find(t => t.SCANTYPE == "PPID S/N");
                        //如果SN存在PPID S/N類型Kp且還未綁定value
                        if (_PPIDKP != null && string.IsNullOrEmpty(_PPIDKP.VALUE))
                        {
                            string ppidSN = _SN.AutoPPIDSNMaker(Station, _SN, _PPIDKP);
                            if (ppidSN.StartsWith("ERROR"))
                            {
                                throw new Exception(ppidSN);
                            }
                            _PPIDKP.VALUE = ppidSN;
                            //_PPIDKP.EXKEY1 = "AutoLink";
                            _PPIDKP.EDIT_TIME = Station.GetDBDateTime();
                            _PPIDKP.EDIT_EMP = "SYSTEM";

                            Station.SFCDB.ORM.Updateable(_PPIDKP).Where(t => t.ID == _PPIDKP.ID).ExecuteCommand();
                        }
                        R_SN_KP _STKP = _SNKPList.Find(t => t.SCANTYPE == "ST S/N");
                        //如果SN存在ST S/N類型Kp且還未綁定value
                        if (_STKP != null && string.IsNullOrEmpty(_STKP.VALUE))
                        {
                            string stSN = _SN.AutoSTSNLink(Station, _SN, "UpdateSNKP");
                            if (stSN.StartsWith("ERROR"))
                            {
                                throw new Exception(stSN);
                            }
                            _STKP.VALUE = stSN;
                            _STKP.EXKEY1 = "AutoLink";
                            _STKP.EDIT_TIME = Station.GetDBDateTime();
                            _STKP.EDIT_EMP = "SYSTEM";

                            Station.SFCDB.ORM.Updateable(_PPIDKP).Where(t => t.ID == _STKP.ID).ExecuteCommand();
                        }
                    }
                }
                Station.AddMessage("MES00000063", new string[] { sessionInputString.Value.ToString() }, StationMessageState.Pass);
                Station.StationMessages.Add(new StationMessage
                {
                    Message = $@"{sessionInputString.Value.ToString()},Update KP Finish!",
                    State = StationMessageState.Pass
                });
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
        public static void SnWeight(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            UIInputData IW = new UIInputData() { MustConfirm = true, Timeout = 30000, IconType = IconType.None, UIArea = new string[] { "30%", "25%" }, Message = "請稱重!", Tittle = "請稱重", Type = UIInputType.Weight, Name = "WEIGHT", ErrMessage = "請稱重!" };
            var weightdata = IW.GetUiInput(Station.API, UIInput.Normal, Station);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SetWeight(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SnObject = (SN)SNSession.Value;

            var weightconfig = Station.SFCDB.ORM.Queryable<C_WEIGHT>()
                .Where(t => t.SKUNO == SnObject.SkuNo && t.STATION == Station.StationName && t.ENABLEFLAG == "Y").ToList();
            if (weightconfig.Count > 0)
            {
                UIInputData W = new UIInputData()
                {
                    MustConfirm = true,
                    Timeout = 30000,
                    IconType = IconType.None,
                    UIArea = new string[] { "30%", "25%" },
                    //Message = "請稱重",
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153928"),
                    //Tittle = "請稱重!            (請打開MESHelpe之後關掉本對話框重試.)",
                    Tittle = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154119"),
                    Type = UIInputType.Weight,
                    Name = "WEIGHT",
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153928"),
                };
                var weightdata = double.Parse(W.GetUiInput(Station.API, UIInput.Normal, Station).ToString());
                //暫時只支持計算整體重量，區分包材及不同MPN的有空再寫
                if (weightconfig.FindAll(t =>
                        double.Parse(t.MAXWEIGHT) >= weightdata && double.Parse(t.MINWEIGHT) <= weightdata).Count == 0)
                {
                    var ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20200511174534", new string[] { SnObject.SerialNo, weightdata.ToString(), weightconfig[0].MINWEIGHT, weightconfig[0].MAXWEIGHT });
                    throw new MESReturnMessage(ErrMessage);
                }
                else
                {
                    Station.SFCDB.ORM.Insertable(new R_WEIGHT()
                    {
                        ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_WEIGHT"),
                        SNID = SnObject.ID,
                        STATION = Station.StationName,
                        WEIGHT = weightdata.ToString(),
                        CREATETIME = DateTime.Now,
                        CREATEBY = Station.LoginUser.EMP_NO
                    }).ExecuteCommand();
                }
            }
        }
        public static void SetCartonWeight(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SnObject = (SN)SNSession.Value;

            var weightconfig = Station.SFCDB.ORM.Queryable<C_WEIGHT>()
                .Where(t => t.SKUNO == SnObject.SkuNo && t.STATION == Station.StationName && t.ENABLEFLAG == "Y").ToList();
            if (weightconfig.Count > 0)
            {
                UIInputData W = new UIInputData()
                {
                    MustConfirm = true,
                    Timeout = 30000,
                    IconType = IconType.None,
                    UIArea = new string[] { "30%", "25%" },
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153928"),
                    Tittle = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154119"),
                    Type = UIInputType.Weight,
                    Name = "WEIGHT",
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814153928"),
                };
                //if (Station.BU == "VNDCN")
                //{
                //    W.Type = UIInputType.String;
                //};
                var weightdata = double.Parse(W.GetUiInput(Station.API, UIInput.Normal, Station).ToString());
                //暫時只支持計算整體重量，區分包材及不同MPN的有空再寫
                if (weightconfig.FindAll(t =>
                        double.Parse(t.MAXWEIGHT) >= weightdata && double.Parse(t.MINWEIGHT) <= weightdata).Count == 0)
                {
                    var ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20200511174534", new string[] { SnObject.SerialNo, weightdata.ToString(), weightconfig[0].MINWEIGHT, weightconfig[0].MAXWEIGHT });
                    throw new MESReturnMessage(ErrMessage);
                }
                else
                {
                    var cartonObj = Station.SFCDB.ORM.Queryable<R_SN_PACKING, R_PACKING>((r, p) => r.PACK_ID == p.ID)
                        .Where((r, p) => r.SN_ID == SnObject.ID).Select((r, p) => p).ToList().FirstOrDefault();
                    if (cartonObj == null)
                    {
                        throw new Exception($@"{SnObject.SerialNo} Not Packed!");
                    }
                    if (cartonObj.CLOSED_FLAG == "0")
                    {
                        throw new Exception($@"The CARTON NUMBER[{cartonObj.PACK_NO}] OF {SnObject.SerialNo} Not Closed!");
                    }
                    bool bWeight = Station.SFCDB.ORM.Queryable<R_WEIGHT>().Where(r => r.SNID == cartonObj.ID && r.STATION == Station.StationName).Any();
                    DateTime sysdate = Station.SFCDB.ORM.GetDate();
                    if (bWeight)
                    {
                        //throw new Exception($@"The CARTON NUMBER[{cartonObj.PACK_NO}] OF {SnObject.SerialNo} Have Already Been Weighted!");
                        Station.SFCDB.ORM.Updateable<R_WEIGHT>()
                            .SetColumns(r => new R_WEIGHT() { WEIGHT = weightdata.ToString(), CREATETIME = sysdate, CREATEBY = Station.LoginUser.EMP_NO })
                            .Where(r => r.SNID == cartonObj.ID && r.STATION == Station.StationName)
                            .ExecuteCommand();
                    }
                    else
                    {
                        Station.SFCDB.ORM.Insertable(new R_WEIGHT()
                        {
                            ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_WEIGHT"),
                            SNID = cartonObj.ID,
                            STATION = Station.StationName,
                            WEIGHT = weightdata.ToString(),
                            CREATETIME = sysdate,
                            CREATEBY = Station.LoginUser.EMP_NO
                        }).ExecuteCommand();
                    }
                }
            }
            else
            {
                throw new Exception("This Sku not config weight!");
            }
        }
        /// <summary>
        /// Allpart自动带料到KP：
        /// 条件: 上传KP时scantype=="AUTOAP" 的
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AutokeypartFromAllpart(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            SN SnObject = null;
            //獲取到 SN 對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            if (Station.StationName.ToUpper().IndexOf("LOADING") > -1)
            {
                SnObject = new SN(SNSession.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            }
            else
            {
                SnObject = (SN)SNSession.Value;
            }

            var snstationkp = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SnObject.ID && t.VALID_FLAG == 1 && t.STATION == Station.StationName && t.SCANTYPE == "AUTOAP").ToList();

            if (snstationkp.Count > 0)
            {
                var apdatacathe = Station.APDB.ORM
                    .Queryable<R_TR_PRODUCT_DETAIL, R_TR_CODE_DETAIL, C_SMT_AP_LOCATION>((rp, rt, cs) =>
                        new object[] {
                            JoinType.Inner,rp.TR_CODE == rt.TR_CODE,
                            JoinType.Left,rt.SMT_CODE == cs.SMT_CODE && rt.KP_NO == cs.KP_NO
                        })
                    .Where((rp, rt, cs) => rp.P_SN == SnObject.SerialNo)
                    .GroupBy((rp, rt, cs) => new { rt.DATE_CODE, rt.LOT_CODE, rt.MFR_KP_NO, rt.KP_NO, cs.LOCATION })
                    .Select((rp, rt, cs) => new { rt.DATE_CODE, rt.LOT_CODE, rt.MFR_KP_NO, rt.KP_NO, cs.LOCATION }).ToList();
                foreach (var kpitem in snstationkp)
                {
                    if (kpitem.VALUE != null && kpitem.VALUE.Length > 0)
                    {
                        continue;
                    }
                    if (string.IsNullOrEmpty(kpitem.LOCATION))
                    {
                        throw new Exception("keypart location is null,can't not get allpart info!");
                    }
                    var apkpitem = apdatacathe.FindAll(t => t.KP_NO == kpitem.PARTNO);
                    if (kpitem.LOCATION.Length > 0)
                    {
                        apkpitem =
                            apkpitem.FindAll(t => t.LOCATION == kpitem.LOCATION.Substring(0, kpitem.LOCATION.Length + 1 - (kpitem.LOCATION.LastIndexOf('-')))).Count > 0
                                ? apkpitem.FindAll(t => t.LOCATION == kpitem.LOCATION.Substring(0, kpitem.LOCATION.Length + 1 - (kpitem.LOCATION.LastIndexOf('-'))))
                                : apkpitem;
                    }

                    if (apkpitem.Count == 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20200616184157",
                            new string[] { kpitem.PARTNO, kpitem.LOCATION }));
                    }

                    var apkpitemdclsit = apkpitem.Select(t => new { t.DATE_CODE, t.LOT_CODE, t.MFR_KP_NO }).Distinct();

                    //LOCATION StartsWith("S.P ")為錫膏，不改變其MPN
                    if (!kpitem.LOCATION.StartsWith("S.P "))
                    {
                        kpitem.MPN = string.Empty;
                    }
                    foreach (var apitem in apkpitemdclsit)
                    {
                        kpitem.EDIT_TIME = Station.SFCDB.ORM.GetDate();//DateTime.Now;
                        if (!kpitem.LOCATION.StartsWith("S.P "))
                        {
                            kpitem.MPN += !string.IsNullOrEmpty(kpitem.MPN) && apitem.MFR_KP_NO != "" ? "," + $@"{apitem.MFR_KP_NO}" : "" + $@"{apitem.MFR_KP_NO}";
                        }
                        kpitem.VALUE += kpitem.VALUE != null && kpitem.VALUE.Length > 0 && $@"{apitem.DATE_CODE}/{apitem.LOT_CODE}".Length > 0 ? "," + $@"{apitem.DATE_CODE}/{apitem.LOT_CODE}" : "" + $@"{apitem.DATE_CODE}/{apitem.LOT_CODE}";
                    }
                    Station.SFCDB.ORM.Updateable(kpitem).ExecuteCommand();
                }
            }
        }

        /// <summary>
        /// xray 超時鎖工單
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void LockWoByXray(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            // 獲取工單
            WorkOrder wo = new WorkOrder();
            string strWO = string.Empty;
            MESStationSession Wo_Session = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (Wo_Session != null)
            {
                if (Wo_Session.Value != null)
                {
                    wo = (WorkOrder)Wo_Session.Value;
                    if (wo.WorkorderNo == null || wo.WorkorderNo.Length <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            strWO = wo.ToString();

            // 獲取機種信息
            string strSKU = null;
            MESStationSession skuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (skuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            strSKU = skuSession.Value.ToString();

            // *xray 按機種卡關*
            T_R_FUNCTION_CONTROL TRFC_SKU = new T_R_FUNCTION_CONTROL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            List<R_F_CONTROL> Lrfc_SKU = TRFC_SKU.GetListByFcv("XRAY_CHECK_SKU", "XRAY_CHECK_SKU", strSKU, Station.SFCDB);
            if (Lrfc_SKU.Count == 0) //沒找到配置，不管控
            {
                return;
            }

            // *判斷工單是否被鎖*
            T_R_SN_LOCK t_r_sn_lock = new T_R_SN_LOCK(Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (t_r_sn_lock.IsLock("", "WO", "", strWO, "", "MISSXRAYTEST", Station.SFCDB))
            {
                //throw new Exception($@"工單 {strWO} 已被鎖定，需掃 XRAY 解鎖才能過站");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154147", new string[] { strWO }));
            }

            string sql = "SELECT SYSDATE FROM DUAL";
            DateTime DBTime = (DateTime)Station.SFCDB.ExecSelectOneValue(sql);

            // *開始判斷*
            bool recordExist = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                .Where(r => r.WORKORDERNO == strWO && r.STATION_NAME.Substring(0, 3) == "AOI" && r.VALID_FLAG == "1")
                .Any();
            if (!recordExist)
            {
                return;
            }
            else
            {

                DateTime firstRecordTime = Convert.ToDateTime(Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                        .Where(r => r.WORKORDERNO == strWO && r.STATION_NAME.Substring(0, 3) == "AOI" && r.VALID_FLAG == "1")
                        .OrderBy(r => r.EDIT_TIME)
                        .Select(r => r.EDIT_TIME)
                        .First().ToString());


                if (DBTime > firstRecordTime.AddHours(1))
                {
                    int xRayCount = Station.SFCDB.ORM.Queryable<R_XRAY_HEAD_HWD, R_XRAY_DETAIL_HWD, R_PANEL_SN>((h, d, p) => h.ID == d.XRAYID && d.SNID == p.PANEL)
                        .Where((h, d, p) => h.RESULT == "PASS" && p.WORKORDERNO == strWO && h.EDIT_TIME > DBTime.AddHours(-2))
                        .GroupBy(h => h.ID)
                        .Select(h => h.ID)
                        .Count();
                    if (xRayCount >= 2)
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }

            }

            // *以上無掃XRAY定義的要求記錄，則開始工單鎖定*
            var s1 = Station.SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == strWO && t.LOCK_STATUS == "1" && t.LOCK_REASON == "MISSXRAYTEST").ToList();
            if (s1.Count == 0)
            {
                var res = Station.SFCDB.ORM.Insertable(new R_SN_LOCK()
                {
                    ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_SN_LOCK"),
                    TYPE = "WO",
                    WORKORDERNO = strWO,
                    LOCK_STATION = Station.StationName,
                    LOCK_STATUS = "1", //鎖定
                    LOCK_REASON = "MISSXRAYTEST",
                    LOCK_EMP = "AUTOUSER",
                    LOCK_TIME = DBTime,
                    UNLOCK_REASON = "",
                    UNLOCK_EMP = "",
                    UNLOCK_TIME = null
                }).ExecuteCommandAsync(); //異步執行，同步的話不知道為什麽插不進

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000263", new string[] { strWO }));
            }
        }

        public static void ReworkAddSapStationRecord(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionReworkStation = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionReworkStation == null || sessionReworkStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            WorkOrder woObject = (WorkOrder)sessionWO.Value;
            SN snObject = (SN)sessionSN.Value;
            string reworkStation = sessionReworkStation.Value.ToString();
            string controlName = Paras[3].VALUE.Trim();
            string controlCategory = Paras[4].VALUE.Trim();
            if (controlName == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }
            if (controlCategory == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[4].SESSION_TYPE + Paras[4].SESSION_KEY }));
            }
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            List<R_F_CONTROL> listControl = t_r_function_control.GetListByFcv(controlName, controlCategory, Station.SFCDB);
            bool bBackflush = listControl.Where(r => woObject.SkuNO.StartsWith(r.VALUE)).Any();
            if (bBackflush)
            {
                T_C_SAP_STATION_MAP t_c_sap_station_map = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
                List<C_SAP_STATION_MAP> listMap = t_c_sap_station_map.GetSAPStationMapBySkuOrderBySAPCodeASC(woObject.SkuNO, Station.SFCDB);
                if (listMap.Count == 0)
                {
                    throw new MESReturnMessage(woObject.SkuNO + " Not Setting SAP BackFlush Station!");
                }
                string backflushStation = listMap.FirstOrDefault().STATION_NAME;
                if (backflushStation != reworkStation)
                {

                    T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                    List<C_ROUTE_DETAIL> listRoute = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.RouteID, Station.SFCDB);
                    if (listRoute.Count == 0)
                    {
                        throw new MESReturnMessage("Get SN Route Detail Error!");
                    }
                    C_ROUTE_DETAIL backflushStationDetail = listRoute.Find(r => r.STATION_NAME == backflushStation);
                    if (backflushStationDetail == null)
                    {
                        throw new MESReturnMessage("Backflush Station Not In SN Route");
                    }
                    C_ROUTE_DETAIL reworkStationDetail = listRoute.Find(r => r.STATION_NAME == reworkStation);
                    if (reworkStationDetail == null)
                    {
                        throw new MESReturnMessage("Rework Station Not In SN Route");
                    }
                    if (backflushStationDetail.SEQ_NO < reworkStationDetail.SEQ_NO)
                    {
                        T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                        t_r_sn.RecordPassStationDetail(snObject.SerialNo, Station.Line, backflushStation, backflushStation, Station.BU, Station.SFCDB);
                    }
                }
            }

        }
        /// <summary>
        /// 參考VT REWORK 出來添加一筆第一個拋站點的過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkAddSapStationRecord_DCN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionWO = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWO == null || sessionWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            MESStationSession sessionReworkStation = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (sessionReworkStation == null || sessionReworkStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            WorkOrder woObject = (WorkOrder)sessionWO.Value;
            SN snObject = (SN)sessionSN.Value;
            string reworkStation = sessionReworkStation.Value.ToString();

            T_C_SAP_STATION_MAP t_c_sap_station_map = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
            List<C_SAP_STATION_MAP> listMap = t_c_sap_station_map.GetSAPStationMapBySkuOrderBySAPCodeASC(woObject.SkuNO, Station.SFCDB);
            if (listMap.Count == 0)
            {
                throw new MESReturnMessage(woObject.SkuNO + " Not Setting SAP BackFlush Station!");
            }
            string backflushStation = listMap.FirstOrDefault().STATION_NAME;
            if (backflushStation != reworkStation)
            {

                T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
                List<C_ROUTE_DETAIL> listRoute = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.RouteID, Station.SFCDB);
                if (listRoute.Count == 0)
                {
                    throw new MESReturnMessage("Get SN Route Detail Error!");
                }
                C_ROUTE_DETAIL backflushStationDetail = listRoute.Find(r => r.STATION_NAME == backflushStation);
                if (backflushStationDetail == null)
                {
                    throw new MESReturnMessage("Backflush Station Not In SN Route");
                }
                C_ROUTE_DETAIL reworkStationDetail = listRoute.Find(r => r.STATION_NAME == reworkStation);
                if (reworkStationDetail == null)
                {
                    throw new MESReturnMessage("Rework Station Not In SN Route");
                }
                if (backflushStationDetail.SEQ_NO < reworkStationDetail.SEQ_NO)
                {
                    T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
                    t_r_sn.RecordPassStationDetail(snObject.SerialNo, Station.Line, backflushStation, backflushStation, Station.BU, Station.SFCDB);
                }
            }


        }


        public static void ReworkAutoPassTestStation(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSN == null || sessionSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionReworkStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionReworkStation == null || sessionReworkStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            SN snObject = (SN)sessionSN.Value;
            string reworkStation = sessionReworkStation.Value.ToString();
            string strSN = snObject.baseSN.SN;
            string RouteID = snObject.baseSN.ROUTE_ID;
            T_R_SN table = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB _MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            var mrbList = _MRB.GetMrbBySN(strSN, Station.SFCDB);
            if (mrbList.Count > 0)
            {
                //var mrbintime = Station.SFCDB.ORM.Queryable<R_MRB>().Where(t => t.SN == strSN && t.MRB_FLAG == "1").Select(t => t.CREATE_TIME).ToList();
                //DateTime MrbTime = mrbintime[0].Value;

                DateTime MrbTime = (DateTime)mrbList[0].CREATE_TIME;
                var station_name = Station.SFCDB.ORM.Queryable<R_TEST_RECORD, C_ROUTE_DETAIL>((r, c) => r.MESSTATION == c.STATION_NAME).Where(r => r.SN == strSN && r.EDIT_TIME > MrbTime && r.STATE == "PASS").OrderBy((r, c) => c.SEQ_NO).Select((r, c) => c).ToList();
                if (station_name.Count > 0)
                {
                    for (int i = 0; i < station_name.Count; i++)
                    {
                        table.PassStation(strSN, Station.Line, station_name[i].STATION_NAME, "AUTOPASS", Station.BU, "PASS", Station.LoginUser.EMP_NO, Station.SFCDB);
                    }
                }
            }
        }


        public static void SNPreprocessor(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            //--Add Data Record To C_SKU_DETAIL
            //--Category = SN-PREPROCESSOR
            //--Category_name = ADD-SUFFIX\ADD-PREFIX\REMOVE-SUFFIX\REMOVE-PREFIX\REPLACE
            //--Value =‘’
            //--Extend =‘’When The Value Of Category_name is 'REPLACE' Fill The New Value This Column
            MESStationSession SKUSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SKUSession == null || SKUSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            var SKUNO = SKUSession.Value.ToString();
            var SerialNO = SNSession.Value.ToString();
            //var categorylist = Station.SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == SKUNO && t.CATEGORY == "SN-PREPROCESSOR").ToList();
            //for (int i = 0; i < categorylist.Count; i++)
            //{
            //    switch (categorylist[i].CATEGORY_NAME)
            //    {
            //        case "REPLACE":
            //            SerialNO = SerialNO.Replace(categorylist[i].VALUE, categorylist[i].EXTEND);
            //            break;
            //        case "ADD-SUFFIX":
            //            SerialNO = SerialNO.Insert(SerialNO.Length, categorylist[i].VALUE);
            //            break;
            //        case "ADD-PREFIX":
            //            SerialNO = SerialNO.Insert(0, categorylist[i].VALUE);
            //            break;
            //        case "REMOVE-SUFFIX":
            //            if (SerialNO.EndsWith(categorylist[i].VALUE))
            //            {
            //                SerialNO = SerialNO.Remove(SerialNO.Length - categorylist[i].VALUE.Length);
            //            }
            //            break;
            //        case "REMOVE-PREFIX":
            //            if (SerialNO.StartsWith(categorylist[i].VALUE))
            //            {
            //                SerialNO = SerialNO.Remove(0, categorylist[i].VALUE.Length);
            //            }
            //            break;
            //        default:
            //            break;
            //    }
            //}
            T_C_SKU_DETAIL c_sku_detail = new T_C_SKU_DETAIL(Station.SFCDB, Station.DBType);
            SerialNO = c_sku_detail.SNPreprocessor(Station.SFCDB, SKUNO, SerialNO, Station.StationName);
            Input.Value = SerialNO;
            SNSession.Value = SerialNO;
        }

        /// <summary>
        /// 待確認是否替換掉舊SN的所有記錄，未啟用
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void SNReworkToNewSkuAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession sessionOldSN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionOldSN == null || sessionOldSN.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionReworkWO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionReworkWO == null || sessionReworkWO.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            T_R_FUNCTION_CONTROL t_r_function_control = new T_R_FUNCTION_CONTROL(Station.SFCDB, Station.DBType);
            T_R_SN t_r_sn = new T_R_SN(Station.SFCDB, Station.DBType);
            T_C_SKU t_c_sku = new T_C_SKU(Station.SFCDB, Station.DBType);
            List<R_F_CONTROL> list_control = new List<R_F_CONTROL>();
            C_SKU skuObj = null;
            WorkOrder reworkWoObj = (WorkOrder)sessionReworkWO.Value;
            SN oldSNObj = (SN)sessionOldSN.Value;
            //Is Wait To Replace
            R_SN_REPLACE wait_replace = Station.SFCDB.ORM.Queryable<R_SN_REPLACE>()
                .Where(r => r.OLDSN == oldSNObj.SerialNo && r.FLAG == "0" && r.STATION == Station.StationName && SqlFunc.IsNullOrEmpty(r.NEWSN))
                .ToList().FirstOrDefault();
            DateTime sysdate = Station.GetDBDateTime();
            int result = 0;
            string new_sn = "";
            string new_skuno = "";
            if (wait_replace != null)
            {
                //Check New Skuno
                list_control = t_r_function_control.GetListByFcv("REWORK_TO_NEW_SKUNO", "SKUNO", Station.SFCDB);
                if (!list_control.Exists(r => r.VALUE == oldSNObj.SkuNo && !string.IsNullOrEmpty(r.EXTVAL)))
                {
                    throw new MESReturnMessage($@"{oldSNObj.SerialNo} Need Rework To New Skuno,But {oldSNObj.SkuNo} Not Setting Rework Link");
                }
                new_skuno = list_control.Find(r => r.VALUE == oldSNObj.SkuNo && !string.IsNullOrEmpty(r.EXTVAL)).EXTVAL;

                //Check SN Rule
                skuObj = t_c_sku.GetBySKU(new_skuno, Station.SFCDB);
                if (skuObj == null)
                {
                    throw new MESReturnMessage($@"{new_skuno} Not Exists!");
                }
                if (string.IsNullOrEmpty(skuObj.SN_RULE))
                {
                    throw new MESReturnMessage($@"{skuObj} Sn Rule Is Null or Empty!");
                }

                UIInputData UI_New_SN = new UIInputData()
                {
                    IconType = IconType.None,
                    UIArea = new string[] { "30%", "45%" },
                    Message = "New SN",
                    Tittle = "Please Input New SN",
                    Type = UIInputType.String,
                    Name = "NewSN",
                    ErrMessage = "Cancel"
                };
                var new_sn_input = UI_New_SN.GetUiInput(Station.API, UIInput.Normal, Station);
                new_sn = new_sn_input.ToString().Trim().ToUpper();

                if (!oldSNObj.CheckSNRule(new_sn, skuObj.SN_RULE, Station.SFCDB, Station.DBType))
                {
                    throw new MESReturnMessage($@"{new_sn} Not Match {skuObj.SN_RULE}!");
                }
                //Is Exists
                if (t_r_sn.CheckSNExists(new_sn, Station.SFCDB))
                {
                    throw new MESReturnMessage($@"{new_sn} Already Exists");
                }
                //Check SN Rule

                //Update R_SN_REPLACE
                wait_replace.NEWSN = new_sn;
                wait_replace.EDITBY = Station.LoginUser.EMP_NO;
                wait_replace.FLAG = "1";
                wait_replace.EDITTIME = sysdate;
                result = Station.SFCDB.ORM.Updateable<R_SN_REPLACE>(wait_replace).Where(r => r.ID == wait_replace.ID).ExecuteCommand();
                if (result == 0)
                {
                    throw new MESReturnMessage("Update R_SN_REPLACE Fail!");
                }

                //Insert R_REPLACE_SN
                T_R_REPLACE_SN t_r_replace_sn = new T_R_REPLACE_SN(Station.SFCDB, Station.DBType);
                R_REPLACE_SN replaceLog = new R_REPLACE_SN();
                replaceLog.ID = t_r_replace_sn.GetNewID(Station.BU, Station.SFCDB);
                replaceLog.OLD_SN_ID = oldSNObj.ID;
                replaceLog.OLD_SN = oldSNObj.SerialNo;
                replaceLog.NEW_SN = new_sn;
                replaceLog.EDIT_EMP = Station.LoginUser.EMP_NO;
                replaceLog.EDIT_TIME = sysdate;
                result = t_r_replace_sn.AddReplaceSNRecord(replaceLog, Station.BU, Station.SFCDB, Station.DBType);
                if (result == 0)
                {
                    throw new MESReturnMessage("Insert R_REPLACE_SN Fail!");
                }

                //Update Old SN R_MRB 
                T_R_MRB t_r_mrb = new T_R_MRB(Station.SFCDB, Station.DBType);
                R_MRB old_mrb = Station.SFCDB.ORM.Queryable<R_MRB>()
                    .Where(r => r.SN == oldSNObj.SerialNo && r.MRB_FLAG == "1" && SqlFunc.IsNullOrEmpty(r.REWORK_WO)).ToList().FirstOrDefault();
                //Insert New Record For Next Action To Rework
                R_MRB new_mrb = new R_MRB();
                new_mrb = old_mrb ?? throw new MESReturnMessage($@"{oldSNObj.SerialNo} Not In MRB!");
                new_mrb.ID = t_r_mrb.GetNewID(Station.BU, Station.SFCDB);
                new_mrb.SN = new_sn;
                new_mrb.SKUNO = new_skuno;
                new_mrb.CREATE_EMP = Station.LoginUser.EMP_NO;
                new_mrb.CREATE_TIME = sysdate;
                new_mrb.EDIT_EMP = Station.LoginUser.EMP_NO;
                new_mrb.EDIT_TIME = sysdate;
                result = Station.SFCDB.ORM.Insertable<R_MRB>(new_mrb).ExecuteCommand();
                if (result == 0)
                {
                    throw new MESReturnMessage("Insert R_MRB Fail!");
                }

                old_mrb.REWORK_WO = reworkWoObj.WorkorderNo;
                old_mrb.MRB_FLAG = "0";
                old_mrb.EDIT_EMP = Station.LoginUser.EMP_NO;
                old_mrb.EDIT_TIME = sysdate;
                result = Station.SFCDB.ORM.Updateable<R_MRB>(old_mrb).ExecuteCommand();
                if (result == 0)
                {
                    throw new MESReturnMessage("Update R_MRB Fail!");
                }
                oldSNObj.ChangeSN(new_sn);
                sessionOldSN.Value = oldSNObj;
            }

        }

        /// <summary>
        /// SE機種充放電抽測確認動作，用彈窗確認取代打印掛卡LABEL Ask By PE楊大堯
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ChargeSampleConfirmAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN _SN = null;
            if (SNSession.Value.GetType() == typeof(SN))
            {
                _SN = (SN)SNSession.Value;
            }
            else
            {
                _SN = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }

            //判斷SN是否被抽測，如是則彈窗，否則什麼都不做
            var boolCharge = Station.SFCDB.ORM.Queryable<R_SN_LOG>().Where(t => t.SN == _SN.SerialNo && t.FLAG == "Y" && t.LOGTYPE == "CHARGE-SAMPLE").Any();
            if (boolCharge)
            {
                UIInputData O = new UIInputData()
                {
                    Timeout = 100000,
                    UIArea = new string[] { "25%", "28%" },
                    IconType = IconType.None,
                    Message = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154352"),// "確認",
                    Tittle = "",
                    Type = UIInputType.Confirm,
                    Name = "",
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154415"),// "未完成充放電抽測確認！"
                };
                //O.OutInputs.Add(new DisplayOutPut() { Name = "溫馨提示：", DisplayType = UIOutputType.TextArea.ToString(), Value = "該產品需要進行充放電抽測！" });
                O.OutInputs.Add(new DisplayOutPut() { Name = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154433"), DisplayType = UIOutputType.TextArea.ToString(), Value = MESReturnMessage.GetMESReturnMessage("MSGCODE20210814154529") });
                O.GetUiInput(Station.API, UIInput.Normal, Station);
            }
        }

        /// <summary>
        /// 自動生成PPID條碼進行綁定
        /// 生成條件：存在PPID S/N類型Keypart
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void AutoPPIDSNMakerAndLinkKpAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            //獲取SN對象
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN _SN = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);

            //獲取R_SN_KP List對象
            List<R_SN_KP> _SNKPList = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == _SN.SerialNo && t.VALID_FLAG == 1).ToList();
            //獲取R_SN_KP ppid對象
            R_SN_KP _PPIDKP = _SNKPList.Find(t => t.SCANTYPE == "PPID S/N");
            //如果SN存在PPID S/N類型Kp且還未綁定value
            if (_PPIDKP != null && string.IsNullOrEmpty(_PPIDKP.VALUE))
            //if (_PPIDKP != null && _PPIDKP.VALUE != null)
            {
                string ppidSN = _SN.AutoPPIDSNMaker(Station, _SN, _PPIDKP);
                if (ppidSN.StartsWith("ERROR"))
                {
                    throw new Exception(ppidSN);

                }
                _PPIDKP.VALUE = ppidSN;
                //_PPIDKP.EXKEY1 = "AutoLink";
                _PPIDKP.EDIT_TIME = Station.GetDBDateTime();
                _PPIDKP.EDIT_EMP = "SYSTEM";

                Station.SFCDB.ORM.Updateable(_PPIDKP).Where(t => t.ID == _PPIDKP.ID).ExecuteCommand();
            }

            R_SN_KP _STKP = _SNKPList.Find(t => t.SCANTYPE == "ST S/N");
            //如果SN存在ST S/N類型Kp且還未綁定value            
            if (_STKP != null && string.IsNullOrEmpty(_STKP.VALUE))
            {
                var stNO = _SN.AutoSTSNLink(Station, _SN, "");
                if (stNO.StartsWith("ERROR"))
                {
                    throw new Exception(stNO);
                }
                _STKP.VALUE = stNO;
                _STKP.EXKEY1 = "AutoLink";
                _STKP.EDIT_TIME = Station.GetDBDateTime();
                _STKP.EDIT_EMP = "SYSTEM";

                Station.SFCDB.ORM.Updateable(_STKP).Where(t => t.ID == _STKP.ID).ExecuteCommand();
            }
        }

        public static void ReturnShareDBBySN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            string sn = SNSession.Value.ToString();
            if (Station.SFCDB.PoolItem.DBPool.ShareDB.Keys.Contains(sn))
            {
                Station.SFCDB.PoolItem.DBPool.ShareDB.Remove(sn);
            }
        }

        public static void UnbondSILoadKP(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN _SN = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (_SN.CurrentStation != "SILOADING")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210106172858"));
            }
            var snIfExists = Station.SFCDB.ORM.Queryable<R_SN, R_SN_KP, MESDataObject.Module.R_SN_LINK, WWN_DATASHARING>((a, b, c, d)
                => a.SN == b.SN && a.SN == c.SN && a.SN == d.CSSN && b.VALUE == c.CSN && b.VALUE == d.VSSN).Where((a, b, c, d)
                => a.SN == _SN.SerialNo && a.CURRENT_STATION == "SILOADING" && a.VALID_FLAG == "1" && b.STATION == a.CURRENT_STATION && b.VALID_FLAG == 1 && c.VALIDFLAG == "1").ToList();
            if (snIfExists.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210107141407"));
            }
            var rSNKP = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == _SN.SerialNo && t.STATION == "SILOADING" && t.VALID_FLAG == 1).ToList();
            if (rSNKP.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210107141407"));
            }
            foreach (var v in rSNKP)
            {
                var strSql = $@"insert into r_sn_station_detail (sn, r_sn_id, workorderno, station_name, edit_emp, edit_time) values ('{v.SN}','{v.VALUE}','{_SN.WorkorderNo}','UNBONDSILOADINGKP','{Station.LoginUser.EMP_NO}',sysdate)";
                Station.SFCDB.ExecSQL(strSql);

                strSql = $@"update wwn_datasharing set vssn = 'N/A', vsku='N/A', lasteditby = '{Station.LoginUser.EMP_NO}', lasteditdt = sysdate where cssn = '{v.SN}' and vssn = '{v.VALUE}'";
                Station.SFCDB.ExecSQL(strSql);

                v.VALID_FLAG = 0;
                Station.SFCDB.ORM.Updateable<R_SN_KP>(v).Where(t => t.ID == v.ID).ExecuteCommand();

                strSql = $@"update r_sn set shipped_flag = '0', edit_emp = '{Station.LoginUser.EMP_NO}', edit_time = sysdate where sn = '{v.VALUE}'";
                Station.SFCDB.ExecSQL(strSql);

                v.VALUE = "";
                v.VALID_FLAG = 1;
                v.ID = MesDbBase.GetNewID(Station.SFCDB.ORM, Station.BU, "R_SN_KP");
                Station.SFCDB.ORM.Insertable<R_SN_KP>(v).ExecuteCommand();
            }
        }


        public static void CBSChangePOStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            var woList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == PackNo.Value.ToString()).Select((a, b, c, d) => d.WORKORDERNO).Distinct().ToList();
            foreach (var v in woList)
            {
                var woQtyList = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == v).ToList();
                if (woQtyList.Count != 1)
                {
                    throw new Exception($@"R_WO_BASE {v} has multiple data!");
                }
                int woqty = int.Parse(woQtyList[0].WORKORDER_QTY.ToString());
                var rSNList = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == v && t.VALID_FLAG == "1" && t.CURRENT_STATION == "CBS").ToList();
                if (woqty == rSNList.Count && woqty > 0)
                {
                    var oOrderMainList = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == v).ToList();
                    if (oOrderMainList.Count > 1)
                    {
                        throw new Exception($@"O_ORDER_MAIN {v} has multiple data!");
                    }
                    if (oOrderMainList.Count > 0)
                    {
                        var mainID = oOrderMainList[0].ID;
                        string sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{mainID}' and VALIDFLAG = '1'";
                        Station.SFCDB.ORM.Ado.ExecuteCommand(sql);

                        //已完成CBS
                        O_PO_STATUS oPoStatus = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(Station.SFCDB.ORM, Station.BU),
                            POID = mainID,
                            STATUSID = "10",
                            VALIDFLAG = "0",
                            CREATETIME = Station.SFCDB.ORM.GetDate(),
                            EDITTIME = Station.SFCDB.ORM.GetDate()
                        };
                        Station.SFCDB.ORM.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
                        //待PreAsn
                        oPoStatus = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(Station.SFCDB.ORM, Station.BU),
                            POID = mainID,
                            STATUSID = "11",
                            VALIDFLAG = "1",
                            CREATETIME = Station.SFCDB.ORM.GetDate(),
                            EDITTIME = Station.SFCDB.ORM.GetDate()
                        };
                        Station.SFCDB.ORM.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();

                    }

                }
            }
        }

        public static void JuniperChangePOStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }

            MESStationSession Wo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Wo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession Status = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Status == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            if (Status.Value.ToString() != "11" && Status.Value.ToString() != "29")
            {
                throw new Exception("Statusid should be 11/29 only");
            }

            var oOrderMainList = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == Wo.Value.ToString()).ToList();
            if (oOrderMainList.Count != 1)
            {
                throw new Exception($@"O_ORDER_MAIN {Wo.Value.ToString()} has multiple data!");
            }
            var mainID = oOrderMainList[0].ID;
            string sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{mainID}' and VALIDFLAG = '1'";
            Station.SFCDB.ORM.Ado.ExecuteCommand(sql);

            //待哪個狀態
            O_PO_STATUS oPoStatus = new O_PO_STATUS()
            {
                ID = MesDbBase.GetNewID<O_PO_STATUS>(Station.SFCDB.ORM, Station.BU),
                POID = mainID,
                STATUSID = Status.Value.ToString(),
                VALIDFLAG = "1",
                CREATETIME = Station.SFCDB.ORM.GetDate(),
                EDITTIME = Station.SFCDB.ORM.GetDate()
            };
            Station.SFCDB.ORM.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
        }

        public static void ShipOutChangePOStatus(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null) //這裡傳的是PALLETNO 
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string palletNo = PackNo.Value.ToString();
            MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(Station.SFCDB.ORM);
            var woPrefix = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == palletNo).Select((a, b, c, d) => d.WORKORDERNO).ToList().FirstOrDefault();
            #region 工單是007的 edit by chc
            if (woPrefix.StartsWith("007"))
            {
                juniperAsn.ShipOutChangePOStatus(palletNo, Station.BU);
            }

            #endregion

            #region 放JuniperASNObj裡
            /*
            SN _SN = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            if (_SN.CurrentStation == "SHIPOUT")
            {
                var woList = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == _SN.WorkorderNo).ToList();
                if (woList.Count != 1)
                {
                    throw new Exception($@"R_WO_BASE {_SN.WorkorderNo} has multiple data!");
                }
                int woqty = int.Parse(woList[0].WORKORDER_QTY.ToString());
                var rSNList = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == _SN.WorkorderNo && t.VALID_FLAG == "1" && t.CURRENT_STATION == "SHIPOUT").ToList();
                if (woqty == rSNList.Count && woqty > 0)
                {
                    var oOrderMainList = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == _SN.WorkorderNo).ToList();
                    if (oOrderMainList.Count != 1)
                    {
                        throw new Exception($@"O_ORDER_MAIN {_SN.WorkorderNo} has multiple data!");
                    }
                    var mainID = oOrderMainList[0].ID;
                    string sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{mainID}' and VALIDFLAG = '1'";
                    Station.SFCDB.ORM.Ado.ExecuteCommand(sql);

                    //待FinalAsn
                    O_PO_STATUS oPoStatus = new O_PO_STATUS()
                    {
                        ID = MesDbBase.GetNewID<O_PO_STATUS>(Station.SFCDB.ORM, Station.BU),
                        POID = mainID,
                        STATUSID = "29",
                        VALIDFLAG = "1",
                        CREATETIME = Station.SFCDB.ORM.GetDate(),
                        EDITTIME = Station.SFCDB.ORM.GetDate()
                    };
                    Station.SFCDB.ORM.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
                }
            }
            */
            #endregion
        }

        public static void ShipOutSendFinalAsn(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession PackNo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PackNo == null) //這裡傳的是PALLETNO 
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            #region FCA 
            var woList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == PackNo.Value.ToString()).Select((a, b, c, d) => d.WORKORDERNO).Distinct().ToList();

            //O_ORDER_MAIN.ORDERTYPE=IDOA do not need to do FinalAsn 2022-01-05
            var isIDOA = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN>().Where(t => t.PREWO == woList[0] && t.ORDERTYPE == "IDOA").Any();
            if (isIDOA && Station.BU == "VNJUNIPER")
                return;

            // 不是007開頭的工單不傳FinalAsn edit by chc
            if (!woList[0].StartsWith("007"))
                return;
            var I137_H = Station.SFCDB.ORM.Queryable<I137_H, I137_I, O_ORDER_MAIN>((h, i, m) => h.TRANID == i.TRANID && m.ITEMID == i.ID).Where((h, i, m) => m.PREWO == woList[0]).Select((h, i, m) => h).ToList().FirstOrDefault(); //
            if (I137_H == null)
                return;
            if (woList.Count() > 1)
                throw new Exception("JNP workorder pallets are not allowed to have multiple workorder#");
            string dnNo = Station.DisplayOutput.Find(t => t.Name == "DN_NO").Value.ToString(),
                   dnLine = Station.DisplayOutput.Find(t => t.Name == "DN_ITEM").Value.ToString();
            var tohead = Station.SFCDB.ORM.Queryable<SD_TO_HEAD, SD_TO_DETAIL, R_DN_STATUS>((t, d, s) => t.TKNUM == d.TKNUM && d.VBELN == s.DN_NO).Where((t, d, s) => s.DN_NO == dnNo && s.DN_LINE == dnLine)
                .Select((t, d, s) => t).ToList().FirstOrDefault();
            if (tohead != null && I137_H != null && I137_H.INCO1 == "FCA" && I137_H.INCO2 == "ORIGIN" && (tohead.EXTI2 == null || tohead.TPBEZ == null))
                throw new Exception("FCA order:Miss carrier name / tracking,pls contact logistics!");

            #endregion

            var Postatuscheck = $@"select*From o_po_status where poid in(
                                    select id From o_order_main where PREASN in(
                                    select PREASN From o_order_main where prewo in(
                                    select distinct WORKORDERNO From r_sn_packing a, r_packing b,r_sn c,r_packing d 
                                    where d.pack_no='{PackNo.Value.ToString()}' and d.id=b.PARENT_PACK_ID and a.PACK_ID=b.id and c.id=a.sn_id and c.VALID_FLAG=1)
                                    )  ) and VALIDFLAG=1 and STATUSID<>'29'";

            DataTable dt = Station.SFCDB.ExecSelect(Postatuscheck).Tables[0];
            if (dt.Rows.Count == 0)
            {
                MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(Station.SFCDB.ORM);
                juniperAsn.ShipOutSendFinalAsn(PackNo.Value.ToString(), Station.BU);
            }



            #region 放JuniperASNObj裡
            /*
            var woList = Station.SFCDB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((a, b, c, d) => a.ID == b.PARENT_PACK_ID && b.ID == c.PACK_ID && c.SN_ID == d.ID)
                .Where((a, b, c, d) => a.PACK_TYPE == "PALLET" && a.PACK_NO == PackNo.Value.ToString()).Select((a, b, c, d) => d.WORKORDERNO).Distinct().ToList();
            foreach (var v in woList)
            {
                var oOrderMain = Station.SFCDB.ORM.Queryable<O_ORDER_MAIN, O_PO_STATUS>((a, b) => a.ID == b.POID).Where((a, b) => b.VALIDFLAG == "1" && b.STATUSID == "29" && a.PREWO == v).Select((a, b) => a).ToList();
                if (oOrderMain.Count > 0)
                {
                    try
                    {
                        MESJuniper.SendData.JuniperASNObj juniperAsn = new MESJuniper.SendData.JuniperASNObj(Station.SFCDB.ORM);
                        var res = juniperAsn.BuildFinalAsn(oOrderMain[0].PONO, oOrderMain[0].POLINE, Station.BU);
                        var mainID = oOrderMain[0].ID;
                        string sql = $@"update o_po_status set VALIDFLAG = '0',edittime = sysdate where poid = '{mainID}' and VALIDFLAG = '1'";
                        Station.SFCDB.ORM.Ado.ExecuteCommand(sql);

                        //Finish
                        O_PO_STATUS oPoStatus = new O_PO_STATUS()
                        {
                            ID = MesDbBase.GetNewID<O_PO_STATUS>(Station.SFCDB.ORM, Station.BU),
                            POID = mainID,
                            STATUSID = "31",
                            VALIDFLAG = "1",
                            CREATETIME = Station.SFCDB.ORM.GetDate(),
                            EDITTIME = Station.SFCDB.ORM.GetDate()
                        };
                        Station.SFCDB.ORM.Insertable<O_PO_STATUS>(oPoStatus).ExecuteCommand();
                    }
                    catch
                    {

                    }
                }
            }
            */
            #endregion

        }


        public static void SilverRotationCheckIn(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN snObj = (SN)snSession.Value;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            int result = t_r_silver_rotation.InsterRotation(Station.SFCDB, snObj.SkuNo, snObj.SerialNo, Station.LoginUser.EMP_NO, Station.BU);
            if (result == 0)
            {
                throw new Exception("Save R_SILVER_ROTATION Fail!");
            }
        }
        public static void SilverRotationCheckOut(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN snObj = (SN)snSession.Value;
            T_R_SILVER_ROTATION t_r_silver_rotation = new T_R_SILVER_ROTATION(Station.SFCDB, Station.DBType);
            int result = t_r_silver_rotation.UpdateRotationStatus(snObj.SerialNo, "1", Station.LoginUser.EMP_NO, Station.SFCDB);
            if (result == 0)
            {
                throw new Exception("Update R_SILVER_ROTATION Fail!");
            }
        }

        public static void VNDCNPCBADataSendToSchneiderDB(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN snObj = (SN)snSession.Value;

            SchneiderAction Schneider = new SchneiderAction();
            Schneider.SendPCBASNTOSE(Station.SFCDB, snObj.SerialNo);

            //var snList = Station.SFCDB.ORM.Queryable<R_SN, R_WO_BASE, C_SKU, C_SERIES>((a, b, c, d) => a.WORKORDERNO == b.WORKORDERNO && b.SKUNO == c.SKUNO && b.SKU_VER == c.VERSION && c.C_SERIES_ID == d.ID)
            //    .Where((a, b, c, d) => a.NEXT_STATION == "JOBFINISH" && a.SN == snObj.SerialNo && d.DESCRIPTION == "Schneider" && c.SKU_TYPE == "PCBA" && a.VALID_FLAG == "1")
            //    .Select((a, b, c, d) => new { a.SN, a.SKUNO, c.DESCRIPTION }).ToList().FirstOrDefault();
            //if (snList != null)
            //{
            //    SchneiderAction.PCBA_Master pcba_sn = new SchneiderAction.PCBA_Master()
            //    {
            //        Sn = snList.SN,
            //        Skuno = snList.SKUNO,
            //        SkunoDescription = snList.DESCRIPTION
            //    };
            //    SchneiderAction Schneider = new SchneiderAction();
            //    try
            //    {
            //        Schneider.SendPCBASNTOSchneiderDB(pcba_sn);
            //    }
            //    catch (Exception ex)
            //    {
            //        throw new Exception("VNDCNPCBADataSendToSchneiderDB Error: " + ex.Message);
            //    }
            //}
        }

        public static void VNDCNGetSchneiderSISN(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string wo = WoSession.InputValue;

            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string sn = snSession.InputValue;

            DateTime sysdate = Station.SFCDB.ORM.GetDate();

            var woList = Station.SFCDB.ORM.Queryable<R_WO_BASE, C_SKU, C_SERIES>((b, c, d) => b.SKUNO == c.SKUNO && b.SKU_VER == c.VERSION && c.C_SERIES_ID == d.ID)
                .Where((b, c, d) => b.WORKORDERNO == wo && d.DESCRIPTION == "Schneider" && c.SKU_TYPE == "MODEL")
                .Select((b, c, d) => b).ToList().FirstOrDefault();
            if (woList != null)
            {
                SchneiderAction.SI_SN ModelSn = new SchneiderAction.SI_SN();
                var rSnDetail = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.STATION_NAME == "KIT_PRINT").OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList().FirstOrDefault();
                if (rSnDetail == null)
                {
                    SchneiderAction Schneider = new SchneiderAction();
                    try
                    {
                        ModelSn = Schneider.GetModelSNFromSchneiderDB(sn);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("VNDCNGetSchneiderSISN Error: " + ex.Message);
                    }

                    if (ModelSn.result == false)
                    {
                        throw new Exception("SN is not exists in Schneider DB, Please check!");
                    }
                    if (ModelSn.Status != "JOBFINISH")
                    {
                        throw new Exception($@"SN Status is not JOBFINISH in Schneider DB, Status = {ModelSn.Status} Please check!");
                    }
                    if (!Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == ModelSn.Csn && t.VALID_FLAG == "1").Any())
                    {
                        throw new Exception($@"CSN is not FVN PCBA SN, Please check!");
                    }
                    if (ModelSn.Workorderno != wo)
                    {
                        throw new Exception($@"SE WO {ModelSn.Workorderno} not match FVN SILOADING WO {wo}, Please check!");
                    }

                    R_SN_STATION_DETAIL sd = new R_SN_STATION_DETAIL() { WORKORDERNO = ModelSn.Workorderno, SN = ModelSn.Sn, PRODUCT_STATUS = ModelSn.Status, DEVICE_NAME = ModelSn.Csn, EDIT_EMP = "SESYSTEM", EDIT_TIME = sysdate, STATION_NAME = "KIT_PRINT" };
                    Station.SFCDB.ORM.Insertable<R_SN_STATION_DETAIL>(sd).ExecuteCommand();
                }
                rSnDetail = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.STATION_NAME == "KIT_PRINT").OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList().FirstOrDefault();
                //if (rSnDetail == null)
                //{
                //    throw new Exception("SN not exists in Schneider DB -2, Please check!");
                //}
                //if (rSnDetail != null && (rSnDetail.EDIT_EMP == null || rSnDetail.EDIT_EMP != "SESYSTEM"))
                //{
                //    throw new Exception($"SN {sn} has exists FVN KIT_PRINT, Please check!");
                //}
                if (rSnDetail != null && rSnDetail.EDIT_EMP == "SESYSTEM")
                {
                    var rSNList = Station.SFCDB.ORM.Queryable<MESDataObject.Module.R_SN_LINK>().Where(t => t.VALIDFLAG == "1" && (t.SN == rSnDetail.SN || t.CSN == rSnDetail.DEVICE_NAME)).ToList();
                    if (rSNList.Count() > 0)
                    {
                        throw new Exception($@"SN {rSnDetail.SN} Or CSN {rSnDetail.DEVICE_NAME} has already bounding (R_SN_LINK), Please check!");
                    }

                    if (Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == rSnDetail.DEVICE_NAME && t.VALID_FLAG == 1).Any())
                    {
                        throw new Exception($@"CSN {rSnDetail.DEVICE_NAME} has already bounding (R_SN_KP), Please check!");
                    }

                    if (Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.SN == rSnDetail.DEVICE_NAME && t.VALID_FLAG == "1" && t.SHIPPED_FLAG == "1").Any())
                    {
                        throw new Exception($@"CSN {rSnDetail.DEVICE_NAME} has already bounding (R_SN.SHIPPED_FLAG), Please check!");
                    }

                    MESDataObject.Module.R_SN_LINK rsnlink = new MESDataObject.Module.R_SN_LINK()
                    {
                        ID = MesDbBase.GetNewID<MESDataObject.Module.R_SN_LINK>(Station.SFCDB.ORM, Station.BU),
                        LINKTYPE = Station.StationName,
                        MODEL = woList.SKUNO,
                        SN = rSnDetail.SN,
                        CSN = rSnDetail.DEVICE_NAME,
                        VALIDFLAG = "1",
                        CREATETIME = sysdate,
                        CREATEBY = "SESYSTEM",
                        EDITTIME = sysdate,
                        EDITBY = Station.LoginUser.EMP_NO
                    };
                    Station.SFCDB.ORM.Insertable<MESDataObject.Module.R_SN_LINK>(rsnlink).ExecuteCommand();

                    Station.SFCDB.ORM.Updateable<R_SN>().SetColumns(r => new R_SN
                    {
                        SHIPPED_FLAG = "1",
                        SHIPDATE = sysdate,
                        EDIT_EMP = Station.LoginUser.EMP_NO,
                        EDIT_TIME = sysdate
                    }).Where(r => r.SN == rSnDetail.DEVICE_NAME && r.VALID_FLAG == "1").ExecuteCommand();
                }
            }
        }

        /// <summary>
        /// SNList掃描MRB過站Action
        /// </summary>
        public static void SNListMRBPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            int result = 0;
            string ZCPP_FLAG = "";//add by fgg 2018.04.10 用於標誌是組裝退料還是從工單入MRB
            SN SnObject = null;
            bool isSame = false;
            string UserEMP = Station.LoginUser.EMP_NO;
            string To_Storage = "";
            string From_Storage = "";
            string Confirmed_Flag = "";
            string DeviceName = "";
            R_SN NewSN = new R_SN();
            R_MRB New_R_MRB = new R_MRB();
            R_MRB_GT HMRB_GT = new R_MRB_GT();
            T_R_SN TR_SN = new T_R_SN(Station.SFCDB, Station.DBType);
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            T_R_MRB_GT TH_MRB_GT = new T_R_MRB_GT(Station.SFCDB, Station.DBType);
            T_R_WO_BASE TR_WO_BASE = new T_R_WO_BASE(Station.SFCDB, Station.DBType);
            T_C_SAP_STATION_MAP TC_SAP_STATION_MAP = new T_C_SAP_STATION_MAP(Station.SFCDB, Station.DBType);
            if (Paras.Count < 3)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }

            //判斷MRBType，0是單板入MRB，1是退料  0則From_Storage放空, 1則From_Storage則取前台傳的工單
            if (Paras[1].VALUE == "0")//0是單板入MRB
            {
                if (Paras.Count != 3)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                From_Storage = "";
                ZCPP_FLAG = "0";
            }
            else if (Paras[1].VALUE == "1")//1是退料
            {
                if (Paras.Count != 4)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                    throw new MESReturnMessage(errMsg);
                }
                MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
                if (WoSession == null || WoSession.Value == null || WoSession.Value.ToString().Length <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
                }
                else
                {
                    From_Storage = WoSession.Value.ToString();
                    ZCPP_FLAG = "1";
                }
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000020", new string[] { "MRBType", "0/1" }));
            }

            //獲取到 SNList 對象
            MESStationSession SNListSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNListSession == null || SNListSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //獲取To_Storage
            MESStationSession ToStorageSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (ToStorageSession == null || ToStorageSession.Value == null || ToStorageSession.Value.ToString().Length <= 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }
            else
            {
                To_Storage = ToStorageSession.Value.ToString();
            }

            List<SN> snListObject = (List<SN>)SNListSession.Value;
            for (int i = 0; i < snListObject.Count; i++)
            {
                SnObject = snListObject[i];
                //SNID必須存在
                if (SnObject.ID == null || SnObject.ID.Length <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
                }
                //SN如果已經完工，Confirmed_Flag=1，否則Confirmed_Flag=0
                if (SnObject.CompletedFlag != null && SnObject.CompletedFlag == "1")
                {
                    Confirmed_Flag = "1";
                }
                else
                {
                    Confirmed_Flag = "0";
                }

                //更新SN當前站，下一站，如果SN的compled=1了也就是Confirmed_Flag==1,則修改當前站和下一站即可
                //如果如果SN的compled!=1,則還需要修改sn的compled=1和SN對應工單的finishedQTY要加一
                if (Confirmed_Flag != "1")
                {
                    result = TR_SN.SN_Mrb_Pass_action(SnObject.ID, UserEMP, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                    }
                    else
                    {
                        if (SnObject.WorkorderNo == null || SnObject.WorkorderNo.Trim().Length <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000125", new string[] { SnObject.SerialNo }));
                        }
                        else
                        {
                            TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);//這裡如果工單不存在GetWo會報錯
                            result = TR_WO_BASE.UpdateFINISHEDQTYAddOne(SnObject.WorkorderNo, Station.SFCDB);
                            if (result <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_WO_BASE:" + SnObject.SerialNo, "UPDATE" }));
                            }
                            TR_WO_BASE.UpdateWoCloseFlag(SnObject.WorkorderNo, Station.SFCDB);//是否需要關閉工單
                        }
                    }
                }
                else
                {
                    result = TR_SN.SN_Mrb_Pass_actionNotUpdateCompleted(SnObject.ID, UserEMP, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN:" + SnObject.SerialNo, "UPDATE" }));
                    }
                }

                //添加一筆MRB記錄
                New_R_MRB.ID = TR_MRB.GetNewID(Station.BU, Station.SFCDB, Station.DBType);
                New_R_MRB.SN = SnObject.SerialNo;
                New_R_MRB.WORKORDERNO = SnObject.WorkorderNo;
                New_R_MRB.NEXT_STATION = SnObject.NextStation;
                New_R_MRB.SKUNO = SnObject.SkuNo;
                New_R_MRB.FROM_STORAGE = From_Storage;
                New_R_MRB.TO_STORAGE = To_Storage;
                New_R_MRB.REWORK_WO = "";//空
                New_R_MRB.CREATE_EMP = UserEMP;
                New_R_MRB.CREATE_TIME = Station.GetDBDateTime();
                New_R_MRB.MRB_FLAG = "1";
                New_R_MRB.SAP_FLAG = "0";
                New_R_MRB.EDIT_EMP = UserEMP;
                New_R_MRB.EDIT_TIME = New_R_MRB.CREATE_TIME;
                result = TR_MRB.Add(New_R_MRB, Station.SFCDB);
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB:" + SnObject.SerialNo, "ADD" }));
                }

                //存在R_MRB_GT WO =? And SAP_FLAG = 0,則檢查FROM_STORAGE，TO_STORAGE，CONFIRMED_FLAG是否一樣，一樣則累加1
                if (Paras[1].VALUE == "0")//0是單板入MRB
                {
                    HMRB_GT = Station.SFCDB.ORM.Queryable<R_MRB_GT>()
                        .Where(r => r.WORKORDERNO == SnObject.WorkorderNo && r.ZCPP_FLAG == ZCPP_FLAG && r.CONFIRMED_FLAG == Confirmed_Flag && r.SAP_FLAG == "0")
                        .ToList().FirstOrDefault();

                }
                else if (Paras[1].VALUE == "1")//1是退料
                {
                    HMRB_GT = Station.SFCDB.ORM.Queryable<R_MRB_GT>()
                        .Where(r => r.WORKORDERNO == From_Storage && r.ZCPP_FLAG == ZCPP_FLAG && r.CONFIRMED_FLAG == Confirmed_Flag && r.SAP_FLAG == "0")
                        .ToList().FirstOrDefault();
                }

                isSame = false;
                if (HMRB_GT != null)
                {
                    HMRB_GT.FROM_STORAGE = (HMRB_GT.FROM_STORAGE == null || HMRB_GT.FROM_STORAGE.Trim().Length <= 0) ? "" : HMRB_GT.FROM_STORAGE;
                    HMRB_GT.TO_STORAGE = (HMRB_GT.TO_STORAGE == null || HMRB_GT.TO_STORAGE.Trim().Length <= 0) ? "" : HMRB_GT.TO_STORAGE;
                    HMRB_GT.CONFIRMED_FLAG = (HMRB_GT.CONFIRMED_FLAG == null || HMRB_GT.CONFIRMED_FLAG.Trim().Length <= 0) ? "" : HMRB_GT.CONFIRMED_FLAG;
                    if (HMRB_GT.FROM_STORAGE == New_R_MRB.FROM_STORAGE && HMRB_GT.TO_STORAGE == New_R_MRB.TO_STORAGE && HMRB_GT.CONFIRMED_FLAG == Confirmed_Flag)
                    {
                        isSame = true;
                        if (Paras[1].VALUE == "0")//0是單板入MRB
                        {
                            result = TH_MRB_GT.updateTotalQTYAddOne(SnObject.WorkorderNo, UserEMP, Confirmed_Flag, Station.SFCDB);
                        }
                        else if (Paras[1].VALUE == "1")//1是退料
                        {
                            result = TH_MRB_GT.updateTotalQTYAddOne(From_Storage, UserEMP, Confirmed_Flag, Station.SFCDB);
                        }
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_MRB_GT:" + SnObject.SerialNo, "UPDATE" }));
                        }
                    }
                }
                if (!isSame)
                {
                    R_MRB_GT New_HMRB_GT = new R_MRB_GT();
                    New_HMRB_GT.ID = TH_MRB_GT.GetNewID(Station.BU, Station.SFCDB);
                    if (Paras[1].VALUE == "0")//0是單板入MRB
                    {
                        New_HMRB_GT.WORKORDERNO = SnObject.WorkorderNo;
                    }
                    else if (Paras[1].VALUE == "1")//1是退料
                    {
                        New_HMRB_GT.WORKORDERNO = From_Storage;
                    }
                    Row_R_WO_BASE RowWO_BASE = TR_WO_BASE.GetWo(SnObject.WorkorderNo, Station.SFCDB);
                    string sapStationCode = TC_SAP_STATION_MAP.GetMAXSAPStationCodeBySku(SnObject.SkuNo, Station.SFCDB);
                    if (sapStationCode == "")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000224", new string[] { SnObject.SkuNo }));
                    }
                    New_HMRB_GT.SAP_STATION_CODE = sapStationCode;
                    New_HMRB_GT.FROM_STORAGE = From_Storage;
                    New_HMRB_GT.TO_STORAGE = To_Storage;
                    New_HMRB_GT.TOTAL_QTY = 1;
                    New_HMRB_GT.CONFIRMED_FLAG = Confirmed_Flag;
                    New_HMRB_GT.ZCPP_FLAG = ZCPP_FLAG;//暫時預留
                    New_HMRB_GT.SAP_FLAG = "0";//0待拋,1已拋,2待重拋
                    New_HMRB_GT.SKUNO = SnObject.SkuNo;
                    New_HMRB_GT.SAP_MESSAGE = "";
                    New_HMRB_GT.EDIT_EMP = UserEMP;
                    New_HMRB_GT.EDIT_TIME = Station.GetDBDateTime();
                    result = TH_MRB_GT.Add(New_HMRB_GT, Station.SFCDB);
                    if (result <= 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "H_MRB_GT:" + SnObject.SerialNo, "ADD" }));
                    }
                }
                //添加過站記錄
                var snobj = Station.SFCDB.ORM.Queryable<R_SN>().Where(t => t.ID == SnObject.ID).ToList().FirstOrDefault();
                result = Convert.ToInt32(TR_SN.RecordPassStationDetail(snobj, Station.Line, Station.StationName, DeviceName, Station.BU, Station.SFCDB));
                if (result <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_SN_STATION_DETAIL:" + SnObject.SerialNo, "ADD" }));
                }
            }

            Station.AddMessage("MES00000063", new string[] { "" }, StationMessageState.Pass); //回饋消息到前台
        }
        /// <summary>
        /// 更新退站后的SN的PPID S/N
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UpdateReturnStationPPIDKP(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string sn = "";
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            sn = snSession.InputValue;
            SN _SN = new SN(sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            var snkp = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.VALID_FLAG == 1).ToList();
            var _PPIDKP = snkp.Find(t => t.SCANTYPE == "PPID S/N");
            if (_PPIDKP != null && string.IsNullOrEmpty(_PPIDKP.VALUE))
            {
                string ppidSN = _SN.AutoPPIDSNMaker(Station, _SN, _PPIDKP);
                if (ppidSN.StartsWith("ERROR"))
                {
                    throw new Exception(ppidSN);
                }
                _PPIDKP.VALUE = ppidSN;
                //_PPIDKP.EXKEY1 = "AutoLink";
                _PPIDKP.EDIT_TIME = Station.GetDBDateTime();
                _PPIDKP.EDIT_EMP = "SYSTEM";
                Station.SFCDB.ORM.Updateable(_PPIDKP).Where(t => t.ID == _PPIDKP.ID).ExecuteCommand();

            }
        }
        /// <summary>
        /// 更新退站后的SN的 ST S/N KP
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void UpdateReturnStationSTKP(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            var hasOld = false;
            string sn = "";
            if (Paras.Count != 1)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            sn = snSession.InputValue;
            SN _SN = new SN(sn, Station.SFCDB, DB_TYPE_ENUM.Oracle);

            var snkp = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.SN == sn && t.VALID_FLAG == 1).ToList();
            var _STKP = snkp.Find(t => t.SCANTYPE == "ST S/N");
            if (_STKP != null && string.IsNullOrEmpty(_STKP.VALUE))
            {
                var oldSTSn = Station.SFCDB.ORM.Queryable<R_SN_KP>()
                   .Where(t => t.VALID_FLAG == 0 && t.SN == sn && t.SCANTYPE == "ST S/N")
                   .OrderBy(t => t.EDIT_TIME, OrderByType.Desc).Select(t => t.VALUE).First();
                if (!string.IsNullOrEmpty(oldSTSn))
                {
                    _STKP.VALUE = oldSTSn;

                    hasOld = true;
                }
                if (!hasOld)
                {
                    string reValue = _SN.UpdateReturnSTKP(Station, _SN);
                    if (reValue.StartsWith("ERROR"))
                    {
                        throw new Exception(reValue);
                    }
                    _STKP.VALUE = reValue;
                }

                _STKP.EXKEY1 = "AutoLink";
                _STKP.EDIT_TIME = Station.GetDBDateTime();
                _STKP.EDIT_EMP = "SYSTEM";
                Station.SFCDB.ORM.Updateable(_STKP).Where(t => t.ID == _STKP.ID).ExecuteCommand();
            }
        }

        /// <summary>
        /// 新增一筆過站記錄:用於將Aruba離線打印的CTOSN與將要SILOADING的工單綁定並寫入過站記錄表
        /// 背景:Aruba的CTOSN規則是除了前綴後綴和PCBASN不一樣外中間部分是一樣的, 而CTOSN在PRE-ASSY已經隨PCBASN一起打印出來, 只是沒有錄入系統
        /// 因此當PCBASN是上個月生成而下個月才用CTOSN去SILOADING時會檢查到CTOSN中的月份與當前月份不符, 而不檢查月份的邏輯需要掃描的SN存在一筆國過站記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ArubaCTOSNAddRecord(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession woSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (woSession == null || woSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (snSession == null || snSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            WorkOrder woObj = new WorkOrder();
            if (woSession.Value is string)
            {
                woObj.Init(woSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else if (woSession.Value is WorkOrder)
            {
                woObj = (WorkOrder)woSession.Value;
            }
            string sn = snSession.Value.ToString();

            var controlFlag = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "ArubaCTOSNAddRecord" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == woObj.SkuNO).Any();
            if (controlFlag)
            {
                R_SN_STATION_DETAIL snDetail = new R_SN_STATION_DETAIL()
                {
                    WORKORDERNO = woObj.WorkorderNo,
                    SKUNO = woObj.SkuNO,
                    SN = sn,
                    STATION_NAME = Station.StationName,
                    EDIT_EMP = Station.LoginUser.EMP_NO,
                    EDIT_TIME = DateTime.Now
                };
                int flag = Station.SFCDB.ORM.Insertable(snDetail).ExecuteCommand();
                if (flag < 1)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { $@"Aruba CTO SN:{sn} Add Record Fail!" }));
                }
            }
        }

        /// <summary>
        /// SILoading Get SN test ORT.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void GetTestOrtJuniper(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            T_R_LOT_STATUS Lot_Status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL Lot_Detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            Row_R_LOT_STATUS Rstatus = (Row_R_LOT_STATUS)Lot_Status.NewRow();
            Row_R_LOT_DETAIL Rdetail = (Row_R_LOT_DETAIL)Lot_Detail.NewRow();

            //SN SNObj = new SN();
            DataTable dt = null;
            if (Paras.Count != 1)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000057");
                throw new MESReturnMessage(errMsg);
            }
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else if (SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            //SNObj = (SN)SNSession.Value;
            //Sku = SNObj.SkuNo;
            //SerialNo = SNObj.LoadSN(SNObj.SerialNo, Station.SFCDB);
            var _SN = (SN)SNSession.Value;
            var SNObj = Station.SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == _SN.SerialNo && r.VALID_FLAG == "1").First();
            var Skusample = Station.SFCDB.ORM.Queryable<C_SKU_DETAIL>().Where(c => c.SKUNO == SNObj.SKUNO && c.CATEGORY == "JUNIPER" && c.CATEGORY_NAME == "ORT_Sample" && c.EXTEND == "Y").ToList().FirstOrDefault();
            string sql = string.Empty;

            //不同機種路由製程不一樣,與其讓QE設定在哪個工站進行抽測,還不如統一設定在SILOADING抽測
            if (Skusample != null && Station.StationName == "SILOADING")
            {
                //Convert percentage to decimal
                Double SampleValue = double.Parse(Skusample.VALUE.Replace("%", "")) / 100;

                //Calculate the current SKU production quantity
                sql = $@"SELECT skuno,sum(WORKORDER_QTY) woqty FROM r_wo_base where skuno='{SNObj.SKUNO}' and RELEASE_DATE BETWEEN TRUNC(SYSDATE, 'Q')  and sysdate  group by skuno";
                dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                int SkuProductQty = Convert.ToInt32(dt.Rows[0]["WOQTY"].ToString());

                //Check whether there is a random test for SKU
                //var InORT = Station.SFCDB.ORM.Queryable<R_LOT_STATUS>().Where(r => r.SKUNO == SNObj.SKUNO && r.SAMPLE_STATION == "ORT").ToList();
                //排除掉R_LOT_DETAIL中CancelORT的數據
                //var InORT = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((s, d) => s.LOT_NO == d.LOT_ID).Where((s, d) => s.SKUNO == SNObj.SKUNO && s.SAMPLE_STATION == "ORT" && d.STATUS != "2").Select((s, d) => d).ToList();
                sql = $@"SELECT * FROM R_LOT_STATUS s, R_LOT_DETAIL d WHERE s.LOT_NO=d.LOT_ID AND  s.SAMPLE_STATION='ORT' AND d.STATUS !='2' and s.SKUNO='{SNObj.SKUNO}' AND d.CREATE_DATE BETWEEN TRUNC(SYSDATE, 'Q')  and sysdate";
                dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                //Calculate the current SKU of sampling tests required
                //Double DoSample = SkuProductQty *  SampleValue;
                Double DoSample = Math.Round(SkuProductQty * SampleValue);//四捨五入, 0.1-0.4則略,0.5-0.9算1  Asked By QE BiBi  2021-11-15
                if (DoSample >= 1 && dt.Rows.Count == 0)
                {
                    Rstatus.ID = Lot_Detail.GetNewID(Station.BU, Station.SFCDB);
                    Rstatus.LOT_NO = "LOT-" + DateTime.Now.ToString("yyyyMMddFF");
                    Rstatus.SKUNO = SNObj.SKUNO;
                    Rstatus.LOT_QTY = SkuProductQty;
                    Rstatus.REJECT_QTY = 0;
                    Rstatus.SAMPLE_QTY = Math.Floor(DoSample);//Rounded up
                    Rstatus.PASS_QTY = 1;
                    Rstatus.FAIL_QTY = 0;
                    Rstatus.CLOSED_FLAG = "2";
                    Rstatus.LOT_STATUS_FLAG = "0";
                    Rstatus.SAMPLE_STATION = "ORT";
                    Rstatus.EDIT_EMP = Station.LoginUser.EMP_NO;
                    Rstatus.EDIT_TIME = DateTime.Now;
                    Station.SFCDB.ExecSQL(Rstatus.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                    Station.SFCDB.CommitTrain();

                    Rdetail.ID = Lot_Detail.GetNewID(Station.BU, Station.SFCDB);
                    Rdetail.LOT_ID = Rstatus.LOT_NO;
                    Rdetail.SN = SNObj.SN;
                    Rdetail.WORKORDERNO = SNObj.WORKORDERNO;
                    Rdetail.CREATE_DATE = DateTime.Now;
                    Rdetail.STATUS = "0";
                    Station.SFCDB.ExecSQL(Rdetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                    Station.SFCDB.CommitTrain();

                    //增加彈窗提示
                    UIInputData O = new UIInputData()
                    {
                        Timeout = 60000,
                        UIArea = new string[] { "25%", "28%" },
                        IconType = IconType.None,
                        MustConfirm = false,
                        Message = "OK",
                        Tittle = "",
                        Type = UIInputType.Confirm,
                        Name = "",
                        ErrMessage = ""
                    };
                    O.OutInputs.Add(new DisplayOutPut() { Name = "Alert", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"{SNObj.SN} has been chosen to ORT test !" });
                    O.GetUiInput(Station.API, UIInput.Normal, Station);
                }
                else if (DoSample >= 1 && dt.Rows.Count > 0)
                {
                    var InORTDetail = Station.SFCDB.ORM.Queryable<R_LOT_STATUS, R_LOT_DETAIL>((rls, rld) => rls.LOT_NO == rld.LOT_ID)
                        .Where((rls, rld) => rls.SKUNO == SNObj.SKUNO && rls.SAMPLE_STATION == "ORT" && (rld.STATUS == "0" || rld.STATUS == "1"))
                        .OrderBy((rls, rld) => rld.CREATE_DATE, SqlSugar.OrderByType.Desc)
                        .Select((rls, rld) => rld)
                        .ToList().FirstOrDefault();
                    //DateTime ORTtestTime = (DateTime)InORTDetail.CREATE_DATE;

                    sql = $@"SELECT MAX(rld.CREATE_DATE) FROM   R_LOT_STATUS rls, R_LOT_DETAIL rld WHERE  rls.LOT_NO = rld.LOT_ID AND rls.SKUNO = '{SNObj.SKUNO}' AND  rls.SAMPLE_STATION = 'ORT' AND  (rld.STATUS = '0' or rld.STATUS = '1')
                            AND RLD.CREATE_DATE BETWEEN TRUNC(SYSDATE, 'Q')  and sysdate";
                    dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                    string Orttesttime = dt.Rows[0]["MAX(rld.CREATE_DATE)"].ToString();
                    string orttime = Orttesttime.Remove(Orttesttime.Length - 3);

                    //var stationdetail = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(r => r.SKUNO == SNObj.SKUNO && r.STATION_NAME == Station.StationName && r.VALID_FLAG == "1" && r.EDIT_TIME > ORTtestTime).ToList();
                    sql = $@"select * from R_SN_STATION_DETAIL where skuno='{SNObj.SKUNO}' and STATION_NAME='{Station.StationName}'  and VALID_FLAG='1' and EDIT_TIME > TO_DATE('{orttime}','mm/dd/yyyy HH24:MI:SS') and EDIT_TIME BETWEEN TRUNC(SYSDATE, 'Q')  and sysdate";
                    dt = Station.SFCDB.ExecSelect(sql).Tables[0];
                    //Calculate the number of draws from the previous one to the next
                    int SnStationCount = dt.Rows.Count;
                    //Using 100 as the unit, calculate how many samples need to be tested for every 100 pieces
                    int SeTValue = int.Parse(Skusample.VALUE.Replace("%", ""));
                    int SamPle = 100 / SeTValue;//Smapling rate :one of  "SamPle"

                    //int SamPle = (int)Math.Floor(100 * SampleValue);
                    if (SnStationCount == SamPle)
                    {
                        Station.SFCDB.ORM.Updateable<R_LOT_STATUS>().SetColumns(r => new R_LOT_STATUS
                        {
                            LOT_QTY = SkuProductQty,
                            SAMPLE_QTY = Math.Floor(DoSample),//Rounded up
                            PASS_QTY = r.PASS_QTY + 1,
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                            EDIT_TIME = DateTime.Now
                        }).Where(r => r.SKUNO == SNObj.SKUNO && r.SAMPLE_STATION == "ORT").ExecuteCommand();

                        Rdetail.ID = Lot_Detail.GetNewID(Station.BU, Station.SFCDB);
                        Rdetail.LOT_ID = Rstatus.LOT_NO;
                        Rdetail.SN = SNObj.SN;
                        Rdetail.WORKORDERNO = SNObj.WORKORDERNO;
                        Rdetail.CREATE_DATE = DateTime.Now;
                        Rdetail.STATUS = "0";
                        Station.SFCDB.ExecSQL(Rdetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        Station.SFCDB.CommitTrain();
                        //No UPDATE EDIT_EMP and  EDIT_TIME ,It is used when reserved for CANCLE ORT

                        //增加彈窗提示
                        UIInputData O = new UIInputData()
                        {
                            Timeout = 60000,
                            UIArea = new string[] { "25%", "28%" },
                            IconType = IconType.None,
                            MustConfirm = false,
                            Message = "OK",
                            Tittle = "",
                            Type = UIInputType.Confirm,
                            Name = "",
                            ErrMessage = ""
                        };
                        //O.OutInputs.Add(new DisplayOutPut() { Name = "Alert", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"{SNObj.SerialNo} has been chosen to ORT test !" });
                        O.OutInputs.Add(new DisplayOutPut() { Name = "Alert", DisplayType = UIOutputType.TextArea.ToString(), Value = $@"{SNObj.SN} has been chosen to ORT test !" });
                        O.GetUiInput(Station.API, UIInput.Normal, Station);
                    }
                }
            }
        }

        /// <summary>
        /// When FunctionConfig: REWORK_UPDATE_SHIPFLAG will update shipFlag=0, so need a new action to update back 2022-01-13
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReworkUpdateShipFlagControl(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var _SN = (SN)SNSession.Value;
            var r_SN = new T_R_SN(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            var isControl = r_SN.CheckReworkUpdateShipFlagControl(_SN.SerialNo, Station.SFCDB);
            var isLink = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where(t => t.VALUE == _SN.SerialNo && t.VALID_FLAG == 1).Any();

            if (isControl && isLink && _SN.ShippedFlag == "0")
            {
                Station.SFCDB.ORM.Updateable<R_SN>().SetColumns(r => new R_SN { SHIPPED_FLAG = "1", }).Where(r => r.SN == _SN.SerialNo && r.VALID_FLAG == "1" && r.SHIPPED_FLAG == "0").ExecuteCommand();
            }
        }

        /// <summary>
        /// scan pass ort for juniper follow log test te push.
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>OrtPassAction_juniper
        public static void OrtPassAction_juniper(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                var ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new Exception("Error: Can't finded SkuSession!");
            }
            SKU SKU = (SKU)SkuSession.Value;
            SN sn = null;

            if (SNSession.Value.GetType().Name.ToUpper() == "STRING")
            {
                sn = new SN(SNSession.Value.ToString(), Station.SFCDB, DB_TYPE_ENUM.Oracle);
            }
            else
            {
                sn = (SN)SNSession.Value;
            }
            string Skuno = SKU.SkuNo;
            string Sn = sn.SerialNo;
            string Routeid = sn.RouteID;
            string Wo = sn.WorkorderNo;
            string Snid = sn.ID;
            string Version = SKU.Version;


            R_TEST_RECORD RO = Station.SFCDB.ORM.Queryable<R_TEST_RECORD>().Where(t => t.SN == Sn && t.MESSTATION == "ORT" && t.STATE == "PASS").OrderBy(t => t.EDIT_TIME, OrderByType.Desc).ToList().FirstOrDefault();
            if (RO != null)
            {
                DateTime? OrtStartTime = null;
                DateTime? OrtOutTime = RO.EDIT_TIME;
                OrtStartTime = RO.STARTTIME;

                DateTime outtime = Convert.ToDateTime(OrtOutTime);
                DateTime startime = Convert.ToDateTime(OrtStartTime);
                TimeSpan duration = outtime - startime;
                if (duration.TotalHours > 168)
                {
                    var r_ORT = Station.SFCDB.ORM.Queryable<R_LOT_DETAIL>().Where(t => t.SN == Sn && t.EDIT_TIME < OrtOutTime && t.STATUS != "2").ToList();
                    if (r_ORT.Count != 0)
                    {
                        Station.SFCDB.ORM.Updateable<R_LOT_DETAIL>().SetColumns(t => new R_LOT_DETAIL
                        {
                            STATUS = "1",
                            EDIT_EMP = Station.LoginUser.EMP_NO,
                            EDIT_TIME = DateTime.Now
                        }).Where(t => t.ID == sn.ID && t.SN == Sn && t.STATUS == "0").ExecuteCommand();
                    }
                    else
                    {
                        throw new Exception($@"{Sn} NOT STATUS TEST ORT");
                    }
                }


            }
            else
            {
                throw new Exception($@"{Sn} NOT HAVE LOG TEST PASS");
            }
        }


        public static void TestPassByWo(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {

            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWo == null || sessionWo.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession sessionStation = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionStation == null || sessionStation.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            WorkOrder objWO = (WorkOrder)sessionWo.Value;
            try
            {
                T_R_SN t_R_SN = new T_R_SN(Station.SFCDB, Station.DBType);
                var user = Station.SFCDB.ORM.Queryable<C_USER>().Where(x => x.EMP_NO == Station.LoginUser.EMP_NO).ToList();
                var listsn = Station.SFCDB.ORM.Queryable<R_SN>().Where(x => x.WORKORDERNO == objWO.WorkorderNo && x.VALID_FLAG == "1" && x.REPAIR_FAILED_FLAG != "1" && x.NEXT_STATION == sessionStation.Value.ToString()).ToList();
                if (listsn.Count == 0)
                {
                    throw new Exception("NOT VALUE");
                }
                int TimeOut = 90000;
                MESAPIClient MESAPI =
                   new MESAPIClient("ws://localhost:2130/ReportService",
                   Station.LoginUser.EMP_NO,
                   user[0].EMP_PASSWORD.ToString());
                JToken JStation;
                JToken CurrStationInput;
                MESAPI.Connect();
                for (int i = 0; i <= listsn.Count; i++)
                {
                    JStation = null;
                    MESAPIData mesdata = new MESAPIData();
                    mesdata.Class = "MESStation.Stations.CallStation";
                    mesdata.Function = "InitStation";
                    mesdata.Data = new { DisplayStationName = sessionStation.Value.ToString(), Line = Station.Line };
                    JObject JO = MESAPI.CallMESAPISync(mesdata, TimeOut);
                    if (JO["Status"].ToString() == "Pass")
                    {
                        JStation = JO["Data"]["Station"];
                    }
                    else
                    {
                        continue;
                    }
                    string strRET = "";
                    try
                    {
                        for (int k = 0; k < JStation["Inputs"].Count(); k++)
                        {
                            if (JStation["Inputs"][k]["DisplayName"].ToString() == "SN")
                            {
                                CurrStationInput = JStation["Inputs"][k];
                                CurrStationInput["Value"] = listsn[i].SN.ToString();
                                //MESAPIData mesdata = new MESAPIData();
                                mesdata.Class = "MESStation.Stations.CallStation";
                                mesdata.Function = "StationInput";
                                mesdata.Data = new { Station = JStation, Input = CurrStationInput, ScanType = "Pass" };
                                JObject JO1 = MESAPI.CallMESAPISync(mesdata, TimeOut);

                                if (JO1["Status"].ToString() == "Pass")
                                {
                                    JStation = JO1["Data"]["Station"];
                                    for (int j = 0; j < JStation["StationMessages"].Count(); j++)
                                    {
                                        strRET += JStation["StationMessages"][j]["Message"].ToString() + "\r\n";
                                    }

                                    if (JO1["Message"].ToString().EndsWith("Input not successfull."))
                                    {
                                        continue;
                                    }
                                    for (int j = 0; j < JStation["StationMessages"].Count(); j++)
                                    {
                                        strRET += JStation["StationMessages"][j]["Message"].ToString() + "\r\n";
                                    }

                                    if (JO1["Data"]["Station"]["ScanKP"].Count() > 0)
                                    {
                                        continue;
                                    }
                                }
                                else
                                {
                                    continue;
                                }
                            }
                        }
                    }
                    catch (Exception ee)
                    {
                        throw ee;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        /// <summary>
        /// cto_kit_NEW LOAD R_JNP_PD_KIT_MAIN FROM R_SAP_AS_BOM
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CtoKitLoadRequestAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null || WOSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession PalletList = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PalletList == null)
            {
                PalletList = new MESStationSession() { MESDataType = Paras[1].SESSION_TYPE, SessionKey = Paras[1].SESSION_KEY, Value = Paras[1].VALUE };
                Station.StationSession.Add(PalletList);
            }

            var _WO = (WorkOrder)WOSession.Value;

            string Strsql1 = $@" SELECT WO, PN PARTNO, ORDERQTY, COUNT(SN) SCANQTY
                                   FROM (SELECT A.WO, A.PN, A.ORDERQTY, C.SN
                                           FROM C_SKU B, R_SAP_PODETAIL A
                                           left join R_JNP_PD_KIT_DETAIL C
                                             ON A.WO = C.WO
                                            AND C.PARTNO = A.PN
                                          WHERE A.PN = B.SKUNO
                                            AND A.WO = '{_WO.WorkorderNo}'
                                            AND B.SKU_TYPE <> 'VIRTUAL')
                                  GROUP BY WO, PN, ORDERQTY ";
            List<object> palletlist = Station.SFCDB.ORM.SqlQueryable<object>(Strsql1).ToList();
            PalletList.Value = palletlist;

            string sql = $@"SELECT A.WO, A.PN, A.ORDERQTY
                              FROM R_SAP_PODETAIL A, C_SKU B
                             WHERE A.PN = B.SKUNO
                               AND WO = '{_WO.WorkorderNo}'
                               AND B.SKU_TYPE <> 'VIRTUAL'
                               and NOT EXISTS (select SCANQTY from (SELECT COUNT(SN) AS SCANQTY FROM 
                               R_JNP_PD_KIT_DETAIL C WHERE A.WO=C.WO
                               AND C.PARTNO=A.PN )D where D.SCANQTY=A.ORDERQTY  )";


            DataSet ds = Station.SFCDB.ExecSelect(sql);

            if (ds.Tables[0].Rows.Count == 0)
            {
                Station.StationMessages.Add(new StationMessage() { Message = $@"WO:{_WO.WorkorderNo } cto_kit pass,do not need to scan" });
            }
        }



        /// <summary>
        /// cto_kit pass action
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CtoKitPassAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }

            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null || WOSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var _WO = (WorkOrder)WOSession.Value;
            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SNSession == null || SNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            var _SN = (SN)SNSession.Value;

            MESStationSession PalletList = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (PalletList == null)
            {
                PalletList = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY, Value = Paras[2].VALUE };
                Station.StationSession.Add(PalletList);
            }



            var KIT_MAIN = Station.SFCDB.ORM.Queryable<R_SAP_PODETAIL>().Where(t => t.WO == _WO.WorkorderNo && t.PN == _SN.SkuNo).ToList();

            if (KIT_MAIN.Count == 0)
            {
                throw new Exception($@"WO’s R_SAP_PODETAIL do not have this sn’s PN ");
            }

            var REQUESTQTY = Convert.ToDouble(KIT_MAIN[0].ORDERQTY);

            var PD_KIT = Station.SFCDB.ORM.Queryable<R_JNP_PD_KIT_DETAIL>().Where(t => t.WO == _WO.WorkorderNo && t.PARTNO == _SN.SkuNo && t.VALID_FLAG == "1").ToList();
            var HaveScanQty = PD_KIT.Count;
            if (HaveScanQty == REQUESTQTY)
            {
                throw new Exception($@"{_SN.SkuNo}  HAVE scan full for this wo:{_WO.WorkorderNo}");

            }
            var isExists = Station.SFCDB.ORM.Queryable<R_JNP_PD_KIT_DETAIL>().Where(t => t.SN == _SN.SerialNo && t.VALID_FLAG == "1").Any();


            var r_JNP_PD_KIT_DETAIL = new T_R_JNP_PD_KIT_DETAIL(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string SID = r_JNP_PD_KIT_DETAIL.GetNewID(Station.BU, Station.SFCDB);

            if (isExists)
            {
                throw new Exception($@"{_SN.SerialNo}  HAVE EXISTS IN R_JNP_PD_KIT ,You do not need to scan again");

            }
            else
            {
                string StrSql = $@" 
                insert into R_JNP_PD_KIT_DETAIL (ID, SKUNO, WO, PARTNO, SN, VALID_FLAG, REQUESTQTY, EDIT_TIME, EDIT_BY)
                values ('{SID}', '{_WO.SkuNO}', '{_WO.WorkorderNo}', '{_SN.SkuNo}', '{_SN.SerialNo}', '1', '{REQUESTQTY}', sysdate,'{Station.LoginUser.EMP_NO}')";

                try
                {
                    Station.SFCDB.ExecSQL(StrSql);
                }
                catch (Exception EX)
                {
                    throw new Exception(StrSql + "," + EX.Message);
                }
            }

            string Strsql1 = $@" SELECT WO, PN PARTNO, ORDERQTY, COUNT(SN) SCANQTY
                                   FROM (SELECT A.WO, A.PN, A.ORDERQTY, C.SN
                                           FROM C_SKU B, R_SAP_PODETAIL A
                                           left join R_JNP_PD_KIT_DETAIL C
                                             ON A.WO = C.WO
                                            AND C.PARTNO = A.PN
                                          WHERE A.PN = B.SKUNO
                                            AND A.WO = '{_WO.WorkorderNo}'
                                            AND B.SKU_TYPE <> 'VIRTUAL')
                                  GROUP BY WO, PN, ORDERQTY ";
            List<object> palletlist = Station.SFCDB.ORM.SqlQueryable<object>(Strsql1).ToList();
            PalletList.Value = palletlist;



            string sql = $@"SELECT A.WO, A.PN, A.ORDERQTY
                              FROM R_SAP_PODETAIL A, C_SKU B
                             WHERE A.PN = B.SKUNO
                               AND WO = '{_WO.WorkorderNo}'
                               AND B.SKU_TYPE <> 'VIRTUAL'
                               and NOT EXISTS (select SCANQTY from (SELECT COUNT(SN) AS SCANQTY FROM 
                               R_JNP_PD_KIT_DETAIL C WHERE A.WO=C.WO
                               AND C.PARTNO=A.PN )D where D.SCANQTY=A.ORDERQTY  )";


            DataSet ds = Station.SFCDB.ExecSelect(sql);

            if (ds.Tables[0].Rows.Count == 0)
            {
                Station.StationMessages.Add(new StationMessage() { Message = $@"WO:{_WO.WorkorderNo } cto_kit pass,do not need to scan" });
            }

        }

        /// <summary>
        /// ctokit show list
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void CtoKitShowAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (WOSession == null || WOSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            var _WO = (WorkOrder)WOSession.Value;
            MESStationSession PARTNOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (PARTNOSession == null || PARTNOSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            var partno = PARTNOSession.Value;

            DataTable dt = new DataTable();
            string sql = $@"select  WO, PARTNO, SN, EDIT_TIME, EDIT_BY from   R_JNP_PD_KIT_DETAIL WHERE WO='{_WO.WorkorderNo}' and VALID_FLAG ='1' AND PARTNO='{partno}' ";
            DataSet ds = Station.SFCDB.ExecSelect(sql);
            dt = ds.Tables[0];

            UIInputData ProgressWin = new UIInputData() { };
            ProgressWin.Timeout = 30000;
            ProgressWin.IconType = IconType.Message;
            ProgressWin.Type = UIInputType.Table;
            ProgressWin.ReturnData = new DataTable();
            ProgressWin.Tittle = "R_JNP_PD_KIT_DETAIL";
            ProgressWin.ErrMessage = "SHOW OK";
            ProgressWin.UIArea = new string[] { "80%", "80%" };
            ProgressWin.OutInputs.Clear();
            ProgressWin.Name = "SHOW R_JNP_PD_KIT_DETAIL";
            ProgressWin.CBMessage = "Detail";

            ProgressWin.OutInputs.Add(new DisplayOutPut()
            {
                DisplayType = UIOutputType.Table.ToString(),
                Name = "layer",
                Value = dt

            });

            ProgressWin.GetUiInput(Station.API, UIInput.Normal);

        }

        /// <summary>
        /// ADD BY HGB 2022.03.12 pe保康經常要it手動替換，風險大，費時間
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void Replacer_sn_kp_value(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            MESStationSession OldSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (OldSNSession == null || OldSNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            string OldSN = OldSNSession.Value.ToString();

            MESStationSession NewSNSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (NewSNSession == null || NewSNSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string NewSN = NewSNSession.Value.ToString();

            if (NewSN.Length != OldSN.Length)
            {
                throw new Exception($@"{NewSN},{NewSN}  new sn'length is not equit to old sn'length  ");
            }


            string newsql = $@"select* from   r_sn   
                  wheRe SN='{NewSN}' AND VALID_FLAG='1'   ";

            DataSet ds = Station.SFCDB.ExecSelect(newsql);
            DataTable dtnewsndt = ds.Tables[0];

            string oldsql = $@"select* from   r_sn   
                  wheRe SN='{OldSN}' AND VALID_FLAG='1'   ";

            ds = Station.SFCDB.ExecSelect(oldsql);
            DataTable dtOldSNdt = ds.Tables[0];


            if (dtOldSNdt.Rows.Count == 0 && dtnewsndt.Rows.Count == 0)
            {
                //NOT FOXCONN PRODUCT
                //replace
                string StrSql = $@" 
                 UPDATE r_sn_kp SET  VALUE='{NewSN}' wheRe VALUE='{OldSN}' AND VALID_FLAG='1' AND VALUE<>SN AND  UPPER(kp_name) like 'AUTOKP%'  ";
                try
                {
                    Station.SFCDB.ExecSQL(StrSql);
                }
                catch (Exception EX)
                {
                    throw new Exception(StrSql + "," + EX.Message);
                }
            }
            else if((dtOldSNdt.Rows.Count == 0 && dtnewsndt.Rows.Count != 0)||(dtOldSNdt.Rows.Count != 0 && dtnewsndt.Rows.Count == 0))
            {
                //ONE IS FOXCONN  ONE  IS NOT
                throw new Exception(  " ONE IS FOXCONN PRODUCT ,OTHER  IS NOT");
            }
            else
            {
                 
                if (dtnewsndt.Rows.Count > 1)
                {
                    throw new Exception($@"{NewSN}  r_sn  exists TWO record   checksql is   " + newsql);
                }
                 
                if (dtOldSNdt.Rows.Count > 1)
                {
                    throw new Exception($@"OldSN {OldSN}  r_sn exists TWO record   checksql is  " + oldsql);
                }
                 
                if (dtnewsndt.Rows[0]["COMPLETED_FLAG"].ToString() != "1" || dtnewsndt.Rows[0]["NEXT_STATION"].ToString() != "JOBFINISH")
                {
                    throw new Exception($@"{NewSN}  r_sn not COMPLETED   OR SN IS WAIT REWORK is   " + newsql);
                }
                 

               
               string sql = $@"select* from   r_sn_kp  
                  wheRe VALUE='{NewSN}' AND VALID_FLAG='1' AND VALUE<>SN  AND UPPER(kp_name) like 'AUTOKP%'  ";

                ds = Station.SFCDB.ExecSelect(sql);

                if (ds.Tables[0].Rows.Count > 0)
                {
                    throw new Exception($@"{NewSN}  Have been bundle  checksql is  " + sql);
                }

                if (dtnewsndt.Rows[0]["SHIPPED_FLAG"].ToString() == "1")
                {
                    throw new Exception($@"{NewSN}  r_sn aready shipped   checksql is   " + newsql);
                }


                sql = $@"select * from r_sn_lock A where A.SN='{NewSN}' AND A.LOCK_STATUS='1' ";
                ds = Station.SFCDB.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    throw new Exception($@"{NewSN} is locked checksql is  " + sql);
                } 
                 

                if (dtOldSNdt.Rows[0]["COMPLETED_FLAG"].ToString() != "1" || dtOldSNdt.Rows[0]["NEXT_STATION"].ToString() != "JOBFINISH")
                {
                    throw new Exception($@"{OldSN}  r_sn not COMPLETED OR SN IS WAIT REWORK   checksql is  " + oldsql);
                } 
               

                sql = $@"select* from   r_sn_kp  
                  wheRe VALUE='{OldSN}' AND VALID_FLAG='1' AND VALUE<>SN  AND UPPER(kp_name) like 'AUTOKP%'  ";

                ds = Station.SFCDB.ExecSelect(sql);

                if (ds.Tables[0].Rows.Count == 0)
                {
                    throw new Exception($@"OldSN {OldSN}  Have not been bundle  , checksql is  " + sql);
                }

                if (dtOldSNdt.Rows[0]["SHIPPED_FLAG"].ToString() != "1")
                {
                    throw new Exception($@" OldSN { OldSN }  r_sn not shipped    checksql is    " + oldsql);
                }

                if (ds.Tables[0].Rows.Count > 1  )
                {
                    throw new Exception($@"OldSN {OldSN}  Have been bundle by two or more sn   , checksql is  " + sql);
                }

                
                //sql = $@"select * from r_sn_lock A where A.SN='{OldSN}' AND A.LOCK_STATUS='1' ";
                //ds = Station.SFCDB.ExecSelect(sql);
                //if (ds.Tables[0].Rows.Count > 0)
                //{
                //    throw new Exception($@"{OldSN} is locked in r_sn_lock    checksql is   " + sql);
                //}


                //replace
                string StrSql = $@" 
                 UPDATE r_sn_kp SET  VALUE='{NewSN}' wheRe VALUE='{OldSN}' AND VALID_FLAG='1' AND VALUE<>SN  AND UPPER(kp_name) like 'AUTOKP%' ";
                try
                {
                    Station.SFCDB.ExecSQL(StrSql);
                }
                catch (Exception EX)
                {
                    throw new Exception(StrSql + "," + EX.Message);
                }

                StrSql = $@" 
                  UPDATE r_sn SET SHIPPED_FLAG='1', SHIPDATE=SYSDATE where SN='{NewSN}' AND VALID_FLAG='1'  ";
                try
                {
                    Station.SFCDB.ExecSQL(StrSql);
                }
                catch (Exception EX)
                {
                    throw new Exception(StrSql + "," + EX.Message);
                }

                StrSql = $@" 
                UPDATE r_sn SET SHIPPED_FLAG='0', SHIPDATE='' where SN='{OldSN}' AND VALID_FLAG='1' ";
                try
                {
                    Station.SFCDB.ExecSQL(StrSql);
                }
                catch (Exception EX)
                {
                    throw new Exception(StrSql + "," + EX.Message);
                }
            }


            T_R_MES_LOG mesLog = new T_R_MES_LOG(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            string id = mesLog.GetNewID("JZJNP", Station.SFCDB);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
            rowMESLog.ID = id;
            rowMESLog.PROGRAM_NAME = "CloudMES";
            rowMESLog.CLASS_NAME = "SNAction";
            rowMESLog.FUNCTION_NAME = "Replacer_sn_kp_value";
            rowMESLog.LOG_MESSAGE = OldSN + "-->" + NewSN;
            rowMESLog.LOG_SQL = "";
            rowMESLog.EDIT_EMP = Station.LoginUser.EMP_NO;
            rowMESLog.EDIT_TIME = System.DateTime.Now;
            rowMESLog.DATA1 = $@"{ NewSN}";
            rowMESLog.DATA2 = $@"{ OldSN}";
            rowMESLog.DATA3 = "";
            Station.SFCDB.ThrowSqlExeception = true;
            Station.SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));

            Station.StationMessages.Add(new StationMessage() { Message = $@" Replace success" });

        }


    }
}