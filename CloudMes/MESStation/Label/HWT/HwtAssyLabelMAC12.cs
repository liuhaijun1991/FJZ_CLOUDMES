using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDBHelper;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDataObject;
using MESPubLab.MESStation.Label;

namespace MESStation.Label.HWT
{
    /// <summary>
    /// HwtAssyLabelTC0010  獲取拉手條碼的變量
    /// HwtAssyLabelMAC12     獲取MAC12的打印變量  SUBTYPE='MAC'
    /// HwtAssyLabelMAC16     獲取MAC16的打印變量   SUBTYPE='SN'
    /// ASSY1  與ASSY 相同，只是沒有拉手條，不用另外寫
    /// ASSY2    
    /// ASSY4
    /// ADD BY HGB 2019.08.26
    /// </summary>
    class HwtAssyLabelMAC12 : LabelBase
    {
        #region 定義輸出

        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_MACSN = new LabelOutput() { Name = "MACSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTMAC = new LabelOutput() { Name = "PRINTMAC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTMAC1 = new LabelOutput() { Name = "PRINTMAC1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTMAC2 = new LabelOutput() { Name = "PRINTMAC2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTMAC3 = new LabelOutput() { Name = "PRINTMAC3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PRINTMAC4 = new LabelOutput() { Name = "PRINTMAC4", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        #endregion 定義輸出

        public HwtAssyLabelMAC12()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);//未用

            Outputs.Add(O_MACSN);
            Outputs.Add(O_PRINTMAC);
            Outputs.Add(O_PRINTMAC1);
            Outputs.Add(O_PRINTMAC2);
            Outputs.Add(O_PRINTMAC3);
            Outputs.Add(O_PRINTMAC4);
        }


        /// <summary>
        /// ADD BY HGB 2019.07.17
        /// </summary>
        /// <param name="DB"></param>
        public override void MakeLabel(OleExec DB)
        {
            T_R_SN_MAC t_r_sn_mac = new T_R_SN_MAC(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_SN_MAC r_sn_mac = new R_SN_MAC();
            
            T_C_MACPRINT_CONFIG t_c_macprint_config = new T_C_MACPRINT_CONFIG(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_MACPRINT_CONFIG c_macprint_config = new C_MACPRINT_CONFIG();
            T_C_CONTROL t_c_control = new T_C_CONTROL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            string SN = string.Empty;
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
            SN = r_sn.SN;
            #endregion 獲取真實SN 

            #region 是否有MAC,MAC16等 ,有則進行變量賦值
            if (t_r_sn_mac.CheckExists(SN, "MAC", "", DB))
            {
                #region 本工站是否要打印
                if (t_c_macprint_config.CheckExists(r_sn.SKUNO, "MAC", r_sn.NEXT_STATION, DB))
                {
                    if (!t_r_sn_mac.CheckExists(SN, "MAC", "CHECKNULL", DB))
                    {
                        throw new MESReturnMessage($@" 該條碼MAC未分配MAC12，請確認！{SN}");
                    }
                    r_sn_mac = t_r_sn_mac.LoadData(SN, "MAC", "", DB);
                    O_MACSN.Value = r_sn_mac.SUBSN.Substring(0, 12);
                    string printmac = r_sn_mac.SUBSN;
                    string MAC = r_sn_mac.SUBSN.Substring(0, 12);

                    if (t_r_sn_mac.CheckUnique(SN, "MAC", printmac, DB))
                    {
                        throw new MESReturnMessage($@"Error,MAC重碼！{SN},{MAC}");
                    }

                    if (t_c_control.ValueIsExist("TCMAC10*6", r_sn.SKUNO, DB))
                    {
                        O_PRINTMAC1.Value = printmac.Substring(0, 4);
                        O_PRINTMAC2.Value = printmac.Substring(4, 4);
                        O_PRINTMAC3.Value = printmac.Substring(0, 9);
                    }

                    if (t_c_control.ValueIsExist("TCMAC18*6", r_sn.SKUNO, DB))
                    {
                        O_PRINTMAC1.Value = printmac.Substring(0, 4);
                        O_PRINTMAC2.Value = printmac.Substring(4, 4);
                        O_PRINTMAC3.Value = printmac.Substring(8, 4);
                        O_PRINTMAC3.Value = printmac.Substring(0, 13);
                    }
                    else
                    {
                        O_PRINTMAC.Value = printmac;
                    }
                }

                # endregion 
            }

            #endregion

        }


    }
}
