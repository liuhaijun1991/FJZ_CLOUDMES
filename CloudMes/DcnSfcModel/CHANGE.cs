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
    public class CHANGE
    {
        public CHANGE()
        {
			
        }
 
       #region  CHANGE实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHANGE_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Change_Type {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REASON {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CREATE_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RELEASE_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRODUCT_LINES {get; set;}
	
	   
	   #endregion
    }
}
 
 