using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{
    public class T_C_AGING_CONFIG_DETAIL : DataObjectTable
    {
        public T_C_AGING_CONFIG_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_AGING_CONFIG_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_AGING_CONFIG_DETAIL);
            TableName = "C_AGING_CONFIG_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_AGING_CONFIG_DETAIL> GetConfigList(OleExec sfcdb, string skuno, string cabinetno)
        {
            var sql = sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(skuno), t => t.ITEM_CODE.Contains(skuno))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinetno), t => t.CABINET_NO.Contains(cabinetno)).ToSql();
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(skuno), t => t.ITEM_CODE.Contains(skuno))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinetno), t => t.CABINET_NO.Contains(cabinetno)).ToList();
        }

        public int DeleteAll(OleExec DB)
        {
            return DB.ORM.Deleteable<C_AGING_CONFIG_DETAIL>().ExecuteCommand();
        }

        public int Insert(OleExec sfcdb, C_AGING_CONFIG_DETAIL cacd)
        {
            return sfcdb.ORM.Insertable(cacd).ExecuteCommand();
        }

        public int Insert(string bu,OleExec sfcdb, string itemName, string itemCode, double agingTime, string agingType, string cabinetNo, string shelfNo, double shelfQty, 
            string toolsFlag,string toolNo,double toolsLength, string toolsSlot, string soltNo, double oneShelfQty, string description1, string description2,string user)
        {
            C_AGING_CONFIG_DETAIL cacd = new C_AGING_CONFIG_DETAIL();
            cacd.ID = this.GetNewID(bu, sfcdb);
            cacd.ITEM_NAME = itemName;
            cacd.ITEM_CODE = itemCode;
            cacd.AGINGTIME = agingTime;
            cacd.AGINGTYPE = agingType;
            cacd.CABINET_NO = cabinetNo;
            cacd.SHELF_NO = shelfNo;
            cacd.SHELF_QTY = shelfQty;
            cacd.TOOLS_FLAG = toolsFlag;
            cacd.TOOLSNO = toolNo;
            cacd.ONESHELFQTY = oneShelfQty;
            cacd.DESCRIPTION1 = description1;
            cacd.DESCRIPTION2 = description2;
            cacd.WORK_FLAG = "";
            cacd.EDIT_EMP = user;
            cacd.EDIT_TIME = GetDBDateTime(sfcdb);
            return sfcdb.ORM.Insertable(cacd).ExecuteCommand();
        }

        public bool CabinetIsExist(OleExec sfcdb, string cabinetno)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Any(c => c.CABINET_NO.Contains(cabinetno));
        }

        public C_AGING_CONFIG_DETAIL GetConfigDetail(OleExec sfcdb, string cabinetno, string itemCode)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Where(a => a.CABINET_NO.Contains(cabinetno) && a.ITEM_CODE == itemCode).ToList().FirstOrDefault();
        }

        public bool ShelfIsConsistent(OleExec sfcdb, string cabinet, string itemCode, string shelf)
        {           
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Any(a => a.CABINET_NO.Contains(cabinet) && a.ITEM_CODE == itemCode && a.SHELF_NO.Contains(shelf));
        }
        public bool ShelfIsConsistent(OleExec sfcdb, string cabinet, string shelf)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Any(a => a.CABINET_NO.Contains(cabinet) && a.SHELF_NO.Contains(shelf));
        }
        public C_AGING_CONFIG_DETAIL GetConfigByCabinetAndShelf(OleExec sfcdb, string cabinet, string itemCode, string shelf)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Where(a => a.CABINET_NO.Contains(cabinet) && a.ITEM_CODE == itemCode && a.SHELF_NO.Contains(shelf)).ToList().FirstOrDefault();
        }
        public C_AGING_CONFIG_DETAIL GetConfigObject(OleExec sfcdb, string cabinet, string itemCode, string shelf,string tool)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(cabinet), r => r.CABINET_NO.Contains(cabinet))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(itemCode), r => r.ITEM_CODE == itemCode)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(shelf), r => r.SHELF_NO.Contains(shelf))
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(tool), r => r.TOOLSNO.Contains(tool)).ToList().FirstOrDefault();
        }
        public C_AGING_CONFIG_DETAIL GetConfigByCabinetAndShelf(OleExec sfcdb, string cabinet, string shelf)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Where(a => a.CABINET_NO.Contains(cabinet) && a.SHELF_NO.Contains(shelf)).ToList().FirstOrDefault();
        }

        public List<C_AGING_CONFIG_DETAIL> GetConfigByItemAndTool(OleExec sfcdb, string itemCode, string tool)
        {
            return sfcdb.ORM.Queryable<C_AGING_CONFIG_DETAIL>().Where(a => a.TOOLSNO.Contains(tool) && a.ITEM_CODE == itemCode).ToList();
        }
    }
    public class Row_C_AGING_CONFIG_DETAIL : DataObjectBase
    {
        public Row_C_AGING_CONFIG_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public C_AGING_CONFIG_DETAIL GetDataObject()
        {
            C_AGING_CONFIG_DETAIL DataObject = new C_AGING_CONFIG_DETAIL();
            DataObject.ID = this.ID;
            DataObject.ITEM_NAME = this.ITEM_NAME;
            DataObject.ITEM_CODE = this.ITEM_CODE;
            DataObject.AGINGTIME = this.AGINGTIME;
            DataObject.AGINGTYPE = this.AGINGTYPE;
            DataObject.CABINET_NO = this.CABINET_NO;
            DataObject.SHELF_NO = this.SHELF_NO;
            DataObject.SHELF_QTY = this.SHELF_QTY;
            DataObject.TOOLS_FLAG = this.TOOLS_FLAG;
            DataObject.TOOLSNO = this.TOOLSNO;
            DataObject.TOOLS_LENGTH = this.TOOLS_LENGTH;
            DataObject.TOOLS_SLOT = this.TOOLS_SLOT;
            DataObject.SOLT_NO = this.SOLT_NO;
            DataObject.ONESHELFQTY = this.ONESHELFQTY;
            DataObject.DESCRIPTION1 = this.DESCRIPTION1;
            DataObject.DESCRIPTION2 = this.DESCRIPTION2;
            DataObject.WORK_FLAG = this.WORK_FLAG;
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
        public string ITEM_NAME
        {
            get
            {
                return (string)this["ITEM_NAME"];
            }
            set
            {
                this["ITEM_NAME"] = value;
            }
        }
        public string ITEM_CODE
        {
            get
            {
                return (string)this["ITEM_CODE"];
            }
            set
            {
                this["ITEM_CODE"] = value;
            }
        }
        public double? AGINGTIME
        {
            get
            {
                return (double?)this["AGINGTIME"];
            }
            set
            {
                this["AGINGTIME"] = value;
            }
        }
        public string AGINGTYPE
        {
            get
            {
                return (string)this["AGINGTYPE"];
            }
            set
            {
                this["AGINGTYPE"] = value;
            }
        }
        public string CABINET_NO
        {
            get
            {
                return (string)this["CABINET_NO"];
            }
            set
            {
                this["CABINET_NO"] = value;
            }
        }
        public string SHELF_NO
        {
            get
            {
                return (string)this["SHELF_NO"];
            }
            set
            {
                this["SHELF_NO"] = value;
            }
        }
        public double? SHELF_QTY
        {
            get
            {
                return (double?)this["SHELF_QTY"];
            }
            set
            {
                this["SHELF_QTY"] = value;
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
        public double? TOOLS_LENGTH
        {
            get
            {
                return (double?)this["TOOLS_LENGTH"];
            }
            set
            {
                this["TOOLS_LENGTH"] = value;
            }
        }
        public string TOOLS_SLOT
        {
            get
            {
                return (string)this["TOOLS_SLOT"];
            }
            set
            {
                this["TOOLS_SLOT"] = value;
            }
        }
        public string SOLT_NO
        {
            get
            {
                return (string)this["SOLT_NO"];
            }
            set
            {
                this["SOLT_NO"] = value;
            }
        }
        public double? ONESHELFQTY
        {
            get
            {
                return (double?)this["ONESHELFQTY"];
            }
            set
            {
                this["ONESHELFQTY"] = value;
            }
        }
        public string DESCRIPTION1
        {
            get
            {
                return (string)this["DESCRIPTION1"];
            }
            set
            {
                this["DESCRIPTION1"] = value;
            }
        }
        public string DESCRIPTION2
        {
            get
            {
                return (string)this["DESCRIPTION2"];
            }
            set
            {
                this["DESCRIPTION2"] = value;
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
    public class C_AGING_CONFIG_DETAIL
    {
        public string ID { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_CODE { get; set; }
        public double? AGINGTIME { get; set; }
        public string AGINGTYPE { get; set; }
        public string CABINET_NO { get; set; }
        public string SHELF_NO { get; set; }
        public double? SHELF_QTY { get; set; }
        public string TOOLS_FLAG { get; set; }
        public string TOOLSNO { get; set; }
        public double? TOOLS_LENGTH { get; set; }
        public string TOOLS_SLOT { get; set; }
        public string SOLT_NO { get; set; }
        public double? ONESHELFQTY { get; set; }
        public string DESCRIPTION1 { get; set; }
        public string DESCRIPTION2 { get; set; }
        public string WORK_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}