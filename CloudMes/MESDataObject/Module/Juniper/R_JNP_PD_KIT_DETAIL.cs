using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_JNP_PD_KIT_DETAIL : DataObjectTable
    {
        public T_R_JNP_PD_KIT_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JNP_PD_KIT_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JNP_PD_KIT_DETAIL);
            TableName = "R_JNP_PD_KIT_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JNP_PD_KIT_DETAIL : DataObjectBase
    {
        public Row_R_JNP_PD_KIT_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_JNP_PD_KIT_DETAIL GetDataObject()
        {
            R_JNP_PD_KIT_DETAIL DataObject = new R_JNP_PD_KIT_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.WO = this.WO;
            DataObject.PARTNO = this.PARTNO;
            DataObject.SN = this.SN;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            DataObject.REQUESTQTY = this.REQUESTQTY;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_BY = this.EDIT_BY;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
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
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public double? REQUESTQTY
        {
            get
            {
                return (double?)this["REQUESTQTY"];
            }
            set
            {
                this["REQUESTQTY"] = value;
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
    }
    public class R_JNP_PD_KIT_DETAIL
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string WO { get; set; }
        public string PARTNO { get; set; }
        public string SN { get; set; }
        public string VALID_FLAG { get; set; }
        public double? REQUESTQTY { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_BY { get; set; }
    }
}