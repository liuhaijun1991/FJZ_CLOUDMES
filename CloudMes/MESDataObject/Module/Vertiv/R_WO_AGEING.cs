using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_AGEING : DataObjectTable
    {
        public T_R_WO_AGEING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_AGEING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_AGEING);
            TableName = "R_WO_AGEING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public object GetWoAgeingList(OleExec DB, string wo)
        {
            return DB.ORM.Queryable<R_WO_AGEING, C_SHIPPING_ADDRESS>((r, c) => r.AGEING_AREA_CODE == c.SHIPPING_AREA)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(wo), (r, c) => r.WO == wo).OrderBy((r, c) => r.WO)
                .Select((r, c) => new {r.ID, r.WO, r.SKU, c.SHIPPING_ADDRESS,r.AGEING_TYPE, r.MAX_AGEING_TIME, r.MAX_TIME_QTY, r.MIN_AGEING_TIME, r.MIN_TIME_QTY, r.EDIT_EMP, r.EDIT_TIME }).ToList();
        }
        public R_WO_AGEING GetWoAgeingObject(OleExec DB, string wo)
        {
            return DB.ORM.Queryable<R_WO_AGEING>().Where(t => t.WO == wo).ToList().FirstOrDefault();
        }
        public int AddNewWoAgeing(OleExec DB,string bu,string wo,string sku,string areaCode,string type,string maxTime,string maxQty,string minTime,string minQty,string user)
        {
            R_WO_AGEING woAgeing = new R_WO_AGEING();
            woAgeing.ID = GetNewID(bu, DB);
            woAgeing.WO = wo;
            woAgeing.SKU = sku;
            woAgeing.AGEING_AREA_CODE = areaCode;
            woAgeing.AGEING_TYPE = type;
            woAgeing.MAX_AGEING_TIME = maxTime;
            woAgeing.MAX_TIME_QTY = maxQty;
            woAgeing.MIN_AGEING_TIME = minTime;
            woAgeing.MIN_TIME_QTY = minQty;
            woAgeing.EDIT_EMP = user;
            woAgeing.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable(woAgeing).ExecuteCommand();
        }

        public int AddNewWoAgeingAreaCode(OleExec DB, string bu, string wo,string sku, string areaCode, string user)
        {
            R_WO_AGEING woAgeing = new R_WO_AGEING();
            woAgeing.ID = GetNewID(bu, DB);
            woAgeing.WO = wo;
            woAgeing.SKU = sku;
            woAgeing.AGEING_AREA_CODE = areaCode;
            woAgeing.AGEING_TYPE = "";
            woAgeing.MAX_AGEING_TIME = "";
            woAgeing.MAX_TIME_QTY = "";
            woAgeing.MIN_AGEING_TIME = "";
            woAgeing.MIN_TIME_QTY = "";
            woAgeing.EDIT_EMP = user;
            woAgeing.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable(woAgeing).ExecuteCommand();
        }
        public int AddNewWoAgeingType(OleExec DB, string bu, string wo, string sku, string type, string user)
        {
            R_WO_AGEING woAgeing = new R_WO_AGEING();
            woAgeing.ID = GetNewID(bu, DB);
            woAgeing.WO = wo;
            woAgeing.SKU = sku;
            woAgeing.AGEING_AREA_CODE = "";
            woAgeing.AGEING_TYPE = type;
            woAgeing.MAX_AGEING_TIME = "";
            woAgeing.MAX_TIME_QTY = "";
            woAgeing.MIN_AGEING_TIME = "";
            woAgeing.MIN_TIME_QTY = "";
            woAgeing.EDIT_EMP = user;
            woAgeing.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable(woAgeing).ExecuteCommand();
        }

        public int ModifyWoAgeingById(OleExec DB, string id, string areaCode, string type, string maxTime, string maxQty, string minTime, string minQty, string user)
        {
            DateTime dt = GetDBDateTime(DB);
            return DB.ORM.Updateable<R_WO_AGEING>().UpdateColumns(
                r => new R_WO_AGEING()
                {
                    AGEING_AREA_CODE = areaCode,
                    AGEING_TYPE = type,
                    MAX_AGEING_TIME = maxTime,
                    MAX_TIME_QTY = maxQty,
                    MIN_AGEING_TIME = minTime,
                    MIN_TIME_QTY = minQty,
                    EDIT_EMP = user,
                    EDIT_TIME = dt
                }).Where(r => r.ID == id).ExecuteCommand();
        }

        public int ModifyWoAgeingAreaCodeById(OleExec DB, string id,string areaCode, string maxTime, string maxQty, string minTime, string minQty, string user)
        {
            DateTime dt = GetDBDateTime(DB);
            return DB.ORM.Updateable<R_WO_AGEING>().UpdateColumns(
                r => new R_WO_AGEING()
                {
                    AGEING_AREA_CODE = areaCode,
                    MAX_AGEING_TIME = maxTime,
                    MAX_TIME_QTY = maxQty,
                    MIN_AGEING_TIME = minTime,
                    MIN_TIME_QTY = minQty,
                    EDIT_EMP = user,
                    EDIT_TIME = dt
                }).Where(r => r.ID == id).ExecuteCommand();
        }

        public int ModifyWoAgeingTypeById(OleExec DB, string id, string type, string maxTime, string maxQty, string minTime, string minQty, string user)
        {
            DateTime dt = GetDBDateTime(DB);
            return DB.ORM.Updateable<R_WO_AGEING>().UpdateColumns(
                r => new R_WO_AGEING()
                {
                    AGEING_TYPE = type,
                    MAX_AGEING_TIME = maxTime,
                    MAX_TIME_QTY = maxQty,
                    MIN_AGEING_TIME = minTime,
                    MIN_TIME_QTY = minQty,
                    EDIT_EMP = user,
                    EDIT_TIME = dt
                }).Where(r => r.ID == id).ExecuteCommand();
        }

        public int DeleteWoAgeingById(OleExec DB, string id)
        {
            return DB.ORM.Deleteable<R_WO_AGEING>().Where(r => r.ID == id).ExecuteCommand();
        }

        public bool WoIsExist(OleExec DB, string wo)
        {
            return DB.ORM.Queryable<R_WO_AGEING>().Any(r => r.WO == wo);
        }

        public int UpdateAreaCodeByWO(OleExec DB, string wo, string areaCode)
        {
            return DB.ORM.Updateable<R_WO_AGEING>().UpdateColumns(r => new R_WO_AGEING() { AGEING_AREA_CODE = areaCode }).Where(r => r.WO == wo).ExecuteCommand();
        }

        public int UpdateAgeingTypeByWO(OleExec DB, string wo, string type)
        {
            return DB.ORM.Updateable<R_WO_AGEING>().UpdateColumns(r => new R_WO_AGEING() { AGEING_TYPE = type }).Where(r => r.WO == wo).ExecuteCommand();
        }


    }
    public class Row_R_WO_AGEING : DataObjectBase
    {
        public Row_R_WO_AGEING(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_AGEING GetDataObject()
        {
            R_WO_AGEING DataObject = new R_WO_AGEING();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.MIN_TIME_QTY = this.MIN_TIME_QTY;
            DataObject.MIN_AGEING_TIME = this.MIN_AGEING_TIME;
            DataObject.MAX_TIME_QTY = this.MAX_TIME_QTY;
            DataObject.MAX_AGEING_TIME = this.MAX_AGEING_TIME;
            DataObject.AGEING_TYPE = this.AGEING_TYPE;
            DataObject.AGEING_AREA_CODE = this.AGEING_AREA_CODE;
            DataObject.SKU = this.SKU;
            DataObject.WO = this.WO;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string MIN_TIME_QTY
        {
            get
            {
                return (string)this["MIN_TIME_QTY"];
            }
            set
            {
                this["MIN_TIME_QTY"] = value;
            }
        }
        public string MIN_AGEING_TIME
        {
            get
            {
                return (string)this["MIN_AGEING_TIME"];
            }
            set
            {
                this["MIN_AGEING_TIME"] = value;
            }
        }
        public string MAX_TIME_QTY
        {
            get
            {
                return (string)this["MAX_TIME_QTY"];
            }
            set
            {
                this["MAX_TIME_QTY"] = value;
            }
        }
        public string MAX_AGEING_TIME
        {
            get
            {
                return (string)this["MAX_AGEING_TIME"];
            }
            set
            {
                this["MAX_AGEING_TIME"] = value;
            }
        }
        public string AGEING_TYPE
        {
            get
            {
                return (string)this["AGEING_TYPE"];
            }
            set
            {
                this["AGEING_TYPE"] = value;
            }
        }
        public string AGEING_AREA_CODE
        {
            get
            {
                return (string)this["AGEING_AREA_CODE"];
            }
            set
            {
                this["AGEING_AREA_CODE"] = value;
            }
        }
        public string SKU
        {
            get
            {
                return (string)this["SKU"];
            }
            set
            {
                this["SKU"] = value;
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
    public class R_WO_AGEING
    {
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string MIN_TIME_QTY { get; set; }
        public string MIN_AGEING_TIME { get; set; }
        public string MAX_TIME_QTY { get; set; }
        public string MAX_AGEING_TIME { get; set; }
        public string AGEING_TYPE { get; set; }
        public string AGEING_AREA_CODE { get; set; }
        public string SKU { get; set; }
        public string WO { get; set; }
        public string ID { get; set; }
    }
}