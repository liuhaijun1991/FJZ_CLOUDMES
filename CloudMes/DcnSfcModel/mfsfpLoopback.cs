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
    public class mfsfpLoopback
    {
        public mfsfpLoopback()
        {
			
        }
 
       #region  mfsfpLoopback实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int times {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int limitQTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool crap_flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lastedit {get; set;}
	
	   
	   #endregion
    }
}
 
 