using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;
using SqlSugar;

namespace MESDataObject.Module.OM
{
    public class T_O_AGILE_ATTR : DataObjectTable
    {
        public T_O_AGILE_ATTR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_AGILE_ATTR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_AGILE_ATTR);
            TableName = "O_AGILE_ATTR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_AGILE_ATTR : DataObjectBase
    {
        public Row_O_AGILE_ATTR(DataObjectInfo info) : base(info)
        {

        }
        public O_AGILE_ATTR GetDataObject()
        {
            O_AGILE_ATTR DataObject = new O_AGILE_ATTR();
            DataObject.ID = this.ID;
            DataObject.ITEM_NUMBER = this.ITEM_NUMBER;
            DataObject.REV = this.REV;
            DataObject.CS_FLAG = this.CS_FLAG;
            DataObject.HIDDEN_BOM = this.HIDDEN_BOM;
            DataObject.SERIALIZATION = this.SERIALIZATION;
            DataObject.USER_ITEM_TYPE = this.USER_ITEM_TYPE;
            DataObject.ROHS_COMPLIANCE = this.ROHS_COMPLIANCE;
            DataObject.OFFERING_TYPE = this.OFFERING_TYPE;
            DataObject.CLEI_CODE = this.CLEI_CODE;
            DataObject.CPR_CODE = this.CPR_CODE;
            DataObject.ECI_BAR_CODE = this.ECI_BAR_CODE;
            DataObject.KCC_CERT_NUMBER = this.KCC_CERT_NUMBER;
            DataObject.REGULATORY_MODEL = this.REGULATORY_MODEL;
            DataObject.UPC_CODE = this.UPC_CODE;
            DataObject.SERIAL_NUMBER_MASK = this.SERIAL_NUMBER_MASK;
            DataObject.CHANGE_NUMBER = this.CHANGE_NUMBER;
            DataObject.EFFECTIVE_DATE = this.EFFECTIVE_DATE;
            DataObject.RELEASE_DATE = this.RELEASE_DATE;
            DataObject.PLANT = this.PLANT;
            DataObject.CREATED_BY = this.CREATED_BY;
            DataObject.DATE_CREATED = this.DATE_CREATED;
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
        public string REV
        {
            get
            {
                return (string)this["REV"];
            }
            set
            {
                this["REV"] = value;
            }
        }
        public string CS_FLAG
        {
            get
            {
                return (string)this["CS_FLAG"];
            }
            set
            {
                this["CS_FLAG"] = value;
            }
        }
        public string HIDDEN_BOM
        {
            get
            {
                return (string)this["HIDDEN_BOM"];
            }
            set
            {
                this["HIDDEN_BOM"] = value;
            }
        }
        public string SERIALIZATION
        {
            get
            {
                return (string)this["SERIALIZATION"];
            }
            set
            {
                this["SERIALIZATION"] = value;
            }
        }
        public string USER_ITEM_TYPE
        {
            get
            {
                return (string)this["USER_ITEM_TYPE"];
            }
            set
            {
                this["USER_ITEM_TYPE"] = value;
            }
        }
        public string ROHS_COMPLIANCE
        {
            get
            {
                return (string)this["ROHS_COMPLIANCE"];
            }
            set
            {
                this["ROHS_COMPLIANCE"] = value;
            }
        }
        public string OFFERING_TYPE
        {
            get
            {
                return (string)this["OFFERING_TYPE"];
            }
            set
            {
                this["OFFERING_TYPE"] = value;
            }
        }
        public string CLEI_CODE
        {
            get
            {
                return (string)this["CLEI_CODE"];
            }
            set
            {
                this["CLEI_CODE"] = value;
            }
        }
        public string CPR_CODE
        {
            get
            {
                return (string)this["CPR_CODE"];
            }
            set
            {
                this["CPR_CODE"] = value;
            }
        }
        public string ECI_BAR_CODE
        {
            get
            {
                return (string)this["ECI_BAR_CODE"];
            }
            set
            {
                this["ECI_BAR_CODE"] = value;
            }
        }
        public string KCC_CERT_NUMBER
        {
            get
            {
                return (string)this["KCC_CERT_NUMBER"];
            }
            set
            {
                this["KCC_CERT_NUMBER"] = value;
            }
        }
        public string REGULATORY_MODEL
        {
            get
            {
                return (string)this["REGULATORY_MODEL"];
            }
            set
            {
                this["REGULATORY_MODEL"] = value;
            }
        }
        public string UPC_CODE
        {
            get
            {
                return (string)this["UPC_CODE"];
            }
            set
            {
                this["UPC_CODE"] = value;
            }
        }
        public string SERIAL_NUMBER_MASK
        {
            get
            {
                return (string)this["SERIAL_NUMBER_MASK"];
            }
            set
            {
                this["SERIAL_NUMBER_MASK"] = value;
            }
        }
        public string CHANGE_NUMBER
        {
            get
            {
                return (string)this["CHANGE_NUMBER"];
            }
            set
            {
                this["CHANGE_NUMBER"] = value;
            }
        }
        public DateTime? EFFECTIVE_DATE
        {
            get
            {
                return (DateTime?)this["EFFECTIVE_DATE"];
            }
            set
            {
                this["EFFECTIVE_DATE"] = value;
            }
        }
        public DateTime? RELEASE_DATE
        {
            get
            {
                return (DateTime?)this["RELEASE_DATE"];
            }
            set
            {
                this["RELEASE_DATE"] = value;
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
        public DateTime? DATE_CREATED
        {
            get
            {
                return (DateTime?)this["DATE_CREATED"];
            }
            set
            {
                this["DATE_CREATED"] = value;
            }
        }
    }
    public class O_AGILE_ATTR
    {
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public string ID { get; set; }
        public string ITEM_NUMBER { get; set; }
        public string REV { get; set; }
        public string CS_FLAG { get; set; }
        public string HIDDEN_BOM { get; set; }
        public string SERIALIZATION { get; set; }
        public string USER_ITEM_TYPE { get; set; }
        public string ROHS_COMPLIANCE { get; set; }
        public string OFFERING_TYPE { get; set; }
        public string CLEI_CODE { get; set; }
        public string CPR_CODE { get; set; }
        public string ECI_BAR_CODE { get; set; }
        public string KCC_CERT_NUMBER { get; set; }
        public string REGULATORY_MODEL { get; set; }
        public string UPC_CODE { get; set; }
        public string SERIAL_NUMBER_MASK { get; set; }
        public string CHANGE_NUMBER { get; set; }
        public DateTime? EFFECTIVE_DATE { get; set; }
        public DateTime? RELEASE_DATE { get; set; }
        public string PLANT { get; set; }
        /// <summary>
        /// CUSTPARTNO:CUSTOMER_PART_NUMBER ,2020/01/04新增客戶料號
        /// </summary>
        [SugarColumn(ColumnName = "CUSTOMER_PART_NUMBER")]
        public string CUSTPARTNO { get; set; }
        /// <summary>
        /// 20200108新增料號描述
        /// </summary>
        public string DESCRIPTION { get; set; }
        public string CREATED_BY { get; set; }
        /// <summary>
        /// Mes系统自动生成,无需填充
        /// </summary>
        public DateTime? DATE_CREATED { get; set; }
        public string ACTIVED { get; set; }        
    }

    public enum ENUM_O_AGILE_ATTR_USERITETYPE
    {
        [EnumValue("SYS")]
        SYS,
        [EnumValue("SPARE")]
        SPARE,
        [EnumValue("APP")]
        APP,
        [EnumValue("ADV")]
        ADV,
        [EnumValue("BNDL")]
        BNDL
    }

    public enum ENUM_O_AGILE_ATTR_OFFERINGTYPE
    {
        [EnumValue("Fixed Appliance")]
        Fixed_Appliance,
        [EnumValue("Base Unit - BB")]
        Base_Unit_BB,
        [EnumValue("FRU")]
        FRU,
        [EnumValue("SWLic OS")]
        SWLic_OS,
        [EnumValue("SWLic PreSAP Perp HWOnlyImage")]
        SWLic_PreSAP_Perp_HWOnlyImage,
        [EnumValue("Fixed Nonstockable Bundle")]
        Fixed_Nonstockable_Bundle,
        [EnumValue("Advanced Fixed System")]
        Advanced_Fixed_System,
        [EnumValue("SWLic PreSAP Perp")]
        SWLic_PreSAP_Perp,
        [EnumValue("Premium Configurable Sys")]
        Premium_Configurable_Sys,
        [EnumValue("Configurable System")]
        Configurable_System,
        [EnumValue("Customer-Specific CTO")]
        Customer_Specific_CTO,
        [EnumValue("Combo - Bundle")]
        Combo_Bundle,
        
    }

}