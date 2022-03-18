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
    public class mfwobatch
    {
        public mfwobatch()
        {
			
        }
 
       #region  mfwobatch实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string batchno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime batchdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string factoryid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productionline {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shift {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string jobdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string jobtime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool jobstarted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool mrpartial {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool mrcompleted {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool closed {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal batchqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal finishedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productlevel {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productcolor {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productlangulage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string prioritycode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string shipcountry {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime lasteditdt {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool released {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime releaseddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool canceled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime canceleddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lineseqnofrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lineseqnoto {get; set;}
	
	   
	   #endregion
    }
}
 
 