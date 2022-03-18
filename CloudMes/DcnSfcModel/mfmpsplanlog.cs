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
    public class mfmpsplanlog
    {
        public mfmpsplanlog()
        {
			
        }
 
       #region  mfmpsplanlog实体
 
	
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
	   public string mfmpskey {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsyear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsquarter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsmonth {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsweek {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpsdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lastqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal planqty {get; set;}
	
	   
	   #endregion
    }
}
 
 