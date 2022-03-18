using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Linq;
using System.Collections.Generic;

namespace MESStation.Config.CMC
{
    public class TMNConfig
    {
        public DataItem AnICTData(string Data)
        {
            DataItem dataItem = new DataItem();
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string[] Datas = Data.Split('#');
            dic.Add("SN", Datas[0]);
            dic.Add("SKUNO", Datas[1]);
            dic.Add("STATION_NO", Datas[2]);
            dic.Add("EMP_NO", Datas[3]);
            dic.Add("STATUS", Datas[4]);
            if (Data.IndexOf("PASS") >= 0)
            {
                //pass分析数据
                //ZACDAA1936001RE#CM3200B#ICT-06#F1033553#PASS#<<<ver1.0.0.2  

                dic.Add("VER", Datas[5].Replace("<<<", ""));
            }
            else
            {
                //fail分析数据
                dic.Add("FAILCODE", Datas[5]);
                dic.Add("FAILDetail", Datas[6]);
                dic.Add("VER", Datas[7].Replace("<<<", ""));
            }

            dataItem.DATA = Data;
            dataItem.keyValuePairs = dic;
            return dataItem;
        }

        public static string GetDataForDataItem(Config.CMC.DataItem item, string KEY)
        {
            return item.keyValuePairs.Where(s => s.Key == KEY).ToArray()[0].Value;
        }
    }

    public class DataItem
    {
        public string DATA { get; set; }
        public Dictionary<string, string> keyValuePairs { get; set; }
    }

    public class TwoDataItem
    {
        public string SN { get; set; }
        public List<string> Data { get; set; }
        public string Status { get; set; }
        public string TestDataString { get; set; }
    }

    public class OneDataItem
    {
        public string SN { get; set; }
        public List<string> ONE { get; set; }
    }


}
