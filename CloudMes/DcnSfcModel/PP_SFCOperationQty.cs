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
    public class PP_SFCOperationQty
    {
        public PP_SFCOperationQty()
        {
			
        }
 
       #region  PP_SFCOperationQty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WO_No {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFC_Operation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Operation_Qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Sort_No {get; set;}
	
	   
	   #endregion
    }
}
 
 