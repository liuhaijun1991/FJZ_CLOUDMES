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
    public class hasncontrol
    {
        public hasncontrol()
        {
			
        }
 
       #region  hasncontrol实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tpcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rectype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime transdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal reccount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal headcount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool converted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime convertdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string convertby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string uploadby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploaddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AsnFileSN {get; set;}
	
	   
	   #endregion
    }
}
 
 