using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;

namespace MESCMCHost.UI
{
    public partial class CMC503_UI : UserControl
    {
        public CMC503Scanda scanda;
        public CMC503_UI()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (scanda != null)
            {
                labIP.Text = scanda.IP;
                labLine.Text = scanda.Line;
                labStation.Text = scanda.Station;
                try
                {
                    labUser.Text = scanda.MESAPI.MES_USER;
                }
                catch
                { }

                if (scanda.CMCSocket != null)
                {
                    if (IsSocketConnected(scanda.CMCSocket) == true)
                    {
                        labConnectState.Text = "已连接";
                        labConnectState.ForeColor = Color.Green;
                    }
                    else
                    {
                        labConnectState.ForeColor = Color.Purple;
                        labConnectState.Text = "断开连接";
                        scanda.ClientSocket = null;
                    }
                }





            }
        }

        public string GetStringIp(Socket _socket)
        {
            string Ip = ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
            return Ip;
        }

        static bool IsSocketConnected(Socket sc)
        {
            return !((sc.Poll(1000, SelectMode.SelectRead) && (sc.Available == 0)) || !sc.Connected);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (btnStart.Text == "Start")
            {
                scanda.isConnectToCMC = true;
                btnStart.Text = "Stop";
            }
            else
            {
                scanda.isConnectToCMC = false;
                btnStart.Text = "Start";
            }
        }

        private void CMC503_UI_Load(object sender, EventArgs e)
        {
            labConnectState.ForeColor = Color.Purple;
            labConnectState.Text = "断开连接";
        }
    }
}
