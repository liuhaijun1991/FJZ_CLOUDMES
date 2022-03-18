using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.HWD
{
    public class R7b5PlanData: MesAPIBase
    {
        protected APIInfo FGet7B5PlanData = new APIInfo()
        {
            FunctionName = "Get7B5PlanData",
            Description = "獲取7B5Plan的數據",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() {
            }
        };
        public R7b5PlanData()
        {
            this.Apis.Add(FGet7B5PlanData.FunctionName, FGet7B5PlanData);
        }

        public void Get7B5PlanData(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            //OleExec oleDB = null;
            //try
            //{
            //     oleDB = DBPools["SFCDB"].Borrow();
            //    var res = oleDB.ORM.Queryable<R_7B5_PLAN>().OrderBy(t => t.LASTEDITDT).ToList();
            //    if (res == null)
            //    {
            //        StationReturn.Status = StationReturnStatusValue.Fail;
            //        StationReturn.MessageCode = "MES00000034";
            //        StationReturn.Data = new object();
            //    }
            //    else
            //    {
            //        StationReturn.Status = StationReturnStatusValue.Pass;
            //        StationReturn.MessageCode = "MES00000033";
            //        StationReturn.MessagePara.Add(1);
            //        StationReturn.Data = res;
            //    }
            //}
            //catch (Exception exception)
            //{
            //    throw exception;
            //}
            //finally
            //{
            //    this.DBPools["SFCDB"].Return(oleDB);
            //}

            OleExec SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                string sql = "select distinct a.item,c.sku_name MODEL,case when a.day1<0 then  a.day1 + b.day1 else a.day1 end day1, case when a.day2<0 then  a.day2 + b.day2 else a.day2 end day2, ";
                sql = sql + "case when a.day3<0 then  a.day3 + b.day3 else a.day3 end day3,case when a.day4<0 then  a.day4 + b.day4 else a.day4 end day4, ";
                sql = sql + "case when a.day5<0 then  a.day5 + b.day5 else a.day5 end day5,case when a.day6<0 then  a.day6 + b.day6 else a.day6 end day6, ";
                sql = sql + "case when a.day7<0 then  a.day7 + b.day7 else a.day7 end day7,case when a.day8<0 then  a.day8 + b.day8 else a.day8 end day8, ";
                sql = sql + "case when a.day9<0 then  a.day9 + b.day9 else a.day9 end day9,case when a.day10<0 then  a.day10 + b.day10 else a.day10 end day10, ";
                sql = sql + "case when a.day11<0 then  a.day11 + b.day11 else a.day11 end day11,case when a.day12<0 then  a.day12 + b.day12 else a.day12 end day12, ";
                sql = sql + "case when a.day13<0 then  a.day13 + b.day13 else a.day13 end day13,case when a.day14<0 then  a.day14 + b.day14 else a.day14 end day14,A.LASTEDITDT,A.LASTEDITBY  ";
                sql = sql + "from r_7b5_plan a,  r_7b5_plan_temp b, c_sku c where a.item=b.item and a.item=c.cust_partno(+) order by a.item";
                DataTable dt = SFCDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.Data = dt;
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = "";
                }
                this.DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ee)
            {
                this.DBPools["SFCDB"].Return(SFCDB);
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.MessageCode = "MES00000037";
                StationReturn.MessagePara.Add(ee.Message);
                return;
            }

        }
    }
}
