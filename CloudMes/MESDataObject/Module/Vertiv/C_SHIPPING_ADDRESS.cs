using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SHIPPING_ADDRESS : DataObjectTable
    {
        public T_C_SHIPPING_ADDRESS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SHIPPING_ADDRESS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SHIPPING_ADDRESS);
            TableName = "C_SHIPPING_ADDRESS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_SHIPPING_ADDRESS> GetShippingAddressList(OleExec DB)
        {
            return DB.ORM.Queryable<C_SHIPPING_ADDRESS>().ToList();
        }

        public bool ShippingAreaIsExist(string area, OleExec DB)
        {
            return DB.ORM.Queryable<C_SHIPPING_ADDRESS>().Any(s => s.SHIPPING_AREA == area);
        }

        public int AddNewShippingArea(string bu,string areaCode, string area,string address,string user,OleExec DB)
        {
            C_SHIPPING_ADDRESS shippingObject = new C_SHIPPING_ADDRESS();
            shippingObject.ID = this.GetNewID(bu, DB);
            shippingObject.AREA_CODE = areaCode;
            shippingObject.SHIPPING_AREA = area;
            shippingObject.SHIPPING_ADDRESS = address;            
            shippingObject.EDIT_EMP = user;
            shippingObject.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable(shippingObject).ExecuteCommand();
        }

        public int ModifyShippingAreaById(string id, string areaCode, string area, string address, string user, OleExec DB)
        {
            DateTime dt = GetDBDateTime(DB);
            return DB.ORM.Updateable<C_SHIPPING_ADDRESS>().UpdateColumns(
                s => new C_SHIPPING_ADDRESS()
                {
                    AREA_CODE = areaCode,
                    SHIPPING_AREA = area,
                    SHIPPING_ADDRESS = address,
                    EDIT_EMP = user,
                    EDIT_TIME = dt
                }).Where(s => s.ID == id).ExecuteCommand();
        }

        public int DeleteShippingAreaById(string id, OleExec DB)
        {
            return DB.ORM.Deleteable<C_SHIPPING_ADDRESS>().Where(s => s.ID == id).ExecuteCommand();
        }

        public C_SHIPPING_ADDRESS GetShippingAddressByAreaCode(OleExec DB, string areaCode)
        {
            return DB.ORM.Queryable<C_SHIPPING_ADDRESS>().Where(s => s.AREA_CODE == areaCode).ToList().FirstOrDefault();
        }
        public C_SHIPPING_ADDRESS GetShippingAddressByAddress(OleExec DB, string shippingAddress)
        {
            return DB.ORM.Queryable<C_SHIPPING_ADDRESS>().Where(s => s.SHIPPING_ADDRESS == shippingAddress).ToList().FirstOrDefault();
        }
        public C_SHIPPING_ADDRESS GetShippingAddressByShippingArea(OleExec DB, string shippingArea)
        {
            return DB.ORM.Queryable<C_SHIPPING_ADDRESS>().Where(s => s.SHIPPING_AREA == shippingArea).ToList().FirstOrDefault();
        }
        public string GetAddressByAreaCode(OleExec DB, string areaCode)
        {
            var list= DB.ORM.Queryable<C_SHIPPING_ADDRESS>().Where(s => s.AREA_CODE == areaCode).ToList();
            string address = "";
            foreach (var s in list)
            {
                address += s.SHIPPING_ADDRESS + ",";
            }
            return address.Substring(0, address.Length - 1);
        }
    }
    public class Row_C_SHIPPING_ADDRESS : DataObjectBase
    {
        public Row_C_SHIPPING_ADDRESS(DataObjectInfo info) : base(info)
        {

        }
        public C_SHIPPING_ADDRESS GetDataObject()
        {
            C_SHIPPING_ADDRESS DataObject = new C_SHIPPING_ADDRESS();
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.SHIPPING_ADDRESS = this.SHIPPING_ADDRESS;
            DataObject.SHIPPING_AREA = this.SHIPPING_AREA;
            DataObject.AREA_CODE = this.AREA_CODE;
            DataObject.ID = this.ID;
            return DataObject;
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
        public string SHIPPING_ADDRESS
        {
            get
            {
                return (string)this["SHIPPING_ADDRESS"];
            }
            set
            {
                this["SHIPPING_ADDRESS"] = value;
            }
        }
        public string SHIPPING_AREA
        {
            get
            {
                return (string)this["SHIPPING_AREA"];
            }
            set
            {
                this["SHIPPING_AREA"] = value;
            }
        }
        public string AREA_CODE
        {
            get
            {
                return (string)this["AREA_CODE"];
            }
            set
            {
                this["AREA_CODE"] = value;
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
    public class C_SHIPPING_ADDRESS
    {
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string SHIPPING_ADDRESS { get; set; }
        public string SHIPPING_AREA { get; set; }
        public string AREA_CODE { get; set; }
        public string ID { get; set; }
    }
}