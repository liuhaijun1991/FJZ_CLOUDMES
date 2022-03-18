using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace MESHelper.Plugin
{
    public class Reader_COM
    {
        static object syncObj = new object();
        static int countT = 5;
        double Weight = 0;
        public SerialPort COM;
        public delegate void ComDataDelegate(object sender, string Mesg);
        public event ComDataDelegate ComDataEvent;
        public string WeighterType { get; set; }
        public string strCom { get; set; }
        public int BaudRate { get; set; }
        public bool ReadCom { get; set; }
        string[] OLDSTRING = new string[4];
        bool TYPE2 = false;
        public Reader_COM(string _ComPortName, int _BaudRate, string _WeighterType)
        {
            COM = new SerialPort();
            strCom = _ComPortName;
            BaudRate = _BaudRate;
            WeighterType = _WeighterType;
        }
        public void Open()
        {
            for (int i = 0; i < OLDSTRING.Length; i++)
            {
                OLDSTRING[i] = "0";
            }

            //給 ComDataEvent 事件註冊 SetWeightEvent 處理方法，當事件被觸發時，調用該方法
            ComDataEvent += new ComDataDelegate(SetWeightEvent);
            COM.PortName = strCom;
            COM.Handshake = Handshake.RequestToSend;
            //給 DataReceived 事件註冊 COM_DataReceived_GetSamples 處理方法，當串口獲得數據后調用該方法
            //在 COM_DataReceived_GetSamples 方法中顯式觸發了 ComDataEvent 事件
            COM.DataReceived += new SerialDataReceivedEventHandler(COM_DataReceived_GetSamples);
            COM.BaudRate = BaudRate;
            try
            {
                ReadCom = true;
                COM.Open();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public double GetWeight()
        {
            var res = Weight;
            Weight = 0;
            return res;
        }
        public void close()
        {
            ReadCom = false;
            System.Threading.Thread.Sleep(1000);
            try
            {
                COM.DiscardInBuffer();
                COM.DiscardOutBuffer();
                COM.Dispose();
            }
            catch
            {
            }
            try
            {
                COM.DataReceived -= new SerialDataReceivedEventHandler(COM_DataReceived_GetSamples);
            }
            catch
            { }
            try
            {
                ComDataEvent -= new ComDataDelegate(SetWeightEvent);
            }
            catch
            { }
            try
            {
                COM.Handshake = Handshake.None;
                COM.Close();
            }
            catch
            { }
        }
        void SetWeightEvent(object sender, string Mesg)
        {
            //lock (syncObj)
            //{

            //    if (countT >= 0)
            //    {
                    
                    
            //        countT--;
                    
            //        return;
            //    }
            //    else
            //    {
            //        countT = 5;
            //    }
            //}
            //WeighterType=1:穩定傳輸
            //WeighterType=2:連續傳輸
            //WeighterType=3:帶ST標記

            double sample = 0;
            string[] tmp;// = Mesg.Split(new char[]{','});
            WeighterType = SocketService.WeighterType;
            if (WeighterType == "1")
            {
                //獲取稱重數據中以 , 分隔的第三部分數據，將該數據去除 +,g,空格 得到結果，設定給 Weight 屬性
                tmp = Mesg.Split(new char[] { ',' });
                try
                {
                    tmp[2] = tmp[2].Replace("+", "");
                    tmp[2] = tmp[2].Replace("g", "");
                    tmp[2] = tmp[2].Replace(" ", "");
                    sample = double.Parse(tmp[2]);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            else if (WeighterType == "2")
            {
                //去除稱重消息中的 空格,?，將以 \r 分隔出來的第一部分數據來判斷，如果為 0 那麼就將其擠入到 OLDSTRING 數組末尾 
                //如果這部分數據與 OLDSTRING 數組中的每個數據都相同（即代表已經穩定，算是平均值），那麼就設置稱重為 tmp[0]/10，
                //否則將輸入擠入到 OLDSTRING 數組末尾繼續稱重
                Mesg = Mesg.Replace(" ", "");
                Mesg = Mesg.Replace("?", "");
                tmp = Mesg.Split(new char[] { '\r' });
                tmp[0] = tmp[0].Substring(0, tmp[0].Length - 1);
                if (tmp[0] == "0")
                {
                    OLDSTRING[0] = OLDSTRING[1];
                    OLDSTRING[1] = OLDSTRING[2];
                    OLDSTRING[2] = OLDSTRING[3];
                    OLDSTRING[3] = tmp[0];
                    TYPE2 = false;
                    return;
                }
                else
                {

                }
                if (TYPE2)
                {
                    return;
                }
                if (tmp[0] == OLDSTRING[0] &&
                    tmp[0] == OLDSTRING[1] &&
                    tmp[0] == OLDSTRING[2] &&
                    tmp[0] == OLDSTRING[3])
                {
                    try
                    {
                        sample = double.Parse(tmp[0]);
                    }
                    catch
                    {
                        return;
                    }
                    sample = sample / 10;
                    TYPE2 = true;
                }
                else
                {
                    OLDSTRING[0] = OLDSTRING[1];
                    OLDSTRING[1] = OLDSTRING[2];
                    OLDSTRING[2] = OLDSTRING[3];
                    OLDSTRING[3] = tmp[0];
                    return;
                }

            }
            else if (WeighterType == "3")
            {
                //SU,NT,+   241.0g   不穩定

                //ST,NT,+   241.0g   穩定
                //ST TR +00.0003kg
                //ST TR -00.0006kg
                //ST TR -00000.3 g
                //ST TR +00000.6 g



                try
                {
                    //將稱重數據以 \r\n 分隔，遍歷每行數據，如果數據是以 ST 開頭而且 數據結尾是 g
                    //就從這行數據的第 7 位開始讀取，讀 8 個字符，並且去除掉讀出來的字符串中的 +,-，
                    //作為稱重結果
                    string[] val = Mesg.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    foreach (string item in val)
                    {
                        string headStr = item.Substring(0, 2);
                        if (headStr.Equals("ST") && item.EndsWith("g"))
                        {
                            string weightStr = item.Substring(6, 8).Trim();
                            weightStr = weightStr.Replace("+", "").Replace("-", "").Trim();
                            double w = Convert.ToDouble(weightStr);
                            sample = w;
                            break;
                        }
                    }
                }
                catch
                {
                    return;
                }
            }
            else if (WeighterType == "4")
            {
                try
                {
                    string val = Mesg.Replace("\r\n", "|").Trim();
                    if (val.EndsWith("kg")) 
                        sample = Convert.ToDouble(val.Replace("kg", "").Replace("|", "").Replace(" ", "")) * 1000;
                    else
                        sample = Convert.ToDouble(val.Replace("g", "").Replace("|", "").Replace(" ", ""));
                }
                catch
                {
                    return;
                }
            }
            else if (WeighterType == "5")
            {
                try
                {
                    string val = Mesg.Replace("\r\n", "").Trim().Split(':')[1];
                    if (val.EndsWith("kg"))
                    {
                        val = val.Substring(0, val.IndexOf("kg"));
                    }
                    else
                    {
                        val = val.Substring(0, val.IndexOf("g"));
                    }
                    if (val.EndsWith("kg"))
                        sample = Convert.ToDouble(val.Replace("kg", "")) * 1000;
                    else
                        sample = Convert.ToDouble(val.Replace("g", ""));
                }
                catch
                {
                    return; 
                }
            }
            else if (WeighterType == "6")
            {

                //FJZ  Juniper 
                //Net         0.4570 kg
                try
                {
                    string val = Mesg.Replace("\r\n", "").Replace("Net","").Trim();
                    //if (val.EndsWith("kg"))
                    //{
                    //    val = val.Substring(0, val.IndexOf("kg"));
                    //}
                    //else
                    //{
                    //    val = val.Substring(0, val.IndexOf("g"));
                    //}


                    if (val.EndsWith("kg"))
                        sample = Convert.ToDouble(val.Replace("kg", "")) * 1000;
                    else
                        sample = Convert.ToDouble(val.Replace("g", ""));
                }
                catch
                {
                    return;
                }
            }
            else if (WeighterType == "7")
            {

                //FJZ  Juniper 
                try
                {
                    string R = "(\\d+.\\d+) kg G";
                    //string val = Mesg.Replace("\r\n", "").Replace("Net", "").Trim();
                    var m = Regex.Match(Mesg, R);


                    if (m.Success == true)
                        sample = double.Parse( m.Groups[1].Value);
                    else
                        return;
                }
                catch
                {
                    return;
                }
            }
            else if (WeighterType == "8")
            {

                //Regex 
                try
                {
                    string R = SocketService.WeighterRegex;
                    //string val = Mesg.Replace("\r\n", "").Replace("Net", "").Trim();
                    var m = Regex.Match(Mesg, R);


                    if (m.Success == true)
                        sample = double.Parse(m.Groups[1].Value);
                    else
                        return;
                }
                catch
                {
                    return;
                }
            }


            Weight = sample;
            SocketService.WeightData = Weight.ToString();
            SocketService.ComMsg = Mesg;
            //ComDataEvent -= new ComDataDelegate(SetWeightEvent);
        }
        /// <summary>
        /// 當稱重數據返回到指定的稱重端口（指定的波特率），使用 SerialPort 的 Read 方法將數據讀入到一個字符數組中
        /// 然後將字符數組中的字符拼接成一個字符串，顯式觸發 ComDataEvent 事件，傳入拼接出來的字符串
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void COM_DataReceived_GetSamples(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                char[] buffer = new char[1000];
                if (ReadCom)
                {
                    if (e.EventType == SerialData.Chars)
                    {
                        System.Threading.Thread.Sleep(200);
                        COM.Read(buffer, 0, 1000);

                        string s = "";
                        int i = 0;
                        while (buffer[i] != '\0')
                        {
                            s += buffer[i].ToString();
                            i++;
                            if (i == 999)
                            {
                                break;
                            }
                        }

                        ComDataEvent(this, s);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

    }
}
