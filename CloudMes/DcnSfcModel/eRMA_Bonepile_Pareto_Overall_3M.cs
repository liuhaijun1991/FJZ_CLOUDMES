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
    public class eRMA_Bonepile_Pareto_Overall_3M
    {
        public eRMA_Bonepile_Pareto_Overall_3M()
        {
			
        }
 
       #region  eRMA_Bonepile_Pareto_Overall_3M实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TranType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string YR {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WKOrMO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime FirstDateOfWork {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProductName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Shipping_Qty_HST {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Shipping_Qty_CZ {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LasteditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LasteditDt {get; set;}
	
	   
	   #endregion
    }
}
 
 