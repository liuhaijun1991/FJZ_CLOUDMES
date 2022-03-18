using MESJuniper.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.JUNIPER
{
    public class EcoProcess : testTesk
    {
        public string bu, dbstr;
        public bool IsRuning = false;

        public override void init()
        {
            InitPara();
        }

        /// <summary>
        /// 加載配置參數
        /// </summary>
        void InitPara()
        {
            bu = ConfigGet("BU");
            dbstr = ConfigGet("MESDB");
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("下載正在執行,請稍後再試");
            }
            IsRuning = true;
            run();
            IsRuning = false;
        }

        void run()
        {
            using (var db = MESDBHelper.OleExec.GetSqlSugarClient(dbstr, false))
            {
                var ecoBase = new EcoBase(db, bu);
                ecoBase.EcoProcess();
            }
        }
    }
}
