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
    public class r_sap_stock_qty
    {
        public r_sap_stock_qty()
        {
			
        }
 
       #region  r_sap_stock_qty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string File_Date {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Part_Number {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stock_qty {get; set;}
	
	   
	   #endregion
    }
}
 
 