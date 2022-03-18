using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_TEXT : DataObjectTable
    {
        public T_R_WO_TEXT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_TEXT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_TEXT);
            TableName = "R_WO_TEXT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckWoTextByWo(string Workorderno, bool Download_Auto, string ColumnName, OleExec DB, DB_TYPE_ENUM DBType)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;

            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    StrSql = $@"insert into H_WO_TEXT(ID,{ ColumnName }) ";
                    StrSql = StrSql + $@" select* from R_WO_TEXT where AUFNR = '{Workorderno}' ";
                    StrReturnMsg = DB.ExecSqlReturn(StrSql);

                    int.TryParse(StrReturnMsg, out n);
                    if (n > 1)
                    {
                        StrSql = $@" delete from R_WO_TEXT where AUFNR = '{Workorderno}' ";
                        StrReturnMsg = DB.ExecSQL(StrSql);

                        int.TryParse(StrReturnMsg, out n);
                        CheckFlag = false;
                    }

                }
            }

            return CheckFlag;
        }

        public bool CheckWoTextByWo(string Workorderno,string taskno, bool Download_Auto,OleExec DB)
        {
            bool CheckFlag = false;
            string StrSql = "";
            string StrReturnMsg = "";
            int n = 0;

            if (Download_Auto)
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' and LTXA1='{taskno}'";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    CheckFlag = true;
                }
            }
            else
            {
                StrSql = $@"select * from R_WO_TEXT where AUFNR='{Workorderno}' and LTXA1='{taskno}' ";
                DataTable dt = DB.ExecSelect(StrSql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    StrSql = $@"insert into H_WO_TEXT select* from R_WO_TEXT where AUFNR = '{Workorderno}' and LTXA1='{taskno}' ";
                    StrReturnMsg = DB.ExecSqlReturn(StrSql);

                    int.TryParse(StrReturnMsg, out n);
                    if (n > 1)
                    {
                        StrSql = $@" delete from R_WO_TEXT where AUFNR = '{Workorderno}' and LTXA1='{taskno}'";
                        StrReturnMsg = DB.ExecSQL(StrSql);

                        int.TryParse(StrReturnMsg, out n);
                        CheckFlag = false;
                    }

                }
            }

            return CheckFlag;
        }
        public string EditWoText(string EditSql, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string ReturnMsg = DB.ExecSQL(EditSql);

            return ReturnMsg;
        }

        public string GetDeviationByWo(string Wo, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string WoDeviation = "";
            string Sql = "";
            DataTable Dt;
            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                Sql = $@" select * from r_wo_text where AUFNR='{Wo}' ";

                Dt = DB.ExecSelect(Sql, null).Tables[0];
                if (Dt.Rows.Count > 0)
                {
                    WoDeviation = Dt.Rows[0]["LTXA1"].ToString();
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return WoDeviation;
        }

        public R_WO_TEXT GetRWoText(string wo, OleExec db)
        {
            string strSql = $@"select * from R_WO_TEXT where AUFNR = '{ wo }' and substr(LTXA1,0,1) not in ('6','7','8')";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            R_WO_TEXT result = new R_WO_TEXT();
            if (table.Rows.Count > 0)
            {
                Row_R_WO_TEXT ret = (Row_R_WO_TEXT)this.NewRow();
                ret.loadData(table.Rows[0]);
                result = ret.GetDataObject();
            }
            else
            {
                result = null;
            }
            return result;
        }
    }
    public class Row_R_WO_TEXT : DataObjectBase
    {
        public Row_R_WO_TEXT(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_TEXT GetDataObject()
        {
            R_WO_TEXT DataObject = new R_WO_TEXT();
            DataObject.ID = this.ID;
            DataObject.AUFNR = this.AUFNR;
            DataObject.MATNR = this.MATNR;
            DataObject.ARBPL = this.ARBPL;
            DataObject.LTXA1 = this.LTXA1;
            DataObject.ISAVD = this.ISAVD;
            DataObject.VORNR = this.VORNR;
            DataObject.MGVRG = this.MGVRG;
            DataObject.LMNGA = this.LMNGA;
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
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string ARBPL
        {
            get
            {
                return (string)this["ARBPL"];
            }
            set
            {
                this["ARBPL"] = value;
            }
        }
        public string LTXA1
        {
            get
            {
                return (string)this["LTXA1"];
            }
            set
            {
                this["LTXA1"] = value;
            }
        }
        public string ISAVD
        {
            get
            {
                return (string)this["ISAVD"];
            }
            set
            {
                this["ISAVD"] = value;
            }
        }
        public string VORNR
        {
            get
            {
                return (string)this["VORNR"];
            }
            set
            {
                this["VORNR"] = value;
            }
        }
        public string MGVRG
        {
            get
            {
                return (string)this["MGVRG"];
            }
            set
            {
                this["MGVRG"] = value;
            }
        }
        public string LMNGA
        {
            get
            {
                return (string)this["LMNGA"];
            }
            set
            {
                this["LMNGA"] = value;
            }
        }
    }
    public class R_WO_TEXT
    {
        public string ID{get;set;}
        public string AUFNR{get;set;}
        public string MATNR{get;set;}
        public string ARBPL{get;set;}
        public string LTXA1{get;set;}
        public string ISAVD{get;set;}
        public string VORNR{get;set;}
        public string MGVRG{get;set;}
        public string LMNGA{get;set;}
    }
}