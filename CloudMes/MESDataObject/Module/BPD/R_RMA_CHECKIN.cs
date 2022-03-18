using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.BPD
{
    public class T_R_RMA_CHECKIN : DataObjectTable
    {
        public T_R_RMA_CHECKIN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RMA_CHECKIN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            TableName = "R_RMA_CHECKIN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public int AddRmaCheckIn(R_RMA_CHECKIN CheckIn,OleExec DB)
        {
            CheckIn.ValidFlag = "1";
            CheckIn.EditTime = GetDBDateTime(DB);
            DB.ORM.Updateable<R_RMA_CHECKIN>().Where(t => t.Sn == CheckIn.Sn).UpdateColumns(t => new { ValidFlag = "0" });
            return DB.ORM.Insertable<R_RMA_CHECKIN>(CheckIn).ExecuteCommand();
        }

        public R_RMA_CHECKIN GetById(string Id,OleExec DB)
        {
            return DB.ORM.Queryable<R_RMA_CHECKIN>().Where(t => t.ID == Id && t.ValidFlag=="1").ToList().FirstOrDefault();
        }

        public List<R_RMA_CHECKIN> GetBySn(string Sn, OleExec DB)
        {
            return DB.ORM.Queryable<R_RMA_CHECKIN>().Where(t => t.Sn == Sn && t.ValidFlag=="1").ToList();
        }
    }
    public class R_RMA_CHECKIN
    {
        [SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        [SugarColumn(ColumnName ="SN")]
        public string Sn { get; set; }
        [SugarColumn(ColumnName ="CHECKIN_REASON")]
        public string CheckinReason { get; set; }
        [SugarColumn(ColumnName ="VALID_FLAG")]
        public string ValidFlag { get; set; }
        [SugarColumn(ColumnName ="EDIT_TIME")]
        public DateTime? EditTime { get; set; }
        [SugarColumn(ColumnName ="EDIT_EMP")]
        public string EditEmp { get; set; }
        [SugarColumn(ColumnName ="ATTACH_FILE")]
        public string AttachFile { get; set; }
    }
    
}
