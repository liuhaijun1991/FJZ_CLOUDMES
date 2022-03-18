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
    public class DepartmentConfig : MESPubLab.MESStation.MesAPIBase
    {
        private APIInfo alldepartment = new APIInfo()
        {
            FunctionName = "GetDepartmentList",
            Description = "獲取所有部門",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo FGetdepartmentByEmpLevel = new APIInfo()
        {
            FunctionName = "GetDepartmentListByEmpLevel",
            Description = "根據用戶等級獲取獲取 部門",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo adddepartment = new APIInfo()
        {
            FunctionName = "AddDepartment",
            Description = "添加部門",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="DepartmentName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SeqNo",InputType="string",DefaultValue=""}


            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo deletedepartment = new APIInfo()
        {
            FunctionName = "DeleteDepartment",
            Description = "刪除部門",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""}


            },
            Permissions = new List<MESPermission>()
            { }

        };

        private APIInfo updatedepartment = new APIInfo()
        {
            FunctionName = "UpdateDepartment",
            Description = "更新部門",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DepartmentName",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="Desc",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="SeqNo",InputType="string",DefaultValue=""}


            },
            Permissions = new List<MESPermission>()
            { }

        };
        private APIInfo querydepartment = new APIInfo()
        {
            FunctionName = "QueryDepartment",
            Description = "查詢部門",
            Parameters = new List<APIInputInfo>()
            {
                 new APIInputInfo() { InputName="ID",InputType="string",DefaultValue=""},
                new APIInputInfo() { InputName="DepartmentName",InputType="string",DefaultValue=""}


            },
            Permissions = new List<MESPermission>()
            { }

        };


        public DepartmentConfig()
        {
            this.Apis.Add(alldepartment.FunctionName, alldepartment);
            this.Apis.Add(deletedepartment.FunctionName, deletedepartment);
            this.Apis.Add(updatedepartment.FunctionName, updatedepartment);
            this.Apis.Add(querydepartment.FunctionName, querydepartment);
            _MastLogin = false;
            this.Apis.Add(FGetdepartmentByEmpLevel.FunctionName, FGetdepartmentByEmpLevel);
        }
       /// <summary>
       /// 獲取所有部門
       /// </summary>
       /// <param name="requestValue"></param>
       /// <param name="Data"></param>
       /// <param name="StationReturn"></param>
        public void GetDepartmentList(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            List<String> DetpartmentList = new List<String>();
            T_C_DEPARTMENT detpartment ;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                detpartment = new T_C_DEPARTMENT(sfcdb, DBTYPE);
                DetpartmentList = detpartment.GetDepartment(sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.Message = "MES00000001";
                StationReturn.Data = DetpartmentList;
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        /// <summary>
        /// 根據用戶等級獲取 部門信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetDepartmentListByEmpLevel(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string EmpLevel = string.Empty;
            List<String> DetpartmentList = new List<String>();
            T_C_DEPARTMENT detpartment;
            T_c_user DptName;
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                detpartment = new T_C_DEPARTMENT(sfcdb, DBTYPE);
                DptName = new T_c_user(sfcdb, DBTYPE);
                if (LoginUser.EMP_LEVEL != "9" && LoginUser.EMP_LEVEL != "1")
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.Message = "MES00000028";
                    StationReturn.Data = "";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    if (LoginUser.EMP_LEVEL == "9")
                    {
                        DetpartmentList = detpartment.GetDepartment(sfcdb);
                    }
                    else if (LoginUser.EMP_LEVEL == "1")
                    {
                        DetpartmentList = DptName.GetDptName(LoginUser.EMP_NO, sfcdb);
                    }
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.Message = "MES00000001";
                    StationReturn.Data = DetpartmentList;
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }

        public void AddDepartment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string InsertSql = "";
            T_C_DEPARTMENT department;
            string DepartmentName = Data["DepartmentName"].ToString().Trim();
            string Desc = Data["Desc"].ToString().Trim();
            double SeqNo =Convert.ToDouble(Data["SeqNo"]);
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                department = new T_C_DEPARTMENT(sfcdb, DBTYPE);
                if (department.CheckDataExist(DepartmentName, sfcdb))
                {
                    Row_C_DEPARTMENT row = (Row_C_DEPARTMENT)department.NewRow();
                    row.ID = department.GetNewID(BU, sfcdb);
                    row.DEPARTMENT_NAME = DepartmentName;
                    row.DESCRIPTION = Desc;
                    row.SEQ_NO = SeqNo;
                    row.EDIT_EMP = LoginUser.EMP_NO;
                    row.EDIT_TIME = GetDBDateTime();
                    InsertSql = row.GetInsertString(DBTYPE);
                    sfcdb.ExecSQL(InsertSql);
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000002";
                    this.DBPools["SFCDB"].Return(sfcdb);
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000005";
                }
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;

            }

        }

        public void DeleteDepartment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string DeleteSql = "";
            string StrID = "";
            T_C_DEPARTMENT department;
            //   string[] ID = Newtonsoft.Json.Linq.JArray(Data["ID"].);
            Newtonsoft.Json.Linq.JArray ID = (Newtonsoft.Json.Linq.JArray)Data["ID"];
            try
            {

                sfcdb = this.DBPools["SFCDB"].Borrow();
                sfcdb.BeginTrain();
                department = new T_C_DEPARTMENT(sfcdb, DBTYPE);
                for (int i = 0; i < ID.Count; i++)
                {
                    StrID = ID[i].ToString();
                    Row_C_DEPARTMENT row = (Row_C_DEPARTMENT)department.GetObjByID(StrID, sfcdb);
                    DeleteSql = row.GetDeleteString(DBTYPE);
                    sfcdb.ExecSQL(DeleteSql);
                }
                sfcdb.CommitTrain();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000004";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        /// <summary>
        /// 更新標簽顯示語言數據
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void UpdateDepartment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            string UpdateSql = "";
            T_C_DEPARTMENT department;
            string ID = Data["ID"].ToString().Trim();
            string DepartmentName = Data["DepartmentName"].ToString().Trim();
            string Desc = Data["Desc"].ToString().Trim();
            double SeqNo = Convert.ToDouble(Data["SeqNo"]);
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                department = new T_C_DEPARTMENT(sfcdb, DBTYPE);
                Row_C_DEPARTMENT row = (Row_C_DEPARTMENT)department.GetObjByID(ID, sfcdb);
                row.ID = ID;
                row.DEPARTMENT_NAME = DepartmentName;
                row.DESCRIPTION = Desc;
                row.SEQ_NO = SeqNo;
                row.DESCRIPTION = Desc;
                row.EDIT_EMP = LoginUser.EMP_NO;
                row.EDIT_TIME = GetDBDateTime();

                UpdateSql = row.GetUpdateString(DBTYPE);
                sfcdb.ExecSQL(UpdateSql);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000003";
                this.DBPools["SFCDB"].Return(sfcdb);
            }
            catch (Exception e)
            {
                this.DBPools["SFCDB"].Return(sfcdb);
                throw e;
            }

        }
        /// <summary>
        /// 查詢標簽語言
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void QueryDepartment(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec sfcdb = null;
            T_C_DEPARTMENT department;
            List<C_DEPARTMENT> InputList;
            string ID = Data["ID"].ToString().Trim();
            string DepartmentName = Data["DepartmentName"].ToString().Trim();
            try
            {
                sfcdb = this.DBPools["SFCDB"].Borrow();
                department = new T_C_DEPARTMENT(sfcdb, DBTYPE);
                InputList = department.QueryDepartment(ID, DepartmentName, sfcdb);
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000001";
                StationReturn.Data = InputList;
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
