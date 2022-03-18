using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_r_2d_sn_relation : DataObjectTable
    {
        public T_r_2d_sn_relation(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_r_2d_sn_relation(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_r_2d_sn_relation);
            TableName = "r_2d_sn_relation".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int Insert(r_2d_sn_relation r_2d_sn_relation, OleExec DB)
        {
            return DB.ORM.Insertable<r_2d_sn_relation>(r_2d_sn_relation).ExecuteCommand();
        }
    }
    public class Row_r_2d_sn_relation : DataObjectBase
    {
        public Row_r_2d_sn_relation(DataObjectInfo info) : base(info)
        {

        }
        public r_2d_sn_relation GetDataObject()
        {
            r_2d_sn_relation DataObject = new r_2d_sn_relation();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.BARCODE_2D = this.BARCODE_2D;
            DataObject.ITEM_NUMBER = this.ITEM_NUMBER;
            DataObject.ITEM_VER = this.ITEM_VER;
            DataObject.UPDATED_DATE = this.UPDATED_DATE;
            DataObject.UPDATED_BY = this.UPDATED_BY;
            DataObject.CREATED_DATE = this.CREATED_DATE;
            DataObject.CREATED_BY = this.CREATED_BY;
            DataObject.UPLOAD_FLAG = this.UPLOAD_FLAG;
            DataObject.WO = this.WO;
            DataObject.LABEL_TYPE = this.LABEL_TYPE;
            DataObject.DATA3 = this.DATA3;
            DataObject.DESCRIPT = this.DESCRIPT;
            DataObject.ROHS_FLAG = this.ROHS_FLAG;
            DataObject.CHECK_MESSAGE = this.CHECK_MESSAGE;
            DataObject.TRANS_ID = this.TRANS_ID;
            DataObject.SN_TYPE = this.SN_TYPE;
            DataObject.PO = this.PO;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string BARCODE_2D
        {
            get
            {
                return (string)this["BARCODE_2D"];
            }
            set
            {
                this["BARCODE_2D"] = value;
            }
        }
        public string ITEM_NUMBER
        {
            get
            {
                return (string)this["ITEM_NUMBER"];
            }
            set
            {
                this["ITEM_NUMBER"] = value;
            }
        }
        public string ITEM_VER
        {
            get
            {
                return (string)this["ITEM_VER"];
            }
            set
            {
                this["ITEM_VER"] = value;
            }
        }
        public DateTime? UPDATED_DATE
        {
            get
            {
                return (DateTime?)this["UPDATED_DATE"];
            }
            set
            {
                this["UPDATED_DATE"] = value;
            }
        }
        public string UPDATED_BY
        {
            get
            {
                return (string)this["UPDATED_BY"];
            }
            set
            {
                this["UPDATED_BY"] = value;
            }
        }
        public DateTime? CREATED_DATE
        {
            get
            {
                return (DateTime?)this["CREATED_DATE"];
            }
            set
            {
                this["CREATED_DATE"] = value;
            }
        }
        public string CREATED_BY
        {
            get
            {
                return (string)this["CREATED_BY"];
            }
            set
            {
                this["CREATED_BY"] = value;
            }
        }
        public double? UPLOAD_FLAG
        {
            get
            {
                return (double?)this["UPLOAD_FLAG"];
            }
            set
            {
                this["UPLOAD_FLAG"] = value;
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
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }
        public string DESCRIPT
        {
            get
            {
                return (string)this["DESCRIPT"];
            }
            set
            {
                this["DESCRIPT"] = value;
            }
        }
        public string ROHS_FLAG
        {
            get
            {
                return (string)this["ROHS_FLAG"];
            }
            set
            {
                this["ROHS_FLAG"] = value;
            }
        }
        public string CHECK_MESSAGE
        {
            get
            {
                return (string)this["CHECK_MESSAGE"];
            }
            set
            {
                this["CHECK_MESSAGE"] = value;
            }
        }
        public string TRANS_ID
        {
            get
            {
                return (string)this["TRANS_ID"];
            }
            set
            {
                this["TRANS_ID"] = value;
            }
        }
        public string SN_TYPE
        {
            get
            {
                return (string)this["SN_TYPE"];
            }
            set
            {
                this["SN_TYPE"] = value;
            }
        }
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
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
    }
    public class r_2d_sn_relation
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string BARCODE_2D { get; set; }
        public string ITEM_NUMBER { get; set; }
        public string ITEM_VER { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public double? UPLOAD_FLAG { get; set; }
        public string WO { get; set; }
        public string LABEL_TYPE { get; set; }
        public string DATA3 { get; set; }
        public string DESCRIPT { get; set; }
        public string ROHS_FLAG { get; set; }
        public string CHECK_MESSAGE { get; set; }
        public string TRANS_ID { get; set; }
        public string SN_TYPE { get; set; }
        public string PO { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
    }
}