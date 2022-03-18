using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module.ALLPART
{
    [SqlSugar.SugarTable("MES4.R_TR_PRODUCT_DETAIL")]
    public class R_TR_PRODUCT_DETAIL
    {
        public string WO { get; set; }
        public string P_SN { get; set; }
        public string SMT_CODE { get; set; }
        public string TR_CODE { get; set; }
        public string REPLACE_FLAG { get; set; }
        public string WORK_FLAG { get; set; }
        public DateTime? WORK_TIME { get; set; }
        public string PROCESS_FLAG { get; set; }
    }
}
