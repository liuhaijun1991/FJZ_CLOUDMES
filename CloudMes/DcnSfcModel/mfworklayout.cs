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
    public class mfworklayout
    {
        public mfworklayout()
        {
			
        }
 
       #region  mfworklayout实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string workorderno {get; set;}
	
	
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
	   public DateTime assigndate {get; set;}
	
	
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
	   public string releaseby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal targetqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal finishedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string labelFrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string labelTo {get; set;}
	
	
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
 
 