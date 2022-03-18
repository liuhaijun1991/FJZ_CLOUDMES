using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MESDataObject.Module;
using MESStation.KeyPart;
using Newtonsoft.Json.Linq;

namespace MESCMCHost.Commamd
{
    public class ScanKeypart : CMCCommand
    {
        public JToken ScanKPList;

        public string SN = "";
        public string WO = "";
        public string STATION = "";
        public SN_KP KPDATA = null;

        public int CurrItemIndex = 0;
        public ScanKeypart(string _SN,string _WO,string _STATION, CMC503Scanda Scanda)
        {
            StrCommandCode = "KPSCAN";
            SN = _SN;
            WO = _WO;
            STATION = _STATION;
            //Actions.Add(new inputKpValue(this));
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.KeyPart.KPScan";
            mesdata.Function = "GetSNStationKPList";
            mesdata.Data = new { SN = SN, WO = WO, STATION = STATION };

            //CommandStatus = CommandState.WaitMesReturn;
            JObject JO = Scanda.MESAPI.CallMESAPISync(mesdata, Comm.SyncTimeOut);
            if (JO["Status"].ToString() == "Pass")
            {
                try
                {
                    KPDATA = Newtonsoft.Json.JsonConvert.DeserializeObject<SN_KP>(JO["Data"].ToString());
                }
                catch(Exception)
                {

                }
                CurrItemIndex = 0;

                while ((KPDATA.KPS[CurrItemIndex].VALUE != null
                    && KPDATA.KPS[CurrItemIndex].VALUE != "")
                    && CurrItemIndex < KPDATA.KPS.Count)
                {
                    CurrItemIndex++;
                }

                //KPDATA.KPS[0].
                inputKpValue IV = new inputKpValue(this);

                this.Actions.Add(IV);
                IV.StrActionName = KPDATA.KPS[CurrItemIndex].KP_NAME + ">>" + KPDATA.KPS[CurrItemIndex].SCANTYPE + ":";
                //Scanda.SendTextDataToCMC(KPDATA.KPS[CurrItemIndex].KP_NAME + ">>" + KPDATA.KPS[CurrItemIndex].SCANTYPE + ":");
                CommandStatus = CommandState.CommandStart;
                CurrActionIndex = Actions.Count - 1;

            }
            else
            {
                throw new Exception(JO["Message"].ToString());
            }


        }
    }
    public class inputKpValue : CMCCommandAction
    {
        public inputKpValue(CMCCommand cmd)
        {
            this.comm = cmd;
            this.StrActionName = "Value:";
            
        }
        public override void DoAction(CMC503Scanda Scanda, object data)
        {
            var Cmd = (ScanKeypart)comm;
            string Value = data.ToString();
            R_SN_KP kpitem = Cmd.KPDATA.KPS[Cmd.CurrItemIndex];

            Regex r = new Regex(kpitem.REGEX);
            if (r.IsMatch(Value))
            //if (true)
            {
                kpitem.VALUE = Value;
            }
            else
            {
                Scanda.SendTextDataToCMC($@"Regex'{kpitem.REGEX}' fail");
                Cmd.CommandStatus = CommandState.CommandStart;
                Cmd.CurrActionIndex = Cmd.Actions.Count - 1;
                return;
            }

            var items = Cmd.KPDATA.KPS.FindAll(t => t.ITEMSEQ == kpitem.ITEMSEQ);
            if (items.Find(t => t.VALUE?.Trim() == "" || t.VALUE == null) == null)
            {
                MESAPIData mesdata = new MESAPIData();
                mesdata.Class = "MESStation.KeyPart.KPScan";
                mesdata.Function = "ScanKPItem";
                mesdata.Data = new { SN = Cmd.SN, STATION = Cmd.STATION, KPITEM = items };
                //CommandStatus = CommandState.WaitMesReturn;
                JObject JO = Scanda.MESAPI.CallMESAPISync(mesdata, Cmd.SyncTimeOut);
                if (JO["Status"].ToString() == "Pass")
                //if (true)
                {
                    Cmd.CurrItemIndex++;
                    while (Cmd.CurrItemIndex < Cmd.KPDATA.KPS.Count &&
                        (Cmd.KPDATA.KPS[Cmd.CurrItemIndex].VALUE != null
                        && Cmd.KPDATA.KPS[Cmd.CurrItemIndex].VALUE != "")
                        )
                    {
                        Cmd.CurrItemIndex++;
                    }

                }
                else
                {
                    Scanda.SendTextDataToCMC($@"KP SAVE ERR:"+ JO["Message"].ToString());

                    for (int i = 0; i < items.Count; i++)
                    {
                        items[i].VALUE = null;
                    }
                    for (int i = 0; i < Cmd.KPDATA.KPS.Count; i++)
                    {
                        if (Cmd.KPDATA.KPS[i].KP_NAME == items[0].KP_NAME &&
                            Cmd.KPDATA.KPS[i].ITEMSEQ == items[0].ITEMSEQ &&
                            Cmd.KPDATA.KPS[i].SCANSEQ == items[0].SCANSEQ)
                        {
                            Cmd.CurrItemIndex = i;
                            break;
                        }
                    }
                    this.StrActionName = Cmd.KPDATA.KPS[Cmd.CurrItemIndex].KP_NAME + ">>" + Cmd.KPDATA.KPS[Cmd.CurrItemIndex].SCANTYPE + ":";
                    Cmd.CommandStatus = CommandState.CommandStart;
                    Cmd.CurrActionIndex = 0;
                }
            }
            else
            {
                Cmd.CurrItemIndex++;
                while ((Cmd.KPDATA.KPS[Cmd.CurrItemIndex].VALUE != null
                    && Cmd.KPDATA.KPS[Cmd.CurrItemIndex].VALUE != "")
                    && Cmd.CurrItemIndex <= Cmd.KPDATA.KPS.Count)
                {
                    Cmd.CurrItemIndex++;
                }
            }
            if (Cmd.CurrItemIndex < Cmd.KPDATA.KPS.Count)
            {
                this.StrActionName = Cmd.KPDATA.KPS[Cmd.CurrItemIndex].KP_NAME + ">>" + Cmd.KPDATA.KPS[Cmd.CurrItemIndex].SCANTYPE + ":";
                Cmd.CommandStatus = CommandState.CommandStart;
                Cmd.CurrActionIndex = 0;
            }
            else
            {
                Cmd.CommandStatus = CommandState.CommandEnd;
            }


        }
    }

    public class inputMPM : CMCCommandAction
    {
        public inputMPM(CMCCommand cmd)
        {
            this.comm = cmd;
            this.StrActionName = "MPM:";
        }
        public override void DoAction(CMC503Scanda Scanda, object data)
        {
            base.DoAction(Scanda, data);
        }
    }
}
