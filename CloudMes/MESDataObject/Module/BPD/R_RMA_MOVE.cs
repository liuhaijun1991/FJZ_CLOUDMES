using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.BPD
{
    public class T_R_RMA_MOVE : DataObjectTable
    {
        public T_R_RMA_MOVE(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RMA_MOVE(OleExec DB, DB_TYPE_ENUM DBType)
        {
            TableName = "R_RMA_MOVE".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int AddRMAMove(R_RMA_MOVE RMAMove, OleExec DB)
        {
            R_RMA_CHECKIN RMACheckIn = DB.ORM.Queryable<R_RMA_CHECKIN>().Where(t => t.Sn == RMAMove.Sn && t.ValidFlag == "1").ToList().FirstOrDefault();
            if (RMACheckIn != null)
            {
                R_RMA_MOVE LastMove = DB.ORM.Queryable<R_RMA_MOVE>().Where(t => t.RMACheckInId == RMACheckIn.ID).OrderBy(t => t.EditTime, OrderByType.Desc).ToList().FirstOrDefault();
                if (LastMove != null)
                {
                    TimeSpan timeSpan = GetDBDateTime(DB).Subtract(LastMove.EditTime.Value);
                    RMAMove.PassedHours = Math.Round(timeSpan.TotalHours, 2);
                }
                else
                {
                    RMAMove.PassedHours = 0d;
                }
                RMAMove.RMACheckInId = RMACheckIn.ID;
                RMAMove.EditTime = GetDBDateTime(DB);
                return DB.ORM.Insertable<R_RMA_MOVE>(RMAMove).ExecuteCommand();
            }
            return 0;
        }
    }
    public class R_RMA_MOVE
    {
        [SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        [SugarColumn(ColumnName ="RMA_CHECKIN_ID")]
        public string RMACheckInId { get; set; }
        [SugarColumn(ColumnName ="SN")]
        public string Sn { get; set; }
        [SugarColumn(ColumnName ="TO_LOCATION")]
        public string ToLocation { get; set; }
        [SugarColumn(ColumnName ="PASSED_HOURS")]
        public double PassedHours { get; set; }
        [SugarColumn(ColumnName ="REMARK")]
        public string Remark { get; set; }
        [SugarColumn(ColumnName ="EDIT_TIME")]
        public DateTime? EditTime { get; set; }
        [SugarColumn(ColumnName ="EDIT_EMP")]
        public string EditEmp { get; set; }
    }
    
}
