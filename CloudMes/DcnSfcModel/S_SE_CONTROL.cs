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
    public class S_SE_CONTROL
    {
        public S_SE_CONTROL()
        {
			
        }
 
       #region  S_SE_CONTROL实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int LABCOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RequestDept {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RequestType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RequestStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CurrentEvent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailDescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RequestTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAUser {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AnalysisType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ScanInTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FailReason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FACompletedTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double FATimeCost {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ScanOutTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FAStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 