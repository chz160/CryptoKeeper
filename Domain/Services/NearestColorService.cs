using System;
using System.Collections.Generic;
using System.Drawing;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class NearestColorService : INearestColorService
    {
        public ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }

        public Color ClosestColor(byte r, byte g, byte b)
        {
            var result = Color.Black;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (var cc in GetAllColors())
            {
                var c = Color.FromName(cc.Name);
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    result = cc;
                }
            }
            return result;
        }

        private List<Color> GetAllColors()
        {
            var allColors = new List<Color>();
            foreach (var property in typeof(Color).GetProperties())
            {
                if (property.PropertyType == typeof(Color))
                {
                    allColors.Add((Color)property.GetValue(null));
                }
            }
            return allColors;
        }
    }
}