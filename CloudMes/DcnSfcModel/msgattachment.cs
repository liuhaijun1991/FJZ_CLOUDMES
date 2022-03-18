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
    public class msgattachment
    {
        public msgattachment()
        {
			
        }
 
       #region  msgattachment实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string attachid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string articleid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string memoid {get; set;}
	
	
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
 
 