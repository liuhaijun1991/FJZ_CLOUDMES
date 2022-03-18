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
    public class so_asn
    {
        public so_asn()
        {
			
        }
 
       #region  so_asn实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Line {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ASN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 