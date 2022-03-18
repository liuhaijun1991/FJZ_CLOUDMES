using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_AGEING_TYPE : DataObjectTable
    {
        public T_C_AGEING_TYPE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_AGEING_TYPE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_AGEING_TYPE);
            TableName = "C_AGEING_TYPE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_AGEING_TYPE> GetAgeingTypeList(OleExec DB, string type)
        {
            return DB.ORM.Queryable<C_AGEING_TYPE>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(type), t => t.AGEING_TYPE == type).OrderBy(t => t.AGEING_TYPE).ToList();
        }

        public C_AGEING_TYPE GetObjectByTypeAndAddress(OleExec DB, string type, string areaCode)
        { 
            return DB.ORM.Queryable<C_AGEING_TYPE, C_SHIPPING_ADDRESS>((t, s) => t.AGEING_AREA_CODE == s.AREA_CODE)
                .Where((t, s) => t.AGEING_TYPE == type && s.SHIPPING_AREA == areaCode).Select((t, s) => t).ToList().FirstOrDefault();
        }

        public bool TypeIsExist(OleExec DB, string type,string areaCode)
        {
            return DB.ORM.Queryable<C_AGEING_TYPE>().Any(t => t.AGEING_TYPE == type && t.AGEING_AREA_CODE == areaCode);
        }

        public int AddNewAgeingType(string bu, string type, string areaCode, string maxTime, string maxPercentage, string minTime, string minPercentage, string user, OleExec DB)
        {
            C_AGEING_TYPE typeObject = new C_AGEING_TYPE();
            typeObject.ID = this.GetNewID(bu, DB);
            typeObject.AGEING_TYPE = type;
            typeObject.AGEING_AREA_CODE = areaCode;
            typeObject.MAX_AGEING_TIME = maxTime;
            typeObject.MAX_AGEING_PERCENTAGE = maxPercentage;
            typeObject.MIN_AGEING_TIME = minTime;
            typeObject.MIN_TIME_PERCENTAGE = minPercentage;
            typeObject.EDIT_EMP = user;
            typeObject.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable(typeObject).ExecuteCommand();
        }

        public int UpdateAgeingTypeById(string id, string areaCode, string maxTime, string maxPercentage, string minTime, string minPercentage, string user, OleExec DB)
        {
            DateTime dt = GetDBDateTime(DB);
            return DB.ORM.Updateable<C_AGEING_TYPE>().UpdateColumns(
                t => new C_AGEING_TYPE()
                {
                    AGEING_AREA_CODE = areaCode,
                    MAX_AGEING_TIME = maxTime,
                    MAX_AGEING_PERCENTAGE = maxPercentage,
                    MIN_AGEING_TIME = minTime,
                    MIN_TIME_PERCENTAGE = minPercentage,
                    EDIT_EMP = user,
                    EDIT_TIME = dt
                }).Where(t => t.ID == id).ExecuteCommand();
        }

        public int DeleteAgeingTypeById(string id,OleExec DB)
        {
            return DB.ORM.Deleteable<C_AGEING_TYPE>().Where(t => t.ID == id).ExecuteCommand();
        }
    }
    public class Row_C_AGEING_TYPE : DataObjectBase
    {
        public Row_C_AGEING_TYPE(DataObjectInfo info) : base(info)
        {

        }
        public C_AGEING_TYPE GetDataObject()
        {
            C_AGEING_TYPE DataObject = new C_AGEING_TYPE();
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.MIN_TIME_PERCENTAGE = this.MIN_TIME_PERCENTAGE;
            DataObject.MIN_AGEING_TIME = this.MIN_AGEING_TIME;
            DataObject.MAX_AGEING_PERCENTAGE = this.MAX_AGEING_PERCENTAGE;
            DataObject.MAX_AGEING_TIME = this.MAX_AGEING_TIME;
            DataObject.AGEING_AREA_CODE = this.AGEING_AREA_CODE;
            DataObject.AGEING_TYPE = this.AGEING_TYPE;
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
        public string MIN_TIME_PERCENTAGE
        {
            get
            {
                return (string)this["MIN_TIME_PERCENTAGE"];
            }
            set
            {
                this["MIN_TIME_PERCENTAGE"] = value;
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
        public string MAX_AGEING_PERCENTAGE
        {
            get
            {
                return (string)this["MAX_AGEING_PERCENTAGE"];
            }
            set
            {
                this["MAX_AGEING_PERCENTAGE"] = value;
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
    public class C_AGEING_TYPE
    {
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string MIN_TIME_PERCENTAGE { get; set; }
        public string MIN_AGEING_TIME { get; set; }
        public string MAX_AGEING_PERCENTAGE { get; set; }
        public string MAX_AGEING_TIME { get; set; }
        public string AGEING_AREA_CODE { get; set; }
        public string AGEING_TYPE { get; set; }
        public string ID { get; set; }
    }
}