using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.DCN
{
    public class T_HPE_EDI_850 : DataObjectTable
    {
        public T_HPE_EDI_850(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_HPE_EDI_850(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_HPE_EDI_850);
            TableName = "HPE_EDI_850".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_HPE_EDI_850 : DataObjectBase
    {
        public Row_HPE_EDI_850(DataObjectInfo info) : base(info)
        {

        }
        public HPE_EDI_850 GetDataObject()
        {
            HPE_EDI_850 DataObject = new HPE_EDI_850();
            DataObject.ID = this.ID;
            DataObject.F_ID = this.F_ID;
            DataObject.F_SITE = this.F_SITE;
            DataObject.F_PO = this.F_PO;
            DataObject.F_PO_TYPE = this.F_PO_TYPE;
            DataObject.F_PO_DATE = this.F_PO_DATE;
            DataObject.F_PO_COMMENT = this.F_PO_COMMENT;
            DataObject.F_COMPANYCODE = this.F_COMPANYCODE;
            DataObject.F_INCO_TERM = this.F_INCO_TERM;
            //DataObject.F_N1_ST = this.F_N1_ST;
            //DataObject.F_N1_DA = this.F_N1_DA;
            //DataObject.F_N1_BT = this.F_N1_BT;
            //DataObject.F_N1_PD = this.F_N1_PD;
            DataObject.F_LINE = this.F_LINE;
            DataObject.F_LINE_QTY = this.F_LINE_QTY;
            DataObject.F_LINE_PRICE = this.F_LINE_PRICE;
            DataObject.F_PN = this.F_PN;
            DataObject.F_PN_DESC = this.F_PN_DESC;
            DataObject.PO_SCH_LINE = this.PO_SCH_LINE;
            DataObject.F_SCH_QTY = this.F_SCH_QTY;
            DataObject.F_SCH_DR_DATE = this.F_SCH_DR_DATE;
            DataObject.F_LASTEDIT_DT = this.F_LASTEDIT_DT;
            DataObject.F_FILENAME = this.F_FILENAME;
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
        public string F_ID
        {
            get
            {
                return (string)this["F_ID"];
            }
            set
            {
                this["F_ID"] = value;
            }
        }
        public string F_SITE
        {
            get
            {
                return (string)this["F_SITE"];
            }
            set
            {
                this["F_SITE"] = value;
            }
        }
        public string F_PO
        {
            get
            {
                return (string)this["F_PO"];
            }
            set
            {
                this["F_PO"] = value;
            }
        }
        public string F_PO_TYPE
        {
            get
            {
                return (string)this["F_PO_TYPE"];
            }
            set
            {
                this["F_PO_TYPE"] = value;
            }
        }
        public DateTime F_PO_DATE
        {
            get
            {
                return (DateTime)this["F_PO_DATE"];
            }
            set
            {
                this["F_PO_DATE"] = value;
            }
        }
        public string F_PO_COMMENT
        {
            get
            {
                return (string)this["F_PO_COMMENT"];
            }
            set
            {
                this["F_PO_COMMENT"] = value;
            }
        }
        public string F_COMPANYCODE
        {
            get
            {
                return (string)this["F_COMPANYCODE"];
            }
            set
            {
                this["F_COMPANYCODE"] = value;
            }
        }
        public string F_INCO_TERM
        {
            get
            {
                return (string)this["F_INCO_TERM"];
            }
            set
            {
                this["F_INCO_TERM"] = value;
            }
        }
        public string F_N1_ST
        {
            get
            {
                return (string)this["F_N1_ST"];
            }
            set
            {
                this["F_N1_ST"] = value;
            }
        }
        public string F_N1_DA
        {
            get
            {
                return (string)this["F_N1_DA"];
            }
            set
            {
                this["F_N1_DA"] = value;
            }
        }
        public string F_N1_BT
        {
            get
            {
                return (string)this["F_N1_BT"];
            }
            set
            {
                this["F_N1_BT"] = value;
            }
        }
        public string F_N1_PD
        {
            get
            {
                return (string)this["F_N1_PD"];
            }
            set
            {
                this["F_N1_PD"] = value;
            }
        }
        public string F_LINE
        {
            get
            {
                return (string)this["F_LINE"];
            }
            set
            {
                this["F_LINE"] = value;
            }
        }
        public string F_LINE_QTY
        {
            get
            {
                return (string)this["F_LINE_QTY"];
            }
            set
            {
                this["F_LINE_QTY"] = value;
            }
        }
        public Single F_LINE_PRICE
        {
            get
            {
                return (Single)this["F_LINE_PRICE"];
            }
            set
            {
                this["F_LINE_PRICE"] = value;
            }
        }
        public string F_PN
        {
            get
            {
                return (string)this["F_PN"];
            }
            set
            {
                this["F_PN"] = value;
            }
        }
        public string F_PN_DESC
        {
            get
            {
                return (string)this["F_PN_DESC"];
            }
            set
            {
                this["F_PN_DESC"] = value;
            }
        }
        public string PO_SCH_LINE
        {
            get
            {
                return (string)this["PO_SCH_LINE"];
            }
            set
            {
                this["PO_SCH_LINE"] = value;
            }
        }
        public string F_SCH_QTY
        {
            get
            {
                return (string)this["F_SCH_QTY"];
            }
            set
            {
                this["F_SCH_QTY"] = value;
            }
        }
        public DateTime F_SCH_DR_DATE
        {
            get
            {
                return (DateTime)this["F_SCH_DR_DATE"];
            }
            set
            {
                this["F_SCH_DR_DATE"] = value;
            }
        }
        public DateTime F_LASTEDIT_DT
        {
            get
            {
                return (DateTime)this["F_LASTEDIT_DT"];
            }
            set
            {
                this["F_LASTEDIT_DT"] = value;
            }
        }
        public string F_FILENAME
        {
            get
            {
                return (string)this["F_FILENAME"];
            }
            set
            {
                this["F_FILENAME"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
    }
    public class HPE_EDI_850
    {
        public string ID { get; set; }
        public string F_ID { get; set; }
        public string F_SITE { get; set; }
        public string F_PO { get; set; }
        public string F_PO_TYPE { get; set; }
        public DateTime F_PO_DATE { get; set; }
        public string F_PO_COMMENT { get; set; }
        public string F_COMPANYCODE { get; set; }
        public string F_INCO_TERM { get; set; }
        public string F_N1_ST { get; set; }
        public string F_N1_DA { get; set; }
        public string F_N1_BT { get; set; }
        public string F_N1_PD { get; set; }
        public string F_LINE { get; set; }
        public string F_LINE_QTY { get; set; }
        public Double F_LINE_PRICE { get; set; }
        public string F_PN { get; set; }
        public string F_PN_DESC { get; set; }
        public string PO_SCH_LINE { get; set; }
        public string F_SCH_QTY { get; set; }
        public DateTime F_SCH_DR_DATE { get; set; }
        public DateTime F_LASTEDIT_DT { get; set; }
        public string F_FILENAME { get; set; }
        public DateTime EDIT_TIME { get; set; }
        public string FLAG { get; set; }
        public string F_SHIP_MODE { get; set; }
        public string F_CARRIER { get; set; }
        public double F_LINE_LEFT_QTY { get; set; }
        public DateTime? F_SHIP_DATE { get; set; }
        public string F_PIP_TYPE { get; set; }
        public string F_PC_CODE { get; set; }
        public string F_PO_VER { get; set; }
    }

    [SqlSugar.SugarTable("hpe.TB_EDI850")]
    public class B2B_HPE_EDI_850
    {
        public string F_ID { get; set; }
        public string F_SITE { get; set; }
        public string F_PO { get; set; }
        public string F_PO_TYPE { get; set; }
        public DateTime F_PO_DATE { get; set; }
        public string F_PO_COMMENT { get; set; }
        public string F_COMPANYCODE { get; set; }
        public string F_INCO_TERM { get; set; }
        public string F_N1_ST { get; set; }
        public string F_N1_DA { get; set; }
        public string F_N1_BT { get; set; }
        public string F_N1_PD { get; set; }
        public string F_LINE { get; set; }
        public string F_LINE_QTY { get; set; }
        public Double F_LINE_PRICE { get; set; }
        public string F_PN { get; set; }
        public string F_PN_DESC { get; set; }
        public string PO_SCH_LINE { get; set; }
        public string F_SCH_QTY { get; set; }
        public DateTime F_SCH_DR_DATE { get; set; }
        public DateTime F_LASTEDIT_DT { get; set; }
        public string F_FILENAME { get; set; }
        public string F_SHIP_MODE { get; set; }
        public string F_CARRIER { get; set; }
        public Double F_LINE_LEFT_QTY { get; set; }
        public DateTime? F_SHIP_DATE { get; set; }
        public string F_PIP_TYPE { get; set; }
        public string F_PC_CODE { get; set; }
        public string F_PO_VER { get; set; }
    }

    public enum aruba860
    {
        [EnumValue("0")]
        init,
        [EnumValue("1")]
        finish,
        [EnumValue("2")]
        skip,
        [EnumValue("3")]
        waitmail
    }

    public enum aruba860status
    {
        [EnumValue("00")]
        neworder,
        [EnumValue("03")]
        cancel,
        [EnumValue("05")]
        change
    }

    public enum ENUM_ARUBA_PO_STATUS
    {
        [EnumValue("100")]
        WaitCommit,
        [EnumValue("101")]
        CommitFinish,
        [EnumValue("102")]
        Cancel,
        [EnumValue("103")]
        ShipOut
    }

}