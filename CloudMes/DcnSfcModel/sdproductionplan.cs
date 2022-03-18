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
    public class sdproductionplan
    {
        public sdproductionplan()
        {
			
        }
 
       #region  sdproductionplan实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string custpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string skuno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime workdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string mps_version {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal pcbawipqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal pcbaaoi2qty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal pcbarepairqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal pcbaictqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal boxaiqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal finishgoodsqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal finishgoodsmrbqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal shipqty {get; set;}
	
	
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
 
 