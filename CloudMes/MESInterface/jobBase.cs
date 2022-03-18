using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;

namespace MESInterface
{
    public class jobBase
    {
        MessageService Msg;
        public string JobName = "jobBase";
        
        public jobBase() 
        {
            int port = tools.GetFirstAvailablePort();
            try
            {
                ChannelServices.RegisterChannel(new TcpChannel(4949), true);
            }
            catch(Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message);
                System.Environment.Exit(0);   
            }
            
            RemotingConfiguration.RegisterWellKnownServiceType(typeof(MessageClient),
                JobName, WellKnownObjectMode.SingleCall);
            string url = "TCP://localhost:" + port.ToString() + "/jobBase";
            MessageClient.job = this;

            Msg = (MessageService)Activator.GetObject(typeof(MessageService),
                "TCP://localhost:4949/MessageService");
            Msg.InitJob(JobName,url);

        }
        virtual public void Start()
        { 
        
        }
        virtual public void Stop()
        { 
        
        }
        virtual public void Run()
        {
            System.Windows.Forms.MessageBox.Show("Runing");
        }
    }
}
