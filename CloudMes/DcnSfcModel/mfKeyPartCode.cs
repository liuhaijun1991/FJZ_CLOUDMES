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
    public class mfKeyPartCode
    {
        public mfKeyPartCode()
        {
			
        }
 
       #region  mfKeyPartCode实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string routeid {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PartNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CodeNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ValidDTFrom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ValidDTTo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Str1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int lng1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Description {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Lasteditby {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime Lasteditdt {get; set;}
	
	   
	   #endregion
    }
}
 
 