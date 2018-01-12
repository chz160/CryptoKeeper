using System.Drawing;

namespace CryptoKeeper.Domain.DataObjects.Dtos
{
    public class AsciiImageDto
    {
        public AsciiImageDto(string character, Color color)
        {
            Character = character;
            Color = color;
        }
        public string Character { get; set; }
        public Color Color { get; set; }
    }
}