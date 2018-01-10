namespace CryptoKeeper.Domain.Services.Interfaces
{
    public interface IMathService
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
        decimal PercentDiff(decimal fromValue, decimal toValue);
        int FindNumbersPosition(decimal number, decimal low, decimal high, int numberOfPositions);
    }
}