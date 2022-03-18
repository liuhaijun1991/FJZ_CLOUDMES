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

namespace MESStation.Config.ORACLE
{
    public class AssemblyMappingConfig : MesAPIBase
    {
        protected APIInfo FGetAllAssemblyMapping = new APIInfo()
        {
            FunctionName = "GetAllAssemblyMapping",
            Description = "Get all mapping data.",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FUpdateAssemblyMapping = new APIInfo()
        {
            FunctionName = "UpdateAssemblyMapping",
            Description = "Update mapping data.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CONFIGHEADERID", InputType = "string", DefaultValue = "SKU" },
                new APIInputInfo() {InputName = "CUSTPARTNO", InputType = "string", DefaultValue = "PARTNO" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "PN Desc" },
                new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "QTY" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "Location/slot" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FAddAssemblyMapping = new APIInfo()
        {
            FunctionName = "AddAssemblyMapping",
            Description = "Add mapping data.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CONFIGHEADERID", InputType = "string", DefaultValue = "MES00000001" },
                new APIInputInfo() {InputName = "REV", InputType = "string", DefaultValue = "PARTNO" },
                new APIInputInfo() {InputName = "CUSTPARTNO", InputType = "string", DefaultValue = "Revision" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "PN Desc" },
                new APIInputInfo() {InputName = "QTY", InputType = "string", DefaultValue = "QTY" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "Location/slot" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo FDeleteAssemblyMappingByID = new APIInfo()
        {
            FunctionName = "DeleteAssemblyMappingByID",
            Description = "Delete mapping data.",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CONFIGHEADERID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUSTPARTNO", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        protected APIInfo _GetDetail = new APIInfo()
        {
            FunctionName = "GetDetail",
            Description = "Get details by input",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "CONFIGHEADERID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "CUSTPARTNO", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "LOCATION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

     


        public AssemblyMappingConfig()
        {
            this.Apis.Add(FGetAllAssemblyMapping.FunctionName, FGetAllAssemblyMapping);
            this.Apis.Add(FUpdateAssemblyMapping.FunctionName, FUpdateAssemblyMapping);
            this.Apis.Add(FAddAssemblyMapping.FunctionName, FAddAssemblyMapping);
            this.Apis.Add(FDeleteAssemblyMappingByID.FunctionName, FDeleteAssemblyMappingByID);
            Apis.Add(_GetDetail.FunctionName, _GetDetail);
        }

        public void GetDetail(JObject requestValue, JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_ORACLE_MFASSEMBLYDATA> ret = null;
            string configheaderid = Data["CONFIGHEADERID"].ToString();
            string custpartno = Data["CUSTPARTNO"].ToString();
            string location = Data["LOCATION"].ToString();
            
            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                ret = new T_C_ORACLE_MFASSEMBLYDATA(Sfcdb, DB_TYPE_ENUM.Oracle)._GetMappingDetail(Sfcdb, configheaderid, custpartno, location);
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

       

        public void GetAllAssemblyMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            List<C_ORACLE_MFASSEMBLYDATA> Ret = new List<C_ORACLE_MFASSEMBLYDATA>();
            T_C_ORACLE_MFASSEMBLYDATA T_C_ORACLE_MFASSEMBLYDATA = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_ORACLE_MFASSEMBLYDATA = new T_C_ORACLE_MFASSEMBLYDATA(Sfcdb, DBTYPE);
                Ret = T_C_ORACLE_MFASSEMBLYDATA.GetAllAssemblyMappingData(Sfcdb, DBTYPE);

                StationReturn.Data = Ret;
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Message = "Get all assembly data OK!";
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(Sfcdb);
                throw (e);
            }
        }

       
        public void UpdateAssemblyMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //2019/04/25 bug on update, need to be able to get old value.
            OleExec Sfcdb = null;
            string UpdateSql = "";
            T_C_ORACLE_MFASSEMBLYDATA C_ORACLE_MFASSEMBLYDATA = null;
            Row_C_ORACLE_MFASSEMBLYDATA Row_C_ORACLE_MFASSEMBLYDATA = null;
            try
            {
                string ID = Data["ID"].ToString();
                string CONFIGHEADERID = Data["CONFIGHEADERID"].ToString();
                string CUSTPARTNO = Data["CUSTPARTNO"].ToString();
                string DESCRIPTION = Data["DESCRIPTION"].ToString();
                string QTY = Data["QTY"].ToString();
                string LOCATION = Data["LOCATION"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();
                C_ORACLE_MFASSEMBLYDATA = new T_C_ORACLE_MFASSEMBLYDATA(Sfcdb, DBTYPE);
                Row_C_ORACLE_MFASSEMBLYDATA = (Row_C_ORACLE_MFASSEMBLYDATA)C_ORACLE_MFASSEMBLYDATA.GetObjByID(ID, Sfcdb);
                if (Row_C_ORACLE_MFASSEMBLYDATA == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000007";
                    return;
                }
                //Row_C_ORACLE_MFASSEMBLYDATA.DESCRIPTION = DESCRIPTION;
                //Row_C_ORACLE_MFASSEMBLYDATA.QTY = QTY;
                //Row_C_ORACLE_MFASSEMBLYDATA.LOCATION = LOCATION;
                UpdateSql = "UPDATE C_ORACLE_MFASSEMBLYDATA set DESCRIPTION='"+ DESCRIPTION + "', QTY='"+ QTY + "' , LOCATION='"+ LOCATION + "' , LASTEDITTIME = TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') where ID = '"+ID+"' and CONFIGHEADERID = '"+ CONFIGHEADERID + "'";
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

        public void AddAssemblyMapping(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;
            T_C_ORACLE_MFASSEMBLYDATA C_ORACLE_MFASSEMBLYDATA = null;
            Row_C_ORACLE_MFASSEMBLYDATA Row_C_ORACLE_MFASSEMBLYDATA = null;
            string InsertSql = "";
            try
            {
                string CONFIGHEADERID = Data["CONFIGHEADERID"].ToString();
                string REV = Data["REV"].ToString();
                string CUSTPARTNO = Data["CUSTPARTNO"].ToString();
                string DESCRIPTION = Data["DESCRIPTION"].ToString();
                string QTY = Data["QTY"].ToString();
                string LOCATION = Data["LOCATION"].ToString();

                Sfcdb = this.DBPools["SFCDB"].Borrow();

                C_ORACLE_MFASSEMBLYDATA = new T_C_ORACLE_MFASSEMBLYDATA(Sfcdb, DBTYPE);
                Row_C_ORACLE_MFASSEMBLYDATA = C_ORACLE_MFASSEMBLYDATA.GetMappingBySKUPN(CONFIGHEADERID, CUSTPARTNO, Sfcdb);
                //
                if (Row_C_ORACLE_MFASSEMBLYDATA != null)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                    StationReturn.Message = "MessageCode已經存在!";
                    return;
                }
                //C_Mes_Message = new T_C_MES_MESSAGE(Sfcdb, DBTYPE);
                Row_C_ORACLE_MFASSEMBLYDATA = (Row_C_ORACLE_MFASSEMBLYDATA)C_ORACLE_MFASSEMBLYDATA.NewRow();
                Row_C_ORACLE_MFASSEMBLYDATA.ID = C_ORACLE_MFASSEMBLYDATA.GetNewID(BU, Sfcdb);
                Row_C_ORACLE_MFASSEMBLYDATA.CONFIGHEADERID = CONFIGHEADERID;
                Row_C_ORACLE_MFASSEMBLYDATA.REV = REV;
                Row_C_ORACLE_MFASSEMBLYDATA.CUSTPARTNO = CUSTPARTNO;
                Row_C_ORACLE_MFASSEMBLYDATA.DESCRIPTION = DESCRIPTION;
                Row_C_ORACLE_MFASSEMBLYDATA.QTY = QTY;
                Row_C_ORACLE_MFASSEMBLYDATA.LOCATION = LOCATION;
                Row_C_ORACLE_MFASSEMBLYDATA.LASTEDITTIME = GetDBDateTime();
                InsertSql = Row_C_ORACLE_MFASSEMBLYDATA.GetInsertString(DBTYPE);
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

        public void DeleteAssemblyMappingByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec Sfcdb = null;

            try
            {
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                JToken[] IDS = Data["ID"].ToArray();
                var temp = Sfcdb.ORM.Queryable<C_ORACLE_MFASSEMBLYDATA>().Where(t => IDS.Contains(t.ID)).ToList();
                Sfcdb.ORM.Deleteable<C_ORACLE_MFASSEMBLYDATA>(temp).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                StationReturn.Message = "By ID delete assembly data OK!";
            }
            catch (Exception e)
            {
                throw (e);
            }
            finally {
                this.DBPools["SFCDB"].Return(Sfcdb);
            }
        }
    }
}