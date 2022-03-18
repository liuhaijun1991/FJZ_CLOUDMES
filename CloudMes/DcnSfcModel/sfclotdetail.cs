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
    public class sfclotdetail
    {
        public sfclotdetail()
        {
			
        }
 
       #region  sfclotdetail实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lotno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool undetermined {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string palletno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string errcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string faillocation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool rework {get; set;}
	
	
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
 
 