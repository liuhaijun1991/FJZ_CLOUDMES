using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_SFCPACKOUTSETTING : DataObjectTable
    {
        public T_SFCPACKOUTSETTING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_SFCPACKOUTSETTING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_SFCPACKOUTSETTING);
            TableName = "SFCPACKOUTSETTING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<SFCPACKOUTSETTING> GETPACKBYSKUNOANDLINE(OleExec DB,string SKU,string Line)
        {
            return DB.ORM.Queryable<SFCPACKOUTSETTING>().Where(t => t.SKUNO == SKU && t.LINE_NAME == Line).ToList();
        }
    }
    public class Row_SFCPACKOUTSETTING : DataObjectBase
    {
        public Row_SFCPACKOUTSETTING(DataObjectInfo info) : base(info)
        {

        }
        public SFCPACKOUTSETTING GetDataObject()
        {
            SFCPACKOUTSETTING DataObject = new SFCPACKOUTSETTING();
            DataObject.LINE_NAME = this.LINE_NAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.TYPE = this.TYPE;
            DataObject.PACKNO = this.PACKNO;
            DataObject.QTY = this.QTY;
            DataObject.MAXQTY = this.MAXQTY;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string LINE_NAME
        {
            get
            {
                return (string)this["LINE_NAME"];
            }
            set
            {
                this["LINE_NAME"] = value;
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
        public string PACKNO
        {
            get
            {
                return (string)this["PACKNO"];
            }
            set
            {
                this["PACKNO"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? MAXQTY
        {
            get
            {
                return (double?)this["MAXQTY"];
            }
            set
            {
                this["MAXQTY"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
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
    }
    public class SFCPACKOUTSETTING
    {
        public string LINE_NAME { get; set; }
        public string SKUNO { get; set; }
        public string TYPE { get; set; }
        public string PACKNO { get; set; }
        public double? QTY { get; set; }
        public double? MAXQTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}