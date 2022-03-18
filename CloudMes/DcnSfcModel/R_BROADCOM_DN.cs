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
    public class R_BROADCOM_DN
    {
        public R_BROADCOM_DN()
        {
			
        }
 
       #region  R_BROADCOM_DN实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sitecode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 