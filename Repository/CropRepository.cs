﻿using CaseStudy.Dtos.CropDto;
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
        ExceptionRepository _exception;
        public CropRepository(DatabaseContext context,ExceptionRepository exception)
        {
            _context = context;
            _exception = exception;
        }

        private enum CropId
        {
            Fruit = 1,
            Vegetable = 2,
            Grain = 3
        }
        #region CreateCrop
        /// <summary>
        /// Method to add a new Crop
        /// </summary>
        /// <param name="crop"></param>
        /// <returns></returns>
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
               await _exception.AddException(e, "AddCrop Method in CropRepo");
            }
            return null;
        }
        #endregion
        #region GetAllCrops
        /// <summary>
        /// Get list of all crops
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CropDetail>> GetAllCropAsync()
        {
            try
            {
                var cropList = await _context.CropDetails.ToListAsync();
                if (cropList.Count > 0)
                {
                    return cropList;
                }
            }
            catch(Exception e)
            {
                await _exception.AddException(e, "GetAllCrop Method in CropRepo");
            }
            return null;
        }
        #endregion

        #region GetCropById
        /// <summary>
        /// Get single crop based on Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult<CropDetail>> GetCropByIdAsync(int id)
        {
            try
            {
                var crop = await _context.CropDetails.FirstOrDefaultAsync(x => x.CropId == id);
                if (crop != null)
                {
                    return crop;
                }
                return null;
            }
            catch(Exception e)
            {
                await _exception.AddException(e, "GetCropById method in CropRepo");
                return null;
            }
          
        }
        #endregion
        #region EditCrop
        /// <summary>
        /// Method to edit the crop details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="crop"></param>
        /// <returns></returns>
        public async Task<ActionResult<CropDetail>> EditCropAsync(int id, UpdateCropDto crop)
        {
            try
            {
                var isExistCrop = await _context.CropDetails.FirstOrDefaultAsync(x => x.CropId == id && x.FarmerId == crop.FarmerId);

                if (isExistCrop != null)
                {
                    isExistCrop.CropName = crop.CropName;
                    isExistCrop.ExpectedPrice = crop.CropExpectedPrice;
                    isExistCrop.Location = crop.CropLocation;
                    isExistCrop.QtyAvailable = crop.CropQtyAvailable;

                    isExistCrop.CropTypeId = (int)Enum.Parse(typeof(CropId), crop.CropType);
                    _context.CropDetails.Update(isExistCrop);
                    await _context.SaveChangesAsync();
                    return isExistCrop;
                }
                return null;
            }
            catch(Exception e)
            {
                await _exception.AddException(e, "EditCrop Method in CropRepo");
                return null;
            }
            
        }
        #endregion

        #region GetCropById
        /// <summary>
        /// Get Crop By Id with more details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ActionResult<ViewCropDto>> ViewCropByIdAsync(int id)
        {
            try
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
                    viewCropDto.FarmerId = crop.User.UserId;
                    return viewCropDto;
                }
                return null;
            }
            catch (Exception e)
            {
                await _exception.AddException(e, "ViewCropById Method in CropRepo");
                return null;
            }
            
        }
        #endregion
    }
}
