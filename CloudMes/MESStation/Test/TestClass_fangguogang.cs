using MESDataObject;
using MESDBHelper;
using MESStation.BaseClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MESDataObject.Module;
using System.Data;
using MESDataObject.Common;

namespace MESStation.Test
{
    public class TestClass_fangguogang : MESStation.BaseClass.MesAPIBase
    {
        protected APIInfo FTest01 = new APIInfo()
        {
            FunctionName = "Test01",
            Description = "方國剛測試專用頁",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "SEQ_NO", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "WORK_SECTION", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "START_TIME", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "WORK_CLASS", InputType = "string", DefaultValue = "" },
                //new APIInputInfo() {InputName = "DAY_DISTINCT", InputType = "string", DefaultValue = "" }               
            },
            Permissions = new List<MESPermission>() { }
        };       
        public TestClass_fangguogang()
        {
            this.Apis.Add(FTest01.FunctionName, FTest01);            
        }

        public void Test01(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //OleExec sfcdb= this.DBPools["SFCDB"].Borrow();
            //T_C_BU bu = new T_C_BU(sfcdb, DBTYPE);
            //List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
            //list = bu.GetBUDetail(sfcdb,Data["BU"].ToString());
            //StationReturn.Status = StationReturnStatusValue.Pass;
            //StationReturn.Message = "OK";
            //StationReturn.Data = list;
            //DataTable dt = new DataTable();
            //var temp= ConvertToJson.DataTableToJson(dt);
            //OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            //DataObjectTable c_route_config = new DataObjectTable("c_class_time".ToUpper(), sfcdb, DB_TYPE_ENUM.Oracle);
            //DataObjectBase row = c_route_config.NewRow();

            //T_C_BU bu = new T_C_BU(sfcdb, DB_TYPE_ENUM.Oracle);
            //Row_C_BU row = (Row_C_BU)classTime.NewRow();
            //row = (Row_C_BU)classTime.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
            //var temp = bu.GetBUDetail(sfcdb, "");
            //StationReturn.Status = "PASS";
            //StationReturn.Message = "OK";
            //StationReturn.Data = temp;
            //try
            //{
            //    OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            //    DataObjectInfo Table_Info = new DataObjectInfo();
            //    Table_Info = DataObjectTable.GetDataObjectInfo("c_class_time".ToUpper(), sfcdb, DB_TYPE_ENUM.Oracle);
            //    foreach (DataObjectColInfo dataInfo in Table_Info.BaseColsInfo)
            //    {
            //        if (dataInfo.name == "WORK_SECTION" && Data["WORK_SECTION"].ToString().Length > dataInfo.length)
            //        {
            //            throw new Exception("輸入的"+ dataInfo.name+"的值過長！");
            //        }
            //    }

            //    this.DBPools["SFCDB"].Return(sfcdb);               
            //}
            //catch (Exception e)
            //{
            //    throw new Exception(e.Message);

            //}
        }


        public void Test02(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {

            //DateTime sysdate = Convert.ToDateTime("05:00:00");
            //DateTime start_time = Convert.ToDateTime("08:00:00");
            //DateTime end_time = Convert.ToDateTime("19:59:59");
            //int i = DateTime.Compare(sysdate, start_time);
            //int ii = DateTime.Compare(sysdate, end_time);
            //string sql = $@"select distinct name from c_work_class  where start_time < to_char (sysdate, 'HH24:MI:SS')  
            //             and end_time >= to_char (sysdate, 'HH24:MI:SS')";
            //DataSet dsClassTime = DB.ExecSelect(sql);

            //if (dsClassTime.Tables[0].Rows.Count > 0)
            //{
            //    return dsClassTime.Tables[0].Rows[0]["name"].ToString();
            //}
            //else
            //{
            //    throw new Exception("The Work Class  Can't Config");
            //}
            //OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            //T_C_CLASS_TIME classTime = new T_C_CLASS_TIME(sfcdb, DB_TYPE_ENUM.Oracle);
            //DataTable dt = classTime.GetShiftInfo(null, sfcdb);
            //List<Row_C_CLASS_TIME.C_CLASS_TIME> classTimeList = new List<Row_C_CLASS_TIME.C_CLASS_TIME>();
            //Row_C_CLASS_TIME BURow;
            //Row_C_CLASS_TIME.C_CLASS_TIME sTimeList = new Row_C_CLASS_TIME.C_CLASS_TIME();
            //foreach (DataRow row in dt.Rows)
            //{
            //    BURow = (Row_C_CLASS_TIME)classTime.NewRow();
            //    BURow.loadData(row);
            //    classTimeList.Add(BURow.GetDataObject());
            //}

            //Row_C_CLASS_TIME r = (Row_C_CLASS_TIME)classTime.NewRow();

            //r = (Row_C_CLASS_TIME)classTime.GetObjByID(Data["ID"].ToString(), sfcdb, DB_TYPE_ENUM.Oracle);
            //r.SEQ_NO = (double)Data["SEQ_NO"];
            //r.EDIT_EMP = this.LoginUser;
            //r.EDIT_TIME = DateTime.Now;
            //string strRet = sfcdb.ExecSQL(r.GetUpdateString(DB_TYPE_ENUM.Oracle));
            //sfcdb.CommitTrain();
            // StationReturn.Data = classTimeList; 


            #region  獲取班別
            //DateTime sysdate;
            //DateTime start_time;
            //DateTime end_time;
            //C_WORK_CLASS workClass = new C_WORK_CLASS();
            //string sqlSysdate ="";            
            //string sql = $@"select * from c_work_class ";           
            //DataSet dsWorkClass = DB.ExecSelect(sql);
            //if (string.IsNullOrEmpty(dateTime))
            //{
            //    sqlSysdate = $@"select to_char(sysdate,'hh24:mi:ss') from dual";
            //}
            //else
            //{
            //    sqlSysdate = $@"select to_char(to_date('{dateTime}','yyyy/mm/dd hh24:mi:ss'),'hh24:mi:ss') from dual";
            //}
            //DataSet dsSysdate = DB.ExecSelect(sqlSysdate);
            //try
            //{
            //    sysdate = Convert.ToDateTime(dsSysdate.Tables[0].Rows[0][0].ToString());
            //    foreach (DataRow row in dsWorkClass.Tables[0].Rows)
            //    {
            //        start_time = Convert.ToDateTime(row["START_TIME"].ToString());
            //        end_time = Convert.ToDateTime(row["End_TIME"].ToString());
            //        if ((DateTime.Compare(sysdate, start_time) >= 0) && (DateTime.Compare(sysdate, end_time) <= 0))
            //        {
            //            workClass.ID = row["ID"].ToString();
            //            workClass.NAME = row["NAME"].ToString();
            //            workClass.START_TIME = row["START_TIME"].ToString();
            //            workClass.END_TIME = row["End_TIME"].ToString();
            //        }
            //    }
            //    return workClass;
            //}
            //catch(Exception ex)
            //{
            //    throw ex;
            //}

            //DataSet dsSysdate;
            //string sql = " ";
            //if (string.IsNullOrEmpty(dateTime))
            //{
            //    sql = $@"select distinct name from c_work_class  where start_time < to_char (sysdate, 'HH24:MI:SS')  
            //            and end_time >= to_char (sysdate, 'HH24:MI:SS')";
            //}
            //else
            //{
            //    sql = $@"select distinct name from c_work_class  where to_char (to_date('{dateTime}','yyyy/mm/dd hh24:mi:ss'),
            //          'HH24:MI:SS')>=start_time and  to_char (to_date('{dateTime}','yyyy/mm/dd hh24:mi:ss'), 'HH24:MI:SS')<=end_time ";
            //}
            //try
            //{
            //    dsSysdate = DB.ExecSelect(sql);
            //    return dsSysdate.Tables[0].Rows[0][0].ToString();
            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}

            //OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            //T_C_WORK_CLASS workClass = new T_C_WORK_CLASS(sfcdb, DB_TYPE_ENUM.Oracle);
            //string _workClass;
            //if (!string.IsNullOrEmpty(Data["DATETIME"].ToString()))
            //{
            //    _workClass = workClass.GetWorkClass(sfcdb, Data["DATETIME"].ToString().Trim());
            //}
            //else
            //{
            //    _workClass = workClass.GetWorkClass(sfcdb, "");
            //}
            //if (!string.IsNullOrEmpty(_workClass))
            //{
            //    StationReturn.Status = "PASS";
            //    StationReturn.Message = "OK";
            //    StationReturn.Data = _workClass;
            //}
            //else
            //{
            //    StationReturn.Status = "FAIL";
            //    StationReturn.Message = "FAIL";
            //    StationReturn.Data = "NO WORK CLASS";
            //}
            //this.DBPools["SFCDB"].Return(sfcdb);
            #endregion

        }

        public void Test03(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //string nowTime = DateTime.Now.ToString("HHmiss");
            //OleExec sfcdb = this.DBPools["SFCDB"].Borrow();      
            //T_C_BU BU = new T_C_BU(sfcdb, DB_TYPE_ENUM.Oracle);

            //List<Row_C_CLASS_TIME> classTimeRow = classTime.GetClassTimeList("WORK_CLASS", Data["SEQ_NO"].ToString(), sfcdb);
            //var temp = (from tt in classTimeRow where tt.ID == "1" select tt).ToList();
            //Row_C_CLASS_TIME temp= classTimeRow.Where(tt=>tt.)
            //Row_C_CLASS_TIME temp1 = (Row_C_CLASS_TIME)classTimeRow.Select(tt => tt.ID = "99");

            //GetClasstt<Row_C_CLASS_TIME>(temp[0]);
            //System.Web.Script.Serialization.JavaScriptSerializer JsonMaker = new System.Web.Script.Serialization.JavaScriptSerializer();
            //List<string> tempstr = new List<string>() {
            //    temp[0].ID,
            //    temp[0].SEQ_NO.ToString(),
            //    temp[0].WORK_CLASS,
            //    temp[0].WORK_SECTION,
            //    temp[0].START_TIME,
            //    temp[0].END_TIME,
            //    temp[0].DAY_DISTINCT
            //};
            //Dictionary<string, string> ttt=new Dictionary<string, string> {
            //    { "ID", temp[0].ID},
            //    { "SEQ_NO", temp[0].SEQ_NO.ToString()},
            //    { "WORK_CLASS", temp[0].WORK_CLASS},
            //    { "WORK_SECTION", temp[0].WORK_SECTION},
            //    { "START_TIME", temp[0].START_TIME},
            //    { "END_TIME", temp[0].END_TIME},
            //    { "DAY_DISTINCT", temp[0].DAY_DISTINCT},
            //};
            //Dictionary<string, string> tttt = new Dictionary<string, string>();
            //tttt.Add("ID", temp[0].ID);
            //tttt.Add("SEQ_NO", temp[0].SEQ_NO.ToString());
            //tttt.Add("WORK_CLASS", temp[0].WORK_CLASS);
            //tttt.Add("WORK_SECTION", temp[0].WORK_SECTION);
            //tttt.Add("START_TIME", temp[0].START_TIME);
            //tttt.Add("END_TIME", temp[0].END_TIME);
            //tttt.Add("DAY_DISTINCT", temp[0].DAY_DISTINCT);                  


            //StationReturn.Data = "";
            //StationReturn.Status = "PASS";
            //StationReturn.Message = "OK";
        }
        private void GetTableColumns()
        {
            //List<APIInputInfo> inputInfoList = new List<APIInputInfo>();
            //OleExec sfcdb = this.DBPools["SFCDB"].Borrow();
            //DataObjectTable c_route_config = new DataObjectTable("c_class_time".ToUpper(), sfcdb, DB_TYPE_ENUM.Oracle);
            //return inputInfoList;

            #region
            //string sql = $@"select * from c_work_class  where 1=1 ";
            //string tempSql = "";
            //if (parameters != null)
            //{
            //    foreach (KeyValuePair<string, string> para in parameters)
            //    {
            //        if (para.Value != "")
            //        {
            //            tempSql = tempSql + $@" and {para.Key} = '{para.Value}' ";
            //        }
            //    }
            //}
            //sql = sql + tempSql;
            //DataSet dsWorkClass = DB.ExecSelect(sql);
            //List<C_WORK_CLASS> workClassList = new List<C_WORK_CLASS>();
            //Row_C_WORK_CLASS workClassRow;
            //foreach (DataRow row in dsWorkClass.Tables[0].Rows)
            //{
            //    workClassRow = (Row_C_WORK_CLASS)this.NewRow();
            //    workClassRow.loadData(row);
            //    workClassList.Add(workClassRow.GetDataObject());
            //}
            //return workClassList;
            #endregion
        }

        private void GetClasstt<T>(T model) {
            //Type t = model.GetType();
            //PropertyInfo[] pi = t.GetProperties();
            //foreach (PropertyInfo pInfo in t.GetProperties())
            //{
            //    object value = pInfo.GetValue(t,null);
            //    string name = pInfo.Name;
            //}
            //foreach (PropertyInfo p in pi)
            //{
            //    string name = p.Name;
            //    object value = p.GetValue(model, null);                
            //}
        }

    }
}
