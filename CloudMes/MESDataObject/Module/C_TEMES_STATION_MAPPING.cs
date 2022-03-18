using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_TEMES_STATION_MAPPING : DataObjectTable
    {
        public T_C_TEMES_STATION_MAPPING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_TEMES_STATION_MAPPING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_TEMES_STATION_MAPPING);
            TableName = "C_TEMES_STATION_MAPPING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_TEMES_STATION_MAPPING GetTeMesStationMapping(OleExec DB, string TeStation,string TeGroup)
        {
            C_TEMES_STATION_MAPPING RES = new C_TEMES_STATION_MAPPING();
            T_C_TEMES_STATION_MAPPING tabel = new T_C_TEMES_STATION_MAPPING(DB, DB_TYPE_ENUM.Oracle);
            Row_C_TEMES_STATION_MAPPING row = (Row_C_TEMES_STATION_MAPPING)tabel.NewRow();
            string sql = $@" SELECT * FROM C_TEMES_STATION_MAPPING WHERE TEGROUP=:TEGROUP AND TE_STATION=:TE_STATION ";
            OleDbParameter[] parameters = new OleDbParameter[]
            {
                new OleDbParameter("TEGROUP",OleDbType.VarChar,10),
                new OleDbParameter("TE_STATION",OleDbType.VarChar,30),
            };
            parameters[0].Value = TeGroup;
            parameters[1].Value = TeStation;
            DataTable dt = DB.ExecSelect(sql, parameters).Tables[0];
            if (dt.Rows.Count > 0)
            {
                row.loadData(dt.Rows[0]);
                RES = row.GetDataObject();
            }
            return RES;
        }
    }
    public class Row_C_TEMES_STATION_MAPPING : DataObjectBase
    {
        public Row_C_TEMES_STATION_MAPPING(DataObjectInfo info) : base(info)
        {

        }
        public C_TEMES_STATION_MAPPING GetDataObject()
        {
            C_TEMES_STATION_MAPPING DataObject = new C_TEMES_STATION_MAPPING();
            DataObject.MES_STATION = this.MES_STATION;
            DataObject.TE_STATION = this.TE_STATION;
            DataObject.TEGROUP = this.TEGROUP;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string MES_STATION
        {
            get
            {
                return (string)this["MES_STATION"];
            }
            set
            {
                this["MES_STATION"] = value;
            }
        }
        public string TE_STATION
        {
            get
            {
                return (string)this["TE_STATION"];
            }
            set
            {
                this["TE_STATION"] = value;
            }
        }
        public string TEGROUP
        {
            get
            {
                return (string)this["TEGROUP"];
            }
            set
            {
                this["TEGROUP"] = value;
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
    public class C_TEMES_STATION_MAPPING
    {
        public string MES_STATION{get;set;}
        public string TE_STATION{get;set;}
        public string TEGROUP{get;set;}
        public string ID{get;set;}
    }
}