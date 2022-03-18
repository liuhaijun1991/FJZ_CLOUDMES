using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESException
{
    public class DBDataMissingException:ExceptionBase
    {
        public static List<ExceptionMessage> Messages = new List<ExceptionMessage>
        {
            new ExceptionMessage(){ Group="1", Lang="CN", MessageTemp=@"数据加载失败,表名:{0},查询条件:{1}" },
            new ExceptionMessage(){ Group="1", Lang="EN", MessageTemp=@"DataMissing,Table Name:{0},Query conditions:{1}" },
        };
        public string TableName;
        public string QueryConditions;
        public DBDataMissingException(string _TableName,string _QueryConditions)
        {
            this.ExcptionNO = 5001;
        }
        public override string GetMessage(string _Lang)
        {
            ExceptionMessage message = Messages.Find(t => t.Lang == _Lang && t.Group == "1");
            if (message == null)
            {
                message = Messages.Find(t => t.Lang == DefLang && t.Group == "1");
            }

            if (message == null)
            {
                return "NOT ErrorMessage";
            }

            string ret = string.Format(message.MessageTemp, TableName, QueryConditions);

            return ret;

        }


    }
}
