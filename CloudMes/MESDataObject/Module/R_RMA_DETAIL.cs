using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_R_RMA_DETAIL : DataObjectTable
    {
        public T_R_RMA_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_RMA_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_RMA_DETAIL);
            TableName = "R_RMA_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 是否存在RMA維修記錄
        /// ADD BY HGB 2019.08.07
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void IsExistsRmaRepairRecord(string SN , OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                SELECT *
            FROM (SELECT SN
                    FROM r_rma_detail a
                   WHERE EXISTS (SELECT 1
                            FROM R_SN
                           WHERE SN = a.SN
                             AND (SN = '{SN}' OR
                                 boxsn = '{SN}'))
                  UNION
                  SELECT SN
                    FROM r_rma_detail a
                   WHERE EXISTS (SELECT 1
                            FROM r_relationdata_external
                           WHERE SN = a.SN
                             AND (SN = '{SN}' OR
                                 PARENT = '{SN}')))
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20210813092819", new string[] { SN });
                throw new MESReturnMessage(errMsg);
            }
        }

        /// <summary>
        /// 是否存在RMA維修記錄
        /// ADD BY HGB 2019.08.07
        /// </summary>
        /// <param name="SN"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public void IsConfirmRmaRepairRecord(string SN, OleExec DB)
        {
            string sql = string.Empty;
            string snrohs = string.Empty;
            string Palletrohs = string.Empty;
            DataTable dt = new DataTable();
            sql = $@" 
                SELECT *
            FROM (SELECT *
                    FROM (SELECT *
                            FROM r_rma_detail a
                           WHERE EXISTS (SELECT 1
                                    FROM R_SN
                                   WHERE SN = a.SN
                                     AND (SN = '{SN}' OR
                                         boxsn = '{SN}'))
                           ORDER BY createdate DESC)
                   WHERE rownum = 1)
           WHERE data1 = '1'
                 ";
            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                throw new MESReturnMessage(SN + "此RMA SN沒有維修確認動作,請聯繫RMA人員！"); ;
            }
        }

        public R_RMA_DETAIL GetObjBySN(OleExec sfcdb, string sn)
        {
            return sfcdb.ORM.Queryable<R_RMA_DETAIL>().Where(r => r.SN == sn).OrderBy(r => r.CREATEDATE, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
        }

        public bool IsCheck(OleExec sfcdb, string sn)
        {
            return sfcdb.ORM.Queryable<R_RMA_DETAIL>().Where(r => r.SN == sn && r.DATA1 == "2").Any();
        }
    }
    public class Row_R_RMA_DETAIL : DataObjectBase
    {
        public Row_R_RMA_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_RMA_DETAIL GetDataObject()
        {
            R_RMA_DETAIL DataObject = new R_RMA_DETAIL();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.MODEL = this.MODEL;
            DataObject.REPAIR_PO = this.REPAIR_PO;
            DataObject.RAMSTATE = this.RAMSTATE;
            DataObject.REPAIRREASON = this.REPAIRREASON;
            DataObject.CREATEDATE = this.CREATEDATE;
            DataObject.REPAIRACTION = this.REPAIRACTION;
            DataObject.REMARK = this.REMARK;
            DataObject.OUTDATE = this.OUTDATE;
            DataObject.DATA1 = this.DATA1;
            DataObject.CHECKDATE = this.CHECKDATE;
            DataObject.EXAMINADATA = this.EXAMINADATA;
            DataObject.DATA4 = this.DATA4;
            DataObject.CHENKREMARK = this.CHENKREMARK;
            DataObject.DATA3 = this.DATA3;
            DataObject.DATA5 = this.DATA5;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string MODEL
        {
            get
            {
                return (string)this["MODEL"];
            }
            set
            {
                this["MODEL"] = value;
            }
        }
        public string REPAIR_PO
        {
            get
            {
                return (string)this["REPAIR_PO"];
            }
            set
            {
                this["REPAIR_PO"] = value;
            }
        }
        public string RAMSTATE
        {
            get
            {
                return (string)this["RAMSTATE"];
            }
            set
            {
                this["RAMSTATE"] = value;
            }
        }
        public string REPAIRREASON
        {
            get
            {
                return (string)this["REPAIRREASON"];
            }
            set
            {
                this["REPAIRREASON"] = value;
            }
        }
        public DateTime? CREATEDATE
        {
            get
            {
                return (DateTime?)this["CREATEDATE"];
            }
            set
            {
                this["CREATEDATE"] = value;
            }
        }
        public string REPAIRACTION
        {
            get
            {
                return (string)this["REPAIRACTION"];
            }
            set
            {
                this["REPAIRACTION"] = value;
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
        public DateTime? OUTDATE
        {
            get
            {
                return (DateTime?)this["OUTDATE"];
            }
            set
            {
                this["OUTDATE"] = value;
            }
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
        public DateTime? CHECKDATE
        {
            get
            {
                return (DateTime?)this["CHECKDATE"];
            }
            set
            {
                this["CHECKDATE"] = value;
            }
        }
        public string EXAMINADATA
        {
            get
            {
                return (string)this["EXAMINADATA"];
            }
            set
            {
                this["EXAMINADATA"] = value;
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
        public string CHENKREMARK
        {
            get
            {
                return (string)this["CHENKREMARK"];
            }
            set
            {
                this["CHENKREMARK"] = value;
            }
        }
        public string DATA3
        {
            get
            {
                return (string)this["DATA3"];
            }
            set
            {
                this["DATA3"] = value;
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
    }
    public class R_RMA_DETAIL
    {
        public string ID{get;set;}
        public string SN{get;set;}
        public string MODEL{get;set;}
        public string REPAIR_PO{get;set;}
        public string RAMSTATE{get;set;}
        public string REPAIRREASON{get;set;}
        public DateTime? CREATEDATE{get;set;}
        public string REPAIRACTION{get;set;}
        public string REMARK{get;set;}
        public DateTime? OUTDATE{get;set;}
        public string DATA1{get;set;}
        public DateTime? CHECKDATE{get;set;}
        public string EXAMINADATA{get;set;}
        public string DATA4{get;set;}
        public string CHENKREMARK{get;set;}
        public string DATA3{get;set;}
        public string DATA5{get;set;}
        public DateTime? EDIT_TIME{get;set;}
        public string EDIT_EMP{get;set;}
    }
}