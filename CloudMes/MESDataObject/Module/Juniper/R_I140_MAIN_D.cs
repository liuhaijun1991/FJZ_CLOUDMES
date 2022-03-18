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
    public class T_R_I140_MAIN_D : DataObjectTable
    {
        public T_R_I140_MAIN_D(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_I140_MAIN_D(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_I140_MAIN_D);
            TableName = "R_I140_MAIN_D".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int InSertRow(R_I140_MAIN_D Row, OleExec DB)
        {
            return DB.ORM.Insertable<R_I140_MAIN_D>(Row).ExecuteCommand();
        }
    }
    public class Row_R_I140_MAIN_D : DataObjectBase
    {
        public Row_R_I140_MAIN_D(DataObjectInfo info) : base(info)
        {

        }
        public R_I140_MAIN_D GetDataObject()
        {
            R_I140_MAIN_D DataObject = new R_I140_MAIN_D();
            DataObject.COMMITDATE = this.COMMITDATE;
            DataObject.VENDORCODE = this.VENDORCODE;
            DataObject.ID = this.ID;
            DataObject.TRANID = this.TRANID;
            DataObject.COMMITDAY = this.COMMITDAY;
            DataObject.COMPLETE = this.COMPLETE;
            DataObject.COMMITID = this.COMMITID;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITEMP = this.EDITEMP;
            return DataObject;
        }
        public string COMMITDATE
        {
            get
            {
                return (string)this["COMMITDATE"];
            }
            set
            {
                this["COMMITDATE"] = value;
            }
        }
        public string VENDORCODE
        {
            get
            {
                return (string)this["VENDORCODE"];
            }
            set
            {
                this["VENDORCODE"] = value;
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
        public string COMMITDAY
        {
            get
            {
                return (string)this["COMMITDAY"];
            }
            set
            {
                this["COMMITDAY"] = value;
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
        public string COMMITID
        {
            get
            {
                return (string)this["COMMITID"];
            }
            set
            {
                this["COMMITID"] = value;
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
    }
    public class R_I140_MAIN_D
    {
        public string COMMITDATE { get; set; }
        public string VENDORCODE { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string TRANID { get; set; }
        public string COMMITDAY { get; set; }
        public string COMPLETE { get; set; }
        public string COMMITID { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITEMP { get; set; }
        public DateTime? STOCKINGTIME { get; set; }

    }
    public enum I_I140_MAIN_D_ENUM
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