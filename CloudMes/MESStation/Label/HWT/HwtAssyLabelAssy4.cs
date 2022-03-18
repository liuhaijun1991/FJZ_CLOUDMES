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
using System.Globalization;
namespace MESStation.Label.HWT
{

    /// HwtAssyLabelTC0010  獲取拉手條碼的變量
    /// HwtAssyLabelMAC12     獲取MAC12的打印變量  SUBTYPE='MAC'
    /// HwtAssyLabelMAC16     獲取MAC16的打印變量   SUBTYPE='SN'
    /// ASSY1  與ASSY 相同，只是沒有拉手條，不用另外寫
    /// ASSY2    
    /// HwtAssyLabelAssy4 ASSY4定制標籤
    /// ADD BY HGB 2019.08.26
    class HwtAssyLabelAssy4 : LabelBase
    {
        #region 定義輸出

        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };

        LabelOutput O_MODEL = new LabelOutput() { Name = "MODEL", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_IMEI = new LabelOutput() { Name = "IMEI", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        
        #endregion 定義輸出

        public HwtAssyLabelAssy4()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);//未用

            Outputs.Add(O_MODEL);
            Outputs.Add(O_IMEI);
           
        }


        /// <summary>
        /// ADD BY HGB 2019.07.17
        /// </summary>
        /// <param name="DB"></param>
        public override void MakeLabel(OleExec DB)
        {
            T_R_SN_MAC t_r_sn_mac = new T_R_SN_MAC(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_SN_MAC r_sn_mac = new R_SN_MAC();

            T_R_SN_KP t_r_sn_kp   = new T_R_SN_KP(DB, MESDataObject.DB_TYPE_ENUM.Oracle);         
            List<R_SN_KP> r_sn_kp = new List<R_SN_KP>();

            T_C_MACPRINT_CONFIG t_c_macprint_config = new T_C_MACPRINT_CONFIG(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_MACPRINT_CONFIG c_macprint_config = new C_MACPRINT_CONFIG();
            T_C_CONTROL t_c_control = new T_C_CONTROL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);

            T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            List <C_SKU_DETAIL>  c_sku_detail = new List < C_SKU_DETAIL >();

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

            #region 賦值O_MODEL
            string rule = string.Empty;
            string getrule = string.Empty;
            string year = string.Empty;
            year = DateTime.Now.Year.ToString().Substring(2,2);
             
            GregorianCalendar gc = new GregorianCalendar();
            int  yearofweek = gc.GetWeekOfYear(DateTime.Now,CalendarWeekRule.FirstDay,DayOfWeek.Monday);
            string ls = string.Empty;//流水
            ls = t_c_sku_detail.GetSerialno(DB);
            c_sku_detail = t_c_sku_detail.LoadListData(r_sn.SKUNO, "KEYPART", "PCB S/N3", DB);
            if (c_sku_detail.Count >0)
            {
                foreach (C_SKU_DETAIL item in c_sku_detail)
                {
                      rule = item.BASETEMPLATE == null ? "" : item.BASETEMPLATE.ToString();
                    if (rule.Length>18)
                    {
                        if (rule.Substring(0, 2)=="KP")
                        {
                            getrule = rule.Substring(0, 18);
                        }
                    } 
                } 
            }
            O_MODEL.Value = getrule + year + yearofweek + ls;

            #endregion

            #region 如果是補印O_IMEI
            string value=string.Empty;          
            r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, "", DB);
            if (r_sn_kp.Count > 0)
            {
                foreach (R_SN_KP item in r_sn_kp)
                {
                    value = item.VALUE == null ? "" : item.VALUE.ToString();
                    if (value.Length > 2)
                    {
                        if (value.Substring(0, 2) == "KP")
                        {
                            O_MODEL.Value = value;
                        }
                    }
                }
            }
            #endregion

            #region 賦值O_IMEI

            r_sn_kp = t_r_sn_kp.LoadListDataBySnAndKpName(SN, "KPIMEI", DB);
            if (r_sn_kp.Count > 0)
            {
                O_IMEI.Value = r_sn_kp[0].VALUE;
            }

            #endregion

        }


    }
}
