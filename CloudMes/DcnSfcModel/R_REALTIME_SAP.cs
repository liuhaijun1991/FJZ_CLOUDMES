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
    public class R_REALTIME_SAP
    {
        public R_REALTIME_SAP()
        {
			
        }
 
       #region  R_REALTIME_SAP实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BUNAME {get; set;}
	
	
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
	   public string sapeventname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime eventdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int eventcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int objectqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sapqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sfcqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lasteditby {get; set;}
	
	   
	   #endregion
    }
}
 
 