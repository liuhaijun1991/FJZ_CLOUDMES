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
    public class SFC_Deal_Request_Set
    {
        public SFC_Deal_Request_Set()
        {
			
        }
 
       #region  SFC_Deal_Request_Set实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProblemFamily {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Problem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ConfirmDP {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 