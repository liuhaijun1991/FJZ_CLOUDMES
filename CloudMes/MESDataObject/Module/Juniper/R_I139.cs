using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_I139 : DataObjectTable
    {
        public T_R_I139(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I139(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I139);
            TableName = "R_I139".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I139 : DataObjectBase
    {
        public Row_R_I139(DataObjectInfo info) : base(info)
        {

        }
        public R_I139 GetDataObject()
        {
            R_I139 DataObject = new R_I139();
            DataObject.CARRIERNAME = this.CARRIERNAME;
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATE = this.CREATIONDATE;
            DataObject.RECIPIENTID = this.RECIPIENTID;
            DataObject.DELIVERYCODE = this.DELIVERYCODE;
            DataObject.ASNNUMBER = this.ASNNUMBER;
            DataObject.ASNCREATIONTIME = this.ASNCREATIONTIME;
            DataObject.VENDORID = this.VENDORID;
            DataObject.SHIPTOID = this.SHIPTOID;
            DataObject.GROSSWEIGHT = this.GROSSWEIGHT;
            DataObject.GROSSCODE = this.GROSSCODE;
            DataObject.NETWEIGHT = this.NETWEIGHT;
            DataObject.NETCODE = this.NETCODE;
            DataObject.VOLUMEWEIGHT = this.VOLUMEWEIGHT;
            DataObject.VOLUMECODE = this.VOLUMECODE;
            DataObject.ARRIVALDATE = this.ARRIVALDATE;
            DataObject.ISSUEDATE = this.ISSUEDATE;
            DataObject.TRACKINGID = this.TRACKINGID;
            DataObject.WAYBILLID = this.WAYBILLID;
            DataObject.FREIGHTINVOICEID = this.FREIGHTINVOICEID;
            DataObject.CLASSIFICATIONCODE = this.CLASSIFICATIONCODE;
            DataObject.TRANSFERLOCATIONNAME = this.TRANSFERLOCATIONNAME;
            DataObject.LINE = this.LINE;
            DataObject.PONUMBER = this.PONUMBER;
            DataObject.ITEM = this.ITEM;
            DataObject.PN = this.PN;
            DataObject.SPECIALREQUEST = this.SPECIALREQUEST;
            DataObject.COO = this.COO;
            DataObject.SERIALID = this.SERIALID;
            DataObject.SHIPPEDQUANTITY = this.SHIPPEDQUANTITY;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.TRANID = this.TRANID;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }
        public string CARRIERNAME
        {
            get
            {
                return (string)this["CARRIERNAME"];
            }
            set
            {
                this["CARRIERNAME"] = value;
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
        public double? F_ID
        {
            get
            {
                return (double?)this["F_ID"];
            }
            set
            {
                this["F_ID"] = value;
            }
        }
        public string F_PLANT
        {
            get
            {
                return (string)this["F_PLANT"];
            }
            set
            {
                this["F_PLANT"] = value;
            }
        }
        public string FILENAME
        {
            get
            {
                return (string)this["FILENAME"];
            }
            set
            {
                this["FILENAME"] = value;
            }
        }
        public string MESSAGEID
        {
            get
            {
                return (string)this["MESSAGEID"];
            }
            set
            {
                this["MESSAGEID"] = value;
            }
        }
        public DateTime? CREATIONDATE
        {
            get
            {
                return (DateTime?)this["CREATIONDATE"];
            }
            set
            {
                this["CREATIONDATE"] = value;
            }
        }
        public string RECIPIENTID
        {
            get
            {
                return (string)this["RECIPIENTID"];
            }
            set
            {
                this["RECIPIENTID"] = value;
            }
        }
        public string DELIVERYCODE
        {
            get
            {
                return (string)this["DELIVERYCODE"];
            }
            set
            {
                this["DELIVERYCODE"] = value;
            }
        }
        public string ASNNUMBER
        {
            get
            {
                return (string)this["ASNNUMBER"];
            }
            set
            {
                this["ASNNUMBER"] = value;
            }
        }
        public DateTime? ASNCREATIONTIME
        {
            get
            {
                return (DateTime?)this["ASNCREATIONTIME"];
            }
            set
            {
                this["ASNCREATIONTIME"] = value;
            }
        }
        public string VENDORID
        {
            get
            {
                return (string)this["VENDORID"];
            }
            set
            {
                this["VENDORID"] = value;
            }
        }
        public string SHIPTOID
        {
            get
            {
                return (string)this["SHIPTOID"];
            }
            set
            {
                this["SHIPTOID"] = value;
            }
        }
        public string GROSSWEIGHT
        {
            get
            {
                return (string)this["GROSSWEIGHT"];
            }
            set
            {
                this["GROSSWEIGHT"] = value;
            }
        }
        public string GROSSCODE
        {
            get
            {
                return (string)this["GROSSCODE"];
            }
            set
            {
                this["GROSSCODE"] = value;
            }
        }
        public string NETWEIGHT
        {
            get
            {
                return (string)this["NETWEIGHT"];
            }
            set
            {
                this["NETWEIGHT"] = value;
            }
        }
        public string NETCODE
        {
            get
            {
                return (string)this["NETCODE"];
            }
            set
            {
                this["NETCODE"] = value;
            }
        }
        public string VOLUMEWEIGHT
        {
            get
            {
                return (string)this["VOLUMEWEIGHT"];
            }
            set
            {
                this["VOLUMEWEIGHT"] = value;
            }
        }
        public string VOLUMECODE
        {
            get
            {
                return (string)this["VOLUMECODE"];
            }
            set
            {
                this["VOLUMECODE"] = value;
            }
        }
        public DateTime? ARRIVALDATE
        {
            get
            {
                return (DateTime?)this["ARRIVALDATE"];
            }
            set
            {
                this["ARRIVALDATE"] = value;
            }
        }
        public DateTime? ISSUEDATE
        {
            get
            {
                return (DateTime?)this["ISSUEDATE"];
            }
            set
            {
                this["ISSUEDATE"] = value;
            }
        }
        public string TRACKINGID
        {
            get
            {
                return (string)this["TRACKINGID"];
            }
            set
            {
                this["TRACKINGID"] = value;
            }
        }
        public string WAYBILLID
        {
            get
            {
                return (string)this["WAYBILLID"];
            }
            set
            {
                this["WAYBILLID"] = value;
            }
        }
        public string FREIGHTINVOICEID
        {
            get
            {
                return (string)this["FREIGHTINVOICEID"];
            }
            set
            {
                this["FREIGHTINVOICEID"] = value;
            }
        }
        public string CLASSIFICATIONCODE
        {
            get
            {
                return (string)this["CLASSIFICATIONCODE"];
            }
            set
            {
                this["CLASSIFICATIONCODE"] = value;
            }
        }
        public string TRANSFERLOCATIONNAME
        {
            get
            {
                return (string)this["TRANSFERLOCATIONNAME"];
            }
            set
            {
                this["TRANSFERLOCATIONNAME"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
        public string PONUMBER
        {
            get
            {
                return (string)this["PONUMBER"];
            }
            set
            {
                this["PONUMBER"] = value;
            }
        }
        public string ITEM
        {
            get
            {
                return (string)this["ITEM"];
            }
            set
            {
                this["ITEM"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string SPECIALREQUEST
        {
            get
            {
                return (string)this["SPECIALREQUEST"];
            }
            set
            {
                this["SPECIALREQUEST"] = value;
            }
        }
        public string COO
        {
            get
            {
                return (string)this["COO"];
            }
            set
            {
                this["COO"] = value;
            }
        }
        public string SERIALID
        {
            get
            {
                return (string)this["SERIALID"];
            }
            set
            {
                this["SERIALID"] = value;
            }
        }
        public string SHIPPEDQUANTITY
        {
            get
            {
                return (string)this["SHIPPEDQUANTITY"];
            }
            set
            {
                this["SHIPPEDQUANTITY"] = value;
            }
        }
        public string QUANTITY
        {
            get
            {
                return (string)this["QUANTITY"];
            }
            set
            {
                this["QUANTITY"] = value;
            }
        }
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
    }
    public class R_I139
    {
        public string CARRIERNAME { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATE { get; set; }
        public string RECIPIENTID { get; set; }
        public string DELIVERYCODE { get; set; }
        public string ASNNUMBER { get; set; }
        public DateTime? ASNCREATIONTIME { get; set; }
        public string VENDORID { get; set; }
        public string SHIPTOID { get; set; }
        public string GROSSWEIGHT { get; set; }
        public string GROSSCODE { get; set; }
        public string NETWEIGHT { get; set; }
        public string NETCODE { get; set; }
        public string VOLUMEWEIGHT { get; set; }
        public string VOLUMECODE { get; set; }
        public DateTime? ARRIVALDATE { get; set; }
        public DateTime? ISSUEDATE { get; set; }
        public string TRACKINGID { get; set; }
        public string WAYBILLID { get; set; }
        public string FREIGHTINVOICEID { get; set; }
        public string CLASSIFICATIONCODE { get; set; }
        public string TRANSFERLOCATIONNAME { get; set; }
        public string LINE { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string PN { get; set; }
        public string SPECIALREQUEST { get; set; }
        public string COO { get; set; }
        public string SERIALID { get; set; }
        public string SHIPPEDQUANTITY { get; set; }
        public string QUANTITY { get; set; }
        public string TRANID { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}