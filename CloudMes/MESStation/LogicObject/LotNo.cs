using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation.LogicObject;
using MESStation.Packing;

namespace MESStation.LogicObject
{
    public class LotNo
    {
        public string ID
        {
            get
            {
                return SLot.ID;
            }
        }
        public string LOT_NO
        {
            get
            {
                if (SLot == null)
                {
                    return "";
                }

                return SLot.LOT_NO;
            }
        }

        public string SKUNO
        {
            get
            {
                return SLot.SKUNO;
            }
        }

        public string AQL_TYPE
        {
            get
            {
                return SLot.AQL_TYPE;
            }
        }

        public double? LOT_QTY
        {
            get
            {
                return SLot.LOT_QTY;
            }
        }

        public double? REJECT_QTY
        {
            get
            {
                return SLot.REJECT_QTY;
            }
        }

        public double? SAMPLE_QTY
        {
            get
            {
                return SLot.SAMPLE_QTY;
            }
        }
        public double? PASS_QTY
        {
            get
            {
                return SLot.PASS_QTY;
            }
        }
        public double? FAIL_QTY
        {
            get
            {
                return SLot.FAIL_QTY;
            }
        }
        public string CLOSED_FLAG
        {
            get
            {
                return SLot.CLOSED_FLAG;
            }
        }

        public string LOT_STATUS_FLAG
        {
            get
            {
                return SLot.LOT_STATUS_FLAG;
            }
        }
        public string SAMPLE_STATION
        {
            get
            {
                return SLot.SAMPLE_STATION;
            }
        }
        public string LINE
        {
            get
            {
                return SLot.LINE;
            }
        }
        public string LotDetailID
        {
            get
            {
                return Slotdetail.ID;
            }
        }
        public string LotDetailLotID
        {
            get
            {
                return Slotdetail.LOT_ID;
            }
        }
        public string WORKORDERNO
        {
            get
            {
                return Slotdetail.WORKORDERNO;
            }
        }
        public string SAMPLING
        {
            get
            {
                return Slotdetail.SAMPLING;
            }
        }
        public string STATUS
        {
            get
            {
                return Slotdetail.STATUS;
            }
        }
        public string FAIL_CODE
        {
            get
            {
                return Slotdetail.FAIL_CODE;
            }
        }
        public string FAIL_LOCATION
        {
            get
            {
                return Slotdetail.FAIL_LOCATION;
            }
        }
        public string DESCRIPTION
        {
            get
            {
                return Slotdetail.DESCRIPTION;
            }
        }
        public string CARTON_NO
        {
            get
            {
                return Slotdetail.CARTON_NO;
            }
        }
        public string PALLET_NO
        {
            get
            {
                return Slotdetail.PALLET_NO;
            }
        }    

        public LotNo()
        {
        }

        public LotNo(MESDataObject.DB_TYPE_ENUM _dbType)
        {
            DBType = _dbType;
        }

        Row_R_LOT_STATUS RLotNo;
        R_LOT_STATUS SLot;
        Row_R_LOT_DETAIL Rlotdetail;
        R_LOT_DETAIL Slotdetail;
        MESDataObject.DB_TYPE_ENUM DBType;

        /*Modify by LLF 2018-02-22
        public void Init(string StrLotNo, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {          
            string ColoumName = "lot_no";
            DBType = _DBType;
            T_R_LOT_STATUS TRWB = new T_R_LOT_STATUS(SFCDB, DBType);
            T_R_LOT_DETAIL TWC = new T_R_LOT_DETAIL(SFCDB, DBType);
            RLotNo = TRWB.GetByInput(StrLotNo, ColoumName, SFCDB);
            SLot = RLotNo.GetDataObject();
            Rlotdetail = TWC.GetByLotID(SLot.ID, SFCDB);
            Slotdetail = Rlotdetail.GetDataObject();
        }

        public void Init(string StrLotNo, MESDBHelper.OleExec SFCDB)
        {
            Init(StrLotNo, SFCDB, DBType);
        }*/

        public void Init(string StrLotNo, string StrSN, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            string ColoumName = "lot_no";
            DBType = _DBType;
            T_R_LOT_STATUS TRWB = new T_R_LOT_STATUS(SFCDB, DBType);
            T_R_LOT_DETAIL TWC = new T_R_LOT_DETAIL(SFCDB, DBType);
            RLotNo = TRWB.GetByInput(StrLotNo, ColoumName, SFCDB);
            SLot = RLotNo.GetDataObject();
            Rlotdetail = TWC.GetByLotID(SLot.ID, StrSN, SFCDB);
            Slotdetail = Rlotdetail.GetDataObject();           
        }

        public void Init(string StrLotNo, string StrSN, MESDBHelper.OleExec SFCDB)
        {
            Init(StrLotNo, StrSN, SFCDB, DBType);
        }
        public string GetNewLotNo(string SeqName, MESDBHelper.OleExec SFCDB)
        {
            string StrLotNo = "";
            T_C_SEQNO T_C_Seqno = new T_C_SEQNO(SFCDB, DBType);
            StrLotNo=T_C_Seqno.GetLotno(SeqName, SFCDB);
            return StrLotNo;    
        }
        
        /// <summary>
        /// OBA工站創建LOT,返回LOT信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="packNo"></param>
        /// <param name="DB"></param>
        public R_LOT_STATUS CreateLotByPackno(User user, string packNo, OleExec DB)
        {
            T_R_PACKING tRPacking = new T_R_PACKING(DB, this.DBType);
            Row_R_PACKING rowRPacking = tRPacking.GetRPackingByPackNo(DB, packNo);
            PalletBase palletBase = new PalletBase(rowRPacking);

            T_C_SKU tCSku = new T_C_SKU(DB, this.DBType);
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(DB, this.DBType);
            T_C_AQLTYPE tCAqlType = new T_C_AQLTYPE(DB, this.DBType);
            T_R_LOT_PACK tRLotPack = new T_R_LOT_PACK(DB, this.DBType);
            T_C_SKU_AQL tCSkuAql = new T_C_SKU_AQL(DB, this.DBType);
            C_SKU_AQL cSkuAql = tCSkuAql.GetSkuAql(DB, rowRPacking.SKUNO);

            List<C_AQLTYPE> cAqlTypeList = tCAqlType.GetAqlTypeBySkunoAndLevel(rowRPacking.SKUNO, cSkuAql.DEFAULLEVEL, DB);
            if (cAqlTypeList.Count == 0)
            {
                throw new Exception(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20180625165842",
                    new string[] { }));
            }
            //C_SKU Sku = tCSku.GetSku(rowRPacking.SKUNO, DB);
            Row_R_LOT_STATUS rowRLotStatus = (Row_R_LOT_STATUS)tRLotStatus.NewRow();
            rowRLotStatus.ID = tRLotStatus.GetNewID(user.BU, DB, this.DBType);
            rowRLotStatus.LOT_NO = MESPubLab.MESStation.SNMaker.SNmaker.GetNextSN("OBALOT", DB);
            rowRLotStatus.SKUNO = rowRPacking.SKUNO;
            rowRLotStatus.AQL_TYPE = cSkuAql.AQLTYPE;
            rowRLotStatus.LOT_QTY = palletBase.GetSnCount(DB);
            //更新批次数量超出配置AQL最大数量则取数量最大一笔配置
            //var cAqlTypeTarget = cAqlTypeList.Where(t => t.LOT_QTY > rowRLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).First();
            var listCAql = cAqlTypeList.Where(t => t.LOT_QTY > rowRLotStatus.LOT_QTY);
            C_AQLTYPE cAqlTypeTarget = null;
            if (listCAql.Count() >0)
            {
                cAqlTypeTarget = listCAql.OrderBy(t => t.LOT_QTY).First();
            }
            else
            {
                cAqlTypeTarget = cAqlTypeList.OrderBy(t => t.LOT_QTY).First();
            }

            rowRLotStatus.REJECT_QTY = cAqlTypeTarget.REJECT_QTY;
            rowRLotStatus.SAMPLE_QTY = cAqlTypeTarget.SAMPLE_QTY;
            rowRLotStatus.SAMPLE_QTY = rowRLotStatus.SAMPLE_QTY > rowRLotStatus.LOT_QTY ? rowRLotStatus.LOT_QTY : rowRLotStatus.SAMPLE_QTY;
            rowRLotStatus.PASS_QTY = 0;
            rowRLotStatus.FAIL_QTY = 0;
            rowRLotStatus.CLOSED_FLAG = "0";
            rowRLotStatus.LOT_STATUS_FLAG = "0";
            rowRLotStatus.SAMPLE_STATION = "OBA";
            rowRLotStatus.AQL_LEVEL = cSkuAql.DEFAULLEVEL;
            rowRLotStatus.LINE = "";
            rowRLotStatus.EDIT_EMP = user.EMP_NO;
            rowRLotStatus.EDIT_TIME = tRPacking.GetDBDateTime(DB);

            Row_R_LOT_PACK rowRLotPack = (Row_R_LOT_PACK)tRLotPack.NewRow();
            rowRLotPack.ID = tRLotPack.GetNewID(user.BU, DB, this.DBType);
            rowRLotPack.LOTNO = rowRLotStatus.LOT_NO;
            rowRLotPack.PACKNO = packNo;
            rowRLotPack.EDIT_EMP = user.EMP_NO;
            rowRLotPack.EDIT_TIME = rowRLotStatus.EDIT_TIME;
            DB.ThrowSqlExeception = true;
            DB.ExecSQL(rowRLotStatus.GetInsertString(this.DBType));
            DB.ExecSQL(rowRLotPack.GetInsertString(this.DBType));
            DB.ThrowSqlExeception = false;
            return rowRLotStatus.GetDataObject();
        }

        /// <summary>
        /// OBA工站InLot,返回LOT信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="packNo"></param>
        /// <param name="DB"></param>
        public R_LOT_STATUS ObaInLotByPackno(User user, R_LOT_STATUS rLotStatus , string packNo,string AqlLevel, OleExec DB)
        {
            T_R_PACKING tRPacking = new T_R_PACKING(DB, this.DBType);
            Row_R_PACKING rowRPacking = tRPacking.GetRPackingByPackNo(DB, packNo);
            PalletBase palletBase = new PalletBase(rowRPacking);
            
            T_R_LOT_STATUS tRLotStatus = new T_R_LOT_STATUS(DB, this.DBType);
            T_C_AQLTYPE tCAqlType = new T_C_AQLTYPE(DB, this.DBType);
            T_R_LOT_PACK tRLotPack = new T_R_LOT_PACK(DB, this.DBType);

            List<C_AQLTYPE> cAqlTypeList = tCAqlType.GetAqlTypeBySkunoAndLevel(rowRPacking.SKUNO, AqlLevel, DB);
            //Row_R_LOT_STATUS rowRLotStatus =  (Row_R_LOT_STATUS)tRLotStatus.NewRow();
            //rowRLotStatus.ID = rLotStatus.ID;
            //rowRLotStatus.LOT_NO = rLotStatus.LOT_NO;
            //rowRLotStatus.SKUNO = rLotStatus.SKUNO;
            //rowRLotStatus.AQL_TYPE = rLotStatus.AQL_TYPE;
            rLotStatus.LOT_QTY = rLotStatus.LOT_QTY+ palletBase.GetSnCount(DB);

            //VNQE阮越用要求掃入LOT最大數量為3200,這個數量讓QE自己配吧 Edit By ZHB 20200923
            var maxSize = DB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "OBALOT_MAXSIZE" && t.CONTROLFLAG == "Y" && t.FUNCTIONTYPE == "NOSYSTEM" && t.VALUE == rowRPacking.SKUNO).Select(t => t.EXTVAL).First();
            if (maxSize != null)
            {
                if (rLotStatus.LOT_QTY > Convert.ToDouble(maxSize))
                {
                    //throw new Exception($@"掃入LOT數量 {rLotStatus.LOT_QTY.ToString()} 大於QE設置的最大數量 {maxSize} ,請找QE確認!");
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143306", new string[] { rLotStatus.LOT_QTY.ToString(), maxSize }));
                }
            }

            if(!cAqlTypeList.Where(t => t.LOT_QTY > rLotStatus.LOT_QTY).Any())
                throw new Exception(MESDataObject.MESReturnMessage.GetMESReturnMessage("MSGCODE20180911154419",
                    new string[] { rLotStatus.LOT_QTY.ToString() }));

            rLotStatus.REJECT_QTY = cAqlTypeList.Where(t => t.LOT_QTY > rLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).Take(1).ToList<C_AQLTYPE>()[0].REJECT_QTY;
            rLotStatus.SAMPLE_QTY = cAqlTypeList.Where(t => t.LOT_QTY > rLotStatus.LOT_QTY).OrderBy(t => t.LOT_QTY).Take(1).ToList<C_AQLTYPE>()[0].SAMPLE_QTY;
            rLotStatus.SAMPLE_QTY = rLotStatus.SAMPLE_QTY > rLotStatus.LOT_QTY ? rLotStatus.LOT_QTY : rLotStatus.SAMPLE_QTY;
            //rowRLotStatus.PASS_QTY = rLotStatus.PASS_QTY;
            //rowRLotStatus.FAIL_QTY = rLotStatus.FAIL_QTY;
            //rowRLotStatus.CLOSED_FLAG = rLotStatus.CLOSED_FLAG;
            //rowRLotStatus.LOT_STATUS_FLAG = rLotStatus.LOT_STATUS_FLAG;
            //rowRLotStatus.SAMPLE_STATION = rLotStatus.SAMPLE_STATION;
            //rowRLotStatus.LINE = rLotStatus.LINE;
            //rowRLotStatus.AQL_LEVEL = AqlLevel;
            //rowRLotStatus.EDIT_EMP = user.EMP_NO;
            //rowRLotStatus.EDIT_TIME = tRPacking.GetDBDateTime(DB);

            Row_R_LOT_PACK rowRLotPack = (Row_R_LOT_PACK)tRLotPack.NewRow();
            rowRLotPack.ID = tRLotPack.GetNewID(user.BU, DB, this.DBType);
            rowRLotPack.LOTNO = rLotStatus.LOT_NO;
            rowRLotPack.PACKNO = packNo;
            rowRLotPack.EDIT_EMP = user.EMP_NO;
            rowRLotPack.EDIT_TIME = rLotStatus.EDIT_TIME;
            DB.ThrowSqlExeception = true;
            //DB.ExecSQL(rowRLotStatus.GetUpdateString(this.DBType) + " ; " + rowRLotPack.GetInsertString(this.DBType));
            DB.ORM.Updateable<R_LOT_STATUS>(rLotStatus).Where(t => t.LOT_NO == rLotStatus.LOT_NO).ExecuteCommand();
            //DB.ExecSQL(rowRLotStatus.GetUpdateString(this.DBType) );
            DB.ExecSQL(rowRLotPack.GetInsertString(this.DBType));
            DB.ThrowSqlExeception = false;
            return rLotStatus;
        }
        

        public override string ToString()
        {
            return LOT_NO;
        }
        /// <summary>
        ///  HWT OBA 獲取抽檢類型
        /// </summary>
        /// <param name="sfcdb"></param>
        /// <param name="skuno"></param>
        /// <returns></returns>
        public string GetOBATypeBySkuno(OleExec sfcdb, string skuno,string version)
        {
            string sampingType = "";       
            T_R_LOT_STATUS TRLS = new T_R_LOT_STATUS(sfcdb, this.DBType);
            List<R_LOT_STATUS> listLot = sfcdb.ORM.Queryable<R_LOT_STATUS>().Where(r => r.SKUNO == skuno && r.SAMPLE_STATION == "OBA").ToList();

            MESDataObject.Module.HWT.T_R_OBASAMPLING_BYTIME TROB = new MESDataObject.Module.HWT.T_R_OBASAMPLING_BYTIME(sfcdb, this.DBType);
            List<MESDataObject.Module.HWT.R_OBASAMPLING_BYTIME> objByTimeList = TROB.GetSamplingList(sfcdb, skuno, version, "OBA", "Y");
            if (objByTimeList.Count > 1)
            {
                //throw new Exception("SKUNO:" + skuno + " Version:" + version + " ,配置的OBA加嚴類型錯誤！");
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814143534", new string[] { skuno, version }));
            }
            if (listLot.Count > 5) 
            {
                //如果機種OBA抽檢的批次大於5
                //第一次邏輯若最後的連續5批次都是PASS,則抽驗類型為NORMAL
                int lastFiveLotStatus = listLot.OrderByDescending(r => r.EDIT_TIME).Take(5).Sum(r => Convert.ToInt32(r.LOT_STATUS_FLAG));
                if (lastFiveLotStatus == 5)
                {
                    //第二此修改邏輯,判斷此機種IPQC是否有額外配置此機種的加嚴信息,并處於生效狀況
                    //第三次修改邏輯,增加了兩種加嚴類型 'TIHGT1','TIGHT2',根據IPQC的配置決定機種抽嚴是哪種加嚴類型                   
                    if (objByTimeList.Count != 0)
                    {
                        sampingType = objByTimeList[0].SAMPLING_TYPE;
                    }
                    else
                    {
                        sampingType = "NORMAL";
                    }
                }
                else
                {
                    //第三次修改邏輯,增第一次邏輯若最後的連續5批次沒有都PASS，則判斷最近3批次的狀況
                    int lastThreeLotStatus = listLot.OrderByDescending(r => r.EDIT_TIME).Take(3).Sum(r => Convert.ToInt32(r.LOT_STATUS_FLAG));
                    if (lastThreeLotStatus == 0)
                    {
                        //第一次修改邏輯，若最後連續3批次都是FAIL,則是抽菸類型為：TIGHT
                        //第二此修改邏輯,判斷此機種IPQC是否有額外配置此機種的加嚴信息,并處於生效狀況
                        //第三次修改邏輯,增加了兩種加嚴類型 'TIHGT1','TIGHT2',根據IPQC的配置決定機種抽嚴是哪種加嚴類型
                        if (objByTimeList.Count != 0)
                        {
                            sampingType = objByTimeList[0].SAMPLING_TYPE;
                        }
                        else
                        {
                            sampingType = "TIGHT";
                        }
                    }
                    else
                    {
                        //第一次修改邏輯，若最後連續3批次不全是FAIL,則抽嚴類型：NOMAL
                        if (objByTimeList.Count != 0)
                        {
                            sampingType = objByTimeList[0].SAMPLING_TYPE;
                        }
                        else
                        {
                            sampingType = "NORMAL";
                        }
                    }
                }
            }
            else
            {
                //第一次邏輯此機種的檢驗 < 5批次的狀況,則抽嚴類型為：TIGHT
                if (objByTimeList.Count != 0)
                {
                    sampingType = objByTimeList[0].SAMPLING_TYPE;
                }
                else
                {
                    sampingType = "TIGHT";
                }
            }   
            return sampingType;
        }

        public R_LOT_STATUS InLotByAQLTypeAndStation(MESPubLab.MESStation.MESStationBase Station, SN snObject, string aql_type,string test_station,string lot_rule)
        {
            T_C_SKU_AQL t_c_sku_aql = new T_C_SKU_AQL(Station.SFCDB, Station.DBType);
            T_C_ROUTE_DETAIL t_c_route_detail = new T_C_ROUTE_DETAIL(Station.SFCDB, Station.DBType);
            T_R_LOT_STATUS t_r_lot_status = new T_R_LOT_STATUS(Station.SFCDB, Station.DBType);
            T_R_LOT_DETAIL t_r_lot_detail = new T_R_LOT_DETAIL(Station.SFCDB, Station.DBType);
            T_C_SEQNO t_c_seqno = new T_C_SEQNO(Station.SFCDB, Station.DBType);
            T_C_AQLTYPE t_c_aqltype = new T_C_AQLTYPE(Station.SFCDB, Station.DBType);
            

            List<C_SKU_AQL> listSkuAQL = t_c_sku_aql.GetAQLListBySkuAndType(Station.SFCDB, snObject.SkuNo, aql_type);
            List<C_ROUTE_DETAIL> listRoute = new List<C_ROUTE_DETAIL>();
            List<C_ROUTE_DETAIL> listLastRoute = new List<C_ROUTE_DETAIL>();
            C_ROUTE_DETAIL routeICT = null;
            C_ROUTE_DETAIL routePriorICT = null;
            R_LOT_STATUS lot5DX = null;
           
            int result = 0;
            string lot_id = "";
            int lot_qty, Sample_Qty;

            //配置了5DX 的AQL
            //配置了對應的AQL
            if (listSkuAQL.Count > 0)
            {
                listRoute = t_c_route_detail.GetByRouteIdOrderBySEQASC(snObject.RouteID, Station.SFCDB);
                if (listRoute == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180808113358", new string[] { snObject.RouteID }));
                }
                //路由中有5DX工站
                //路由中有對應的測試工站
                routeICT = listRoute.Find(r => r.STATION_NAME == test_station);
                if (routeICT != null)
                {
                    listLastRoute = t_c_route_detail.GetLastStations(routeICT.ROUTE_ID, routeICT.STATION_NAME, Station.SFCDB);
                    routePriorICT = listLastRoute.OrderByDescending(r => r.SEQ_NO).ToList().FirstOrDefault();
                    //5DX的前一站是SAMPLETESTLOT類型
                    //測試工站的前一站是SAMPLETESTLOT類型
                    if (t_c_route_detail.RouteEXExist(routePriorICT.ID, "SAMPLETESTLOT", "SAMPLETESTLOT", Station.SFCDB))
                    {
                        lot5DX = t_r_lot_status.GetNotClosingLot(snObject.SkuNo, listSkuAQL.FirstOrDefault().AQLTYPE, test_station,false, Station.SFCDB);
                        if (lot5DX == null)
                        {
                            //New A Lot                           
                            R_LOT_STATUS newLot = new R_LOT_STATUS();
                            newLot.ID = t_r_lot_status.GetNewID(Station.BU, Station.SFCDB);
                            lot_id = newLot.ID;
                            newLot.LOT_NO = t_c_seqno.GetLotno(lot_rule, Station.SFCDB);
                            newLot.SKUNO = snObject.SkuNo;
                            newLot.AQL_TYPE = listSkuAQL.FirstOrDefault().AQLTYPE;
                            newLot.LOT_QTY = 1;
                            newLot.REJECT_QTY = 0;
                            newLot.SAMPLE_QTY = 1;
                            newLot.PASS_QTY = 0;
                            newLot.FAIL_QTY = 0;
                            newLot.CLOSED_FLAG = "0";
                            newLot.LOT_STATUS_FLAG = "0";
                            newLot.SAMPLE_STATION = test_station;
                            newLot.LINE = Station.Line;
                            newLot.EDIT_EMP = Station.LoginUser.EMP_NO;
                            newLot.EDIT_TIME = t_r_lot_status.GetDBDateTime(Station.SFCDB);
                            newLot.AQL_LEVEL = "";
                            result = t_r_lot_status.InsertNewLot(newLot, Station.SFCDB);
                            if (result <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + snObject.SerialNo, "ADD" }));
                            }
                        }
                        else
                        {
                            //Update Lot
                            lot_id = lot5DX.ID;
                            lot_qty = (int)lot5DX.LOT_QTY + 1;                            
                            Sample_Qty = t_c_aqltype.GetSampleQty(aql_type, lot_qty, Station.SFCDB);
                            lot5DX.LOT_QTY = lot5DX.LOT_QTY + 1;
                            lot5DX.SAMPLE_QTY = Sample_Qty;
                            lot5DX.EDIT_EMP = Station.LoginUser.EMP_NO;
                            lot5DX.EDIT_TIME = t_r_lot_status.GetDBDateTime(Station.SFCDB);
                            result = t_r_lot_status.UpdateLot(lot5DX, Station.SFCDB);
                            if (result <= 0)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_STATUS:" + snObject.SerialNo, "UPDATE" }));
                            }
                        }
                        R_LOT_DETAIL lotDetail = new R_LOT_DETAIL();
                        lotDetail.ID = t_r_lot_detail.GetNewID(Station.BU, Station.SFCDB);
                        lotDetail.LOT_ID = lot_id;
                        lotDetail.SN = snObject.SerialNo;
                        lotDetail.WORKORDERNO = snObject.WorkorderNo;
                        lotDetail.CREATE_DATE = t_r_lot_detail.GetDBDateTime(Station.SFCDB);
                        lotDetail.SAMPLING = "0";
                        lotDetail.STATUS = "0";
                        lotDetail.FAIL_CODE = "";
                        lotDetail.FAIL_LOCATION = "";
                        lotDetail.DESCRIPTION = "";
                        lotDetail.CARTON_NO = "";
                        lotDetail.PALLET_NO = "";
                        lotDetail.EDIT_EMP = Station.LoginUser.EMP_NO;
                        lotDetail.EDIT_TIME = t_r_lot_detail.GetDBDateTime(Station.SFCDB);
                        result = t_r_lot_detail.InsertNew(lotDetail, Station.SFCDB);
                        if (result <= 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000083", new string[] { "R_LOT_DETAIL:" + snObject.SerialNo, "ADD" }));
                        }
                    }
                }
            }

            return lot5DX;
        }
    }
    
}
