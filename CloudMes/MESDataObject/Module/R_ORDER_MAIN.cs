using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDBHelper;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_ORDER_MAIN : DataObjectTable
    {
        public T_R_ORDER_MAIN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ORDER_MAIN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ORDER_MAIN);
            TableName = "R_ORDER_MAIN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
    }
    public class Row_R_ORDER_MAIN : DataObjectBase
    {
        public Row_R_ORDER_MAIN(DataObjectInfo info) : base(info)
        {

        }
        public R_ORDER_MAIN GetDataObject()
        {
            R_ORDER_MAIN DataObject = new R_ORDER_MAIN();
            DataObject.ID = this.ID;
            DataObject.PO = this.PO;
            DataObject.POLINE = this.POLINE;
            DataObject.STATUSID = this.STATUSID;
            DataObject.POQTY = this.POQTY;
            DataObject.PREWOFLAG = this.PREWOFLAG;
            DataObject.UNITPRICE = this.UNITPRICE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SEQ = this.SEQ;
            DataObject.DELIVERYDATE = this.DELIVERYDATE;
            DataObject.FINISHED = this.FINISHED;
            DataObject.POTYPE = this.POTYPE;
            DataObject.CREATETIME = this.CREATETIME; 
            DataObject.CUSTOMER = this.CUSTOMER;
            DataObject.POID = this.POID;
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
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
            }
        }
        public string POLINE
        {
            get
            {
                return (string)this["POLINE"];
            }
            set
            {
                this["POLINE"] = value;
            }
        }
        public string STATUSID
        {
            get
            {
                return (string)this["STATUSID"];
            }
            set
            {
                this["STATUSID"] = value;
            }
        }
        public double? POQTY
        {
            get
            {
                return (double?)this["POQTY"];
            }
            set
            {
                this["POQTY"] = value;
            }
        }
        public string PREWOFLAG
        {
            get
            {
                return (string)this["PREWOFLAG"];
            }
            set
            {
                this["PREWOFLAG"] = value;
            }
        }
        public double? UNITPRICE
        {
            get
            {
                return (double?)this["UNITPRICE"];
            }
            set
            {
                this["UNITPRICE"] = value;
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
        public string SEQ
        {
            get
            {
                return (string)this["SEQ"];
            }
            set
            {
                this["SEQ"] = value;
            }
        }
        public DateTime? DELIVERYDATE
        {
            get
            {
                return (DateTime?)this["DELIVERYDATE"];
            }
            set
            {
                this["DELIVERYDATE"] = value;
            }
        }
        public string FINISHED
        {
            get
            {
                return (string)this["FINISHED"];
            }
            set
            {
                this["FINISHED"] = value;
            }
        }
        public string POTYPE
        {
            get
            {
                return (string)this["POTYPE"];
            }
            set
            {
                this["POTYPE"] = value;
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
        public string CUSTOMER
        {
            get
            {
                return (string)this["CUSTOMER"];
            }
            set
            {
                this["CUSTOMER"] = value;
            }
        }
        public string POID
        {
            get
            {
                return (string)this["POID"];
            }
            set
            {
                this["POID"] = value;
            }
        }
    }
    public class R_ORDER_MAIN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string PO { get; set; }
        public string POLINE { get; set; }
        public string STATUSID { get; set; }
        public double? POQTY { get; set; }
        public string PREWOFLAG { get; set; }
        public double? UNITPRICE { get; set; }
        public string SKUNO { get; set; }
        public string SEQ { get; set; }
        public DateTime? DELIVERYDATE { get; set; }
        public string FINISHED { get; set; }
        public string POTYPE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CUSTOMER { get; set; }
        public string POID { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string CommitQty { get; set; }
        [SugarColumn(IsIgnore = true)]
        public string R_ORDERSTATUS_MAPNAME { get; set; }
    }

    public enum ENUM_R_ORDER_MAIN
    {
        [EnumValue("1")]
        Finished_Y,
        [EnumValue("0")]
        Finished_N,
        [EnumValue("1")]
        PreWo_Y,
        [EnumValue("0")]
        PreWo_N,
        [EnumValue("ATO")]
        POTYPE_ATO,
        [EnumValue("PTO")]
        POTYPE_PTO
    }


    public enum ENUM_ORDER_STATUS
    {
        /// <summary>
        /// PO接收,初始状态
        /// </summary>
        [EnumValue("0")]
        PoReceive,
        /// <summary>
        /// 待POcommit
        /// </summary>
        [EnumValue("8")]
        WaitPoCommit,
        /// <summary>
        /// PoCommitFinish
        /// </summary>
        [EnumValue("10")]
        PoCommitFinish,
        /// <summary>
        /// Commit进行中
        /// </summary>
        [EnumValue("9")]
        PoCommitOn,
    }
}