using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Config
{
    public class FactoryConfig : MESPubLab.MESStation.MesAPIBase
    {
        protected APIInfo FGETALLFACTORY = new APIInfo()
        {
            FunctionName = "GetAllFactory",
            Description = "GetAllFactory",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "FACTORY_CODE", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FINSERT = new APIInfo()
        {
            FunctionName = "InsertFactory",
            Description = "InsertFactory",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "FACTORY_CODE", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "FACTORY_NAME", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "FACTORY_ADDRESS", InputType = "string", DefaultValue = "" },
                                                    new APIInputInfo() { InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
                                                    },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUPDATE = new APIInfo()
        {
            FunctionName = "UpdateFactory",
            Description = "UpdateFactory",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "FACTORY_CODE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "FACTORY_NAME", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "FACTORY_ADDRESS", InputType = "string", DefaultValue = "" },
                new APIInputInfo() { InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
             },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FDELETE = new APIInfo()
        {
            FunctionName = "DeleteFactory",
            Description = "DeleteFactory",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName = "del_object", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public FactoryConfig()
        {
            _MastLogin = false;
            this.Apis.Add(FGETALLFACTORY.FunctionName, FGETALLFACTORY);
            this.Apis.Add(FINSERT.FunctionName, FINSERT);
            this.Apis.Add(FUPDATE.FunctionName, FUPDATE);
            this.Apis.Add(FDELETE.FunctionName, FDELETE);
        }

        public void GetAllFactory(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_FACTORY factory = null;
            string FACTORY_CODE = string.Empty;
            List<C_FACTORY> factoryList = new List<C_FACTORY>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                factory = new T_C_FACTORY(oleDB, DBTYPE);
                FACTORY_CODE = Data["FACTORY_CODE"].ToString().Trim().ToUpper();
                factoryList = factory.GetAllFactory(FACTORY_CODE, oleDB);
                if (factoryList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(factoryList.Count);
                    StationReturn.Data = factoryList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
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

        public void InsertFactory(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            OleExec oleDB = null;
            T_C_FACTORY Table = null;
            string FACTORY_CODE = string.Empty;
            string FACTORY_NAME = string.Empty;
            string FACTORY_ADDRESS = string.Empty;
            string DESCRIPTION = string.Empty;
            string EDIT_EMP = string.Empty;
            int result;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_FACTORY(oleDB, DBTYPE);
                FACTORY_CODE = Data["FACTORY_CODE"].ToString().Trim().ToUpper();
                FACTORY_NAME = Data["FACTORY_NAME"].ToString().Trim().ToUpper();
                FACTORY_ADDRESS = Data["FACTORY_ADDRESS"].ToString().Trim().ToUpper();
                DESCRIPTION = Data["DESCRIPTION"].ToString().Trim();
                EDIT_EMP = LoginUser.EMP_NO.Trim();

                result = Table.Insert(FACTORY_CODE, FACTORY_NAME, FACTORY_ADDRESS, DESCRIPTION, EDIT_EMP, BU, DBTYPE, oleDB);
                if (result == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Insert fail Factory code, Factory name, Factory address existed";
                    StationReturn.Data = new object();
                }
                else if (result == 1)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Insert success";
                    StationReturn.Data = new object();
                }
                else if (result == 2)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Insert fail";
                    StationReturn.Data = new object();
                }
                else if (result == 3)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.Data = new object();
                }


                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }

        }


        public void UpdateFactory(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_FACTORY Table = null;
            string ID = string.Empty;
            string FACTORY_CODE = string.Empty;
            string FACTORY_NAME = string.Empty;
            string FACTORY_ADDRESS = string.Empty;
            string DESCRIPTION = string.Empty;
            string EDIT_EMP = string.Empty;
            int result;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_FACTORY(oleDB, DBTYPE);
                ID = Data["ID"].ToString().Trim();
                FACTORY_CODE = Data["FACTORY_CODE"].ToString().Trim().ToUpper();
                FACTORY_NAME = Data["FACTORY_NAME"].ToString().Trim().ToUpper();
                FACTORY_ADDRESS = Data["FACTORY_ADDRESS"].ToString().Trim().ToUpper();
                DESCRIPTION = Data["DESCRIPTION"].ToString().Trim();
                EDIT_EMP = LoginUser.EMP_NO.Trim();

                result = Table.Update(ID, FACTORY_CODE, FACTORY_NAME, FACTORY_ADDRESS, DESCRIPTION, EDIT_EMP, oleDB);
                if (result == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Update fail ID not exist";
                    StationReturn.Data = new object();
                }
                else if (result == 1)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Update fail Factorycode, Factoryname, Factoryaddress existed ";
                    StationReturn.Data = new object();
                }
                else if (result == 2)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Update success";
                    StationReturn.Data = new object();
                }
                else if (result == 3)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Update fail";
                    StationReturn.Data = new object();
                }
                else if (result == 4)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.Data = new object();
                }
                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }

        }

        public void DeleteFactory(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_FACTORY Table = null;
            string ID = string.Empty;
            int result;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                Table = new T_C_FACTORY(oleDB, DBTYPE);
                ID = Data["ID"].ToString().Trim();
                result = Table.Delete(ID, oleDB);
                if (result == 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "Delete success";
                    StationReturn.Data = new object();
                }
                else if (result == 1)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Delete fail";
                    StationReturn.Data = new object();
                }
                else if (result == 2)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "Delete fail ID is empty";
                    StationReturn.Data = new object();
                }
                else if (result == 3)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000037";
                    StationReturn.Data = new object();
                }

                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }
            catch (Exception e)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(e.Message);
                StationReturn.Data = e.Message;

                if (oleDB != null)
                {
                    this.DBPools["SFCDB"].Return(oleDB);
                }
            }

        }

    }
}
