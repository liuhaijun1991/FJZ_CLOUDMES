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
    public class R_MPS
    {
        public R_MPS()
        {
			
        }
 
       #region  R_MPS实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime filedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string site {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime scheduleddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string brocadepartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal mps {get; set;}
	
	
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
	   public bool flag {get; set;}
	
	   
	   #endregion
    }
}
 
 