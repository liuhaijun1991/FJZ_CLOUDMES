using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;

namespace MESStation.Label
{
    public class SnPalletList : LabelBase
    {
        
        LabelInputValue I_PALLETNO = new LabelInputValue() { Name = "PLNO", Type = "STRING", Value = "", StationSessionType = "PRINT_PL", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CVER = new LabelOutput() { Name = "CVER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PLNO = new LabelOutput() { Name = "PLNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        List<R_SN_KP> listAllKP = new List<R_SN_KP>();
        public SnPalletList()
        {
            Inputs.Add(I_PALLETNO);

            Outputs.Add(O_SN);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_VER);
            Outputs.Add(O_QTY);
            Outputs.Add(O_PLNO);
        }
        public override void MakeLabel(OleExec DB)
        {
            Packing.PalletBase pallet = new Packing.PalletBase(I_PALLETNO.Value.ToString(), DB);
            List<string> snList = pallet.GetSNList(DB);

            if (snList.Count == 0)
            {
                throw new Exception("Pallet is empty");
            }

            Row_R_PACKING rowPallet = pallet.DATA;

            //C_SKU_DETAIL skuDetail = DB.ORM.Queryable<C_SKU_DETAIL>().Where(d => d.SKUNO == rowPallet.SKUNO && d.CATEGORY == "PRINT_PALLET"
            //                            && d.CATEGORY_NAME == "KEYPART").ToList().FirstOrDefault();

            R_F_CONTROL controlObj = DB.ORM.Queryable<R_F_CONTROL>().Where(r => r.FUNCTIONNAME == "PRINT_PALLET" && r.FUNCTIONTYPE == "NOSYSTEM" && r.CATEGORY == "SKUNO" && r.CONTROLFLAG == "Y"
              && r.VALUE == rowPallet.SKUNO).ToList().FirstOrDefault();

            //如有有配置要打印KEYPART的值，則取KEYPART的值
            if (controlObj == null)
            {
                O_SN.Value = snList;
            }
            else
            {

                var packSnList = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING>((r_sn, rp, rsp) => rp.ID == rsp.PACK_ID && rsp.SN_ID == r_sn.ID)
                      .Where((r_sn, rp, rsp) => rp.PARENT_PACK_ID == rowPallet.ID).Select((r_sn, rp, rsp) => r_sn.SN).ToList();
                foreach (string l in packSnList)
                {
                    GetSnKP(DB, l);
                }
                string station = controlObj.EXTVAL;
                List<R_F_CONTROL_EX> listEX = DB.ORM.Queryable<R_F_CONTROL_EX>().Where(r => r.DETAIL_ID == controlObj.ID).OrderBy(r => r.SEQ_NO, SqlSugar.OrderByType.Asc).ToList();
                string kp_name = listEX.Find(r => r.NAME == "KP_NAME") == null ? "" : listEX.Find(r => r.NAME == "KP_NAME").VALUE;
                string scan_type = listEX.Find(r => r.NAME == "SCAN_TYPE") == null ? "" : listEX.Find(r => r.NAME == "SCAN_TYPE").VALUE;
                //string partno = listEX.Find(r => r.NAME == "PARTNO") == null ? "" : listEX.Find(r => r.NAME == "PARTNO").VALUE;
                O_SN.Value = listAllKP.Where(r => r.STATION == station && r.SCANTYPE == scan_type && r.KP_NAME == kp_name).Select(r => r.VALUE).ToList();

                #region fgg 2020.07.27屏蔽掉 改用r_function_control表的配置 
                ////沒有配置要打印的KEYPART料號則報錯
                //if (string.IsNullOrEmpty(skuDetail.VALUE))
                //{
                //    throw new Exception("the keypar partno is empty");
                //}
                ////如果配置的KEYPART料號的EXTEND值為空則取KEYPART的信息，不為空則取KEYPART中SCANTYPE為EXTEND值的信息
                //if (string.IsNullOrEmpty(skuDetail.EXTEND))
                //{
                //    O_SN.Value = pallet.GetKeyPartList(DB);
                //}
                //else
                //{
                //    //查詢慢，去掉
                //    //O_SN.Value = pallet.GetScanTypeList(skuDetail.EXTEND, DB);
                //    //return DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING, R_SN_KP, R_SN_KP>((s, p, sp, kp, kkp) => s.ID == sp.SN_ID && sp.PACK_ID == p.ID && s.ID == kp.R_SN_ID && kp.VALUE == kkp.SN)
                //    //.Where((s, p, sp, kp, kkp) => p.PARENT_PACK_ID == DATA.ID && kkp.SCANTYPE == scanType).Select((s, p, sp, kp, kkp) => kkp.VALUE).ToList();

                //    var packSnList = DB.ORM.Queryable<R_SN, R_PACKING, R_SN_PACKING>((r_sn, rp, rsp) => rp.ID == rsp.PACK_ID && rsp.SN_ID == r_sn.ID)
                //        .Where((r_sn, rp, rsp) => rp.PARENT_PACK_ID == rowPallet.ID).Select((r_sn, rp, rsp) => r_sn.ID).ToList();
                //    O_SN.Value = DB.ORM.Queryable<R_SN, R_SN_KP, R_SN_KP>((r_sn, rsk, rskp) => r_sn.ID == rsk.R_SN_ID && rsk.VALUE == rskp.SN)
                //        .Where((r_sn, rsk, rskp) => packSnList.Contains(r_sn.ID) && rskp.SCANTYPE == skuDetail.EXTEND && rskp.VALID_FLAG == 1).Select((r_sn, rsk, rskp) => rskp.VALUE).ToList();
                //}
                #endregion 
            }

            LogicObject.SN sn = new LogicObject.SN(snList[0], DB, DB_TYPE_ENUM.Oracle);
            LogicObject.WorkOrder wo = new LogicObject.WorkOrder();
            wo.Init(sn.WorkorderNo, DB, DB_TYPE_ENUM.Oracle);
            if (wo.SkuNO.StartsWith("VT"))
            {
                C_SKU_VER_MAPPING verMapping = new C_SKU_VER_MAPPING();
                T_C_SKU_VER_MAPPING t_c_sku_ver_mapping = new T_C_SKU_VER_MAPPING(DB, DB_TYPE_ENUM.Oracle);
                verMapping = t_c_sku_ver_mapping.GetMappingBySkuAndVersion(wo.SkuNO, wo.SKU_VER, DB);
                if (verMapping != null)
                {
                    O_VER.Value = verMapping.FOX_VERSION2;
                    O_CVER.Value = verMapping.CUSTOMER_VERSION;
                }
                else
                {
                    O_VER.Value = wo.SKU_VER;
                    O_CVER.Value = wo.SKU_VER;
                }
            }
            else
            {
                O_VER.Value = wo.SKU_VER;
                O_CVER.Value = wo.SKU_VER;
            }
            O_QTY.Value = ((List<string>)O_SN.Value).Count.ToString();
            O_PLNO.Value = pallet.DATA.PACK_NO;
            O_SKUNO.Value = wo.SkuNO;
        }

        private void GetSnKP(OleExec DB, string sn)
        {
            var list = DB.ORM.Queryable<R_SN_KP>().Where(r => r.SN == sn && r.VALID_FLAG == 1).ToList();
            if (list.Count > 0)
            {
                listAllKP.AddRange(list);
                foreach (var kp in list)
                {
                    if (kp.VALUE != kp.SN)
                    {
                        GetSnKP(DB, kp.VALUE);
                    }
                }
            }
        }
    }
}
