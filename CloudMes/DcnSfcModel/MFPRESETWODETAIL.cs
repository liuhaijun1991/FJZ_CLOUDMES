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
    public class MFPRESETWODETAIL
    {
        public MFPRESETWODETAIL()
        {
			
        }
 
       #region  MFPRESETWODETAIL实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Wo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int RequestQty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitPrice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UnitWeight {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int PackageFlag {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNoType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime CreatTime {get; set;}
	
	   
	   #endregion
    }
}
 
 