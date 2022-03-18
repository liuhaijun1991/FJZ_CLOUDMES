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
    public class msgbbsmessage
    {
        public msgbbsmessage()
        {
			
        }
 
       #region  msgbbsmessage实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string bbsagenda {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string messageid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replytomsgid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool attachment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string replytoemail {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string headline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string detailconent {get; set;}
	
	
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
 
 