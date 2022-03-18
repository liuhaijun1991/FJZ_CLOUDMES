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
    public class msgmisservice
    {
        public msgmisservice()
        {
			
        }
 
       #region  msgmisservice实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string servciceid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string requestdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logonname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string employeeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string department {get; set;}
	
	   
	   #endregion
    }
}
 
 