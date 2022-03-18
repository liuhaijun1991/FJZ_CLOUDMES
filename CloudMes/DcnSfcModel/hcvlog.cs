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
    public class hcvlog
    {
        public hcvlog()
        {
			
        }
 
       #region  hcvlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime transdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string filename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string type {get; set;}
	
	
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
	   public string ref3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ref4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ref5 {get; set;}
	
	   
	   #endregion
    }
}
 
 