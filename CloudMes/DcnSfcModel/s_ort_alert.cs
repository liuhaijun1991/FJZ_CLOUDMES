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
    public class s_ort_alert
    {
        public s_ort_alert()
        {
			
        }
 
       #region  s_ort_alert实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno_ort {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sn_ort {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scanReason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int alert_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string controlby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime controldt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scanby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandt {get; set;}
	
	   
	   #endregion
    }
}
 
 