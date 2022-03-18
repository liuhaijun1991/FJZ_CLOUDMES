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
    public class BRCD_error_log
    {
        public BRCD_error_log()
        {
			
        }
 
       #region  BRCD_error_log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string flow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pipsType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pipeline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SOline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string service {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string error_MSG_title {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string error_MSG_detail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string errorType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reloadflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OutReload {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string InReload {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lastDT {get; set;}
	
	   
	   #endregion
    }
}
 
 