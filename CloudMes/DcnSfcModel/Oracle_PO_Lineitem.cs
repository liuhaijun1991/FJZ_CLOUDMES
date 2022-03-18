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
    public class Oracle_PO_Lineitem
    {
        public Oracle_PO_Lineitem()
        {
			
        }
 
       #region  Oracle_PO_Lineitem实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PONumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string POLineNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SubLineItem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SubLineItemBOMLevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SubLineItemQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitOfMeasure {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SublineItemNumber {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SublineItemNumberDesc {get; set;}
	
	   
	   #endregion
    }
}
 
 