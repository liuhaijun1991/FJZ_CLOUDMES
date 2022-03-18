using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab
{
    public class Multilingual
    {
        static object syncobj = new object();
        static Dictionary<string, Dictionary<string, string>> data;

        public string Language { get; set; } = "en";

        public string Get(string label)
        {
            lock (syncobj)
            {
                if (data == null)
                {
                    var js = MesMessage.MesMessage.Multilingual_Tags;
                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(js);
                }
            }

            if (!data.Keys.Contains(label))
            {
                throw new Exception($@"No def label '{label}'");

            }
            var msg = data[label];
            if (!msg.Keys.Contains(Language))
            {
                return msg["en"];
            }
            return msg[Language];
        }

        public Dictionary<string, string> GetFulldata(string label)
        {
            lock (syncobj)
            {
                if (data == null)
                {
                    var js = MesMessage.MesMessage.Multilingual_Tags;
                    data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(js);
                }
            }

            if (!data.Keys.Contains(label))
            {
                throw new Exception($@"No def label '{label}'");

            }
            return  data[label];
            
        }

    }
}
