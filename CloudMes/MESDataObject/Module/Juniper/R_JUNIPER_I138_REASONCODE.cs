using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_JUNIPER_I138_REASONCODE : DataObjectTable
    {
        public T_R_JUNIPER_I138_REASONCODE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_JUNIPER_I138_REASONCODE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_JUNIPER_I138_REASONCODE);
            TableName = "R_JUNIPER_I138_REASONCODE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_JUNIPER_I138_REASONCODE : DataObjectBase
    {
        public Row_R_JUNIPER_I138_REASONCODE(DataObjectInfo info) : base(info)
        {

        }
        public R_JUNIPER_I138_REASONCODE GetDataObject()
        {
            R_JUNIPER_I138_REASONCODE DataObject = new R_JUNIPER_I138_REASONCODE();
            DataObject.ID = this.ID;
            DataObject.REASONCODE = this.REASONCODE;
            DataObject.NAME = this.NAME;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.APPLIESTOPULLIN = this.APPLIESTOPULLIN;
            DataObject.APPLIESTOPULLOUT = this.APPLIESTOPULLOUT;
            DataObject.FIRSTCOMMIT = this.FIRSTCOMMIT;
            DataObject.POSTATSRELEVENTDATE = this.POSTATSRELEVENTDATE;
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
        public string REASONCODE
        {
            get
            {
                return (string)this["REASONCODE"];
            }
            set
            {
                this["REASONCODE"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
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
        public string APPLIESTOPULLIN
        {
            get
            {
                return (string)this["APPLIESTOPULLIN"];
            }
            set
            {
                this["APPLIESTOPULLIN"] = value;
            }
        }
        public string APPLIESTOPULLOUT
        {
            get
            {
                return (string)this["APPLIESTOPULLOUT"];
            }
            set
            {
                this["APPLIESTOPULLOUT"] = value;
            }
        }
        public string FIRSTCOMMIT
        {
            get
            {
                return (string)this["FIRSTCOMMIT"];
            }
            set
            {
                this["FIRSTCOMMIT"] = value;
            }
        }
        public string POSTATSRELEVENTDATE
        {
            get
            {
                return (string)this["POSTATSRELEVENTDATE"];
            }
            set
            {
                this["POSTATSRELEVENTDATE"] = value;
            }
        }
    }
    public class R_JUNIPER_I138_REASONCODE
    {
        public string ID { get; set; }
        public string REASONCODE { get; set; }
        public string NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string APPLIESTOPULLIN { get; set; }
        public string APPLIESTOPULLOUT { get; set; }
        public string FIRSTCOMMIT { get; set; }
        public string POSTATSRELEVENTDATE { get; set; }
    }
}