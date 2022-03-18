using MESDBHelper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{
    public class T_R_JSON : DataObjectTable
    {
        public T_R_JSON(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JSON(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JSON);
            TableName = "R_JSON".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JSON : DataObjectBase
    {
        public Row_R_JSON(DataObjectInfo info) : base(info)
        {

        }
        public R_JSON GetDataObject()
        {
            R_JSON DataObject = new R_JSON();
            DataObject.ID = this.ID;
            DataObject.NAME = this.NAME;
            DataObject.TYPE = this.TYPE;
            DataObject.BLOBDATA = this.BLOBDATA;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.INDEX1 = this.INDEX1;
            DataObject.INDEX2 = this.INDEX2;
            DataObject.INDEX3 = this.INDEX3;
            DataObject.INDEX4 = this.INDEX4;
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
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public byte[] BLOBDATA
        {
            get
            {
                return (byte[])this["BLOBDATA"];
            }
            set
            {
                this["BLOBDATA"] = value;
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
        public string INDEX1
        {
            get
            {
                return (string)this["INDEX1"];
            }
            set
            {
                this["INDEX1"] = value;
            }
        }
        public string INDEX2
        {
            get
            {
                return (string)this["INDEX2"];
            }
            set
            {
                this["INDEX2"] = value;
            }
        }
        public string INDEX3
        {
            get
            {
                return (string)this["INDEX3"];
            }
            set
            {
                this["INDEX3"] = value;
            }
        }
        public string INDEX4
        {
            get
            {
                return (string)this["INDEX4"];
            }
            set
            {
                this["INDEX4"] = value;
            }
        }
    }
    public class R_JSON
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string NAME { get; set; }
        public string TYPE { get; set; }
        [JsonConverter(typeof(Common.ByteToStringConverter))]
        public byte[] BLOBDATA { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string INDEX1 { get; set; }
        public string INDEX2 { get; set; }
        public string INDEX3 { get; set; }
        public string INDEX4 { get; set; }
    }
}
