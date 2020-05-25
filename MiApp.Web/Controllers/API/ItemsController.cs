using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiApp.Web.Data;
using MiApp.Web.Data.Entities;
using MiApp.Common.Models;
using MiApp.Web.Helpers;
using System.IO;
using MiApp.Common.Helpers;

namespace MiApp.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ItemsController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public ItemsController(DataContext context, IConverterHelper converterHelper, IImageHelper imageHelper)
        {
            _context = context;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        // GET: api/Items
        [HttpGet]
        public async Task<IActionResult> GetItems()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var items = await _context.Items
                .OrderBy(t => t.Name)
                .ToListAsync();

            if (items == null)
            {
                return BadRequest("No hay Items.");
            }


            List<ItemResponse> itemResponses = new List<ItemResponse>();

            foreach (ItemEntity item in items)
            {
                ItemResponse itemResponse = _converterHelper.ToItemResponse(item);
                itemResponses.Add(itemResponse);
            }

            return Ok(itemResponses);
        }

        // GET: api/Items/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemEntity([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var itemEntity = await _context.Items.FindAsync(id);

            if (itemEntity == null)
            {
                return NotFound();
            }

            return Ok(itemEntity);
        }

        // PUT: api/Items/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutItem([FromRoute] int id, [FromBody] ItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != request.Id)
            {
                return BadRequest();
            }

            var oldItem = await _context.Items.FindAsync(request.Id);
            if (oldItem == null)
            {
                return BadRequest("Este Item no existe.");
            }

            var imageUrl = oldItem.LogoPath;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                var stream = new MemoryStream(request.PictureArray);
                var guid = Guid.NewGuid().ToString();
                var file = $"{guid}.jpg";
                var folder = "wwwroot\\images\\Items";
                var fullPath = $"~/images/Items/{file}";
                var response = FilesHelper.UploadPhoto(stream, folder, file);

                if (response)
                {
                    imageUrl = fullPath;
                }
            }

            oldItem.Id = request.Id;
            oldItem.LogoPath = imageUrl;
            oldItem.Name = request.Name;
            oldItem.Price = request.Price;
            oldItem.Quantity = request.Quantity;
            oldItem.Date = request.Date;
            oldItem.Active = request.Active;
            

            _context.Items.Update(oldItem);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToItemResponse(oldItem));
        }

        // POST: api/Items

        [HttpPost]
        public async Task<IActionResult> PostItemEntity([FromBody] ItemRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string picturePath = string.Empty;
            if (request.PictureArray != null && request.PictureArray.Length > 0)
            {
                picturePath = _imageHelper.UploadImage(request.PictureArray, "Items");
            }

            var item = new ItemEntity
            {
             
                Active=request.Active,
                Date=request.Date,
                Id = request.Id,
                Name=request.Name,
                Price = request.Price,
                Quantity = request.Quantity,
                LogoPath = picturePath,
            };

            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return Ok(_converterHelper.ToItemResponse(item));
            //return NoContent();
        }

        // DELETE: api/Items/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return this.BadRequest(ModelState);
            }

            var item = await _context.Items
                .FirstOrDefaultAsync(p => p.Id == id);
            if (item == null)
            {
                return this.NotFound();
            }



            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
            return Ok("Item borrado");
        }

        private bool ItemEntityExists(int id)
        {
            return _context.Items.Any(e => e.Id == id);
        }
    }
}