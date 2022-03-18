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
    public class C_STATIONEVENT_CONFIG
    {
        public C_STATIONEVENT_CONFIG()
        {
			
        }
 
       #region  C_STATIONEVENT_CONFIG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EVENT_DESC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTION_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int GROUP_ID {get; set;}
	
	   
	   #endregion
    }
}
 
 