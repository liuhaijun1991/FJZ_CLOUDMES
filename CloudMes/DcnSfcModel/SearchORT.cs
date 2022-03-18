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
    public class SearchORT
    {
        public SearchORT()
        {
			
        }
 
       #region  SearchORT实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sku {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ortevent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int counter {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mdstime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string worktime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UserID {get; set;}
	
	   
	   #endregion
    }
}
 
 