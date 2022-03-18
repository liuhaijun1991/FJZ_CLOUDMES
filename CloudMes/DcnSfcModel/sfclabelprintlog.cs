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
    public class sfclabelprintlog
    {
        public sfclabelprintlog()
        {
			
        }
 
       #region  sfclabelprintlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime printtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string printtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string macaddress {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string labelfrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string labelto {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string printby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string printresult {get; set;}
	
	
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
 
 