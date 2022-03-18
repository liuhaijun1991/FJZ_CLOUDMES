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
    public class eProperty
    {
        public eProperty()
        {
			
        }
 
       #region  eProperty实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PROPERTYTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PROPERTYHEAD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PROPERTYNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PROPERTYVALUE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOGONNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
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
 
 