using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;

namespace MESDataObject.Module
{
    public class T_R_MESNO : DataObjectTable
    {
        public T_R_MESNO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MESNO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MESNO);
            TableName = "R_MESNO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        /// <summary>
        /// WZW insert R_MESNO一筆記錄 
        /// </summary>
        /// <param name="_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void/*int*/ InRMESNO(string MESNONAME, string BU, string Lot, string IP, string StationName, string Line, string Emp, OleExec DB)
        {
            //string strsql = "";
            //int Res;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_MESNO Table_T_R_MESNO = new T_R_MESNO(DB, DBType);
                    Row_R_MESNO Row_R_MESNO = (Row_R_MESNO)NewRow();
                    //                strsql = $@"INSERT INTO R_LOT_STATUS (ID, LOT_NO, SKUNO, AQL_TYPE, LOT_QTY, REJECT_QTY, SAMPLE_QTY, PASS_QTY, FAIL_QTY, CLOSED_FLAG, LOT_STATUS_FLAG, SAMPLE_STATION, LINE, EDIT_EMP, EDIT_TIME, AQL_LEVEL) 
                    //VALUES( '{Table_R_Lot_Status.GetNewID(BU, DB)}', '{Lot}', '{SKU}', '0', '0', '0','0', '0', '0', '0', '0', '{StationName}', '{Line}','{Emp}', SYSDATE, '')";
                    //                string ID = DB.ExecSelectOneValue(strsql)?.ToString();
                    //                //DB.ExecuteCommand()
                    //                Res = Convert.ToInt32(DB.ExecSQL(strsql));
                    //                return Res;
                    string ID = "";
                    ID = Table_T_R_MESNO.GetNewID(BU, DB);
                    Row_R_MESNO.ID = ID;
                    Row_R_MESNO.MES_NO = Lot;
                    Row_R_MESNO.MES_NO_NAME = MESNONAME;
                    Row_R_MESNO.STATION = StationName;
                    Row_R_MESNO.LINE = Line;
                    Row_R_MESNO.IP = IP;
                    Row_R_MESNO.VALID_FLAG = "1";
                    Row_R_MESNO.EDIT_EMP = Emp;
                    Row_R_MESNO.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_MESNO.GetInsertString(DBType));
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
        /// WZW insert R_MESNO一筆記錄 
        /// </summary>
        /// <param name="_SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void UupdateRMESNO(string Lot, string IP, string Line, string Emp, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    T_R_MESNO Table_T_R_MESNO = new T_R_MESNO(DB, DBType);
                    Row_R_MESNO Row_R_MESNO = (Row_R_MESNO)NewRow();
                    Row_R_MESNO = (Row_R_MESNO)GetObjByLOT(Lot, DB);
                    Row_R_MESNO.MES_NO_NAME = Lot;
                    Row_R_MESNO.LINE = Line;
                    Row_R_MESNO.IP = IP;
                    Row_R_MESNO.VALID_FLAG = "0";
                    Row_R_MESNO.EDIT_EMP = Emp;
                    Row_R_MESNO.EDIT_TIME = GetDBDateTime(DB);
                    DB.ExecSQL(Row_R_MESNO.GetUpdateString(DBType));
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
        /// WZW GetObjByLOT R_MESNO
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public DataObjectBase GetObjByLOT(string ID, OleExec DB)
        {
            return GetObjByLOT(ID, DB, DBType);
        }

        public DataObjectBase GetObjByLOT(string ID, OleExec DB, DB_TYPE_ENUM DBType)
        {
            string strSql = $@"select * from {TableName} where MES_NO = '{ID}'";
            DataSet res = DB.ExecSelect(strSql);
            if (RowType == null)
            {
                DataObjectBase ret = NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                Assembly assembly = Assembly.Load("MESDataObject");
                object API_CLASS = assembly.CreateInstance(RowType.FullName, true, BindingFlags.CreateInstance, null, new object[] { DataInfo }, null, null);
                MethodInfo Function = RowType.GetMethod("loadData");
                Function.Invoke(API_CLASS, new object[] { res.Tables[0].Rows[0] });
                return (DataObjectBase)API_CLASS;
            }
        }
    }
    public class Row_R_MESNO : DataObjectBase
    {
        public Row_R_MESNO(DataObjectInfo info) : base(info)
        {

        }
        public R_MESNO GetDataObject()
        {
            R_MESNO DataObject = new R_MESNO();
            DataObject.ID = this.ID;
            DataObject.MES_NO = this.MES_NO;
            DataObject.MES_NO_NAME = this.MES_NO_NAME;
            DataObject.STATION = this.STATION;
            DataObject.LINE = this.LINE;
            DataObject.IP = this.IP;
            DataObject.VALID_FLAG = this.VALID_FLAG;
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
        public string MES_NO
        {
            get
            {
                return (string)this["MES_NO"];
            }
            set
            {
                this["MES_NO"] = value;
            }
        }
        public string MES_NO_NAME
        {
            get
            {
                return (string)this["MES_NO_NAME"];
            }
            set
            {
                this["MES_NO_NAME"] = value;
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
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
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
    public class R_MESNO
    {
        public string ID;
        public string MES_NO;
        public string MES_NO_NAME;
        public string STATION;
        public string LINE;
        public string IP;
        public string VALID_FLAG;
        public string EDIT_EMP;
        public DateTime? EDIT_TIME;
    }
}