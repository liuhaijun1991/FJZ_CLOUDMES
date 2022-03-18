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
    public class SFCFreshTotal
    {
        public SFCFreshTotal()
        {
			
        }
 
       #region  SFCFreshTotal实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scandatetime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpass {get; set;}
	
	   
	   #endregion
    }
}
 
 