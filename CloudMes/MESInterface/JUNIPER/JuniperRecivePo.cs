using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESJuniper.OrderManagement;

namespace MESInterface.JUNIPER
{
    public class JuniperRecivePo :testTesk
    {
        public string bu,dbstr;
        public bool IsRuning = false;
        public MESJuniper.OrderManagement.JuniperRecivePo ReceivePo;

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
            dbstr = ConfigGet("DB");
            ReceivePo = new MESJuniper.OrderManagement.JuniperRecivePo(dbstr, bu);
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
            ReceivePo.Run();
        }
    }
}
