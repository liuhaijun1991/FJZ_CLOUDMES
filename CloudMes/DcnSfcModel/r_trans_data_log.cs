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
    public class r_trans_data_log
    {
        public r_trans_data_log()
        {
			
        }
 
       #region  r_trans_data_log实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string billtocode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime transdt {get; set;}
	
	   
	   #endregion
    }
}
 
 