using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;

namespace MESStation.Config.ORACLE
{
    public class PartNoExceptionConfig: MesAPIBase
    {
        protected APIInfo FGetAllPartNoException = new APIInfo()
        {
            FunctionName = "GetAllPartNoException",
            Description = "Get all PN exception.",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

       
        protected APIInfo FAddPartNoException = new APIInfo()
        {
            FunctionName = "AddPartNoException",
            Description = "Add PN exception.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "PARTNO" },
                new APIInputInfo() {InputName = "EXCEPTIONTYPE", InputType = "string", DefaultValue = "EXCEPTIONTYPE" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo FDeletePartNoException = new APIInfo()
        {
            FunctionName = "DeletePartNoException",
            Description = "Delete partno exception.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXCEPTIONTYPE", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()
        };

        protected APIInfo _GetDetail = new APIInfo()
        {
            FunctionName = "GetDetail",
            Description = "Get details by input",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EXCEPTIONTYPE", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_EMP", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "EDIT_TIME", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public PartNoExceptionConfig()
        {
            this.Apis.Add(FGetAllPartNoException.FunctionName, FGetAllPartNoException);
            this.Apis.Add(FAddPartNoException.FunctionName, FAddPartNoException);
            this.Apis.Add(FDeletePartNoException.FunctionName, FDeletePartNoException);
            Apis.Add(_GetDetail.FunctionName, _GetDetail);
        }

        public void GetDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_PARTNO_EXCEPTION> ret = null;
            string PARTNO = Data["PARTNO"].ToString();
            string EXCEPTIONTYPE = Data["EXCEPTIONTYPE"].ToString();

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                ret = new T_C_PARTNO_EXCEPTION(Sfcdb, DB_TYPE_ENUM.Oracle)._GetPNExceptionDetail(Sfcdb, PARTNO, EXCEPTIONTYPE);
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

        public void GetAllPartNoException(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_PARTNO_EXCEPTION> Ret = new List<C_PARTNO_EXCEPTION>();
            T_C_PARTNO_EXCEPTION t_c_partno_exception = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                t_c_partno_exception = new T_C_PARTNO_EXCEPTION(Sfcdb, DBTYPE);
                Ret = t_c_partno_exception.GetAllPNException(Sfcdb, DBTYPE);

                StationReturn.Data = Ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Message = "Get all PN exception data OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }
               

        public void GetAllException(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_PARTNO_EXCEPTION excp = null;
            List<C_PARTNO_EXCEPTION> excpList = new List<C_PARTNO_EXCEPTION>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                excp = new T_C_PARTNO_EXCEPTION(oleDB, DBTYPE);
                excpList = excp.GetAllException(oleDB, DBTYPE);
                if (excpList.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(excpList.Count);
                    StationReturn.Data = excpList;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(oleDB);
            }
            catch (Exception exception)
            {
                this.DBPools["SFCDB"].Return(oleDB);
                throw exception;
            }
        }


        public void AddPartNoException(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            T_C_PARTNO_EXCEPTION c_partno_exception = null;
            Row_C_PARTNO_EXCEPTION row_c_partno_exception = null;
            string InsertSql = "";
            try
            {
                string PARTNO = Data["PARTNO"].ToString();
                string EXCEPTIONTYPE = Data["EXCEPTIONTYPE"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();

                c_partno_exception = new T_C_PARTNO_EXCEPTION(Sfcdb, DBTYPE);
                row_c_partno_exception = c_partno_exception.GetExceptionByPN(PARTNO, EXCEPTIONTYPE, Sfcdb);
                //
                if (row_c_partno_exception != null)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Message = "PN has existed in exception list!";
                    return;
                }
                //C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                row_c_partno_exception = (Row_C_PARTNO_EXCEPTION)c_partno_exception.NewRow();
                row_c_partno_exception.PARTNO = PARTNO;
                row_c_partno_exception.EXCEPTIONTYPE = EXCEPTIONTYPE;
                row_c_partno_exception.EDIT_EMP = LoginUser.EMP_NO;
                row_c_partno_exception.EDIT_TIME = GetDBDateTime();
                InsertSql = row_c_partno_exception.GetInsertString(DBTYPE);
                Sfcdb.ExecSQL(InsertSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000002";
                StationReturn.Message = "Insert PN exception OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

        public void DeletePartNoException(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            T_C_PARTNO_EXCEPTION c_partno_exception = null;
            Row_C_PARTNO_EXCEPTION row_c_partno_exception = null;
            string DeleteSql = "";

            try
            {
                string PARTNO = Data["PARTNO"].ToString();
                string EXCEPTION = Data["EXCEPTIONTYPE"].ToString();
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                c_partno_exception = new T_C_PARTNO_EXCEPTION(Sfcdb, DBTYPE);
                row_c_partno_exception = (Row_C_PARTNO_EXCEPTION)c_partno_exception.GetExceptionByPN(PARTNO, EXCEPTION, Sfcdb);

                DeleteSql = row_c_partno_exception.GetDeleteString(DBTYPE);
                Sfcdb.ExecSQL(DeleteSql);

                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                StationReturn.Message = "Delete exception PN OK!";
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
