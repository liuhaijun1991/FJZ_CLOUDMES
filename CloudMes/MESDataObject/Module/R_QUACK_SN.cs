using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_SN : DataObjectTable
    {
        public T_R_QUACK_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_SN);
            TableName = "R_QUACK_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW 檢查quackSN是否存在
        /// </summary>
        /// <param name="QuackSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckQuackSNExist(string QuackSN, OleExec DB)
        {
            bool res = false;
            DataTable dt = new DataTable();
            string sql = $@"SELECT * FROM R_QUACK_SN WHERE QSN='{QuackSN}'";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }
        /// <summary>
        /// WZW 查詢quackSN的信息
        /// </summary>
        /// <param name="QuackSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_QUACK_SN RowQuackSN(string QuackSN, OleExec DB)
        {
            string strSql = $@"SELECT * FROM R_QUACK_SN WHERE QSN='{QuackSN}'";
            DataSet res = DB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_QUACK_SN A = (Row_R_QUACK_SN)NewRow();
                A.loadData(res.Tables[0].Rows[0]);
                return A;
            }
            return null;
        }
        /// <summary>
        /// WZW 
        /// </summary>
        /// <param name="QuackSN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public Row_R_QUACK_SN GetLotNo(string LOTNO, OleExec DB)
        {
            string strSql = $@"SELECT * FROM R_QUACK_SN WHERE LOT_NO='{LOTNO}'";
            DataSet res = DB.RunSelect(strSql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_QUACK_SN A = (Row_R_QUACK_SN)NewRow();
                A.loadData(res.Tables[0].Rows[0]);
                return A;
            }
            return null;
        }
        /// <summary>
        /// wzw into QUACK_SN SQL 數據庫
        /// </summary>
        public void InR_QUACK_SN(string BU, string QSN, string FACTORY_ID, string LOT_NO, string SKUNO, string SHIFT, string STARTED_FLAG, string REPAIR_FLAG, string COMPLETED_FLAG, string SHIPPED_FLAG, string CURRENT_STATION, string NEXT_STATION, string CONFIRM_FLAG, string SAP_FLAG, string LOCATION, string EDIT_EMP, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_QUACK_SN Table_R_QUACK_SN = new T_R_QUACK_SN(DB, DBType);
                    Row_R_QUACK_SN Row_R_QUACK_SN = (Row_R_QUACK_SN)NewRow();
                    string ID = "";
                    ID = Table_R_QUACK_SN.GetNewID(BU, DB);
                    Row_R_QUACK_SN.ID = ID;
                    Row_R_QUACK_SN.QSN = QSN;
                    Row_R_QUACK_SN.PLANT = FACTORY_ID;
                    Row_R_QUACK_SN.LOT_NO = LOT_NO;
                    Row_R_QUACK_SN.SKUNO = SKUNO;
                    Row_R_QUACK_SN.CLASS_NAME = SHIFT;
                    Row_R_QUACK_SN.STARTED_FLAG = STARTED_FLAG;
                    Row_R_QUACK_SN.START_TIME = GetDBDateTime(DB);
                    Row_R_QUACK_SN.REPAIR_FLAG = REPAIR_FLAG;
                    Row_R_QUACK_SN.REPAIR_TIME = GetDBDateTime(DB);
                    Row_R_QUACK_SN.COMPLETED_FLAG = COMPLETED_FLAG;
                    Row_R_QUACK_SN.COMPLETED_TIME = GetDBDateTime(DB);
                    Row_R_QUACK_SN.SHIPPED_FLAG = SHIPPED_FLAG;
                    Row_R_QUACK_SN.SHIPDATE = GetDBDateTime(DB);
                    Row_R_QUACK_SN.CURRENT_STATION = CURRENT_STATION;
                    Row_R_QUACK_SN.NEXT_STATION = NEXT_STATION;
                    Row_R_QUACK_SN.CONFIRM_FLAG = CONFIRM_FLAG;
                    Row_R_QUACK_SN.SAP_FLAG = SAP_FLAG;
                    Row_R_QUACK_SN.LOCATION = LOCATION;
                    Row_R_QUACK_SN.EDIT_EMP = EDIT_EMP;
                    Row_R_QUACK_SN.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_QUACK_SN.GetInsertString(DBType));
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
        /// wzw
        /// </summary>
        /// <param name="CURRENTSTATION"></param>
        /// <param name="NEXTSTATION"></param>
        /// <param name="CONFIRMFLAG"></param>
        /// <param name="Emp"></param>
        /// <param name="WhereLOTNO"></param>
        /// <param name="WhereCURRENTSTATION"></param>
        /// <param name="WhereCONFIRMFLAG"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateCURRENTNEXTSTATIONCONFIRMEMPTIME(string CURRENTSTATION, string NEXTSTATION, string CONFIRMFLAG, string Emp, string WhereLOTNO, string WhereCURRENTSTATION, string WhereCONFIRMFLAG, OleExec DB)
        {
            string strsql = $@"UPDATE R_QUACK_SN SET CURRENT_STATION='{CURRENTSTATION}',NEXT_STATION='{NEXTSTATION}',CONFIRM_FLAG='{CONFIRMFLAG}',EDIT_EMP='{Emp}',EDIT_TIME=SYSDATE 
WHERE LOT_NO='{WhereLOTNO}' AND NEXT_STATION='{WhereCURRENTSTATION}' AND CONFIRM_FLAG='{WhereCONFIRMFLAG}'";
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
        /// wzw 將Oracle數據庫的內容插入SQLServer數據庫
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertSQLServer(string qserialno, string factoryid, string lotno, string skuno, string shift, string started, DateTime? startdate, string repairheld, DateTime? repairdate,
             string completed, DateTime? completedate, string shipped, DateTime? shipdate, string currentevent, string nextevent, string confirmflag, string location, string lasteditby,
             DateTime? lasteditdt, string sapflag, OleExec DB)
        {
            string strsql = $@"insert into qsysproduct values( '{qserialno}','{factoryid}','{lotno}','{skuno}','{shift}','{started}','{startdate}','{repairheld}','{repairdate}','{completed}','{completedate}','{shipped}','{shipdate}','{currentevent}','{nextevent}','{confirmflag}','{location}','{lasteditby}','{lasteditdt}','{sapflag}')";
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
        public List<R_QUACK_SN> GetLot(string Lot, OleExec DB)
        {
            string strsql = $@"SELECT * FROM R_QUACK_SN WHERE LOT_NO='{Lot}'";
            DataTable dt = DB.ExecSelect(strsql, null).Tables[0];
            Row_R_QUACK_SN RowRQUACKSN = (Row_R_QUACK_SN)NewRow();
            List<R_QUACK_SN> ListRQUACKSN = new List<R_QUACK_SN>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    RowRQUACKSN.loadData(item);
                    ListRQUACKSN.Add(RowRQUACKSN.GetDataObject());
                }
            }
            return ListRQUACKSN;
        }
        /// <summary>
        /// wzw 
        /// </summary>
        /// <param name="Lot"></param>
        /// <returns></returns>
        public bool DeleteLot(string Lot, OleExec DB)
        {
            bool res = false;
            string strsql = $@"DELETE FROM R_QUACK_SN WHERE LOT_NO='{Lot}'";
            int Num = DB.ExecSqlNoReturn(strsql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }

        public int UpdateRowBySn(R_QUACK_SN RQuackSn, string Qsn, OleExec db)
        {
            return db.ORM.Updateable(RQuackSn).Where(t => t.QSN == Qsn).ExecuteCommand();
        }

        public R_QUACK_SN GetRowByQsn(string Qsn, OleExec db)
        {
            return db.ORM.Queryable<R_QUACK_SN>().Where(t => t.QSN == Qsn).ToList().FirstOrDefault();
        }

        /// <summary>
        /// WZW /*字符转化为ASCII*/
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public int Asc(string character)
        {
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                throw new Exception("Character is not valid.");
            }
        }
        /// <summary>
        /// WZW /*字符转化为ASCII*/
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public string AscArray(string character)
        {
            if (character.Length >= 1)
            {
                byte[] array = System.Text.Encoding.ASCII.GetBytes(character);  //数组array为对应的ASCII数组     
                string ASCIIstr2 = null;
                for (int i = 0; i < array.Length; i++)
                {
                    int asciicode = (int)(array[i]);
                    ASCIIstr2 += Convert.ToString(asciicode);//字符串ASCIIstr2 为对应的ASCII字符串
                }
                return ASCIIstr2;
            }
            else
            {
                throw new Exception("Character is not valid.");
            }
        }
        /// <summary>
        /// WZW /*ASCII 转化为 字符*/
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public string Chr(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
                //return Encoding.Default.GetBytes(xmlStr);//字符串转ASCII
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }
        /// <summary>
        /// WZW /*ASCII 转化为 字符*/
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string Ascii2Str(byte[] buf)
        {
            return System.Text.Encoding.ASCII.GetString(buf);
        }
        /// <summary>
        /// WZW 根据LOt号查询 order by 排序
        /// 新方法 Lambda 写法 LINQ 语法 以下三个
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_QUACK_SN> QuackQueryByLot(string Lot, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.LOT_NO == Lot).OrderBy(x => x.QSN).ToList()/*.FirstOrDefault()*/; ;
        }
        public List<R_QUACK_SN> QuackQueryByQSNQueryQSN(string QSN, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.QSN == QSN).OrderBy(x => x.QSN).Take(1).ToList();
        }
        public List<R_QUACK_SN> QuackQueryByQSNANDCONFIRM_FLAG(string QSN, string CONFIRM_FLAG, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.QSN == QSN && x.CONFIRM_FLAG == CONFIRM_FLAG).OrderBy(x => x.QSN)./*Take(1).*/ToList();
        }
        public List<R_QUACK_SN> QuackQueryByQuackStartEndSN(string Lot, string CURRENT_STATION, string NEXT_STATION, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.LOT_NO == Lot && x.CURRENT_STATION == CURRENT_STATION && x.NEXT_STATION == NEXT_STATION).OrderBy(x => x.QSN).ToList();
        }
        public List<R_QUACK_SN> QuackQueryByCurrenteventNextevent(string Currentevent, string Nextevent, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.CURRENT_STATION == Currentevent && x.NEXT_STATION == Nextevent).OrderBy(x => x.QSN).ToList();
        }

        public R_QUACK_SN GetQuackSNByNextStation(string QSN, string NextStation, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.QSN == QSN && x.NEXT_STATION == NextStation).OrderBy(x => x.QSN).ToList().FirstOrDefault();
        }

        public List<R_QUACK_SN> GetQuackSNByLotNextStation(string LotNo, string NextStation, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.LOT_NO == LotNo && x.NEXT_STATION == NextStation).OrderBy(x => x.QSN).ToList();
        }

        public R_QUACK_SN GetQuackSNByCurrentNextStation(string SecondEmpNo, string NextStation, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(t => t.CURRENT_STATION == SecondEmpNo && t.NEXT_STATION == NextStation).OrderBy(t => t.QSN).ToList().FirstOrDefault();
        }

        public R_QUACK_SN GetQuackSNCompleteFlag(string QuackSN, string Completed_Flag, OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_SN>().Where(x => x.QSN == QuackSN && x.COMPLETED_FLAG == Completed_Flag).ToList().FirstOrDefault();
        }
    }
    public class Row_R_QUACK_SN : DataObjectBase
    {
        public Row_R_QUACK_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_SN GetDataObject()
        {
            R_QUACK_SN DataObject = new R_QUACK_SN();
            DataObject.ID = this.ID;
            DataObject.QSN = this.QSN;
            DataObject.PLANT = this.PLANT;
            DataObject.LOT_NO = this.LOT_NO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CLASS_NAME = this.CLASS_NAME;
            DataObject.STARTED_FLAG = this.STARTED_FLAG;
            DataObject.START_TIME = this.START_TIME;
            DataObject.REPAIR_FLAG = this.REPAIR_FLAG;
            DataObject.REPAIR_TIME = this.REPAIR_TIME;
            DataObject.COMPLETED_FLAG = this.COMPLETED_FLAG;
            DataObject.COMPLETED_TIME = this.COMPLETED_TIME;
            DataObject.SHIPPED_FLAG = this.SHIPPED_FLAG;
            DataObject.SHIPDATE = this.SHIPDATE;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.NEXT_STATION = this.NEXT_STATION;
            DataObject.CONFIRM_FLAG = this.CONFIRM_FLAG;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.LOCATION = this.LOCATION;
            DataObject.LABEL_TYPE = this.LABEL_TYPE;
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
        public string QSN
        {
            get
            {
                return (string)this["QSN"];
            }
            set
            {
                this["QSN"] = value;
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
        public string CLASS_NAME
        {
            get
            {
                return (string)this["CLASS_NAME"];
            }
            set
            {
                this["CLASS_NAME"] = value;
            }
        }
        public string STARTED_FLAG
        {
            get
            {
                return (string)this["STARTED_FLAG"];
            }
            set
            {
                this["STARTED_FLAG"] = value;
            }
        }
        public DateTime? START_TIME
        {
            get
            {
                return (DateTime?)this["START_TIME"];
            }
            set
            {
                this["START_TIME"] = value;
            }
        }
        public string REPAIR_FLAG
        {
            get
            {
                return (string)this["REPAIR_FLAG"];
            }
            set
            {
                this["REPAIR_FLAG"] = value;
            }
        }
        public DateTime? REPAIR_TIME
        {
            get
            {
                return (DateTime?)this["REPAIR_TIME"];
            }
            set
            {
                this["REPAIR_TIME"] = value;
            }
        }
        public string COMPLETED_FLAG
        {
            get
            {
                return (string)this["COMPLETED_FLAG"];
            }
            set
            {
                this["COMPLETED_FLAG"] = value;
            }
        }
        public DateTime? COMPLETED_TIME
        {
            get
            {
                return (DateTime?)this["COMPLETED_TIME"];
            }
            set
            {
                this["COMPLETED_TIME"] = value;
            }
        }
        public string SHIPPED_FLAG
        {
            get
            {
                return (string)this["SHIPPED_FLAG"];
            }
            set
            {
                this["SHIPPED_FLAG"] = value;
            }
        }
        public DateTime? SHIPDATE
        {
            get
            {
                return (DateTime?)this["SHIPDATE"];
            }
            set
            {
                this["SHIPDATE"] = value;
            }
        }
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string NEXT_STATION
        {
            get
            {
                return (string)this["NEXT_STATION"];
            }
            set
            {
                this["NEXT_STATION"] = value;
            }
        }
        public string CONFIRM_FLAG
        {
            get
            {
                return (string)this["CONFIRM_FLAG"];
            }
            set
            {
                this["CONFIRM_FLAG"] = value;
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
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
            }
        }
        public string LABEL_TYPE
        {
            get
            {
                return (string)this["LABEL_TYPE"];
            }
            set
            {
                this["LABEL_TYPE"] = value;
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

    public class R_QUACK_SN
    {
        public string ID { get; set; }
        public string QSN { get; set; }
        public string PLANT { get; set; }
        public string LOT_NO { get; set; }
        public string SKUNO { get; set; }
        public string CLASS_NAME { get; set; }
        public string STARTED_FLAG { get; set; }
        public DateTime? START_TIME { get; set; }
        public string REPAIR_FLAG { get; set; }
        public DateTime? REPAIR_TIME { get; set; }
        public string COMPLETED_FLAG { get; set; }
        public DateTime? COMPLETED_TIME { get; set; }
        public string SHIPPED_FLAG { get; set; }
        public DateTime? SHIPDATE { get; set; }
        public string CURRENT_STATION { get; set; }
        public string NEXT_STATION { get; set; }
        public string CONFIRM_FLAG { get; set; }
        public string SAP_FLAG { get; set; }
        public string LOCATION { get; set; }
        public string LABEL_TYPE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}