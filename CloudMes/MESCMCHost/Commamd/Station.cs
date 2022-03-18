using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MESCMCHost.Commamd
{
    public class Station : CMCCommand
    {
        public string StationName;
        public JToken JStation;
        public JToken CurrStationInput;
        public string ScanType = "Pass";
        public Station()
        {
            StrCommandCode = "Station";
            Actions.Add(new inputStationName(this));
        }
    }

    public class inputStationName : CMCCommandAction
    {
        public inputStationName(CMCCommand cmd)
        {
            this.comm = cmd;
            this.StrActionName = "StationName:";
        }
        public override void DoAction(CMC503Scanda Scanda, object data)
        {

            ((Station)comm).StationName = data.ToString();
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Stations.CallStation";
            mesdata.Function = "InitStation";
            mesdata.Data = new { DisplayStationName = ((Station)comm).StationName, Line = Scanda.Line };
            
            comm.CommandStatus = CommandState.WaitMesReturn;
            JObject JO = Scanda.MESAPI.CallMESAPISync(mesdata, comm.SyncTimeOut);

            Station Cmd = (Station)this.comm;
            if (JO["Status"].ToString() == "Pass")
            {
                Cmd.JStation = JO["Data"]["Station"];
                Cmd.Actions.Clear();
                inputStationInput input = null;
                if (Cmd.JStation["NextInput"] == null)
                {
                    input = new inputStationInput(this.comm, Cmd.JStation["Inputs"][0]["DisplayName"].ToString());
                    Cmd.CurrStationInput = Cmd.JStation["Inputs"][0];
                    Cmd.Actions.Add(input);
                    Cmd.CommandStatus = CommandState.CommandStart;
                    Cmd.CurrActionIndex = Cmd.Actions.Count - 1;
                }

                //Scanda.SendTextDataToCMC(input.StrActionName + ":");
            }
            else
            {
                Scanda.SendTextDataToCMC(JO["Message"].ToString());
                Cmd.CommandStatus = CommandState.CommandStart;
                Cmd.CurrActionIndex = Cmd.Actions.Count - 1;
            }

        }
    }

    public class inputStationInput : CMCCommandAction
    {

        public inputStationInput(CMCCommand cmd, string inputName)
        {
            this.comm = cmd;
            this.StrActionName = inputName;
        }
        public override void DoAction(CMC503Scanda Scanda, object data)
        {

            string inputTxt = data.ToString();
            this.AutoRunData = null;
            Station Cmd = (Station)this.comm;

            Cmd.CurrStationInput["Value"] = inputTxt;
            
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.Stations.CallStation";
            mesdata.Function = "StationInput";
           // Cmd.JStation["ScanType"] = Cmd.ScanType;
            mesdata.Data = new { Station = Cmd.JStation, Input = Cmd.CurrStationInput, ScanType= Cmd.ScanType };

            //comm.CommandStatus = CommandState.WaitMesReturn;
            //Scanda.CallMESAPI(mesdata, new CMC503Scanda.MESCallBack(DoActionCallBack));
            JObject JO = Scanda.MESAPI.CallMESAPISync(mesdata, Cmd.SyncTimeOut);

           
            if (JO["Status"].ToString() == "Pass")
            {
                Cmd.JStation = JO["Data"]["Station"]; ;
                //Cmd.Actions.Clear();

                for (int i = 0; i < Cmd.JStation["StationMessages"].Count(); i++)
                {
                    Scanda.SendTextDataToCMC(Cmd.JStation["StationMessages"][i]["Message"].ToString());
                }

                if (JO["Data"]["Station"]["ScanKP"].Count() > 0)
                {
                    for (int i = JO["Data"]["Station"]["ScanKP"].Count() -1 ; i >=0 ; /*i--*/)
                    {
                        string KP_SN = JO["Data"]["Station"]["ScanKP"][i]["SN"].ToString();
                        string KP_WO = JO["Data"]["Station"]["ScanKP"][i]["WO"].ToString();
                        string KP_Station = JO["Data"]["Station"]["ScanKP"][i]["Station"].ToString();
                        ScanKeypart kps = new ScanKeypart(KP_SN, KP_WO, KP_Station, Scanda);
                        Scanda.AddCommand(kps);
                        this.AutoRunData = data;
                        return;
                    } 
                }


                if (JO["Data"]["NextInput"] == null)
                {
                    int currindex = 0;
                    for (int i = 0; i < Cmd.JStation["Inputs"].Count(); i++)
                    {
                        if (Cmd.JStation["Inputs"][i]["ID"].ToString() == Cmd.CurrStationInput["ID"].ToString())
                        {
                            currindex = i + 1;
                        }
                    }
                    if (currindex < Cmd.JStation["Inputs"].Count())
                    {
                        Cmd.CurrStationInput = Cmd.JStation["Inputs"][currindex];
                    }
                    else
                    {
                        Cmd.CurrStationInput = Cmd.JStation["Inputs"][0];
                    }
                    this.StrActionName = Cmd.CurrStationInput["DisplayName"].ToString();
                    Cmd.CommandStatus = CommandState.CommandStart;

                }
                else
                {
                    int currindex = 0;
                    for (int i = 0; i < Cmd.JStation["Inputs"].Count(); i++)
                    {
                        if (Cmd.JStation["Inputs"][i]["ID"].ToString() == JO["Data"]["NextInput"]["ID"].ToString())
                        {
                            currindex = i;
                        }
                    }
                    Cmd.CurrStationInput = Cmd.JStation["Inputs"][currindex];
                    Cmd.CommandStatus = CommandState.CommandStart;
                    this.StrActionName = Cmd.CurrStationInput["DisplayName"].ToString();
                }
                if (JO["Data"]["Station"]["LabelPrint"] != null)
                {
                    for (int i = 0; i < JO["Data"]["Station"]["LabelPrint"].Count(); i++)
                    {
                        PrintTXTLabel(Scanda, JO["Data"]["Station"]["LabelPrint"][i]);
                    }
                }

            }
            else
            {
                Scanda.SendTextDataToCMC(JO["Message"].ToString());
                //comm.toNextAction();
            }
        }

        void PrintTXTLabel(CMC503Scanda Scanda, JToken Page)
        {
            string FileName = Page["FileName"].ToString();
            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.FileUpdate.FileUpload";
            mesdata.Function = "GetFileByName";
           // Cmd.JStation["ScanType"] = Cmd.ScanType;
            mesdata.Data = new { Name = FileName, UseType = "LABEL" };
            JObject JO = Scanda.MESAPI.CallMESAPISync(mesdata, 5000);

            if (JO["Status"].ToString() == "Pass")
            {
                string txtBas64 = JO["Data"]["BLOB_FILE"].ToString();
                byte[] bytes = Convert.FromBase64String(txtBas64);
                string strLab = Encoding.UTF8.GetString(bytes);

                for (int i = 0; i < Page["Outputs"].Count(); i++)
                {
                    strLab = strLab.Replace("@" + Page["Outputs"][i]["Name"].ToString() + "@", Page["Outputs"][i]["Value"].ToString());
                }
                try
                {
                    Scanda.SendDataToCMCCom(strLab);
                }
                catch(Exception ee)
                {
                    Scanda.SendTextDataToCMC("Print Err:"+ee.Message);
                }
            }
            else
            {
                Scanda.SendTextDataToCMC("Get Label Fail:" +JO["Message"].ToString());
                
            }

        }


    }
}
