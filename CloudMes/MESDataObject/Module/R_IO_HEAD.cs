using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_R_IO_HEAD : DataObjectTable
    {
        public T_R_IO_HEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_IO_HEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_IO_HEAD);
            TableName = "R_IO_HEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public R_IO_HEAD GetWashPsbBySN(string sn, OleExec db)
        {
            return db.ORM.Queryable<R_IO_HEAD>().Where(r => r.SN == sn).OrderBy(label => label.EDIT_TIME, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        public R_IO_HEAD GetWashPcbBySNCheckStatus(string sn, OleExec db)
        {
            return db.ORM.Queryable<R_IO_HEAD>().Where(r => r.SN == sn && r.IOSTATUS == "0").ToList().FirstOrDefault();
        }
        public string PassWashActionDetail(string sn,string Reason, string Location,string Line, string EMP_NO, string Bu, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            T_R_IO_HEAD SnDetailTable = new T_R_IO_HEAD(DB, this.DBType);
            Row_R_IO_HEAD SnDetailRow;

            string result = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                string SqlCheck = string.Empty;
                var tt = SnDetailTable.GetWashPsbBySN(sn, DB);
                var aa = SnDetailTable.GetWashPcbBySNCheckStatus(sn, DB);

                if (tt != null && aa != null)
                {
                    SnDetailRow = (Row_R_IO_HEAD)SnDetailTable.GetObjByID(tt.ID, DB);
                    SnDetailRow.IOSTATUS = "1";
                    SnDetailRow.EDIT_EMP = EMP_NO;
                    SnDetailRow.EDIT_TIME = GetDBDateTime(DB);
                    sql = SnDetailRow.GetUpdateString(this.DBType);
                    result = DB.ExecSQL(sql);
                    if (Int32.Parse(result) == 0)
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { sn }));
                }

                else
                {
                    try
                    {
                        SnDetailRow = (Row_R_IO_HEAD)SnDetailTable.NewRow();
                        SnDetailRow.ID = SnDetailTable.GetNewID(Bu, DB);
                        SnDetailRow.SN = sn;
                        SnDetailRow.IOSTATUS = "0";
                        SnDetailRow.DATA1 = Reason;
                        SnDetailRow.DATA2 = Location;
                        SnDetailRow.DATA3= Line;
                        SnDetailRow.IOTYPE = "WASHPCB";
                        SnDetailRow.EDIT_EMP = EMP_NO;
                        SnDetailRow.EDIT_TIME = GetDBDateTime(DB);
                        sql = SnDetailRow.GetInsertString(this.DBType);
                        result = DB.ExecSQL(sql);
                        if (Int32.Parse(result) == 0)
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { sn }));
                    }
                    catch (Exception ex)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { sn }) + ex.Message);
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }
        public int UpdateWashpcbStatus(string sn, string status, string user, OleExec DB)
        {
            return DB.ORM.Updateable<R_IO_HEAD>().UpdateColumns(t => new R_IO_HEAD { IOSTATUS = status, LAST_EDITBY = user, LAST_EDIT_TIME = DateTime.Now })
                .Where(t => t.SN == sn && t.IOSTATUS == "0").ExecuteCommand();
        }
    }
    public class Row_R_IO_HEAD : DataObjectBase
    {
        public Row_R_IO_HEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_IO_HEAD GetDataObject()
        {
            R_IO_HEAD DataObject = new R_IO_HEAD();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.IOSTATUS = this.IOSTATUS;
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.IOTYPE = this.IOTYPE;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.LAST_EDITBY = this.LAST_EDITBY;
            DataObject.LAST_EDIT_TIME = this.LAST_EDIT_TIME;
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
        public string IOSTATUS
        {
            get
            {
                return (string)this["IOSTATUS"];
            }
            set
            {
                this["IOSTATUS"] = value;
            }
        }
        public string DATA1
        {
            get
            {
                return (string)this["DATA1"];
            }
            set
            {
                this["DATA1"] = value;
            }
        }
        public string DATA2
        {
            get
            {
                return (string)this["DATA2"];
            }
            set
            {
                this["DATA2"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
            }
        }

        public string IOTYPE
        {
            get
            {
                return (string)this["IOTYPE"];
            }
            set
            {
                this["IOTYPE"] = value;
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
        public string LAST_EDITBY
        {
            get
            {
                return (string)this["LAST_EDITBY"];
            }
            set
            {
                this["LAST_EDITBY"] = value;
            }
        }

        public DateTime? LAST_EDIT_TIME
        {
            get
            {
                return (DateTime?)this["LAST_EDIT_TIME"];
            }
            set
            {
                this["LAST_EDIT_TIME"] = value;
            }
        }
    }
    public class R_IO_HEAD
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string IOTYPE { get; set; }
        public string IOSTATUS { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string DATA6 { get; set; }
        public string DATA7 { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string EDIT_EMP { get; set; }
        public string LAST_EDITBY { get; set; }
        public DateTime? LAST_EDIT_TIME { get; set; }
        
    }

}