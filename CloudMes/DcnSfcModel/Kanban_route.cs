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
    public class Kanban_route
    {
        public Kanban_route()
        {
			
        }
 
       #region  Kanban_route实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string currentURL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string nextURL {get; set;}
	
	   
	   #endregion
    }
}
 
 