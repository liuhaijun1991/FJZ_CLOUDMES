using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_WEIGHT : DataObjectTable
    {
        public T_R_WEIGHT(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WEIGHT(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_WEIGHT);
            TableName = "R_WEIGHT".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public string GetallCTNweight(string StrPackno, string BU, OleExec sfcdb)
        {
            string Strweights = "0";
            DataTable dt = new DataTable();

            string sql = $@"SELECT sum(weight) as PLWeight FROM r_weight WHERE SNID in(
            SELECT ID FROM r_packing WHERE parent_pack_id in(
            SELECT ID FROM r_packing WHERE pack_no ='{StrPackno}'))";
            dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Strweights = dt.Rows[0]["PLWeight"].ToString();
                return Strweights;
            }
            else
            {
                return Strweights;
            }
        }
    }
    public class Row_R_WEIGHT : DataObjectBase
    {
        public Row_R_WEIGHT(DataObjectInfo info) : base(info)
        {

        }
        public R_WEIGHT GetDataObject()
        {
            R_WEIGHT DataObject = new R_WEIGHT();
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.WEIGHT = this.WEIGHT;
            DataObject.STATION = this.STATION;
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
        public string WEIGHT
        {
            get
            {
                return (string)this["WEIGHT"];
            }
            set
            {
                this["WEIGHT"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
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
    public class R_WEIGHT
    {
        public string CREATEBY { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string WEIGHT { get; set; }
        public string STATION { get; set; }
        public string SNID { get; set; }
        public string ID { get; set; }
    }
}