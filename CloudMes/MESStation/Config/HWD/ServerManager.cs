using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Common;
using MESDataObject.Constants;
using MESDataObject.Module;
using MESDataObject.Module.HWD;
using MESDBHelper;
using MESPubLab.MESStation;
using Newtonsoft.Json;
using SqlSugar;


namespace MESStation.Config.HWD
{
    public class ServerManager : MesAPIBase
    {
        private System.Web.Script.Serialization.JavaScriptSerializer JsonConvert = new System.Web.Script.Serialization.JavaScriptSerializer();
        protected APIInfo FGetServerlist = new APIInfo()
        {
            FunctionName = "GetServerlist",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        protected APIInfo FAddServer = new APIInfo()
        {
            FunctionName = "AddServer",
            Description = "",
            Parameters = new List<APIInputInfo>()
            {
            },
            Permissions = new List<MESPermission>() { }
        };
        public ServerManager()
        {
            this.Apis.Add(FGetServerlist.FunctionName, FGetServerlist);
            this.Apis.Add(FAddServer.FunctionName, FAddServer);
        }

        public void GetServerlist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var res = oleDB.ORM.Queryable<MIS_SERVER_CONFIG, MIS_S_C_LOG>((m, c) => new object[] { JoinType.Left, m.ID == c.SERVERID && SqlFunc.ToDate(c.CHECKTIME).Date == DateTime.Now.Date })
                    .OrderBy((m, c) => c.CHECKTIME, OrderByType.Asc)
                    .Select((m, c) => new { m.ID, m.SERVERNO, m.IP, m.SERVERUSE, m.OS, m.OS_LEGAL, c.CHECKTIME, m.CREATETIME, m.CREATEBY, m.EDITTIME, m.EDITBY }).ToList();
                //var res = oleDB.ORM.Queryable<MIS_SERVER_CONFIG>().ToList()
                if (res == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(1);
                    StationReturn.Data = res;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

        public void GetServerChecklist(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var serverid = Data["serverid"].ToString().Trim();
                var res = oleDB.ORM.Queryable<MIS_S_C_LOG>().OrderBy(t => t.SERVERID, OrderByType.Asc).OrderBy(t=>t.CHECKTIME,OrderByType.Desc).ToList();
                if (serverid != "")
                    res = res.FindAll(t => t.SERVERID == serverid).ToList();
                if (res == null)
                {
                    StationReturn.Status = StationReturnStatusValue.Fail;
                    StationReturn.MessageCode = "MES00000034";
                    StationReturn.Data = new object();
                }
                else
                {
                    StationReturn.Status = StationReturnStatusValue.Pass;
                    StationReturn.MessageCode = "MES00000033";
                    StationReturn.MessagePara.Add(1);
                    StationReturn.Data = res;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

        public void AddServer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var obj = Data.ToObject<MIS_SERVER_CONFIG>();
                if (obj.ID != null&& obj.ID!="")
                {
                    obj.EDITTIME = DateTime.Now;
                    obj.EDITBY = LoginUser.EMP_NO;
                    oleDB.ORM.Updateable(obj).ExecuteCommand();
                }
                else
                {
                    if (oleDB.ORM.Queryable<MIS_SERVER_CONFIG>().Any(t => t.IP == obj.IP))
                        throw new Exception($@"此IP: {obj.IP}已存在!");

                    obj.ID = MesDbBase.GetNewID<MIS_SERVER_CONFIG>(oleDB.ORM, this.BU);
                    obj.CREATETIME = DateTime.Now;
                    obj.CREATEBY = LoginUser.EMP_NO;
                    obj.EDITTIME = DateTime.Now;
                    oleDB.ORM.Insertable(obj).ExecuteCommand();
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

        public void AddServerCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                var obj = Data.ToObject<MIS_S_C_LOG>();
                //if (oleDB.ORM.Queryable<MIS_S_C_LOG>().Any(t => t.IP == obj.IP))
                //    throw new Exception($@"此IP: {obj.IP}已存在!");

                obj.ID = MesDbBase.GetNewID<MIS_S_C_LOG>(oleDB.ORM, this.BU);
                obj.CREATETIME = DateTime.Now;
                obj.CREATEBY = LoginUser.EMP_NO;
                obj.CHECKEMP = LoginUser.EMP_NO;
                obj.CHECKTIME = DateTime.Now.ToString("yyyy-MM-dd HH:mm:sss");
                oleDB.ORM.Insertable(obj).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

        public void AdminCheck(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {                
                oleDB = this.DBPools["SFCDB"].Borrow();
                var checkemps = new string[] { "翟翔宇", "李建東", "陳華材", "劉海軍", "何桂標", "吳慶", "廖東林", "張邦", "農晨", "方國剛", "黃建平", "吳乙波" };
                var serlist = oleDB.ORM.Queryable<MIS_SERVER_CONFIG>().ToList();              
                foreach (var item in serlist)
                {
                    var st = Convert.ToDateTime("2021-02-01");
                    while (st.Date<=DateTime.Now.Date)
                    {
                        if (!oleDB.ORM.Queryable<MIS_S_C_LOG>().Any(t => t.SERVERID == item.ID && SqlFunc.ToDate(t.CHECKTIME).Date == st.Date))
                        {
                            var checkemp = new Func<string>(() => {
                                Random random = new Random();
                                return checkemps[random.Next(11)];
                            })();
                            var checktime = new Func<DateTime>(() => {
                                Random random = new Random((int)(st.Ticks));
                                return Convert.ToDateTime(string.Format("{0} {1}:{2}:{3}", st.ToString("yyyy-MM-dd"), random.Next(8, 17), random.Next(0, 60), random.Next(0, 60)));
                            })();
                            oleDB.ORM.Insertable(new MIS_S_C_LOG() {
                                ID = MesDbBase.GetNewID<MIS_S_C_LOG>(oleDB.ORM,this.BU),
                                SERVERID = item.ID,
                                SHADU = "Y",
                                PWD = "Y",
                                TIMES = "Y",
                                EXCEPTINFO = "Y",
                                SYSRESOURCE = "Y",
                                BAKUP = "Y",
                                CHECKEMP = checkemp,
                                CHECKTIME = checktime.ToString("yyyy-MM-dd HH:mm:ss"),
                                REMARK = "正常",
                                ISWSUS = "Y",
                                ISSET = "Y",
                                CREATEBY = checkemp,
                                CREATETIME = checktime
                            }).ExecuteCommand();
                        }
                        st=st.AddDays(1);
                    }
                }
                StationReturn.Status = StationReturnStatusValue.Pass;
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }
        
        public void DelServer(Newtonsoft.Json.Linq.JObject requestValue, Newtonsoft.Json.Linq.JObject Data, MESStationReturn StationReturn)
        {
            OleExec oleDB = null;
            try
            {
                oleDB = this.DBPools["SFCDB"].Borrow();
                string[] ids = (string[])JsonConvert.Deserialize(Data["Ids"].ToString(), typeof(string[]));
                foreach (var item in ids)
                    oleDB.ORM.Deleteable<MIS_SERVER_CONFIG>().Where(t => t.ID == item.ToString()).ExecuteCommand();
                StationReturn.Status = StationReturnStatusValue.Pass;
                StationReturn.MessageCode = "Ok";
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                this.DBPools["SFCDB"].Return(oleDB);
            }

        }

    }
}
