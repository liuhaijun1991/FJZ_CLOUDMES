using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{
    public class T_R_SN_AGING_INFO : DataObjectTable
    {
        public T_R_SN_AGING_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_AGING_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_AGING_INFO);
            TableName = "R_SN_AGING_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Save(OleExec sfcdb, R_SN_AGING_INFO info)
        {
            return sfcdb.ORM.Insertable(info).ExecuteCommand();
        }
        public int UpdateByID(OleExec sfcdb, R_SN_AGING_INFO info)
        {
            //沒有在類里設置主鍵故按一些方式寫
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO
            {
                SN = info.SN,
                WORKORDERNO = info.WORKORDERNO,
                ITEMCODE = info.ITEMCODE,
                ITEMNAME = info.ITEMNAME,
                AGINGTIME = info.AGINGTIME,
                FLOOR = info.FLOOR,
                CABINETNO = info.CABINETNO,
                SHELFNO = info.SHELFNO,
                TOOLS_FLAG = info.TOOLS_FLAG,
                TOOLSNO = info.TOOLSNO,
                SLOTNO = info.SLOTNO,
                STARTTIME = info.STARTTIME,
                ENDTIME = info.ENDTIME,
                STARTEMPNO = info.STARTEMPNO,
                REALFINISHTIME = info.REALFINISHTIME,
                ENDEMPNO = info.ENDEMPNO,
                IPADDRESS = info.IPADDRESS,
                LOT_NO = info.LOT_NO,
                WORK_FLAG = info.WORK_FLAG,
                EVENTPASS = info.EVENTPASS,
                REMARK = info.REMARK,
                EDIT_EMP = info.EDIT_EMP,
                EDIT_TIME = info.EDIT_TIME
            }).Where(r => r.ID == info.ID).ExecuteCommand();
        }
        public int EndAging(OleExec sfcdb, string id,string eventpass,string remark,string end_empno,DateTime ? realfinishtime)
        {
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO
            {
                WORK_FLAG = "5",
                REALFINISHTIME = realfinishtime,
                REMARK = remark,
                ENDEMPNO = end_empno
            }).Where(r => r.ID == id).ExecuteCommand();
        }
        public int UpdateWorkFlag(OleExec sfcdb, string ip, string cabinet, string oldFlag, string newFlag)
        {
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }

        public int UpdateWorkFlag(OleExec sfcdb, string ip, string cabinet, string shelf,string oldFlag, string newFlag)
        {
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.SHELFNO == shelf && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }

        public int UpdateWorkFlag(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string oldFlag, string newFlag)
        {
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.SHELFNO == shelf
                && r.TOOLSNO == tool && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }

        public int UpdateWorkFlag(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string lot,string oldFlag,string newFlag)
        {
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.SHELFNO == shelf
                && r.TOOLSNO == tool && r.LOT_NO == lot && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }

        public int UpdateWorkFlagByLot(OleExec sfcdb, string ip, string cabinet, string shelf, string lot, string oldFlag, string newFlag)
        {
            return sfcdb.ORM.Updateable<R_SN_AGING_INFO>().UpdateColumns(r => new R_SN_AGING_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.SHELFNO == shelf
                && r.LOT_NO == lot && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }

        public bool CabinetHaveNotFullShelf(OleExec sfcdb, string shelfno, string cabinetno, string ip)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.IPADDRESS == ip && r.CABINETNO == cabinetno && r.SHELFNO != shelfno && r.WORK_FLAG == "1");
        }
        public bool ShelfIsFull(OleExec sfcdb, string shelfno, string cabinetno)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.CABINETNO == cabinetno && r.SHELFNO == shelfno && r.WORK_FLAG == "2");
        }
        public bool ShelfIsWaitForStartAging(OleExec sfcdb, string shelfno, string cabinetno)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.CABINETNO == cabinetno && r.SHELFNO == shelfno && r.WORK_FLAG == "3");
        }
        public bool ShelfIsAginging(OleExec sfcdb, string shelfno, string cabinetno)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.CABINETNO == cabinetno && r.SHELFNO == shelfno && r.WORK_FLAG == "4");
        }
        public bool ToolIsFull(OleExec sfcdb, string tool)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.TOOLSNO == tool && r.WORK_FLAG == "2");
        }
        public bool ToolIsWaitForStartAging(OleExec sfcdb, string tool)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.TOOLSNO == tool && r.WORK_FLAG == "3");
        }
        public bool ToolIsAginging(OleExec sfcdb, string tool)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Any(r => r.TOOLSNO == tool && r.WORK_FLAG == "4");
        }
        public bool SoltIsUseed(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string solf)
        {
            string workFlag = "1,2,3,4";
            List<R_SN_AGING_INFO> list = sfcdb.ORM.Queryable<R_SN_AGING_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELFNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO == tool)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(solf), r => r.SLOTNO == solf)
                .Where(r => workFlag.Contains(r.WORK_FLAG)).ToList();
            if (list.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        
        public DataTable GetScanList(OleExec sfcdb, string ip)
        {
            string sql = string.Format("select rownum as no, sn,slotno,shelfno,tools_flag,toolsno,workorderno,slotno from r_sn_aging_info where ipaddress='{0}' and work_flag='1' order by edit_time ", ip);            
            return sfcdb.ExecSelect(sql).Tables[0];
        }

        public int DeleteWaitSubmitListByIP(OleExec sfcdb, string ip)
        {
            return sfcdb.ORM.Deleteable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && (r.WORK_FLAG == "1" || r.WORK_FLAG == "2")).ExecuteCommand();
        }
        public int DeleteNoFullShelf(OleExec sfcdb, string ip,string cabinet)
        {
            return sfcdb.ORM.Deleteable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.WORK_FLAG == "1").ExecuteCommand();
        }
        public int DeleteNoFullTool(OleExec sfcdb, string ip, string cabinet,string shelf)
        {
            return sfcdb.ORM.Deleteable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.SHELFNO== shelf && r.WORK_FLAG == "1").ExecuteCommand();
        }

        public int DeleteSn(OleExec sfcdb, string ip, string sn)
        {
            return sfcdb.ORM.Deleteable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.SN == sn && (r.WORK_FLAG == "1" || r.WORK_FLAG == "2")).ExecuteCommand();
        }
        public int DeleteShelf(OleExec sfcdb, string ip, string shelf,string workFlag)
        {
            return sfcdb.ORM.Deleteable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.SHELFNO == shelf && r.WORK_FLAG == workFlag).ExecuteCommand();
        }

        public string GetAgingLot(OleExec sfcdb)
        {
            string sql = "";
            sql = "select  'NO'||substr(to_char(SYSDATE, 'YYYYMMDDhh24miss'), 3, 12) from dual";
            return sfcdb.ExecSelect(sql).Tables[0].Rows[0][0].ToString();
        }

        public List<R_SN_AGING_INFO> GetSNAgingList(OleExec sfcdb, string ip,string cabinet,string shelf,string tool,string lot, string location, string sn, string workFlag, string toolFlag)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELFNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO == tool)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lot), r => r.LOT_NO == lot)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(location), r => r.SLOTNO == location)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), r => r.SN == sn)
                 .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(toolFlag), r => r.TOOLS_FLAG == toolFlag)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(workFlag), r => r.WORK_FLAG == workFlag).ToList();
        }

        public List<R_SN_AGING_INFO> GetNoSubmitSNList(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string lot, string sn)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELFNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO == tool)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lot), r => r.LOT_NO == lot)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), r => r.SN == sn)
                .Where(r => r.WORK_FLAG == "1" || r.WORK_FLAG == "2").ToList();
        }

        public List<R_SN_AGING_INFO> GetNoSubmitSNList(OleExec sfcdb, string ip)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && (r.WORK_FLAG == "1" || r.WORK_FLAG == "2")).ToList();
        }

        public List<R_SN_AGING_INFO> GetNoUseList(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string lot)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELFNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO == tool)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lot), r => r.LOT_NO == lot)
                .Where(r => (r.WORK_FLAG == "1" || r.WORK_FLAG == "2") && r.SN == "EMPTY" && r.SN == "ERROR").ToList();
        }

        public List<R_SN_AGING_INFO> GetUseList(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string lot)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELFNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO == tool)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lot), r => r.LOT_NO == lot)
                .Where(r => r.WORK_FLAG == "2" && r.SN != "EMPTY" && r.SN != "ERROR").ToList();
        }

        public R_SN_AGING_INFO GetSNAgingObj(OleExec sfcdb, string sn)
        {
            string workFlag = "1,2,3,4";
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Where(r => r.SN == sn && workFlag.Contains(r.WORK_FLAG)).ToList().FirstOrDefault();
        }
        public R_SN_AGING_INFO GetSNAgingObj(OleExec sfcdb, string ip, string cabinet, string shelf, string tool, string lot, string location, string sn,string workFlag)
        {            
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELFNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO == shelf)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lot), r => r.LOT_NO == lot)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(location), r => r.SLOTNO == location)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), r => r.SN == sn)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(workFlag), r => r.WORK_FLAG == workFlag)
                .OrderBy(r=>r.EDIT_TIME,SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        public List<R_SN_AGING_INFO> GetWaitStartList(OleExec sfcdb,string ip,string floor,string cabinet,string lot)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.FLOOR == floor && r.CABINETNO == cabinet && r.LOT_NO == lot && (r.EVENTPASS == "" || r.EVENTPASS ==null) ).ToList();
        }

        public R_SN_AGING_INFO GetMaxEndTimeObj(OleExec sfcdb,string ip,string floor, string cabinet,string lot)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.LOT_NO == lot && r.FLOOR == floor && (r.EVENTPASS == "" || r.EVENTPASS == null))
                .OrderBy(r => r.ENDTIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }
        public R_SN_AGING_INFO GetMinEndTimeObj(OleExec sfcdb, string ip, string floor, string cabinet, string lot)
        {
            return sfcdb.ORM.Queryable<R_SN_AGING_INFO>().Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.LOT_NO == lot && r.FLOOR == floor && (r.EVENTPASS == "" || r.EVENTPASS == null))
                .OrderBy(r => r.ENDTIME, SqlSugar.OrderByType.Asc).ToList().FirstOrDefault();
        }
        public int StartAging(OleExec sfcdb, string ip, string floor, string cabinet, string lot,string sn, string user)
        {
            string sql = $@"UPDATE r_sn_aging_info
                             SET starttime  = SYSDATE,
                                 endtime    = SYSDATE + agingtime / 24 / 60,
                                 work_flag  = '4',
                                 startempno = '{user}'
                           WHERE sn = '{sn}'
                             AND ipaddress = '{ip}'
                             and floor='{floor}'
                             and cabinetno='{cabinet}'
                             AND lot_no = '{lot}'";
            return sfcdb.ExecuteNonQuery(sql,CommandType.Text, null);
        }

    }
    public class Row_R_SN_AGING_INFO : DataObjectBase
    {
        public Row_R_SN_AGING_INFO(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_AGING_INFO GetDataObject()
        {
            R_SN_AGING_INFO DataObject = new R_SN_AGING_INFO();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.ITEMCODE = this.ITEMCODE;
            DataObject.ITEMNAME = this.ITEMNAME;
            DataObject.AGINGTIME = this.AGINGTIME;
            DataObject.FLOOR = this.FLOOR;
            DataObject.CABINETNO = this.CABINETNO;
            DataObject.SHELFNO = this.SHELFNO;
            DataObject.TOOLS_FLAG = this.TOOLS_FLAG;
            DataObject.TOOLSNO = this.TOOLSNO;
            DataObject.SLOTNO = this.SLOTNO;
            DataObject.STARTTIME = this.STARTTIME;
            DataObject.ENDTIME = this.ENDTIME;
            DataObject.STARTEMPNO = this.STARTEMPNO;
            DataObject.REALFINISHTIME = this.REALFINISHTIME;
            DataObject.ENDEMPNO = this.ENDEMPNO;
            DataObject.IPADDRESS = this.IPADDRESS;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.WORK_FLAG = this.WORK_FLAG;
            DataObject.EVENTPASS = this.EVENTPASS;
            DataObject.REMARK = this.REMARK;
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
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
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
        public string SHELFNO
        {
            get
            {
                return (string)this["SHELFNO"];
            }
            set
            {
                this["SHELFNO"] = value;
            }
        }
        public string TOOLS_FLAG
        {
            get
            {
                return (string)this["TOOLS_FLAG"];
            }
            set
            {
                this["TOOLS_FLAG"] = value;
            }
        }
        public string TOOLSNO
        {
            get
            {
                return (string)this["TOOLSNO"];
            }
            set
            {
                this["TOOLSNO"] = value;
            }
        }
        public string SLOTNO
        {
            get
            {
                return (string)this["SLOTNO"];
            }
            set
            {
                this["SLOTNO"] = value;
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
        public string EVENTPASS
        {
            get
            {
                return (string)this["EVENTPASS"];
            }
            set
            {
                this["EVENTPASS"] = value;
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
    public class R_SN_AGING_INFO
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string WORKORDERNO { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string AGINGTIME { get; set; }
        public string FLOOR { get; set; }
        public string CABINETNO { get; set; }
        public string SHELFNO { get; set; }
        public string TOOLS_FLAG { get; set; }
        public string TOOLSNO { get; set; }
        public string SLOTNO { get; set; }
        public DateTime? STARTTIME { get; set; }
        public DateTime? ENDTIME { get; set; }
        public string STARTEMPNO { get; set; }
        public DateTime? REALFINISHTIME { get; set; }
        public string ENDEMPNO { get; set; }
        public string IPADDRESS { get; set; }
        public string LOT_NO { get; set; }
        public string WORK_FLAG { get; set; }
        public string EVENTPASS { get; set; }
        public string REMARK { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}