using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlSugar;

namespace MESDBHelper
{
    public static class MesDbEx
    {
        public static List<SqlFuncExternal> MesSqlFuncEx()
        {
            var expMethods = new List<SqlFuncExternal>();
            expMethods.Add(OracleContainLimitEx());
            return expMethods;
        }

        public static SqlFuncExternal OracleContainLimitEx()
        {
            return new SqlFuncExternal()
            {
                UniqueMethodName = "OracleContain",
                MethodValue = (expInfo, dbType, expContext) =>
                {
                    var s = expContext;
                    if (dbType == DbType.Oracle)
                    {
                        var tartgetList = (List<string>)expInfo.Args[1].MemberValue;
                        var sqlex = " 1 = 1 ";
                        for (int i = 1; i <= tartgetList.Count; i++)
                        {
                            if (i % 999 == 1 && i / 999 == 0)
                                sqlex += " and (";

                            if (i % 999 == 1)
                            {
                                if (i / 999 > 0 )
                               //     if (i / 999 > 0 && i != tartgetList.Count)
                                        sqlex += ") or ";
                                sqlex += $@"  {expInfo.Args[0].MemberName} in ('{tartgetList[i - 1].ToString()}' ";
                            }
                            else
                                sqlex += $@" ,'{tartgetList[i - 1].ToString()}'";
                        }
                        if (tartgetList.Count>0)
                            sqlex += ")) ";
                        return sqlex;
                    }
                    else
                        throw new Exception("未实现");
                }
            };
        }
    }
}
