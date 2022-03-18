using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_REPAIR_CHECK : DataObjectTable
    {
        public T_R_REPAIR_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_REPAIR_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_REPAIR_CHECK);
            TableName = "R_REPAIR_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skuno"></param>
        /// <param name="line"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public bool CheckAOIReverseStop(string skuno, string line, string lineName, OleExec DB)
        //{
        //    string StrSql = "";
        //    bool CheckFlag = false;
        //    DataTable Dt = new DataTable();
        //    StrSql = $@"select * from R_line_stop where skuno = '{skuno}' and line = '{line}' and line not in ('{lineName}') and corrected='Open' and stop_description like 'AOI%' and (stop_description like '%反向'  or stop_description like '%零件錯誤')";
        //    Dt = DB.ExecSelect(StrSql).Tables[0];
        //    if (Dt.Rows.Count > 0)
        //    {
        //        CheckFlag = true;
        //    }
        //    return CheckFlag;
        //}

        #region 方法信息集合
        //根據ID查
        public List<R_REPAIR_CHECK> GetByid(string id, OleExec DB)
        {
            string strSql = $@"select * from R_LINE_STOP where id=:id order by EDIT_TIME desc,CORRECTED desc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_REPAIR_CHECK> result = new List<R_REPAIR_CHECK>();
            if (res.Rows.Count > 0)
            {
                Row_R_REPAIR_CHECK ret = (Row_R_REPAIR_CHECK)NewRow();
                ret.loadData(res.Rows[0]);
                result.Add(ret.GetDataObject());
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<R_REPAIR_CHECK> GetBySN(string SN, OleExec DB)
        {
            string strSql = $@"select  A.sn,B.skuno,A.FAIL_LOCATION,A.EDIT_EMP,A.EDIT_TIME from R_REPAIR_ACTION A ,R_REPAIR_TRANSFER B WHERE A.SN=B.SN  order by EDIT_TIME desc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":SN", SN) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_REPAIR_CHECK> result = new List<R_REPAIR_CHECK>();
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_REPAIR_CHECK ret = (Row_R_REPAIR_CHECK)NewRow();
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
        //public List<R_REPAIR_CHECK> GetAllCodeName(OleExec DB)
        //{
        //    string strSql = $@"select * from R_LINE_STOP order by EDIT_TIME desc,CORRECTED desc";
        //    List<R_REPAIR_CHECK> result = new List<R_REPAIR_CHECK>();
        //    DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
        //    if (res.Rows.Count > 0)
        //    {
        //        for (int i = 0; i < res.Rows.Count; i++)
        //        {
        //            Row_R_REPAIR_CHECK ret = (Row_R_REPAIR_CHECK)NewRow();
        //            ret.loadData(res.Rows[i]);
        //            result.Add(ret.GetDataObject());
        //        }
        //        return result;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        /// <summary>
        /// 添加新的信息
        /// </summary>
        /// <param name="NewActionCode"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public int AddCODENAME(R_LINE_STOP NewCODE, OleExec DB)
        //{
        //    Row_R_line_stop NewCODENAME = (Row_R_line_stop)NewRow();
        //    NewCODENAME.ID = NewCODE.ID;
        //    NewCODENAME.SKUNO = NewCODE.SKUNO;
        //    NewCODENAME.EDIT_EMP = NewCODE.EDIT_EMP;
        //    NewCODENAME.EDIT_TIME = NewCODE.EDIT_TIME;
        //    int result = DB.ExecuteNonQuery(NewCODENAME.GetInsertString(DBType), CommandType.Text);
        //    return result;
        //}
        //public int UpdateBySKU(R_LINE_STOP NewCODE, OleExec DB)
        //{
        //    Row_R_line_stop NewVALIDFLAG = (Row_R_line_stop)NewRow();
        //    NewVALIDFLAG.ID = NewCODE.ID;
        //    NewVALIDFLAG.SKUNO = NewCODE.SKUNO;
        //    NewVALIDFLAG.CHECK_DATE = NewCODE.CHECK_DATE;
        //    NewVALIDFLAG.CHECK_TIME = NewCODE.CHECK_TIME;
        //    NewVALIDFLAG.EDIT_EMP = NewCODE.EDIT_EMP;
        //    NewVALIDFLAG.EDIT_TIME = NewCODE.EDIT_TIME;
        //    int result = DB.ExecuteNonQuery(NewVALIDFLAG.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
        //    return result;
        //}
        //public int UpdateById(R_LINE_STOP NewCODE, OleExec DB)
        //{
        //    Row_R_line_stop NewCODENAMERow = (Row_R_line_stop)NewRow();
        //    NewCODENAMERow.ID = NewCODE.ID;
        //    NewCODENAMERow.SKUNO = NewCODE.SKUNO;
        //    NewCODENAMERow.CHECK_DATE = NewCODE.CHECK_DATE;
        //    NewCODENAMERow.CHECK_TIME = NewCODE.CHECK_TIME;                
        //    NewCODENAMERow.EDIT_EMP = NewCODE.EDIT_EMP;
        //    NewCODENAMERow.EDIT_TIME = NewCODE.EDIT_TIME;            
        //    int result = DB.ExecuteNonQuery(NewCODENAMERow.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
        //    return result;
        //}
        //public int DeleteById(string Id, OleExec DB)
        //{
        //    string strSql = $@"DELETE R_LINE_STOP WHERE ID =:Id";
        //    OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
        //    int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        //    return result;
        //}
        public int InsertByID(string Id, OleExec DB)
        {
            string strSql = $@"Insert into '";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        #endregion 
        public int Addcheck(R_REPAIR_CHECK NewCODE, OleExec DB)
        {
            Row_R_REPAIR_CHECK NewCODENAME = (Row_R_REPAIR_CHECK)NewRow();
            NewCODENAME.ID = NewCODE.ID;
            NewCODENAME.SN = NewCODE.SN;
            NewCODENAME.SKUNO = NewCODE.SKUNO;
            NewCODENAME.FAIL_LOCATION = NewCODE.FAIL_LOCATION;
            NewCODENAME.RE_EMP = NewCODE.RE_EMP;
            NewCODENAME.EDIT_EMP = NewCODE.EDIT_EMP;
            NewCODENAME.EDIT_TIME = NewCODE.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewCODENAME.GetInsertString(DBType), CommandType.Text);
            return result;
        }


        //public R_LINE_STOP GetLineStopBySkunoLineOther(string skuno, string line, string other, OleExec db)
        //{
        //    string strSql = $@" select * from r_line_stop where skuno = '{skuno}' and line = '{line}' {other} ";
        //    OleDbParameter[] paramet = new OleDbParameter[1];
        //    //paramet[0] = new OleDbParameter(":control_name", controlName);
        //    DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
        //    DataTable table = db.ExecSelect(strSql).Tables[0];
        //    R_LINE_STOP result = new R_LINE_STOP();
        //    if (table.Rows.Count > 0)
        //    {
        //        Row_R_line_stop ret = (Row_R_line_stop)this.NewRow();
        //        ret.loadData(table.Rows[0]);
        //        result = ret.GetDataObject();
        //    }
        //    else
        //    {
        //        result = null;
        //    }
        //    return result;
        //}

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
        public List<R_REPAIR_CHECK> GetSNbyRepairFICheck(string SN, OleExec DB)
        {
            string StrSql = $@"SELECT * FROM R_REPAIR_CHECK WHERE SN ='{SN}' AND EDIT_TIME > (
SELECT IN_TIME FROM R_REPAIR_TRANSFER WHERE SN='{SN}') ";
            DataTable DT = DB.ExecuteDataTable(StrSql, CommandType.Text);
            List<R_REPAIR_CHECK> ListRRepairCheck = new List<R_REPAIR_CHECK>();
            if (DT.Rows.Count > 0)
            {
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    Row_R_REPAIR_CHECK RowRRepairCheck = (Row_R_REPAIR_CHECK)NewRow();
                    RowRRepairCheck.loadData(DT.Rows[i]);
                    ListRRepairCheck.Add(RowRRepairCheck.GetDataObject());
                }
            }
            return ListRRepairCheck;
        }

        public DataTable GetRepairVisualList(string SN, OleExec DB)
        {
            string strSql = $@"select*from 
                (select a.SN,b.SKUNO,A.FAIL_LOCATION as LOCATION,A.EDIT_EMP,A.EDIT_TIME from r_repair_action a,R_REPAIR_TRANSFER b 
                where a.sn=b.sn and upper(a.edit_emp)<>'SFCAUTOREPAIR' and not exists
                (select 1 from R_REPAIR_TRANSFER c where c.closed_flag='1' and a.sn=c.sn and a.fail_time=c.create_time
                and c.edit_time>=(select*from (select edit_time from r_repair_transfer d where d.sn='{SN}' order by edit_time desc) where rownum=1)
                )and a.SN ='{SN}'
                union
                select*from (select a.sn,b.skuno,a.location,a.repair_emp,a.edit_time From R_REPAIR_OFFLINE a, R_REPAIR_TRANSFER b where a.sn=b.sn 
                and not exists(select 1 from r_repair_transfer c where closed_flag='1' and a.sn=c.sn and a.edit_time>c.create_time)
                and a.sn not in(select sn from r_repair_check) and a.sn='{SN}') where rownum=1) where 1=1
                order by 1,5 desc";

            DataTable res = DB.ExecSelect(strSql).Tables[0];
            return res;
        }

        public int InsertRepairCheck(R_REPAIR_CHECK RepairCheck, OleExec DB)
        {
           int n=DB.ORM.Insertable<R_REPAIR_CHECK>(RepairCheck).ExecuteCommand();

           return n;
        }
    }
    public class Row_R_REPAIR_CHECK : DataObjectBase
    {
        public Row_R_REPAIR_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public R_REPAIR_CHECK GetDataObject()
        {
            R_REPAIR_CHECK DataObject = new R_REPAIR_CHECK();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FAIL_LOCATION = this.FAIL_LOCATION;
            DataObject.RE_EMP = this.RE_EMP;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string FAIL_LOCATION
        {
            get
            {
                return (string)this["FAIL_LOCATION"];
            }
            set
            {
                this["FAIL_LOCATION"] = value;
            }
        }
        public string RE_EMP
        {
            get
            {
                return (string)this["RE_EMP"];
            }
            set
            {
                this["RE_EMP"] = value;
            }
        }
        public string CHECK_EMP
        {
            get
            {
                return (string)this["CHECK_EMP"];
            }
            set
            {
                this["CHECK_EMP"] = value;
            }
        }
        public string URL
        {
            get
            {
                return (string)this["URL"];
            }
            set
            {
                this["URL"] = value;
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
    public class R_REPAIR_CHECK
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string FAIL_LOCATION { get; set; }
        public string RE_EMP { get; set; }
        public string CHECK_EMP { get; set; }
        public string URL { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}