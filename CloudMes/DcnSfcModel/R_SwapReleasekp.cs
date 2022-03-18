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
    public class R_SwapReleasekp
    {
        public R_SwapReleasekp()
        {
			
        }
 
       #region  R_SwapReleasekp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string cserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string swaprelease {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mpn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime oldscandt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 