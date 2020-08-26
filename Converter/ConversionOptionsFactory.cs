using ImageMagick;
using imgresizer.Models;
using Microsoft.Extensions.Options;
using System;

namespace imgresizer.Converter
{
    public static class ConversionOptionsFactory
    {
        public static ConversionOptions FromResizeRequest(ResizeRequestModel requestModel)
        {
            var options = new ConversionOptions
            {
                Name = requestModel.Name
            };
            
            if(requestModel.Width.HasValue && requestModel.Width > 0)
            {
                options.Width = Math.Min(requestModel.Width.Value, ConversionOptions.MaxSize);
            }

            if (requestModel.Height.HasValue && requestModel.Height > 0)
            {
                options.Height = Math.Min(requestModel.Height.Value, ConversionOptions.MaxSize);
            }

            if(requestModel.Quality.HasValue && requestModel.Quality >= 1 && requestModel.Quality.Value <= 100)
            {
                options.Quality = requestModel.Quality.Value;
            }
            else
            {
                options.Quality = options.TargetFormat == MagickFormat.Png24
                    ? 100
                    : 82;
            }

            options.TargetFormat = GetMagickFormat(requestModel.Format);

            return options;
        }

        private static MagickFormat GetMagickFormat(string format)
        {
            if(string.IsNullOrWhiteSpace(format) == false)
            {
                switch(format.ToLower())
                {
                    case "png":
                        return MagickFormat.Png24;

                    case "jpeg":
                    case "jpg":
                        return MagickFormat.Jpeg;
                }
            }

            return MagickFormat.Jpeg;
        }
    }
}