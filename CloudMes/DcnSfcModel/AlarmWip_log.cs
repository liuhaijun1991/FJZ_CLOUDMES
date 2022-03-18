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
    public class AlarmWip_log
    {
        public AlarmWip_log()
        {
			
        }
 
       #region  AlarmWip_log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int alarm_type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Runintype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Esstype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Vskuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime shipdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int shipQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string errmsg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 