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
    public class MDSReTransferlog
    {
        public MDSReTransferlog()
        {
			
        }
 
       #region  MDSReTransferlog实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string datapoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime uploadagaindt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string filename {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string description {get; set;}
	
	   
	   #endregion
    }
}
 
 