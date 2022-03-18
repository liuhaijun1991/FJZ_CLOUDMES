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
    public class sfcFunctionAction
    {
        public sfcFunctionAction()
        {
			
        }
 
       #region  sfcFunctionAction实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SectionName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FunctionName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ActionName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SeqNo {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ActionOutput {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ActionCount {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ActionCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FunctionCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TemplateFile {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ActionStr1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ActionStr2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ActionNum1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int ActionNum2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool Enabled {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Comment {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LastEditBy {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime LastEditDT {get; set;}
	
	   
	   #endregion
    }
}
 
 