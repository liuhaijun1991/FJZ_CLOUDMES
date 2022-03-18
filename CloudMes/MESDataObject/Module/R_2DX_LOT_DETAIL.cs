using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_2DX_LOT_DETAIL : DataObjectTable
    {
        public T_R_2DX_LOT_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DX_LOT_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DX_LOT_DETAIL);
            TableName = "R_2DX_LOT_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 插入數據 R_2DX_LOT_DETAIL
        /// </summary>
        /// <param name="_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void/*int*/ InLot(string BU, string LotID,string LotNo,string SN,string WO, string SKU,string STATION,string STATUS_FLAG, double SEQNO,string SAMPLE_FLAG, string Emp, OleExec DB)
        {
            //string strsql = "";
            //int Res;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_2DX_LOT_DETAIL Table_R_Lot_DETAIL = new T_R_2DX_LOT_DETAIL(DB, DBType);
                    Row_R_2DX_LOT_DETAIL Row_R_Lot_DETAIL = (Row_R_2DX_LOT_DETAIL)NewRow();
                    string Detail_ID = "";
                    Detail_ID = Table_R_Lot_DETAIL.GetNewID(BU, DB);
                    Row_R_Lot_DETAIL.ID = Detail_ID;
                    Row_R_Lot_DETAIL.LOT_ID = LotID;
                    Row_R_Lot_DETAIL.SN = SN;
                    Row_R_Lot_DETAIL.WO = WO;
                    Row_R_Lot_DETAIL.SKUNO = SKU;
                    Row_R_Lot_DETAIL.STATION = STATION;
                    Row_R_Lot_DETAIL.STATUS_FLAG = STATUS_FLAG;
                    Row_R_Lot_DETAIL.SEQNO = SEQNO;
                    Row_R_Lot_DETAIL.SAMPLE_FLAG = SAMPLE_FLAG;
                    Row_R_Lot_DETAIL.EDIT_EMP = Emp;
                    Row_R_Lot_DETAIL.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_Lot_DETAIL.GetInsertString(DBType));
                }
                catch (Exception)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { LotNo });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public R_2DX_LOT_DETAIL Get2DXLotDetailByLotIDSN(string LotID,string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_2DX_LOT_DETAIL>().Where(t => t.LOT_ID == LotID && t.SN==SN).ToList().FirstOrDefault();
        }
        public double GetSeqNoByLot(string LotID, OleExec DB)
        {
            string QuerySql = $@"select max(SEQNO) from R_2DX_LOT_DETAIL where LOT_ID='{LotID}'";
            string DBSeq = DB.ExecSqlReturn(QuerySql).ToString();
            double SeqNo;
            if (DBSeq.Equals(""))
            {
                SeqNo = 0;
            }
            else { SeqNo = double.Parse(DBSeq); }
            return SeqNo;
        }
    }
    public class Row_R_2DX_LOT_DETAIL : DataObjectBase
    {
        public Row_R_2DX_LOT_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_2DX_LOT_DETAIL GetDataObject()
        {
            R_2DX_LOT_DETAIL DataObject = new R_2DX_LOT_DETAIL();
            DataObject.ID = this.ID;
            DataObject.LOT_ID = this.LOT_ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION = this.STATION;
            DataObject.STATUS_FLAG = this.STATUS_FLAG;
            DataObject.SEQNO = this.SEQNO;
            DataObject.SAMPLE_FLAG = this.SAMPLE_FLAG;
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
        public string LOT_ID
        {
            get
            {
                return (string)this["LOT_ID"];
            }
            set
            {
                this["LOT_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
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
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string STATUS_FLAG
        {
            get
            {
                return (string)this["STATUS_FLAG"];
            }
            set
            {
                this["STATUS_FLAG"] = value;
            }
        }
        public double? SEQNO
        {
            get
            {
                return (double?)this["SEQNO"];
            }
            set
            {
                this["SEQNO"] = value;
            }
        }
        public string SAMPLE_FLAG
        {
            get
            {
                return (string)this["SAMPLE_FLAG"];
            }
            set
            {
                this["SAMPLE_FLAG"] = value;
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
    public class R_2DX_LOT_DETAIL
    {
        public string ID;
        public string LOT_ID;
        public string SN;
        public string WO;
        public string SKUNO;
        public string STATION;
        public string STATUS_FLAG;
        public double? SEQNO;
        public string SAMPLE_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}