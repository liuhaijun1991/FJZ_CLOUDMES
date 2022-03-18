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
    public class sfcEventMaster
    {
        public sfcEventMaster()
        {
			
        }
 
       #region  sfcEventMaster实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventFeature {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createDT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool defaultRoute {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool released {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime releasedate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool disabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool CollectMDS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool CollectAS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CustomerID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventProcess {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EventNote {get; set;}
	
	
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
 
 