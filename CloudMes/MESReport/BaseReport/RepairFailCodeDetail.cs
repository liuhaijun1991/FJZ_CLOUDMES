using MESDBHelper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MESReport.BaseReport
{
    // <copyright file="RepairFailCodeDetail.cs" company="Foxconn">
    // Copyright(c) foxconn All rights reserved
    // </copyright>
    // <author>fangguogang</author>
    // <date> 2018-1-27 </date>
    /// <summary>
    /// RepairFailCodeDetail
    /// </summary>
    public class RepairFailCodeDetail : ReportBase
    {       
        ReportInput errorCode = new ReportInput() { Name = "ErrorCode", InputType = "TXT", Value = "", Enable = true, SendChangeEvent = false, ValueForUse = null };      

        public RepairFailCodeDetail()
        {            
            Inputs.Add(errorCode);
            //HWD 數據庫舊的表為sfcfailuresymptominfo
            string sqlErrorCode = "select * from c_error_code where 1=1";
            Sqls.Add("SqlErrorCode", sqlErrorCode);
        }

        public override void Init()
        {
            
        }       

        public override void Run()
        {
            string sqlCode = "";
            string sqlRun = "";
            if (errorCode.Value != null && !string.IsNullOrEmpty(errorCode.Value.ToString()))
            {
                sqlCode = $@" and error_code ='{errorCode.Value.ToString()}' ";
            }            
            sqlRun = Sqls["SqlErrorCode"] + sqlCode;
            RunSqls.Add(sqlRun);
            OleExec SFCDB = DBPools["SFCDB"].Borrow();            
            try
            {
                DataSet dsUresymptom = SFCDB.RunSelect(sqlRun);
                ReportTable reportTable = new ReportTable();
                reportTable.LoadData(dsUresymptom.Tables[0], null);
                reportTable.Tittle = "ErrorCodeTable";
                reportTable.ColNames.RemoveAt(0);
                Outputs.Add(reportTable);
                DBPools["SFCDB"].Return(SFCDB);
            }
            catch (Exception exception)
            {
                DBPools["SFCDB"].Return(SFCDB);
                throw exception;
            }
        }
    }
}
