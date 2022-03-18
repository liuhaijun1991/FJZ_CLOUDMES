using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_LINK_CONTROL : DataObjectTable
    {
        public T_R_LINK_CONTROL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_LINK_CONTROL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_LINK_CONTROL);
            TableName = "R_LINK_CONTROL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_LINK_CONTROL> GetControlListByMainItem(string mainItem,OleExec db)
        {
            return db.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.MAIN_ITEM == mainItem).ToList();
        }

        public List<R_LINK_CONTROL> GetControlList(string type, string item, OleExec db)
        {
            if (!string.IsNullOrEmpty(item))
            {
                return db.ORM.Queryable<R_LINK_CONTROL>().Where(r => (r.MAIN_ITEM == item || r.SUB_ITEM == item) && r.CONTROL_TYPE == type).ToList();
            }
            else
            {
                return db.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.CONTROL_TYPE == type).ToList();
            }
        }

        public List<R_LINK_CONTROL> GetControlList(string type, string mainItem, string mainIVer, string subItem, string subVer, string category, OleExec db)
        {
            if (mainIVer == null || subVer == null)
            {
                return db.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.CONTROL_TYPE == type && r.MAIN_ITEM == mainItem && r.SUB_ITEM == subItem && r.CATEGORY == category).ToList();
            }
            else
            {
                return db.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.CONTROL_TYPE == type && r.MAIN_ITEM == mainItem && r.SUB_ITEM == subItem && r.MAIN_REV == mainIVer && r.SUB_REV == subVer && r.CATEGORY == category).ToList();
            }
        }
        public bool IsLinkControl(string mainItem, string subItem, OleExec db)
        {
            //return db.ORM.Queryable<R_LINK_CONTROL>().Where(r => r.MAIN_ITEM == mainItem && r.SUB_ITEM == subItem).Any();
            return db.ORM.Queryable<R_LINK_CONTROL>().Any(r => r.MAIN_ITEM == mainItem && r.SUB_ITEM == subItem);
        }
        public bool IsLinkControl(string type, string mainItem, string subItem, OleExec db)
        {
            return db.ORM.Queryable<R_LINK_CONTROL>().Any(r => r.MAIN_ITEM == mainItem && r.SUB_ITEM == subItem && r.CONTROL_TYPE == type);
        }

        public bool IsLinkControl(string type,string mainItem, string subItem, string category, OleExec db)
        {
            return db.ORM.Queryable<R_LINK_CONTROL>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(category), r => r.CATEGORY == category)
                .Where(r =>r.CONTROL_TYPE==type&& r.MAIN_ITEM == mainItem && r.SUB_ITEM == subItem)
                .Any();
        }
    }
    public class Row_R_LINK_CONTROL : DataObjectBase
    {
        public Row_R_LINK_CONTROL(DataObjectInfo info) : base(info)
        {

        }
        public R_LINK_CONTROL GetDataObject()
        {
            R_LINK_CONTROL DataObject = new R_LINK_CONTROL();
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.EDITBY = this.EDITBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.SUB_ITEM = this.SUB_ITEM;
            DataObject.MAIN_ITEM = this.MAIN_ITEM;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.MAIN_REV = this.MAIN_REV;
            DataObject.SUB_REV = this.SUB_REV;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string SUB_ITEM
        {
            get
            {
                return (string)this["SUB_ITEM"];
            }
            set
            {
                this["SUB_ITEM"] = value;
            }
        }
        public string MAIN_ITEM
        {
            get
            {
                return (string)this["MAIN_ITEM"];
            }
            set
            {
                this["MAIN_ITEM"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string MAIN_REV
        {
            get
            {
                return (string)this["MAIN_REV"];
            }
            set
            {
                this["MAIN_REV"] = value;
            }
        }
        public string SUB_REV
        {
            get
            {
                return (string)this["SUB_REV"];
            }
            set
            {
                this["SUB_REV"] = value;
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
    }
    public class R_LINK_CONTROL
    {
        public string CATEGORY { get; set; }
        public string EDITBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string SUB_ITEM { get; set; }
        public string MAIN_ITEM { get; set; }
        public string CONTROL_TYPE { get; set; }
        public string MAIN_REV { get; set; }
        public string SUB_REV { get; set; }
        public string ID { get; set; }
    }
}