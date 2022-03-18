using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SIGN : DataObjectTable
    {
        public T_R_SIGN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SIGN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SIGN);
            TableName = "R_SIGN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }


        /// <summary>
        /// 檢查簽核批次號是否存在
        /// add by hgb 2019.06.04
        /// </summary>        
        /// <param name="LOT_NO">批次號</param>
        /// <param name="TYPE">類型,是ECN還是PLAN_lock</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSignExists( string LOT_NO, string TYPE, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_SIGN WHERE LOT_NO='{LOT_NO}' and TYPE='{TYPE}'   ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }


        /// <summary>
        /// 檢查部門(Columns)是否簽核
        /// add by hgb 2019.06.04
        /// </summary>
        /// <param name="Columns">部門,入PE,R_SING中的PE欄位</param>
        /// <param name="LOT_NO">批次號</param>
        /// <param name="TYPE">類型,是ECN還是PLAN_lock</param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckIsSign(string Columns, string LOT_NO, string TYPE, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"SELECT * FROM R_SIGN WHERE LOT_NO='{LOT_NO}' and TYPE='{TYPE}' and  length({Columns})>0    ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }
    }
    public class Row_R_SIGN : DataObjectBase
    {
        public Row_R_SIGN(DataObjectInfo info) : base(info)
        {

        } 

        public R_SIGN GetDataObject()
        {
            R_SIGN DataObject = new R_SIGN();
            DataObject.ID = this.ID;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.TYPE = this.TYPE;
            DataObject.PE = this.PE;
            DataObject.PE_SIGNTIME = this.PE_SIGNTIME;
            DataObject.PQE = this.PQE;
            DataObject.PQE_SIGNTIME = this.PQE_SIGNTIME;
            DataObject.PM = this.PM;
            DataObject.PM_SIGNTIME = this.PM_SIGNTIME;
            DataObject.DC = this.DC;
            DataObject.DC_SIGNTIME = this.DC_SIGNTIME;
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
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string PE
        {
            get
            {
                return (string)this["PE"];
            }
            set
            {
                this["PE"] = value;
            }
        }
        public DateTime? PE_SIGNTIME
        {
            get
            {
                return (DateTime?)this["PE_SIGNTIME"];
            }
            set
            {
                this["PE_SIGNTIME"] = value;
            }
        }
        public string PQE
        {
            get
            {
                return (string)this["PQE"];
            }
            set
            {
                this["PQE"] = value;
            }
        }
        public DateTime? PQE_SIGNTIME
        {
            get
            {
                return (DateTime?)this["PQE_SIGNTIME"];
            }
            set
            {
                this["PQE_SIGNTIME"] = value;
            }
        }
        public string PM
        {
            get
            {
                return (string)this["PM"];
            }
            set
            {
                this["PM"] = value;
            }
        }
        public DateTime? PM_SIGNTIME
        {
            get
            {
                return (DateTime?)this["PM_SIGNTIME"];
            }
            set
            {
                this["PM_SIGNTIME"] = value;
            }
        }
        public string DC
        {
            get
            {
                return (string)this["DC"];
            }
            set
            {
                this["DC"] = value;
            }
        }
        public DateTime? DC_SIGNTIME
        {
            get
            {
                return (DateTime?)this["DC_SIGNTIME"];
            }
            set
            {
                this["DC_SIGNTIME"] = value;
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
    public class R_SIGN
    {
        public string ID;
        public string LOT_NO;
        public string TYPE;
        public string PE;
        public DateTime? PE_SIGNTIME;
        public string PQE;
        public DateTime? PQE_SIGNTIME;
        public string PM;
        public DateTime? PM_SIGNTIME;
        public string DC;
        public DateTime? DC_SIGNTIME;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}