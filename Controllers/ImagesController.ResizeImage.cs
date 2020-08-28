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
        [HttpGet("api/resize")]
        public async Task<IActionResult> ResizeImage(
            [FromQuery]ResizeRequestModel requestModel,
            [FromServices]IWebHostEnvironment env,
            [FromServices]ImageConverter converter
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
            var imageSource = await System.IO.File.ReadAllBytesAsync(filepath);
            var result = await converter.Convert(imageSource, options);

            if(result.Length == 0)
            {
                return BadRequest("Could not convert file.");
            }

            return File(result, options.TargetMimeType);
        }
    }
}
