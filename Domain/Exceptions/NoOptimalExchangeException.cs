using System;

namespace CryptoKeeper.Domain.Exceptions
{
    public class NoOptimalExchangeException : Exception
    {
        public NoOptimalExchangeException(string message) : base(message)
        { }
    }
}
