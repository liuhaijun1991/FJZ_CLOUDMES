using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;
using System.Reflection;
using System.Collections;

namespace MESDataObject.Module
{
    public class T_R_PANEL_SN : DataObjectTable
    {
        public T_R_PANEL_SN(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PANEL_SN(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PANEL_SN);
            TableName = "R_PANEL_SN".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public int ReplaceRPanelSn(string NewSn, string OldSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            string strSql = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                strSql = $@"UPDATE r_panel_sn R SET R.SN='{NewSn}' WHERE R.SN='{OldSn}'";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return result;
        }

        /// <summary>
        /// 獲取數據庫時間
        /// </summary>
        /// <param name="DB"></param>
        /// <returns></returns>
        //public DateTime GetDBDateTime(OleExec DB)
        //{
        //    string strSql = "select sysdate from dual";
        //    if (this.DBType == DB_TYPE_ENUM.Oracle)
        //    {
        //        strSql = "select sysdate from dual";
        //    }
        //    else if (this.DBType == DB_TYPE_ENUM.SqlServer)
        //    {
        //        strSql = "select get_date() ";
        //    }
        //    else
        //    {
        //        throw new Exception(this.DBType.ToString() + " not Work");
        //    }
        //    return (DateTime)DB.ExecSelectOneValue(strSql);

        //}

        public string AddSnToPanel(List<R_PANEL_SN> PanelSNs,string Bu, OleExec DB)
        {
            string sql = string.Empty;
            Row_R_PANEL_SN row = null;
            string result = string.Empty;
            DateTime DateTime = GetDBDateTime(DB);
            T_R_PANEL_SN TRPS = new T_R_PANEL_SN(DB, DBType);

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                foreach (R_PANEL_SN PanelSn  in PanelSNs)
                {
                    PanelSn.ID = TRPS.GetNewID(Bu, DB);
                    PanelSn.EDIT_TIME = DateTime;
                    row = (Row_R_PANEL_SN)this.ConstructRow(PanelSn);
                    sql = row.GetInsertString(this.DBType);
                    result = DB.ExecSQL(sql);
                }
                return result;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public Boolean CheckPanelExist(string _panel, OleExec DB)
        {
            string strsql = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                strsql = $@"select COUNT(ID) from r_panel_sn where panel='{_panel.Replace("'", "''")}'";
                string ID = DB.ExecSelectOneValue(strsql).ToString();
                if (Convert.ToInt32(ID) == 0)
                {
                    return false;
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return true;
        }
        public List<R_PANEL_SN> GetPanel(string _panel, OleExec DB)
        {
            string strList = "";
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                List<R_PANEL_SN> list = new List<R_PANEL_SN>();
                strList = $@"select * from r_panel_sn where panel='{_panel.Replace("'", "''")}'";
                DataTable dt = new DataTable();
                dt = DB.ExecSelect(strList).Tables[0];
                list = DataTableToList<R_PANEL_SN>(dt);
                return list;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<R_SN> GetSn(string _panel, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                List<R_SN> list = new List<R_SN>();
                string sql = $@"SELECT * FROM R_SN  WHERE valid_flag = '1' and SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{_panel.Replace("'", "''")}')";
                DataTable dt = new DataTable();
                dt = DB.ExecSelect(sql).Tables[0];
                list = DataTableToList<R_SN>(dt);
                return list;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public List<R_SN> GetValidSnByPanel(string _panel, OleExec DB)
        {
            if (DBType == DB_TYPE_ENUM.Oracle)
            {
                List<R_SN> list = new List<R_SN>();
                string sql = $@"SELECT * FROM R_SN  WHERE valid_flag='1' and  SN IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{_panel.Replace("'", "''")}')";
                DataTable dt = new DataTable();
                dt = DB.ExecSelect(sql).Tables[0];
                list = DataTableToList<R_SN>(dt);
                return list;
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
        }

        public R_PANEL_SN GetPanelBySn(string SerialNo, OleExec DB)
        {
            string sql = string.Empty;
            DataTable dt = new DataTable();
            Row_R_PANEL_SN row = (Row_R_PANEL_SN)NewRow();
            R_PANEL_SN Panel = null;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                sql = $@"SELECT * FROM R_PANEL_SN WHERE SN='{SerialNo}'";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    row.loadData(dt.Rows[0]);
                    Panel = row.GetDataObject();
                }
            }
            else
            {
                string errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return Panel;
        }

        //datatable turn to list
        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            // 定义集合  
            List<T> ts = new List<T>();
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                                       //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                        {
                            if (pi.PropertyType.FullName.Contains("System.Nullable`1[[System.Double"))
                            {
                                pi.SetValue(t, value == null ? 0 : double.Parse(value.ToString()));
                            }
                            else if (pi.PropertyType.FullName.Contains("System.Nullable`1[[System.Int"))
                            {
                                pi.SetValue(t, value == null ? 0 : Convert.ToInt32(value));
                            }
                            else
                            {
                                pi.SetValue(t, value, null);
                            }
                        }
                        //pi.SetValue(t, value);
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }
            return ts;
        }

        public bool CheckPanelVirtualSN(string Panel, OleExec DB)
        {
            string StrSql = $@"select * from r_sn where sn in(select sn from r_panel_sn where panel='{Panel}') and ID=SN";
            bool CheckFlag = false;
            DataTable Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                CheckFlag = true;
            }
            return CheckFlag;
        }

        public R_PANEL_SN GetPanelVirtualSN(string Panel, OleExec DB)
        {
            R_PANEL_SN Row = null;
            string StrSql = $@"SELECT B.* FROM R_SN A, R_PANEL_SN B WHERE B.PANEL='{ Panel }' AND A.ID=B.SN order by B.SEQ_NO";
            DataTable Dt = DB.ExecSelect(StrSql).Tables[0];
            if (Dt.Rows.Count > 0)
            {
                Row=GetRow(Dt.Rows[0]);
            }
            return Row;
        }


        public R_PANEL_SN GetRow(DataRow DR)
        {
            Row_R_PANEL_SN Ret = (Row_R_PANEL_SN)NewRow();
            Ret.loadData(DR);
            return Ret.GetDataObject();
        }

        //選出Panel下面的SN中站位在最前面的作為Panel的站位
        //add by ZGJ 2018-03-07
        public string GetPanelNextStation(string PanelNo, OleExec DB)
        {
            string NextStation = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string errMsg = string.Empty;

            if (this.DBType == DB_TYPE_ENUM.Oracle)
            {
                if (!string.IsNullOrEmpty(PanelNo))
                {
                    sql = $@"SELECT STATION_NAME FROM 
                            (SELECT * FROM C_ROUTE_DETAIL 
                                WHERE ROUTE_ID =(SELECT DISTINCT ROUTE_ID FROM R_SN A WHERE EXISTS(SELECT * FROM R_PANEL_SN B WHERE A.SN=B.SN AND A.WORKORDERNO=B.WORKORDERNO AND PANEL='{PanelNo}') AND VALID_FLAG='1' AND ROWNUM=1)
                                AND STATION_NAME IN (SELECT DISTINCT NEXT_STATION FROM R_SN C WHERE EXISTS(SELECT * FROM R_PANEL_SN D WHERE C.SN=D.SN AND C.WORKORDERNO=D.WORKORDERNO AND PANEL='{PanelNo}') AND VALID_FLAG='1' /*AND PRODUCT_STATUS = 'FRESH'*/)
                                ORDER BY SEQ_NO 
                            )
                        WHERE ROWNUM=1";//修復BUG－一個PANEL裡有SN已經掃LINK，其他沒過BIP的產品會導致ROUTE改變，增加WORKORDER作為條件2018-04-07 11:00 by LJD
                    dt = DB.ExecSelect(sql).Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        NextStation = dt.Rows[0]["STATION_NAME"].ToString();
                    }
                    else
                    {
                        errMsg = MESReturnMessage.GetMESReturnMessage("MES00000146", new string[] { PanelNo});
                        throw new MESReturnMessage(errMsg);
                    }
                }
                else
                {
                    errMsg = MESReturnMessage.GetMESReturnMessage("MES00000147", new string[] { });
                    throw new MESReturnMessage(errMsg);
                }
            }
            else
            {
                errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return NextStation;
        }

        /// <summary>
        /// 返回 Panel 的唯一站位，如果根據 Panel 查不到對應的 R_SN 記錄或者查到多條下一站不同的 R_SN 記錄
        /// 則返回空字符串""
        /// add by ZGJ 2018-3-24
        /// </summary>
        /// <param name="PanelNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetPanelUniqueStation(string PanelNo, OleExec DB)
        {
            string UniqueStation = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string errMsg = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT DISTINCT CURRENT_STATION FROM R_SN WHERE ID IN 
                        (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelNo}')";
                dt = DB.ExecSelect(sql, null).Tables[0];

                if (dt.Rows.Count == 1)
                {
                    UniqueStation = dt.Rows[0]["CURRENT_STATION"].ToString();
                }
            }
            else
            {
                errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

                return UniqueStation;
        }

        public int SetPanelInValid(string PanelNo, OleExec DB,DB_TYPE_ENUM DBType)
        {
            string strSql = string.Empty;
            int result = 0;
            string errMsg = string.Empty;

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                strSql = $@"UPDATE R_SN SET VALID_FLAG='0',CURRENT_STATION='UNDO_LOADING' WHERE ID IN 
                        (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelNo}')";
                result = DB.ExecSqlNoReturn(strSql, null);
            }
            else
            {
                errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;

        }

        public int RecordPanelStationDetail(string PanelNo,string Line,string StationName,string DeviceName,string Bu, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int result = 0;
            //string strSql = string.Empty;
            //DataTable dt = new DataTable();
            string errMsg = string.Empty;
            T_R_SN RSN = new T_R_SN(DB, DBType);

            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                var snlist = DB.ORM.Queryable<R_SN, R_PANEL_SN>((rs, rps) => rs.ID == rps.SN)
                    .Where((rs, rps) => rps.PANEL == PanelNo).Select((rs, rps) => rs).ToList();
                foreach (var snobj in snlist)
                {
                    RSN.RecordPassStationDetail(snobj, Line, StationName, DeviceName, Bu, DB);
                }
                
                //strSql = $@"SELECT * FROM R_SN WHERE ID IN (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelNo}')";
                //dt = DB.ExecSelect(strSql, null).Tables[0];
                //foreach (DataRow dr in dt.Rows)
                //{
                //    RSN.RecordPassStationDetail(dr["SN"].ToString(), Line, StationName, DeviceName, Bu, DB);
                //}
            }
            else
            {
                errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }

            return result;
        }

        /// <summary>
        /// 獲取BIP Panel未分板數量
        /// </summary>
        /// <param name="PanelSn"></param>
        /// <param name="DB"></param>
        /// <param name="DBType"></param>
        /// <returns></returns>
        public int PanelNoBIPQty(string PanelSn, OleExec DB, DB_TYPE_ENUM DBType)
        {
            int NoBIPQty = 0;
            string errMsg = string.Empty;
            if (DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                string strsql = $@"select count(1) from r_sn where sn in (
                        select sn from r_panel_sn where panel = '{PanelSn}' ) 
                        and id = sn";
                NoBIPQty = int.Parse(DB.ExecSelectOneValue(strsql).ToString());
            }
            else
            {
                errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return NoBIPQty;
        }

        /// <summary>
        /// 返回 Panel 的唯一當前站位，如果根據 Panel 查不到對應的 R_SN 記錄或者查到多條當前站位 不同的 R_SN 記錄
        /// 則返回空字符串""
        /// </summary>
        /// <param name="PanelNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public string GetPanelCurrentStation(string PanelNo, OleExec DB)
        {
            string CurrentStation = string.Empty;
            string sql = string.Empty;
            DataTable dt = new DataTable();
            string errMsg = string.Empty;

            if (this.DBType.Equals(DB_TYPE_ENUM.Oracle))
            {
                sql = $@"SELECT DISTINCT CURRENT_STATION FROM R_SN WHERE SN IN 
                        (SELECT SN FROM R_PANEL_SN WHERE PANEL='{PanelNo}')";
                dt = DB.ExecSelect(sql, null).Tables[0];
                if (dt.Rows.Count == 1)
                {
                    CurrentStation = dt.Rows[0]["CURRENT_STATION"].ToString();
                }
            }
            else
            {
                errMsg = MESReturnMessage.GetMESReturnMessage("MES00000019", new string[] { DBType.ToString() });
                throw new MESReturnMessage(errMsg);
            }
            return CurrentStation;
        }

        public int UpdateWOByPanel(string oldWO,string newWO,string panelSn, OleExec DB)
        {
            return DB.ORM.Updateable<R_PANEL_SN>().UpdateColumns(r => new R_PANEL_SN { WORKORDERNO = newWO }).Where(r => r.WORKORDERNO == oldWO && r.PANEL == panelSn).ExecuteCommand();
        }
        /// <summary>
        /// 根據SN檢查該SN所處的PANEL是否分板OK
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="sn"></param>
        /// <returns></returns>
        public bool CheckPanelIsBIPOKBySN(OleExec sfcdb, string sn)
        {
            string sql = $@"select * from r_sn where sn in ( select sn from r_panel_sn where panel in (select panel from r_panel_sn where sn='{sn}')) and id=sn ";          
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool CheckPanelBySNStatus(OleExec sfcdb, string panel,string station)
        {
            string sql = $@"select count(1) lotnum
  from (select count(1) as lotnum1
          from r_sn a, r_panel_sn b
         where a.sn = b.sn
           and b.panel = '{panel}'
           and a.valid_flag =1
           and a.next_station = '{station}') a,
       (select count(1) as lotnum2 from r_panel_sn where panel = '{panel}') b
 where a.lotnum1 = b.lotnum2
   and a.lotnum1 <> 0 ";
            DataTable dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
    public class Row_R_PANEL_SN : DataObjectBase
    {
        public Row_R_PANEL_SN(DataObjectInfo info) : base(info)
        {

        }
        public R_PANEL_SN GetDataObject()
        {
            R_PANEL_SN DataObject = new R_PANEL_SN();
            DataObject.ID = this.ID;
            DataObject.SN = this.SN;
            DataObject.PANEL = this.PANEL;
            DataObject.WORKORDERNO = this.WORKORDERNO;
            DataObject.SEQ_NO = this.SEQ_NO;
            DataObject.EDIT_EMP = this.EDIT_EMP;
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
        public string SN
        {
            get
            {
                return (string)this["SN"];
            }
            set
            {
                this["SN"] = value;
            }
        }
        public string PANEL
        {
            get
            {
                return (string)this["PANEL"];
            }
            set
            {
                this["PANEL"] = value;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return (string)this["WORKORDERNO"];
            }
            set
            {
                this["WORKORDERNO"] = value;
            }
        }
        public double? SEQ_NO
        {
            get
            {
                return (double)this["SEQ_NO"];
            }
            set
            {
                this["SEQ_NO"] = value;
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
                return (DateTime)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
    }
    public class R_PANEL_SN
    {
        public string ID { get; set; }
        public string SN { get; set; }
        public string PANEL { get; set; }
        public string WORKORDERNO { get; set; }
        public double? SEQ_NO { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime EDIT_TIME { get; set; }
    }
}
