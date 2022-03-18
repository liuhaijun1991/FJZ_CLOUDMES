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
    public class s_ort_qty
    {
        public s_ort_qty()
        {
			
        }
 
       #region  s_ort_qty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ortQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int markQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 