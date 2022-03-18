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
    public class hcvschedule
    {
        public hcvschedule()
        {
			
        }
 
       #region  hcvschedule实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string schedulename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scheduletype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prioritycode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime dailyfromtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime dailytotime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime validfromdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime validtodate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
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
 
 