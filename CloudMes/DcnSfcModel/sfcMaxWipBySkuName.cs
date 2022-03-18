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
    public class sfcMaxWipBySkuName
    {
        public sfcMaxWipBySkuName()
        {
			
        }
 
       #region  sfcMaxWipBySkuName实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SkuName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int MaxWip {get; set;}
	
	
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
 
 