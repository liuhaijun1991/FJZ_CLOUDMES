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
    public class mftransaction
    {
        public mftransaction()
        {
			
        }
 
       #region  mftransaction实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime transdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lastqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal requestqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal currentqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transref1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transref2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	   
	   #endregion
    }
}
 
 