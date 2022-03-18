using MESDataObject.Module;
using MESDBHelper;
using MESStation.BaseClass;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class BUExConfig : MesAPIBase
    {

        protected APIInfo GETALLBUEX = new APIInfo()  
        {
            FunctionName = "GetAllBuEx",
            Description = "GetAllBuEx",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo GETBUEXBYNAME = new APIInfo()  
        {
            FunctionName = "GetBuExByName",
            Description = "GetBuExByName",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "BUNAME", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo DELETEBUEXBYIDSEQNO = new APIInfo()  
        {
            FunctionName = "DeleteBuExByIDSEQNO",
            Description = "DeleteBuExByIDSEQNO",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ID", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "SEQ_NO", InputType = "double", DefaultValue = "" }  },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo CHECKIDSEQNOISEXIST = new APIInfo()  
        {
            FunctionName = "CheckIdSeqNoIsExist",
            Description = "CheckIdSeqNoIsExist",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "ID", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "SEQ_NO", InputType = "double", DefaultValue = "" }  },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo CHECKNAMEVALUEISEXIST = new APIInfo()  
        {
            FunctionName = "CheckNameValueIsExist",
            Description = "CheckNameValueIsExist",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "NAME", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "VALUE", InputType = "string", DefaultValue = "" }  },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo CHECKIDSEQNONAMEVALUEISEXIST = new APIInfo()  
        {
            FunctionName = "CheckIdSeqNoNameValueIsExist",
            Description = "CheckIdSeqNoNameValueIsExist",
            Parameters = new List<APIInputInfo>() {
                                                    new APIInputInfo() { InputName = "ID", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "SEQ_NO", InputType = "double", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "NAME", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "VALUE", InputType = "string", DefaultValue = "" }  },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo UPDATE = new APIInfo()  
        {
            FunctionName = "Update",
            Description = "Update",
           Parameters = new List<APIInputInfo>() {
                                                    new APIInputInfo() { InputName = "old_data_obj", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "new_data_obj", InputType = "string", DefaultValue = "" }
                                                    },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo INSERT = new APIInfo()  
        {
            FunctionName = "Insert",
            Description = "Insert",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "NAME", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "VALUE", InputType = "string", DefaultValue = "" }  },
            Permissions = new List<MESPermission>() { }
        };
        public BUExConfig()
        {
            this.Apis.Add(GETALLBUEX.FunctionName, GETALLBUEX); 
            this.Apis.Add(GETBUEXBYNAME.FunctionName, GETBUEXBYNAME);
            this.Apis.Add(DELETEBUEXBYIDSEQNO.FunctionName, DELETEBUEXBYIDSEQNO);
            this.Apis.Add(CHECKIDSEQNOISEXIST.FunctionName, CHECKIDSEQNOISEXIST);
            this.Apis.Add(CHECKNAMEVALUEISEXIST.FunctionName, CHECKNAMEVALUEISEXIST);
            this.Apis.Add(CHECKIDSEQNONAMEVALUEISEXIST.FunctionName, CHECKIDSEQNONAMEVALUEISEXIST);
            this.Apis.Add(UPDATE.FunctionName, UPDATE);
            this.Apis.Add(INSERT.FunctionName, INSERT);
        }

        public void GetAllBuEx(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)  
        {
            OleExec oleDB = null;
            T_C_BU_EX buex = null;
            List<C_BU_EX> buexList = new List<C_BU_EX>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                buex = new T_C_BU_EX(oleDB, DBTYPE);
                buexList = buex.GetAllBUEx(oleDB);
                if (buexList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryOK";
                    StationReturn.MessagePara.Add(buexList.Count);
                    StationReturn.Data = buexList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryNoData";
                    StationReturn.Data = "";
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = exception.Message;
                StationReturn.Data = "";
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
        }


        public void GetBuExByName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<C_BU_EX> BUExList = new List<C_BU_EX>();
            T_C_BU_EX Table = null;
            string BUNAME = string.Empty;

            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);
                BUNAME = Data["BUNAME"].ToString().Trim();

                BUExList = Table.GetBuExByName(BUNAME, sfcdb);
                if (BUExList.Count() == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryNoData";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryOK";
                    StationReturn.MessagePara.Add(BUExList.Count().ToString());
                    StationReturn.Data = BUExList;
                }


                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }
        public void DeleteBuExByIDSEQNO(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_BU_EX Table = null;
            string ID = string.Empty;
            decimal SEQ_NO;
            int result;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);

                ID = Data["ID"].ToString().Trim();
                SEQ_NO = decimal.Parse(Data["SEQ_NO"].ToString().Trim());
                result = Table.DeleteBuExByID_SEQ_NO(ID, SEQ_NO, sfcdb);
                if (result == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Delete Fail";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Delete success";
                    StationReturn.Data = new object();
                }


                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }
        public void Update(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            C_BU_EX old_data = JsonConvert.DeserializeObject<C_BU_EX>(Data["old_data_obj"].ToString());
            C_BU_EX new_data = JsonConvert.DeserializeObject<C_BU_EX>(Data["new_data_obj"].ToString());

            OleExec sfcdb = null;
            T_C_BU_EX Table = null;
            int result;
            bool flag;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);
                flag = Table.ID_SEQ_NO_IsExist(old_data.ID, decimal.Parse(old_data.SEQ_NO.ToString()), sfcdb);    //check id seq_no exist
                if(flag)
                {
                  if   ((old_data.NAME+old_data.VALUE)!=(new_data.NAME+new_data.VALUE))
                    {
                        ///check new name and value
                        flag= Table.NAME_VALUE_IsExist(new_data.NAME, new_data.VALUE, sfcdb);
                        if(!flag)
                        {
                            //update
                           result= Table.UpDate(new_data.ID, decimal.Parse(new_data.SEQ_NO.ToString()), new_data.NAME, new_data.VALUE, sfcdb);
                            if(result>0)
                            {
                                StationReturn.Status = StationReturnStatusValue.Pass;
                                StationReturn.MessageCode = "Update success";
                                StationReturn.Data = new object();
                            }
                            else
                            {
                                StationReturn.Status = StationReturnStatusValue.Pass;
                                StationReturn.MessageCode = "Update Fail";
                                StationReturn.Data = new object();
                            }
                        }
                        else
                        {
                            StationReturn.Status = StationReturnStatusValue.Pass;
                            StationReturn.MessageCode = "Update fail new name and value existed";
                            StationReturn.Data = new object();
                        }
                    }
                   else
                    {
                        StationReturn.Status = StationReturnStatusValue.Pass;
                        StationReturn.MessageCode = "Update Fail";
                        StationReturn.Data = new object();
                    }
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Update Fail";
                    StationReturn.Data = new object();
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }


        }

        public void Insert(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_BU_EX Table = null;
            string NAME = string.Empty;
            string VALUE = string.Empty;
            int result;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);
                NAME = Data["NAME"].ToString().Trim();
                VALUE = Data["VALUE"].ToString().Trim();
                 result = Table.Insert(BU,NAME, VALUE, sfcdb,SystemName,DBTYPE);
                if (result == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Insert Fail";
                    StationReturn.Data = new object();
                }
                else if (result > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Insert success";
                    StationReturn.Data = new object();
                }
                else if (result ==-1)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Error name and value existed";
                    StationReturn.Data = new object();
                }


                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }


        public void CheckIdSeqNoIsExist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_BU_EX Table = null;
            string ID = string.Empty;
            decimal SEQ_NO;
            bool result;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);
                ID = Data["ID"].ToString().Trim();
                SEQ_NO = decimal.Parse(Data["SEQ_NO"].ToString().Trim());
                result = Table.ID_SEQ_NO_IsExist(ID, SEQ_NO, sfcdb);
                if (!result)  //not exist
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryNoData";
                    StationReturn.Data = new object();
                }
                else
                //  exist
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryOK";
                    StationReturn.Data = new object();
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }


        public void CheckNameValueIsExist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_BU_EX Table = null;
            string NAME = string.Empty;
            string VALUE = string.Empty;
            bool result;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);
                NAME = Data["NAME"].ToString().Trim();
                VALUE = Data["VALUE"].ToString().Trim();
                result = Table.NAME_VALUE_IsExist(NAME, VALUE, sfcdb);
                if (result)  // exist
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryOK";
                    StationReturn.Data = new object();

                }
                else
                // not  exist
                {
                 
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryNoData";
                    StationReturn.Data = new object();
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }

        public void CheckIdSeqNoNameValueIsExist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_BU_EX Table = null;
            string ID = string.Empty;
            decimal SEQ_NO;
            string NAME = string.Empty;
            string VALUE = string.Empty;
            bool result;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_BU_EX(sfcdb, DBTYPE);
                ID = Data["ID"].ToString().Trim();
                SEQ_NO = decimal.Parse(Data["SEQ_NO"].ToString().Trim());
                NAME = Data["NAME"].ToString().Trim();
                VALUE = Data["VALUE"].ToString().Trim();
                result = Table.ID_SEQ_NO_NAME_VALUE_IsExist(ID,SEQ_NO,NAME, VALUE, sfcdb);
                if (!result)  //not exist
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryNoData";
                    StationReturn.Data = new object();
                }
                else
                //  exist
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "QueryOK";
                    StationReturn.Data = new object();
                }

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "Exception";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }

        }
    }
}
