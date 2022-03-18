using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class O_ORDER_MAIN_H
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
    }
}