using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SD_CUSTOMER_SO : DataObjectTable
    {
        public T_SD_CUSTOMER_SO(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SD_CUSTOMER_SO(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SD_CUSTOMER_SO);
            TableName = "SD_CUSTOMER_SO".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_SD_CUSTOMER_SO : DataObjectBase
    {
        public Row_SD_CUSTOMER_SO(DataObjectInfo info) : base(info)
        {

        }
        public SD_CUSTOMER_SO GetDataObject()
        {
            SD_CUSTOMER_SO DataObject = new SD_CUSTOMER_SO();
            DataObject.IHREZ_E = this.IHREZ_E;
            DataObject.NAME1 = this.NAME1;
            DataObject.POSEX_E = this.POSEX_E;
            DataObject.BSTKD_E = this.BSTKD_E;
            DataObject.POSEX = this.POSEX;
            DataObject.BSTKD = this.BSTKD;
            DataObject.POSNR = this.POSNR;
            DataObject.VBELN = this.VBELN;
            return DataObject;
        }
        public string IHREZ_E
        {
            get
            {
                return (string)this["IHREZ_E"];
            }
            set
            {
                this["IHREZ_E"] = value;
            }
        }
        public string NAME1
        {
            get
            {
                return (string)this["NAME1"];
            }
            set
            {
                this["NAME1"] = value;
            }
        }
        public string POSEX_E
        {
            get
            {
                return (string)this["POSEX_E"];
            }
            set
            {
                this["POSEX_E"] = value;
            }
        }
        public string BSTKD_E
        {
            get
            {
                return (string)this["BSTKD_E"];
            }
            set
            {
                this["BSTKD_E"] = value;
            }
        }
        public string POSEX
        {
            get
            {
                return (string)this["POSEX"];
            }
            set
            {
                this["POSEX"] = value;
            }
        }
        public string BSTKD
        {
            get
            {
                return (string)this["BSTKD"];
            }
            set
            {
                this["BSTKD"] = value;
            }
        }
        public string POSNR
        {
            get
            {
                return (string)this["POSNR"];
            }
            set
            {
                this["POSNR"] = value;
            }
        }
        public string VBELN
        {
            get
            {
                return (string)this["VBELN"];
            }
            set
            {
                this["VBELN"] = value;
            }
        }
    }
    public class SD_CUSTOMER_SO
    {
        public string IHREZ_E{ get; set; }
        public string NAME1{ get; set; }
        public string POSEX_E{ get; set; }
        public string BSTKD_E{ get; set; }
        public string POSEX{ get; set; }
        public string BSTKD{ get; set; }
        public string POSNR{ get; set; }
        public string VBELN{ get; set; }
    }
}