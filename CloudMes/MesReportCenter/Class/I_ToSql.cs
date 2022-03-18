using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MesReportCenter.Class
{
    interface I_ToSql
    {
        string Where(DBQuery dbQuery);
        string Select (DBQuery dbQuery);
        string GroupBy(DBQuery dbQuery);
        string From(DBQuery dbQuery);
        string Having(DBQuery dbQuery);
        string OrderBy(DBQuery dbQuery);
    }
}
