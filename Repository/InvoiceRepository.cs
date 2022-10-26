using CaseStudy.Dtos;
using CaseStudy.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Scaffolding.Metadata;
using System.Net.Mail;
using System.Net;
using System.Web.Http.ModelBinding;
using System.Security.Cryptography;

namespace CaseStudy.Repository
{
    public class InvoiceRepository
    {
        DatabaseContext _context;
        public InvoiceRepository(DatabaseContext cotnext)
        {
            _context = cotnext;
        }

        public async Task<ActionResult<Invoice>> CreateInvoice(InvoiceDto data)
        {
            var farm = _context.Users.SingleOrDefault(a => a.UserId == data.FarmerId);
            var deal = _context.Users.SingleOrDefault(a => a.UserId == data.DealerId);

            var crop = _context.CropDetails.Include("CropType")
                .SingleOrDefault(a => a.CropId == data.CropId);
            
            var invoice = new Invoice();
            invoice.Amount = crop.ExpectedPrice;
            invoice.DealerId = data.DealerId;
            invoice.FarmerId = data.FarmerId;
            invoice.CropId = data.CropId;
            invoice.InvoiceDate = DateTime.Now;

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            

            if(farm==null || deal==null)
            {
                return null;
            }
            
            //Sending to Farmer Receipt
            SendMailFarmer(invoice, farm.Email, crop);
            //Sending to Dealer Invoice
            SendMailDealer(invoice, deal.Email, crop);

            return invoice;
        }

        public async Task<IEnumerable<FarmerReceipt>> FarmerInvoices(int fid)
        {
            var invoices = await _context.Invoices.Where(a => a.FarmerId == fid)
                .OrderBy(p=>p.InvoiceDate)
                //.Select(p => new FarmerReceipt()
                //{
                //    InvoiceDate = DateTime.Now,
                //    InvoiceId = p.InvoiceId,
                //    CropName= _context.CropDetails.SingleOrDefault(a => a.CropId == p.CropId).CropName,
                //    CropType = _context.CropDetails.Include("CropType").SingleOrDefault(a => a.CropId == p.CropId).CropType.TypeName,
                //    FarmerAccNumber= _context.Accounts.SingleOrDefault(a => a.UserId == fid).AccountNumber,
                //    DealerAccNumber = _context.Accounts.SingleOrDefault(a => a.UserId == p.DealerId).AccountNumber,
                //    Quantity = _context.CropDetails.SingleOrDefault(a => a.CropId == p.CropId).QtyAvailable,
                //    Price = _context.CropDetails.SingleOrDefault(a => a.CropId == p.CropId).ExpectedPrice
                //})
                .ToListAsync();
            var receipts = new List<FarmerReceipt>();
            foreach(Invoice p in invoices)
            {
                var crop = _context.CropDetails.Include("CropType").SingleOrDefault(a => a.CropId == p.CropId);
                var farmer = _context.Accounts.SingleOrDefault(a => a.UserId == fid);
                var deal = _context.Accounts.SingleOrDefault(a => a.UserId == p.DealerId);

                var receipt = new FarmerReceipt()
                {
                    InvoiceDate = p.InvoiceDate,
                    InvoiceId = p.InvoiceId,
                    CropName = crop.CropName,
                    CropType = crop.CropType.TypeName,
                    DealerAccNumber = deal.AccountNumber,
                    FarmerAccNumber = farmer.AccountNumber,
                    Price = crop.ExpectedPrice,
                    Quantity = crop.QtyAvailable
                };
                receipts.Add(receipt);
            }

            if (invoices.Count < 0)
            {
                return null;
            }
            return receipts;
        }
        public async Task<IEnumerable<FarmerReceipt>> DealerInvoices(int did)
        {
            var invoices = await _context.Invoices.Where(a => a.DealerId == did)
                .Select(p => new FarmerReceipt()
                {
                    InvoiceDate = p.InvoiceDate,
                    InvoiceId = p.InvoiceId,
                    CropName = _context.CropDetails.SingleOrDefault(a => a.CropId == p.CropId).CropName,
                    CropType = _context.CropDetails.Include("CropType").SingleOrDefault(a => a.CropId == p.CropId).CropType.TypeName,
                    FarmerAccNumber = _context.Accounts.SingleOrDefault(a => a.UserId == p.FarmerId).AccountNumber,
                    DealerAccNumber = _context.Accounts.SingleOrDefault(a => a.UserId == did).AccountNumber,
                    Quantity = _context.CropDetails.SingleOrDefault(a => a.CropId == p.CropId).QtyAvailable,
                    Price = _context.CropDetails.SingleOrDefault(a => a.CropId == p.CropId).ExpectedPrice
                })
                .ToListAsync();

            if (invoices.Count < 0)
            {
                return null;
            }
            return invoices;
        }
        public async Task<IEnumerable<FarmerReceipt>> GetInvoices()
        {
            var invoices =await  _context.Invoices
                .OrderBy(p=>p.InvoiceDate)
                .ToListAsync();
            var receipts = new List<FarmerReceipt>();
            foreach (Invoice p in invoices)
            {
                var crop = _context.CropDetails.Include("CropType").SingleOrDefault(a => a.CropId == p.CropId);
                var farmer = _context.Accounts.SingleOrDefault(a => a.UserId == p.FarmerId);
                var deal = _context.Accounts.SingleOrDefault(a => a.UserId == p.DealerId);

                var receipt = new FarmerReceipt()
                {
                    InvoiceDate = DateTime.Now,
                    InvoiceId = p.InvoiceId,
                    CropName = crop.CropName,
                    CropType = crop.CropType.TypeName,
                    DealerAccNumber = deal.AccountNumber,
                    FarmerAccNumber = farmer.AccountNumber,
                    Price = crop.ExpectedPrice,
                    Quantity = crop.QtyAvailable
                };
                receipts.Add(receipt);
            }

            if (invoices.Count < 0)
            {
                return null;
            }
            return receipts;
        }
        private void SendMailFarmer(Invoice invoice, string To, CropDetail crop)
        {
            using (MailMessage message = new MailMessage("dsh1123583@gmail.com", "dsh145002@gmail.com"))
            {
                message.Body = "Successfull Transaction" +
                    $"Crop Name: {crop.CropName}\n" +
                    $"Crop Type: {crop.CropType.TypeName}\n" +
                    $"Crop Qty: {crop.QtyAvailable}\n" +
                    $"Your Account Number: {crop.CropName}\n" +
                    $"Dealer Account Number: {crop.CropName}\n" +
                    $"Amount: {invoice.Amount}\n" +
                    $"Invoice Id: {invoice.InvoiceId}\n"
                    ;

                message.Subject = "Here Is Your Receipt";
                message.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential("dsh1123583@gmail.com", "zqvbjjlrrukhjumc");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(message);
                }
            }

        }
        private void SendMailDealer(Invoice invoice, string To, CropDetail crop)
        {
            using (MailMessage message = new MailMessage("dsh1123583@gmail.com", "dsh145002@gmail.com"))
            {
                message.Body = "Successfull Transaction" +
                    $"Crop Name: {crop.CropName}\n" +
                    $"Crop Type: {crop.CropType.TypeName}\n" +
                    $"Crop Qty: {crop.QtyAvailable}\n" +
                    $"Farmer Account Number: {crop.CropName}\n" +
                    $"Your Account Number: {crop.CropName}\n" +
                    $"Amount: {invoice.Amount}\n" +
                    $"Invoice Id: {invoice.InvoiceId}\n"
                    ;

                message.Subject = "Here Is Your Receipt";
                message.IsBodyHtml = false;

                using (SmtpClient smtp = new SmtpClient())
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.EnableSsl = true;
                    NetworkCredential cred = new NetworkCredential("dsh1123583@gmail.com", "zqvbjjlrrukhjumc");
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = cred;
                    smtp.Port = 587;
                    smtp.Send(message);
                }
            }

        }
        
    }
}
