using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MESDataObject;
using MESDataObject.Module;
using MESStation;
using MESPubLab.MESStation;
using MESDBHelper;
using System.Net;

namespace MESStation.Interface
{
    public class Interface:MesAPIBase
    {
        T_C_INTERFACE C_Interface;
        //Row_C_INTERFACE Row_C_Interface;

        T_C_PROGRAM_SERVER C_Program_Server;
        //Row_C_PROGRAM_SERVER Row_C_Program_Server;

        T_R_SYNC_LOCK R_Sync_Lock;
        //Row_R_SYNC_LOCK Row_R_Sync_Lock;

        //OleExec Sfcdb;
        //string IP = "";

        protected APIInfo FInterface = new APIInfo()
        {
            FunctionName = "GetInterfaceStatus",
            Description = "Get Interface Status",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "PROGRAM", InputType = "STRING", DefaultValue = "INTERFACE"}
            },
            Permissions = new List<MESPermission>()//不需要任何權限
        };

        public Interface()
        {
            this.Apis.Add(FInterface.FunctionName, FInterface);
        }

        public Dictionary<string, string> GetInterfacePara()
        {
            Dictionary<string, string> Dic_Interface = new Dictionary<string, string>();
            Dic_Interface.Add("Start", "0");
            Dic_Interface.Add("CurrentDate", "2017-12-21");
            Dic_Interface.Add("NextDate", "2017-12-22");
            Dic_Interface.Add("ConsoleIP", "127.0.0.1");

            return Dic_Interface;
        }
        public static void WriteIntoMESLog(OleExec SFCDB, string bu, string programName, string className, string functionName, string logMessage, string logSql, string editEmp)
        {
            //OleExec SFCDB = new OleExec(db, false);
            T_R_MES_LOG mesLog = new T_R_MES_LOG(SFCDB, DB_TYPE_ENUM.Oracle);
            string id = mesLog.GetNewID(bu, SFCDB);
            Row_R_MES_LOG rowMESLog = (Row_R_MES_LOG)mesLog.NewRow();
            rowMESLog.ID = id;
            rowMESLog.PROGRAM_NAME = programName;
            rowMESLog.CLASS_NAME = className;
            rowMESLog.FUNCTION_NAME = functionName;
            rowMESLog.LOG_MESSAGE = logMessage;
            rowMESLog.LOG_SQL = logSql;
            rowMESLog.EDIT_EMP = editEmp;
            rowMESLog.EDIT_TIME = System.DateTime.Now;
            SFCDB.ThrowSqlExeception = true;
            SFCDB.ExecSQL(rowMESLog.GetInsertString(DB_TYPE_ENUM.Oracle));
        }
        /// <summary>
        /// 獲取Interface 信息
        /// </summary>
        /// <param name="requestValue"></param>
        /// <param name="Data"></param>
        /// <param name="StationReturn"></param>
        public void GetInterfaceStatus(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
         {
            OleExec Sfcdb = null;
            try
            {
                string Program_Name = Data["PROGRAM"].ToString();
                Dictionary<string, object> ListInterfaceObj = new Dictionary<string, object>();

                List<C_INTERFACE> ListInterface = new List<C_INTERFACE>();
                List<C_PROGRAM_SERVER> ListProgramServer = new List<C_PROGRAM_SERVER>();
                Sfcdb = this.DBPools["SFCDB"].Borrow();
                C_Interface = new T_C_INTERFACE(Sfcdb, DB_TYPE_ENUM.Oracle);
                ListInterface = C_Interface.GetInterfaceStatus(BU, IP, Program_Name, "ALL", LoginUser.EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);

                ListInterfaceObj.Add(Program_Name, ListInterface);

                C_Program_Server = new T_C_PROGRAM_SERVER(Sfcdb, DB_TYPE_ENUM.Oracle);
                ListProgramServer = C_Program_Server.GetProgramServer(BU, IP, Program_Name, "ALL", LoginUser.EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);
                ListInterfaceObj.Add(Program_Name + "_SERVER", ListProgramServer);

                StationReturn.Data = ListInterfaceObj;
                StationReturn.Status = StationReturnStatusValue.Pass;
            }catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (Sfcdb != null)
                {
                    this.DBPools["SFCDB"].Return(Sfcdb);
                }
            }
        }

        /// <summary>
        /// 檢查Interface Item 是否被使用
        /// </summary>
        /// <param name="ProgramName"></param>
        /// <param name="ItemName"></param>
        /// <param name="Sfcdb"></param>
        /// <returns></returns>
        public bool CheckInterfaceRun(string ProgramName, string ItemName, OleExec Sfcdb)
        {
            bool InUse = false;
            bool Flag = true;
            string EMP_NO = LoginUser.EMP_NO;
            
            //OleExec Sfcdb = this.DBPools["SFCDB"].Borrow();
            C_Interface = new T_C_INTERFACE(Sfcdb, DB_TYPE_ENUM.Oracle);
            C_Program_Server = new T_C_PROGRAM_SERVER(Sfcdb, DB_TYPE_ENUM.Oracle);
            IP = Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault<IPAddress>(a => a.AddressFamily.ToString().Equals("InterNetwork")).ToString();
            //C_INTERFACE C_interface = (C_INTERFACE)C_Interface.CHECK_IP(BU, IP, "INTERFACE", ItemName, LoginUser.EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);
            C_PROGRAM_SERVER C_program_server = (C_PROGRAM_SERVER)C_Program_Server.CHECK_IP(BU, IP, "INTERFACE", ItemName, EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);

            T_R_SYNC_LOCK R_Syn_Lock = new T_R_SYNC_LOCK(Sfcdb, DB_TYPE_ENUM.Oracle);
            InUse = R_Syn_Lock.Check_SYNC_Lock(BU, IP, ProgramName, ItemName, EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);

            if (C_program_server == null || InUse)
            {
                Flag = false;
            }

            return Flag;
        }

        /// <summary>
        /// 增加鎖，當一個在執行時，其它不允許執行
        /// </summary>
        /// <param name="ProgramName"></param>
        /// <param name="ItemName"></param>
        /// <param name="Sfcdb"></param>
        /// <returns></returns>
        public bool LockItem(string ProgramName, string ItemName, OleExec Sfcdb)
        {
            string EMP_NO = LoginUser.EMP_NO;
            bool LockFlag=R_Sync_Lock.SYNC_Lock(BU, IP, ProgramName, ItemName, EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);
            return LockFlag;
        }

        public bool UnLockItem(string ProgramName, string ItemName, OleExec Sfcdb)
        {
            string EMP_NO = LoginUser.EMP_NO;
            bool LockFlag = R_Sync_Lock.SYNC_Lock(BU, IP, ProgramName, ItemName, EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);
            return LockFlag;
        }

        public void UpdateNextRunTime(string ProgramName, string ItemName, OleExec Sfcdb)
        {
            string EMP_NO = LoginUser.EMP_NO;
            C_Interface = new T_C_INTERFACE(Sfcdb, DB_TYPE_ENUM.Oracle);
            C_Interface.UpdateNextRunTime(BU, IP, ProgramName, ItemName, EMP_NO, Sfcdb, DB_TYPE_ENUM.Oracle);
        }
    } 

}
