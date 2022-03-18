using MESDataObject;
using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.Json;
using MESPubLab.MESStation;
using MESPubLab.MESStation.Label;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MESStation.Stations.StationActions.ActionRunners
{
    public class LabelPrintAction
    {
        /// <summary>
        /// 提供工站打印本站
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        /// 

        public static int countCIS;
        public static void PrintStationLabelAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            countCIS = 0;
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }


            string strSKU = Station.StationSession.Find(T => T.MESDataType == Paras[1].SESSION_TYPE && T.SessionKey == Paras[1].SESSION_KEY).Value.ToString();
            SKU SKU = new SKU();
            SKU.Init(strSKU, SFCDB, DB_TYPE_ENUM.Oracle);

            //找到當時在配置這個Action 的時候傳入的第三個參數所支持的處理 Lab 的類型
            List<string> ProcessLabType = new List<string>();
            if (Paras.Count > 2)
            {
                for (int i = 2; i < Paras.Count; i++)
                {
                    ProcessLabType.Add(Paras[i].VALUE.ToString());
                }
            }

            T_C_SKU_Label TCSL = new T_C_SKU_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            //獲取該機種所設置的 label 配置
            List<C_SKU_Label> labs = SFCDB.ORM.Queryable<C_SKU_Label>().Where((C) => C.SKUNO == SKU.SkuBase.SKUNO && C.STATION == Station.StationName).OrderBy((C) => C.LABELTYPE).OrderBy((C) => C.SEQ).ToList();
            //TCSL.GetLabelConfigBySkuStation(SKU.SkuBase.SKUNO, Station.StationName, SFCDB);

            //Add CIS for ODA-HA vince_20190826
            T_C_SERIES t_c_series = new T_C_SERIES(SFCDB, DB_TYPE_ENUM.Oracle);
            C_SERIES c_series = t_c_series.GetDetailById(SFCDB, SKU.CSeriesId);//sku.CSeriesId
            if (c_series.SERIES_NAME.Contains("ODA_HA"))
            {
                List<C_SKU_Label> labs_sub1 = SFCDB.ORM.Queryable<C_SKU_Label>().Where((C) => C.SKUNO == SKU.SkuBase.SKUNO && C.STATION == Station.StationName && C.LABELNAME.Contains("CIS")).OrderBy((C) => C.LABELTYPE).OrderBy((C) => C.SEQ).ToList();
                List<C_SKU_Label> labs_sub2 = SFCDB.ORM.Queryable<C_SKU_Label>().Where((C) => C.SKUNO == SKU.SkuBase.SKUNO && C.STATION == Station.StationName && C.LABELNAME.Contains("CIS")).OrderBy((C) => C.LABELTYPE).OrderBy((C) => C.SEQ).ToList();
                labs.AddRange(labs_sub1);
                labs.AddRange(labs_sub2);
            }//end

            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            //如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            //因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type
            for (int i = 0; i < labs.Count; i++)
            {
                if (ProcessLabType.Count > 0)
                {
                    if (!ProcessLabType.Contains(labs[i].LABELTYPE))
                    {
                        continue;
                    }
                }

                if (labs[i].LABELTYPE == "ORA_X7_2_CIS_P1")
                {
                    countCIS = countCIS + 1;
                }

                //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labs[i].LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labs[i].LABELTYPE, SFCDB);

                //添加NULL值報錯信息 Add By ZHB 2020年8月25日13:12:36
                if (RL == null)
                {
                    throw new System.Exception($@"Can't Get Label File By LabelName:{labs[i].LABELNAME}");
                }

                LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, SFCDB);
                    Lab = API_CLASS;
                }


                //給label的輸入變量加載值
                //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
                for (int j = 0; j < Lab.Inputs.Count; j++)
                {
                    if (Lab.Inputs[j].Name.ToUpper() == "STATION")
                    {
                        Lab.Inputs[j].Value = Station.StationName;
                    }



                    MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j].StationSessionType && T.SessionKey == Lab.Inputs[j].StationSessionKey);
                    if (S != null)
                    {
                        if (Lab.Inputs[j].Name.ToUpper() == "PRINTQTY")
                        {
                            labs[i].QTY = int.Parse(S.Value.ToString());
                        }
                        Lab.Inputs[j].Value = S.Value;
                    }
                }
                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = (int)labs[i].QTY;
                Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                try
                {
                    Lab.MakeLabel(SFCDB);
                }
                catch (Exception ee)
                {
                    throw new Exception(RL.LABELNAME + ":" + ee.Message);
                }
                var noprint = Lab.Outputs.Find(t => t.Name == "NotPrint" && t.Value.ToString() == "TRUE");
                if (noprint != null)
                {
                    continue;
                }


                List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                }
                Station.LabelPrints.Add(RL.R_FILE_NAME, pages);
            }
            ///    Station.CurrActionRrturn = StationActionReturn.PassStopRunNext;

        }

        /// <summary>
        /// 在線打印 Label
        /// 三個參數
        /// 第一個參數表示是否打印
        /// 第二個參數是機種對象
        /// 第三個參數表示是否沿用之前的模板打印
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PrintStillLabelAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count < 3)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057"));
            }
            OleExec SFCDB = Station.SFCDB;
            string Run = "";

            MESStationSession RunSession = Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY);
            if (RunSession != null)
            {
                Run = RunSession.Value.ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            else
            {
                return;
            }


            SKU SKU = (SKU)(Station.StationSession.Find(T => T.MESDataType == Paras[1].SESSION_TYPE && T.SessionKey == Paras[1].SESSION_KEY).Value);

            bool StillFlag = (Paras[2].VALUE.ToString()).ToUpper().Equals("TRUE");
            if (StillFlag)
            {
                //沿用之前的模板打印
                //Station.LabelStillPrint.Clear();
                Station.LabelStillPrint.AddRange(DoPrint(SKU, Station));
            }
            else
            {
                //不沿用之前的模板打印
                Station.LabelPrint.AddRange(DoPrint(SKU, Station));
            }
        }

        public static void DeleteLabelPrintCache(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            Station.LabelStillPrint.Clear();
            Station.LabelPrint.Clear();
            Station.LabelPrints.Clear();
        }

        /// <summary>
        /// 打印包裝單位
        /// PRINT 1
        /// SKU 1
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PrintPackAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }

            Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value = "FALSE";
            SKU sku = null;
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            sku = (SKU)SkuSession.Value;

            Station.LabelPrint.AddRange(DoPrint(sku, Station));

            //T_C_SKU_Label TCSL = new T_C_SKU_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            ////獲取該機種所設置的 label 配置
            //List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(sku.SkuNo, Station.StationName, SFCDB);

            //List<LabelBase> PrintLabs = new List<LabelBase>();
            //T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            //T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            ////如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            ////因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type
            //for (int i = 0; i < labs.Count; i++)
            //{

            //    //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
            //    R_Label RL = TRL.GetLabelByLabelName(labs[i].LABELNAME, SFCDB);
            //    C_Label_Type RC = TCLT.GetLabelTypeByName(labs[i].LABELTYPE, SFCDB);

            //    //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
            //    string path = System.AppDomain.CurrentDomain.BaseDirectory;
            //    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
            //    System.Type APIType = assembly.GetType(RC.CLASS);
            //    object API_CLASS = assembly.CreateInstance(RC.CLASS);

            //    LabelBase Lab = (LabelBase)API_CLASS;
            //    //給label的輸入變量加載值
            //    //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
            //    for (int j = 0; j < Lab.Inputs.Count; j++)
            //    {
            //        if (Lab.Inputs[j].Name.ToUpper() == "STATION")
            //        {
            //            Lab.Inputs[j].Value = Station.StationName;
            //        }

            //        MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j].StationSessionType && T.SessionKey == Lab.Inputs[j].StationSessionKey);
            //        if (S != null)
            //        {
            //            //Lab.Inputs[i].Value = S.Value;
            //            Lab.Inputs[j].Value = S.Value;
            //        }
            //    }
            //    Lab.LabelName = RL.LABELNAME;
            //    Lab.FileName = RL.R_FILE_NAME;
            //    Lab.PrintQTY = (int)labs[i].QTY;
            //    Lab.MakeLabel(SFCDB);
            //    List<LabelBase> pages = MakePrintPage(Lab, RL.ARRYLENGTH);

            //    for (int k = 0; k < pages.Count; k++)
            //    {
            //        pages[k].ALLPAGE = pages.Count;
            //        Station.LabelPrint.Add(pages[k]);
            //    }

            //}
        }

        /// <summary>
        /// 打印方法
        /// 如果 Parameters 參數有傳遞的話，那麼 Parameters 對象數組的第一個元素就認定是一個 Dictionary<string,object> 類型，代表的是要傳入到打印類裡面的參數
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="Station"></param>
        /// <param name="Parameters"></param>
        /// <returns></returns>
        public static List<LabelBase> DoPrint(SKU sku, MESStationBase Station, params object[] Parameters)
        {
            List<LabelBase> pages = null;
            T_C_SKU_Label TCSL = new T_C_SKU_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //獲取該機種所設置的 label 配置
            List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(sku.SkuNo, Station.StationName, Station.SFCDB);
            Dictionary<string, object> LabelParas = null;
            if (Parameters.Length > 0)
            {
                LabelParas = (Dictionary<string, object>)Parameters[0];
            }

            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            //如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            //因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type
            for (int i = 0; i < labs.Count; i++)
            {

                //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
                R_Label RL = TRL.GetLabelByLabelName(labs[i].LABELNAME, Station.SFCDB);
                C_Label_Type RC = TCLT.GetLabelTypeByName(labs[i].LABELTYPE, Station.SFCDB);

                ////根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                //string path = System.AppDomain.CurrentDomain.BaseDirectory;
                //Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                //System.Type APIType = assembly.GetType(RC.CLASS);
                //object API_CLASS = assembly.CreateInstance(RC.CLASS);

                //LabelBase Lab = (LabelBase)API_CLASS;

                LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, Station.SFCDB);
                    Lab = API_CLASS;
                }


                //給label的輸入變量加載值
                //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
                for (int j = 0; j < Lab.Inputs.Count; j++)
                {
                    if (Lab.Inputs[j].Name.ToUpper() == "STATION")
                    {
                        Lab.Inputs[j].Value = Station.StationName;
                    }

                    if (Parameters.Length > 0 && LabelParas.ContainsKey(Lab.Inputs[j].Name))
                    {
                        Lab.Inputs[j].Value = LabelParas[Lab.Inputs[j].Name];
                    }
                    else
                    {
                        MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j].StationSessionType && T.SessionKey == Lab.Inputs[j].StationSessionKey);
                        if (S != null)
                        {
                            //Lab.Inputs[i].Value = S.Value;
                            Lab.Inputs[j].Value = S.Value;
                        }
                    }
                }
                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = (int)labs[i].QTY;
                Lab.MakeLabel(Station.SFCDB);
                pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                    //Station.LabelPrint.Add(pages[k]);
                }

            }
            return pages;
        }

        public static void PrintTest(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            //LabelBase lb = new LabelBase();
            //lb.Outputs.Add(new LabelOutput { Name = "SN", Type = LabelOutPutTypeEnum.String, Value = "210223096921870V00B1", Description = "測試 SN" });
            //lb.Outputs.Add(new LabelOutput { Name = "WO", Type = LabelOutPutTypeEnum.String, Value = "002328000318", Description = "測試 WO" });
            //lb.Outputs.Add(new LabelOutput { Name = "SKU", Type = LabelOutPutTypeEnum.String, Value = "VT02130969", Description = "測試 Sku" });
            //lb.Outputs.Add(new LabelOutput { Name = "VER", Type = LabelOutPutTypeEnum.String, Value = "A5", Description = "測試 Ver" });
            //lb.Outputs.Add(new LabelOutput { Name = "S", Type = LabelOutPutTypeEnum.StringArry, Value = new string[] { "210233096921870V00B7", "210233096921870V00B8", "210233096921870V00B9", "210234096921870V00B1", "210233096921870V00B5" }, Description = "測試 SN List" });
            //lb.PAGE = 1;
            //lb.ALLPAGE = 1;
            //lb.LabelName = "ZGJTESTZBL";
            //lb.PrintQTY = 1;
            //lb.FileName = "ZGJTESTZBL.TXT";
            //Station.LabelPrint.Add(lb);

            string PackNo = Input.Value.ToString();
            T_R_Label RL = new T_R_Label(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            R_Label label = RL.GetLabelByLabelName("ZGJTEST", Station.SFCDB);
            T_C_Label_Type LT = new T_C_Label_Type(Station.SFCDB, DB_TYPE_ENUM.Oracle);
            C_Label_Type LabelType = LT.GetByName("ZGJTEST", Station.SFCDB);


            string path = System.AppDomain.CurrentDomain.BaseDirectory;
            Assembly assembly = Assembly.LoadFile(path + LabelType.DLL);
            System.Type APIType = assembly.GetType(LabelType.CLASS);
            object API_CLASS = assembly.CreateInstance(LabelType.CLASS);



            LabelBase Lab = (LabelBase)API_CLASS;

            Lab.Inputs.Find(t => t.StationSessionType == "PALLETNO" && t.StationSessionKey == "1").Value = PackNo;

            Lab.LabelName = label.LABELNAME;
            Lab.FileName = label.R_FILE_NAME;
            //C_SKU_LABEL 获得
            Lab.PrintQTY = 1;
            Lab.MakeLabel(Station.SFCDB);
            List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.GetLabelConfigByLabelName("ZGJTEST", Station.SFCDB).ARRYLENGTH);

            //for (int k = 0; k < pages.Count; k++)
            //{
            //    pages[k].ALLPAGE = pages.Count;
            //    Station.LabelPrint.AddRange(pages);
            //}
            Station.LabelPrint.AddRange(pages);
            //Station.LabelPrint.Add(Lab);

        }

        /// <summary>
        /// 補打指定工站及LABEL TYEP的LABEL
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintLabelAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession reprintStation = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (reprintStation == null || reprintStation.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession reprintLabelType = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (reprintLabelType == null || reprintLabelType.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession skuSession = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            MESStationSession labelInputSession = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (labelInputSession == null || labelInputSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + Paras[3].SESSION_KEY }));
            }

            OleExec db = Station.SFCDB;
            string _reprintStation = reprintStation.Value.ToString();
            string _reprintLabelType = reprintLabelType.Value.ToString();

            C_SKU skuObject = (C_SKU)skuSession.Value;
            Dictionary<string, string> labelInput = (Dictionary<string, string>)labelInputSession.Value;
            if (labelInput.ContainsKey("STATION"))
            {
                labelInput["STATION"] = _reprintStation;
            }
            else
            {
                labelInput.Add("STATION", _reprintStation);
            }
            T_C_SKU_Label TCSL = new T_C_SKU_Label(db, DB_TYPE_ENUM.Oracle);
            //獲取該機種所設置的 label 配置
            List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(skuObject.SKUNO, _reprintStation, db);

            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(db, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(db, DB_TYPE_ENUM.Oracle);
            //如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            //因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type
            for (int i = 0; i < labs.Count; i++)
            {
                if (_reprintLabelType.ToUpper() != "ALL" && !_reprintLabelType.Equals(labs[i].LABELTYPE.ToUpper()))// labs[i].LABELTYPE 转大写 2021 0626  chc
                {
                    continue;
                }
                //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labs[i].LABELNAME, db);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labs[i].LABELTYPE, db);

                ////根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                //string path = System.AppDomain.CurrentDomain.BaseDirectory;
                //Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                //System.Type APIType = assembly.GetType(RC.CLASS);
                //object API_CLASS = assembly.CreateInstance(RC.CLASS);

                //LabelBase Lab = (LabelBase)API_CLASS;

                LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, Station.SFCDB);
                    Lab = API_CLASS;
                }


                //給label的輸入變量加載值
                //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
                for (int j = 0; j < Lab.Inputs.Count; j++)
                {
                    foreach (var l in labelInput)
                    {
                        if (l.Key == Lab.Inputs[j].Name)
                        {
                            Lab.Inputs[j].Value = l.Value;
                        }
                    }
                }
                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrintQTY = (int)labs[i].QTY;
                Lab.MakeLabel(db);
                List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                    Station.LabelPrint.Add(pages[k]);
                }

            }

        }

        public static void PrintHWDLabBySkuAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession skuSession = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (skuSession == null || skuSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession printNumSession = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (printNumSession == null || printNumSession.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession currentSnSession = Station.StationSession.Find(s => s.MESDataType == "CurrentSN" && s.SessionKey == "1");
            if (currentSnSession == null)
            {
                currentSnSession = new MESStationSession() { MESDataType = "CurrentSN", SessionKey = "1", Value = null, InputValue = null, ResetInput = null };
                Station.StationSession.Add(currentSnSession);
            }

            OleExec DB = Station.SFCDB;
            try
            {
                SKU skuObject = (SKU)skuSession.Value;
                T_C_SKU_Label TCSL = new T_C_SKU_Label(DB, DB_TYPE_ENUM.Oracle);
                T_R_Label TRL = new T_R_Label(DB, DB_TYPE_ENUM.Oracle);
                T_C_Label_Type TCLT = new T_C_Label_Type(DB, DB_TYPE_ENUM.Oracle);
                int printNum = System.Int32.Parse(printNumSession.Value.ToString());
                //獲取該機種所設置的 label 配置
                List<C_SKU_Label> labs = TCSL.GetLabelConfigBySkuStation(skuObject.SkuNo, Station.StationName, DB);
                if (labs.Count == 0)
                {
                    //在該工站沒有LABEL
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20181101141954", new string[] { skuObject.SkuNo, Station.StationName }));
                }

                //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labs[0].LABELNAME, DB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labs[0].LABELTYPE, DB);

                //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                //string path = System.AppDomain.CurrentDomain.BaseDirectory;
                // Assembly assembly = Assembly.LoadFile(path + RC.DLL);

                List<LabelBase> labList = new List<LabelBase>();
                for (int i = 0; i < printNum; i++)
                {
                    //LabelBase Lab = (LabelBase)assembly.CreateInstance(RC.CLASS);

                    LabelBase Lab = null;
                    if (RC.DLL != "JSON")
                    {
                        //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                        string path = System.AppDomain.CurrentDomain.BaseDirectory;
                        Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                        System.Type APIType = assembly.GetType(RC.CLASS);
                        object API_CLASS = assembly.CreateInstance(RC.CLASS);
                        Lab = (LabelBase)API_CLASS;
                    }
                    else
                    {
                        var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, Station.SFCDB);
                        Lab = API_CLASS;
                    }

                    Lab.PAGE = 1;
                    Lab.ALLPAGE = 1;
                    Lab.LabelName = RL.LABELNAME;
                    Lab.FileName = RL.R_FILE_NAME;
                    Lab.PrintQTY = 1;
                    Lab.Inputs.Find(t => t.StationSessionType == Paras[0].SESSION_TYPE && t.StationSessionKey == Paras[0].SESSION_KEY).Value = skuObject.SkuNo;
                    OleExec printDB = Station.DBS["SFCDB"].Borrow();
                    Lab.MakeLabel(printDB);
                    if (printDB != null)
                    {
                        Station.DBS["SFCDB"].Return(printDB);
                    }
                    labList.Add(Lab);
                    //string slab = Newtonsoft.Json.JsonConvert.SerializeObject(Lab);
                    //string sn = Lab.Outputs.Find(l => l.Name == "SN").Value.ToString();
                    //T_R_MES_LOG t_r_mes_log = new T_R_MES_LOG(Station.SFCDB, Station.DBType);
                    //t_r_mes_log.InsertMESLog(Station.SFCDB, Station.BU, "PrintLab", "MESStation.Stations.StationActions.ActionRunners.LabelPrintAction",
                    //    "PrintHWDLabBySkuAction", sn, slab, "", Station.LoginUser.EMP_NO);
                }
                if (labList.Count > 0)
                {
                    currentSnSession.Value = labList.Last().Outputs.Find(o => o.Name == "SN").Value;
                }
                if (Station.LabelPrints.Keys.Contains("SN"))
                {
                    Station.LabelPrints["SN"] = labList;
                }
                else
                {
                    Station.LabelPrints.Add("SN", labList);
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }

        public static void PrintHWDPanel(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            string printType = "", left = "", height = "", temperature = "", labelName = "", isTest = "";
            bool isFor97a = false;

            MESStationSession printTypeSession = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            if (printTypeSession == null || printTypeSession.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession printQtySession = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (printQtySession == null || printQtySession.Value == null || printQtySession.Value.ToString() == "")
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession leftSession = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (leftSession != null)
            {
                left = leftSession.Value == null ? "" : leftSession.Value.ToString();
            }

            MESStationSession heightSession = Station.StationSession.Find(s => s.MESDataType == Paras[3].SESSION_TYPE && s.SessionKey == Paras[3].SESSION_KEY);
            if (heightSession != null)
            {
                height = heightSession.Value == null ? "" : heightSession.Value.ToString();
            }

            MESStationSession temperatureSession = Station.StationSession.Find(s => s.MESDataType == Paras[4].SESSION_TYPE && s.SessionKey == Paras[4].SESSION_KEY);
            if (temperatureSession != null)
            {
                temperature = temperatureSession.Value == null ? "" : temperatureSession.Value.ToString();
            }
            else
            {
                temperature = Paras[4].VALUE.ToString();
            }
            MESStationSession print79aSession = Station.StationSession.Find(s => s.MESDataType == Paras[5].SESSION_TYPE && s.SessionKey == Paras[5].SESSION_KEY);
            if (print79aSession != null && print79aSession.Value != null && print79aSession.Value.ToString() != "")
            {
                isFor97a = print79aSession.Value.ToString().ToUpper() == "YES" ? true : false;
            }
            else
            {
                isFor97a = Paras[5].VALUE.ToString().ToUpper() == "YES" ? true : false;
            }
            //MESStationSession printTestSession = Station.StationSession.Find(s => s.MESDataType == Paras[6].SESSION_TYPE && s.SessionKey == Paras[6].SESSION_KEY);
            //if (printTestSession != null && printTestSession.Value != null && printTestSession.Value.ToString() != "")
            //{
            //    isTest = printTestSession.Value.ToString().ToUpper();
            //}
            //else
            //{
            //    isTest = Paras[6].VALUE.ToString().ToUpper();
            //}

            isTest = Paras[6].VALUE.ToString().ToUpper();

            OleExec DB = Station.SFCDB;
            try
            {
                printType = printTypeSession.Value.ToString();

                if (isFor97a)
                {
                    labelName = "Panle_Label_97a";
                }
                else if (printType.ToUpper() == "CODESOFT")
                {
                    labelName = "Panel_Label_CodeSoft";
                }
                else if (printType.ToUpper().StartsWith("ZEBRA"))
                {
                    labelName = "Panel_Label_Zebra";
                }
                else
                {
                    throw new System.Exception("Get Label Name Fail!");
                }

                T_R_Label TRL = new T_R_Label(DB, DB_TYPE_ENUM.Oracle);
                T_C_Label_Type TCLT = new T_C_Label_Type(DB, DB_TYPE_ENUM.Oracle);
                int printNum = System.Int32.Parse(printQtySession.Value.ToString());
                string format = "";
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labelName, DB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labelName, DB);
                //string path = System.AppDomain.CurrentDomain.BaseDirectory;
                //Assembly assembly = Assembly.LoadFile(path + RC.DLL);

                if (printType == "Zebra_600DPI")
                {
                    format = "JMA";
                }
                else if (printType == "Zebra_300DPI")
                {
                    format = "JMB";
                }

                List<LabelBase> labList = new List<LabelBase>();
                for (int i = 0; i < printNum; i++)
                {
                    //LabelBase Lab = (LabelBase)assembly.CreateInstance(RC.CLASS);

                    LabelBase Lab = null;
                    if (RC.DLL != "JSON")
                    {
                        //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                        string path = System.AppDomain.CurrentDomain.BaseDirectory;
                        Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                        System.Type APIType = assembly.GetType(RC.CLASS);
                        object API_CLASS = assembly.CreateInstance(RC.CLASS);
                        Lab = (LabelBase)API_CLASS;
                    }
                    else
                    {
                        var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, Station.SFCDB);
                        Lab = API_CLASS;
                    }

                    Lab.PAGE = 1;
                    Lab.ALLPAGE = 1;
                    Lab.LabelName = RL.LABELNAME;
                    Lab.FileName = RL.R_FILE_NAME;
                    Lab.PrintQTY = 1;
                    Lab.Inputs.Find(t => t.StationSessionType == "Left" && t.StationSessionKey == "1").Value = left;
                    Lab.Inputs.Find(t => t.StationSessionType == "Height" && t.StationSessionKey == "1").Value = height;
                    Lab.Inputs.Find(t => t.StationSessionType == "Temperature" && t.StationSessionKey == "1").Value = temperature;
                    Lab.Inputs.Find(t => t.StationSessionType == "Format" && t.StationSessionKey == "1").Value = format;
                    Lab.Inputs.Find(t => t.StationSessionType == "IsTest" && t.StationSessionKey == "1").Value = isTest;

                    //Lab.MakeLabel(Station.SFCDB);
                    OleExec printDB = Station.DBS["SFCDB"].Borrow();
                    Lab.MakeLabel(printDB);
                    if (printDB != null)
                    {
                        Station.DBS["SFCDB"].Return(printDB);
                    }
                    labList.Add(Lab);
                }

                if (Station.LabelPrints.Keys.Contains("Panel"))
                {
                    Station.LabelPrints["Panel"] = labList;
                }
                else
                {
                    Station.LabelPrints.Add("Panel", labList);
                }

            }
            catch (System.Exception ex)
            {
                throw ex;
            }

        }


        /// <summary>
        /// 根據實際需要的輸入，收集用戶輸入達到指定數量后提交然後進行打印
        /// 如果沒有達到，則更新下一個輸入
        /// SKU 1   機種對象
        /// STATIONNAME 1   工站名
        /// CURRENTINPUT 1  當前輸入的值
        /// NEXTINPUT 1     下一個輸入的類型
        /// NEXTINPUTS 1    用來收集輸入的
        /// INPUTS 1        所有的輸入，用來判斷輸入的是否足夠
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ReprintAction(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            StringBuilder sb = new StringBuilder();
            T_R_REPRINT_RECORD TRRR = new T_R_REPRINT_RECORD(Station.SFCDB, Station.DBType);
            MESStationSession SkuSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SkuSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            SKU Sku = (SKU)SkuSession.Value;

            MESStationSession StationNameSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (StationNameSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }
            string StationName = StationNameSession.Value.ToString();

            MESStationSession CurrentInputSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (CurrentInputSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + " " + Paras[2].SESSION_KEY }));
            }
            string CurrentInput = CurrentInputSession.Value.ToString();

            MESStationSession NextInputTypeSession = Station.StationSession.Find(t => t.MESDataType == Paras[3].SESSION_TYPE && t.SessionKey == Paras[3].SESSION_KEY);
            if (NextInputTypeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[3].SESSION_TYPE + " " + Paras[3].SESSION_KEY }));
            }
            string NextInputType = NextInputTypeSession.Value.ToString();

            MESStationSession NextInputsSession = Station.StationSession.Find(t => t.MESDataType == Paras[4].SESSION_TYPE && t.SessionKey == Paras[4].SESSION_KEY);
            if (NextInputsSession == null)
            {
                NextInputsSession = new MESStationSession() { MESDataType = Paras[4].SESSION_TYPE, SessionKey = Paras[4].SESSION_KEY, Value = new Dictionary<string, object>() };
                Station.StationSession.Add(NextInputsSession);
            }
            if (((Dictionary<string, object>)NextInputsSession.Value).ContainsKey(NextInputType))
            {
                ((Dictionary<string, object>)NextInputsSession.Value)[NextInputType] = CurrentInput;
            }
            else
            {
                ((Dictionary<string, object>)NextInputsSession.Value).Add(NextInputType, CurrentInput);
            }

            MESStationSession InputsSession = Station.StationSession.Find(t => t.MESDataType == Paras[5].SESSION_TYPE && t.SessionKey == Paras[5].SESSION_KEY);
            if (InputsSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[5].SESSION_TYPE + " " + Paras[5].SESSION_KEY }));
            }
            DataTable InputsTable = (DataTable)InputsSession.Value;

            //達到了要求的輸入，則打印
            if (((Dictionary<string, object>)NextInputsSession.Value).Count >= InputsTable.Rows.Count)
            {
                object[] parameters = new object[] { (Dictionary<string, object>)NextInputsSession.Value };
                Station.StationName = StationName;
                Station.LabelPrint.AddRange(DoPrint(Sku, Station, parameters));
                //插入到補印記錄 R_REPRINT_RECORD 中
                if (parameters.Length > 0)
                {
                    foreach (string key in ((Dictionary<string, object>)NextInputsSession.Value).Keys)
                    {
                        sb.Append(key).Append(":").Append(((Dictionary<string, object>)NextInputsSession.Value)[key]).Append(",");
                    }
                    TRRR.AddReprintRecord(Sku.SkuNo, sb.ToString().Substring(0, sb.Length - 1), StationName, Station.LoginUser.EMP_NO, Station.SFCDB);
                }
                NextInputTypeSession.Value = InputsTable.Rows[0]["Name"].ToString();
                ((Dictionary<string, object>)NextInputsSession.Value).Clear();
            }
            //沒有達到要求的輸入，則提示下一個輸入
            else
            {
                for (int i = 0; i < InputsTable.Rows.Count; i++)
                {
                    if (InputsTable.Rows[i]["Name"].Equals(NextInputType))
                    {
                        NextInputTypeSession.Value = InputsTable.Rows[i + 1]["Name"].ToString();
                        break;
                    }
                }
            }
            Station.NextInput = Station.FindInputByName("Input");

        }

        ///add by Mala 02052020 for Oracle L11 
        public static void PrintStationCableLabelAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";

            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }


            SKU SKU = (SKU)(Station.StationSession.Find(T => T.MESDataType == Paras[1].SESSION_TYPE && T.SessionKey == Paras[1].SESSION_KEY).Value);

            //找到當時在配置這個Action 的時候傳入的第三個參數所支持的處理 Lab 的類型
            List<string> ProcessLabType = new List<string>();
            if (Paras.Count > 2)
            {
                for (int i = 2; i < Paras.Count; i++)
                {
                    ProcessLabType.Add(Paras[i].VALUE.ToString());
                }
            }

            T_C_SKU_Label TCSL = new T_C_SKU_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            //獲取該機種所設置的 label 配置
            List<C_SKU_Label> labs = SFCDB.ORM.Queryable<C_SKU_Label>().Where((C) => C.SKUNO == SKU.SkuBase.SKUNO && C.STATION == Station.StationName).OrderBy((C) => C.LABELTYPE).OrderBy((C) => C.SEQ).ToList();
            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            //如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            //因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type
            for (int i = 0; i < labs.Count; i++)
            {
                if (ProcessLabType.Count > 0)
                {
                    if (!ProcessLabType.Contains(labs[i].LABELTYPE))
                    {
                        continue;
                    }
                }

                //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(labs[i].LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(labs[i].LABELTYPE, SFCDB);

                //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                //string path = System.AppDomain.CurrentDomain.BaseDirectory;
                //Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                //System.Type APIType = assembly.GetType(RC.CLASS);

                int cablecount = 0;
                string RackSKUNO = SKU.SkuBase.SKUNO.Substring(0, 7);

                //Mala: 2/18/2020 for L11 Print status Log Table
                string label_type = "";
                string SNno = "default";
                int SEQno = 0;
                string EMP_NO = Station.LoginUser.EMP_NO;


                if (labs[i].LABELTYPE == "L11_ORACLE_RACK")
                {
                    //Mala: 2/18/2020 for L11 Print status Log Table
                    label_type = "RACK";
                    string strCableQL = $" select * from SFCBASE.C_L11_RACK_CABLE where partno = '{RackSKUNO}' ";
                    DataTable rackdt = SFCDB.ExecSelect(strCableQL).Tables[0];
                    cablecount = rackdt.Rows.Count;
                }
                else if (labs[i].LABELTYPE == "L11_ORACLE_POWER")
                {
                    //Mala: 2/18/2020 for L11 Print status Log Table
                    label_type = "POWER";
                    string strCableQL = $" select * from SFCBASE.C_L11_POWER_CABLE where partno = '{RackSKUNO}' ";
                    DataTable rackdt = SFCDB.ExecSelect(strCableQL).Tables[0];
                    cablecount = rackdt.Rows.Count;
                }



                for (int j = 0; j < cablecount; j++)
                {
                    LabelBase Lab = null;
                    if (RC.DLL != "JSON")
                    {
                        //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                        string path = System.AppDomain.CurrentDomain.BaseDirectory;
                        Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                        System.Type APIType = assembly.GetType(RC.CLASS);
                        object API_CLASS = assembly.CreateInstance(RC.CLASS);
                        Lab = (LabelBase)API_CLASS;
                    }
                    else
                    {
                        var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, Station.SFCDB);
                        Lab = API_CLASS;
                    }
                    //給label的輸入變量加載值
                    //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
                    for (int j1 = 0; j1 < Lab.Inputs.Count; j1++)
                    {

                        MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[0].StationSessionType && T.SessionKey == Lab.Inputs[0].StationSessionKey);
                        if (S != null)
                        {
                            // first input Rack SN
                            Lab.Inputs[j1].Value = S.Value;
                            SNno = S.Value.ToString();
                        }

                        //second input Rack SKU
                        if (Lab.Inputs[j1].Name.ToUpper() == "SKU")
                        {
                            Lab.Inputs[j1].Value = RackSKUNO;
                        }
                        //third output SEQ
                        if (Lab.Inputs[j1].Name.ToUpper() == "SEQ")
                        {
                            Lab.Inputs[j1].Value = j;
                            SEQno = j;
                        }
                        //fourth input Cable Type
                        if (Lab.Inputs[j1].Name.ToUpper() == "LABEL_TYPE")
                        {
                            Lab.Inputs[j1].Value = label_type;
                        }

                    }
                    Lab.LabelName = RL.LABELNAME;
                    Lab.FileName = RL.R_FILE_NAME;
                    Lab.PrintQTY = (int)labs[i].QTY;
                    Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                    Lab.MakeLabel(SFCDB);
                    List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

                    for (int k = 0; k < pages.Count; k++)
                    {
                        pages[k].ALLPAGE = pages.Count;
                        Station.LabelPrint.Add(pages[k]);
                    }


                    //Mala: 2/18/2020 for L11 Print status Log Table
                    string sqlSelectLog = $"Select* from SFCRUNTIME.R_L11_PRINT_LOG where SN = '{SNno}' and LAB_TYPE='{label_type}'";

                    DataTable sqlSelectLogData = SFCDB.ExecSelect(sqlSelectLog).Tables[0];

                    int dataexistcount = sqlSelectLogData.Rows.Count;
                    if (dataexistcount == 0)
                    {

                        string sqlInsertLog = $"INSERT INTO SFCRUNTIME.R_L11_PRINT_LOG(SN, SEQ, LAB_TYPE,EDIT_TIME,EDIT_EMP) VALUES('{SNno}',{SEQno},'{label_type}',SYSDATE,'{EMP_NO}')";
                        SFCDB.ExecSQL(sqlInsertLog);

                    }
                    else
                    {
                        string sqlUpdatetLog = $"UPDATE SFCRUNTIME.R_L11_PRINT_LOG SET SEQ='{SEQno}',EDIT_TIME=SYSDATE,EDIT_EMP='{EMP_NO}' WHERE SN='{SNno}' and LAB_TYPE='{label_type}'";
                        SFCDB.ExecSQL(sqlUpdatetLog);
                    }


                }

            }
        }
        ///end by Mala 02052020
        ///
           //Mala 02/10/2020: Reprint L11 Rack and Power Label
        public static void RePrintL11CableLabelAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;

            if (Paras.Count != 4)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession cablesku = Station.StationSession.Find(s => s.MESDataType == Paras[0].SESSION_TYPE && s.SessionKey == Paras[0].SESSION_KEY);
            string strSkuQL = $" select * from SFCBASE.C_L11_RACK_CABLE where partno = '{cablesku.Value}'";
            DataTable dt2 = SFCDB.ExecSelect(strSkuQL).Tables[0];
            //if (cablesku == null || cablesku.Value == null)
            if (dt2.Rows.Count == 0)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }

            MESStationSession cableseq = Station.StationSession.Find(s => s.MESDataType == Paras[1].SESSION_TYPE && s.SessionKey == Paras[1].SESSION_KEY);
            if (cableseq == null || cableseq.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }

            MESStationSession cablelabletype = Station.StationSession.Find(s => s.MESDataType == Paras[2].SESSION_TYPE && s.SessionKey == Paras[2].SESSION_KEY);
            if (cablelabletype == null || cablelabletype.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY }));
            }

            //SKU SKU = (SKU)(Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value);



            string _cablesku = cablesku.Value.ToString();
            //string _cablesku = SKU.SkuBase.SKUNO;
            string _cableseq = cableseq.Value.ToString();
            string _cablelabletype = cablelabletype.Value.ToString();
            string _cable_labelname = Paras[3].VALUE;

            //Mala_02272020 to facilitate user to enter first 7 digits of SKU as an input
            //List<C_SKU_Label> labs = SFCDB.ORM.Queryable<C_SKU_Label>().Where((C) => C.SKUNO == SKU.SkuBase.SKUNO && C.STATION == Station.StationName).OrderBy((C) => C.LABELTYPE).OrderBy((C) => C.SEQ).ToList();
            List<C_SKU_Label> labs = SFCDB.ORM.Queryable<C_SKU_Label>().Where((C) => C.SKUNO.Contains(_cablesku) && C.STATION == Station.StationName).OrderBy((C) => C.LABELTYPE).OrderBy((C) => C.SEQ).ToList();

            //根據 C_SKU_LABEL 的 LabelName 去 R_Label 中獲取該機種對應的 Label 模板文件的文件名
            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            Row_R_Label RL = TRL.GetLabelConfigByLabelName(_cable_labelname, SFCDB);
            Row_C_Label_Type RC = TCLT.GetConfigByName(_cable_labelname, SFCDB);

            ////根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
            //string path = System.AppDomain.CurrentDomain.BaseDirectory;
            //Assembly assembly = Assembly.LoadFile(path + RC.DLL);
            //System.Type APIType = assembly.GetType(RC.CLASS);

            //object API_CLASS = assembly.CreateInstance(RC.CLASS);

            //LabelBase Lab = (LabelBase)API_CLASS;

            LabelBase Lab = null;
            if (RC.DLL != "JSON")
            {
                //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                System.Type APIType = assembly.GetType(RC.CLASS);
                object API_CLASS = assembly.CreateInstance(RC.CLASS);
                Lab = (LabelBase)API_CLASS;
            }
            else
            {
                var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, Station.SFCDB);
                Lab = API_CLASS;
            }

            //給label的輸入變量加載值
            //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
            for (int j1 = 0; j1 < Lab.Inputs.Count; j1++)
            {


                //First input LabelBase cable type
                if (Lab.Inputs[j1].Name.ToUpper() == "SKU")
                {
                    ////Mala_02272020 to facilitate user to enter first 7 digits of SKU as an input
                    Lab.Inputs[j1].Value = _cablesku;
                    //Lab.Inputs[j1].Value = _cablesku.Substring(0, 7);
                }
                //2nd input LabelBase SEQ
                if (Lab.Inputs[j1].Name.ToUpper() == "CABLETYPE")
                {
                    Lab.Inputs[j1].Value = _cablelabletype;
                }

                //3rd input LabelBase SEQ
                if (Lab.Inputs[j1].Name.ToUpper() == "SEQ")
                {
                    Lab.Inputs[j1].Value = _cableseq;
                }
            }
            Lab.LabelName = RL.LABELNAME;
            Lab.FileName = RL.R_FILE_NAME;
            Lab.PrintQTY = 1;
            Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
            Lab.MakeLabel(SFCDB);
            List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

            for (int k = 0; k < pages.Count; k++)
            {
                pages[k].ALLPAGE = pages.Count;
                Station.LabelPrint.Add(pages[k]);
            }

        }

        //Mala 02/10/2020 END: Reprint L11 Rack and Power Label  


        /// <summary>
        ///Reprint Off Line Station 
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PrintOffLineStationLabelAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            string LabelName, FileName;
            try
            {
                Run = (Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY).Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            catch
            {

            }


            SKU SKU = (SKU)(Station.StationSession.Find(T => T.MESDataType == Paras[1].SESSION_TYPE && T.SessionKey == Paras[1].SESSION_KEY).Value);

            MESStationSession labelNews = Station.StationSession.Find(t => t.MESDataType == "STATION");

            if (labelNews == null || labelNews.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
            }
            //找到當時在配置這個Action 的時候傳入的第三個參數所支持的處理 Lab 的類型
            List<string> ProcessLabType = new List<string>();
            if (Paras.Count > 2)
            {
                for (int i = 2; i < Paras.Count; i++)
                {
                    ProcessLabType.Add(Paras[i].VALUE.ToString());
                }
            }
            string str = labelNews.Value.ToString();
            string[] arr = arr = str.Split(',');
            if (arr.Count() == 2)
            {
                LabelName = arr[0].ToString(); ;
                FileName = arr[1].ToString(); ;

            }
            else
            {

                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150555"));
            }

            T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            //如果當時在配置這個 Action 時傳入的第三個參數包含從數據庫讀取的機種的 label 配置中的 labeltype 則繼續
            //因此在配置這個 Action 的時候需要實際考慮該機種的配置，並且只需要配置第三個傳入參數的 value 屬性即可，不需要 session_key 和 session_type


            Row_R_Label RL = TRL.GetLabelConfigByLabelName(LabelName, SFCDB);
            //Row_C_Label_Type RC = TCLT.GetConfigByName("OFFLINE_STATION", SFCDB);

            var skuLabel = SFCDB.ORM.Queryable<C_SKU_Label>().Where(r => r.STATION == Station.StationName && r.SKUNO == SKU.SkuNo && r.LABELNAME == LabelName).ToList().FirstOrDefault();
            if (skuLabel == null)
            {
                throw new MESReturnMessage($@"{SKU.SkuNo},{Station.StationName},{LabelName},no label");
            }
            if (string.IsNullOrWhiteSpace(skuLabel.LABELTYPE))
            {
                throw new MESReturnMessage($@"{SKU.SkuNo},{Station.StationName},{LabelName},label type is null");
            }
            Row_C_Label_Type RC = TCLT.GetConfigByName(skuLabel.LABELTYPE, SFCDB);

            LabelBase Lab = null;
            if (RC.DLL != "JSON")
            {
                //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                System.Type APIType = assembly.GetType(RC.CLASS);
                object API_CLASS = assembly.CreateInstance(RC.CLASS);
                Lab = (LabelBase)API_CLASS;
            }
            else
            {
                var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, SFCDB);
                Lab = API_CLASS;
            }


            //給label的輸入變量加載值
            //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
            for (int j = 0; j < Lab.Inputs.Count; j++)
            {
                if (Lab.Inputs[j].Name.ToUpper() == "STATION")
                {
                    Lab.Inputs[j].Value = Station.StationName;
                }

                MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j].StationSessionType && T.SessionKey == Lab.Inputs[j].StationSessionKey);
                if (S != null)
                {
                    if (Lab.Inputs[j].Name.ToUpper() == "PRINTQTY" || Lab.Inputs[j].Name.ToUpper() == "QTY")
                    {
                        skuLabel.QTY = int.Parse(S.Value.ToString());
                    }
                    Lab.Inputs[j].Value = S.Value;
                }
            }
            //MESStationSession sessionQty= QTY

            //Lab.LabelName = RL.LABELNAME;
            //Lab.FileName = RL.R_FILE_NAME;
            Lab.LabelName = LabelName;
            Lab.FileName = FileName;
            Lab.PrintQTY = (int)skuLabel.QTY; ;
            Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
            Lab.MakeLabel(SFCDB);
            List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

            for (int k = 0; k < pages.Count; k++)
            {
                pages[k].ALLPAGE = pages.Count;
                Station.LabelPrint.Add(pages[k]);
            }

        }

        public static void PrintOffLineLabelAndSaveLog(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string labelType = "", labelName = "", fileName = "", printQty = "";
            MESStationSession sessionLabel = Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY);
            if (sessionLabel == null || sessionLabel.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY }));
            }
            MESStationSession sessionQty = Station.StationSession.Find(T => T.MESDataType == Paras[1].SESSION_TYPE && T.SessionKey == Paras[1].SESSION_KEY);
            if (sessionQty == null || sessionQty.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY }));
            }

            string[] labelArry = sessionLabel.Value.ToString().Split(',');
            if (labelArry.Count() == 4)
            {
                labelType = labelArry[0].ToString();
                labelName = labelArry[1].ToString();
                fileName = labelArry[2].ToString();
                printQty = labelArry[3].ToString();
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814150555"));
            }
            if (string.IsNullOrWhiteSpace(labelType) || string.IsNullOrWhiteSpace(labelName) || string.IsNullOrWhiteSpace(fileName))
            {
                throw new MESReturnMessage("Label info error");
            }

            R_Label labelObj = SFCDB.ORM.Queryable<R_Label>()
                .Where(r => SqlSugar.SqlFunc.ToUpper(r.LABELNAME) == SqlSugar.SqlFunc.ToUpper(labelName)).ToList().FirstOrDefault();

            C_Label_Type labelTypeObj = SFCDB.ORM.Queryable<C_Label_Type>()
                .Where(r => SqlSugar.SqlFunc.ToUpper(r.NAME) == SqlSugar.SqlFunc.ToUpper(labelType)).ToList().FirstOrDefault();

            LabelBase labelBase = null;
            if (labelTypeObj.DLL != "JSON")
            {
                //根據 C_SKU_LABEL 獲取到該機種對應的 LabelType 屬性，根據 LabelType 去 C_LABEL_TYPE 中獲取對應的處理邏輯所在的類并通過反射進行加載調用
                string path = System.AppDomain.CurrentDomain.BaseDirectory;
                Assembly assembly = Assembly.LoadFile(path + labelTypeObj.DLL);
                Type APIType = assembly.GetType(labelTypeObj.CLASS);
                object API_CLASS = assembly.CreateInstance(labelTypeObj.CLASS);
                labelBase = (LabelBase)API_CLASS;
            }
            else
            {
                var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(labelTypeObj.CLASS, SFCDB);
                labelBase = API_CLASS;
            }

            //給label的輸入變量加載值
            //給即將要被調用的打印方法傳遞輸入參數，構造 Label 對象
            for (int j = 0; j < labelBase.Inputs.Count; j++)
            {
                if (labelBase.Inputs[j].Name.ToUpper() == "STATION")
                {
                    labelBase.Inputs[j].Value = Station.StationName;
                }
                else if (labelBase.Inputs[j].Name.ToUpper() == "EMP_NO")
                {
                    labelBase.Inputs[j].Value = Station.LoginUser.EMP_NO;
                }
                else
                {
                    MESStationSession S = Station.StationSession.Find(T => T.MESDataType == labelBase.Inputs[j].StationSessionType && T.SessionKey == labelBase.Inputs[j].StationSessionKey);
                    if (S != null)
                    {
                        labelBase.Inputs[j].Value = S.Value;
                    }
                }
            }

            labelBase.LabelName = labelName;
            labelBase.FileName = fileName;
            labelBase.PrintQTY = int.Parse(printQty);
            labelBase.PrinterIndex = int.Parse(labelObj.PRINTTYPE);
            labelBase.MakeLabel(SFCDB);

            List<LabelBase> pages = LabelBase.MakePrintPage(labelBase, labelObj.ARRYLENGTH);
            //for (int k = 0; k < pages.Count; k++)
            //{
            //    pages[k].ALLPAGE = pages.Count;
            //    Station.LabelPrint.Add(pages[k]);
            //}

            for (int k = 0; k < pages.Count; k++)
            {
                pages[k].ALLPAGE = pages.Count;
            }
            Station.LabelPrints.Add(labelObj.R_FILE_NAME, pages);
        }


        /// <summary>
        /// Print Label For Offline Station Without SKU Label Config
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void PrintOfflineStationLabelWithoutSkuConfigAction(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec SFCDB = Station.SFCDB;
            string Run = "";
            if (Paras.Count < 2)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { }));
            }
            MESStationSession ControlFlagSession = Station.StationSession.Find(T => T.MESDataType == Paras[0].SESSION_TYPE && T.SessionKey == Paras[0].SESSION_KEY);
            if (ControlFlagSession != null && ControlFlagSession.Value != null)
            {
                Run = (ControlFlagSession.Value).ToString();
                if (Run.ToUpper() == "FALSE")
                {
                    return;
                }
            }
            var Labels = Paras.FindAll(t => t.SESSION_TYPE == "PrintLabel").Select(t => new { LABELNAME = t.VALUE, LABELTYPE = t.SESSION_KEY }).ToList();
            for (int i = 0; i < Labels.Count; i++)
            {

            }
            List<LabelBase> PrintLabs = new List<LabelBase>();
            T_R_Label TRL = new T_R_Label(SFCDB, DB_TYPE_ENUM.Oracle);
            T_C_Label_Type TCLT = new T_C_Label_Type(SFCDB, DB_TYPE_ENUM.Oracle);
            for (int i = 0; i < Labels.Count; i++)
            {
                Row_R_Label RL = TRL.GetLabelConfigByLabelName(Labels[i].LABELNAME, SFCDB);
                Row_C_Label_Type RC = TCLT.GetConfigByName(Labels[i].LABELTYPE, SFCDB);
                if (RL == null)
                {
                    throw new System.Exception($@"Can't Get Label File By LabelName:{Labels[i].LABELNAME}");
                }

                LabelBase Lab = null;
                if (RC.DLL != "JSON")
                {
                    string path = System.AppDomain.CurrentDomain.BaseDirectory;
                    Assembly assembly = Assembly.LoadFile(path + RC.DLL);
                    System.Type APIType = assembly.GetType(RC.CLASS);
                    object API_CLASS = assembly.CreateInstance(RC.CLASS);
                    Lab = (LabelBase)API_CLASS;
                }
                else
                {
                    var API_CLASS = JsonSave.GetFromDB<ConfigableLabelBase>(RC.CLASS, SFCDB);
                    Lab = API_CLASS;
                }

                for (int j = 0; j < Lab.Inputs.Count; j++)
                {
                    if (Lab.Inputs[j].Name.ToUpper() == "STATION")
                    {
                        Lab.Inputs[j].Value = Station.StationName;
                    }

                    MESStationSession S = Station.StationSession.Find(T => T.MESDataType == Lab.Inputs[j].StationSessionType && T.SessionKey == Lab.Inputs[j].StationSessionKey);
                    if (S != null)
                    {
                        if (Lab.Inputs[j].Name.ToUpper() == "PRINTQTY")
                        {
                            Lab.PrintQTY = int.Parse(S.Value.ToString());
                        }
                        Lab.Inputs[j].Value = S.Value;
                    }
                    else
                    {
                        throw new System.Exception($@"Can't Get Label Input Session:{ Lab.Inputs[j].StationSessionType + Lab.Inputs[j].StationSessionKey}");
                    }
                }

                Lab.LabelName = RL.LABELNAME;
                Lab.FileName = RL.R_FILE_NAME;
                Lab.PrinterIndex = int.Parse(RL.PRINTTYPE);
                try
                {
                    Lab.MakeLabel(SFCDB);
                }
                catch (Exception ee)
                {
                    throw new Exception(RL.LABELNAME + ":" + ee.Message);
                }
                var noprint = Lab.Outputs.Find(t => t.Name == "NotPrint" && t.Value.ToString() == "TRUE");
                if (noprint != null)
                {
                    continue;
                }


                List<LabelBase> pages = LabelBase.MakePrintPage(Lab, RL.ARRYLENGTH);

                for (int k = 0; k < pages.Count; k++)
                {
                    pages[k].ALLPAGE = pages.Count;
                }
                Station.LabelPrints.Add(RL.R_FILE_NAME, pages);
            }
        }

    }
}
