﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CryptoKeeper.Domain.Constants;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.DataObjects.Params;
using CryptoKeeper.Domain.Exceptions;
using CryptoKeeper.Domain.Services.Factories;
using CryptoKeeper.Domain.Services.Interfaces;

namespace CryptoKeeper.Domain.Services
{
    public class TradingService
    {
        private readonly string _exchangeCurrentlyHoldingFunds;
        private readonly string _primaryCoin;
        private decimal _investment;
        private readonly IMathService _mathService;
        private readonly IExchangeApiServiceFactory _apiServiceFactory;
        private readonly ICryptoCompareDataService _cryptoCompareDataService;
        private readonly IConfigService _configService;

        private List<string> _eligibleSymbols;
        private List<Exchange> _exchanges;

        public TradingService(string exchangeCurrentlyHoldingFunds, string primaryCoin, decimal investment)
            : this(exchangeCurrentlyHoldingFunds, primaryCoin, investment, new MathService(), new ExchangeApiServiceFactory(), new CryptoCompareDataService(), new ConfigService())
        { }

        public TradingService(string exchangeCurrentlyHoldingFunds, string primaryCoin, decimal investment, IMathService mathService, IExchangeApiServiceFactory apiServiceFactory, ICryptoCompareDataService cryptoCompareDataService, IConfigService configService)
        {
            _exchangeCurrentlyHoldingFunds = exchangeCurrentlyHoldingFunds;
            _primaryCoin = primaryCoin;
            _investment = investment;
            _mathService = mathService;
            _apiServiceFactory = apiServiceFactory;
            _cryptoCompareDataService = cryptoCompareDataService;
            _configService = configService;
        }
        public void Trade()
        {
            try
            {
                Console.Write($"Getting top volume coins for {_primaryCoin}...");
                _eligibleSymbols = _cryptoCompareDataService.GetTopVolumeSymbols(_primaryCoin).ToList();
                _eligibleSymbols = _eligibleSymbols.Where(m => m != "BTG").ToList();
                Console.WriteLine("Done.");
                //_exchanges = _cryptoCompareDataService.GetTopExchangesForPair(_primaryCoin, _valueCoin);
                //_cryptoCompareDataService.GetExchangeCoins(_primaryCoin, _valueCoin, _exchanges, _eligibleSymbols);
                //TEMP use
                //_exchanges = _cryptoCompareDataService.GetAllExchanges();
                //_cryptoCompareDataService.GetExchangeCoins(_primaryCoin, null, _exchanges, _eligibleSymbols);

                Console.Write("Loading API config file...");
                _exchanges = _configService.GetConfiguredExchanges();
                Console.WriteLine("Done.");

                Console.Write("Getting coins for exchanges...");
                _exchanges.ForEach(exchange=> { _apiServiceFactory.Create(exchange).GetProducts(exchange, _eligibleSymbols); });
                Console.WriteLine("Done.");

                _exchanges.ForEach(exchange =>
                {
                    var fees = _apiServiceFactory.Create(exchange).GetWithdrawalFees();
                    exchange.Coins.ForEach(coin =>
                        {
                            if (!fees.Any(m => m.Symbol == coin.Symbol))
                            {
                                Colorful.Console.WriteLine($"Missing withdawal fee for {coin.Symbol} on {exchange.Name}.", Color.Red);
                            }
                        });
                });

                var exchangePairParam = new ExchangePairParam(_primaryCoin, _exchangeCurrentlyHoldingFunds, _eligibleSymbols, _exchanges);
                //var exchangePairParam = new ExchangePairParam(_primaryCoin, _valueCoin, _exchangeCurrentlyHoldingFunds, null, _exchanges);

                PricingService.Instance.StartPricingDataThreads(exchangePairParam);

                LocateAllTradableAssets(exchangePairParam);
                MoveAssetsToOptimalExchange(exchangePairParam);

                //Console.WriteLine("Press any key to continue.");
                //Console.ReadKey();

                var iteration = 1;
                while (true)
                {
                    Console.WriteLine("---------------------------------------------------------------");
                    Console.WriteLine($"Investing {_investment} on iteration {iteration}...");
                    FindOptimalExchangePair(exchangePairParam);
                    if (exchangePairParam.LowestExchange == null || exchangePairParam.HighestExchange == null)
                    {
                        Console.WriteLine($"An optimal exchange path was not found for {_primaryCoin}. Will try again in 1 minute.");
                        Task.Delay(1000 * 60 * 1).Wait();
                        continue;
                    }
                    Console.WriteLine($"Buying {exchangePairParam.TrackbackFromCoin.Symbol} from {exchangePairParam.LowestExchange.Name} for {exchangePairParam.TrackbackFromCoin.Coins.First(m=>m.Symbol == _primaryCoin).Ask} then selling at {exchangePairParam.HighestExchange.Name} for {exchangePairParam.TrackbackToCoin.Coins.First(m => m.Symbol == _primaryCoin).Bid} at {decimal.Round(exchangePairParam.TrackbackPercentDiff * -100, 2, MidpointRounding.ToEven)}%");

                    decimal initialCoinAmount = _investment;
                    decimal buyTrackbackTakerFee = _apiServiceFactory.Create(exchangePairParam.LowestExchange).TakerFee;
                    decimal initialCoinAmountMinusTakerFee = initialCoinAmount - buyTrackbackTakerFee;
                    decimal purchasedTrackbackAmount = initialCoinAmountMinusTakerFee / exchangePairParam.TrackbackFromCoin.Coins.First(m => m.Symbol == _primaryCoin).Ask;
                    decimal firstNetworkTransferFee = PricingService.Instance.GetWithdrawalFeesForExchangeAndSymbol(exchangePairParam.LowestExchange, exchangePairParam.TrackbackFromCoin.Symbol);
                    decimal purchasedTrackbackAmountMinusTransferFee = purchasedTrackbackAmount - firstNetworkTransferFee;
                    decimal buyPrimaryTakerFee = _apiServiceFactory.Create(exchangePairParam.HighestExchange).TakerFee;
                    decimal trackBackAmountMinusTakerFee = purchasedTrackbackAmountMinusTransferFee - buyPrimaryTakerFee;
                    decimal purchasePrimaryAmount = trackBackAmountMinusTakerFee * exchangePairParam.TrackbackToCoin.Coins.First(m => m.Symbol == _primaryCoin).Ask;

                    //Console.WriteLine($"Projected gain from buying {exchangePairParam.TrackbackFromCoin.Symbol} from {exchangePairParam.LowestExchange.Name} ({exchangePairParam.LowestExchange.Coins.First(m => m.Symbol == _primaryCoin).Coins.First(m => _valueCoin.Contains(m.Symbol)).Price}) to {exchangePairParam.HighestExchange.Name} ({exchangePairParam.HighestExchange.Coins.First(m => m.Symbol == _primaryCoin).Coins.First(m => _valueCoin.Contains(m.Symbol)).Price}) is {decimal.Round(exchangePairParam.PrimaryPercentDiff * -100, 2, MidpointRounding.ToEven)}%");
                    //Console.Write($"Waiting on transfer if {exchangePairParam.PrimaryCoin} from {exchangePairParam.LowestExchange.Name} to {exchangePairParam.HighestExchange.Name}...");
                    ////Thread.Sleep(1000 * 60 * 55);
                    //Thread.Sleep(1000 * 60);
                    //Console.WriteLine("Done.");

                    ////Console.WriteLine("Press any key to continue.");
                    ////Console.ReadKey();

                    //UpdateStatsAfterDirectMove(exchangePairParam);
                    //Console.WriteLine($"Actual gain {exchangePairParam.LowestExchange.Coins.First(m => m.Symbol == _primaryCoin).Coins.First(m => _valueCoin.Contains(m.Symbol)).Price} --> {exchangePairParam.HighestExchange.Coins.First(m => m.Symbol == _primaryCoin).Coins.First(m => _valueCoin.Contains(m.Symbol)).Price} = {decimal.Round(exchangePairParam.PrimaryPercentDiff * -100, 2, MidpointRounding.ToEven)}%");

                    //decimal differenceAfterTransferToHigh = initalCoinPurchaseMinusFee * (exchangePairParam.PrimaryPercentDiff * -1);
                    //decimal valueAfterTransferToHigh = initalCoinPurchaseMinusFee + differenceAfterTransferToHigh;

                    //FindBestTrackbackMove(exchangePairParam);
                    //Console.WriteLine($"Projected gain from tracking back {exchangePairParam.TrackbackFromCoin.Symbol} from {exchangePairParam.HighestExchange.Name} ({exchangePairParam.TrackbackFromCoin.Coins.First(m => m.Symbol == _primaryCoin).Price}) to {exchangePairParam.LowestExchange.Name} ({exchangePairParam.TrackbackToCoin.Coins.First(m => m.Symbol == _primaryCoin).Price}) is {decimal.Round(exchangePairParam.TrackbackPercentDiff * -100, 2, MidpointRounding.ToEven)}%");
                    //decimal convertionToTrackback = valueAfterTransferToHigh / exchangePairParam.TrackbackFromCoin.Coins.First(m => m.Symbol == _primaryCoin).Price;
                    //decimal firstTakerFee = convertionToTrackback * PricingService.Instance.GetTakerFeeForExchange(exchangePairParam.HighestExchange);
                    //decimal secondNetworkTransferFee = PricingService.Instance.GetWithdrawalFeesForExchangeAndSymbol(exchangePairParam.HighestExchange, exchangePairParam.TrackbackFromCoin.Symbol);
                    //decimal trackbackSansFees = convertionToTrackback - firstTakerFee - secondNetworkTransferFee;

                    //Console.Write($"Waiting on transfer if {exchangePairParam.TrackbackFromCoin.Symbol} --> {exchangePairParam.HighestExchange.Name} to {exchangePairParam.LowestExchange.Name}...");
                    ////Thread.Sleep(1000 * 60 * 20);
                    //Thread.Sleep(1000 * 60);
                    //Console.WriteLine("Done.");

                    ////Console.WriteLine("Press any key to continue.");
                    ////Console.ReadKey();

                    //UpdateStatsAfterTrackback(exchangePairParam);
                    //Console.WriteLine($"Actual gain {exchangePairParam.TrackbackFromCoin.Coins.First(m => m.Symbol == _primaryCoin).Price} --> {exchangePairParam.TrackbackToCoin.Coins.First(m => m.Symbol == _primaryCoin).Price} = {decimal.Round(exchangePairParam.TrackbackPercentDiff * -100, 2, MidpointRounding.ToEven)}%");

                    //decimal differenceAfterTransferToLow = trackbackSansFees * (-1 * exchangePairParam.TrackbackPercentDiff);
                    //decimal valueAfterTransferToLow = trackbackSansFees + differenceAfterTransferToLow;
                    //decimal convertionToPrimary = valueAfterTransferToLow * exchangePairParam.TrackbackToCoin.Coins.First(m => m.Symbol == _primaryCoin).Price;
                    //decimal secondTakerFee = convertionToPrimary * PricingService.Instance.GetTakerFeeForExchange(exchangePairParam.LowestExchange);
                    //decimal primaryFinalSansFees = convertionToPrimary - secondTakerFee;
                    //decimal gainsAfterRoundTrip = primaryFinalSansFees - initialCoinAmount;
                    //decimal usdGains = gainsAfterRoundTrip * exchangePairParam.LowestExchange.Coins.First(m => m.Symbol == _primaryCoin).Coins.First(m => m.Symbol == SymbolConstants.Usd || m.Symbol == SymbolConstants.Usdt).Price;
                    //decimal usdTotal = _investment + usdGains;

                    //_investment = decimal.Round(usdTotal, 2, MidpointRounding.ToEven);
                    //Console.WriteLine($"Investment grew {decimal.Round(gainsAfterRoundTrip / initialCoinAmount * 100, 2, MidpointRounding.ToEven)}% to ${_investment}!");
                    Task.Delay(1000 * 60).Wait();
                    iteration++;

                    //Console.WriteLine("Press any key to continue.");
                    //Console.ReadKey();
                }
            }
            catch (NoOptimalExchangeException noOptExcEx)
            {
                Console.WriteLine(noOptExcEx.Message);
                Task.Delay(1000 * 60 * 5).Wait();
                Trade();
            }
        }

        public void FindOptimalExchangePair(ExchangePairParam param, bool disreguardSittingExchange = false)
        {
            PricingService.Instance.UpdateExchangeCoinPrices(param.Exchanges);

            //var possibleLowestList = param.Exchanges.OrderBy(m => m.Coins.FirstOrDefault(n => n.Symbol == param.PrimaryCoin)?.Coins.FirstOrDefault(n => param.ValueCoin.Contains(n.Symbol))?.Price ?? 99999999999).ToList();
            var possibleLowestList = param.Exchanges.OrderBy(m=>m.Name).ToList();
            if (disreguardSittingExchange == false)
            {
                var cloneParam = param.DeepClone();
                FindOptimalExchangePair(cloneParam, true);
                possibleLowestList = possibleLowestList.Where(m => m.Name == param.ExchangeCurrentlyHoldingFunds).ToList();
                if (cloneParam.LowestExchange.Name != param.ExchangeCurrentlyHoldingFunds)
                {
                    Console.WriteLine($"Transfering from {cloneParam.LowestExchange.Name} to {cloneParam.HighestExchange.Name} is the optimal route at this time but your funds are at {param.ExchangeCurrentlyHoldingFunds}. Think about restarting to funds can be routed to the optimal exchange.");
                }
            }
            var possibleHighestList = param.Exchanges.OrderByDescending(m => m.Name).ToList();
            foreach (var possibleLowest in possibleLowestList)
            {
                foreach (var possibleHighest in possibleHighestList.Where(m => m.Name != possibleLowest.Name))
                {
                    foreach (var highestTrackBackCoin in possibleHighest.Coins.Where(m => m.Symbol != param.PrimaryCoin))
                    {
                        var lowestTrackBackCoin = possibleLowest.Coins.FirstOrDefault(m =>
                            m.Symbol == highestTrackBackCoin.Symbol &&
                            m.Coins.Any(n => n.Symbol == param.PrimaryCoin));
                        if (lowestTrackBackCoin != null &&
                            lowestTrackBackCoin.Coins.Any(m => m.Symbol == param.PrimaryCoin && m.Ask != 0 && m.Bid != 0) &&
                            highestTrackBackCoin.Coins.Any(m => m.Symbol == param.PrimaryCoin && m.Ask != 0 && m.Bid != 0))
                        {
                            var trackbackFromValue = lowestTrackBackCoin.Coins.First(m => m.Symbol == param.PrimaryCoin).Ask;
                            var trackbackToValue = highestTrackBackCoin.Coins.First(m => m.Symbol == param.PrimaryCoin).Bid; 
                            var checkTrackBackPercentDiff = _mathService.PercentDiff(trackbackFromValue, trackbackToValue);

                            var primaryFromValue = possibleLowest.Coins.FirstOrDefault(m => m.Symbol == highestTrackBackCoin.Symbol)?.Coins?.FirstOrDefault(m => m.Symbol == param.PrimaryCoin)?.Bid ?? decimal.MaxValue;
                            var primaryToValue = possibleHighest.Coins.FirstOrDefault(m => m.Symbol == highestTrackBackCoin.Symbol)?.Coins?.FirstOrDefault(m => m.Symbol == param.PrimaryCoin)?.Ask ?? decimal.MaxValue;
                            var checkPrimaryPercentDiff = _mathService.PercentDiff(primaryFromValue, primaryToValue);

                            var checkTotalPercentDiff = checkPrimaryPercentDiff + checkTrackBackPercentDiff;

                            //decimal initialCoinAmount = _investment;
                            //decimal buyTrackbackTakerFee = _apiServiceFactory.Create(exchangePairParam.LowestExchange).TakerFee;
                            //decimal initialCoinAmountMinusTakerFee = initialCoinAmount - buyTrackbackTakerFee;
                            //decimal purchasedTrackbackAmount = initialCoinAmountMinusTakerFee / exchangePairParam.TrackbackFromCoin.Coins.First(m => m.Symbol == _primaryCoin).Price;
                            //decimal firstNetworkTransferFee = PricingService.Instance.GetWithdrawalFeesForExchangeAndSymbol(exchangePairParam.LowestExchange, exchangePairParam.TrackbackFromCoin.Symbol);
                            //decimal purchasedTrackbackAmountMinusTransferFee = purchasedTrackbackAmount - firstNetworkTransferFee;
                            //decimal buyPrimaryTakerFee = _apiServiceFactory.Create(exchangePairParam.HighestExchange).TakerFee;
                            //decimal trackBackAmountMinusTakerFee = purchasedTrackbackAmountMinusTransferFee - buyPrimaryTakerFee;
                            //decimal purchasePrimaryAmount = trackBackAmountMinusTakerFee * exchangePairParam.TrackbackToCoin.Coins.First(m => m.Symbol == _primaryCoin).Price;

                            if (primaryFromValue != decimal.MaxValue &&
                                primaryToValue != decimal.MaxValue)
                            {
                                if (//checkPrimaryPercentDiff < 0.00m &&
                                    checkTrackBackPercentDiff < -0.015m &&
                                    checkTrackBackPercentDiff > -0.10m &&
                                    //checkTotalPercentDiff < -0.05m &&
                                    (checkTrackBackPercentDiff < param.TrackbackPercentDiff || param.TrackbackPercentDiff == 0))
                                {
                                    param.LowestExchange = possibleLowest;
                                    param.HighestExchange = possibleHighest;
                                    param.PrimaryPercentDiff = checkPrimaryPercentDiff;

                                    param.TrackbackFromCoin = lowestTrackBackCoin;
                                    param.TrackbackToCoin = highestTrackBackCoin;
                                    param.TrackbackPercentDiff = checkTrackBackPercentDiff;

                                    param.TotalPercentDiff = checkTotalPercentDiff;
                                }
                            }
                            
                        }
                    }
                }
            }
        }

        public void LocateAllTradableAssets(ExchangePairParam param)
        {
            Console.Write("Locating tradable assets...");
            if (string.IsNullOrEmpty(param.ExchangeCurrentlyHoldingFunds))
            {
                var largest = 0m;
                foreach (var exchange in param.Exchanges)
                {
                    var amount = _apiServiceFactory.Create(exchange).GetBalances(param.PrimaryCoin);
                    if (amount > largest)
                    {
                        largest = amount;
                        param.ExchangeCurrentlyHoldingFunds = exchange.Name;
                    }
                }
            }
            Console.WriteLine("Done.");
        }

        //TODO: after finding all tradable assets find optimal starting exchange and move assets there.
        //      figure out the appropriate threshold for direct move vs trackback based on primary coin
        //      price difference, transfer fees, and exchange fees.
        public void MoveAssetsToOptimalExchange(ExchangePairParam exchangePairParam)
        {
            PricingService.Instance.UpdateExchangeCoinPrices(exchangePairParam.Exchanges);
            var tempExchangePairParam = exchangePairParam.DeepClone();
            FindOptimalExchangePair(tempExchangePairParam, true);
            var startingExchange = tempExchangePairParam.LowestExchange;
            if (startingExchange?.Name == null) throw new NoOptimalExchangeException($"An optimal exchange path was not found for {tempExchangePairParam.PrimaryCoin}. Will try again in 5 minutes...");
            if (tempExchangePairParam.ExchangeCurrentlyHoldingFunds != startingExchange.Name)
            {
                //var bestDirectExchangePairParam = exchangePairParam.DeepClone();
                //FindBestDirectMove(bestDirectExchangePairParam);
                //var bestTrackBackExchangePairParam = exchangePairParam.DeepClone();
                //bestTrackBackExchangePairParam.HighestExchange = bestDirectExchangePairParam.LowestExchange;
                //bestTrackBackExchangePairParam.LowestExchange = startingExchange;
                //FindBestTrackbackMove(bestTrackBackExchangePairParam);

                //string verbage;
                //if (bestDirectExchangePairParam.TotalPercentDiff <= bestTrackBackExchangePairParam.TotalPercentDiff)
                //{
                //    verbage = $"as a direct transfer on {exchangePairParam.PrimaryCoin}";
                //}
                //else
                //{
                //    verbage = $"as a trackback with {bestTrackBackExchangePairParam.TrackbackFromCoin.Symbol}";
                //}

                //Console.Write($"For optimal trading, assets are being moved from {tempExchangePairParam.ExchangeCurrentlyHoldingFunds} to {startingExchange.Name} {verbage}...");

                Console.Write($"For optimal trading, assets are being moved from {tempExchangePairParam.ExchangeCurrentlyHoldingFunds} to {startingExchange.Name}...");

                exchangePairParam.ExchangeCurrentlyHoldingFunds = startingExchange.Name;
                Console.WriteLine("Done.");
            }
            else
            {
                Console.WriteLine($"Assets already in optimal position at {startingExchange.Name}.");
                //exchangePairParam.ExchangeCurrentlyHoldingFunds = exchangePairParam.ExchangeCurrentlyHoldingFunds;
            }
        }

        //public void FindBestDirectMove(ExchangePairParam param)
        //{
        //    decimal? holdPrimaryPercentDiff = null;
        //    var currentExchange = param.Exchanges.First(m => m.Name == param.ExchangeCurrentlyHoldingFunds);
        //    var possibleHighestList = param.Exchanges
        //        .Where(m=>m.Name != currentExchange.Name)
        //        .OrderByDescending(m => m.Coins.First(n => n.Symbol == param.PrimaryCoin).Coins.First(n => param.ValueCoin.Contains(n.Symbol)).Price).ToList();
        //    foreach (var possibleHighest in possibleHighestList)
        //    {
        //        foreach (var highestTrackBackCoin in possibleHighest.Coins.Where(m => m.Symbol != param.PrimaryCoin))
        //        {
        //            var lowestTrackBackCoin = currentExchange.Coins.FirstOrDefault(m =>
        //                m.Symbol == highestTrackBackCoin.Symbol &&
        //                m.Coins.Any(n => n.Symbol == param.PrimaryCoin));
        //            if (lowestTrackBackCoin != null &&
        //                lowestTrackBackCoin.Coins.Any(m => m.Symbol == param.PrimaryCoin) &&
        //                highestTrackBackCoin.Coins.Any(m => m.Symbol == param.PrimaryCoin))
        //            {
        //                var primaryFromValue = currentExchange.Coins.First(m => m.Symbol == param.PrimaryCoin).Coins.First(m => param.ValueCoin.Contains(m.Symbol)).Price;
        //                var primaryToValue = possibleHighest.Coins.First(m => m.Symbol == param.PrimaryCoin).Coins.First(m => param.ValueCoin.Contains(m.Symbol)).Price;
        //                var checkPrimaryPercentDiff = _mathService.PercentDiff(primaryFromValue, primaryToValue);

        //                //var trackbackFromValue = highestTrackBackCoin.Coins.First(m => m.Symbol == param.PrimaryCoin).Price;
        //                //var trackbackToValue = lowestTrackBackCoin.Coins.First(m => m.Symbol == param.PrimaryCoin).Price;
        //                //var checkTrackBackPercentDiff = _mathService.PercentDiff(trackbackFromValue, trackbackToValue);

        //                //var checkTotalPercentDiff = checkPrimaryPercentDiff + checkTrackBackPercentDiff;

        //                if (holdPrimaryPercentDiff == null || checkPrimaryPercentDiff < holdPrimaryPercentDiff.Value)
        //                {
        //                    param.LowestExchange = currentExchange;
        //                    param.HighestExchange = possibleHighest;
        //                    param.PrimaryPercentDiff = checkPrimaryPercentDiff;

        //                    //param.TrackbackToCoin = lowestTrackBackCoin;
        //                    //param.TrackbackFromCoin = highestTrackBackCoin;
        //                    //param.TrackbackPercentDiff = checkTrackBackPercentDiff;

        //                    //param.TotalPercentDiff = checkTotalPercentDiff;
        //                    holdPrimaryPercentDiff = checkPrimaryPercentDiff;
        //                }
        //            }
        //        }
        //    }
        //}

        public void FindBestTrackbackMove(ExchangePairParam param)
        {
            var weight = 0m;
            foreach (var highestTrackBackCoin in param.HighestExchange.Coins.Where(m => m.Symbol != param.PrimaryCoin))
            {
                var lowestTrackBackCoin = param.LowestExchange.Coins.FirstOrDefault(m =>
                    m.Symbol == highestTrackBackCoin.Symbol &&
                    m.Coins.Any(n => n.Symbol == param.PrimaryCoin));
                if (lowestTrackBackCoin != null &&
                    lowestTrackBackCoin.Coins.Any(m => m.Symbol == param.PrimaryCoin) &&
                    highestTrackBackCoin.Coins.Any(m => m.Symbol == param.PrimaryCoin))
                {
                    //var primaryFromValue = param.HighestExchange.Coins.First(m => m.Symbol == param.PrimaryCoin).Coins.First(m => m.Symbol == param.ValueCoin).Price;
                    //var primaryToValue = param.LowestExchange.Coins.First(m => m.Symbol == param.PrimaryCoin).Coins.First(m => m.Symbol == param.ValueCoin).Price;
                    //var checkPrimaryPercentDiff = PercentDiff(primaryFromValue, primaryToValue);

                    var trackbackFromValue = highestTrackBackCoin.Coins.First(m => m.Symbol == param.PrimaryCoin).Price;
                    var trackbackToValue = lowestTrackBackCoin.Coins.First(m => m.Symbol == param.PrimaryCoin).Price;
                    var checkTrackBackPercentDiff = _mathService.PercentDiff(trackbackFromValue, trackbackToValue);

                    var checkTotalPercentDiff = param.PrimaryPercentDiff + checkTrackBackPercentDiff;
                    var checkWeight = PricingService.Instance.CalculateTrackbackWeight(param.HighestExchange.Name, param.LowestExchange.Name, highestTrackBackCoin.Symbol, param.PrimaryCoin, checkTrackBackPercentDiff);

                    if (param.PrimaryPercentDiff < -0.05m &&
                        checkTrackBackPercentDiff < 0.01m &&
                        checkTrackBackPercentDiff > -0.1m &&
                        checkTotalPercentDiff < -0.05m &&
                        ((checkTotalPercentDiff < param.TotalPercentDiff && checkWeight >= weight) || param.TrackbackPercentDiff == 0))
                    {
                        //param.LowestExchange = param.LowestExchange;
                        //param.HighestExchange = param.HighestExchange;
                        //param.PrimaryPercentDiff = checkPrimaryPercentDiff;

                        param.TrackbackToCoin = lowestTrackBackCoin;
                        param.TrackbackFromCoin = highestTrackBackCoin;
                        param.TrackbackPercentDiff = checkTrackBackPercentDiff;

                        param.TotalPercentDiff = checkTotalPercentDiff;

                        weight = checkWeight;
                    }
                }
            }
        }

        public void UpdateStatsAfterDirectMove(ExchangePairParam param)
        {
            PricingService.Instance.UpdateExchangeCoinPrices(
                param.Exchanges,
                new List<string> { param.LowestExchange.Name, param.HighestExchange.Name },
                null);

            param.ExchangeCurrentlyHoldingFunds = param.HighestExchange.Name;
            //var primaryFromValue = param.Exchanges
            //    .First(m => m.Name == param.LowestExchange.Name).Coins
            //    .First(m => m.Symbol == param.PrimaryCoin).Coins
            //    .First(m => param.ValueCoin.Contains(m.Symbol)).Price;
            //var primaryToValue = param.Exchanges
            //    .First(m => m.Name == param.HighestExchange.Name).Coins
            //    .First(m => m.Symbol == param.PrimaryCoin).Coins
            //    .First(m => param.ValueCoin.Contains(m.Symbol)).Price;
            //param.PrimaryPercentDiff = _mathService.PercentDiff(primaryFromValue, primaryToValue);
            //if (param.TrackbackPercentDiff != 0)
            //{
            //    param.TotalPercentDiff = param.PrimaryPercentDiff + param.TrackbackPercentDiff;
            //}
        }

        public void UpdateStatsAfterTrackback(ExchangePairParam param)
        {
            PricingService.Instance.UpdateExchangeCoinPrices(
                param.Exchanges,
                new List<string> { param.LowestExchange.Name, param.HighestExchange.Name },
                new List<string> { param.PrimaryCoin, param.TrackbackFromCoin.Symbol });

            param.ExchangeCurrentlyHoldingFunds = param.LowestExchange.Name;
            var trackbackFromValue = param.Exchanges.First(m => m.Name == param.HighestExchange.Name).Coins.First(m => m.Symbol == param.TrackbackFromCoin.Symbol).Coins.First(m => m.Symbol == param.PrimaryCoin).Price;
            var trackbackToValue = param.Exchanges.First(m => m.Name == param.LowestExchange.Name).Coins.First(m => m.Symbol == param.TrackbackToCoin.Symbol).Coins.First(m => m.Symbol == param.PrimaryCoin).Price;
            param.TrackbackPercentDiff = _mathService.PercentDiff(trackbackFromValue, trackbackToValue);
            if (param.TrackbackPercentDiff != 0)
            {
                param.TotalPercentDiff = param.PrimaryPercentDiff + param.TrackbackPercentDiff;
            }
        }
    }
}
