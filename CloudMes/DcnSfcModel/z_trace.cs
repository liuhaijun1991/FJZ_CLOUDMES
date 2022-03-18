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
    public class z_trace
    {
        public z_trace()
        {
			
        }
 
       #region  z_trace实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RowNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int EventClass {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TextData {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NTUserName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ClientProcessID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ApplicationName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LoginName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SPID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long Duration {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime StartTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long Reads {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long Writes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CPU {get; set;}
	
	   
	   #endregion
    }
}
 
 