using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWD
{
    public class T_R_7B5_PGI_DATA : DataObjectTable
    {
        public T_R_7B5_PGI_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_7B5_PGI_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_7B5_PGI_DATA);
            TableName = "R_7B5_PGI_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_7B5_PGI_DATA> GetListByTaskAndLot(OleExec sfcdb, string task_no, string lot_no)
        {
            return sfcdb.ORM.Queryable<R_7B5_PGI_DATA>().Where(r => r.TASK_NO == task_no && r.LOT_NO == lot_no).ToList();
        }
    }
    public class Row_R_7B5_PGI_DATA : DataObjectBase
    {
        public Row_R_7B5_PGI_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_7B5_PGI_DATA GetDataObject()
        {
            R_7B5_PGI_DATA DataObject = new R_7B5_PGI_DATA();
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.TASK_NO = this.TASK_NO;
            DataObject.SO = this.SO;
            DataObject.DN = this.DN;
            DataObject.POST_TIME = this.POST_TIME;
            DataObject.POST_DATE = this.POST_DATE;
            DataObject.MEINS = this.MEINS;
            DataObject.QTY = this.QTY;
            DataObject.WORK_TYPE = this.WORK_TYPE;
            DataObject.PGI_STORAGE = this.PGI_STORAGE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PLANT = this.PLANT;
            DataObject.CLIENT = this.CLIENT;
            DataObject.DOC_NO = this.DOC_NO;
            return DataObject;
        }
        public string LOT_NO
        {
            get
            {
                return (string)this["LOT_NO"];
            }
            set
            {
                this["LOT_NO"] = value;
            }
        }
        public string TASK_NO
        {
            get
            {
                return (string)this["TASK_NO"];
            }
            set
            {
                this["TASK_NO"] = value;
            }
        }
        public string SO
        {
            get
            {
                return (string)this["SO"];
            }
            set
            {
                this["SO"] = value;
            }
        }
        public string DN
        {
            get
            {
                return (string)this["DN"];
            }
            set
            {
                this["DN"] = value;
            }
        }
        public string POST_TIME
        {
            get
            {
                return (string)this["POST_TIME"];
            }
            set
            {
                this["POST_TIME"] = value;
            }
        }
        public string POST_DATE
        {
            get
            {
                return (string)this["POST_DATE"];
            }
            set
            {
                this["POST_DATE"] = value;
            }
        }
        public string MEINS
        {
            get
            {
                return (string)this["MEINS"];
            }
            set
            {
                this["MEINS"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string WORK_TYPE
        {
            get
            {
                return (string)this["WORK_TYPE"];
            }
            set
            {
                this["WORK_TYPE"] = value;
            }
        }
        public string PGI_STORAGE
        {
            get
            {
                return (string)this["PGI_STORAGE"];
            }
            set
            {
                this["PGI_STORAGE"] = value;
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
        public string CLIENT
        {
            get
            {
                return (string)this["CLIENT"];
            }
            set
            {
                this["CLIENT"] = value;
            }
        }
        public string DOC_NO
        {
            get
            {
                return (string)this["DOC_NO"];
            }
            set
            {
                this["DOC_NO"] = value;
            }
        }
    }
    public class R_7B5_PGI_DATA
    {
        public string LOT_NO { get; set; }
        public string TASK_NO { get; set; }
        public string SO { get; set; }
        public string DN { get; set; }
        public string POST_TIME { get; set; }
        public string POST_DATE { get; set; }
        public string MEINS { get; set; }
        public double? QTY { get; set; }
        public string WORK_TYPE { get; set; }
        public string PGI_STORAGE { get; set; }
        public string SKUNO { get; set; }
        public string PLANT { get; set; }
        public string CLIENT { get; set; }
        public string DOC_NO { get; set; }
    }
}