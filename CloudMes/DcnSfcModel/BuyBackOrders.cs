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
    public class BuyBackOrders
    {
        public BuyBackOrders()
        {
			
        }
 
       #region  BuyBackOrders实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DocType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SOline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PipelineName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IsProcessed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 