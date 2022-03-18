using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_QUACK_SN_LOT_DETAIL : DataObjectTable
    {
        public T_H_QUACK_SN_LOT_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_QUACK_SN_LOT_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_QUACK_SN_LOT_DETAIL);
            TableName = "H_QUACK_SN_LOT_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// wzw 將R表數據插入到H表中
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public int InsertRInToHTable(string ID, string LOT_NO, DateTime? CREATE_DATE, string PARTNO, double? LOT_QTY, double? QTY, string VENDOR, string PO_NO, string PO_ITEM, string DATE_CODE, string REVERSION, string CLOSED_FLAG, string SAPGR_FLAG, DateTime? SAPGR_DATE, string SAPST_FLAG, DateTime? SAPST_DATE, string SAPGT_FLAG, DateTime? SAPGT_DATE, string EDIT_EMP, DateTime? EDIT_TIME, OleExec DB)
        public int InsertRInToHTable(OleExec DB)
        {
            int Num = 0;
            //string strsql = $@"INSERT H_QUACK_SN_LOT_DETAIL INTO VALUES ('{ID}', '{LOT_NO}', '{CREATE_DATE}', '{PARTNO}', '{LOT_QTY}', '{QTY}', '{VENDOR}', '{PO_NO}', '{PO_ITEM}', '{DATE_CODE}', '{REVERSION}', '{CLOSED_FLAG}', '{SAPGR_FLAG}', '{SAPGR_DATE}', '{SAPST_FLAG}', '{SAPST_DATE}', '{SAPGT_FLAG}', '{SAPGT_DATE}', '{EDIT_EMP}', '{EDIT_TIME}')";
            string strsql = $@"INSERT INTO H_QUACK_SN_LOT_DETAIL SELECT * FROM R_QUACK_SN_LOT_DETAIL";
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                Num = DB.ExecSqlNoReturn(strsql, null);
                return Num;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
    }
    public class Row_H_QUACK_SN_LOT_DETAIL : DataObjectBase
    {
        public Row_H_QUACK_SN_LOT_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public H_QUACK_SN_LOT_DETAIL GetDataObject()
        {
            H_QUACK_SN_LOT_DETAIL DataObject = new H_QUACK_SN_LOT_DETAIL();
            DataObject.ID = this.ID;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.CREATE_DATE = this.CREATE_DATE;
            DataObject.PARTNO = this.PARTNO;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.QTY = this.QTY;
            DataObject.VENDOR = this.VENDOR;
            DataObject.PO_NO = this.PO_NO;
            DataObject.PO_ITEM = this.PO_ITEM;
            DataObject.DATE_CODE = this.DATE_CODE;
            DataObject.REVERSION = this.REVERSION;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.SAPGR_FLAG = this.SAPGR_FLAG;
            DataObject.SAPGR_DATE = this.SAPGR_DATE;
            DataObject.SAPST_FLAG = this.SAPST_FLAG;
            DataObject.SAPST_DATE = this.SAPST_DATE;
            DataObject.SAPGT_FLAG = this.SAPGT_FLAG;
            DataObject.SAPGT_DATE = this.SAPGT_DATE;
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
        public DateTime? CREATE_DATE
        {
            get
            {
                return (DateTime?)this["CREATE_DATE"];
            }
            set
            {
                this["CREATE_DATE"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public double? LOT_QTY
        {
            get
            {
                return (double?)this["LOT_QTY"];
            }
            set
            {
                this["LOT_QTY"] = value;
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
        public string VENDOR
        {
            get
            {
                return (string)this["VENDOR"];
            }
            set
            {
                this["VENDOR"] = value;
            }
        }
        public string PO_NO
        {
            get
            {
                return (string)this["PO_NO"];
            }
            set
            {
                this["PO_NO"] = value;
            }
        }
        public string PO_ITEM
        {
            get
            {
                return (string)this["PO_ITEM"];
            }
            set
            {
                this["PO_ITEM"] = value;
            }
        }
        public string DATE_CODE
        {
            get
            {
                return (string)this["DATE_CODE"];
            }
            set
            {
                this["DATE_CODE"] = value;
            }
        }
        public string REVERSION
        {
            get
            {
                return (string)this["REVERSION"];
            }
            set
            {
                this["REVERSION"] = value;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public string SAPGR_FLAG
        {
            get
            {
                return (string)this["SAPGR_FLAG"];
            }
            set
            {
                this["SAPGR_FLAG"] = value;
            }
        }
        public DateTime? SAPGR_DATE
        {
            get
            {
                return (DateTime?)this["SAPGR_DATE"];
            }
            set
            {
                this["SAPGR_DATE"] = value;
            }
        }
        public string SAPST_FLAG
        {
            get
            {
                return (string)this["SAPST_FLAG"];
            }
            set
            {
                this["SAPST_FLAG"] = value;
            }
        }
        public DateTime? SAPST_DATE
        {
            get
            {
                return (DateTime?)this["SAPST_DATE"];
            }
            set
            {
                this["SAPST_DATE"] = value;
            }
        }
        public string SAPGT_FLAG
        {
            get
            {
                return (string)this["SAPGT_FLAG"];
            }
            set
            {
                this["SAPGT_FLAG"] = value;
            }
        }
        public DateTime? SAPGT_DATE
        {
            get
            {
                return (DateTime?)this["SAPGT_DATE"];
            }
            set
            {
                this["SAPGT_DATE"] = value;
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
    public class H_QUACK_SN_LOT_DETAIL
    {
        public string ID;
        public string LOT_NO;
        public DateTime? CREATE_DATE;
        public string PARTNO;
        public double? LOT_QTY;
        public double? QTY;
        public string VENDOR;
        public string PO_NO;
        public string PO_ITEM;
        public string DATE_CODE;
        public string REVERSION;
        public string CLOSED_FLAG;
        public string SAPGR_FLAG;
        public DateTime? SAPGR_DATE;
        public string SAPST_FLAG;
        public DateTime? SAPST_DATE;
        public string SAPGT_FLAG;
        public DateTime? SAPGT_DATE;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}