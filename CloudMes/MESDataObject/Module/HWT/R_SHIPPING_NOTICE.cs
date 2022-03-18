using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_SHIPPING_NOTICE : DataObjectTable
    {
        public T_R_SHIPPING_NOTICE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SHIPPING_NOTICE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SHIPPING_NOTICE);
            TableName = "R_SHIPPING_NOTICE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SHIPPING_NOTICE : DataObjectBase
    {
        public Row_R_SHIPPING_NOTICE(DataObjectInfo info) : base(info)
        {

        }
        public R_SHIPPING_NOTICE GetDataObject()
        {
            R_SHIPPING_NOTICE DataObject = new R_SHIPPING_NOTICE();
            DataObject.SHIP_NO = this.SHIP_NO;
            DataObject.TO_NO = this.TO_NO;
            DataObject.SHIP_DATE = this.SHIP_DATE;
            DataObject.SHIP_TIME = this.SHIP_TIME;
            DataObject.SEND_CAR_NO = this.SEND_CAR_NO;
            DataObject.CAR_NUMBER = this.CAR_NUMBER;
            DataObject.SHIP_ADDRESS = this.SHIP_ADDRESS;
            DataObject.SHIP_FLAG = this.SHIP_FLAG;
            DataObject.TOTAL_NET_WEIGHT = this.TOTAL_NET_WEIGHT;
            DataObject.TOTAL_GROSS_WEIGHT = this.TOTAL_GROSS_WEIGHT;
            DataObject.TOTAL_PALLET_NUM = this.TOTAL_PALLET_NUM;
            DataObject.TOTAL_CARTON_NUM = this.TOTAL_CARTON_NUM;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.EDIT_BY = this.EDIT_BY;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string SHIP_NO
        {
            get
            {
                return (string)this["SHIP_NO"];
            }
            set
            {
                this["SHIP_NO"] = value;
            }
        }
        public string TO_NO
        {
            get
            {
                return (string)this["TO_NO"];
            }
            set
            {
                this["TO_NO"] = value;
            }
        }
        public string SHIP_DATE
        {
            get
            {
                return (string)this["SHIP_DATE"];
            }
            set
            {
                this["SHIP_DATE"] = value;
            }
        }
        public string SHIP_TIME
        {
            get
            {
                return (string)this["SHIP_TIME"];
            }
            set
            {
                this["SHIP_TIME"] = value;
            }
        }
        public string SEND_CAR_NO
        {
            get
            {
                return (string)this["SEND_CAR_NO"];
            }
            set
            {
                this["SEND_CAR_NO"] = value;
            }
        }
        public string CAR_NUMBER
        {
            get
            {
                return (string)this["CAR_NUMBER"];
            }
            set
            {
                this["CAR_NUMBER"] = value;
            }
        }
        public string SHIP_ADDRESS
        {
            get
            {
                return (string)this["SHIP_ADDRESS"];
            }
            set
            {
                this["SHIP_ADDRESS"] = value;
            }
        }
        public double? SHIP_FLAG
        {
            get
            {
                return (double?)this["SHIP_FLAG"];
            }
            set
            {
                this["SHIP_FLAG"] = value;
            }
        }
        public string TOTAL_NET_WEIGHT
        {
            get
            {
                return (string)this["TOTAL_NET_WEIGHT"];
            }
            set
            {
                this["TOTAL_NET_WEIGHT"] = value;
            }
        }
        public string TOTAL_GROSS_WEIGHT
        {
            get
            {
                return (string)this["TOTAL_GROSS_WEIGHT"];
            }
            set
            {
                this["TOTAL_GROSS_WEIGHT"] = value;
            }
        }
        public double? TOTAL_PALLET_NUM
        {
            get
            {
                return (double?)this["TOTAL_PALLET_NUM"];
            }
            set
            {
                this["TOTAL_PALLET_NUM"] = value;
            }
        }
        public double? TOTAL_CARTON_NUM
        {
            get
            {
                return (double?)this["TOTAL_CARTON_NUM"];
            }
            set
            {
                this["TOTAL_CARTON_NUM"] = value;
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
        public string EDIT_BY
        {
            get
            {
                return (string)this["EDIT_BY"];
            }
            set
            {
                this["EDIT_BY"] = value;
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
    public class R_SHIPPING_NOTICE
    {
        public string SHIP_NO { get; set; }
        public string TO_NO { get; set; }
        public string SHIP_DATE { get; set; }
        public string SHIP_TIME { get; set; }
        public string SEND_CAR_NO { get; set; }
        public string CAR_NUMBER { get; set; }
        public string SHIP_ADDRESS { get; set; }
        public double? SHIP_FLAG { get; set; }
        public string TOTAL_NET_WEIGHT { get; set; }
        public string TOTAL_GROSS_WEIGHT { get; set; }
        public double? TOTAL_PALLET_NUM { get; set; }
        public double? TOTAL_CARTON_NUM { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string EDIT_BY { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}