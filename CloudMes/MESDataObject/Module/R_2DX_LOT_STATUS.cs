using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_2DX_LOT_STATUS : DataObjectTable
    {
        public T_R_2DX_LOT_STATUS(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2DX_LOT_STATUS(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2DX_LOT_STATUS);
            TableName = "R_2DX_LOT_STATUS".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 生成LOT號
        /// </summary>
        /// <param name="_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void/*int*/ InLot(string BU, string Lot, string SKU, double LOT_SIZE, string StationName, string Line, string Emp, OleExec DB)
        {
            //string strsql = "";
            //int Res;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_2DX_LOT_STATUS Table_R_Lot_Status = new T_R_2DX_LOT_STATUS(DB, DBType);
                    Row_R_2DX_LOT_STATUS Row_R_Lot_Status = (Row_R_2DX_LOT_STATUS)NewRow();
                    //T_R_LOT_DETAIL Table_R_Lot_Detail = new T_R_LOT_DETAIL(DB, DBType);
                    //Row_R_LOT_DETAIL Row_R_Lot_Detail = (Row_R_LOT_DETAIL)Table_R_Lot_Detail.NewRow();
                    //T_C_AQLTYPE Table_C_AQLTYPE = new T_C_AQLTYPE(DB, DBType);
                    //                strsql = $@"INSERT INTO R_LOT_STATUS (ID, LOT_NO, SKUNO, AQL_TYPE, LOT_QTY, REJECT_QTY, SAMPLE_QTY, PASS_QTY, FAIL_QTY, CLOSED_FLAG, LOT_STATUS_FLAG, SAMPLE_STATION, LINE, EDIT_EMP, EDIT_TIME, AQL_LEVEL) 
                    //VALUES( '{Table_R_Lot_Status.GetNewID(BU, DB)}', '{Lot}', '{SKU}', '0', '0', '0','0', '0', '0', '0', '0', '{StationName}', '{Line}','{Emp}', SYSDATE, '')";
                    //                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                    //                //DB.ExecuteCommand()
                    //                Res = Convert.ToInt32(DB.ExecSQL(strsql));
                    //                return Res;
                    string LotID = "";
                    LotID = Table_R_Lot_Status.GetNewID(BU, DB);
                    Row_R_Lot_Status.ID = LotID;
                    Row_R_Lot_Status.LOT_NO = Lot;
                    Row_R_Lot_Status.SKUNO = SKU;
                    Row_R_Lot_Status.LINE = Line;
                    Row_R_Lot_Status.STATION = StationName;
                    Row_R_Lot_Status.LOT_QTY = 0;
                    Row_R_Lot_Status.LOT_SIZE = LOT_SIZE;
                    Row_R_Lot_Status.LOTFULL_FLAG = "0";
                    Row_R_Lot_Status.LOTPASS_FLAG = "0";
                    Row_R_Lot_Status.SAMPLE_FLAG = "0";
                    Row_R_Lot_Status.REMARK = "";
                    Row_R_Lot_Status.EDIT_EMP = Emp;
                    Row_R_Lot_Status.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_Lot_Status.GetInsertString(DBType));
                }
                catch (Exception)
                {
                    string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { Lot });
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
        /// WZW 檢查該料該線體該工站號是否存在，且LOTFULL_FLAG=0
        /// </summary>
        /// <param name="SKU"></param>
        /// <param name="StationName"></param>
        /// <param name="Line"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_2DX_LOT_STATUS> GetBySkuStation(string SKU, string StationName, string Line, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            List<R_2DX_LOT_STATUS> row = new List<R_2DX_LOT_STATUS>();
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_2DX_LOT_STATUS WHERE SKUNO='{SKU}' AND STATION='{StationName}' AND LINE='{Line}' AND LOTFULL_FLAG = 0";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    Row_R_2DX_LOT_STATUS RowRLotStation = (Row_R_2DX_LOT_STATUS)NewRow();
                    RowRLotStation.loadData(dt.Rows[0]);
                    row.Add(RowRLotStation.GetDataObject());

                }
                return row;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// 檢查是否有LOT 超过半小时未测试的
        /// </summary>
        /// <param name="StationName"></param>
        /// <param name="Line"></param>
        /// <param name="lotsize"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool GetLotTimeOut(string StationName, string Line, OleExec DB)
        {
            bool SampleFlag = false;
            string StrSql = $@"select * from R_2DX_LOT_STATUS where lotfull_flag = '1' and station = '{StationName}'   
    and line = '{Line}'  
    and lot_qty = lot_size and lotpass_flag = 'N' and edit_time < to_char(edit_time-1/48,'yyyymmdd HH:mi:dd')";
            int TimeOut = DB.ExecSqlNoReturn(StrSql, null);
            if (TimeOut > 0)
            {
                SampleFlag = true;
            }
            return SampleFlag;
        }
        /// <summary>
        /// 把前一LOT 超过半小时未测试的抓出来报错
        /// </summary>
        /// <param name="StationName"></param>
        /// <param name="Line"></param>
        /// <param name="lotsize"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_2DX_LOT_STATUS GetByStationLine(string StationName, string Line, OleExec DB)
        {
            string StrSql = $@"select * from R_2DX_LOT_STATUS where lotfull_flag = '1' and station = '{StationName}'   
    and line = '{Line}'  
    and lot_qty = lot_size and lotpass_flag = '0' and edit_time < SYSDATE-1/48";
            DataSet ds = DB.ExecSelect(StrSql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_2DX_LOT_STATUS RowR2DXSTATUS = (Row_R_2DX_LOT_STATUS)NewRow();
                RowR2DXSTATUS.loadData(ds.Tables[0].Rows[0]);
                RowR2DXSTATUS.GetDataObject();
                return RowR2DXSTATUS.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// WZW 跟新LOT
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateLOT(string LotNo, string lotQty, string lasteditby, OleExec DB)
        {
            string StrSql = "";
            int res = 0;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                StrSql = $@"update R_2DX_LOT_STATUS set lot_Qty = '{lotQty}',LOTFULL_FLAG=CASE WHEN {lotQty}>=LOT_SIZE THEN '1' ELSE '0' END, edit_emp = '{lasteditby}', edit_time = SYSDATE   
      where LOT_NO = '{LotNo}'  ";
                res = DB.ExecSqlNoReturn(StrSql, null);
                return res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// WZW 跟新lotfull_FLAG
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateLOTFULL_FLAG(string SKU, string station, String lasteditby, string Line, OleExec DB)
        {
            string StrSql = "";
            int res = 0;
            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                StrSql = $@"UPDATE R_2DX_LOT_STATUS set lotfull_FLAG = 1, EDIT_TIME = SYSDATE where lotfull_FLAG = 0 and skuno != '{SKU}' and line='{Line}' and station='{station}'";
                res = DB.ExecSqlNoReturn(StrSql, null);
                return res;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }
        /// <summary>
        /// 通過LotNo獲取2DXLotStatus
        /// </summary>
        /// <param name="LotNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_2DX_LOT_STATUS Get2DXLotStatusByLotno(string LotNo, OleExec DB)
        {
            string StrSql = $@"select * from R_2DX_LOT_STATUS where lot_no = '{LotNo}'";
            DataSet ds = DB.ExecSelect(StrSql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_2DX_LOT_STATUS RowR2DXSTATUS = (Row_R_2DX_LOT_STATUS)NewRow();
                RowR2DXSTATUS.loadData(ds.Tables[0].Rows[0]);
                RowR2DXSTATUS.GetDataObject();
                return RowR2DXSTATUS.GetDataObject();
            }
            else
            {
                return null;
            }
        }




        //modify by LLF 2019-04-12 for ADD Station
        public void UpdateLotPassFlag(string ProductLine,string Station, OleExec DB)
        {
            if (IsLotPassFlagNeedUpdate(ProductLine, Station, DB))
            {
                string UpdateSql = $@"update R_2DX_LOT_STATUS set LOTPASS_FLAG=1 
                                        where LOTFULL_FLAG=1 and LOTPASS_FLAG=0
                                        and LINE=:productline and LOT_QTY=LOT_SIZE
                                        and exists
                                        (
                                        select 1 from R_2DX_LOT_DETAIL b
                                        where R_2DX_LOT_STATUS.ID= b.LOT_ID
                                        and b.STATION=R_2DX_LOT_STATUS.STATION
                                        and b.EDIT_TIME>sysdate-12
                                        and exists(
                                            select 1 from R_2DX c
                                            where c.SN=b.SN
                                            and c.STATUS='PASS'
                                            and c.station='{Station}' 
                                            and c.SCANDATE>b.EDIT_TIME
                                            and c.SCANDATE>sysdate-1
                                            )
                                        )";
                OleDbParameter[] paramet = new OleDbParameter[]
                {
                    new OleDbParameter(":productionline",ProductLine)
                };
                DB.ExecuteNonQuery(UpdateSql, CommandType.Text, paramet);
            }

        }
        /// <summary>
        /// 判斷產線是否有需要關閉的lot(AOI用)
        /// </summary>
        /// <param name="ProductLine"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsLotPassFlagNeedUpdate(string ProductLine, string Station, OleExec DB)
        {
            bool boo = false;
            string SelSql = $@"select count(*) from R_2DX_LOT_STATUS a ,R_2DX_LOT_DETAIL b ,R_2DX c 
                    where
                    a.LOTFULL_FLAG = 1
                    and a.LOT_QTY = a.LOT_SIZE
                    and a.ID = b.LOT_ID
                    and b.SN = c.SN
                    and a.LOTPASS_FLAG = 0
                    and a.LINE =:productionline
                     and a.STATION = b.STATION
                    and c.STATUS = 'PASS'
                    and c.station='{Station}'
                    and b.EDIT_TIME > SYSDATE - 12
                    and c.SCANDATE > b.EDIT_TIME";
            OleDbParameter[] paramet = new OleDbParameter[]
                {
                    new OleDbParameter(":productionline",ProductLine)
                };
            if (int.Parse(DB.ExecuteScalar(SelSql, CommandType.Text, paramet)) > 0)
            {
                boo = true;
            }

            return boo;
        }

        /// <summary>
        /// 獲取一個沒有滿的2DXLOT號(AOI用)
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="ProductLine"></param>
        /// <param name="Station"></param>
        /// <param name="LotSize"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string Get2DXLotNoNotFull(string Skuno, string ProductLine, string Station, int LotSize, OleExec DB)
        {
            string SqlStr = $@"
                select LOT_NO from R_2DX_LOT_STATUS 
                WHERE LOTFULL_FLAG=0 
                AND SKUNO='{Skuno}'        
                and LINE='{ProductLine}'
                AND LOT_QTY<'{LotSize}'
                ";

            //and SAMPLE_STATION = '{Station}'
            // string res = DB.ExecSqlReturn(SqlStr);

            DataSet result = DB.ExecSelect(SqlStr);
            string res = string.Empty;



            if (result.Tables[0].Rows.Count > 0)
            {
                res = result.Tables[0].Rows[0][0].ToString();
            }







            return res;
        }

        /// <summary>
        /// 獲取一個新的2DXLOT號(AOI用)
        /// </summary>
        /// <param name="Skuno"></param>
        /// <param name="ProductLine"></param>
        /// <param name="Station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetNew2DXLotNo(string Skuno, string ProductLine, string Station, OleExec DB)
        {
            //更新LOT狀態,關閉lot
            string UpdateSql = $@"
                    UPDATE R_2DX_LOT_STATUS set LOTFULL_FLAG = 1, EDIT_TIME = SYSDATE where LOTFULL_FLAG = 0 and SKUNO = '{Skuno}' and LINE='{ProductLine}' and STATION='{Station}'
                ";
            DB.ExecSqlNoReturn(UpdateSql, null);

            //獲取新的Lot號
            string NewLotNo = "LOT-" + DateTime.Now.ToString("yyMMdd");

            string QuerySql = $@" 
                                    select max(LOT_NO) from r_2DX_lot_status where substr(LOT_NO,0,10)='{NewLotNo}'
                            ";
            string MaxLotNo = DB.ExecSqlReturn(QuerySql).ToUpper().Trim();
            if (MaxLotNo.Equals(""))
            {
                NewLotNo += "00001";
            }
            else
            {
                NewLotNo += (int.Parse(MaxLotNo.Substring(10, 5)) + 1).ToString();
            }

            return NewLotNo;
        }

    }
    public class Row_R_2DX_LOT_STATUS : DataObjectBase
    {
        public Row_R_2DX_LOT_STATUS(DataObjectInfo info) : base(info)
        {

        }
        public R_2DX_LOT_STATUS GetDataObject()
        {
            R_2DX_LOT_STATUS DataObject = new R_2DX_LOT_STATUS();
            DataObject.ID = this.ID;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.LINE = this.LINE;
            DataObject.STATION = this.STATION;
            DataObject.LOT_QTY = this.LOT_QTY;
            DataObject.LOT_SIZE = this.LOT_SIZE;
            DataObject.LOTFULL_FLAG = this.LOTFULL_FLAG;
            DataObject.LOTPASS_FLAG = this.LOTPASS_FLAG;
            DataObject.SAMPLE_FLAG = this.SAMPLE_FLAG;
            DataObject.REMARK = this.REMARK;
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
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
        public double? LOT_SIZE
        {
            get
            {
                return (double?)this["LOT_SIZE"];
            }
            set
            {
                this["LOT_SIZE"] = value;
            }
        }
        public string LOTFULL_FLAG
        {
            get
            {
                return (string)this["LOTFULL_FLAG"];
            }
            set
            {
                this["LOTFULL_FLAG"] = value;
            }
        }
        public string LOTPASS_FLAG
        {
            get
            {
                return (string)this["LOTPASS_FLAG"];
            }
            set
            {
                this["LOTPASS_FLAG"] = value;
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
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
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
    public class R_2DX_LOT_STATUS
    {
        public string ID;
        public string LOT_NO;
        public string SKUNO;
        public string LINE;
        public string STATION;
        public double? LOT_QTY;
        public double? LOT_SIZE;
        public string LOTFULL_FLAG;
        public string LOTPASS_FLAG;
        public string SAMPLE_FLAG;
        public string REMARK;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}