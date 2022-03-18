using MESDataObject.Module;
using MESDBHelper;
using MESPubLab.MESStation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESStation.Config.HWD
{
    public class MoveDataToHisDB : MesAPIBase
    {
        protected APIInfo _MoveWoDataByDate = new APIInfo()
        {
            FunctionName = "MoveWoDataByDate",
            Description = "MoveWoDataByDate",
            Parameters = new List<APIInputInfo>()
            {
                new APIInputInfo() {InputName = "START_TIME", InputType = "string", DefaultValue = "2011-01-01" },
                new APIInputInfo() {InputName = "END_TIME", InputType = "string", DefaultValue = "2011-02-01" },

            },
            Permissions = new List<MESPermission>() { }
        };

        public MoveDataToHisDB()
        {
            Apis.Add(_MoveWoDataByDate.FunctionName, _MoveWoDataByDate);
        }

        int T = 0;

        public void MoveWoDataByDate(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            int T_Count = 5;
            try
            {
                OleExec SFCDB = new OleExec("SFCDB", true);
                OleExec HDB = new OleExec("HWDSFCDBH", true);
                List<Thread> LT = new List<Thread>();
                var START_TIME = DateTime.Parse(Data["START_TIME"].ToString());
                var END_TIME = DateTime.Parse(Data["END_TIME"].ToString());
                var Wolist = SFCDB.ORM.Queryable<R_WO_BASE>().Where(t => t.RELEASE_DATE > START_TIME && t.RELEASE_DATE < END_TIME).ToList();
                
                int C = Wolist.Count / T_Count;

                for (int i = 0; i < T_Count; i++)
                {
                    int s = i * C;
                    int e = i * C + C-1;
                    Thread T = new Thread(new ParameterizedThreadStart(MoveWoData));
                    T.Start(new object[] {Wolist,s,e, new OleExec("SFCDB", true), new OleExec("HWDSFCDBH", true) });
                    LT.Add(T);
                }
                for (int i = 0; i < LT.Count; i++)
                {
                    LT[i].Join();
                }

            }
            catch(Exception ee)
            {
                throw ee;
            }
        }

        void MoveWoData(object args)
        {
            int C_T = 0;
            lock (this)
            {
                T++;
                C_T = T;
            }

            List<R_WO_BASE> WO = (List<R_WO_BASE>) ((object[])args)[0];
            int start = (int)((object[])args)[1];
            int end = (int)((object[])args)[2];
            OleExec sfcdb = (OleExec)((object[])args)[3];
            OleExec hdb = (OleExec)((object[])args)[4];
            
            for (int i = start; i <= end; i++)
            {
                hdb.ORM.Deleteable<R_WO_BASE>().Where(t => t.ID == WO[i].ID).ExecuteCommand();
                hdb.ORM.Insertable<R_WO_BASE>(WO[i]).ExecuteCommand();
                sfcdb.ExecSQL($@"delete sfcruntime.r_panel_sn where workorderno='{WO[i].WORKORDERNO}'");
                var SNs = sfcdb.ORM.Queryable<R_SN>().Where(t => t.WORKORDERNO == WO[i].WORKORDERNO).ToList();
                for (int j = 0; j < SNs.Count; j++)
                {
                    DateTime startTime = DateTime.Now;
                    string msg = "";
                    var EVENT = sfcdb.ORM.Queryable<R_SN_STATION_DETAIL>().Where(t => t.SN == SNs[j].SN && t.WORKORDERNO == SNs[j].WORKORDERNO).ToList();
                    var r_sn_kp = sfcdb.ORM.Queryable<R_SN_KP>().Where(t => t.R_SN_ID == SNs[j].ID).ToList();
                    var r_sn_keypart_detail = sfcdb.ORM.Queryable <R_SN_KEYPART_DETAIL>().Where(t=>t.R_SN_ID == SNs[j].ID).ToList();
                    for (int k = 0; k < EVENT.Count; k++)
                    {
                        //hdb.ORM.Deleteable<R_SN_STATION_DETAIL>().Where(t => t.ID == EVENT[k].ID).ExecuteCommand();
                        try
                        {
                            hdb.ORM.Insertable<R_SN_STATION_DETAIL>(EVENT[k]).ExecuteCommand();
                        }
                        catch
                        {
                            try
                            {
                                hdb.ORM.Updateable<R_SN_STATION_DETAIL>(EVENT[k]).Where(t => t.ID == EVENT[k].ID).ExecuteCommand();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        sfcdb.ORM.Deleteable<R_SN_STATION_DETAIL>().Where(t => t.ID == EVENT[k].ID).ExecuteCommand();
                    }
                    msg += " event:" + EVENT.Count.ToString();
                    msg += " r_sn_kp:" + r_sn_kp.Count.ToString();
                    msg += " r_sn_keypart_detail:" + r_sn_keypart_detail.Count.ToString();
                    for (int k = 0; k < r_sn_kp.Count; k++)
                    {
                        //hdb.ORM.Deleteable<R_SN_KP>().Where(t => t.ID == r_sn_kp[k].ID).ExecuteCommand();
                        try
                        {
                            hdb.ORM.Insertable<R_SN_KP>(r_sn_kp[k]).ExecuteCommand();
                        }
                        catch
                        {
                            try
                            {
                                hdb.ORM.Updateable<R_SN_KP>(r_sn_kp[k]).Where(t=>t.ID== r_sn_kp[k].ID).ExecuteCommand();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        sfcdb.ORM.Deleteable<R_SN_KP>().Where(t => t.ID == r_sn_kp[k].ID).ExecuteCommand();
                    }
                    for (int k = 0; k < r_sn_keypart_detail.Count; k++)
                    {
                        //hdb.ORM.Deleteable<R_SN_KEYPART_DETAIL>().Where(t => t.ID == r_sn_keypart_detail[k].ID).ExecuteCommand();
                        try
                        {
                            hdb.ORM.Insertable<R_SN_KEYPART_DETAIL>(r_sn_keypart_detail[k]).ExecuteCommand();
                        }
                        catch
                        {
                            try
                            {
                                hdb.ORM.Updateable<R_SN_KEYPART_DETAIL>(r_sn_keypart_detail[k]).Where(t=>t.ID== r_sn_keypart_detail[k].ID).ExecuteCommand();
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        
                        sfcdb.ORM.Deleteable<R_SN_KEYPART_DETAIL>().Where(t => t.ID == r_sn_keypart_detail[k].ID).ExecuteCommand();
                    }

                    sfcdb.ExecSQL($@"delete r_sn_ex where ID = '{SNs[j].SN}'");
                    //hdb.ORM.Deleteable<R_SN>().Where(t => t.ID == SNs[j].ID).ExecuteCommand();
                    try
                    {
                        hdb.ORM.Insertable<R_SN>(SNs[j]).ExecuteCommand();
                    }
                    catch
                    {
                        try
                        {
                            hdb.ORM.Updateable<R_SN>(SNs[j]).Where(t => t.ID == SNs[j].ID).ExecuteCommand();
                        }
                        catch
                        {
                            continue;
                        }
                    }
                    sfcdb.ORM.Deleteable<R_SN>().Where(t => t.ID == SNs[j].ID).ExecuteCommand();
                    System.Console.WriteLine(SNs[j].SN+","+ SNs[j].WORKORDERNO+msg+" Tuse:"+(DateTime.Now- startTime).TotalMilliseconds.ToString()+"ms "+C_T.ToString()+"/"+T.ToString());

                }
                sfcdb.ORM.Deleteable<R_WO_BASE>().Where(t => t.ID == WO[i].ID).ExecuteCommand();
                System.Console.WriteLine("DELETE: "+ WO[i].WORKORDERNO);
                
            }
            lock (this)
            {
                T--;
            }
            
        }


    }
}
