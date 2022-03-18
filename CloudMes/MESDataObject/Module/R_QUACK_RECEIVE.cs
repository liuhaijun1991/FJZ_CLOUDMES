using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_RECEIVE : DataObjectTable
    {
        public T_R_QUACK_RECEIVE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_RECEIVE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_RECEIVE);
            TableName = "R_QUACK_RECEIVE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int IntR_QUACK_RECEIVE(R_QUACK_RECEIVE QuackReceive , OleExec DB)
        {
            return DB.ORM.Insertable<R_QUACK_RECEIVE>(QuackReceive).ExecuteCommand();
        }
    }
    public class Row_R_QUACK_RECEIVE : DataObjectBase
    {
        public Row_R_QUACK_RECEIVE(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_RECEIVE GetDataObject()
        {
            R_QUACK_RECEIVE DataObject = new R_QUACK_RECEIVE();
            DataObject.ID = this.ID;
            DataObject.ROLL_ID = this.ROLL_ID;
            DataObject.QTY = this.QTY;
            DataObject.START_QSN = this.START_QSN;
            DataObject.END_QSN = this.END_QSN;
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
        public string ROLL_ID
        {
            get
            {
                return (string)this["ROLL_ID"];
            }
            set
            {
                this["ROLL_ID"] = value;
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
        public string START_QSN
        {
            get
            {
                return (string)this["START_QSN"];
            }
            set
            {
                this["START_QSN"] = value;
            }
        }
        public string END_QSN
        {
            get
            {
                return (string)this["END_QSN"];
            }
            set
            {
                this["END_QSN"] = value;
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
    public class R_QUACK_RECEIVE
    {
        public string ID { get; set; }
        public string ROLL_ID { get; set; }
        public double? QTY { get; set; }
        public string START_QSN { get; set; }
        public string END_QSN { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}