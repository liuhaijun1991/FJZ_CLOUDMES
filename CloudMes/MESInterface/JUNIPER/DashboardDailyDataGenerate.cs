using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESJuniper.OrderManagement;
using MESJuniper.SendData;
using MESPubLab;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MESInterface.JUNIPER
{
    public class DashboardDailyDataGenerate : taskBase
    {
        public string BU = "";
        public string SFCDBstr = "";
        public bool IsRuning = false;
        OleExec SFCDB;

        public DataTable MSGDATA = null;

        public override void init()
        {
            MSGDATA = new DataTable();
            MSGDATA.TableName = "RES";
            DataColumn dc = null;
            dc = MSGDATA.Columns.Add("TrackName", Type.GetType("System.String"));
            dc = MSGDATA.Columns.Add("TrackType", Type.GetType("System.String"));
            dc = MSGDATA.Columns.Add("StartTime", Type.GetType("System.String"));
            dc = MSGDATA.Columns.Add("Qty", Type.GetType("System.String"));
            dc = MSGDATA.Columns.Add("Message", Type.GetType("System.String"));
            this.Output.Tables.Add(MSGDATA);
            try
            {
                BU = ConfigGet("BU");
                SFCDBstr = ConfigGet("SFCDB");
                SFCDB = new OleExec(SFCDBstr, false);
            }
            catch (Exception ex)
            {
                AddMessage("init", "", "", "Fail", ex.Message);
            }
            Output.UI = new DashboardDailyDataGenerate_UI(this);
        }

        public override void Start()
        {
            if (IsRuning)
            {
                throw new Exception("The task is in progress. Please try again later...");
            }
            IsRuning = true;
            try
            {
                GenerateData();
            }
            catch
            {
                throw;
            }
            finally
            {
                IsRuning = false;
            }
        }

        private void GenerateData()
        {
            var TrackList = GetTrackList(BU);
            var TrackDate = MesDbBase.GetOraDbTime(SFCDB.ORM).ToString("yyyy-MM-dd");
            AddMessage("Progress", "Start", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "");
            for (int i = 0; i < TrackList.Count; i++)
            {
                var dt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                try
                {
                    var res = SFCDB.ORM.Ado.SqlQuerySingle<string>(TrackList[i].REALTIMESQL);
                    var his = SFCDB.ORM.Queryable<R_DASHBOARDTRACKHISDATA>()
                        .Where(t => t.TRACKDATE == TrackDate && t.TRACK == TrackList[i].TRACK && t.TRACKTYPE == TrackList[i].TRACKTYPE && t.BU == TrackList[i].BU)
                        .First();
                    if (his == null)
                    {
                        var data = new R_DASHBOARDTRACKHISDATA()
                        {
                            TRACKDATE = TrackDate,
                            TRACK = TrackList[i].TRACK,
                            TRACKTYPE = TrackList[i].TRACKTYPE,
                            BU = BU,
                            DATA = res,
                            CREATETIME = DateTime.Now
                        };
                        SFCDB.ORM.Insertable<R_DASHBOARDTRACKHISDATA>(data).ExecuteCommand();
                    }
                    else
                    {
                        his.DATA = res;
                        his.CREATETIME = DateTime.Now;
                        SFCDB.ORM.Updateable<R_DASHBOARDTRACKHISDATA>()
                            .SetColumns(t => new R_DASHBOARDTRACKHISDATA { DATA = his.DATA, CREATETIME = his.CREATETIME })
                            .Where(t => t.BU == his.BU && t.TRACKDATE == TrackDate && t.TRACKTYPE == his.TRACKTYPE && t.TRACK == his.TRACK)
                            .ExecuteCommand();
                    }
                    AddMessage(TrackList[i].TRACK, TrackList[i].TRACKTYPE, dt, res, "Success");
                }
                catch (Exception e)
                {
                    AddMessage(TrackList[i].TRACK, TrackList[i].TRACKTYPE, dt, "0", e.Message);
                    throw;
                }
            }
            AddMessage("Progress", "End", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "", "");
        }

        /// <summary>
        /// Get Pending AS build SN List
        /// </summary>
        /// <returns>R_SN List,Does Not Contain Locked</returns>
        private List<R_DASHBOARDTRACKLIST> GetTrackList(string BU)
        {
            var res = SFCDB.ORM.Queryable<R_DASHBOARDTRACKLIST>()
                .Where(t => t.BU == BU)
                .OrderBy(t => t.SEQ)
                .ToList();
            return res;
        }

        private void AddMessage(string TrackName, string TrackType, string StartTime, string Qty, string Message)
        {
            if (MSGDATA.Rows.Count > 200)
            {
                MSGDATA.Clear();
            }
            this.Output.UI.Invoke((Action)delegate {
                DataRow dr = MSGDATA.NewRow();
                dr["TrackName"] = TrackName;
                dr["TrackType"] = TrackType;
                dr["StartTime"] = StartTime;
                dr["Qty"] = Qty;
                dr["Message"] = Message;
                MSGDATA.Rows.Add(dr);
            });
        }
    }
}
