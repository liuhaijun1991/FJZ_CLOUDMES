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
    public class R_PACK_INFO
    {
        public R_PACK_INFO()
        {
			
        }
 
       #region  R_PACK_INFO实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PACKNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Gross_Weight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal Volumetric_Weight {get; set;}
	
	
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
 
 