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
    public class DWHTransferLog
    {
        public DWHTransferLog()
        {
			
        }
 
       #region  DWHTransferLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sitename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploaddt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ftpip {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ftppath {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string localpath {get; set;}
	
	
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
 
 