using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_PACK_INFO : DataObjectTable
    {
        public T_R_PACK_INFO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PACK_INFO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PACK_INFO);
            TableName = "R_PACK_INFO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
              
    }
    public class Row_R_PACK_INFO : DataObjectBase
    {
        public Row_R_PACK_INFO(DataObjectInfo info) : base(info)
        {

        }
        public R_PACK_INFO GetDataObject()
        {
            R_PACK_INFO DataObject = new R_PACK_INFO();
            DataObject.ID = this.ID;
            DataObject.DN_NO = this.DN_NO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.QTY = this.QTY;
            DataObject.ROW_MATERIAL_PN = this.ROW_MATERIAL_PN;
            DataObject.ROW_MATERIAL_QTY = this.ROW_MATERIAL_QTY;
            DataObject.SHIPPING_TIME = this.SHIPPING_TIME;
            DataObject.TRANSACTION_202_STATUS = this.TRANSACTION_202_STATUS;
            DataObject.DN_CANCELED = this.DN_CANCELED;
            DataObject.TRANSACTION_201_STATUS = this.TRANSACTION_201_STATUS;
            DataObject.SAP_EXCEPTION = this.SAP_EXCEPTION;
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
        public string DN_NO
        {
            get
            {
                return (string)this["DN_NO"];
            }
            set
            {
                this["DN_NO"] = value;
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
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string ROW_MATERIAL_PN
        {
            get
            {
                return (string)this["ROW_MATERIAL_PN"];
            }
            set
            {
                this["ROW_MATERIAL_PN"] = value;
            }
        }
        public double? ROW_MATERIAL_QTY
        {
            get
            {
                return (double?)this["ROW_MATERIAL_QTY"];
            }
            set
            {
                this["ROW_MATERIAL_QTY"] = value;
            }
        }
        public DateTime? SHIPPING_TIME
        {
            get
            {
                return (DateTime?)this["SHIPPING_TIME"];
            }
            set
            {
                this["SHIPPING_TIME"] = value;
            }
        }
        public string TRANSACTION_202_STATUS
        {
            get
            {
                return (string)this["TRANSACTION_202_STATUS"];
            }
            set
            {
                this["TRANSACTION_202_STATUS"] = value;
            }
        }
        public string DN_CANCELED
        {
            get
            {
                return (string)this["DN_CANCELED"];
            }
            set
            {
                this["DN_CANCELED"] = value;
            }
        }
        public string TRANSACTION_201_STATUS
        {
            get
            {
                return (string)this["TRANSACTION_201_STATUS"];
            }
            set
            {
                this["TRANSACTION_201_STATUS"] = value;
            }
        }
        public string SAP_EXCEPTION
        {
            get
            {
                return (string)this["SAP_EXCEPTION"];
            }
            set
            {
                this["SAP_EXCEPTION"] = value;
            }
        }
    }
    public class R_PACK_INFO
    {
        public string ID { get; set; }
        public string DN_NO { get; set; }
        public string WORKORDERNO { get; set; }
        public string SKUNO { get; set; }
        public double? QTY { get; set; }
        public string ROW_MATERIAL_PN { get; set; }
        public double? ROW_MATERIAL_QTY { get; set; }
        public DateTime? SHIPPING_TIME { get; set; }
        public string TRANSACTION_202_STATUS { get; set; }
        public string DN_CANCELED { get; set; }
        public string TRANSACTION_201_STATUS { get; set; }
        public string SAP_EXCEPTION { get; set; }
    }
}