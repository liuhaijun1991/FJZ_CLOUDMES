using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_MDS_HEAD : DataObjectTable
    {
        public T_R_MDS_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MDS_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MDS_HEAD);
            TableName = "R_MDS_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_MDS_HEAD : DataObjectBase
    {
        public Row_R_MDS_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_MDS_HEAD GetDataObject()
        {
            R_MDS_HEAD DataObject = new R_MDS_HEAD();
            DataObject.ID = this.ID;
            DataObject.MDSKEY = this.MDSKEY;
            DataObject.MDSTYPE = this.MDSTYPE;
            DataObject.MDSFILE = this.MDSFILE;
            DataObject.CQA = this.CQA;
            DataObject.CONVERTED = this.CONVERTED;
            DataObject.SEND = this.SEND;
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
        public string MDSKEY
        {
            get
            {
                return (string)this["MDSKEY"];
            }
            set
            {
                this["MDSKEY"] = value;
            }
        }
        public string MDSTYPE
        {
            get
            {
                return (string)this["MDSTYPE"];
            }
            set
            {
                this["MDSTYPE"] = value;
            }
        }
        public string MDSFILE
        {
            get
            {
                return (string)this["MDSFILE"];
            }
            set
            {
                this["MDSFILE"] = value;
            }
        }
        public string CQA
        {
            get
            {
                return (string)this["CQA"];
            }
            set
            {
                this["CQA"] = value;
            }
        }
        public string CONVERTED
        {
            get
            {
                return (string)this["CONVERTED"];
            }
            set
            {
                this["CONVERTED"] = value;
            }
        }
        public string SEND
        {
            get
            {
                return (string)this["SEND"];
            }
            set
            {
                this["SEND"] = value;
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
    public class R_MDS_HEAD
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string MDSKEY { get; set; }
        public string MDSTYPE { get; set; }
        public string MDSFILE { get; set; }
        public string CQA { get; set; } = "0";
        public string CONVERTED { get; set; } = "0";
        public string SEND { get; set; } = "0";
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
    }

    public enum ENUM_R_MDS_HEAD
    {
        /// <summary>
        /// 已发送
        /// </summary>
        [EnumValue("1")]
        SEND_TRUE,
        /// <summary>
        /// 未发送
        /// </summary>
        [EnumValue("0")]
        SEND_FALSE,
        /// <summary>
        /// 已收集数据
        /// </summary>
        [EnumValue("1")]
        CONVERTED_TRUE,
        /// <summary>
        /// 待收集数据
        /// </summary>
        [EnumValue("0")]
        CONVERTED_FALSE,
        /// <summary>
        /// 已CQA
        /// </summary>
        [EnumValue("1")]
        CQA_TRUE,
        /// <summary>
        /// 待CQA
        /// </summary>
        [EnumValue("0")]
        CQA_FALSE
    }
}