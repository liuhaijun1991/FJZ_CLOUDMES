using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.DCN
{
    public class OSPBoard : MesAPIBase
    {
        private APIInfo _Get = new APIInfo
        {
            FunctionName = "GET",
            Description = "獲取OSP看板數據",
            Parameters = new List<APIInputInfo>() { },
            Permissions = new List<MESPermission>() { }
        };

        public OSPBoard()
        {
            this.Apis.Add(_Get.FunctionName, _Get);
        }

        public void GET(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec SFCDB = null;
            string SQL = null;
            DataTable resDT = null;
            try
            {
                SFCDB = DBPools["SFCDB"].Borrow();

                #region 超級啰嗦的SQL，從NN網頁COPY
                //SQL = $@"
                //select a.* from (
                //select distinct c.skuno,c.workorderno,cc.qty,
                //       case when d.qty is null then 0 else d.qty end as SMT1_12,
                //       case when e.qty is null then 0 else e.qty end as SMT1_72,
                //       case when f.qty is null then 0 else f.qty end as SMT2_36,
                //       case when g.qty is null then 0 else g.qty end as SMT2_72,
                //       case when h.qty is null then 0 else h.qty end as VI_48,
                //       case when i.qty is null then 0 else i.qty end as VI_72 
                //from (
                //-- 1Y < SMT1 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT1' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 12H < SMT1 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT1' and a.started_flag='1' 
                //and b.edit_time < sysdate - 12/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 1Y < SMT2 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT2' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 36H < SMT2 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT2' and a.started_flag='1' 
                //and b.edit_time < sysdate - 36/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 1Y < VI < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='VI' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 48H < VI < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='VI' and a.started_flag='1' 
                //and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //) c left join (
                //-- 12H < SMT1 < 72H AND SKUNO NOT IN ()
                //select a.skuno,a.workorderno,count(*) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT1' and a.started_flag='1' 
                //and b.edit_time < sysdate - 12/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1'
                //and x.skuno not in ('BU12O339T90','40-1001063-01','B95.1311T00','40-1000533-02','40-1000534-06','40-1000534-01','B95.0605T00','40-1000534-04','A03021GNF-A','40-1000533-05','A40-1000533-07','B95.0990T00','40-1000534-05','B96.1312T00','40-1000534-03','B95.0925T00','B95.1016T00','B96.1392T00','40-1000533-04','40-1000533-06','A40-1000534-06','ER-7000-0594','40-1000533-01','40-1000533-07','B96.1312T01')) 
                //group by a.skuno,a.workorderno
                //) d on c.workorderno=d.workorderno 
                //left join (
                //-- 1Y < SMT1 < 72H AND SKUNO NOT IN ()
                //select a.skuno,a.workorderno,count(*) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT1' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1'
                //and x.skuno not in ('BU12O339T90','40-1001063-01','B95.1311T00','40-1000533-02','40-1000534-06','40-1000534-01','B95.0605T00','40-1000534-04','A03021GNF-A','40-1000533-05','A40-1000533-07','B95.0990T00','40-1000534-05','B96.1312T00','40-1000534-03','B95.0925T00','B95.1016T00','B96.1392T00','40-1000533-04','40-1000533-06','A40-1000534-06','ER-7000-0594','40-1000533-01','40-1000533-07','B96.1312T01')) 
                //group by a.skuno,a.workorderno
                //) e on c.workorderno=e.workorderno 
                //left join (
                //-- 36H < SMT2 < 72H
                //select a.skuno,a.workorderno,count(*) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT2' and a.started_flag='1' 
                //and b.edit_time < sysdate - 36/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //) f on c.workorderno=f.workorderno
                //left join (
                //-- 1Y < SMT2 < 72H
                //select a.skuno,a.workorderno,count(*) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT2' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //) g on c.workorderno=g.workorderno
                //left join (
                //-- 48H < VI < 72H
                //select a.skuno,a.workorderno,count(*) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='VI' and a.started_flag='1' 
                //and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //) h on c.workorderno=h.workorderno
                //left join (
                //-- 1Y < VI < 72H
                //select a.skuno,a.workorderno,count(*) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='VI' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //) i on c.workorderno=i.workorderno
                //left join (
                //select c.skuno,c.workorderno,sum(c.qty) as qty from (
                //-- 1Y < SMT1 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT1' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 12H < SMT1 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT1' and a.started_flag='1' 
                //and b.edit_time < sysdate - 12/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 1Y < SMT2 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT2' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 36H < SMT2 < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='SMT2' and a.started_flag='1' 
                //and b.edit_time < sysdate - 36/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 1Y < VI < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='VI' and a.started_flag='1' 
                //and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //union 
                //-- 48H < VI < 72H
                //select a.skuno,a.workorderno,count(a.sn) as qty 
                //from r_sn a, r_sn_station_detail b 
                //where a.next_station='VI' and a.started_flag='1' 
                //and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                //and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                //and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                //group by a.skuno,a.workorderno
                //) c group by c.skuno, c.workorderno
                //) cc on c.workorderno=cc.workorderno
                //) a, r_wo_base b where a.workorderno=b.workorderno and b.closed_flag='0' order by 3 desc ";
                #endregion

                SQL = $@"
                select a.* from (
                select distinct c.skuno,c.workorderno,cc.qty,
                       case when d.qty is null then 0 else d.qty end as SMT1_12,
                       case when e.qty is null then 0 else e.qty end as SMT1_72,
                       case when f.qty is null then 0 else f.qty end as SMT2_36,
                       case when g.qty is null then 0 else g.qty end as SMT2_72,
                       case when h.qty is null then 0 else h.qty end as VI_48,
                       case when i.qty is null then 0 else i.qty end as VI_72, 
                       case when ict.qty is null then 0 else ict.qty end as ICT_72 
                from (
                -- 1Y < SMT1 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT1' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 12H < SMT1 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT1' and a.started_flag='1' 
                and b.edit_time < sysdate - 12/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 1Y < SMT2 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT2' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 36H < SMT2 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT2' and a.started_flag='1' 
                and b.edit_time < sysdate - 36/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 1Y < VI < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='VI' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 48H < VI < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='VI' and a.started_flag='1' 
                and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                
                 union 
                -- 48H < VI < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='ICT' and a.started_flag='1' 
                and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='ICT' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) c left join (
                -- 12H < SMT1 < 72H AND SKUNO NOT IN ()
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT1' and a.started_flag='1' 
                and b.edit_time < sysdate - 12/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1'
                and x.skuno not in ('BU12O339T90','40-1001063-01','B95.1311T00','40-1000533-02','40-1000534-06','40-1000534-01','B95.0605T00','40-1000534-04','A03021GNF-A','40-1000533-05','A40-1000533-07','B95.0990T00','40-1000534-05','B96.1312T00','40-1000534-03','B95.0925T00','B95.1016T00','B96.1392T00','40-1000533-04','40-1000533-06','A40-1000534-06','ER-7000-0594','40-1000533-01','40-1000533-07','B96.1312T01')) 
                group by a.skuno,a.workorderno
                ) d on c.workorderno=d.workorderno 
                left join (
                -- 1Y < SMT1 < 72H AND SKUNO NOT IN ()
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT1' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1'
                and x.skuno not in ('BU12O339T90','40-1001063-01','B95.1311T00','40-1000533-02','40-1000534-06','40-1000534-01','B95.0605T00','40-1000534-04','A03021GNF-A','40-1000533-05','A40-1000533-07','B95.0990T00','40-1000534-05','B96.1312T00','40-1000534-03','B95.0925T00','B95.1016T00','B96.1392T00','40-1000533-04','40-1000533-06','A40-1000534-06','ER-7000-0594','40-1000533-01','40-1000533-07','B96.1312T01')) 
                group by a.skuno,a.workorderno
                ) e on c.workorderno=e.workorderno 
                left join (
                -- 36H < SMT2 < 72H
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT2' and a.started_flag='1' 
                and b.edit_time < sysdate - 36/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) f on c.workorderno=f.workorderno
                left join (
                -- 1Y < SMT2 < 72H
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT2' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) g on c.workorderno=g.workorderno
                left join (
                -- 48H < VI < 72H
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='VI' and a.started_flag='1' 
                and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) h on c.workorderno=h.workorderno
                left join (
                -- 1Y < VI < 72H
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='VI' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) i on c.workorderno=i.workorderno
                 left join (
                -- 60H < ICT < 72H
                select a.skuno,a.workorderno,count(*) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='ICT' and a.started_flag='1' 
                and b.edit_time < sysdate - 60/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='ICT' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) ict on i.workorderno=ict.workorderno
                left join (
                select c.skuno,c.workorderno,sum(c.qty) as qty from (
                -- 1Y < SMT1 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT1' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 12H < SMT1 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT1' and a.started_flag='1' 
                and b.edit_time < sysdate - 12/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT1' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 1Y < SMT2 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT2' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 36H < SMT2 < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='SMT2' and a.started_flag='1' 
                and b.edit_time < sysdate - 36/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='SMT2' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 1Y < VI < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='VI' and a.started_flag='1' 
                and b.edit_time < sysdate - 72/24 and b.edit_time > add_months(sysdate,-12) and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union 
                -- 48H < VI < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='VI' and a.started_flag='1' 
                and b.edit_time < sysdate - 48/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='VI' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                union
                -- 60H < ICT < 72H
                select a.skuno,a.workorderno,count(a.sn) as qty 
                from r_sn a, r_sn_station_detail b 
                where a.next_station='ICT' and a.started_flag='1' 
                and b.edit_time < sysdate - 60/24 and b.edit_time > sysdate - 72/24 and substr(a.sn,1,1) not in ('#','*','~')
                and a.repair_failed_flag <> '1' and a.valid_flag='1' and a.sn=b.sn and b.station_name='SMTLOADING'
                and not exists(select * from r_sn_station_detail x where a.sn=x.sn and x.station_name='ICT' and x.valid_flag='1') 
                group by a.skuno,a.workorderno
                ) c group by c.skuno, c.workorderno
                ) cc on c.workorderno=cc.workorderno where substr(c.skuno, 1, 2) != '0P'
                ) a, r_wo_base b where a.workorderno=b.workorderno and b.closed_flag='0' order by 3 desc";

                resDT = SFCDB.RunSelect(SQL).Tables[0];
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "MES00000016";
                StationReturn.Data = resDT;

                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                if (SFCDB != null)
                {
                    DBPools["SFCDB"].Return(SFCDB);
                }
                StationReturn.Status = StationReturnStatusValue.Fail;
                StationReturn.Message = ex.Message;
                StationReturn.Data = resDT;
            }
        }
    }
}
