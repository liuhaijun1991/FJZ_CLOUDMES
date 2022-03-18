using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PN_MASTER_DATA : DataObjectTable
    {
        public T_R_PN_MASTER_DATA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PN_MASTER_DATA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PN_MASTER_DATA);
            TableName = "R_PN_MASTER_DATA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_PN_MASTER_DATA GetMasterObj(OleExec DB, string skuno)
        {
            return DB.ORM.Queryable<R_PN_MASTER_DATA>().Where(r => r.PN == skuno).ToList().FirstOrDefault();
        }

        public int Update(OleExec DB, R_PN_MASTER_DATA master)
        {
            return DB.ORM.Updateable<R_PN_MASTER_DATA>(master).Where(r => r.ID == master.ID).ExecuteCommand();
        }

        public int Save(OleExec DB,R_PN_MASTER_DATA master)
        {
            return DB.ORM.Insertable<R_PN_MASTER_DATA>(master).ExecuteCommand();
        }

    }
    public class Row_R_PN_MASTER_DATA : DataObjectBase
    {
        public Row_R_PN_MASTER_DATA(DataObjectInfo info) : base(info)
        {

        }
        public R_PN_MASTER_DATA GetDataObject()
        {
            R_PN_MASTER_DATA DataObject = new R_PN_MASTER_DATA();
            DataObject.IMPORT_USER = this.IMPORT_USER;
            DataObject.LAST_EDIT_DT = this.LAST_EDIT_DT;
            DataObject.LAST_EDIT_BY = this.LAST_EDIT_BY;
            DataObject.ID = this.ID;
            DataObject.PLANT = this.PLANT;
            DataObject.PN = this.PN;
            DataObject.VERSION = this.VERSION;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CPUDT = this.CPUDT;
            DataObject.SELLING_PRICE = this.SELLING_PRICE;
            DataObject.PRICE_UNIT = this.PRICE_UNIT;
            DataObject.CURRENCY = this.CURRENCY;
            DataObject.CONDITION_UNIT = this.CONDITION_UNIT;
            DataObject.NET_WEIGHT = this.NET_WEIGHT;
            DataObject.WEIGHT_UNIT = this.WEIGHT_UNIT;
            DataObject.NET_WEIGHT_BY_UNIT = this.NET_WEIGHT_BY_UNIT;
            DataObject.ENABLED = this.ENABLED;
            DataObject.STANDARD_VALUE = this.STANDARD_VALUE;
            DataObject.UNIT = this.UNIT;
            DataObject.PRICE = this.PRICE;
            DataObject.PRICE1 = this.PRICE1;
            DataObject.PRICE2 = this.PRICE2;
            DataObject.PRICE3 = this.PRICE3;
            DataObject.SAP_WEIGHT = this.SAP_WEIGHT;
            DataObject.WEIGHT1 = this.WEIGHT1;
            DataObject.WEIGHT2 = this.WEIGHT2;
            DataObject.CHINESE_DESC = this.CHINESE_DESC;
            DataObject.IMPORT_DATE_TIME = this.IMPORT_DATE_TIME;
            return DataObject;
        }
        public string IMPORT_USER
        {
            get
            {
                return (string)this["IMPORT_USER"];
            }
            set
            {
                this["IMPORT_USER"] = value;
            }
        }
        public DateTime? LAST_EDIT_DT
        {
            get
            {
                return (DateTime?)this["LAST_EDIT_DT"];
            }
            set
            {
                this["LAST_EDIT_DT"] = value;
            }
        }
        public string LAST_EDIT_BY
        {
            get
            {
                return (string)this["LAST_EDIT_BY"];
            }
            set
            {
                this["LAST_EDIT_BY"] = value;
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
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public DateTime? CPUDT
        {
            get
            {
                return (DateTime?)this["CPUDT"];
            }
            set
            {
                this["CPUDT"] = value;
            }
        }
        public double? SELLING_PRICE
        {
            get
            {
                return (double?)this["SELLING_PRICE"];
            }
            set
            {
                this["SELLING_PRICE"] = value;
            }
        }
        public double? PRICE_UNIT
        {
            get
            {
                return (double?)this["PRICE_UNIT"];
            }
            set
            {
                this["PRICE_UNIT"] = value;
            }
        }
        public string CURRENCY
        {
            get
            {
                return (string)this["CURRENCY"];
            }
            set
            {
                this["CURRENCY"] = value;
            }
        }
        public string CONDITION_UNIT
        {
            get
            {
                return (string)this["CONDITION_UNIT"];
            }
            set
            {
                this["CONDITION_UNIT"] = value;
            }
        }
        public double? NET_WEIGHT
        {
            get
            {
                return (double?)this["NET_WEIGHT"];
            }
            set
            {
                this["NET_WEIGHT"] = value;
            }
        }
        public string WEIGHT_UNIT
        {
            get
            {
                return (string)this["WEIGHT_UNIT"];
            }
            set
            {
                this["WEIGHT_UNIT"] = value;
            }
        }
        public double? NET_WEIGHT_BY_UNIT
        {
            get
            {
                return (double?)this["NET_WEIGHT_BY_UNIT"];
            }
            set
            {
                this["NET_WEIGHT_BY_UNIT"] = value;
            }
        }
        public string ENABLED
        {
            get
            {
                return (string)this["ENABLED"];
            }
            set
            {
                this["ENABLED"] = value;
            }
        }
        public double? STANDARD_VALUE
        {
            get
            {
                return (double?)this["STANDARD_VALUE"];
            }
            set
            {
                this["STANDARD_VALUE"] = value;
            }
        }
        public string UNIT
        {
            get
            {
                return (string)this["UNIT"];
            }
            set
            {
                this["UNIT"] = value;
            }
        }
        public double? PRICE
        {
            get
            {
                return (double?)this["PRICE"];
            }
            set
            {
                this["PRICE"] = value;
            }
        }
        public double? PRICE1
        {
            get
            {
                return (double?)this["PRICE1"];
            }
            set
            {
                this["PRICE1"] = value;
            }
        }
        public double? PRICE2
        {
            get
            {
                return (double?)this["PRICE2"];
            }
            set
            {
                this["PRICE2"] = value;
            }
        }
        public double? PRICE3
        {
            get
            {
                return (double?)this["PRICE3"];
            }
            set
            {
                this["PRICE3"] = value;
            }
        }
        public double? SAP_WEIGHT
        {
            get
            {
                return (double?)this["SAP_WEIGHT"];
            }
            set
            {
                this["SAP_WEIGHT"] = value;
            }
        }
        public double? WEIGHT1
        {
            get
            {
                return (double?)this["WEIGHT1"];
            }
            set
            {
                this["WEIGHT1"] = value;
            }
        }
        public double? WEIGHT2
        {
            get
            {
                return (double?)this["WEIGHT2"];
            }
            set
            {
                this["WEIGHT2"] = value;
            }
        }
        public string CHINESE_DESC
        {
            get
            {
                return (string)this["CHINESE_DESC"];
            }
            set
            {
                this["CHINESE_DESC"] = value;
            }
        }
        public DateTime? IMPORT_DATE_TIME
        {
            get
            {
                return (DateTime?)this["IMPORT_DATE_TIME"];
            }
            set
            {
                this["IMPORT_DATE_TIME"] = value;
            }
        }
    }
    public class R_PN_MASTER_DATA
    {
        private double? _net_weight_by_unit;
        private double? _price;

        public string IMPORT_USER { get; set; }
        public DateTime? LAST_EDIT_DT { get; set; }
        public string LAST_EDIT_BY { get; set; }
        public string ID { get; set; }
        public string PLANT { get; set; }
        public string PN { get; set; }
        public string VERSION { get; set; }
        public string DESCRIPTION { get; set; }
        public DateTime? CPUDT { get; set; }
        public double? SELLING_PRICE { get; set; }
        public double? PRICE_UNIT { get; set; }
        public string CURRENCY { get; set; }
        public string CONDITION_UNIT { get; set; }
        public double? NET_WEIGHT { get; set; }
        public string WEIGHT_UNIT { get; set; }
        public double? NET_WEIGHT_BY_UNIT
        {
            get { return _net_weight_by_unit; }
            set
            {
                if (WEIGHT_UNIT != null )
                {
                    if (WEIGHT_UNIT.ToUpper() == "MG")
                    {
                        _net_weight_by_unit = NET_WEIGHT != null ? NET_WEIGHT / 1000000 : null;
                    }
                }
                else if (WEIGHT_UNIT != null  )
                {
                    if (WEIGHT_UNIT.ToUpper() == "G")
                    {
                        _net_weight_by_unit = NET_WEIGHT != null ? NET_WEIGHT / 1000 : null;
                    }
                }
                else
                {
                    _net_weight_by_unit = NET_WEIGHT;
                }
            }
        }
        public string ENABLED { get; set; }
        public double? STANDARD_VALUE { get; set; }
        public string UNIT { get; set; }
        public double? PRICE
        {
            get { return _price; }
            set
            {
                if (PRICE_UNIT == 0 || PRICE_UNIT == null)
                {
                    _price = SELLING_PRICE;
                }
                else
                {
                    _price = SELLING_PRICE != null ? SELLING_PRICE / PRICE_UNIT : null;
                }
            }
        }
        public double? PRICE1 { get; set; }
        public double? PRICE2 { get; set; }
        public double? PRICE3 { get; set; }
        public double? SAP_WEIGHT { get; set; }
        public double? WEIGHT1 { get; set; }
        public double? WEIGHT2 { get; set; }
        public string CHINESE_DESC { get; set; }
        public DateTime? IMPORT_DATE_TIME { get; set; }
    }
}