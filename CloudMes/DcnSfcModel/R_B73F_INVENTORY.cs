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
    public class R_B73F_INVENTORY
    {
        public R_B73F_INVENTORY()
        {
			
        }
 
       #region  R_B73F_INVENTORY实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SyserialNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PosiTion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CheckInBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CheckInDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CheckOutWo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CheckOutBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CheckOutDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location {get; set;}
	
	   
	   #endregion
    }
}
 
 