using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDataObject.ModuleHelp
{
    public class FuncExecRes
    {
        public bool IsSuccess { get; set; }
        public Exception ErrorException { get; set; }
        public string ErrorMessage { get; set; }
    }
}
