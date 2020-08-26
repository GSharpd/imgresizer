using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace imgresizer.Controllers
{
    public partial class ImagesController : Controller
    {
        [HttpGet("api/images")]
        public async Task<IActionResult> GetImage(
            [FromQuery]string name,
            [FromServices]IWebHostEnvironment env
            )
        {
            var filepath = Path.Combine(env.ContentRootPath, "Images", name);
            var fileExists = System.IO.File.Exists(filepath);
            if (!fileExists)
            {
                return NotFound();
            }

            var file = await System.IO.File.ReadAllBytesAsync(filepath);
            return File(file, "image/jpeg");
        }
    }
}
