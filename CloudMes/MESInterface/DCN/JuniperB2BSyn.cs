using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.DCN;
using MESDataObject.Module.Juniper;
using MESJuniper.Base;
using MESPubLab.Common;
using SqlSugar;
using static MESDataObject.Common.EnumExtensions;
using MESPubLab.MESInterface;
using MESJuniper.OrderManagement;
using MESPubLab.SAP_RFC;

namespace MESInterface.DCN
{
    public class JuniperB2BSyn : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, b2bdbstr, bustr,functiontype,plantcode, tccode;
        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
                functiontype = ConfigGet("FUNCTIONTYPE");
                b2bdbstr = ConfigGet("LHB2BDB");
                bustr = ConfigGet("BU");
                plantcode = ConfigGet("PLANTCODE");
                tccode = ConfigGet("TCCODE");
            }
            catch (Exception)
            {
            }
        }
        
        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            try
            {
                MesLog.Info("Start");
                var functionlist = functiontype.Split(',');
                Syn137 syn137 = new Syn137(mesdbstr, b2bdbstr, bustr,-1);
                Syn140 syn140 = new Syn140(mesdbstr, b2bdbstr, bustr);
                Syn282 syn282 = new Syn282(mesdbstr, b2bdbstr, bustr);
                Syn285 syn285 = new Syn285(mesdbstr, b2bdbstr, bustr);
                Syn244 syn244 = new Syn244(mesdbstr, b2bdbstr, bustr);
                SynAck synAck = new SynAck(mesdbstr, b2bdbstr, bustr);
                SynCrem synCrem = new SynCrem(mesdbstr, bustr, tccode, plantcode, this);
                SynTest synTest = new SynTest(mesdbstr, b2bdbstr, bustr);
                //synTest.Run();
                JuniperPreWoGanerate JuniperPreWoGanerate = new JuniperPreWoGanerate(mesdbstr, bustr);
                JuniperPreUpoadBom JuniperPreUpoadBom = new JuniperPreUpoadBom(mesdbstr, bustr, plantcode);
                JuniperSecUpoadBom JuniperSecUpoadBom = new JuniperSecUpoadBom(mesdbstr, bustr, plantcode);
                JuniperCreateWo JuniperCreateWo = new JuniperCreateWo(mesdbstr, bustr, plantcode, this);
                JuniperGroupIdReceive JuniperGroupIdReceive = new JuniperGroupIdReceive(mesdbstr, bustr);                
                AddNonBom AddNonBom = new AddNonBom(mesdbstr, bustr, plantcode);
                JuniperSapJob JuniperSapJob = new JuniperSapJob(mesdbstr, bustr);
                if (functionlist.Contains(ServicesFunctionEnum.Syn137.ExtValue()))
                    syn137.Run();
                if (functionlist.Contains(ServicesFunctionEnum.Syn140.ExtValue()))
                    syn140.Run();
                if (functionlist.Contains(ServicesFunctionEnum.Syn285.ExtValue()))
                    syn285.Run();
                if (functionlist.Contains(ServicesFunctionEnum.Syn282.ExtValue()))
                    syn282.Run();
                if (functionlist.Contains(ServicesFunctionEnum.Syn244.ExtValue()))
                    syn244.Run();
                if (functionlist.Contains(ServicesFunctionEnum.JuniperPreWoGanerate.ExtValue()))
                    JuniperPreWoGanerate.Run();
                if (functionlist.Contains(ServicesFunctionEnum.JuniperPreUpoadBom.ExtValue()))
                    JuniperPreUpoadBom.Run();
                if (functionlist.Contains(ServicesFunctionEnum.JuniperSecUpoadBom.ExtValue()))
                    JuniperSecUpoadBom.Run();
                if (functionlist.Contains(ServicesFunctionEnum.AddNonBom.ExtValue()))
                    AddNonBom.Run();
                if (functionlist.Contains(ServicesFunctionEnum.JuniperCreateWo.ExtValue()))
                    JuniperCreateWo.Run();
                if (functionlist.Contains(ServicesFunctionEnum.JuniperGroupIdReceive.ExtValue()))
                    JuniperGroupIdReceive.Run();
                if (functionlist.Contains(ServicesFunctionEnum.SynAck.ExtValue()))
                    synAck.Run();
                if (functionlist.Contains(ServicesFunctionEnum.SynCrem.ExtValue()))
                    synCrem.Run();
                if (functionlist.Contains(ServicesFunctionEnum.JuniperSapJob.ExtValue()))
                    JuniperSapJob.Run();
                IsRuning = false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                MesLog.Info("End");
                IsRuning = false;
            }
        }


    }
}
