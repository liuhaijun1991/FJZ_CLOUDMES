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
    public class R_BAOFEI
    {
        public R_BAOFEI()
        {
			
        }
 
       #region  R_BAOFEI实体
 
	
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
	   public DateTime BAOFEITIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LASTEDITBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REMARK {get; set;}
	
	   
	   #endregion
    }
}
 
 