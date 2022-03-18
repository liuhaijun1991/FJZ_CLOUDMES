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
    [SqlSugar.SugarTable("MES4.R_TR_SN_WIP")]
    public class R_TR_SN_WIP
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string TR_SN { get; set; }
        public string WO { get; set; }
        public string KP_NO { get; set; }
        public string MFR_KP_NO { get; set; }
        public string MFR_CODE { get; set; }
        public string DATE_CODE { get; set; }
        public string LOT_CODE { get; set; }
        public int QTY { get; set; }
        public int EXT_QTY { get; set; }
        public DateTime? WORK_TIME { get; set; }
        public string STATION { get; set; }
        public string STATION_FLAG { get; set; }
        public DateTime? START_TIME { get; set; }
        public string WORK_FLAG { get; set; }
        public string EMP_NO { get; set; }
        public string SLOT_NO { get; set; }
        public string PROCESS_FLAG { get; set; }
    }
}
