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
    public class SFC_TEST_Search
    {
        public SFC_TEST_Search()
        {
			
        }
 
       #region  SFC_TEST_Search实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime testdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string power1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sensitivity1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string power2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sensitivity2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string power3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sensitivity3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TATIME {get; set;}
	
	   
	   #endregion
    }
}
 
 