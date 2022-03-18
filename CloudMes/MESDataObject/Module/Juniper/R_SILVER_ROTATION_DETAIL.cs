using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_SILVER_ROTATION_DETAIL : DataObjectTable
    {
        public T_R_SILVER_ROTATION_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SILVER_ROTATION_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SILVER_ROTATION_DETAIL);
            TableName = "R_SILVER_ROTATION_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_SILVER_ROTATION_DETAIL> GetRotationListByCSN(string csn, OleExec db)
        {
            return db.ORM.Queryable<R_SILVER_ROTATION_DETAIL>().Where(r => r.CSN == csn && r.VALID_FLAG == "1").ToList();
        }

        public List<R_SILVER_ROTATION_DETAIL> GetRotationListBySN(string sn, OleExec db)
        {
            return db.ORM.Queryable<R_SILVER_ROTATION_DETAIL>().Where(r => r.SN == sn && r.VALID_FLAG == "1").ToList();
        }

        public bool CSNIsNotEndRotation(string csn, OleExec db)
        {
            return db.ORM.Queryable<R_SILVER_ROTATION_DETAIL>().Any(r => r.CSN == csn && SqlFunc.IsNullOrEmpty(r.ENDTIME) == true && r.VALID_FLAG == "1");
        }
        public int InsertRotationDetail(OleExec SFCDB, R_SILVER_ROTATION_DETAIL detailObj)
        {
            return SFCDB.ORM.Insertable<R_SILVER_ROTATION_DETAIL>(detailObj).ExecuteCommand();
        }

    }
    public class Row_R_SILVER_ROTATION_DETAIL : DataObjectBase
    {
        public Row_R_SILVER_ROTATION_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SILVER_ROTATION_DETAIL GetDataObject()
        {
            R_SILVER_ROTATION_DETAIL DataObject = new R_SILVER_ROTATION_DETAIL();
            DataObject.ENDBY = this.ENDBY;
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.CSN = this.CSN;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.SEQNO = this.SEQNO;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.STARTBY = this.STARTBY;
            DataObject.VALID_FLAG = this.VALID_FLAG;
            return DataObject;
        }
        public string ENDBY
        {
            get
            {
                return (string)this["ENDBY"];
            }
            set
            {
                this["ENDBY"] = value;
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
        public string CSN
        {
            get
            {
                return (string)this["CSN"];
            }
            set
            {
                this["CSN"] = value;
            }
        }
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public DateTime? STARTTIME
        {
            get
            {
                return (DateTime?)this["STARTTIME"];
            }
            set
            {
                this["STARTTIME"] = value;
            }
        }
        public DateTime? ENDTIME
        {
            get
            {
                return (DateTime?)this["ENDTIME"];
            }
            set
            {
                this["ENDTIME"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string STARTBY
        {
            get
            {
                return (string)this["STARTBY"];
            }
            set
            {
                this["STARTBY"] = value;
            }
        }
    }
    public class R_SILVER_ROTATION_DETAIL
    {
        
        [SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string SN { get; set; }
        public string CSN { get; set; }
        public string STATION_NAME { get; set; }
        public double? SEQNO { get; set; }
        public string VALID_FLAG { get; set; }
        public DateTime? STARTTIME { get; set; }
        public string STARTBY { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string ENDBY { get; set; }
    }
}