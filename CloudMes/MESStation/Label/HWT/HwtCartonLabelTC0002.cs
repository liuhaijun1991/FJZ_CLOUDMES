using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;

namespace MESStation.Label.HWT
{

    /// <summary>
    /// TC0030整機單包規模板調用
    /// TC0031整機多包模板調用
    /// TC0003舊整機調用
    /// TC0002舊單板
    /// TC0002_NEW單板調用
    /// ADD BY HGB 2019.06.26
    /// </summary>
    class HwtCartonLabelTC0002 : LabelBase
    {
        #region 定義輸出

        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_CAROTON = new LabelInputValue() { Name = "CAROTON", Type = "STRING", Value = "", StationSessionType = "PRINT_CTN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_CAROTON = new LabelOutput() { Name = "CAROTON", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_DESC = new LabelOutput() { Name = "DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_ENGLISH_DESC = new LabelOutput() { Name = "ENGLISH_DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CHINESE_DESC = new LabelOutput() { Name = "CHINESE_DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CHINESE_AUTO = new LabelOutput() { Name = "CHINESE_AUTO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_FGSKUNO = new LabelOutput() { Name = "FGSKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_HEALTH = new LabelOutput() { Name = "HEALTH", Type = LabelOutPutTypeEnum.String, Description = "", Value = "1" };
        LabelOutput O_ROHS = new LabelOutput() { Name = "ROHS", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_REMARK = new LabelOutput() { Name = "REMARK", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNOTYPE = new LabelOutput() { Name = "SKUNOTYPE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Var10 = new LabelOutput() { Name = "Var10", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Var12 = new LabelOutput() { Name = "Var12", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_Var13 = new LabelOutput() { Name = "Var13", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_WO = new LabelOutput() { Name = "WO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        LabelOutput O_MAC = new LabelOutput() { Name = "MAC", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        #endregion 定義輸出

        public HwtCartonLabelTC0002()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_CAROTON);//未用
            Inputs.Add(I_STATION);//未用

            Outputs.Add(O_CAROTON);
            Outputs.Add(O_DESC);
            Outputs.Add(O_ENGLISH_DESC);
            Outputs.Add(O_CHINESE_DESC);
            Outputs.Add(O_CHINESE_AUTO);
            Outputs.Add(O_FGSKUNO);
            Outputs.Add(O_HEALTH);
            Outputs.Add(O_ROHS);
            Outputs.Add(O_QTY);
            Outputs.Add(O_REMARK);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_SKUNOTYPE);
            Outputs.Add(O_Var10);
            Outputs.Add(O_Var12);
            Outputs.Add(O_Var13);
            Outputs.Add(O_VER);
            Outputs.Add(O_WO);
            Outputs.Add(O_SN);
            Outputs.Add(O_MAC);
        }



        /// <summary>
        /// ADD BY HGB 2019.06.20
        /// </summary>
        /// <param name="DB"></param>
        public override void MakeLabel(OleExec DB)
        {
            #region 獲取真實SN
            //base.MakeLabel(DB);

            T_R_SN t_r_sn = new T_R_SN(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            //C_SKU_Label labelName = null;
            LogicObject.SN snObj;
            R_SN r_sn;
            if (I_SN.Value is string)
            {
                r_sn = t_r_sn.LoadData(I_SN.Value.ToString(), DB);
            }
            else if (typeof(LogicObject.SN) == I_SN.Value.GetType())
            {
                snObj = (LogicObject.SN)I_SN.Value;
                r_sn = t_r_sn.LoadData(snObj.SerialNo, DB);
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
            #endregion 獲取真實SN 

            O_FGSKUNO.Value = r_sn.SKUNO;
            O_WO.Value = r_sn.WORKORDERNO;

            #region 檢查機種BUSINESS模式
            try
            {
                T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(DB, DB_TYPE_ENUM.Oracle);

                T_C_SKU t_c_sku = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle);

                C_SKU_DETAIL c_sku_detail = t_c_sku_detail.GetSkuDetail("SKUNOTYPE", "BUYSELL", r_sn.SKUNO, DB);
                if (c_sku_detail == null)
                {
                    throw new System.Exception("機種BUSINESS模式沒有設定[C_SKU_DETAIL]");
                }
                else
                {
                    if (t_c_sku.CheckSku(r_sn.SKUNO, DB))
                    {
                        C_SKU c_sku = t_c_sku.GetBySKU(r_sn.SKUNO, DB);

                        O_DESC.Value = c_sku.DESCRIPTION;
                        O_SKUNO.Value = c_sku.CUST_PARTNO;
                        if (c_sku.VERSION == null || c_sku.VERSION.Length == 0)
                        {
                            O_VER.Value = "/";
                        }
                        else
                        {
                            O_VER.Value = c_sku.VERSION;
                        }
                        O_SKUNOTYPE.Value = c_sku_detail.CATEGORY_NAME;

                    }

                }
            }
            catch (Exception)
            {
                throw new System.Exception("機種BUSINESS模式沒有設定[C_SKU_DETAIL]");
            }
            #endregion

            string defaultqty, qty;
            #region  獲取卡通數量
            T_R_PACKING t_r_packing = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            try
            {
                R_PACKING r_packing = t_r_packing.GetBYPACKNO(I_CAROTON.Value.ToString(), DB);
                O_QTY.Value = r_packing.QTY;
                defaultqty = r_packing.MAX_QTY.ToString();
                qty = r_packing.QTY.ToString();
            }
            catch
            {
                throw new System.Exception("獲取卡通數量異常[R_PACKING]," + I_CAROTON.Value.ToString());
            }
            O_HEALTH.Value = "Y";//永遠是Y

            T_C_SKUNO_BASE t_c_skuno_base = new T_C_SKUNO_BASE(DB, DB_TYPE_ENUM.Oracle);
            if (!t_c_skuno_base.CheckExists(O_SKUNO.Value.ToString(), DB))
            {
                throw new MESReturnMessage("機種基本信息未建立-C_SKUNO_BASE");

            }

            #endregion 獲取卡通數量

            #region 獲取中英文描述


            try
            {
                C_SKUNO_BASE c_skuno_base = t_c_skuno_base.LOAD_C_SKUNO_BASE(O_SKUNO.Value.ToString(), DB);

                O_CHINESE_AUTO.Value = c_skuno_base.DESC_CHINESE;
                O_CHINESE_DESC.Value = c_skuno_base.DESC_CHINESE;
                O_ENGLISH_DESC.Value = c_skuno_base.DESCRIPTION;
            }
            catch
            {
                throw new MESReturnMessage("機種基本信息未建立-C_SKUNO_BASE");
            }

            #endregion  獲取中英文描述

            #region 獲取中文簡體存在數據庫的一些變量

            string weishuxiang = string.Empty;//尾數箱簡體
            string feihuanbao = string.Empty;//非環保簡體
            string cuntrolrun = string.Empty;//cuntrolrun不可混裝 簡體
            try
            {
                T_C_CONTROL t_c_control = new T_C_CONTROL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (!t_c_control.ValueIsExist("TC0002_NEW_SIMPLE_CHINESE", "尾数箱", DB))
                {
                    throw new System.Exception("獲取簡體尾數箱，環保，非環保，CONTROL RUN 不可混裝錯誤[C_CONTROL.TC0002_NEW_SIMPLE_CHINESE],");
                }
                C_CONTROL c_control = t_c_control.GetControlList("TC0002_NEW_SIMPLE_CHINESE", "尾数箱", DB)[0];
                weishuxiang = c_control.CONTROL_VALUE;
                feihuanbao = c_control.CONTROL_TYPE;
                cuntrolrun = c_control.CONTROL_LEVEL;
            }
            catch
            {
                throw new System.Exception("獲取簡體尾數箱，環保，非環保，CONTROL RUN 不可混裝錯誤[C_CONTROL.TC0002_NEW_SIMPLE_CHINESE],");
            }

            #endregion  獲取中文簡體存在數據庫的一些變量

            #region 尾數箱判斷

            if (defaultqty == qty)
            {
                O_Var10.Value = "0";
                O_Var12.Value = "";
            }
            else
            {
                O_Var10.Value = "1";
                O_Var13.Value = weishuxiang;//尾數箱簡體
            }
            #endregion




            #region 獲取 備註信息(remark)
            try
            {
                T_C_CONTROL t_c_control = new T_C_CONTROL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (t_c_control.ValueIsExist("WO_REMARK", r_sn.WORKORDERNO, DB))
                {
                    string REMARK = "";
                    try
                    {
                        REMARK = t_c_control.GetControlByNameAndValue("WO_REMARK", r_sn.WORKORDERNO, DB).CONTROL_DESC;
                    }
                    catch
                    {

                    }

                    if (REMARK.Length > 0)
                    {
                        O_REMARK.Value = REMARK;
                    }
                }
                else
                {
                    if (t_c_control.ValueIsExist("NO_MIXED", r_sn.WORKORDERNO, DB))
                    {
                        O_REMARK.Value = cuntrolrun;
                    }
                    else
                    {
                        O_REMARK.Value = "";
                    }

                }
            }
            catch
            {

            }

            #endregion 獲取 備註信息(remark)

            #region 獲取環保屬性ROHS
            try
            {
                T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                R_WO_BASE r_wo_base = t_r_wo_base.GetWoByWoNo(r_sn.WORKORDERNO, DB);
                if (r_wo_base.ROHS == "R5")
                {
                    O_ROHS.Value = "N";
                }
                else if (r_wo_base.ROHS == "R6")
                {
                    O_ROHS.Value = "Y";
                }
                else
                {
                    throw new System.Exception(" 必須是R5或者R6");
                }
            }

            catch
            {
                throw new System.Exception("加載工單失敗");
            }




            #endregion 獲取環保屬性ROHS

            #region 獲取SNLIST 和MAC LIST
            try
            {
                List<string> SnList = new List<string>();
                t_r_packing.GetSNListByPackNo(I_CAROTON.Value.ToString(), ref SnList, DB);
                O_SN.Value = SnList;

                List<string> MacList = new List<string>();
                t_r_packing.GetSNListByPackNo(I_CAROTON.Value.ToString(), ref MacList, DB);
                O_MAC.Value = MacList;
            }
            catch
            {
                throw new System.Exception("獲取卡通箱號SN列表[R_PACKING]," + I_CAROTON.Value.ToString());
            }

            #endregion 獲取SNLIST 和MAC LIST
        }


    }
}
