using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_LABEL_PRINT_T : DataObjectTable
    {
        public T_R_LABEL_PRINT_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LABEL_PRINT_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LABEL_PRINT_T);
            TableName = "R_LABEL_PRINT_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_LABEL_PRINT_T : DataObjectBase
    {
        public Row_R_LABEL_PRINT_T(DataObjectInfo info) : base(info)
        {

        }
        public R_LABEL_PRINT_T GetDataObject()
        {
            R_LABEL_PRINT_T DataObject = new R_LABEL_PRINT_T();
            DataObject.ID = this.ID;
            DataObject.WO = this.WO;
            DataObject.SN = this.SN;
            DataObject.SN_2D = this.SN_2D;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string SN_2D
        {
            get
            {
                return (string)this["SN_2D"];
            }
            set
            {
                this["SN_2D"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
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
    }
    public class R_LABEL_PRINT_T
    {
        public string ID { get; set; }
        public string WO { get; set; }
        public string SN { get; set; }
        public string SN_2D { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}