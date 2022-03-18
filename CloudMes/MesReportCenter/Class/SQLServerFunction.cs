using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesReportCenter.Class
{
    public class SQLServerFunction : I_ToSql
    {
        List<string> havingKeys = new List<string>
            {"COUNT","MIN","MAX","" };
        public string From(DBQuery dbQuery)
        {
            var Tables = dbQuery.UseTables;
            StringBuilder txt = new StringBuilder("From ");
            if (dbQuery.Joins.Count == 0)
            {
                for (int i = 0; i < Tables.Count; i++)
                {
                    txt.Append($@"{Tables[i].Table.DBTableName} {Tables[i].Alias}");
                    if (i < Tables.Count - 1)
                    {
                        txt.Append($@",");
                    }
                }
            }
            else
            {
                var j = dbQuery.Joins[0];
                var t1 = dbQuery.UseTables.Find(t => t.Alias == j.Table1);
                var t2 = dbQuery.UseTables.Find(t => t.Alias == j.Table2);
                txt.Append($@"{t1.Table.DBTableName} {t1.Alias} {j.Type} {t2.Table.DBTableName} {t2.Alias} ON ");

                for (int i = 0; i < dbQuery.Joins[0].Data.Count; i++)
                {
                    txt.Append($@" {t1.Alias}.{dbQuery.Joins[0].Data[i].Col1} {dbQuery.Joins[0].Data[i].Operator} {t2.Alias}.{dbQuery.Joins[0].Data[i].Col2}");
                    if (i < dbQuery.Joins[0].Data.Count - 1)
                    {
                        txt.Append(" AND ");
                    }

                }

                List<string> joinTables = new List<string>();
                joinTables.Add(t1.Alias);
                joinTables.Add(t2.Alias);
                List<JoinType> notProccess = new List<JoinType>();
                for (int i = 1; i < dbQuery.Joins.Count; i++)
                {
                    notProccess.Add(dbQuery.Joins[i]);
                }
                //while (notProccess.Count != 0)
                //{
                //    for (int i = 0; i < notProccess.Count; i++)
                //    {
                //        if(joinTables.Contains(notProccess[i].Table1) ) 
                //    }
                //}
            }
            return txt.ToString();
        }

        public string GroupBy(DBQuery dbQuery)
        {
            var paras = dbQuery.Paras;
            var outputs = paras.FindAll(t => t.GroupBy == "Group By");
            StringBuilder txt = new StringBuilder(" Group By ");
            var m = true;
            for (int i = 0; i < outputs.Count; i++)
            {
                if (outputs[i].Filter != null && outputs[i].Filter.Trim() != "")
                {
                    if (outputs[i].Table != null && outputs[i].Table.Trim() != "")
                    {
                        txt.Append($@"{outputs[i].Table}.{outputs[i].Column} {outputs[i].Filter}");
                        m = false;
                    }

                }
            }
            if (m == false)
            {
                return txt.ToString();
            }
            else
            {
                return "";
            }
        }

        public string Having(DBQuery dbQuery)
        {

            var paras = dbQuery.Paras;
            var outputs = paras.FindAll(t => havingKeys.Contains(t.GroupBy) && (t.Filter != null && t.Filter.Trim() != "" ));
            StringBuilder txt = new StringBuilder(" HAVING ");
            var m = true;
            for (int i = 0; i < outputs.Count; i++)
            {
                if (outputs[i].Filter != null && outputs[i].Filter.Trim() != "")
                {
                    if (outputs[i].Table != null && outputs[i].Table.Trim() != "")
                    {
                        txt.Append($@"{outputs[i].GroupBy}( {outputs[i].Table}.{outputs[i].Column}) {outputs[i].Filter}");
                        m = false;
                    }
                }
            }
            if (m == false)
            {
                return txt.ToString();
            }
            else
            {
                return "";
            }
        }

        public string OrderBy(DBQuery dbQuery)
        {
            var paras = dbQuery.Paras;
            var orders = paras.FindAll(t => t.SortType == "DESC" || t.SortType == "ASC");
            StringBuilder txt = new StringBuilder(" ORDER BY ");
            orders.Sort((o1, o2) => 
            {
                if (o1.SortOrder != null && o2.SortOrder != null)
                {
                    if (o1.SortOrder > o2.SortOrder)
                    {
                        return 1;
                    }
                    else if (o1.SortOrder == o2.SortOrder)
                    {
                        return 0;
                    }else
                    {
                        return -1;    
                    }
                }else
                return 0;
            }
            );

            var m = true;
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].SortType == "DESC")
                {
                    txt.Append($@" {orders[i].Table}.{orders[i].Column} DESC");
                    m = false;
                }
                else
                {
                    txt.Append($@" {orders[i].Table}.{orders[i].Column} ");
                    m = false;
                }
                if (i < orders.Count - 1)
                {
                    txt.Append($@",");
                    m = false;
                }
            }
            if (m == false)
            {
                return txt.ToString();
            }
            else
            {
                return "";
            }
        }

        public string Select(DBQuery dbQuery)
        {
            var paras = dbQuery.Paras;
            var outputs = paras.FindAll(t => t.Output == true);
            StringBuilder txt = new StringBuilder("Select ");
            for (int i = 0; i < outputs.Count; i++)
            {
                string a = $@"{paras[i].Table}.{paras[i].Column} ";
                if (havingKeys.Contains(outputs[i].GroupBy))
                {
                    a = $@"{outputs[i].GroupBy}({a.Trim()}) ";
                        
                }

                txt.Append(a);
                if (paras[i].Alias != null && paras[i].Alias.Trim() != "")
                {
                    txt.Append($@"as {paras[i].Alias}");
                }
                if (i < outputs.Count - 1)
                {
                    txt.Append($@",");
                }
            }
            return txt.ToString();
        }

        public string Where(DBQuery dbQuery)
        {
            var paras = dbQuery.Paras;
            var outputs = paras.FindAll(t => t.GroupBy == "Where");
            StringBuilder txt = new StringBuilder(" Where ");
            var m = true;
            for (int i = 0; i < outputs.Count; i++)
            {
                if (outputs[i].Filter != null && outputs[i].Filter.Trim() != "")
                {
                    if (outputs[i].Table != null && outputs[i].Table.Trim() != "")
                    {
                        txt.Append($@"{outputs[i].Table}.{outputs[i].Column} {outputs[i].Filter}");
                        m = false;
                    }
                    
                }
            }
            if (m == false)
            {
                return txt.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
