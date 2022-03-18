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
    public class nsgsrepairstatus
    {
        public nsgsrepairstatus()
        {
			
        }
 
       #region  nsgsrepairstatus实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string codename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string failstation {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool checkin {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string checkinby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkindate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool checkout {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime checkoutdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string clear {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime cleardate {get; set;}
	
	
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
	   public string transfer {get; set;}
	
	   
	   #endregion
    }
}
 
 