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
    [SqlSugar.SugarTable("MES4.R_SN_LINK")]
    public class R_SN_LINK
    {
        public string SN_CODE { get; set; }
        public string P_SN { get; set; }
        public string WO { get; set; }
        public DateTime? WORK_TIME { get; set; }
        public string EMP_NO { get; set; }
        public string PANEL_NO { get; set; }
    }
}
