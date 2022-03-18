using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MODELSUBPN_MAP : DataObjectTable
    {
        public T_R_MODELSUBPN_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MODELSUBPN_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MODELSUBPN_MAP);
            TableName = "R_MODELSUBPN_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MODELSUBPN_MAP : DataObjectBase
    {
        public Row_R_MODELSUBPN_MAP(DataObjectInfo info) : base(info)
        {

        }
        public R_MODELSUBPN_MAP GetDataObject()
        {
            R_MODELSUBPN_MAP DataObject = new R_MODELSUBPN_MAP();
            DataObject.ID = this.ID;
            DataObject.CUSTPN = this.CUSTPN;
            DataObject.PARTNO = this.PARTNO;
            DataObject.SUBPARTNO = this.SUBPARTNO;
            DataObject.SUBPNREV = this.SUBPNREV;
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
        public string SUBPARTNO
        {
            get
            {
                return (string)this["SUBPARTNO"];
            }
            set
            {
                this["SUBPARTNO"] = value;
            }
        }
        public string SUBPNREV
        {
            get
            {
                return (string)this["SUBPNREV"];
            }
            set
            {
                this["SUBPNREV"] = value;
            }
        }
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
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
    public class R_MODELSUBPN_MAP
    {
        public string ID { get; set; }
        public string CUSTPN { get; set; }
        public string PARTNO { get; set; }
        public string SUBPARTNO { get; set; }
        public string SUBPNREV { get; set; }
        public string FLAG { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}