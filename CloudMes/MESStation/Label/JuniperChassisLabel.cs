using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Label.Public
{
    public class JuniperChassisLabel : LabelValueGroup
    {
        public JuniperChassisLabel()
        {
            ConfigGroup = "JuniperChassisLabel";
            
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetFrontPanelClei", Description = "Get Front Panel Clei By PN", Paras = new List<string>() { "WO", "PN"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetFrontPanelPN", Description = "Get Front Panel PN By PN", Paras = new List<string>() { "WO", "PN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetFrontPanelRev", Description = "Get Front Panel Rev By PN", Paras = new List<string>() { "WO", "PN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetFrontPanelSN", Description = "Get Front Panel SN By PN", Paras = new List<string>() { "WO", "PN" } });

            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMidPlaneClei", Description = "Get Mid Plane Clei By PN", Paras = new List<string>() { "WO", "PN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMidPanelPN", Description = "Get Mid Plane PN By PN", Paras = new List<string>() { "WO", "PN" } });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMidPanelRev", Description = "Get Mid Plane Rev By PN", Paras = new List<string>() { "WO", "PN"} });
            Functions.Add(new LabelValueFunctionConfig() { FunctionName = "GetMidPanelSN", Description = "Get Mid Plane SN By PN", Paras = new List<string>() { "WO", "PN" } });
            
        }

        public string GetFrontPanelClei(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t=>t.CLEI_CODE).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetFrontPanelPN(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.PN_7XX).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetFrontPanelRev(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.REV).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetFrontPanelSN(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.SN_RULE).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetMidPlaneClei(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.CLEI_CODE).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetMidPanelPN(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.PN).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetMidPanelRev(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.REV).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }

        public string GetMidPanelSN(OleExec SFCDB, string WO, string PN)
        {
            string output = "";

            var kp_list = JsonSave.GetFromDB<List<JuniperAutoKpConfig>>(WO, "JuniperAutoKPConfig", SFCDB);

            var aa = kp_list.FindAll(t => t.PN == PN).Select(t => t.SN_RULE).First();

            if (aa != null)
            {
                output = aa;
            }
            return output;
        }
       
    }
}
