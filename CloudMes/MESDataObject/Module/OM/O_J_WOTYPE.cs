using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class T_O_J_WOTYPE : DataObjectTable
    {
        public T_O_J_WOTYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_J_WOTYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_J_WOTYPE);
            TableName = "O_J_WOTYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_J_WOTYPE : DataObjectBase
    {
        public Row_O_J_WOTYPE(DataObjectInfo info) : base(info)
        {

        }
        public O_J_WOTYPE GetDataObject()
        {
            O_J_WOTYPE DataObject = new O_J_WOTYPE();
            DataObject.ID = this.ID;
            DataObject.WOTYPE = this.WOTYPE;
            DataObject.WOPRE = this.WOPRE;
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
        public string WOTYPE
        {
            get
            {
                return (string)this["WOTYPE"];
            }
            set
            {
                this["WOTYPE"] = value;
            }
        }
        public string WOPRE
        {
            get
            {
                return (string)this["WOPRE"];
            }
            set
            {
                this["WOPRE"] = value;
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
    public class O_J_WOTYPE
    {
        public string ID { get; set; }
        public string WOTYPE { get; set; }
        public string WOPRE { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}