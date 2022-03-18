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
    public class R_CARRIER_LINK
    {
        public R_CARRIER_LINK()
        {
			
        }
 
       #region  R_CARRIER_LINK实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SYSTEMSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CARRIERNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LASTEDITTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 