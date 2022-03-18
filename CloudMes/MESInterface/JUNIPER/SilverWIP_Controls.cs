using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject;
using MESDataObject.Module;
using MESDataObject.Module.Juniper;
using MESDataObject.Module.OM;
using MESDBHelper;
using MESJuniper.OrderManagement;
using MESJuniper.SendData;
using SqlSugar;

namespace MESInterface.JUNIPER
{
    public class SilverWIP_Controls : taskBase
    {
        public string BU = "";
        public string SFCDBstr = "";
        public string Factory = "";
        public bool IsRuning = false;
        OleExec SFCDB;
        string ErrMessage = null;

        public string BuildSite
        {
            get
            {
                return Factory == "FVN" ? "Foxconn Vietnam" : (Factory == "FJZ" ? "Foxconn Juarez" : "");
            }
        }

        public DataTable ASBDATA = null;

        public override void init()
        {
            ASBDATA = new DataTable();
            ASBDATA.TableName = "RES";
            DataColumn dc = null;
            dc = ASBDATA.Columns.Add("TranID", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("StartTime", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("Qty", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("FinishQty", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("Status", Type.GetType("System.String"));
            dc = ASBDATA.Columns.Add("Message", Type.GetType("System.String"));
            this.Output.Tables.Add(ASBDATA);

            try
            {
                SFCDBstr = ConfigGet("SFCDB");
                Factory = ConfigGet("FACTORY");
                SFCDB = new OleExec(SFCDBstr, false);
            }
            catch (Exception ex)
            {
                AddMessage("init", "", "", "Fail", ex.Message);
            }

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
                ErrMessage = SilverWIP_Control_Lock();
                if (!String.IsNullOrEmpty(ErrMessage))
                {
                    throw new Exception(ErrMessage);
                }
            }
            catch (Exception ex)
            {
                AddMessage("Start", "", "", "Fail", ex.Message); ;
            }
            finally
            {
                IsRuning = false;
            }
        }

        public string SilverWIP_Control_Lock()
        {
            try
            {
                List<R_JUNIPER_SILVER_WIP> sw_data = SFCDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>()
                .Where(sw => sw.STATE_FLAG == "1").ToList();

                C_SKU_DETAIL sku_config;

                List<R_F_CONTROL> lock_ctrl = SFCDB.ORM.Queryable<R_F_CONTROL>()
                    .Where(fc1 => fc1.CONTROLFLAG == "Y" && fc1.FUNCTIONTYPE == "NOSYSTEM" && fc1.FUNCTIONNAME.Trim().ToUpper() == "SILVERWIP_CONTROL"
                        && fc1.CATEGORY.ToUpper().Contains("TOLERANCE_LOCK")).ToList();

                foreach (R_JUNIPER_SILVER_WIP sw_item in sw_data)
                {
                    if (!SFCDB.ORM.Queryable<R_SN_LOCK>().WhereIF(!String.IsNullOrEmpty(sw_item.SN), lk => lk.SN == sw_item.SN && lk.LOCK_STATUS == "1" && lk.LOCK_STATION == "ALL"
                        && (lk.LOCK_REASON.Contains("TEST_HOURS") || lk.LOCK_REASON.Contains("WIP_DAYS") || lk.LOCK_REASON.Contains("UNIT_QTY"))).Any())
                    {
                        sku_config = SFCDB.ORM.Queryable<C_SKU_DETAIL>()
                                .Where(sku => sku.CATEGORY == "JUNIPER" && sku.CATEGORY_NAME == "SilverWip" && sku.SKUNO == sw_item.SKUNO).ToList().FirstOrDefault();

                        double hours = 0.0, days = 0.0, qty = 0.0;

                        string _hours = lock_ctrl.FirstOrDefault(a => a.CATEGORY.Contains("TEST_HOURS")) == null ? null : lock_ctrl.FirstOrDefault(a => a.CATEGORY.Contains("TEST_HOURS")).VALUE;
                        string _days = lock_ctrl.FirstOrDefault(a => a.CATEGORY.Contains("WIP_DAYS")) == null ? null : lock_ctrl.FirstOrDefault(a => a.CATEGORY.Contains("WIP_DAYS")).VALUE;
                        //Lock SN by UNIT_QTY not implement, need confirm before
                        string _qty = lock_ctrl.FirstOrDefault(a => a.CATEGORY.Contains("UNIT_QTY")) == null ? null : lock_ctrl.FirstOrDefault(a => a.CATEGORY.Contains("UNIT_QTY")).VALUE;

                        hours = String.IsNullOrEmpty(_hours) ? 0.0 : Convert.ToDouble(_hours);
                        days = String.IsNullOrEmpty(_days) ? 0.0 : Convert.ToDouble(_days);
                        qty = String.IsNullOrEmpty(_qty) ? 0.0 : Convert.ToDouble(_qty);

                        //pls dont change the LOCK_REASON message, it is used in other validations, 
                        if (sw_item.TEST_HOURS >= (Convert.ToDouble(sku_config.BASETEMPLATE) - hours))
                        {
                            Lock_Manager(sw_item.SN.Trim().ToUpper(), "SilverWIP Control: TEST_HOURS TimeOut, pls Check with PE and QE", "ALL");
                        }

                        double in_wip_days = (Math.Round((Convert.ToDateTime(MesDbBase.GetOraDbTime(SFCDB.ORM)).Subtract(Convert.ToDateTime(sw_item.START_TIME)).TotalDays), 2));

                        if (in_wip_days >= (Convert.ToDouble(sku_config.EXTEND) - days))
                        {
                            Lock_Manager(sw_item.SN.Trim().ToUpper(), "SilverWIP Control: WIP_DAYS TimeOut, pls Check with PE and QE", "ALL");
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return ErrMessage;
        }

        protected void Lock_Manager(string sn, string reason, string lockstation)
        {
            R_SN_LOCK lock1 = new R_SN_LOCK()
            {
                ID = MesDbBase.GetNewID(SFCDB.ORM, BU, "R_SN_LOCK"),
                TYPE = "SN",
                SN = sn,
                LOCK_REASON = reason,
                LOCK_EMP = "SW_MESSYSTEM",
                LOCK_STATION = lockstation,
                LOCK_STATUS = "1",
                LOCK_TIME = MesDbBase.GetOraDbTime(SFCDB.ORM)
            };
            SFCDB.ORM.Insertable(lock1).ExecuteCommand();
        }


        private void AddMessage(string TranID, string Qty, string FinishQty, string Status, string Message)
        {
            if (ASBDATA.Rows.Count > 200)
            {
                ASBDATA.Clear();
            }
            using (ASBDATA)
            {
                DataRow dr = ASBDATA.NewRow();
                dr["TranID"] = TranID;
                dr["StartTime"] = DateTime.Now;
                dr["Qty"] = Qty;
                dr["FinishQty"] = FinishQty;
                dr["Status"] = Status;
                dr["Message"] = Message;
                ASBDATA.Rows.Add(dr);
            }
        }
    }

    public class T_SilverWIP_Data : R_JUNIPER_SILVER_WIP
    {

        OleExec SFCDB = null;
        string ErrMessage = "OK";

        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }


        private C_SKU_DETAIL _sku_detail;

        public C_SKU_DETAIL SKU_Detail
        {
            get { return _sku_detail; }
            set { _sku_detail = value; }
        }


        public T_SilverWIP_Data()
        {
            R_JUNIPER_SILVER_WIP sw_data = SFCDB.ORM.Queryable<R_JUNIPER_SILVER_WIP>()
                .Where(sw => sw.STATE_FLAG == "1").First();

            this.ID = sw_data.ID;
            this.SN = sw_data.SN;
            this.START_TIME = sw_data.START_TIME;
            this.IN_WIP_USER = sw_data.IN_WIP_USER;
            this.END_TIME = sw_data.END_TIME;
            this.OUT_WIP_USER = sw_data.OUT_WIP_USER;
            this.SKUNO = sw_data.SKUNO;
            this.EDIT_EMP = sw_data.EDIT_EMP;
            this.EDIT_TIME = sw_data.EDIT_TIME;
            this.STATE_FLAG = sw_data.STATE_FLAG;
            this.TEST_HOURS = sw_data.TEST_HOURS;

            C_SKU_DETAIL sku_detail = SFCDB.ORM.Queryable<C_SKU_DETAIL>()
                .Where(sku => sku.CATEGORY == "JUNIPER" && sku.CATEGORY_NAME == "SilverWip" && sku.SKUNO == SKUNO).ToList().FirstOrDefault();

            this._sku_detail = sku_detail;
        }


    }
}