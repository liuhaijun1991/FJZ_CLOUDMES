using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Common;
using System.Data;
using System.IO;


namespace MESInterface.DCN
{
    public class InterfaceTest : taskBase
    {
        public bool IsRuning = false;
        private string _fromDb, _toDb;

        public override void init()
        {
            try
            {
                _fromDb = ConfigGet("FROMDB");
                _toDb = ConfigGet("TODB");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                Dictionary<string, string> listresult = new Dictionary<string, string>();
                var fromDb = MESDBHelper.OleExec.GetSqlSugarClient(this._fromDb, false, SqlSugar.DbType.SqlServer);
                var toDb = MESDBHelper.OleExec.GetSqlSugarClient(this._toDb, false);
                var sql = $@"select sysserialno as sn from mfworkstatus where sysserialno in (SELECT A.topasssn as SN
                              FROM asShipped A(NOLOCK)
                             WHERE EXISTS
                             (SELECT TOP 1 1
                                      FROM R_DN_BROCADE B(NOLOCK)
                                     WHERE A.shiporderno = B.SHIP_ORDER
                                       AND B.DN_NO in
                                           (SELECT DN_NO
                                              FROM R_DN_BROCADE A WITH(NOLOCK)
                                             WHERE ORDER_NO IN
                                                   (SELECT ORDERNO
                                                      FROM SDBROORDERMASTER WITH(NOLOCK)
                                                     WHERE CUSTOMERNAME in ('Houston', 'RMA'))
                                               AND WORKTIME > '2021-01-01 00:00:01'
                                               AND EXISTS (SELECT TOP 1 1
                                                      FROM SERVICELOG C WITH(NOLOCK)
                                                     WHERE FUNCTIONTYPE = 'TranDataFTX'
                                                       AND DATA1 = DN_NO)
                                               AND NOT EXISTS (SELECT TOP 1 1
                                                      FROM SDSHIPSKU D WITH(NOLOCK)
                                                     WHERE D.SHIPMENTFINISH = 0
                                                       AND A.SHIP_ORDER = D.SHIPORDERNO)
                                               AND EXISTS
                                             (SELECT TOP 1 1
                                                      FROM SDSHIPSKU D WITH(NOLOCK)
                                                     WHERE D.SHIPMENTFINISH = 1
                                                       AND A.SHIP_ORDER = D.SHIPORDERNO)))) 
and substring(sysserialno,1,2) <> 'RW' and substring(sysserialno,1,1) <> '~' and substring(sysserialno,1,1) <> '#' and substring(sysserialno,1,1) <> '*' and substring(sysserialno,1,1) <> '-'";
                var dt = fromDb.Ado.GetDataTable(sql);
                foreach(DataRow r in dt.Rows)
                {
                    sql = $@"select sysserialno from mfworkstatus where sysserialno = '{r["SN"].ToString()}'";
                    if (toDb.Ado.GetDataTable(sql).Rows.Count == 0)
                    {
                        listresult.Add(r["SN"].ToString(), r["SN"].ToString());
                    }
                }
                foreach(var r in listresult)
                {
                    RecodeLocalLog(r.Value);
                }

            }
            catch (Exception ex)
            {
                MesLog.Info($@"Error:{ex.Message}");
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        private void RecodeLocalLog(string msg)
        {
            string logPath = System.IO.Directory.GetCurrentDirectory() + "\\SN\\";
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            string logFile = System.IO.Directory.GetCurrentDirectory() + "\\SN\\SN_" + DateTime.Now.ToString("yyyyMMdd") + ".log";
            FileStream fs = new FileStream(logFile, FileMode.Append, FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.GetEncoding("BIG5"));
            //通过指定字符编码方式可以实现对汉字的支持，否则在用记事本打开查看会出现乱码            
            sw.Flush();
            sw.BaseStream.Seek(0, SeekOrigin.Current);
            if (msg == "")
            {
                sw.WriteLine();
            }
            else
            {
                sw.WriteLine(msg);
            }
            sw.Flush();
            sw.Close();
        }

    }
}

