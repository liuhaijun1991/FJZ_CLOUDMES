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
    public class borrow_return_sn
    {
        public borrow_return_sn()
        {
			
        }
 
       #region  borrow_return_sn实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string sysserialno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string borrowtype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string borrowerid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string borrowername {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dept {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string tel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string director {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime borrowdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime estimatereturndate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string returnederid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string returnedername {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string returndate {get; set;}
	
	
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
 
 