using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module.Juniper
{
    public class T_R_I282 : DataObjectTable
    {
        public T_R_I282(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I282(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I282);
            TableName = "R_I282".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I282 : DataObjectBase
    {
        public Row_R_I282(DataObjectInfo info) : base(info)
        {

        }
        public R_I282 GetDataObject()
        {
            R_I282 DataObject = new R_I282();
            DataObject.ERRORCODE = this.ERRORCODE;
            DataObject.ERRORDESCRIPTION = this.ERRORDESCRIPTION;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.ID = this.ID;
            //DataObject.F_ID = this.F_ID;
            DataObject.TRANID = this.TRANID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATETIME = this.CREATIONDATETIME;
            DataObject.ASNNUMBER = this.ASNNUMBER;
            DataObject.VENDORID = this.VENDORID;
            DataObject.DELIVERYNUMBER = this.DELIVERYNUMBER;
            DataObject.SHIPTOPARTYNAME = this.SHIPTOPARTYNAME;
            DataObject.SHIPTOPARTYADDRESS2 = this.SHIPTOPARTYADDRESS2;
            DataObject.SHIPTOPARTYCITY = this.SHIPTOPARTYCITY;
            DataObject.SHIPTOPARTYCOUNTRY = this.SHIPTOPARTYCOUNTRY;
            DataObject.SHIPTOPARTYZIPCODE = this.SHIPTOPARTYZIPCODE;
            DataObject.ITEMID = this.ITEMID;
            DataObject.PRODUCTCODE = this.PRODUCTCODE;
            return DataObject;
        }
        public string ERRORCODE
        {
            get
            {
                return (string)this["ERRORCODE"];
            }
            set
            {
                this["ERRORCODE"] = value;
            }
        }
        public string ERRORDESCRIPTION
        {
            get
            {
                return (string)this["ERRORDESCRIPTION"];
            }
            set
            {
                this["ERRORDESCRIPTION"] = value;
            }
        }
        public DateTime? F_LASTEDITDT
        {
            get
            {
                return (DateTime?)this["F_LASTEDITDT"];
            }
            set
            {
                this["F_LASTEDITDT"] = value;
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
        public DateTime? CREATIONDATETIME
        {
            get
            {
                return (DateTime?)this["CREATIONDATETIME"];
            }
            set
            {
                this["CREATIONDATETIME"] = value;
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
        public string DELIVERYNUMBER
        {
            get
            {
                return (string)this["DELIVERYNUMBER"];
            }
            set
            {
                this["DELIVERYNUMBER"] = value;
            }
        }
        public string SHIPTOPARTYNAME
        {
            get
            {
                return (string)this["SHIPTOPARTYNAME"];
            }
            set
            {
                this["SHIPTOPARTYNAME"] = value;
            }
        }
        public string SHIPTOPARTYADDRESS2
        {
            get
            {
                return (string)this["SHIPTOPARTYADDRESS2"];
            }
            set
            {
                this["SHIPTOPARTYADDRESS2"] = value;
            }
        }
        public string SHIPTOPARTYCITY
        {
            get
            {
                return (string)this["SHIPTOPARTYCITY"];
            }
            set
            {
                this["SHIPTOPARTYCITY"] = value;
            }
        }
        public string SHIPTOPARTYCOUNTRY
        {
            get
            {
                return (string)this["SHIPTOPARTYCOUNTRY"];
            }
            set
            {
                this["SHIPTOPARTYCOUNTRY"] = value;
            }
        }
        public string SHIPTOPARTYZIPCODE
        {
            get
            {
                return (string)this["SHIPTOPARTYZIPCODE"];
            }
            set
            {
                this["SHIPTOPARTYZIPCODE"] = value;
            }
        }
        public string ITEMID
        {
            get
            {
                return (string)this["ITEMID"];
            }
            set
            {
                this["ITEMID"] = value;
            }
        }
        public string PRODUCTCODE
        {
            get
            {
                return (string)this["PRODUCTCODE"];
            }
            set
            {
                this["PRODUCTCODE"] = value;
            }
        }
    }
    public class R_I282
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string ERRORCODE { get; set; }
        public string ERRORDESCRIPTION { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATETIME { get; set; }
        public string ASNNUMBER { get; set; }
        public string VENDORID { get; set; }
        public string DELIVERYNUMBER { get; set; }
        public string SHIPTOPARTYNAME { get; set; }
        public string SHIPTOPARTYADDRESS2 { get; set; }
        public string SHIPTOPARTYCITY { get; set; }
        public string SHIPTOPARTYCOUNTRY { get; set; }
        public string SHIPTOPARTYZIPCODE { get; set; }
        public string ITEMID { get; set; }
        public string PRODUCTCODE { get; set; }
        public string MFLAG { get; set; }
        public string ERRORCORRECTIVEACTION { get; set; }
    }

    [SqlSugar.SugarTable("jnp.TB_I282")]
    public class B2B_R_I282
    {
        public string ERRORCODE { get; set; }
        public string ERRORDESCRIPTION { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATETIME { get; set; }
        public string ASNNUMBER { get; set; }
        public string VENDORID { get; set; }
        public string DELIVERYNUMBER { get; set; }
        public string SHIPTOPARTYNAME { get; set; }
        public string SHIPTOPARTYADDRESS2 { get; set; }
        public string SHIPTOPARTYCITY { get; set; }
        public string SHIPTOPARTYCOUNTRY { get; set; }
        public string SHIPTOPARTYZIPCODE { get; set; }
        public string ITEMID { get; set; }
        public string PRODUCTCODE { get; set; }
        public string ERRORCORRECTIVEACTION { get; set; }
    }
}