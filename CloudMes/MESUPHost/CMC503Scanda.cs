using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using MESCMCHost.UI;

namespace MESCMCHost
{
    public class CMC503Scanda
    {
        public string HostName;
        public string IP;
        public string Station;
        public string Line;
        public string MESWebSocketConnStr = "ws://10.120.154.223/ReportService";
        public string Token = "";
        public string txtLog = "";
        public bool isConnectToCMC = false;
        public bool CMCConnectState = false;

        public delegate void MESCallBack(CMC503Scanda Scanda, object data);
        public CMC503_UI MAIN_UI = new CMC503_UI();

        public Socket CMCSocket = null;
        public Socket ClientSocket = null;
        Queue<string> CMCReceiveTxTData = new Queue<string>();
        Thread ReceiveCMCDataThread;
        Thread DataProccessThread;
        bool RecCMCDATA = true;
        public string Id = "";
        public MESAPIClient MESAPI;
        Commamd.Station cmd = new Commamd.Station();

        Random rand = new Random();

        DateTime lastTestConnect = DateTime.MinValue;
        //Queue<CMCCommand> CommandQueue = new Queue<CMCCommand>();
        public Stack<CMCCommand> CommandStack = new Stack<CMCCommand>();
        CMCCommand CurrComm = null;

        Dictionary<string, MESCallBack> dirMesCallback = new Dictionary<string, MESCallBack>();
        Thread ConnectCMCThread;// = new Thread(new ThreadStart(ConnectToCMCFunction));

        bool FirstTimeConnect = true;
        bool FirstTimeStation = true;


        public void AddCommand(CMCCommand CMD)
        {
            CommandStack.Push(CMD);
            CurrComm = CMD;
        }

        public CMC503Scanda(string _MESWebSocketConnStr, string id, string ip, string station, string line)
        {
            MESWebSocketConnStr = _MESWebSocketConnStr;
            MESAPI = new MESAPIClient(MESWebSocketConnStr);
            Id = id;
            MESAPI.MES_USER = id;
            MESAPI.MES_PWD = "11111";
            this.IP = ip;
            this.Line = line;
            this.Station = station;
            // MESAPI.LoginTest();
            //InitStation();
            isConnectToCMC = true;
            MAIN_UI.scanda = this;
            ConnectCMCThread = new Thread(new ThreadStart(ConnectToCMCFunction));
            ConnectCMCThread.Start();

        }

        public CMC503Scanda()
        {

            //MESWebSocketConnStr = _MESWebSocketConnStr;
            //MESAPI = new MESAPIClient(MESWebSocketConnStr);
            MAIN_UI.scanda = this;
            ConnectCMCThread = new Thread(new ThreadStart(GetData));
            ConnectCMCThread.Start();

        }

        public void GetData()
        {
            while (true)
            {
                if (this.ClientSocket != null)
                {
                    string Str = "";
                    byte[] RecByte = new byte[4096];
                    int bytes = this.ClientSocket.Receive(RecByte);
                    byte[] _RecByte = new byte[bytes];
                    for (int i = 0; i < bytes; i++)
                    {
                        _RecByte[i] = RecByte[i];
                    }
                    Str += Encoding.ASCII.GetString(_RecByte);
                    if (Str.Length > 0 && Str != "HELLO")
                    {
                        //Str = Str.Replace("\\r\\n","^");
                        //string[] strs = Str.Split('^');

                        //for (int y = 0; y <strs.Length; y++)
                        //{
                        //    lock (txtLog)
                        //    {
                        //        txtLog += "\r\n" + strs[y];
                        //    }
                        //    this.inputStation(strs[y]);
                        //    Thread.Sleep(500);
                        //}
                        lock (txtLog)
                        {
                            txtLog += "\r\n" + Str;
                        }
                        this.inputStation(Str);
                    }
                }
            }
        }

        public void SendData(string Data)
        {
            lock (txtLog)
            {
                txtLog += "\r\n" + Data;
            }
            byte[] SendByte = Encoding.ASCII.GetBytes(Data);
            if (this.ClientSocket != null)
                this.ClientSocket?.Send(SendByte, SendByte.Length, 0);
            else
            {
                lock (txtLog)
                {
                    txtLog += "已经断开连接！";
                }
            }

        }

        public void InitStation()
        {
            Commamd.inputStationName isn = new Commamd.inputStationName(cmd);
            isn.DoActionTest(this, this.Station);
        }

        public void UndoStation(string data)
        {
            Commamd.inputStationInput isn = new Commamd.inputStationInput(cmd, "");
            isn.DoUNDOCMC(this, data);
        }

        public void inputStation(string SendText)
        {
            Commamd.inputStationInput isn = new Commamd.inputStationInput(cmd, "");
            isn.DoActionTest(this, SendText);
        }

        public void FreeMe()
        {
            isConnectToCMC = false;
            if (ReceiveCMCDataThread != null)
            {
                try
                {
                    ReceiveCMCDataThread.Abort();
                }
                catch
                { }
                ReceiveCMCDataThread = null;
            }

            if (DataProccessThread != null)
            {
                try
                {
                    DataProccessThread.Abort();
                }
                catch
                { }
                DataProccessThread = null;
            }

            if (ConnectCMCThread != null)
            {
                try
                {
                    ConnectCMCThread.Abort();
                }
                catch
                { }
                ConnectCMCThread = null;
            }

            try
            {
                MESAPI.DisConnect();
            }
            catch
            { }

        }

        public void DisConnectMESAPI()
        {
            MESAPI?.DisConnect();
        }

        public void ConnectToCMCFunction()
        {
            while (true)
            {
                if (isConnectToCMC)
                {

                    IPAddress ip = IPAddress.Parse(IP);
                    if (CMCSocket == null)
                    {
                        CMCSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                    try
                    {
                        TestConnect();
                        Thread.Sleep(100);
                    }
                    catch (Exception)
                    {
                        CMCConnectState = false;
                    }
                    if ((DateTime.Now - lastTestConnect).TotalSeconds >= 3)
                    {
                        CMCConnectState = false;
                        try
                        {
                            CMCSocket.Disconnect(false);
                        }
                        catch (Exception)
                        {

                        }
                    }
                    if (CMCConnectState == false)
                    {
                        try
                        {
                            CMCSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            CMCSocket.Connect(new IPEndPoint(ip, 55962));
                            CMCConnectState = true;


                            if (DataProccessThread == null)
                            {
                                DataProccessThread = new Thread(new ThreadStart(ProccessCMCData));
                                DataProccessThread.Start();
                            }
                            if (ReceiveCMCDataThread == null)
                            {
                                ReceiveCMCDataThread = new Thread(new ThreadStart(ReceiveCMCData));
                                ReceiveCMCDataThread.Start();
                            }
                            Thread.Sleep(5000);
                            lastTestConnect = DateTime.Now;
                            if (FirstTimeConnect)
                            {
                                FirstTimeConnect = false;
                                SendTextDataToCMC("EMP?");
                            }

                        }
                        catch (Exception)
                        {
                            CMCConnectState = false;
                        }
                    }
                }
                else
                {

                    if (ReceiveCMCDataThread != null)
                    {
                        try
                        {
                            ReceiveCMCDataThread.Abort();
                        }
                        catch
                        { }
                        ReceiveCMCDataThread = null;
                    }

                    if (DataProccessThread != null)
                    {
                        try
                        {
                            DataProccessThread.Abort();
                        }
                        catch
                        { }
                        DataProccessThread = null;
                    }

                    if (CMCSocket != null && CMCSocket.Connected == true)
                    {
                        CMCSocket.Disconnect(true);
                        CMCConnectState = false;
                    }
                }
                Thread.Sleep(1000);
            }

        }

        /// <summary>
        /// 连接到远程CMC
        /// </summary>
        public bool ConnectToCMC()
        {
            DisConnectToCMC();
            RecCMCDATA = true;

            CMCSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(IP);
            try
            {
                CMCSocket.Connect(new IPEndPoint(ip, 55962));
            }
            catch
            {
                return false;
            }
            CMCSocket.SendTimeout = 2000;
            CMCSocket.ReceiveTimeout = 2000;
            ReceiveCMCDataThread = new Thread(new ThreadStart(ReceiveCMCData));
            ReceiveCMCDataThread.Start();

            if (DataProccessThread == null)
            {
                DataProccessThread = new Thread(new ThreadStart(ProccessCMCData));
                DataProccessThread.Start();
            }
            isConnectToCMC = true;
            return true;


        }

        public void ProccessCMCData()
        {
            while (RecCMCDATA)
            {
                string Data = "";
                if (CMCReceiveTxTData.Count != 0 && CMCReceiveTxTData.Peek().ToUpper() == "UNDO")
                {
                    if (MESAPI.Token != "")
                    {
                        UndoStation(CMCReceiveTxTData.Peek().ToUpper());
                        MESAPI.Token = "";
                    }
                    SendTextDataToCMC("EMP?");
                    lock (CMCReceiveTxTData)
                    {
                        CMCReceiveTxTData.Clear();
                    }
                    continue;
                }

                if (CurrComm != null && CurrComm.CommandStatus == "WaitMesReturn")
                {
                    Thread.Sleep(100);
                    continue;
                }

                lock (CMCReceiveTxTData)
                {
                    if (CMCReceiveTxTData.Count >= 1)
                    {
                        Data = CMCReceiveTxTData.Dequeue();
                        lock (txtLog)
                        {
                            txtLog += "\r\n" + Data;
                        }
                        if (MESAPI.Token == "" || MESAPI.Token == null)
                        {
                            this.MESAPI.MES_USER = Data;
                            MESAPI.LoginTest1();
                            InitStation();
                            MESAPI.ReSetApStation(Id, Data);
                            SendTextDataToCMC("SN:");
                        }
                        else if (Data.Length >= 1)
                        {
                            inputStation(Data);
                            // this.SendTextDataToCMC("SET STATION:" + this.Station);
                        }
                    }
                }
                if (RecCMCDATA)
                {
                    Thread.Sleep(100);
                }
            }
        }

        /// <summary>
        /// 将CMC连接断开
        /// </summary>
        public void DisConnectToCMC()
        {
            RecCMCDATA = false;
            Thread.Sleep(50);
            if (CMCSocket != null)
            {
                try
                {
                    CMCSocket.Disconnect(true);
                }
                catch
                { }

            }
            //Thread.Sleep(50);
            if (ReceiveCMCDataThread != null)
            {
                ReceiveCMCDataThread.Abort();
                ReceiveCMCDataThread = null;
            }
            //Thread.Sleep(50);
            if (DataProccessThread != null)
            {
                DataProccessThread.Abort();
                DataProccessThread = null;
            }

            //Thread.Sleep(50);
            CMCSocket = null;
            isConnectToCMC = false;
            CMCConnectState = false;


        }
        /// <summary>
        /// 给CMC屏幕发送信息
        /// </summary>
        /// <param name="Data">汉字只支持繁体中文</param>
        public void SendTextDataToCMC(string Data)
        {
            StringBuilder SB = new StringBuilder();
            if (!Data.StartsWith(" "))
            {
                SB.Append(" " + Data);
            }
            else
            {
                SB.Append(Data);
            }
            SB.Append((char)13);
            byte[] byteArray = System.Text.Encoding.GetEncoding("Big5").GetBytes(SB.ToString());
            CMCSocket.Send(byteArray);

            lock (txtLog)
            {
                txtLog += "\r\n" + Data;
            }
        }

        /// <summary>
        /// 给CMC串口发送信息
        /// </summary>
        /// <param name="Data">汉字只支持繁体中文</param>
        public void SendDataToCMCCom(string Data)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append(Data);

            byte[] byteArray = System.Text.Encoding.GetEncoding("Big5").GetBytes(SB.ToString());
            CMCSocket.Send(new byte[] { 0x1b, 0x07 });
            CMCSocket.Send(byteArray);
            CMCSocket.Send(new byte[] { 0x0d });

            lock (txtLog)
            {
                txtLog += "\r\nCOM>>" + Data;
            }
        }
        /// <summary>
        /// 给CMC串口发送信息
        /// </summary>
        /// <param name="Data"></param>
        public void SendDataToCMCCom(byte[] Data)
        {
            CMCSocket.Send(new byte[] { 0x1b, 0x07 });
            CMCSocket.Send(Data);
            CMCSocket.Send(new byte[] { 0x0d });
        }
        /// <summary>
        /// 接受CMC传送的文本
        /// </summary>
        public void ReceiveCMCData()
        {
            StringBuilder SB = new StringBuilder();

            try
            {
                while (RecCMCDATA)
                {
                    string rec = "";
                    byte[] temp = new byte[255];
                    try
                    {
                        CMCSocket.Receive(temp);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    int i = 0;
                    //0x1b 0x10 0x0d

                    if ((temp[0] == 0x1b && temp[1] == 0x10 && temp[2] == 0x0d)
                        || (temp[0] == 0x1b && temp[1] == 0x11 && temp[2] == 0x0d))
                    {
                        lastTestConnect = DateTime.Now;
                    }

                    SB.Clear();
                    if (temp[i] == 27)
                    {
                        continue;
                    }
                    while (true)
                    {
                        if (temp[i] == 0)
                        {
                            break;
                        }
                        SB.Append((char)temp[i]);
                        i++;
                    }
                    rec += (SB.ToString().Trim());
                    lock (CMCReceiveTxTData)
                    {
                        CMCReceiveTxTData.Enqueue(rec);
                    }
                    if (RecCMCDATA)
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception)
            {
                //ConnectToCMC();
            }
        }
        public void TestConnect()
        {
            CMCSocket?.Send(new byte[] { 0x1b, 0x0e });
        }
        /// <summary>
        /// 清屏
        /// </summary>
        public void Cls()
        {
            CMCSocket?.Send(new byte[] { 0x1b, 0x60 });
        }
    }

}
