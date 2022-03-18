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
    public class member
    {
        public member()
        {
			
        }
 
       #region  member实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workerid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workername {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string department_id {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime workdt {get; set;}
	
	   
	   #endregion
    }
}
 
 