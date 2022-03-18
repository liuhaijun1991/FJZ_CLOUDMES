using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{
    public class T_R_AGING_CABINET_INFO : DataObjectTable
    {
        public T_R_AGING_CABINET_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_AGING_CABINET_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_AGING_CABINET_INFO);
            TableName = "R_AGING_CABINET_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Save(OleExec sfcdb, R_AGING_CABINET_INFO info)
        {
            return sfcdb.ORM.Insertable(info).ExecuteCommand();
        }

        public int UpdateByID(OleExec sfcdb, R_AGING_CABINET_INFO info)
        {
            return sfcdb.ORM.Updateable<R_AGING_CABINET_INFO>().UpdateColumns(r => new R_AGING_CABINET_INFO
            {
                FLOOR = info.FLOOR,
                CABINETNO = info.CABINETNO,
                ITEMCODE = info.ITEMCODE,
                ITEMNAME = info.ITEMNAME,
                AGINGTIME = info.AGINGTIME,
                QTY = info.QTY,
                STARTTIME = info.STARTTIME,
                ENDTIME = info.ENDTIME,
                STARTEMPNO = info.STARTEMPNO,
                REALFINISHTIME = info.REALFINISHTIME,
                ENDEMPNO = info.ENDEMPNO,
                IPADDRESS = info.IPADDRESS,
                LOT_NO = info.LOT_NO,
                SUBMITTIME = info.SUBMITTIME,
                SUBMITEMPNO = info.SUBMITEMPNO,
                WORK_FLAG = info.WORK_FLAG,
                REMARK = info.REMARK,
                LASTEDITDT = info.LASTEDITDT
            }).Where(r => r.ID == info.ID).ExecuteCommand();
        }

        public DataTable GetWaitEndList(OleExec sfcdb, string floor, string cabinet)
        {
            string sqlFloor = "";
            string sqlCabinet = "";
            if (!string.IsNullOrEmpty(floor))
            {
                sqlFloor = $@"  and  floor like '%{floor}%' ";
            }           
            if (!string.IsNullOrEmpty(cabinet))
            {
                sqlCabinet = $@" and cabinetno like '%{cabinet}%' ";
            }
            string sql = $@"SELECT ROWNUM AS NO, floor,itemcode,cabinetno,agingtime,qty,starttime,endtime,realfinishtime,ipaddress,lot_no,remark, 
                                DECODE(ENDTIME,'','NO',NULL,'NO',decode(sign(endtime-sysdate),-1,'YES',0,'YES',1,'NO')) flag from 
                            (select *  FROM R_AGING_CABINET_INFO Where starttime Is not Null and work_flag<>'5' {sqlFloor} {sqlCabinet} order by endtime asc )";                      
            return sfcdb.ExecSelect(sql).Tables[0];
        }
        public DataTable GetWaitStartList(OleExec sfcdb, string floor, string cabinet)
        {
            string sqlFloor = "";
            string sqlCabinet = "";
            if (!string.IsNullOrEmpty(floor))
            {
                sqlFloor = $@"  and  floor like '%{floor}%' ";
            }           
            if (!string.IsNullOrEmpty(cabinet))
            {
                sqlCabinet = $@" and cabinetno like '%{cabinet}%' ";
            }
            string sql = $@"SELECT ROWNUM AS NO, floor,itemcode,cabinetno,agingtime,qty,starttime,endtime,realfinishtime,ipaddress,lot_no from 
                            (select *  FROM R_AGING_CABINET_INFO Where starttime Is Null  {sqlFloor} {sqlCabinet} order by endtime asc )";  
            return sfcdb.ExecSelect(sql).Tables[0];
        }
        public R_AGING_CABINET_INFO GetObjectByCabinet(OleExec sfcdb, string floor, string cabinet, string workFlag)
        {
            return sfcdb.ORM.Queryable<R_AGING_CABINET_INFO>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(floor), r => r.FLOOR == floor)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .Where(r => r.WORK_FLAG == workFlag).ToList().FirstOrDefault();
        }
        public int StartAging(OleExec sfcdb, string ip, string floor, string cabinet, string lot,string user,DateTime? startTime, DateTime? endTime)
        {              
            return sfcdb.ORM.Updateable<R_AGING_CABINET_INFO>().UpdateColumns(r => new R_AGING_CABINET_INFO() { STARTTIME = startTime, ENDTIME = endTime, WORK_FLAG = "4", STARTEMPNO = user })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.LOT_NO == lot && r.FLOOR == floor).ExecuteCommand();  
        }
        public int EndAging(OleExec sfcdb, string ip, string floor, string cabinet, string lot, string user, DateTime? realfinishtime)
        {
            return sfcdb.ORM.Updateable<R_AGING_CABINET_INFO>().UpdateColumns(r => new R_AGING_CABINET_INFO() {  REALFINISHTIME = realfinishtime, WORK_FLAG = "5", ENDEMPNO = user })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.LOT_NO == lot && r.FLOOR == floor).ExecuteCommand();
        }
    }
    public class Row_R_AGING_CABINET_INFO : DataObjectBase
    {
        public Row_R_AGING_CABINET_INFO(DataObjectInfo info) : base(info)
        {

        }
        public R_AGING_CABINET_INFO GetDataObject()
        {
            R_AGING_CABINET_INFO DataObject = new R_AGING_CABINET_INFO();
            DataObject.ID = this.ID;
            DataObject.SUBMITTIME = this.SUBMITTIME;
            DataObject.SUBMITEMPNO = this.SUBMITEMPNO;
            DataObject.WORK_FLAG = this.WORK_FLAG;
            DataObject.REMARK = this.REMARK;
            DataObject.LASTEDITDT = this.LASTEDITDT;
            DataObject.FLOOR = this.FLOOR;
            DataObject.CABINETNO = this.CABINETNO;
            DataObject.ITEMCODE = this.ITEMCODE;
            DataObject.ITEMNAME = this.ITEMNAME;
            DataObject.AGINGTIME = this.AGINGTIME;
            DataObject.QTY = this.QTY;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.STARTEMPNO = this.STARTEMPNO;
            DataObject.REALFINISHTIME = this.REALFINISHTIME;
            DataObject.ENDEMPNO = this.ENDEMPNO;
            DataObject.IPADDRESS = this.IPADDRESS;
            DataObject.LOT_NO = this.LOT_NO;
            return DataObject;
        }
        public DateTime? SUBMITTIME
        {
            get
            {
                return (DateTime?)this["SUBMITTIME"];
            }
            set
            {
                this["SUBMITTIME"] = value;
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
        public string SUBMITEMPNO
        {
            get
            {
                return (string)this["SUBMITEMPNO"];
            }
            set
            {
                this["SUBMITEMPNO"] = value;
            }
        }
        public string WORK_FLAG
        {
            get
            {
                return (string)this["WORK_FLAG"];
            }
            set
            {
                this["WORK_FLAG"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
        public string FLOOR
        {
            get
            {
                return (string)this["FLOOR"];
            }
            set
            {
                this["FLOOR"] = value;
            }
        }
        public string CABINETNO
        {
            get
            {
                return (string)this["CABINETNO"];
            }
            set
            {
                this["CABINETNO"] = value;
            }
        }
        public string ITEMCODE
        {
            get
            {
                return (string)this["ITEMCODE"];
            }
            set
            {
                this["ITEMCODE"] = value;
            }
        }
        public string ITEMNAME
        {
            get
            {
                return (string)this["ITEMNAME"];
            }
            set
            {
                this["ITEMNAME"] = value;
            }
        }
        public string AGINGTIME
        {
            get
            {
                return (string)this["AGINGTIME"];
            }
            set
            {
                this["AGINGTIME"] = value;
            }
        }
        public string QTY
        {
            get
            {
                return (string)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
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
        public string STARTEMPNO
        {
            get
            {
                return (string)this["STARTEMPNO"];
            }
            set
            {
                this["STARTEMPNO"] = value;
            }
        }
        public DateTime? REALFINISHTIME
        {
            get
            {
                return (DateTime?)this["REALFINISHTIME"];
            }
            set
            {
                this["REALFINISHTIME"] = value;
            }
        }
        public string ENDEMPNO
        {
            get
            {
                return (string)this["ENDEMPNO"];
            }
            set
            {
                this["ENDEMPNO"] = value;
            }
        }
        public string IPADDRESS
        {
            get
            {
                return (string)this["IPADDRESS"];
            }
            set
            {
                this["IPADDRESS"] = value;
            }
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
    }
    public class R_AGING_CABINET_INFO
    {
        public string ID { get; set; }
        public string FLOOR { get; set; } 
        public string CABINETNO { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string AGINGTIME { get; set; }
        public string QTY { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string STARTEMPNO { get; set; }
        public DateTime? REALFINISHTIME { get; set; }
        public string ENDEMPNO { get; set; }
        public string IPADDRESS { get; set; }
        public string LOT_NO { get; set; }
        public DateTime? SUBMITTIME { get; set; }
        public string SUBMITEMPNO { get; set; }
        public string WORK_FLAG { get; set; }
        public string REMARK { get; set; }
        public DateTime? LASTEDITDT { get; set; }
    }
       
}