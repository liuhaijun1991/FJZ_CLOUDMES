using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module
{
    public class T_R_WO_LINK : DataObjectTable
    {
        public T_R_WO_LINK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_LINK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_LINK);
            TableName = "R_WO_LINK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

    }

    public class Row_R_WO_LINK : DataObjectBase
    {
        public Row_R_WO_LINK(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_LINK GetDataObject()
        {
            R_WO_LINK DataObject = new R_WO_LINK
            {
                ID = this.ID,
                LINKTYPE = this.LINKTYPE,
                WO = this.WO,
                LINKWO = this.LINKWO,
                CREATETIME = this.CREATETIME,
                CREATEBY = this.CREATEBY
            };
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
        public string LINKTYPE
        {
            get
            {
                return (string)this["LINKTYPE"];
            }
            set
            {
                this["LINKTYPE"] = value;
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
        public string LINKWO
        {
            get
            {
                return (string)this["LINKWO"];
            }
            set
            {
                this["LINKWO"] = value;
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
    }
    public class R_WO_LINK
    {
        public string ID { get; set; }
        public string LINKTYPE { get; set; }
        public string WO { get; set; }
        public string LINKWO { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }

    }
}
