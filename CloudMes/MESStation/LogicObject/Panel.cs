using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Module;
using System.Collections.ObjectModel;
using System.Collections;
using MESDataObject;

namespace MESStation.LogicObject
{
    public class Panel
    {
        private ObservableCollection<R_PANEL_SN> _panelCollection;
        public ObservableCollection<R_PANEL_SN> PanelCollection
        {
            get
            {
                if (this._panelCollection == null)
                {
                    this._panelCollection = new ObservableCollection<R_PANEL_SN>();
                }
                return this._panelCollection;
            }

            set
            {
                if (value != this._panelCollection)
                {
                    if (this._panelCollection != null)
                    {
                        this._panelCollection.Clear();
                    }
                    this._panelCollection = value;
                }
            }
        }

        public List<R_SN> PanelSnList = new List<R_SN>();
        MESDataObject.DB_TYPE_ENUM DBType;
        List<R_PANEL_SN> RPanelList;
        public string PanelNo { get; set; }
        public string Workorderno { get; set; }
        public Panel()
        {

        }

        public Panel(string strPanel, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            Init(strPanel, SFCDB, _DBType);
        }
        public void Init(string strPanel, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            try
            {
                this.PanelNo = strPanel;
                DBType = _DBType;
                T_R_PANEL_SN TRWB = new T_R_PANEL_SN(SFCDB, DBType);
                RPanelList = TRWB.GetPanel(strPanel, SFCDB);
                foreach (R_PANEL_SN item in RPanelList)
                {
                    this.Workorderno = item.WORKORDERNO;
                    this.PanelCollection.Add(item);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<R_SN> GetSnDetail(string strPanel, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            try
            {
                DBType = _DBType;
                T_R_PANEL_SN TRWB = new T_R_PANEL_SN(SFCDB, DBType);
                this.PanelSnList = TRWB.GetSn(strPanel, SFCDB);
                return PanelSnList;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Boolean CreatePanel(Hashtable temp, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            try
            {
                T_R_PANEL_SN tPanel = new T_R_PANEL_SN(SFCDB, _DBType);
                Row_R_PANEL_SN rPanel = (Row_R_PANEL_SN)tPanel.NewRow();
                rPanel.ID = tPanel.GetNewID(temp["BU"].ToString(), SFCDB);
                rPanel.PANEL = temp["Panel"].ToString();
                rPanel.SN = rPanel.ID;
                rPanel.WORKORDERNO = temp["WO"].ToString();
                rPanel.SEQ_NO = 0;
                rPanel.EDIT_EMP = temp["User"].ToString();
                rPanel.EDIT_TIME = DateTime.Now;
                string strRet = SFCDB.ExecSQL(rPanel.GetInsertString(_DBType));
                if (Convert.ToInt32(strRet)>0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Boolean AddSnToPanel(Hashtable temp, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            string strSn = temp["SN"].ToString();
            SN sn = new SN(strSn, SFCDB, _DBType);
            string wo = sn.WorkorderNo;
            if (wo != this.PanelCollection[0].WORKORDERNO)
            {
                return false;
            }
            T_R_PANEL_SN tPanel = new T_R_PANEL_SN(SFCDB, _DBType);
            Row_R_PANEL_SN rPanel = (Row_R_PANEL_SN)tPanel.NewRow();
            rPanel.ID = tPanel.GetNewID(temp["BU"].ToString(), SFCDB);
            rPanel.SN = temp["SNID"].ToString();
            rPanel.PANEL = this.PanelNo;
            rPanel.WORKORDERNO = wo;
            rPanel.SEQ_NO = this.PanelCollection.Count;
            rPanel.EDIT_EMP = temp["User"].ToString();
            rPanel.EDIT_TIME = DateTime.Now;
            string strRet = SFCDB.ExecSQL(rPanel.GetInsertString(_DBType));
            if (Convert.ToInt32(strRet)>0)
            {
                this.PanelCollection.Add(rPanel.GetDataObject());
                return true;
            }
            return false;
        }

        public Boolean DeletePanelByID(string strID,MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            try
            {
                T_R_PANEL_SN tPanel = new T_R_PANEL_SN(SFCDB, _DBType);
                Row_R_PANEL_SN rPanel = (Row_R_PANEL_SN)tPanel.GetObjByID(strID, SFCDB, _DBType);
                string strRet = SFCDB.ExecSQL(rPanel.GetDeleteString(_DBType));
                if (Convert.ToInt32(strRet) > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void Reload(MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            try
            {
                DBType = _DBType;
                T_R_PANEL_SN TRWB = new T_R_PANEL_SN(SFCDB, DBType);
                RPanelList = TRWB.GetPanel(this.PanelNo, SFCDB);
                foreach (R_PANEL_SN item in RPanelList)
                {
                    this.PanelCollection.Add(item);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<R_SN> GetPanel(string PanelSN, MESDBHelper.OleExec SFCDB, MESDataObject.DB_TYPE_ENUM _DBType)
        {
            try
            {
                T_R_SN trs = new T_R_SN(SFCDB, DBType);
                List<R_SN> sn = trs.GetRSNbyPsn(PanelSN, SFCDB);
                return sn;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void LockCheck(MESDBHelper.OleExec SFCDB,string station)
        {
            var SnLock = SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.SN == this.PanelNo && (t.LOCK_STATION == station || t.LOCK_STATION == "ALL") && t.LOCK_STATUS == "1" && t.TYPE == "PANEL").ToList();
            string ErrMsg = "";
            if (SnLock.Count > 0)
            {

                try
                {
                    ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { this.PanelNo, SnLock[0].LOCK_EMP, SnLock[0].LOCK_REASON });
                }
                catch
                {
                    throw new Exception($@"Panel:'{this.PanelNo}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                }
                throw new Exception(ErrMsg);
            }
            //检查工单被锁的
            var WoLock = SFCDB.ORM.Queryable<R_SN_LOCK>().Where(t => t.WORKORDERNO == this.Workorderno && (t.LOCK_STATION == station || t.LOCK_STATION == "ALL") && t.LOCK_STATUS == "1" && t.TYPE == "WO").ToList();
            if (WoLock.Count > 0)
            {
                try
                {
                    ErrMsg = MESReturnMessage.GetMESReturnMessage("MSGCODE20180903163443", new string[] { this.Workorderno, WoLock[0].LOCK_EMP, WoLock[0].LOCK_REASON });
                }
                catch
                {
                    throw new Exception($@"Panel:'{this.Workorderno}' Locked By:'{SnLock[0].LOCK_EMP}' Reason:'{SnLock[0].LOCK_REASON}'");
                }
                throw new Exception(ErrMsg);
            }
        }
    }

}
