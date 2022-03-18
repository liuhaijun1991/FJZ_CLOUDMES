using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_PACKOUT_WEIGHT : DataObjectTable
    {
        public T_C_PACKOUT_WEIGHT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_PACKOUT_WEIGHT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_PACKOUT_WEIGHT);
            TableName = "C_PACKOUT_WEIGHT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_C_PACKOUT_WEIGHT : DataObjectBase
    {
        public Row_C_PACKOUT_WEIGHT(DataObjectInfo info) : base(info)
        {

        }
        public C_PACKOUT_WEIGHT GetDataObject()
        {
            C_PACKOUT_WEIGHT DataObject = new C_PACKOUT_WEIGHT();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.PN = this.PN;
            DataObject.BOX_NT = this.BOX_NT;
            DataObject.BOX_GW = this.BOX_GW;
            DataObject.PCS_NT = this.PCS_NT;
            DataObject.PCS_GW = this.PCS_GW;
            DataObject.P_NULLWG = this.P_NULLWG;
            DataObject.P_GW = this.P_GW;
            DataObject.PCS_B = this.PCS_B;
            DataObject.PCS_P = this.PCS_P;
            DataObject.BOX_P = this.BOX_P;
            DataObject.LASTEDITDT = this.LASTEDITDT;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
            }
        }
        public string PN
        {
            get
            {
                return (string)this["PN"];
            }
            set
            {
                this["PN"] = value;
            }
        }
        public string BOX_NT
        {
            get
            {
                return (string)this["BOX_NT"];
            }
            set
            {
                this["BOX_NT"] = value;
            }
        }
        public string BOX_GW
        {
            get
            {
                return (string)this["BOX_GW"];
            }
            set
            {
                this["BOX_GW"] = value;
            }
        }
        public string PCS_NT
        {
            get
            {
                return (string)this["PCS_NT"];
            }
            set
            {
                this["PCS_NT"] = value;
            }
        }
        public string PCS_GW
        {
            get
            {
                return (string)this["PCS_GW"];
            }
            set
            {
                this["PCS_GW"] = value;
            }
        }
        public string P_NULLWG
        {
            get
            {
                return (string)this["P_NULLWG"];
            }
            set
            {
                this["P_NULLWG"] = value;
            }
        }
        public string P_GW
        {
            get
            {
                return (string)this["P_GW"];
            }
            set
            {
                this["P_GW"] = value;
            }
        }
        public string PCS_B
        {
            get
            {
                return (string)this["PCS_B"];
            }
            set
            {
                this["PCS_B"] = value;
            }
        }
        public string PCS_P
        {
            get
            {
                return (string)this["PCS_P"];
            }
            set
            {
                this["PCS_P"] = value;
            }
        }
        public string BOX_P
        {
            get
            {
                return (string)this["BOX_P"];
            }
            set
            {
                this["BOX_P"] = value;
            }
        }
        public DateTime? LASTEDITDT
        {
            get
            {
                return (DateTime?)this["LASTEDITDT"];
            }
            set
            {
                this["LASTEDITDT"] = value;
            }
        }
    }
    public class C_PACKOUT_WEIGHT
    {
        public string ID { get; set; }
        public string BU { get; set; }
        public string DESCRIPTION { get; set; }
        public string PN { get; set; }
        public string BOX_NT { get; set; }
        public string BOX_GW { get; set; }
        public string PCS_NT { get; set; }
        public string PCS_GW { get; set; }
        public string P_NULLWG { get; set; }
        public string P_GW { get; set; }
        public string PCS_B { get; set; }
        public string PCS_P { get; set; }
        public string BOX_P { get; set; }
        public DateTime? LASTEDITDT { get; set; }
    }
}