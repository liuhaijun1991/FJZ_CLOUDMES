using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_MES_EXT : DataObjectTable
    {
        public T_R_MES_EXT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MES_EXT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MES_EXT);
            TableName = "R_MES_EXT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MES_EXT : DataObjectBase
    {
        public Row_R_MES_EXT(DataObjectInfo info) : base(info)
        {

        }
        public R_MES_EXT GetDataObject()
        {
            R_MES_EXT DataObject = new R_MES_EXT();
            DataObject.ID = this.ID;
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
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITEMP = this.EDITEMP;
            DataObject.EDITTIME = this.EDITTIME;
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
        public string EDITEMP
        {
            get
            {
                return (string)this["EDITEMP"];
            }
            set
            {
                this["EDITEMP"] = value;
            }
        }
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
    }
    public class R_MES_EXT
    {
        public string ID { get; set; }
        public string DATA1{ get; set; }
        public string DATA2{ get; set; }
        public string DATA3{ get; set; }
        public string DATA4{ get; set; }
        public string DATA5{ get; set; }
        public string DATA6{ get; set; }
        public string DATA7{ get; set; }
        public string DATA8{ get; set; }
        public string DATA9{ get; set; }
        public string DATA10{ get; set; }
        public string DATA11{ get; set; }
        public string DATA12{ get; set; }
        public string DATA13{ get; set; }
        public DateTime? CREATETIME{ get; set; }
        public string EDITEMP{ get; set; }
        public DateTime? EDITTIME{ get; set; }
    }
}