using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESPubLab.Common;
using MES_DCN.Juniper;
using MESDBHelper;

namespace MESInterface.DCN
{
    public class JuniperSendDISCQMSDataB2BSyn: taskBase
    {
        private bool IsRuning = false;
        private string mesdbstr, apdbstr, bustr, keypath, filetype, filepath, filebackpath, remotepath, plant,taskNum;
        private bool IsPROD = true;
        public string csvFilePath = "";

        public override void init()
        {
            try
            {
                apdbstr = ConfigGet("APDB");
                mesdbstr = ConfigGet("MESDB");
                bustr = ConfigGet("BU");
                filepath = ConfigGet("FILEPATH");
                filebackpath = ConfigGet("FILEPATHBACKPATH");
                keypath = ConfigGet("KEYPATH");
                remotepath = ConfigGet("REMOTEPATH");
                filetype = ConfigGet("FILETYPE");
                plant = ConfigGet("PLANT");
                IsPROD = ConfigGet("IsPROD").ToUpper().Equals("TRUE") ? true : false;
                taskNum = ConfigGet("TASK_NUM");
                csvFilePath = $@"{filepath}";
                Output.UI = new JuniperSendDiscUI(this);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void Start()
        {
            try
            {
                SqlSugar.SqlSugarClient SFCDB = OleExec.GetSqlSugarClient(mesdbstr, false);
                DateTime collectDate = SFCDB.GetDate().AddDays(-1);
                SendData(collectDate);
                SFCDB.Dispose();
            }
            catch (Exception ex)
            {
                throw ex;
            }           
        }
        
        public void SendData(DateTime collectDate)
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes> listresult = new Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes>();
            try
            {
                MesLog.Info("Start");
                if (filetype == "")
                {
                    throw new Exception("Please setting FILETYPE");
                }
                //JuniperSendDISCQMSData SendDISCQMSDate = new JuniperSendDISCQMSData(mesdbstr, apdbstr, bustr, filepath, filebackpath, remotepath, keypath, plant, IsPROD);
                ////SendDISCQMSDate.Build(filetype, collectDate, ref listresult);
                //SendDISCQMSDate.BuildTask(filetype, collectDate, ref listresult);

                DISCQMSDataNew disc = new DISCQMSDataNew(mesdbstr, apdbstr, bustr, filepath, filebackpath, remotepath, keypath, plant, IsPROD);
                disc.Run(filetype, collectDate, taskNum);
            }
            catch (Exception ex)
            {
                MesLog.Info($@"Error:{ex.Message}");
                throw ex;
            }
            finally
            {
                try
                {
                    foreach (var r in listresult)
                    {
                        string log = $@"{r.Key} send ";
                        if (r.Value.IsSuccess)
                        {
                            log += $@"success";
                        }
                        else
                        {
                            log += $@"fail;Error message:{r.Value.ErrorMessage}";
                        }
                        MesLog.Info(log);
                    }
                }
                catch (Exception)
                {
                }
                MesLog.Info("End");
                IsRuning = false;
            }
        }

        public void SaveCSV(DateTime collectDate)
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes> listresult = new Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes>();
            try
            {
                MesLog.Info("Start");
                if (filetype == "")
                {
                    throw new Exception("Please setting FILETYPE");
                }
                //JuniperSendDISCQMSData SendDISCQMSDate = new JuniperSendDISCQMSData(mesdbstr, apdbstr, bustr, filepath, filebackpath, remotepath, keypath, plant, IsPROD);
                ////SendDISCQMSDate.Build(filetype, collectDate, ref listresult,false);
                //SendDISCQMSDate.BuildTask(filetype, collectDate, ref listresult, false);

                DISCQMSDataNew disc = new DISCQMSDataNew(mesdbstr, apdbstr, bustr, filepath, filebackpath, remotepath, keypath, plant, IsPROD);
                disc.Run(filetype, collectDate, taskNum, false);
            }
            catch (Exception ex)
            {
                MesLog.Info($@"Error:{ex.Message}");
                throw ex;
            }
            finally
            {
                try
                {
                    foreach (var r in listresult)
                    {
                        string log = $@"{r.Key} send ";
                        if (r.Value.IsSuccess)
                        {
                            log += $@"success";
                        }
                        else
                        {
                            log += $@"fail;Error message:{r.Value.ErrorMessage}";
                        }
                        MesLog.Info(log);
                    }
                }
                catch (Exception)
                {
                }
                MesLog.Info("End");
                IsRuning = false;
            }
        }
    
        public void RebulidGZFile(DateTime fromDate, DateTime toDate,string discKey)
        {
            if (IsRuning)
            {
                throw new Exception("Running,Please wait....");
            }
            IsRuning = true;
            Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes> listresult = new Dictionary<string, MESDataObject.ModuleHelp.FuncExecRes>();
            try
            {
                MesLog.Info("Start");                              
                DISCQMSDataNew disc = new DISCQMSDataNew(mesdbstr, apdbstr, bustr, filepath, filebackpath, remotepath, keypath, plant, IsPROD);
                disc.RebulidGZFile(fromDate, toDate, discKey);
            }
            catch (Exception ex)
            {
                MesLog.Info($@"Error:{ex.Message}");
                throw ex;
            }
            finally
            {
                try
                {
                    foreach (var r in listresult)
                    {
                        string log = $@"{r.Key} send ";
                        if (r.Value.IsSuccess)
                        {
                            log += $@"success";
                        }
                        else
                        {
                            log += $@"fail;Error message:{r.Value.ErrorMessage}";
                        }
                        MesLog.Info(log);
                    }
                }
                catch (Exception)
                {
                }
                MesLog.Info("End");
                IsRuning = false;
            }
        }
    }
}
