using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Data.OleDb;

namespace MESDataObject.Module
{
    public class T_C_WAREHOUSE_PALLET_POSITION_T : DataObjectTable
    {
        public T_C_WAREHOUSE_PALLET_POSITION_T(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_C_WAREHOUSE_PALLET_POSITION_T(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_C_WAREHOUSE_PALLET_POSITION_T);
            TableName = "C_WAREHOUSE_PALLET_POSITION_T".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }
        public C_WAREHOUSE_PALLET_POSITION_T ConstructWarehouse(DataRow dr)
        {
            C_WAREHOUSE_PALLET_POSITION_T C_warehouse_pallet_position_t = new C_WAREHOUSE_PALLET_POSITION_T();
            Row_C_WAREHOUSE_PALLET_POSITION_T row = (Row_C_WAREHOUSE_PALLET_POSITION_T)NewRow();
            row.loadData(dr);
            C_warehouse_pallet_position_t = row.GetDataObject();
            return C_warehouse_pallet_position_t;
        }
        public List<Dictionary<int, Dictionary<string, int>>> GetAllPositionByWHID(string WH_ID, OleExec DB)
        {
            string sql = ""; ;
            DataTable dt = null;
            List<Dictionary<int, Dictionary<string, int>>> data_mapping = new List<Dictionary<int, Dictionary<string, int>>>();
            try
            {
                //init array map
                sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUSE_CONFIG_T where WH_ID='{WH_ID.Trim()}'";
                DataSet res = DB.ExecSelect(sql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    for (int row = 1; row <= int.Parse(res.Tables[0].Rows[0]["ROW_SIZE"].ToString()); row++)
                    {
                        Dictionary<int, Dictionary<string, int>> c = new Dictionary<int, Dictionary<string, int>>();
                        for (int col = 1; col <= int.Parse(res.Tables[0].Rows[0]["COL_SIZE"].ToString()); col++)
                        {
                            Dictionary<string, int> d = new Dictionary<string, int>();
                            d.Add("row", row);
                            d.Add("col", col);
                            d.Add("state", 0);
                            d.Add("hide", 0);
                            c.Add(col, d);
                        }
                        data_mapping.Add(c);
                    }
                    //scan position hide
                    sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUS_POSITION_HIDE_T where WH_ID='{WH_ID.Trim()}'";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = null;
                        for (int e = 0; e < dt.Rows.Count; e++)
                        {
                            dr = dt.Rows[e];
                            data_mapping[int.Parse(dr["row_position"].ToString()) - 1][int.Parse(dr["col_position"].ToString())]["hide"] = 1;
                        }
                    }
                    //scan postion status
                    sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUSE_PALLET_POSITION_T where WH_ID='{WH_ID.Trim()}' AND OUT_FLAG=0 ORDER BY SORT_POSITION";
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        DataRow dr = null;
                        for (int e = 0; e < dt.Rows.Count; e++)
                        {
                            dr = dt.Rows[e];
                            data_mapping[int.Parse(dr["row_position"].ToString()) - 1][int.Parse(dr["col_position"].ToString())]["state"] = int.Parse(dr["sort_position"].ToString());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return data_mapping;
        }
        public int CheckInfoPosition(string WH_ID, int ROW_POSITION, int COL_POSITION, OleExec DB)
        {
            List<C_WAREHOUSE_PALLET_POSITION_T> PositionList = new List<C_WAREHOUSE_PALLET_POSITION_T>();
            string sql = "";
            DataTable dt = null;
            try
            {
                if (!string.IsNullOrEmpty(WH_ID.Trim()))
                {
                    sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUSE_CONFIG_T WHERE WH_ID='{WH_ID.ToUpper()}'";
                }
                dt = DB.ExecSelect(sql).Tables[0];
                if (int.Parse(dt.Rows[0]["COL_SIZE"].ToString()) < COL_POSITION || int.Parse(dt.Rows[0]["ROW_SIZE"].ToString()) < ROW_POSITION)
                {
                    return 0;
                }
                else return 1;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<C_WAREHOUSE_PALLET_POSITION_T> GetInfoPosition(string WH_ID, int ROW_POSITION, int COL_POSITION, OleExec DB)
        {
            List<C_WAREHOUSE_PALLET_POSITION_T> PositionList = new List<C_WAREHOUSE_PALLET_POSITION_T>();
            string sql = "";
            DataTable dt = null;
            try
            {
                if (!string.IsNullOrEmpty(WH_ID.Trim()))
                {
                    sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUSE_PALLET_POSITION_T WHERE WH_ID='{WH_ID.ToUpper()}' AND ROW_POSITION=" + ROW_POSITION + " AND COL_POSITION=" + COL_POSITION + " AND OUT_FLAG=0 ORDER BY SORT_POSITION";
                }
                string a = sql;
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        PositionList.Add(ConstructWarehouse(dr));
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
            return PositionList;
        }
        public int InsertPalletToPosition(string WH_ID, string PALLET_NO, int ROW_POSITION, int COL_POSITION, int SORT_POSITION, string EMP, string BU, DB_TYPE_ENUM dbt, OleExec DB)
        {
            OleDbParameter[] param = null;
            string sql;
            int result;
            T_C_WAREHOUSE_PALLET_POSITION_T CWarehousePosition = new T_C_WAREHOUSE_PALLET_POSITION_T(DB, dbt);
            DataTable table;
            try
            {
                sql = $@"SELECT *FROM SFCBASE.C_WAREHOUSE_PALLET_POSITION_T WHERE WH_ID='{WH_ID}' AND ROW_POSITION='{ROW_POSITION}' AND COL_POSITION='{COL_POSITION}' AND OUT_FLAG=0";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count >= 2)
                {
                    return 4;//position full
                }
                else if (table.Rows.Count == 1 && int.Parse(table.Rows[0]["SORT_POSITION"].ToString()) == SORT_POSITION)
                {
                    return 5;//duplicate sort position
                }
                sql = $@"SELECT * FROM SFCBASE.C_WAREHOUSE_PALLET_POSITION_T WHERE UPPER(PALLET_NO)='{PALLET_NO}' AND OUT_FLAG=0";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count > 0)
                {
                    return 0;//duplicate pallet
                }
                sql = $@"SELECT * FROM r_sn WHERE id IN (
                        SELECT sn_id FROM r_sn_packing WHERE PACK_ID IN (
                        SELECT ID FROM r_packing WHERE PARENT_PACK_ID IN (
                        SELECT ID FROM r_packing WHERE PACK_NO='{PALLET_NO}' AND PACK_TYPE='PALLET'))) AND SHIPPED_FLAG=0 AND  NEXT_STATION ='SHIPOUT'";
                table = DB.ExecSelect(sql).Tables[0];
                if (table.Rows.Count <= 0)
                {
                    return -1;// pallet not exist or shipped
                }
                param = new OleDbParameter[]
                                             {
                                             new OleDbParameter("WH_ID",WH_ID),
                                             new OleDbParameter("PALLET_NO",PALLET_NO),
                                             new OleDbParameter("ROW_POSITION",ROW_POSITION),
                                             new OleDbParameter("COL_POSITION",COL_POSITION),
                                             new OleDbParameter("SORT_POSITION",SORT_POSITION),
                                             new OleDbParameter("EMP",EMP)
                                              };
                sql = $@"INSERT INTO SFCBASE.C_WAREHOUSE_PALLET_POSITION_T VALUES(:WH_ID,:PALLET_NO,:ROW_POSITION,:COL_POSITION,:SORT_POSITION,0,:EMP,SYSDATE,null)";
                result = DB.ExecuteNonQuery(sql, CommandType.Text, param);
                if (result > 0)
                    return 1;//insert success
                else
                    return 2;//insert fail
            }
            catch (Exception e)
            {
                return 3;//insert problem
                throw e;
            }
        }
        public List<C_WAREHOUSE_PALLET_POSITION_T> GetPalletInfo(string WH_ID, string DATA_SEARCH, OleExec DB)
        {
            string sql = "";
            List<C_WAREHOUSE_PALLET_POSITION_T> ret = new List<C_WAREHOUSE_PALLET_POSITION_T>();
            DataSet res = null;
            try
            {
                //sql = $@"SELECT *FROM C_WAREHOUSE_PALLET_POSITION_t WHERE WH_ID='{WH_ID}' AND PALLET_NO='{PALLET_NO}' AND OUT_FLAG=0";
                sql = $@"select *from sfcbase.c_warehouse_pallet_position_t where OUT_FLAG='0' and pallet_no in (
                        SELECT PACK_NO FROM r_packing WHERE PACK_NO='{DATA_SEARCH}' AND PACK_TYPE='PALLET')";
                res = DB.ExecSelect(sql);
                if (res.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in res.Tables[0].Rows)
                    {
                        ret.Add(ConstructWarehouse(dr));
                    }
                    return ret;
                }
                else return null;
            }
            catch (Exception e)
            {
                throw e;
            }
            //return null;
        }
        public int DeletePalletOutPosition(string WH_ID, List<string> PALLET_LIST,int row, int col,OleExec DB)
        {
            try
            {
                if (!string.IsNullOrEmpty(WH_ID) && PALLET_LIST.Count > 0)
                {
                    string str = "";
                    for (int i = 0; i < PALLET_LIST.Count; i++)
                    {
                        str += ",'" + PALLET_LIST[i] + "'";
                        if (str.Length > 17)
                        {
                            str = str.Substring(str.Length - 13, 12);
                            str = "''" + str + "'";
                        }
                    }
                    string sql1 = $@"select * from sfcbase.C_WAREHOUSE_PALLET_POSITION_T  WHERE WH_ID='{WH_ID}' and ROW_POSITION='{row}' AND COL_POSITION='{col}' AND PALLET_NO IN({str.Remove(0, 1)}) AND OUT_FLAG=0";
                    DataSet res = null;
                    res = DB.ExecSelect(sql1);
                    if (res.Tables[0].Rows.Count == 0)
                    {
                        return 4;
                    }
                    string sql = $@"UPDATE sfcbase.C_WAREHOUSE_PALLET_POSITION_T SET OUT_FLAG=1,TIME_CHECK_OUT=SYSDATE WHERE WH_ID='{WH_ID}' AND PALLET_NO IN({str.Remove(0, 1)}) AND OUT_FLAG=0";
                    int result = DB.ExecuteNonQuery(sql, CommandType.Text);
                    if (result > 0)
                    {
                        return 1;
                    }
                    else
                        return 0;
                }
                else return 2;

            }
            catch (Exception e)
            {
                return 3;  //exception
                throw e;

            }

        }
        public int InsertPLtoLocation(string wh_id, string listpl,string EMP ,OleExec DB)
        {
            string sql="",sqlcheck;
            int result=0;
            var lspl = listpl.Split('\n');
            DataSet res = null;
            for (int i=0; i < lspl.Length; i++)
            {
                string pl = lspl[i].ToString();
                sqlcheck = $@"select * from C_WAREHOUSE_PALLET_POSITION_T where pallet_no ='{pl}' ";
                res = DB.ExecSelect(sqlcheck);
                if (res.Tables[0].Rows.Count != 0)
                {
                    sql = $@"update C_WAREHOUSE_PALLET_POSITION_T set wh_id='{wh_id}',EMP='{EMP}',TIME_CHECK_IN=SYSDATE,OUT_FLAG=0  where pallet_no ='{pl}'";
                    result = DB.ExecuteNonQuery(sql, CommandType.Text);
                }
                else
                {
                    sql = $@"INSERT INTO SFCBASE.C_WAREHOUSE_PALLET_POSITION_T (WH_ID,PALLET_NO,OUT_FLAG,EMP,TIME_CHECK_IN)
                                                                    VALUES('{wh_id}','{pl}',0,'{EMP}',SYSDATE)";
                    result = DB.ExecuteNonQuery(sql, CommandType.Text);
                }
            }
            return result;
        }
        public int UpdatePLLocation(string wh_id, string listpl, string EMP, OleExec DB)
        {
            string sql = "";
            int result = 0;
            var lspl = listpl.Split('\n');
            for (int i = 0; i < lspl.Length; i++)
            {
                string pl = lspl[i].ToString();
                sql = $@"UPDATE  SFCBASE.C_WAREHOUSE_PALLET_POSITION_T SET OUT_FLAG=1,TIME_CHECK_OUT=SYSDATE WHERE PALLET_NO='{pl}' AND WH_ID='{wh_id}' AND OUT_FLAG=0";
                result = DB.ExecuteNonQuery(sql, CommandType.Text);
            }

            return result;

        }
        public bool checkplexit(string pl, OleExec db)
        {
            string sql;
            DataSet res;
            sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUSE_PALLET_POSITION_T WHERE PALLET_NO='{pl}' AND OUT_FLAG=0";
            res = db.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
        public bool checkplexitlocation(string pl,string wh_id, OleExec db)
        {
            string sql;
            DataSet res;
            sql = $@"SELECT * FROM  SFCBASE.C_WAREHOUSE_PALLET_POSITION_T WHERE PALLET_NO='{pl}' and wh_id='{wh_id}' AND OUT_FLAG=0";
            res = db.ExecSelect(sql);
            if (res.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            return false;
        }
    }
    public class C_WAREHOUSE_PALLET_POSITION_T
    {
        public string WH_ID { get; set; }
        public string PALLET_NO { get; set; }

        public double? ROW_POSITION { get; set; }
        public double? COL_POSITION { get; set; }
        public double? SORT_POSITION { get; set; }
        public double? OUT_FLAG { get; set; }
        public string EMP { get; set; }
        public DateTime? TIME_CHECK_IN { get; set; }
        public DateTime? TIME_CHECK_OUT { get; set; }
    }

    public class Row_C_WAREHOUSE_PALLET_POSITION_T : DataObjectBase
    {
        public Row_C_WAREHOUSE_PALLET_POSITION_T(DataObjectInfo info) : base(info)
        {

        }
        public C_WAREHOUSE_PALLET_POSITION_T GetDataObject()
        {
            C_WAREHOUSE_PALLET_POSITION_T DataObject = new C_WAREHOUSE_PALLET_POSITION_T();
            DataObject.WH_ID = this.WH_ID;
            DataObject.PALLET_NO = this.PALLET_NO;
            DataObject.ROW_POSITION = this.ROW_POSITION;
            DataObject.COL_POSITION = this.COL_POSITION;
            DataObject.SORT_POSITION = this.SORT_POSITION;
            DataObject.OUT_FLAG = this.OUT_FLAG;
            DataObject.EMP = this.EMP;
            DataObject.TIME_CHECK_IN = this.TIME_CHECK_IN;
            DataObject.TIME_CHECK_OUT = this.TIME_CHECK_OUT;
            return DataObject;
        }
        public string WH_ID
        {
            get
            {
                return (string)this["WH_ID"];
            }
            set
            {
                this["WH_ID"] = value;
            }
        }
        public string PALLET_NO
        {
            get
            {
                return (string)this["PALLET_NO"];
            }
            set
            {
                this["PALLET_NO"] = value;
            }
        }
        public double? ROW_POSITION
        {
            get
            {
                return (double?)this["ROW_POSITION"];
            }
            set
            {
                this["ROW_POSITION"] = value;
            }
        }
        public double? COL_POSITION
        {
            get
            {
                return (double?)this["COL_POSITION"];
            }
            set
            {
                this["COL_POSITION"] = value;
            }
        }
        public double? SORT_POSITION
        {
            get
            {
                return (double?)this["SORT_POSITION"];
            }
            set
            {
                this["SORT_POSITION"] = value;
            }
        }
        public double? OUT_FLAG
        {
            get
            {
                return (double?)this["OUT_FLAG"];
            }
            set
            {
                this["OUT_FLAG"] = value;
            }
        }
        public string EMP
        {
            get
            {
                return (string)this["EMP"];
            }
            set
            {
                this["EMP"] = value;
            }
        }
        public DateTime? TIME_CHECK_IN
        {
            get
            {
                return (DateTime?)this["TIME_CHECK_IN"];
            }
            set
            {
                this["TIME_CHECK_IN"] = value;
            }
        }

        public DateTime? TIME_CHECK_OUT
        {
            get
            {
                return (DateTime?)this["TIME_CHECK_OUT"];
            }
            set
            {
                this["TIME_CHECK_OUT"] = value;
            }
        }

    }
}
