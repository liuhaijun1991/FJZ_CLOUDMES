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
    public class mferpworkorder
    {
        public mferpworkorder()
        {
			
        }
 
       #region  mferpworkorder实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuversion {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workordertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int workorderqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime startdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool converted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime worktime {get; set;}
	
	   
	   #endregion
    }
}
 
 