using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{
    public class T_C_BU_EX : DataObjectTable
    {
        public T_C_BU_EX(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {
        }
        public T_C_BU_EX(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_BU);
            TableName = "C_BU_EX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_BU_EX ConstructBuex(DataRow dr, OleExec DB)
        {
            C_BU_EX buex = new C_BU_EX();
            buex = SessionManager<C_BU_EX>.ConstructObject(dr);
            return buex;
        }

        public List<C_BU_EX> GetAllBUEx(OleExec DB)
        {
            List<C_BU_EX> BUExList = new List<C_BU_EX>();
            string sql = $@"SELECT ID,SEQ_NO,NAME,VALUE,NAME||' '||VALUE  SUBSITE FROM C_BU_EX ";
            DataTable dt = null;
            try
            {
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        BUExList.Add(ConstructBuex(dr, DB));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return BUExList;
        }



        public List<C_BU_EX> GetBuExByName(string BuName, OleExec DB)
        {
            List<C_BU_EX> BUExList = new List<C_BU_EX>();
            string sql = $@"SELECT ID,SEQ_NO,NAME,VALUE,NAME||' '||VALUE  SUBSITE FROM C_BU_EX  WHERE NAME =:NAME";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter("NAME", BuName) };
            DataTable dt = null;
            try
            {
                dt = DB.ExecSelect(sql, parameters).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        BUExList.Add(ConstructBuex(dr, DB));
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return BUExList;
        }
        public int DeleteBuExByID_SEQ_NO(string ID, decimal SEQ_NO, OleExec DB)
        {
            string sql = $@"DELETE FROM  C_BU_EX  WHERE ID=:ID AND SEQ_NO=:SEQ_NO";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter("ID", ID), new OleDbParameter("SEQ_NO", SEQ_NO) };
            int result = DB.ExecuteNonQuery(sql, CommandType.Text, parameters);
            return result;
        }
        public int UpDate(string ID, decimal SEQ_NO,string NAME,string VALUE, OleExec DB)
        {
            string sql = $@" UPDATE C_BU_EX SET NAME='{NAME}' ,VALUE='{VALUE}' WHERE ID='{ID}' AND SEQ_NO={SEQ_NO}";
            OleDbParameter[] parameters = null;
            int result = DB.ExecuteNonQuery(sql, CommandType.Text, parameters);
            return result;
        }

        public int Insert(string BU ,string NAME, string VALUE, OleExec DB,string systemname ,DB_TYPE_ENUM dbt)
        {
            OleDbParameter[] parameters;
            string sql = "";
            string ID;
            decimal SEQ_NO;
            DataTable dt;
            T_C_BU_EX buex = null;
            buex = new T_C_BU_EX(DB, dbt);
            ID=buex.GetNewID(systemname, DB, dbt);
            sql = $@"SELECT * FROM   (SELECT SEQ_NO  from C_BU_EX  WHERE NAME='{NAME}' ORDER BY SEQ_NO DESC)  WHERE ROWNUM=1";
            dt = new DataTable();
            dt = DB.ExecSelect(sql).Tables[0];
            if(dt.Rows.Count>0)
            { 
                SEQ_NO =   decimal.Parse(dt.Rows[0][0].ToString())+1;
            }
            else
            {
                SEQ_NO = 1;  
            }

            //CHECK NAME VALUE EXIST
            if (NAME_VALUE_IsExist(NAME, VALUE, DB))
            {
                return -1;   // error name value existed
            }
            else
            {

                sql = $@" INSERT INTO C_BU_EX VALUES(:ID,:SEQ_N0,:NAME,:VALUE)";
                parameters = new OleDbParameter[] { new OleDbParameter("ID", ID), new OleDbParameter("SEQ_NO", SEQ_NO),
                                          new OleDbParameter("NAME", NAME), new OleDbParameter("VALUE", VALUE)
                                        };
                int result = DB.ExecuteNonQuery(sql, CommandType.Text, parameters);
                return result;
            }
        }


        public bool ID_SEQ_NO_IsExist(string ID, decimal SEQ_NO, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = "SELECT * FROM C_BU_EX WHERE ID=:ID AND SEQ_NO=:SEQ_NO";
            OleDbParameter[] parameters = new OleDbParameter[] {
                new OleDbParameter("ID",ID),
                new OleDbParameter("SEQ_NO",SEQ_NO)
            };
            dt = DB.ExecSelect(sql, parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool NAME_VALUE_IsExist(string NAME, string VALUE, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = "SELECT * FROM C_BU_EX WHERE NAME=:NAME AND VALUE=:VALUE";
            OleDbParameter[] parameters = new OleDbParameter[] {
                new OleDbParameter("NAME",NAME),
                new OleDbParameter("VALUE",VALUE)
            };
            dt = DB.ExecSelect(sql, parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool ID_SEQ_NO_NAME_VALUE_IsExist(string ID, decimal SEQ_NO,string NAME, string VALUE, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = "SELECT * FROM C_BU_EX WHERE ID=:ID AND SEQ_NO=:SEQ_NO AND NAME =:NAME AND VALUE=:VALUE";
            OleDbParameter[] parameters = new OleDbParameter[] {
                new OleDbParameter("ID",ID),
                new OleDbParameter("SEQ_NO",SEQ_NO),
                new OleDbParameter("NAME",NAME),
                new OleDbParameter("VALUE",VALUE)
            };
            dt = DB.ExecSelect(sql, parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            return false;
        }


    }


    public class Row_C_BU_EX : DataObjectBase
    {
        public Row_C_BU_EX(DataObjectInfo info) : base(info)
        {

        }
        public C_BU_EX GetDataObject()
        {
            C_BU_EX DataObject = new C_BU_EX();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
            DataObject.SUBSITE = this.SUBSITE;
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
        public decimal SEQ_NO
        {
            get
            {
                return (decimal)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public string SUBSITE
        {
            get
            {
                return (string)this["SUBSITE"];
            }
            set
            {
                this["SUBSITE"] = value;
            }
        }
    }
    public class C_BU_EX
    {
        public string ID;
        public decimal SEQ_NO;
        public string NAME;
        public string VALUE;
        public string SUBSITE;
    }

}
