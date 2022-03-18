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
    public class C_AssemblyMap
    {
        public C_AssemblyMap()
        {
			
        }
 
       #region  C_AssemblyMap实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GroupID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Custkpno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Location {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int Qty {get; set;}
	
	   
	   #endregion
    }
}
 
 