﻿using System;
using System.Collections.Generic;
using System.Text;
using RocketPOS.Core.Constants;
using System.Xml;
using RocketPOS.Model;
using RocketPOS.ViewModels;

namespace RocketPOS.Views
{
    public class TallyXMLView
    {
        GeneralClass clsGeneral = new GeneralClass();
        public void xyz()
        {



            XmlWriter writer = XmlWriter.Create("d:\\1.xml");

            writer.WriteStartDocument();
            writer.WriteStartElement("People");

            writer.WriteStartElement("Person");
            writer.WriteAttributeString("Name", "Nick");
            writer.WriteEndElement();

            writer.WriteStartElement("Person");
            writer.WriteStartAttribute("Name");
            writer.WriteValue("Nick");
            writer.WriteEndAttribute();
            writer.WriteEndElement();

            writer.WriteEndElement();
            writer.WriteEndDocument();

            writer.Flush();
        }

        public void GenerateSalesVoucher(string fromDate, string toDate, string path)
        {
            List<TallySetupModel> tallySetupModel = new List<TallySetupModel>();
            List<TallySalesVoucherModel> tallySalesVoucherModels = new List<TallySalesVoucherModel>();
            TallyViewModel tallyViewModel = new TallyViewModel();
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.CreateElement("Root");

            tallySetupModel = tallyViewModel.GetTallySetup();
            tallySalesVoucherModels = tallyViewModel.GetSalesVoucherData(fromDate, toDate);
            int i = 1;


            foreach (var salesVoucher in tallySalesVoucherModels)
            {

                var clsSalesFields = new SalesFields();
                clsSalesFields.VoucherType = "Sales";
                clsSalesFields.VoucherUniqueID = salesVoucher.BillDate.Replace("/", "");
                clsSalesFields.VoucherNumber = "BillNo";
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

                pcledgerParty = new ALLLedgerEntries();
                pcledgerParty.LedgerName = tallySetupModel.Find(x => x.Keyname.Contains("CashSales")).LedgerName;
                pcledgerParty.IsDeemedPositive = "No";
                pcledgerParty.LedgerFromItem = "No";
                pcledgerParty.RemoveZeroEntries = "No";
                pcledgerParty.IsPartyLedger = "No";
                pcledgerParty.Amount = Convert.ToDouble(salesVoucher.CashSales);
                clsSalesFields.SalesAllLedgerEntriesList.Add(tallySetupModel.Find(x => x.Keyname.Contains("CashSales")).LedgerName, pcledgerParty);

                //var pcBillalloc = new BillAllocation();
                //pcBillalloc.Name = "AccountID";
                //pcBillalloc.BillType = "New Ref";
                //pcBillalloc.Amount = Val("Total") * -1;
                //pcledgerParty.BillAllocationList.Add("PartyLedger", pcBillalloc);
                //clsSalesFields.SalesAllLedgerEntriesList.Add("PartyLedger", pcledgerParty);


                var SalesLedgerCount = new List<string>(new string[] {

                tallySetupModel.Find(x => x.Keyname.Contains("Cash")).LedgerName,
                tallySetupModel.Find(x => x.Keyname.Contains("ExemptedSales")).LedgerName,
                tallySetupModel.Find(x => x.Keyname.Contains("OutputVAT ")).LedgerName,
                tallySetupModel.Find(x => x.Keyname.Contains("CashSales")).LedgerName
                 });

                var clsSalesVoucher = new SaleVoucher();
                xmldoc = clsSalesVoucher.CreateSaleVoucherXML(clsSalesFields, SalesLedgerCount);

                string newpath;
                newpath = path;
                newpath = newpath.Replace(".XML", i.ToString() + ".XML");

                xmldoc.Save(newpath);
                i += 1;
            }

           
        }
    }
}
