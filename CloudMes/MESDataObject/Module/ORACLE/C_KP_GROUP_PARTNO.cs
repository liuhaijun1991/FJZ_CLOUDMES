using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_GROUP_PARTNO : DataObjectTable
    {
        public T_C_KP_GROUP_PARTNO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_GROUP_PARTNO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_GROUP_PARTNO);
            TableName = "C_KP_GROUP_PARTNO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_KP_GROUP_PARTNO> GetKPGroupPNList(string groupid, OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_KP_GROUP_PARTNO> returnKPGroupPNList = new List<C_KP_GROUP_PARTNO>();
            returnKPGroupPNList = DB.ORM.Queryable<C_KP_GROUP_PARTNO>().Where(t => t.KP_GROUP_ID == groupid).OrderBy(t => t.PARTNO).ToList();
            return returnKPGroupPNList;
        }

        public bool CheckKPGroupPN(string groupid, string pn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            List<C_KP_GROUP_PARTNO> returnKPGroupPN = new List<C_KP_GROUP_PARTNO>();
            returnKPGroupPN = DB.ORM.Queryable<C_KP_GROUP_PARTNO>().Where(t => t.KP_GROUP_ID == groupid && t.PARTNO == pn).ToList();
            if (returnKPGroupPN.Count > 0)
            {
                return true;
            }
            
            return false;
        }

        public List<C_KP_GROUP_PARTNO> GetKPGroupPNID(string groupid, string PN, OleExec DB)
        {
            List<C_KP_GROUP_PARTNO> returnKPGroupPNList = new List<C_KP_GROUP_PARTNO>();
            returnKPGroupPNList = DB.ORM.Queryable<C_KP_GROUP_PARTNO>().Where(t => t.KP_GROUP_ID == groupid && t.PARTNO == PN).ToList();
            return returnKPGroupPNList;
        }


        public C_KP_GROUP_PARTNO GetRow(DataRow DR)
        {
            Row_C_KP_GROUP_PARTNO Ret = (Row_C_KP_GROUP_PARTNO)NewRow();
            Ret.loadData(DR);
            return Ret.GetDataObject();
        }
    }
    public class Row_C_KP_GROUP_PARTNO : DataObjectBase
    {
        public Row_C_KP_GROUP_PARTNO(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_GROUP_PARTNO GetDataObject()
        {
            C_KP_GROUP_PARTNO DataObject = new C_KP_GROUP_PARTNO();
            DataObject.ID = this.ID;
            DataObject.KP_GROUP_ID = this.KP_GROUP_ID;
            DataObject.PARTNO = this.PARTNO;
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
        public string KP_GROUP_ID
        {
            get
            {
                return (string)this["KP_GROUP_ID"];
            }
            set
            {
                this["KP_GROUP_ID"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
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
    public class C_KP_GROUP_PARTNO
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string KP_GROUP_ID { get; set; }
        public string PARTNO { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}