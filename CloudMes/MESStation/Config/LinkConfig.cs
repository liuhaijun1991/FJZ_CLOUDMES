using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using Newtonsoft.Json.Linq;
using System.Data;

namespace MESStation.Config
{
    class LinkConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo addLink = new APIInfo()
        {
            FunctionName = "AddLink",
            Description = "添加KeyPart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName= "KEYPART_ID",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "STATION_NAME",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SKUNO",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SKUNO_VER",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "KEYPARTLIST",InputType ="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo deleteLink = new APIInfo()
        {
            FunctionName = "DeleteKeyPart",
            Description = "添加KeyPart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName= "ID",InputType ="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo updateLink = new APIInfo()
        {
            FunctionName = "UpdateKeyPart",
            Description = "更新KeyPart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName= "ID",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "KEYPART_ID",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SEQ_NO",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "PART_NO",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "PART_NO_VER",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "QTY",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "STATION_NAME",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "CATEGORY",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "CATEGORY_NAME",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SKUNO",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName= "SKUNO_VER",InputType ="string",DefaultValue=""}

            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo sLink = new APIInfo()
        {
            FunctionName = "SeKeyPart",
            Description = "查询KeyPart",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SKUNO",InputType ="string",DefaultValue=""},
                new APIInputInfo() { InputName="STATION_NAME",InputType ="string",DefaultValue=""}
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo FGetCategoryLits = new APIInfo()
        {
            FunctionName = "GetCategoryLits",
            Description = "獲取KeyPart類型",
            Parameters = new List<APIInputInfo>()
            { },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo FGetRuleList = new APIInfo()
        {
            FunctionName= "GetRuleList",
            Description="獲取所有的 Keypart 規則",
            Parameters=new List<APIInputInfo>() { },
            Permissions=new List<MESPermission>() { }
        };

        private APIInfo FAddKeypart = new APIInfo()
        {
            FunctionName="AddKeypart",
            Description="添加一條 Keypart 配置",
            Parameters=new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="KEYPART_ID", InputType="string" },
                 new APIInputInfo() { InputName="STATION_NAME", InputType="string" },
                 new APIInputInfo() { InputName="SKUNO", InputType="string" },
                 new APIInputInfo() { InputName="SKUNO_VER", InputType="string" },
                 new APIInputInfo() { InputName="KEYPARTLIST", InputType="string" }
            },
            Permissions=new List<MESPermission>() { }
        };

        private APIInfo FUpdateKeypart = new APIInfo()
        {
            FunctionName= "UpdateKeypart",
            Description="更新一條 Keypart 配置",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="KEYPARTLIST", InputType="string" }
            },
            Permissions=new List<MESPermission>() { }
        };

        private APIInfo FGetKeypartSettings = new APIInfo()
        {
            FunctionName= "GetKeypartSettings",
            Description="獲取所有 Keypart 設定",
            Parameters=new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="SKUNO", InputType="string" },
                new APIInputInfo() { InputName="STATION_NAME",InputType="string" }
            }
        };


        public LinkConfig()
        {
            this.Apis.Add(addLink.FunctionName, addLink);
            this.Apis.Add(deleteLink.FunctionName, deleteLink);
            this.Apis.Add(updateLink.FunctionName, updateLink);
            this.Apis.Add(sLink.FunctionName, sLink);
            this.Apis.Add(FGetCategoryLits.FunctionName, FGetCategoryLits);
            this.Apis.Add(FGetRuleList.FunctionName, FGetRuleList);
            this.Apis.Add(FAddKeypart.FunctionName, FAddKeypart);
            this.Apis.Add(FUpdateKeypart.FunctionName, FUpdateKeypart);
            this.Apis.Add(FGetKeypartSettings.FunctionName, FGetKeypartSettings);
        }

        public void AddLink(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            T_C_KEYPART ketpart;
            Row_C_KEYPART newketpart;
            string KeyPartId = Data["KEYPART_ID"].ToString().Trim();
            string StationName = Data["STATION_NAME"].ToString().Trim();
            string Skuno = Data["SKUNO"].ToString().Trim();
            string SkunoVer = Data["SKUNO_VER"].ToString().Trim();
            //string SEQ = Data["SEQ"].ToString().Trim();
            string SQL = "";
            sfcdb.BeginTrain();
            try
            {
                if (KeyPartId.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("KEYPART_ID");
                    return;
                }

                
                if (StationName.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("STAION_NAME");
                    return;
                }
               
                if (Skuno.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SKUNO");
                    return;
                }
                if (SkunoVer.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SKUNO_VER");
                    return;
                }

                KEYPARTLIST list = new KEYPARTLIST();

                T_C_KP_LIST TCKL = new T_C_KP_LIST(sfcdb, DBTYPE);
                string tempMsg = "";                
                string sql = "";
                DataTable dt = new DataTable();
                foreach (Newtonsoft.Json.Linq.JToken s in Data["KEYPARTLIST"])
                {
                    int i = 10;
                    list = (KEYPARTLIST)JsonConvert.Deserialize(s.ToString(), typeof(KEYPARTLIST));
                    //SEQ是10則檢查對應的PART_NO是否有配置c_kp_list
                    if (list.SEQ == 10 && TCKL.GetListIDBySkuno(list.PART_NO, sfcdb).Count == 0)
                    {
                        tempMsg = tempMsg + ",This " + list.PART_NO + " KP List Not Setting!";
                    }
                    sql = $@"select * from C_KEYPART where skuno='{Skuno}' and SEQ_NO={list.SEQ}";
                    dt = sfcdb.ExecSelect(sql, null).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        throw new Exception($@"{Skuno} {list.SEQ} Already Exist!");
                    }
                    ketpart = new T_C_KEYPART(sfcdb, DBTYPE);
                    newketpart = (Row_C_KEYPART)ketpart.NewRow();
                    newketpart.ID = ketpart.GetNewID(BU, sfcdb);
                    newketpart.KEYPART_ID = KeyPartId;
                    newketpart.SEQ_NO = list.SEQ;
                    newketpart.PART_NO = list.PART_NO;
                    newketpart.PART_NO_VER = list.PART_NO_VER;
                    newketpart.QTY = Convert.ToDouble(list.QTY);
                    newketpart.STATION_NAME = StationName;
                    newketpart.CATEGORY = list.CATEGORY;
                    newketpart.CATEGORY_NAME = list.CATEGORY_NAME;
                    newketpart.SKUNO = Skuno;
                    newketpart.SKUNO_VER = SkunoVer;
                    newketpart.EDIT_EMP = LoginUser.EMP_NO;
                    newketpart.EDIT_TIME = GetDBDateTime();
                    i += 10;
                    SQL += newketpart.GetInsertString(DB_TYPE_ENUM.Oracle) + ";\n";
                }

                sfcdb.ExecSQL("Begin\n" + SQL + "End;");
                sfcdb.CommitTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "添加成功！！" + tempMsg;
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = "添加失敗！！" + e.Message;
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                //throw e;

            }
        }

        public void AddKeypart(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            T_C_KEYPART TCK=null;
            T_C_KEYPART_RULE TCKR = null;
            C_KEYPART NewKeypart;
            string KeyPartId = Data["KEYPART_ID"].ToString().Trim();
            string StationName = Data["STATION_NAME"].ToString().Trim();
            string Skuno = Data["SKUNO"].ToString().Trim();
            string SkunoVer = Data["SKUNO_VER"].ToString().Trim();
            //string SEQ = Data["SEQ"].ToString().Trim();
            //string SQL = "";
            sfcdb.BeginTrain();
            try
            {
                if (KeyPartId.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("KEYPART_ID");
                    return;
                }


                if (StationName.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("STAION_NAME");
                    return;
                }

                if (Skuno.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SKUNO");
                    return;
                }
                if (SkunoVer.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SKUNO_VER");
                    return;
                }

                CKeyPart Keypart = new CKeyPart();
                TCK = new T_C_KEYPART(sfcdb, DBTYPE);
                TCKR = new T_C_KEYPART_RULE(sfcdb, DBTYPE);
                foreach (Newtonsoft.Json.Linq.JToken s in Data["KEYPARTLIST"])
                {
                    Keypart = (CKeyPart)JsonConvert.Deserialize(s.ToString(), typeof(CKeyPart));
                    NewKeypart = new C_KEYPART();
                    NewKeypart.ID = TCK.GetNewID(BU, sfcdb);
                    NewKeypart.KEYPART_ID = KeyPartId;
                    NewKeypart.SEQ_NO = Keypart.SEQ_NO;
                    NewKeypart.PART_NO = Keypart.PART_NO;
                    NewKeypart.PART_NO_VER = Keypart.PART_NO_VER;
                    NewKeypart.QTY = Convert.ToDouble(Keypart.QTY);
                    NewKeypart.STATION_NAME = StationName;
                    NewKeypart.CATEGORY = Keypart.CATEGORY;
                    NewKeypart.CATEGORY_NAME = Keypart.CATEGORY_NAME;
                    NewKeypart.SKUNO = Skuno;
                    NewKeypart.SKUNO_VER = SkunoVer;
                    NewKeypart.EDIT_EMP = LoginUser.EMP_NO;
                    NewKeypart.EDIT_TIME = GetDBDateTime();
                    NewKeypart.UNIQUE_FLAG = Keypart.UNIQUE_FLAG;
                    NewKeypart.KEYPART_RULE_ID = TCKR.GetRuleByName(Keypart.KEYPART_RULE, sfcdb).ID;
                    TCK.Add(NewKeypart, sfcdb);
                }
                sfcdb.CommitTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "添加成功！！";
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "添加失敗！！";
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;

            }
        }

        public void UpdateKeypart(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
            C_KEYPART NewKeypart = new C_KEYPART();
            T_C_KEYPART_RULE TCKR = null;
            T_C_KEYPART TCK = null;
            if (Data["KEYPARTLIST"] != null && Data["KEYPARTLIST"].Count() > 0)
            {
                try
                {
                    sfcdb = this.DBPools["SFCDB"].Borrow();
                    TCKR = new T_C_KEYPART_RULE(sfcdb, DBTYPE);
                    TCK = new T_C_KEYPART(sfcdb, DBTYPE);
                    CKeyPart Keypart = (CKeyPart)JsonConvert.Deserialize(Data["KEYPARTLIST"][0].ToString(), typeof(CKeyPart));
                    NewKeypart.ID = Keypart.ID;
                    NewKeypart.KEYPART_ID = Keypart.KEYPART_ID;
                    NewKeypart.SEQ_NO = Keypart.SEQ_NO;
                    NewKeypart.PART_NO = Keypart.PART_NO;
                    NewKeypart.PART_NO_VER = Keypart.PART_NO_VER;
                    NewKeypart.QTY = Convert.ToDouble(Keypart.QTY);
                    NewKeypart.STATION_NAME = Keypart.STATION_NAME;
                    NewKeypart.CATEGORY = Keypart.CATEGORY;
                    NewKeypart.CATEGORY_NAME = Keypart.CATEGORY_NAME;
                    NewKeypart.SKUNO = Keypart.SKUNO;
                    NewKeypart.SKUNO_VER = Keypart.SKUNO_VER;
                    NewKeypart.EDIT_EMP = LoginUser.EMP_NO;
                    NewKeypart.EDIT_TIME = GetDBDateTime();
                    NewKeypart.UNIQUE_FLAG = Keypart.UNIQUE_FLAG;
                    NewKeypart.KEYPART_RULE_ID = TCKR.GetRuleByName(Keypart.KEYPART_RULE, sfcdb).ID;
                    sfcdb.BeginTrain();
                    TCK.Modify(NewKeypart, sfcdb);
                    sfcdb.CommitTrain();
                    this.DBPools["SFCDB"].Return(sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "更新成功！！";
                }
                catch (Exception e)
                {
                    sfcdb.RollbackTrain();
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "更新失敗！！";
                    if (sfcdb != null)
                    {
                        this.DBPools["SFCDB"].Return(sfcdb);
                    }
                    throw e;
                }
            }
        }
        public void DeleteKeyPart(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_KEYPART ketpart;
            Row_C_KEYPART newketpart;
            string[] ids = Data["ID"].ToString().Trim().Split(',');
            string SQL = "";
            sfcdb = this.DBPools["SFCDB"].Borrow();
            sfcdb.BeginTrain();
            try
            {
                
                foreach (string id in ids)
                {
                    ketpart = new T_C_KEYPART(sfcdb, DBTYPE);
                    newketpart = (Row_C_KEYPART)ketpart.GetObjByID(id, sfcdb);
                    SQL += newketpart.GetDeleteString(DB_TYPE_ENUM.Oracle) + ";\n";
                }
                sfcdb.ExecSQL("Begin\n" + SQL + "End;");
                sfcdb.CommitTrain();
                this.DBPools["SFCDB"].Return(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "刪除成功！！"; 
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                sfcdb.RollbackTrain();
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "刪除失敗！！";
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }

        public void UpdateKeyPart(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_KEYPART ketpart;
            Row_C_KEYPART upketpart;
            string Id = Data["ID"].ToString().Trim();
            string KeyPartId = Data["KEYPART_ID"].ToString().Trim();
            string SeqNo = Data["SEQ_NO"].ToString().Trim();
            string PartNo = Data["PART_NO"].ToString().Trim();
            string PartNoVer = Data["PART_NO_VER"].ToString().Trim();
            string Qty = Data["QTY"].ToString().Trim();
            string StationName = Data["STATION_NAME"].ToString().Trim();
            string Category = Data["CATEGORY"].ToString().Trim();
            string CategoryName = Data["CATEGORY_NAME"].ToString().Trim();
            string Skuno = Data["SKUNO"].ToString().Trim();
            string SkunoVer = Data["SKUNO_VER"].ToString().Trim();
            try
            {
                if (Id.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("ID");
                    return;
                }
                if (KeyPartId.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("KEYPART_ID");
                    return;
                }

                if (SeqNo.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SEQ_NO");
                    return;
                }
                if (PartNo.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("PART_NO");
                    return;
                }
                if (PartNoVer.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("PART_NO_VER");
                    return;
                }
                if (Qty.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("Qty");
                    return;
                }
                if (StationName.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("StationName");
                    return;
                }
                if (Category.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("Category");
                    return;
                }
                if (CategoryName.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("CategoryName");
                    return;
                }
                if (Skuno.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("Skuno");
                    return;
                }
                if (SkunoVer.Length <= 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000006";
                    StationReturn.MessagePara.Add("SkunoVer");
                    return;
                }
                double? Seqno = Convert.ToDouble(SeqNo);
                double? qty = Convert.ToDouble(Qty);
                sfcdb = this.DBPools["SFCDB"].Borrow();
                ketpart = new T_C_KEYPART(sfcdb, DBTYPE);
                upketpart = (Row_C_KEYPART)ketpart.GetObjByID(Id, sfcdb);
                if (upketpart.KEYPART_ID != null || upketpart.KEYPART_ID != "")
                {
                    upketpart.KEYPART_ID = KeyPartId;
                    upketpart.SEQ_NO = Seqno;
                    upketpart.PART_NO = PartNo;
                    upketpart.PART_NO_VER = PartNoVer;
                    upketpart.QTY = qty;
                    upketpart.STATION_NAME = StationName;
                    upketpart.CATEGORY = Category;
                    upketpart.CATEGORY_NAME = CategoryName;
                    upketpart.SKUNO = Skuno;
                    upketpart.SKUNO_VER = SkunoVer;
                    upketpart.EDIT_EMP = LoginUser.EMP_NO;
                    upketpart.EDIT_TIME = GetDBDateTime();
                    int result = sfcdb.ExecuteNonQuery(upketpart.GetUpdateString(DB_TYPE_ENUM.Oracle), System.Data.CommandType.Text);
                    if (result > 0)
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.Message = "MES00000001";
                    }
                    else
                    {
                        StationReturn.Status = StationReturnStatusValue.Fail;
                        StationReturn.MessageCode = "MES00000021";
                    }
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }

        public void SeKeyPart(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_KEYPART ketpart;
            string skuno = Data["SKUNO"].ToString().Trim();
            string station = Data["STATION_NAME"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                ketpart = new T_C_KEYPART(sfcdb, DBTYPE);

                List<C_KEYPART> list = ketpart.GetKeypartList(sfcdb,skuno,station);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "獲取成功！！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "沒有查詢到任何數據！！";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }

        public void GetCategoryLits(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_KEYPART ketpart;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                ketpart = new T_C_KEYPART(sfcdb, DBTYPE);

                List<C_CATEGORY> list = ketpart.GETCATEGORY(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "獲取成功！！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "獲取失敗！！";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }

        public void GetRuleList(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_KEYPART_RULE TCKR = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCKR = new T_C_KEYPART_RULE(sfcdb, DBTYPE);

                List<C_KEYPART_RULE> list = TCKR.GetAllRule(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "獲取成功！！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "獲取失敗！！";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }
              
        }

        public void GetKeypartSettings(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_KEYPART TCK = null;
            string Skuno = string.Empty;
            string Station = string.Empty;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                TCK = new T_C_KEYPART(sfcdb, DBTYPE);
                Skuno = Data["SKUNO"].ToString();
                Station = Data["STATION_NAME"].ToString();
                List<CKeyPart> Keyparts = TCK.GetAllKeyparts(Skuno, Station, sfcdb);
                if (Keyparts.Count > 0)
                {
                    StationReturn.Data = Keyparts;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "獲取成功！！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "獲取失敗！！";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }
        }

        public void CheckSkuno(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_SKU sku;
            string skuno = Data["SKUNO"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                sku = new T_C_SKU(sfcdb, DBTYPE);
                if (sku.CheckSku(skuno, sfcdb))
                {
                    List<string> list = sku.GetStationBySku(sfcdb,skuno);
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "獲取成功！！";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "獲取失敗！！";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                throw e;
            }

        }

    }
}
