using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.Base
{
    public class CheckProccessRun:ProccessCkeckBase
    {
        public override void Init(C_PROCCESS_CHECK dbdata)
        {
            if (dbdata.CONFIG == null || dbdata.CONFIG.ToString().Trim() == "")
            {
                CheckProccessRunConfig c = new CheckProccessRunConfig();
                Newtonsoft.Json.JsonSerializerSettings setting = new Newtonsoft.Json.JsonSerializerSettings()
                {
                    ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
                };
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(c, setting);
                dbdata.CONFIG = json;
                this.Name = "CheckProccessRun";
            }



        }

        public override void Run(string ProccessName, string Config  , OleExec DB)
        {
            runtime.LastStartTime = DateTime.Now;
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<CheckProccessRunConfig>(Config);
            DateTime now = DateTime.Now;
            //now = DateTime.Parse("2020-08-19 20:30:00");
            DateTime StartTime = now.AddHours(-config.CkeckTimeLoneHours);

            var Events = DB.ORM.Queryable<R_PROCCESS_EVENT>()
                .Where(t => t.PROCCESS_NAME == ProccessName && t.EDIT_DATE > StartTime && t.EDIT_DATE <= now && t.EVENT_TYPE == "START").OrderBy(t => t.EDIT_DATE, SqlSugar.OrderByType.Desc).ToList();
            try
            {
                if (Events.Count < config.CheckStartCount)
                {
                    throw new Exception($@"{ProccessName}{config.CkeckTimeLoneHours}小时内执行了{Events.Count}次,小于报警值{config.CheckStartCount}次 ");
                }
                if (Events[0].EVENT_TYPE == "START")
                {
                    var EndEvent = DB.ORM.Queryable<R_PROCCESS_EVENT>().Where(t => t.RUNTIME_ID == Events[0].RUNTIME_ID && t.EVENT_TYPE == "END").First();
                    if (EndEvent == null)
                    {
                        var tl = DateTime.Now - Events[0].EDIT_DATE;
                        if (((TimeSpan)tl).TotalSeconds > config.RunTimeOutSecond)
                        {
                            throw new Exception($@"{ProccessName}最后运行时间{((TimeSpan)tl).TotalSeconds}秒超过了{config.RunTimeOutSecond}秒而没有结束 ");
                        }
                    }
                }

            }
            catch (Exception ee)
            {
                if (runtime.Message != ee.Message)
                {
                    runtime.Message = ee.Message;
                    runtime.MAIL_FLAG = "0";
                    runtime.SMS_FLAG = "0";
                    runtime.Alert_LV = config.AlertLV.ToString();
                    //runtime.Data = data;
                    runtime.AlertTime = DateTime.Now;
                }
                else
                {
                    if (config.AutoResetHouts < (DateTime.Now - (DateTime)(runtime.AlertTime)).TotalHours)
                    {
                        runtime.Message = ee.Message;
                        runtime.MAIL_FLAG = "0";
                        runtime.SMS_FLAG = "0";
                        runtime.Alert_LV = config.AlertLV.ToString();
                        //runtime.Data = data;
                        runtime.AlertTime = DateTime.Now;
                    }
                }
            }
            finally
            {
                var nowTime = DateTime.Now;
                runtime.NextStartTime = nowTime.AddSeconds(config.TimeSpan);
                
            }

        }
    }

    public class CheckProccessRunConfig: CheckConfigBase
    {
        /// <summary>
        /// 檢查時長
        /// </summary>
        public float CkeckTimeLoneHours = 24;
        /// <summary>
        /// 運行次數
        /// </summary>
        public int CheckStartCount = 2;
        /// <summary>
        /// 檢查超時時間
        /// </summary>
        public int RunTimeOutSecond = 3600;
       
    }
}
