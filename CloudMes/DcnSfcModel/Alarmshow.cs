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
    public class Alarmshow
    {
        public Alarmshow()
        {
			
        }
 
       #region  Alarmshow实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int uph {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qtyship {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qtycbs {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int qtywip {get; set;}
	
	   
	   #endregion
    }
}
 
 