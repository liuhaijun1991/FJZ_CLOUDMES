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
    public class R_ORT_FAIL
    {
        public R_ORT_FAIL()
        {
			
        }
 
       #region  R_ORT_FAIL实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ortevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string reasoncode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int counter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool sendflag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime mdstime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	   
	   #endregion
    }
}
 
 