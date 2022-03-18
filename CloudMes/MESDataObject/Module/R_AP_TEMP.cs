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
    public class T_R_AP_TEMP : DataObjectTable
    {
        public T_R_AP_TEMP(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_AP_TEMP(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_AP_TEMP);
            TableName = "R_AP_TEMP".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public void InitApEmp(OleExec DB, string Station_no, string emp_no)
        {
            string sql = $@"  Insert into R_AP_TEMP
                                         (DATA1, DATA2, DATA3, DATA4, DATA5, 
                                          DATA6, DATA7, WORK_TIME)
                                   Values
                                         ('SCADA-GW28', :Station_no, 1, 'EMP', :EMP, 
                                          'S/N', 0,sysdate) ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no), new OleDbParameter(":EMP", emp_no) };
            DB.ExecuteNonQuery(sql, CommandType.Text, paramet);
        }

        public void InitApLine(OleExec DB, string Station_no, string line_name)
        {
            string sql = $@"  Insert into R_AP_TEMP
                                         (DATA1, DATA2, DATA3, DATA4, DATA5, 
                                          DATA6, DATA7, WORK_TIME)
                                   Values
                                         ('SCADA-GW28', :Station_no, 2, 'LINE_NAME', :LINE_NAME, 
                                          'S/N', 0,sysdate) ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no), new OleDbParameter(":LINE_NAME", line_name) };
            DB.ExecuteNonQuery(sql, CommandType.Text, paramet);
        }

        public void InitApStation(OleExec DB, string Station_no, string S_name)
        {
            string sql = $@"  Insert into R_AP_TEMP
                                         (DATA1, DATA2, DATA3, DATA4, DATA5, 
                                          DATA6, DATA7, WORK_TIME)
                                   Values
                                         ('SCADA-GW28', :Station_no, 3, 'STATION_NAME', :S_name, 
                                          'S/N', 0,sysdate) ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no), new OleDbParameter(":S_name", S_name) };
            DB.ExecuteNonQuery(sql, CommandType.Text, paramet);
        }

        public string GetApNextInput(OleExec DB, DB_TYPE_ENUM DBType, string Station_no)
        {
            string sql = $@" select * from r_ap_temp where data2='{Station_no}' and DATA3 in (select max(DATA3) from r_ap_temp where data2='{Station_no}') ";
            DataTable dt = DB.ExecSelect(sql).Tables[0];

            return dt.Rows[0]["DATA4"].ToString();
        }

        public R_AP_TEMP GetMaxByStation_no(OleExec DB, string Station_no)
        {
            string sql = $@" select * from r_ap_temp where data2='{Station_no}' and data3 in (select max(data3) from r_ap_temp where data2='{Station_no}') ";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            R_AP_TEMP ap = new R_AP_TEMP();
            ap.DATA1 = dt.Rows[0]["DATA1"].ToString();
            ap.DATA2 = dt.Rows[0]["DATA2"].ToString();
            ap.DATA3 = Convert.ToDouble(dt.Rows[0]["DATA3"].ToString());
            ap.DATA4 = dt.Rows[0]["DATA4"].ToString();
            ap.DATA5 = dt.Rows[0]["DATA5"].ToString();
            ap.DATA6 = dt.Rows[0]["DATA6"].ToString();
            ap.DATA7 = Convert.ToDouble(dt.Rows[0]["DATA7"].ToString());
            ap.DATA8 = dt.Rows[0]["DATA8"].ToString();
            ap.DATA9 = dt.Rows[0]["DATA9"].ToString();
            ap.DATA10 = dt.Rows[0]["DATA10"].ToString();
            return ap;
        }

        public List<R_AP_TEMP> GetRecordByStationNo(OleExec DB, string Station_no)
        {
            string sql = $@"select DATA1,DATA2,to_char(DATA3) DATA3,DATA4,DATA5,DATA6,to_char(DATA7) DATA7,DATA8,DATA9,DATA10
                              from r_ap_temp where data2='{Station_no}' order by data3 asc";
            DataSet res = DB.ExecSelect(sql);
            List<R_AP_TEMP> retlist = res.Tables[0].AsEnumerable()
                .Select(t => new R_AP_TEMP
                {
                    DATA1 = t.Field<string>("DATA1"),
                    DATA2 = t.Field<string>("DATA2"),
                    DATA3 = Convert.ToDouble(t.Field<string>("DATA3")),
                    DATA4 = t.Field<string>("DATA4"),
                    DATA5 = t.Field<string>("DATA5"),
                    DATA6 = t.Field<string>("DATA6"),
                    DATA7 = Convert.ToDouble(t.Field<string>("DATA7")),
                    DATA8 = t.Field<string>("DATA8"),
                    DATA9 = t.Field<string>("DATA9"),
                    DATA10 = t.Field<string>("DATA10")
                }).ToList();
            return retlist;
            /*
            return DB.ORM.Queryable<R_AP_TEMP>()
                .Where(t => t.DATA2 == Station_no)
                .OrderBy(t => t.DATA3).ToList();
           */
        }

        public Row_R_AP_TEMP GetMaxRecordByStationNo(OleExec DB, string Station_no)
        {

            string sql = $@"select * from r_ap_temp where data2='{Station_no}' and data3 in (select max(data3) from r_ap_temp where data2='{Station_no}')";
            DataSet res = DB.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                Row_R_AP_TEMP ret = (Row_R_AP_TEMP)NewRow();
                ret.loadData(res.Tables[0].Rows[0]);
                return ret;
            }
            else
            {
                return null;
            }
        }

        public string GetWorkorderNo(OleExec DB, string Station_no)
        {
            string sql = $@" select * from r_ap_temp where data2='{Station_no}' and data8='W/O' and data6='W/O'";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["DATA5"].ToString();
            else
                return "";
        }
        public string GetSysserialNo(OleExec DB, string Station_no)
        {
            string sql = $@" select * from r_ap_temp where data2='{Station_no}' and data9='S/N'";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["DATA5"].ToString();
            else
                return "";
        }

        public void UpdateApStation(OleExec DB, string Station_no, string Station)
        {
            string strSql = $@" UPDATE R_AP_TEMP SET DATA5=:Station WHERE DATA2=:Station_no and DATA4='STATION_NAME' ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no),
                                                              new OleDbParameter(":Station", Station) };
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }

        public void UpdataApScanTypeStation(OleExec DB, string Station_no, double? Eseqno, string Currentscantype, string Nextscantype)
        {
            string strSql = $@" UPDATE R_AP_TEMP SET DATA9=:CURRENTSCAN,DATA10=:NEXTSCAN WHERE DATA2=:Station_no and DATA3=:Eseqno ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no),
                                                              new OleDbParameter(":Eseqno", Eseqno),
                                                              new OleDbParameter(":CURRENTSCAN", Currentscantype),
                                                              new OleDbParameter(":NEXTSCAN", Nextscantype)};
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }

        public string GetSfcStation(OleExec DB, string Station_no)
        {
            string sql = $@" select * from r_ap_temp where data2='{Station_no}' and data4='STATION_NAME'";
            DataTable dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
                return dt.Rows[0]["DATA5"].ToString();
            else
                return "";
        }

        public void SetApNextInput(OleExec DB, string Station_no, double? Eseqno, string Ds_name, string Value, string NEXT)
        {
            string sql = $@"  Insert into R_AP_TEMP
                                         (DATA1, DATA2, DATA3, DATA4, DATA5, 
                                          DATA6, DATA7, WORK_TIME)
                                   Values
                                         ('SCADA-GW28', :Station_no,:Eseqno, :Ds_name, :Value, 
                                          :NEXT,'0',sysdate) ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no), new OleDbParameter(":Eseqno", Eseqno), new OleDbParameter(":Ds_name", Ds_name), new OleDbParameter(":Value", Value), new OleDbParameter(":NEXT", NEXT) };
            DB.ExecuteNonQuery(sql, CommandType.Text, paramet);
        }

        public void InsertApTemp(OleExec DB, string Station_no, double? Station_Seq, string Ds_name, string Value, string ValueType, double Data7, string Data8, string Data9, string Data10)
        {
            string sql = $@" Insert into R_AP_TEMP
                             (DATA1,DATA2,DATA3,DATA4,DATA5,
                              DATA6,DATA7,DATA8,DATA9,DATA10,WORK_TIME) Values
                             ('SCADA-GW28',:Station_no,:Station_Seq,:Ds_name,:Value,
                              :ValueType,:Data7,:Data8,:Data9,:Data10,SYSDATE)";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no",Station_no),new OleDbParameter(":Station_Seq", Station_Seq),new OleDbParameter(":Ds_name",Ds_name),
                new OleDbParameter(":Value",Value),new OleDbParameter(":ValueType",ValueType),new OleDbParameter(":Data7",Data7),new OleDbParameter(":Data8",Data8),new OleDbParameter(":Data9",Data9),new OleDbParameter(":Data10",Data10)
            };
            DB.ExecuteNonQuery(sql, CommandType.Text, paramet);
        }

        public void PassDeleteAp(OleExec DB, string Station_no)
        {
            string strSql = $@" delete r_ap_temp where data2=:Station_no and DATA3>3 ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no) };
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }

        public void InitDeleteAp(OleExec DB, string Station_no)
        {
            string strSql = $@" delete r_ap_temp where data2=:Station_no ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no) };
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }

        public void FailDeleteAp(OleExec DB, string Station_no)
        {
            string strSql = $@" delete r_ap_temp where data2=:Station_no and data3 in (select max(data3) from r_ap_temp where data2=:Station_no)  ";
            OleDbParameter[] paramet = new OleDbParameter[] { new OleDbParameter(":Station_no", Station_no) };
            DB.ExecuteNonQuery(strSql, CommandType.Text, paramet);
        }
    }
    public class Row_R_AP_TEMP : DataObjectBase
    {
        public Row_R_AP_TEMP(DataObjectInfo info) : base(info)
        {

        }
        public R_AP_TEMP GetDataObject()
        {
            R_AP_TEMP DataObject = new R_AP_TEMP();
            DataObject.DATA1 = this.DATA1;
            DataObject.DATA2 = this.DATA2;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA4 = this.DATA4;
            DataObject.DATA5 = this.DATA5;
            DataObject.DATA6 = this.DATA6;
            DataObject.DATA7 = this.DATA7;
            DataObject.WORK_TIME = this.WORK_TIME;
            DataObject.DATA8 = this.DATA8;
            DataObject.DATA9 = this.DATA9;
            DataObject.DATA10 = this.DATA10;
            return DataObject;
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
        public double? DATA3
        {
            get
            {
                return (double?)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
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
        public double? DATA7
        {
            get
            {
                return (double?)this["DATA7"];
            }
            set
            {
                this["DATA7"] = value;
            }
        }
        public DateTime? WORK_TIME
        {
            get
            {
                return (DateTime?)this["WORK_TIME"];
            }
            set
            {
                this["WORK_TIME"] = value;
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
        public string DATA10
        {
            get
            {
                return (string)this["DATA10"];
            }
            set
            {
                this["DATA10"] = value;
            }
        }
    }
    public class R_AP_TEMP
    {
        public string DATA1;
        public string DATA2;
        public double? DATA3;
        public string DATA4;
        public string DATA5;
        public string DATA6;
        public double? DATA7;
        public DateTime? WORK_TIME;
        public string DATA8;
        public string DATA9;
        public string DATA10;
    }
}
