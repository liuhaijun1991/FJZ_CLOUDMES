
using System;
using System.Collections.Generic;
using System.Threading;
using MESDataObject.Common;
using MESDBHelper;
using static MESDataObject.Common.EnumExtensions;

namespace MESPubLab.MESStation
{
    public class UIInputData
    {
        public string placeholder;
        /// <summary>
        /// Input控件ID,不能有空格或者奇奇怪怪的字符; / Input control ID, there can be no spaces or strange characters;
        /// 黨UIInputType.Alart時為子標題; (我不明白/I don't understand)
        /// 必選; / required
        /// </summary>
        public string Name;
        /// <summary>
        /// UIInputType.Confirm時為BTN顯示的值; The value displayed for BTN when UIInputType.Confirm;
        /// UIInputType.String 為文本前的Label值 / UIInputType.String is the Label value before the text
        /// </summary>
        public string Message;
        /// <summary>
        /// 未成功提交(包括關閉,取消)返回的錯誤信息 / Error message returned from unsuccessful submission (including close, cancel)
        /// </summary>
        public string ErrMessage;
        /// <summary>
        /// Callback error message 
        /// </summary>
        public string CBMessage;
        /// <summary>
        /// 彈窗標題 / Popup title
        /// 可選 / Optional
        /// </summary>
        public string Tittle;
        /// <summary>
        /// 控制右上角關閉按鈕; / Control the close button in the upper right corner
        /// true=>中斷執行, / true=>Interrupt execution
        /// false=>關閉后繼續執行 / false=>Continue execution after closing
        /// 默認為true / The default is true
        /// 可選 / Optional
        /// </summary>
        public bool MustConfirm = true;
        /// <summary>
        /// 控制顯示區域的長寬
        /// 默認為["30%","45%"]
        /// 可選
        /// </summary>
        public string[] UIArea = new string[] { "30%", "45%" };
        /// <summary>
        /// 彈出窗類型 / Control the length and width of the display area
        /// 必選 / required
        /// </summary>
        public UIInputType Type;
        /// <summary>
        /// 超時時間 / overtime time
        /// 默認60000ms / Default 60000ms
        /// </summary>
        public int Timeout = 60000;//default 1 minute
        /// <summary>
        /// 彈窗style / Pop-up style
        /// 默認為IconType.None / The default is IconType.None
        /// 可選 / Optional
        /// </summary>
        public IconType IconType = IconType.None;
        public List<string> ValueForUse = new List<string>();
        /// <summary>
        /// output控件組 / output control group
        /// 可選 / Optional
        /// </summary>
        public List<DisplayOutPut> OutInputs = new List<DisplayOutPut>();
        public object ReturnData;
        public delegate bool DelegateDone(string res);
        /// <summary>
        /// 播放声音的选项 / Options for playing sound
        /// </summary>
        public PlaySound SoundType = PlaySound.None;
        public string SoundURL = "";
        /// <summary>
        /// 當在工站中使用時lockObj請輸入Station / When using lockObj in the station, please enter Station
        /// </summary>
        /// <param name="API"></param>
        /// <param name="uiInput"></param>
        /// <param name="lockObj"></param>
        /// <returns></returns>
        public object GetUiInput(MesAPIBase API, UIInput uiInput, I_LockThread lockObj = null, DelegateDone callBack = null)
        {
            if (lockObj != null)
            {
                lockObj.LockTread = Thread.CurrentThread;
            }
            DateTime callTime = DateTime.Now;
            if (lockObj != null)
                lockObj.AddTIMEOUT(Timeout / 1000);

            var ret = API.CallUIFunctionSync(this, uiInput, Timeout, lockObj, callBack);
            //var retO = Newtonsoft.Json.JsonConvert.DeserializeObject<UIInputData>(ret.ToString());
            try
            {

                var timeLeft = Timeout / 1000 - (DateTime.Now - callTime).TotalSeconds;
                lockObj.SubTIMEOUT((int)timeLeft);
            }
            catch
            {

            }
            if (lockObj != null)
            {
                lockObj.LockTread = null;
            }

            return ret;
        }

    }


    public enum UIInput
    {
        /// <summary>
        /// 普通彈窗
        /// </summary>
        [EnumName("UIInput")]
        Normal = 1,
        /// <summary>
        /// KeyPart 彈窗
        /// </summary>
        [EnumName("KeyPart")]
        KeyPart = 2,
        [EnumName("CanPrint")]
        CanPrint = 3,
        /// <summary>
        /// Show Progress window,Backgroud no stop
        /// </summary>
        [EnumName("Progress")]
        Progress = 4
    }

    public enum UIInputType
    {
        /// <summary>
        /// 普通文本框
        /// </summary>
        [EnumName("String")]
        String = 1,
        [EnumName("YesNo")]
        YesNo = 2,
        [EnumName("Password")]
        Password = 3,
        [EnumName("Select")]
        Select = 4,
        [EnumName("Table")]
        Table = 5,
        /// <summary>
        /// 確認框,需要點擊按鈕確認才能繼續,取消或者關閉都將中斷
        /// </summary>
        [EnumName("Confirm")]
        Confirm = 6,
        /// <summary>
        /// 提示框,只要關閉UI都將繼續執行
        /// </summary>
        [EnumName("Alart")]
        Alart = 7,
        /// <summary>
        /// 提示框,只要關閉UI都將繼續執行
        /// </summary>
        [EnumName("Weight")]
        Weight = 8
    }

    public enum UIOutputType
    {
        [EnumName("Text")]
        Text,
        [EnumName("Table")]
        Table,
        [EnumName("Select")]
        Select,
        [EnumName("Password")]
        Password,
        [EnumName("TextArea")]
        TextArea
    }

    public enum IconType
    {
        [EnumName("Message")]
        Message = 1,
        [EnumName("Alert")]
        Alert = 2,
        [EnumName("Warning")]
        Warning = 4,
        [EnumName("None")]
        None = 0
    }

    public enum StationLayerReturnType
    {
        [EnumName("None")]
        None = 0,
        [EnumName("Cancel")]
        Cancel = 1,
        [EnumName("Reply")]
        Reply = 2,
        [EnumName("Close")]
        Close = 4,
    }
    public enum PlaySound
    {
        [EnumName("None")]
        None = 0,
        [EnumName("Pass")]
        Pass = 1,
        [EnumName("Fail")]
        Fail = 2,
        [EnumName("URL")]
        URL = 4,
    }
}
