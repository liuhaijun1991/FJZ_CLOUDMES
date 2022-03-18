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
    public class R_inventory_SAP
    {
        public R_inventory_SAP()
        {
			
        }
 
       #region  R_inventory_SAP实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QTY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string F_Stockin {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string T_Stockin {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Docno {get; set;}
	
	   
	   #endregion
    }
}
 
 