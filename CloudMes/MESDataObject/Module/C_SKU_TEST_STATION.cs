using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;

namespace MESDataObject.Module
{
    public class T_C_SKU_TEST_STATION : DataObjectTable
    {
        public T_C_SKU_TEST_STATION(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_SKU_TEST_STATION(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_SKU_TEST_STATION);
            TableName = "C_SKU_TEST_STATION".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public C_SKU_TEST_STATION GetSkuTestStationBySkuID(string skuID, OleExec db)
        {
            string sql = $@"select * from c_sku_test_station where c_sku_id = '{skuID}'";
            DataTable dt = db.ExecSelect(sql).Tables[0];
            C_SKU_TEST_STATION res = new C_SKU_TEST_STATION();
            if (dt.Rows.Count > 0)
            {
                Row_C_SKU_TEST_STATION row = (Row_C_SKU_TEST_STATION)this.NewRow();
                row.loadData(dt.Rows[0]);
                res = row.GetDataObject();
            }
            else
            {
                res = null;
            }
            return res;
        }


        /// <summary>
        /// 增、刪、改 C_SKU_TEST_STATION表信息
        /// Update the data in the table through a C_SKU object
        /// </summary>
        /// <param name="BU"></param>
        /// <param name="Sku"></param>
        /// <param name="Operation"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string UpdateSku(string BU, C_SKU_TEST_STATION SkuTS, string Operation, DateTime EditTime, String SkuId, OleExec DB)
        {
            string result = string.Empty;
            string sql = string.Empty;
            string format = "yyyy-MM-dd HH:mm:ss";

            C_SKU_TEST_STATION NowSku = GetSkuTestStationBySkuID(SkuId, DB);
            Row_C_SKU_TEST_STATION row = (Row_C_SKU_TEST_STATION)NewRow();

            if (NowSku != null)
            {
                if (NowSku.ID != null && NowSku.ID != "")
                {
                    row.ID = NowSku.ID;
                }
            }

            row.C_SKU_ID = SkuId;
            row.VALID_FLAG = "1";
            row.TEST_STATION = SkuTS.TEST_STATION;
            row.LENGTH = SkuTS.LENGTH;
            row.LABEL_VER = SkuTS.LABEL_VER;
            row.MODELCLEI = SkuTS.MODELCLEI;
            row.DESCRIPTION2 = SkuTS.DESCRIPTION2;
            row.ECI_NO = SkuTS.ECI_NO;
            row.FO6 = SkuTS.FO6;
            row.EDIT_EMP = SkuTS.EDIT_EMP;

            switch (Operation.ToUpper())
            {
                case "ADD":
                    row.ID = GetNewID(BU, DB);
                    row.EDIT_TIME = EditTime;
                    sql = row.GetInsertString(DBType);

                    break;
                case "UPDATE":

                    string Current = NowSku.EDIT_TIME.ToString(format);
                    string Before = SkuTS.EDIT_TIME.ToString(format).Replace('T', ' ');
                    // 模擬樂觀鎖機制，如果出現贓讀情況，重新從數據庫加載實例
                    if (Before.Equals("") && !Current.Equals(Before))
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000152", new string[] { }));
                    }
                    row.EDIT_TIME = EditTime;
                    sql = row.GetUpdateString(DBType, row.ID);
                    break;
                case "DELETE":
                    sql = row.GetDeleteString(DBType, row.ID);
                    break;
                default:
                    break;
            }

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                DB.BeginTrain();
                result = DB.ExecSQL(sql);
                if (Int32.Parse(result) <= 0)
                {
                    DB.RollbackTrain();
                    return result;
                }
                DB.CommitTrain();
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }


        /// <summary>
        /// WZW 根據機種查測試路由
        /// </summary>
        /// <param name="SKUID"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public C_SKU_TEST_STATION GetBySKU(string SKUID, OleExec DB)
        {
            return DB.ORM.Queryable<C_SKU_TEST_STATION>().Where(t => t.C_SKU_ID == SKUID).ToList().FirstOrDefault();
        }
        public int UpdateAutoStation(string AutoStation, string SKUID, OleExec DB)
        {
            return DB.ORM.Updateable<C_SKU_TEST_STATION>().UpdateColumns(t => t.TEST_STATION == AutoStation).Where(t => t.C_SKU_ID == SKUID).ExecuteCommand();
        }
    }
    public class Row_C_SKU_TEST_STATION : DataObjectBase
    {
        #region 數據庫行級映射
        public Row_C_SKU_TEST_STATION(DataObjectInfo info) : base(info)
        {

        }
        public C_SKU_TEST_STATION GetDataObject()
        {
            C_SKU_TEST_STATION DataObject = new C_SKU_TEST_STATION();
            DataObject.ID = this.ID == null ? "" : this.ID;
            DataObject.C_SKU_ID = this.C_SKU_ID == null ? "" : this.C_SKU_ID;
            DataObject.TEST_STATION = this.TEST_STATION == null ? "" : this.TEST_STATION;
            DataObject.VALID_FLAG = this.VALID_FLAG == null ? "" : this.VALID_FLAG;
            DataObject.EDIT_EMP = this.EDIT_EMP == null ? "" : this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
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
        public string C_SKU_ID
        {
            get
            {
                return (string)this["C_SKU_ID"];
            }
            set
            {
                this["C_SKU_ID"] = value;
            }
        }
        public string TEST_STATION
        {
            get
            {
                return (string)this["TEST_STATION"];
            }
            set
            {
                this["TEST_STATION"] = value;
            }
        }
        public string VALID_FLAG
        {
            get
            {
                return (string)this["VALID_FLAG"];
            }
            set
            {
                this["VALID_FLAG"] = value;
            }
        }
        public string EDIT_EMP
        {
            get
            {
                return (string)this["EDIT_EMP"];
            }
            set
            {
                this["EDIT_EMP"] = value;
            }
        }
        public DateTime EDIT_TIME
        {
            get
            {
                if (this["EDIT_TIME"] == null)
                {
                    return DateTime.Now;
                }
                else
                {
                    return (DateTime)this["EDIT_TIME"];
                }
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public double LENGTH
        {
            get
            {
                if (this["LENGTH"] == null)
                {
                    return 0;
                }
                else
                {
                    return (double)this["LENGTH"];
                }
            }
            set
            {
                this["LENGTH"] = value;
            }
        }
        public string LABEL_VER
        {
            get
            {
                return (string)this["LABEL_VER"];
            }
            set
            {
                this["LABEL_VER"] = value;
            }
        }
        public string MODELCLEI
        {
            get
            {
                return (string)this["MODELCLEI"];
            }
            set
            {
                this["MODELCLEI"] = value;
            }
        }
        public string DESCRIPTION2
        {
            get
            {
                return (string)this["DESCRIPTION2"];
            }
            set
            {
                this["DESCRIPTION2"] = value;
            }
        }
        public string ECI_NO
        {
            get
            {
                return (string)this["ECI_NO"];
            }
            set
            {
                this["ECI_NO"] = value;
            }
        }
        public string FO6
        {
            get
            {
                return (string)this["FO6"];
            }
            set
            {
                this["FO6"] = value;
            }
        }
        #endregion
    }
    public class C_SKU_TEST_STATION
    {
        public string ID { get; set; }
        public string C_SKU_ID { get; set; }
        public string TEST_STATION { get; set; }
        public string VALID_FLAG { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime EDIT_TIME { get; set; }
        public double LENGTH { get; set; }
        public string LABEL_VER { get; set; }
        public string MODELCLEI { get; set; }
        public string DESCRIPTION2 { get; set; }
        public string ECI_NO { get; set; }
        public string FO6 { get; set; }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}