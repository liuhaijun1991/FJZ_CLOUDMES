using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config
{
    public class CSectionConfig : MESPubLab.MESStation.MesAPIBase
    {
        public CSectionConfig()
        {
            this.Apis.Add(FSelectByColumnName.FunctionName, FSelectByColumnName);
            this.Apis.Add(FSectionInsert.FunctionName, FSectionInsert);
            this.Apis.Add(FSectionDelete.FunctionName, FSectionDelete);
            this.Apis.Add(FShowAllData.FunctionName, FShowAllData);
            this.Apis.Add(FUpdateSectionByID.FunctionName, FUpdateSectionByID);
            this.Apis.Add(FGetSection.FunctionName, FGetSection);
        }

        protected APIInfo FShowAllData = new APIInfo()
        {
            FunctionName = "ShowAllData",
            Description = "查询Section表的所有数据",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FGetSection = new APIInfo()
        {
            FunctionName = "GetSection",
            Description = "SelectAllSection",
            Parameters = new List<APIInputInfo>() { new APIInputInfo() { InputName = "SECTION_NAME", InputType = "string", DefaultValue = "" } },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FUpdateSectionByID = new APIInfo()
        {
            FunctionName = "UpdateSectionByID",
            Description = "根据ID值更新SECTION",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SectionName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSelectByColumnName = new APIInfo()
        {
            FunctionName = "SelectByColumnName",
            Description = "根据传入的栏位及值进行相应查询",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ColumnName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "ColumnValue", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSectionInsert = new APIInfo()
        {
            FunctionName = "SectionInsert",
            Description = "执行插入操作",
            Parameters = new List<APIInputInfo>()
            {
                //new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "SectionName", InputType = "string", DefaultValue = "" },
                new APIInputInfo() {InputName = "DESCRIPTION", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        protected APIInfo FSectionDelete = new APIInfo()
        {
            FunctionName = "DeleteSectionByID",
            Description = "通过ID进行删除操作",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "ID", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };

        public void UpdateSectionByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            T_C_SECTION cSection = null;
            OleExec sfcdb = null;
            try
            {
                sfcdb= this.DBPools["SFCDB"].Borrow();
                cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SECTION r = (Row_C_SECTION)cSection.NewRow();
                r = (Row_C_SECTION)cSection.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                r.SECTION_NAME = (Data["SectionName"].ToString()).Trim();
                r.DESCRIPTION = (Data["DESCRIPTION"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet)>0)
                {
                    StationReturn.MessageCode = "MES00000003";
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Data = ""; 
                }
                else
                {
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void GetSection(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            T_C_SECTION sec = null;
            string SECTION_NAME = string.Empty;
            List<C_SECTION_DETAIL> List = new List<C_SECTION_DETAIL>();
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                sec = new T_C_SECTION(oleDB, DBTYPE);
                SECTION_NAME = Data["SECTION_NAME"].ToString().Trim();
                List = sec.GetSectionT(SECTION_NAME, oleDB);
                if (List.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(List.Count);
                    StationReturn.Data = List;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
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
        public void SectionInsert(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SECTION r = (Row_C_SECTION)cSection.NewRow();
                r.ID = cSection.GetNewID(BU, sfcdb, DBTYPE);
                r.SECTION_NAME = (Data["SectionName"].ToString()).Trim();
                r.DESCRIPTION = (Data["DESCRIPTION"].ToString()).Trim();
                string strRet = sfcdb.ExecSQL(r.GetInsertString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    StationReturn.Data = strRet;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000036";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void ShowAllData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            try
            {
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
                list = cSection.ShowAllData(sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000001";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);

            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
        public void SelectByColumnName(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                List<C_SECTION_DETAIL> list = new List<C_SECTION_DETAIL>();
                string data = Data["ColumnValue"].ToString();
                string column = Data["ColumnName"].ToString();
                list = cSection.GetDataByColumn(column, data, sfcdb);
                if (list.Count > 0)
                {
                    StationReturn.Data = list;
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(list.Count);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }

        public void DeleteSectionByID(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                T_C_SECTION cSection = new T_C_SECTION(sfcdb, DB_TYPE_ENUM.Oracle);
                Row_C_SECTION r = (Row_C_SECTION)cSection.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
                string strRet = sfcdb.ExecSQL(r.GetDeleteString(DB_TYPE_ENUM.Oracle));
                if (Convert.ToInt32(strRet) > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000004";
                    StationReturn.Data = "";
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "NotLatestData";
                }
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }
        }
    }
}

//RuRun
