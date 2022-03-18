using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_c_sku_ft_first_config : DataObjectTable
    {
        public T_c_sku_ft_first_config(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_c_sku_ft_first_config(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_c_sku_ft_first_config);
            TableName = "c_sku_ft_first_config".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_c_sku_ft_first_config : DataObjectBase
    {
        public Row_c_sku_ft_first_config(DataObjectInfo info) : base(info)
        {

        }
        public c_sku_ft_first_config GetDataObject()
        {
            c_sku_ft_first_config DataObject = new c_sku_ft_first_config();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FT1 = this.FT1;
            DataObject.FT2 = this.FT2;
            DataObject.FT3 = this.FT3;
            DataObject.ORT = this.ORT;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string FT1
        {
            get
            {
                return (string)this["FT1"];
            }
            set
            {
                this["FT1"] = value;
            }
        }
        public string FT2
        {
            get
            {
                return (string)this["FT2"];
            }
            set
            {
                this["FT2"] = value;
            }
        }
        public string FT3
        {
            get
            {
                return (string)this["FT3"];
            }
            set
            {
                this["FT3"] = value;
            }
        }
        public string ORT
        {
            get
            {
                return (string)this["ORT"];
            }
            set
            {
                this["ORT"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
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
    }
    public class c_sku_ft_first_config
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string FT1 { get; set; }
        public string FT2 { get; set; }
        public string FT3 { get; set; }
        public string ORT { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string CREATE_EMP { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}