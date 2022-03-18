using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_R_AGING_SHELF_INFO : DataObjectTable
    {
        public T_R_AGING_SHELF_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_AGING_SHELF_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_AGING_SHELF_INFO);
            TableName = "R_AGING_SHELF_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int Save(OleExec sfcdb, R_AGING_SHELF_INFO info)
        {
            return sfcdb.ORM.Insertable(info).ExecuteCommand();
        }
       
        public DataTable GetScanList(OleExec sfcdb, string ip)
        {
            string sql = string.Format("select  rownum as no, cabinetno,shelfno,useqty,unuseqty,tools_flag from  r_aging_shelf_info  where ipaddress='{0}' and work_flag='2' order by edit_time ", ip);
            return sfcdb.ExecSelect(sql).Tables[0];
        }

        public int DeleteWaitSubmitListByIP(OleExec sfcdb, string ip)
        {
            return sfcdb.ORM.Deleteable<R_AGING_SHELF_INFO>().Where(r => r.IPADDRESS == ip && (r.WORK_FLAG == "1" || r.WORK_FLAG == "2")).ExecuteCommand();
        }

        public int DeleteShelf(OleExec sfcdb, string ip, string shelf, string workFlag)
        {
            return sfcdb.ORM.Deleteable<R_AGING_SHELF_INFO>().Where(r => r.IPADDRESS == ip && r.SHELFNO == shelf && r.WORK_FLAG == workFlag).ExecuteCommand();
        }


        public int UpdateWorkFlag(OleExec sfcdb, string ip, string cabinet, string oldFlag,string newFlag)
        {
            return sfcdb.ORM.Updateable<R_AGING_SHELF_INFO>().UpdateColumns(r => new R_AGING_SHELF_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }
        public int UpdateWorkFlag(OleExec sfcdb, string ip, string cabinet,string lot, string oldFlag, string newFlag)
        {
            return sfcdb.ORM.Updateable<R_AGING_SHELF_INFO>().UpdateColumns(r => new R_AGING_SHELF_INFO { WORK_FLAG = newFlag })
                .Where(r => r.IPADDRESS == ip && r.CABINETNO == cabinet && r.LOT_NO== lot && r.WORK_FLAG == oldFlag).ExecuteCommand();
        }        
        public List<R_AGING_SHELF_INFO> GetShelfWaitSubmitList(OleExec sfcdb, string cabinet, string ip)
        {
            return sfcdb.ORM.Queryable<R_AGING_SHELF_INFO>()
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(ip), r => r.IPADDRESS == ip)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINETNO == cabinet)
                .Where(r =>r.WORK_FLAG == "1" || r.WORK_FLAG == "2").ToList();
        }

    }
    public class Row_R_AGING_SHELF_INFO : DataObjectBase
    {
        public Row_R_AGING_SHELF_INFO(DataObjectInfo info) : base(info)
        {

        }
        public R_AGING_SHELF_INFO GetDataObject()
        {
            R_AGING_SHELF_INFO DataObject = new R_AGING_SHELF_INFO();
            DataObject.ID = this.ID;
            DataObject.CABINETNO = this.CABINETNO;
            DataObject.SHELFNO = this.SHELFNO;
            DataObject.USEQTY = this.USEQTY;
            DataObject.UNUSEQTY = this.UNUSEQTY;
            DataObject.ITEMCODE = this.ITEMCODE;
            DataObject.ITEMNAME = this.ITEMNAME;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.IPADDRESS = this.IPADDRESS;
            DataObject.WORK_FLAG = this.WORK_FLAG;
            DataObject.TOOLSNO = this.TOOLSNO;
            DataObject.TOOLS_FLAG = this.TOOLS_FLAG;
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
        public string USEQTY
        {
            get
            {
                return (string)this["USEQTY"];
            }
            set
            {
                this["USEQTY"] = value;
            }
        }
        public string UNUSEQTY
        {
            get
            {
                return (string)this["UNUSEQTY"];
            }
            set
            {
                this["UNUSEQTY"] = value;
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
    public class R_AGING_SHELF_INFO
    {
        public string ID { get; set; }
        public string CABINETNO { get; set; }
        public string SHELFNO { get; set; }
        public string USEQTY { get; set; }
        public string UNUSEQTY { get; set; }
        public string ITEMCODE { get; set; }
        public string ITEMNAME { get; set; }
        public string LOT_NO { get; set; }
        public string IPADDRESS { get; set; }
        public string WORK_FLAG { get; set; }
        public string TOOLSNO { get; set; }
        public string TOOLS_FLAG { get; set; }
        public string REMARK { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}