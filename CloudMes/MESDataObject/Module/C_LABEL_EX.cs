using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_c_label_ex : DataObjectTable
    {
        public T_c_label_ex(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_c_label_ex(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_c_label_ex);
            TableName = "c_label_ex".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<c_label_ex> GetKPRecordBySnID(string SKUNOID, OleExec SFCDB)
        {

            return SFCDB.ORM.Queryable<c_label_ex>().Where(t => t.ID == SKUNOID).ToList();
        }
    }
    public class Row_c_label_ex : DataObjectBase
    {
        public Row_c_label_ex(DataObjectInfo info) : base(info)
        {

        }
        public c_label_ex GetDataObject()
        {
            c_label_ex DataObject = new c_label_ex();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
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
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
    }
    public class c_label_ex
    {
        public string ID { get; set; }
        public double? SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }
}