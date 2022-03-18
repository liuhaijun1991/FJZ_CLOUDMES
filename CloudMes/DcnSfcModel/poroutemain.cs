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
    public class poroutemain
    {
        public poroutemain()
        {
			
        }
 
       #region  poroutemain实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeno {get; set;}
	
	
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
	   public string routedesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool defaultroute {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string contactname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string phone {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short unitoftime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string transportuom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string carrier {get; set;}
	
	
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
	   public string note {get; set;}
	
	
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
 
 