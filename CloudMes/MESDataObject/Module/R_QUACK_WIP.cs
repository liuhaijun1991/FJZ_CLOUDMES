using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_QUACK_WIP : DataObjectTable
    {
        public T_R_QUACK_WIP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_QUACK_WIP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_QUACK_WIP);
            TableName = "R_QUACK_WIP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int InsertRowToRQuackWip(R_QUACK_WIP RQuackWip, OleExec db)
        {
            return db.ORM.Insertable(RQuackWip).ExecuteCommand();
        }

        public int UpdateRowsByStationAndSkuno(R_QUACK_WIP RQuackWip,string StationName,string Skuno, OleExec db)
        {
            return db.ORM.Updateable(RQuackWip).Where(t => t.STATION_NAME==StationName && t.SKUNO == Skuno).ExecuteCommand();
        }

        public void SfcRQuackWipSP(string StationName,string Skuno,string Bu,OleExec db, DB_TYPE_ENUM DBType)
        {
            if (StationName== "QUACK")
            {
                db.ORM.Updateable<R_QUACK_WIP>().UpdateColumns(t => new R_QUACK_WIP { PASS_QTY = t.PASS_QTY - 1, EDIT_TIME = this.GetDBDateTime(db) }).Where(t => t.STATION_NAME == "QUACK" && t.SKUNO == "").ExecuteCommand();
            }
            if (StationName == "repairrw")
            {
                db.ORM.Updateable<R_QUACK_WIP>().UpdateColumns(t => new R_QUACK_WIP { PASS_QTY = t.PASS_QTY - 1, EDIT_TIME = this.GetDBDateTime(db) }).Where(t => t.STATION_NAME == "repairrw" && t.SKUNO == Skuno).ExecuteCommand();
            }
            if (StationName == "MRBRW")
            {
                db.ORM.Updateable<R_QUACK_WIP>().UpdateColumns(t => new R_QUACK_WIP { PASS_QTY = t.PASS_QTY - 1, EDIT_TIME = this.GetDBDateTime(db) }).Where(t => t.STATION_NAME == "MRBRW" && t.SKUNO == Skuno).ExecuteCommand();
            }
            if (StationName == "RMARW")
            {
                db.ORM.Updateable<R_QUACK_WIP>().UpdateColumns(t => new R_QUACK_WIP { PASS_QTY = t.PASS_QTY - 1, EDIT_TIME = this.GetDBDateTime(db) }).Where(t => t.STATION_NAME == "RMARW" && t.SKUNO == Skuno).ExecuteCommand();
            }
            T_R_SN RSnTable = new T_R_SN(db, DBType);
            T_C_ROUTE CRouteTable = new T_C_ROUTE(db, DBType);
            C_ROUTE CRoute = CRouteTable.GetRouteBySkuno(Skuno, db);
            string NextStationName = RSnTable.GetNextStation(CRoute.ID,"QUACK",db);
            if (NextStationName== "QUACK")
            {
                NextStationName = "B23F";
                if (db.ORM.Queryable<R_QUACK_WIP>().Where(t=>t.STATION_NAME==NextStationName&&t.SKUNO==Skuno).ToList().Count<=0)
                {
                }
            }
        }
    }
    public class Row_R_QUACK_WIP : DataObjectBase
    {
        public Row_R_QUACK_WIP(DataObjectInfo info) : base(info)
        {

        }
        public R_QUACK_WIP GetDataObject()
        {
            R_QUACK_WIP DataObject = new R_QUACK_WIP();
            DataObject.ID = this.ID;
            DataObject.SKUNO = this.SKUNO;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.PASS_QTY = this.PASS_QTY;
            DataObject.FAIL_QTY = this.FAIL_QTY;
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
        public string STATION_NAME
        {
            get
            {
                return (string)this["STATION_NAME"];
            }
            set
            {
                this["STATION_NAME"] = value;
            }
        }
        public double? PASS_QTY
        {
            get
            {
                return (double?)this["PASS_QTY"];
            }
            set
            {
                this["PASS_QTY"] = value;
            }
        }
        public double? FAIL_QTY
        {
            get
            {
                return (double?)this["FAIL_QTY"];
            }
            set
            {
                this["FAIL_QTY"] = value;
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
    public class R_QUACK_WIP
    {
        public string ID { get; set; }
        public string SKUNO { get; set; }
        public string STATION_NAME { get; set; }
        public double? PASS_QTY { get; set; }
        public double? FAIL_QTY { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}