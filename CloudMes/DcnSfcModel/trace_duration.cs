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
    public class trace_duration
    {
        public trace_duration()
        {
			
        }
 
       #region  trace_duration实体
 
	
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
	   public long Duration {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TextData {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SPID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int CPU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ApplicationName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HostName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EndTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DatabaseName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LoginName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long Reads {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime StartTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long Writes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public byte[] BinaryData {get; set;}
	
	   
	   #endregion
    }
}
 
 