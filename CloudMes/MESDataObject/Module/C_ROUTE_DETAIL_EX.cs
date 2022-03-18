using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_ROUTE_DETAIL_EX : DataObjectTable
    {
        public T_C_ROUTE_DETAIL_EX(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_ROUTE_DETAIL_EX(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_ROUTE_DETAIL_EX);
            TableName = "C_ROUTE_DETAIL_EX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_ROUTE_DETAIL_EX GetObject(OleExec DB,string detail_id,string name,string value)
        {
            return DB.ORM.Queryable<C_ROUTE_DETAIL_EX>().Where(r => r.DETAIL_ID == detail_id && r.NAME == name && r.VALUE == value).ToList().FirstOrDefault();
        }

        public int Delete(OleExec DB, string detail_id, string name, string value)
        {
            return DB.ORM.Deleteable<C_ROUTE_DETAIL_EX>().Where(r => r.DETAIL_ID == detail_id && r.NAME == name && r.VALUE == value).ExecuteCommand();
        }

        public int Save(OleExec DB, C_ROUTE_DETAIL_EX exObje)
        {
            return DB.ORM.Insertable<C_ROUTE_DETAIL_EX>(exObje).ExecuteCommand();
        }
    }
    public class Row_C_ROUTE_DETAIL_EX : DataObjectBase
    {
        public Row_C_ROUTE_DETAIL_EX(DataObjectInfo info) : base(info)
        {

        }
        public C_ROUTE_DETAIL_EX GetDataObject()
        {
            C_ROUTE_DETAIL_EX DataObject = new C_ROUTE_DETAIL_EX();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
            DataObject.DETAIL_ID = this.DETAIL_ID;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
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
        public string DETAIL_ID
        {
            get
            {
                return (string)this["DETAIL_ID"];
            }
            set
            {
                this["DETAIL_ID"] = value;
            }
        }
    }
    public class C_ROUTE_DETAIL_EX
    {
        public string ID { get; set; }
        public double? SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string DETAIL_ID { get; set; }
    }
}