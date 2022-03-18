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
    public class sfcControlRunSummary
    {
        public sfcControlRunSummary()
        {
			
        }
 
       #region  sfcControlRunSummary实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ControlRunNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime StartDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ModelPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CustomerDev {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Revision {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string StartStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EndStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ControlRunStation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ControlRunQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FaiSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CompleteDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductionLine {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ControlRunStatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProcessEngineer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TestingEngineer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QualityEngineer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime UpdateDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Reason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comments {get; set;}
	
	   
	   #endregion
    }
}
 
 