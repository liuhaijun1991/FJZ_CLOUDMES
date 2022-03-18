using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESDBHelper;

namespace MESInterface.Base
{
    public class CheckEventLog : ProccessCkeckBase
    {
        public override void Init(C_PROCCESS_CHECK dbdata)
        {
            if (dbdata.CONFIG == null || dbdata.CONFIG.ToString().Trim() == "")
            {
                CheckEventLogConfig c = new CheckEventLogConfig();
                Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(c, setting);
                dbdata.CONFIG = json;
                this.Name = "CheckEventLog";
            }



        }
        public override void Run(string ProccessName, string Config,  OleExec DB)
        {
            runtime.LastStartTime = DateTime.Now;
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckEventLogConfig>(Config);

            DateTime now = DateTime.Now;
            now = now.AddHours(-config.CkeckTimeLoneHours);

            var Events = DB.ORM.Queryable<R_PROCCESS_EVENT>()
                .Where(t => t.PROCCESS_NAME == ProccessName && t.EDIT_DATE > now && t.EVENT_TYPE == "ERR").OrderBy(t => t.EDIT_DATE, SqlSugar.OrderByType.Desc).ToList();
            List<string> data = new List<string>();
            try
            {
                if (Events.Count != 0)
                {
                    for (int i = 0; i < Events.Count; i++)
                    {
                        data.Add(Events[i].EDIT_DATE.ToString() + ":"+Events[i].MESSAGE);
                        if (double.Parse(runtime.Alert_LV) < Events[i].EVENT_LV)
                        {
                            runtime.Alert_LV = Events[i].EVENT_LV.ToString();
                        }
                    }
                    throw new Exception($@"{ProccessName}最运行时发生异常! ");
                }

            }
            catch (Exception ee)
            {
                if (runtime.Message != ee.Message)
                {
                    runtime.Message = ee.Message;
                    runtime.MAIL_FLAG = "0";
                    runtime.SMS_FLAG = "0";
                    //runtime.Alert_LV = config.AlertLV.ToString();
                    runtime.Data = data;
                    runtime.AlertTime = DateTime.Now;
                }
                else
                {
                    if (config.AutoResetHouts < (DateTime.Now - (DateTime)(runtime.AlertTime)).TotalHours)
                    {
                        runtime.Message = ee.Message;
                        runtime.MAIL_FLAG = "0";
                        runtime.SMS_FLAG = "0";
                        //runtime.Alert_LV = config.AlertLV.ToString();
                        runtime.Data = data;
                        runtime.AlertTime = DateTime.Now;
                    }
                }
            }
            finally
            {
                runtime.NextStartTime = DateTime.Now.AddSeconds(config.TimeSpan);

            }

        }
    }
    public class CheckEventLogConfig : CheckConfigBase
    {
        /// <summary>
        /// 檢查時長
        /// </summary>
        public float CkeckTimeLoneHours = 24;
        
    }
}
