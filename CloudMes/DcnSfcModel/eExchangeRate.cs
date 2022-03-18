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
    public class eExchangeRate
    {
        public eExchangeRate()
        {
			
        }
 
       #region  eExchangeRate实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CurrencyType {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ExchangeDate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double BuyInRate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double SellOutRate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public double CIQRate {get; set;}
	
	
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
 
 