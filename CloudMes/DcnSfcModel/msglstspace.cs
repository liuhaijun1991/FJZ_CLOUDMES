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
    public class msglstspace
    {
        public msglstspace()
        {
			
        }
 
       #region  msglstspace实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string listid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string publishdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string expiredate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string department {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string companycode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool departmentlevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool companylevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool enterpriselevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool extranetlevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string headline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string detailcontent {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string filename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string fileextension {get; set;}
	
	
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
 
 