using CaseStudy.Dtos;
using CaseStudy.Models;
using CaseStudy.Repository;
using Microsoft.AspNetCore.Mvc;

namespace CaseStudy.Services
{
    public class InvoiceService
    {
        InvoiceRepository _repo;
        public InvoiceService(InvoiceRepository repository)
        {
            _repo = repository;
        }
        public async Task<ActionResult<Invoice>> CreateInvoice(InvoiceDto data)
        {
            return await _repo.CreateInvoice(data);
        }

        public async Task<IEnumerable<FarmerReceipt>> FarmerInvoices(int fid)
        {
            return await _repo.FarmerInvoices(fid);
        }
        public async Task<IEnumerable<Invoice>> DealerInvoices(int did)
        {
            return await _repo.DealerInvoices(did);
        }
    }
}
