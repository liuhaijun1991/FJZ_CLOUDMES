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
    public class T_R_FAI : DataObjectTable
    {
        public T_R_FAI(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_FAI(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_FAI);
            TableName = "R_FAI".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public bool CheckExistSku(string Skuno, string SkuVer, OleExec DB)
        {
            return DB.ORM.Queryable<R_FAI>().Where(r => r.SKUNO == Skuno && r.SKU_VER == SkuVer && r.STATUS!="9" && r.FAITYPE== "SKUNO").Any();
        }



        /// <summary>
        /// 判斷工單是否需要做FAI或者是否已經做過FAI
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckWoHaveDoneFai(string WO, string station, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select * from r_fai a,r_fai_station b where  a.id=b.faiid and a.workorderno='{WO}' and a.status=0 and b.faistation='{station}'and a.EDITTIME IS NULL";

            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// 判斷工單在AOI1工站是否需要做FAI或者是否已經做過FAI
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckWoHaveRecorfromBIP(string WO, string station, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select * from r_fai a,r_fai_station b where  a.id=b.faiid and a.workorderno='{WO}' and a.status=0 and b.faistation='{station}'and a.EDITTIME IS NOT NULL";

            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// 判斷第一個SN過BIP到第二個SN到AIO1是否超過兩個小時，HWD使用
        /// </summary>
        /// <param name="WO"></param>
        /// <param name="station"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckSnHave2HoursFai(string WO, string station, OleExec DB)
        {
            bool res = false;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            sql = $@"select * from (
                    SELECT round((sysdate - a.edittime) * 24,3) Hours from r_fai a,r_fai_station b where a.id=b.faiid and a.status=0 and b.faistation='{station}' and a.workorderno='{WO}') aa
                    where aa.Hours>2";

            dt = DB.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count == 0)
            {
                res = true;
            }
            return res;
        }

        /// <summary>
        /// 寫過站記錄
        /// </summary>
        /// <param name="SerialNo"></param>
        /// <param name="WO"></param>
        /// <param name="Remark"></param>
        /// <param name="Station"></param>
        /// <param name="Emp"></param>
        /// <param name="DB"></param>
        public string RecordFAIBySN(string SerialNo, string WO, string Remark, string Station, string Emp, OleExec DB)
        {
            string sql = string.Empty;
            string strsql = string.Empty;
            DataTable dt = new DataTable();
            string result = string.Empty;
            string result1 = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_SN WHERE SN='{SerialNo}' AND VALID_FLAG=1";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        sql = $@"update r_fai_detail set sn='{SerialNo}',faitime=sysdate,faiby='{Emp}'
                                 where faistationid in
                                       (select id
                                          from r_fai_station
                                         where faiid in (select id
                                                           from r_fai
                                                          where faitype = 'WORKORDERNO'
                                                            and workorderno = '{WO}' and status ='0')
                                           and faistation = '{Station}')";
                        strsql = $@"update R_FAI set status ='1',REMARK='{Remark}'
                                 where workorderno='{WO}'and status ='0' and id in (select faiid from R_FAI_STATION where FAISTATION='{Station}') ";
                        result = DB.ExecSQL(sql);
                        result1 = DB.ExecSQL(strsql);
                        if (Int32.Parse(result) == 0 && Int32.Parse(result1) == 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SerialNo }) + ex.Message);
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { SerialNo }));
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public List<FAIList> GetSample(string WO, OleExec DB)
        {
            List<FAIList> FAIlist = new List<FAIList>();
            if (WO == "")
            {
                FAIlist = DB.ORM.Queryable<R_FAI, R_FAI_STATION, R_FAI_DETAIL>((a, b, c) => a.ID == b.FAIID && b.ID == c.FAISTATIONID).
                Select((a, b, c) => new FAIList
                {
                    WORKORDERNO = a.WORKORDERNO,
                    SN = c.SN,
                    FAISTATION = b.FAISTATION,
                    STATUS = SqlSugar.SqlFunc.IIF(a.STATUS == "1", "CLOSED", "OPEN"),
                    REMARK = a.REMARK,
                    FAIBY = c.FAIBY,
                    FAITIME = c.FAITIME
                }).OrderBy(a => a.EDITTIME, SqlSugar.OrderByType.Desc).ToList();
            }
            else
            {
                FAIlist = DB.ORM.Queryable<R_FAI, R_FAI_STATION, R_FAI_DETAIL>((a, b, c) => a.ID == b.FAIID && b.ID == c.FAISTATIONID && a.WORKORDERNO == WO).
                Select((a, b, c) => new FAIList
                {
                    WORKORDERNO = a.WORKORDERNO,
                    SN = c.SN,
                    FAISTATION = b.FAISTATION,
                    STATUS = SqlSugar.SqlFunc.IIF(a.STATUS == "1", "CLOSED", "OPEN"),
                    REMARK = a.REMARK,
                    FAIBY = c.FAIBY,
                    FAITIME = c.FAITIME
                }).OrderBy(a => a.EDITTIME, SqlSugar.OrderByType.Desc).ToList();
            }
            return FAIlist;
        }

        public List<string> GetAllFAISkuno(OleExec DB)
        {
            List<string> SKUNO = new List<string>();
            //string sql = "SELECT SKUNO FROM R_FAI ORDER BY SKUNO";
            string sql = "SELECT DISTINCT SKUNO FROM C_SKU ORDER BY SKUNO";
            DataTable dt = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SKUNO.Add(dr["SKUNO"].ToString());
                        string testTemp = dr["SKUNO"].ToString();
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SKUNO;
        }


        public List<string> GetSkuVerBySkuno(string Skuno, OleExec DB)
        {
            List<string> SKUNO = new List<string>();
            //string sql = $@"SELECT SKU_VER FROM R_FAI WHERE SKUNO='{Skuno}' AND ROWNUM=1";
            string sql = $@"SELECT VERSION FROM C_SKU WHERE ROWNUM=1 AND SKUNO='{Skuno}'";
            DataTable dt = null;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        SKUNO.Add(dr["VERSION"].ToString());
                    }
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return SKUNO;
        }

        public List<FAIList> GetOBAFAIList(OleExec DB)
        {
            List<FAIList> FAIlist = new List<FAIList>();
            FAIlist = DB.ORM.Queryable<R_FAI, R_FAI_STATION, R_FAI_DETAIL>((a, b, c) => a.ID == b.FAIID && b.ID == c.FAISTATIONID)
            .Where(a=>a.FAITYPE== "SKUNO" && a.STATUS!="9" && a.PRODUCTTYPE != "PRR")
            .Select((a, b, c) => new FAIList
            {
                ID = a.ID,
                FAITYPE = a.FAITYPE,
                SKUNO = a.SKUNO,
                SKU_VER = a.SKU_VER,
                WORKORDERNO = a.WORKORDERNO,
                ECONO = a.ECONO,
                LINE = a.LINE,
                PRODUCTTYPE = a.PRODUCTTYPE,
                FAISTATION = b.FAISTATION,
                STATUS = a.STATUS,
                FILENAME = c.FILENAME,
                REMARK = a.REMARK,
                EDITTIME = a.EDITTIME,
                EDITBY = a.EDITBY
            }).OrderBy(a => a.EDITTIME, SqlSugar.OrderByType.Desc).ToList();
            return FAIlist;
        }

        public List<FAIList> GetMRRList(OleExec DB)
        {
            List<FAIList> FAIlist = new List<FAIList>();
            FAIlist = DB.ORM.Queryable<R_FAI, R_FAI_STATION, R_FAI_DETAIL, C_USER>((a, b, c, d) => a.ID == b.FAIID && b.ID == c.FAISTATIONID && a.EDITBY == d.EMP_NO)
            .Where(a => a.FAITYPE == "SKUNO" && a.PRODUCTTYPE == "PRR")
            .Select((a, b, c, d) => new FAIList
            {
                ID = a.ID,
                FAITYPE = a.FAITYPE,
                SKUNO = a.SKUNO,
                SKU_VER = a.SKU_VER,
                WORKORDERNO = a.WORKORDERNO,
                ECONO = a.ECONO,
                LINE = a.LINE,
                PRODUCTTYPE = a.PRODUCTTYPE,
                FAISTATION = b.FAISTATION,
                STATUS = a.STATUS,
                FILENAME = c.FILENAME,
                REMARK = a.REMARK,
                EDITTIME = a.EDITTIME,
                EDITBY = d.EMP_NAME

            }).OrderBy(a => a.EDITTIME, SqlSugar.OrderByType.Desc).ToList();
            return FAIlist;
        }

        public string UpdateFAIbyWo(FAIList FAI, string EDIT_EMP, DateTime EditTime, OleExec DB)
        {
            string result = string.Empty;
            int RF, RFS, RFD = 0;


            RF = DB.ORM.Updateable<R_FAI>()
          .UpdateColumns(a => new R_FAI()
          {
              PRODUCTTYPE = FAI.PRODUCTTYPE,
              REMARK = FAI.REMARK,
              EDITBY = EDIT_EMP,
              EDITTIME = EditTime,
          })
          .Where(a => a.ID == FAI.ID).ExecuteCommand();


            RFS = DB.ORM.Updateable<R_FAI_STATION>()
                   .UpdateColumns(a => new R_FAI_STATION()
                   {
                       FAISTATION = FAI.FAISTATION
                   })
                   .Where(a => a.FAIID == FAI.ID).ExecuteCommand();

            string FAISTATIONID = null;

            FAISTATIONID = DB.ORM.Queryable<R_FAI_STATION>().Where(a => a.FAIID == FAI.ID).Select(a => a.ID).First();

            RFD = DB.ORM.Updateable<R_FAI_DETAIL>()
                   .UpdateColumns(a => new R_FAI_DETAIL()
                   {
                       FILENAME = FAI.FILENAME
                   })
                   .Where(a => a.FAISTATIONID == FAISTATIONID).ExecuteCommand();

            result = RF.ToString();
            return result;
        }

        public int UpdateFAIbySku(string ID,string PRODUCTTYPE, string REMARK,string EDITBY,DateTime EDITTIME, string FILENAME, OleExec DB)
        {
           int result;
           int RF, RFD = 0;
           RF = DB.ORM.Updateable<R_FAI>()
          .UpdateColumns(a => new R_FAI()
          {
              PRODUCTTYPE = PRODUCTTYPE,
              REMARK = REMARK,
              EDITBY = EDITBY,
              EDITTIME = EDITTIME,
          })
          .Where(a => a.ID == ID).ExecuteCommand();
   
     

          string FAISTATIONID = DB.ORM.Queryable<R_FAI_STATION>().
                Where(a => a.FAIID == ID).Select(a => a.ID).First();

           RFD = DB.ORM.Updateable<R_FAI_DETAIL>()
                   .UpdateColumns(a => new R_FAI_DETAIL()
                   {
                       FILENAME = FILENAME,
                       STATUS = "1",
                       FAIBY = EDITBY,
                       FAITIME = EDITTIME,
                       
                   })
                   .Where(a => a.FAISTATIONID == FAISTATIONID).ExecuteCommand();

         result = (RFD==1&& RF==1)?1:0;

            return result;
        }

        public int UpdateFAIToWo(string Wo, string SN, string Remark, string EDIT_EMP, OleExec DB)
        {
            string sql = string.Empty;
            string strsql = string.Empty;
            DataTable dt = new DataTable();
            string result = string.Empty;
            string result1 = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_SN WHERE SN ='{SN}'  AND VALID_FLAG=1";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    try
                    {
                        sql = $@"update r_fai_detail set sn='{SN}',status=1,faitime=SYSDATE,faiby='{EDIT_EMP}'
                                 where faistationid in
                                       (select id
                                          from r_fai_station
                                         where faiid in (select id
                                                           from r_fai
                                                          where faitype = 'WORKORDER'
                                                            and workorderno = '{Wo}'and status=0))";

                        strsql = $@"update r_fai set status=1,REMARK='{Remark}',editby='{EDIT_EMP}',edittime=SYSDATE where workorderno='{Wo}'and status=0";

                        result = DB.ExecSQL(sql);
                        result1 = DB.ExecSQL(strsql);
                        if (Int32.Parse(result) == 0 || Int32.Parse(result1) == 0)
                        {
                            return 0;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { SN }) + ex.Message);
                    }
                }
                else
                {
                    return 1;
                }

            }
            else
            {
                return 2;
            }
            return 3;
        }

        public FAIList GetFAIList(string FAIID, OleExec DB)
        {
            FAIList RFD = DB.ORM.Queryable<R_FAI, R_FAI_STATION, R_FAI_DETAIL>
                ((a, b, c) => (a.ID == b.FAIID && b.ID == c.FAISTATIONID))
                .Where(a => a.ID == FAIID)
                .Select((a, b, c) => new FAIList
                {
                    ID = a.ID,
                    FAITYPE = a.FAITYPE,
                    SKUNO = a.SKUNO,
                    SKU_VER = a.SKU_VER,
                    WORKORDERNO = a.WORKORDERNO,
                    ECONO = a.ECONO,
                    LINE = a.LINE,
                    PRODUCTTYPE = a.PRODUCTTYPE,
                    FAISTATION = b.FAISTATION,
                    STATUS = a.STATUS,
                    FILENAME = c.FILENAME,
                    REMARK = a.REMARK,
                    EDITTIME = a.EDITTIME,
                    EDITBY = a.EDITBY
                })
                .First();

            return RFD;
        }

        public string UpdateFaidetail(string Wo, string emp_no, OleExec DB)
        {
            string sql = string.Empty;
            string strsql = string.Empty;
            DataTable dt = new DataTable();
            string result = string.Empty;
            string result1 = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                try
                {

                    sql = $@"update r_fai_detail set status=1,EDITTIME=SYSDATE,EDITBY='{emp_no}'
                                 where faistationid in
                                       (select id
                                          from r_fai_station
                                         where faiid in (select id
                                                           from r_fai
                                                          where faitype = 'WORKORDER'
                                                            and workorderno = '{Wo}'and status=0))";
                    strsql = $@"update r_fai set editby='{emp_no}',edittime=SYSDATE where faitype = 'WORKORDER' and workorderno='{Wo}'and status=0";
                    result = DB.ExecSQL(sql);
                    result1 = DB.ExecSQL(strsql);
                    if (Int32.Parse(result) == 0 || Int32.Parse(result) == 0)
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { Wo }));
                }
                catch (Exception ex)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000231", new string[] { Wo }) + ex.Message);
                }

            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        public void DeleteOBAFAI(string ID,OleExec DB)
        {
             DB.ORM.Updateable<R_FAI>()
                .UpdateColumns(a => new R_FAI()
                {
                    STATUS = "9"
                })
                .Where(a => a.ID== ID)
                .ExecuteCommand();
        }

    }


    public class Row_R_FAI : DataObjectBase
    {
        public Row_R_FAI(DataObjectInfo info) : base(info)
        {

        }
        public R_FAI GetDataObject()
        {
            R_FAI DataObject = new R_FAI();
            DataObject.ID = this.ID;
            DataObject.FAITYPE = this.FAITYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.LINE = this.LINE;
            DataObject.ECONO = this.ECONO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.REMARK = this.REMARK;
            DataObject.STATUS = this.STATUS;
            DataObject.PRODUCTTYPE = this.PRODUCTTYPE;
            DataObject.CREATETIME = this.CREATETIME;
            DataObject.CREATEBY = this.CREATEBY;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
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
        public string FAITYPE
        {
            get
            {
                return (string)this["FAITYPE"];
            }
            set
            {
                this["FAITYPE"] = value;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
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
        public string ECONO
        {
            get
            {
                return (string)this["ECONO"];
            }
            set
            {
                this["ECONO"] = value;
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
        public string PRODUCTTYPE
        {
            get
            {
                return (string)this["PRODUCTTYPE"];
            }
            set
            {
                this["PRODUCTTYPE"] = value;
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
    }
    public class R_FAI
    {
        public string ID { get; set; }
        public string FAITYPE { get; set; }
        public string SKUNO { get; set; }
        public string SKU_VER { get; set; }
        public string LINE { get; set; }
        public string ECONO { get; set; }
        public string WORKORDERNO { get; set; }
        public string REMARK { get; set; }
        public string STATUS { get; set; }
        public string PRODUCTTYPE { get; set; }
        public DateTime? CREATETIME { get; set; }
        public string CREATEBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string EDITBY { get; set; }
    }

    public class FAIList
    {
        public string ID { get; set; }
        public string FAITYPE { get; set; }
        public string SKUNO { get; set; }
        public string SKU_VER { get; set; }
        public string WORKORDERNO { get; set; }
        public string SN { get; set; }
        public string ECONO { get; set; }
        public string LINE { get; set; }
        public string PRODUCTTYPE { get; set; }
        public string FAISTATION { get; set; }
        public string FAIBY { get; set; }
        public DateTime? EDITTIME { get; set; }
        public string STATUS { get; set; }
        public string FILENAME { get; set; }
        public string REMARK { get; set; }
        public DateTime? FAITIME { get; set; }
        public string EDITBY { get; set; }

    }

    public class Row_FAIList : DataObjectBase
    {
        public Row_FAIList(DataObjectInfo info) : base(info)
        {

        }
        public FAIList GetDataObject()
        {
            FAIList DataObject = new FAIList();
            DataObject.ID = this.ID;
            DataObject.FAITYPE = this.FAITYPE;
            DataObject.SKUNO = this.SKUNO;
            DataObject.SKU_VER = this.SKU_VER;
            DataObject.LINE = this.LINE;
            DataObject.ECONO = this.ECONO;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.REMARK = this.REMARK;
            DataObject.STATUS = this.STATUS;
            DataObject.PRODUCTTYPE = this.PRODUCTTYPE;
            DataObject.EDITTIME = this.EDITTIME;
            DataObject.EDITBY = this.EDITBY;
            DataObject.FAITIME = this.FAITIME;
            DataObject.FAIBY = this.FAIBY;
            DataObject.SN = this.SN;
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
        public string FAITYPE
        {
            get
            {
                return (string)this["FAITYPE"];
            }
            set
            {
                this["FAITYPE"] = value;
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
        public string SKU_VER
        {
            get
            {
                return (string)this["SKU_VER"];
            }
            set
            {
                this["SKU_VER"] = value;
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
        public string ECONO
        {
            get
            {
                return (string)this["ECONO"];
            }
            set
            {
                this["ECONO"] = value;
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
        public string PRODUCTTYPE
        {
            get
            {
                return (string)this["PRODUCTTYPE"];
            }
            set
            {
                this["PRODUCTTYPE"] = value;
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
        public string FAIBY
        {
            get
            {
                return (string)this["FAIBY"];
            }
            set
            {
                this["FAIBY"] = value;
            }
        }
        public DateTime? FAITIME
        {
            get
            {
                return (DateTime?)this["FAITIME"];
            }
            set
            {
                this["FAITIME"] = value;
            }
        }
    }
}