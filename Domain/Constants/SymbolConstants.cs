﻿namespace CryptoKeeper.Domain.Constants
{
    public static class SymbolConstants
    {
        public const string Usd = "USD"; //Dollar
        public const string Eur = "EUR"; //Euro
        public const string Gbp = "GBP"; //Pound
        public const string Jpy = "JPY"; //Yen
        public const string Pln = "PLN"; //Poland Zloty

        public const string Usdt = "USDT";
        public const string Btc = "BTC";
        public const string Ltc = "LTC";

        public static string[] FiatCurrency = new[] {Usd, Eur, Gbp, Jpy, Pln};
    }
}
