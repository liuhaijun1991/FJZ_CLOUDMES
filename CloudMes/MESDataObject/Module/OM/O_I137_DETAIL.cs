using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.OM
{
    public class O_I137_DETAIL
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string SALESORDERLINEITEM { get; set; }
        public string COMPONENTID { get; set; }
        public string COMCUSTPRODID { get; set; }
        public string COMSALESORDERLINEITEM { get; set; }
        public string LINEUOM { get; set; }
        public string COMPONENTQTY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
    [SqlSugar.SugarTable("O_I137_DETAIL")]
    public class I137_D
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string SALESORDERLINEITEM { get; set; }
        public string COMPONENTID { get; set; }
        public string COMCUSTPRODID { get; set; }
        public string COMSALESORDERLINEITEM { get; set; }
        public string LINEUOM { get; set; }
        public string COMPONENTQTY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
        public DateTime? CREATETIME { get; set; }
    }


    [SqlSugar.SugarTable("jnp.tb_i137Detail")]
    public class B2B_I137_D
    {
        public double? F_ID { get; set; }
        public string TRANID { get; set; }
        public string F_PLANT { get; set; }
        public string FILENAME { get; set; }
        public string MESSAGEID { get; set; }
        public string PONUMBER { get; set; }
        public string ITEM { get; set; }
        public string SALESORDERLINEITEM { get; set; }
        public string COMPONENTID { get; set; }
        public string COMCUSTPRODID { get; set; }
        public string COMSALESORDERLINEITEM { get; set; }
        public string LINEUOM { get; set; }
        public string COMPONENTQTY { get; set; }
        public DateTime? F_LASTEDITDT { get; set; }
    }
}