using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MESInterface.DCN
{
    public class SQLPara
    {
        public Type TYPE;
        public object Value;
        public SQLPara(Type type, object value)
        {
            TYPE = type;
            Value = value;
        }
        public SQLPara(string strValue)
        {
            TYPE = typeof(System.String);
            Value = (object)strValue;
        }
        public SQLPara(DateTime DataTimeValue)
        {
            TYPE = typeof(System.DateTime);
            Value = (object)DataTimeValue;
        }
        public string ToString()
        {
            string strRet = "";
            if (this.TYPE == typeof(System.Decimal))
            {
                strRet = this.Decimal_To_String();
            }
            else if (this.TYPE == typeof(System.DateTime))
            {
                strRet = this.DateTime_To_String();
            }
            else if (this.TYPE == typeof(System.String))
            {
                strRet = String_To_String();
            }
            else
            {
                strRet = Value.ToString();
            }
            return strRet;
        }
        string String_To_String()
        {
            string ret = Value.ToString();
            ret = ret.Replace("'", "''");
            ret = "'" + ret + "'";
            return ret;
        }
        string DateTime_To_String()
        {
            DateTime dt = DateTime.Parse(Value.ToString());
            string ret = "";
            ret = "to_date('" + dt.ToString("dd-MM-yyyy HH:mm:ss") + "', 'dd-mm-yyyy hh24:mi:ss')";
            return ret;
        }
        string Decimal_To_String()
        {
            return Value.ToString();
        }
    }
}