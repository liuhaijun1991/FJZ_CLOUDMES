using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.HWT;
using MESDBHelper;
using MESPubLab.MESStation;
using MESPubLab.MESStation.MESReturnView.Station;
using MESStation.LogicObject;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Linq;

namespace MESStation.Stations.StationActions.DataCheckers
{
    class CheckInputData
    {
        /// <summary>
        /// 檢查投入的SN的條碼規則,投入SN可以用L0009加載
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,SN規則檢查，SKU保存的位置</param>
        public static void SNRuleDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objsku;
            bool SnRuleFlag = false;
            string regexstr = string.Empty;
            string Getnewsn = string.Empty;
            if (Paras.Count < 2)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession Ssku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Ssku == null)
            {
                //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }
            else
            {
                if (Ssku.Value != null)
                {
                    objsku = (SKU)Ssku.Value;
                    if (objsku != null)
                    {
                        regexstr = objsku.SnRule;
                    }
                }
                else
                {
                    //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114357", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY, Paras[0].VALUE }));
                }
            }

            MESStationSession Snewsn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Snewsn == null)
            {
                //throw new Exception("Can Not Find " + Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY + " !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { Paras[1].SESSION_TYPE, Paras[1].SESSION_KEY }));
            }
            else
            {
                if (Snewsn.Value != null)
                {
                    Getnewsn = Snewsn.Value.ToString();
                }
                else
                {
                    //throw new Exception("Can Not Find " + Paras[1].SESSION_TYPE + " " + Paras[1].SESSION_KEY + "" + Paras[1].VALUE + " !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114357", new string[] { Paras[1].SESSION_TYPE, Paras[1].SESSION_KEY, Paras[1].VALUE }));
                }
            }

            string modeltype = "";
            if (Paras.Count > 2)
            {
                MESStationSession Smodeltype = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
                if (Smodeltype != null)
                {
                    if (Smodeltype.Value != null)
                    {
                        modeltype = Smodeltype.Value.ToString();
                    }
                }
            }

            if (string.IsNullOrEmpty(regexstr))
            {
                //throw new Exception("SnRule Is Null or Empty!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114509"));

            }
            else
            {
                if (modeltype.IndexOf("115") >= 0) //如果有設定ModelType:115,則使用舊MBD SFC的條碼規則檢查邏輯檢查條碼.
                {
                    //MBD與其它BU存在編碼定義區間差異(待確認),不能直接使用該方法.
                    //T_C_SN_RULE C_SN_RULE = new T_C_SN_RULE(Station.SFCDB, DB_TYPE_ENUM.Oracle);
                    //SnRuleFlag = C_SN_RULE.CheckSNRule(Getnewsn, regexstr, Station.SFCDB);
                    SnRuleFlag = true;
                    if (Getnewsn.Length != regexstr.Length)
                    {
                        //條碼錯誤,長度不符
                        SnRuleFlag = false;
                    }
                    else
                    {
                        char[] ruleAry = new char[regexstr.Length];
                        ruleAry = regexstr.ToArray();
                        char[] checksnAry = new char[Getnewsn.Length];
                        checksnAry = Getnewsn.ToArray();

                        for (int i = 0; i < ruleAry.Length; i++)
                        {

                            if (ruleAry[i] == '#')//0~9
                            {
                                if (!(checksnAry[i] >= 48 && checksnAry[i] <= 57))
                                {
                                    //條碼錯誤,第[i]位必須是數字
                                    SnRuleFlag = false;
                                    break;
                                }
                            }
                            else if (ruleAry[i] == '!')//A~Z
                            {
                                if (!(checksnAry[i] >= 65 && checksnAry[i] <= 90))
                                {
                                    //條碼錯誤,第[i]位必須是字符
                                    SnRuleFlag = false;
                                    break;
                                }
                            }
                            else if (ruleAry[i] == '*')//0~9 or A~Z
                            {
                                if (!((checksnAry[i] >= 48 && checksnAry[i] <= 57) || (checksnAry[i] >= 65 && checksnAry[i] <= 90)))
                                {
                                    //條碼錯誤,第[i]位必須是數字或字符
                                    SnRuleFlag = false;
                                    break;
                                }
                            }
                            else
                            {
                                if (ruleAry[i] != checksnAry[i])
                                {
                                    //條碼錯誤,第[i]位必須是ruleAry[i]
                                    SnRuleFlag = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    SnRuleFlag = Regex.IsMatch(Getnewsn, regexstr);
                }
            }

            if (SnRuleFlag)
            {
                Station.AddMessage("MES00000059", new string[] { Getnewsn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { Getnewsn, regexstr }));
                //Station.AddMessage("MES00000058", new string[] { Getnewsn, regexstr }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
            }
        }

        /// <summary>
        /// 兩個參數：
        /// 1.SKU 對象
        /// 2.所輸入值的類型
        /// 檢查SN規則，非精確檢查，主要思想是存在即合理
        /// （1）遇到前綴直接查看SN是否以指定前綴開頭，如果是則將SN的頭部截去進行後面的判斷
        /// （2）遇到YYYY/MM/DD/WW等，從輸入SN中截取設定長度的字符串，再去該條規則對應的 C_CODE_MAPPING 中查找是否有該字符串的值，例如當前規則是 WEEK_10_2，那麼去
        /// C_CODE_MAPPING中可以找到52筆記錄，所以如果從SN中截取2位得到的字符串可以在52筆記錄中找到，那麼表示這是正確的，因此將SN從頭截去2位之後再次判斷
        /// （3）遇到SN流水碼這樣的規則，先比較長度，如果長度一樣，則再比較流水碼中每一位是否在 C_CODE_MAPPING 中是否存在從而判斷流水碼是否合法，最後還是將SN從頭部去掉
        /// 流水碼長度之後再進入後面的判斷
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InaccurateSNRuleChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objSku;
            bool snRuleFlag = true;
            string skuConfigRule = string.Empty;
            string inputSn = string.Empty;
            string[] ruleArray;
            string[] snArray;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); 
            }

            MESStationSession sessionInputType = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if(sessionInputType==null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            var inputType = sessionInputType.Value.ToString();
            

            try
            {
                objSku = (SKU)sessionSku.Value;
                if (objSku != null)
                {
                    if (inputType.Equals("SN") || inputType.Equals("PANEL"))
                    {
                        skuConfigRule = inputType.Equals("SN") ? objSku.SnRule : objSku.SkuBase.PANEL_RULE;
                    }
                    else if (inputType.Contains("CARTON") || inputType.Contains("PALLET"))
                    {
                        T_C_PACKING TCP = new T_C_PACKING(Station.SFCDB, Station.DBType);
                        var Packing = TCP.GetPackingBySkuAndType(objSku.SkuNo, inputType, Station.SFCDB);
                        skuConfigRule = Packing.SN_RULE;
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                inputSn = Input.Value.ToString();
                //如果 RuleName 是 C_SN_RULE 里配置的规则,就按照该规则检查.Tiny 2020-02-18
                //否则使用简易规则
                var Rule = Station.SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.NAME == skuConfigRule).First();
                if (Rule != null)
                {

                    if (MESPubLab.MESStation.SNMaker.SNmaker.CheckSnRuleInaccurately(inputSn, skuConfigRule, Station.SFCDB))
                    //检查不通过会报异常,不用处理
                    {
                        return;
                    }
                }

                if (string.IsNullOrEmpty(skuConfigRule))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000251"));
                }

                if (inputSn.Length != skuConfigRule.Length)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000252", new string[] { skuConfigRule }));
                }
                char[] charConfigRule = skuConfigRule.ToCharArray();
                ruleArray = new string[charConfigRule.Length];
                for (int i = 0; i < charConfigRule.Length; i++)
                {
                    ruleArray[i] = charConfigRule[i].ToString();
                }

                char[] charSn = inputSn.ToCharArray();
                snArray = new string[charSn.Length];
                for (int j = 0; j < charSn.Length; j++)
                {
                    snArray[j] = charSn[j].ToString();
                }

                for (int k = 0; k < ruleArray.Length; k++)
                {
                    if (ruleArray[k] == "*")
                    {
                        continue;
                    }
                    if (!ruleArray[k].Equals(snArray[k]))
                    {
                        snRuleFlag = false;
                        break;
                    }
                }

                if (snRuleFlag)
                {
                    Station.AddMessage("MES00000059", new string[] { inputSn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { inputSn, skuConfigRule }));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        /// <summary>
        /// 預組裝檢查SN規則
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>        

        public static void SNRuleStringDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objSku;
            bool snRuleFlag = true;
            string skuConfigRule = string.Empty;
            string inputSn = string.Empty;
            string[] ruleArray;
            string[] snArray;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); ;
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                objSku = (SKU)sessionSku.Value;
                if (objSku != null)
                {
                    skuConfigRule = objSku.SnRule;
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                inputSn = sessionSn.Value.ToString();
                //如果 RuleName 是 C_SN_RULE 里配置的规则,就按照该规则检查.Tiny 2020-02-18
                //否则使用简易规则
                var Rule = Station.SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.NAME == objSku.SkuBase.SN_RULE).First();
                var UseRegex = objSku.SkuBase.SN_RULE.ToUpper().StartsWith("REG://");

                if (UseRegex || Rule != null )
                {

                    if (MESPubLab.MESStation.SNMaker.SNmaker.CkeckSNRule(inputSn, objSku.SkuBase.SN_RULE, Station.SFCDB))
                        //检查不通过会报异常,不用处理
                    {
                        return;
                    }
                }

                if (string.IsNullOrEmpty(skuConfigRule))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000251"));
                }
                
                if (inputSn.Length != skuConfigRule.Length)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000252", new string[] { skuConfigRule }));
                }
                char[] charConfigRule = skuConfigRule.ToCharArray();
                ruleArray = new string[charConfigRule.Length];
                for (int i = 0; i < charConfigRule.Length; i++)
                {
                    ruleArray[i] = charConfigRule[i].ToString();
                }

                char[] charSn = inputSn.ToCharArray();
                snArray = new string[charSn.Length];
                for (int j = 0; j < charSn.Length; j++)
                {
                    snArray[j] = charSn[j].ToString();
                }

                for (int k = 0; k < ruleArray.Length; k++)
                {
                    if (ruleArray[k] == "*")
                    {
                        continue;
                    }
                    if (!ruleArray[k].Equals(snArray[k]))
                    {
                        snRuleFlag = false;
                        break;
                    }
                }

                if (snRuleFlag)
                {
                    Station.AddMessage("MES00000059", new string[] { inputSn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { inputSn, skuConfigRule }));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 預組裝檢查PanelSN規則
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>        

        public static void PanelSNRuleStringDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objSku;
            bool PanelsnRuleFlag = true;
            string skuConfigRule = string.Empty;
            string inputPanelSn = string.Empty;
            string[] ruleArray;
            string[] snArray;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); ;
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                objSku = (SKU)sessionSku.Value;
                if (objSku != null)
                {
                    skuConfigRule = objSku.SkuBase.PANEL_RULE;
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                inputPanelSn = sessionSn.Value.ToString();
                if (string.IsNullOrEmpty(skuConfigRule))
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000251"));
                }

                if (inputPanelSn.Length != skuConfigRule.Length)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000252", new string[] { skuConfigRule }));
                }
                //如果 RuleName 是 C_SN_RULE 里配置的规则,就按照该规则检查.
                //否则使用简易规则
                var Rule = Station.SFCDB.ORM.Queryable<C_SN_RULE>().Where(t => t.NAME == objSku.SkuBase.PANEL_RULE).First();
                var UseRegex = objSku.SkuBase.PANEL_RULE.ToUpper().StartsWith("REG://");

                if (UseRegex || Rule != null)
                {

                    if (MESPubLab.MESStation.SNMaker.SNmaker.CkeckSNRule(inputPanelSn, objSku.SkuBase.PANEL_RULE, Station.SFCDB))
                    //检查不通过会报异常,不用处理
                    {
                        return;
                    }
                }
                char[] charConfigRule = skuConfigRule.ToCharArray();
                ruleArray = new string[charConfigRule.Length];
                for (int i = 0; i < charConfigRule.Length; i++)
                {
                    ruleArray[i] = charConfigRule[i].ToString();
                }

                char[] charSn = inputPanelSn.ToCharArray();
                snArray = new string[charSn.Length];
                for (int j = 0; j < charSn.Length; j++)
                {
                    snArray[j] = charSn[j].ToString();
                }

                for (int k = 0; k < ruleArray.Length; k++)
                {
                    if (ruleArray[k] == "*")
                    {
                        continue;
                    }
                    if (!ruleArray[k].Equals(snArray[k]))
                    {
                        PanelsnRuleFlag = false;
                        break;
                    }
                }

                if (PanelsnRuleFlag)
                {
                    Station.AddMessage("MES00000059", new string[] { inputPanelSn }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000058", new string[] { inputPanelSn, skuConfigRule }));
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 1.檢查SN 在r_repair_failcode 表中是否已经维修完成
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void SNFailCodeReapirDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            T_R_REPAIR_FAILCODE RepairFailcode = new T_R_REPAIR_FAILCODE(Station.SFCDB, Station.DBType);
            Row_R_REPAIR_FAILCODE FailCodeRow;
            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession SNFailCodeSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNFailCodeSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            else
            {
                if (SNFailCodeSession.Value != null)
                {
                    FailCodeRow = RepairFailcode.GetByFailCodeID(SNFailCodeSession.Value.ToString(), Station.SFCDB);
                    if (FailCodeRow == null)
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000192", new string[] { }));
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].VALUE }));
                }
            }


        }

        /// <summary>
        /// 1.檢查输入的Location位置是否是在数据库中存在，不存在则报错
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void ReapirLocationDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            OleExec apdb = null;
            List<string> LocationList = new List<string>();

            if (Paras.Count != 2)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }

            MESStationSession SNSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            SN ObjSN = (SN)SNSession.Value;

            MESStationSession LocationSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (LocationSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string StrLocation = LocationSession.Value.ToString();

            //獲取ALLPART數據
            AP_DLL APDLL = new AP_DLL();
            MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "Location");
            List<object> ret = I.DataForUse;
            ret.Clear();
            try
            {
                apdb = Station.DBS["APDB"].Borrow();
                LocationList = APDLL.CheckLocationExist(ObjSN.SkuNo, StrLocation, apdb);
                if (LocationList.Count <= 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000072", new string[] { StrLocation, ObjSN.SkuNo }));
                }
                else
                {
                    foreach (object item in LocationList)
                    {
                        ret.Add(item);
                    }
                }
                Station.DBS["APDB"].Return(apdb);
                Station.AddMessage("MES00000001", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                if (apdb != null)
                {
                    Station.DBS["APDB"].Return(apdb);
                }
                throw ex;
            }

        }

        /// <summary>
        /// 1.檢查工單是否可以進行SMTloading/SIloading.只有正常工單可以進行.
        /// 2.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
        /// 3.Check工單不能關結(R_WO_BASE.Closed != 1)
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void WoLoadingStationDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;

            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE", new string[] { Paras[0].SESSION_TYPE }));
            }
            else
            {
                if (Swo.Value != null)
                {
                    ObjWorkorder = (WorkOrder)Swo.Value;
                    if (ObjWorkorder != null)
                    {
                        if (ObjWorkorder.WORKORDER_QTY <= ObjWorkorder.INPUT_QTY)
                        {
                            //Station.AddMessage("MES00000060", new string[] { ObjWorkorder.WorkorderNo.ToString(),ObjWorkorder.WORKORDER_QTY.ToString(), ObjWorkorder.INPUT_QTY.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000060",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString(),
                                        ObjWorkorder.WORKORDER_QTY.ToString(),
                                        ObjWorkorder.INPUT_QTY.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        if (ObjWorkorder.CLOSED_FLAG != "0")
                        {
                            //Station.AddMessage("MES00000041", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Fail);
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        Station.AddMessage("MES00000061", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    }
                }
                else
                {
                    //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114357", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY, Paras[0].VALUE }));
                }
            }


        }

        /// <summary>
        /// 1.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
        /// 2.Check工單狀態必須正確Closed=0,Release_date is not null
        /// 3.檢查工單是否可以進行SMTloading.只有路由起始站為SMTLoading可以進行.
        /// 4.工單不能被鎖定、工單料號不能被鎖定--?還未實現該功能
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras">1個參數,WO保存的位置</param>
        public static void SMTLoadingWoStationDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;
            string ParaStation = "SMTLOADING";

            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));

            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }
            else
            {
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    ParaStation = Paras[0].VALUE.ToString();
                }
                if (Swo.Value != null)
                {
                    ObjWorkorder = (WorkOrder)Swo.Value;
                    ObjWorkorder.Init(Swo.Value.ToString(), Station.SFCDB);
                    if (ObjWorkorder != null)
                    {
                        //1.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
                        if (ObjWorkorder.WORKORDER_QTY <= ObjWorkorder.INPUT_QTY)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000060",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString(),
                                        ObjWorkorder.WORKORDER_QTY.ToString(),
                                        ObjWorkorder.INPUT_QTY.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        //2.Check工單狀態
                        if (ObjWorkorder.CLOSED_FLAG == "1")
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        if (ObjWorkorder.RELEASE_DATE == null)
                        {
                            Station.AddMessage("MES00000042", new string[] { "WO:" + ObjWorkorder.WorkorderNo }, StationMessageState.Fail);
                            return;
                        }
                        //add by LLF 2018-03-20
                        if (ObjWorkorder.ROUTE.DETAIL == null)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000194",
                                   new string[] { ObjWorkorder.WorkorderNo.ToString() });
                            throw new MESReturnMessage(ErrMessage);
                        }
                        //Modify by LLF 2018-03-20
                        // 3.檢查工單是否可以進行SMTloading.只有路由起始站為SMTLoading可以進行.
                        if (ObjWorkorder.ROUTE.DETAIL[0].STATION_NAME != ParaStation)
                        {
                            ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000112",
                                    new string[] { ObjWorkorder.WorkorderNo.ToString(), ParaStation });
                            throw new MESReturnMessage(ErrMessage);

                        }
                        // 4.工單不能被鎖定、工單料號不能被鎖定--?還未實現該功能
                        Station.AddMessage("MES00000061", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    }
                }
                else
                {
                    //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114357", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY, Paras[0].VALUE }));
                }
            }
        }

        public static void SMTLoadingSNStationDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;
            string ParaStation = "SMTLOADING";
            string sql = string.Empty;
            OleExec apdb = null;

            if (Paras.Count != 1)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (Swo == null)
            {
                //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }
            else
            {
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    ParaStation = Paras[0].VALUE.ToString();
                }
                if (Swo.Value != null)
                {
                    ObjWorkorder = (WorkOrder)Swo.Value;
                    ObjWorkorder.Init(Swo.Value.ToString(), Station.SFCDB);
                    if (ObjWorkorder != null)
                    {
                        apdb = Station.APDB;

                        if (Station.DisplayName == "SFC_SMT_LOADING")
                        {
                            sql = "select * from mes1.c_product_config where p_no='" + ObjWorkorder.CUST_PN + "' and data2='N'";
                            DataTable res = apdb.ExecSelect(sql).Tables[0];
                            if (res.Rows.Count == 0)
                            {
                                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091215");
                                throw new MESReturnMessage(ErrMessage);
                            }
                        }
                        if (Station.DisplayName == "PCBA_SMTLOADING")
                        {
                            sql = "select * from mes1.c_product_config where p_no ='" + ObjWorkorder.CUST_PN + "' and data2='Y'";
                            DataTable res = apdb.ExecSelect(sql).Tables[0];
                            if (res.Rows.Count == 0)
                            {
                                ErrMessage = MESReturnMessage.GetMESReturnMessage("MSGCODE20190412091215");
                                throw new MESReturnMessage(ErrMessage);
                            }
                        }
                    }
                }
                else
                {
                    //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + "" + Paras[0].VALUE + " !");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814114357", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY, Paras[0].VALUE }));
                }
            }
        }

        public static void SMTLoadingWoInputQtyDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            WorkOrder ObjWorkorder = new WorkOrder();
            string ErrMessage = string.Empty;
            int IntLinkQty = 0;
            string ParaStation = "SMTLOADING"; //add by LLF 2018-03-20

            if (Paras.Count <= 0)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession Swo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);

            if (Swo == null)
            {
                //throw new Exception("Can Not Find " + Paras[0].SESSION_TYPE + " " + Paras[0].SESSION_KEY + " !");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814111940", new string[] { Paras[0].SESSION_TYPE, Paras[0].SESSION_KEY }));
            }

            MESStationSession Linkqty = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (Linkqty != null)
            {
                IntLinkQty = Convert.ToInt16(Linkqty.Value.ToString());
            }

            if (Swo.Value != null)
            {
                //add by LLF 2018-03-28
                if (!string.IsNullOrEmpty(Paras[0].VALUE))
                {
                    ParaStation = Paras[0].VALUE.ToString();
                }
                //ObjWorkorder = (WorkOrder)Swo.Value;
                ObjWorkorder.Init(Swo.Value.ToString(), Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                if (ObjWorkorder != null)
                {
                    //1.工單不能R_WO_BASE.Inputqty >= R_WO_BASE.Workorderqty
                    if (ObjWorkorder.WORKORDER_QTY < ObjWorkorder.INPUT_QTY + IntLinkQty)
                    {
                        Station.Inputs[3].Value = ""; //add by LLF 2018-01-26 
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000134",
                                new string[] { ObjWorkorder.WorkorderNo.ToString(),
                                    ObjWorkorder.INPUT_QTY.ToString(),
                                    IntLinkQty.ToString(),
                                    ObjWorkorder.WORKORDER_QTY.ToString()
                                     });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    //2.Check工單狀態
                    if (ObjWorkorder.CLOSED_FLAG == "1")
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000041",
                                new string[] { ObjWorkorder.WorkorderNo.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    if (ObjWorkorder.RELEASE_DATE == null)
                    {
                        Station.AddMessage("MES00000042", new string[] { "WO:" + ObjWorkorder.WorkorderNo }, StationMessageState.Fail);
                        return;
                    }
                    //add by LLF 2018-03-20
                    if (ObjWorkorder.ROUTE.DETAIL == null)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000194",
                               new string[] { ObjWorkorder.WorkorderNo.ToString() });
                        throw new MESReturnMessage(ErrMessage);
                    }
                    // 3.檢查工單是否可以進行SMTloading.只有路由起始站為SMTLoading可以進行.
                    if (ObjWorkorder.ROUTE.DETAIL[0].STATION_NAME != ParaStation)
                    {
                        ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000112",
                                new string[] { ObjWorkorder.WorkorderNo.ToString(), ParaStation });
                        throw new MESReturnMessage(ErrMessage);

                    }
                    // 4.工單不能被鎖定、工單料號不能被鎖定--?還未實現該功能
                    Station.AddMessage("MES00000061", new string[] { ObjWorkorder.WorkorderNo.ToString() }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                }

            }
        }

        public static void RouteDatachecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            string ErrMessage = "";
            List<R_MRB> GetMRBList = new List<R_MRB>();
            R_MRB New_R_MRB = new R_MRB();
            T_R_MRB TR_MRB = new T_R_MRB(Station.SFCDB, Station.DBType);
            T_R_WO_TYPE TRWT = new T_R_WO_TYPE(Station.SFCDB, Station.DBType);

            if (Paras.Count != 3)
            {
                //throw new Exception("參數數量不正確!");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814105115"));
            }
            MESStationSession SnSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SnSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            else if (SnSession.Value == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].VALUE + Paras[0].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            SN ObjSn = (SN)SnSession.Value;

            MESStationSession WoSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WoSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            WorkOrder ObjWo = (WorkOrder)WoSession.Value;

            MESStationSession StationSession = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (StationSession == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].SESSION_TYPE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }
            else if (StationSession.Value == null)
            {
                ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[2].VALUE + Paras[2].SESSION_KEY });
                throw new MESReturnMessage(ErrMessage);
            }

            string NextStation = StationSession.Value.ToString();

            try
            {
                //BPD RMA 時打回不檢查站位合理性
                R_WO_TYPE WoType = TRWT.GetWoTypeByWo(Station.SFCDB, ObjWo.WorkorderNo);
                //if(Station.BU.Equals("BPD") && WoType.WORKORDER_TYPE.Equals("RMA"))
                //if (WoType.WORKORDER_TYPE.Equals("RMA"))
                if (WoType.CATEGORY.Equals("RMA"))
                {
                    Station.AddMessage("MES00000026", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
                    return;
                }
                GetMRBList = TR_MRB.GetMrbInformationBySN(ObjSn.SerialNo, Station.SFCDB);

                if (GetMRBList == null || GetMRBList.Count == 0)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000045", new string[] { "R_MRB:" + ObjSn.SerialNo });
                    throw new MESReturnMessage(ErrMessage);
                }
                //MESStationInput I = Station.Inputs.Find(t => t.DisplayName == "StationName");
                //List<object> snStationList = I.DataForUse;
                //snStationList.Clear();
                //snStationList.Add(""); ///BY SDL  加載頁面默認賦予空值,操作員必須點選其他有內容選項

                Route routeDetail = new Route(ObjWo.RouteID, GetRouteType.ROUTEID, Station.SFCDB, MESDataObject.DB_TYPE_ENUM.Oracle);
                List<RouteDetail> routeDetailList = routeDetail.DETAIL;
                RouteDetail h = routeDetailList.Find(t => t.STATION_NAME == GetMRBList[0].NEXT_STATION || t.STATION_TYPE == GetMRBList[0].NEXT_STATION);
                if (h == null)   //R_MRB next_station欄位的值
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000205", new string[] { ObjSn.SerialNo, ObjWo.WorkorderNo });
                    throw new MESReturnMessage(ErrMessage);
                }

                RouteDetail g = routeDetailList.Find(t => t.STATION_NAME == NextStation);
                if (g == null)  //REWORK選擇的要打回工站
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { NextStation });
                    throw new MESReturnMessage(ErrMessage);
                }

                if (g.SEQ_NO > h.SEQ_NO)
                {
                    ErrMessage = MESReturnMessage.GetMESReturnMessage("MES00000211", new string[] { });
                    throw new MESReturnMessage(ErrMessage);
                }


                Station.AddMessage("MES00000026", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// 檢查輸入的Action_code是否存在
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ActionCodeDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 1)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            string ActionCodeInput = "";
            MESStationSession sessionActionCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionActionCode == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            if (sessionActionCode.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            try
            {
                T_C_ACTION_CODE t_c_action_code = new T_C_ACTION_CODE(Station.SFCDB, Station.DBType);
                C_ACTION_CODE c_action_code = new C_ACTION_CODE();
                c_action_code = t_c_action_code.GetByActionCode(sessionActionCode.Value.ToString(), Station.SFCDB);
                if (c_action_code == null)
                {
                    sessionActionCode.Value = null;
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000007", new string[] { "ActionCode", ActionCodeInput }));
                }
                Station.AddMessage("MES00000026", new string[] { }, MESPubLab.MESStation.MESReturnView.Station.StationMessageState.Pass);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 檢查指定的輸入是否添加到Session,是否為空
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void InputSessionDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            for (int i = 0; i < Paras.Count; i++)
            {
                MESStationSession session = Station.StationSession.Find(t => t.MESDataType == Paras[i].VALUE.ToString().Trim());
                if (session == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { Paras[i].VALUE.ToString().Trim() }));
                }
                else if (session.Value == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616103112", new string[] { Paras[i].VALUE.ToString().Trim() }));
                }
            }
        }

        public static void OriginalSNDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SN == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[0].SESSION_TYPE.ToString().Trim() }));
            }
            MESStationSession CSNPARTNO = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (CSNPARTNO == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[1].SESSION_TYPE.ToString().Trim() }));
            }
            MESStationSession CSNTYPE = Station.StationSession.Find(t => t.MESDataType == Paras[2].SESSION_TYPE && t.SessionKey == Paras[2].SESSION_KEY);
            if (CSNTYPE == null)
            {
                CSNTYPE = new MESStationSession() { MESDataType = Paras[2].SESSION_TYPE, SessionKey = Paras[2].SESSION_KEY };
                Station.StationSession.Add(CSNTYPE);
            }
            Boolean Flag = true;
            string value = Input.Value.ToString();
            string partno = CSNPARTNO.Value.ToString();
            while (Flag)
            {
                List<R_SN_KP> sn = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((K) => K.VALUE == value && K.VALID_FLAG == 1).ToList();
                if (sn.Count > 0)
                {
                    sn = sn.FindAll((t) => t.PARTNO == partno || partno == "");
                    if (sn.Count > 0)
                    {
                        List<R_SN_KP> kp = sn.FindAll((t) => t.SN == SN.Value.ToString() && t.VALID_FLAG == 1);
                        if (kp.Count > 0)
                        {
                            Flag = false;
                            Station.AddMessage("MES00000101", new string[] { }, StationMessageState.Pass);
                        }
                        else
                        {
                            partno = "";
                            value = sn[0].SN;
                        }
                        if (CSNTYPE.Value == null)
                        {
                            CSNTYPE.Value = sn[0].SCANTYPE;
                        }
                    }
                    else
                    {
                        throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20190128223617", new string[] { Input.Value.ToString() }));
                    }
                }
                else
                {
                    throw new Exception(MESReturnMessage.GetMESReturnMessage("MSGCODE20180616081316", new string[] { SN.Value.ToString(), Input.Value.ToString() }));
                }
            }
        }

        public static void ReplaceSNDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession PartNoSession = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (PartNoSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[0].SESSION_TYPE.ToString().Trim() }));
            }
            MESStationSession CSNTYPE = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (CSNTYPE == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[1].SESSION_TYPE.ToString().Trim() }));
            }
            //add By James Zhu 09/21/2019 for L6 chassis replaced in DF Site. i assumed "KPNAME" exist in all sites
            MESStationSession KPNAME = Station.StationSession.Find(t => t.MESDataType == "KPNAME");

            // End By James Zhu

            if (CSNTYPE.Value.ToString() == "SystemSN" && KPNAME.Value.ToString() != "L6_CHASSIS")
            {
                List<R_SN> snn = Station.SFCDB.ORM.Queryable<R_SN>().Where((K) => K.SN == Input.Value.ToString() && K.VALID_FLAG == "1").ToList();
                if (snn.Count > 0)
                {
                    if (snn[0].SHIPPED_FLAG == "1")
                    {
                        List<R_SN_KP> kps = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((s) => s.VALUE == Input.Value.ToString()).ToList();
                        if (kps.Count > 0)
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000165", new string[] { Input.Value.ToString() }));
                        }
                        else
                        {
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20190128213735", new string[] { Input.Value.ToString() }));
                        }
                    }
                    else if (snn[0].COMPLETED_FLAG == "0")
                    {
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000144", new string[] { Input.Value.ToString() }));
                    }
                    else
                    {
                        Station.AddMessage("MES00000101", new string[] { }, StationMessageState.Pass);
                    }
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000048", new string[] { Input.Value.ToString() }));
                }
            }
            else
            {
                List<R_SN_KP> kps = Station.SFCDB.ORM.Queryable<R_SN_KP>().Where((s) => s.VALUE == Input.Value.ToString()).ToList();
                if (kps.Count > 0)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000165", new string[] { Input.Value.ToString() }));
                }
                else
                {
                    Station.AddMessage("MES00000101", new string[] { }, StationMessageState.Pass);
                }
            }
        }


        public static void ErrorCodeChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession ErrorCode = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (ErrorCode == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[0].SESSION_TYPE.ToString().Trim() }));
            }
            List<C_ERROR_CODE> codes = Station.SFCDB.ORM.Queryable<C_ERROR_CODE>().Where((K) => K.ERROR_CODE.ToUpper() == Input.Value.ToString().ToUpper()).ToList();
            if (codes.Count > 0)
            {
                Station.AddMessage("MES00000101", new string[] { }, StationMessageState.Pass);
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { Input.Value.ToString() }));
            }

        }

        /// <summary>
        /// 檢查輸入的字符串中是否有指定的錯誤字符
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void ErrorStringInputChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession sessionString = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionString == null || sessionString.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[0].SESSION_TYPE.ToString().Trim() }));
            }
            string inputString = sessionString.Value.ToString();
            string errorString = "";
            for (int i = 1; i < Paras.Count; i++)
            {
                errorString = Paras[i].VALUE;
                if (string.IsNullOrEmpty(errorString))
                {
                    continue;
                }
                if (inputString.Contains(errorString))
                {
                    //throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000142", new string[] { Input.Value.ToString() }));
                    //throw new MESReturnMessage(inputString + " Contain Error String " + errorString);
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115000", new string[] { inputString, errorString }));
                }
            }
        }
        /// <summary>
        /// 輸入的字符串中是否只是數字和字母
        /// </summary>
        /// <param name="Station"></param>
        /// <param name="Input"></param>
        /// <param name="Paras"></param>
        public static void StringInputChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession sessionString = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionString == null || sessionString.Value == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[0].SESSION_TYPE.ToString().Trim() }));
            }
            List<C_SKU_DETAIL> NoCheck_SC = new List<C_SKU_DETAIL>();
            if (Paras.Count>1)
            {
                MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
                if (WOSession == null)
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[1].SESSION_TYPE.ToString().Trim() }));
                }
                string woString = WOSession.Value.ToString();
                NoCheck_SC = Station.SFCDB.ORM.Queryable<R_WO_BASE, C_SKU_DETAIL>((a, b) => a.SKUNO == b.SKUNO && a.SKU_VER == b.VERSION)
                    .Where((a, b) => a.WORKORDERNO == woString && b.CATEGORY == "NOCHECK_SPECIAL_CHARACTERS")
                    .Select((a, b) => b)
                    .ToList();
            }
            string inputString = sessionString.Value.ToString();
            string regexstr = "^[a-zA-Z0-9]+$";
            if (!Regex.IsMatch(inputString, regexstr) && NoCheck_SC.Count == 0)
            {
                //throw new Exception(inputString + " Input SN Contain Error Letter");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115625", new string[] { inputString }));
            }
        }

        public static void StationLineChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            List<R_Station_Action_Para> list = Paras.FindAll(r => r.SESSION_TYPE == "STATION_NAME");
            var sessionObj = snSession.Value;
            SN snObj = null;
            R_SN r_sn = null;
            Panel panel = null;
            string sn = "";           
            bool is_panel = false;
            string last_line = "";
            T_R_SN_STATION_DETAIL t_r_sn_station_detail = new T_R_SN_STATION_DETAIL(Station.SFCDB, Station.DBType);
            if (sessionObj is string)
            {
                sn = sessionObj.ToString();
            }
            else if (typeof(SN) == sessionObj.GetType())
            {
                snObj = (SN)sessionObj;
                sn = snObj.SerialNo;
            }
            else if (typeof(R_SN) == sessionObj.GetType())
            {
                r_sn = (R_SN)sessionObj;
                sn = r_sn.SN;
            }
            else if (typeof(Panel) == sessionObj.GetType())
            {
                panel = (Panel)sessionObj;
                is_panel = true;
            }
            else
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20180607163531", new string[] { sessionObj.ToString() }));
            }
            if (is_panel)
            {
                foreach (var l in list)
                {
                    if (!string.IsNullOrEmpty(l.VALUE))
                    {
                        last_line = Station.SFCDB.ORM.Queryable<R_PANEL_SN, R_SN_STATION_DETAIL>((p, s) => p.SN == s.SN)
                            .Where((p, s) => p.PANEL == panel.PanelNo && s.STATION_NAME == l.VALUE)
                            .OrderBy((p, s) => s.EDIT_TIME, SqlSugar.OrderByType.Desc)
                            .Select((p, s) => s.LINE).ToList()[0];
                        if (!last_line.Equals(Station.Line))
                        {
                            //throw new MESReturnMessage($@"{Station.Line} Not Equal {l.VALUE} Last Pass Line {last_line}!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115713", new string[] { Station.Line, l.VALUE, last_line }));
                        }
                    }
                }
            }
            else
            {
                foreach (var l in list)
                {
                    if (!string.IsNullOrEmpty(l.VALUE))
                    {
                        last_line = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>()
                            .Where(r => r.STATION_NAME == l.VALUE && r.SN == sn).OrderBy(r => r.EDIT_TIME, SqlSugar.OrderByType.Desc)
                            .Select(r => r.LINE).ToList()[0];
                        if (!last_line.Equals(Station.Line))
                        {
                            //throw new MESReturnMessage($@"{Station.Line} Not Equal {l.VALUE} Last Pass Line {last_line}!");
                            throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115713", new string[] { Station.Line, l.VALUE, last_line }));
                        }
                    }
                }
            }
        }

        public static void CtnNetWeightDataChecker(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            MESStationSession SessionCarton = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SessionCarton == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            
            var sfcdb = Station.SFCDB;

            var strCtn = SessionCarton.Value.ToString();

            var pack =sfcdb.ORM.Queryable<R_PACKING>().Where(t => t.PACK_NO == strCtn).First();

            if (pack != null)
            {
                var strWeight = sfcdb.ORM.Queryable<R_WEIGHT>().Where(t => t.SNID == pack.ID).Select(t => t.WEIGHT).First();
                if (strWeight == null)
                {
                    //throw new Exception($@"'Can't get Gross weight'");
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814115815"));
                }
                var cSkuType = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT_CTO" && t.SKUNO == pack.SKUNO).ToList().FirstOrDefault();
                if (cSkuType == null)
                {
                    var c_netweight = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == pack.SKUNO && t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT").First();
                    if (c_netweight == null)
                    {
                        //throw new Exception($@"'{pack.SKUNO} not config net weight'");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814120009", new string[] { pack.SKUNO }));
                    }
                    var nettotalweight = double.Parse(c_netweight.VALUE) * pack.QTY;
                    if (nettotalweight >= double.Parse(strWeight))
                    {
                        //throw new Exception($@"Gross weight({strWeight}) must be greater then net weight({nettotalweight})");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134045", new string[] { strWeight, nettotalweight.ToString() }));
                    }
                }
                else
                {
                    var c_netweight = sfcdb.ORM.Queryable<C_SKU_DETAIL>().Where(t => t.SKUNO == pack.SKUNO && t.CATEGORY == "JUNIPER" && t.CATEGORY_NAME == "NET_WEIGHT_CTO").First();
                    if (c_netweight == null)
                    {
                        //throw new Exception($@"'{pack.SKUNO} not config net weight'");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814120010", new string[] { pack.SKUNO }));
                    }
                    var nettotalweight = double.Parse(c_netweight.VALUE) * pack.QTY;
                    if (nettotalweight >= double.Parse(strWeight))
                    {
                        //throw new Exception($@"Gross weight({strWeight}) must be greater then net weight({nettotalweight})");
                        throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134045", new string[] { strWeight, nettotalweight.ToString() }));
                    }
                }

            }
            else
            {
                //throw new Exception($@"Can't load ctnno '{strCtn}'");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134139", new string[] { strCtn }));
            }


        }

        public static void UploadLinkDataChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            MESStationSession snSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[0].SESSION_TYPE) && t.SessionKey.Equals(Paras[0].SESSION_KEY));
            if (snSession == null || snSession.Value ==null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE + Paras[0].SESSION_KEY }));
            }
            MESStationSession linkValueSession = Station.StationSession.Find(t => t.MESDataType.Equals(Paras[1].SESSION_TYPE) && t.SessionKey.Equals(Paras[1].SESSION_KEY));
            if (linkValueSession == null || linkValueSession.Value==null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE + Paras[1].SESSION_KEY }));
            }
            string linkType = "";
            if (Paras.Count > 2)
            {
                linkType = Paras[2].VALUE == null ? "" : Paras[2].VALUE.ToString().ToUpper();
            }

            R_SN_LINK linkObj = null;
            if (string.IsNullOrWhiteSpace(linkType))
            {
                linkObj = Station.SFCDB.ORM.Queryable<R_SN_LINK>().Where(r => r.SN == snSession.Value.ToString() && r.VALIDFLAG == "1").ToList().FirstOrDefault();
            }
            else
            {
                linkObj = Station.SFCDB.ORM.Queryable<R_SN_LINK>().Where(r => r.SN == snSession.Value.ToString() && r.VALIDFLAG == "1" && r.LINKTYPE == linkType).ToList().FirstOrDefault();
            }
            if(linkObj==null)
            {
                //throw new Exception($@"{snSession.Value.ToString()} no link data,please call PE upload.");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134307", new string[] { snSession.Value.ToString() }));
            }
            if (linkObj.CSN != linkValueSession.Value.ToString())
            {
                //throw new Exception($@"{linkValueSession.Value.ToString()} not match link data.");
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20210814134342", new string[] { linkValueSession.Value.ToString() }));
            }
            linkObj.MODEL = "Y";
            linkObj.EDITBY = Station.LoginUser.EMP_NO;
            linkObj.EDITTIME = Station.SFCDB.ORM.GetDate();
            Station.SFCDB.ORM.Updateable<R_SN_LINK>(linkObj).Where(r => r.ID == linkObj.ID).ExecuteCommand();
        }

        /// <summary>
        /// 檢查KIT_PRINT的SN是否匹配WO對應的SKU和VER
        /// </summary>
        public static void InputKitPrintSNSKUVerChecker(MESStationBase Station, MESStationInput Input, List<R_Station_Action_Para> Paras)
        {
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionWo = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionWo == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); ;
            }
            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }

            try
            {
                string wo = sessionWo.Value.ToString();
                string sn = sessionSn.Value.ToString();
                var _wo = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == wo).ToList().FirstOrDefault();
                if (_wo != null)
                {
                    string woSKU = _wo.SKUNO;
                    string woVER = _wo.SKU_VER;

                    var _sn = Station.SFCDB.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == sn && t.STATION_NAME == "KIT_PRINT").ToList().FirstOrDefault();
                    if (_sn != null)
                    {
                        var _snWO = Station.SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.WORKORDERNO == _sn.WORKORDERNO).ToList().FirstOrDefault();
                        if (_snWO != null)
                        {
                            string snSKU = _snWO.SKUNO;
                            string snVER = _snWO.SKU_VER;

                            //if the sku/ver of wo not match the sku/ver of sn, throw exception    asked by PE 楊大饒 2022-01-29
                            if (woSKU != snSKU || woVER != snVER)
                            {
                                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MSGCODE20220129093920", new string[] { sn, wo }));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void ArubaCheckPN(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {
            SKU objSku;
            string skuConfigRule = string.Empty;
            string inputSn = string.Empty;
            if (Paras.Count != 2)
            {
                throw new Exception(MESReturnMessage.GetMESReturnMessage("MES00000050"));
            }
            MESStationSession sessionSku = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (sessionSku == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE })); ;
            }

            MESStationSession sessionSn = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (sessionSn == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[1].SESSION_TYPE }));
            }
            try
            {
                objSku = (SKU)sessionSku.Value;
                if (objSku != null)
                {
                    skuConfigRule = objSku.SnRule;
                }
                else
                {
                    throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000052", new string[] { Paras[0].SESSION_TYPE }));
                }
                inputSn = sessionSn.Value.ToString();
                var check = Station.SFCDB.ORM.Queryable<R_F_CONTROL>().Where(t => t.FUNCTIONNAME == "ARUBA_CHECK_MATERIAL" && t.VALUE== objSku.SkuNo && t.CONTROLFLAG=="Y" && t.FUNCTIONTYPE== "NOSYSTEM").ToList();
                if (check.Count != 0)
                {
                    for (int i= 0; i < check.Count; i ++)
                    {
                        var check1 = Station.SFCDB.ORM.Queryable<R_F_CONTROL_EX>().Where(t => t.DETAIL_ID == check[i].ID && (t.VALUE != null || t.VALUE != "")).ToList();
                        if (check1.Count == 0)
                        {
                            throw new Exception("PE not setup QTY .FInd PE");
                        }

                        UIInputData O = new UIInputData() { };
                        O.Timeout = 3000000;
                        O.IconType = IconType.Warning;
                        O.Type = UIInputType.String;
                        O.Tittle = $@"CHECK THERMAL PAD";
                        O.ErrMessage = "No input";
                        O.Message = "QTY";
                        O.Name = "QTY";
                        O.UIArea = new string[] { "40%", "70%" };
                        O.OutInputs.Add(new DisplayOutPut
                        {
                            Name = "MATERIAL",
                            Value = check[i].EXTVAL.ToString(),
                            DisplayType = UIOutputType.Text.ToString()
                        });
                        while (true)
                        {
                            var input_qty = O.GetUiInput(Station.API, UIInput.CanPrint, Station);
                            Station.LabelPrint.Clear();
                            Station.LabelPrints.Clear();
                            Station.LabelStillPrint.Clear();
                            if (input_qty == null)
                            {
                                O.CBMessage = "Please Scan QTY";
                            }
                            else
                            {
                                string check_qty = input_qty.ToString();
                                if (string.IsNullOrEmpty(check_qty))
                                {
                                    O.CBMessage = "Please Input QTY";
                                }
                                else if (check_qty != check1[0].VALUE.ToString())
                                {
                                    O.CBMessage = $@"{input_qty} Not Same PE setup. Find PE comfirm";
                                }
                                else if (input_qty.Equals("No input"))
                                {
                                    throw new Exception("User Cancel");
                                }
                                else if (check_qty == check1[0].VALUE.ToString())
                                {
                                    O.CBMessage = $@"o so ke";
                                    break;
                                }
                            }
                        }
                    }
                    
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CheckSNSiReturn(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {




            MESStationSession SNN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNN == null)
            {
                SNN = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNN);
            }


            string SN = SNN.InputValue.ToString();
            DataTable dt;
            string SQL;

            try
            {
                string sql = $@"select a.CUST_KP_NO,a.MFR_KP_NO,a.DATE_CODE,a.LOT_CODE,a.EXT_QTY,b.MFR_NAME as mfr_name from mes4.r_tr_sn a,mes1.c_mfr_config b where a.tr_sn='{SN}' and a.mfr_code=b.mfr_code";
                dt = new DataTable();
                dt.Clear();
                dt = Station.APDB.ExecuteDataTable(sql, CommandType.Text, null);
                if (dt.Rows.Count > 0)
                {

                    string sqlLine = $@"select station as LINE,WO from mes4.r_tr_sn_wip where tr_sn='{SN}'";
                    dt = new DataTable();
                    dt.Clear();
                    dt = Station.APDB.ExecuteDataTable(sqlLine, CommandType.Text, null);
                    if (dt.Rows.Count > 0)
                    {
                        String LINE = dt.Rows[0]["LINE"].ToString();

                        String WO = dt.Rows[0]["WO"].ToString();

                        string sqlupdate = $@"UPDATE MES4.R_TR_SN SET LOCATION_FLAG = '1',KITTING_FLAG = 'a', kitting_wo='' WHERE TR_SN = '{SN}'";
                        dt = new DataTable();
                        dt.Clear();
                        dt = Station.APDB.ExecuteDataTable(sqlupdate, CommandType.Text, null);


                        string sqladd = $@"INSERT INTO MES4.R_TR_SN_DETAIL(TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,QTY,EXT_QTY,LOCATION_FLAG,WORK_FLAG,WO,STATION,MFG_EMP_NO,WORK_TIME,EMP_NO)
                                                    SELECT TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,QTY,EXT_QTY,2,'1','{WO}','{LINE}','KITTING' MFG_EMP_NO,SYSDATE,'{Station.LoginUser.EMP_NO}' FROM MES4.R_TR_SN WHERE TR_SN = '{SN}'";
                        dt = new DataTable();
                        dt.Clear();
                        dt = Station.APDB.ExecuteDataTable(sqladd, CommandType.Text, null);

                        string sqladd1 = $@"INSERT INTO MES4.R_TR_SN_DETAIL(TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,QTY,EXT_QTY,LOCATION_FLAG,WORK_FLAG,WO,STATION,MFG_EMP_NO,WORK_TIME,EMP_NO)
                    SELECT TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,QTY,EXT_QTY,1,'0','{WO}','{LINE}','KITTING' MFG_EMP_NO,SYSDATE,'{Station.LoginUser.EMP_NO}' FROM MES4.R_TR_SN WHERE TR_SN = '{SN}'";
                        dt = new DataTable();
                        dt.Clear();
                        dt = Station.APDB.ExecuteDataTable(sqladd1, CommandType.Text, null);


                        string sqlDel = $@"Delete mes4.r_tr_sn_wip where tr_sn='{SN}'";
                        dt = new DataTable();
                        dt.Clear();
                        dt = Station.APDB.ExecuteDataTable(sqlDel, CommandType.Text, null);
                    }
                    else
                    {
                        throw new Exception("NO TRSN IN TR_SN_WIP");
                    }

                    throw new Exception("CUST_KP_NO NOT CONFIG!");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void CheckTrsnSI(MESPubLab.MESStation.MESStationBase Station, MESPubLab.MESStation.MESStationInput Input, List<MESDataObject.Module.R_Station_Action_Para> Paras)
        {




            MESStationSession SNN = Station.StationSession.Find(t => t.MESDataType == Paras[0].SESSION_TYPE && t.SessionKey == Paras[0].SESSION_KEY);
            if (SNN == null)
            {
                SNN = new MESStationSession() { MESDataType = Paras[0].SESSION_TYPE, InputValue = Input.Value.ToString(), SessionKey = Paras[0].SESSION_KEY, ResetInput = Input };
                Station.StationSession.Add(SNN);
            }
            MESStationSession WOSession = Station.StationSession.Find(t => t.MESDataType == Paras[1].SESSION_TYPE && t.SessionKey == Paras[1].SESSION_KEY);
            if (WOSession == null)
            {
                throw new MESReturnMessage(MESReturnMessage.GetMESReturnMessage("MES00000057", new string[] { Paras[1].SESSION_TYPE.ToString().Trim() }));
            }


            string woString = WOSession.InputValue.ToString();
            DataTable dt;
            string SQL;



            try
            {
                string SN = SNN.InputValue.ToString();

                SQL = $@"SELECT DISTINCT B.CUST_KP_NO FROM mes1.C_WHS_STOCK_CONFIG A,MES4.R_TR_SN B WHERE A.CUST_KP_NO=B.CUST_KP_NO AND A.AREA='P05A-SI' AND B.TR_SN='{SN}'";

                dt = new DataTable();
                dt = Station.APDB.ExecuteDataTable(SQL, CommandType.Text, null);
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("NOT IS SI MATERIAL! PLEASE CHECK !!!");
                }

                else
                {
                    //SQL = $@"SELECT* FROM MES4.R_WO_REQUEST A, MES4.R_TR_SN B WHERE A.CUST_KP_NO = B.CUST_KP_NO AND A.WO = '{woString}' AND B.TR_SN = '{SN}'";
                    //dt = new DataTable();
                    //dt = Station.APDB.ExecuteDataTable(SQL, CommandType.Text, null);
                    //if (dt.Rows.Count == 0)
                    //{
                    //    throw new Exception("SN not used for WO! Check again!");
                    //}
                    //else
                    //{
                    string sql1 = $@"SELECT * FROM MES4.R_TR_SN where tr_sn='{SN}'";
                    DataTable dt3 = new DataTable();
                    dt3 = Station.APDB.ExecuteDataTable(sql1, CommandType.Text, null);

                    if (dt3.Rows.Count > 0)
                    {

                        int locationFlag = Int32.Parse(dt3.Rows[0]["LOCATION_FLAG"].ToString());



                        switch (locationFlag)
                        {
                            case 0:
                                throw new Exception("Material is in WHS");
                                break;
                            case 1:

                                bool h_special = false;
                                int workFlag;
                                string cust_kp_no;
                                string sql = $@"SELECT * FROM MES4.R_TR_SN WHERE TR_SN = '{SN}'";
                                DataTable dtt = new DataTable();
                                DataTable dt1 = new DataTable();
                                DataTable dt2 = new DataTable();
                                DataTable h_dt = new DataTable();
                                string EMP_NO_CHECKOUT = "KITTING";

                                string hsql = $@"select * from MES1.C_EMP where emp_no ='{Station.LoginUser.EMP_NO}' and DPT_NAME='DCN-KIT'";
                                h_dt = Station.APDB.ExecuteDataTable(hsql, CommandType.Text, null);

                                int h_count = h_dt.Rows.Count;
                                if (h_count > 0)
                                {
                                    h_special = true;
                                    EMP_NO_CHECKOUT = Station.LoginUser.EMP_NO;
                                }


                                dt = Station.APDB.ExecuteDataTable(sql, CommandType.Text, null);
                                workFlag = Int32.Parse(dt.Rows[0]["WORK_FLAG"].ToString());
                                cust_kp_no = dt.Rows[0]["CUST_KP_NO"].ToString();

                                string sql2 = $@"SELECT MIN(SUBSTR (tr_sn, 2, 6)) AS trno, cust_kp_no, tr_sn
                                                    From mes4.r_tr_sn
                                                     WHERE cust_kp_no = '{cust_kp_no}' and location_flag = '1' and work_flag = 0 and KITTING_FLAG in ('b', 'a')
                                                    and tr_sn not in (select tr_sn from mes4.r_trsn_buffer
                                                    ) group by  cust_kp_no, tr_sn
                                                    ORDER BY TO_NUMBER(trno)";

                                dt1 = Station.APDB.ExecuteDataTable(sql2, CommandType.Text, null);
                                string check = dt1.Rows[0]["trno"].ToString();

                                string dateSql = $@"SELECT MIN(SUBSTR (tr_sn, 2, 6)) AS trno, cust_kp_no, tr_sn
                                                    From mes4.r_tr_sn
                                                     WHERE cust_kp_no = '{cust_kp_no}' and location_flag = '1' and work_flag = 0 and KITTING_FLAG in ('b', 'a')
                                                    and tr_sn not in (select tr_sn from mes4.r_trsn_buffer
                                                    ) group by  cust_kp_no, tr_sn
                                                    ORDER BY TO_NUMBER(trno)";

                                dt2 = Station.APDB.ExecuteDataTable(dateSql, CommandType.Text, null);
                                string dateFirst = dt2.Rows[0]["trno"].ToString();

                                if (!SN.Substring(1, 6).Trim().Equals(dateFirst) && !h_special)
                                {
                                    throw new Exception("FIFO RULE IS NOT MATCH ! YOU MUST CHECK OUT MIN SN : " + dateFirst + " BEFORE ");
                                    //MessageBox.Show("FIFO RULE IS NOT MATCH ! YOU MUST CHECK OUT MIN SN : " + dateFirst + " BEFORE ");

                                }
                                else
                                {
                                    string getWoRequestSql = $@"select WO_REQUEST from MES4.R_WO_REQUEST
                                                         where WO='{woString}'
                                                       and CUST_KP_NO=(SELECT distinct(cust_kp_no) FROM MES4.R_TR_SN
                                                        WHERE TR_SN='{SN}')";
                                    DataTable dt4 = new DataTable();
                                    dt4 = Station.APDB.ExecuteDataTable(getWoRequestSql, CommandType.Text, null);
                                    int woRequest = Int32.Parse(dt4.Rows[0]["WO_REQUEST"].ToString());

                                    int sumWo;
                                    string getCurrentWoSql = $@"select sum(EXT_QTY) as QTY FROM MES4.R_TR_SN_DETAIL
                                                       WHERE WO= '{woString}' and CUST_KP_NO=(SELECT distinct(cust_kp_no) FROM MES4.R_TR_SN
                                                       WHERE TR_SN='{SN}') and LOCATION_FLAG=1 and WORK_FLAG=1";
                                    try
                                    {
                                        DataTable dt5 = new DataTable();
                                        dt5 = Station.APDB.ExecuteDataTable(getCurrentWoSql, CommandType.Text, null);
                                        sumWo = Int32.Parse(dt5.Rows[0]["QTY"].ToString());
                                    }
                                    catch
                                    {
                                        sumWo = 0;
                                    }


                                    string getRequestSql = $@"select EXT_QTY from MES4.R_TR_SN
                                            where TR_SN='{SN}'";
                                    DataTable dt6 = new DataTable();
                                    dt6 = Station.APDB.ExecuteDataTable(getRequestSql, CommandType.Text, null);
                                    int requestWo = 0;
                                    try
                                    {
                                        requestWo = Int32.Parse(dt6.Rows[0]["EXT_QTY"].ToString());
                                    }
                                    catch
                                    {
                                        requestWo = 0;
                                    }

                                    //if (woRequest - sumWo >= requestWo)
                                    //{
                                    string sql3 = $@"select a.CUST_KP_NO,a.MFR_KP_NO,a.DATE_CODE,a.LOT_CODE,a.EXT_QTY,b.MFR_NAME as mfr_name from mes4.r_tr_sn a, mes1.c_mfr_config b where a.tr_sn='{SN}' and a.mfr_code=b.mfr_code";

                                    DataTable dt7 = new DataTable();
                                    dt7 = Station.APDB.ExecuteDataTable(sql3, CommandType.Text, null);
                                    string kp = dt7.Rows[0]["CUST_KP_NO"].ToString();
                                    int qty = Int32.Parse(dt7.Rows[0]["EXT_QTY"].ToString());


                                    string sqladd = $@"INSERT INTO MES4.R_TR_SN_DETAIL(TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,QTY,EXT_QTY,LOCATION_FLAG,WORK_FLAG,WO,STATION,MFG_EMP_NO,WORK_TIME,EMP_NO) SELECT TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE,LOT_CODE,QTY,EXT_QTY,2,'0','{woString}','{Station.Line}','KITTING' MFG_EMP_NO,SYSDATE,'{Station.LoginUser.EMP_NO}' FROM MES4.R_TR_SN WHERE TR_SN = '{SN}'";
                                    Station.APDB.ExecuteDataTable(sqladd, CommandType.Text, null);

                                    string sqladd1 = $@"INSERT INTO MES4.R_TR_SN_DETAIL(TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE, LOT_CODE,QTY,EXT_QTY,LOCATION_FLAG,WORK_FLAG, WO,STATION,MFG_EMP_NO,WORK_TIME,EMP_NO) SELECT TR_SN,CUST_KP_NO,MFR_KP_NO,MFR_CODE,DATE_CODE, LOT_CODE,QTY,EXT_QTY,1,'1','{woString}','{Station.Line}','{EMP_NO_CHECKOUT}',SYSDATE,'{Station.LoginUser.EMP_NO}' FROM MES4.R_TR_SN WHERE TR_SN = '{SN}'";
                                    Station.APDB.ExecuteDataTable(sqladd1, CommandType.Text, null);

                                    string insert_r_kt = $@"INSERT INTO MES4.R_KITTING_SCAN_DETAIL(TR_SN,CUST_KP_NO,QTY,FROM_LOCATION,TO_LOCATION,MOVE_TYPE,MOVE_REASON,MOVE_EMP,MOVE_TIME,PROCESS_FLAG) SELECT TR_SN,CUST_KP_NO,QTY,'KITTING' from_location,'{Station.Line}' to_location,'c' move_type, '999' move_reason, '{Station.LoginUser.EMP_NO}',sysdate,'T' process_flag FROM MES4.R_TR_SN WHERE TR_SN='{SN}'";
                                    Station.APDB.ExecuteDataTable(insert_r_kt, CommandType.Text, null);

                                    string sqlUpdate = $@"UPDATE MES4.R_TR_SN SET LOCATION_FLAG = '2',KITTING_FLAG = '', kitting_wo='{woString}' WHERE TR_SN = '{SN}'";
                                    Station.APDB.ExecuteDataTable(sqlUpdate, CommandType.Text, null);


                                    //string sqlUpdate1 = $@"UPDATE MES4.R_WO_REQUEST SET CHECKOUT_QTY = CHECKOUT_QTY + '{qty}' WHERE wo='{woString}'  AND CUST_KP_NO = '{kp}'";
                                    //Station.APDB.ExecuteDataTable(sqlUpdate1, CommandType.Text, null);

                                    string sqlInsert = $@"Insert into MES4.R_TR_SN_WIP(TR_SN, WO, KP_NO, MFR_KP_NO, MFR_CODE,DATE_CODE, LOT_CODE, QTY, EXT_QTY, WORK_TIME, STATION,STATION_FLAG, WORK_FLAG, EMP_NO) select TR_SN, '{woString}', cust_KP_NO, MFR_KP_NO, MFR_CODE,  DATE_CODE, LOT_CODE, QTY, EXT_QTY,sysdate, '{Station.Line}' ,'1','0','{Station.LoginUser.EMP_NO}' from mes4.r_tr_sn where tr_sn='{SN}'";
                                    Station.APDB.ExecuteDataTable(sqlInsert, CommandType.Text, null);
                                    //}
                                    //else
                                    //{
                                    //    MessageBox.Show("WO number is: " + (woRequest - sumWo) + " but need to deliver: " + requestWo + ". Please check again!");
                                    //}


                                }
                                break;
                            case 2:
                                throw new Exception("Material is Online");
                                break;
                            case 3:
                                throw new Exception("Material is MRB");
                                break;
                            default:
                                throw new Exception("This is UNKNOWN");
                                break;



                        }

                    }
                    else
                    {

                        throw new Exception("NO TR SN");
                    }

                }
                //}



            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

