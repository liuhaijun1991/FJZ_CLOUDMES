using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;

namespace MESDataObject.Module
{
    public class T_R_TESTRECORD : DataObjectTable
    {
        public T_R_TESTRECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TESTRECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TESTRECORD);
            TableName = "R_TESTRECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_TESTRECORD GetFirstRecordBySNAndStation(string station, string sn, OleExec db)
        {
            string strSql = $@" select * from R_TESTRECORD where station_name in ('{station}') and sn in ('{sn}') order by test_time desc";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            R_TESTRECORD result = new R_TESTRECORD();
            if (table.Rows.Count > 0)
            {
                Row_R_TESTRECORD ret = (Row_R_TESTRECORD)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="sn"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetBySNActionCode(string sn, OleExec DB)
        {
            bool res = false;
            string strSQL = $@"SELECT * FROM R_TESTRECORD where SN='{sn}' and Station_name='ICT' and Status='P'   
 and EDIT_TIME>
 (select max(REPAIR_TIME) from R_REPAIR_ACTION where SN='{sn}'
 and ACTION_CODE IN (SELECT CONTROL_VALUE FROM C_CONTROL WHERE CONTROL_NAME='CHECK_ICT_RETEST_CODE')) 
UNION  
 SELECT * FROM R_TESTRECORD where sn='{sn}' and Station_name='ICT' and status='P'   
 and EDIT_TIME>
 (select max(EDIT_TIME) from R_REPAIR_OFFLINE where sn='{sn}'   
 and ACTION_CODE IN (SELECT CONTROL_VALUE FROM C_CONTROL WHERE CONTROL_NAME='CHECK_ICT_RETEST_CODE'))AND ROWNUM=1";
            DataTable Dt = DB.ExecSelect(strSQL).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }
        /// <summary>
        ///  WZW 檢查從T_C_CONTROL表中取出的測試工站是否有測試
        /// </summary>
        /// <param name="_panel"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetControlTestStation(string Station, OleExec DB)
        {
            bool res = false;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                List<R_TEST_RECORD> list = new List<R_TEST_RECORD>();
                //string strSQL = $@"select * from R_TESTRECORD where STATION_NAME='{Station}' and STATUS='P' order by EDIT_TIME desc";
                string strSQL = $@"select NVL(STATUS,'') as STATUS from R_TESTRECORD where STATION_NAME='{Station}' order by EDIT_TIME desc";
                //Station.Replace("'", "''")
                DataTable dt = new DataTable();
                dt = DB.ExecSelect(strSQL).Tables[0];
                //list = DataTableToList<R_TEST_RECORD>(dt);
                //foreach (DataRow item in dt.Rows)
                //{
                //    Row_R_TEST_RECORD ret = (Row_R_TEST_RECORD)NewRow();
                //    ret.loadData(item);
                //    list.Add(ret.GetDataObject());
                //}
                //return list;
                if (dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["STATUS"].ToString() == "P")
                    {
                        res = true;
                    }
                }
                return res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            // 定义集合  
            List<T> ts = new List<T>();
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                                       //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType.FullName.Contains("System.Nullable`1[[System.Double"))
                            {
                                pi.SetValue(t, value == null ? 0 : double.Parse(value.ToString()));
                            }
                            else if (pi.PropertyType.FullName.Contains("System.Nullable`1[[System.Int"))
                            {
                                pi.SetValue(t, value == null ? 0 : Convert.ToInt32(value));
                            }
                            else
                            {
                                pi.SetValue(t, value, null);
                            }
                        }
                        //pi.SetValue(t, value);
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }
            return ts;
        }
        public R_TESTRECORD GetFirstRecordBySN(string sn, OleExec db, string otherSql = "")
        {
            string strSql = $@" select * from R_TESTRECORD where sn='{sn}' {otherSql} order by test_time desc";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            R_TESTRECORD result = new R_TESTRECORD();
            if (table.Rows.Count > 0)
            {
                Row_R_TESTRECORD ret = (Row_R_TESTRECORD)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="Station"></param>
        /// <param name="Status"></param>
        /// <param name="TESTDATE"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_TESTRECORD> GetListBYSNStationStatusDateTime(string SN, List<string> Station, string Status, DateTime? TESTDATE, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && Station.Contains(t.STATION_NAME) && t.STATUS == Status && t.TEST_TIME > TESTDATE).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNNOStationPStatus(string SN, string Station, string Status, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && t.STATION_NAME != Station && t.STATUS == Status).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNStationStatus(string SN, string Station, string Status, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && t.STATION_NAME == Station && t.STATUS == Status).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNNOStationFStatus(string SN, string Station, string Status, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && !t.STATION_NAME.Contains(Station)/*t.STATION_NAME != Station*/ && t.STATUS == Status).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNNOStationFListStatus(string SN, string Station, List<string> Status, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && t.STATION_NAME != Station && Status.Contains(t.STATUS)).OrderBy(t => t.TEST_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNNOStationFListStatusNO(string SN, string Station, List<string> Status, OleExec DB)
        {
            List<R_TESTRECORD> ListRTestRecord = new List<R_TESTRECORD>();
            R_TESTRECORD RTestRecord = DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN).OrderBy(t => t.TEST_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (RTestRecord == null)
            {
                //ListRTestRecord.TEST_TIME = null;
                return ListRTestRecord;
            }
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && t.STATION_NAME != Station && Status.Contains(t.STATUS) && t.TEST_TIME == RTestRecord.TEST_TIME).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNNOStationStatusBCONVERT(string SN, string Station, string Status, string Bconvert, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && t.STATION_NAME != Station && t.STATUS == Status && t.REPAIR_STATUS == Bconvert).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYSN(List<string> SN, OleExec DB)
        {
            //Modify by LLF 2019-04-28,取最後一筆測試資料
            //return DB.ORM.Queryable<R_TESTRECORD>().Where(t => SN.Contains(t.SN)).ToList();
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => SN.Contains(t.SN)).OrderBy(t => t.TEST_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNStationPStatus(string SN, List<string> Station, string Status, OleExec DB)
        {
            //Modify By LLF 2019-04-28,不包含ICT,PCBFA
            //return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && Station.Contains(t.STATION_NAME) && t.STATUS == Status).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && !Station.Contains(t.STATION_NAME) && t.STATUS == Status).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetListBYRepairSN(string SN, string Category, string Status, OleExec DB)
        {
            //return DB.ORM.Queryable<R_TESTRECORD, C_CONTROL>((p1, p2) => p1.CUSTPARTNO == p2.CONTROL_VALUE).Where((p1, p2) => p1.SN == SN && p2.CONTROL_NAME == Category).ToList();
            return DB.ORM.Queryable<R_TESTRECORD, C_CONTROL>((p1, p2) => p1.CUSTPARTNO == p2.CONTROL_VALUE && p1.SN == SN && p2.CONTROL_NAME == Category && p1.STATUS == Status).Select((p1, p2) => p1).ToList();
        }
        public List<R_TESTRECORD> GetListBYSNDesc(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => SN.Contains(t.SN)).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public int UpdateSNINFO(string SN, OleExec DB)
        {
            int UpdateSNINFONum = 0;
            R_TESTRECORD ListSNOrderdEITtIMEDESC = DB.ORM.Queryable<R_TESTRECORD>().Where(t => SN.Contains(t.SN)).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (ListSNOrderdEITtIMEDESC == null)
            {
                return UpdateSNINFONum;
            }
            return DB.ORM.Updateable<R_TESTRECORD>().UpdateColumns(t => new R_TESTRECORD { REPAIR_STATUS = "1" }).Where(t => t.SN == SN && t.EDIT_TIME == ListSNOrderdEITtIMEDESC.EDIT_TIME).ExecuteCommand();
        }
        public int SRTestDataInsert(R_TESTRECORD TestData, OleExec DB)
        {
            return DB.ORM.Insertable(TestData).ExecuteCommand();
        }
        public R_TESTRECORD GetLastTestRecord(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN).OrderBy(t => t.TEST_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }
        public List<R_TESTRECORD> GetTestAction(string SN, OleExec db)
        {
            string strSql = $@"SELECT * FROM R_TESTRECORD WHERE SN='{SN}' AND STATUS='F' AND STATION_NAME NOT IN ('XRAY')        
   AND NOT EXISTS(SELECT * FROM R_REPAIR_ACTION B WHERE R_TESTRECORD.SN=B.SN        
   AND R_TESTRECORD.TEST_TIME+NUMTODSINTERVAL(SFC.FUN_DATEDIFF(TO_CHAR(R_TESTRECORD.TEST_TIME,'YYYY-MM-DD')),'hour')=B.FAIL_TIME)";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            Row_R_TESTRECORD result;
            List<R_TESTRECORD> TESTRECORD = new List<R_TESTRECORD>();
            try
            {
                table = db.ExecSelect(strSql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    result = (Row_R_TESTRECORD)this.NewRow();
                    result.loadData(dr);
                    TESTRECORD.Add(result.GetDataObject());
                }
            }
            catch (Exception ex)
            {
                //MES00000037
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
            return TESTRECORD;
        }
        public List<R_TESTRECORD> GetListBYSN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN).ToList();
        }
        public List<R_TESTRECORD> GetSNStatusBYSN(string SN, List<string> ListStatus, OleExec DB)
        {
            List<R_TESTRECORD> ListRTestRecordSNTestOrderDESC = DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN).OrderBy(t => t.TEST_TIME, SqlSugar.OrderByType.Desc).ToList();
            return DB.ORM.Queryable<R_TESTRECORD>().Where(t => t.SN == SN && ListStatus.Contains(t.STATUS) && t.EDIT_TIME == ListRTestRecordSNTestOrderDESC[0].EDIT_TIME).OrderBy(t => t.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
        public List<R_TESTRECORD> GetSNBYAuto(string SN, OleExec db)
        {
            string strSql = $@"SELECT * FROM R_TESTRECORD WHERE SN = '{SN}' AND STATUS = 'F' AND CUSTPARTNO IN (SELECT CONTROL_VALUE FROM C_CONTROL WHERE CONTROL_NAME = 'ENCLOSURE' )";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            Row_R_TESTRECORD result;
            List<R_TESTRECORD> TESTRECORD = new List<R_TESTRECORD>();
            try
            {
                table = db.ExecSelect(strSql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    result = (Row_R_TESTRECORD)this.NewRow();
                    result.loadData(dr);
                    TESTRECORD.Add(result.GetDataObject());
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
            return TESTRECORD;
        }
        public List<R_TESTRECORD> GetRepairNotA(string SN, OleExec db)
        {
            string strSql = $@"SELECT * FROM R_TESTRECORD  WHERE SN='{SN}' AND STATUS IN ('F','A','S') AND TEST_TIME=(SELECT MAX(TEST_TIME) FROM R_TESTRECORD WHERE SN='{SN}') ";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            Row_R_TESTRECORD result;
            List<R_TESTRECORD> TESTRECORD = new List<R_TESTRECORD>();
            try
            {
                table = db.ExecSelect(strSql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    result = (Row_R_TESTRECORD)this.NewRow();
                    result.loadData(dr);
                    TESTRECORD.Add(result.GetDataObject());
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
            return TESTRECORD;
        }
        public List<R_TESTRECORD> GetSNStatusBYSN(string SN, OleExec db)
        {
            string strSql = $@"SELECT * FROM R_TESTRECORD WHERE SN = '{SN}' AND STATUS = 'P'      
    AND TEST_TIME = (SELECT MAX(TEST_TIME) FROM R_TESTRECORD  WHERE SN = '{SN}')      ";
            DataTable table = db.ExecSelect(strSql).Tables[0];
            Row_R_TESTRECORD result;
            List<R_TESTRECORD> TESTRECORD = new List<R_TESTRECORD>();
            try
            {
                table = db.ExecSelect(strSql).Tables[0];
                foreach (DataRow dr in table.Rows)
                {
                    result = (Row_R_TESTRECORD)this.NewRow();
                    result.loadData(dr);
                    TESTRECORD.Add(result.GetDataObject());
                }
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
            return TESTRECORD;
        }
        public bool LH_NSDI_Check_TestFileCheck(string SN, ref List<R_TESTRECORD> ListRTestRecord1, MESDBHelper.OleExec SFCDB)
        {
            bool TestFile = true;
            if (SN.Substring(0, 3) == "FOC")
            {
                TestFile = false;
            }
            else
            {
                T_R_TESTRECORD RTestRecord = new T_R_TESTRECORD(SFCDB, DB_TYPE_ENUM.Oracle);
                List<R_TESTRECORD> ListRTestRecord = RTestRecord.GetListBYRepairSN(SN, "ENCLOSURE", "F", SFCDB);
                if (ListRTestRecord.Count <= 0)
                {
                    TestFile = false;
                }
                else
                {
                    ListRTestRecord1 = ListRTestRecord;
                }
            }
            return TestFile;
        }
    }
    public class Row_R_TESTRECORD : DataObjectBase
    {
        public Row_R_TESTRECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_TESTRECORD GetDataObject()
        {
            R_TESTRECORD DataObject = new R_TESTRECORD();
            DataObject.REPAIR_STATUS = this.REPAIR_STATUS;
            DataObject.ID = this.ID;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.TEST_TIME = this.TEST_TIME;
            DataObject.START_TIME = this.START_TIME;
            DataObject.END_TIME = this.END_TIME;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.MES_STATION = this.MES_STATION;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.STATUS = this.STATUS;
            DataObject.DEVICE = this.DEVICE;
            DataObject.TEST_INFO = this.TEST_INFO;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string REPAIR_STATUS
        {
            get
            {
                return (string)this["REPAIR_STATUS"];
            }
            set
            {
                this["REPAIR_STATUS"] = value;
            }
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public DateTime? TEST_TIME
        {
            get
            {
                return (DateTime?)this["TEST_TIME"];
            }
            set
            {
                this["TEST_TIME"] = value;
            }
        }
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public DateTime? END_TIME
        {
            get
            {
                return (DateTime?)this["END_TIME"];
            }
            set
            {
                this["END_TIME"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string MES_STATION
        {
            get
            {
                return (string)this["MES_STATION"];
            }
            set
            {
                this["MES_STATION"] = value;
            }
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string DEVICE
        {
            get
            {
                return (string)this["DEVICE"];
            }
            set
            {
                this["DEVICE"] = value;
            }
        }
        public string TEST_INFO
        {
            get
            {
                return (string)this["TEST_INFO"];
            }
            set
            {
                this["TEST_INFO"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_TESTRECORD
    {
        public string REPAIR_STATUS { get; set; }
        public string ID { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public DateTime? TEST_TIME { get; set; }
        public DateTime? START_TIME { get; set; }
        public DateTime? END_TIME { get; set; }
        public string STATION_NAME { get; set; }
        public string MES_STATION { get; set; }
        public string CUSTPARTNO { get; set; }
        public string STATUS { get; set; }
        public string DEVICE { get; set; }
        public string TEST_INFO { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}