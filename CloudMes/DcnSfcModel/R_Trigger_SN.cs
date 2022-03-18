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
    public class R_Trigger_SN
    {
        public R_Trigger_SN()
        {
			
        }
 
       #region  R_Trigger_SN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TriggerID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string supplier {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int defaultqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int finishedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ReleaseDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 