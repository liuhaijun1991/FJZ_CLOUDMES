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
    public class EDI_997_Head
    {
        public EDI_997_Head()
        {
			
        }
 
       #region  EDI_997_Head实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FileName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MessageID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AkCustomer {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AkPipe {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AkType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AkNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AkDateTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AkICN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime WorkTime {get; set;}
	
	   
	   #endregion
    }
}
 
 