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
    public class R_SN_IMEI
    {
        public R_SN_IMEI()
        {
			
        }
 
       #region  R_SN_IMEI实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IMEI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime EDITDT {get; set;}
	
	   
	   #endregion
    }
}
 
 