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
    public class R_Scheme
    {
        public R_Scheme()
        {
			
        }
 
       #region  R_Scheme实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public long SchemeID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Command {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string RunMode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastRunTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Parameters {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime NextRunTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Status {get; set;}
	
	   
	   #endregion
    }
}
 
 