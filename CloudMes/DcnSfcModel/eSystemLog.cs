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
    public class eSystemLog
    {
        public eSystemLog()
        {
			
        }
 
       #region  eSystemLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LOGDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGMEMO {get; set;}
	
	   
	   #endregion
    }
}
 
 