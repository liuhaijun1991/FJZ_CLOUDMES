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
    public class OEORDHDR
    {
        public OEORDHDR()
        {
			
        }
 
       #region  OEORDHDR实体
 
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public DateTime ORDER_DATE_ENTERED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ORDER_APPLY_TO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_PUR_ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_CUSTOMER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CUSTOMER_BAL_METHOD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BILL_TO_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BILL_TO_ADDRESS_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BILL_TO_ADDRESS_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BILL_TO_ADDRESS_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BILL_TO_COUNTRY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_NAME {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_ADDRESS_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_ADDRESS_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_ADDRESS_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_COUNTRY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal SHIPPING_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_VIA_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TERMS_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FREIGHT_PAY_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_INSTRUCTION_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_INSTRUCTION_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_NO_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_PCT_COMM_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_COMM_AMT_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_NO_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_PCT_COMM_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_COMM_AMT_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_NO_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_PCT_COMM_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALESMAN_COMM_AMT_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_CODE_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_PERCENT_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_CODE_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_PERCENT_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_CODE_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_PERCENT_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DISCOUNT_PERCENT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string JOB_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MFGING_LOCATION {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PROFIT_CENTER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DEPARTMENT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string AR_REFERENCE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_SALE_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_SALE_DISCOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_TAXABLE_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_COST {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_WEIGHT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MISC_CHARGES_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MISC_CHARGES_MN_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MISC_CHARGES_PC_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MISC_CHARGES_DP_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FREIGHT_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FREIGHT_MN_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FREIGHT_PC_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FREIGHT_DP_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_TAX_AMOUNT_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_TAX_AMOUNT_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_TAX_AMOUNT_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMMISSION_PERCENT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMMISSION_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMMENT_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMMENT_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COMMENT_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PAYMENT_AMOUNT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PAYMENT_DISCOUNT_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal CHECK_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal CHECK_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CASH_MN_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CASH_PC_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CASH_DP_ACCT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal DATE_PICKED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal DATE_BILLED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal INVOICE_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal INVOICE_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SELECTION_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal POSTED_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PART_POSTED_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SHIP_TO_FREE_FRM_FLG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string BILL_TO_FREE_FRM_FLG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string COPY_TO_BM_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string EDI_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CLOSED_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MISC_CHARGES_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FREIGHT_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOT_TAXABLE_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_TAX_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_SALE_AMT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HOLD_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PREPAYMENT_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string LOST_SALE_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORIG_ORDER_TYPE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ORIG_ORDER_DATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ORIG_ORDER_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal AWARD_PROBABILITY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal OE_CASH_KEY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_EXCHG_RT {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CURR_CODE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORIG_TRX_CURR_RATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string CURR_TRX_CURR_RATE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAX_SCHEDULE {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_1 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_2 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_3 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_4 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string USER_FIELD_5 {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DETERMINE_RATE_BY {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal FORM_NUMBER {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TAXABLE_FLAG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_ACCT_MN_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_ACCT_PC_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string SALES_ACCT_DP_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal ORDER_DATE_SHIPPED {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string ORDER_TOTAL_DOLLARS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string MULTI_LOCATION_FG {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string TOTAL_TAXABLE_COST {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string HISTORY_LOAD_RECORD {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string PRE_SELECTED_STATUS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal PACKING_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string DELIVERY_TERMS {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string INV_BATCH_ID {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public decimal RMA_NO {get; set;}
	
	
	   /// <summary>
	   /// 
	   /// </summary>
	   public string FILLER_0001 {get; set;}
	
	   
	   #endregion
    }
}
 
 