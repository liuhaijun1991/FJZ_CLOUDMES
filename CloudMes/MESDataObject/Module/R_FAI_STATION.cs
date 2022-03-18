using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_FAI_STATION : DataObjectTable
    {
        public T_R_FAI_STATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FAI_STATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FAI_STATION);
            TableName = "R_FAI_STATION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

    }
    public class Row_R_FAI_STATION : DataObjectBase
    {
        public Row_R_FAI_STATION(DataObjectInfo info) : base(info)
        {

        }
        public R_FAI_STATION GetDataObject()
        {
            R_FAI_STATION DataObject = new R_FAI_STATION();
            DataObject.ID = this.ID;
            DataObject.FAIID = this.FAIID;
            DataObject.FAISTATION = this.FAISTATION;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.STARTSTATION = this.STARTSTATION;
            DataObject.CHECKTIME = this.CHECKTIME;
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
        public string FAIID
        {
            get
            {
                return (string)this["FAIID"];
            }
            set
            {
                this["FAIID"] = value;
            }
        }
        public string FAISTATION
        {
            get
            {
                return (string)this["FAISTATION"];
            }
            set
            {
                this["FAISTATION"] = value;
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
        public string STARTSTATION
        {
            get
            {
                return (string)this["STARTSTATION"];
            }
            set
            {
                this["STARTSTATION"] = value;
            }
        }
        public string CHECKTIME
        {
            get
            {
                return (string)this["CHECKTIME"];
            }
            set
            {
                this["CHECKTIME"] = value;
            }
        }
    }
    public class R_FAI_STATION
    {
        public string ID { get; set; }
        public string FAIID { get; set; }
        public string FAISTATION { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public string STARTSTATION { get; set; }
        public string CHECKTIME { get; set; }
    }
}