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
    [SqlSugar.SugarTable("MES1.C_SMT_AP_LOCATION")]
    public class C_SMT_AP_LOCATION
    {
        public string SMT_CODE { get; set; }
        public string SLOT_NO { get; set; }
        public string KP_NO { get; set; }
        public string LOCATION { get; set; }
        public string UNUSABLE_FLAG { get; set; }
    }
}
