using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_QUACK_SN : DataObjectTable
    {
        public T_H_QUACK_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_QUACK_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_QUACK_SN);
            TableName = "H_QUACK_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// wzw insert into IH_QUACK_SN
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="QSN"></param>
        /// <param name="FACTORY_ID"></param>
        /// <param name="LOT_NO"></param>
        /// <param name="SKUNO"></param>
        /// <param name="SHIFT"></param>
        /// <param name="STARTED_FLAG"></param>
        /// <param name="REPAIR_FLAG"></param>
        /// <param name="COMPLETED_FLAG"></param>
        /// <param name="SHIPPED_FLAG"></param>
        /// <param name="CURRENT_STATION"></param>
        /// <param name="NEXT_STATION"></param>
        /// <param name="CONFIRM_FLAG"></param>
        /// <param name="SAP_FLAG"></param>
        /// <param name="LOCATION"></param>
        /// <param name="EDIT_EMP"></param>
        /// <param name="EDIT_TIME"></param>
        /// <param name="DB"></param>
        public void InH_QUACK_SN(string BU, string QSN, string FACTORY_ID, string LOT_NO, string SKUNO, string SHIFT, string STARTED_FLAG, string REPAIR_FLAG, string COMPLETED_FLAG, string SHIPPED_FLAG, string CURRENT_STATION, string NEXT_STATION, string CONFIRM_FLAG, string SAP_FLAG, string LOCATION,string C_Label_Type, string EDIT_EMP, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_H_QUACK_SN Table_H_QUACK_SN = new T_H_QUACK_SN(DB, DBType);
                    Row_H_QUACK_SN Row_H_QUACK_SN = (Row_H_QUACK_SN)NewRow();
                    string ID = "";
                    ID = Table_H_QUACK_SN.GetNewID(BU, DB);
                    Row_H_QUACK_SN.ID = ID;
                    Row_H_QUACK_SN.QSN = QSN;
                    Row_H_QUACK_SN.PLANT = FACTORY_ID;
                    Row_H_QUACK_SN.LOT_NO = LOT_NO;
                    Row_H_QUACK_SN.SKUNO = SKUNO;
                    Row_H_QUACK_SN.CLASS_NAME = SHIFT;
                    Row_H_QUACK_SN.STARTED_FLAG = STARTED_FLAG;
                    Row_H_QUACK_SN.START_TIME = GetDBDateTime(DB);
                    Row_H_QUACK_SN.REPAIR_FLAG = REPAIR_FLAG;
                    Row_H_QUACK_SN.REPAIR_TIME = GetDBDateTime(DB);
                    Row_H_QUACK_SN.COMPLETED_FLAG = COMPLETED_FLAG;
                    Row_H_QUACK_SN.COMPLETED_TIME = GetDBDateTime(DB);
                    Row_H_QUACK_SN.SHIPPED_FLAG = SHIPPED_FLAG;
                    Row_H_QUACK_SN.SHIPDATE = GetDBDateTime(DB);
                    Row_H_QUACK_SN.CURRENT_STATION = CURRENT_STATION;
                    Row_H_QUACK_SN.NEXT_STATION = NEXT_STATION;
                    Row_H_QUACK_SN.CONFIRM_FLAG = CONFIRM_FLAG;
                    Row_H_QUACK_SN.SAP_FLAG = SAP_FLAG;
                    Row_H_QUACK_SN.LOCATION = LOCATION;
                    Row_H_QUACK_SN.LABEL_TYPE = C_Label_Type;
                    Row_H_QUACK_SN.EDIT_EMP = EDIT_EMP;
                    Row_H_QUACK_SN.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_H_QUACK_SN.GetInsertString(DBType));
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
        /// <param name="Lot"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<H_QUACK_SN> GetLot(string Lot, OleExec DB)
        {
            string strsql = $@"SELECT * FROM H_QUACK_SN WHERE LOT_NO='{Lot}'";
            DataTable dt = DB.ExecSelect(strsql, null).Tables[0];
            Row_H_QUACK_SN RowHQuackSN = (Row_H_QUACK_SN)NewRow();
            List<H_QUACK_SN> ListHQuackSN = new List<H_QUACK_SN>();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow item in dt.Rows)
                {
                    RowHQuackSN.loadData(item);
                    ListHQuackSN.Add(RowHQuackSN.GetDataObject());
                }
            }
            return ListHQuackSN;
        }
        /// <summary>
        /// wzw 將Oracle數據庫的內容插入SQLServer數據庫
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertSQLServer(string qserialno, string factoryid, string lotno, string skuno, string shift, string started, string startdate, string repairheld, string repairdate, string completed, string completedate, string shipped, string shipdate, string currentevent, string nextevent, string confirmflag, string location, string lasteditby, string lasteditdt, string sapflag, OleExec DB)
        {
            string strsql = $@"insert H_QUACK_SN into qsysproduct values( '{qserialno}','{factoryid}','{lotno}','{skuno}','{shift}','{started}','{startdate}','{repairheld}','{repairdate}','{completed}','{completedate}','{shipped}','{shipdate}','{currentevent}','{nextevent}','{confirmflag}','{location}','{lasteditby}','{lasteditdt}','{sapflag}')";
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
        /// wzw 將R表數據插入到H表中
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public int InsertRInToHTable(string ID, string QSN, string FACTORY_ID, string LOT_NO, string SKUNO, string SHIFT, string STARTED_FLAG, DateTime? START_TIME, string REPAIR_FLAG, DateTime? REPAIR_TIME, string COMPLETED_FLAG, DateTime? COMPLETED_TIME, string SHIPPED_FLAG, DateTime? SHIPDATE, string CURRENT_STATION, string NEXT_STATION, string CONFIRM_FLAG, string SAP_FLAG, string LOCATION, string EDIT_EMP, DateTime? EDIT_TIME, OleExec DB)
        public int InsertRInToHTable(OleExec DB)
        {
            int Num = 0;
            //string strsql = $@"INSERT INTO VALUES ('{ID}', '{QSN}', '{FACTORY_ID}', '{LOT_NO}', '{SKUNO}', '{SHIFT}', '{STARTED_FLAG}', '{START_TIME}', '{REPAIR_FLAG}', '{REPAIR_TIME}', '{COMPLETED_FLAG}', '{COMPLETED_TIME}', '{SHIPPED_FLAG}', '{SHIPDATE}', '{CURRENT_STATION}', '{NEXT_STATION}', '{CONFIRM_FLAG}', '{SAP_FLAG}', '{LOCATION}', '{EDIT_EMP}', '{EDIT_TIME}')";
            string strsql = $@"INSERT INTO H_QUACK_SN SELECT * FROM R_QUACK_SN";
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
        /// <summary>
        /// WZW
        /// </summary>
        /// <param name="Lot"></param>
        /// <param name="BU"></param>
        /// <param name="Emp"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool UpadateLot(string Lot, string BU, string Emp, OleExec DB)
        {
            bool res = false;
            string strsql = $@"UPDATE H_QUACK_SN SET CURRENT_STATION='{Emp}',NEXT_STATION='{BU}',EDIT_TIME=SYSDATE WHERE LOT_NO='{Lot}'";
            int Num = DB.ExecSqlNoReturn(strsql, null);
            if (Num > 0)
            {
                res = true;
            }
            return res;
        }
    }
    public class Row_H_QUACK_SN : DataObjectBase
    {
        public Row_H_QUACK_SN(DataObjectInfo info) : base(info)
        {

        }
        public H_QUACK_SN GetDataObject()
        {
            H_QUACK_SN DataObject = new H_QUACK_SN();
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
    public class H_QUACK_SN
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