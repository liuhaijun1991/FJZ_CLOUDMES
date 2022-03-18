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
    public class mfEcoSys
    {
        public mfEcoSys()
        {
			
        }
 
       #region  mfEcoSys实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ecorouteunqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool upgraded {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string upgradedby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime upgradeddt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string notes {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ecounqno {get; set;}
	
	
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
 
 