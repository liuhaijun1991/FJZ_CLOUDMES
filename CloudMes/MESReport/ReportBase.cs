using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;
using MESDBHelper;
using Newtonsoft.Json;
using System.Web.Script.Serialization;

namespace MESReport
{
    public class ReportBase
    { 
        /// <summary>
        /// 查詢輸入項目
        /// </summary>
        public List<ReportInput> Inputs = new List<ReportInput>();

        /// <summary>
        /// 報表輸出項目
        /// </summary>
        public List<object> Outputs = new List<object>();
        
        [JsonIgnore]
        [ScriptIgnore]
        public Dictionary<string, OleExecPool> DBPools;

        /// <summary>
        /// ???
        /// </summary>
        public Dictionary<string, string> Sqls = new Dictionary<string, string>();
        
        /// <summary>
        /// ???
        /// </summary>
        public List<string> RunSqls = new List<string>();

        /// <summary>
        /// 頁面加載后自動執行報表查詢
        /// </summary>
        public bool AutoRun = false;

        /// <summary>
        /// 佈局每個ReportLayout數組為一行，每個ReportLayout是一個容器，容器ID
        /// </summary>
        public List<ReportLayout[]> Layout = new List<ReportLayout[]>();

        /// <summary>
        /// 後台分頁標誌
        /// </summary>
        public bool PaginationServer = false;

        /// <summary>
        /// 啟用後台分頁后,前台傳進來的的當前頁
        /// </summary>
        public int PageNumber  = 0;

        /// <summary>
        /// 啟用後台分頁后,前台傳進來的的每頁行數
        /// </summary>
        public int PageSize  = 0;

        /// <summary>
        /// 儅查詢輸入自動提交時，判斷是否是查詢輸入的回調
        /// </summary>
        public bool isCallBack = false;

        public string LoginBU = "";

        /// <summary>
        /// 報表執行查詢提交時執行
        /// </summary>
        public virtual void Run()
        {

        }

        /// <summary>
        /// 報表查詢SendChangeEvent=True自動提交時執行的方法
        /// </summary>
        public virtual void InputChangeEvent()
        {

        }

        /// <summary>
        /// 報表初始化執行方法
        /// </summary>
        public virtual void Init()
        {

        }

        /// <summary>
        /// 下載數據
        /// </summary>
        public virtual void DownFile()
        {
            throw new NotImplementedException();
        }
    }
}
