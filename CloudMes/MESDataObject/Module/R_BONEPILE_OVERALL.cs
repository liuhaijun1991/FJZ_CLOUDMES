using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BONEPILE_OVERALL : DataObjectTable
    {
        public T_R_BONEPILE_OVERALL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BONEPILE_OVERALL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BONEPILE_OVERALL);
            TableName = "R_BONEPILE_OVERALL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Insert(OleExec SFCDB, R_BONEPILE_OVERALL obj)
        {
            return SFCDB.ORM.Insertable<R_BONEPILE_OVERALL>(obj).ExecuteCommand();
        }

        public int Update(OleExec SFCDB, R_BONEPILE_OVERALL obj)
        {
            return SFCDB.ORM.Updateable<R_BONEPILE_OVERALL>(obj).Where(r => r.ID == obj.ID).ExecuteCommand();
        }
    }
    public class Row_R_BONEPILE_OVERALL : DataObjectBase
    {
        public Row_R_BONEPILE_OVERALL(DataObjectInfo info) : base(info)
        {

        }
        public R_BONEPILE_OVERALL GetDataObject()
        {
            R_BONEPILE_OVERALL DataObject = new R_BONEPILE_OVERALL();
            DataObject.LASTEDIT_DATE = this.LASTEDIT_DATE;
            DataObject.LASTEDIT_BY = this.LASTEDIT_BY;
            DataObject.OPEN_E_AMOUNT = this.OPEN_E_AMOUNT;
            DataObject.OPEN_D_AMOUNT = this.OPEN_D_AMOUNT;
            DataObject.OPEN_C_AMOUNT = this.OPEN_C_AMOUNT;
            DataObject.OPEN_B_AMOUNT = this.OPEN_B_AMOUNT;
            DataObject.OPEN_A_AMOUNT = this.OPEN_A_AMOUNT;
            DataObject.OPEN_E_QTY = this.OPEN_E_QTY;
            DataObject.OPEN_D_QTY = this.OPEN_D_QTY;
            DataObject.OPEN_C_QTY = this.OPEN_C_QTY;
            DataObject.OPEN_B_QTY = this.OPEN_B_QTY;
            DataObject.OPEN_A_QTY = this.OPEN_A_QTY;
            DataObject.CLOSED_E_QTY = this.CLOSED_E_QTY;
            DataObject.CLOSED_D_QTY = this.CLOSED_D_QTY;
            DataObject.CLOSED_C_QTY = this.CLOSED_C_QTY;
            DataObject.CLOSED_B_QTY = this.CLOSED_B_QTY;
            DataObject.CLOSED_A_QTY = this.CLOSED_A_QTY;
            DataObject.NEW_E_QTY = this.NEW_E_QTY;
            DataObject.NEW_D_QTY = this.NEW_D_QTY;
            DataObject.NEW_C_QTY = this.NEW_C_QTY;
            DataObject.NEW_B_QTY = this.NEW_B_QTY;
            DataObject.NEW_A_QTY = this.NEW_A_QTY;
            DataObject.OPEN_AMOUNT = this.OPEN_AMOUNT;
            DataObject.OPEN_QTY = this.OPEN_QTY;
            DataObject.CLOSED_QTY = this.CLOSED_QTY;
            DataObject.NEW_ADD_QTY = this.NEW_ADD_QTY;
            DataObject.FLH = this.FLH;
            DataObject.BRCD = this.BRCD;
            DataObject.DATA_CLASS = this.DATA_CLASS;
            DataObject.DATA_TYPE = this.DATA_TYPE;
            DataObject.WEEK_OR_MONTH = this.WEEK_OR_MONTH;
            DataObject.YEAR = this.YEAR;
            DataObject.TRAN_TYPE = this.TRAN_TYPE;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public DateTime? LASTEDIT_DATE
        {
            get
            {
                return (DateTime?)this["LASTEDIT_DATE"];
            }
            set
            {
                this["LASTEDIT_DATE"] = value;
            }
        }
        public string LASTEDIT_BY
        {
            get
            {
                return (string)this["LASTEDIT_BY"];
            }
            set
            {
                this["LASTEDIT_BY"] = value;
            }
        }
        public double? OPEN_E_AMOUNT
        {
            get
            {
                return (double?)this["OPEN_E_AMOUNT"];
            }
            set
            {
                this["OPEN_E_AMOUNT"] = value;
            }
        }
        public double? OPEN_D_AMOUNT
        {
            get
            {
                return (double?)this["OPEN_D_AMOUNT"];
            }
            set
            {
                this["OPEN_D_AMOUNT"] = value;
            }
        }
        public double? OPEN_C_AMOUNT
        {
            get
            {
                return (double?)this["OPEN_C_AMOUNT"];
            }
            set
            {
                this["OPEN_C_AMOUNT"] = value;
            }
        }
        public double? OPEN_B_AMOUNT
        {
            get
            {
                return (double?)this["OPEN_B_AMOUNT"];
            }
            set
            {
                this["OPEN_B_AMOUNT"] = value;
            }
        }
        public double? OPEN_A_AMOUNT
        {
            get
            {
                return (double?)this["OPEN_A_AMOUNT"];
            }
            set
            {
                this["OPEN_A_AMOUNT"] = value;
            }
        }
        public double? OPEN_E_QTY
        {
            get
            {
                return (double?)this["OPEN_E_QTY"];
            }
            set
            {
                this["OPEN_E_QTY"] = value;
            }
        }
        public double? OPEN_D_QTY
        {
            get
            {
                return (double?)this["OPEN_D_QTY"];
            }
            set
            {
                this["OPEN_D_QTY"] = value;
            }
        }
        public double? OPEN_C_QTY
        {
            get
            {
                return (double?)this["OPEN_C_QTY"];
            }
            set
            {
                this["OPEN_C_QTY"] = value;
            }
        }
        public double? OPEN_B_QTY
        {
            get
            {
                return (double?)this["OPEN_B_QTY"];
            }
            set
            {
                this["OPEN_B_QTY"] = value;
            }
        }
        public double? OPEN_A_QTY
        {
            get
            {
                return (double?)this["OPEN_A_QTY"];
            }
            set
            {
                this["OPEN_A_QTY"] = value;
            }
        }
        public double? CLOSED_E_QTY
        {
            get
            {
                return (double?)this["CLOSED_E_QTY"];
            }
            set
            {
                this["CLOSED_E_QTY"] = value;
            }
        }
        public double? CLOSED_D_QTY
        {
            get
            {
                return (double?)this["CLOSED_D_QTY"];
            }
            set
            {
                this["CLOSED_D_QTY"] = value;
            }
        }
        public double? CLOSED_C_QTY
        {
            get
            {
                return (double?)this["CLOSED_C_QTY"];
            }
            set
            {
                this["CLOSED_C_QTY"] = value;
            }
        }
        public double? CLOSED_B_QTY
        {
            get
            {
                return (double?)this["CLOSED_B_QTY"];
            }
            set
            {
                this["CLOSED_B_QTY"] = value;
            }
        }
        public double? CLOSED_A_QTY
        {
            get
            {
                return (double?)this["CLOSED_A_QTY"];
            }
            set
            {
                this["CLOSED_A_QTY"] = value;
            }
        }
        public double? NEW_E_QTY
        {
            get
            {
                return (double?)this["NEW_E_QTY"];
            }
            set
            {
                this["NEW_E_QTY"] = value;
            }
        }
        public double? NEW_D_QTY
        {
            get
            {
                return (double?)this["NEW_D_QTY"];
            }
            set
            {
                this["NEW_D_QTY"] = value;
            }
        }
        public double? NEW_C_QTY
        {
            get
            {
                return (double?)this["NEW_C_QTY"];
            }
            set
            {
                this["NEW_C_QTY"] = value;
            }
        }
        public double? NEW_B_QTY
        {
            get
            {
                return (double?)this["NEW_B_QTY"];
            }
            set
            {
                this["NEW_B_QTY"] = value;
            }
        }
        public double? NEW_A_QTY
        {
            get
            {
                return (double?)this["NEW_A_QTY"];
            }
            set
            {
                this["NEW_A_QTY"] = value;
            }
        }
        public double? OPEN_AMOUNT
        {
            get
            {
                return (double?)this["OPEN_AMOUNT"];
            }
            set
            {
                this["OPEN_AMOUNT"] = value;
            }
        }
        public double? OPEN_QTY
        {
            get
            {
                return (double?)this["OPEN_QTY"];
            }
            set
            {
                this["OPEN_QTY"] = value;
            }
        }
        public double? CLOSED_QTY
        {
            get
            {
                return (double?)this["CLOSED_QTY"];
            }
            set
            {
                this["CLOSED_QTY"] = value;
            }
        }
        public double? NEW_ADD_QTY
        {
            get
            {
                return (double?)this["NEW_ADD_QTY"];
            }
            set
            {
                this["NEW_ADD_QTY"] = value;
            }
        }
        public string FLH
        {
            get
            {
                return (string)this["FLH"];
            }
            set
            {
                this["FLH"] = value;
            }
        }
        public string BRCD
        {
            get
            {
                return (string)this["BRCD"];
            }
            set
            {
                this["BRCD"] = value;
            }
        }
        public string DATA_CLASS
        {
            get
            {
                return (string)this["DATA_CLASS"];
            }
            set
            {
                this["DATA_CLASS"] = value;
            }
        }
        public string DATA_TYPE
        {
            get
            {
                return (string)this["DATA_TYPE"];
            }
            set
            {
                this["DATA_TYPE"] = value;
            }
        }
        public string WEEK_OR_MONTH
        {
            get
            {
                return (string)this["WEEK_OR_MONTH"];
            }
            set
            {
                this["WEEK_OR_MONTH"] = value;
            }
        }
        public string YEAR
        {
            get
            {
                return (string)this["YEAR"];
            }
            set
            {
                this["YEAR"] = value;
            }
        }
        public string TRAN_TYPE
        {
            get
            {
                return (string)this["TRAN_TYPE"];
            }
            set
            {
                this["TRAN_TYPE"] = value;
            }
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
    }
    public class R_BONEPILE_OVERALL
    {
        public DateTime? LASTEDIT_DATE { get; set; }
        public string LASTEDIT_BY { get; set; }
        public double? OPEN_E_AMOUNT { get; set; }
        public double? OPEN_D_AMOUNT { get; set; }
        public double? OPEN_C_AMOUNT { get; set; }
        public double? OPEN_B_AMOUNT { get; set; }
        public double? OPEN_A_AMOUNT { get; set; }
        public double? OPEN_E_QTY { get; set; }
        public double? OPEN_D_QTY { get; set; }
        public double? OPEN_C_QTY { get; set; }
        public double? OPEN_B_QTY { get; set; }
        public double? OPEN_A_QTY { get; set; }
        public double? CLOSED_E_QTY { get; set; }
        public double? CLOSED_D_QTY { get; set; }
        public double? CLOSED_C_QTY { get; set; }
        public double? CLOSED_B_QTY { get; set; }
        public double? CLOSED_A_QTY { get; set; }
        public double? NEW_E_QTY { get; set; }
        public double? NEW_D_QTY { get; set; }
        public double? NEW_C_QTY { get; set; }
        public double? NEW_B_QTY { get; set; }
        public double? NEW_A_QTY { get; set; }
        public double? OPEN_AMOUNT { get; set; }
        public double? OPEN_QTY { get; set; }
        public double? CLOSED_QTY { get; set; }
        public double? NEW_ADD_QTY { get; set; }
        public string FLH { get; set; }
        public string BRCD { get; set; }
        public string DATA_CLASS { get; set; }
        public string DATA_TYPE { get; set; }
        public string WEEK_OR_MONTH { get; set; }
        public string YEAR { get; set; }
        public string TRAN_TYPE { get; set; }
        public string ID { get; set; }
    }
}