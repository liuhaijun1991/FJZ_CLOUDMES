using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_QUACK_LAYOUT : DataObjectTable
    {
        public T_H_QUACK_LAYOUT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_QUACK_LAYOUT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_QUACK_LAYOUT);
            TableName = "H_QUACK_LAYOUT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// wzw 將R表數據插入到H表中
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public int InsertRInToHTable(string ID, string LOT_NO, string PLANT, string LABEL_FROM, string LABEL_TO, double? TOTAL_QTY, double? FINISHED_QTY, string CLOSED_FLAG, string CURRENT_STATUS, string RECEIVED_FLAG, DateTime? RECEIVED_DATE, string RECEIVE_EMP, string DELIVERED_FLAG, DateTime? DELIVERD_DATE, string DELIVER_EMP, string SAP_FLAG, string EDIT_EMP, DateTime? EDIT_TIME, OleExec DB)
        public int InsertRInToHTable(OleExec DB)
        {
            int Num = 0;
            //string strsql = $@"INSERT INTO VALUES ('{ID}', '{LOT_NO}', '{PLANT}', '{LABEL_FROM}', '{LABEL_TO}', '{TOTAL_QTY}', '{FINISHED_QTY}', '{CLOSED_FLAG}', '{CURRENT_STATUS}', '{RECEIVED_FLAG}', '{RECEIVED_DATE}', '{RECEIVE_EMP}', '{DELIVERED_FLAG}', '{DELIVERD_DATE}', '{DELIVER_EMP}', '{SAP_FLAG}', '{EDIT_EMP}', '{EDIT_TIME}')";
            string strsql = $@"INSERT INTO H_QUACK_LAYOUT SELECT * FROM R_QUACK_LAYOUT";
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
    public class Row_H_QUACK_LAYOUT : DataObjectBase
    {
        public Row_H_QUACK_LAYOUT(DataObjectInfo info) : base(info)
        {

        }
        public H_QUACK_LAYOUT GetDataObject()
        {
            H_QUACK_LAYOUT DataObject = new H_QUACK_LAYOUT();
            DataObject.ID = this.ID;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.PLANT = this.PLANT;
            DataObject.LABEL_FROM = this.LABEL_FROM;
            DataObject.LABEL_TO = this.LABEL_TO;
            DataObject.TOTAL_QTY = this.TOTAL_QTY;
            DataObject.FINISHED_QTY = this.FINISHED_QTY;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.CURRENT_STATUS = this.CURRENT_STATUS;
            DataObject.RECEIVED_FLAG = this.RECEIVED_FLAG;
            DataObject.RECEIVED_DATE = this.RECEIVED_DATE;
            DataObject.RECEIVE_EMP = this.RECEIVE_EMP;
            DataObject.DELIVERED_FLAG = this.DELIVERED_FLAG;
            DataObject.DELIVERD_DATE = this.DELIVERD_DATE;
            DataObject.DELIVER_EMP = this.DELIVER_EMP;
            DataObject.SAP_FLAG = this.SAP_FLAG;
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
        public string LABEL_FROM
        {
            get
            {
                return (string)this["LABEL_FROM"];
            }
            set
            {
                this["LABEL_FROM"] = value;
            }
        }
        public string LABEL_TO
        {
            get
            {
                return (string)this["LABEL_TO"];
            }
            set
            {
                this["LABEL_TO"] = value;
            }
        }
        public double? TOTAL_QTY
        {
            get
            {
                return (double?)this["TOTAL_QTY"];
            }
            set
            {
                this["TOTAL_QTY"] = value;
            }
        }
        public double? FINISHED_QTY
        {
            get
            {
                return (double?)this["FINISHED_QTY"];
            }
            set
            {
                this["FINISHED_QTY"] = value;
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
        public string CURRENT_STATUS
        {
            get
            {
                return (string)this["CURRENT_STATUS"];
            }
            set
            {
                this["CURRENT_STATUS"] = value;
            }
        }
        public string RECEIVED_FLAG
        {
            get
            {
                return (string)this["RECEIVED_FLAG"];
            }
            set
            {
                this["RECEIVED_FLAG"] = value;
            }
        }
        public DateTime? RECEIVED_DATE
        {
            get
            {
                return (DateTime?)this["RECEIVED_DATE"];
            }
            set
            {
                this["RECEIVED_DATE"] = value;
            }
        }
        public string RECEIVE_EMP
        {
            get
            {
                return (string)this["RECEIVE_EMP"];
            }
            set
            {
                this["RECEIVE_EMP"] = value;
            }
        }
        public string DELIVERED_FLAG
        {
            get
            {
                return (string)this["DELIVERED_FLAG"];
            }
            set
            {
                this["DELIVERED_FLAG"] = value;
            }
        }
        public DateTime? DELIVERD_DATE
        {
            get
            {
                return (DateTime?)this["DELIVERD_DATE"];
            }
            set
            {
                this["DELIVERD_DATE"] = value;
            }
        }
        public string DELIVER_EMP
        {
            get
            {
                return (string)this["DELIVER_EMP"];
            }
            set
            {
                this["DELIVER_EMP"] = value;
            }
        }
        public string SAP_FLAG
        {
            get
            {
                return (string)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
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
    public class H_QUACK_LAYOUT
    {
        public string ID;
        public string LOT_NO;
        public string PLANT;
        public string LABEL_FROM;
        public string LABEL_TO;
        public double? TOTAL_QTY;
        public double? FINISHED_QTY;
        public string CLOSED_FLAG;
        public string CURRENT_STATUS;
        public string RECEIVED_FLAG;
        public DateTime? RECEIVED_DATE;
        public string RECEIVE_EMP;
        public string DELIVERED_FLAG;
        public DateTime? DELIVERD_DATE;
        public string DELIVER_EMP;
        public string SAP_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}