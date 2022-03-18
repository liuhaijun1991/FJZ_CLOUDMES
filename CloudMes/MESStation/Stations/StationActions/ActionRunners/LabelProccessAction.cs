using MESPubLab.MESStation.Label;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class LabelProccessAction
    {
        public static void MakeCISPages(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            List<LabelBase> labels = new List<LabelBase>();
            List<int> CISPageStart = new List<int>();
            int CIScountSet = 0;
            int CISCount = 0;

            for (int i = 0; i < Station.LabelPrint.Count; i++)
            {
                var t = Station.LabelPrint[i].Outputs.Find(t1 => t1.Name == "CISFLAG");
                if (t != null)
                {
                    labels.Add(Station.LabelPrint[i]);

                    //Add for ODA_HA vince_20190829
                    if (Station.LabelPrint[i].LabelName == "X7_2_CIS_PAGE1")
                    {
                        CISPageStart.Add(i);
                        CIScountSet++;                   
                    }
                }
            }

           int[] CISPageCount = new int[CIScountSet];

            for (int i = CISPageStart.Count; i > 0; i--)
            {
                if (i == CIScountSet)
                {
                    CISPageCount[i - 1] = Station.LabelPrint.Count - CISPageStart[i - 1];
                }
                else
                {
                    CISPageCount[i - 1] = CISPageStart[i] - CISPageStart[i - 1];
                }              
            }

            //for (int i = 0; i < labels.Count; i++)
            //{
            //    labels[i].Outputs.Find(t => t.Name == "PAGE1").Value = (i+1).ToString();
            //    labels[i].Outputs.Find(t => t.Name == "PAGE2").Value = labels.Count.ToString();
            //}

            for (int j = 0; j < CIScountSet; j++)
            {
                for (int i = 0; i < CISPageCount[j]; i++)
                {
                    labels[CISCount + i].Outputs.Find(t => t.Name == "PAGE1").Value = (i + 1).ToString();
                    labels[CISCount + i].Outputs.Find(t => t.Name == "PAGE2").Value = CISPageCount[j].ToString();
                }

                CISCount = CISCount + CISPageCount[j];
            }

        }
    }
}
