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
    public class SFC_KP_OffLineControl
    {
        public SFC_KP_OffLineControl()
        {
			
        }
 
       #region  SFC_KP_OffLineControl实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KP_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CheckINBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CheckInDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CheckOutBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CheckOutDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CheckOutFlag {get; set;}
	
	   
	   #endregion
    }
}
 
 