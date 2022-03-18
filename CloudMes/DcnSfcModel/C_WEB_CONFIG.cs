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
    public class C_WEB_CONFIG
    {
        public C_WEB_CONFIG()
        {
			
        }
 
       #region  C_WEB_CONFIG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AP_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTION_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int LAYER_COUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SORT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTION_DESC_CN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTION_DESC_EN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EDIT_EMP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EDIT_TIME {get; set;}
	
	   
	   #endregion
    }
}
 
 