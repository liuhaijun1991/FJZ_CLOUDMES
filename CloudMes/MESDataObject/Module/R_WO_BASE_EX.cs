using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_R_WO_BASE_EX : DataObjectTable
    {
        public T_R_WO_BASE_EX(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_WO_BASE_EX(OleExec DB, DB_TYPE_ENUM DBType)
        {
            //RowType = typeof(Row_R_WO_BASE_EX);
            TableName = "R_WO_BASE_EX".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        /// <summary>
        /// 添加擴展表數據
        /// </summary>
        /// <param name="WoId"></param>
        /// <param name="Name"></param>
        /// <param name="Value"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int InsertWoBaseEx(string WoId, string Name, string Value, OleExec DB)
        {
            R_WO_BASE_EX WoBaseEx = new R_WO_BASE_EX();
            WoBaseEx.ID = WoId;
            WoBaseEx.NAME = Name;
            WoBaseEx.VALUE = Value;
            var ExistWoBaseEx = DB.ORM.Queryable<R_WO_BASE_EX>().Where(t => t.ID == WoId).OrderBy(t => t.SEQ_NO, SqlSugar.OrderByType.Desc).ToList().FirstOrDefault();
            if (ExistWoBaseEx != null)
            {
                WoBaseEx.SEQ_NO = ExistWoBaseEx.SEQ_NO + 1;
            }
            else
            {
                WoBaseEx.SEQ_NO = 1;
            }
            return DB.ORM.Insertable<R_WO_BASE_EX>(WoBaseEx).ExecuteCommand();
        }

        /// <summary>
        /// 根據工單ID，類型名獲得所有擴展信息
        /// </summary>
        /// <param name="WoBaseId"></param>
        /// <param name="Name"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public List<R_WO_BASE_EX> GetExBySnId(string WoBaseId, string Name, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_BASE_EX>().WhereIF(!string.IsNullOrEmpty(Name), t => t.NAME == Name).Where(t => t.ID == WoBaseId).ToList();
        }
        public List<R_WO_BASE_EX> Getdata(string wo, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_BASE_EX>().WhereIF(!wo.Equals(""), sd => sd.VALUE == wo).ToList();
        }
        public string InsertWoBaseEx(string wo,string seq, OleExec DB)
        {
            int a = 0;
            var checkexits= DB.ORM.Queryable<R_WO_BASE_EX>().Any(t => t.VALUE == wo );
            if (checkexits)
            {
                return "WO already exist";
            }
            try
            {
                if (seq == "YES")
                {
                    a = 1;
                }
                int i= DB.ORM.Insertable(new R_WO_BASE_EX()
                {
                    ID = this.GetNewID("VNJUNIPER", DB),
                    SEQ_NO=a,
                    NAME="ATT",
                    VALUE=wo
                }
                ).ExecuteCommand();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "OK";
        }

        public int DeleteWoBaseEx(string wo,  OleExec DB)
        {
           return DB.ORM.Deleteable<R_WO_BASE_EX>().Where(t => t.VALUE == wo).ExecuteCommand();
        }
        public int UpdateWoBaseEx(string wo, string seq_no, OleExec DB)
        {
            int a = 0;
            if (seq_no == "YES")
            {
                a = 1;
            }
            return DB.ORM.Updateable<R_WO_BASE_EX>()
                .Where(it => it.VALUE == wo)
                .UpdateColumns(it => new R_WO_BASE_EX { SEQ_NO =a}).ExecuteCommand();
        }
    }
    public class Row_R_WO_BASE_EX : DataObjectBase
    {
        public Row_R_WO_BASE_EX(DataObjectInfo info) : base(info)
        {

        }
        public R_WO_BASE_EX GetDataObject()
        {
            R_WO_BASE_EX DataObject = new R_WO_BASE_EX();
            DataObject.ID = this.ID;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.NAME = this.NAME;
            DataObject.VALUE = this.VALUE;
            return DataObject;
        }
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }
        public double? SEQ_NO
        {
            get
            {
                return (double?)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
            }
        }
        public string NAME
        {
            get
            {
                return (string)this["NAME"];
            }
            set
            {
                this["NAME"] = value;
            }
        }
        public string VALUE
        {
            get
            {
                return (string)this["VALUE"];
            }
            set
            {
                this["VALUE"] = value;
            }
        }
    }
    public class R_WO_BASE_EX
    {
        public string ID { get; set; }
        public double? SEQ_NO { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
    }
}