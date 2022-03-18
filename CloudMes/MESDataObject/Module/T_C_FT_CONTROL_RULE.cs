using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_FT_CONTROL_RULE : DataObjectTable
    {
        public T_C_FT_CONTROL_RULE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_FT_CONTROL_RULE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_FT_CONTROL_RULE);
            TableName = "C_FT_CONTROL_RULE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<C_FT_CONTROL_RULE> GetCheckList(string Sku, OleExec DB, string StationName)
        {
            return DB.ORM.Queryable<C_FT_CONTROL_RULE>().Where(t => t.SKUNO == Sku && t.CONTROL_TYPE == "CHECK_SN KeyPart" && t.USE_FLAG == "Y" && t.STATION == StationName).ToList();
        }
    }
    public class Row_C_FT_CONTROL_RULE : DataObjectBase
    {
        public Row_C_FT_CONTROL_RULE(DataObjectInfo info) : base(info)
        {

        }
        public C_FT_CONTROL_RULE GetDataObject()
        {
            C_FT_CONTROL_RULE DataObject = new C_FT_CONTROL_RULE();
            DataObject.CONTROL_VALUE = this.CONTROL_VALUE;
            DataObject.USE_FLAG = this.USE_FLAG;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_BY = this.EDIT_BY;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION = this.STATION;
            DataObject.CONTROL_TYPE = this.CONTROL_TYPE;
            DataObject.CONTROL_FIELD = this.CONTROL_FIELD;
            return DataObject;
        }
        public string CONTROL_VALUE
        {
            get
            {
                return (string)this["CONTROL_VALUE"];
            }
            set
            {
                this["CONTROL_VALUE"] = value;
            }
        }
        public string USE_FLAG
        {
            get
            {
                return (string)this["USE_FLAG"];
            }
            set
            {
                this["USE_FLAG"] = value;
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
        public string EDIT_BY
        {
            get
            {
                return (string)this["EDIT_BY"];
            }
            set
            {
                this["EDIT_BY"] = value;
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
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string CONTROL_TYPE
        {
            get
            {
                return (string)this["CONTROL_TYPE"];
            }
            set
            {
                this["CONTROL_TYPE"] = value;
            }
        }
        public string CONTROL_FIELD
        {
            get
            {
                return (string)this["CONTROL_FIELD"];
            }
            set
            {
                this["CONTROL_FIELD"] = value;
            }
        }
    }
    public class C_FT_CONTROL_RULE
    {
        public string CONTROL_VALUE { get; set; }
        public string USE_FLAG { get; set; }
        public string DESCRIPTION { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_BY { get; set; }
        public string SKUNO { get; set; }
        public string STATION { get; set; }
        public string CONTROL_TYPE { get; set; }
        public string CONTROL_FIELD { get; set; }
    }

    public class FtControl
    {
        public string CheckType { get; set; }
        public string Value { get; set; }
    }
}