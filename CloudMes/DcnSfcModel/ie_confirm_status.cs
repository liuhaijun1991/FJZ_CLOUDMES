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
    public class ie_confirm_status
    {
        public ie_confirm_status()
        {
			
        }
 
       #region  ie_confirm_status实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FILENO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKUVERSION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WORKORDERNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QTY {get; set;}
	
	   
	   #endregion
    }
}
 
 