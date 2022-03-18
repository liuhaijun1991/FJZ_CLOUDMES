
using MESDataObject.Module;
using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESInterface.Base
{
    public class ProccessCkeckBase
    {
        public RunTimeData runtime;
        public string Name;
        public string Config;
        public static ProccessCkeckBase GetProccessCkeck(C_PROCCESS_CHECK dbdata , OleExec DB)
        {
            bool SaveFlag = false;
            ProccessCkeckBase a;
            if (dbdata.CONFIG == null || dbdata.CONFIG == "")
            {
                SaveFlag = true;
            }
            if (dbdata.CHECK_TYPE == "CheckProccessRun")
            {
                a = new CheckProccessRun();
                a.Init(dbdata);
                a.Config = dbdata.CONFIG;
                a.Name = "CheckProccessRun";


            }
            else if (dbdata.CHECK_TYPE == "CheckEventLog")
            {
                a = new CheckEventLog();
                a.Init(dbdata);
                a.Config = dbdata.CONFIG;
                a.Name = "CheckEventLog";


            }
            else
            {
                return null;
            }
            if (SaveFlag)
            {
                DB.ORM.Updateable<C_PROCCESS_CHECK>(dbdata).Where(t => t.ID == dbdata.ID).ExecuteCommand();
            }
            
            return a;
        }

        public virtual void Init(C_PROCCESS_CHECK dbdata)
        {

        }
        public virtual void Run(string ProccessName,string Config ,OleExec DB)
        {
            
        }
    }
}
