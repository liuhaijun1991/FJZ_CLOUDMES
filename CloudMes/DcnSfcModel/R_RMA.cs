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
    public class R_RMA
    {
        public R_RMA()
        {
			
        }
 
       #region  R_RMA实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WORKORDERNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime RMATIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool flag_update {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	   
	   #endregion
    }
}
 
 