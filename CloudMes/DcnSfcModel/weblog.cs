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
    public class weblog
    {
        public weblog()
        {
			
        }
 
       #region  weblog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USERNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGINTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACCESSPAGE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACCESSTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACTIONTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACTIONDESC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REMARK {get; set;}
	
	   
	   #endregion
    }
}
 
 