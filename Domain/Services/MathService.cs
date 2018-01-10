using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class MathService : IMathService
    {
        /// <summary>
        /// Percentage difference for the same coin between two exchanges.
        /// </summary>
        /// <param name="fromValue">Value from the source exchange.</param>
        /// <param name="toValue">Value from the target exchange.</param>
        /// <returns>
        /// Percent difference. 1 being 100%, 0.01 being 1%. 
        /// When fromValue is lower than toValue the return
        /// will be negative. This is good.
        /// </returns>
        public decimal PercentDiff(decimal fromValue, decimal toValue)
        {
            if (fromValue > toValue)
            {
                //if the toValue is larger than fromValue,
                //reverse the numbers so the division works
                //and we get a decimal style percent, then
                //invert the negative so we have a positive
                //result and can act accordingly.
                return PercentDiff(toValue, fromValue) * -1;
            }
            return (fromValue / toValue) - 1;
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
    }
}
