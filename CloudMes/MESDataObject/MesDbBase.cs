using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data;
using System.Reflection;
using System.Data.OleDb;
using SqlSugar;

namespace MESDataObject
{
    public class MesDbBase
    {
        /// <summary>
        /// Get Table seqId
        /// 222
        /// </summary>
        /// <param name="DbStr"></param>
        /// <param name="Bu"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static string GetNewID(string DbStr,string Bu,string TableName)
        {
            using (var db = OleExec.GetSqlSugarClient(DbStr,true)) {
                var IN_BU = new SugarParameter("@IN_BU", Bu);
                var IN_TYPE = new SugarParameter("@IN_TYPE", TableName);
                var OUT_RES = new SugarParameter("@OUT_RES", null, true);//isOutput=true
                var excDt = db.Ado.UseStoredProcedure().GetDataTable("SFC.GET_ID", IN_BU, IN_TYPE, OUT_RES);
                string newID = OUT_RES.Value.ToString();
                if (newID.StartsWith("ERR"))
                {
                    throw new Exception("Get table '" + TableName + "' ID exception! " + newID);
                }
                return newID;
            }
        }

        public static string ReverseJuniperByType(string IN_TYPE, string IN_INPUT, string IN_USER, string IN_REASON, OleExec db)
        {
            
            var USER = new SugarParameter("@IN_USER", IN_USER);
            var REASON = new SugarParameter("@IN_REASON", IN_REASON);
            var OUT_RES = new SugarParameter("@OUT_RES", null, true);
            
            string res = null;

            if (IN_TYPE == "SN")
            {
                var SN = new SugarParameter("@IN_SN", IN_INPUT);
                var excDt = db.ORM.Ado.UseStoredProcedure().GetDataTable("SFCRUNTIME.SP_REVERSE_JUNIPER_SN", SN, USER, REASON, OUT_RES);
                res = OUT_RES.Value.ToString();
            }
            else
            {
                var WO = new SugarParameter("@IN_WO", IN_INPUT);
                var excDt = db.ORM.Ado.UseStoredProcedure().GetDataTable("SFCRUNTIME.SP_REVERSE_JUNIPER_WO", WO, USER, REASON, OUT_RES);
                res = OUT_RES.Value.ToString();
            }

            return res;
        }

        /// <summary>
        /// Get Table seqId
        /// 222
        /// </summary>
        /// <param name="DbStr"></param>
        /// <param name="Bu"></param>
        /// <param name="TableName"></param>
        /// <param name="ReadXMLConfig">true=>from xml</param>
        /// <returns></returns>
        public static string GetNewID(string DbStr, string Bu, string TableName,bool ReadXMLConfig)
        {
            using (var db = OleExec.GetSqlSugarClient(DbStr, ReadXMLConfig))
            {
                var IN_BU = new SugarParameter("@IN_BU", Bu);
                var IN_TYPE = new SugarParameter("@IN_TYPE", TableName);
                var OUT_RES = new SugarParameter("@OUT_RES", null, true);//isOutput=true
                var excDt = db.Ado.UseStoredProcedure().GetDataTable("SFC.GET_ID", IN_BU, IN_TYPE, OUT_RES);
                string newID = OUT_RES.Value.ToString();
                if (newID.StartsWith("ERR"))
                {
                    throw new Exception("Get table'" + TableName + "' ID exception! " + newID);
                }
                return newID;
            }
        }

        /// <summary>
        /// Get Table seqId
        /// </summary>
        /// <param name="DbStr"></param>
        /// <param name="Bu"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static string GetNewID(SqlSugarClient db, string Bu, string TableName)
        {
            var IN_BU = new SugarParameter("@IN_BU", Bu);
            var IN_TYPE = new SugarParameter("@IN_TYPE", TableName);
            var OUT_RES = new SugarParameter("@OUT_RES", null, true); //isOutput=true
            var excDt = db.Ado.UseStoredProcedure().GetDataTable("SFC.GET_ID", IN_BU, IN_TYPE, OUT_RES);
            string newID = OUT_RES.Value.ToString();
            if (newID.StartsWith("ERR"))
            {
                throw new Exception("Get table '" + TableName + "' ID exception! " + newID);
            }
            return newID;
        }

        public static string GetNewWorkorder(SqlSugarClient db, string seqName)
        {
            var IN_SEQ_NAME = new SugarParameter("@IN_SEQ_NAME", seqName);
            var OUT_SEQNO = new SugarParameter("@OUT_SEQNO", null, true); //isOutput=true
            var excDt = db.Ado.UseStoredProcedure().GetDataTable("SFC.GET_SEQNO_SP", IN_SEQ_NAME, OUT_SEQNO);
            string newWO = OUT_SEQNO.Value.ToString();
            if (newWO.StartsWith("ERR"))
            {
                throw new Exception("Get sequence JNP711VWO exception! ");
            }
            return newWO;
        }

        /// <summary>
        /// Get Table seqId
        /// </summary>
        /// <param name="DbStr"></param>
        /// <param name="Bu"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static string GetNewID<T>(SqlSugarClient db, string Bu)
        {
            var TableName = typeof(T).ToString();
            TableName = TableName.Substring(TableName.LastIndexOf(".") + 1);
            var IN_BU = new SugarParameter("@IN_BU", Bu);
            var IN_TYPE = new SugarParameter("@IN_TYPE", TableName);
            var OUT_RES = new SugarParameter("@OUT_RES", null, true); //isOutput=true
            var excDt = db.Ado.UseStoredProcedure().GetDataTable("SFC.GET_ID", IN_BU, IN_TYPE, OUT_RES);
            string newID = OUT_RES.Value.ToString();
            if (newID.StartsWith("ERR"))
            {
                throw new Exception("Get table '" + TableName + "' ID exception! " + newID);
            }
            return newID;
        }
        
        /// <summary>
        /// Get Table seqId
        /// </summary>
        /// <param name="DbStr"></param>
        /// <param name="Bu"></param>
        /// <param name="TableName"></param>
        /// <returns></returns>
        public static DateTime GetOraDbTime(SqlSugarClient db)
        {
            return db.Ado.GetDateTime("select sysdate from dual");
        }
    }
}
