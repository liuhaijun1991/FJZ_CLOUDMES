using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;

namespace MESInterface
{
    public class MessageService : MarshalByRefObject
    {
        static int A = 0;
        static Dictionary<string, RemotingJobInfo> JOBS = new Dictionary<string, RemotingJobInfo>();
        public void showKey(string aa)
        {
            A++;
            System.Windows.Forms.MessageBox.Show(aa + "," + A.ToString());
        }

        public void InitJob(string jobName , string url )
        {
            RemotingJobInfo newJob = new RemotingJobInfo();
            newJob.Client = (MessageClient)Activator.GetObject(typeof(MessageClient),
                url);
            newJob.Name = jobName;
            if (newJob.Client != null)
            {
                newJob.Client.FirstConnet();
                JOBS.Add(jobName, newJob);
                //frmMain.Ref.jobAdd(newJob);
                newJob.Client.RUN();
            }

        }


    }
    public class RemotingJobInfo
    {
        public string Name;
        public MessageClient Client;

    }
}
