using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_KP_LIST_ITEM_CHECK : DataObjectTable
    {
        public T_C_KP_LIST_ITEM_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_KP_LIST_ITEM_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_KP_LIST_ITEM_CHECK);
            TableName = "C_KP_LIST_ITEM_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        //public List<C_KP_LIST_ITEM_CHECK> GetBySku(string SKUNO, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_KP_LIST_ITEM_CHECK>().Where(t => t.CHECKBOXSKUNO_FLAG == SKUNO).ToList();
        //}
        //public List<C_KP_LIST_ITEM_CHECK> GetBySN1(string KPSNSKUNO, string SNSKUNO, OleExec DB)
        //{
        //    //List<R_SN> ListRSNKPSN = DB.ORM.Queryable<R_SN, R_SN_KP>
        //    //    ((p1, p2) => p1.SN == p2.VALUE).Where((p1, p2) => p2.SN == KPSN).ToList();
        //    //List<R_SN> ListRSNSN = DB.ORM.Queryable<R_SN, R_SN_KP>
        //    //    ((p1, p2) => p1.SN == p2.SN).Where((p1, p2) => p2.SN == KPSN).ToList();
        //    return DB.ORM.Queryable<C_KP_LIST_ITEM_CHECK, C_KP_List_Item, C_KP_LIST>((p1, p2, p3) => p1.C_KP_LIST_ITEM_ID == p2.ID && p2.LIST_ID == p3.ID).Where((p1, p2, p3) => p2.KP_PARTNO == KPSNSKUNO && p3.SKUNO == SNSKUNO).Select((p1, p2, p3) =>p1).ToList();
        //}
        //public List<C_KP_LIST_ITEM_CHECK> GetBySNSKUNO(string SNSKUNO, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_KP_LIST_ITEM_CHECK, C_KP_List_Item, C_KP_LIST>((p1, p2, p3) => p1.C_KP_LIST_ITEM_ID == p2.ID && p2.LIST_ID == p3.ID).Where((p1, p2, p3) => p2.KP_PARTNO == SNSKUNO).Select((p1, p2, p3) => p1).ToList();
        //}
        //public List<C_KP_LIST_ITEM_CHECK> GetByKPSNSKUNO(string KPSNSKUNO, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_KP_LIST_ITEM_CHECK, C_KP_List_Item, C_KP_LIST>((p1, p2, p3) => p1.C_KP_LIST_ITEM_ID == p2.ID && p2.LIST_ID == p3.ID).Where((p1, p2, p3) => p2.KP_PARTNO == KPSNSKUNO).Select((p1, p2, p3) => p1).ToList();
        //}
        //public List<C_KP_LIST_ITEM_CHECK> GetByKPSNSKUNOSEQ(string KPSNSKUNO, double? SEQ, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_KP_LIST_ITEM_CHECK, C_KP_List_Item, C_KP_LIST>((p1, p2, p3) => p1.C_KP_LIST_ITEM_ID == p2.ID && p2.LIST_ID == p3.ID).Where((p1, p2, p3) => p2.KP_PARTNO == KPSNSKUNO && p2.SEQ == SEQ).Select((p1, p2, p3) => p1).ToList();
        //}
        //public List<C_KP_LIST_ITEM_CHECK> GetIDBKPName(string ID, OleExec DB)
        //{
        //    return DB.ORM.Queryable<C_KP_LIST_ITEM_CHECK>().Where(t => t.C_KP_LIST_ITEM_ID == ID).ToList();
        //}
        //public int InsertKPListItemCheck(C_KP_LIST_ITEM_CHECK CKPList, OleExec DB)
        //{
        //    return DB.ORM.Insertable(CKPList).ExecuteCommand();
        //}
        //public int UpdateKPListItemCheck(C_KP_LIST_ITEM_CHECK CKPList, String ListItemID, OleExec DB)
        //{
        //    return DB.ORM.Updateable<C_KP_LIST_ITEM_CHECK>(CKPList).Where(t=>t.ID== ListItemID).ExecuteCommand();
        //}
    }
    public class Row_C_KP_LIST_ITEM_CHECK : DataObjectBase
    {
        public Row_C_KP_LIST_ITEM_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public C_KP_LIST_ITEM_CHECK GetDataObject()
        {
            C_KP_LIST_ITEM_CHECK DataObject = new C_KP_LIST_ITEM_CHECK();
            DataObject.ID = this.ID;
            DataObject.C_KP_LIST_ITEM_ID = this.C_KP_LIST_ITEM_ID;
            DataObject.SILOADING_FLAG = this.SILOADING_FLAG;
            DataObject.PACKING_FLAG = this.PACKING_FLAG;
            DataObject.SN_FLAG = this.SN_FLAG;
            DataObject.CHECKPCBAVER_FALG = this.CHECKPCBAVER_FALG;
            DataObject.CHECKPCBMODELVER_FLAG = this.CHECKPCBMODELVER_FLAG;
            DataObject.AUTOSTATION = this.AUTOSTATION;
            DataObject.AUTOSTATION2 = this.AUTOSTATION2;
            DataObject.CHECKBOXSKUNO_FLAG = this.CHECKBOXSKUNO_FLAG;
            DataObject.TESTSTATION = this.TESTSTATION;
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
        public string C_KP_LIST_ITEM_ID
        {
            get
            {
                return (string)this["C_KP_LIST_ITEM_ID"];
            }
            set
            {
                this["C_KP_LIST_ITEM_ID"] = value;
            }
        }
        public string SILOADING_FLAG
        {
            get
            {
                return (string)this["SILOADING_FLAG"];
            }
            set
            {
                this["SILOADING_FLAG"] = value;
            }
        }
        public string PACKING_FLAG
        {
            get
            {
                return (string)this["PACKING_FLAG"];
            }
            set
            {
                this["PACKING_FLAG"] = value;
            }
        }
        public string SN_FLAG
        {
            get
            {
                return (string)this["SN_FLAG"];
            }
            set
            {
                this["SN_FLAG"] = value;
            }
        }
        public string CHECKPCBAVER_FALG
        {
            get
            {
                return (string)this["CHECKPCBAVER_FALG"];
            }
            set
            {
                this["CHECKPCBAVER_FALG"] = value;
            }
        }
        public string CHECKPCBMODELVER_FLAG
        {
            get
            {
                return (string)this["CHECKPCBMODELVER_FLAG"];
            }
            set
            {
                this["CHECKPCBMODELVER_FLAG"] = value;
            }
        }
        public string AUTOSTATION
        {
            get
            {
                return (string)this["AUTOSTATION"];
            }
            set
            {
                this["AUTOSTATION"] = value;
            }
        }
        public string AUTOSTATION2
        {
            get
            {
                return (string)this["AUTOSTATION2"];
            }
            set
            {
                this["AUTOSTATION2"] = value;
            }
        }
        public string CHECKBOXSKUNO_FLAG
        {
            get
            {
                return (string)this["CHECKBOXSKUNO_FLAG"];
            }
            set
            {
                this["CHECKBOXSKUNO_FLAG"] = value;
            }
        }
        public string TESTSTATION
        {
            get
            {
                return (string)this["TESTSTATION"];
            }
            set
            {
                this["TESTSTATION"] = value;
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
    public class C_KP_LIST_ITEM_CHECK
    {
        public string ID { get; set; }
        public string C_KP_LIST_ITEM_ID { get; set; }
        public string SILOADING_FLAG { get; set; }
        public string PACKING_FLAG { get; set; }
        public string SN_FLAG { get; set; }
        public string CHECKPCBAVER_FALG { get; set; }
        public string CHECKPCBMODELVER_FLAG { get; set; }
        public string AUTOSTATION { get; set; }
        public string AUTOSTATION2 { get; set; }
        public string CHECKBOXSKUNO_FLAG { get; set; }
        public string TESTSTATION { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}