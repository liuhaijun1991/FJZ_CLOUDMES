using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MESDataObject.Common
{
    public static class JsonHelper
    {
    }
    public class ByteToStringConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(byte[]);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return StringToByteArray(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(ByteToString((byte[])value));
        }

        public static byte[] StringToByteArray(string s)
        {
            return Encoding.Unicode.GetBytes(s);
        }

        public static string ByteToString(byte[] b)
        {
            string r = Encoding.Unicode.GetString(b);
            return r;
        }
    }
}
