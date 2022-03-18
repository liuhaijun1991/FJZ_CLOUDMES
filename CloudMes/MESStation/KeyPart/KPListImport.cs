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

namespace MESStation.KeyPart
{
    public class KPListImport : MesAPIBase
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
                new APIInputInfo() {InputName = "KPITEM", InputType = "STRING", DefaultValue = ""}
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

        public KPListImport()
        {
            this.Apis.Add(_GetSNStationKPList.FunctionName, _GetSNStationKPList);
            this.Apis.Add(_GetKPListBySkuno.FunctionName, _GetKPListBySkuno);
            this.Apis.Add(_GetAllKPList.FunctionName, _GetAllKPList);
            this.Apis.Add(_UpLoadKPList.FunctionName, _UpLoadKPList);
            this.Apis.Add(_RemoveKPList.FunctionName, _RemoveKPList);
            this.Apis.Add(_CheckKPListName.FunctionName, _CheckKPListName);
            this.Apis.Add(_GetKPListByListName.FunctionName, _GetKPListByListName);
            this.Apis.Add(_ScanKPItem.FunctionName, _ScanKPItem);
        }

        public void GetKPListByListName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            string ListName = Data["ListName"].ToString();
            try
            {
                List<KPListShowData> ret = KPListShowData.GetKpListByListName(ListName, SFCDB);
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
            }
            this.DBPools["SFCDB"].Return(SFCDB);
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
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void GetSNStationKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string strSN = Data["SN"].ToString();
                string strSTATION = Data["STATION"].ToString();
                string strWO = null;
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

                SN_KP ret = new SN_KP(snkp, SN.WorkorderNo, SN.SkuNo, SFCDB,this.BU);


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
            this.DBPools["SFCDB"].Return(SFCDB);
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
                T_C_KP_LIST T = new T_C_KP_LIST(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string NewListID = T.GetNewID(this.BU, SFCDB);
                Row_C_KP_LIST R = (Row_C_KP_LIST)T.NewRow();
                DateTime Now = DateTime.Now;

                T_C_KP_List_Item TItem = new T_C_KP_List_Item(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item RItem = (Row_C_KP_List_Item)TItem.NewRow();

                T_C_KP_List_Item_Detail TDetail = new T_C_KP_List_Item_Detail(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_List_Item_Detail RDetail = (Row_C_KP_List_Item_Detail)TDetail.NewRow();

                T_C_KP_Rule TRule = new T_C_KP_Rule(SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                Row_C_KP_Rule RRule = (Row_C_KP_Rule)TRule.NewRow();

                KPListBase oldList = KPListBase.GetKPListByListName(ListName, SFCDB);
                if (oldList != null)
                {
                    oldList.ReMoveFromDB(SFCDB);
                }

                R.ID = NewListID;
                R.SKUNO = Skuno.Trim();
                R.LISTNAME = ListName;
                R.EDIT_EMP = this.LoginUser.EMP_NO;
                R.EDIT_TIME = Now;

                SFCDB.ExecSQL(R.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                //Item	PartNO	KPName	Station	QTY	ScanType
                DataTable dt = new DataTable();
                dt.Columns.Add("Item");
                dt.Columns.Add("PartNO");
                dt.Columns.Add("KPName");
                dt.Columns.Add("Station");
                dt.Columns.Add("QTY");
                dt.Columns.Add("ScanType");
                dt.Columns.Add("MPN");
                dt.Columns.Add("Rule");
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
                    dr["MPN"] = ListData[i]["MPN"].ToString();
                    dr["Rule"] = ListData[i]["Rule"].ToString();
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
                        RDetail.SEQ = j + 1;
                        RDetail.EDIT_EMP = RItem.EDIT_EMP;
                        RDetail.EDIT_TIME = Now;
                        SFCDB.ExecSQL(RDetail.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));

                        if (Items[j]["MPN"].ToString() == "")
                        {
                            Items[j]["MPN"] = Items[j]["PartNO"].ToString();
                        }

                        C_KP_Rule TmpRule = TRule.GetKPRule(SFCDB, Items[j]["PartNO"].ToString(), Items[j]["MPN"].ToString(), Items[j]["ScanType"].ToString());
                        if (TmpRule != null)
                        {
                            RRule.ID = TmpRule.ID;
                            RRule.PARTNO = TmpRule.PARTNO;
                            RRule.MPN = TmpRule.MPN;
                            RRule.SCANTYPE = TmpRule.MPN;
                            RRule.REGEX = Items[j]["Rule"].ToString();
                            RRule.EDIT_EMP = this.LoginUser.EMP_NO;
                            RRule.EDIT_TIME = Now;
                            SFCDB.ExecSQL(RRule.GetUpdateString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        }
                        else
                        {
                            RRule.ID = TRule.GetNewID(BU,SFCDB);
                            RRule.PARTNO = Items[j]["PartNO"].ToString();
                            RRule.MPN = Items[j]["MPN"].ToString();
                            RRule.SCANTYPE = Items[j]["ScanType"].ToString();
                            RRule.REGEX = Items[j]["Rule"].ToString();
                            RRule.EDIT_EMP = this.LoginUser.EMP_NO;
                            RRule.EDIT_TIME = Now;
                            SFCDB.ExecSQL(RRule.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        }

                        T_C_SKU_MPN Tskumpn = new T_C_SKU_MPN(SFCDB,MESDataObject.DB_TYPE_ENUM.Oracle);
                        List<C_SKU_MPN> TmpSkuMpn = Tskumpn.GetMpnBySkuAndPartno(SFCDB, Skuno, Items[j]["PartNO"].ToString()).Where(p => p.MPN == Items[j]["MPN"].ToString()).ToList();
                        if (TmpSkuMpn == null||TmpSkuMpn.Count==0)
                        {
                            Row_C_SKU_MPN Rskumpn = (Row_C_SKU_MPN)Tskumpn.NewRow();
                            Rskumpn.ID = Tskumpn.GetNewID(BU,SFCDB);
                            Rskumpn.SKUNO = Skuno;
                            Rskumpn.PARTNO = Items[j]["PartNO"].ToString();
                            Rskumpn.MPN = Items[j]["MPN"].ToString();
                            Rskumpn.EDIT_EMP = this.LoginUser.EMP_NO;
                            Rskumpn.EDIT_TIME = Now;
                            SFCDB.ExecSQL(Rskumpn.GetInsertString(MESDataObject.DB_TYPE_ENUM.Oracle));
                        }
                    }
                }

                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                SFCDB.RollbackTrain();
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            SFCDB.CommitTrain();
            this.DBPools["SFCDB"].Return(SFCDB);
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
                        list.ReMoveFromDB(SFCDB);
                        ret.Add(names[i].ToString());
                        SFCDB.CommitTrain();
                    }
                    catch
                    {
                        SFCDB.RollbackTrain();
                    }
                }
                StationReturn.Data = ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            this.DBPools["SFCDB"].Return(SFCDB);
        }
        public void GetAllKPList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            MESDBHelper.OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                List<MESDataObject.Module.C_KP_LIST > list = KPListBase.getAllData(SFCDB);
                StationReturn.Data = list;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            this.DBPools["SFCDB"].Return(SFCDB);
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
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }
            this.DBPools["SFCDB"].Return(SFCDB);
            
        }

        public void CheckKP()
        {

        }



    }
}
