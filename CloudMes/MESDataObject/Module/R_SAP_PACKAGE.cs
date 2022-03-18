using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SAP_PACKAGE : DataObjectTable
    {
        public T_R_SAP_PACKAGE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SAP_PACKAGE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SAP_PACKAGE);
            TableName = "R_SAP_PACKAGE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SAP_PACKAGE : DataObjectBase
    {
        public Row_R_SAP_PACKAGE(DataObjectInfo info) : base(info)
        {

        }
        public R_SAP_PACKAGE GetDataObject()
        {
            R_SAP_PACKAGE DataObject = new R_SAP_PACKAGE();
            DataObject.ID = this.ID;
            DataObject.BU = this.BU;
            DataObject.PN = this.PN;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.BOX_NT = this.BOX_NT;
            DataObject.BOX_GW = this.BOX_GW;
            DataObject.PCS_NT = this.PCS_NT;
            DataObject.PCS_GW = this.PCS_GW;
            DataObject.P_NULLWG = this.P_NULLWG;
            DataObject.PCS_B = this.PCS_B;
            DataObject.PCS_P = this.PCS_P;
            DataObject.BOX_P = this.BOX_P;
            DataObject.P_GW = this.P_GW;
            DataObject.P_GROES = this.P_GROES;
            DataObject.C_GROES = this.C_GROES;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
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
        public string P_GROES
        {
            get
            {
                return (string)this["P_GROES"];
            }
            set
            {
                this["P_GROES"] = value;
            }
        }
        public string C_GROES
        {
            get
            {
                return (string)this["C_GROES"];
            }
            set
            {
                this["C_GROES"] = value;
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
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
    }
    public class R_SAP_PACKAGE
    {
        public string ID { get; set; }
        public string BU { get; set; }
        public string PN { get; set; }
        public string DESCRIPTION { get; set; }
        public string BOX_NT { get; set; }
        public string BOX_GW { get; set; }
        public string PCS_NT { get; set; }
        public string PCS_GW { get; set; }
        public string P_NULLWG { get; set; }
        public string PCS_B { get; set; }
        public string PCS_P { get; set; }
        public string BOX_P { get; set; }
        public string P_GW { get; set; }
        public string P_GROES { get; set; }
        public string C_GROES { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
    }
}