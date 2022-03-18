using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESStation
{
    public class MESPermission
    {
        public int? ID;
        public string Name;
        public string Desc;
        public MESPermission(int id)
        {
            if (id == 0)
            {
                ID = 0;
                Name = "創建用戶";
                Desc = "創建用戶的權限";
            }
            else if (id == 1)
            {
                ID = 1;
                Name = "讀取公共報表";
                Desc = "讀取公共報表的權限";
            }
        }
    }
}
