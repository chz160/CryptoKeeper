using System;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class MathService : IMathService
    {
        /// <summary>
        /// Percentage difference for the same coin between two exchanges.
        /// </summary>
        /// <param name="valueA">Value from the source exchange.</param>
        /// <param name="valueB">Value from the target exchange.</param>
        /// <returns>
        /// Percent difference. 1 being 100%, 0.01 being 1%. 
        /// When valueA is larger than valueB the return
        /// will be negative. This is bad.
        /// </returns>
        public decimal PercentDiff(decimal valueA, decimal valueB)
        {
            if (valueA > valueB)
            {
                //if the valueA is larger than valueB,
                //reverse the numbers so the division works
                //and we get a decimal style percent, then
                //invert the value so we have a negative
                //result and can act accordingly.
                return PercentDiff(valueB, valueA) * -1;
            }
            return 1 - (valueA / valueB);
        }

        public int FindNumbersPosition(decimal number, decimal low, decimal high, int numberOfPositions)
        {
            if (high == null && number < 0) high = number * -1;
            if (high == null || number > high) high = number;
            if (low == null && number > 0) low = number * -1;
            if (low == null || number < low) low = number;
            var diffZeroToX = Positive(low) +
                              Positive(high);

            var numberShifted = Positive(number) + Positive(low);
            var position = 0;
            var positionSize = diffZeroToX / numberOfPositions;
            for (var i = 1; i <= numberOfPositions; i++)
            {
                if (i * positionSize > numberShifted) break;
                position = i;
            }
            return position;
        }

        public decimal Positive(decimal number)
        {
            if (number < 0)
            {
                return number * -1;
            }
            return number;
        }

        public string FormatPercent(decimal percent)
        {
            return $"{decimal.Round(percent * 100, 2, MidpointRounding.ToEven)}%";
        }
    }
}
