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
        public string MESWebSocketConnStr = "ws://127.0.0.1:2130/ReportService";
        public string Token = "";
        public string txtLog = "";
        public bool isConnectToCMC = false;
        public bool CMCConnectState = false;

        public delegate void MESCallBack(CMC503Scanda Scanda, object data);
        public CMC503_UI MAIN_UI = new CMC503_UI();

        Socket CMCSocket=null;
        Queue<string> CMCReceiveTxTData = new Queue<string>();
        Thread ReceiveCMCDataThread;
        Thread DataProccessThread;
        bool RecCMCDATA = true;

        public MESAPIClient MESAPI;

        Random rand = new Random();

        DateTime lastTestConnect = DateTime.MinValue;
        //Queue<CMCCommand> CommandQueue = new Queue<CMCCommand>();
        public Stack<CMCCommand> CommandStack = new Stack<CMCCommand>();
        CMCCommand CurrComm =null;

        Dictionary<string, MESCallBack> dirMesCallback = new Dictionary<string, MESCallBack>();
        Thread ConnectCMCThread;// = new Thread(new ThreadStart(ConnectToCMCFunction));

        bool FirstTimeConnect = true;
        bool FirstTimeStation = true;


        public void AddCommand(CMCCommand CMD)
        {
            CommandStack.Push(CMD);
            CurrComm = CMD;
        }

        public CMC503Scanda(string _MESWebSocketConnStr)
        {
            MESWebSocketConnStr = _MESWebSocketConnStr;
            MESAPI = new MESAPIClient(MESWebSocketConnStr);
            MAIN_UI.scanda = this;
            ConnectCMCThread = new Thread(new ThreadStart(ConnectToCMCFunction));
            ConnectCMCThread.Start();

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
                    catch(Exception)
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
                                CMCReceiveTxTData.Enqueue("login");
                            }

                        }
                        catch(Exception)
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

                    if (CMCSocket!=null && CMCSocket.Connected == true)
                    {
                        CMCSocket.Disconnect(true);
                        CMCConnectState = false;
                    }
                }
                Thread.Sleep(1000);
            }
            
        }

        public void ConnectToTMNFunction()
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
                                CMCReceiveTxTData.Enqueue("login");
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
                    this.CurrComm = null;
                    CommandStack.Clear();
                    //this.CommandQueue.Clear();
                    SendTextDataToCMC("Pls Enter Cmd Code!");
                    lock (CMCReceiveTxTData)
                    {
                        CMCReceiveTxTData.Clear();
                    }
                    continue;
                }

                if (CurrComm!=null && CurrComm.CommandStatus == "WaitMesReturn")
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
                        if (CurrComm == null && Data.Length>=1)
                        {
                            if (Data.ToUpper() == "LOGIN")
                            {
                                Commamd.LOGIN cmd = new Commamd.LOGIN();
                                CommandStack.Push(cmd);
                                //CommandQueue.Enqueue(cmd);
                                CurrComm = cmd;
                                SendTextDataToCMC(cmd.Actions[cmd.CurrActionIndex].StrActionName);

                            }else if (Data.ToUpper() == "SETLINE")
                            {
                                Commamd.SetLine cmd = new Commamd.SetLine();
                                CommandStack.Push(cmd);
                                //CommandQueue.Enqueue(cmd);
                                CurrComm = cmd;
                                SendTextDataToCMC(cmd.Actions[cmd.CurrActionIndex].StrActionName);

                            }
                            else if (Data.ToUpper() == "STATION")
                            {
                                Commamd.Station cmd = new Commamd.Station();
                                CommandStack.Push(cmd);
                                //CommandQueue.Enqueue(cmd);
                                CurrComm = cmd;
                                SendTextDataToCMC(cmd.Actions[cmd.CurrActionIndex].StrActionName);

                            }
                            else
                            {
                                SendTextDataToCMC("Error Input Pls input Cmd code!");
                            }

                        }
                        else if(CurrComm != null && Data.Length >= 1)
                        {
                            CurrComm.Actions[CurrComm.CurrActionIndex].DoAction(this, Data);
                            if (CurrComm.CommandStatus == CommandState.CommandStart)
                            {
                                SendTextDataToCMC(CurrComm.Actions[CurrComm.CurrActionIndex].StrActionName);
                            }
                            else if (CurrComm.CommandStatus == CommandState.CommandEnd)
                            {
                                if (CurrComm.GetType() == typeof(MESCMCHost.Commamd.LOGIN) && FirstTimeStation)
                                {
                                    FirstTimeStation = false;
                                    CMCReceiveTxTData.Enqueue("station");
                                    this.SendTextDataToCMC("SET STATION:" + this.Station);
                                    CMCReceiveTxTData.Enqueue(this.Station);
                                }
                                //CommandQueue.Dequeue();
                                CommandStack.Pop();
                                if (CommandStack.Count > 0)
                                {
                                    CurrComm = CommandStack.First();
                                    

                                    SendTextDataToCMC(CurrComm.Actions[CurrComm.CurrActionIndex].StrActionName);
                                    
                                }
                                else
                                {
                                    SendTextDataToCMC("Pls Enter Cmd Code!");
                                    CurrComm = null;
                                }
                                
                            }
                            try
                            {
                                if (CurrComm != null&& CurrComm.Actions[CurrComm.CurrActionIndex].AutoRunData != null)
                                {
                                    CMCReceiveTxTData.Reverse();
                                    CMCReceiveTxTData.Enqueue(CurrComm.Actions[CurrComm.CurrActionIndex].AutoRunData.ToString());
                                    CMCReceiveTxTData.Reverse();
                                }
                            }
                            catch
                            { }

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
            if (Data.IndexOf(" ") < 0)
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
        /// 给CMC串口发送信息
        /// </summary>
        /// <param name="Data"></param>
        public void SetOutput1(bool tag)
        {
            CMCSocket.Send(new byte[] { 0x1b, 0x09 });
            if (tag)
            {
                CMCSocket.Send(new byte[] { 0x01 });
            }else
            {
                CMCSocket.Send(new byte[] { 0x00 });
            }
            //CMCSocket.Send(new byte[] { 0x0d });
        }

        /// <summary>
        /// 给CMC串口发送信息
        /// </summary>
        /// <param name="Data"></param>
        public void SetOutput2(bool tag)
        {
            CMCSocket.Send(new byte[] { 0x1b, 0x14 });
            if (tag)
            {
                CMCSocket.Send(new byte[] { 0x01 });
            }
            else
            {
                CMCSocket.Send(new byte[] { 0x00 });
            }
            //CMCSocket.Send(new byte[] { 0x0d });
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
                    catch(Exception)
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
