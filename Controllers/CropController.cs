using CaseStudy.Dtos.CropDto;
using CaseStudy.Models;
using CaseStudy.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace CaseStudy.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CropController : ControllerBase
    {
        private readonly CropService _service;
        public CropController(CropService service)
        {
            _service = service;
        }

        [HttpPost("addCrop")]
        [Authorize(Roles = "Farmer")]
        public async Task<ActionResult<CropDetail>> AddNewCrop(AddCropDto crop)
        {

            var res = await _service.AddCropAsync(crop);
            if (res == null)
            {
                return BadRequest("Error while adding crop details");
            }
            return Ok(res);

        }

        
        [HttpGet("getCrops")]
        public async Task<ActionResult<IEnumerable<CropDetail>>> GetAllCrops()
        {
            var res = await _service.GetAllCropAsync();
            if(res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }

        
        [HttpGet("getCrops/{id}")]
        public async Task<ActionResult<CropDetail>> GetCropById(int id)
        {
            var res = await _service.GetCropByIdAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
        [HttpPut("editCrop/{cid}")]
        public async Task<ActionResult<CropDetail>> UpdateCrop(UpdateCropDto crop, int cid)
        {

            var res = await _service.EditCropAsync(cid, crop);
            if (res == null)
            {
                return BadRequest("Error while updating crop details");
            }
            return Ok(res);
        }

        [HttpGet("viewCrop/{id}")]
        public async Task<ActionResult<CropDetail>> viewCropById(int id)
        {
            var res = await _service.ViewCropByIdAsync(id);
            if (res == null)
            {
                return NotFound();
            }
            return Ok(res);
        }
    }
}
