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

namespace MESCMCHost
{
    public partial class Form1 : Form
    {
        Socket _Socket;
        Thread T;
        string rec = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_Socket != null)
            {
                _Socket.Disconnect(true);
                _Socket = null;
            }
            if (T != null)
            {
                T.Abort();
                T = null;
            }

            _Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ip = IPAddress.Parse(textBox1.Text);

            _Socket.Connect(new IPEndPoint(ip, 55962));
            _Socket.SendTimeout = 2000;
            _Socket.ReceiveTimeout = 2000;
            T = new Thread(new ThreadStart(recdata));
            T.Start();
        }

        void recdata()
        {
            StringBuilder SB = new StringBuilder();
            try
            {
               
                while (true)
                {
                    byte[] temp = new byte[255];
                    try
                    {
                        _Socket.Receive(temp);
                    }
                    catch
                    {
                        continue;
                    }
                    int i = 0;
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
                    lock (rec)
                    {
                        rec += (SB.ToString().Trim() + "\r\n");
                    }
                }
            }
            catch(Exception)
            {

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (rec)
            {
                textBox3.Text = rec;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringBuilder SB = new StringBuilder();
            SB.Append(textBox2.Text);
            SB.Append((char)13);
            byte[] byteArray = System.Text.Encoding.GetEncoding(textBox4.Text).GetBytes(SB.ToString());
            _Socket.Send(byteArray);
        }
    }
}
