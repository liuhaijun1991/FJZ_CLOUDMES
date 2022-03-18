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
    public class mminvphycount
    {
        public mminvphycount()
        {
			
        }
 
       #region  mminvphycount实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string countdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string whid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string auditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal currentqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal countqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string countby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool autoadjusted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime autoadjustdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string autoadjustby {get; set;}
	
	
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
 
 