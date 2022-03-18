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
    public class euseraction
    {
        public euseraction()
        {
			
        }
 
       #region  euseraction实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tableName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string actionCode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SQL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string result {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ip {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string userhostname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string logonname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime edittime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AccessPage {get; set;}
	
	   
	   #endregion
    }
}
 
 