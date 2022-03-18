using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_LAYOUT : DataObjectTable
    {
        public T_R_QUACK_LAYOUT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_LAYOUT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_LAYOUT);
            TableName = "R_QUACK_LAYOUT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="LOT"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InLot(string LOT, OleExec DB)
        {
            int Num = 0;
            string strsql = $@" INSERT INTO R_QUACK_MISS_SN SELECT * FROM R_QUACK_MISS_SN WHERE LOT_NO='{LOT}' ";
            Num = DB.ExecSqlNoReturn(strsql, null);
            return Num;
        }
        /// <summary>
        /// wzw 將Oracle數據庫的內容插入SQLServer數據庫
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertSQLServer(string lotno, string factoryid, string LabelFrom, string LabelTo, double? totalqty, double? finishedqty, string closed, string currentstatus,
            string received, DateTime? receivedate, string receiveby, string delivered, DateTime? deliverdate, string deliverby, string sapid, string lasteditby, DateTime? lasteditdt, OleExec DB)
        {
            string strsql = $@"insert into qworklayout values( '{lotno}','{factoryid}','{LabelFrom}','{LabelTo}','{totalqty}','{finishedqty}','{closed}','{currentstatus}','{received}','{receivedate}','{receiveby}','{delivered}','{deliverdate}','{deliverby}','{sapid}','{lasteditby}','{lasteditdt}')";
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                int Num = DB.ExecSqlNoReturn(strsql, null);
                return Num;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_LAYOUT> GetLot(string Lot, OleExec DB)
        {
            string strsql = $@"SELECT * FROM R_QUACK_LAYOUT WHERE LOT_NO='{Lot}'";
            DataTable dt = DB.ExecSelect(strsql, null).Tables[0];
            Row_R_QUACK_LAYOUT RowRQUACKLAYOUT = (Row_R_QUACK_LAYOUT)NewRow();
            List<R_QUACK_LAYOUT> ListRQUACKLAYOUT = new List<R_QUACK_LAYOUT>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    RowRQUACKLAYOUT.loadData(item);
                    ListRQUACKLAYOUT.Add(RowRQUACKLAYOUT.GetDataObject());
                }
            }
            return ListRQUACKLAYOUT;
        }
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_LAYOUT> GetLotColsed(string Lot, OleExec DB)
        {
            string strsql = $@"SELECT * FROM R_QUACK_LAYOUT WHERE LOT_NO='{Lot}' AND CLOSED_FLAG = 1";
            DataTable dt = DB.ExecSelect(strsql, null).Tables[0];
            Row_R_QUACK_LAYOUT RowRQUACKLAYOUT = (Row_R_QUACK_LAYOUT)NewRow();
            List<R_QUACK_LAYOUT> ListRQUACKLAYOUT = new List<R_QUACK_LAYOUT>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    RowRQUACKLAYOUT.loadData(item);
                    ListRQUACKLAYOUT.Add(RowRQUACKLAYOUT.GetDataObject());
                }
            }
            return ListRQUACKLAYOUT;
        }
        /// <summary>
        /// WZW 新方法 Lambda 写法 LINQ 语法
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="CLOSED_FLAG"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_LAYOUT> QuackQueryByLotANDCLOSED(string Lot, string CLOSED_FLAG, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_LAYOUT>().Where(x => x.LOT_NO == Lot && x.CLOSED_FLAG == CLOSED_FLAG).ToList(); ;
        }
        /// <summary>
        /// wzw 
        /// </summary>
        /// <param name="Lot"></param>
        /// <returns></returns>
        public bool DeleteLot(string Lot, OleExec DB)
        {
            bool res = false;
            string strsql = $@"DELETE FROM R_QUACK_LAYOUT WHERE LOTNO='{Lot}'";
            int Num = DB.ExecSqlNoReturn(strsql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }

		public int InsertRowToRQuackLayoutRow(R_QUACK_LAYOUT RQuackLayoutRow, OleExec db)
        {
            //return db.ORM.Updateable(rQuackPoItemsRow).Where(t => t.PO_NO == rQuackPoItemsRow.PO_NO).ExecuteCommand();
            return db.ORM.Insertable(RQuackLayoutRow).ExecuteCommand();
        }

        public int UpdateRowByLotNo(R_QUACK_LAYOUT RQuackLayoutRow, string LotNo, OleExec db)
        {
            return db.ORM.Updateable(RQuackLayoutRow).Where(t => t.LOT_NO == LotNo).ExecuteCommand();
        }
        public R_QUACK_LAYOUT GetRowByLotNo( string LotNo, OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_LAYOUT>().Where(t => t.LOT_NO == LotNo).ToList().FirstOrDefault();
        }
		
    }
    public class Row_R_QUACK_LAYOUT : DataObjectBase
    {
        public Row_R_QUACK_LAYOUT(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_LAYOUT GetDataObject()
        {
            R_QUACK_LAYOUT DataObject = new R_QUACK_LAYOUT();
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
    public class R_QUACK_LAYOUT
    {
        public string ID { get; set; }
        public string LOT_NO { get; set; }
        public string PLANT { get; set; }
        public string LABEL_FROM { get; set; }
        public string LABEL_TO { get; set; }
        public double? TOTAL_QTY { get; set; }
        public double? FINISHED_QTY { get; set; }
        public string CLOSED_FLAG { get; set; }
        public string CURRENT_STATUS { get; set; }
        public string RECEIVED_FLAG { get; set; }
        public DateTime? RECEIVED_DATE { get; set; }
        public string RECEIVE_EMP { get; set; }
        public string DELIVERED_FLAG { get; set; }
        public DateTime? DELIVERD_DATE { get; set; }
        public string DELIVER_EMP { get; set; }
        public string SAP_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }

    }
}