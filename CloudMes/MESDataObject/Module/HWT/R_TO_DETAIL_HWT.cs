using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_TO_DETAIL_HWT : DataObjectTable
    {
        public T_R_TO_DETAIL_HWT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TO_DETAIL_HWT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TO_DETAIL_HWT);
            TableName = "R_TO_DETAIL_HWT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_TO_DETAIL_HWT> GetDetailByTONO(OleExec sfcdb, string to_no)
        {
            return sfcdb.ORM.Queryable<R_TO_DETAIL_HWT>().Where(r => r.TO_NO == to_no).ToList();
        }

        public R_TO_DETAIL_HWT GetDetailByDNNO(OleExec sfcdb, string dn_no)
        {
            return sfcdb.ORM.Queryable<R_TO_DETAIL_HWT>().Where(r => r.DN_NO == dn_no).ToList().FirstOrDefault();
        }
    }
    public class Row_R_TO_DETAIL_HWT : DataObjectBase
    {
        public Row_R_TO_DETAIL_HWT(DataObjectInfo info) : base(info)
        {

        }
        public R_TO_DETAIL_HWT GetDataObject()
        {
            R_TO_DETAIL_HWT DataObject = new R_TO_DETAIL_HWT();
            DataObject.ID = this.ID;
            DataObject.TO_NO = this.TO_NO;
            DataObject.TO_ITEM_NO = this.TO_ITEM_NO;
            DataObject.DN_NO = this.DN_NO;
            DataObject.DN_CUSTOMER = this.DN_CUSTOMER;
            DataObject.DN_STARTTIME = this.DN_STARTTIME;
            DataObject.DN_ENDTIME = this.DN_ENDTIME;
            DataObject.DN_FLAG = this.DN_FLAG;
            DataObject.SECOND_FLAG = this.SECOND_FLAG;
            DataObject.SECOND_STARTTIME = this.SECOND_STARTTIME;
            DataObject.SECOND_ENDTIME = this.SECOND_ENDTIME;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.CARTON_QTY = this.CARTON_QTY;
            DataObject.PLANT = this.PLANT;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
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
        public string TO_NO
        {
            get
            {
                return (string)this["TO_NO"];
            }
            set
            {
                this["TO_NO"] = value;
            }
        }
        public string TO_ITEM_NO
        {
            get
            {
                return (string)this["TO_ITEM_NO"];
            }
            set
            {
                this["TO_ITEM_NO"] = value;
            }
        }
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
            }
        }
        public string DN_CUSTOMER
        {
            get
            {
                return (string)this["DN_CUSTOMER"];
            }
            set
            {
                this["DN_CUSTOMER"] = value;
            }
        }
        public DateTime? DN_STARTTIME
        {
            get
            {
                return (DateTime?)this["DN_STARTTIME"];
            }
            set
            {
                this["DN_STARTTIME"] = value;
            }
        }
        public DateTime? DN_ENDTIME
        {
            get
            {
                return (DateTime?)this["DN_ENDTIME"];
            }
            set
            {
                this["DN_ENDTIME"] = value;
            }
        }
        public string DN_FLAG
        {
            get
            {
                return (string)this["DN_FLAG"];
            }
            set
            {
                this["DN_FLAG"] = value;
            }
        }
        public string SECOND_FLAG
        {
            get
            {
                return (string)this["SECOND_FLAG"];
            }
            set
            {
                this["SECOND_FLAG"] = value;
            }
        }
        public DateTime? SECOND_STARTTIME
        {
            get
            {
                return (DateTime?)this["SECOND_STARTTIME"];
            }
            set
            {
                this["SECOND_STARTTIME"] = value;
            }
        }
        public DateTime? SECOND_ENDTIME
        {
            get
            {
                return (DateTime?)this["SECOND_ENDTIME"];
            }
            set
            {
                this["SECOND_ENDTIME"] = value;
            }
        }
        public string SEND_FLAG
        {
            get
            {
                return (string)this["SEND_FLAG"];
            }
            set
            {
                this["SEND_FLAG"] = value;
            }
        }
        public double? CARTON_QTY
        {
            get
            {
                return (double?)this["CARTON_QTY"];
            }
            set
            {
                this["CARTON_QTY"] = value;
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
    public class R_TO_DETAIL_HWT
    {
        public string ID { get; set; }
        public string TO_NO { get; set; }
        public string TO_ITEM_NO { get; set; }
        public string DN_NO { get; set; }
        public string DN_CUSTOMER { get; set; }
        public DateTime? DN_STARTTIME { get; set; }
        public DateTime? DN_ENDTIME { get; set; }
        public string DN_FLAG { get; set; }
        public string SECOND_FLAG { get; set; }
        public DateTime? SECOND_STARTTIME { get; set; }
        public DateTime? SECOND_ENDTIME { get; set; }
        public string SEND_FLAG { get; set; }
        public double? CARTON_QTY { get; set; }
        public string PLANT { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}