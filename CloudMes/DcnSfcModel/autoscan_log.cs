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
    public class autoscan_log
    {
        public autoscan_log()
        {
			
        }
 
       #region  autoscan_log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sn {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string station {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string teststation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime testdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failure {get; set;}
	
	
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
 
 