using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class T_O_SKU_PACKAGE : DataObjectTable
    {
        public T_O_SKU_PACKAGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_O_SKU_PACKAGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_O_SKU_PACKAGE);
            TableName = "O_SKU_PACKAGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_O_SKU_PACKAGE : DataObjectBase
    {
        public Row_O_SKU_PACKAGE(DataObjectInfo info) : base(info)
        {

        }
        public O_SKU_PACKAGE GetDataObject()
        {
            O_SKU_PACKAGE DataObject = new O_SKU_PACKAGE();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SCENARIO = this.SCENARIO;
            DataObject.TYPE = this.TYPE;
            DataObject.USAGE = this.USAGE;
            DataObject.FROMN = this.FROMN;
            DataObject.TON = this.TON;
            DataObject.PARTNO = this.PARTNO;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string SCENARIO
        {
            get
            {
                return (string)this["SCENARIO"];
            }
            set
            {
                this["SCENARIO"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string USAGE
        {
            get
            {
                return (string)this["USAGE"];
            }
            set
            {
                this["USAGE"] = value;
            }
        }
        public string FROMN
        {
            get
            {
                return (string)this["FROMN"];
            }
            set
            {
                this["FROMN"] = value;
            }
        }
        public string TON
        {
            get
            {
                return (string)this["TON"];
            }
            set
            {
                this["TON"] = value;
            }
        }
        public string PARTNO
        {
            get
            {
                return (string)this["PARTNO"];
            }
            set
            {
                this["PARTNO"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
    }
    public class O_SKU_PACKAGE
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string SCENARIO { get; set; }
        public string TYPE { get; set; }
        public string USAGE { get; set; }
        public string FROMN { get; set; }
        public string TON { get; set; }
        public string PARTNO { get; set; }
        public DateTime? CREATETIME { get; set; }
    }

    public enum ENUM_O_SKU_PACKAGE_SCENARIO
    {
        [EnumValue("Overpack")]
        Overpack,
        [EnumValue("Multipack")]
        Multipack,
        [EnumValue("CartonOverpack")]
        Carton_Overpack,
        [EnumValue("CartonMultipack")]
        Carton_Multipack
    }

    public enum ENUM_O_SKU_PACKAGE_PACKTYPE
    {
        [EnumValue("Deletion")]
        Deletion,
        [EnumValue("Compulsory")]
        Compulsory,
        [EnumValue("Optional")]
        Optional        
    }
}
