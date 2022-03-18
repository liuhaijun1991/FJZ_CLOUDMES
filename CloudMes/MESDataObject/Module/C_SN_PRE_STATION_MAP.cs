using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SN_PRE_STATION_MAP : DataObjectTable
    {
        public T_C_SN_PRE_STATION_MAP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SN_PRE_STATION_MAP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SN_PRE_STATION_MAP);
            TableName = "C_SN_PRE_STATION_MAP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_SN_PRE_STATION_MAP GetRowByReplacedSnPreAndStation(string ReplacedSnPre, string stationName, OleExec db)
        {
            return db.ORM.Queryable<C_SN_PRE_STATION_MAP>().Where(t => t.REPLACED_SN_PRE == ReplacedSnPre && t.STATION_NAME == stationName).ToList().FirstOrDefault();
        }
        public List<C_SN_PRE_STATION_MAP> GetSNStartBySNStart(string ReplacedSnPre, OleExec db)
        {
            return db.ORM.Queryable<C_SN_PRE_STATION_MAP>().Where(t => t.REPLACED_SN_PRE == ReplacedSnPre).ToList();
        }
    }
    public class Row_C_SN_PRE_STATION_MAP : DataObjectBase
    {
        public Row_C_SN_PRE_STATION_MAP(DataObjectInfo info) : base(info)
        {

        }
        public C_SN_PRE_STATION_MAP GetDataObject()
        {
            C_SN_PRE_STATION_MAP DataObject = new C_SN_PRE_STATION_MAP();
            DataObject.ID = this.ID;
            DataObject.SN_PRE = this.SN_PRE;
            DataObject.REPLACED_SN_PRE = this.REPLACED_SN_PRE;
            DataObject.STATION_NAME = this.STATION_NAME;
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
        public string SN_PRE
        {
            get
            {
                return (string)this["SN_PRE"];
            }
            set
            {
                this["SN_PRE"] = value;
            }
        }
        public string REPLACED_SN_PRE
        {
            get
            {
                return (string)this["REPLACED_SN_PRE"];
            }
            set
            {
                this["REPLACED_SN_PRE"] = value;
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
    public class C_SN_PRE_STATION_MAP
    {
        public string ID { get; set; }
        public string SN_PRE { get; set; }
        public string REPLACED_SN_PRE { get; set; }
        public string STATION_NAME { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}