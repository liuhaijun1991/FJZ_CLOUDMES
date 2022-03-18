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
    public class T_R_MFPRESETWOHEAD : DataObjectTable
    {
        public T_R_MFPRESETWOHEAD(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_MFPRESETWOHEAD(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_MFPRESETWOHEAD);
            TableName = "R_MFPRESETWOHEAD".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_MFPRESETWOHEAD> GetWO(OleExec DB, string Wo)
        {

            return DB.ORM.Queryable<R_MFPRESETWOHEAD>().WhereIF(!Wo.Equals(""), t => t.WO == Wo && (t.CANCELLED == null || t.CANCELLED == "0") && t.SAPFLAG == "3").ToList();
        }
        public List<R_MFPRESETWOHEAD> GetAllCWOList(OleExec DB)
        {

            return DB.ORM.Queryable<R_MFPRESETWOHEAD>().Where(t =>(t.CANCELLED==null || t.CANCELLED=="0") && t.SAPFLAG == "3").ToList();

        }

        public void UpdateSAPFLAG(OleExec DB,string wo,string mesg)
        {
            string strSql = $@"Update R_MFPRESETWOHEAD set RETURNMESG=:mesg,EDIT_EMP = 'MESInterface', EDIT_TIME = TO_DATE('" + DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") + "','MM/DD/YYYY HH24:MI:SS') where WO=:wo";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":wo", wo), new OleDbParameter(":mesg", mesg) };
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }

           }
    public class Row_R_MFPRESETWOHEAD : DataObjectBase
    {
        public Row_R_MFPRESETWOHEAD(DataObjectInfo info) : base(info)
        {

        }
        public R_MFPRESETWOHEAD GetDataObject()
        {
            R_MFPRESETWOHEAD DataObject = new R_MFPRESETWOHEAD();
            DataObject.CUSTOMERID = this.CUSTOMERID;
            DataObject.WO = this.WO;
            DataObject.GROUPID = this.GROUPID;
            DataObject.WOQTY = this.WOQTY;
            DataObject.CONFIGID = this.CONFIGID;
            DataObject.PO = this.PO;
            DataObject.POLINE = this.POLINE;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.PID = this.PID;
            DataObject.TOTUNITPRICE = this.TOTUNITPRICE;
            DataObject.WOLEVEL = this.WOLEVEL;
            DataObject.SAPFLAG = this.SAPFLAG;
            DataObject.CONFIGHEADERID = this.CONFIGHEADERID;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CANCELLED = this.CANCELLED;
            DataObject.PRODUCTION_TYPE = this.PRODUCTION_TYPE;
            return DataObject;
        }
        public string CUSTOMERID
        {
            get
            {
                return (string)this["CUSTOMERID"];
            }
            set
            {
                this["CUSTOMERID"] = value;
            }
        }
        public string PRODUCTION_TYPE
        {
            get
            {
                return (string)this["PRODUCTION_TYPE"];
            }
            set
            {
                this["PRODUCTION_TYPE"] = value;
            }
        }
        public string WO
        {
            get
            {
                return (string)this["WO"];
            }
            set
            {
                this["WO"] = value;
            }
        }
        public string GROUPID
        {
            get
            {
                return (string)this["GROUPID"];
            }
            set
            {
                this["GROUPID"] = value;
            }
        }
        public string WOQTY
        {
            get
            {
                return (string)this["WOQTY"];
            }
            set
            {
                this["WOQTY"] = value;
            }
        }
        public string CONFIGID
        {
            get
            {
                return (string)this["CONFIGID"];
            }
            set
            {
                this["CONFIGID"] = value;
            }
        }
        public string PO
        {
            get
            {
                return (string)this["PO"];
            }
            set
            {
                this["PO"] = value;
            }
        }
        public string POLINE
        {
            get
            {
                return (string)this["POLINE"];
            }
            set
            {
                this["POLINE"] = value;
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
        public string PID
        {
            get
            {
                return (string)this["PID"];
            }
            set
            {
                this["PID"] = value;
            }
        }
        public string TOTUNITPRICE
        {
            get
            {
                return (string)this["TOTUNITPRICE"];
            }
            set
            {
                this["TOTUNITPRICE"] = value;
            }
        }
        public string WOLEVEL
        {
            get
            {
                return (string)this["WOLEVEL"];
            }
            set
            {
                this["WOLEVEL"] = value;
            }
        }
        public string SAPFLAG
        {
            get
            {
                return (string)this["SAPFLAG"];
            }
            set
            {
                this["SAPFLAG"] = value;
            }
        }
        public string CONFIGHEADERID
        {
            get
            {
                return (string)this["CONFIGHEADERID"];
            }
            set
            {
                this["CONFIGHEADERID"] = value;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return (string)this["DESCRIPTION"];
            }
            set
            {
                this["DESCRIPTION"] = value;
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
        public string CANCELLED
        {
            get
            {
                return (string)this["CANCELLED"];
            }
            set
            {
                this["CANCELLED"] = value;
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
        public string EDIT_TIME
        {
            get
            {
                return (string)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_MFPRESETWOHEAD
    {
        public string CUSTOMERID { get; set; }
        public string WO { get; set; }
        public string GROUPID { get; set; }
        public string WOQTY { get; set; }
        public string CONFIGID { get; set; }
        public string PO { get; set; }
        public string POLINE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string PID { get; set; }
        public string TOTUNITPRICE { get; set; }
        public string WOLEVEL { get; set; }
        public string SAPFLAG { get; set; }
        public string CONFIGHEADERID { get; set; }
        public string DESCRIPTION { get; set; }
        public string SKUNO { get; set; }
        public string CANCELLED { get; set; }
        public string EDIT_EMP { get; set; }

        public string EDIT_TIME { get; set; }
        public string PRODUCTION_TYPE { get; set; }


        public int CompareTo(R_MFPRESETWOHEAD other)
        {


            if (this.WO == other.WO)
            {
                return 0;
            }
            if (this.SAPFLAG == other.SAPFLAG && this.CANCELLED == other.CANCELLED)
            {
                return 0;
            }
            return 1;
            // throw new NotImplementedException();
        }
    }
}