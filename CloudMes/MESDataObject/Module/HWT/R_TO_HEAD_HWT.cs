using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_TO_HEAD_HWT : DataObjectTable
    {
        public T_R_TO_HEAD_HWT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TO_HEAD_HWT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TO_HEAD_HWT);
            TableName = "R_TO_HEAD_HWT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public object GetWaitShippingToData(OleExec sfcdb, string to_no)
        {            
            var data = sfcdb.ORM.Queryable<R_TO_HEAD_HWT>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(to_no), r => r.TO_NO == to_no)
                .Where(r => SqlSugar.SqlFunc.IsNullOrEmpty(r.REAL_ENDTIME) && r.TO_CREATETIME > SqlSugar.SqlFunc.GetDate().AddDays(-30))
                .Select(r => new { r.TO_NO, r.TO_CREATETIME, r.REAL_STARTTIME, r.REAL_ENDTIME, r.CONTAINER_NO, r.VEHICLE_NO, r.ABNORMITY_FLAG, r.PLANT }).ToList();
            return data;
        }

        public R_TO_HEAD_HWT GetTOHeadObjectByTO(OleExec sfcdb,string to_no)
        {
            return sfcdb.ORM.Queryable<R_TO_HEAD_HWT>().Where(r => r.TO_NO == to_no).ToList().FirstOrDefault();
        }
    }
    public class Row_R_TO_HEAD_HWT : DataObjectBase
    {
        public Row_R_TO_HEAD_HWT(DataObjectInfo info) : base(info)
        {

        }
        public R_TO_HEAD_HWT GetDataObject()
        {
            R_TO_HEAD_HWT DataObject = new R_TO_HEAD_HWT();
            DataObject.SECOND_FLAG = this.SECOND_FLAG;
            DataObject.SECOND_STARTTIME = this.SECOND_STARTTIME;
            DataObject.SECOND_ENDTIME = this.SECOND_ENDTIME;
            DataObject.SEND_FLAG = this.SEND_FLAG;
            DataObject.DATA1 = this.DATA1;
            DataObject.PLANT = this.PLANT;
            DataObject.ASN_FLAG = this.ASN_FLAG;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ID = this.ID;
            DataObject.TO_NO = this.TO_NO;
            DataObject.PLAN_STARTIME = this.PLAN_STARTIME;
            DataObject.PLAN_ENDTIME = this.PLAN_ENDTIME;
            DataObject.REAL_STARTTIME = this.REAL_STARTTIME;
            DataObject.REAL_ENDTIME = this.REAL_ENDTIME;
            DataObject.TO_CREATETIME = this.TO_CREATETIME;
            DataObject.TO_LASTUPDATETIME = this.TO_LASTUPDATETIME;
            DataObject.TO_FLAG = this.TO_FLAG;
            DataObject.CONTAINER_NO = this.CONTAINER_NO;
            DataObject.VEHICLE_NO = this.VEHICLE_NO;
            DataObject.EXTERNAL_NO = this.EXTERNAL_NO;
            DataObject.ABNORMITY_FLAG = this.ABNORMITY_FLAG;
            DataObject.DROP_FLAG = this.DROP_FLAG;
            DataObject.SHIP_TYPE = this.SHIP_TYPE;
            DataObject.SHIP_COUNTRY = this.SHIP_COUNTRY;
            return DataObject;
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
        public string ASN_FLAG
        {
            get
            {
                return (string)this["ASN_FLAG"];
            }
            set
            {
                this["ASN_FLAG"] = value;
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
        public DateTime? PLAN_STARTIME
        {
            get
            {
                return (DateTime?)this["PLAN_STARTIME"];
            }
            set
            {
                this["PLAN_STARTIME"] = value;
            }
        }
        public DateTime? PLAN_ENDTIME
        {
            get
            {
                return (DateTime?)this["PLAN_ENDTIME"];
            }
            set
            {
                this["PLAN_ENDTIME"] = value;
            }
        }
        public DateTime? REAL_STARTTIME
        {
            get
            {
                return (DateTime?)this["REAL_STARTTIME"];
            }
            set
            {
                this["REAL_STARTTIME"] = value;
            }
        }
        public DateTime? REAL_ENDTIME
        {
            get
            {
                return (DateTime?)this["REAL_ENDTIME"];
            }
            set
            {
                this["REAL_ENDTIME"] = value;
            }
        }

        public DateTime? TO_CREATETIME
        {
            get
            {
                return (DateTime?)this["TO_CREATETIME"];
            }
            set
            {
                this["TO_CREATETIME"] = value;
            }
        }
        public DateTime? TO_LASTUPDATETIME
        {
            get
            {
                return (DateTime?)this["TO_LASTUPDATETIME"];
            }
            set
            {
                this["TO_LASTUPDATETIME"] = value;
            }
        }
        public string TO_FLAG
        {
            get
            {
                return (string)this["TO_FLAG"];
            }
            set
            {
                this["TO_FLAG"] = value;
            }
        }
        public string CONTAINER_NO
        {
            get
            {
                return (string)this["CONTAINER_NO"];
            }
            set
            {
                this["CONTAINER_NO"] = value;
            }
        }
        public string VEHICLE_NO
        {
            get
            {
                return (string)this["VEHICLE_NO"];
            }
            set
            {
                this["VEHICLE_NO"] = value;
            }
        }
        public string EXTERNAL_NO
        {
            get
            {
                return (string)this["EXTERNAL_NO"];
            }
            set
            {
                this["EXTERNAL_NO"] = value;
            }
        }
        public string ABNORMITY_FLAG
        {
            get
            {
                return (string)this["ABNORMITY_FLAG"];
            }
            set
            {
                this["ABNORMITY_FLAG"] = value;
            }
        }
        public string DROP_FLAG
        {
            get
            {
                return (string)this["DROP_FLAG"];
            }
            set
            {
                this["DROP_FLAG"] = value;
            }
        }
        public string SHIP_TYPE
        {
            get
            {
                return (string)this["SHIP_TYPE"];
            }
            set
            {
                this["SHIP_TYPE"] = value;
            }
        }
        public string SHIP_COUNTRY
        {
            get
            {
                return (string)this["SHIP_COUNTRY"];
            }
            set
            {
                this["SHIP_COUNTRY"] = value;
            }
        }
    }
    public class R_TO_HEAD_HWT
    {
        public string SECOND_FLAG { get; set; }
        public DateTime? SECOND_STARTTIME { get; set; }
        public DateTime? SECOND_ENDTIME { get; set; }
        public string SEND_FLAG { get; set; }
        public string DATA1 { get; set; }
        public string PLANT { get; set; }
        public string ASN_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string ID { get; set; }
        public string TO_NO { get; set; }
        public DateTime? PLAN_STARTIME { get; set; }
        public DateTime? PLAN_ENDTIME { get; set; }
        public DateTime? REAL_STARTTIME { get; set; }
        public DateTime? REAL_ENDTIME { get; set; }
        public DateTime? TO_CREATETIME { get; set; }
        public DateTime? TO_LASTUPDATETIME { get; set; }
        public string TO_FLAG { get; set; }
        public string CONTAINER_NO { get; set; }
        public string VEHICLE_NO { get; set; }
        public string EXTERNAL_NO { get; set; }
        public string ABNORMITY_FLAG { get; set; }
        public string DROP_FLAG { get; set; }
        public string SHIP_TYPE { get; set; }
        public string SHIP_COUNTRY { get; set; }
    }
}