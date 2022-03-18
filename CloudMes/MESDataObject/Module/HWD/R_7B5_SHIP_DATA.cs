using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_SHIP_DATA : DataObjectTable
    {
        public T_R_7B5_SHIP_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_SHIP_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_SHIP_DATA);
            TableName = "R_7B5_SHIP_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int SaveShipData(OleExec sfcdb, R_7B5_SHIP_DATA objShipData)
        {
            bool exist = sfcdb.ORM.Queryable<R_7B5_SHIP_DATA>().Any(
                r => r.LOTNO == objShipData.LOTNO && r.TASK_NO == objShipData.TASK_NO
                && r.HH_ITEM == objShipData.HH_ITEM && r.HW_ITEM == objShipData.HW_ITEM
                && r.QTY == objShipData.QTY && r.REMARK == objShipData.REMARK
                && r.LASTEDITBY == objShipData.LASTEDITBY && r.LASTEDITDT == objShipData.LASTEDITDT
                && r.DELETE_FLAG == objShipData.DELETE_FLAG && r.SAP_FLAG == objShipData.SAP_FLAG);
            if (exist)
            {
                return 1;
            }
            else
            {
                return sfcdb.ORM.Insertable<R_7B5_SHIP_DATA>(objShipData).ExecuteCommand();
            }
        }

        public List<R_7B5_SHIP_DATA> GetListByLotNo(OleExec sfcdb, string lot_no)
        {
            return sfcdb.ORM.Queryable<R_7B5_SHIP_DATA>().Where(r => r.LOTNO == lot_no).ToList();
        }
    }
    public class Row_R_7B5_SHIP_DATA : DataObjectBase
    {
        public Row_R_7B5_SHIP_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_SHIP_DATA GetDataObject()
        {
            R_7B5_SHIP_DATA DataObject = new R_7B5_SHIP_DATA();
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.DELETE_FLAG = this.DELETE_FLAG;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.LASTEDITBY = this.LASTEDITBY;
            DataObject.REMARK = this.REMARK;
            DataObject.QTY = this.QTY;
            DataObject.HW_ITEM = this.HW_ITEM;
            DataObject.HH_ITEM = this.HH_ITEM;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.LOTNO = this.LOTNO;
            return DataObject;
        }
        public string SAP_FLAG
        {
            get
            {
                return (string)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
            }
        }
        public string DELETE_FLAG
        {
            get
            {
                return (string)this["DELETE_FLAG"];
            }
            set
            {
                this["DELETE_FLAG"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string LASTEDITBY
        {
            get
            {
                return (string)this["LASTEDITBY"];
            }
            set
            {
                this["LASTEDITBY"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
        public string HW_ITEM
        {
            get
            {
                return (string)this["HW_ITEM"];
            }
            set
            {
                this["HW_ITEM"] = value;
            }
        }
        public string HH_ITEM
        {
            get
            {
                return (string)this["HH_ITEM"];
            }
            set
            {
                this["HH_ITEM"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
            }
        }
        public string LOTNO
        {
            get
            {
                return (string)this["LOTNO"];
            }
            set
            {
                this["LOTNO"] = value;
            }
        }
    }
    public class R_7B5_SHIP_DATA
    {
        public string SAP_FLAG { get; set; }
        public string DELETE_FLAG { get; set; }
        public DateTime? LASTEDITDT { get; set; }
        public string LASTEDITBY { get; set; }
        public string REMARK { get; set; }
        public double? QTY { get; set; }
        public string HW_ITEM { get; set; }
        public string HH_ITEM { get; set; }
        public string TASK_NO { get; set; }
        public string LOTNO { get; set; }
    }
}