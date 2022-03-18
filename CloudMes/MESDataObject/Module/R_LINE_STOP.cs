using System;
using System.Collections.Generic;
using System.Data;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_line_stop : DataObjectTable
    {
        public T_R_line_stop(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_line_stop(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_line_stop);
            TableName = "R_line_stop".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// AOI 反向零件錯誤停線检查,lineName 的值通過配API的時候寫在傳參(Paras[1])的VLUE里，其中它的值表現形式為：'E62S08A','E62S04A',...,''
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="line"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckAOIReverseStop(string skuno, string line, string lineName, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"select * from R_line_stop where skuno = '{skuno}' and line = '{line}'  and corrected='Open' and stop_description like 'AOI%' and (stop_description like '%反向'  or stop_description like '%零件錯誤')";
            if (!lineName.Equals("''"))
            {
                StrSql += $@"and line not in ({lineName})";
            }

            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// AOI連續不良停線检查,lineName 的值通過配API的時候寫在傳參(Paras[1],Paras[2])的VLUE里，其中它的值表現形式為：'E62S08A','E62S04A',...,''
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="line"></param>
        /// <param name="DB"></param>
        /// <param name="res1"></param>
        /// <param name="res2"></param>
        public void CheckAOIMultiFailStop(string skuno, string line, string lineName1, string lineName2, OleExec DB, ref bool res1, ref bool res2)
        {
            string StrSql1 = "";
            string StrSql2 = "";
            DataTable Dt1 = new DataTable();
            DataTable Dt2 = new DataTable();
            StrSql1 = $@"select * from R_line_stop where skuno = '{skuno}' and line = '{line}' and  corrected = 'Open' and stop_description like 'AOI%'";
            StrSql2 = $@"select * from R_line_stop where skuno in(select rtrim(skuno73) from c_k_mapping where skuno800 = '{skuno}')  and line = '{line}'  and corrected = 'Open' and stop_description like 'AOI%'";
            if (!lineName1.Equals("''"))
            {
                StrSql1 += $@"and line not in ({lineName1})";
            }
            if (!lineName2.Equals("''"))
            {
                StrSql2 += $@"and line not in ({lineName2})";
            }

            Dt1 = DB.ExecSelect(StrSql1).Tables[0];
            Dt2 = DB.ExecSelect(StrSql2).Tables[0];
            if (Dt1.Rows.Count > 0)
            {
                res1 = true;
            }
            if (Dt2.Rows.Count > 0)
            {
                res2 = true;
            }
        }
        #region 方法信息集合
        //根據ID查
        public List<R_LINE_STOP> GetByid(string id, OleExec DB)
        {
            string strSql = $@"select * from R_LINE_STOP where id=:id order by EDIT_TIME desc,CORRECTED desc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_LINE_STOP> result = new List<R_LINE_STOP>();
            if (res.Rows.Count > 0)
            {
                Row_R_line_stop ret = (Row_R_line_stop)NewRow();
                ret.loadData(res.Rows[0]);
                result.Add(ret.GetDataObject());
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<R_LINE_STOP> GetBySKUNO(string SKUNO, OleExec DB)
        {
            string strSql = $@"select * from R_LINE_STOP where SKUNO=:SKUNO order by EDIT_TIME desc,CORRECTED desc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SKUNO", SKUNO) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_LINE_STOP> result = new List<R_LINE_STOP>();
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_line_stop ret = (Row_R_line_stop)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        //修改停线原因或解决方法
        public int GetUpdate(string STOP_CAUSE, string STOP_PROGRESS,string ID, string CONFIRM, string CONFIRM_EMP,string CORRECTED, OleExec DB)
        {    
            string strSql = $@"UPDATE R_LINE_STOP SET STOP_CAUSE='"+STOP_CAUSE+"',STOP_PROGRESS='"+STOP_PROGRESS+ "',CONFIRM='" + CONFIRM + "',CONFIRM_EMP='" + CONFIRM_EMP + "',CONFIRM_TIME=SYSDATE,CORRECTED='"+ CORRECTED + "' where ID ='" + ID+"'";
            //OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", ID) };
            //int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text);
            return result;          
        }
        //獲取查询的SN维修记录
        public List<R_LINE_STOP> GetSelect(string SN,string TIME,string SKUNO,string LINE, OleExec DB)
        {
            string TIME1 = Convert.ToDateTime(TIME).ToString("yyyy-MM-dd HH:mm:ss");
            string TIME2 = Convert.ToDateTime(TIME).AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss");
            string strsql = "select to_char(m.CREATE_TIME ,'yyyy-mm-dd') as WorkDay,m.Skuno,c.VERSION,m.SN,m.CREATE_TIME, "
            + " m.FAIL_STATION as Eventpoint,b.FAIL_CODE as FailSymptom,r.REASON_CODE as Rootcause,errcode.ENGLISH_DESCRIPTION as FailureCode, "
            + " R.ACTION_CODE,r.PROCESS,r.FAIL_LOCATION as Location,m.DISTRIBUTION_EMP as Repairby, "
            + " m.EDIT_EMP as lasteditby,m.EDIT_TIME as Repairdate,r.NEW_DATE_CODE as Componentcode,r.NEW_MFR_NAME as Vendorcode, "
            + " r.SECTION_ID as Datacode,r.LOT_CODE,r.SOLUTION,r.DESCRIPTION,r.PART_DESC as Partdes,m.WORKORDERNO,  "
            + " m.FAIL_LINE as Productionline,b.FAIL_LOCATION as Faillocation from r_repair_main m "
            + " left join R_WO_BASE A ON M.WORKORDERNO = A.WORKORDERNO "
            + " left  join (select SN,CREATE_TIME ,max(FAIL_CODE) as FAIL_CODE,max(FAIL_LOCATION) as FAIL_LOCATION from R_REPAIR_FAILCODE group by SN,CREATE_TIME ) B on m.SN = b.SN and m.CREATE_TIME = b.CREATE_TIME  "
            + " left join R_REPAIR_ACTION R on m.sn = R.sn and m.CREATE_TIME = R.REPAIR_TIME "
            + " left join (select distinct REASON_CODE, ENGLISH_DESCRIPTION from C_REASON_CODE )errcode ON errcode.REASON_CODE = R.REASON_CODE "
            + " left join C_sku C ON C.SKUNO = A.SKUNO  ";
            if (SN == "")
            {
                strsql = strsql + " where a.skuno='" + SKUNO + "' and m.sn in (SELECT SN FROM R_SN_LINE_STOP WHERE STOP_TYPE='AOISTOPSN' AND EDIT_TIME BETWEEN '" + TIME1 + "' AND '" + TIME2 + "' AND EDIT_EMP='" + LINE + "')";
            }
            else
            {
                strsql = strsql + " where a.skuno='" + SKUNO + "' and m.sn = '" + SN + "'";
            }
          
            List<R_LINE_STOP> result = new List<R_LINE_STOP>();
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_line_stop ret = (Row_R_line_stop)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        //獲取R_LINE_STOP所有信息
        public List<R_LINE_STOP> GetAllCodeName(OleExec DB)
        {
            //string strSql = $@"select * from R_LINE_STOP where rownum<=100 order by rownum asc";
            string strSql = $@"SELECT ID,SKUNO,CORRECTED,CHECK_DATE,LINE,STATION,CHECK_TIME,STOP_DESCRIPTION,DPPM,STOP_PROGRESS,EDIT_EMP,EDIT_TIME,STOP_CAUSE,CONFIRM,CONFIRM_EMP,CONFIRM_TIME FROM (SELECT * FROM R_LINE_STOP  ORDER BY CHECK_DATE DESC )WHERE ROWNUM<=100  ORDER BY CORRECTED DESC";

            List<R_LINE_STOP> result = new List<R_LINE_STOP>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_line_stop ret = (Row_R_line_stop)NewRow();
                    ret.loadData(res.Rows[i]);
                    result.Add(ret.GetDataObject());
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        public int ChangeByID(string Id, OleExec DB)
        {
            string strSql = $@"UPDATE R_LINE_STOP SET CORRECTED='Closed' where ID =:Id and CORRECTED ='Open'";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        #endregion 

        public R_LINE_STOP GetLineStopBySkunoLineOther(string skuno, string line, string other, OleExec db)
        {
            string strSql = $@" select * from r_line_stop where skuno = '{skuno}' and line = '{line}' {other} ";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            R_LINE_STOP result = new R_LINE_STOP();
            if (table.Rows.Count > 0)
            {
                Row_R_line_stop ret = (Row_R_line_stop)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }

        public string InsertOneRowLineStop(string bu, string skuno, string productionLine, string stationName, DateTime dbTime, string stopDesc, string stopProgress, double? dppm, string stopCause, string corrected, string confirm, DateTime? confirmTime, string confirmEmp, string empNo, OleExec db, DB_TYPE_ENUM dbtype)
        {
            Row_R_line_stop rowLineStop = (Row_R_line_stop)this.NewRow();
            rowLineStop.ID = this.GetNewID(bu, db);
            rowLineStop.SKUNO = skuno;
            rowLineStop.LINE = productionLine;
            rowLineStop.STATION = stationName;
            rowLineStop.CHECK_DATE = dbTime.ToShortDateString();
            rowLineStop.CHECK_TIME = dbTime.ToLongTimeString();
            //rowLineStop.CHECK_TIME = Convert.ToDateTime(string.Format("{0}:{1}:{2}",dbTime.Hour.ToString(),dbTime.Minute.ToString(),dbTime.Second.ToString()));
            rowLineStop.STOP_DESCRIPTION = stopDesc;
            rowLineStop.STOP_PROGRESS = stopProgress;
            rowLineStop.DPPM = dppm;
            rowLineStop.STOP_CAUSE = stopCause;
            rowLineStop.CORRECTED = corrected;
            rowLineStop.CONFIRM = confirm;
            rowLineStop.CONFIRM_EMP = confirmEmp;
            rowLineStop.CONFIRM_TIME = confirmTime;
            rowLineStop.EDIT_EMP = empNo;
            rowLineStop.EDIT_TIME = dbTime;
            return db.ExecSQL(rowLineStop.GetInsertString(dbtype));
        }
        //根據ID查

        /// <summary>
        /// 檢測是否有open的停線記錄
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="Productline"></param>
        /// <param name="ErrorStr"></param>
        /// <param name="Description_Flag"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int IsLineStopOpen(string Skuno, string Productline, string ErrorStr, int Description_Flag, OleExec DB)
        {
            string Stopsql = $@"select count(*) from R_LINE_STOP where SKUNO=:skuno and LINE=:productline and CORRECTED='Open' and STOP_DESCRIPTION like 'AOI%'";
            if (Description_Flag == 1)
            {
                Stopsql += "and STOP_DESCRIPTION=:stopdescription";
            }
            OleDbParameter[] paramet = new OleDbParameter[]
               {
                    new OleDbParameter(":skuno", Skuno),
                    new OleDbParameter(":productline", Productline),
                    new OleDbParameter(":stopdescription", ErrorStr)
               };

            int R_LINE_STOP_Result = int.Parse(DB.ExecuteScalar(Stopsql, CommandType.Text, paramet));
            return R_LINE_STOP_Result;
        }


        /// <summary>
        /// 檢測是否需要向R_LINE_STOP中插入一條錯誤記錄,並且插入
        /// ErrorStr為停線具體信息
        /// Description_Flag為1,則sql語句多一部份,匹配更詳細的記錄
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Productline"></param>
        /// <param name="Skuno"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void AOILineStop5FailIn20Pcs(string Station, string Productline, string Skuno, OleExec DB, string BU, DateTime dbTime, string emp_no, int Description_Flag, int CriticalValue)
        {

            //獲取過站記錄中FAIL的數量
            T_R_SN_STATION_DETAIL tSNStationTable = new T_R_SN_STATION_DETAIL(DB, DBType);
            int R_SN_STATION_DETAIL_Count = tSNStationTable.FailCountByStationAndSkunoAndLine(Station, Productline, Skuno, 1, DB);
            //獲取未處理的停線記錄
            string ErrorStr = "AOI:停線,在產品" + Skuno + "線別" + Productline + "下,20片中5片板子出現異常:";
            int R_LINE_STOP_Result = IsLineStopOpen(Skuno, Productline, ErrorStr, Description_Flag, DB);

            if (R_SN_STATION_DETAIL_Count > CriticalValue && R_LINE_STOP_Result == 0)
            {
                InsertOneRowLineStop(BU, Skuno, Productline, Station, dbTime, ErrorStr, "", 0, "", "Open", null, null, null, emp_no, DB, DB_TYPE_ENUM.Oracle);
            }


        }





    }
    public class Row_R_line_stop : DataObjectBase
    {
        public Row_R_line_stop(DataObjectInfo info) : base(info)
        {

        }
        public R_LINE_STOP GetDataObject()
        {
            R_LINE_STOP DataObject = new R_LINE_STOP();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LINE = this.LINE;
            DataObject.STATION = this.STATION;
            DataObject.CHECK_DATE = this.CHECK_DATE;
            DataObject.CHECK_TIME = this.CHECK_TIME;
            DataObject.STOP_DESCRIPTION = this.STOP_DESCRIPTION;
            DataObject.STOP_PROGRESS = this.STOP_PROGRESS;
            DataObject.DPPM = this.DPPM;
            DataObject.STOP_CAUSE = this.STOP_CAUSE;
            DataObject.CORRECTED = this.CORRECTED;
            DataObject.CONFIRM = this.CONFIRM;
            DataObject.CONFIRM_EMP = this.CONFIRM_EMP;
            DataObject.CONFIRM_TIME = this.CONFIRM_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string CHECK_DATE
        {
            get
            {
                return (string)this["CHECK_DATE"];
            }
            set
            {
                this["CHECK_DATE"] = value;
            }
        }
        public string CHECK_TIME
        {
            get
            {
                return (string)this["CHECK_TIME"];
            }
            set
            {
                this["CHECK_TIME"] = value;
            }
        }
        public string STOP_DESCRIPTION
        {
            get
            {
                return (string)this["STOP_DESCRIPTION"];
            }
            set
            {
                this["STOP_DESCRIPTION"] = value;
            }
        }
        public string STOP_PROGRESS
        {
            get
            {
                return (string)this["STOP_PROGRESS"];
            }
            set
            {
                this["STOP_PROGRESS"] = value;
            }
        }
        public double? DPPM
        {
            get
            {
                return (double?)this["DPPM"];
            }
            set
            {
                this["DPPM"] = value;
            }
        }
        public string STOP_CAUSE
        {
            get
            {
                return (string)this["STOP_CAUSE"];
            }
            set
            {
                this["STOP_CAUSE"] = value;
            }
        }
        public string CORRECTED
        {
            get
            {
                return (string)this["CORRECTED"];
            }
            set
            {
                this["CORRECTED"] = value;
            }
        }
        public string CONFIRM
        {
            get
            {
                return (string)this["CONFIRM"];
            }
            set
            {
                this["CONFIRM"] = value;
            }
        }
        public string CONFIRM_EMP
        {
            get
            {
                return (string)this["CONFIRM_EMP"];
            }
            set
            {
                this["CONFIRM_EMP"] = value;
            }
        }
        public DateTime? CONFIRM_TIME
        {
            get
            {
                return (DateTime?)this["CONFIRM_TIME"];
            }
            set
            {
                this["CONFIRM_TIME"] = value;
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
    public class R_LINE_STOP
    {
        public string ID;
        public string SKUNO;
        public string LINE;
        public string STATION;
        public string CHECK_DATE;
        public string CHECK_TIME;
        public string STOP_DESCRIPTION;
        public string STOP_PROGRESS;
        public double? DPPM;
        public string STOP_CAUSE;
        public string CORRECTED;
        public string CONFIRM;
        public string CONFIRM_EMP;
        public DateTime? CONFIRM_TIME;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}