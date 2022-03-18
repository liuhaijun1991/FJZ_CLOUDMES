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
    public class R_account_Respone
    {
        public R_account_Respone()
        {
			
        }
 
       #region  R_account_Respone实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BrocadeDN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BrocadeDNline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Action {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Worktime {get; set;}
	
	   
	   #endregion
    }
}
 
 