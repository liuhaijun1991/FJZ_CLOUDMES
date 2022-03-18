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
    public class AllPart_loss
    {
        public AllPart_loss()
        {
			
        }
 
       #region  AllPart_loss实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Message {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ScanBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 