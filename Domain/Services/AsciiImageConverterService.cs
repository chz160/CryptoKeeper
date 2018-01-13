using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class AsciiImageConverterService : IAsciiImageConverterService
    {
        private readonly INearestColorService _nearestColorService;
        private const string Black = "@";
        private const string Charcoal = "#";
        private const string Darkgray = "8";
        private const string Mediumgray = "&";
        private const string Medium = "o";
        private const string Gray = ":";
        private const string Slategray = "*";
        private const string Lightgray = ".";
        private const string White = " ";

        public AsciiImageConverterService() : this(new NearestColorService())
        { }

        public AsciiImageConverterService(INearestColorService nearestColorService)
        {
            _nearestColorService = nearestColorService;
        }

        public List<AsciiImageDto> ImageToAscii(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                return ImageToAscii(Image.FromStream(memoryStream));
            }
        }

        public List<AsciiImageDto> ImageToAscii(Image img)
        {
            var result = new List<AsciiImageDto>();
            Bitmap bmp = null;
            try
            {
                bmp = new Bitmap(img);
                for (var y = 0; y < bmp.Height; y++)
                {
                    for (var x = 0; x < bmp.Width; x++)
                    {
                        var color = bmp.GetPixel(x, y);
                        var grayscaleColor = Color.FromArgb((color.R + color.G + color.B) / 3,
                            (color.R + color.G + color.B) / 3,
                            (color.R + color.G + color.B) / 3);
                        var rValue = int.Parse(grayscaleColor.R.ToString());
                        result.Add(new AsciiImageDto(GetGrayShade(rValue), _nearestColorService.ClosestColor(color.R, color.G, color.B)));
                        if (x == bmp.Width - 1)
                            result.Add(new AsciiImageDto("\r\n", Color.Black));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bmp.Dispose();
            }
            return result;
        }

        private string GetGrayShade(int redValue)
        {
            var asciival = " ";
            if (redValue >= 230)
            {
                asciival = Black;
            }
            else if (redValue >= 200)
            {
                asciival = Charcoal;
            }
            else if (redValue >= 180)
            {
                asciival = Darkgray;
            }
            else if (redValue >= 160)
            {
                asciival = Mediumgray;
            }
            else if (redValue >= 130)
            {
                asciival = Medium;
            }
            else if (redValue >= 100)
            {
                asciival = Gray;
            }
            else if (redValue >= 70)
            {
                asciival = Slategray;
            }
            else if (redValue >= 50)
            {
                asciival = Lightgray;
            }
            else
            {
                asciival = White;
            }
            return asciival;
        }
    }
}
