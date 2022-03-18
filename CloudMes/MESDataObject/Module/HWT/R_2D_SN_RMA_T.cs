using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.HWT
{
    public class T_R_2D_SN_RMA_T : DataObjectTable
    {
        public T_R_2D_SN_RMA_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2D_SN_RMA_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2D_SN_RMA_T);
            TableName = "R_2D_SN_RMA_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// add by hgb 2019.08.26
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool IsExists(string SN, OleExec DB)
        {
            string StrSql = "";
            bool CheckFlag = false;
            DataTable Dt = new DataTable();
            StrSql = $@"SELECT * FROM R_2D_SN_RMA_T WHERE SN = '{SN}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        /// <summary>
        /// add by hgb 2019.08.26
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public R_2D_SN_RMA_T LoadData(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_2D_SN_RMA_T>().Where(t => t.SN == SN).ToList().FirstOrDefault();
        }
    }
    public class Row_R_2D_SN_RMA_T : DataObjectBase
    {
        public Row_R_2D_SN_RMA_T(DataObjectInfo info) : base(info)
        {

        }
        public R_2D_SN_RMA_T GetDataObject()
        {
            R_2D_SN_RMA_T DataObject = new R_2D_SN_RMA_T();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.BARCODE_2D = this.BARCODE_2D;
            DataObject.ITEM_NAME = this.ITEM_NAME;
            DataObject.ITEM_VER = this.ITEM_VER;
            DataObject.MFR_CODE = this.MFR_CODE;
            DataObject.CREATED_TIME = this.CREATED_TIME;
            DataObject.CREATED_BY = this.CREATED_BY;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
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
        public string ITEM_NAME
        {
            get
            {
                return (string)this["ITEM_NAME"];
            }
            set
            {
                this["ITEM_NAME"] = value;
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
        public string MFR_CODE
        {
            get
            {
                return (string)this["MFR_CODE"];
            }
            set
            {
                this["MFR_CODE"] = value;
            }
        }
        public DateTime? CREATED_TIME
        {
            get
            {
                return (DateTime?)this["CREATED_TIME"];
            }
            set
            {
                this["CREATED_TIME"] = value;
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
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
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
    public class R_2D_SN_RMA_T
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string BARCODE_2D { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_VER { get; set; }
        public string MFR_CODE { get; set; }
        public DateTime? CREATED_TIME { get; set; }
        public string CREATED_BY { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}