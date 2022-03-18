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
    public class MMReportSchedule
    {
        public MMReportSchedule()
        {
			
        }
 
       #region  MMReportSchedule实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ScheduleDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TaskID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Horizon {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TaskType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string str1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string str2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string str3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int num1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int num2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UpdateBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime UpdateDT {get; set;}
	
	   
	   #endregion
    }
}
 
 