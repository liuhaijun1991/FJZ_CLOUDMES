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
    public class mmprodcategory
    {
        public mmprodcategory()
        {
			
        }
 
       #region  mmprodcategory实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categoryname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partnoprefix {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string categorydesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string materialtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sourcetype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string uom {get; set;}
	
	
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
 
 