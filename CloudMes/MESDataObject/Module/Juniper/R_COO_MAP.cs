using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.Juniper
{
    public class T_R_COO_MAP : DataObjectTable
    {
        public T_R_COO_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_COO_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_COO_MAP);
            TableName = "R_COO_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_COO_MAP : DataObjectBase
    {
        public Row_R_COO_MAP(DataObjectInfo info) : base(info)
        {

        }
        public R_COO_MAP GetDataObject()
        {
            R_COO_MAP DataObject = new R_COO_MAP();
            DataObject.ID = this.ID;
            DataObject.CODE = this.CODE;
            DataObject.COUNTRY = this.COUNTRY;
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
        public string CODE
        {
            get
            {
                return (string)this["CODE"];
            }
            set
            {
                this["CODE"] = value;
            }
        }
        public string COUNTRY
        {
            get
            {
                return (string)this["COUNTRY"];
            }
            set
            {
                this["COUNTRY"] = value;
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
    public class R_COO_MAP
    {
        public string ID { get; set; }
        public string CODE { get; set; }
        public string COUNTRY { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}


//using System;

//namespace MESDataObject.Module.Juniper
//{
//    public class R_COO_MAP
//    {
//        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
//        public string ID { get; set; }
//        public string CODE { get; set; }
//        public string COUNTRY { get; set; }
//        public DateTime? CREATETIME { get; set; }
//    }
//}
