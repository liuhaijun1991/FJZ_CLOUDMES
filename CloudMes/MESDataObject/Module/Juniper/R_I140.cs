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
    public class T_R_I140 : DataObjectTable
    {
        public T_R_I140(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }

        public T_R_I140(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I140);
            TableName = "R_I140".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }

    public class Row_R_I140 : DataObjectBase
    {
        public Row_R_I140(DataObjectInfo info) : base(info)
        {

        }

        public R_I140 GetDataObject()
        {
            R_I140 DataObject = new R_I140();
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.TRANID = this.TRANID;
            DataObject.F_PLANT = this.F_PLANT;
            DataObject.FILENAME = this.FILENAME;
            DataObject.MESSAGEID = this.MESSAGEID;
            DataObject.CREATIONDATETIME = this.CREATIONDATETIME;
            DataObject.VENDORCODE = this.VENDORCODE;
            DataObject.PN = this.PN;
            DataObject.STARTDATETIME = this.STARTDATETIME;
            DataObject.ENDDATETIME = this.ENDDATETIME;
            DataObject.QUANTITY = this.QUANTITY;
            DataObject.F_LASTEDITDT = this.F_LASTEDITDT;
            DataObject.MFLAG = this.MFLAG;
            DataObject.CREATETIME = this.CREATETIME;
            return DataObject;
        }

        public string ID
        {
            get { return (string) this["ID"]; }
            set { this["ID"] = value; }
        }

        public string F_ID
        {
            get { return (string) this["F_ID"]; }
            set { this["F_ID"] = value; }
        }

        public string TRANID
        {
            get { return (string) this["TRANID"]; }
            set { this["TRANID"] = value; }
        }

        public string F_PLANT
        {
            get { return (string) this["F_PLANT"]; }
            set { this["F_PLANT"] = value; }
        }

        public string FILENAME
        {
            get { return (string) this["FILENAME"]; }
            set { this["FILENAME"] = value; }
        }

        public string MESSAGEID
        {
            get { return (string) this["MESSAGEID"]; }
            set { this["MESSAGEID"] = value; }
        }

        public DateTime? CREATIONDATETIME
        {
            get { return (DateTime?) this["CREATIONDATETIME"]; }
            set { this["CREATIONDATETIME"] = value; }
        }

        public string VENDORCODE
        {
            get { return (string) this["VENDORCODE"]; }
            set { this["VENDORCODE"] = value; }
        }

        public string PN
        {
            get { return (string) this["PN"]; }
            set { this["PN"] = value; }
        }

        public DateTime? STARTDATETIME
        {
            get { return (DateTime?) this["STARTDATETIME"]; }
            set { this["STARTDATETIME"] = value; }
        }

        public DateTime? ENDDATETIME
        {
            get { return (DateTime?) this["ENDDATETIME"]; }
            set { this["ENDDATETIME"] = value; }
        }

        public string QUANTITY
        {
            get { return (string) this["QUANTITY"]; }
            set { this["QUANTITY"] = value; }
        }

        public DateTime? F_LASTEDITDT
        {
            get { return (DateTime?) this["F_LASTEDITDT"]; }
            set { this["F_LASTEDITDT"] = value; }
        }

        public string MFLAG
        {
            get { return (string) this["MFLAG"]; }
            set { this["MFLAG"] = value; }
        }

        public DateTime? CREATETIME
        {
            get { return (DateTime?) this["CREATETIME"]; }
            set { this["CREATETIME"] = value; }
        }
    }

    public class R_I140
    {
        public string ID { get; set; }
        public string F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATETIME { get; set; }
        public string VENDORCODE  { get; set; }
        public string PN { get; set; }
        public DateTime? STARTDATETIME { get; set; }
        public DateTime? ENDDATETIME { get; set; }
        public string QUANTITY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public string MFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
    }

    [SqlSugar.SugarTable("jnp.tb_i140")]
    public class B2B_R_I140
    {
        [SugarColumn(ColumnName = "ID")]
        public string F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public DateTime? CREATIONDATETIME { get; set; }
        public string VENDORCODE { get; set; }
        public string PN { get; set; }
        public DateTime? STARTDATETIME { get; set; }
        public DateTime? ENDDATETIME { get; set; }
        public string QUANTITY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
    }
}