using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_H_WO_TEXT : DataObjectTable
    {
        public T_H_WO_TEXT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_H_WO_TEXT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_H_WO_TEXT);
            TableName = "H_WO_TEXT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_H_WO_TEXT : DataObjectBase
    {
        public Row_H_WO_TEXT(DataObjectInfo info) : base(info)
        {

        }
        public H_WO_TEXT GetDataObject()
        {
            H_WO_TEXT DataObject = new H_WO_TEXT();
            DataObject.LMNGA = this.LMNGA;
            DataObject.MGVRG = this.MGVRG;
            DataObject.VORNR = this.VORNR;
            DataObject.ISAVD = this.ISAVD;
            DataObject.LTXA1 = this.LTXA1;
            DataObject.ARBPL = this.ARBPL;
            DataObject.MATNR = this.MATNR;
            DataObject.AUFNR = this.AUFNR;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string LMNGA
        {
            get
            {
                return (string)this["LMNGA"];
            }
            set
            {
                this["LMNGA"] = value;
            }
        }
        public string MGVRG
        {
            get
            {
                return (string)this["MGVRG"];
            }
            set
            {
                this["MGVRG"] = value;
            }
        }
        public string VORNR
        {
            get
            {
                return (string)this["VORNR"];
            }
            set
            {
                this["VORNR"] = value;
            }
        }
        public string ISAVD
        {
            get
            {
                return (string)this["ISAVD"];
            }
            set
            {
                this["ISAVD"] = value;
            }
        }
        public string LTXA1
        {
            get
            {
                return (string)this["LTXA1"];
            }
            set
            {
                this["LTXA1"] = value;
            }
        }
        public string ARBPL
        {
            get
            {
                return (string)this["ARBPL"];
            }
            set
            {
                this["ARBPL"] = value;
            }
        }
        public string MATNR
        {
            get
            {
                return (string)this["MATNR"];
            }
            set
            {
                this["MATNR"] = value;
            }
        }
        public string AUFNR
        {
            get
            {
                return (string)this["AUFNR"];
            }
            set
            {
                this["AUFNR"] = value;
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
    public class H_WO_TEXT
    {
        public string LMNGA{get;set;}
        public string MGVRG{get;set;}
        public string VORNR{get;set;}
        public string ISAVD{get;set;}
        public string LTXA1{get;set;}
        public string ARBPL{get;set;}
        public string MATNR{get;set;}
        public string AUFNR{get;set;}
        public string ID{get;set;}
    }
}