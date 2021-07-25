using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Az101.Gallery.Dto;
using Az101.Gallery.Infrastructure.Persistence;
using Az101.Gallery.Infrastructure.Storage;

using Az101.Gallery.Models;
using System.Linq;

namespace Az101.Gallery.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    public class PhotosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IStorage storage;

        public PhotosController(ApplicationDbContext dbContext, IStorage storage)
        {
            this.storage = storage ?? throw new System.ArgumentNullException(nameof(storage));
            this.dbContext = dbContext ?? throw new System.ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = dbContext.Photos.AsNoTracking();
            var containerUrl = storage.GetContainerUrl();

            var result = await query
                .Select(r => new PhotoViewModel
                {
                    Id = r.Id,
                    Alt = r.Alt,
                    CreatedAt = r.CreatedAt,
                    FileUrl = $"{containerUrl}/{r.FileName}",
                    Title = r.Title,
                    UpdateAt = r.UpdateAt
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get([FromRoute] int id)
        {
            var photo = await dbContext.Photos
                .AsNoTracking()
                .SingleOrDefaultAsync(r => r.Id == id);

            if (photo is null)
            {
                NotFound(id);
            }

            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] PhotoForm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var stream = model.File.OpenReadStream();
            var newFileName = GenerateFileName(model.Title);

            await storage.SaveAsync(newFileName, stream);

            var photo = new Photo
            {
                Title = model.Title,
                Alt = model.Alt,
                FileName = newFileName
            };

            await dbContext.AddAsync(photo);
            await dbContext.SaveChangesAsync();

            return CreatedAtAction("Get", new { id = photo.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit([FromRoute] int id, [FromForm] PhotoForm model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var photoToEdit = await dbContext.Photos.FindAsync(id);
            if (photoToEdit is null)
            {
                return NotFound(id);
            }

            var oldFileName = photoToEdit.FileName;
            var newFileName = GenerateFileName(model.Title);

            using var stream = model.File.OpenReadStream();
            await storage.SaveAsync(newFileName, stream);

            photoToEdit.Title = model.Title;
            photoToEdit.Alt = model.Alt;
            photoToEdit.UpdateAt = DateTime.UtcNow;
            photoToEdit.FileName = newFileName;

            await dbContext.SaveChangesAsync();

            await storage.DeleteAsync(oldFileName);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var photoToDelete = await dbContext.Photos.FindAsync(id);

            if (photoToDelete is null)
            {
                return NotFound(id);
            }

            await storage.DeleteAsync(photoToDelete.FileName);

            dbContext.Remove(photoToDelete);
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private string GenerateFileName(string title) => $"{title}_{DateTime.UtcNow:yyyyMMddHHmmssffff}";

    }
}
