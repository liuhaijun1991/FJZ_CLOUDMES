using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.Module.HWT
{

    public class T_R_OBA_TEMP : DataObjectTable
    {
        public T_R_OBA_TEMP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_OBA_TEMP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_OBA_TEMP);
            TableName = "R_OBA_TEMP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int SaveNewRecord(OleExec sfcdb, R_OBA_TEMP temp)
        {
            return sfcdb.ORM.Insertable(temp).ExecuteCommand();
        }

        public bool IsExistByTypeAndValue(OleExec sfcdb, string type, string value)
        {
            return sfcdb.ORM.Queryable<R_OBA_TEMP>().Any(r => r.TYPE == type && r.VALUE == value);
        }

        public List<R_OBA_TEMP> GetListByTypeAndIP(OleExec sfcdb, string type, string ip)
        {
            return sfcdb.ORM.Queryable<R_OBA_TEMP>().Where(r => r.TYPE == type && r.IP == ip).ToList();
        }

        public int DeleteByTypeAndIP(OleExec sfcdb, string type, string ip)
        {
            return sfcdb.ORM.Deleteable<R_OBA_TEMP>().Where(r => r.TYPE == type && r.IP == ip).ExecuteCommand();
        }

        public DataTable GetTableByTypeAndIP(OleExec sfcdb, string type, string ip)
        {
            string sql = $@"select rownum as no,type,value,qty,status,remark  from r_oba_temp where type='{type}' and ip='{ip}'";
            return sfcdb.ExecSelect(sql, null).Tables[0];
        }
    }
    public class Row_R_OBA_TEMP : DataObjectBase
    {
        public Row_R_OBA_TEMP(DataObjectInfo info) : base(info)
        {

        }
        public R_OBA_TEMP GetDataObject()
        {
            R_OBA_TEMP DataObject = new R_OBA_TEMP();
            DataObject.ID = this.ID;
            DataObject.LOT = this.LOT;
            DataObject.TYPE = this.TYPE;
            DataObject.VALUE = this.VALUE;
            DataObject.QTY = this.QTY;
            DataObject.STATUS = this.STATUS;
            DataObject.REMARK = this.REMARK;
            DataObject.IP = this.IP;
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
        public string LOT
        {
            get
            {
                return (string)this["LOT"];
            }
            set
            {
                this["LOT"] = value;
            }
        }
        public string TYPE
        {
            get
            {
                return (string)this["TYPE"];
            }
            set
            {
                this["TYPE"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public string STATUS
        {
            get
            {
                return (string)this["STATUS"];
            }
            set
            {
                this["STATUS"] = value;
            }
        }
        public string REMARK
        {
            get
            {
                return (string)this["REMARK"];
            }
            set
            {
                this["REMARK"] = value;
            }
        }
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
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
    public class R_OBA_TEMP
    {
        public string ID { get; set; }
        public string LOT { get; set; }
        public string TYPE { get; set; }
        public string VALUE { get; set; }
        public double? QTY { get; set; }
        public string STATUS { get; set; }
        public string REMARK { get; set; }
        public string IP { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}