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
    public class VertivCartonLabel:LabelBase
    {
        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_GPN = new LabelOutput() { Name = "LSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_GSN = new LabelOutput() { Name = "GSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTDATE = new LabelOutput() { Name = "PRINTDATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CVER = new LabelOutput() { Name = "CVER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CARTON = new LabelOutput() { Name = "CARTON", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        public VertivCartonLabel()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);
            Outputs.Add(O_SN);
            Outputs.Add(O_GPN);
            Outputs.Add(O_GSN);
            Outputs.Add(O_PRINTDATE);
            Outputs.Add(O_VER);
            Outputs.Add(O_CVER);
            Outputs.Add(O_CARTON);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_QTY);
        }

        T_R_SN t_r_sn;
        T_R_SN_KP t_r_sn_kp;
        T_C_SKU_VER_MAPPING t_c_sku_ver_mapping;

        public override void MakeLabel(OleExec DB)
        {
            //base.MakeLabel(DB);
            t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //C_SKU_Label labelName = null;
            LogicObject.SN snObj;
            R_SN r_sn;
            if (I_SN.Value is string)
            {
                r_sn = t_r_sn.LoadSN(I_SN.Value.ToString(), DB);
            }
            else if (typeof(LogicObject.SN) == I_SN.Value.GetType())
            {
                snObj = (LogicObject.SN)I_SN.Value;
                r_sn = t_r_sn.LoadSN(snObj.SerialNo, DB);
            }
            else if (typeof(R_SN) == I_SN.Value.GetType())
            {
                r_sn = (R_SN)I_SN.Value;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { I_SN.Value.ToString() }));
            }

            if (r_sn == null)
            {
                return;
            }

            t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            t_c_sku_ver_mapping = new T_C_SKU_VER_MAPPING(DB, DB_TYPE_ENUM.Oracle);

            R_SN kpSN;
            R_SN_STATION_DETAIL snStationDetail = null;
            C_SKU_VER_MAPPING verMapping = null;
            R_WO_BASE r_wo_base = null;
            R_PACKING r_packing = null;
            List<R_SN_KP> KPList = new List<R_SN_KP>();
            List<R_SN_KP> printKPList = new List<R_SN_KP>();
            List<C_SKU_DETAIL> skuDetailList = new List<C_SKU_DETAIL>(); 

            KPList = t_r_sn_kp.GetKPRecordBySnID(r_sn.ID, DB);
            if (KPList.Count > 0)
            {
                skuDetailList = DB.ORM.Queryable<C_SKU_DETAIL>().Where(d => d.SKUNO == r_sn.SKUNO && d.STATION_NAME == I_STATION.Value.ToString()
                        && d.CATEGORY == "PRINT_CARTON" && d.CATEGORY_NAME == "KEYPART").ToList();

                foreach (C_SKU_DETAIL de in skuDetailList)
                {
                    foreach (R_SN_KP printKP in KPList)
                    {
                        kpSN = t_r_sn.LoadSN(printKP.VALUE, DB);
                        if (kpSN == null)
                        {
                            break;
                        }

                        printKPList = t_r_sn_kp.GetKPRecordBySnID(kpSN.ID, DB);
                        foreach (R_SN_KP k in printKPList)
                        {
                            foreach (LabelOutput o in Outputs)
                            {
                                if (o.Name == k.SCANTYPE)
                                {
                                    o.Value = k.VALUE;
                                }
                            }
                        }
                    }
                }
            }

            O_SN.Value = r_sn.SN;
            snStationDetail = DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(s => s.SN == r_sn.SN && s.VALID_FLAG =="1" && s.STATION_NAME == I_STATION.Value.ToString()).ToList().FirstOrDefault();
            DateTime dateTime = (DateTime)snStationDetail.EDIT_TIME;
            O_PRINTDATE.Value = dateTime.ToString("MM/dd/yyyy");
            r_wo_base = DB.ORM.Queryable<R_WO_BASE>().Where(wo => wo.WORKORDERNO == r_sn.WORKORDERNO).ToList().FirstOrDefault();
            O_SKUNO.Value = r_wo_base.SKUNO;
           r_packing = DB.ORM.Queryable<R_SN, R_SN_PACKING, R_PACKING>((rsn, rsnp, rp) => rsn.ID == rsnp.SN_ID && rsnp.PACK_ID == rp.ID)
                .Where((rsn, rsnp, rp) => rsn.ID == r_sn.ID).Select((rsn, rsnp, rp) => rp).ToList().FirstOrDefault();
            if (r_packing != null)
            {
                O_CARTON.Value = r_packing.PACK_NO;
                //O_QTY.Value = r_packing.QTY;
            }
            List<R_SN_PACKING> list = DB.ORM.Queryable<R_SN, R_SN_PACKING, R_SN_PACKING>((rsn, rsnp, rp) => rsn.ID == rsnp.SN_ID && rsnp.PACK_ID == rp.PACK_ID)
                .Where((rsn, rsnp, rp) => rsn.ID == r_sn.ID).Select((rsn, rsnp, rp) => rp).ToList();
            O_QTY.Value = list.Count;

            verMapping = t_c_sku_ver_mapping.GetMappingBySkuAndVersion(r_wo_base.SKUNO, r_wo_base.SKU_VER, DB);
            if (verMapping != null)
            {
                O_VER.Value = verMapping.FOX_VERSION2;
                O_CVER.Value = verMapping.CUSTOMER_VERSION;
            }
            else
            {
                O_VER.Value = r_wo_base.SKU_VER;
                O_CVER.Value = r_wo_base.SKU_VER;
            }

            //labelName = DB.ORM.Queryable<C_SKU_Label>().Where(l => l.SKUNO == r_sn.SKUNO && l.STATION == I_STATION.Value.ToString()).ToList().FirstOrDefault();
            //if (labelName.LABELNAME == "2000E3_CARTON")
            //{
            //    Get2000E3CartonValue(r_sn, DB);
            //}
        }

        private void Get2000E3CartonValue(R_SN r_sn, OleExec DB)
        {
            t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            t_c_sku_ver_mapping = new T_C_SKU_VER_MAPPING(DB, DB_TYPE_ENUM.Oracle);

            R_SN kpSN;
            R_SN_STATION_DETAIL snStationDetail = null;
            C_SKU_VER_MAPPING verMapping = null;
            R_WO_BASE r_wo_base = null;
            List<R_SN_KP> KPList = new List<R_SN_KP>();
            List<R_SN_KP> printKPList = new List<R_SN_KP>();
            R_SN_KP printKP = null;
            R_SN_KP GPNKP;
            R_SN_KP GSNKP;
            C_SKU_DETAIL skuDetail;

            r_wo_base = DB.ORM.Queryable<R_WO_BASE>().Where(wo => wo.WORKORDERNO == r_sn.WORKORDERNO).ToList().FirstOrDefault();
            KPList = t_r_sn_kp.GetKPRecordBySnID(r_sn.ID, DB);

            skuDetail = DB.ORM.Queryable<C_SKU_DETAIL>().Where(d => d.SKUNO == r_wo_base.SKUNO && d.STATION_NAME == I_STATION.Value.ToString()
                        && d.CATEGORY == "PRINT" && d.CATEGORY_NAME == "KEYPART").ToList().FirstOrDefault();
            if (skuDetail != null)
            {
                //打印keypart SN 對應的keypart 信息
                printKP = KPList.Find(k => k.PARTNO == skuDetail.VALUE);
            }

            if (printKP != null)
            {
                kpSN = t_r_sn.LoadSN(printKP.VALUE, DB);
                printKPList = t_r_sn_kp.GetKPRecordBySnID(kpSN.ID, DB);

                GPNKP = printKPList.Find(k => k.SCANTYPE == "GPN");
                GSNKP = printKPList.Find(k => k.SCANTYPE == "GSN");
                if (GPNKP != null)
                {
                    O_GPN.Value = GPNKP.VALUE;
                }
                if (GSNKP != null)
                {
                    O_GSN.Value = GSNKP.VALUE;
                }
            }

            snStationDetail = DB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(s => s.R_SN_ID == r_sn.ID && s.STATION_NAME == I_STATION.Value.ToString()).ToList().FirstOrDefault();
            DateTime dateTime = (DateTime)snStationDetail.EDIT_TIME;
            O_PRINTDATE.Value = dateTime.ToString("MM/dd/yyyy");

            verMapping = t_c_sku_ver_mapping.GetMappingBySkuAndVersion(r_wo_base.SKUNO, r_wo_base.SKU_VER, DB);
            if (verMapping != null)
            {
                O_VER.Value = verMapping.CUSTOMER_VERSION;
            }
            else
            {
                O_VER.Value = r_wo_base.SKU_VER;
            }
        }
    }
}
