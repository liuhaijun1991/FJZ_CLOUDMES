using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_CUSTPN_MAP : DataObjectTable
    {
        public T_R_CUSTPN_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_CUSTPN_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_CUSTPN_MAP);
            TableName = "R_CUSTPN_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_CUSTPN_MAP : DataObjectBase
    {
        public Row_R_CUSTPN_MAP(DataObjectInfo info) : base(info)
        {

        }
        public R_CUSTPN_MAP GetDataObject()
        {
            R_CUSTPN_MAP DataObject = new R_CUSTPN_MAP();
            DataObject.ID = this.ID;
            DataObject.CUSTPN = this.CUSTPN;
            DataObject.PARTNO = this.PARTNO;
            DataObject.DESCITIONS = this.DESCITIONS;
            DataObject.REV = this.REV;
            DataObject.PLANT = this.PLANT;
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
        public string CUSTPN
        {
            get
            {
                return (string)this["CUSTPN"];
            }
            set
            {
                this["CUSTPN"] = value;
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
        public string DESCITIONS
        {
            get
            {
                return (string)this["DESCITIONS"];
            }
            set
            {
                this["DESCITIONS"] = value;
            }
        }
        public string REV
        {
            get
            {
                return (string)this["REV"];
            }
            set
            {
                this["REV"] = value;
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
    public class R_CUSTPN_MAP
    {
        public string ID { get; set; }
        public string CUSTPN { get; set; }
        public string PARTNO { get; set; }
        public string DESCITIONS { get; set; }
        public string REV { get; set; }
        public string PLANT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}