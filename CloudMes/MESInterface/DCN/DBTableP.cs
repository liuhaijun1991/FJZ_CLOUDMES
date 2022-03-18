using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using HWDNNSFCBase;

namespace MESInterface.DCN
{
    internal class DBTableP
    {
        public string TableName;
        public HWDNNSFCBase.OleExec DBExec;
        public List<string> ColNames;
        public Dictionary<string, Type> ColDataType;
        public DBTYPE myDBTYPE = DBTYPE.ORA;
        static HWDNNSFCBase.LogManagedItem LOG = new HWDNNSFCBase.LogManagedItem("INSERT_SQL", HWDNNSFCBase.LogMode.File);
        static HWDNNSFCBase.LogManagedItem INSERT_Err_log = new HWDNNSFCBase.LogManagedItem("INSERT_Err", HWDNNSFCBase.LogMode.File);
        public static void setLogName(string name)
        {
            LOG = new HWDNNSFCBase.LogManagedItem(name + "_INSERT_SQL", HWDNNSFCBase.LogMode.File);
            INSERT_Err_log = new HWDNNSFCBase.LogManagedItem(name + "_INSERT_Err", HWDNNSFCBase.LogMode.File);
        }

        public DBTableP(string tableName, HWDNNSFCBase.OleExec _DBExec)
        {
            DBExec = _DBExec;
            TableName = tableName;
            analyse();
        }
        public DataSet GetData(List<SQLFilter> filters)
        {
            DataSet ret;
            string strSQL = string.Format("select * from {0} where 1=1", TableName);
            for (int i = 0; i < filters.Count; i++)
            {
                strSQL += " AND " + filters[i].ToString();
            }
            ret = DBExec.RunSelect(strSQL);
            return ret;
            //throw new Exception();
        }
        public void InsertData(DataTable insertData)
        {
            //string strsql = "insert ({0}) into {1} values ({2}) ";
            foreach (DataRow dr in insertData.Rows)
            {
                string valueName = "";
                string values = "";
                foreach (string name in this.ColNames)
                {
                    string tmp = "";
                    try
                    {
                        tmp = dr[name].ToString();
                    }
                    catch
                    {
                        continue;
                    }
                    if (values.Length > 0)
                    {
                        values += ",";
                        valueName += ",";
                    }
                    valueName += name;

                    if (this.ColDataType[name] == typeof(System.String))
                    {
                        values += ("'" + tmp.Replace("'", "''") + "'");
                    }
                    else if (this.ColDataType[name] == typeof(System.DateTime))
                    {
                        //to_date('19-04-2011 09:53:18', 'dd-mm-yyyy hh24:mi:ss')
                        try
                        {
                            DateTime datetime = (DateTime)dr[name];
                            if (myDBTYPE == DBTYPE.ORA)
                            {
                                values += ("to_date('" + datetime.ToString("dd-MM-yyyy HH:mm:ss.fff") + "', 'dd-mm-yyyy hh24:mi:ss')");
                            }
                            else if (myDBTYPE == DBTYPE.SQLSERVER)
                            {
                                values += "'" + datetime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                            }
                        }
                        catch
                        {
                            values += "''";
                        }

                    }
                    else if (this.ColDataType[name] == typeof(System.Boolean))
                    {
                        try
                        {
                            if (Boolean.Parse(tmp))
                            {
                                values += "1";
                            }
                            else
                            {
                                values += "0";
                            }
                        }
                        catch
                        {
                            if (tmp == "0" || tmp == "1")
                            {
                                values += tmp;
                            }
                            else
                            {
                                values += "null";
                            }
                        }

                    }
                    else
                    {
                        if (tmp.Trim() == "")
                        {
                            tmp = "null";
                        }
                        values += tmp;
                    }
                }
                string strSQL = string.Format("insert into {0} ({1}) values ({2})", this.TableName, valueName, values);
                try
                {
                    string strinster = DBExec.ExecSQL(strSQL);
                    try
                    {
                        int o = Int32.Parse(strinster);
                    }
                    catch
                    {
                        //HWDNNSFCBase.LogManagedItem Log = new HWDNNSFCBase.LogManagedItem("INSERT Err:", HWDNNSFCBase.LogMode.File);
                        lock (LOG)
                        {
                            LOG.Write(strinster);
                        }
                    }
                }
                catch (Exception ee)
                {
                    lock (INSERT_Err_log)
                    {
                        string strlog = "INSERT INTO servicelog(FUNCTIONTYPE,CURRENTEDITTIME,DATA1,DATA2,DATA9) VALUES ('SendCZData',GETDATE(),'INSERT_Err_log','" + ee.Message + "','N')";
                        DBExec.ExecSQL(strlog);
                        INSERT_Err_log.Write(ee.Message + "\r\n" + strSQL);

                    }
                }

            }

        }
        public void analyse()
        {
            string strsql = string.Format("select * from {0} where 1>1", TableName);

            DataSet ds;
            try
            {
                ds = DBExec.RunSelect(strsql);
            }
            catch (Exception e)
            {
                throw e;
            }
            DataTable dt = ds.Tables[0];
            ColNames = new List<string>();
            ColDataType = new Dictionary<string, Type>();
            foreach (DataColumn dc in dt.Columns)
            {
                ColNames.Add(dc.ColumnName);
                ColDataType.Add(dc.ColumnName, dc.DataType);
            }

        }

        public string GetInsertSql(DataRow dr)
        {
            string valueName = "";
            string values = "";
            foreach (string name in this.ColNames)
            {
                string tmp = "";
                try
                {
                    tmp = dr[name].ToString();
                }
                catch
                {
                    continue;
                }
                if (values.Length > 0)
                {
                    values += ",";
                    valueName += ",";
                }
                valueName += name;

                if (this.ColDataType[name] == typeof(System.String))
                {
                    values += ("'" + tmp.Replace("'", "''") + "'");
                }
                else if (this.ColDataType[name] == typeof(System.DateTime))
                {
                    //to_date('19-04-2011 09:53:18', 'dd-mm-yyyy hh24:mi:ss')
                    try
                    {
                        //DateTime datetime = (DateTime)dr[name];
                        DateTime datetime = Convert.ToDateTime(tmp);
                        if (myDBTYPE == DBTYPE.ORA)
                        {
                            values += ("to_date('" + datetime.ToString("dd-MM-yyyy HH:mm:ss.fff") + "', 'dd-mm-yyyy hh24:mi:ss')");
                        }
                        else if (myDBTYPE == DBTYPE.SQLSERVER)
                        {
                            values += "'" + datetime.ToString("yyyy-MM-dd HH:mm:ss.fff") + "'";
                        }
                    }
                    catch
                    {
                        values += "''";
                    }

                }
                else if (this.ColDataType[name] == typeof(System.Boolean))
                {
                    try
                    {
                        if (Boolean.Parse(tmp))
                        {
                            values += "1";
                        }
                        else
                        {
                            values += "0";
                        }
                    }
                    catch
                    {
                        if (tmp == "0" || tmp == "1")
                        {
                            values += tmp;
                        }
                        else
                        {
                            values += "null";
                        }
                    }

                }
                else
                {
                    if (tmp.Trim() == "")
                    {
                        tmp = "null";
                    }
                    values += tmp;
                }
            }
            return  string.Format("insert into {0} ({1}) values ({2})", this.TableName, valueName, values);
        }
        public enum DBTYPE
        {
            SQLSERVER,
            ORA
        }
    }
}