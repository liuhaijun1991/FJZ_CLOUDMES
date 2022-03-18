using System;
using MESDBHelper;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using static MESDataObject.Common.EnumExtensions;

namespace MESDataObject.Module
{
    public class T_R_SN_LOCK : DataObjectTable
    {
        public T_R_SN_LOCK(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_LOCK(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_SN_LOCK);
            TableName = "R_SN_LOCK".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public List<R_SN_LOCK> GetLockList(string LOCK_LOT, string SN,string WORKORDERNO, OleExec DB)
        {
            List<R_SN_LOCK> Seq = new List<R_SN_LOCK>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_R_SN_LOCK SeqRow = (Row_R_SN_LOCK)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select * from R_SN_LOCK where 1=1  ";
                if (LOCK_LOT != "")
                    sql += $@" and LOCK_LOT='{LOCK_LOT}' ";
                if (SN != "")
                    sql += $@" and SN='{SN}' ";
                if(WORKORDERNO != "")
                    sql += $@" and WORKORDERNO='{WORKORDERNO}' ";
                if (LOCK_LOT == "" && SN == "" &&  WORKORDERNO == "")
                    sql += $@" and  rownum<21  order by LOCK_TIME ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SeqRow.loadData(dr);
                    Seq.Add(SeqRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return Seq;
        }
         

        public List<R_SN_LOCK> GetLockList(string LOCK_ID,string LOCK_LOT, string SN, string WORKORDERNO,string STATUS, OleExec DB)
        {
            List<R_SN_LOCK> Seq = new List<R_SN_LOCK>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_R_SN_LOCK SeqRow = (Row_R_SN_LOCK)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select LK.ID, LK.TYPE,LK.WORKORDERNO,LK.LOCK_LOT,LK.SN,LK.LOCK_STATION,LK.LOCK_STATUS,LK.LOCK_REASON,LK.UNLOCK_REASON,LK.LOCK_EMP,LK.LOCK_TIME,LK.UNLOCK_EMP,LK.UNLOCK_TIME from R_SN_LOCK LK  where 1=1  ";

                if (LOCK_ID != "")
                {
                    sql += $@" and LK.ID='{LOCK_ID}' ";
                }

                if (LOCK_LOT != "")
                {
                    sql += $@" and LK.LOCK_LOT='{LOCK_LOT}' ";
                }

                if (SN != "")
                {
                    sql += $@" and LK.SN='{SN}' ";
                }

                if (WORKORDERNO != "")
                {
                    sql += $@" and LK.WORKORDERNO='{WORKORDERNO}' ";
                }
                if (STATUS != "")
                {
                    sql += $@" and LK.LOCK_STATUS='{STATUS}'";
                }
                if (LOCK_LOT == "" && SN == "" && WORKORDERNO == "")
                {
                    sql += $@" and  rownum<21  order by LK.LOCK_TIME ";
                }
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SeqRow.loadData(dr);
                    Seq.Add(SeqRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return Seq;
        }

        public List<R_SN_LOCK> FTXGetLockList(string SN, string WORKORDERNO, string STATUS, OleExec DB)
        {
            List<R_SN_LOCK> Seq = new List<R_SN_LOCK>();
            string sql = string.Empty;
            DataTable dt = new DataTable("C_SEQNO");
            Row_R_SN_LOCK SeqRow = (Row_R_SN_LOCK)NewRow();

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select LK.ID, LK.TYPE,NVL(LK.WORKORDERNO,SN.WORKORDERNO) AS WORKORDERNO,LK.LOCK_LOT,NVL(LK.SN,SN.SN) AS SN,LK.LOCK_STATION,LK.LOCK_STATUS,LK.LOCK_REASON,LK.UNLOCK_REASON,LK.LOCK_EMP,LK.LOCK_TIME,LK.UNLOCK_EMP,LK.UNLOCK_TIME from R_SN_LOCK LK,R_SN SN  where 1=1 AND SN.VALID_FLAG='1' AND (LK.SN=SN.SN OR LK.WORKORDERNO=SN.WORKORDERNO)  ";


                if (SN != "")
                {
                    sql = $@"  select LK.ID, LK.TYPE,NVL(LK.WORKORDERNO,SN.WORKORDERNO) AS WORKORDERNO,LK.LOCK_LOT,NVL(LK.SN,SN.SN) AS SN,LK.LOCK_STATION,LK.LOCK_STATUS,LK.LOCK_REASON,LK.UNLOCK_REASON,LK.LOCK_EMP,LK.LOCK_TIME,LK.UNLOCK_EMP,LK.UNLOCK_TIME from R_SN_LOCK LK left join R_SN SN  on LK.SN=SN.SN OR LK.WORKORDERNO=SN.WORKORDERNO where 1=1   ";

                    sql += $@" and (LK.SN='{SN}' or SN.SN = '{SN}') ";
                }

                if (WORKORDERNO != "")
                {
                    sql = $@"  select LK.ID, LK.TYPE,LK.WORKORDERNO,LK.LOCK_STATION,LK.LOCK_STATUS,LK.LOCK_REASON,LK.UNLOCK_REASON,LK.LOCK_EMP,LK.LOCK_TIME,LK.UNLOCK_EMP,LK.UNLOCK_TIME from R_SN_LOCK LK  where 1=1  ";

                    sql += $@" and (LK.WORKORDERNO='{WORKORDERNO}') ";
                }
                if (STATUS != "")
                {
                    sql += $@" and LK.LOCK_STATUS='{STATUS}'";
                }

                sql += $@" order by LK.UNLOCK_TIME desc ";
                dt = DB.ExecSelect(sql, null).Tables[0];
                foreach (DataRow dr in dt.Rows)
                {
                    SeqRow.loadData(dr);
                    Seq.Add(SeqRow.GetDataObject());
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return Seq;
        }
        public R_SN_LOCK GetDetailBySN(OleExec sfcdb, string sn, string station)
        {
            if (string.IsNullOrEmpty(sn) || string.IsNullOrEmpty(station))
            {
                return null;
            }
            string sql = $@"select * from {TableName} where sn='{sn.Replace("'", "''")}' 
                            and lock_station='{station.Replace("'", "''")}' and lock_status='1' ";
            DataTable dt = null;
            Row_R_SN_LOCK row_r_sn_lock = null;
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                dt = sfcdb.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row_r_sn_lock = (Row_R_SN_LOCK) this.NewRow();
                    row_r_sn_lock.loadData(dt.Rows[0]);
                }
            }
            else
            {
                return null;
            }
            return row_r_sn_lock == null? null: row_r_sn_lock.GetDataObject();
        }


        /// <summary>
        /// 檢查(機種)鎖定鎖定 
        /// ADD BY HGB 2019.06.12
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="sn"></param>
        /// <param name="station"></param>
        /// <returns></returns>
        public void CheckSkuLock(string SN, string TYPE, string STATION,   OleExec DB)
        {
            bool  LOCK = IsLock("", TYPE, SN, "", STATION, "", DB);
            if (LOCK) //試製機種被鎖定,僅有一個SN可以過卡通
            {
                var SkuLock = DB.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == SN
                   && (t.LOCK_STATION == STATION)
                   && t.LOCK_STATUS == "1"
                   && t.TYPE == TYPE).ToList();
                string ErrMsg = "";
                if (SkuLock.Count > 0)
                { 
                    try
                    {
                        ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { SN, SkuLock[0].LOCK_EMP, SkuLock[0].LOCK_REASON });
                    }
                    catch
                    {
                        throw new Exception($@"SN:'{SN}' Locked By:'{SkuLock[0].LOCK_EMP}' Reason:'{SkuLock[0].LOCK_REASON}'");
                    }
                    throw new Exception(ErrMsg);
                }

            }
        }

        public bool IsExist(OleExec sfcdb, string SerialNoOrLineName, string CurrentStation)
        {
            if (string.IsNullOrEmpty(SerialNoOrLineName) || string.IsNullOrEmpty(CurrentStation))
            {
                return true;
            }
            string sql = $@" select id from {TableName} where sn='{SerialNoOrLineName.Replace("'", "''")}' 
                            and lock_station='{CurrentStation.Replace("'", "''")}' and lock_status='0' ";
            //object obj = sfcdb.ExecSelectOneValue(sql);
            try
            {
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
            catch (Exception)
            {
                return true;
            }
            
        }

        public List<R_SN_LOCK> GetLockListByPackNo(string station,string packNo,OleExec DB)
        {
           List<R_SN_LOCK> res = new List<R_SN_LOCK>();
            ////string strSql = $@" select E.* from R_PACKING A,R_PACKING B ,R_SN_PACKING C,R_SN D,R_SN_LOCK E WHERE A.ID=C.PACK_ID AND B.ID=A.PARENT_PACK_ID AND C.SN_ID=D.ID AND D.SN=E.SN
            ////                     AND B.PACK_NO='{packNo}' AND E.LOCK_STATUS='1' ";
            //// DataSet ds = DB.ExecSelect(strSql);
            //// foreach (DataRow VARIABLE in ds.Tables[0].Rows)
            //// {
            ////     Row_R_SN_LOCK r = (Row_R_SN_LOCK)this.NewRow();
            ////     r.loadData(VARIABLE);
            ////     res.Add(r.GetDataObject());
            //// }
             List<R_SN> SNList = new List<R_SN>();
             T_R_PACKING TRP = new T_R_PACKING(DB, this.DBType);
             TRP.GetSnListByPackNo(packNo, ref SNList, DB);
            // List<string> SNs = new List<string>();
            // foreach (R_SN RSN in SNList)
            // {
            //     SNs.Add(RSN.SN);
            // }
            // res = DB.ORM.Queryable<R_SN_LOCK>().Where(t => SNs.Contains(t.SN) && t.LOCK_STATUS == "1").ToList();

            //按CARTON來查
            res = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_LOCK>((rs, rp, rsp, rsl) => rs.SN == rsl.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                .Where((rs, rp, rsp, rsl) => rp.PACK_NO == packNo && rsl.LOCK_STATUS == "1" && rsl.LOCK_STATION==station)
                .Select((rs, rp, rsp, rsl) => rsl)
                .ToList();
            if (res.Count == 0)
            {
                //按棧板來查
                res = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_LOCK,R_PACKING>((rs, rp, rsp, rsl, rpp) => rs.SN == rsl.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && rp.PARENT_PACK_ID==rpp.ID)
               .Where((rs, rp, rsp, rsl, rpp) => rpp.PACK_NO == packNo && rsl.LOCK_STATUS == "1" && rsl.LOCK_STATION == station)
               .Select((rs, rp, rsp, rsl, rpp) => rsl)
               .ToList();
            }

            //foreach (R_SN RSN in SNList)
            //{
            //    SNLock = DB.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN.Equals(RSN.SN) && t.LOCK_STATUS == "1").ToList().FirstOrDefault();
            //    if (SNLock != null)
            //    {
            //        res.Add(SNLock);
            //    }
            //}
            return res;
        }

        public List<R_SN_LOCK> GetLockSnListByPackNo(string packNo, OleExec DB)
        {
            
            return DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_LOCK>((rs, rp, rsp, rsl) => rs.SN == rsl.SN && rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                .Where((rs, rp, rsp, rsl) => rp.PACK_NO == packNo && rsl.LOCK_STATUS=="1")
                .Select((rs, rp, rsp, rsl) => rsl)
                .ToList();
        }

        /// <summary>
        /// 獲取卡通內被鎖定的SN集合
        /// </summary>
        /// <param name="cartonNo"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SN_LOCK> GetLockListByCartonNo(string cartonNo, OleExec sfcdb)
        {
            string sql = $@"select D.* from R_PACKING A,R_SN_PACKING B, R_SN C, R_SN_LOCK D WHERE A.ID = B.PACK_ID
                            AND B.SN_ID = C.ID   AND D.SN = C.SN   AND A.PACK_NO = '{cartonNo}'   AND D.LOCK_STATUS = '1'  ";
            List<R_SN_LOCK> lockList = new List<R_SN_LOCK>();
            DataSet ds = sfcdb.ExecSelect(sql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                Row_R_SN_LOCK rowLock = (Row_R_SN_LOCK)this.NewRow();
                rowLock.loadData(row);
                lockList.Add(rowLock.GetDataObject());
            }
            return lockList;
        }

        public void LockSnInOba(string lotNo,OleExec DB)
        {
            try
            {
                string strSql = $@" INSERT INTO R_SN_LOCK (ID,LOCK_LOT,SN,TYPE,WORKORDERNO,LOCK_STATION,LOCK_STATUS,LOCK_REASON,LOCK_EMP,LOCK_TIME)
                            SELECT 'MES'||SFC.SEQ_C_ID.NEXTVAL,'{lotNo}',E.SN,'SN',E.WORKORDERNO,E.NEXT_STATION,'1','OBAFAILSAMPLE','SYSTEM',SYSDATE FROM R_LOT_PACK A,R_PACKING B,R_PACKING C,R_SN_PACKING D,R_SN E
                             WHERE A.LOTNO='{lotNo}' AND A.PACKNO=B.PACK_NO AND B.ID=C.PARENT_PACK_ID AND C.ID=D.PACK_ID AND D.SN_ID=E.ID ";
                DB.ThrowSqlExeception = true;
                DB.ExecSQL(strSql);
            }
            catch(Exception e) { throw e; }
            finally
            {
                DB.ThrowSqlExeception = false;
            }
        }

        public bool IsUnLock(string LOCK_LOT, string SN, string WORKORDERNO,string station, OleExec DB)
        {
            List<R_SN_LOCK> Seq = new List<R_SN_LOCK>();
            string sql = string.Empty;
            bool isLock = true;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"  select * from R_SN_LOCK where 1=1 and lock_status='1' and lock_station='{station}'  ";
                if (LOCK_LOT != "")
                {
                    sql += $@" and LOCK_LOT='{LOCK_LOT}' ";
                }
                if (SN != "")
                {
                    sql += $@" and SN='{SN}' ";
                }
                if (WORKORDERNO != "")
                {
                    sql += $@" and WORKORDERNO='{WORKORDERNO}' ";
                }
                if (DB.ExecSelect(sql, null).Tables[0].Rows.Count == 0)
                {
                    isLock = false;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return isLock;
        }

        public bool IsLock(string lock_lot, string type, string sn, string workorderno, string station, string lock_reason, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_LOCK>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lock_lot), r => r.LOCK_LOT == lock_lot)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(type), r => r.TYPE == type)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), r => r.SN == sn)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(workorderno), r => r.WORKORDERNO == workorderno)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lock_reason), r => r.LOCK_REASON == lock_reason)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(station), r => r.LOCK_STATION == station).Any(r => r.LOCK_STATUS == "1");           
        }

        public int AddNewLock(string bu,string lock_lot,string type,string sn,string workorderno,string station,string lock_reason,string user,OleExec DB)
        {
            R_SN_LOCK lockObject = new R_SN_LOCK();
            lockObject.ID = this.GetNewID(bu, DB);
            lockObject.LOCK_LOT = lock_lot;
            lockObject.TYPE = type;
            lockObject.SN = sn;
            lockObject.WORKORDERNO = workorderno;
            lockObject.LOCK_STATION = station;
            lockObject.LOCK_REASON = lock_reason;
            lockObject.LOCK_STATUS = "1";
            lockObject.LOCK_TIME = GetDBDateTime(DB);
            lockObject.LOCK_EMP = user;
            return DB.ORM.Insertable(lockObject).ExecuteCommand();
        }

        public R_SN_LOCK GetLockObject(string lock_lot, string type, string sn,string workorderno, string reason,string station, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_LOCK>().WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(lock_lot), r => r.LOCK_LOT == lock_lot)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(type), r => r.TYPE == type)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(sn), r => r.SN == sn)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(workorderno), r => r.WORKORDERNO == workorderno)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(reason), r => r.LOCK_REASON == reason)
                .WhereIF(!SqlSugar.SqlFunc.IsNullOrEmpty(station), r => r.LOCK_STATION == station || r.LOCK_STATION == "ALL") //FTX can lock ALL stations
                .ToList().FirstOrDefault();            
        }
    }
    public class Row_R_SN_LOCK : DataObjectBase
    {
        public Row_R_SN_LOCK(DataObjectInfo info) : base(info)
        {

        }
        public R_SN_LOCK GetDataObject()
        {
            R_SN_LOCK DataObject = new R_SN_LOCK();
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
            DataObject.ID = this.ID;
            DataObject.LOCK_LOT = this.LOCK_LOT;
            DataObject.SN = this.SN;
            DataObject.TYPE = this.TYPE;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.LOCK_STATION = this.LOCK_STATION;
            DataObject.LOCK_STATUS = this.LOCK_STATUS;
            DataObject.LOCK_REASON = this.LOCK_REASON;
            DataObject.UNLOCK_REASON = this.UNLOCK_REASON;
            DataObject.LOCK_EMP = this.LOCK_EMP;
            DataObject.LOCK_TIME = this.LOCK_TIME;
            DataObject.UNLOCK_EMP = this.UNLOCK_EMP;
            DataObject.UNLOCK_TIME = this.UNLOCK_TIME;
            return DataObject;
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
        public DateTime? EDITTIME
        {
            get
            {
                return (DateTime?)this["EDITTIME"];
            }
            set
            {
                this["EDITTIME"] = value;
            }
        }
        public string EDITBY
        {
            get
            {
                return (string)this["EDITBY"];
            }
            set
            {
                this["EDITBY"] = value;
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
        public string LOCK_LOT
        {
            get
            {
                return (string)this["LOCK_LOT"];
            }
            set
            {
                this["LOCK_LOT"] = value;
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
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public string LOCK_STATION
        {
            get
            {
                return (string)this["LOCK_STATION"];
            }
            set
            {
                this["LOCK_STATION"] = value;
            }
        }
        public string LOCK_STATUS
        {
            get
            {
                return (string)this["LOCK_STATUS"];
            }
            set
            {
                this["LOCK_STATUS"] = value;
            }
        }
        public string LOCK_REASON
        {
            get
            {
                return (string)this["LOCK_REASON"];
            }
            set
            {
                this["LOCK_REASON"] = value;
            }
        }
        public string UNLOCK_REASON
        {
            get
            {
                return (string)this["UNLOCK_REASON"];
            }
            set
            {
                this["UNLOCK_REASON"] = value;
            }
        }
        public string LOCK_EMP
        {
            get
            {
                return (string)this["LOCK_EMP"];
            }
            set
            {
                this["LOCK_EMP"] = value;
            }
        }
        public DateTime? LOCK_TIME
        {
            get
            {
                return (DateTime?)this["LOCK_TIME"];
            }
            set
            {
                this["LOCK_TIME"] = value;
            }
        }
        public string UNLOCK_EMP
        {
            get
            {
                return (string)this["UNLOCK_EMP"];
            }
            set
            {
                this["UNLOCK_EMP"] = value;
            }
        }
        public DateTime? UNLOCK_TIME
        {
            get
            {
                return (DateTime?)this["UNLOCK_TIME"];
            }
            set
            {
                this["UNLOCK_TIME"] = value;
            }
        }
    }
    public class R_SN_LOCK
    {
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
        [SqlSugar.SugarColumn(IsPrimaryKey =true)]
        public string ID { get; set; }
        public string LOCK_LOT { get; set; }
        public string SN { get; set; }
        public string TYPE { get; set; }
        public string WORKORDERNO { get; set; }
        public string LOCK_STATION { get; set; }
        public string LOCK_STATUS { get; set; }
        public string LOCK_REASON { get; set; }
        public string UNLOCK_REASON { get; set; }
        public string LOCK_EMP { get; set; }
        public DateTime? LOCK_TIME { get; set; }
        public string UNLOCK_EMP { get; set; }
        public DateTime? UNLOCK_TIME { get; set; }
    }

    public enum LockType
    {
        [EnumValue("WO")]
        WorkOrderNo,
        [EnumValue("SN")]
        Sn,
        [EnumValue("SKU")]
        Skuno,
    }

    public enum LockStation
    {
        [EnumValue("ALL")]
        ALL
    }

}