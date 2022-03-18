using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_2D5D_WIP_QUERY : DataObjectTable
    {
        public T_R_2D5D_WIP_QUERY(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_2D5D_WIP_QUERY(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_2D5D_WIP_QUERY);
            TableName = "R_2D5D_WIP_QUERY".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_2D5D_WIP_QUERY> GetSNBYCheckOutSN(string SN, OleExec DB)
        {
            string StrSql = $@"SELECT * FROM R_2D5D_WIP_QUERY WHERE SN ='{SN}'";
            DataTable DT = DB.ExecSelect(StrSql).Tables[0];
            List<R_2D5D_WIP_QUERY> ListR2D5DWIPQuery = new List<R_2D5D_WIP_QUERY>();
            if (DT.Rows.Count > 0)
            {
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    Row_R_2D5D_WIP_QUERY RowR2D5DWIPQuery = (Row_R_2D5D_WIP_QUERY)NewRow();
                    RowR2D5DWIPQuery.loadData(DT.Rows[i]);
                    ListR2D5DWIPQuery.Add(RowR2D5DWIPQuery.GetDataObject());
                }

            }
            return ListR2D5DWIPQuery;
        }
        public int UpdateR2D5DWIPQYERY(R_2D5D_WIP_QUERY R2D5DWIPQUERY, OleExec DB)
        {
            return DB.ORM.Updateable<R_2D5D_WIP_QUERY>(R2D5DWIPQUERY).Where(t => t.SN == R2D5DWIPQUERY.SN).ExecuteCommand();
        }

        public int InsertR2D5DWIPQYERY(R_2D5D_WIP_QUERY R2D5DWIPQUERY, OleExec DB)
        {
            return DB.ORM.Insertable<R_2D5D_WIP_QUERY>(R2D5DWIPQUERY).ExecuteCommand();
        }
    }
    public class Row_R_2D5D_WIP_QUERY : DataObjectBase
    {
        public Row_R_2D5D_WIP_QUERY(DataObjectInfo info) : base(info)
        {

        }
        public R_2D5D_WIP_QUERY GetDataObject()
        {
            R_2D5D_WIP_QUERY DataObject = new R_2D5D_WIP_QUERY();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.WO = this.WO;
            DataObject.SKUNO = this.SKUNO;
            DataObject.CURRENT_STATION = this.CURRENT_STATION;
            DataObject.LOCATION = this.LOCATION;
            DataObject.DESCRIPTION = this.DESCRIPTION;
            DataObject.CHECKIN_FLAG = this.CHECKIN_FLAG;
            DataObject.CHECKIN_TIME = this.CHECKIN_TIME;
            DataObject.CHECKOUT_FLAG = this.CHECKOUT_FLAG;
            DataObject.CHECKOUT_TIME = this.CHECKOUT_TIME;
            DataObject.TRACK_NO = this.TRACK_NO;
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
        public string CURRENT_STATION
        {
            get
            {
                return (string)this["CURRENT_STATION"];
            }
            set
            {
                this["CURRENT_STATION"] = value;
            }
        }
        public string LOCATION
        {
            get
            {
                return (string)this["LOCATION"];
            }
            set
            {
                this["LOCATION"] = value;
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
        public string CHECKIN_FLAG
        {
            get
            {
                return (string)this["CHECKIN_FLAG"];
            }
            set
            {
                this["CHECKIN_FLAG"] = value;
            }
        }
        public DateTime? CHECKIN_TIME
        {
            get
            {
                return (DateTime?)this["CHECKIN_TIME"];
            }
            set
            {
                this["CHECKIN_TIME"] = value;
            }
        }
        public string CHECKOUT_FLAG
        {
            get
            {
                return (string)this["CHECKOUT_FLAG"];
            }
            set
            {
                this["CHECKOUT_FLAG"] = value;
            }
        }
        public DateTime? CHECKOUT_TIME
        {
            get
            {
                return (DateTime?)this["CHECKOUT_TIME"];
            }
            set
            {
                this["CHECKOUT_TIME"] = value;
            }
        }
        public string TRACK_NO
        {
            get
            {
                return (string)this["TRACK_NO"];
            }
            set
            {
                this["TRACK_NO"] = value;
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
    public class R_2D5D_WIP_QUERY
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string WO { get; set; }
        public string SKUNO { get; set; }
        public string CURRENT_STATION { get; set; }
        public string LOCATION { get; set; }
        public string DESCRIPTION { get; set; }
        public string CHECKIN_FLAG { get; set; }
        public DateTime? CHECKIN_TIME { get; set; }
        public string CHECKOUT_FLAG { get; set; }
        public DateTime? CHECKOUT_TIME { get; set; }
        public string TRACK_NO { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}