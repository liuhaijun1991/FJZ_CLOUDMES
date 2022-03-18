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
    public class sfcalertlookup
    {
        public sfcalertlookup()
        {
			
        }
 
       #region  sfcalertlookup实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string eventpoint {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productiontype {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string productstatus {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal targetqty {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lowqtyredalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lowqtywarnalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal hiqtyalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal targetyieldrate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lowyieldrateredalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal lowyieldratewarnalert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal hiyieldratealert {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lowalertmessage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string lowwarnmessage {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string hialertmessage {get; set;}
	
	
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
 
 