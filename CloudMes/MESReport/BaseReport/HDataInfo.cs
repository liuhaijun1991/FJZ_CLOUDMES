using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
   public  class HDataInfo : ReportBase
    {
        ReportInput userNameInput = new ReportInput()
        {
            Name = "UserName",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput ExtInput = new ReportInput()
        {
            Name = "Ext",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput userIDInput = new ReportInput()
        {
            Name = "UserID",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput ManagerInput = new ReportInput()
        {
            Name = "Manager",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput AgentInput = new ReportInput()
        {
            Name = "Agent",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput mailInput = new ReportInput()
        {
            Name = "Mail",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        ReportInput departInput = new ReportInput()
        {
            Name = "Depart",
            InputType = "TXT",
            Value = "",
            Enable = true,
            SendChangeEvent = false,
            ValueForUse = null
        };
        public HDataInfo()
        {
            this.Inputs.Add(userNameInput);
            this.Inputs.Add(ExtInput);
            this.Inputs.Add(userIDInput);
            this.Inputs.Add(ManagerInput);
            this.Inputs.Add(AgentInput);
            this.Inputs.Add(mailInput);
            this.Inputs.Add(departInput);
        }

        public override void Init()
        {
            base.Init();
        }

        void clearinput()
        {
            userNameInput.Value = "";
            ExtInput.Value = "";
            userIDInput.Value = "";
            ManagerInput.Value = "";
            AgentInput.Value = "";
            mailInput.Value = "";
            departInput.Value = "";
        }

        public override void Run()
        {
            var el = new HRMWebReference.ElistQuery();
            var ds = new DataSet();
            string userNameValue = userNameInput.Value.ToString();
            string ExtValue = ExtInput.Value.ToString();
            string userIDValue = userIDInput.Value.ToString();
            string ManagerValue = ManagerInput.Value.ToString();
            string AgentValue = AgentInput.Value.ToString();
            string mailValue = mailInput.Value.ToString();
            string departValue = departInput.Value.ToString();
            if (!string.IsNullOrEmpty(userNameValue))
            {
                ds = el.ByUserName(userNameValue);
                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }
            else if (!string.IsNullOrEmpty(userIDValue))
            {
                ds = el.ByUserID(userIDValue);
                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }
            else if(!string.IsNullOrEmpty(ExtValue))
            {
                ds = el.ByUserID_ExtData(ExtValue);
                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }
            else if(!string.IsNullOrEmpty(ManagerValue))
            {
                ds = el.ByUserID(ManagerValue);
                ds = el.ByUserID_Abnormal_Manager_New(ds.Tables[0].Rows[0]["USER_ID"].ToString(), ds.Tables[0].Rows[0]["USER_LEVEL"].ToString(), ds.Tables[0].Rows[0]["JOB_TITLE"].ToString(), "3", "3");

                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }
            else if(!string.IsNullOrEmpty(AgentValue))
            {
                ds = el.ByUserID(AgentValue);
                ds = el.ByUserID_FindAgent(AgentValue, ds.Tables[0].Rows[0]["CURRENT_OU_CODE"].ToString());//
                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }
            else if(!string.IsNullOrEmpty(mailValue))
            {
                ds = el.ByNotesID(mailValue);
                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }
            else if(!string.IsNullOrEmpty(departValue))
            {
                ds = el.ByDepartment(departValue);
                if (ds.Tables[0].Rows.Count == 0)
                    throw new Exception("No Data!");
            }

            try { 

                ReportTable retTab = new ReportTable();
                retTab.LoadData(ds.Tables[0], null);
                retTab.Tittle = "H Report";
                Outputs.Add(retTab);
            }
            catch (Exception ex)
            {
                ReportAlart alart = new ReportAlart(ex.Message);
                Outputs.Add(alart);
            }
            clearinput();

        }
    }
}
