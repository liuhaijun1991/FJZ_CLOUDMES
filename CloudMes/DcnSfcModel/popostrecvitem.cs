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
    public class popostrecvitem
    {
        public popostrecvitem()
        {
			
        }
 
       #region  popostrecvitem实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string receiveno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string vendorpartno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short seqno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string partdesc {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal unitprice {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal receivedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal returnqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal flaginqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal rtnflaginqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal rtnreceivedqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool reject {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string rejectreason {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string purchaseno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string podnno {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal costqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime costdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool costed {get; set;}
	
	
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
 
 