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
    public class T_R_ERMA : DataObjectTable
    {
        public T_R_ERMA(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_ERMA(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_ERMA);
            TableName = "R_ERMA".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        //SN在R_RMA表 HHH 20190326
        public bool HasSN(string SN, OleExec db)
        {
            bool res = false;
            DataTable Dt = new DataTable();
            string strsql = $@"SELECT * FROM R_ERMA WHERE SN='{SN}' OR SN =('FOC' || substr('{SN}', -8)) ";
            Dt = db.ExecSelect(strsql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                res = true;
            }
            return res;
        }
        /// <summary>
        /// 獲取RMA前1500條信息  order by EDIT_TIME   add  by  zsh  2019-04-16 
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_ERMA> SelectAll(OleExec DB)
        {
            string strsql = string.Empty;
            List<R_ERMA> ERMA = new List<R_ERMA>();
            strsql = $@" SELECT * FROM  R_ERMA WHERE ROWNUM<1500 ORDER BY EDIT_TIME DESC ";
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_ERMA ret = (Row_R_ERMA)NewRow();
                    ret.loadData(res.Rows[i]);
                    ERMA.Add(ret.GetDataObject());
                }
            }
            return ERMA;
        }

        /// <summary>
        /// 根據SN查詢RMA信息  由PM上傳的RMA信息    add by zsh  2019-04-16
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_ERMA> SelectRMAbySN(string SN, OleExec DB)
        {
            string strsql = string.Empty;
            List<R_ERMA> ERMA = new List<R_ERMA>();
            strsql = $@" SELECT * FROM  R_ERMA WHERE SN=:SN";
            OleDbParameter[] paramet = new OleDbParameter[] {
                new OleDbParameter(":SN",SN)
            };
            DataTable res = DB.ExecuteDataTable(strsql, CommandType.Text, paramet);
            if (res.Rows.Count > 0)
            {
                for (int i = 0; i < res.Rows.Count; i++)
                {
                    Row_R_ERMA ret = (Row_R_ERMA)NewRow();
                    ret.loadData(res.Rows[i]);
                    ERMA.Add(ret.GetDataObject());
                }
            }
            return ERMA;
        }

        public int InsertDate(string R_SN_ID,string SN,string DF_Site,string SKUNO,string BU,DateTime? DF_INPUT_DATE,string EDIT_EMP,DateTime? EDIT_TIME,string USER_BU,OleExec DB)
        {
            int result = 0;
            string strsql = string.Empty;
            string ID = MesDbBase.GetNewID(DB.ORM,USER_BU,"R_ERMA");
            strsql = $@" INSERT INTO R_ERMA (ID,R_SN_ID,SN,DF_Site,SKUNO,BU,DF_INPUT_DATE,EDIT_EMP,EDIT_TIME) VALUES
  (:ID,:R_SN_ID,:SN,:DF_Site,:SKUNO,:BU,:DF_INPUT_DATE,:EDIT_EMP,:EDIT_TIME)";
            OleDbParameter[] parameters = new OleDbParameter[] {new OleDbParameter(":ID",ID),
                new OleDbParameter(":R_SN_ID", R_SN_ID),
                new OleDbParameter(":SN", SN),
                new OleDbParameter(":DF_Site", DF_Site),
                new OleDbParameter(":SKUNO", SKUNO),
                new OleDbParameter(":BU", BU),
                new OleDbParameter(":DF_INPUT_DATE", DF_INPUT_DATE),
                new OleDbParameter(":EDIT_EMP", EDIT_EMP),
                new OleDbParameter(":EDIT_TIME",EDIT_TIME)
        };
            result = DB.ExecuteNonQuery(strsql, CommandType.Text, parameters);
            return result; 
        }
        /// <summary>
        /// 修改信息by ID
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="DF_SITE"></param>
        /// <param name="SKUNO"></param>
        /// <param name="BU"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateByID(string ID, string DF_SITE ,string SKUNO,string  BU, OleExec DB)
        {
            int result = 0;
            string strsql = string.Empty;
            strsql = $@" UPDATE  R_ERMA SET DF_SITE=:DF_SITE,SKUNO=:SKUNO,BU=:BU WHERE ID=:ID";
            OleDbParameter[] parameters = new OleDbParameter[] { new OleDbParameter(":ID", ID),
                new OleDbParameter(":DF_SITE", DF_SITE),
                new OleDbParameter(":SKUNO", SKUNO),
                new OleDbParameter(":BU", BU)
        };
            result = DB.ExecuteNonQuery(strsql, CommandType.Text, parameters);
            return result;
        }

        public List<R_ERMA> GetSNAndBySN(string SN, OleExec DB)
        {
            return DB.ORM.Queryable<R_ERMA>().Where(t => (t.SN == SN || t.SN == "F" + SN.Substring(SN.Length - 10, 10))).ToList();
        }
    }
    public class Row_R_ERMA : DataObjectBase
    {
        public Row_R_ERMA(DataObjectInfo info) : base(info)
        {

        }
        public R_ERMA GetDataObject()
        {
            R_ERMA DataObject = new R_ERMA();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.DF_SITE = this.DF_SITE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.BU = this.BU;
            DataObject.DF_INPUT_DATE = this.DF_INPUT_DATE;
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
        public string R_SN_ID
        {
            get
            {
                return (string)this["R_SN_ID"];
            }
            set
            {
                this["R_SN_ID"] = value;
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
        public string DF_SITE
        {
            get
            {
                return (string)this["DF_SITE"];
            }
            set
            {
                this["DF_SITE"] = value;
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
        public string BU
        {
            get
            {
                return (string)this["BU"];
            }
            set
            {
                this["BU"] = value;
            }
        }
        public DateTime? DF_INPUT_DATE
        {
            get
            {
                return (DateTime?)this["DF_INPUT_DATE"];
            }
            set
            {
                this["DF_INPUT_DATE"] = value;
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
    public class R_ERMA
    {
        public string ID { get; set; }
        public string R_SN_ID { get; set; }
        public string SN { get; set; }
        public string DF_SITE { get; set; }
        public string SKUNO { get; set; }
        public string BU { get; set; }
        public DateTime? DF_INPUT_DATE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
    }
}