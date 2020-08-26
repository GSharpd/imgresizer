using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageMagick;
using imgresizer.Converter;
using imgresizer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace imgresizer.Controllers
{
    public partial class ImagesController : Controller
    {
        [HttpGet("api/images/resize")]
        public async Task<IActionResult> ResizeImage(
            [FromQuery]ResizeRequestModel requestModel,
            [FromServices]IWebHostEnvironment env
            )
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var filepath = Path.Combine(env.ContentRootPath, "Images", requestModel.Name);
            var fileExists = System.IO.File.Exists(filepath);
            if (!fileExists)
            {
                return NotFound();
            }

            var options = ConversionOptionsFactory.FromResizeRequest(requestModel);

            using(var memory = new MemoryStream())
            using (var image = new MagickImage(filepath))
            {
                image.Resize(options.Width, options.Height);
                image.Strip();
                image.Quality = options.Quality;
                image.Format = options.TargetFormat;
                image.Write(memory);
                var file = memory.ToArray();
                return File(file, options.TargetMimeType);
            }
        }
    }
}
