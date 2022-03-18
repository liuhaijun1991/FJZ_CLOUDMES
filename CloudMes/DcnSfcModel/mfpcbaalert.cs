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
    public class mfpcbaalert
    {
        public mfpcbaalert()
        {
			
        }
 
       #region  mfpcbaalert实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Sku {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WrongSku {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Station {get; set;}
	
	
	   /// <summary>
	   /// 0
	   /// </summary>
	   public int Flag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Datetime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditedby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string memo {get; set;}
	
	   
	   #endregion
    }
}
 
 