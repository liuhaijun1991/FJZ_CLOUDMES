using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_FAI_SECTION : DataObjectTable
    {
        public T_C_FAI_SECTION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_FAI_SECTION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_FAI_SECTION);
            TableName = "C_FAI_SECTION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 通過機種,SMT_SECTION_FAI_B=0 OR SMT_SECTION_FAI_T=0,機種版本查詢C_FAI_SECTION表
        /// </summary>
        /// <param name="checkskuno"></param>
        /// <param name="checkversion"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public C_FAI_SECTION GetFaiBySmtBorT(string checkskuno, string checkversion, OleExec db)
        {
            string strSql = $@" select * from C_FAI_SECTION where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  (SMT_SECTION_FAI_B=0 or SMT_SECTION_FAI_T=0)";
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_FAI_SECTION result = new C_FAI_SECTION();
            if (table.Rows.Count > 0)
            {
                Row_C_FAI_SECTION ret = (Row_C_FAI_SECTION)this.NewRow();
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
        /// 通過機種,SMT_SECTION_FAI_B=0和機種版本查詢C_FAI_SECTION表,若傳參為update,則執行update
        /// </summary>
        /// <param name="checkskuno"></param>
        /// <param name="checkversion"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public object GetFaiBySmtB(string checkskuno, string checkversion, OleExec db, string mode = "select")
        {
            string strSql = "";
            if (mode == "update")
            {
                try
                {
                    strSql = $@" update C_FAI_SECTION set SMT_SECTION_FAI_B=1 where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  SMT_SECTION_FAI_B=0";
                    db.ExecSQL(strSql);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                return true;
                
            }
            else
            {
                strSql = $@" select * from C_FAI_SECTION where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  SMT_SECTION_FAI_B=0";
            }
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            DataTable table = db.ExecSelect(strSql).Tables[0];
            C_FAI_SECTION result = new C_FAI_SECTION();
            if (table.Rows.Count > 0)
            {
                Row_C_FAI_SECTION ret = (Row_C_FAI_SECTION)this.NewRow();
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
        /// 通過機種,SMT_SECTION_FAI_T=0和機種版本查詢C_FAI_SECTION表
        /// </summary>
        /// <param name="checkskuno"></param>
        /// <param name="checkversion"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public object GetFaiBySmtT(string checkskuno, string checkversion, OleExec db,string mode = "select")
        {
            string strSql = "";
            if (mode == "update")
            {
                try
                {
                    strSql = $@" update C_FAI_SECTION set SMT_SECTION_FAI_T=1 where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  SMT_SECTION_FAI_T=0";
                    db.ExecSQL(strSql);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                return true;
            }
            else
            {
                strSql = $@" select * from C_FAI_SECTION where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  SMT_SECTION_FAI_T=0";
                DataTable table = db.ExecSelect(strSql).Tables[0];
                C_FAI_SECTION result = new C_FAI_SECTION();
                if (table.Rows.Count > 0)
                {
                    Row_C_FAI_SECTION ret = (Row_C_FAI_SECTION)this.NewRow();
                    ret.loadData(table.Rows[0]);
                    result = ret.GetDataObject();
                }
                else
                {
                    result = null;
                }
                return result;
            }
            //OleDbParameter[] paramet = new OleDbParameter[1];
            ////paramet[0] = new OleDbParameter(":control_name", controlName);
            //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
            
        }

        /// <summary>
        /// 通過機種,PTH_SECTION_FAI=0,機種版本查詢C_FAI_SECTION表
        /// </summary>
        /// <param name="checkskuno"></param>
        /// <param name="checkversion"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public object GetFaiByPTH(string checkskuno, string checkversion, OleExec db, string mode = "select")
        {
            string strSql = "";
            if (mode == "update")
            {
                try
                {
                    strSql = $@" update C_FAI_SECTION set PTH_SECTION_FAI=1 where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  PTH_SECTION_FAI=0";
                    db.ExecSQL(strSql);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                return true;
            }
            else
            {
                strSql = $@" select * from C_FAI_SECTION where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  PTH_SECTION_FAI=0";
                //OleDbParameter[] paramet = new OleDbParameter[1];
                ////paramet[0] = new OleDbParameter(":control_name", controlName);
                //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
                DataTable table = db.ExecSelect(strSql).Tables[0];
                C_FAI_SECTION result = new C_FAI_SECTION();
                if (table.Rows.Count > 0)
                {
                    Row_C_FAI_SECTION ret = (Row_C_FAI_SECTION)this.NewRow();
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

        /// <summary>
        /// 通過機種,SI_SECTION_FAI=0,機種版本查詢C_FAI_SECTION表
        /// </summary>
        /// <param name="checkskuno"></param>
        /// <param name="checkversion"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public object GetFaiBySI(string checkskuno, string checkversion, OleExec db, string mode = "select")
        {
            string strSql = "";
            if (mode == "update")
            {
                try
                {
                    strSql = $@" update C_FAI_SECTION set SI_SECTION_FAI=1 where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  SI_SECTION_FAI=0";
                    db.ExecSQL(strSql);
                }
                catch (Exception ex)
                {
                    throw (ex);
                }
                return true;
            }
            else
            {
                strSql = $@" select * from C_FAI_SECTION where SKUNO='{checkskuno}' and SKU_VER='{checkversion}' and  SI_SECTION_FAI=0";
                //OleDbParameter[] paramet = new OleDbParameter[1];
                ////paramet[0] = new OleDbParameter(":control_name", controlName);
                //DataTable table = db.ExecuteDataTable(strSql, CommandType.Text, paramet);
                DataTable table = db.ExecSelect(strSql).Tables[0];
                C_FAI_SECTION result = new C_FAI_SECTION();
                if (table.Rows.Count > 0)
                {
                    Row_C_FAI_SECTION ret = (Row_C_FAI_SECTION)this.NewRow();
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

	 /// <summary>
        /// 通過機種版本查詢C_FAI_SECTION表
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="Ver"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public C_FAI_SECTION GetFAIBySkuVer(string Skuno, string Ver, OleExec db)
        {
            return db.ORM.Queryable<C_FAI_SECTION>().Where(t => t.SKUNO == Skuno && t.SKU_VER == Ver).ToList().FirstOrDefault();
        }

        /// <summary>
        /// 將新料號版本Insert Into C_FAI_SECTION表
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="Ver"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public int InsertFAIBySkuVer(C_FAI_SECTION NewFAIObj, OleExec db)
        {
            return db.ORM.Insertable<C_FAI_SECTION>(NewFAIObj).ExecuteCommand();
        }

    }
    public class Row_C_FAI_SECTION : DataObjectBase
    {
        public Row_C_FAI_SECTION(DataObjectInfo info) : base(info)
        {

        }
        public C_FAI_SECTION GetDataObject()
        {
            C_FAI_SECTION DataObject = new C_FAI_SECTION();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.SMT_SECTION_FAI_T = this.SMT_SECTION_FAI_T;
            DataObject.SMT_SECTION_FAI_B = this.SMT_SECTION_FAI_B;
            DataObject.PTH_SECTION_FAI = this.PTH_SECTION_FAI;
            DataObject.SI_SECTION_FAI = this.SI_SECTION_FAI;
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
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
            }
        }
        public string SMT_SECTION_FAI_T
        {
            get
            {
                return (string)this["SMT_SECTION_FAI_T"];
            }
            set
            {
                this["SMT_SECTION_FAI_T"] = value;
            }
        }
        public string SMT_SECTION_FAI_B
        {
            get
            {
                return (string)this["SMT_SECTION_FAI_B"];
            }
            set
            {
                this["SMT_SECTION_FAI_B"] = value;
            }
        }
        public string PTH_SECTION_FAI
        {
            get
            {
                return (string)this["PTH_SECTION_FAI"];
            }
            set
            {
                this["PTH_SECTION_FAI"] = value;
            }
        }
        public string SI_SECTION_FAI
        {
            get
            {
                return (string)this["SI_SECTION_FAI"];
            }
            set
            {
                this["SI_SECTION_FAI"] = value;
            }
        }
    }
    public class C_FAI_SECTION
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string WO { get; set; }
        public string SKUNO { get; set; }
        public string SKU_VER { get; set; }
        public string SMT_SECTION_FAI_T { get; set; }
        public string SMT_SECTION_FAI_B { get; set; }
        public string PTH_SECTION_FAI { get; set; }
        public string SI_SECTION_FAI { get; set; }
    }
}