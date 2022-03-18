using MESDBHelper;

namespace MESDataObject.Module
{
    public class R_TDMS_MDV_DATA
    {
        public R_TDMS_MDV_DATA()
        {

        }

        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string DOMAIN { get; set; }
        public string LOCATION { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "PARTNO")]
        public string PARTNUM { get; set; }
        public string UNITNAME { get; set; }
        public string WORKORDER { get; set; }
        public string FAMILY { get; set; }
        public string ROUTER { get; set; }
        public string OPERATION { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "SWRELEASE")]
        public string STATUS { get; set; }
        public string ZONETYPE { get; set; }
        public string ZONEID { get; set; }
        public string SOLNUM { get; set; }
        public string RUNTIME { get; set; }
        public string TESTSET { get; set; }
        public string TESTEVENT { get; set; }
        public string STARTTIME { get; set; }
        public string ENDTIME { get; set; }
        public string NCCODE { get; set; }
        public string MESSAGE { get; set; }
        [SqlSugar.SugarColumn(ColumnName = "SERIALNO")]
        public string SERIALNUM { get; set; }
    }
}