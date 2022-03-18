using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MESDataObject.Module.Juniper;
using MESStation.Management;

namespace MESStation.KeyPart
{
    public class KPScan : MesAPIBase
    {
        protected APIInfo _UpLoadKPList = new APIInfo()
        {
            FunctionName = "UpLoadKPList",
            Description = "UpLoadKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SkuNo", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ListName", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ListData", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _RemoveKPList = new APIInfo()
        {
            FunctionName = "RemoveKPList",
            Description = "RemoveKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ListNames", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetSNStationKPList = new APIInfo()
        {
            FunctionName = "GetSNStationKPList",
            Description = "GetSNStationKPList",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "WO", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "STATION", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _ScanKPItem = new APIInfo()
        {
            FunctionName = "ScanKPItem",
            Description = "ScanKPItem",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "STATION", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "KPITEM", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ISTEST", InputType = "STRING", DefaultValue = "FALSE"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetKPListBySkuno = new APIInfo()
        {
            FunctionName = "GetKPListBySkuno",
            Description = "GetKPListBySkuno",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SKUNO", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetAllKPList = new APIInfo()
        {
            FunctionName = "GetAllKPList",
            Description = "GetAllKPList",
            Parameters = new List<APIInputInfo>(),
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _CheckKPListName = new APIInfo()
        {
            FunctionName = "CheckKPListName",
            Description = "CheckKPListName",
            Parameters = new List<APIInputInfo>() {
                new APIInputInfo() { InputName = "ListName", InputType = "STRING", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _GetKPListByListName = new APIInfo()
        {
            FunctionName = "GetKPListByListName",
            Description = "GetKPListByListName",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ListName", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetSNAllKeypart = new APIInfo()
        {
            FunctionName = "GetSNAllKeypart",
            Description = "Get SN All Keypart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo _ReplaceSNKeypart = new APIInfo()
        {
            FunctionName = "ReplaceSNKeypart",
            Description = "Replace SN Keypart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "NEW_PARTNO", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "NEW_VALUE", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "NEW_MPN", InputType = "STRING", DefaultValue = ""}                
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };
        protected APIInfo FGetReplaceReturnStation = new APIInfo()
        {
            FunctionName = "GetReplaceReturnStation",
            Description = "Get Replace Return Station",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "ROW_KP", InputType = "STRING", DefaultValue = ""}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public KPScan()
        {
            this.Apis.Add(_GetSNStationKPList.FunctionName, _GetSNStationKPList);
            this.Apis.Add(_GetKPListBySkuno.FunctionName, _GetKPListBySkuno);
            this.Apis.Add(_GetAllKPList.FunctionName, _GetAllKPList);
            this.Apis.Add(_UpLoadKPList.FunctionName, _UpLoadKPList);
            this.Apis.Add(_RemoveKPList.FunctionName, _RemoveKPList);
            this.Apis.Add(_CheckKPListName.FunctionName, _CheckKPListName);
            this.Apis.Add(_GetKPListByListName.FunctionName, _GetKPListByListName);
            this.Apis.Add(_ScanKPItem.FunctionName, _ScanKPItem);
            this.Apis.Add(_GetSNAllKeypart.FunctionName, _GetSNAllKeypart);
            this.Apis.Add(FGetReplaceReturnStation.FunctionName, FGetReplaceReturnStation);
        }
        public void ScanKPItem(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            MESDBHelper.OleExec APDB = this.DBPools["APDB"].Borrow();
            MESDBHelper.OleExec SFCDB = null;
            bool isBorrow = true;

            try
            {
                string strSN = Data["SN"].ToString();
                string station = Data["STATION"].ToString();
                JToken _ItemData = Data["KPITEM"];

                if (this.DBPools["SFCDB"].ShareDB.Keys.Contains(strSN))
                {
                    SFCDB = this.DBPools["SFCDB"].ShareDB[strSN];
                    isBorrow = false;
                }
                else
                {
                    SFCDB = this.DBPools["SFCDB"].Borrow();
                }

                if (isBorrow)
                {
                    SFCDB.BeginTrain();
                    APDB.BeginTrain();
                }

                /*
                 new APIInputInfo() {InputName = "SN", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "STATION", InputType = "STRING", DefaultValue = ""},
                new APIInputInfo() {InputName = "KPITEM", InputType = "STRING", DefaultValue = ""}
                 */


                bool IsTest = false;
                try
                {
                    string StrIsTest = Data["ISTEST"] == null ? "" : Data["ISTEST"].ToString();
                    if (StrIsTest.ToUpper() == "TRUE")
                    {
                        IsTest = true;
                    }
                }
                catch
                {

                }

                T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);
                T_R_SN t_r_sn = new T_R_SN(SFCDB, DB_TYPE_ENUM.Oracle);
                //20190121 patty added for Oracel: PN can have differnet MPNs, store the MPN and REGEX into R_SN_KP when scanning.
                R_SN R_SN_Data = t_r_sn.LoadSN(strSN, SFCDB);
                string str_skuno = R_SN_Data.SKUNO;
                MESDataObject.Module.T_R_WO_BASE O_TWO = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE row_r_wo_base = O_TWO.LoadWorkorder(R_SN_Data.WORKORDERNO, SFCDB);
                string plantO = row_r_wo_base.PLANT;
                string production_type = row_r_wo_base.PRODUCTION_TYPE;
                T_R_FUNCTION_CONTROL TRFC = new T_R_FUNCTION_CONTROL(SFCDB, DB_TYPE_ENUM.Oracle);

                List<R_SN_KP> ItemData = new List<R_SN_KP>();
                for (int i = 0; i < _ItemData.Count(); i++)
                {
                    #region 這個卡關放在這裡會影響到所有keypart 的掃描，故即將遷移到每一個KP TYPE的單獨卡關裡面，如有涉及到的BU 請及時變更 2020.03.20 add by fgg

                    string _partNo = _ItemData[i]["PARTNO"].ToString();
                    string Csn = _ItemData[i]["VALUE"].ToString();

                    var isAruba = SFCDB.ORM.Queryable<C_SKU, C_SERIES, C_CUSTOMER>((sku, se, cus) => sku.C_SERIES_ID == se.ID && se.CUSTOMER_ID == cus.ID)
                                        .Where((sku, se, cus) => sku.SKUNO == _partNo && cus.CUSTOMER_NAME.ToUpper() == "ARUBA").Select((sku, se, cus) => cus).Any();
                    if (isAruba && !t_r_sn.CheckExists(Csn, SFCDB))
                    {
                        int position = Csn.IndexOf(' ');
                        if (position > 0)
                        {
                            Csn = Csn.Substring(0, position);
                        }
                    }

                    //Keypart掃描時檢查是否被綁定過
                    if (!TRKP.IsNoCheckLinkScanType(_ItemData[i]["SCANTYPE"].ToString(), SFCDB))
                    {
                        //已被綁定的就不能重複綁定
                        if (TRKP.CheckLinkByValue(Csn, SFCDB))
                        {
                            if (LoginUser.FACTORY == "FTX") //FTX needs to show married SN
                            {
                                List<R_SN_KP> LinkedKP =
                                    TRKP.GetKPRecordByValue(Csn, SFCDB);
                                throw new Exception(Csn + " has been link on other sn: " + LinkedKP[0].SN);
                            }
                            //else
                            //{
                                //這個卡關放在這裡會影響到所有keypart 的掃描，故即將遷移到每一個KP TYPE的單獨卡關裡面
                                //throw new Exception(_ItemData[i]["VALUE"].ToString() + " has been link on other sn!");
                            //}
                        }
                    }
                    #endregion

                    #region Lock Checker
                    var locks = LockManager.CheckLock(_ItemData[i]["VALUE"].ToString(), "CSN", station, SFCDB);
                    var locksCSN = LockManager.CheckLock(Csn, "CSN", station, SFCDB);
                    if (locks.Count > 0 || locksCSN.Count > 0)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210628172337", new string[] { locks[0].TYPE, _ItemData[i]["VALUE"].ToString(), locks[0].LOCK_EMP, locks[0].LOCK_REASON }));
                    }
                    #endregion

                    if (_ItemData[i]["SCANTYPE"].ToString() == "ACC KIT S/N"
                      && TRFC.CheckUserFunctionExist("ACC KIT S/N", "ACC KIT S/N", str_skuno, SFCDB))
                    {
                        if (R_SN_Data.SN == Csn)
                        {
                            _ItemData[i]["VALUE"] = "~" + Csn;
                        }
                        else
                        {
                            throw new Exception("ACC KIT S/N needs to be the same as SN.");
                        }
                    }

                    R_SN r_sn = new R_SN();
                    r_sn = t_r_sn.LoadSN(Csn, SFCDB);

                    // NLEZ 不卡SCANTYPE =LabelSN
                    if (plantO == "NLEZ" && _ItemData[i]["SCANTYPE"].ToString() == "LabelSN")
                    {

                    }

                    //如果KEYPART是本廠做的條碼則必須是完工狀態
                    else if (((r_sn != null && r_sn.COMPLETED_FLAG != "1") ||
                              (r_sn != null && r_sn.NEXT_STATION == "REWORK")) &&
                             (plantO != "TOGA" && _ItemData[i]["SCANTYPE"].ToString() != "CHASIS_CHECK"))
                    {
                        throw new Exception(_ItemData[i]["VALUE"].ToString() + " is on " + r_sn.NEXT_STATION + ", can't completed!");
                    }

                    //查詢被綁定的CSN是否未測試完成LLT
                    var IsLLT = SFCDB.ORM.Queryable<R_LLT>().Where(c => c.SN == Csn && (c.STATUS == "0" || c.STATUS == "1")).ToList();

                    if (IsLLT.Count > 0)
                    {
                        throw new Exception(_ItemData[i]["VALUE"].ToString() + " is Need LLT test,can't completed!");
                    }

                    if (BU.Contains("FJZ") || BU.Contains("FVN"))
                    {
                        // Check if sn has not OUT_SILVER_WIP
                        var InSw = SFCDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>().Where(t => t.SN == Csn && t.STATE_FLAG == "1").First();
                        if (InSw != null)
                        {
                            throw new MESReturnMessage($"{Csn} Is in SILVER_WIP,You should scan OUT_SILVER_WIP");
                        }
                        //Add by POHONG 2021/11/19
                        //if (_ItemData[i]["SCANTYPE"].ToString() == "CLEI")
                        //{
                        //    if (Csn.Contains("[)>0625SLBJPNW"))
                        //    {
                        //        if (Csn.StartsWith("[") && Csn.EndsWith(row.CLEI_CODE) && Csn.Contains("[)>0625SLBJPNW" + strSN + "11P" + row.CLEI_CODE))
                        //        {
                        //            throw new MESReturnMessage($"Wrong CLEI Lable,Pls Check");
                        //        }
                        //    }
                        //}
                    }
                    R_SN_KP I = new R_SN_KP();
                    I.ID = _ItemData[i]["ID"].ToString();
                    I.VALUE = Csn;
                    I.MPN = _ItemData[i]["MPN"].ToString();
                    I.PARTNO = _ItemData[i]["PARTNO"].ToString();
                    I.SCANTYPE = _ItemData[i]["SCANTYPE"].ToString();
                    I.ITEMSEQ = double.Parse(_ItemData[i]["ITEMSEQ"].ToString());
                    I.SCANSEQ = double.Parse(_ItemData[i]["SCANSEQ"].ToString());
                    I.DETAILSEQ = double.Parse(_ItemData[i]["DETAILSEQ"].ToString());

                    ItemData.Add(I);
                }

                if (ItemData.Count == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                }

                LogicObject.SN SN = new LogicObject.SN();
                SN.Load(strSN, SFCDB, DB_TYPE_ENUM.Oracle);
                MESDataObject.Module.T_R_WO_BASE TWO = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE RWO = TWO.GetWo(SN.WorkorderNo, SFCDB);

                List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(SN.ID, station, SFCDB);
                SN_KP KPCONFIG;

                KPCONFIG = new SN_KP(snkp, SN.WorkorderNo, SN.SkuNo, SFCDB, BU);



                R_SN_KP kpItem = KPCONFIG.KPS.Find(T => T.ID == ItemData[0].ID);
                if (kpItem == null)
                {
                    throw new Exception("Data Error!");
                }

                List<R_SN_KP> ConfigItem = KPCONFIG.KPS.FindAll(T => T.PARTNO == kpItem.PARTNO
                                                                     && T.ITEMSEQ == kpItem.ITEMSEQ
                                                                     && T.SCANSEQ == kpItem.SCANSEQ
                );

                if (ConfigItem.Count != ItemData.Count)
                {
                    throw new Exception("Data Error! ConfigItem.Count != ItemData.Count");
                }


                List<Row_R_SN_KP> items = new List<Row_R_SN_KP>();
                for (int i = 0; i < ItemData.Count; i++)
                {
                    Row_R_SN_KP item = (Row_R_SN_KP)TRKP.GetObjByID(ItemData[i].ID, SFCDB);
                    if (item.ITEMSEQ == ItemData[i].ITEMSEQ
                        && item.SCANSEQ == ItemData[i].SCANSEQ
                        && item.DETAILSEQ == ItemData[i].DETAILSEQ)
                    {
                        item.VALUE = ItemData[i].VALUE;
                        item.MPN = ItemData[i].MPN;
                        item.PARTNO = ItemData[i].PARTNO;
                        item.EDIT_TIME = DateTime.Now;
                        item.EDIT_EMP = LoginUser.EMP_NO; 
                        SFCDB.ExecSQL(item.GetUpdateString(DB_TYPE_ENUM.Oracle));
                        item.AcceptChange();
                        items.Add(item);
                    }
                    else
                    {
                        throw new Exception("Data Error! 1");
                    }
                }

                for (int i = 0; i < items.Count; i++)
                {
                    var ScanTypes = SFCDB.ORM.Queryable<C_KP_Check>().Where(t => t.TYPENAME == items[i].SCANTYPE)
                        .ToList();
                    if (ScanTypes.Count > 0)
                    {
                        Assembly assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + ScanTypes[0].DLL);
                        Type APIType = assembly.GetType(ScanTypes[0].CLASS);
                        object API_CLASS = assembly.CreateInstance(ScanTypes[0].CLASS);
                        var Methods = APIType.GetMethods();
                        var Funs = Methods.Where<MethodInfo>(t => t.Name == ScanTypes[0].FUNCTION);
                        if (Funs.Count() > 0)
                        {
                            Funs.ElementAt(0).Invoke(API_CLASS,
                                new object[] { KPCONFIG, SN, items[i], items, this, SFCDB, APDB });
                        }
                    }
                }




                StationReturn.Status = StationReturnStatusValue.Pass;
                //如果是测试执行,会将结果回滚.
                if (!IsTest)
                {
                    if (isBorrow)
                    {
                        SFCDB.CommitTrain();
                        APDB.CommitTrain();
                    }
                }
                else
                {
                    SFCDB.RollbackTrain();
                    APDB.RollbackTrain();
                }
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                APDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                if (ee.InnerException != null)
                {
                    StationReturn.MessagePara.Add(ee.InnerException.Message);
                }
                else
                {
                    StationReturn.MessagePara.Add(ee.Message);
                }
            }
            finally
            {
                if (isBorrow)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
                //this.DBPools["SFCDB"].Return(SFCDB);
                this.DBPools["APDB"].Return(APDB);
            }
        }
        public void GetKPListByListName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ListName = Data["ListName"].ToString();
            try
            {
                KPListBase ret = KPListBase.GetKPListByListName(ListName, SFCDB);
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void CheckKPListName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ListName = Data["ListName"].ToString();
            try
            {
                MESDataObject.Module.T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                StationReturn.Data = T.CheckKPListName(ListName,SFCDB);
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetSNStationKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = null;
            bool isBorrow = true;
            //= this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSN = Data["SN"].ToString();
                string strSTATION = Data["STATION"].ToString();
                string strWO = null;
                if (this.DBPools["SFCDB"].ShareDB.Keys.Contains(strSN))
                {
                    SFCDB = this.DBPools["SFCDB"].ShareDB[strSN];
                    isBorrow = false;
                }
                else
                {
                    SFCDB = this.DBPools["SFCDB"].Borrow();
                }

                try
                {
                    strWO = Data["WO"].ToString();
                }
                catch
                { }

                LogicObject.SN SN = new LogicObject.SN();
                SN.Load(strSN, SFCDB, DB_TYPE_ENUM.Oracle);

                MESDataObject.Module.T_R_WO_BASE TWO = new T_R_WO_BASE(SFCDB, DB_TYPE_ENUM.Oracle);
                Row_R_WO_BASE RWO = TWO.GetWo(SN.WorkorderNo, SFCDB);
                T_R_SN_KP TRKP = new T_R_SN_KP(SFCDB, DB_TYPE_ENUM.Oracle);

                List<R_SN_KP> snkp = TRKP.GetKPRecordBySnIDStation(SN.ID, strSTATION, SFCDB);

                //20190315 Patty added for ATO, using PF level instead of SKU level 
                SN_KP ret;
                if (RWO.PLANT == "TOGA")
                {
                    if (RWO.PRODUCTION_TYPE == "ATO" && RWO.SKU_NAME != "ODA_HA" && RWO.SKU_NAME != "BDA_ATO" && RWO.SKU_NAME != "ORACLE_RACK" || (RWO.PRODUCTION_TYPE == "PTO" && RWO.SKU_NAME != "X8-8" && RWO.SKU_NAME != "X7-2C" && RWO.SKU_NAME != "E1-2C" && RWO.SKU_NAME != "E2-2C" && RWO.SKU_NAME != "ODA_X8-2"))
                    {
                        ret = new SN_KP(snkp, SN.WorkorderNo, RWO.SKU_NAME, SFCDB, BU);
                    }
                    else
                    {
                        ret = new SN_KP(snkp, SN.WorkorderNo, SN.SkuNo, SFCDB, BU);
                    }

                }
                else
                {
                    ret = new SN_KP(snkp, SN.WorkorderNo, SN.SkuNo, SFCDB, BU);
                }

                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                //this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                if (isBorrow)
                {
                    this.DBPools["SFCDB"].Return(SFCDB);
                }
            }
        }

        public void UpLoadKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string Skuno = Data["SkuNo"].ToString();
                string ListName = Data["ListName"].ToString();
                Newtonsoft.Json.Linq.JToken ListData = Data["ListData"];
                string CustVersion = Data["CustVersion"].ToString();
                T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string NewListID = T.GetNewID(this.BU,SFCDB);
                Row_C_KP_LIST R = (Row_C_KP_LIST)T.NewRow();
                DateTime Now = DateTime.Now;

                T_C_KP_List_Item TItem = new T_C_KP_List_Item(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item RItem = (Row_C_KP_List_Item)TItem.NewRow();

                T_C_KP_List_Item_Detail TDetail = new T_C_KP_List_Item_Detail(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item_Detail RDetail = (Row_C_KP_List_Item_Detail)TDetail.NewRow();

                KPListBase oldList = KPListBase.GetKPListByListName(ListName, SFCDB);
                if (oldList != null)
                {
                    //NewListID = oldList.ID;
                    //oldList.ReMoveFromDB(SFCDB);
                    throw new Exception($@"KP Name {ListName} is exists!Pls add version# after ListName!");
                }

                R.ID = NewListID;
                R.SKUNO = Skuno.Trim();
                R.LISTNAME = ListName;
                R.EDIT_EMP = this.LoginUser.EMP_NO;
                R.EDIT_TIME = Now;
                R.FLAG = "1";
                R.CUSTVERSION = CustVersion;
                SFCDB.ORM.Updateable<C_KP_LIST>().SetColumns(t => t.FLAG == "0").Where(t => t.SKUNO == R.SKUNO)
                    .ExecuteCommand();
                SFCDB.ExecSQL(R.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                //Item	PartNO	KPName	Station	QTY	ScanType
                DataTable dt = new DataTable();
                dt.Columns.Add("Item");
                dt.Columns.Add("PartNO");
                dt.Columns.Add("KPName");
                dt.Columns.Add("Station");
                dt.Columns.Add("QTY");
                dt.Columns.Add("ScanType");
                dt.Columns.Add("Location");
                List<DataRow> ListItem = new List<DataRow>();
                for (int i = 0; i < ListData.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Item"] = ListData[i]["Item"].ToString();
                    dr["PartNO"] = ListData[i]["PartNO"].ToString();
                    dr["KPName"] = ListData[i]["KPName"].ToString();
                    dr["Station"] = ListData[i]["Station"].ToString();
                    dr["QTY"] = ListData[i]["QTY"].ToString();
                    dr["ScanType"] = ListData[i]["ScanType"].ToString();
                    dr["Location"] = ListData[i]["Location"].ToString();
                    dt.Rows.Add(dr);
                    ListItem.Add(dr);
                }

                //檢查上傳kp Location位置是否存在逗號,
                DataView dv = new DataView(dt);
                DataTable dt2 = dv.ToTable(false, "PartNO", "KPName", "Station", "ScanType", "Location");
                foreach (DataRow dataRow in dt2.Rows)
                {
                    var s = dataRow["Location"].ToString();
                    if (s.Contains(","))
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20211215161228", new string[] { s }));
                    }
                }
                
                // 檢查上傳的kp是否有重複數據
                var query = from isbn in dt2.Rows.Cast<DataRow>()
                            group isbn by new
                            {
                                PartNO = isbn["PartNO"],
                                KPName = isbn["KPName"],
                                Station = isbn["Station"],
                                ScanType = isbn["ScanType"],
                                Location = isbn["Location"]
                            } into isbn2
                            select new
                            {
                                sapComCode = isbn2.Key,
                                count = isbn2.Count()
                            };

                var items = query.Where(A => A.count > 1);
                if (items.Count() > 0)
                {
                    string sameSapComCode = string.Empty;
                    foreach (var item in items)
                    {
                        sameSapComCode += item.sapComCode + ";\n";
                    }
                    sameSapComCode = sameSapComCode.TrimEnd(';');
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20211215152436", new string[] { sameSapComCode }));
                }

                Dictionary<string, string> Item = new Dictionary<string, string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!Item.ContainsKey(dt.Rows[i]["Item"].ToString()))
                    {
                        Item.Add(dt.Rows[i]["Item"].ToString(), dt.Rows[i]["PartNO"].ToString());
                    }
                }
                string[] itemNo = new string[Item.Keys.Count];
                Item.Keys.CopyTo(itemNo, 0);
                for (int i = 0; i < itemNo.Length; i++)
                {
                    List<DataRow> Items = ListItem.FindAll(TT => TT["Item"].ToString() == itemNo[i]);
                    for (int j = 0; j < Items.Count; j++)
                    {
                        if (j == 0)
                        {
                            RItem.ID = TItem.GetNewID(BU, SFCDB);
                            RItem.LIST_ID = NewListID;
                            RItem.KP_NAME = Items[j]["KPName"].ToString();
                            RItem.KP_PARTNO = Items[j]["PartNO"].ToString();
                            RItem.STATION = Items[j]["Station"].ToString();
                            RItem.QTY = double.Parse( Items[j]["QTY"].ToString());
                            RItem.SEQ = i;
                            RItem.EDIT_EMP = this.LoginUser.EMP_NO;
                            RItem.EDIT_TIME = Now;
                            SFCDB.ExecSQL(RItem.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        }

                        RDetail.ID = TDetail.GetNewID(BU, SFCDB);
                        RDetail.ITEM_ID = RItem.ID;
                        RDetail.SCANTYPE = Items[j]["ScanType"].ToString();
                        RDetail.LOCATION = Items[j]["Location"].ToString();
                        RDetail.SEQ = j+1;
                        RDetail.EDIT_EMP = RItem.EDIT_EMP;
                        RDetail.EDIT_TIME = Now;
                        SFCDB.ExecSQL(RDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                    }
                }
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void UpLoadKPListJuniper(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string Skuno = Data["SkuNo"].ToString();
                string ListName = Data["ListName"].ToString();
                Newtonsoft.Json.Linq.JToken ListData = Data["ListData"];
                string CustVersion = Data["CustVersion"].ToString();
                T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string NewListID = T.GetNewID(this.BU, SFCDB);
                Row_C_KP_LIST R = (Row_C_KP_LIST)T.NewRow();
                DateTime Now = DateTime.Now;

                T_C_KP_List_Item TItem = new T_C_KP_List_Item(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item RItem = (Row_C_KP_List_Item)TItem.NewRow();

                T_C_KP_List_Item_Detail TDetail = new T_C_KP_List_Item_Detail(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item_Detail RDetail = (Row_C_KP_List_Item_Detail)TDetail.NewRow();

                T_C_SKU_MPN Tmpn=new T_C_SKU_MPN(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_SKU_MPN Rmpn = (Row_C_SKU_MPN)Tmpn.NewRow();

                T_C_KP_Rule Trule = new T_C_KP_Rule(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_Rule Rrule = (Row_C_KP_Rule)Trule.NewRow();

                KPListBase oldList = KPListBase.GetKPListByListName(ListName, SFCDB);
                if (oldList != null)
                {
                    //NewListID = oldList.ID;
                    //oldList.ReMoveFromDB(SFCDB);
                    throw new Exception($@"KP Name {ListName} is exists!Pls add version# after ListName!");
                }

                R.ID = NewListID;
                R.SKUNO = Skuno.Trim();
                R.LISTNAME = ListName;
                R.EDIT_EMP = this.LoginUser.EMP_NO;
                R.EDIT_TIME = Now;
                R.FLAG = "1";
                R.CUSTVERSION = CustVersion;
                SFCDB.ORM.Updateable<C_KP_LIST>().SetColumns(t => t.FLAG == "0").Where(t => t.SKUNO == R.SKUNO)
                    .ExecuteCommand();
                SFCDB.ExecSQL(R.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                //Item	PartNO	KPName	Station	QTY	ScanType
                DataTable dt = new DataTable();
                dt.Columns.Add("Item");
                dt.Columns.Add("PartNO");
                dt.Columns.Add("KPName");
                dt.Columns.Add("Station");
                dt.Columns.Add("QTY");
                dt.Columns.Add("ScanType");
                dt.Columns.Add("Location");
                List<DataRow> ListItem = new List<DataRow>();
                for (int i = 0; i < ListData.Count(); i++)
                {
                    DataRow dr = dt.NewRow();
                    dr["Item"] = ListData[i]["Item"].ToString();
                    dr["PartNO"] = ListData[i]["PartNO"].ToString();
                    dr["KPName"] = ListData[i]["KPName"].ToString();
                    dr["Station"] = ListData[i]["Station"].ToString();
                    dr["QTY"] = ListData[i]["QTY"].ToString();
                    dr["ScanType"] = ListData[i]["ScanType"].ToString();
                    dr["Location"] = ListData[i]["Location"].ToString();
                    dt.Rows.Add(dr);
                    ListItem.Add(dr);
                }
                Dictionary<string, string> Item = new Dictionary<string, string>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!Item.ContainsKey(dt.Rows[i]["Item"].ToString()))
                    {
                        Item.Add(dt.Rows[i]["Item"].ToString(), dt.Rows[i]["PartNO"].ToString());
                    }
                }

                string[] itemNo = new string[Item.Keys.Count];
                Item.Keys.CopyTo(itemNo, 0);
                
                for (int i = 0; i < itemNo.Length; i++)
                {
                    List<DataRow> Items = ListItem.FindAll(TT => TT["Item"].ToString() == itemNo[i]);
                    for (int j = 0; j < Items.Count; j++)
                    {
                        //增加检查扫描的KP TYPE为系统上已有的类型
                        var itemDetail = SFCDB.ORM.Queryable<C_KP_List_Item_Detail>().Where(c => c.SCANTYPE == Items[j]["ScanType"].ToString()).ToList();
                        if (itemDetail.Count() < 1)
                        {
                            throw new Exception($@"{Items[j]["ScanType"].ToString()},Set the error, please modify ");
                        }
                        if (j == 0)
                        {
                            RItem.ID = TItem.GetNewID(BU, SFCDB);
                            RItem.LIST_ID = NewListID;
                            RItem.KP_NAME = Items[j]["KPName"].ToString();
                            RItem.KP_PARTNO = Items[j]["PartNO"].ToString();
                            RItem.STATION = Items[j]["Station"].ToString();
                            RItem.QTY = double.Parse(Items[j]["QTY"].ToString());
                            RItem.SEQ = i;
                            RItem.EDIT_EMP = this.LoginUser.EMP_NO;
                            RItem.EDIT_TIME = Now;
                            SFCDB.ExecSQL(RItem.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        }

                        RDetail.ID = TDetail.GetNewID(BU, SFCDB);
                        RDetail.ITEM_ID = RItem.ID;
                        RDetail.SCANTYPE = Items[j]["ScanType"].ToString();
                        RDetail.LOCATION = Items[j]["Location"].ToString();
                        RDetail.SEQ = j + 1;
                        RDetail.EDIT_EMP = RItem.EDIT_EMP;
                        RDetail.EDIT_TIME = Now;
                        SFCDB.ExecSQL(RDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));

                        string StrMpn = "";
                        switch (Items[j]["PartNO"].ToString())
                        {
                            case "CLEI LABEL":
                                StrMpn = Skuno + "-CLEI LABEL";
                                break;
                            case "PID LABEL":
                                StrMpn = Skuno + "-PID LABEL";
                                break;
                            case "REVISION":
                                StrMpn = Skuno + "-REVISION";
                                break;
                            default:
                                StrMpn = Items[j]["PartNO"].ToString();
                                break;
                        }

                        var IsExistMpn = SFCDB.ORM.Queryable<C_SKU_MPN>().Where((c) => c.SKUNO == Skuno && c.PARTNO == Items[j]["PartNO"].ToString()).ToList();
                        if (IsExistMpn.Count == 0)
                        {
                            Rmpn.ID = Tmpn.GetNewID(BU, SFCDB);
                            Rmpn.SKUNO = Skuno;
                            Rmpn.PARTNO = Items[j]["PartNO"].ToString();
                            Rmpn.MPN = StrMpn;
                            Rmpn.MFRCODE = "FOXCONN";
                            Rmpn.EDIT_EMP = this.LoginUser.EMP_NO;
                            Rmpn.EDIT_TIME = Now;
                            SFCDB.ExecSQL(Rmpn.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            SFCDB.CommitTrain();
                        }

                        var IsExistRule = SFCDB.ORM.Queryable<C_KP_Rule>()
                            .Where((c) => c.PARTNO == Items[j]["PartNO"].ToString() && c.MPN== StrMpn && c.SCANTYPE == Items[j]["ScanType"].ToString() )
                            .ToList();
                        if (IsExistRule.Count == 0)
                        {
                            var skudetail = SFCDB.ORM.Queryable<C_SKU>().Where((c) => c.SKUNO == Skuno).Select((c) => c).ToList();
                            var partdetail = SFCDB.ORM.Queryable<C_SKU>().Where((c) => c.SKUNO == Items[j]["PartNO"].ToString()).Select((c) => c).ToList();
                            Rrule.ID = Trule.GetNewID(BU, SFCDB);
                            Rrule.PARTNO = Items[j]["PartNO"].ToString();
                            Rrule.MPN = StrMpn;
                            Rrule.SCANTYPE = Items[j]["ScanType"].ToString();
                            switch (Items[j]["ScanType"].ToString())
                            {
                                case "PN":
                                    if (Items[j]["PartNO"].ToString().Contains("PID LABEL"))
                                    {
                                        Rrule.REGEX = "";
                                    } else 
                                    {
                                        Rrule.REGEX = "^" + Items[j]["PartNO"].ToString() + "$";
                                    }
                                    break;
                                case "YN":
                                    Rrule.REGEX = "^Y$";
                                    break;
                                case "REV":
                                    if (Items[j]["PartNO"].ToString().Contains("REVISION"))
                                    {
                                        Rrule.REGEX = "^" + Skuno + "R" + skudetail[0].VERSION + "$";
                                    }
                                    else if (partdetail.Count > 0)
                                    {
                                        Rrule.REGEX = "^" + Items[j]["PartNO"].ToString() + "R" + partdetail[0].VERSION + "$";
                                    }
                                    else
                                    {
                                        Rrule.REGEX = "";
                                    }
                                    break;
                                default:
                                    Rrule.REGEX = "";
                                    break;
                            }
                            Rrule.EDIT_EMP = this.LoginUser.EMP_NO;
                            Rrule.EDIT_TIME = Now;
                            SFCDB.ExecSQL(Rrule.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                            SFCDB.CommitTrain();
                        }
                    }
                }

                //The unloaded wo automatically replaces the KP list
                var Wobase = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.SKUNO == Skuno).ToList();
                for (int k = 0; k < Wobase.Count; k++)
                {
                    var InputWo = SFCDB.ORM.Queryable<R_WO_BASE>().Where(r => r.WORKORDERNO == Wobase[k].WORKORDERNO && r.INPUT_QTY == 0).ToList();
                    if (InputWo.Count > 0)
                    {
                        var aa = SFCDB.ORM.Updateable<R_WO_BASE>().SetColumns(r => r.KP_LIST_ID == NewListID).Where(r => r.WORKORDERNO == InputWo[0].WORKORDERNO).ExecuteCommand();
                    }
                }
                SFCDB.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void RemoveKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            List<string> ret = new List<string>();
            try
            {
                Newtonsoft.Json.Linq.JToken names = Data["ListNames"];
                for (int i = 0; i < names.Count(); i++)
                {
                    KPListBase list = KPListBase.GetKPListByListName(names[i].ToString(), SFCDB);
                    try
                    {
                        SFCDB.BeginTrain();
                        var listAny = SFCDB.ORM.Queryable<C_KP_LIST, R_WO_BASE>((ckl, rwb) => ckl.ID == rwb.KP_LIST_ID)
                            .Where((ckl, rwb) => ckl.LISTNAME == list.ListName).Select((ckl, rwb) => ckl).Any();
                        if (listAny)
                        {
                            StationReturn.Status = StationReturnStatusValue.Fail;
                            StationReturn.MessageCode = "MSGCODE20200709234829";
                            StationReturn.MessagePara.Add(list.ListName);
                            return;
                        }
                        list.ReMoveFromDB(SFCDB);

                        /* To set valid flag 1 to latest valid P List if exists BY DAN 12-06-2021*/
                        var otherKPListExists = SFCDB.ORM.Queryable<C_KP_LIST>().Where(kp => kp.SKUNO == list.SkuNo).OrderBy(kp => kp.EDIT_TIME, SqlSugar.OrderByType.Desc).First();
                        if(otherKPListExists != null)
                        {
                            otherKPListExists.FLAG = "1";
                            SFCDB.ORM.Updateable(otherKPListExists).ExecuteCommand();
                        }

                        ret.Add(names[i].ToString());
                        SFCDB.CommitTrain();
                    }
                    catch(Exception e)
                    {
                        SFCDB.RollbackTrain();
                    }
                }
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
        public void GetAllKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<MESDataObject.Module.C_KP_LIST> list = KPListBase.getAllData(SFCDB);
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetKPListBySkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string Skuno = Data["SKUNO"].ToString();
                List<KPListBase> list = KPListBase.GetKPListBySkuNo(Skuno,SFCDB);
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
            } catch(Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }

        }

        public void CheckKP()
        {

        }

        List<R_SN_KP> listAllKP = new List<R_SN_KP>();
        public void GetSNAllKeypart(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString();
                R_SN snObj = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObj == null)
                {
                    throw new Exception($@"{sn} Not Exist!");
                }
                if (snObj.SCRAPED_FLAG == "1")
                {
                    throw new Exception($@"{sn} Already Unbond!");
                }
                //if (snObj.REPAIR_FAILED_FLAG=="1")
                //{
                //    throw new Exception($@"{sn} Is Wait To Repair");
                //}
                if (snObj.COMPLETED_FLAG == "1")
                {
                    throw new Exception($@"{sn} Already Completed!");
                }
                if (snObj.COMPLETED_FLAG == "1")
                {
                    throw new Exception($@"{sn} Already Finished!");
                }
                if (snObj.SHIPPED_FLAG == "1")
                {
                    throw new Exception($@"{sn} Already Shipped!");
                }
                if (snObj.NEXT_STATION.ToUpper() == "REWORK")
                {
                    throw new Exception($@"{sn} Is Wait To Rework");
                }
                GetSnKP(SFCDB, sn);
                StationReturn.Data = listAllKP;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {               
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }       
        private void GetSnKP(MESDBHelper.OleExec DB, string sn)
        {
            T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, this.DBTYPE);
            var list = t_r_sn_kp.GetKPListBYSN(sn, 1, DB);
            if (list.Count > 0)
            {
                listAllKP.AddRange(list);
                foreach (var kp in list)
                {
                    if (kp.VALUE != kp.SN)
                    {
                        GetSnKP(DB, kp.VALUE);
                    }
                }
            }           
        }

        public void ReplaceSNKeypart(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            SFCDB.BeginTrain();
            try
            {
                string id = Data["ID"] == null ? "" : Data["ID"].ToString();
                string new_partno = Data["NEW_PARTNO"] == null ? "" : Data["NEW_PARTNO"].ToString();
                string new_value = Data["NEW_VALUE"] == null ? "" : Data["NEW_VALUE"].ToString();
                string new_mpn = Data["NEW_MPN"] == null ? "" : Data["NEW_MPN"].ToString();
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString(); 
                string return_station = Data["RETURN_STATION"] == null ? "" : Data["RETURN_STATION"].ToString(); 
                List<string> listRegex = new List<string>();                
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(SFCDB, this.DBTYPE);
                T_R_SN_LOG t_r_sn_log = new T_R_SN_LOG(SFCDB, this.DBTYPE);
                T_R_SN t_r_sn = new T_R_SN(SFCDB, this.DBTYPE);
                R_SN_KP kpObj = t_r_sn_kp.GetObjByID(SFCDB, id);
                if (kpObj == null)
                {
                    throw new Exception("Get Keypart Fail!");
                }
                if (string.IsNullOrWhiteSpace(new_partno) && string.IsNullOrWhiteSpace(new_value))
                {
                    throw new Exception("Please Input New PARTNO Or New VALUE");
                }
                DateTime sysdate = t_r_sn_kp.GetDBDateTime(SFCDB);
                R_SN kpSN = t_r_sn.LoadSN(kpObj.SN, SFCDB);
                R_SN snObj = new R_SN();
                R_SN newSNObj = new R_SN();
                C_KP_Rule kpRule = new C_KP_Rule();
                R_SN_KP newKpObj = new R_SN_KP();
                Row_R_SN_KP rowNewKp = (Row_R_SN_KP)t_r_sn_kp.NewRow();
                bool bKpRule = false;
                //string kp_sn_new = "";
                //string kp_sn_old = "";
                rowNewKp.ConstructRow(kpObj);

                kpObj.VALID_FLAG = 0;
                kpObj.EDIT_EMP = LoginUser.EMP_NO;
                kpObj.EDIT_TIME = sysdate;
                t_r_sn_kp.UpdateObj(SFCDB, kpObj);

                rowNewKp.ID = t_r_sn_kp.GetNewID(this.BU, SFCDB, this.DBTYPE);               
                if (!string.IsNullOrWhiteSpace(new_partno))
                {
                    if (string.IsNullOrWhiteSpace(new_mpn))
                    {
                        throw new Exception("Please Input New MPN");
                    }
                    var p = SFCDB.ORM.Queryable<C_KP_LIST, C_KP_List_Item>((ck, ckl) => ck.ID == ckl.LIST_ID)
                        .Where((ck, ckl) => ck.SKUNO == kpSN.SKUNO && ckl.KP_PARTNO == new_partno).Select((ck, ckl) => ckl).ToList().FirstOrDefault();
                    if (p == null)
                    {
                        throw new Exception($@"Partno Error!");
                    }
                    rowNewKp.PARTNO = new_partno;
                }
                rowNewKp.MPN = string.IsNullOrWhiteSpace(new_mpn) ? kpObj.MPN : new_mpn;
                if (!string.IsNullOrWhiteSpace(new_value))
                {
                    List<string> noCheckType = new List<string>() { "PN", "DCBarcode" };
                    if (!noCheckType.Contains(kpObj.SCANTYPE) && t_r_sn_kp.CheckLinkByValue(new_value, SFCDB))
                    {
                        throw new Exception($@"{new_value} Has Been Link By Other SN");
                    }

                    new_partno = string.IsNullOrWhiteSpace(new_partno) ? kpObj.PARTNO : new_partno;
                    new_mpn = string.IsNullOrWhiteSpace(new_mpn) ? kpObj.MPN : new_mpn;

                    List<C_KP_List_Item_Detail> listItemDetail = SFCDB.ORM.Queryable<C_KP_LIST, C_KP_List_Item, C_KP_List_Item_Detail>((ckl, ckli, cklid) => ckl.ID == ckli.LIST_ID && ckli.ID == cklid.ITEM_ID)
                        .Where((ckl, ckli, cklid) => ckl.SKUNO == kpSN.SKUNO && ckli.KP_PARTNO == new_partno)
                        .Select((ckl, ckli, cklid) => cklid).ToList();
                    foreach (var d in listItemDetail)
                    {
                        kpRule = SFCDB.ORM.Queryable<C_KP_Rule>().Where(r => r.PARTNO == new_partno && r.MPN == new_mpn && r.SCANTYPE == d.SCANTYPE).ToList().FirstOrDefault();
                        if (kpRule != null && kpRule.REGEX != "")
                        {
                            listRegex.Add(kpRule.REGEX);
                        }
                    }
                    listRegex.Add(rowNewKp.REGEX);
                    foreach (var s in listRegex)
                    {
                        if (Regex.IsMatch(new_value, rowNewKp.REGEX))
                        {
                            bKpRule = true;
                            break;                           
                        }
                    }
                    if (!bKpRule)
                    {
                        throw new Exception($@"New VALUE Is Not Match Rule");
                    }                    
                    //update ship flag
                    snObj = t_r_sn.LoadSN(kpObj.VALUE, SFCDB);
                    if (snObj != null)
                    {                        
                        snObj.SHIPDATE = null;
                        snObj.SHIPPED_FLAG = "0";
                        snObj.EDIT_EMP = LoginUser.EMP_NO;
                        snObj.EDIT_TIME = sysdate;
                        t_r_sn.Update(snObj, SFCDB);
                    }
                    newSNObj = t_r_sn.LoadSN(new_value, SFCDB);                    
                    if (newSNObj != null)
                    {
                        if (newSNObj.SHIPPED_FLAG == "1")
                        {
                            throw new Exception($@"{new_value} Has Been Shipped");
                        }
                        if (newSNObj.SCRAPED_FLAG == "1")
                        {
                            throw new Exception($@"{new_value} Has Been Scraped");
                        }
                        if (newSNObj.REPAIR_FAILED_FLAG == "1")
                        {
                            throw new Exception($@"{new_value} Is In Repair");
                        }                       
                        if (newSNObj.COMPLETED_FLAG != "1")
                        {
                            throw new Exception($@"{new_value} Is Not Complete");
                        }
                        if (newSNObj.NEXT_STATION == "REWORK")
                        {
                            throw new Exception($@"{new_value} Is In Rework");
                        }                        
                        newSNObj.SHIPDATE = sysdate;
                        newSNObj.SHIPPED_FLAG = "1";
                        newSNObj.EDIT_EMP = LoginUser.EMP_NO;
                        newSNObj.EDIT_TIME = sysdate;
                        t_r_sn.Update(newSNObj, SFCDB);
                    }
                    rowNewKp.VALUE = new_value;
                }
                newKpObj = rowNewKp.GetDataObject();
                t_r_sn_kp.Save(SFCDB, newKpObj);

                R_SN_KP parentKp = null;
                R_SN parentSN = null;
                if (BU.Equals("VNDCN"))
                {
                    bool isWsn = SFCDB.ORM.Queryable<DcnSfcModel.WWN_Datasharing>().Where(w => w.WSN == kpObj.VALUE && w.SKU == kpObj.PARTNO).Any();
                    if (isWsn)
                    {
                        DcnSfcModel.WWN_Datasharing wsn = SFCDB.ORM.Queryable<DcnSfcModel.WWN_Datasharing>()
                        .Where(w => w.WSN == newKpObj.VALUE && w.SKU == newKpObj.PARTNO).ToList().FirstOrDefault();
                        if (wsn == null)
                        {
                            throw new Exception($@"{newKpObj.VALUE} Not In WWN_Datasharing Or PARTNO Error!");
                        }
                        if (wsn.VSSN != "N/A")
                        {
                            throw new Exception($@"{newKpObj.VALUE} Link Other VSSN In WWN_Datasharing");
                        }
                        wsn.VSSN = newKpObj.SN;
                        wsn.VSKU = kpSN.SKUNO;
                        parentKp = SFCDB.ORM.Queryable<R_SN_KP>().Where(r => r.VALUE == newKpObj.SN && r.VALID_FLAG == 1).ToList().FirstOrDefault();
                        if (parentKp != null)
                        {
                            parentSN = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == parentKp.SN && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                            wsn.CSSN = parentSN.SN;
                            wsn.VSKU = parentSN.SKUNO;
                        }
                        wsn.lasteditby = LoginUser.EMP_NO;
                        wsn.lasteditdt = sysdate;
                        SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>(wsn).ExecuteCommand();

                        SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing {
                                VSSN = "N/A" , VSKU = "N/A" , CSSN = "N/A" , CSKU = "N/A" , lasteditby = LoginUser.EMP_NO , lasteditdt = sysdate})
                            .Where(w => w.WSN == kpObj.VALUE && w.SKU == kpObj.PARTNO).ExecuteCommand();
                    }

                    bool isVssn = SFCDB.ORM.Queryable<DcnSfcModel.WWN_Datasharing>().Where(w => w.VSSN == kpObj.VALUE && w.VSKU == kpObj.PARTNO).Any();
                    if (isVssn)
                    {
                        DcnSfcModel.WWN_Datasharing vssn = SFCDB.ORM.Queryable<DcnSfcModel.WWN_Datasharing>()
                        .Where(w => w.VSSN == newKpObj.VALUE && w.VSSN == newKpObj.PARTNO).ToList().FirstOrDefault();
                        if (vssn == null)
                        {
                            throw new Exception($@"{newKpObj.VALUE} Not In WWN_Datasharing Or PARTNO Error!");
                        }
                        if (vssn.CSSN != "N/A")
                        {
                            throw new Exception($@"{newKpObj.VALUE} Link Other CSSN In WWN_Datasharing");
                        }
                        vssn.CSSN = newKpObj.SN;
                        vssn.CSKU = kpSN.SKUNO;
                        vssn.lasteditby = LoginUser.EMP_NO;
                        vssn.lasteditdt = sysdate;
                        SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>(vssn).ExecuteCommand();

                        SFCDB.ORM.Updateable<DcnSfcModel.WWN_Datasharing>()
                            .UpdateColumns(w => new DcnSfcModel.WWN_Datasharing {
                                CSSN = "N/A" , CSKU = "N/A" , lasteditby = LoginUser.EMP_NO , lasteditdt = sysdate } )
                            .Where(w => w.VSSN == kpObj.VALUE && w.VSKU == kpObj.PARTNO).ExecuteCommand();
                    }

                    bool isLink = SFCDB.ORM.Queryable<R_SN_LINK>().Where(r => r.SN == kpObj.SN && r.CSN == kpObj.VALUE && r.VALIDFLAG == "1").Any();
                    if (isLink)
                    {
                        R_SN_LINK newLink = new R_SN_LINK();
                        T_R_SN_LINK T_RSL = new T_R_SN_LINK(SFCDB, DB_TYPE_ENUM.Oracle);
                        newLink.ID = T_RSL.GetNewID(BU, SFCDB);
                        newLink.LINKTYPE = "ReplaceKeypart";
                        newLink.MODEL = kpSN.SKUNO;
                        newLink.SN = kpSN.SN;
                        newLink.CSN = newKpObj.VALUE;
                        newLink.VALIDFLAG = "1";
                        newLink.CREATETIME = sysdate;
                        newLink.CREATEBY = LoginUser.EMP_NO;
                        newLink.EDITTIME = sysdate;
                        newLink.EDITBY = LoginUser.EMP_NO;
                        SFCDB.ORM.Insertable<R_SN_LINK>(newLink).ExecuteCommand();

                        SFCDB.ORM.Updateable<R_SN_LINK>(r => r.VALIDFLAG == "0" && r.EDITBY == LoginUser.EMP_NO && r.EDITTIME == sysdate).ExecuteCommand();
                    }
                }

                //SN Return Station
                R_SN parentSNObj = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (parentSNObj == null)
                {
                    throw new Exception($@"{sn} Not Exist!");
                }
                if (!string.IsNullOrWhiteSpace(return_station))
                {
                    C_ROUTE_DETAIL route = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == parentSNObj.ROUTE_ID
                      && r.STATION_NAME == return_station).ToList().FirstOrDefault();
                    if (route == null)
                    {
                        throw new Exception($@"Return Station({return_station}) Not In Route Of SN({sn}) !");
                    }
                    C_ROUTE_DETAIL currentStation = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == parentSNObj.ROUTE_ID &&
                     r.SEQ_NO < route.SEQ_NO).OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
                    if (currentStation == null)
                    {
                        throw new Exception($@"The Last Station Of Return Station({return_station}) Is Null !");
                    }
                    parentSNObj.CURRENT_STATION = currentStation.STATION_NAME;
                    parentSNObj.NEXT_STATION = route.STATION_NAME;
                    parentSNObj.EDIT_EMP = LoginUser.EMP_NO;
                    parentSNObj.EDIT_TIME = sysdate;
                    t_r_sn.Update(parentSNObj, SFCDB);
                    //打回去則加一筆過站記錄
                    t_r_sn.RecordPassStationDetail(parentSNObj, "Replace", "ReplaceKP", "ReplaceKP", BU, SFCDB, "0");
                }

                R_SN_LOG snLog = new R_SN_LOG();
                snLog.ID = t_r_sn_log.GetNewID(this.BU, SFCDB);
                snLog.SN = kpObj.SN;
                snLog.SNID = kpObj.R_SN_ID;
                snLog.LOGTYPE = "REPLACE_KP";
                snLog.DATA1 = kpObj.VALUE;
                snLog.DATA2 = newKpObj.VALUE;
                snLog.DATA3 = kpObj.PARTNO;
                snLog.DATA4 = newKpObj.PARTNO;
                snLog.DATA5 = kpObj.MPN;
                snLog.DATA6 = newKpObj.MPN;
                snLog.FLAG = "1";
                snLog.CREATETIME = sysdate;
                snLog.CREATEBY = LoginUser.EMP_NO;
                t_r_sn_log.Save(SFCDB, snLog);
               
                StationReturn.Message = "OK!";
                StationReturn.Data = "";
                StationReturn.Status = StationReturnStatusValue.Pass;
                SFCDB.CommitTrain();
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }

        public void GetReplaceReturnStation(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sn = Data["SN"] == null ? "" : Data["SN"].ToString();
                string returnStation = ""; 
                R_SN_KP kpRow = Data["ROW_KP"] == null ? null : Data["ROW_KP"].ToObject<R_SN_KP>();
                R_SN snObj = SFCDB.ORM.Queryable<R_SN>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList().FirstOrDefault();
                if (snObj == null)
                {
                    throw new Exception($@"{sn} Not Exist!");
                }
                C_ROUTE_DETAIL route = SFCDB.ORM.Queryable<C_ROUTE_DETAIL>().Where(r => r.ROUTE_ID == snObj.ROUTE_ID
                  && SqlSugar.SqlFunc.Subqueryable<C_TEMES_STATION_MAPPING>().Where(t => r.STATION_NAME == t.MES_STATION).Any())
                .OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Asc).ToList().FirstOrDefault();

                List<string> listReturnStation = SFCDB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "ReplaceKeypartNotReturnStation"
                && r.CONTROLFLAG == "Y" && r.FUNCTIONTYPE == "NOSYSTEM" && r.CATEGORY == "STATION").Select(r => SqlSugar.SqlFunc.Trim(SqlSugar.SqlFunc.ToUpper(r.VALUE)))
                .ToList();
                returnStation = route == null ? "" : route.STATION_NAME;

                if (kpRow != null && listReturnStation.Contains(kpRow.STATION.ToUpper()))
                {
                    //CARTON工站綁定的keypart不用打回到第一個測試工站，因為該工站綁定的都是一些電源，附件包之類的東西
                    returnStation = "";
                }
                
                StationReturn.Data = returnStation;
                StationReturn.Message = "OK!";                
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ex)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ex.Message);
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }
    }
}
