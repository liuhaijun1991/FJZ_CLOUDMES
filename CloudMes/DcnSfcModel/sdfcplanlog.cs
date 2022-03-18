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
    public class sdfcplanlog
    {
        public sdfcplanlog()
        {
			
        }
 
       #region  sdfcplanlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime logdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string planname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string customercode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventdetail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sdfcplankey {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendarmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendarquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string calendarweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal planqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal total {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	   
	   #endregion
    }
}
 
 