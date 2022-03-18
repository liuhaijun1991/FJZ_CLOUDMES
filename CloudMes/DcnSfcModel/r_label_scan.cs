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
    public class r_label_scan
    {
        public r_label_scan()
        {
			
        }
 
       #region  r_label_scan实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFCSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SCANSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SCANBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime SCANDT {get; set;}
	
	   
	   #endregion
    }
}
 
 