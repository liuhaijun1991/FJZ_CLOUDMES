using MESDBHelper;
using System.Collections.Generic;
using System.Linq;

namespace MESDataObject.Module
{
    public class T_R_SN_EX : DataObjectTable
    {
        public T_R_SN_EX(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_SN_EX(OleExec DB, DB_TYPE_ENUM DBType)
        {
            TableName = "R_SN_EX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 添加擴展表數據
        /// </summary>
        /// <param name="SnId"></param>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertSnEx(string SnId, string Name, string Value,OleExec DB)
        {
            R_SN_EX SnEx = new R_SN_EX();
            SnEx.ID = SnId;
            SnEx.NAME = Name;
            SnEx.VALUE = Value;
            var ExistSnEx = DB.ORM.Queryable<R_SN_EX>().Where(t => t.ID == SnId).OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (ExistSnEx != null)
            {
                SnEx.SEQ_NO = ExistSnEx.SEQ_NO + 1;
            }
            else
            {
                SnEx.SEQ_NO = 1;
            }
            return DB.ORM.Insertable<R_SN_EX>(SnEx).ExecuteCommand();
        }

        /// <summary>
        /// 根據SN ID 和 類型名獲得擴展信息
        /// </summary>
        /// <param name="SnId"></param>
        /// <param name="Name"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_SN_EX> GetExBySnId(string SnId, string Name, OleExec DB)
        {
            return DB.ORM.Queryable<R_SN_EX>().WhereIF(!string.IsNullOrEmpty(Name), t => t.NAME == Name).Where(t => t.ID == SnId).ToList();
        }
    }
    
    public class R_SN_EX
    {
        public string ID { get; set; }
        public int SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }
}