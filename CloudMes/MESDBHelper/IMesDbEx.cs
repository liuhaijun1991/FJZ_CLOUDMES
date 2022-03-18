using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESDBHelper
{
    public static class IMesDbEx
    {

        /// <summary>
        /// 突破Oracle in 操作不能超過1000的限制，數據量越大速度越慢 --add by Eden
        /// </summary>
        /// <param name="item"></param>
        /// <param name="tartgetList"></param>
        /// <returns></returns>
        public static bool OracleContain<T>(string item, List<T> tartgetList)
        {
            throw new NotImplementedException();
        }
    }
}
