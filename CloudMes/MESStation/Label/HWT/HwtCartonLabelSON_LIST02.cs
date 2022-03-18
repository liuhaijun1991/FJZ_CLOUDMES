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
        /// ADD BY HGB 2019.07.17
        /// </summary>
        class HwtCartonLabelSON_LIST02 : LabelBase
        {
            #region 定義輸出

            LabelInputValue I_SN = new LabelInputValue() { Name = "SN", Type = "STRING", Value = "", StationSessionType = "SN", StationSessionKey = "1" };
            LabelInputValue I_CAROTON = new LabelInputValue() { Name = "CAROTON", Type = "STRING", Value = "", StationSessionType = "PRINT_CTN", StationSessionKey = "1" };
            LabelInputValue I_STATION = new LabelInputValue() { Name = "STATION", Type = "STRING", Value = "", StationSessionType = "STATION", StationSessionKey = "1" };
         
            LabelOutput O_SN = new LabelOutput() { Name = "SN", Type = LabelOutPutTypeEnum.StringArry, Description = "", Value = new List<string>() };
          
            #endregion 定義輸出

            public HwtCartonLabelSON_LIST02()
            {
                Inputs.Add(I_SN);
                Inputs.Add(I_CAROTON);//未用
                Inputs.Add(I_STATION);//未用
             
                Outputs.Add(O_SN); 
            }


        /// <summary>
        /// ADD BY HGB 2019.07.17
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
                 

            #region 獲取SNLIST 和MAC LIST
            T_R_PACKING t_r_packing = new T_R_PACKING(DB, DB_TYPE_ENUM.Oracle);
            try
                {
                    List<string> SnList = new List<string>();
                    t_r_packing.GetSNListByPackNoForThirdlabel(I_CAROTON.Value.ToString(), ref SnList, DB);
                    O_SN.Value = SnList;

                    //List<string> MacList = new List<string>();
                    //t_r_packing.GetSN_MacListByPackNo(I_CAROTON.Value.ToString(), ref MacList, DB);
                    //O_MAC.Value = MacList;
                }
                catch
                {
                    throw new System.Exception("獲取卡通箱號SN列表[R_PACKING]," + I_CAROTON.Value.ToString());
                }

                #endregion 獲取SNLIST 和MAC LIST
            }


        }
    }

 
