using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.Vertiv
{
    class DCBarcodeExpiredReport : ReportBase
    {
        ReportInput FromDay = new ReportInput() { Name = "FromDay", InputType = "DateTime", Value = "2018-01-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        ReportInput ToDate = new ReportInput() { Name = "ToDay", InputType = "DateTime", Value = "2018-02-01", Enable = true, SendChangeEvent = false, ValueForUse = null };
        public DCBarcodeExpiredReport()
        {
            Inputs.Add(FromDay);
            Inputs.Add(ToDate);
        }
        public override void Init()
        {
            try
            {
                FromDay.Value = DateTime.Now.AddDays(-365).ToString("yyyy-MM-dd");
                ToDate.Value = DateTime.Now.ToString("yyyy-MM-dd");
            }
            catch (Exception ex)
            {
                Outputs.Add(new ReportAlart(ex.Message));
            }
        }
        public override void Run()
        {
            string fromDate = Convert.ToDateTime(FromDay.Value).ToString("yyyy-MM-dd");
            string toDate = Convert.ToDateTime(ToDate.Value).ToString("yyyy-MM-dd");
            OleExec SFCDB = DBPools["SFCDB"].Borrow();
            try
            {
                string runSql = $@"select distinct aa.*
  from (select workorderno,
               skuno,
               sn,
               next_station,
               value,
               sysdateyyyyww,
               keypartyyyyww,
               case
                 when substr(sysdateyyyyww, 1, 4) =
                      substr(keypartyyyyww, 1, 4) then
                  sysdateyyyyww - keypartyyyyww
                 when substr(sysdateyyyyww, 1, 4) -
                      substr(keypartyyyyww, 1, 4) = 1 then
                  (select (substr(keypartyyyyww, 1, 4) ||
                          ceil((trunc(sysdate) -
                                to_date(substr(keypartyyyyww, 1, 4) || '0101',
                                         'YYYYMMDD') +
                                TO_CHAR(to_date(substr(keypartyyyyww, 1, 4) ||
                                                 '0101',
                                                 'YYYYMMDD'),
                                         'd') +
                                TO_CHAR(TO_DATE(to_char(sysdate, 'YYYY') ||
                                                 '0101',
                                                 'YYYYMMDD'),
                                         'd')) / 7)) - keypartyyyyww
                     from dual)
                 else
                  99
               end ww
          from (select b.workorderno,
                       b.skuno,
                       a.sn,
                       b.next_station,
                       a.value,
                       (select to_char(sysdate, 'YYYY') ||
                               ceil((trunc(sysdate) -
                                    TO_DATE(to_char(sysdate, 'YYYY') ||
                                             '0101',
                                             'YYYYMMDD') +
                                    TO_CHAR(TO_DATE(to_char(sysdate, 'YYYY') ||
                                                     '0101',
                                                     'YYYYMMDD'),
                                             'd')) / 7) ww
                          from dual) sysdateyyyyww,
                       nvl(c.value, '1991') || substr(a.value, 2, 2) keypartyyyyww
                  from r_sn_kp a
                 inner join r_sn b
                    on a.sn = b.sn
                   and b.valid_flag = 1
                  left join c_code_mapping c
                    on c.codetype = '1YEAR_1'
                   and c.codevalue = substr(a.value, 1, 1)
                 where b.next_station <> 'SHIPFINISH'
                   and a.scantype = 'DCBarcode'
                   and a.valid_flag = 1
                   and a.value is not null
                   and a.edit_time between to_date('{fromDate}','yyyy-mm-dd') and to_date('{toDate}','yyyy-mm-dd') )) aa
 where aa.ww > 4
 order by aa.ww desc
";
                DataSet res = SFCDB.RunSelect(runSql);
                ReportTable retTab = new ReportTable();

                retTab.LoadData(res.Tables[0], null);
                retTab.Tittle = "DCBarcode Expired Report";
                Outputs.Add(retTab);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception ex)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw ex;
            }
        }

    }
}
