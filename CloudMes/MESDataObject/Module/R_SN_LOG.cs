using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_SN_LOG : DataObjectTable
    {
        public T_R_SN_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_LOG);
            TableName = "R_SN_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_SN_LOG> GetListCartonLog(string sn, OleExec DB)
        {
            List<R_SN_LOG> packingList = new List<R_SN_LOG>();
            Row_R_SN_LOG rowPacking;
            string strSql = $@"SELECT * FROM r_sn_log WHERE SN='{sn}'";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                rowPacking = (Row_R_SN_LOG)this.NewRow();
                rowPacking.loadData(row);
                packingList.Add(rowPacking.GetDataObject());
            }
            return packingList;
        }

        public int Save(OleExec SFCDB, R_SN_LOG logObj)
        {
            return SFCDB.ORM.Insertable<R_SN_LOG>(logObj).ExecuteCommand();
        }

        public int Update(OleExec SFCDB, R_SN_LOG logObj)
        {
            return SFCDB.ORM.Updateable<R_SN_LOG>(logObj).Where(r => r.ID == logObj.ID).ExecuteCommand();
        }

        public List<R_SN_LOG> GetLogBySN(OleExec SFCDB, string sn)
        {
            return SFCDB.ORM.Queryable<R_SN_LOG>().Where(r => r.SN == sn).ToList();
        }
    }
    public class Row_R_SN_LOG : DataObjectBase
    {
        public Row_R_SN_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_LOG GetDataObject()
        {
            R_SN_LOG DataObject = new R_SN_LOG();
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.FLAG = this.FLAG;
            DataObject.DATA9 = this.DATA9;
            DataObject.DATA8 = this.DATA8;
            DataObject.DATA7 = this.DATA7;
            DataObject.DATA6 = this.DATA6;
            DataObject.DATA5 = this.DATA5;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA1 = this.DATA1;
            DataObject.LOGTYPE = this.LOGTYPE;
            DataObject.SN = this.SN;
            DataObject.SNID = this.SNID;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string CREATEBY
        {
            get
            {
                return (string)this["CREATEBY"];
            }
            set
            {
                this["CREATEBY"] = value;
            }
        }
        public DateTime? CREATETIME
        {
            get
            {
                return (DateTime?)this["CREATETIME"];
            }
            set
            {
                this["CREATETIME"] = value;
            }
        }
        public string FLAG
        {
            get
            {
                return (string)this["FLAG"];
            }
            set
            {
                this["FLAG"] = value;
            }
        }
        public string DATA9
        {
            get
            {
                return (string)this["DATA9"];
            }
            set
            {
                this["DATA9"] = value;
            }
        }
        public string DATA8
        {
            get
            {
                return (string)this["DATA8"];
            }
            set
            {
                this["DATA8"] = value;
            }
        }
        public string DATA7
        {
            get
            {
                return (string)this["DATA7"];
            }
            set
            {
                this["DATA7"] = value;
            }
        }
        public string DATA6
        {
            get
            {
                return (string)this["DATA6"];
            }
            set
            {
                this["DATA6"] = value;
            }
        }
        public string DATA5
        {
            get
            {
                return (string)this["DATA5"];
            }
            set
            {
                this["DATA5"] = value;
            }
        }
        public string DATA4
        {
            get
            {
                return (string)this["DATA4"];
            }
            set
            {
                this["DATA4"] = value;
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
        public string LOGTYPE
        {
            get
            {
                return (string)this["LOGTYPE"];
            }
            set
            {
                this["LOGTYPE"] = value;
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
        public string SNID
        {
            get
            {
                return (string)this["SNID"];
            }
            set
            {
                this["SNID"] = value;
            }
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
    }
    public class R_SN_LOG
    {
        public string ID { get; set; }
        public string SNID { get; set; }
        public string SN { get; set; }
        public string LOGTYPE { get; set; }
        public string DATA1 { get; set; }
        public string DATA2 { get; set; }
        public string DATA3 { get; set; }
        public string DATA4 { get; set; }
        public string DATA5 { get; set; }
        public string DATA6 { get; set; }
        public string DATA7 { get; set; }
        public string DATA8 { get; set; }
        public string DATA9 { get; set; }
        public string FLAG { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
    }
}