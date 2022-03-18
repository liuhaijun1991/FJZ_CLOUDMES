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
    public class MDSTransferLog
    {
        public MDSTransferLog()
        {
			
        }
 
       #region  MDSTransferLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datapoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploaddt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string port {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string path {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string directory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string filename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string faildesc {get; set;}
	
	   
	   #endregion
    }
}
 
 