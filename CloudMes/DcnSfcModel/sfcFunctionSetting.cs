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
    public class sfcFunctionSetting
    {
        public sfcFunctionSetting()
        {
			
        }
 
       #region  sfcFunctionSetting实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ChangeDate {get; set;}
	
	
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
	   public string MacAddress {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SettingName {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SettingValue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SettingCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FunctionCategory {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SettingStr1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SettingStr2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SettingNum1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public int SettingNum2 {get; set;}
	
	
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
 
 