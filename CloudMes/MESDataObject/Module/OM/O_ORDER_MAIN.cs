using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module.OM
{
    public class O_ORDER_MAIN
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string UPOID { get; set; }
        public string PONO { get; set; }
        public string POLINE { get; set; }
        public string VERSION { get; set; }
        public string POTYPE { get; set; }
        public string QTY { get; set; }
        public string PREWO { get; set; }
        public string UNITPRICE { get; set; }
        public string PID { get; set; }
        public string SEQ { get; set; }
        public DateTime? DELIVERY { get; set; }
        public string COMPLETED { get; set; }
        public DateTime? COMPLETIME { get; set; }
        public string CLOSED { get; set; }
        public DateTime? CLOSETIME { get; set; }
        public string CANCEL { get; set; }
        public DateTime? CANCELTIME { get; set; }
        public string CUSTOMER { get; set; }
        public DateTime? CREATETIME { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string ITEMID { get; set; }
        public string ORIGINALID { get; set; }
        public string USERITEMTYPE { get; set; }
        public string OFFERINGTYPE { get; set; }
        public DateTime? POCREATETIME { get; set; }
        public string PLANT { get; set; }
        public string ORIGINALITEMID { get; set; }
        public string LASTCHANGETIME { get; set; }
        public string CUSTPID { get; set; }
        public string PREASN { get; set; }
        public DateTime? PREASNTIME { get; set; }
        public string FINALASN { get; set; }
        public DateTime? FINALASNTIME { get; set; }
        public string ORDERTYPE { get; set; }
        public string RMQPONO { get; set; }        
    }

    public enum ENUM_O_ORDER_MAIN
    {
        [EnumValue("0")]
        COMPLETED_NO,
        [EnumValue("1")]
        COMPLETED_YES,
        [EnumValue("0")]
        CLOSED_NO,
        [EnumValue("1")]
        CLOSED_YES,
        [EnumValue("0")]
        CANCEL_NO,
        [EnumValue("1")]
        CANCEL_WAIT,
        [EnumValue("2")]
        CANCEL_YES,
        [EnumValue("0")]
        PREASN_NO,
        [EnumValue("1")]
        PREASN_YES,
        [EnumValue("0")]
        FINALASN_NO,
        [EnumValue("1")]
        FINALASN_YES,

    }

    public enum ENUM_O_ORDER_MAIN_POTYPE
    {
        [EnumValue("CTO")]
        CTO,
        [EnumValue("BTS")]
        BTS,
        [EnumValue("BNDL")]
        BNDL,
        [EnumValue("OPTICS")]
        OPTICS,
        [EnumValue("NONE")]
        NONE
    }
    
    public enum ENUM_JNP_ORDER_TYPE
    {
        [EnumValue("NORMAL")]
        NORMAL,
        [EnumValue("BTS")]
        RMQ
    }

}