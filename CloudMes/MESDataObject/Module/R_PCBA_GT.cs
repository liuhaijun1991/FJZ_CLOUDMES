using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_PCBA_GT : DataObjectTable
    {
        public T_R_PCBA_GT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PCBA_GT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PCBA_GT);
            TableName = "R_PCBA_GT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public DataTable GetPCBAGTQtyByTime(DateTime startTime, DateTime endTime, OleExec sfcdb)
        {
            string sql = $@"select m.skuno,m.from_storage,m.to_storage,count(1) qty
                          from r_pcba_gt m
                         where m.sap_flag='0'
                           and m.gt_time >= to_date('{startTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss')
                           and m.gt_time < to_date('{endTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss') group by m.skuno,m.from_storage,m.to_storage";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return dt;
            }
            else
            {
                return null;
            }
        }

        public string UpdatePCBAGTQtyByTime(DateTime startTime, DateTime endTime, OleExec sfcdb)
        {
            int res = 0;
            string sql = $@"update r_pcba_gt m set sap_flag='1',edit_time=sysdate
                         where m.sap_flag='0'
                           and m.gt_time >= to_date('{startTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss')
                           and m.gt_time < to_date('{endTime.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy/mm/dd hh24:mi:ss') group by m.skuno,m.from_storage,m.to_storage";
            try
            {
                res = sfcdb.ExecuteNonQuery(sql, CommandType.Text, null);
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
            return res.ToString();
        }

        public R_PCBA_GT GetPCBAGT(string StrSN, OleExec sfcdb)
        {
            R_PCBA_GT RPCBAGT = null;
            Row_R_PCBA_GT Row = (Row_R_PCBA_GT)NewRow();

            string sql = $@"select *
                          from r_pcba_gt where sn='{StrSN}' order by edit_time desc";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Row.loadData(dt.Rows[0]);
                RPCBAGT = Row.GetDataObject();
            }
            return RPCBAGT;
        }
    }
    public class Row_R_PCBA_GT : DataObjectBase
    {
        public Row_R_PCBA_GT(DataObjectInfo info) : base(info)
        {

        }
        public R_PCBA_GT GetDataObject()
        {
            R_PCBA_GT DataObject = new R_PCBA_GT();
            DataObject.ID = this.ID;
            DataObject.SN_ID = this.SN_ID;
            DataObject.SN = this.SN;
            DataObject.SKUNO = this.SKUNO;
            DataObject.FROM_STORAGE = this.FROM_STORAGE;
            DataObject.TO_STORAGE = this.TO_STORAGE;
            DataObject.SAP_FLAG = this.SAP_FLAG;
            DataObject.GT_TIME = this.GT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public string SN_ID
        {
            get
            {
                return (string)this["SN_ID"];
            }
            set
            {
                this["SN_ID"] = value;
            }
        }
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string FROM_STORAGE
        {
            get
            {
                return (string)this["FROM_STORAGE"];
            }
            set
            {
                this["FROM_STORAGE"] = value;
            }
        }
        public string TO_STORAGE
        {
            get
            {
                return (string)this["TO_STORAGE"];
            }
            set
            {
                this["TO_STORAGE"] = value;
            }
        }
        public string SAP_FLAG
        {
            get
            {
                return (string)this["SAP_FLAG"];
            }
            set
            {
                this["SAP_FLAG"] = value;
            }
        }
        public DateTime? GT_TIME
        {
            get
            {
                return (DateTime?)this["GT_TIME"];
            }
            set
            {
                this["GT_TIME"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_PCBA_GT
    {
        public string ID { get; set; }
        public string SN_ID { get; set; }
        public string SN { get; set; }
        public string SKUNO { get; set; }
        public string FROM_STORAGE { get; set; }
        public string TO_STORAGE { get; set; }
        public string SAP_FLAG { get; set; }
        public DateTime? GT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}