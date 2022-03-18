using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MESDBHelper;
using MESDataObject.Module;
using System.Data;

namespace MESWebService
{
    public partial class DataService : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            OleExec sfcdb = new OleExec("VERTIVDB", false);
            if (Request["NAME"] == "LINEFAIL")
            {
                DateTime startDT = DateTime.Now.AddDays(-30);
                DateTime endDT = DateTime.Now;
                string dateFrom = $@"to_date('{startDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
                string dateTO = $@"to_date('{endDT.ToString("yyyy/MM/dd HH:mm:ss")}', 'yyyy-MM-dd hh24:mi:ss')";
                string sqlRun = $@"select line ,skuno 料號,input 投入, fail 不良總數,decode(failrate,0,'0',to_char(round(failrate * 100, 2),'fm9999990.9999')) ||'%' as 不良率
                                from (select d.line,d.skuno,count(distinct d.sn) input,count(distinct f.sn) fail,
                                count(distinct f.sn) / count(distinct d.sn) failrate from (
                                    select a.sn, a.skuno, b.line from r_sn_station_detail a
                                    inner join r_sn_station_detail b on a.sn = b.sn
                                    inner join c_sku c on a.skuno = c.skuno
                                    where a.edit_time between {dateFrom} and {dateTO} and a.current_station = 'BIP' and b.current_station = 'AOI1') d
                                    left join (select sn from r_repair_main e where e.create_time between {dateFrom} and {dateTO}) f
                                on d.sn = f.sn group by d.skuno, d.line ) order by line , FAILRATE desc";
                DataSet res = sfcdb.RunSelect(sqlRun);
                string js = DataTableToJson(res.Tables[0]);
                Response.Write(js);

            }
        }

        string DataTableToJson(DataTable dt)
        {
            
            List<Dictionary<string, object>> L = new List<Dictionary<string, object>>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                
                DataRow dr = dt.Rows[i];
                Dictionary<string, object> item = new Dictionary<string, object>();
                L.Add(item);
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    item.Add(dt.Columns[j].ColumnName,dr[dt.Columns[j].ColumnName] );
                }
                
            }
            System.Web.Script.Serialization.JavaScriptSerializer JsonMaker = new System.Web.Script.Serialization.JavaScriptSerializer();
            JsonMaker.MaxJsonLength = int.MaxValue;

            string json = JsonMaker.Serialize(L);
            return json;
        }
    }
}