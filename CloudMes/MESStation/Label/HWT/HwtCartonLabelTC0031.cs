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
    /// ADD BY HGB 2019.06.28
    class HwtCartonLabelTC0031 : LabelBase
    {
        #region 定義輸出

        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_CAROTON = new LabelInputValue() { Name = "CAROTON", Type = "STRING", Value = "", StationSessionType = "PRINT_CTN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_UPC = new LabelOutput() { Name = "UPC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_EAN = new LabelOutput() { Name = "EAN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CAROTON = new LabelOutput() { Name = "CAROTON", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_DESC = new LabelOutput() { Name = "DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_ENGLISH_DESC = new LabelOutput() { Name = "ENGLISH_DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CHINESE_DESC = new LabelOutput() { Name = "CHINESE_DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CHINESE_AUTO = new LabelOutput() { Name = "CHINESE_AUTO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_REMARK = new LabelOutput() { Name = "REMARK", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
        #endregion 定義輸出

        public HwtCartonLabelTC0031()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_CAROTON);//未用
            Inputs.Add(I_STATION);//未用

            Outputs.Add(O_UPC);
            Outputs.Add(O_EAN);
            Outputs.Add(O_CAROTON);
            Outputs.Add(O_DESC);
            Outputs.Add(O_ENGLISH_DESC);
            Outputs.Add(O_CHINESE_DESC);
            Outputs.Add(O_CHINESE_AUTO); 
            Outputs.Add(O_QTY);
            Outputs.Add(O_REMARK);
            Outputs.Add(O_SKUNO); 
            Outputs.Add(O_VER);
           
            Outputs.Add(O_SN);             
        }



        /// <summary>
        /// ADD BY HGB 2019.06.28
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

            //O_FGSKUNO.Value = r_sn.SKUNO;
            // O_WO.Value = r_sn.WORKORDERNO;

            #region 獲取客戶料號
            try
            {
                 T_C_SKU t_c_sku = new T_C_SKU(DB, DB_TYPE_ENUM.Oracle);
                if (t_c_sku.CheckSku(r_sn.SKUNO, DB))
                {
                    C_SKU c_sku = t_c_sku.GetBySKU(r_sn.SKUNO, DB); 
                    O_SKUNO.Value = c_sku.CUST_PARTNO; 
                }
            }
            catch (Exception)
            {
                throw new System.Exception("獲取客戶料號失敗[C_SKU]");
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

            #endregion 獲取卡通數量

            #region 獲取卡通箱號SN列表            

            try
            {
                List<string> SnList = new List<string>();
                 t_r_packing.GetSNListByPackNo(I_CAROTON.Value.ToString(),ref SnList, DB);
                O_SN.Value = SnList;
            }
            catch
            {
                throw new System.Exception("獲取卡通箱號SN列表[R_PACKING]," + I_CAROTON.Value.ToString());
            }

            #endregion 獲取卡通箱號SN列表

            #region 獲取中英文描述

            T_C_SKUNO_BASE t_c_skuno_base = new T_C_SKUNO_BASE(DB, DB_TYPE_ENUM.Oracle);
            if (!t_c_skuno_base.CheckExists(O_SKUNO.Value.ToString(), DB))
            {
                throw new MESReturnMessage("機種基本信息未建立-C_SKUNO_BASE");

            }
            try
            {
                C_SKUNO_BASE c_skuno_base = t_c_skuno_base.LOAD_C_SKUNO_BASE(O_SKUNO.Value.ToString(), DB);
                O_DESC.Value = c_skuno_base.MODEL;
                O_EAN.Value = c_skuno_base.EAN;
                O_UPC.Value = c_skuno_base.UPC;
                O_CHINESE_AUTO.Value = c_skuno_base.DESC_CHINESE;
                #region 系統加換行符
                string chinese = c_skuno_base.DESC_CHINESE;//有一行
                if (chinese.Length > 38)//有兩行
                {
                    chinese = chinese.Insert(47, "\n");
                    if (chinese.Length > 47 * 2 + 2)//有三行
                    {
                        chinese = chinese.Insert(47 * 2 + 2, "\n");
                        if (chinese.Length > 47 * 3 + 2)//有四行
                        {
                            chinese = chinese.Insert(47 * 3 + 2, "\n");
                        }
                    }
                }

                O_CHINESE_DESC.Value = chinese;
                #endregion 系統加換行符
                O_ENGLISH_DESC.Value = c_skuno_base.DESCRIPTION;
            }
            catch
            {
                throw new MESReturnMessage("機種基本信息未建立-C_SKUNO_BASE");
            }

            #endregion  獲取中英文描述

            #region 獲取 備註信息(remark)
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

            #endregion 獲取 備註信息(remark) 

            
        }


    }
}
