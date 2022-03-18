using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_DN_DETAIL : DataObjectTable
    {
        public T_R_DN_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_DN_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_DN_DETAIL);
            TableName = "R_DN_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_DN_DETAIL> GetDNDetailListByTO(OleExec sfcdb, string to_no)
        {
            return sfcdb.ORM.Queryable<R_DN_DETAIL, R_TO_DETAIL_HWT>((rdd, rtd) => rdd.DN_NO == rtd.DN_NO).Where((rdd, rtd) => rtd.TO_NO == to_no).Select((rdd, rtd) => rdd).ToList();
        }
        public List<R_DN_DETAIL> GetListByDNNO(OleExec sfcdb, string dn_no)
        {
            return sfcdb.ORM.Queryable<R_DN_DETAIL>().Where(r => r.DN_NO == dn_no).ToList();
        }
        public R_DN_DETAIL GetDetailByDNNO(OleExec sfcdb, string dn_no,string dn_item)
        {
            return sfcdb.ORM.Queryable<R_DN_DETAIL>().Where(r => r.DN_NO == dn_no && r.DN_ITEM_NO == dn_item).ToList().FirstOrDefault();
        }
    }
    public class Row_R_DN_DETAIL : DataObjectBase
    {
        public Row_R_DN_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_DN_DETAIL GetDataObject()
        {
            R_DN_DETAIL DataObject = new R_DN_DETAIL();
            DataObject.ID = this.ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_ITEM_NO = this.DN_ITEM_NO;
            DataObject.P_NO = this.P_NO;
            DataObject.P_NO_DESC = this.P_NO_DESC;
            DataObject.NET_WEIGHT = this.NET_WEIGHT;
            DataObject.GROSS_WEIGHT = this.GROSS_WEIGHT;
            DataObject.VOLUME = this.VOLUME;
            DataObject.PRICE = this.PRICE;
            DataObject.P_NO_QTY = this.P_NO_QTY;
            DataObject.REAL_QTY = this.REAL_QTY;
            DataObject.SO_NO = this.SO_NO;
            DataObject.SO_ITEM_NO = this.SO_ITEM_NO;
            DataObject.PO_NO = this.PO_NO;
            DataObject.DN_ITEM_FLAG = this.DN_ITEM_FLAG;
            DataObject.UNIT = this.UNIT;
            DataObject.VERSION = this.VERSION;
            DataObject.WAREHOUSE = this.WAREHOUSE;
            DataObject.STATUS = this.STATUS;
            DataObject.SHIPTYPE = this.SHIPTYPE;
            DataObject.SECOND_FLAG = this.SECOND_FLAG;
            DataObject.SECOND_STARTTIME = this.SECOND_STARTTIME;
            DataObject.SECOND_ENDTIME = this.SECOND_ENDTIME;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.DATA1 = this.DATA1;
            DataObject.CUST_P_NO = this.CUST_P_NO;
            DataObject.PLANT = this.PLANT;
            DataObject.NN_GT = this.NN_GT;
            DataObject.P_VERSION = this.P_VERSION;
            DataObject.CUSTOMER_KP_NO = this.CUSTOMER_KP_NO;
            DataObject.CUSTOMER_KP_NO_VER = this.CUSTOMER_KP_NO_VER;
            DataObject.CUST_CODE = this.CUST_CODE;
            DataObject.CREATE_TIME = this.CREATE_TIME;
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
        public string DN_ITEM_NO
        {
            get
            {
                return (string)this["DN_ITEM_NO"];
            }
            set
            {
                this["DN_ITEM_NO"] = value;
            }
        }
        public string P_NO
        {
            get
            {
                return (string)this["P_NO"];
            }
            set
            {
                this["P_NO"] = value;
            }
        }
        public string P_NO_DESC
        {
            get
            {
                return (string)this["P_NO_DESC"];
            }
            set
            {
                this["P_NO_DESC"] = value;
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
        public string VOLUME
        {
            get
            {
                return (string)this["VOLUME"];
            }
            set
            {
                this["VOLUME"] = value;
            }
        }
        public string PRICE
        {
            get
            {
                return (string)this["PRICE"];
            }
            set
            {
                this["PRICE"] = value;
            }
        }
        public double? P_NO_QTY
        {
            get
            {
                return (double?)this["P_NO_QTY"];
            }
            set
            {
                this["P_NO_QTY"] = value;
            }
        }
        public double? REAL_QTY
        {
            get
            {
                return (double?)this["REAL_QTY"];
            }
            set
            {
                this["REAL_QTY"] = value;
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
        public string SO_ITEM_NO
        {
            get
            {
                return (string)this["SO_ITEM_NO"];
            }
            set
            {
                this["SO_ITEM_NO"] = value;
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
        public string DN_ITEM_FLAG
        {
            get
            {
                return (string)this["DN_ITEM_FLAG"];
            }
            set
            {
                this["DN_ITEM_FLAG"] = value;
            }
        }
        public string UNIT
        {
            get
            {
                return (string)this["UNIT"];
            }
            set
            {
                this["UNIT"] = value;
            }
        }
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string WAREHOUSE
        {
            get
            {
                return (string)this["WAREHOUSE"];
            }
            set
            {
                this["WAREHOUSE"] = value;
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
        public string SHIPTYPE
        {
            get
            {
                return (string)this["SHIPTYPE"];
            }
            set
            {
                this["SHIPTYPE"] = value;
            }
        }
        public string SECOND_FLAG
        {
            get
            {
                return (string)this["SECOND_FLAG"];
            }
            set
            {
                this["SECOND_FLAG"] = value;
            }
        }
        public DateTime? SECOND_STARTTIME
        {
            get
            {
                return (DateTime?)this["SECOND_STARTTIME"];
            }
            set
            {
                this["SECOND_STARTTIME"] = value;
            }
        }
        public DateTime? SECOND_ENDTIME
        {
            get
            {
                return (DateTime?)this["SECOND_ENDTIME"];
            }
            set
            {
                this["SECOND_ENDTIME"] = value;
            }
        }
        public string SEND_FLAG
        {
            get
            {
                return (string)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
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
        public string CUST_P_NO
        {
            get
            {
                return (string)this["CUST_P_NO"];
            }
            set
            {
                this["CUST_P_NO"] = value;
            }
        }
        public string PLANT
        {
            get
            {
                return (string)this["PLANT"];
            }
            set
            {
                this["PLANT"] = value;
            }
        }
        public string NN_GT
        {
            get
            {
                return (string)this["NN_GT"];
            }
            set
            {
                this["NN_GT"] = value;
            }
        }
        public string P_VERSION
        {
            get
            {
                return (string)this["P_VERSION"];
            }
            set
            {
                this["P_VERSION"] = value;
            }
        }
        public string CUSTOMER_KP_NO
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO"];
            }
            set
            {
                this["CUSTOMER_KP_NO"] = value;
            }
        }
        public string CUSTOMER_KP_NO_VER
        {
            get
            {
                return (string)this["CUSTOMER_KP_NO_VER"];
            }
            set
            {
                this["CUSTOMER_KP_NO_VER"] = value;
            }
        }
        public string CUST_CODE
        {
            get
            {
                return (string)this["CUST_CODE"];
            }
            set
            {
                this["CUST_CODE"] = value;
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
    }
    public class R_DN_DETAIL
    {
        public string ID { get; set; }
        public string DN_NO { get; set; }
        public string DN_ITEM_NO { get; set; }
        public string P_NO { get; set; }
        public string P_NO_DESC { get; set; }
        public string NET_WEIGHT { get; set; }
        public string GROSS_WEIGHT { get; set; }
        public string VOLUME { get; set; }
        public string PRICE { get; set; }
        public double? P_NO_QTY { get; set; }
        public double? REAL_QTY { get; set; }
        public string SO_NO { get; set; }
        public string SO_ITEM_NO { get; set; }
        public string PO_NO { get; set; }
        public string DN_ITEM_FLAG { get; set; }
        public string UNIT { get; set; }
        public string VERSION { get; set; }
        public string WAREHOUSE { get; set; }
        public string STATUS { get; set; }
        public string SHIPTYPE { get; set; }
        public string SECOND_FLAG { get; set; }
        public DateTime? SECOND_STARTTIME { get; set; }
        public DateTime? SECOND_ENDTIME { get; set; }
        public string SEND_FLAG { get; set; }
        public string DATA1 { get; set; }
        public string CUST_P_NO { get; set; }
        public string PLANT { get; set; }
        public string NN_GT { get; set; }
        public string P_VERSION { get; set; }
        public string CUSTOMER_KP_NO { get; set; }
        public string CUSTOMER_KP_NO_VER { get; set; }
        public string CUST_CODE { get; set; }
        public DateTime? CREATE_TIME { get; set; }
    }
}