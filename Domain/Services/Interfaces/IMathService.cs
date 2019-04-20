namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IMathService
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
        decimal PercentDiff(decimal fromValue, decimal toValue);
        int FindNumbersPosition(decimal number, decimal low, decimal high, int numberOfPositions);
        decimal Positive(decimal number);
        string FormatPercent(decimal percent);
    }
}