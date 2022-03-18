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
    public class SdConvertBom
    {
        public SdConvertBom()
        {
			
        }
 
       #region  SdConvertBom实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string jobcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parentpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partdescription {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partversion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal perqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string field2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal field3 {get; set;}
	
	
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
 
 