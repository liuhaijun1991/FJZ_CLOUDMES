using MESDBHelper;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.BPD
{
    public class T_R_RMA_REPAIR : DataObjectTable
    {
        public T_R_RMA_REPAIR(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RMA_REPAIR(OleExec DB, DB_TYPE_ENUM DBType)
        {
            TableName = "R_RMA_REPAIR".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int AddRMARepair(R_RMA_REPAIR RMARepair, OleExec DB)
        {
            R_RMA_CHECKIN RMACheckIn = DB.ORM.Queryable<R_RMA_CHECKIN>().Where(t => t.Sn == RMARepair.Sn && t.ValidFlag == "1").ToList().FirstOrDefault();
            if (RMACheckIn != null)
            {
                RMARepair.RMACheckInId = RMACheckIn.ID;
                RMARepair.EditTime = GetDBDateTime(DB);
                return DB.ORM.Insertable<R_RMA_REPAIR>(RMARepair).ExecuteCommand();
            }

            return 0;
        }
    }
    public class R_RMA_REPAIR
    {
        [SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        [SugarColumn(ColumnName ="RMA_CHECKIN_ID")]
        public string RMACheckInId { get; set; }
        [SugarColumn(ColumnName ="SN")]
        public string Sn { get; set; }
        [SugarColumn(ColumnName ="ERROR_CODE")]
        public string ErrorCode { get; set; }
        [SugarColumn(ColumnName ="REASON_CODE")]
        public string ReasonCode { get; set; }
        [SugarColumn(ColumnName ="REPAIR_CODE")]
        public string RepairCode { get; set; }
        [SugarColumn(ColumnName ="REPAIR_LOCATION")]
        public string RepairLocation { get; set; }
        [SugarColumn(ColumnName ="REMARK")]
        public string Remark { get; set; }
        [SugarColumn(ColumnName ="FILE_LOCATION")]
        public string FileLocation { get; set; }
        [SugarColumn(ColumnName ="EDIT_TIME")]
        public DateTime? EditTime { get; set; }
        [SugarColumn(ColumnName ="EDIT_EMP")]
        public string EditEmp { get; set; }
    }
}
