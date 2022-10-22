using CaseStudy.Dtos.CropDto;
using CaseStudy.Models;
using CaseStudy.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CaseStudy.Repository
{
    public class CropRepository : ICropRepository
    {
        DatabaseContext _context;
        public CropRepository(DatabaseContext context)
        {
            _context = context;
        }

        private enum CropId
        {
            Fruit = 1,
            Vegetable = 2,
            Grain = 3
        }

        public async Task<ActionResult<CropDetail>> AddCropAsync(AddCropDto crop)
        {
            try
            {
                CropDetail cropDetail = new CropDetail();
                cropDetail.CropName = crop.CropName;
                cropDetail.QtyAvailable = crop.CropQtyAvailable;
                cropDetail.Location = crop.CropLocation;
                cropDetail.ExpectedPrice = crop.CropExpectedPrice;
                cropDetail.FarmerId = crop.fid;

                cropDetail.CropTypeId = (int)Enum.Parse(typeof(CropId), crop.CropType);

                _context.CropDetails.Add(cropDetail);
                await _context.SaveChangesAsync();
                return cropDetail;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error while adding crop");
            }
            return null;
        }

        public async Task<IEnumerable<CropDetail>> GetAllCropAsync()
        {
            var cropList = await _context.CropDetails.ToListAsync();
            if(cropList.Count > 0)
            {
                return cropList;
            }
            return null;
        }

        public async Task<ActionResult<CropDetail>> GetCropByIdAsync(int id)
        {
            var crop = await  _context.CropDetails.FirstOrDefaultAsync(x => x.CropId == id);
            if(crop != null)
            {
                return crop;
            }

            return null;
        }

        public async Task<ActionResult<CropDetail>> EditCropAsync(int id, UpdateCropDto crop)
        {
            var isExistCrop = await _context.CropDetails.FirstOrDefaultAsync(x => x.CropId == id);

            if (isExistCrop != null)
            {
                isExistCrop.CropName = crop.CropName;
                isExistCrop.ExpectedPrice = crop.CropExpectedPrice;
                isExistCrop.Location = crop.CropLocation;
                isExistCrop.QtyAvailable = crop.CropQtyAvailable;
                isExistCrop.CropTypeId = (int)Enum.Parse(typeof(CropId), crop.CropType);
                //_context.Attach(isExistCrop);
                await _context.SaveChangesAsync();
                return isExistCrop;
            }

            return null;
        }

        public async Task<ActionResult<ViewCropDto>> ViewCropByIdAsync(int id)
        {
            var crop = await _context.CropDetails.Include("CropType").Include("User").FirstOrDefaultAsync(x => x.CropId == id);
            if (crop != null)
            {
                ViewCropDto viewCropDto = new ViewCropDto();
                viewCropDto.CropType = crop.CropType.TypeName;
                viewCropDto.CropName = crop.CropName;
                viewCropDto.CropLocation = crop.Location;
                viewCropDto.CropQtyAvailable = crop.QtyAvailable;
                viewCropDto.CropExpectedPrice = crop.ExpectedPrice;
                viewCropDto.FarmerName = crop.User.Name;
                viewCropDto.FarmerPhone = crop.User.Phone;
                viewCropDto.FarmerEmail = crop.User.Email;
                return viewCropDto;
            }

            return null;
        }
    }
}
