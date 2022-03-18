using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.MESStation;

namespace MesReportCenter
{
    public class Class1:MesAPIBase
    {
        protected APIInfo FAddNewBU = new APIInfo()
        {
            FunctionName = "AddNewBU",
            Description = "Add a new BU",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "BU", InputType = "string", DefaultValue = "" }
            },
            Permissions = new List<MESPermission>() { }
        };
    }
}
