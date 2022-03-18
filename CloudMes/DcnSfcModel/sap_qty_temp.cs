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
    public class sap_qty_temp
    {
        public sap_qty_temp()
        {
			
        }
 
       #region  sap_qty_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string plant {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sku {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int FGI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int QFGI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SIP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int OPWO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int R_Value {get; set;}
	
	   
	   #endregion
    }
}
 
 