using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_BONEPILE_BASIC2 : DataObjectTable
    {
        public T_R_BONEPILE_BASIC2(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_BONEPILE_BASIC2(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_BONEPILE_BASIC2);
            TableName = "R_BONEPILE_BASIC2".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int Insert(OleExec SFCDB, R_BONEPILE_BASIC2 obj)
        {
            return SFCDB.ORM.Insertable<R_BONEPILE_BASIC2>(obj).ExecuteCommand();
        }

        public int Update(OleExec SFCDB, R_BONEPILE_BASIC2 obj)
        {
            return SFCDB.ORM.Updateable<R_BONEPILE_BASIC2>(obj).Where(r => r.ID == obj.ID).ExecuteCommand();
        }
    }
    public class Row_R_BONEPILE_BASIC2 : DataObjectBase
    {
        public Row_R_BONEPILE_BASIC2(DataObjectInfo info) : base(info)
        {

        }
        public R_BONEPILE_BASIC2 GetDataObject()
        {
            R_BONEPILE_BASIC2 DataObject = new R_BONEPILE_BASIC2();
            DataObject.LASTEDIT_DATE = this.LASTEDIT_DATE;
            DataObject.LASTEDIT_BY = this.LASTEDIT_BY;
            DataObject.E_AMOUNT = this.E_AMOUNT;
            DataObject.E_QTY = this.E_QTY;
            DataObject.D_AMOUNT = this.D_AMOUNT;
            DataObject.D_QTY = this.D_QTY;
            DataObject.C_AMOUNT = this.C_AMOUNT;
            DataObject.C_QTY = this.C_QTY;
            DataObject.B_AMOUNT = this.B_AMOUNT;
            DataObject.B_QTY = this.B_QTY;
            DataObject.A_AMOUNT = this.A_AMOUNT;
            DataObject.A_QTY = this.A_QTY;
            DataObject.BONEPILE_DESCRIPTION = this.BONEPILE_DESCRIPTION;
            DataObject.PRODUCT_NAME = this.PRODUCT_NAME;
            DataObject.SUB_SERIES = this.SUB_SERIES;
            DataObject.SERIES = this.SERIES;
            DataObject.HARDCORE = this.HARDCORE;
            DataObject.STATUS = this.STATUS;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.RMA = this.RMA;
            DataObject.NORMAL = this.NORMAL;
            DataObject.FLH = this.FLH;
            DataObject.BRCD = this.BRCD;
            DataObject.DATA_CLASS = this.DATA_CLASS;
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
        public double? E_AMOUNT
        {
            get
            {
                return (double?)this["E_AMOUNT"];
            }
            set
            {
                this["E_AMOUNT"] = value;
            }
        }
        public double? E_QTY
        {
            get
            {
                return (double?)this["E_QTY"];
            }
            set
            {
                this["E_QTY"] = value;
            }
        }
        public double? D_AMOUNT
        {
            get
            {
                return (double?)this["D_AMOUNT"];
            }
            set
            {
                this["D_AMOUNT"] = value;
            }
        }
        public double? D_QTY
        {
            get
            {
                return (double?)this["D_QTY"];
            }
            set
            {
                this["D_QTY"] = value;
            }
        }
        public double? C_AMOUNT
        {
            get
            {
                return (double?)this["C_AMOUNT"];
            }
            set
            {
                this["C_AMOUNT"] = value;
            }
        }
        public double? C_QTY
        {
            get
            {
                return (double?)this["C_QTY"];
            }
            set
            {
                this["C_QTY"] = value;
            }
        }
        public double? B_AMOUNT
        {
            get
            {
                return (double?)this["B_AMOUNT"];
            }
            set
            {
                this["B_AMOUNT"] = value;
            }
        }
        public double? B_QTY
        {
            get
            {
                return (double?)this["B_QTY"];
            }
            set
            {
                this["B_QTY"] = value;
            }
        }
        public double? A_AMOUNT
        {
            get
            {
                return (double?)this["A_AMOUNT"];
            }
            set
            {
                this["A_AMOUNT"] = value;
            }
        }
        public double? A_QTY
        {
            get
            {
                return (double?)this["A_QTY"];
            }
            set
            {
                this["A_QTY"] = value;
            }
        }
        public string BONEPILE_DESCRIPTION
        {
            get
            {
                return (string)this["BONEPILE_DESCRIPTION"];
            }
            set
            {
                this["BONEPILE_DESCRIPTION"] = value;
            }
        }
        public string PRODUCT_NAME
        {
            get
            {
                return (string)this["PRODUCT_NAME"];
            }
            set
            {
                this["PRODUCT_NAME"] = value;
            }
        }
        public string SUB_SERIES
        {
            get
            {
                return (string)this["SUB_SERIES"];
            }
            set
            {
                this["SUB_SERIES"] = value;
            }
        }
        public string SERIES
        {
            get
            {
                return (string)this["SERIES"];
            }
            set
            {
                this["SERIES"] = value;
            }
        }
        public string HARDCORE
        {
            get
            {
                return (string)this["HARDCORE"];
            }
            set
            {
                this["HARDCORE"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string RMA
        {
            get
            {
                return (string)this["RMA"];
            }
            set
            {
                this["RMA"] = value;
            }
        }
        public string NORMAL
        {
            get
            {
                return (string)this["NORMAL"];
            }
            set
            {
                this["NORMAL"] = value;
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
    public class R_BONEPILE_BASIC2
    {
        public DateTime? LASTEDIT_DATE { get; set; }
        public string LASTEDIT_BY { get; set; }
        public double? E_AMOUNT { get; set; }
        public double? E_QTY { get; set; }
        public double? D_AMOUNT { get; set; }
        public double? D_QTY { get; set; }
        public double? C_AMOUNT { get; set; }
        public double? C_QTY { get; set; }
        public double? B_AMOUNT { get; set; }
        public double? B_QTY { get; set; }
        public double? A_AMOUNT { get; set; }
        public double? A_QTY { get; set; }
        public string BONEPILE_DESCRIPTION { get; set; }
        public string PRODUCT_NAME { get; set; }
        public string SUB_SERIES { get; set; }
        public string SERIES { get; set; }
        public string HARDCORE { get; set; }
        public string STATUS { get; set; }
        public string CATEGORY { get; set; }
        public string RMA { get; set; }
        public string NORMAL { get; set; }
        public string FLH { get; set; }
        public string BRCD { get; set; }
        public string DATA_CLASS { get; set; }
        public string WEEK_OR_MONTH { get; set; }
        public string YEAR { get; set; }
        public string TRAN_TYPE { get; set; }
        public string ID { get; set; }
    }
}