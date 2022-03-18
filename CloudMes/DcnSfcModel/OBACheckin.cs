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
    public class OBACheckin
    {
        public OBACheckin()
        {
			
        }
 
       #region  OBACheckin实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string palletno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string scanby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scantime {get; set;}
	
	   
	   #endregion
    }
}
 
 