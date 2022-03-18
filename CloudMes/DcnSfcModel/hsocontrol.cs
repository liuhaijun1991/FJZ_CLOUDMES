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
    public class hsocontrol
    {
        public hsocontrol()
        {
			
        }
 
       #region  hsocontrol实体
 
	
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
	   public string transtype {get; set;}
	
	
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
	   public bool parsed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string parseby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime parsedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string receiveby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime receivedate {get; set;}
	
	   
	   #endregion
    }
}
 
 