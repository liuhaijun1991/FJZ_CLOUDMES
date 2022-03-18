using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_TEST_IDENTITY : DataObjectTable
    {
        public T_R_TEST_IDENTITY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_IDENTITY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_IDENTITY);
            TableName = "R_TEST_IDENTITY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_TEST_IDENTITY : DataObjectBase
    {
        public Row_R_TEST_IDENTITY(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_IDENTITY GetDataObject()
        {
            R_TEST_IDENTITY DataObject = new R_TEST_IDENTITY();
            DataObject.VALIDFLAG = this.VALIDFLAG;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.EDITBY = this.EDITBY;
            DataObject.STATION = this.STATION;
            DataObject.VAL = this.VAL;
            DataObject.CATEGORYNAME = this.CATEGORYNAME;
            DataObject.SN = this.SN;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string VALIDFLAG
        {
            get
            {
                return (string)this["VALIDFLAG"];
            }
            set
            {
                this["VALIDFLAG"] = value;
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
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string VAL
        {
            get
            {
                return (string)this["VAL"];
            }
            set
            {
                this["VAL"] = value;
            }
        }
        public string CATEGORYNAME
        {
            get
            {
                return (string)this["CATEGORYNAME"];
            }
            set
            {
                this["CATEGORYNAME"] = value;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
    }
    public class R_TEST_IDENTITY
    {
        public string VALIDFLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string EDITBY { get; set; }
        public string STATION { get; set; }
        public string VAL { get; set; }
        public string CATEGORYNAME { get; set; }
        public string SN { get; set; }
        public string R_SN_ID { get; set; }
        public string ID { get; set; }
    }
}