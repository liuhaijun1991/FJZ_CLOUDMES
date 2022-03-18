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
    public class SAVE_RESULTS
    {
        public SAVE_RESULTS()
        {
			
        }
 
       #region  SAVE_RESULTS实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Status {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrorCode1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ErrorCode2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TestTime {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Hostname {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Model {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Package {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ESN_DEC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ESN_HEX {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SKU {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FW_REV {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AKEY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IMSI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MSL_SPC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OTKSL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOCK_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NAI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CHAP_KEY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QNC_USER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string QNC_PASS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HA_KEY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AAA_KEY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CONFIG_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string U_VER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRI_VER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AKEY2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IMEI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string GSM_LOCK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string UNLOCK_NS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ME_LOCK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ICCID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IMSI_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BATCH_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HOST_FSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HOST_FW_VER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MNA {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HOST_BATCH {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRL {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Royalty1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Royalty2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CSN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HDRAN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MEID_DEC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MEID_HEX {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pESN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string pHEX {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WIFI_MAC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ETHERNET_MAC {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IDSCRE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string IDSTPS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string APPPBK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DEVLOGIN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DEVPWD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FWPBK {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string NONCE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SVRPWD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRL2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string Battery_SN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string STN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string OUTER_BOX {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string W_SSID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string W_ADMIN_PWD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string WPSPIN {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string W_Passphrase {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AT_Custom {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FW_REV_VZW_GOBI {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short FAILCOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public bool FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime TATIME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public short PASSCOUNT {get; set;}
	
	   
	   #endregion
    }
}
 
 