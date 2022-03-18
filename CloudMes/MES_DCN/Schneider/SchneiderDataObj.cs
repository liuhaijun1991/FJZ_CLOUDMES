using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Common;
using System.Data;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using MESDataObject.Module;

namespace MES_DCN.Schneider
{
    public class SchneiderDataObj
    {
        private string _mesdbstr, _customerdbstr, _bustr, _seriesname, _days, _siSn;
        public SchneiderDataObj(string mesdbstr, string customerdbstr, string bustr, string seriesname, string days, string siSn)
        {
            _mesdbstr = mesdbstr;
            _customerdbstr = customerdbstr;
            _bustr = bustr;
            _seriesname = seriesname;
            _days = days;
            _siSn = siSn;
        }
        public void Build()
        {
            this.SendPlanData();
        }
        void SendPlanData()
        {
            try
            {
                DataTable dt = null;
                var sql = "";

                //insert PCBA SN
                using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient(this._mesdbstr, false))
                {
                    sql = $@"select distinct a.sn, a.skuno, c.description
                              from r_sn a
                             inner join r_wo_base b
                                on a.workorderno = b.workorderno
                             inner join c_sku c
                                on b.skuno = c.skuno
                               and b.sku_ver = c.version
                             inner join c_series d
                                on c.c_series_id = d.id
                             where a.next_station = 'JOBFINISH'
                               and a.edit_time > sysdate - {_days}
                               and d.description in ({_seriesname})
                               and c.sku_type = 'PCBA'
                               and a.valid_flag = '1'";
                    dt = mesdb.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
                        {
                            foreach (DataRow r in dt.Rows)
                            {
                                sql = $@"select * from [dbo].[PCBA_Master] where PCBA_SerialNumber = '{r["sn"].ToString()}'";
                                if (customerdb.Ado.GetDataTable(sql).Rows.Count == 0)
                                {
                                    sql = $@"insert into [dbo].[PCBA_Master](PCBA_SerialNumber,PCBA_PartNo,PCBA_Part_Description) values ('{r["sn"].ToString()}','{r["skuno"].ToString()}','{r["description"].ToString()}')";
                                    customerdb.Ado.ExecuteCommand(sql);
                                }
                            }
                        }
                    }
                }

                //insert MODEL WO
                using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient(this._mesdbstr, false))
                {
                    sql = $@"select distinct a.workorderno, a.skuno, a.workorder_qty, b.description
                              from r_wo_base a
                             inner join c_sku b
                                on a.skuno = b.skuno
                               and a.sku_ver = b.version
                             inner join c_series c
                                on b.c_series_id = c.id
                             where c.description in ({_seriesname})
                               and b.sku_type = 'MODEL'
                               and a.download_date > sysdate - {_days}";
                    dt = mesdb.Ado.GetDataTable(sql);
                    if (dt.Rows.Count > 0)
                    {
                        using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
                        {
                            foreach (DataRow r in dt.Rows)
                            {
                                sql = $@"select * from [dbo].[Plan] where workorderno = '{r["workorderno"].ToString()}'";
                                if (customerdb.Ado.GetDataTable(sql).Rows.Count == 0)
                                {
                                    sql = $@"insert into [dbo].[Plan](skuno,skunodescription,workorderno,quantity) values ('{r["skuno"].ToString()}','{r["description"].ToString()}','{r["workorderno"].ToString()}','{r["workorder_qty"].ToString()}')";
                                    customerdb.Ado.ExecuteCommand(sql);
                                }
                                else
                                {
                                    sql = $@"select * from [dbo].[Plan] where workorderno = '{r["workorderno"].ToString()}' and quantity <> '{r["workorder_qty"].ToString()}'";
                                    if (customerdb.Ado.GetDataTable(sql).Rows.Count > 0)
                                    {
                                        sql = $@"update [dbo].[Plan] set quantity = '{r["workorder_qty"].ToString()}' where workorderno = '{r["workorderno"].ToString()}'";
                                        customerdb.Ado.ExecuteCommand(sql);
                                    }
                                }
                            }
                        }
                    }
                }

                //Get Schneider DB SI SN
                if (_siSn == "Y")
                {
                    using (var customerdb = MESDBHelper.OleExec.GetSqlSugarClient(this._customerdbstr, false, SqlSugar.DbType.SqlServer))
                    {
                        sql = $@"select * from [dbo].[PCBA_Trace]";
                        dt = customerdb.Ado.GetDataTable(sql);
                        if (dt.Rows.Count > 0)
                        {
                            using (var mesdb = MESDBHelper.OleExec.GetSqlSugarClient(this._mesdbstr, false))
                            {
                                foreach (DataRow r in dt.Rows)
                                {
                                    if (!mesdb.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == r["SerialNumber"].ToString().ToUpper().Trim() && t.STATION_NAME == "KIT_PRINT").Any())
                                    {
                                        sql = $@"select * from [dbo].[PCBA_Trace] where PCBA = '{r["PCBA"].ToString().ToUpper().Trim()}' order by id desc";
                                        if (customerdb.Ado.GetDataTable(sql).Rows.Count > 1)
                                        {
                                            MesLog.Info($@"PCBA {r["PCBA"].ToString().ToUpper().Trim()} has multiple data");
                                            continue;
                                        }
                                        R_SN_STATION_DETAIL sd = new R_SN_STATION_DETAIL() { WORKORDERNO = r["OrderNo"].ToString().ToUpper().Trim(), SN = r["SerialNumber"].ToString().ToUpper().Trim(), PRODUCT_STATUS = "JOBFINISH", DEVICE_NAME = r["PCBA"].ToString().ToUpper().Trim(), EDIT_EMP = "SESYSTEM", EDIT_TIME = mesdb.GetDate(), STATION_NAME = "KIT_PRINT" };
                                        mesdb.Insertable<R_SN_STATION_DETAIL>(sd).ExecuteCommand();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MesLog.Info($@"Insert Data Err:{e.Message}");
            }
            finally
            {
                MesLog.Info("Insert Data End");
            }
        }
    }
}
