using System;
using System.Collections.Generic;
using System.Text;
using RocketPOS.Core.Constants;
using System.Xml;
using RocketPOS.Model;
using RocketPOS.ViewModels;
using System.Xml.Serialization;
using System.IO;

namespace RocketPOS.Views
{
    public class TallyXMLView
    {
        GeneralClass clsGeneral = new GeneralClass();

        public void GenerateSalesVoucher(string fromDate, string toDate, string path)
        {
            List<TallySetupModel> tallySetupModel = new List<TallySetupModel>();
            List<TallySalesVoucherModel> tallySalesVoucherModels = new List<TallySalesVoucherModel>();
            TallyViewModel tallyViewModel = new TallyViewModel();
            XmlDocument xmldoc = new XmlDocument();
            List<XmlDocument> xmldocVoucher = new List<XmlDocument>();

            tallySetupModel = tallyViewModel.GetTallySetup();
            tallySalesVoucherModels = tallyViewModel.GetSalesVoucherData(fromDate, toDate);
            int i = 1;

            foreach (var salesVoucher in tallySalesVoucherModels)
            {
                var clsSalesFields = new SalesFields();
                clsSalesFields.VoucherType = "Sales";
                clsSalesFields.VoucherUniqueID = salesVoucher.BillDate.Replace("/", "");
                clsSalesFields.VoucherNumber = tallySetupModel.Find(x => x.Keyname.Contains("BillPrefix")).LedgerName + salesVoucher.BillDate.Replace("/", "");
                clsSalesFields.VoucherDate = Convert.ToDateTime(salesVoucher.BillDate);
                clsSalesFields.PartyLedgerName = tallySetupModel.Find(x => x.Keyname.Contains("Cash")).LedgerName;
                clsSalesFields.EffectiveDate = Convert.ToDateTime(salesVoucher.BillDate);
                clsSalesFields.IsInvoice = "No";
                clsSalesFields.VoucherNarration = "";

                var pcledgerParty = new ALLLedgerEntries();
                pcledgerParty.LedgerName = tallySetupModel.Find(x => x.Keyname.Contains("Cash")).LedgerName;
                pcledgerParty.IsDeemedPositive = "Yes";
                pcledgerParty.LedgerFromItem = "No";
                pcledgerParty.RemoveZeroEntries = "No";
                pcledgerParty.IsPartyLedger = "Yes";
                pcledgerParty.Amount = Convert.ToDouble(salesVoucher.Cash) * -1;
                clsSalesFields.SalesAllLedgerEntriesList.Add(tallySetupModel.Find(x => x.Keyname.Contains("Cash")).LedgerName, pcledgerParty);

                pcledgerParty = new ALLLedgerEntries();
                pcledgerParty.LedgerName = tallySetupModel.Find(x => x.Keyname.Contains("CashSales")).LedgerName;
                pcledgerParty.IsDeemedPositive = "No";
                pcledgerParty.LedgerFromItem = "No";
                pcledgerParty.RemoveZeroEntries = "No";
                pcledgerParty.IsPartyLedger = "No";
                pcledgerParty.Amount = Convert.ToDouble(salesVoucher.CashSales);
                clsSalesFields.SalesAllLedgerEntriesList.Add(tallySetupModel.Find(x => x.Keyname.Contains("CashSales")).LedgerName, pcledgerParty);
                
                pcledgerParty = new ALLLedgerEntries();
                pcledgerParty.LedgerName = tallySetupModel.Find(x => x.Keyname.Contains("ExemptedSales")).LedgerName;
                pcledgerParty.IsDeemedPositive = "No";
                pcledgerParty.LedgerFromItem = "No";
                pcledgerParty.RemoveZeroEntries = "No";
                pcledgerParty.IsPartyLedger = "No";
                pcledgerParty.Amount = Convert.ToDouble(salesVoucher.ExemptedSales);
                clsSalesFields.SalesAllLedgerEntriesList.Add(tallySetupModel.Find(x => x.Keyname.Contains("ExemptedSales")).LedgerName, pcledgerParty);

                pcledgerParty = new ALLLedgerEntries();
                pcledgerParty.LedgerName = tallySetupModel.Find(x => x.Keyname.Contains("OutputVAT ")).LedgerName;
                pcledgerParty.IsDeemedPositive = "No";
                pcledgerParty.LedgerFromItem = "No";
                pcledgerParty.RemoveZeroEntries = "No";
                pcledgerParty.IsPartyLedger = "No";
                pcledgerParty.Amount = Convert.ToDouble(salesVoucher.OutputVAT);
                clsSalesFields.SalesAllLedgerEntriesList.Add(tallySetupModel.Find(x => x.Keyname.Contains("OutputVAT")).LedgerName, pcledgerParty);
 
                //var pcBillalloc = new BillAllocation();
                //pcBillalloc.Name = "AccountID";
                //pcBillalloc.BillType = "New Ref";
                //pcBillalloc.Amount = Val("Total") * -1;
                //pcledgerParty.BillAllocationList.Add("PartyLedger", pcBillalloc);
                //clsSalesFields.SalesAllLedgerEntriesList.Add("PartyLedger", pcledgerParty);

                var SalesLedgerCount = new List<string>(new string[] {

                tallySetupModel.Find(x => x.Keyname.Contains("Cash")).LedgerName,
                tallySetupModel.Find(x => x.Keyname.Contains("CashSales")).LedgerName,
                tallySetupModel.Find(x => x.Keyname.Contains("ExemptedSales")).LedgerName,
                tallySetupModel.Find(x => x.Keyname.Contains("OutputVAT ")).LedgerName
                 });

                var clsSalesVoucher = new SaleVoucher();
                xmldocVoucher.Add(clsSalesVoucher.CreateSaleVoucherXML(clsSalesFields, SalesLedgerCount, tallySetupModel.Find(x => x.Keyname.Contains("CompanyName")).LedgerName));

                i += 1;
            }
            SerializeToXml(xmldocVoucher, path);
        }

        public static void SerializeToXml<T>(T anyobject, string xmlFilePath)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(anyobject.GetType());

            using (StreamWriter writer = new StreamWriter(xmlFilePath))
            {
                xmlSerializer.Serialize(writer, anyobject);
            }
        }
    }
}
