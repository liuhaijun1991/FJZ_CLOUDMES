using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_R_PALLET_DOUBLE_CHECK : DataObjectTable
    {
        public T_R_PALLET_DOUBLE_CHECK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PALLET_DOUBLE_CHECK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PALLET_DOUBLE_CHECK);
            TableName = "R_PALLET_DOUBLE_CHECK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Save(OleExec sfcdb, R_PALLET_DOUBLE_CHECK checkObject)
        {
            return sfcdb.ORM.Insertable<R_PALLET_DOUBLE_CHECK>(checkObject).ExecuteCommand();
        }

        public bool IsExist(OleExec sfcdb, string palletNo, string checkType,string checkValue)
        {
            return sfcdb.ORM.Queryable<R_PALLET_DOUBLE_CHECK>().Where(r => r.PALLET_NO == palletNo && r.CHECK_TYPE == checkType && r.CHECK_VALUE == checkValue).Any();
        }
        public List<R_PALLET_DOUBLE_CHECK> GetCheckList(OleExec sfcdb, string palletNo, string checkType)
        {
            return sfcdb.ORM.Queryable<R_PALLET_DOUBLE_CHECK>().Where(r => r.PALLET_NO == palletNo && r.CHECK_TYPE == checkType).ToList();
        }
    }
    public class Row_R_PALLET_DOUBLE_CHECK : DataObjectBase
    {
        public Row_R_PALLET_DOUBLE_CHECK(DataObjectInfo info) : base(info)
        {

        }
        public R_PALLET_DOUBLE_CHECK GetDataObject()
        {
            R_PALLET_DOUBLE_CHECK DataObject = new R_PALLET_DOUBLE_CHECK();
            DataObject.ID = this.ID;
            DataObject.PALLET_NO = this.PALLET_NO;
            DataObject.CHECK_TYPE = this.CHECK_TYPE;
            DataObject.CHECK_VALUE = this.CHECK_VALUE;
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
        public string PALLET_NO
        {
            get
            {
                return (string)this["PALLET_NO"];
            }
            set
            {
                this["PALLET_NO"] = value;
            }
        }
        public string CHECK_TYPE
        {
            get
            {
                return (string)this["CHECK_TYPE"];
            }
            set
            {
                this["CHECK_TYPE"] = value;
            }
        }
        public string CHECK_VALUE
        {
            get
            {
                return (string)this["CHECK_VALUE"];
            }
            set
            {
                this["CHECK_VALUE"] = value;
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
    public class R_PALLET_DOUBLE_CHECK
    {
        public string ID { get; set; }
        public string PALLET_NO { get; set; }
        public string CHECK_TYPE { get; set; }
        public string CHECK_VALUE { get; set; }
        public double? STATUS { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}