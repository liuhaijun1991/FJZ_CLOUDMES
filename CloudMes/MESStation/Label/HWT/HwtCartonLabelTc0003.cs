using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject;
using MESPubLab.MESStation.Label;
using System.Data;

namespace MESStation.Label.HWT
{

    /// <summary>
    /// TC0030整機單包規模板調用
    /// TC0031整機多包模板調用
    /// TC0003舊整機調用
    /// TC0002舊單板
    /// TC0002_NEW單板調用
    /// ADD BY HGB 2019.06.14
    /// </summary>
    class HwtCartonLabelTC0003 : LabelBase
    {
        #region 定義輸出

        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };
        /// <summary>
        /// 目前不用
        /// </summary>
        LabelOutput O_PRINTDATE = new LabelOutput() { Name = "PRINTDATE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SKUNO = new LabelOutput() { Name = "SKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        /// <summary>
        /// 寫死了是1
        /// </summary>
        LabelOutput O_QTY = new LabelOutput() { Name = "QTY", Type = LabelOutPutTypeEnum.String, Description = "", Value = "1" };
        LabelOutput O_UPC = new LabelOutput() { Name = "UPC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_EAN = new LabelOutput() { Name = "EAN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        /// <summary>
        /// 對外描述，model
        /// </summary>
        LabelOutput O_DESC = new LabelOutput() { Name = "DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_MAC = new LabelOutput() { Name = "MAC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        /// <summary>
        /// 取的是MAC16  
        /// </summary>
        LabelOutput O_GPONSN = new LabelOutput() { Name = "GPONSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        /// <summary>
        /// 未見使用
        /// </summary>
        LabelOutput O_IMEI = new LabelOutput() { Name = "IMEI", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_REMARK = new LabelOutput() { Name = "REMARK", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };


        /// <summary>
        /// 中文描述，系統加\n換行
        /// </summary>
        LabelOutput O_CHINESE_AUTO = new LabelOutput() { Name = "CHINESE_AUTO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        /// <summary>
        /// 中文描述，PE載描述里加\n換行
        /// </summary>
        LabelOutput O_CHINESE_DESC = new LabelOutput() { Name = "CHINESE_DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        LabelOutput O_ENGLISH_DESC = new LabelOutput() { Name = "ENGLISH_DESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        /// <summary>
        /// 都是空，未見抓取
        /// </summary>
        LabelOutput O_VER = new LabelOutput() { Name = "VER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CARTON = new LabelOutput() { Name = "CARTON", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };

        #endregion 定義輸出

        public HwtCartonLabelTC0003()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);//未用

            Outputs.Add(O_DESC);
            Outputs.Add(O_PRINTDATE);//未用
            Outputs.Add(O_SN);
            Outputs.Add(O_SKUNO);
            Outputs.Add(O_QTY);
            Outputs.Add(O_UPC);
            Outputs.Add(O_EAN);
            Outputs.Add(O_MAC);
            Outputs.Add(O_GPONSN);
            Outputs.Add(O_IMEI);
            Outputs.Add(O_REMARK);
            Outputs.Add(O_CHINESE_AUTO);
            Outputs.Add(O_CHINESE_DESC);
            Outputs.Add(O_ENGLISH_DESC);
            Outputs.Add(O_VER);//未用
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

            #region 獲取華為料號
            T_C_ITEMCODE_MAPPING_EMS t_c_itemcode_mapping_ems = new T_C_ITEMCODE_MAPPING_EMS(DB, DB_TYPE_ENUM.Oracle);
            string part_no = t_c_itemcode_mapping_ems.Get_Customer_Partno("KP_NO", r_sn.SKUNO, DB);
            O_SKUNO.Value = part_no;

            #endregion

            #region 中英文描述，EAN UPC MODEL
            T_C_SKUNO_BASE t_c_skuno_base = new T_C_SKUNO_BASE(DB, DB_TYPE_ENUM.Oracle);
            if (!t_c_skuno_base.CheckExists(part_no, DB))
            {
                throw new MESReturnMessage("機種基本信息未建立-C_SKUNO_BASE");

            }
            C_SKUNO_BASE c_skuno_base = t_c_skuno_base.LOAD_C_SKUNO_BASE(part_no, DB);

            O_SN.Value = r_sn.SN;
            O_DESC.Value = c_skuno_base.MODEL;
            O_EAN.Value = c_skuno_base.EAN;
            O_UPC.Value = c_skuno_base.UPC;
            O_CHINESE_AUTO.Value = c_skuno_base.DESC_CHINESE;

            #region 系統加換行符
            string chinese = c_skuno_base.DESC_CHINESE;//有一行
            if (chinese.Length > 35)//有兩行
            {
                chinese = chinese.Insert(35, "\n");
                if (chinese.Length > 35 * 2 + 2)//有三行
                {
                    chinese = chinese.Insert(35 * 2 + 2, "\n");
                    if (chinese.Length > 35 * 3 + 2)//有四行
                    {
                        chinese = chinese.Insert(35 * 3 + 2, "\n");
                    }
                }
            }

            O_CHINESE_DESC.Value = chinese;
            #endregion 系統加換行符

            O_ENGLISH_DESC.Value = c_skuno_base.DESCRIPTION;
            #endregion 中英文描述，EAN UPC MODEL 


            #region 獲取版本
            string SKUTYPE = t_c_itemcode_mapping_ems.Get_Customer_Partno("SKUTYPE", r_sn.SKUNO, DB);
            if (SKUTYPE == "NEW")
            {
                string sql = string.Empty;
              
                DataTable dt = new DataTable();
                sql = $@" 
                SELECT p_version
        FROM mes4.r_wo_base@hwapdb
       WHERE p_no = var_skuno
         AND wo = '{r_sn.WORKORDERNO}'
                 ";
                dt = DB.ExecSelect(sql).Tables[0];
                if (dt.Rows.Count > 0)
                {
                    O_VER.Value = dt.Rows[0][0].ToString();
                }
                else
                {
                    throw new MESReturnMessage(r_sn.WORKORDERNO + ",獲取ALLPART版本失敗"); ;
                }               
            }
            else
            {
                O_VER.Value = "";//舊版本機種取空，新版本機種取ALLPART版本
            }


            #endregion 獲取版本


            #region 獲取mac部分

            T_C_MACLABEL_CONFIG t_c_maclabel_config = new T_C_MACLABEL_CONFIG(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
           
            if (t_c_maclabel_config.IsExist(r_sn.SKUNO, DB))
            {
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                try
                {

                    string  var_maccategory = t_c_maclabel_config.LoadData( r_sn.SKUNO, DB).MACCATEGORY;//第一層KP
                    string var_maccategory2 = t_c_maclabel_config.LoadData(r_sn.SKUNO, DB).DATA1;//第二層KP
                    string var_step = t_c_maclabel_config.LoadData(r_sn.SKUNO, DB).STEP.ToString();
                    if (var_maccategory == "MAC1 S/N")
                    {
                        O_MAC.Value = "";

                        if (t_r_sn_kp.IsExistsBySnAndKpName(r_sn.SN, "MAC1 S/N", DB))
                        {
                            O_MAC.Value = t_r_sn_kp.LoadDataBySnAndKpName(r_sn.SN, "MAC1 S/N", DB).VALUE;
                        }
                        else
                        {
                            throw new MESReturnMessage("獲取第一層MAC失敗MAC1 S/N-R_SN_KP");
                        } 
                    }
                    else
                    {
                        O_MAC.Value = "";
                        if (t_r_sn_kp.IsExistsBySnAndKpName(r_sn.SN, var_maccategory, DB))
                        {
                            string SECONDKPSN = t_r_sn_kp.LoadDataBySnAndKpName(r_sn.SN, var_maccategory, DB).VALUE;

                            if (t_r_sn_kp.IsExistsBySnAndKpName(SECONDKPSN, "MAC1 S/N", DB))
                            {
                                O_MAC.Value = t_r_sn_kp.LoadDataBySnAndKpName(SECONDKPSN, "MAC1 S/N", DB).VALUE;
                            }
                            else
                            {

                                if (t_r_sn_kp.IsExistsBySnAndKpName(SECONDKPSN, var_maccategory2, DB))
                                {
                                    string ThirdKPSN = t_r_sn_kp.LoadDataBySnAndKpName(SECONDKPSN, var_maccategory2, DB).VALUE;

                                    if (t_r_sn_kp.IsExistsBySnAndKpName(ThirdKPSN, "MAC1 S/N", DB))
                                    {
                                        O_MAC.Value = t_r_sn_kp.LoadDataBySnAndKpName(ThirdKPSN, "MAC1 S/N", DB).VALUE;
                                    }
                                    else
                                    {
                                        throw new MESReturnMessage("獲取第三層MAC失敗MAC1 S/N-R_SN_KP");
                                    }
                                }
                                else
                                {
                                    throw new MESReturnMessage("獲取第三層KP失敗MAC1 S/N-R_SN_KP");
                                } 
                               
                            }

                        }
                        else
                        {
                            throw new MESReturnMessage("獲取第二層KP失敗-R_SN_KP");

                        } 

                    }
                }
                catch
                {
                    throw new MESReturnMessage("獲取MAC配置信息失敗C_MACLABEL_CONFIG"+ r_sn.SKUNO);
                }

            }

            #endregion 獲取mac部分

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

            #region 備件模板調用,R5模板調用寫在外層，調用MAKElabel之前
            #endregion  備件模板調用,R5模板調用寫在外層，調用MAKElabel之前

            #region  獲取IMEI
            T_C_MACPRINT_CONFIG t_c_macprint_config = new T_C_MACPRINT_CONFIG(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            if (t_c_macprint_config.CheckExistsByskuAndLabeltype(r_sn.SKUNO, "KPIMEI", DB))
            {
                T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string KPIMEI = "";
                try
                {
                    KPIMEI = t_r_sn_kp.GetSecondKp(r_sn.SN, "KPIMEI", "ASSY", DB).VALUE;
                }
                catch
                { }

                if (KPIMEI.Length == 0 || KPIMEI == null)
                {
                    throw new MESReturnMessage("配置需要打印KPIMEI，但是獲取到KPIMEI為空");
                }
                else
                {
                    O_IMEI.Value = KPIMEI;
                }

            }
            else
            {
                //檢查是否需要打印IMEI
                if (t_c_control.ValueIsExist("TC0041", r_sn.SKUNO, DB))
                {
                    T_R_SN_KP t_r_sn_kp = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                    string IMEI = "";
                    try
                    {
                        IMEI = t_r_sn_kp.LoadDataBySnAndKpName(r_sn.SN, "KPIMEI", DB).VALUE;
                    }
                    catch
                    { }


                    if (IMEI.Length == 0 || IMEI == null)
                    {
                        IMEI = t_r_sn_kp.GetSecondKp(r_sn.SN, "KPIMEI", "", DB).VALUE;
                        if (IMEI.Length == 0 || IMEI == null)
                        {
                            throw new MESReturnMessage("配置需要打印KPIMEI，但是獲取到KPIMEI為空");
                        }
                        else
                        {
                            O_IMEI.Value = IMEI;
                        }

                    }
                    else
                    {
                        O_IMEI.Value = IMEI;
                    }
                }

            }

            #endregion  獲取IMEI

            #region  獲取gponsn  由sfccodelike_extend改為C_CONTROL控制是否打印

            if (t_c_control.ValueIsExist("PRINT_GPONSN", r_sn.SKUNO, DB))
            {
                T_R_SN_MAC t_r_sn_mac = new T_R_SN_MAC(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                string GPONSN = "";
                try
                {
                    GPONSN = t_r_sn_mac.GetGponSnByMac(O_MAC.Value.ToString(), DB).SUBSN;
                }
                catch
                { }


                if (GPONSN.Length == 0 || GPONSN == null)
                {
                    throw new MESReturnMessage("配置需要打印GPONSN，但是獲取到配置需要打印GPONSN為空," + O_MAC.Value.ToString());
                }
                else
                {
                    O_GPONSN.Value = GPONSN;
                }

            }

            #endregion     獲取gponsn

            #region SN變BOXSN
            
            if (t_c_control.ValueIsExist("TC0016", r_sn.SKUNO, DB))
            {                 
               T_R_PACKING t_r_packing =new T_R_PACKING(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (t_c_control.ValueIsExist("TC0015", r_sn.SKUNO, DB))
                {
                   O_SN.Value = r_sn.BOXSN;
                   O_CARTON.Value= t_r_packing.GetPackingObjectBySN(r_sn.SN,DB).PACK_NO;
                }
                else
                {
                    O_SN.Value = r_sn.BOXSN;
                } 

            }
           # endregion SN變BOXSN

        }


    }
}
