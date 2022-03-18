using MESPubLab.MesBase;
using MESPubLab.MESInterface.CRC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESPubLab.MESInterface
{
    public interface I_Crc
    {
        CrcRes Send(CrcObj crcobj);
    }
}
