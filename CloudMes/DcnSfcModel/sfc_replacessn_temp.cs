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
    public class sfc_replacessn_temp
    {
        public sfc_replacessn_temp()
        {
			
        }
 
       #region  sfc_replacessn_temp实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string oldsn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string newsn {get; set;}
	
	
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
 
 