using System.Collections.Generic;
using System.Drawing;
using CryptoKeeper.Domain.DataObjects.Dtos;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IAsciiImageConverterService
    {
        List<AsciiImageDto> ImageToAscii(byte[] bytes);
        List<AsciiImageDto> ImageToAscii(Image img);
    }
}