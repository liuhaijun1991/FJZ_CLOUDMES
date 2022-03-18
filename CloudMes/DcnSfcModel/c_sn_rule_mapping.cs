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
    public class c_sn_rule_mapping
    {
        public c_sn_rule_mapping()
        {
			
        }
 
       #region  c_sn_rule_mapping实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string dataname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datavalue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mapping {get; set;}
	
	
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
 
 