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
    public class R_DN_Invoice
    {
        public R_DN_Invoice()
        {
			
        }
 
       #region  R_DN_Invoice实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DN_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Invoice_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime send_Date {get; set;}
	
	   
	   #endregion
    }
}
 
 