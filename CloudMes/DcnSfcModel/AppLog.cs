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
    public class AppLog
    {
        public AppLog()
        {
			
        }
 
       #region  AppLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AppName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MsgType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Message {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Time {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AddValue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AddValue2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AddValue3 {get; set;}
	
	   
	   #endregion
    }
}
 
 