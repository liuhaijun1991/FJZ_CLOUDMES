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
    public class vhdm_train
    {
        public vhdm_train()
        {
			
        }
 
       #region  vhdm_train实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal num {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string subject {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string des {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string trainfile {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prepared {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploaddt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lastedit {get; set;}
	
	   
	   #endregion
    }
}
 
 