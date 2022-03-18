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
    public class T_R_FIFO : DataObjectTable
    {
        public T_R_FIFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FIFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FIFO);
            TableName = "R_FIFO".ToUpper();
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
            //StrSql = $@"select * from R_FIFO where skuno = '{skuno}' and line = '{line}' and line not in ('{lineName}') and corrected='Open' and stop_description like 'AOI%' and (stop_description like '%反向'  or stop_description like '%零件錯誤')";
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
        //public void CheckAOIMultiFailStop(string skuno, string line, string lineName1, string lineName2, OleExec DB, ref bool res1, ref bool res2)
        //{
        //    string StrSql1 = "";
        //    string StrSql2 = "";
        //    DataTable Dt1 = new DataTable();
        //    DataTable Dt2 = new DataTable();
        //    StrSql1 = $@"select * from R_line_stop where skuno = '{skuno}' and line = '{line}' and line not in ('{lineName1}') and corrected = 'Open' and stop_description like 'AOI%'";
        //    StrSql2 = $@"select * from R_line_stop where skuno in(select rtrim(skuno73) from c_k_mapping where skuno800 = '{skuno}')  and line = '{line}' and line not in ('{lineName2}') and corrected = 'Open' and stop_description like 'AOI%'";
        //    Dt1 = DB.ExecSelect(StrSql1).Tables[0];
        //    Dt2 = DB.ExecSelect(StrSql2).Tables[0];
        //    if (Dt1.Rows.Count > 0)
        //    {
        //        res1 = true;
        //    }
        //    if (Dt2.Rows.Count > 0)
        //    {
        //        res2 = true;
        //    }
        //}
        #region 方法信息集合
        //根據ID查
        public List<R_FIFO> GetByid(string id, OleExec DB)
        {
            string strSql = $@"select * from R_FIFO where id=:id order by EDIT_TIME desc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":id", id) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_FIFO> result = new List<R_FIFO>();
            if (res.Rows.Count > 0)
            {
                Row_R_FIFO ret = (Row_R_FIFO)NewRow();
                ret.loadData(res.Rows[0]);
                result.Add(ret.GetDataObject());
                return result;
            }
            else
            {
                return null;
            }
        }
        public List<R_FIFO> GetByPALLET(string PALLET, OleExec DB)
        {
            string strSql = $@"select * from R_FIFO where PALLET=:PALLET order by EDIT_TIME desc";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":PALLET", PALLET) };
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text, paramet);
            List<R_FIFO> result = new List<R_FIFO>();
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_FIFO ret = (Row_R_FIFO)NewRow();
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
        public List<R_FIFO> GetAllCodeName(OleExec DB)
        {
            string strSql = $@"select * from R_FIFO where rownum<=100 order by EDIT_TIME desc";
            List<R_FIFO> result = new List<R_FIFO>();
            DataTable res = DB.ExecuteDataTable(strSql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_FIFO ret = (Row_R_FIFO)NewRow();
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

        /// <summary>
        /// 添加新的信息
        /// </summary>
        /// <param name="NewActionCode"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int AddCODENAME(R_FIFO NewCODE, OleExec DB)
        {
            Row_R_FIFO NewCODENAME = (Row_R_FIFO)NewRow();
            NewCODENAME.ID = NewCODE.ID;
            NewCODENAME.PALLET = NewCODE.PALLET;
            NewCODENAME.EDIT_EMP = NewCODE.EDIT_EMP;
            NewCODENAME.EDIT_TIME = NewCODE.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewCODENAME.GetInsertString(DBType), CommandType.Text);
            return result;
        }
        //public int UpdateBySKU(R_FIFO NewCODE, OleExec DB)
        //{
        //    Row_R_FIFO NewVALIDFLAG = (Row_R_FIFO)NewRow();
        //    NewVALIDFLAG.ID = NewCODE.ID;
        //    NewVALIDFLAG.PALLET = NewCODE.PALLET;
        //    NewVALIDFLAG.EDIT_EMP = NewCODE.EDIT_EMP;
        //    NewVALIDFLAG.EDIT_TIME = NewCODE.EDIT_TIME;
        //    int result = DB.ExecuteNonQuery(NewVALIDFLAG.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
        //    return result;
        //}
        public int UpdateById(R_FIFO NewCODE, OleExec DB)
        {
            Row_R_FIFO NewCODENAMERow = (Row_R_FIFO)NewRow();
            NewCODENAMERow.ID = NewCODE.ID;
            NewCODENAMERow.PALLET = NewCODE.PALLET;
            NewCODENAMERow.EDIT_EMP = NewCODE.EDIT_EMP;
            NewCODENAMERow.EDIT_TIME = NewCODE.EDIT_TIME;
            int result = DB.ExecuteNonQuery(NewCODENAMERow.GetUpdateString(DBType, NewCODE.ID), CommandType.Text);
            return result;
        }
        public int DeleteById(string Id, OleExec DB)
        {
            string strSql = $@"DELETE R_FIFO WHERE ID =:Id";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Id", Id) };
            int result = DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
            return result;
        }
        #endregion 

        //public R_FIFO GetLineStopBySkunoLineOther(string skuno, string line, string other, OleExec db)
        //{
        //    string strSql = $@" select * from R_FIFO where skuno = '{skuno}' and line = '{line}' {other} ";
        //    //OleDbParameter[] paramet = new OleDbParameter[1];
        //    ////paramet[0] = new OleDbParameter(":control_name", controlName);
        //    //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
        //    DataTable table = db.ExecSelect(strSql).Tables[0];
        //    R_FIFO result = new R_FIFO();
        //    if (table.Rows.Count > 0)
        //    {
        //        Row_R_FIFO ret = (Row_R_FIFO)this.NewRow();
        //        ret.loadData(table.Rows[0]);
        //        result = ret.GetDataObject();
        //    }
        //    else
        //    {
        //        result = null;
        //    }
        //    return result;
        //}

        public string InsertOneRowLineStop(string bu, string pallet, string productionLine, string stationName, DateTime dbTime, string stopDesc, string stopProgress, double? dppm, string stopCause, string corrected, string confirm, DateTime? confirmTime, string confirmEmp, string empNo, OleExec db, DB_TYPE_ENUM dbtype)
        {
            Row_R_FIFO rowLineStop = (Row_R_FIFO)this.NewRow();
            rowLineStop.ID = this.GetNewID(bu, db);
            rowLineStop.PALLET = pallet;
            rowLineStop.EDIT_EMP = empNo;
            rowLineStop.EDIT_TIME = dbTime;
            return db.ExecSQL(rowLineStop.GetInsertString(dbtype));
        }
    }
    public class Row_R_FIFO : DataObjectBase
    {
        public Row_R_FIFO(DataObjectInfo info) : base(info)
        {

        }
        public R_FIFO GetDataObject()
        {
            R_FIFO DataObject = new R_FIFO();
            DataObject.ID = this.ID;
            DataObject.PALLET = this.PALLET;
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
        public string PALLET
        {
            get
            {
                return (string)this["PALLET"];
            }
            set
            {
                this["PALLET"] = value;
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
    public class R_FIFO
    {
        public string ID;
        public string PALLET;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}