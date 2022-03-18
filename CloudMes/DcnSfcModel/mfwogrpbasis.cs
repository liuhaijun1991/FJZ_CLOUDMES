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
    public class mfwogrpbasis
    {
        public mfwogrpbasis()
        {
			
        }
 
       #region  mfwogrpbasis实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string basisname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string basisdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool ood {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ooddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string createby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime createdate {get; set;}
	
	
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
 
 