using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.DCN
{
    public class T_BROADCOM_CSV_HEAD : DataObjectTable
    {
        public T_BROADCOM_CSV_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_BROADCOM_CSV_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_BROADCOM_CSV_HEAD);
            TableName = "BROADCOM_CSV_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_BROADCOM_CSV_HEAD : DataObjectBase
    {
        public Row_BROADCOM_CSV_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public BROADCOM_CSV_HEAD GetDataObject()
        {
            BROADCOM_CSV_HEAD DataObject = new BROADCOM_CSV_HEAD();
            DataObject.FILENAME = this.FILENAME;
            DataObject.CREATEDT = this.CREATEDT;
            DataObject.STATUS = this.STATUS;
            return DataObject;
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
        public DateTime? CREATEDT
        {
            get
            {
                return (DateTime?)this["CREATEDT"];
            }
            set
            {
                this["CREATEDT"] = value;
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
    }
    public class BROADCOM_CSV_HEAD
    {
        public string FILENAME { get; set; }
        public DateTime? CREATEDT { get; set; }
        public string STATUS { get; set; }
    }
}
