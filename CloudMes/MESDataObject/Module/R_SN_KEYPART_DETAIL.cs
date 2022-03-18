using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper; 
 
 namespace MESDataObject.Module
{
    public class T_R_SN_KEYPART_DETAIL : DataObjectTable
    {
        public T_R_SN_KEYPART_DETAIL(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_KEYPART_DETAIL(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_KEYPART_DETAIL);
            TableName = "R_SN_KEYPART_DETAIL".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_SN_KEYPART_DETAIL> GetKeypartBySN(OleExec sfcdb, string sn, string station)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_SN_KEYPART_DETAIL row_main = null;
            List<R_SN_KEYPART_DETAIL> mains = new List<R_SN_KEYPART_DETAIL>();
            string sql = $@"select * from {TableName} where KEYPART_SN='{sn.Replace("'", "''")}' and STATION_NAME='{station}' ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_R_SN_KEYPART_DETAIL)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public List<R_SN_KEYPART_DETAIL> GetKeypartBySub_Sn(OleExec sfcdb, string sn, string station)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_SN_KEYPART_DETAIL row_main = null;
            List<R_SN_KEYPART_DETAIL> mains = new List<R_SN_KEYPART_DETAIL>();
            string sql = $@"select * from {TableName} where SN='{sn.Replace("'", "''")}' and STATION_NAME='{station}'  order by SEQ_NO ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_R_SN_KEYPART_DETAIL)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    //MES00000037
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public void INSN_KEYPART_DETAIL(OleExec DB, string bu, string SN_ID, string SN, string KEYPART_SN, string STATION_NAME, string PART_NO, double? SEQ_NO, string CATEGORY_NAME, string CATEGORY, string EDIT_EMP)
        {
            T_R_SN_KEYPART_DETAIL T_kd = new T_R_SN_KEYPART_DETAIL(DB, DB_TYPE_ENUM.Oracle);
            Row_R_SN_KEYPART_DETAIL R_kd = (Row_R_SN_KEYPART_DETAIL)T_kd.NewRow();
            try
            {
                R_kd.ID = T_kd.GetNewID(bu, DB);
                R_kd.R_SN_ID = SN_ID;
                R_kd.SN = SN;
                R_kd.KEYPART_SN = KEYPART_SN;
                R_kd.STATION_NAME = STATION_NAME;
                R_kd.PART_NO = PART_NO;
                R_kd.SEQ_NO = SEQ_NO;
                R_kd.CATEGORY_NAME = CATEGORY_NAME;
                R_kd.CATEGORY = CATEGORY;
                R_kd.ORIGINAL_CSN = KEYPART_SN;
                R_kd.VALID = "1";
                R_kd.CREATE_EMP = EDIT_EMP;
                R_kd.CREATE_TIME = GetDBDateTime(DB);
                R_kd.EDIT_EMP = EDIT_EMP;
                R_kd.EDIT_TIME = GetDBDateTime(DB);
                DB.ExecSQL(R_kd.GetInsertString(DB_TYPE_ENUM.Oracle));
            }
            catch (Exception ex)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
            }
        }

        public R_SN_KEYPART_DETAIL GetKeypartSN(string sn,OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KEYPART_DETAIL>().Where(t => t.SN == sn).ToList().FirstOrDefault(); ;
        }

        public bool IsLinked(string sn, OleExec sfcdb)
        {
            return sfcdb.ORM.Queryable<R_SN_KEYPART_DETAIL>().Any(t => t.KEYPART_SN == sn);
        }

        public List<R_SN_KEYPART_DETAIL> GetKeypartBySN(OleExec sfcdb, string sn)
        {
            if (string.IsNullOrEmpty(sn))
            {
                return null;
            }
            DataTable dt = null;
            Row_R_SN_KEYPART_DETAIL row_main = null;
            List<R_SN_KEYPART_DETAIL> mains = new List<R_SN_KEYPART_DETAIL>();
            string sql = $@"select * from {TableName} where SN='{sn.Replace("'", "''")}' and VALID= '1' order by SEQ_NO ";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {
                    dt = sfcdb.ExecSelect(sql).Tables[0];
                    foreach (DataRow dr in dt.Rows)
                    {
                        row_main = (Row_R_SN_KEYPART_DETAIL)this.NewRow();
                        row_main.loadData(dr);
                        mains.Add(row_main.GetDataObject());
                    }
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000037", new string[] { ex.Message }));
                }

            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }
            return mains;
        }

        public bool isBindingPartNo(OleExec sfcdb, string partno)
        {

            if (string.IsNullOrEmpty(partno)) return false;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select id from {TableName} where part_no='{partno}' ";
                DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }

        }

        public bool isLoadIn(OleExec sfcdb, string wo, string partno)
        {
            //if (string.IsNullOrEmpty(partno)) return null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                string sql = $@"select wo.workorderno,sn.sn,kp.keypart_sn,kp.part_no 
from r_sn_keypart_detail kp left join r_sn sn on kp.sn=sn.sn 
left join r_wo_base wo on sn.workorderno=wo.workorderno 
where wo.workorderno='{wo}' and kp.part_no='{partno}' ";
                DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() }));
            }

        }

        public int ReplaceSnKeypartDetail(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE R_SN_KEYPART_DETAIL R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;

        }

        public int InsertKeypartDetails(List<R_SN_KEYPART_DETAIL> Details, string Bu, OleExec DB)
        {
            T_R_SN RSN = new T_R_SN(DB, DB_TYPE_ENUM.Oracle);
            foreach (R_SN_KEYPART_DETAIL detail in Details)
            {
                try
                {
                    detail.ID = GetNewID(Bu, DB);
                    detail.R_SN_ID = RSN.GetSN(detail.SN, DB).ID;
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return DB.ORM.Insertable<R_SN_KEYPART_DETAIL>(Details).ExecuteCommand();
        }

        public int InsertKeypartDetail(R_SN_KEYPART_DETAIL Detail, string Bu, OleExec DB)
        {
            Detail.ID = GetNewID(Bu, DB);
            Detail.CREATE_TIME = GetDBDateTime(DB);
            Detail.EDIT_TIME = GetDBDateTime(DB);
            return DB.ORM.Insertable<R_SN_KEYPART_DETAIL>(Detail).ExecuteCommand();
        }

        public List<R_SN_KEYPART_DETAIL> GetKeypartsBySN(string Sn, string Station, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_KEYPART_DETAIL>().Where(t => t.SN == Sn && t.STATION_NAME == Station && t.VALID=="1" ).OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Asc)
                .ToList();
        }

        public int Update(OleExec DB, R_SN_KEYPART_DETAIL kpObj)
        {
            return DB.ORM.Updateable<R_SN_KEYPART_DETAIL>(kpObj).Where(r => r.ID == kpObj.ID).ExecuteCommand();
        }
    }
    public class Row_R_SN_KEYPART_DETAIL : DataObjectBase
    {
        public Row_R_SN_KEYPART_DETAIL(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_KEYPART_DETAIL GetDataObject()
        {
            R_SN_KEYPART_DETAIL DataObject = new R_SN_KEYPART_DETAIL();
            DataObject.ID = this.ID;
            DataObject.R_SN_ID = this.R_SN_ID;
            DataObject.SN = this.SN;
            DataObject.KEYPART_SN = this.KEYPART_SN;
            DataObject.STATION_NAME = this.STATION_NAME;
            DataObject.PART_NO = this.PART_NO;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.CATEGORY_NAME = this.CATEGORY_NAME;
            DataObject.CATEGORY = this.CATEGORY;
            DataObject.ORIGINAL_CSN = this.ORIGINAL_CSN;
            DataObject.MPN = this.MPN;
            DataObject.OLD_MPN = this.OLD_MPN;
            DataObject.MFR_NAME = this.MFR_NAME;
            DataObject.OLD_MFR_NAME = this.OLD_MFR_NAME;
            DataObject.VALID = this.VALID;
            DataObject.CREATE_EMP = this.CREATE_EMP;
            DataObject.CREATE_TIME = this.CREATE_TIME;
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
        public string KEYPART_SN
        {
            get
            {
                return (string)this["KEYPART_SN"];
            }
            set
            {
                this["KEYPART_SN"] = value;
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
        public string PART_NO
        {
            get
            {
                return (string)this["PART_NO"];
            }
            set
            {
                this["PART_NO"] = value;
            }
        }
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string CATEGORY_NAME
        {
            get
            {
                return (string)this["CATEGORY_NAME"];
            }
            set
            {
                this["CATEGORY_NAME"] = value;
            }
        }
        public string CATEGORY
        {
            get
            {
                return (string)this["CATEGORY"];
            }
            set
            {
                this["CATEGORY"] = value;
            }
        }
        public string ORIGINAL_CSN
        {
            get
            {
                return (string)this["ORIGINAL_CSN"];
            }
            set
            {
                this["ORIGINAL_CSN"] = value;
            }
        }
        public string MPN
        {
            get
            {
                return (string)this["MPN"];
            }
            set
            {
                this["MPN"] = value;
            }
        }
        public string OLD_MPN
        {
            get
            {
                return (string)this["OLD_MPN"];
            }
            set
            {
                this["OLD_MPN"] = value;
            }
        }
        public string MFR_NAME
        {
            get
            {
                return (string)this["MFR_NAME"];
            }
            set
            {
                this["MFR_NAME"] = value;
            }
        }
        public string OLD_MFR_NAME
        {
            get
            {
                return (string)this["OLD_MFR_NAME"];
            }
            set
            {
                this["OLD_MFR_NAME"] = value;
            }
        }
        public string VALID
        {
            get
            {
                return (string)this["VALID"];
            }
            set
            {
                this["VALID"] = value;
            }
        }
        public string CREATE_EMP
        {
            get
            {
                return (string)this["CREATE_EMP"];
            }
            set
            {
                this["CREATE_EMP"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
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
    public class R_SN_KEYPART_DETAIL
    {
        public string ID{get;set;}
        public string R_SN_ID{get;set;}
        public string SN{get;set;}
        public string KEYPART_SN{get;set;}
        public string STATION_NAME{get;set;}
        public string PART_NO{get;set;}
        public double? SEQ_NO{get;set;}
        public string CATEGORY_NAME{get;set;}
        public string CATEGORY{get;set;}
        public string ORIGINAL_CSN{get;set;}
        public string MPN{get;set;}
        public string OLD_MPN{get;set;}
        public string MFR_NAME{get;set;}
        public string OLD_MFR_NAME{get;set;}
        public string VALID{get;set;}
        public string CREATE_EMP{get;set;}
        public DateTime? CREATE_TIME{get;set;}
        public string EDIT_EMP{get;set;}
        public DateTime? EDIT_TIME{get;set;}
    }
}
