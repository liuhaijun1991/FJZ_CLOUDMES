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
    public class porecvroute
    {
        public porecvroute()
        {
			
        }
 
       #region  porecvroute实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string podnno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int stageno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routepoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string contactname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime etadate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime atadate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime etddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime atddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool partial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool finished {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string contactagentno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string inchargeby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string gennote {get; set;}
	
	
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
 
 