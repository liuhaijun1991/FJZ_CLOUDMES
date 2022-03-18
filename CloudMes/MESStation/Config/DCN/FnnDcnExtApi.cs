using MESDBHelper;
using MESPubLab.MESStation;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.DCN
{
    public class FnnDcnExtApi : MesAPIBase
    {
        string dcndbstr = "server=10.120.176.101,3000;uid=dcnadmin;pwd=nsdiisfc169!;database=sjefox;";
        string oracledbstr = "server=10.120.246.100,3000;uid=dcnadmin;pwd=nsdiisfc169!;database=sjefox;";
        string vertivdbstr = "Data Source = 10.120.246.140:1527 / VERTIVODB; User ID = TEST; Password = SFCTEST;";

        protected APIInfo FGetIcarDataWithFnnDcn = new APIInfo
        {
            FunctionName = "GetIcarDataWithFnnDcn",
            Description = "GetIcarDataWithFnnDcn",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>()
        };

        public FnnDcnExtApi()
        {
            this.Apis.Add(FGetIcarDataWithFnnDcn.FunctionName, FGetIcarDataWithFnnDcn);
        }

        public void GetIcarDataWithFnnDcn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var floor = Data["Floor"].ToString().Trim();
                var Vertiv = GetIcarDataWithVertiv();
                var Dcn = GetIcarDataWithDcn();
                var Oracle = GetIcarDataWithOracle();
                if (floor.Equals("ALL"))                
                    StationReturn.Data = new { Vertiv = Vertiv, Dcn = Dcn, Oracle = Oracle };
                else if (floor.ToUpper().Equals("VERTIV"))
                    StationReturn.Data = new { Vertiv = Vertiv};
                else if (floor.ToUpper().Equals("DCN"))
                    StationReturn.Data = new { Dcn = Dcn };
                else if (floor.ToUpper().Equals("ORACLE"))
                    StationReturn.Data = new { Oracle = Oracle };
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }


        public void GetIcarFailDataWithFnnDcn(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JToken Data, MESStationReturn StationReturn)
        {
            var SFCDB = this.DBPools["SFCDB"].Borrow();
            try
            {
                var floor = Data["Floor"].ToString().Trim();
                var Vertiv = GetIcarFailDataWithVertiv();
                var Dcn = GetIcarFailDataWithDcn();
                var Oracle = GetIcarFailDataWithOracle();
                if (floor.Equals("ALL"))
                    StationReturn.Data = new { Vertiv = Vertiv, Dcn = Dcn, Oracle = Oracle };
                else if (floor.ToUpper().Equals("VERTIV"))
                    StationReturn.Data = new { Vertiv = Vertiv };
                else if (floor.ToUpper().Equals("DCN"))
                    StationReturn.Data = new { Dcn = Dcn };
                else if (floor.ToUpper().Equals("ORACLE"))
                    StationReturn.Data = new { Oracle = Oracle };
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception ee)
            {
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ee.Message;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(SFCDB);
            }
        }


        DataTable GetIcarFailDataWithDcn()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dcndbstr, SqlSugar.DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"
                            select 'B04-1' made,'DCN' buname,'OBA' type,CONVERT (char(10),GETDATE()-1,121) as createdate,right(sysserialno,8) sysserialno,GETDATE() lasteditdt  
                             from sfclotdetail where lotno like 'ltn%' and errcode<>''
                            and  createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and dateadd(hh,7,CONVERT (char(10),GETDATE(),121))
                            ");
                return res;
            }
        }

        DataTable GetIcarFailDataWithOracle()
        {
            using (var db = OleExec.GetSqlSugarClient(this.oracledbstr, SqlSugar.DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"
                           select 'B08-3' made,'DCN' buname,'OBA' type,CONVERT (char(10),GETDATE()-1,121) as createdate,right(sysserialno,8) sysserialno,GETDATE() lasteditdt  
                             from sfclotdetail where lotno like 'ltn%' and errcode<>''
                            and  createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and dateadd(hh,7,CONVERT (char(10),GETDATE(),121))");
                return res;
            }
        }


        DataTable GetIcarFailDataWithVertiv()
        {
            using (var db = OleExec.GetSqlSugarClient(this.vertivdbstr))
            {
                var res = db.Ado.GetDataTable($@"SELECT 'B08-1' made,'DCN' buname,'OBA' type,to_char(sysdate-1,'YYYY-MM-DD') as createdate,SN,SYSDATE lasteditdt 
                     FROM R_LOT_DETAIL where  ((STATUS='0' AND PALLET_NO in ('SNSAMPLINGCOMPLETED','WAITOBA')) OR STATUS='2') and create_date between TO_DATE(to_char(sysdate-1,'YYYY-MM-DD') || ' 07:00:00','YYYY-MM-DD HH24:MI:SS') 
                                                AND TO_DATE(to_char(sysdate,'YYYY-MM-DD') || ' 07:00:00','YYYY-MM-DD HH24:MI:SS')   ");
                return res;
            }
        }

        DataTable GetIcarDataWithDcn()
        {
            using (var db = OleExec.GetSqlSugarClient(this.dcndbstr,  SqlSugar.DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"select 'B04-1' MADE,'DCN' BUNAME,'OBA' type,a.*,b.FailSampleQty,GETDATE()AS lasteditdt from(
                            select  CONVERT (char(10),GETDATE()-1,121) as createdate,sum(b.SampleQty) as SampleQty,sum(lotqty) as TotalBuildQty  from (
                            select lotno,max(createdate) as createdate,sum(lotqty) as lotqty From sfclotstatus(nolock) where lotno like 'LTN%' 
                            and completed='1'  and rework = '0' and undetermined='0' and createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and
                            dateadd(hh,7,CONVERT (char(10),GETDATE(),121))   group by lotno) a , 
                            (select lotno,max(b.skuno) as skuno,max(c.codename) as codename, count(*) as  SampleQty from 
                            sfclotdetail a(nolock),sfcshippack b(nolock),sfccodelike c(nolock)    
                            where a.palletno=b.packno and b.skuno=c.skuno  and a.lotno like 'LTN%' 
                            and (a.createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and
                            dateadd(hh,7,CONVERT (char(10),GETDATE(),121)) )  group by lotno 
                            ) b   where a.lotno=b.lotno  )a 
                            left join 
                            ( 
                            select CONVERT (char(10),GETDATE()-1,121) as createdate,count(1) FailSampleQty  from sfclotdetail where 
                            createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and dateadd(hh,7,CONVERT (char(10),GETDATE(),121)) and 
                            lotno like 'lot%' and errcode<>''     and sysserialno not like '%#%') b 
                            on  a.createdate=b.createdate");
                return res;
            }
        }
        DataTable GetIcarDataWithOracle()
        {
            using (var db = OleExec.GetSqlSugarClient(this.oracledbstr,  SqlSugar.DbType.SqlServer))
            {
                var res = db.Ado.GetDataTable($@"select 'B08-3' MADE,'DCN' BUNAME,'OBA' type,a.*,b.FailSampleQty,GETDATE()AS lasteditdt from(
                            select  CONVERT (char(10),GETDATE()-1,121) as createdate,sum(b.SampleQty) as SampleQty,sum(lotqty) as TotalBuildQty  from (
                            select lotno,max(createdate) as createdate,sum(lotqty) as lotqty From sfclotstatus(nolock) where lotno like 'LTN%' 
                            and completed='1'  and rework = '0' and undetermined='0' and createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and
                            dateadd(hh,7,CONVERT (char(10),GETDATE(),121))   group by lotno) a , 
                            (select lotno,max(b.skuno) as skuno,max(c.codename) as codename, count(*) as  SampleQty from 
                            sfclotdetail a(nolock),sfcshippack b(nolock),sfccodelike c(nolock)    
                            where a.palletno=b.packno and b.skuno=c.skuno  and a.lotno like 'LTN%' 
                            and (a.createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and
                            dateadd(hh,7,CONVERT (char(10),GETDATE(),121)) )  group by lotno 
                            ) b   where a.lotno=b.lotno  )a 
                            left join 
                            ( 
                            select CONVERT (char(10),GETDATE()-1,121) as createdate,count(1) FailSampleQty  from sfclotdetail where 
                            createdate between dateadd(hh,7,CONVERT (char(10),GETDATE()-1,121)) and dateadd(hh,7,CONVERT (char(10),GETDATE(),121)) and 
                            lotno like 'lot%' and errcode<>''     and sysserialno not like '%#%') b 
                            on  a.createdate=b.createdate");
                return res;
            }
        }

        DataTable GetIcarDataWithVertiv()
        {
            using (var db = OleExec.GetSqlSugarClient(this.vertivdbstr))
            {
                var res = db.Ado.GetDataTable($@"SELECT 'B08-1' made,'DCN' BUNAME,'OBA'  ""type"",to_char( sysdate,'YYYY-MM-DD') createdate ,sum(sample_qty) SampleQty, sum(lot_qty) TotalBuildQty,sum(fail_qty) FailSampleQty,sysdate lasteditdt
                        FROM r_lot_status WHERE edit_time between TO_DATE(to_char(sysdate - 1, 'YYYY-MM-DD'), 'YYYY-MM-DD') AND TO_DATE(to_char(sysdate, 'YYYY-MM-DD'), 'YYYY-MM-DD') ");
                return res;
            }
        }
    }
}
