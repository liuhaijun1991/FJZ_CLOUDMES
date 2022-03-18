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
    public class EDI_FILE3
    {
        public EDI_FILE3()
        {
			
        }
 
       #region  EDI_FILE3实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IN_OUT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DOC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDT {get; set;}
	
	   
	   #endregion
    }
}
 
 