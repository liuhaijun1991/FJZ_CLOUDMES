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
    public class sfcfailureprocessinfo
    {
        public sfcfailureprocessinfo()
        {
			
        }
 
       #region  sfcfailureprocessinfo实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProcessName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProcessDesc1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ProcessDesc2 {get; set;}
	
	
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
 
 