using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class T_O_I138 : DataObjectTable
    {
        public T_O_I138(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_I138(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_I138);
            TableName = "O_I138".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_I138 : DataObjectBase
    {
        public Row_O_I138(DataObjectInfo info) : base(info)
        {

        }
        public O_I138 GetDataObject()
        {
            O_I138 DataObject = new O_I138();
            DataObject.ID = this.ID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATE = this.CREATIONDATE;
            DataObject.PONUMBER = this.PONUMBER;
            DataObject.POCREATIONDATE = this.POCREATIONDATE;
            DataObject.JNP_PLANT = this.JNP_PLANT;
            DataObject.VENDORID = this.VENDORID;
            DataObject.POITEMNUMBER = this.POITEMNUMBER;
            DataObject.PN = this.PN;
            DataObject.NOTE = this.NOTE;
            DataObject.DELIVERYSTARTDATE = this.DELIVERYSTARTDATE;
            DataObject.DELIVERYENDDATE = this.DELIVERYENDDATE;
            DataObject.SHIPPINGSTARTDATE = this.SHIPPINGSTARTDATE;
            DataObject.SHIPPINGENDDATE = this.SHIPPINGENDDATE;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.TRANID = this.TRANID;
            DataObject.CREATETIME = this.CREATETIME;
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
        public DateTime? POCREATIONDATE
        {
            get
            {
                return (DateTime?)this["POCREATIONDATE"];
            }
            set
            {
                this["POCREATIONDATE"] = value;
            }
        }
        public string JNP_PLANT
        {
            get
            {
                return (string)this["JNP_PLANT"];
            }
            set
            {
                this["JNP_PLANT"] = value;
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
        public string POITEMNUMBER
        {
            get
            {
                return (string)this["POITEMNUMBER"];
            }
            set
            {
                this["POITEMNUMBER"] = value;
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
        public string NOTE
        {
            get
            {
                return (string)this["NOTE"];
            }
            set
            {
                this["NOTE"] = value;
            }
        }
        public DateTime? DELIVERYSTARTDATE
        {
            get
            {
                return (DateTime?)this["DELIVERYSTARTDATE"];
            }
            set
            {
                this["DELIVERYSTARTDATE"] = value;
            }
        }
        public DateTime? DELIVERYENDDATE
        {
            get
            {
                return (DateTime?)this["DELIVERYENDDATE"];
            }
            set
            {
                this["DELIVERYENDDATE"] = value;
            }
        }
        public DateTime? SHIPPINGSTARTDATE
        {
            get
            {
                return (DateTime?)this["SHIPPINGSTARTDATE"];
            }
            set
            {
                this["SHIPPINGSTARTDATE"] = value;
            }
        }
        public DateTime? SHIPPINGENDDATE
        {
            get
            {
                return (DateTime?)this["SHIPPINGENDDATE"];
            }
            set
            {
                this["SHIPPINGENDDATE"] = value;
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
    public class O_I138
    {
        public string ID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATE { get; set; }
        public string PONUMBER { get; set; }
        public DateTime? POCREATIONDATE { get; set; }
        public string JNP_PLANT { get; set; }
        public string VENDORID { get; set; }
        public string POITEMNUMBER { get; set; }
        public string PN { get; set; }
        public string NOTE { get; set; }
        public DateTime? DELIVERYSTARTDATE { get; set; }
        public DateTime? DELIVERYENDDATE { get; set; }
        public DateTime? SHIPPINGSTARTDATE { get; set; }
        public DateTime? SHIPPINGENDDATE { get; set; }
        public string QUANTITY { get; set; }
        public string TRANID { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public string SALESORDERNUMBER { get; set; }        
    }


    public enum ENUM_I138_TYPE
    {
        [EnumValue("NEWSCH")]
        NEWSCH,
        [EnumValue("PULLIN")]
        PULLIN,
        [EnumValue("RISK")]
        RISK,
        [EnumValue("PUSHOUT")]
        PUSHOUT
    }
       
    public enum ENUM_I138_PLANT
    {
        [EnumValue("1360")]
        I1360,
        [EnumValue("PULLIN")]
        PULLIN,
        [EnumValue("RISK")]
        RISK,
        [EnumValue("PUSHOUT")]
        PUSHOUT
    }
}