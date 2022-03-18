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
    public class sfcsamplelotstatus
    {
        public sfcsamplelotstatus()
        {
			
        }
 
       #region  sfcsamplelotstatus实体
 
	
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
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lotqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal passqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal failqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lotstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
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
 
 