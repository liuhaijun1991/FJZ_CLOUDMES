using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_GROUP : DataObjectTable
    {
        public T_C_KP_GROUP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_GROUP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_GROUP);
            TableName = "C_KP_GROUP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_KP_GROUP GetKPGROUP(string gpname, OleExec DB, DB_TYPE_ENUM DBType)
        {

            string sql = null;
            DataTable dt = null;
            Row_C_KP_GROUP row_c_kp_group = null;
            sql = $@"select * from C_KP_GROUP where GROUPNAME='{gpname}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 1)
            {
                row_c_kp_group = (Row_C_KP_GROUP)this.NewRow();
                row_c_kp_group.loadData(dt.Rows[0]);
            }
            else if (dt.Rows.Count > 1)
            {
                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000269", new string[] { gpname }));
            }
            else
            {
                throw new MESDataObject.MESReturnMessage(MESDataObject.MESReturnMessage.GetMESReturnMessage("MES00000270", new string[] { gpname }));
            }
            return row_c_kp_group.GetDataObject();
            
            
        }

        public List<C_KP_GROUP> GetAllGroupName(OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_KP_GROUP> Ret = new List<C_KP_GROUP>();
            string StrSql = $@"select distinct GROUPNAME from C_KP_GROUP order by GROUPNAME";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            if (DT.Rows.Count > 0)
            {
                foreach (DataRow DR in DT.Rows)
                {
                    C_KP_GROUP Row = GetRow(DR);
                    Ret.Add(Row);
                }
                return Ret;
            }
            else
            {
                return null;
            }
        }

        public C_KP_GROUP GetRow(DataRow DR)
        {
            Row_C_KP_GROUP Ret = (Row_C_KP_GROUP)NewRow();
            Ret.loadData(DR);
            return Ret.GetDataObject();
        }
    }
    public class Row_C_KP_GROUP : DataObjectBase
    {
        public Row_C_KP_GROUP(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_GROUP GetDataObject()
        {
            C_KP_GROUP DataObject = new C_KP_GROUP();
            DataObject.ID = this.ID;
            DataObject.GROUPNAME = this.GROUPNAME;
            DataObject.REQUIRED_FLAG = this.REQUIRED_FLAG;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
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
        public string GROUPNAME
        {
            get
            {
                return (string)this["GROUPNAME"];
            }
            set
            {
                this["GROUPNAME"] = value;
            }
        }
        public string REQUIRED_FLAG
        {
            get
            {
                return (string)this["REQUIRED_FLAG"];
            }
            set
            {
                this["REQUIRED_FLAG"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
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
    public class C_KP_GROUP
    {
        public string ID { get; set; }
        public string GROUPNAME { get; set; }
        public string REQUIRED_FLAG { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}