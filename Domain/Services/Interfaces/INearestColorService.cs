using System;
using System.Drawing;

namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface INearestColorService
    {
        ConsoleColor ClosestConsoleColor(byte r, byte g, byte b);
        Color ClosestColor(byte r, byte g, byte b);
    }
}