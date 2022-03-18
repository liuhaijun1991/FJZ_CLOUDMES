using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.Juniper
{
    public class T_R_I140_MAIN : DataObjectTable
    {
        public T_R_I140_MAIN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I140_MAIN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I140_MAIN);
            TableName = "R_I140_MAIN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_I140_MAIN : DataObjectBase
    {
        public Row_R_I140_MAIN(DataObjectInfo info) : base(info)
        {

        }
        public R_I140_MAIN GetDataObject()
        {
            R_I140_MAIN DataObject = new R_I140_MAIN();
            DataObject.ID = this.ID;
            DataObject.TRANID = this.TRANID;
            DataObject.WEEKNO = this.WEEKNO;
            DataObject.YEARNO = this.YEARNO;
            DataObject.COMPLETE = this.COMPLETE;
            DataObject.PARTNUM = this.PARTNUM;
            DataObject.ITEMNUM = this.ITEMNUM;
            DataObject.CREATETIME = this.CREATETIME;
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
        public string TRANID
        {
            get
            {
                return (string)this["TRANID"];
            }
            set
            {
                this["TRANID"] = value;
            }
        }
        public string WEEKNO
        {
            get
            {
                return (string)this["WEEKNO"];
            }
            set
            {
                this["WEEKNO"] = value;
            }
        }
        public string YEARNO
        {
            get
            {
                return (string)this["YEARNO"];
            }
            set
            {
                this["YEARNO"] = value;
            }
        }
        public string COMPLETE
        {
            get
            {
                return (string)this["COMPLETE"];
            }
            set
            {
                this["COMPLETE"] = value;
            }
        }
        public string PARTNUM
        {
            get
            {
                return (string)this["PARTNUM"];
            }
            set
            {
                this["PARTNUM"] = value;
            }
        }
        public string ITEMNUM
        {
            get
            {
                return (string)this["ITEMNUM"];
            }
            set
            {
                this["ITEMNUM"] = value;
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
    public class R_I140_MAIN
    {
        public string ID { get; set; }
        public string TRANID { get; set; }
        public string WEEKNO { get; set; }
        public string YEARNO { get; set; }
        public string COMPLETE { get; set; }
        public string PARTNUM { get; set; }
        public string ITEMNUM { get; set; }
        public string PLANT { get; set; }        
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
    }

    public enum I_I140_MAIN_ENUM
    {
        /// <summary>
        /// 0-未完成
        /// </summary>
        [EnumValue("0")]
        COMPLETE_NO,
        /// <summary>
        /// 1-已完成
        /// </summary>
        [EnumValue("1")]
        COMPLETE_YES
    }
}