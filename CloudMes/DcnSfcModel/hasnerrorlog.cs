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
    public class hasnerrorlog
    {
        public hasnerrorlog()
        {
			
        }
 
       #region  hasnerrorlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string orderlineno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO_Reference {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Shiporderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrorMeg {get; set;}
	
	
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
 
 