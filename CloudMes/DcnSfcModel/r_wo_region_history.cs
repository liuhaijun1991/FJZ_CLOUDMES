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
    public class r_wo_region_history
    {
        public r_wo_region_history()
        {
			
        }
 
       #region  r_wo_region_history实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prefix {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datatype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short length {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string from1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string to1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal from2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal to2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string suffix {get; set;}
	
	
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
 
 