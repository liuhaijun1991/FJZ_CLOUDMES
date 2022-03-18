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
    /// ADD BY HGB 2019.07.17
    /// </summary>
    class HwtAssyLabelTC0010 : LabelBase
    {
        #region 定義輸出

        LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
        LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };


        LabelOutput O_PANELSSN = new LabelOutput() { Name = "PANELSSN", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PANELSSN1 = new LabelOutput() { Name = "PANELSSN1", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PANELSSN2 = new LabelOutput() { Name = "PANELSSN2", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PANELSSN3 = new LabelOutput() { Name = "PANELSSN3", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CUSTVER = new LabelOutput() { Name = "CUSTVER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_HWSKUNO = new LabelOutput() { Name = "HWSKUNO", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_HWSKUVER = new LabelOutput() { Name = "HWSKUVER", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_SNVERSION = new LabelOutput() { Name = "SNVERSION", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_BARCODE = new LabelOutput() { Name = "BARCODE", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CUSTDESC = new LabelOutput() { Name = "CUSTDESC", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_CUSTSKU = new LabelOutput() { Name = "CUSTSKU", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        LabelOutput O_PANELSSNNEW = new LabelOutput() { Name = "PANELSSNNEW", Type = LabelOutPutTypeEnum.String, Description = "", Value = "" };
        #endregion 定義輸出

        public HwtAssyLabelTC0010()
        {
            Inputs.Add(I_SN);
            Inputs.Add(I_STATION);//未用

            Outputs.Add(O_PANELSSN);
            Outputs.Add(O_PANELSSN1);
            Outputs.Add(O_PANELSSN2);
            Outputs.Add(O_PANELSSN3);
            Outputs.Add(O_CUSTVER);
            Outputs.Add(O_HWSKUNO);
            Outputs.Add(O_HWSKUVER);
            Outputs.Add(O_SNVERSION);
            Outputs.Add(O_BARCODE);
            Outputs.Add(O_CUSTDESC);
            Outputs.Add(O_CUSTSKU);
            Outputs.Add(O_PANELSSNNEW);
        }


        /// <summary>
        /// ADD BY HGB 2019.07.17
        /// </summary>
        /// <param name="DB"></param>
        public override void MakeLabel(OleExec DB)
        {
            T_R_WO_TYPE WOType = new T_R_WO_TYPE(DB, DB_TYPE_ENUM.Oracle);
            T_C_SKU_DETAIL t_c_sku_detail = new T_C_SKU_DETAIL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_SKU_DETAIL c_sku_detail = new C_SKU_DETAIL();
            T_R_WO_BASE t_r_wo_base = new T_R_WO_BASE(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_WO_BASE r_wo_base = new R_WO_BASE();
            T_R_RELATIONDATA_EXTERNAL t_r_relationdata_external = new T_R_RELATIONDATA_EXTERNAL(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_RELATIONDATA_EXTERNAL r_relationdata_external = new R_RELATIONDATA_EXTERNAL();
            T_R_2D_SN_RMA_T t_r_2d_sn_rma_t = new T_R_2D_SN_RMA_T(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            R_2D_SN_RMA_T r_2d_sn_rma_t = new R_2D_SN_RMA_T();
            T_C_SKU t_c_sku = new T_C_SKU(DB, MESDataObject.DB_TYPE_ENUM.Oracle);
            C_SKU c_sku = new C_SKU();

            string SN = string.Empty;
            string WO = string.Empty;
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
            WO = r_sn.WORKORDERNO;
            #endregion 獲取真實SN 

            #region 獲取工單版本

            if (t_r_wo_base.IsExist(r_sn.WORKORDERNO, DB))
            {
                r_wo_base = t_r_wo_base.GetWoByWoNo(r_sn.WORKORDERNO, DB);
                O_CUSTVER.Value = r_wo_base.SKU_VER;
            }
            else
            {
                throw new MESReturnMessage($@"工單不存在于R_WO_BASE{r_sn.WORKORDERNO}");
            }
            #endregion

            #region 獲取各種打印類型的變量

            string var_sntype = string.Empty;
            if (t_c_sku_detail.CheckExists(r_sn.SKUNO, "LABELTYPE", r_sn.NEXT_STATION, DB))
            {
                c_sku_detail = t_c_sku_detail.LoadData(r_sn.SKUNO, "LABELTYPE", r_sn.NEXT_STATION, DB);
                var_sntype = c_sku_detail.EXTEND;
                string sntypeall = "10,20,30,40,50,60,70,80";
                if (!sntypeall.Contains(var_sntype))
                {
                    throw new MESReturnMessage($@"拉手條碼的編碼規則沒有設定{r_sn.SKUNO}﹐[C_SKU_DETAIL的EXTEND欄位]");
                }
                sntypeall = "10,40,50,60,70,80";
                if (sntypeall.Contains(var_sntype))
                {
                    #region 獲取父項
                    O_PANELSSN.Value = SN.Substring(0,10) + "6" + SN.Substring(SN.Length - 5, 5);
                    if (WOType.IsTypeInput("RMA", WO.Substring(0, 6), DB))
                    {
                        if (t_r_relationdata_external.IsExists(r_sn.SN, DB))
                        {
                            O_PANELSSN.Value = t_r_relationdata_external.LoadData(SN, DB).PARENT;
                        }

                    }
                    #endregion

                    #region 獲取新一代二維碼標籤的SN編碼規則
                    if (t_c_sku_detail.CheckExists(r_sn.SKUNO, "LABELTYPE", "NEW_2D", DB))
                    {
                        string var_year = string.Empty;
                        var_year = SN.Substring(SN.Length - 9, 1);
                        if (var_year == "H")
                        {
                            var_year = "17";
                        }
                        else if (var_year == "J")
                        {
                            var_year = "18";
                        }
                        else if (var_year == "K")
                        {
                            var_year = "19";
                        }
                        else if (var_year == "L")
                        {
                            var_year = "20";
                        }
                        else if (var_year == "M")
                        {
                            var_year = "21";
                        }
                        else if (var_year == "N")
                        {
                            var_year = "22";
                        }

                        else if (var_year == "P")
                        {
                            var_year = "23";
                        }
                        else if (var_year == "Q")
                        {
                            var_year = "24";
                        }
                        else if (var_year == "R")
                        {
                            var_year = "25";
                        }
                        else
                        {
                            throw new MESReturnMessage($@"獲取拉手條年份失敗{var_year}");
                        }
                        O_PANELSSN.Value = "DM" + var_year + SN.Substring(SN.Length - 7, 7);
                    }
                    #endregion
                }

                if (var_sntype == "20")//這個類型不用
                {
                }
                if (var_sntype == "30")//這個類型不用
                {
                    O_PANELSSN.Value = SN;
                }
                // r_labelprint_pair  只有金雨生的一個機種，生成的數據還是錯的，不用了

                if (t_c_sku_detail.CheckExists(r_sn.SKUNO, "LABELTYPE", r_sn.NEXT_STATION, DB))
                {
                    c_sku_detail = t_c_sku_detail.LoadData(r_sn.SKUNO, "LABELTYPE", r_sn.NEXT_STATION, DB);
                    O_SNVERSION.Value = c_sku_detail.VERSION;
                 
                }

                sntypeall = "60,70";
                if (sntypeall.Contains(var_sntype))
                {
                    O_HWSKUNO.Value = r_wo_base.CUST_PN;
                    O_HWSKUVER.Value = r_wo_base.SKU_VER;
                    if (t_r_2d_sn_rma_t.IsExists(SN, DB))
                    {
                        O_HWSKUVER.Value = t_r_2d_sn_rma_t.LoadData(SN, DB).ITEM_VER;
                    }

                }

                if (var_sntype == "40")
                {
                    string PANELSSN = O_PANELSSN.Value.ToString();
                    if (PANELSSN.Length == 16)
                    {
                        O_PANELSSN.Value = PANELSSN.Substring(0, 6) + " " + PANELSSN.Substring(PANELSSN.Length - 10, 10);
                    }
                    if (PANELSSN.Length == 20)
                    {
                        O_PANELSSN.Value = PANELSSN.Substring(0, 10) + " " + PANELSSN.Substring(PANELSSN.Length - 10, 10);
                    }
                    O_SNVERSION.Value = c_sku_detail.VERSION;
                    O_BARCODE.Value = PANELSSN;
                }

                if (var_sntype == "50")
                {
                    string PANELSSN = O_PANELSSN.Value.ToString();
                    if (PANELSSN.Length == 16)
                    {
                        O_PANELSSN.Value = PANELSSN.Substring(0, 8) + " " + PANELSSN.Substring(PANELSSN.Length - 8, 8);
                    }
                    if (PANELSSN.Length == 20)
                    {
                        O_PANELSSN.Value = PANELSSN.Substring(0, 10) + " " + PANELSSN.Substring(PANELSSN.Length - 10, 10);
                    }
                    O_SNVERSION.Value = c_sku_detail.VERSION;//描述 例如:Y3 AND1CXPB
                    O_BARCODE.Value = PANELSSN;//二維條碼,內容為一行，值為SN
                }

                if (var_sntype == "60")//二維碼的模板變更為18*6             
                {
                    string var_custskunover = $@"{O_HWSKUNO.Value.ToString()}/{O_HWSKUVER.Value.ToString()}";
                    if (t_c_sku.IsExists(r_sn.SKUNO, DB))
                    {
                        c_sku = t_c_sku.GetBySKU(r_sn.SKUNO, DB);
                        O_CUSTDESC.Value = c_sku.DESCRIPTION; 
                    }
                    else
                    {
                        throw new MESReturnMessage($@"機種描述獲取錯誤[C_SKU]{r_sn.SKUNO}");
                    }

                    if (t_c_sku_detail.CheckExists(r_sn.SKUNO, "LABELTYPE", r_sn.NEXT_STATION, DB))
                    {
                        O_SNVERSION.Value = c_sku_detail.VERSION.Substring(0, 2);//上面已加載
                    }

                    string var_panelssn2d = $@"[)>_1E06_1DS{O_PANELSSN.Value.ToString()}_1D1P{O_HWSKUNO.Value.ToString()}_1D2P{O_HWSKUVER.Value.ToString()}_1D18VLEHWT_1E_04";

                    string PANELSSN = O_PANELSSN.Value.ToString();

                    O_SNVERSION.Value = c_sku_detail.VERSION;
                    O_BARCODE.Value = PANELSSN;

                    if (PANELSSN.Length == 12)
                    {
                        O_CUSTSKU.Value = var_custskunover;
                        O_PANELSSNNEW.Value = PANELSSN;
                    }

                    if (PANELSSN.Length == 16)
                    {
                        O_PANELSSN.Value = PANELSSN.Substring(0, 10);
                        O_PANELSSN.Value = PANELSSN.Substring(PANELSSN.Length - 6, 6);
                    }
                    if (PANELSSN.Length == 20)
                    {
                        O_PANELSSN.Value = PANELSSN.Substring(0, 10);
                        O_PANELSSN.Value = PANELSSN.Substring(PANELSSN.Length - 10, 10);
                    }

                }
                if (var_sntype == "70")//NNHWT 無此類機種 好多變量未賦值*6             
                {
                }
                if (var_sntype == "80")
                {
                    string var_custskunover = $@"{O_HWSKUNO.Value.ToString()}/{O_HWSKUVER.Value.ToString()}";
                    if (t_c_sku.IsExists(r_sn.SKUNO, DB))
                    {
                        c_sku = t_c_sku.GetBySKU(r_sn.SKUNO, DB);
                        O_CUSTDESC.Value = c_sku.DESCRIPTION;
                    }
                    else
                    {
                        throw new MESReturnMessage($@"機種描述獲取錯誤[C_SKU]{r_sn.SKUNO}");
                    }

                    if (t_c_sku_detail.CheckExists(r_sn.SKUNO, "LABELTYPE", r_sn.NEXT_STATION, DB))
                    {
                        O_SNVERSION.Value = c_sku_detail.VERSION.Substring(0, 2);//上面已加載
                    }

                    string PANELSSN = O_PANELSSN.Value.ToString();


                    if (PANELSSN.Length == 16)
                    {
                        O_PANELSSN1.Value = PANELSSN.Substring(0, 5);
                        O_PANELSSN2.Value = PANELSSN.Substring(PANELSSN.Length - 5, 6);
                        O_PANELSSN3.Value = PANELSSN.Substring(0, 12);
                    }
                    else
                    {
                    }


                }


            }
            else
            {
                throw new MESReturnMessage($@"此工站{r_sn.SKUNO},{r_sn.NEXT_STATION}未配置打印類型[C_SKU_DETAIL]");
            }
            #endregion

        }


    }
}
