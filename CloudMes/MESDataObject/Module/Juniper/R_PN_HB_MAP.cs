using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_PN_HB_MAP : DataObjectTable
    {
        public T_R_PN_HB_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PN_HB_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PN_HB_MAP);
            TableName = "R_PN_HB_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_PN_HB_MAP : DataObjectBase
    {
        public Row_R_PN_HB_MAP(DataObjectInfo info) : base(info)
        {

        }
        public R_PN_HB_MAP GetDataObject()
        {
            R_PN_HB_MAP DataObject = new R_PN_HB_MAP();
            DataObject.ID = this.ID;
            DataObject.CUSTPN = this.CUSTPN;
            DataObject.HBPN = this.HBPN;
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
        public string HBPN
        {
            get
            {
                return (string)this["HBPN"];
            }
            set
            {
                this["HBPN"] = value;
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
    public class R_PN_HB_MAP
    {
        public string ID { get; set; }
        public string CUSTPN { get; set; }
        public string HBPN { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}