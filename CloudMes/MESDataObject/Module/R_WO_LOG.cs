using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WO_LOG : DataObjectTable
    {
        public T_R_WO_LOG(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_LOG(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WO_LOG);
            TableName = "R_WO_LOG".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public bool CheckKpSetCount(string Workorderno, OleExec DB)
        {
            bool checkLog = DB.ORM.Queryable<R_WO_LOG>().Where(it => it.WORKORDERNO == Workorderno && it.FLAG == "1").Any();
          
            int COUNT1 = DB.ORM.Queryable<R_WO_ITEM, R_WO_HEADER, C_KP_LIST, C_KP_List_Item>
                ((a, b, c, d)
                => a.MATNR == d.KP_PARTNO
                && d.LIST_ID == c.ID
                && c.SKUNO == b.MATNR
                && b.AUFNR == a.AUFNR)
                .Where((a, b, c, d) => a.AUFNR == Workorderno && c.FLAG == "1")
                .GroupBy(a => a.MATNR).Select(a => a.MATNR).ToList().Count;

            int COUNT2 = DB.ORM.Queryable<R_WO_HEADER, C_KP_LIST, C_KP_List_Item>
                ((a, b, c)
                => a.MATNR == b.SKUNO
                && c.LIST_ID == b.ID)
                .Where((a, b, c) => a.AUFNR == Workorderno && b.FLAG == "1")
                .GroupBy((a, b, c) => c.KP_PARTNO).Select((a, b, c) => c.KP_PARTNO).ToList().Count;

            string Skuno = DB.ORM.Queryable<R_WO_HEADER>().Where(it => it.AUFNR == Workorderno).Select(it => it.MATNR).First();
            string lastSnID = DB.ORM.Queryable<R_SN>().Where(it => it.SKUNO == Skuno && it.VALID_FLAG == "1").OrderBy(it => it.START_TIME, SqlSugar.OrderByType.Desc).Select(it => it.ID).First();

            int COUNT3 = DB.ORM.Queryable<R_SN_KP>().Where(it => it.R_SN_ID == lastSnID && it.VALID_FLAG == 1).GroupBy(it => it.PARTNO).Select(it => it.PARTNO).ToList().Count;

            #region QE胡航州要求在MES系統的function control user config增加NOT CHECK BOM功能 wuqing 20201111
            List<string> kp = DB.ORM.Queryable<R_F_CONTROL>().Where(p => p.FUNCTIONNAME == "NOT_CHECK_BOM" &&p.VALUE== Skuno).Select(p=>p.EXTVAL).ToList();
            int COUNT4= DB.ORM.Queryable<R_WO_HEADER, C_KP_LIST, C_KP_List_Item>
                ((a, b, c)
                => a.MATNR == b.SKUNO
                && c.LIST_ID == b.ID)
                .Where((a, b, c) => a.AUFNR == Workorderno && b.FLAG == "1" && !kp.Contains(c.KP_PARTNO) )
                .GroupBy((a, b, c) => c.KP_PARTNO).Select((a, b, c) => c.KP_PARTNO).ToList().Count;
            #endregion

            if ((COUNT1 == COUNT2 && COUNT3 == COUNT2) || checkLog||(COUNT1== COUNT4&& COUNT3 == COUNT2)) return true;
            else return false;
        }

    }
    public class Row_R_WO_LOG : DataObjectBase
    {
        public Row_R_WO_LOG(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_LOG GetDataObject()
        {
            R_WO_LOG DataObject = new R_WO_LOG();
            DataObject.ID = this.ID;
            DataObject.FUNCTIONNAME = this.FUNCTIONNAME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.VERSION = this.VERSION;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.REASON = this.REASON;
            DataObject.FLAG = this.FLAG;
            DataObject.DATA1 = this.DATA1;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string FUNCTIONNAME
        {
            get
            {
                return (string)this["FUNCTIONNAME"];
            }
            set
            {
                this["FUNCTIONNAME"] = value;
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
        public string VERSION
        {
            get
            {
                return (string)this["VERSION"];
            }
            set
            {
                this["VERSION"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string REASON
        {
            get
            {
                return (string)this["REASON"];
            }
            set
            {
                this["REASON"] = value;
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
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
            }
        }
    }
    public class R_WO_LOG
    {
        public string ID { get; set; }
        public string FUNCTIONNAME { get; set; }
        public string SKUNO { get; set; }
        public string VERSION { get; set; }
        public string WORKORDERNO { get; set; }
        public string REASON { get; set; }
        public string FLAG { get; set; }
        public string DATA1 { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }
}