using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_5DX_TESTRECORD : DataObjectTable
    {
        public T_R_5DX_TESTRECORD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_5DX_TESTRECORD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_5DX_TESTRECORD);
            TableName = "R_5DX_TESTRECORD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_5DX_TESTRECORD> GetSNStatusBYSN(string SN, string Status, OleExec DB)
        {
            return DB.ORM.Queryable<R_5DX_TESTRECORD>().Where(t => t.SN == SN && t.STATUS == Status).OrderBy(t => t.TEST_START_TIME, SqlSugar.OrderByType.Desc).ToList();
        }
    }

    public class Row_R_5DX_TESTRECORD : DataObjectBase
    {
        public Row_R_5DX_TESTRECORD(DataObjectInfo info) : base(info)
        {

        }
        public R_5DX_TESTRECORD GetDataObject()
        {
            R_5DX_TESTRECORD DataObject = new R_5DX_TESTRECORD();
            DataObject.FILE_NAME = this.FILE_NAME;
            DataObject.ID = this.ID;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.CUSTPARTNO = this.CUSTPARTNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.TEST_START_TIME = this.TEST_START_TIME;
            DataObject.TEST_END_TIME = this.TEST_END_TIME;
            DataObject.STATUS = this.STATUS;
            DataObject.PINS_QTY = this.PINS_QTY;
            DataObject.DEL_PINS_QTY = this.DEL_PINS_QTY;
            DataObject.REPAIR_PINS_QTY = this.REPAIR_PINS_QTY;
            DataObject.NOT_REPAIR_PINS_QTY = this.NOT_REPAIR_PINS_QTY;
            DataObject.FAIL_REPAIR_PINS_QTY = this.FAIL_REPAIR_PINS_QTY;
            DataObject.DEL_COMPONENTS_QTY = this.DEL_COMPONENTS_QTY;
            DataObject.DPMO = this.DPMO;
            DataObject.DPMO_TIME = this.DPMO_TIME;
            DataObject.MACHINE_SERIAL_NO = this.MACHINE_SERIAL_NO;
            DataObject.REPAIR_START_TIME = this.REPAIR_START_TIME;
            DataObject.REPAIR_END_TIME = this.REPAIR_END_TIME;
            DataObject.REPAIR_ALL_TIME = this.REPAIR_ALL_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string FILE_NAME
        {
            get
            {
                return (string)this["FILE_NAME"];
            }
            set
            {
                this["FILE_NAME"] = value;
            }
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
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
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
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public string CUSTPARTNO
        {
            get
            {
                return (string)this["CUSTPARTNO"];
            }
            set
            {
                this["CUSTPARTNO"] = value;
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
        public DateTime? TEST_START_TIME
        {
            get
            {
                return (DateTime?)this["TEST_START_TIME"];
            }
            set
            {
                this["TEST_START_TIME"] = value;
            }
        }
        public DateTime? TEST_END_TIME
        {
            get
            {
                return (DateTime?)this["TEST_END_TIME"];
            }
            set
            {
                this["TEST_END_TIME"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public double? PINS_QTY
        {
            get
            {
                return (double?)this["PINS_QTY"];
            }
            set
            {
                this["PINS_QTY"] = value;
            }
        }
        public double? DEL_PINS_QTY
        {
            get
            {
                return (double?)this["DEL_PINS_QTY"];
            }
            set
            {
                this["DEL_PINS_QTY"] = value;
            }
        }
        public double? REPAIR_PINS_QTY
        {
            get
            {
                return (double?)this["REPAIR_PINS_QTY"];
            }
            set
            {
                this["REPAIR_PINS_QTY"] = value;
            }
        }
        public double? NOT_REPAIR_PINS_QTY
        {
            get
            {
                return (double?)this["NOT_REPAIR_PINS_QTY"];
            }
            set
            {
                this["NOT_REPAIR_PINS_QTY"] = value;
            }
        }
        public double? FAIL_REPAIR_PINS_QTY
        {
            get
            {
                return (double?)this["FAIL_REPAIR_PINS_QTY"];
            }
            set
            {
                this["FAIL_REPAIR_PINS_QTY"] = value;
            }
        }
        public double? DEL_COMPONENTS_QTY
        {
            get
            {
                return (double?)this["DEL_COMPONENTS_QTY"];
            }
            set
            {
                this["DEL_COMPONENTS_QTY"] = value;
            }
        }
        public double? DPMO
        {
            get
            {
                return (double?)this["DPMO"];
            }
            set
            {
                this["DPMO"] = value;
            }
        }
        public DateTime? DPMO_TIME
        {
            get
            {
                return (DateTime?)this["DPMO_TIME"];
            }
            set
            {
                this["DPMO_TIME"] = value;
            }
        }
        public string MACHINE_SERIAL_NO
        {
            get
            {
                return (string)this["MACHINE_SERIAL_NO"];
            }
            set
            {
                this["MACHINE_SERIAL_NO"] = value;
            }
        }
        public DateTime? REPAIR_START_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_START_TIME"];
            }
            set
            {
                this["REPAIR_START_TIME"] = value;
            }
        }
        public DateTime? REPAIR_END_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_END_TIME"];
            }
            set
            {
                this["REPAIR_END_TIME"] = value;
            }
        }
        public string REPAIR_ALL_TIME
        {
            get
            {
                return (string)this["REPAIR_ALL_TIME"];
            }
            set
            {
                this["REPAIR_ALL_TIME"] = value;
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
    public class R_5DX_TESTRECORD
    {
        public string FILE_NAME { get; set; }
        public string ID { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public string WO { get; set; }
        public string STATION_NAME { get; set; }
        public string CUSTPARTNO { get; set; }
        public string SKUNO { get; set; }
        public DateTime? TEST_START_TIME { get; set; }
        public DateTime? TEST_END_TIME { get; set; }
        public string STATUS { get; set; }
        public double? PINS_QTY { get; set; }
        public double? DEL_PINS_QTY { get; set; }
        public double? REPAIR_PINS_QTY { get; set; }
        public double? NOT_REPAIR_PINS_QTY { get; set; }
        public double? FAIL_REPAIR_PINS_QTY { get; set; }
        public double? DEL_COMPONENTS_QTY { get; set; }
        public double? DPMO { get; set; }
        public DateTime? DPMO_TIME { get; set; }
        public string MACHINE_SERIAL_NO { get; set; }
        public DateTime? REPAIR_START_TIME { get; set; }
        public DateTime? REPAIR_END_TIME { get; set; }
        public string REPAIR_ALL_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}