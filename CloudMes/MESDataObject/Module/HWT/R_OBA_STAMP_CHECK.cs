using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_R_OBA_STAMP_CHECK : DataObjectTable
    {
        public T_R_OBA_STAMP_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_OBA_STAMP_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_OBA_STAMP_CHECK);
            TableName = "R_OBA_STAMP_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int Save(OleExec sfcdb, R_OBA_STAMP_CHECK stampObject)
        {
            return sfcdb.ORM.Insertable<R_OBA_STAMP_CHECK>(stampObject).ExecuteCommand();
        }

        public bool IsExist(OleExec sfcdb, string packNo, string packType)
        {
            return sfcdb.ORM.Queryable<R_OBA_STAMP_CHECK>().Where(r => r.PACK_NO == packNo && r.PACK_TYPE == packType).Any();
        }
    }
    public class Row_R_OBA_STAMP_CHECK : DataObjectBase
    {
        public Row_R_OBA_STAMP_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public R_OBA_STAMP_CHECK GetDataObject()
        {
            R_OBA_STAMP_CHECK DataObject = new R_OBA_STAMP_CHECK();
            DataObject.ID = this.ID;
            DataObject.PACK_NO = this.PACK_NO;
            DataObject.PACK_TYPE = this.PACK_TYPE;
            DataObject.QTY = this.QTY;
            DataObject.STATUS = this.STATUS;
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
        public string PACK_NO
        {
            get
            {
                return (string)this["PACK_NO"];
            }
            set
            {
                this["PACK_NO"] = value;
            }
        }
        public string PACK_TYPE
        {
            get
            {
                return (string)this["PACK_TYPE"];
            }
            set
            {
                this["PACK_TYPE"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? STATUS
        {
            get
            {
                return (double?)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
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
    public class R_OBA_STAMP_CHECK
    {
        public string ID { get; set; }
        public string PACK_NO { get; set; }
        public string PACK_TYPE { get; set; }
        public double? QTY { get; set; }
        public double? STATUS { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}