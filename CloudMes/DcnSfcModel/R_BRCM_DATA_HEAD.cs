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
    public class R_BRCM_DATA_HEAD
    {
        public R_BRCM_DATA_HEAD()
        {
			
        }
 
       #region  R_BRCM_DATA_HEAD实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DNNO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SONO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SOLINENO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FILENAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FILETYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CREATETIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CREATEFILETIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime SENDTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 