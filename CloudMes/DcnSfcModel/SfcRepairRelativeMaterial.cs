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
    public class SfcRepairRelativeMaterial
    {
        public SfcRepairRelativeMaterial()
        {
			
        }
 
       #region  SfcRepairRelativeMaterial实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string modelname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string componentcode {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int quantity {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string returnstation {get; set;}
	
	
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
 
 