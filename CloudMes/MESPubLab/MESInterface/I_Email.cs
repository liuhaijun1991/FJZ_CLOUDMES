using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface
{
    public interface I_Email
    {
        bool Init(string config);
        bool Send(EmailObj mail);
        string GetFailInf();
    }
}
