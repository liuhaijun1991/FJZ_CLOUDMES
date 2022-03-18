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
    public class eModuleFunction
    {
        public eModuleFunction()
        {
			
        }
 
       #region  eModuleFunction实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MODULENAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SECTIONNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FUNCTIONNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int sortno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PROMPTNAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DESCRIPTION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FuncCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FuncFigure {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FuncVBForm {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FuncDesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string webpage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool notshow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool newwindow {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LUPBY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LUPDATE {get; set;}
	
	   
	   #endregion
    }
}
 
 