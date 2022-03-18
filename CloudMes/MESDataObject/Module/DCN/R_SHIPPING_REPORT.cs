using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SHIPPING_REPORT : DataObjectTable
    {
        public T_R_SHIPPING_REPORT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SHIPPING_REPORT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SHIPPING_REPORT);
            TableName = "R_SHIPPING_REPORT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SHIPPING_REPORT : DataObjectBase
    {
        public Row_R_SHIPPING_REPORT(DataObjectInfo info) : base(info)
        {

        }
        public R_SHIPPING_REPORT GetDataObject()
        {
            R_SHIPPING_REPORT DataObject = new R_SHIPPING_REPORT();
            DataObject.ID = this.ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.PO_NO = this.PO_NO;
            DataObject.SO_NO = this.SO_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SHIP_QTY = this.SHIP_QTY;
            DataObject.PALLET_QTY = this.PALLET_QTY;
            DataObject.CARTON_QTY = this.CARTON_QTY;
            DataObject.GROSS_WEIGHT = this.GROSS_WEIGHT;
            DataObject.NET_WEIGHT = this.NET_WEIGHT;
            DataObject.SHIP_DATE = this.SHIP_DATE;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
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
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string SO_NO
        {
            get
            {
                return (string)this["SO_NO"];
            }
            set
            {
                this["SO_NO"] = value;
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
        public double? SHIP_QTY
        {
            get
            {
                return (double?)this["SHIP_QTY"];
            }
            set
            {
                this["SHIP_QTY"] = value;
            }
        }
        public double? PALLET_QTY
        {
            get
            {
                return (double?)this["PALLET_QTY"];
            }
            set
            {
                this["PALLET_QTY"] = value;
            }
        }
        public double? CARTON_QTY
        {
            get
            {
                return (double?)this["CARTON_QTY"];
            }
            set
            {
                this["CARTON_QTY"] = value;
            }
        }
        public string GROSS_WEIGHT
        {
            get
            {
                return (string)this["GROSS_WEIGHT"];
            }
            set
            {
                this["GROSS_WEIGHT"] = value;
            }
        }
        public string NET_WEIGHT
        {
            get
            {
                return (string)this["NET_WEIGHT"];
            }
            set
            {
                this["NET_WEIGHT"] = value;
            }
        }
        public DateTime? SHIP_DATE
        {
            get
            {
                return (DateTime?)this["SHIP_DATE"];
            }
            set
            {
                this["SHIP_DATE"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
    }
    public class R_SHIPPING_REPORT
    {
        public string ID { get; set; }
        public string DN_NO { get; set; }
        public string PO_NO { get; set; }
        public string SO_NO { get; set; }
        public string SKUNO { get; set; }
        public double? SHIP_QTY { get; set; }
        public double? PALLET_QTY { get; set; }
        public double? CARTON_QTY { get; set; }
        public string GROSS_WEIGHT { get; set; }
        public string NET_WEIGHT { get; set; }
        public DateTime? SHIP_DATE { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
    }
}