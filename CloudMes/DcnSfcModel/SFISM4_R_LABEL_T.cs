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
    public class SFISM4_R_LABEL_T
    {
        public SFISM4_R_LABEL_T()
        {
			
        }
 
       #region  SFISM4_R_LABEL_T实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MD5 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LABEL_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LABEL_PATH {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime INPUT_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRINTER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATA1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATA2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WORKDATE1 {get; set;}
	
	   
	   #endregion
    }
}
 
 