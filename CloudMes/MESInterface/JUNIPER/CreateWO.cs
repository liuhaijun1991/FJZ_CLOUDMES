using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESJuniper.OrderManagement;
using MESDataObject.Module;
using System.Data;
using MESDataObject;
using MESPubLab;

namespace MESInterface.JUNIPER
{
    public class CreateWO:taskBase
    {
        public string bu, plant, dbstr;
        public bool IsRuning = false;
        public MESPubLab.SAP_RFC.ZCPP_NSBG_0129 ZCPP_NSBG_0129;
        public MESPubLab.SAP_RFC.ZCPP_NSBG_0130 ZCPP_NSBG_0130;
        public JuniperCreateWo juniperCreateWo;
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
            plant = ConfigGet("PLANT");
            dbstr = ConfigGet("DB");
            Output.UI = new CreateWO_UI(this);
            try
            {
                ZCPP_NSBG_0129 = new MESPubLab.SAP_RFC.ZCPP_NSBG_0129(bu);
                ZCPP_NSBG_0130 = new MESPubLab.SAP_RFC.ZCPP_NSBG_0130(bu);
                //juniperCreateWo = new JuniperCreateWo(dbstr,bu, plant, ZCPP_NSBG_0129, ZCPP_NSBG_0130);
            }
            catch (Exception)
            {

            }
            Output.Tables.Add(ZCPP_NSBG_0129.GetTableValue("RETURN"));
            Output.Tables.Add(ZCPP_NSBG_0130.GetTableValue("RETURN"));
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("The task is in progress. Please try again later...");
            }            
            IsRuning = true;
            try
            {
                juniperCreateWo.Run();
            }
            catch
            {
            }
            IsRuning = false;
        }      
       
    }
}
