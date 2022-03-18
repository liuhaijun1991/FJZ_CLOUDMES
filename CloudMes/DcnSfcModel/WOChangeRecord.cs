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
    public class WOChangeRecord
    {
        public WOChangeRecord()
        {
			
        }
 
       #region  WOChangeRecord实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string linetype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ScanDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ChangeTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PreWO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PreSkuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PreCloseTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NexWO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NexSkuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime NexStartTime {get; set;}
	
	   
	   #endregion
    }
}
 
 