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
    public class AsDataLog
    {
        public AsDataLog()
        {
			
        }
 
       #region  AsDataLog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string senddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sendby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ref1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ref2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime datafilename {get; set;}
	
	   
	   #endregion
    }
}
 
 