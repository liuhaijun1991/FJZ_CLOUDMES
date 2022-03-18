using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace MESCMCHost.Commamd
{
    public class LOGIN:CMCCommand
    {
        public string EMP_NO;
        public string PWD;
        public static string Language = "ENGLISH";
        public static string  BU_NAME = "HWD";


        public LOGIN()
        {
            StrCommandCode = "LOGIN";
            Actions.Add(new inputLOGIN_EMP_NO(this));
            Actions.Add(new inputLOGIN_PWD(this));
        }
    }
    public class inputLOGIN_EMP_NO : CMCCommandAction
    {
        public inputLOGIN_EMP_NO(CMCCommand cmd)
        {
            this.comm = cmd;
            this.StrActionName = "EMP_NO:";
        }
        public override void DoAction(CMC503Scanda Scanda,  object data)
        {
           
            ((LOGIN)comm).EMP_NO = data.ToString();
            comm.toNextAction();
            
        }
    }
    public class inputLOGIN_PWD : CMCCommandAction
    {
        public inputLOGIN_PWD(CMCCommand cmd)
        {
            this.comm = cmd;
            this.StrActionName = "PWD:";
        }
        public override void DoAction(CMC503Scanda Scanda,  object data)
        {

            ((LOGIN)comm).PWD = data.ToString();

            MESAPIData mesdata = new MESAPIData();
            mesdata.Class = "MESStation.MESUserManager.UserManager";
            mesdata.Function = "Login";
            mesdata.Data = new { EMP_NO = ((LOGIN)comm).EMP_NO, Password = ((LOGIN)comm).PWD, Language = MESCMCHost.Commamd.LOGIN.Language, BU_NAME = MESCMCHost.Commamd.LOGIN.BU_NAME };
            comm.CommandStatus = CommandState.WaitMesReturn;
            Scanda.MESAPI.MES_PWD = ((LOGIN)comm).PWD;
            Scanda.MESAPI.MES_USER = ((LOGIN)comm).EMP_NO;
            Scanda.Cls();
            try
            {
                Scanda.MESAPI.Login();
                Scanda.SendTextDataToCMC(((LOGIN)comm).EMP_NO + " Wellcome to Cloud MES!");
            }
            catch (Exception ee)
            {
                Scanda.SendTextDataToCMC(ee.Message);
            }
            finally
            {
                comm.toNextAction();
            }
            
            
        }
       
    }
}
