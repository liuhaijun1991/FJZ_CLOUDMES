using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface
{
    public class MessageClient : MarshalByRefObject
    {
        public static jobBase job;
        jobBase MyJob = null;

        public string FirstConnet()
        {
            if (MyJob == null && job != null)
            {
                MyJob = job;
                return "OK";
            }
            else if (job != null)
            {
                return "OK";
            }
            else
            {
                return "沒有指定當前任務!MessageClient.job==null";
            }
        }

        public void RUN()
        {
            job.Run();
        }


    }
}
