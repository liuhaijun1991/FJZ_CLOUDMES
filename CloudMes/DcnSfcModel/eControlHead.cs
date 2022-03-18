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
    public class eControlHead
    {
        public eControlHead()
        {
			
        }
 
       #region  eControlHead实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLLEVEL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	   
	   #endregion
    }
}
 
 