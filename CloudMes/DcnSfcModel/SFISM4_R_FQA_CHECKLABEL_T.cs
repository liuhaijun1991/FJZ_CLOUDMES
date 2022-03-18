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
    public class SFISM4_R_FQA_CHECKLABEL_T
    {
        public SFISM4_R_FQA_CHECKLABEL_T()
        {
			
        }
 
       #region  SFISM4_R_FQA_CHECKLABEL_T实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LABEL_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GROUP_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime PASS_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime UPD_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EMP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PASS_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHECKSUM {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MEMO {get; set;}
	
	   
	   #endregion
    }
}
 
 