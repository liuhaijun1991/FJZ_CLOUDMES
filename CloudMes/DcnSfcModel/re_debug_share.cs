using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
 
namespace DcnSfcModel
{
    /// <summary>
    /// 功能: 实体类 ()
    /// 创建人：Eden     
    /// 创建日期：2019/12/31    
    /// </summary>
    [Serializable]
    public class re_debug_share
    {
        public re_debug_share()
        {
			
        }
 
       #region  re_debug_share实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pcba_sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failure_station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failure_symptom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failure_qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failure_analysis {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string analysis_process {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AnalyzedBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailureDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string root_cause {get; set;}
	
	   
	   #endregion
    }
}
 
 