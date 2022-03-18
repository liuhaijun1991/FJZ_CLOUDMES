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
    public class Labelcheckoutdetail
    {
        public Labelcheckoutdetail()
        {
			
        }
 
       #region  Labelcheckoutdetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Label_pn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stockqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int totalqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkouttime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkoutby {get; set;}
	
	   
	   #endregion
    }
}
 
 