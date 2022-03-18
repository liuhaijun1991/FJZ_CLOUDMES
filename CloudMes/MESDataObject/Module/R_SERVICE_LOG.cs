using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SERVICE_LOG : DataObjectTable
    {
        public T_R_SERVICE_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SERVICE_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SERVICE_LOG);
            TableName = "R_SERVICE_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_SERVICE_LOG : DataObjectBase
    {
        public Row_R_SERVICE_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_SERVICE_LOG GetDataObject()
        {
            R_SERVICE_LOG DataObject = new R_SERVICE_LOG();
            DataObject.ID = this.ID;
            DataObject.FUNCTIONTYPE = this.FUNCTIONTYPE;
            DataObject.LASTEDITTIME = this.LASTEDITTIME;
            DataObject.CURRENTEDITTIME = this.CURRENTEDITTIME;
            DataObject.NEXTEDITTIME = this.NEXTEDITTIME;
            DataObject.SOURCECODE = this.SOURCECODE;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
            DataObject.DATA6 = this.DATA6;
            DataObject.DATA7 = this.DATA7;
            DataObject.DATA8 = this.DATA8;
            DataObject.DATA9 = this.DATA9;
            DataObject.DATA10 = this.DATA10;
            DataObject.DATA11 = this.DATA11;
            DataObject.DATA12 = this.DATA12;
            DataObject.DATA13 = this.DATA13;
            DataObject.DATA14 = this.DATA14;
            DataObject.DATA15 = this.DATA15;
            DataObject.MAILFLAG = this.MAILFLAG;
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
        public string FUNCTIONTYPE
        {
            get
            {
                return (string)this["FUNCTIONTYPE"];
            }
            set
            {
                this["FUNCTIONTYPE"] = value;
            }
        }
        public DateTime? LASTEDITTIME
        {
            get
            {
                return (DateTime?)this["LASTEDITTIME"];
            }
            set
            {
                this["LASTEDITTIME"] = value;
            }
        }
        public DateTime? CURRENTEDITTIME
        {
            get
            {
                return (DateTime?)this["CURRENTEDITTIME"];
            }
            set
            {
                this["CURRENTEDITTIME"] = value;
            }
        }
        public DateTime? NEXTEDITTIME
        {
            get
            {
                return (DateTime?)this["NEXTEDITTIME"];
            }
            set
            {
                this["NEXTEDITTIME"] = value;
            }
        }
        public string SOURCECODE
        {
            get
            {
                return (string)this["SOURCECODE"];
            }
            set
            {
                this["SOURCECODE"] = value;
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
        public string DATA4
        {
            get
            {
                return (string)this["DATA4"];
            }
            set
            {
                this["DATA4"] = value;
            }
        }
        public string DATA5
        {
            get
            {
                return (string)this["DATA5"];
            }
            set
            {
                this["DATA5"] = value;
            }
        }
        public string DATA6
        {
            get
            {
                return (string)this["DATA6"];
            }
            set
            {
                this["DATA6"] = value;
            }
        }
        public string DATA7
        {
            get
            {
                return (string)this["DATA7"];
            }
            set
            {
                this["DATA7"] = value;
            }
        }
        public string DATA8
        {
            get
            {
                return (string)this["DATA8"];
            }
            set
            {
                this["DATA8"] = value;
            }
        }
        public string DATA9
        {
            get
            {
                return (string)this["DATA9"];
            }
            set
            {
                this["DATA9"] = value;
            }
        }
        public string DATA10
        {
            get
            {
                return (string)this["DATA10"];
            }
            set
            {
                this["DATA10"] = value;
            }
        }
        public string DATA11
        {
            get
            {
                return (string)this["DATA11"];
            }
            set
            {
                this["DATA11"] = value;
            }
        }
        public string DATA12
        {
            get
            {
                return (string)this["DATA12"];
            }
            set
            {
                this["DATA12"] = value;
            }
        }
        public string DATA13
        {
            get
            {
                return (string)this["DATA13"];
            }
            set
            {
                this["DATA13"] = value;
            }
        }
        public string DATA14
        {
            get
            {
                return (string)this["DATA14"];
            }
            set
            {
                this["DATA14"] = value;
            }
        }
        public string DATA15
        {
            get
            {
                return (string)this["DATA15"];
            }
            set
            {
                this["DATA15"] = value;
            }
        }
        public string MAILFLAG
        {
            get
            {
                return (string)this["MAILFLAG"];
            }
            set
            {
                this["MAILFLAG"] = value;
            }
        }
    }
    public class R_SERVICE_LOG
    {
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string FUNCTIONTYPE { get; set; }
        public DateTime? LASTEDITTIME { get; set; }
        public DateTime? CURRENTEDITTIME { get; set; }
        public DateTime? NEXTEDITTIME { get; set; }
        public string SOURCECODE { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string DATA6 { get; set; }
        public string DATA7 { get; set; }
        public string DATA8 { get; set; }
        public string DATA9 { get; set; }
        public string DATA10 { get; set; }
        public string DATA11 { get; set; }
        public string DATA12 { get; set; }
        public string DATA13 { get; set; }
        public string DATA14 { get; set; }
        public string DATA15 { get; set; }
        public string MAILFLAG { get; set; }
    }
}