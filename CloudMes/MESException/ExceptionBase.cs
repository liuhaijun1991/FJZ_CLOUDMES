using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESException
{
    public class ExceptionBase:Exception
    {
        public int ExcptionNO { get; set; }
        public string Lang { get; set; }
        public string DefLang { get; set; } = "EN";

        public string GetMessage()
        {
            return GetMessage(Lang);
        }
        public virtual string GetMessage(string _Lang)
        {
            throw new NotImplementedException();
        }

        public override string Message
        {
            get
            {
                return GetMessage(Lang);
            }
        }

    }
}
