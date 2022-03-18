using MESDataObject.Module;
using MESDataObject.Module.DCN;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MES_DCN.Broadcom
{
    public class PackingList
    {      
        SqlSugarClient SFCDB= null;
        int FirstPageSNRows = 10;
        int NextPageSNRows = 80;
        public PackingList( SqlSugarClient _sfcdb) {           
            SFCDB = _sfcdb;
        }
        public PackingList(SqlSugarClient _sfcdb,int snRows1,int snRows2)
        {
            SFCDB = _sfcdb;
            FirstPageSNRows = snRows1;
            NextPageSNRows = snRows2;
        }
        public Spire.Xls.Workbook GetWorkbook(string labelName, BroadcomPackingList list) {

            Spire.Xls.Workbook plWorkbook = new Spire.Xls.Workbook();               
            R_Label tempLabel = SFCDB.Queryable<R_Label>().Where(r => r.LABELNAME == labelName).ToList().FirstOrDefault();            
            if (tempLabel == null)
            {
                throw new Exception($@"{labelName} Label Not Exists.R_LABEL");
            }
            R_FILE file = SFCDB.Queryable<R_FILE>().Where(r => r.NAME == tempLabel.LABELNAME && r.USETYPE == "LABEL" && r.VALID == 1).ToList().FirstOrDefault();
            if (file == null)
            {
                throw new Exception($@"{tempLabel.R_FILE_NAME} Not Exists.R_FILE");
            }
            byte[] template = (byte[])file.BLOB_FILE;
           
            plWorkbook.Worksheets.Clear();
            int printAreaIndex = 0;
            int currentPage = 1,totalPage=0;

            for (int p = 0; p < list.PalletList.Count; p++)
            {
                if (p == 0)
                {
                    if (list.PalletList[p].SNList.Count <= FirstPageSNRows)
                    {
                        totalPage += 1;
                    }
                    else
                    {
                        totalPage = (list.PalletList[p].SNList.Count - FirstPageSNRows) / NextPageSNRows == 0 ? totalPage + 2 : totalPage + 3;
                    }
                }
                else
                {
                    totalPage = list.PalletList[p].SNList.Count / NextPageSNRows == 0 ? totalPage + 1 : totalPage + 2;
                }
            }

            Spire.Xls.Worksheet temp_page = null;
            using (Spire.Xls.Workbook tempfile = new Spire.Xls.Workbook())
            {
                System.IO.Stream stream = new System.IO.MemoryStream(template);
                tempfile.LoadFromStream(stream);
                int palletPage = 0;
                int snIndex = 0;
                int rowIndx = 0;
                for (int i = 0; i < list.PalletList.Count; i++)
                {
                    palletPage = 0;
                    if (i == 0)
                    {
                        #region 第一頁固定
                        tempfile.Worksheets[1].Range["H5"].Text = string.IsNullOrEmpty(list.SALES_ORDER_NUMBER) ? "" : list.SALES_ORDER_NUMBER;
                        tempfile.Worksheets[1].Range["H6"].Text = string.IsNullOrEmpty(list.SHIP_DATE) ? "" : list.SHIP_DATE;
                        tempfile.Worksheets[1].Range["H7"].Text = string.IsNullOrEmpty(list.CUSTOMER_PO_NUMBER) ? "" : list.CUSTOMER_PO_NUMBER;
                        tempfile.Worksheets[1].Range["H8"].Text = string.IsNullOrEmpty(list.DN_NO) ? "" : $"FXV{list.DN_NO}";
                        tempfile.Worksheets[1].Range["H9"].Text = string.IsNullOrEmpty(list.DN_NO) ? "" : $"*FXV{list.DN_NO}*";
                        tempfile.Worksheets[1].Range["K5"].Text = $"{currentPage}/{totalPage}";

                        tempfile.Worksheets[1].Range["B15"].Text = string.IsNullOrEmpty(list.BILL_TO_COMPANY_NAME) ? "" : list.BILL_TO_COMPANY_NAME;
                        tempfile.Worksheets[1].Range["B16"].Text = string.IsNullOrEmpty(list.BILL_TO_ADDRESS1) ? "" : list.BILL_TO_ADDRESS1;
                        tempfile.Worksheets[1].Range["B17"].Text = string.IsNullOrEmpty(list.BILL_TO_ADDRESS2) ? "" : list.BILL_TO_ADDRESS2;
                        tempfile.Worksheets[1].Range["B18"].Text = string.IsNullOrEmpty(list.BILL_TO_ADDRESS3) ? "" : list.BILL_TO_ADDRESS3;
                        tempfile.Worksheets[1].Range["B19"].Text = string.IsNullOrEmpty(list.BILL_TO_ADDRESS4) ? "" : list.BILL_TO_ADDRESS4;
                        tempfile.Worksheets[1].Range["B20"].Text = new Func<string>(() => {
                            string city = string.IsNullOrEmpty(list.BILL_TO_CITY) ? "":list.BILL_TO_CITY;
                            string state = string.IsNullOrEmpty(list.BILL_TO_STATE) ? "" : list.BILL_TO_STATE;
                            string postalCode = string.IsNullOrEmpty(list.BILL_TO_POSTAL_CODE) ? "" : list.BILL_TO_POSTAL_CODE;
                            return $"{ city},{state},{postalCode}";
                        })();
                        tempfile.Worksheets[1].Range["B21"].Text = string.IsNullOrEmpty(list.BILL_TO_COUNTRY) ? "" : list.BILL_TO_COUNTRY;
                        tempfile.Worksheets[1].Range["D24"].Text = string.IsNullOrEmpty(list.SHIPPING_METHOD) ? "" : list.SHIPPING_METHOD;
                        tempfile.Worksheets[1].Range["B26"].Text = string.IsNullOrEmpty(list.SHIPPING_NOTE) ? "" : list.SHIPPING_NOTE;
                        tempfile.Worksheets[1].Range["G15"].Text = string.IsNullOrEmpty(list.SHIP_TO_COMPANY_NAME) ? "" : list.SHIP_TO_COMPANY_NAME;
                        tempfile.Worksheets[1].Range["G16"].Text = string.IsNullOrEmpty(list.SHIP_TO_ADDRESS1) ? "" : list.SHIP_TO_ADDRESS1;
                        tempfile.Worksheets[1].Range["G17"].Text = string.IsNullOrEmpty(list.SHIP_TO_ADDRESS2) ? "" : list.SHIP_TO_ADDRESS2;
                        tempfile.Worksheets[1].Range["G18"].Text = string.IsNullOrEmpty(list.SHIP_TO_ADDRESS3) ? "" : list.SHIP_TO_ADDRESS3;
                        tempfile.Worksheets[1].Range["G19"].Text = string.IsNullOrEmpty(list.SHIP_TO_ADDRESS4) ? "" : list.SHIP_TO_ADDRESS4;
                        tempfile.Worksheets[1].Range["G20"].Text = new Func<string>(() => {
                            string city = string.IsNullOrEmpty(list.SHIP_TO_CITY) ? "" : list.SHIP_TO_CITY;
                            string state = string.IsNullOrEmpty(list.SHIP_TO_STATE) ? "" : list.SHIP_TO_STATE;
                            string postalCode = string.IsNullOrEmpty(list.SHIP_TO_POSTAL_CODE) ? "" : list.SHIP_TO_POSTAL_CODE;
                            return $"{ city},{state},{postalCode}";
                        })();
                        tempfile.Worksheets[1].Range["G21"].Text = string.IsNullOrEmpty(list.SHIP_TO_COUNTRY) ? "" : list.SHIP_TO_COUNTRY;
                        tempfile.Worksheets[1].Range["H24"].Text = string.IsNullOrEmpty(list.INCO_TERM) ? "" : list.INCO_TERM;
                        tempfile.Worksheets[1].Range["H25"].Text = string.IsNullOrEmpty(list.FobCode) ? "" : list.FobCode;
                        tempfile.Worksheets[1].Range["B35"].Text = string.IsNullOrEmpty(list.Solineno) ? "" : list.Solineno;
                        tempfile.Worksheets[1].Range["C35"].Text = string.IsNullOrEmpty(list.SKUNO) ? "" : $"BP/N:{list.SKUNO}";
                        tempfile.Worksheets[1].Range["C36"].Text = string.IsNullOrEmpty(list.Itemdesc) ? "" : $"BP/D:{list.Itemdesc}";
                        tempfile.Worksheets[1].Range["C38"].Text = string.IsNullOrEmpty(list.CUSTOMER_ITEM) ? "" : $"CP/N:{list.CUSTOMER_ITEM}";
                        #endregion

                        #region 第一頁第一個表格
                        int insertRows = list.PalletList.Count > 4 ? list.PalletList.Count - 4 : 0;
                        if (insertRows > 0)
                        {
                            tempfile.Worksheets[1].InsertRow(39, insertRows,Spire.Xls.InsertOptionsType.FormatAsBefore);
                        }
                        for (int k = 0; k < list.PalletList.Count; k++)
                        {
                            tempfile.Worksheets[1].Range["D" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].Packages) ? "" : list.PalletList[k].Packages;
                            tempfile.Worksheets[1].Range["E" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].QuantityShipped) ? "" : list.PalletList[k].QuantityShipped;
                            tempfile.Worksheets[1].Range["F" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].UOM) ? "" : list.PalletList[k].UOM;
                            tempfile.Worksheets[1].Range["G" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].BoxWeight) ? "" : list.PalletList[k].BoxWeight;
                            tempfile.Worksheets[1].Range["H" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].NetWeightUnit) ? "" : list.PalletList[k].NetWeightUnit;
                            tempfile.Worksheets[1].Range["I" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].VolumetricWeight) ? "" : list.PalletList[k].VolumetricWeight;
                            tempfile.Worksheets[1].Range["J" + (35 + k).ToString()].Text = string.IsNullOrEmpty(list.PalletList[k].CountryOfOrigin) ? "" : list.PalletList[k].CountryOfOrigin;
                        }
                        insertRows += 1;
                        tempfile.Worksheets[1].Range["D" + (39 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.TotalPackages) ? "" : list.TotalPackages;
                        tempfile.Worksheets[1].Range["E" + (39 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.TotalQuantityShipped) ? "" : list.TotalQuantityShipped;
                        tempfile.Worksheets[1].Range["F" + (39 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[0].UOM) ? "" : list.PalletList[0].UOM;
                        tempfile.Worksheets[1].Range["G" + (39 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.TotalGrossWeight) ? "" : list.TotalGrossWeight;
                        tempfile.Worksheets[1].Range["H" + (39 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.TotalNetWeight) ? "" : list.TotalNetWeight;
                        tempfile.Worksheets[1].Range["I" + (39 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.Total_Volumetric_Weight_Sum) ? "" : list.Total_Volumetric_Weight_Sum;
                        #endregion

                        #region 第一頁第二表格
                        tempfile.Worksheets[1].Range["B" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.Solineno) ? "" : list.Solineno;
                        tempfile.Worksheets[1].Range["C" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.SKUNO) ? "" : $"BP/N:{list.SKUNO}";
                        tempfile.Worksheets[1].Range["C" + (44 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.CUSTOMER_ITEM) ? "" : $"CP/N:{list.CUSTOMER_ITEM}";

                        tempfile.Worksheets[1].Range["D" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[i].BoxLPNNumber) ? "" : list.PalletList[i].BoxLPNNumber;
                        tempfile.Worksheets[1].Range["E" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[i].QuantityShipped) ? "" : list.PalletList[i].QuantityShipped;
                        tempfile.Worksheets[1].Range["F" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[i].UOM) ? "" : list.PalletList[i].UOM;
                        tempfile.Worksheets[1].Range["G" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[i].BoxWeight) ? "" : list.PalletList[i].BoxWeight;
                        tempfile.Worksheets[1].Range["H" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[i].NetWeightUnit) ? "" : list.PalletList[i].NetWeightUnit;
                        tempfile.Worksheets[1].Range["I" + (43 + insertRows).ToString()].Text = string.IsNullOrEmpty(list.PalletList[i].VolumetricWeight) ? "" : $"{list.PalletList[i].VolumetricWeight}";
                        #endregion 第一頁第二表格

                        snIndex = 0;
                        rowIndx = 0;
                        while (true)
                        {
                            string sn1 = "", sn2 = "";
                            try
                            {
                                sn1 = list.PalletList[i].SNList[snIndex];                                
                            }
                            catch (Exception)
                            {
                                sn1 = "";
                            }
                            try
                            {
                                snIndex++;
                                sn2 = list.PalletList[i].SNList[snIndex];
                            }
                            catch (Exception)
                            {
                                sn2 = "";
                            }
                            tempfile.Worksheets[1].Range["B" + (47 + insertRows + rowIndx).ToString()].Text = sn1;
                            tempfile.Worksheets[1].Range["D" + (47 + insertRows + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn1) ? "" : $"*{sn1}*"; 
                            tempfile.Worksheets[1].Range["G" + (47 + insertRows + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn2) ? "" : sn2;
                            tempfile.Worksheets[1].Range["H" + (47 + insertRows + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn2) ? "" : $"*{sn2}*";
                            snIndex++;
                            rowIndx++;
                            if (snIndex >= FirstPageSNRows)
                            {
                                break;
                            }
                        }
                        
                        plWorkbook.Worksheets.AddCopy(tempfile.Worksheets[1]);
                        //printAreaIndex = 68;// Convert.ToInt32(tempfile.Worksheets[1].PageSetup.PrintArea.Split('$')[4].ToString());
                        printAreaIndex = GetPrintAreaIndex(tempfile.Worksheets[1].PageSetup.PrintArea.Split(':')[1]);

                        if (list.PalletList[i].SNList.Count > FirstPageSNRows)
                        {
                            palletPage = (list.PalletList[i].SNList.Count - FirstPageSNRows) / NextPageSNRows == 0 ? palletPage + 1 : palletPage + 2;
                            temp_page = tempfile.Worksheets[2];
                            for (int pp = 0; pp < palletPage; pp++)
                            {
                                currentPage++;
                                temp_page.Range["H3"].Text = string.IsNullOrEmpty(list.SALES_ORDER_NUMBER) ? "" : list.SALES_ORDER_NUMBER;
                                temp_page.Range["H4"].Text = string.IsNullOrEmpty(list.SHIP_DATE) ? "" : list.SHIP_DATE;
                                temp_page.Range["H5"].Text = string.IsNullOrEmpty(list.CUSTOMER_PO_NUMBER) ? "" : list.CUSTOMER_PO_NUMBER;
                                temp_page.Range["H6"].Text = string.IsNullOrEmpty(list.DN_NO) ? "" : $"FXV{list.DN_NO}";
                                temp_page.Range["H7"].Text = string.IsNullOrEmpty(list.DN_NO) ? "" : $"*FXV{list.DN_NO}*";
                                temp_page.Range["K3"].Text = $"{currentPage}/{totalPage}";

                                temp_page.Range["B11"].Text = string.IsNullOrEmpty(list.Solineno) ? "" : list.Solineno;
                                temp_page.Range["C11"].Text = string.IsNullOrEmpty(list.SKUNO) ? "" : $"BP/N:{list.SKUNO}";
                                temp_page.Range["C12"].Text = string.IsNullOrEmpty(list.CUSTOMER_ITEM) ? "" : $"CP/N:{list.CUSTOMER_ITEM}";

                                temp_page.Range["D11"].Text = string.IsNullOrEmpty(list.PalletList[i].BoxLPNNumber) ? "" : list.PalletList[i].BoxLPNNumber;
                                temp_page.Range["E11"].Text = string.IsNullOrEmpty(list.PalletList[i].QuantityShipped) ? "" : list.PalletList[i].QuantityShipped;
                                temp_page.Range["F11"].Text = string.IsNullOrEmpty(list.PalletList[i].UOM) ? "" : list.PalletList[i].UOM;
                                temp_page.Range["G11"].Text = string.IsNullOrEmpty(list.PalletList[i].BoxWeight) ? "" : list.PalletList[i].BoxWeight;
                                temp_page.Range["H11"].Text = string.IsNullOrEmpty(list.PalletList[i].NetWeightUnit) ? "" : list.PalletList[i].NetWeightUnit;
                                temp_page.Range["I11"].Text = string.IsNullOrEmpty(list.PalletList[i].VolumetricWeight) ? "" : $"{list.PalletList[i].VolumetricWeight}";
                                snIndex = pp * NextPageSNRows + FirstPageSNRows;
                                rowIndx = 0;
                                while (true)
                                {                                    
                                    string sn1 = "", sn2 = "";
                                    try
                                    {
                                        sn1 = list.PalletList[i].SNList[snIndex];
                                    }
                                    catch (Exception)
                                    {
                                        sn1 = "";
                                    }
                                    try
                                    {
                                        snIndex++;
                                        sn2 = list.PalletList[i].SNList[snIndex];
                                    }
                                    catch (Exception)
                                    {
                                        sn2 = "";
                                    }
                                    temp_page.Range["B" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn1) ? "" : sn1; 
                                    temp_page.Range["D" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn1) ? "" : $"*{sn1}*";
                                    temp_page.Range["G" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn2) ? "" : sn2;
                                    temp_page.Range["H" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn2) ? "" : $"*{sn2}*";
                                    snIndex++;
                                    rowIndx++;
                                    if (snIndex >= (pp + 1) * NextPageSNRows + FirstPageSNRows)
                                    {
                                        break;
                                    }
                                }
                                temp_page.CopyRow(temp_page.PrintRange, plWorkbook.Worksheets[0], 68 * (currentPage-1) + 1, Spire.Xls.CopyRangeOptions.All);
                                printAreaIndex += string.IsNullOrEmpty(temp_page.PageSetup.PrintArea) ? 68 : GetPrintAreaIndex(temp_page.PageSetup.PrintArea.Split(':')[1]);
                            }
                        }
                    }
                    else
                    {

                        palletPage = (list.PalletList[i].SNList.Count - FirstPageSNRows) / NextPageSNRows == 0 ? palletPage + 1 : palletPage + 2;
                        temp_page = tempfile.Worksheets[2];
                        for (int pp = 0; pp < palletPage; pp++)
                        {
                            currentPage++;
                            temp_page.Range["H3"].Text = string.IsNullOrEmpty(list.SALES_ORDER_NUMBER) ? "" : list.SALES_ORDER_NUMBER;
                            temp_page.Range["H4"].Text = string.IsNullOrEmpty(list.SHIP_DATE) ? "" : list.SHIP_DATE;
                            temp_page.Range["H5"].Text = string.IsNullOrEmpty(list.CUSTOMER_PO_NUMBER) ? "" : list.CUSTOMER_PO_NUMBER;
                            temp_page.Range["H6"].Text = string.IsNullOrEmpty(list.DN_NO) ? "" : $"FXV{list.DN_NO}";
                            temp_page.Range["H7"].Text = string.IsNullOrEmpty(list.DN_NO) ? "" : $"*FXV{list.DN_NO}*";
                            temp_page.Range["K3"].Text = $"{currentPage}/{totalPage}";

                            temp_page.Range["B11"].Text = string.IsNullOrEmpty(list.Solineno) ? "" : list.Solineno;
                            temp_page.Range["C11"].Text = string.IsNullOrEmpty(list.SKUNO) ? "" : $"BP/N:{list.SKUNO}";
                            temp_page.Range["C12"].Text = string.IsNullOrEmpty(list.CUSTOMER_ITEM) ? "" : $"CP/N:{list.CUSTOMER_ITEM}";

                            temp_page.Range["D11"].Text = string.IsNullOrEmpty(list.PalletList[i].BoxLPNNumber) ? "" : list.PalletList[i].BoxLPNNumber;
                            temp_page.Range["E11"].Text = string.IsNullOrEmpty(list.PalletList[i].QuantityShipped) ? "" : list.PalletList[i].QuantityShipped;
                            temp_page.Range["F11"].Text = string.IsNullOrEmpty(list.PalletList[i].UOM) ? "" : list.PalletList[i].UOM;
                            temp_page.Range["G11"].Text = string.IsNullOrEmpty(list.PalletList[i].BoxWeight) ? "" : list.PalletList[i].BoxWeight;
                            temp_page.Range["H11"].Text = string.IsNullOrEmpty(list.PalletList[i].NetWeightUnit) ? "" : list.PalletList[i].NetWeightUnit;
                            temp_page.Range["I11"].Text = string.IsNullOrEmpty(list.PalletList[i].VolumetricWeight) ? "" : $"{list.PalletList[i].VolumetricWeight}";
                            //snIndex = pp * NextPageSNRows + FirstPageSNRows;
                            snIndex = 0;
                            rowIndx = 0;

                            while (true)
                            {
                                string sn1 = "", sn2 = "";
                                try
                                {
                                    sn1 = list.PalletList[i].SNList[snIndex];
                                }
                                catch (Exception)
                                {
                                    sn1 = "";
                                }
                                try
                                {
                                    snIndex++;
                                    sn2 = list.PalletList[i].SNList[snIndex];
                                }
                                catch (Exception)
                                {
                                    sn2 = "";
                                }
                                temp_page.Range["B" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn1) ? "" : sn1;
                                temp_page.Range["D" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn1) ? "" : $"*{sn1}*";
                                temp_page.Range["G" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn2) ? "" : sn2;
                                temp_page.Range["H" + (15 + rowIndx).ToString()].Text = string.IsNullOrEmpty(sn2) ? "" : $"*{sn2}*";
                                snIndex++;
                                rowIndx++;
                                if (snIndex >= (pp + 1) * NextPageSNRows + FirstPageSNRows)
                                {
                                    break;
                                }
                            }
                            temp_page.CopyRow(temp_page.PrintRange, plWorkbook.Worksheets[0], 68 * (currentPage - 1) + 1, Spire.Xls.CopyRangeOptions.All);
                            printAreaIndex += string.IsNullOrEmpty(temp_page.PageSetup.PrintArea) ? 68 : GetPrintAreaIndex(temp_page.PageSetup.PrintArea.Split(':')[1]);
                        }
                    }
                }
                
                plWorkbook.Worksheets[0].PageSetup.PrintArea = "A1:K" + printAreaIndex.ToString();
                plWorkbook.Worksheets[0].Name = "Packlist";               
            }
            return plWorkbook;
        }

        public int GetPrintAreaIndex(string printArea)
        {

            Regex r = new Regex(@"\d+");
            MatchCollection m = r.Matches(printArea);
            string temp = "";
            for (int n = 0; n < m.Count; n++)
            {
                temp += m[n].Value;
            }
            return Convert.ToInt32(temp);
        }
    }
    
}