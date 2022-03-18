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
    public class modulefunction
    {
        public modulefunction()
        {
			
        }
 
       #region  modulefunction实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sectionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string functionname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sortno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prompname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string webpage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool notshow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool newwindow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public Guid rowguid {get; set;}
	
	   
	   #endregion
    }
}
 
 