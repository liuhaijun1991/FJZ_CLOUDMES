using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.LogicObject
{
    public class Packing
    {
        //包裝類型
        public string PackID { get; set; }
        public string PackNo { get; set; }        
        public string PackType { get; set; }
        public string ParentPackID { get; set; }
        public string Skuno { get; set; }
        public string SkunoVer { get; set; }
        public double? MaxQty { get; set; }
        public double? Qty { get; set; }
        public string ClosedFlag { get; set; }
        public DateTime? CreatTime { get; set; }
        public DateTime? EditTime { get; set; }
        public string EditEmp { get; set; }
        public string Line { get; set; }
        public string Station { get; set; }
        public string IP { get; set; }
        public List<string> PackList { get; set; }
        public List<R_PACKING> CartonList { get; set; }
        public List<R_SN> SNList { get; set; }

        public Packing()
        {
        }

        public void DataLoad(string packNo,string bu, OleExec sfcdb,DB_TYPE_ENUM DBType)
        {
            List<string> itemList = new List<string>();
            T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBType);
            T_R_PACKING t_r_packing = new T_R_PACKING(sfcdb, DBType);
            T_R_SN_PACKING t_r_sn_packing = new T_R_SN_PACKING(sfcdb, DBType);
            T_R_SN t_r_sn = new T_R_SN(sfcdb, DBType);
            T_C_PACKING t_c_packing = new T_C_PACKING(sfcdb, DBType);
            R_PACKING packing = new R_PACKING();
            R_SN r_sn = new R_SN();
            C_SKU sku = new C_SKU();
            Packing packObject = new Packing();
            C_PACKING c_packing = new C_PACKING();
            packing = t_r_packing.GetRPackingByPackNo(sfcdb, packNo).GetDataObject();
            sku = t_c_sku.GetSku(packing.SKUNO, sfcdb);
            if (packing.PACK_TYPE == LogicObject.PackType.PALLET.ToString().ToUpper())
            {
                c_packing = t_c_packing.GetPackingBySkuAndType(sku.SKUNO, LogicObject.PackType.CARTON.ToString().ToUpper(), sfcdb);                
                if (c_packing.MAX_QTY==1 && bu.ToUpper().Equals("VERTIV"))
                {
                    //VERTIV 當卡通包規為1時，調棧板顯示卡通內的SN
                    itemList = t_r_packing.GetPakcingSNList(packing.ID, sfcdb);
                }
                else
                {
                    T_C_CUSTOMER c_cus = new T_C_CUSTOMER(sfcdb, DBType);
                    string typesku = c_cus.GetTypeSkuno(sku.SKUNO, sfcdb);
                    if (typesku == "ARUBA" || typesku == "UFI" )
                    {

                        List<R_SN> r_snlist = t_r_sn.GetListSNByParentPackId(packing.ID, sfcdb);
                        foreach (R_SN pack in r_snlist)
                        {
                            itemList.Add(pack.SN);
                        }
                    }
                    else
                    {
                        List<R_PACKING> packingList = t_r_packing.GetListPackByParentPackId(packing.ID, sfcdb);
                        foreach (R_PACKING pack in packingList)
                        {
                            itemList.Add(pack.PACK_NO);
                        }
                    }
                }
            }
            else if (packing.PACK_TYPE == LogicObject.PackType.CARTON.ToString().ToUpper())
            {
                List<Row_R_SN_PACKING> snPackingList = t_r_sn_packing.GetPackItem(packing.ID, sfcdb);
                foreach (Row_R_SN_PACKING sn in snPackingList)
                {
                    itemList.Add(t_r_sn.GetById(sn.SN_ID, sfcdb).SN);
                }
            }
            this.PackID = packing.ID;
            this.PackNo = packing.PACK_NO;
            this.PackType = packing.PACK_TYPE;
            this.ParentPackID = packing.PARENT_PACK_ID;
            this.Skuno = packing.SKUNO;
            this.SkunoVer = sku.VERSION;
            this.MaxQty = packing.MAX_QTY;
            this.Qty = packing.QTY;
            this.ClosedFlag = packing.CLOSED_FLAG;
            this.CreatTime = packing.CREATE_TIME;
            this.EditTime = packing.EDIT_TIME;
            this.EditEmp = packing.EDIT_EMP;
            this.Line = packing.LINE;
            this.Station = packing.STATION;
            this.IP = packing.IP;
            this.PackList = itemList;
        }

        public void DataLoad(string packNo, OleExec sfcdb, DB_TYPE_ENUM DBType)
        {
            T_C_SKU t_c_sku = new T_C_SKU(sfcdb, DBType);
            T_R_PACKING t_r_packing = new T_R_PACKING(sfcdb, DBType);
            T_R_SN_PACKING t_r_sn_packing = new T_R_SN_PACKING(sfcdb, DBType);
            T_R_SN t_r_sn = new T_R_SN(sfcdb, DBType);
            R_PACKING packing = new R_PACKING();
            C_SKU sku = new C_SKU();
            R_SN sn = new R_SN();
            CartonList = new List<R_PACKING>();
            SNList = new List<R_SN>();

            packing = t_r_packing.GetRPackingByPackNo(sfcdb, packNo).GetDataObject();           
            if (packing == null || packing.PACK_NO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { packNo }));
            }
            if (packing.PACK_TYPE == LogicObject.PackType.CARTON.ToString().ToUpper())
            {
                CartonList.Add(packing);
            }
            else
            {
                CartonList = t_r_packing.GetListPackByParentPackId(packing.ID, sfcdb);                
            }
            SNList = t_r_packing.GetPakcingSNList(packing.ID, packing.PACK_TYPE, sfcdb);
            sku = t_c_sku.GetSku(packing.SKUNO, sfcdb);
            this.PackID = packing.ID;
            this.PackNo = packing.PACK_NO;
            this.PackType = packing.PACK_TYPE;
            this.ParentPackID = packing.PARENT_PACK_ID;
            this.Skuno = packing.SKUNO;
            this.SkunoVer = sku.VERSION;
            this.MaxQty = packing.MAX_QTY;
            this.Qty = packing.QTY;
            this.ClosedFlag = packing.CLOSED_FLAG;
            this.CreatTime = packing.CREATE_TIME;
            this.EditTime = packing.EDIT_TIME;
            this.EditEmp = packing.EDIT_EMP;
            this.Line = packing.LINE;
            this.Station = packing.STATION;
            this.IP = packing.IP;
        }

        public bool IsHadMoreRoute(OleExec sfcdb)
        {
            List<string> routeList = new List<string>();
            if (this.PackType == LogicObject.PackType.PALLET.ToString())
            {
                routeList = sfcdb.ORM.Queryable<R_PACKING, R_PACKING, R_SN_PACKING, R_SN>((RP, RPG, RSP, RS) => RP.ID == RPG.PARENT_PACK_ID && RPG.ID == RSP.PACK_ID && RSP.SN_ID == RS.ID)
                    .Where((RP, RPG, RSP, RS) => RP.ID == PackID && RS.VALID_FLAG == "1").Select((RP, RPG, RSP, RS) => RS.ROUTE_ID).ToList();
            }
            else
            {
                routeList = sfcdb.ORM.Queryable<R_PACKING, R_SN_PACKING, R_SN>((RP, RSP, RS) => RP.ID == RSP.PACK_ID && RSP.SN_ID == RS.ID)
                    .Where((RP, RSP, RS) => RP.ID == PackID && RS.VALID_FLAG == "1").Select((RP, RSP, RS) => RS.ROUTE_ID).ToList();
            }
            if (routeList.Count > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsInThePack(string inputValue)
        {
            bool bExist = false;
            if (CartonList.Find(r => r.PACK_NO == inputValue) != null)
            {
                bExist = true;
            }
            else if (SNList.Find(r => r.SN == inputValue || r.BOXSN == inputValue) != null)
            {
                bExist = true;
            }
            return bExist;
        }

        public static R_PACKING GetNewNullPallet(OleExec sfcdb,DB_TYPE_ENUM dbtype, string skuno,string line,string stationName,string ip,string BU,string user)
        {
            T_C_PACKING TCP = new T_C_PACKING(sfcdb, dbtype);
            C_PACKING palletConfig = TCP.GetPackingBySkuAndType(skuno, "PALLET", sfcdb);
            if (palletConfig == null)
            {
                throw new Exception("Can't find PalletConfig");
            }
            return MESStation.Packing.PackingBase.GetNewPacking(palletConfig, line, stationName, ip, BU, user, sfcdb).GetDataObject();
        }

        public static R_PACKING GetNewNullCarton(OleExec sfcdb, DB_TYPE_ENUM dbtype, string skuno, string line, string stationName, string ip, string BU, string user)
        {
            T_C_PACKING TCP = new T_C_PACKING(sfcdb, dbtype);
            C_PACKING palletConfig = TCP.GetPackingBySkuAndType(skuno, "CARTON", sfcdb);
            if (palletConfig == null)
            {
                throw new Exception("Can't find CartonConfig");
            }
            return MESStation.Packing.PackingBase.GetNewPacking(palletConfig, line, stationName, ip, BU, user, sfcdb).GetDataObject();
        }

        public static void MoveCartonToNewPallet(OleExec sfcdb, DB_TYPE_ENUM dbtype,R_PACKING carton,R_PACKING oldPallet,R_PACKING newPallet,string BU,string user)
        {
            string result = "";
            T_R_PACKING TRP = new T_R_PACKING(sfcdb, dbtype);
            //修改CARTON棧板ID
            TRP.UpdateParentPackIDByPackNo(carton.PACK_NO, newPallet.ID, user, sfcdb);
            if (Convert.ToInt32(result) == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
            }
            //新棧板數量加1
            result = TRP.UpdateQtyByID(newPallet.ID, true, 1, user, sfcdb);
            if (Convert.ToInt32(result) == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
            }
            //舊棧板數量減1
            result = TRP.UpdateQtyByID(oldPallet.ID, false, 1, user, sfcdb);
            if (Convert.ToInt32(result) == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
            }
            //寫入記錄
            T_R_MOVE_LIST TRML = new T_R_MOVE_LIST(sfcdb, dbtype);
            int i = TRML.SaveMoveList(sfcdb, BU, carton.ID, newPallet.PACK_TYPE, oldPallet.ID, newPallet.ID, user);
            if (i == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_MOVE_LIST" }));
            }
        }

        public static void MoveSNToNewCarton(OleExec sfcdb, DB_TYPE_ENUM dbtype, R_SN sn, R_PACKING oldCarton, R_PACKING newCarton, string BU, string user)
        {
            string result = "";
            var snp = sfcdb.ORM.Queryable<R_SN_PACKING>().Where(t => t.SN_ID == sn.ID).First();
            snp.PACK_ID = newCarton.ID;
            sfcdb.ORM.Updateable(snp).ExecuteCommand();
            T_R_PACKING TRP = new T_R_PACKING(sfcdb, dbtype);
            //新Carton數量加1
            result = TRP.UpdateQtyByID(newCarton.ID, true, 1, user, sfcdb);
            if (Convert.ToInt32(result) == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
            }
            //舊Carton數量減1
            result = TRP.UpdateQtyByID(oldCarton.ID, false, 1, user, sfcdb);
            if (Convert.ToInt32(result) == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000025", new string[] { "R_PACKING" }));
            }
            //寫入記錄
            T_R_MOVE_LIST TRML = new T_R_MOVE_LIST(sfcdb, dbtype);
            int i = TRML.SaveMoveList(sfcdb, BU, sn.ID, newCarton.PACK_TYPE, oldCarton.ID, newCarton.ID, user);
            if (i == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000021", new string[] { "R_MOVE_LIST" }));
            }
        }
    }

    public enum PackType
    {
        PALLET = 0,
        CARTON = 1
    }
}
