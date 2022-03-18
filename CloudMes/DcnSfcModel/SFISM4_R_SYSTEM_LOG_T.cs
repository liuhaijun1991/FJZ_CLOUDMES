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
    public class SFISM4_R_SYSTEM_LOG_T
    {
        public SFISM4_R_SYSTEM_LOG_T()
        {
			
        }
 
       #region  SFISM4_R_SYSTEM_LOG_T实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EMP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRG_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACTION_TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ACTION_DESC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TIME {get; set;}
	
	   
	   #endregion
    }
}
 
 