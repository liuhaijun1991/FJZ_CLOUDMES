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
    public class SAPLOG
    {
        public SAPLOG()
        {
			
        }
 
       #region  SAPLOG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFCUSER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SFCWEBPAGE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAPUSER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAPIP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SAPRFC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string REMARK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGTIME {get; set;}
	
	   
	   #endregion
    }
}
 
 