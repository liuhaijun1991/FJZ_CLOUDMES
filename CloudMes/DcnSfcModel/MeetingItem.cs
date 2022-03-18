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
    public class MeetingItem
    {
        public MeetingItem()
        {
			
        }
 
       #region  MeetingItem实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string meetingID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short itemID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string issue {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string actionitem {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string responder {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime startdate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime finisheddate {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string status {get; set;}
	
	
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
 
 