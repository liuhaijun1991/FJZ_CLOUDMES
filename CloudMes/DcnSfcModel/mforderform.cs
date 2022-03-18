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
    public class mforderform
    {
        public mforderform()
        {
			
        }
 
       #region  mforderform实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ordertype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime orderdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string referencenno1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string referencenno2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string referencenno3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string referencenno4 {get; set;}
	
	
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
 
 