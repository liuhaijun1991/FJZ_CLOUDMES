using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.JUNIPER
{
    public class CBSBackflush : taskBase
    {
        public bool IsRuning = false;
        private string mesdbstr, b2bdbstr, bustr, functiontype, plantcode;
        public override void init()
        {
            try
            {
                mesdbstr = ConfigGet("MESDB");
                //functiontype = ConfigGet("FUNCTIONTYPE");
                //b2bdbstr = ConfigGet("LHB2BDB");
                bustr = ConfigGet("BU");
                plantcode = ConfigGet("PLANTCODE");
                Output.UI = new CBSBackflush_UI(this);
               
            }
            catch (Exception)
            {
            }
        }



        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("The task is in progress. Please try again later...");
            }
            IsRuning = true;
            try
            {
                //juniperCreateWo.Run();
            }
            catch
            {
            }
            IsRuning = false;
        }
    }
}
