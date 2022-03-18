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
    public class sfc_AgingDays_Alert_Log
    {
        public sfc_AgingDays_Alert_Log()
        {
			
        }
 
       #region  sfc_AgingDays_Alert_Log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TranType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProdName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Rev {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ErrorNum {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ErrorLine {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrMsg {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 