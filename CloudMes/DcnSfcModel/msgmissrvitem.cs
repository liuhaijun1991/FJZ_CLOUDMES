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
    public class msgmissrvitem
    {
        public msgmissrvitem()
        {
			
        }
 
       #region  msgmissrvitem实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string servciceid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string servicecate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string serviceitem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string servicedesc {get; set;}
	
	   
	   #endregion
    }
}
 
 