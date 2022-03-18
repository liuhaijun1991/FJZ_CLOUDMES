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
    public class eControlValue
    {
        public eControlValue()
        {
			
        }
 
       #region  eControlValue实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLVALUE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLDESC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLTYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONTROLLEVEL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ControlNote {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SortNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATA1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATA2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DATA3 {get; set;}
	
	   
	   #endregion
    }
}
 
 