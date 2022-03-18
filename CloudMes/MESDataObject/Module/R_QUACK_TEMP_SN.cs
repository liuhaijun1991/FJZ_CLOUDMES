using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_TEMP_SN : DataObjectTable
    {
        public T_R_QUACK_TEMP_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_TEMP_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_TEMP_SN);
            TableName = "R_QUACK_TEMP_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public void InR_QUACK_TEMP_SN(string BU, string QSN, string FACTORY_ID, string LOT_NO, string SKUNO, string SHIFT, string STARTED_FLAG, string REPAIR_FLAG, string COMPLETED_FLAG, string SHIPPED_FLAG, string CURRENT_STATION, string NEXT_STATION, string CONFIRM_FLAG, string SAP_FLAG, string LOCATION, string EDIT_EMP, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    Row_R_QUACK_TEMP_SN Row_R_QUACK_TEMP_SN = (Row_R_QUACK_TEMP_SN)NewRow();
                    string ID = "";
                    ID = GetNewID(BU, DB);
                    Row_R_QUACK_TEMP_SN.ID = ID;
                    Row_R_QUACK_TEMP_SN.QSN = QSN;
                    Row_R_QUACK_TEMP_SN.PLANT = FACTORY_ID;
                    Row_R_QUACK_TEMP_SN.LOT_NO = LOT_NO;
                    Row_R_QUACK_TEMP_SN.SKUNO = SKUNO;
                    Row_R_QUACK_TEMP_SN.CLASS_NAME = SHIFT;
                    Row_R_QUACK_TEMP_SN.STARTED_FLAG = STARTED_FLAG;
                    Row_R_QUACK_TEMP_SN.START_TIME = GetDBDateTime(DB);
                    Row_R_QUACK_TEMP_SN.REPAIR_FLAG = REPAIR_FLAG;
                    Row_R_QUACK_TEMP_SN.REPAIR_TIME = GetDBDateTime(DB);
                    Row_R_QUACK_TEMP_SN.COMPLETED_FLAG = COMPLETED_FLAG;
                    Row_R_QUACK_TEMP_SN.COMPLETED_TIME = GetDBDateTime(DB);
                    Row_R_QUACK_TEMP_SN.SHIPPED_FLAG = SHIPPED_FLAG;
                    Row_R_QUACK_TEMP_SN.SHIPDATE = GetDBDateTime(DB);
                    Row_R_QUACK_TEMP_SN.CURRENT_STATION = CURRENT_STATION;
                    Row_R_QUACK_TEMP_SN.NEXT_STATION = NEXT_STATION;
                    Row_R_QUACK_TEMP_SN.CONFIRM_FLAG = CONFIRM_FLAG;
                    Row_R_QUACK_TEMP_SN.SAP_FLAG = SAP_FLAG;
                    Row_R_QUACK_TEMP_SN.LOCATION = LOCATION;
                    Row_R_QUACK_TEMP_SN.EDIT_EMP = EDIT_EMP;
                    Row_R_QUACK_TEMP_SN.EDIT_TIME = GetDBDateTime(DB);
                    string n=DB.ExecSQL(Row_R_QUACK_TEMP_SN.GetInsertString(DBType));
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

        public List<R_QUACK_TEMP_SN> GetQuackMissSN(OleExec DB)
        {
            return DB.ORM.Queryable<R_QUACK_TEMP_SN>().ToList();
        }

    }
    public class Row_R_QUACK_TEMP_SN : DataObjectBase
    {
        public Row_R_QUACK_TEMP_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_TEMP_SN GetDataObject()
        {
            R_QUACK_TEMP_SN DataObject = new R_QUACK_TEMP_SN();
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
    public class R_QUACK_TEMP_SN
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