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
    public class mfworkordertype
    {
        public mfworkordertype()
        {
			
        }
 
       #region  mfworkordertype实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WorkOrderType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string category {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prefix {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OrderType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short Days1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short Days2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short AllDays {get; set;}
	
	
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
 
 