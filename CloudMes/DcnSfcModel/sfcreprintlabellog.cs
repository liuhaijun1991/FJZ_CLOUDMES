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
    public class sfcreprintlabellog
    {
        public sfcreprintlabellog()
        {
			
        }
 
       #region  sfcreprintlabellog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PrintSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string macaddress {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string printby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime printdate {get; set;}
	
	   
	   #endregion
    }
}
 
 