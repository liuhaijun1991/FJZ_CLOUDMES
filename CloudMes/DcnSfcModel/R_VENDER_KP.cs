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
    public class R_VENDER_KP
    {
        public R_VENDER_KP()
        {
			
        }
 
       #region  R_VENDER_KP实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string KP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VENDER_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string VENDER_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MPN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WORKTIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 