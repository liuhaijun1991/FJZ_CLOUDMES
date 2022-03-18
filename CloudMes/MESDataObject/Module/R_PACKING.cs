using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using MESDBHelper;


namespace MESDataObject.Module
{
    public class T_R_PACKING : DataObjectTable
    {
        public T_R_PACKING(string _TableName, OleExec DB, DB_TYPE_ENUM DBType) : base(_TableName, DB, DBType)
        {

        }
        public T_R_PACKING(OleExec DB, DB_TYPE_ENUM DBType)
        {
            RowType = typeof(Row_R_PACKING);
            TableName = "R_PACKING".ToUpper();
            DataInfo = GetDataObjectInfo(TableName, DB, DBType);
        }

        public Row_R_PACKING GetRPackingByPackNo(OleExec DB, string PackNo)
        {
            string strSql = $@" SELECT * FROM R_PACKING where PACK_NO='{PackNo}' ";
            DataSet ds = DB.ExecSelect(strSql);
            Row_R_PACKING r = (Row_R_PACKING)this.NewRow();
            if (ds.Tables[0].Rows.Count > 0)
                r.loadData(ds.Tables[0].Rows[0]);
            return r;
        }

        public R_PACKING GetPackingByID(string PackId, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING>().Where(t => t.ID == PackId).ToList().FirstOrDefault();
        }

        public List<R_PACKING> GetListPackByParentPackId(string parentPackId, OleExec DB)
        {
            List<R_PACKING> packingList = new List<R_PACKING>();
            Row_R_PACKING rowPacking;
            string strSql = $@"select * from r_packing where parent_pack_id='{parentPackId}'";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                rowPacking = (Row_R_PACKING)this.NewRow();
                rowPacking.loadData(row);
                packingList.Add(rowPacking.GetDataObject());
            }
            return packingList;
        }
        public List<R_PACKING> GetCarton(string sn, OleExec DB)
        {
            List<R_PACKING> packingList = new List<R_PACKING>();
            Row_R_PACKING rowPacking;
            string strSql = $@"SELECT * FROM r_packing WHERE id  IN (
                                        SELECT PACK_ID FROM r_sn_packing WHERE SN_ID IN (
                                        SELECT ID FROM r_sn WHERE sn ='{sn}'))";
            DataSet ds = DB.ExecSelect(strSql);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                rowPacking = (Row_R_PACKING)this.NewRow();
                rowPacking.loadData(row);
                packingList.Add(rowPacking.GetDataObject());
            }
            return packingList;
        }
        public bool CheckCloseByPackno(string packNo, OleExec db)
        {
            string sql = $@" SELECT * FROM R_PACKING where closed_flag='1' and PACK_NO='{packNo}' ";
            DataSet ds = db.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string CheckSNinPallet(string sn, string pl, OleExec db)
        {
            string sql = $@"SELECT * FROM r_packing WHERE id IN (
                            SELECT PARENT_PACK_ID FROM r_packing WHERE ID IN (
                            SELECT PACK_ID FROM r_sn_packing WHERE SN_ID IN (
                            SELECT ID FROM r_sn WHERE sn ='{sn}'))) AND PACK_NO='{pl}'";
            DataSet ds = db.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return "OK";
            }
            else
            {
                return "";
            }

        }
        public string CheckStatusSNinPallet( string pl, OleExec db)
        {
            string sql = $@"SELECT * FROM r_sn rsn,r_sn_packing rsnp ,r_packing rpc, r_packing rpp WHERE
                            rsn.ID= RSNP.SN_ID AND RSNP.PACK_ID=RPC.ID AND
                            rpc.PARENT_PACK_ID= rpp.ID AND rpp.PACK_NO='{pl}' AND (rsn.NEXT_STATION <>'SHIPFINISH' AND rsn.SHIPPED_FLAG <> 1)";
            DataSet ds = db.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return "OK";
            }
            else
            {
                return "";
            }

        }
        public bool CheckSNPrexid(string packNo, OleExec db)
        {
            string sql = $@" select RP1.* from R_PACKING RP1,R_PACKING RP2,R_SN_PACKING RSP,R_SN RS,R_FUNCTION_CONTROL RFC,R_FUNCTION_CONTROL_EX RFCE 
                            WHERE RP1.ID=RP2.PARENT_PACK_ID AND RP2.ID=RSP.PACK_ID AND RS.ID=RSP.SN_ID AND RP1.SKUNO=RFC.VALUE AND RFC.ID=RFCE.DETAIL_ID
                            AND RFC.EXTVAL <>SUBSTR(RS.SN,1,4) AND RFC.CONTROLFLAG='Y' AND RFC.FUNCTIONNAME='CHECK_PREXID'AND RFCE.SEQ_NO=1
                            AND RFCE.Value<to_char(sysdate,'YYYYMMDD') and RP1.PACK_NO='{packNo}'";
            DataSet ds = db.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查卡通是否在對應棧板內
        /// </summary>
        /// <param name="packNo"></param>
        /// <param name="parnetPackID"></param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public bool CheckPackNoExistByParentPackID(string packNo, string parnetPackID, OleExec sfcdb)
        {
            string sql = $@" SELECT * FROM R_PACKING where parent_pack_id='{parnetPackID}' and PACK_NO='{packNo}' ";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查卡通是否已經在棧板內
        /// </summary>
        /// <param name="PackNo"></param>
        /// <param name="ParentPackNo"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public bool CheckPackNoExistInParentPack(string PackNo, string ParentPackNo, OleExec DB)
        {
            R_PACKING Packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == ParentPackNo).ToList().FirstOrDefault();
            if (Packing != null)
            {
                return CheckPackNoExistByParentPackID(PackNo, Packing.ID, DB);
            }
            else
            {
                return false;
            }
        }

        public string UpdateParentPackIDByPackNo(string packNo, string parnetPackID, string emp, OleExec sfcdb)
        {
            string sql = $@" update r_packing set parent_pack_id='{parnetPackID}',edit_time=sysdate,edit_emp='{emp}' where pack_no='{packNo}'";
            return sfcdb.ExecSQL(sql);
        }

        /// <summary>
        /// 更新子包裝單位的父包裝
        /// </summary>
        /// <param name="PackNo"></param>
        /// <param name="ParentPackNo"></param>
        /// <param name="Emp"></param>
        /// <param name="DB"></param>
        /// <returns></returns>
        public int UpdateParentPackByChildPack(string PackNo, string ParentPackNo, string Emp, OleExec DB)
        {
            R_PACKING packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == ParentPackNo).ToList().FirstOrDefault();
            if (packing != null)
            {
                return DB.ORM.Updateable<R_PACKING>().UpdateColumns(t => new R_PACKING() { PARENT_PACK_ID = packing.ID, EDIT_EMP = Emp }).Where(t => t.PACK_NO == PackNo).ExecuteCommand();
            }
            else
            {
                return 0;
            }
        }

        public string UpdateParentPackIDBySN(string sn, string parentPackID, OleExec sfcdb)
        {
            string sql = $@" update r_packing set parent_pack_id='' where id in (select n.pack_id from r_sn_packing n,r_sn m where n.sn_id=m.id and m.sn='{sn}' and m.valid_flag='1')";
            return sfcdb.ExecSQL(sql);
        }

        public R_PACKING GetPackingObjectBySN(string sn, OleExec sfcdb)
        {
            string sql = $@"select * from r_packing  where id in (select n.pack_id from r_sn_packing n,r_sn m where n.sn_id=m.id and m.sn='{sn}' and m.valid_flag='1')";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_PACKING rowPacking = (Row_R_PACKING)this.NewRow();
                rowPacking.loadData(ds.Tables[0].Rows[0]);
                return rowPacking.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public int JNPGetCTNqty(string StrPackno, string BU, OleExec sfcdb)
        {
            string Strqty = "0";
            int qty = 0;
            DataTable dt = new DataTable();

            string sql = $@"SELECT count (*) as cartonQtys FROM r_packing WHERE parent_pack_id in(
            SELECT ID FROM r_packing WHERE pack_no ='{StrPackno}')";
            dt = sfcdb.ExecSelect(sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                Strqty = dt.Rows[0]["cartonQtys"].ToString();
                qty = Convert.ToInt32(Strqty);
                return qty;
            }
            else
            {
                return qty;
            }
        }

        public R_PACKING GetPackingBySNorCarton(string value, string packno, OleExec sfcdb)
        {
            string sql = $@"select * from r_packing where id in(
                            select PARENT_PACK_ID from r_packing where pack_no='{value}') and pack_no='{packno}'
                            union
                            select * from r_packing where id in(
                            select PARENT_PACK_ID from r_packing  where id in 
                            (select n.pack_id from r_sn_packing n,r_sn m where n.sn_id=m.id and m.sn='{value}' and m.valid_flag='1')) and pack_no='{packno}' ";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_PACKING rowPacking = (Row_R_PACKING)this.NewRow();
                rowPacking.loadData(ds.Tables[0].Rows[0]);
                return rowPacking.GetDataObject();
            }
            else
            {
                return null;
            }
        }

        public string UpdateCloseFlagByPackID(string packID, string closedFlag, OleExec sfcdb)
        {
            Row_R_PACKING rowPacking = (Row_R_PACKING)this.GetObjByID(packID, sfcdb);
            rowPacking.CLOSED_FLAG = closedFlag;
            return sfcdb.ExecSQL(rowPacking.GetUpdateString(DB_TYPE_ENUM.Oracle));
        }

        public string UpdateQtyByID(string packId, bool isAdd, double qty, string emp, OleExec sfcdb)
        {
            Row_R_PACKING rowPacking = (Row_R_PACKING)this.GetObjByID(packId, sfcdb);
            if (isAdd)
            {
                rowPacking.QTY = rowPacking.QTY + qty;
            }
            else
            {
                rowPacking.QTY = rowPacking.QTY - qty;
            }
            rowPacking.EDIT_TIME = this.GetDBDateTime(sfcdb);
            rowPacking.EDIT_EMP = emp;
            return sfcdb.ExecSQL(rowPacking.GetUpdateString(DB_TYPE_ENUM.Oracle));
        }

        public bool PackNoIsExist(string packNo, OleExec sfcdb)
        {
            string sql = $@" SELECT * FROM R_PACKING where PACK_NO='{packNo}' ";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GetSNByPackNo(string PackNo, ref List<R_SN> SNList, OleExec DB)
        {
            List<string> Ids = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).Select(t => t.ID).ToList();
            foreach (string Id in Ids)
            {
                List<string> ChildPackNos = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == Id).Select(t => t.PACK_NO).ToList();
                foreach (string ChildPackNo in ChildPackNos)
                {
                    GetSNByPackNo(ChildPackNo, ref SNList, DB);
                }

                List<R_SN> TempList = DB.ORM.Queryable<R_SN, R_SN_PACKING>((s, sp) => s.ID == sp.SN_ID).Where((s, sp) => sp.PACK_ID == Id).Select((s, sp) => s).ToList();
                SNList.AddRange(TempList);
            }
        }

        /// <summary>
        /// 傳入卡通號，獲取SN列表
        /// ADD BY HGB
        /// </summary>
        /// <param name="PackNo"></param>
        /// <param name="SNList"></param>
        /// <param name="DB"></param>
        public void GetSNListByPackNo(string PackNo, ref List<string> SNList, OleExec DB)
        {
            List<string> Ids = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).Select(t => t.ID).ToList();
            foreach (string Id in Ids)
            {
                List<string> ChildPackNos = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == Id).Select(t => t.PACK_NO).ToList();
                foreach (string ChildPackNo in ChildPackNos)
                {
                    GetSNListByPackNo(ChildPackNo, ref SNList, DB);
                }

                List<string> TempList = DB.ORM.Queryable<R_SN, R_SN_PACKING>((s, sp) => s.ID == sp.SN_ID).Where((s, sp) => sp.PACK_ID == Id).OrderBy(s => s.SN, SqlSugar.OrderByType.Asc).Select((s, sp) => s.SN).ToList();
                SNList.AddRange(TempList);
                #region 調試用
                string sql = $@" select * from r_sn s,r_sn_packing sp where s.ID = sp.SN_ID and sp.PACK_ID ='{Id}'";
                DataSet ds = DB.ExecSelect(sql);
                #endregion
            }
        }

        /// <summary>
        /// 傳入卡通號，獲取SN列表,sn去掉前三位字符
        /// HwtCartonLabelSON_LIST02  會用到  Thirdlabel用
        /// ADD BY HGB
        /// </summary>
        /// <param name="PackNo"></param>
        /// <param name="SNList"></param>
        /// <param name="DB"></param>
        public void GetSNListByPackNoForThirdlabel(string PackNo, ref List<string> SNList, OleExec DB)
        {
            List<string> Ids = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).Select(t => t.ID).ToList();
            foreach (string Id in Ids)
            {
                List<string> ChildPackNos = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == Id).Select(t => t.PACK_NO).ToList();
                foreach (string ChildPackNo in ChildPackNos)
                {
                    GetSNListByPackNoForThirdlabel(ChildPackNo, ref SNList, DB);
                }

                List<string> TempList = DB.ORM.Queryable<R_SN, R_SN_PACKING>((s, sp) => s.ID == sp.SN_ID).Where((s, sp) => sp.PACK_ID == Id).OrderBy(s => s.SN, SqlSugar.OrderByType.Asc).Select((s, sp) => s.SN.Substring(2, s.SN.Length - 2)).ToList();
                SNList.AddRange(TempList);
            }
        }

        /// <summary>
        /// 傳入卡通號，獲取SN的MAC列表
        /// ADD BY HGB
        /// </summary>
        /// <param name="PackNo"></param>
        /// <param name="SNList"></param>
        /// <param name="DB"></param>
        public void GetSN_MacListByPackNo(string PackNo, ref List<string> SNList, OleExec DB)
        {
            List<string> Ids = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).Select(t => t.ID).ToList();
            foreach (string Id in Ids)
            {
                List<string> ChildPackNos = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == Id).Select(t => t.PACK_NO).ToList();
                foreach (string ChildPackNo in ChildPackNos)
                {
                    GetSN_MacListByPackNo(ChildPackNo, ref SNList, DB);
                }

                List<string> TempList = DB.ORM.Queryable<R_SN, R_SN_PACKING, R_SN_MAC>((s, sp, mac) => s.ID == sp.SN_ID && s.SN == mac.SN && mac.SUBSN_TYPE == "MAC").Where((s, sp) => sp.PACK_ID == Id).OrderBy(s => s.SN, SqlSugar.OrderByType.Asc).Select((s, sp, mac) => mac.SUBSN).ToList();
                SNList.AddRange(TempList);

                #region 調試用
                string sql = $@" select mac.SUBSN from r_sn s,r_sn_packing sp ,R_SN_MAC mac where s.ID = sp.SN_ID and s.SN=mac.SN and sp.PACK_ID ='{Id}'";
                DataSet ds = DB.ExecSelect(sql);
                #endregion
            }
        }

        public List<string> GetPakcingSNList(string parentPackId, OleExec sfcdb)
        {
            string sql = $@"select c.sn from r_packing a,r_sn_packing b,r_sn c where a.parent_pack_id='{parentPackId}' and a.id = b.pack_id and b.sn_id = c.id ";
            List<string> snList = new List<string>();
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    snList.Add(ds.Tables[0].Rows[i]["SN"].ToString());
                }
            }
            else
            {
                snList = null;
            }
            return snList;
        }

        public List<R_SN> GetPakcingSNList(string packId, string packTtyp, OleExec sfcdb)
        {
            List<R_SN> snList = new List<R_SN>();
            if (packTtyp == "PALLET")
            {
                snList = sfcdb.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((rp, rsp, rs) => rp.ID == rsp.PACK_ID && rsp.SN_ID == rs.ID).Where((rp, rsp, rs) => rp.PARENT_PACK_ID == packId)
                    .Select((rp, rsp, rs) => new R_SN() { ID = SqlSugar.SqlFunc.GetSelfAndAutoFill(rs.ID) }).ToList();
            }
            else if (packTtyp == "CARTON")
            {
                snList = sfcdb.ORM.Queryable<R_SN_PACKING, R_SN>((rsp, rs) => rsp.SN_ID == rs.ID).Where((rsp, rs) => rsp.PACK_ID == packId)
                    .Select((rsp, rs) => new R_SN() { ID = SqlSugar.SqlFunc.GetSelfAndAutoFill(rs.ID) }).ToList();
            }
            return snList;
        }

        public R_PACKING InsertPacking(string Bu, string Line, string Station, string Ip, string Emp, string PackNo, string PackType, string ParentPackId, string Skuno, double? MaxQty, double? Qty, string CloseFlag, OleExec DB)
        {
            R_PACKING packing = new R_PACKING();
            packing.ID = GetNewID(Bu, DB);
            packing.PACK_NO = PackNo;
            packing.PACK_TYPE = PackType;
            packing.PARENT_PACK_ID = ParentPackId;
            packing.SKUNO = Skuno;
            packing.MAX_QTY = MaxQty;
            packing.QTY = Qty;
            packing.CLOSED_FLAG = "0";
            packing.CREATE_TIME = GetDBDateTime(DB);
            packing.EDIT_EMP = Emp;
            packing.EDIT_TIME = packing.CREATE_TIME;
            packing.LINE = Line;
            packing.STATION = Station;
            packing.CLOSED_FLAG = CloseFlag;
            packing.IP = Ip;
            int i = DB.ORM.Insertable<R_PACKING>(packing).ExecuteCommand();
            return packing;
        }

        public int UpdatePacking(R_PACKING Packing, OleExec DB)
        {
            return DB.ORM.Updateable<R_PACKING>(Packing).Where(t => t.ID == Packing.ID).ExecuteCommand();
        }

        public int DeletePacking(string PackNo, OleExec DB)
        {
            R_PACKING Packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
            if (Packing == null)
                return 0;
            List<R_PACKING> ChildPackings = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == Packing.ID).ToList();
            string id = "";
            foreach (R_PACKING CP in ChildPackings)
            {
                //DeletePacking(CP.PACK_NO, DB);
                id = CP.ID;
                CP.ID = "*" + CP.ID;
                CP.PACK_NO = "*" + CP.PACK_NO;
                CP.PARENT_PACK_ID = "*" + CP.PARENT_PACK_ID;
                DB.ORM.Updateable<R_PACKING>(CP).Where(r => r.ID == id).ExecuteCommand();
            }

            //DB.ORM.Deleteable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ExecuteCommand();
            id = Packing.ID;
            Packing.ID = "*" + Packing.ID;
            Packing.PACK_NO = "*" + Packing.PACK_NO;
            Packing.PARENT_PACK_ID = string.IsNullOrEmpty(Packing.PARENT_PACK_ID) ? "" : "*" + Packing.PARENT_PACK_ID;
            return DB.ORM.Updateable<R_PACKING>(Packing).Where(t => t.ID == id).ExecuteCommand();
            //return 1;
        }

        public string UpdatePalletNoByCartonParentID(string ID, string emp, OleExec sfcdb)
        {
            string sql = $@" update r_packing set qty=qty-1,edit_emp='{emp}',edit_time=sysdate where id='{ID}' ";
            return sfcdb.ExecSQL(sql);

        }
        /// <summary>
        /// 關閉當前包裝單位及下級所有包裝單位
        /// </summary>
        /// <param name="PackNo"></param>
        /// <param name="Emp"></param>
        /// <param name="DB"></param>
        public void ClosePack(string PackNo, string Emp, OleExec DB)
        {
            R_PACKING Packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
            if (Packing != null)
            {
                List<R_PACKING> ChildPackings = DB.ORM.Queryable<R_PACKING>().Where(t => t.PARENT_PACK_ID == Packing.ID).ToList();
                foreach (R_PACKING CP in ChildPackings)
                {
                    ClosePack(CP.PACK_NO, Emp, DB);
                }

                DB.ORM.Updateable<R_PACKING>().UpdateColumns(t => new R_PACKING() { CLOSED_FLAG = "1", EDIT_EMP = Emp, EDIT_TIME = DateTime.Now })
                    .Where(t => t.ID == Packing.ID).ExecuteCommand();
            }
        }


        /// <summary>
        /// get sn object list by pallet ID
        /// </summary>
        /// <param name="palletID">pallet ID</param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SN> GetSnListByPalletID(string palletID, OleExec sfcdb)
        {
            string sql = $@"select c.* from r_packing a,r_sn_packing b,r_sn c where a.parent_pack_id='{palletID}' and a.id = b.pack_id and b.sn_id = c.id and c.shipped_flag='1'";
            List<R_SN> snList = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(sfcdb, this.DBType);
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Row_R_SN rowSN = (Row_R_SN)t_r_sn.NewRow();
                    rowSN.loadData(row);
                    snList.Add(rowSN.GetDataObject());
                }
            }
            return snList;
        }

        /// <summary>
        /// get sn object list by pallet ID but the Pallet haven't DN
        /// </summary>
        /// <param name="palletID">pallet ID</param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SN> GetSnListByPalletIDShippedNotyet(string palletID, OleExec sfcdb)
        {
            string sql = $@"select c.* from r_packing a,r_sn_packing b,r_sn c where a.parent_pack_id='{palletID}' and a.id = b.pack_id and b.sn_id = c.id ";//and a.CLOSED_FLAG !='1' and c.SHIPPED_FLAG!='1'
            List<R_SN> snList = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(sfcdb, this.DBType);
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Row_R_SN rowSN = (Row_R_SN)t_r_sn.NewRow();
                    rowSN.loadData(row);
                    snList.Add(rowSN.GetDataObject());
                }
            }
            return snList;
        }

        /// <summary>
        ///  get sn object list by carton ID
        /// </summary>
        /// <param name="cartonID">carton ID</param>
        /// <param name="sfcdb"></param>
        /// <returns></returns>
        public List<R_SN> GetSnListByCartonID(string cartonID, OleExec sfcdb)
        {
            string sql = $@" select b.* from r_sn_packing a,r_sn b where a.pack_id='{cartonID}' and a.sn_id = b.id  and b.shipped_flag='1'";
            List<R_SN> snList = new List<R_SN>();
            T_R_SN t_r_sn = new T_R_SN(sfcdb, this.DBType);
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    Row_R_SN rowSN = (Row_R_SN)t_r_sn.NewRow();
                    rowSN.loadData(row);
                    snList.Add(rowSN.GetDataObject());
                }
            }
            return snList;
        }

        public int GetQtyByCarton(string CartonNo, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((p, sp, s) => p.ID == sp.PACK_ID && sp.SN_ID == s.ID)
                .Where((p, sp, s) => p.PACK_NO == CartonNo).Select((p, sp, s) => s).ToList().Count;
        }

        public R_PACKING GetPackingByPackNo(string PackNo, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
        }

        public R_PACKING GetExistPacking(string Skuno, string Station, string PackType, string Line, string Ip, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING>().Where(t => t.SKUNO == Skuno && t.STATION == Station && t.PACK_TYPE == PackType
                && t.LINE == Line && t.IP == Ip && t.CLOSED_FLAG == "0").ToList().FirstOrDefault();
        }

        public List<R_PACKING> GetChildPacks(string ParentPackNo, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING, R_PACKING>((r1, r2) => r1.PARENT_PACK_ID == r2.ID).Where((r1, r2) => r2.PACK_NO == ParentPackNo)
                .Select((r1, r2) => r1).ToList();
        }

        public void GetQtyByPackNo(string PackNo, ref int Qty, OleExec DB)
        {

            R_PACKING Packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();

            if (GetChildPacks(PackNo, DB).Count > 0)
            {
                foreach (R_PACKING p in GetChildPacks(PackNo, DB))
                {
                    GetQtyByPackNo(p.PACK_NO, ref Qty, DB);
                }
            }
            else
            {
                Qty += (int)Packing.QTY;
            }
        }

        public void GetSnListByPackNo(string PackNo, ref List<R_SN> SnList, OleExec DB)
        {
            //R_PACKING Packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).ToList().FirstOrDefault();
            //if (GetChildPacks(PackNo, DB).Count > 0)
            //{
            //    foreach (R_PACKING p in GetChildPacks(PackNo, DB))
            //    {
            //        GetSnListByPackNo(p.PACK_NO, ref SnList, DB);
            //    }
            //}
            //else
            //{
            //    List<R_SN> Sns = DB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rs, rsp, rp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
            //        .Where((rs,rsp,rp)=>rp.ID==Packing.ID)
            //        .Select((rs, rsp, rp) => rs).ToList();
            //    SnList.AddRange(Sns);
            //}
            var Packing = DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PackNo).Select(t => t.ID).ToList();
            var LastPacking = Packing;
            while (Packing.Count > 0)
            {
                Packing = DB.ORM.Queryable<R_PACKING>().Where(t => Packing.Contains(t.PARENT_PACK_ID)).Select(t => t.ID).ToList();
                if (Packing.Count > 0)
                {
                    LastPacking = Packing;
                }
            }
            var SNs = DB.ORM.Queryable<R_SN_PACKING, R_SN>((rsp, rs) => rsp.SN_ID == rs.ID).Where((rsp, rs) => LastPacking.Contains(rsp.PACK_ID))
                .Select((rsp, rs) => rs).ToList();

            for (int i = 0; i < SNs.Count; i++)
            {
                SnList.Add(SNs[i]);
            }
        }

        /// <summary>
        /// 查看棧板中是否有SN
        /// </summary>
        /// <param name="packNo"></param>
        /// <param name="db"></param>
        /// <returns></returns>
        public bool CheckCTNNumber(string packNo, OleExec db)
        {
            string sql = $@"SELECT * FROM R_SN_PACKING where PACK_ID IN (
SELECT ID FROM R_PACKING WHERE PARENT_PACK_ID IN (
SELECT ID FROM R_PACKING WHERE PACK_NO='{packNo}') )";
            DataSet ds = db.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<R_PACKING> GetSkunoIsNotTrue(string PackNo, string Skuno, OleExec db)
        {
            string Sql = $@"select * from R_PACKING WHERE SKUNO != '{Skuno}' AND ( PARENT_PACK_ID IN (select ID from R_PACKING WHERE PACK_NO = '{PackNo}') OR PACK_NO = '{PackNo}')";
            List<R_PACKING> PackList = new List<R_PACKING>();
            DataTable dt = db.ExecSelect(Sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_PACKING rowPacking = (Row_R_PACKING)this.NewRow();
                    rowPacking.loadData(dt.Rows[i]);
                    PackList.Add(rowPacking.GetDataObject());
                }
            }
            else
            {
                PackList = null;
            }
            return PackList;
        }


        public List<R_PACKING> GetPackWithRSnNotExists(string PackNo, OleExec db)
        {
            string Sql = $@"select p1.* from r_packing p1 where p1.parent_pack_id in (select id from r_packing p2 where p2.pack_no='{PackNo}' and p2.pack_type='PALLET') and not exists (select 1 from r_sn s,r_packing p3,r_sn_packing sp where s.id=sp.sn_id and p3.id = sp.pack_id and p3.pack_no = p1.pack_no)";
            List<R_PACKING> PackList = new List<R_PACKING>();
            DataTable dt = db.ExecSelect(Sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_PACKING rowPacking = (Row_R_PACKING)this.NewRow();
                    rowPacking.loadData(dt.Rows[i]);
                    PackList.Add(rowPacking.GetDataObject());
                }
            }
            else
            {
                PackList = null;
            }
            return PackList;
        }

        public List<R_PACKING> GetPackByPL(string PackNo, OleExec db)
        {
            string Sql = $@"select * from R_PACKING WHERE PARENT_PACK_ID IN (select ID from R_PACKING WHERE PACK_NO = '{PackNo}' and PACK_TYPE = 'PALLET')";
            List<R_PACKING> PackList = new List<R_PACKING>();
            DataTable dt = db.ExecSelect(Sql).Tables[0];
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Row_R_PACKING rowPacking = (Row_R_PACKING)this.NewRow();
                    rowPacking.loadData(dt.Rows[i]);
                    PackList.Add(rowPacking.GetDataObject());
                }
            }
            else
            {
                PackList = null;
            }
            return PackList;
        }

        public Dictionary<string, int> GetPackIdAndSnCount(List<string> PacknoList, OleExec sfcdb)
        {
            Dictionary<string, int> Dic = new Dictionary<string, int>();
            foreach (var PackNo in PacknoList)
            {
                string sql = $@"select '{PackNo}' as PackNo,count(s.id) as CTQty from r_sn s,r_packing p,r_sn_packing sp where s.id=sp.sn_id and p.id=sp.pack_id and p.pack_no='{PackNo}' ";
                DataSet ds = sfcdb.ExecSelect(sql);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        Dic.Add(ds.Tables[0].Rows[i]["PackNo"].ToString(), Convert.ToInt32(ds.Tables[0].Rows[i]["CTQty"]));
                    }
                }
                else
                {
                    Dic = null;
                }
            }
            //List<Dictionary<string, int>> List = new List<Dictionary<string, int>>();

            return Dic;
        }

        public R_PACKING GetBYPACKNO(string PACKING, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == PACKING).ToList().FirstOrDefault();
        }
        public List<string> GetBYPACKNOVER(string PACKING, OleExec DB)
        {
            return DB.ORM.Queryable<R_WO_BASE, R_PACKING, R_SN_PACKING, R_SN>((p1, p2, p3, p4) => p2.ID == p3.PACK_ID && p3.SN_ID == p4.ID && p4.WORKORDERNO == p1.WORKORDERNO).Where((p1, p2, p3, p4) => p2.PACK_NO == PACKING).Select((p1, p2, p3, p4) => p1.SKU_VER).ToList().Distinct().ToList();
        }
        public int DeleteEmptyCarton(string PackNO, OleExec DB)
        {
            return DB.ORM.Deleteable<R_PACKING>().Where(t => t.PACK_NO == PackNO).ExecuteCommand();
        }
        public List<string> GetPackNOBySNList(string PACKNO, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((p1, p2, p3, p4) => p1.ID == p2.PARENT_PACK_ID && p2.ID == p3.PACK_ID && p3.SN_ID == p4.ID && p1.PACK_NO == PACKNO).Select((p1, p2, p3, p4) => p4.SN).ToList();
        }
        public int UpdateCartonQty(string CartonID, OleExec DB)
        {
            return DB.ORM.Updateable<R_PACKING>().UpdateColumns(t => t.QTY == t.QTY - 1).Where(t => t.ID == CartonID).ExecuteCommand();
        }
        public List<R_PACKING> checkPL(string lOTPL, OleExec DB)
        {
            return DB.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == lOTPL).ToList();
        }

        public R_PACKING GetPalletObjectBySN(string sn, OleExec sfcdb)
        {
            string sql = $@"select * from r_packing  where id in (select parent_pack_id from r_packing j,r_sn_packing n,r_sn m where j.id=n.pack_id and n.sn_id=m.id and m.sn='{sn}' and m.valid_flag='1') and pack_type='PALLET'
                          union 
                          select * from r_packing  where id in (select pack_id from r_sn_packing n,r_sn m where n.sn_id=m.id and m.sn='{sn}' and m.valid_flag='1') and pack_type='PALLET'
                            ";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                Row_R_PACKING rowPacking = (Row_R_PACKING)this.NewRow();
                rowPacking.loadData(ds.Tables[0].Rows[0]);
                return rowPacking.GetDataObject();
            }
            else
            {
                return null;
            }
        }
        public R_PACKING GetPalletObjectBySNOrBoxSN(OleExec sfcdb, string sn)
        {
            T_R_SN TRS = new T_R_SN(sfcdb, this.DBType);
            R_SN snObject = TRS.LoadData(sn, sfcdb);
            if (snObject != null)
            {
                return sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING>((rp, rpg, rsp) => rp.PARENT_PACK_ID == rpg.ID && rpg.ID == rsp.PACK_ID)
                    .Where((rp, rpg, rsp) => rsp.SN_ID == snObject.ID).Select((rp, rpg, rsp) => rp).ToList().FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public R_PACKING GetPalletObjectByCartonNo(OleExec sfcdb, string carton_no)
        {
            return sfcdb.ORM.Queryable<R_PACKING, R_PACKING>((rp, rpg) => rp.PARENT_PACK_ID == rpg.ID)
                .Where((rp, rpg) => rp.PACK_NO == carton_no).Select((rp, rpg) => rpg).ToList().FirstOrDefault();
        }

        public List<R_PACKING> GetCartonListByWO(OleExec SFCDB, string[] wo)
        {
            return SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rs, rsp, rp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID)
                .Where((rs, rsp, rp) => SqlSugar.SqlFunc.ContainsArray(wo, rs.WORKORDERNO) && rs.VALID_FLAG == "1")
                .Select((rs, rsp, rp) => rp).Distinct().ToList();
        }
        public List<R_PACKING> GetPalletListByWO(OleExec SFCDB, string[] wo)
        {
            return SFCDB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING, R_PACKING>((rs, rsp, rp, rpp) => rs.ID == rsp.SN_ID && rsp.PACK_ID == rp.ID && rp.PARENT_PACK_ID == rpp.ID)
                .Where((rs, rsp, rp, rpp) => SqlSugar.SqlFunc.ContainsArray(wo, rs.WORKORDERNO) && rs.VALID_FLAG == "1")
                .Select((rs, rsp, rp, rpp) => rpp).Distinct().ToList();
        }

        public bool CheckSNExistByID(string snID, string packID, OleExec sfcdb)
        {
            string sql = $@"SELECT * FROM r_packing WHERE ID IN (
                            SELECT PARENT_PACK_ID FROM r_packing WHERE ID IN (
                            SELECT PACK_ID FROM r_sn_packing WHERE SN_ID='{snID}')) AND ID='{packID}' ";
            DataSet ds = sfcdb.ExecSelect(sql);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public class Row_R_PACKING : DataObjectBase
    {
        public Row_R_PACKING(DataObjectInfo info) : base(info)
        {

        }
        public R_PACKING GetDataObject()
        {
            R_PACKING DataObject = new R_PACKING();
            DataObject.IP = this.IP;
            DataObject.STATION = this.STATION;
            DataObject.LINE = this.LINE;
            DataObject.EDIT_EMP = this.EDIT_EMP;
            DataObject.EDIT_TIME = this.EDIT_TIME;
            DataObject.CREATE_TIME = this.CREATE_TIME;
            DataObject.CLOSED_FLAG = this.CLOSED_FLAG;
            DataObject.QTY = this.QTY;
            DataObject.MAX_QTY = this.MAX_QTY;
            DataObject.SKUNO = this.SKUNO;
            DataObject.PARENT_PACK_ID = this.PARENT_PACK_ID;
            DataObject.PACK_TYPE = this.PACK_TYPE;
            DataObject.PACK_NO = this.PACK_NO;
            DataObject.ID = this.ID;
            return DataObject;
        }
        public string IP
        {
            get
            {
                return (string)this["IP"];
            }
            set
            {
                this["IP"] = value;
            }
        }
        public string STATION
        {
            get
            {
                return (string)this["STATION"];
            }
            set
            {
                this["STATION"] = value;
            }
        }
        public string LINE
        {
            get
            {
                return (string)this["LINE"];
            }
            set
            {
                this["LINE"] = value;
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
        public DateTime? EDIT_TIME
        {
            get
            {
                return (DateTime?)this["EDIT_TIME"];
            }
            set
            {
                this["EDIT_TIME"] = value;
            }
        }
        public DateTime? CREATE_TIME
        {
            get
            {
                return (DateTime?)this["CREATE_TIME"];
            }
            set
            {
                this["CREATE_TIME"] = value;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return (string)this["CLOSED_FLAG"];
            }
            set
            {
                this["CLOSED_FLAG"] = value;
            }
        }
        public double? QTY
        {
            get
            {
                return (double?)this["QTY"];
            }
            set
            {
                this["QTY"] = value;
            }
        }
        public double? MAX_QTY
        {
            get
            {
                return (double?)this["MAX_QTY"];
            }
            set
            {
                this["MAX_QTY"] = value;
            }
        }
        public string SKUNO
        {
            get
            {
                return (string)this["SKUNO"];
            }
            set
            {
                this["SKUNO"] = value;
            }
        }
        public string PARENT_PACK_ID
        {
            get
            {
                return (string)this["PARENT_PACK_ID"];
            }
            set
            {
                this["PARENT_PACK_ID"] = value;
            }
        }
        public string PACK_TYPE
        {
            get
            {
                return (string)this["PACK_TYPE"];
            }
            set
            {
                this["PACK_TYPE"] = value;
            }
        }
        public string PACK_NO
        {
            get
            {
                return (string)this["PACK_NO"];
            }
            set
            {
                this["PACK_NO"] = value;
            }
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
    }
    public class R_PACKING
    {
        [SqlSugar.SugarColumn(IsPrimaryKey = true)]
        public string ID { get; set; }
        public string IP { get; set; }
        public string STATION { get; set; }
        public string LINE { get; set; }
        public string EDIT_EMP { get; set; }
        public DateTime? EDIT_TIME { get; set; }
        public DateTime? CREATE_TIME { get; set; }
        public string CLOSED_FLAG { get; set; }
        public double? QTY { get; set; }
        public double? MAX_QTY { get; set; }
        public string SKUNO { get; set; }
        //public string PARENT_PACK_ID{get;set;}
        public string PARENT_PACK_ID { get; set; }
        public string PACK_TYPE { get; set; }
        public string PACK_NO { get; set; }
    }
}