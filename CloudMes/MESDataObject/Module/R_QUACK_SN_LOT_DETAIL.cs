using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_SN_LOT_DETAIL : DataObjectTable
    {
        public T_R_QUACK_SN_LOT_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_SN_LOT_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_SN_LOT_DETAIL);
            TableName = "R_QUACK_SN_LOT_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// wzw
        /// </summary>
        /// <param name="DB"></param>
        public void InH_QUACK_SN(string BU, string LOT_NO, string CREATE_DATE, string PARTNO, double LOT_QTY, double QTY, string VENDOR, string PO_NO, string PO_ITEM,
            string DATE_CODE, string REVERSION, string CLOSED_FLAG, string SAPGR_FLAG, string SAPGR_DATE, string SAPST_FLAG, string SAPST_DATE, string SAPGT_FLAG,
            string SAPGT_, string EDIT_EMP, string EDIT_TIME, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_QUACK_SN_LOT_DETAIL Table_R_QUACK_SN_LOT_DETAIL = new T_R_QUACK_SN_LOT_DETAIL(DB, DBType);
                    Row_R_QUACK_SN_LOT_DETAIL Row_R_QUACK_SN_LOT_DETAIL = (Row_R_QUACK_SN_LOT_DETAIL)NewRow();
                    string ID = "";
                    ID = Table_R_QUACK_SN_LOT_DETAIL.GetNewID(BU, DB);
                    Row_R_QUACK_SN_LOT_DETAIL.ID = ID;
                    Row_R_QUACK_SN_LOT_DETAIL.LOT_NO = LOT_NO;
                    Row_R_QUACK_SN_LOT_DETAIL.PARTNO = PARTNO;
                    Row_R_QUACK_SN_LOT_DETAIL.LOT_QTY = LOT_QTY;
                    Row_R_QUACK_SN_LOT_DETAIL.QTY = QTY;
                    Row_R_QUACK_SN_LOT_DETAIL.VENDOR = VENDOR;
                    Row_R_QUACK_SN_LOT_DETAIL.PO_NO = PO_NO;
                    Row_R_QUACK_SN_LOT_DETAIL.PO_ITEM = PO_ITEM;
                    Row_R_QUACK_SN_LOT_DETAIL.DATE_CODE = DATE_CODE;
                    Row_R_QUACK_SN_LOT_DETAIL.REVERSION = REVERSION;
                    Row_R_QUACK_SN_LOT_DETAIL.CLOSED_FLAG = CLOSED_FLAG;
                    Row_R_QUACK_SN_LOT_DETAIL.SAPGR_FLAG = SAPGR_FLAG;
                    Row_R_QUACK_SN_LOT_DETAIL.SAPGR_DATE = GetDBDateTime(DB);
                    Row_R_QUACK_SN_LOT_DETAIL.SAPST_FLAG = SAPST_FLAG;
                    Row_R_QUACK_SN_LOT_DETAIL.SAPST_DATE = GetDBDateTime(DB);
                    Row_R_QUACK_SN_LOT_DETAIL.SAPGT_FLAG = SAPGT_FLAG;
                    Row_R_QUACK_SN_LOT_DETAIL.SAPGT_DATE = GetDBDateTime(DB);
                    Row_R_QUACK_SN_LOT_DETAIL.EDIT_EMP = EDIT_EMP;
                    Row_R_QUACK_SN_LOT_DETAIL.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_QUACK_SN_LOT_DETAIL.GetInsertString(DBType));
                }
                catch (Exception)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { });
                    throw new MESReturnMessage(errMsg);
                }
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
        public int InsertSQLServer(string LOTNO, DateTime? Createdate, string PARTNO, double? lotqty, double? QTY, string VENDOR, string PONO, string POItem, string DATECODE,
            string REVERSION, string CLOSED, DateTime? lasteditdt, string lasteditby, string sapgrflag, DateTime? sapgrdt, string sapstflag, DateTime? sapstdt, string sapgtflag,
            DateTime? sapgtdt, string sapflag, OleExec DB)
        {
            string strsql = $@"insert into qlotdetail values( '{LOTNO}','{Createdate}','{PARTNO}','{lotqty}','{QTY}','{VENDOR}','{PONO}','{POItem}','{DATECODE}','{REVERSION}','{CLOSED}','{lasteditdt}','{lasteditby}','{sapgrflag}','{sapgrdt}','{sapstflag}','{sapstdt}','{sapgtflag}','{sapgtdt}')";
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
        /// WZW LOT CLOSED =1
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN_LOT_DETAIL> GetLotClosed(string Lot, OleExec DB)
        {
            string strsql = $@"SELECT * FROM R_QUACK_SN_LOT_DETAIL WHERE LOT_NO='{Lot}' AND CLOSED_FLAG = 1";
            DataTable dt = DB.ExecSelect(strsql, null).Tables[0];
            Row_R_QUACK_SN_LOT_DETAIL RowRQUACKSNLOTDETAIL = (Row_R_QUACK_SN_LOT_DETAIL)NewRow();
            List<R_QUACK_SN_LOT_DETAIL> ListRQUACKSNLOTDETAIL = new List<R_QUACK_SN_LOT_DETAIL>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    RowRQUACKSNLOTDETAIL.loadData(item);
                    ListRQUACKSNLOTDETAIL.Add(RowRQUACKSNLOTDETAIL.GetDataObject());
                }
            }
            return ListRQUACKSNLOTDETAIL;
        }
        /// <summary>
        /// WZW 新方法 Lambda 写法 LINQ 语法
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="CLOSED_FLAG"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN_LOT_DETAIL> QuackDetailQueryByLotANDCLOSED(string Lot, string CLOSED_FLAG, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN_LOT_DETAIL>().Where(x => x.LOT_NO == Lot && x.CLOSED_FLAG == CLOSED_FLAG).ToList(); ;
        }
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN_LOT_DETAIL> GetLot(string Lot, OleExec DB)
        {
            string strsql = $@"SELECT * FROM R_QUACK_SN_LOT_DETAIL WHERE LOT_NO='{Lot}'";
            DataTable dt = DB.ExecSelect(strsql, null).Tables[0];
            Row_R_QUACK_SN_LOT_DETAIL RowRQUACKSNLOTDETAIL = (Row_R_QUACK_SN_LOT_DETAIL)NewRow();
            List<R_QUACK_SN_LOT_DETAIL> ListRQUACKSNLOTDETAIL = new List<R_QUACK_SN_LOT_DETAIL>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    RowRQUACKSNLOTDETAIL.loadData(item);
                    ListRQUACKSNLOTDETAIL.Add(RowRQUACKSNLOTDETAIL.GetDataObject());
                }
            }
            return ListRQUACKSNLOTDETAIL;
        }
        /// <summary>
        /// wzw 
        /// </summary>
        /// <param name="Lot"></param>
        /// <returns></returns>
        public bool DeleteLot(string Lot, OleExec DB)
        {
            bool res = false;
            string strsql = $@"DELETE FROM R_QUACK_SN_LOT_DETAIL WHERE LOTNO='{Lot}'";
            int Num = DB.ExecSqlNoReturn(strsql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }

        public int UpdateRowByLotNo(R_QUACK_SN_LOT_DETAIL rQuackSnLotDetailRow, OleExec db)
        {
            return db.ORM.Updateable(rQuackSnLotDetailRow).Where(t => t.LOT_NO == rQuackSnLotDetailRow.LOT_NO).ExecuteCommand();
        }

		public int InsertRow(R_QUACK_SN_LOT_DETAIL rQuackSnLotDetailRow, OleExec db)
        {
            return db.ORM.Insertable(rQuackSnLotDetailRow).ExecuteCommand();
        }
        public List<R_QUACK_SN_LOT_DETAIL> GetRowsByPoNo(string poNo, OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_SN_LOT_DETAIL>().Where(t => t.PO_NO == poNo).ToList();
        }

        public R_QUACK_SN_LOT_DETAIL GetRowByLotNo(string lotNo, OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_SN_LOT_DETAIL>().Where(t => t.LOT_NO == lotNo).ToList().FirstOrDefault();
        }

    }
    public class Row_R_QUACK_SN_LOT_DETAIL : DataObjectBase
    {
        public Row_R_QUACK_SN_LOT_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_SN_LOT_DETAIL GetDataObject()
        {
            R_QUACK_SN_LOT_DETAIL DataObject = new R_QUACK_SN_LOT_DETAIL();
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
    public class R_QUACK_SN_LOT_DETAIL
    {
        public string ID { get; set; }
        public string LOT_NO { get; set; }
        public DateTime? CREATE_DATE { get; set; }
        public string PARTNO { get; set; }
        public double? LOT_QTY { get; set; }
        public double? QTY { get; set; }
        public string VENDOR { get; set; }
        public string PO_NO { get; set; }
        public string PO_ITEM { get; set; }
        public string DATE_CODE { get; set; }
        public string REVERSION { get; set; }
        public string CLOSED_FLAG { get; set; }
        public string SAPGR_FLAG { get; set; }
        public DateTime? SAPGR_DATE { get; set; }
        public string SAPST_FLAG { get; set; }
        public DateTime? SAPST_DATE { get; set; }
        public string SAPGT_FLAG { get; set; }
        public DateTime? SAPGT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}