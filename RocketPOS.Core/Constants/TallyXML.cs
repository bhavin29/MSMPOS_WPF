using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualBasic; // Install-Package Microsoft.VisualBasic
using System.Net;

namespace RocketPOS.Core.Constants
{

    public class TallyXML
    {
        GeneralClass clsGeneral = new GeneralClass();
   }

    public class SaleVoucher
    {
        GeneralClass clsGeneral = new GeneralClass();
        public XmlDocument CreateSaleVoucherXML(SalesFields SalesFieldData, List<string> SalesLedgerCount)
        {
            XmlDocument CreateSaleVoucherXMLRet = default;
            XmlAttribute xmlAttr;
            var xmldoc = new XmlDocument();
            XmlElement xmlEnvelop;
            XmlElement xmlBody;
            XmlNode ndTallyMsg;
            xmlEnvelop = xmldoc.CreateElement("ENVELOPE");
            var xmlHeader = xmldoc.CreateElement("HEADER");
            var xmlTallyRequest = xmldoc.CreateElement("TALLYREQUEST");
            xmlTallyRequest.InnerText = "Import Data";
            xmlHeader.AppendChild(xmlTallyRequest);
            xmlEnvelop.AppendChild(xmlHeader);
            xmlBody = xmldoc.CreateElement("BODY");
            var xmlImportData = xmldoc.CreateElement("IMPORTDATA");
            xmlBody.AppendChild(xmlImportData);
            var xmlRequestDesc = xmldoc.CreateElement("REQUESTDESC");
            xmlImportData.AppendChild(xmlRequestDesc);
            var xmlReportName = xmldoc.CreateElement("REPORTNAME");
            xmlReportName.InnerText = "Vouchers";
            xmlRequestDesc.AppendChild(xmlReportName);
            var xmlStaticVariables = xmldoc.CreateElement("STATICVARIABLES");
            xmlRequestDesc.AppendChild(xmlStaticVariables);
            var xmlSvExportFormat = xmldoc.CreateElement("SVEXPORTFORMAT");
            xmlSvExportFormat.InnerText = "$$Sysname:XML";
            xmlStaticVariables.AppendChild(xmlSvExportFormat);
            var xmlRequestData = xmldoc.CreateElement("REQUESTDATA");
            xmlImportData.AppendChild(xmlRequestData);
            ndTallyMsg = xmldoc.CreateElement("TALLYMESSAGE");
            var tlyMsgAttr = xmldoc.CreateAttribute("xmlns:UDF");
            tlyMsgAttr.Value = "TallyUDF";
            ndTallyMsg.Attributes.Append(tlyMsgAttr);
            XmlElement xmlSaleBody;
            xmlSaleBody = xmldoc.CreateElement("VOUCHER");
            xmlAttr = xmldoc.CreateAttribute("VCHTYPE");
            xmlAttr.Value = SalesFieldData.VoucherType;
            if (SalesFieldData.VoucherUniqueID == "")
            {
                xmlSaleBody.Attributes.Append(xmlAttr);
                xmlAttr = xmldoc.CreateAttribute("ACTION");
                xmlAttr.Value = "Create";
                xmlSaleBody.Attributes.Append(xmlAttr);
            }
            else
            {
                xmlAttr = xmldoc.CreateAttribute("DATE");
                xmlAttr.Value = clsGeneral.ConvertDateToTallyFormat(SalesFieldData.VoucherDate);
                xmlSaleBody.Attributes.Append(xmlAttr);
                xmlAttr = xmldoc.CreateAttribute("TAGNAME");
                xmlAttr.Value = "MASTER ID";
                xmlSaleBody.Attributes.Append(xmlAttr);
                xmlAttr = xmldoc.CreateAttribute("TAGVALUE");
                xmlAttr.Value = SalesFieldData.VoucherUniqueID;
                xmlSaleBody.Attributes.Append(xmlAttr);
                xmlAttr = xmldoc.CreateAttribute("ACTION");
                xmlAttr.Value = "ALTER"; // if want to delete xmlAttr.Value = "DELETE"
                xmlSaleBody.Attributes.Append(xmlAttr);
            }

            xmlAttr = xmldoc.CreateAttribute("OBJVIEW");
            xmlAttr.Value = "Invoice Voucher View";
            xmlSaleBody.Attributes.Append(xmlAttr);
            var xmlVouchDate = xmldoc.CreateElement("DATE");
            xmlVouchDate.InnerText = clsGeneral.ConvertDateToTallyFormat(SalesFieldData.VoucherDate);
            xmlSaleBody.AppendChild(xmlVouchDate);
            var xmlNarration = xmldoc.CreateElement("NARRATION");
            xmlNarration.InnerText = SalesFieldData.VoucherNarration;
            xmlSaleBody.AppendChild(xmlNarration);
            var xmlTypeName = xmldoc.CreateElement("VOUCHERTYPENAME");
            xmlTypeName.InnerText = SalesFieldData.VoucherType;
            xmlSaleBody.AppendChild(xmlTypeName);
            var xmlVoucherNumber = xmldoc.CreateElement("VOUCHERNUMBER");
            xmlVoucherNumber.InnerText = SalesFieldData.VoucherNumber;
            xmlSaleBody.AppendChild(xmlVoucherNumber);
            var xmlPartyLedgerName = xmldoc.CreateElement("PARTYLEDGERNAME");
            xmlPartyLedgerName.InnerText = SalesFieldData.PartyLedgerName;
            xmlSaleBody.AppendChild(xmlPartyLedgerName);
            var xmlPersistedView = xmldoc.CreateElement("PERSISTEDVIEW");
            xmlPersistedView.InnerText = "Invoice Voucher View";
            xmlSaleBody.AppendChild(xmlPersistedView);
            var xmlEffectiveDate = xmldoc.CreateElement("EFFECTIVEDATE");
            xmlEffectiveDate.InnerText = clsGeneral.ConvertDateToTallyFormat(SalesFieldData.EffectiveDate);
            xmlSaleBody.AppendChild(xmlEffectiveDate);
            var xmlIsInvoice = xmldoc.CreateElement("ISINVOICE");
            xmlIsInvoice.InnerText = SalesFieldData.IsInvoice;
            xmlSaleBody.AppendChild(xmlIsInvoice);
            foreach (string LedgerName in SalesLedgerCount)
            {
                var xmlLedgerList = xmldoc.CreateElement("LEDGERENTRIES.LIST");
                xmlSaleBody.AppendChild(xmlLedgerList);
                var xmlLedgerName = xmldoc.CreateElement("LEDGERNAME");
                xmlLedgerName.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].LedgerName;
                //xmlLedgerName.InnerText = SalesFieldData.SalesAllLedgerEntriesList(LedgerName).LedgerName;
                xmlLedgerList.AppendChild(xmlLedgerName);
                var xmlIsDeemedPositive = xmldoc.CreateElement("ISDEEMEDPOSITIVE");
                xmlIsDeemedPositive.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].IsDeemedPositive;
                xmlLedgerList.AppendChild(xmlIsDeemedPositive);
                var xmlLedgerFromItem = xmldoc.CreateElement("LEDGERFROMITEM");
                xmlLedgerFromItem.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].LedgerFromItem;
                xmlLedgerList.AppendChild(xmlLedgerFromItem);
                var xmlRemoveZeroEntries = xmldoc.CreateElement("REMOVEZEROENTRIES");
                xmlRemoveZeroEntries.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].RemoveZeroEntries;
                xmlLedgerList.AppendChild(xmlRemoveZeroEntries);
                var xmlIsPartyLedger = xmldoc.CreateElement("ISPARTYLEDGER");
                xmlIsPartyLedger.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].IsPartyLedger;
                xmlLedgerList.AppendChild(xmlIsPartyLedger);
                var xmlLedgerAmount = xmldoc.CreateElement("AMOUNT");
                xmlLedgerAmount.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].Amount.ToString();
                xmlLedgerList.AppendChild(xmlLedgerAmount);
                if (SalesFieldData.SalesAllLedgerEntriesList[LedgerName].BillAllocationList.Count > 0)
                {
                    var xmlBillAllocationList = xmldoc.CreateElement("BILLALLOCATIONS.LIST");
                    xmlLedgerList.AppendChild(xmlBillAllocationList);
                    var xmlName = xmldoc.CreateElement("NAME");
                    xmlName.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].BillAllocationList["LedgerName"].Name;
                    xmlBillAllocationList.AppendChild(xmlName);
                    var xmlBillType = xmldoc.CreateElement("BILLTYPE");
                    xmlBillType.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].BillAllocationList["LedgerName"].BillType;
                    xmlBillAllocationList.AppendChild(xmlBillType);
                    var xmlAmount = xmldoc.CreateElement("AMOUNT");
                    xmlAmount.InnerText = SalesFieldData.SalesAllLedgerEntriesList[LedgerName].BillAllocationList["LedgerName"].Amount.ToString();
                    xmlBillAllocationList.AppendChild(xmlAmount);
                }
            }

            ndTallyMsg.AppendChild(xmlSaleBody); // to add data
            xmlRequestData.AppendChild(ndTallyMsg);
            xmldoc.AppendChild(xmlEnvelop);
            xmlEnvelop.AppendChild(xmlBody);
            xmldoc.AppendChild(xmlEnvelop);
            CreateSaleVoucherXMLRet = xmldoc;
            return CreateSaleVoucherXMLRet;
        }
    }

    public class SalesFields
    {
        public Dictionary<string, ALLLedgerEntries> SalesAllLedgerEntriesList = new Dictionary<string, ALLLedgerEntries>();
        public Dictionary<string, InventoryEntriesList> AllInventoryEntriesList = new Dictionary<string, InventoryEntriesList>();
        private string pcVoucherType;

        public string VoucherType
        {
            get
            {
                return pcVoucherType;
            }

            set
            {
                pcVoucherType = value;
            }
        }

        private DateTime pcDate;

        public DateTime VoucherDate
        {
            get
            {
                return pcDate;
            }

            set
            {
                pcDate = value;
            }
        }

        private string pcNarration;

        public string VoucherNarration
        {
            get
            {
                return pcNarration;
            }

            set
            {
                pcNarration = value;
            }
        }

        private string pcVoucherNumber;

        public string VoucherNumber
        {
            get
            {
                return pcVoucherNumber;
            }

            set
            {
                pcVoucherNumber = value;
            }
        }

        private string pcVoucherUniqueID;

        public string VoucherUniqueID
        {
            get
            {
                return pcVoucherUniqueID;
            }

            set
            {
                pcVoucherUniqueID = value;
            }
        }

        private string pcPartyLedgerName;

        public string PartyLedgerName
        {
            get
            {
                return pcPartyLedgerName;
            }

            set
            {
                pcPartyLedgerName = value;
            }
        }

        private string pcPartyName;

        public string PartyName
        {
            get
            {
                return pcPartyName;
            }

            set
            {
                pcPartyName = value;
            }
        }

        private string pcBasicBasePartyName;

        public string BasicBasePartyName
        {
            get
            {
                return pcBasicBasePartyName;
            }

            set
            {
                pcBasicBasePartyName = value;
            }
        }

        private string pcBasicBuyerName;

        public string BasicBuyerName
        {
            get
            {
                return pcBasicBuyerName;
            }

            set
            {
                pcBasicBuyerName = value;
            }
        }

        private DateTime pcEffectiveDate;

        public DateTime EffectiveDate
        {
            get
            {
                return pcEffectiveDate;
            }

            set
            {
                pcEffectiveDate = value;
            }
        }

        private string pcIsInvoice;

        public string IsInvoice
        {
            get
            {
                return pcIsInvoice;
            }

            set
            {
                pcIsInvoice = value;
            }
        }
    }

    public class ALLLedgerEntries
    {
        public Dictionary<string, BillAllocation> BillAllocationList = new Dictionary<string, BillAllocation>();
        public Dictionary<string, TaxObjectAllocations> TaxObjectAllocationsList = new Dictionary<string, TaxObjectAllocations>();
        public Dictionary<string, BankAllLocations> BankAllLocationsList = new Dictionary<string, BankAllLocations>();
        private string pcLedgerName;

        public string LedgerName
        {
            get
            {
                return pcLedgerName;
            }

            set
            {
                pcLedgerName = value;
            }
        }

        private string pcIsDeemedPositive;

        public string IsDeemedPositive
        {
            get
            {
                return pcIsDeemedPositive;
            }

            set
            {
                pcIsDeemedPositive = value;
            }
        }

        private string pcLedgerFromItem;

        public string LedgerFromItem
        {
            get
            {
                return pcLedgerFromItem;
            }

            set
            {
                pcLedgerFromItem = value;
            }
        }

        private string pcRemoveZeroEntries;

        public string RemoveZeroEntries
        {
            get
            {
                return pcRemoveZeroEntries;
            }

            set
            {
                pcRemoveZeroEntries = value;
            }
        }

        private string pcIsPartyLedger;

        public string IsPartyLedger
        {
            get
            {
                return pcIsPartyLedger;
            }

            set
            {
                pcIsPartyLedger = value;
            }
        }

        private double pcAmount;

        public double Amount
        {
            get
            {
                return pcAmount;
            }

            set
            {
                pcAmount = value;
            }
        }
    }

    public class BankAllLocations
    {
        private DateTime pcBankDate;

        public DateTime BankDate
        {
            get
            {
                return pcBankDate;
            }

            set
            {
                pcBankDate = value;
            }
        }

        private DateTime pcInstrumentDate;

        public DateTime InstrumentDate
        {
            get
            {
                return pcInstrumentDate;
            }

            set
            {
                pcInstrumentDate = value;
            }
        }

        private string pcBankBranchName;

        public string BankBranchName
        {
            get
            {
                return pcBankBranchName;
            }

            set
            {
                pcBankBranchName = value;
            }
        }

        private string pcTransactionType;

        public string TransactionType
        {
            get
            {
                return pcTransactionType;
            }

            set
            {
                pcTransactionType = value;
            }
        }

        private string pcBankName;

        public string BankName
        {
            get
            {
                return pcBankName;
            }

            set
            {
                pcBankName = value;
            }
        }

        private string pcPaymentFavouring;

        public string PaymentFavouring
        {
            get
            {
                return pcPaymentFavouring;
            }

            set
            {
                pcPaymentFavouring = value;
            }
        }

        private string pcStatus;

        public string Status
        {
            get
            {
                return pcStatus;
            }

            set
            {
                pcStatus = value;
            }
        }

        private string pcInstrumentNumber;

        public string InstrumentNumber
        {
            get
            {
                return pcInstrumentNumber;
            }

            set
            {
                pcInstrumentNumber = value;
            }
        }

        private double pcBankAmount;

        public double BankAmount
        {
            get
            {
                return pcBankAmount;
            }

            set
            {
                pcBankAmount = value;
            }
        }
    }

    public class BillAllocation
    {
        private string pcName;

        public string Name
        {
            get
            {
                return pcName;
            }

            set
            {
                pcName = value;
            }
        }

        private string pcBillType;

        public string BillType
        {
            get
            {
                return pcBillType;
            }

            set
            {
                pcBillType = value;
            }
        }

        private double pcAmount;

        public double Amount
        {
            get
            {
                return pcAmount;
            }

            set
            {
                pcAmount = value;
            }
        }
    }

    public class TaxObjectAllocations
    {
        public Dictionary<string, SubCategoryAllocation> SubCategoryAllocationList = new Dictionary<string, SubCategoryAllocation>();
        private double pcVATAssessableValue;

        public double VATAssessableValue
        {
            get
            {
                return pcVATAssessableValue;
            }

            set
            {
                pcVATAssessableValue = value;
            }
        }

        private string pcCategory;

        public string Category
        {
            get
            {
                return pcCategory;
            }

            set
            {
                pcCategory = value;
            }
        }

        private string pcTaxType;

        public string TaxType
        {
            get
            {
                return pcTaxType;
            }

            set
            {
                pcTaxType = value;
            }
        }

        private string pcTaxName;

        public string TaxName
        {
            get
            {
                return pcTaxName;
            }

            set
            {
                pcTaxName = value;
            }
        }

        private string pcPartyLedger;

        public string PartyLedger
        {
            get
            {
                return pcPartyLedger;
            }

            set
            {
                pcPartyLedger = value;
            }
        }

        private string pcRefType;

        public string RefType
        {
            get
            {
                return pcRefType;
            }

            set
            {
                pcRefType = value;
            }
        }
    }

    public class SubCategoryAllocation
    {
        private string pcStockItemName;

        public string StockItemName
        {
            get
            {
                return pcStockItemName;
            }

            set
            {
                pcStockItemName = value;
            }
        }

        private string pcDutyLedger;

        public string DutyLedger
        {
            get
            {
                return pcDutyLedger;
            }

            set
            {
                pcDutyLedger = value;
            }
        }

        private double pcTaxRate;

        public double TaxRate
        {
            get
            {
                return pcTaxRate;
            }

            set
            {
                pcTaxRate = value;
            }
        }

        private double pcAssessableAmount;

        public double AssessableAmount
        {
            get
            {
                return pcAssessableAmount;
            }

            set
            {
                pcAssessableAmount = value;
            }
        }

        private double pcTax;

        public double Tax
        {
            get
            {
                return pcTax;
            }

            set
            {
                pcTax = value;
            }
        }

        private double pcBilledQty;

        public double BilledQty
        {
            get
            {
                return pcBilledQty;
            }

            set
            {
                pcBilledQty = value;
            }
        }



        public partial class InventoryEntriesList
        {
            public Dictionary<string, AccountingAllocations> AccountingAllocationsList = new Dictionary<string, AccountingAllocations>();
            private string pcStockItemName;

            public string StockItemName
            {
                get
                {
                    return pcStockItemName;
                }

                set
                {
                    pcStockItemName = value;
                }
            }

            private string pcIsDeemedPositive;

            public string IsDeemedPositive
            {
                get
                {
                    return pcIsDeemedPositive;
                }

                set
                {
                    pcIsDeemedPositive = value;
                }
            }

            private string pcIsLastDeemedPositive;

            public string IsLastDeemedPositive
            {
                get
                {
                    return pcIsLastDeemedPositive;
                }

                set
                {
                    pcIsLastDeemedPositive = value;
                }
            }

            private double pcRate;

            public double Rate
            {
                get
                {
                    return pcRate;
                }

                set
                {
                    pcRate = value;
                }
            }

            private double pcAmount;

            public double Amount
            {
                get
                {
                    return pcAmount;
                }

                set
                {
                    pcAmount = value;
                }
            }

            private string pcActualQty;

            public string ActualQty
            {
                get
                {
                    return pcActualQty;
                }

                set
                {
                    pcActualQty = value;
                }
            }

            private string pcBilledQty;

            public string BilledQty
            {
                get
                {
                    return pcBilledQty;
                }

                set
                {
                    pcBilledQty = value;
                }
            }
        }
    }

    public class AccountingAllocations
    {
        private string pcLedgerName;

        public string LedgerName
        {
            get
            {
                return pcLedgerName;
            }

            set
            {
                pcLedgerName = value;
            }
        }

        private string pcIsDeemedPositive;

        public string IsDeemedPositive
        {
            get
            {
                return pcIsDeemedPositive;
            }

            set
            {
                pcIsDeemedPositive = value;
            }
        }

        private string pcIsPartyLedger;

        public string IsPartyLedger
        {
            get
            {
                return pcIsPartyLedger;
            }

            set
            {
                pcIsPartyLedger = value;
            }
        }

        private string pcIsLastDeemedPositive;

        public string IsLastDeemedPositive
        {
            get
            {
                return pcIsLastDeemedPositive;
            }

            set
            {
                pcIsLastDeemedPositive = value;
            }
        }

        private double pcAmount;

        public double Amount
        {
            get
            {
                return pcAmount;
            }

            set
            {
                pcAmount = value;
            }
        }
    }

    public class InventoryEntriesList
    {
        public Dictionary<string, AccountingAllocations> AccountingAllocationsList = new Dictionary<string, AccountingAllocations>();
        private string pcStockItemName;

        public string StockItemName
        {
            get
            {
                return pcStockItemName;
            }

            set
            {
                pcStockItemName = value;
            }
        }

        private string pcIsDeemedPositive;

        public string IsDeemedPositive
        {
            get
            {
                return pcIsDeemedPositive;
            }

            set
            {
                pcIsDeemedPositive = value;
            }
        }

        private string pcIsLastDeemedPositive;

        public string IsLastDeemedPositive
        {
            get
            {
                return pcIsLastDeemedPositive;
            }

            set
            {
                pcIsLastDeemedPositive = value;
            }
        }

        private double pcRate;

        public double Rate
        {
            get
            {
                return pcRate;
            }

            set
            {
                pcRate = value;
            }
        }

        private double pcAmount;

        public double Amount
        {
            get
            {
                return pcAmount;
            }

            set
            {
                pcAmount = value;
            }
        }

        private string pcActualQty;

        public string ActualQty
        {
            get
            {
                return pcActualQty;
            }

            set
            {
                pcActualQty = value;
            }
        }

        private string pcBilledQty;

        public string BilledQty
        {
            get
            {
                return pcBilledQty;
            }

            set
            {
                pcBilledQty = value;
            }
        }
    }

    public class GeneralClass
    {
        //public XmlDocument TransferDataToTally(XmlDocument xmlData)
        //{
        //    XmlDocument TransferDataToTallyRet = default;
        //      var ServerHTTP = new Microsoft.VisualBasic MSXML2.ServerXMLHTTP30();
        //    XML
        //    //  ServerHTTP.open("POST", clsConfig.TallyServer + clsConfig.TallyPort);
        //    try
        //    {
        //        // ServerHTTP.send(xmlData.InnerXml);
        //    }
        //    catch (Exception ex)
        //    {
        //       // MessageBox.Show(ex.Message + '\r' + "An error occured while connecting Tally. - Tally Connection Error");
        //        Environment.Exit(0);
        //    }

        //    //   string ResponseStr = ServerHTTP.responseText;
        //    var retunXmlDoc = new XmlDocument();
        //    // retunXmlDoc.LoadXml(ResponseStr);
        //    TransferDataToTallyRet = retunXmlDoc;
        //    return TransferDataToTallyRet;
        //}

        public string ConvertDateToTallyFormat(DateTime dtDate)
        {
            string ConvertDateToTallyFormatRet = default;
            ConvertDateToTallyFormatRet = Strings.Format(dtDate, "yyyy") + Strings.Format(dtDate, "MM") + Strings.Format(dtDate, "dd");
            return ConvertDateToTallyFormatRet;
        }

        public string ReplaceXmlText(string strXmlText)
        {
            string ReplaceXmlTextRet = default;
            string strXml;
            strXml = Strings.Replace(strXmlText, "&", "&amp;");
            strXml = Strings.Replace(strXml, "'", "&apos;");
            strXml = Strings.Replace(strXml, "\"", "&quot;");
            strXml = Strings.Replace(strXml, ">", "&gt;");
            strXml = Strings.Replace(strXml, "<", "&lt;");
            ReplaceXmlTextRet = strXml;
            return ReplaceXmlTextRet;
        }

        public string ReplaceSQLText(string strXmlText)
        {
            string ReplaceSQLTextRet = default;
            string strXml;

            // strXml = Replace(strXmlText, "&", "&amp;")
            strXml = Strings.Replace(strXmlText, "'", "''");
            // strXml = Replace(strXml, """", "&quot;")
            // strXml = Replace(strXml, ">", "&gt;")
            // strXml = Replace(strXml, "<", "&lt;")

            ReplaceSQLTextRet = strXml;
            return ReplaceSQLTextRet;
        }
    }
}
