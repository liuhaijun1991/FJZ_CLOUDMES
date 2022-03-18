using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Threading.Tasks;
using MESDBHelper;
using SqlSugar;

namespace MESDataObject.Module
{
    public class T_R_TEST_DETAIL_VERTIV : DataObjectTable
    {
        public T_R_TEST_DETAIL_VERTIV(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_TEST_DETAIL_VERTIV(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_TEST_DETAIL_VERTIV);
            TableName = "R_TEST_DETAIL_VERTIV".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public List<R_TEST_DETAIL_VERTIV> GetRTestDetailVertivBySn(OleExec DB, string sn)
        {
            List<R_TEST_DETAIL_VERTIV> res = new List<R_TEST_DETAIL_VERTIV>();
            string sql = $@" select * from R_TEST_DETAIL_VERTIV where SN==:SN ";
            OleDbParameter[] paras = new OleDbParameter[]
            {
        new OleDbParameter("SN",OleDbType.VarChar,100)
            };
            paras[0].Value = sn;
            DataTable dt = DB.ExecSelect(sql, paras).Tables[0];
            foreach (DataRow VARIABLE in dt.Rows)
            {
                Row_R_TEST_DETAIL_VERTIV row = (Row_R_TEST_DETAIL_VERTIV)this.NewRow();
                row.loadData(VARIABLE);
                res.Add(row.GetDataObject());
            }
            return res;
        }

        public DataTable GetDTRTestDetailVertivBySn(OleExec DB, string sn)
        {
            List<R_TEST_DETAIL_VERTIV> res = new List<R_TEST_DETAIL_VERTIV>();
            string sql = $@" select * from R_TEST_DETAIL_VERTIV where SN=:SN ";
            OleDbParameter[] paras = new OleDbParameter[]
            {
        new OleDbParameter("SN",OleDbType.VarChar,100)
            };
            paras[0].Value = sn;
            DataTable dt = DB.ExecSelect(sql, paras).Tables[0];
            return dt;
        }

        public R_TEST_DETAIL_VERTIV GetTestByOrtSn(OleExec DB, string sn,string stationname)
        {
            R_TEST_DETAIL_VERTIV R_Sn_Test_Detail = null;
            Row_R_TEST_DETAIL_VERTIV Rows = (Row_R_TEST_DETAIL_VERTIV)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = string.Empty;

            StrSql = $@" SELECT * FROM r_test_detail_vertiv WHERE SN='{sn}'and station='{stationname}'";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Test_Detail = Rows.GetDataObject();
            }
            return R_Sn_Test_Detail;
        }

        public R_TEST_DETAIL_VERTIV GETCheckSnORTTestTime(OleExec DB, string sn,string stationname)
        {
            R_TEST_DETAIL_VERTIV R_Sn_Test_Detail = null;
            Row_R_TEST_DETAIL_VERTIV Rows = (Row_R_TEST_DETAIL_VERTIV)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = string.Empty;
            string FT2SCANTIME = string.Empty;

            var ORTtestData = DB.ORM.Queryable<R_TEST_DETAIL_VERTIV>().Where(t => t.SN == sn && t.STATE == "PASS" && t.STATION =="ORT")
                .OrderBy(t => t.CREATETIME, OrderByType.Desc).ToList().FirstOrDefault();
            var FIStationExists = DB.ORM.Queryable<R_SN, C_ROUTE_DETAIL>((rs, crd) => rs.ROUTE_ID == crd.ROUTE_ID)
                .Where((rs, crd) => rs.SN == sn && rs.VALID_FLAG == "1" && crd.STATION_NAME == "FI")
                .Select((rs, crd) => rs).Any();
            var sneventRecord = DB.ORM.Queryable<R_SN_STATION_DETAIL>()
                .Where(t => t.SN == sn && "RETURN,FI".Contains(t.STATION_NAME)).OrderBy(t => t.EDIT_TIME, OrderByType.Desc)
                .ToList();
            var snobj = DB.ORM.Queryable<R_SN>().Where(t => t.SN == sn && t.VALID_FLAG == "1").ToList()
                .FirstOrDefault();
            var returnRecord = sneventRecord.FindAll(t => t.STATION_NAME == "RETURN").FirstOrDefault();
            var FiRerecord = sneventRecord.FindAll(t => t.STATION_NAME == "FI").FirstOrDefault();
            if (ORTtestData == null)
                return ORTtestData;
            if (FIStationExists)
            {
                if (FiRerecord == null || FiRerecord.EDIT_TIME >= ORTtestData.CREATETIME)
                    return null;
            }
            if (snobj.START_TIME > ORTtestData.CREATETIME ||
                     (returnRecord != null && ORTtestData.EDIT_TIME < returnRecord.EDIT_TIME))
                return null;

            return ORTtestData;

            //if (DB.ORM.Queryable<R_SN, C_ROUTE_DETAIL>((rs, crd) => rs.ROUTE_ID == crd.ROUTE_ID)
            //    .Where((rs, crd) => rs.SN == sn && rs.VALID_FLAG == "1" && crd.STATION_NAME == "FI").Select((rs, crd) =>rs).Any())
            //    StrSql =
            //        $@"select * from r_test_detail_vertiv a where a.station='{stationname}'and a.state='PASS' and a.sn='{sn}' and a.createtime>(
            //           select max(b.edit_time) from r_sn_station_detail b where b.sn='{sn}' and b.station_name='FI')";
            //else
            //    StrSql =
            //        $@"select * from r_test_detail_vertiv a where a.station='{stationname}'and a.state='PASS' and a.sn='{sn}' and a.createtime>(
            //           select max(b.edit_time) from r_sn_station_detail b where b.sn='{sn}' and b.station_name='FI')";
            //Dt = DB.ExecSelect(StrSql).Tables[0];
            //if (Dt.Rows.Count > 0)
            //{
            //    Rows.loadData(Dt.Rows[0]);
            //    R_Sn_Test_Detail = Rows.GetDataObject();
            //}

            //return R_Sn_Test_Detail;
        }

        public R_TEST_DETAIL_VERTIV CheckORT24HOrNot(OleExec DB, string sn,string stationname)
        {
            R_TEST_DETAIL_VERTIV R_Sn_Test_Detail = null;
            Row_R_TEST_DETAIL_VERTIV Rows = (Row_R_TEST_DETAIL_VERTIV)NewRow();
            DataTable Dt = new DataTable();
            string StrSql = string.Empty;
            
            StrSql = $@"select cast((rr.TESTTIME1-tt.TESTTIME2)*24 as decimal(18,2)) AS HH from 
                        (select STARTTIME as TESTTIME1 ,sn from  (select * From r_test_record aa where aa.sn='{sn}' 
                        and aa.testation='{stationname}'and aa.State in('PASS','FAIL') order by aa.starttime desc )b where  rownum=1 )rr ,
                        (select STARTTIME AS TESTTIME2,sn from  (select * From r_test_record aa where aa.sn='{sn}' 
                        and aa.State='R'and aa.testation='{stationname}' order by aa.starttime desc )a where  rownum=1)tt  where rr.sn=tt.sn ";
            Dt = DB.ExecSelect(StrSql).Tables[0];
            Decimal sas = (Decimal)Dt.Rows[0]["HH"];
            if (sas >= 1)//將24H測試時間轉化為小時
            {
                Rows.loadData(Dt.Rows[0]);
                R_Sn_Test_Detail = Rows.GetDataObject();
            }

            return R_Sn_Test_Detail;
        }

        public List<R_TEST_DETAIL_VERTIV> GetTestStationQtyByWo(string wo, string station, OleExec DB)
        {
            return DB.ORM.Queryable<R_TEST_DETAIL_VERTIV>().Where(t => t.STATION == station && SqlSugar.SqlFunc.Subqueryable<R_SN>()
            .Where(s => s.SN == t.SN && s.WORKORDERNO == wo).Any()).ToList();
        }

    }
    public class Row_R_TEST_DETAIL_VERTIV : DataObjectBase
    {
        public Row_R_TEST_DETAIL_VERTIV(DataObjectInfo info) : base(info)
        {

        }
        public R_TEST_DETAIL_VERTIV GetDataObject()
        {
            R_TEST_DETAIL_VERTIV DataObject = new R_TEST_DETAIL_VERTIV();
            DataObject.ID = this.ID;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.ERROR_CODE = this.ERROR_CODE;
            DataObject.OPERATOR = this.OPERATOR;
            DataObject.CELL = this.CELL;
            DataObject.STATION = this.STATION;
            DataObject.STATE = this.STATE;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SN = this.SN;
            DataObject.R_TEST_RECORD_ID = this.R_TEST_RECORD_ID;
            DataObject.BURNIN_TIME = this.BURNIN_TIME;
            DataObject.LINE = this.LINE;
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
        public string ERROR_CODE
        {
            get
            {
                return (string)this["ERROR_CODE"];
            }
            set
            {
                this["ERROR_CODE"] = value;
            }
        }
        public string OPERATOR
        {
            get
            {
                return (string)this["OPERATOR"];
            }
            set
            {
                this["OPERATOR"] = value;
            }
        }
        public string CELL
        {
            get
            {
                return (string)this["CELL"];
            }
            set
            {
                this["CELL"] = value;
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
        public string STATE
        {
            get
            {
                return (string)this["STATE"];
            }
            set
            {
                this["STATE"] = value;
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
        public string R_TEST_RECORD_ID
        {
            get
            {
                return (string)this["R_TEST_RECORD_ID"];
            }
            set
            {
                this["R_TEST_RECORD_ID"] = value;
            }
        }
        public string BURNIN_TIME
        {
            get
            {
                return (string)this["BURNIN_TIME"];
            }
            set
            {
                this["BURNIN_TIME"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
            }
        }
    }
    public class R_TEST_DETAIL_VERTIV
    {
        public string ID { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public string ERROR_CODE { get; set; }
        public string OPERATOR { get; set; }
        public string CELL { get; set; }
        public string STATION { get; set; }
        public string STATE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string SKUNO { get; set; }
        public string SN { get; set; }
        public string R_TEST_RECORD_ID { get; set; }
        public string BURNIN_TIME { get; set; }
        public string LINE { get; set; }
    }
}