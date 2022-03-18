using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;
using MESDBHelper;
using MESDataObject.Module;
using Newtonsoft.Json;
using MESDataObject;
using Newtonsoft.Json.Linq;

namespace MESStation.GlobalConfig
{
    public class MESMessageConfig : MesAPIBase
    {
        protected APIInfo FGetAllMESMessage = new APIInfo()
        {
            FunctionName = "GetAllMESMessage",
            Description = "獲取所有MES Message",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FUpdateMESMessage = new APIInfo()
        {
            FunctionName = "UpdateMESMessage",
            Description = "更新MES Message By ID",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "查詢條件" },
                new APIInputInfo() {InputName = "MessageCode", InputType = "string", DefaultValue = "MES00000001" },
                new APIInputInfo() {InputName = "Chinese", InputType = "string", DefaultValue = "中文簡體" },
                new APIInputInfo() {InputName = "Chinese_TW", InputType = "string", DefaultValue = "中文繁體" },
                new APIInputInfo() {InputName = "English", InputType = "string", DefaultValue = "英語" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FAddMESMessage = new APIInfo()
        {
            FunctionName = "AddMESMessage",
            Description = "新增MES Message",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "MessageCode", InputType = "string", DefaultValue = "MES00000001" },
                new APIInputInfo() {InputName = "Chinese", InputType = "string", DefaultValue = "中文簡體" },
                new APIInputInfo() {InputName = "Chinese_TW", InputType = "string", DefaultValue = "中文繁體" },
                new APIInputInfo() {InputName = "English", InputType = "string", DefaultValue = "英語" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FDeleteMESMessageByID = new APIInfo()
        {
            FunctionName = "DeleteMESMessageByID",
            Description = "刪除MES Message",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetDetail = new APIInfo()
        {
            FunctionName = "GetDetail",
            Description = "通過名稱獲取詳細信息",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ZH_CN", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ZH_TW", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo _GetMsgCode = new APIInfo()
        {
            FunctionName = "GetMsgCode",
            Description = "通過名稱獲取詳細信息",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "ZH_CN", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "ZH_TW", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "EN", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };


        public MESMessageConfig()
        {
            this.Apis.Add(FGetAllMESMessage.FunctionName, FGetAllMESMessage);
            this.Apis.Add(FUpdateMESMessage.FunctionName, FUpdateMESMessage);
            this.Apis.Add(FAddMESMessage.FunctionName, FAddMESMessage);
            this.Apis.Add(FDeleteMESMessageByID.FunctionName, FDeleteMESMessageByID);
            Apis.Add(_GetDetail.FunctionName, _GetDetail);
        }

        public void GetDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_MES_MESSAGE> ret = null;
            string zh_cn = Data["ZH_CN"].ToString();
            string zh_tw = Data["ZH_TW"].ToString();
            string en = Data["EN"].ToString();
            
            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                //
               //ret = new T_C_MES_MESSAGE(Sfcdb, DB_TYPE_ENUM.Oracle).GetMsgDetail(Sfcdb, "CHINESE", value);
                ret = new T_C_MES_MESSAGE(Sfcdb, DB_TYPE_ENUM.Oracle)._GetMsgDetail(Sfcdb, zh_cn, zh_tw, en);
                if (ret == null || ret.Count == 0)
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                }
                else
                {
                    StationReturn.Data = ret;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                //
                if (Sfcdb != null)
                {
                    DBPools["SFCDB"].Return(Sfcdb);
                }
            }
            catch (Exception ex)
            {
                if (Sfcdb != null)
                {
                    DBPools["SFCDB"].Return(Sfcdb);
                }
                throw ex;
            }
        }

        public void GetMsgCode(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                //MESCODE修改為唯一;add by Eden 20180525
                //string MessageCode = new T_C_SEQNO(Sfcdb, DB_TYPE_ENUM.Oracle).GetLotno("MESMSGCODE", Sfcdb);
                string MessageCode = "MSGCODE" + this.GetDBDateTime().ToString("yyyyMMddHHmmsss");
                if (string.IsNullOrEmpty(MessageCode))
                {
                    StationReturn.Data = "";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                }
                else
                {
                    StationReturn.Data = MessageCode;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                //
                if (Sfcdb != null)
                {
                    DBPools["SFCDB"].Return(Sfcdb);
                }
            }
            catch (Exception ex)
            {
                if (Sfcdb != null)
                {
                    DBPools["SFCDB"].Return(Sfcdb);
                }
                throw ex;
            }
        }

        public void GetAllMESMessage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_MES_MESSAGE> Ret = new List<C_MES_MESSAGE>();
            T_C_MES_MESSAGE C_Mes_Message = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                Ret = C_Mes_Message.GetAllMESMessage(Sfcdb, DBTYPE);

                StationReturn.Data = Ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Message = "獲取ALLMessage OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

        public void UpdateMESMessage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            string UpdateSql = "";
            T_C_MES_MESSAGE C_Mes_Message = null;
            Row_C_MES_MESSAGE Row_C_Mes_Message = null;
            try
            {
                string ID = Data["ID"].ToString();
                string MessageCode = Data["MessageCode"].ToString();
                string Chinese = Data["Chinese"].ToString();
                string English = Data["English"].ToString();
                string Chinese_TW = Data["Chinese_TW"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();
                C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                Row_C_Mes_Message = (Row_C_MES_MESSAGE)C_Mes_Message.GetObjByID(Data["ID"].ToString(), Sfcdb);
                if (Row_C_Mes_Message == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    StationReturn.MessagePara = new List<object>() { MessageCode };
                    return;
                }
                Row_C_Mes_Message.ID = ID;
                Row_C_Mes_Message.MESSAGE_CODE = MessageCode;
                Row_C_Mes_Message.CHINESE = Chinese;
                Row_C_Mes_Message.ENGLISH = English;
                Row_C_Mes_Message.CHINESE_TW = Chinese_TW;
                UpdateSql = Row_C_Mes_Message.GetUpdateString(DBTYPE);
                Sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                StationReturn.Message = "MessageCode更新OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

        public void AddMESMessage(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            T_C_MES_MESSAGE C_Mes_Message = null;
            Row_C_MES_MESSAGE Row_C_Mes_Message = null;
            string InsertSql = "";
            try
            {
                string MessageCode = Data["MessageCode"].ToString();
                string Chinese = Data["Chinese"].ToString();
                string English = Data["English"].ToString();
                string Chinese_TW = Data["Chinese_TW"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();
                //GET MESSAGE CODE TEST
                //string MessageCode = new T_C_SEQNO(Sfcdb, DB_TYPE_ENUM.Oracle).GetLotno("MESMSGCODE", Sfcdb);

                C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                Row_C_Mes_Message =C_Mes_Message.GetMESMessageByMessageCode(MessageCode, Sfcdb,DBTYPE);
                //
                if (Row_C_Mes_Message != null)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Message = "MessageCode已經存在!";
                    return;
                }
                //C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                Row_C_Mes_Message = (Row_C_MES_MESSAGE)C_Mes_Message.NewRow();
                Row_C_Mes_Message.ID = C_Mes_Message.GetNewID(BU, Sfcdb);
                Row_C_Mes_Message.MESSAGE_CODE = MessageCode;
                Row_C_Mes_Message.CHINESE = Chinese;
                Row_C_Mes_Message.ENGLISH = English;
                Row_C_Mes_Message.CHINESE_TW = Chinese_TW;
                Row_C_Mes_Message.EDIT_EMP = LoginUser.EMP_NO;
                Row_C_Mes_Message.EDIT_TIME = GetDBDateTime();
                InsertSql = Row_C_Mes_Message.GetInsertString(DBTYPE);
                Sfcdb.ExecSQL(InsertSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Message = "新增MessageCode OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

        public void DeleteMESMessageByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            T_C_MES_MESSAGE C_Mes_Message = null;
            Row_C_MES_MESSAGE Row_C_Mes_Message = null;
            string DeleteSql = "";

            try
            {
                string ID = Data["ID"].ToString();
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                Row_C_Mes_Message = (Row_C_MES_MESSAGE)C_Mes_Message.GetObjByID(ID, Sfcdb);

                DeleteSql = Row_C_Mes_Message.GetDeleteString(DBTYPE);
                Sfcdb.ExecSQL(DeleteSql);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                StationReturn.Message = "By ID刪除Message OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }
    }
}