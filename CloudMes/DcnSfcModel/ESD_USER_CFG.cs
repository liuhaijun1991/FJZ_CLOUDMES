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
    public class ESD_USER_CFG
    {
        public ESD_USER_CFG()
        {
			
        }
 
       #region  ESD_USER_CFG实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string userID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string name {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string department {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool valid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string role {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short schedule {get; set;}
	
	   
	   #endregion
    }
}
 
 