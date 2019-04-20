using System;
using System.Globalization;
using System.Linq;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers
{
    public class PricingItemMapper : 
        IUpdateMapper<DataObjects.Dtos.CryptoCompare.HistoMinuteItem, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.CryptoCompare.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Coinbase.TickerChannelDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.BitTrex.MarketSummaryDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.HitBtc.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.WavesDex.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Abucoins.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Binance.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.BitBay.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.BitMarket.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Bitstamp.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Bleutrade.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.BXinth.MarketDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Exmo.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.HuobiPro.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.TrustDex.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Exx.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Gatecoin.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Gemini.TickerDto, PricingItem>,
        IUpdateMapper<DataObjects.Dtos.Poloniex.TickerDto, PricingItem>
    {
        public void Update(DataObjects.Dtos.CryptoCompare.HistoMinuteItem sourceType, PricingItem updateType)
        {
            updateType.Timestamp = sourceType.time;
            updateType.Price = sourceType.close ?? 0m;
        }

        public void Update(DataObjects.Dtos.Coinbase.TickerChannelDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Price = decimal.Parse(sourceType.Price, NumberStyles.Float);
            if (!string.IsNullOrEmpty(sourceType.Best_Ask))
            {
                updateType.Ask = decimal.Parse(sourceType.Best_Ask, NumberStyles.Float);
            }
            if (!string.IsNullOrEmpty(sourceType.Best_Bid))
            {
                updateType.Bid = decimal.Parse(sourceType.Best_Bid, NumberStyles.Float);
            }
            updateType.Volume = decimal.Parse(sourceType.Volume_24h, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.BitTrex.MarketSummaryDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.Parse($"{sourceType.TimeStamp}Z").ToUnixTimeSeconds(); //Had to add Z to the end so that the parse would see time as UTC
            updateType.Price = sourceType.Last;
            updateType.Ask = sourceType.Ask;
            updateType.Bid = sourceType.Bid;
            updateType.Volume = sourceType.Volume;
        }

        public void Update(DataObjects.Dtos.CryptoCompare.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = sourceType.LastUpdate;
            updateType.Price = sourceType.Price;
            updateType.Ask = sourceType.Offer;
            updateType.Bid = sourceType.Bid;
            updateType.Volume = sourceType.Volume24Hour;
        }

        public void Update(DataObjects.Dtos.HitBtc.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.Parse(sourceType.Timestamp).ToUnixTimeSeconds();
            updateType.Price = decimal.Parse(sourceType.Last, NumberStyles.Float);
            updateType.Ask = decimal.Parse(sourceType.Ask, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.Bid, NumberStyles.Float);
            updateType.Volume = decimal.Parse(sourceType.Volume, NumberStyles.Float);
        }

        //TODO
        public void Update(DataObjects.Dtos.WavesDex.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(sourceType.Timestamp).ToUnixTimeSeconds(); //Convert from miliseconds to seconds.
            updateType.Price = (decimal.Parse(sourceType.Open24Hour) + 
                                decimal.Parse(sourceType.High24Hour) + 
                                decimal.Parse(sourceType.Low24Hour) + 
                                decimal.Parse(sourceType.Close24Hour)) / 4m;
            //updateType.Ask = sourceType.;
            //updateType.Bid = sourceType.Bid;
        }

        public void Update(DataObjects.Dtos.Abucoins.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.Parse(sourceType.time).ToUnixTimeSeconds();
            updateType.Price = decimal.Parse(sourceType.price, NumberStyles.Float);
            if (!string.IsNullOrEmpty(sourceType.ask))
            {
                updateType.Ask = decimal.Parse(sourceType.ask, NumberStyles.Float); 
            }
            if (!string.IsNullOrEmpty(sourceType.bid))
            {
                updateType.Bid = decimal.Parse(sourceType.bid, NumberStyles.Float); 
            }
            updateType.Volume = decimal.Parse(sourceType.volume, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.Binance.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(sourceType.EventTime).ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.BestAskPrice, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.BestBidPrice, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.TotalTradedBaseAssetVolume, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.BitBay.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = sourceType.ask;
            updateType.Bid = sourceType.bid;
            updateType.Price = (sourceType.ask + sourceType.bid) / 2;
            updateType.Volume = sourceType.volume;
        }

        public void Update(DataObjects.Dtos.BitMarket.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = sourceType.ask;
            updateType.Bid = sourceType.bid;
            updateType.Price = (sourceType.ask + sourceType.bid) / 2;
            updateType.Volume = sourceType.volume;
        }

        public void Update(DataObjects.Dtos.Bitstamp.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = long.Parse(sourceType.timestamp);
            updateType.Ask = decimal.Parse(sourceType.ask, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.bid, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.volume, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.Bleutrade.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.Ask, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.Bid, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
        }

        public void Update(DataObjects.Dtos.BXinth.MarketDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = sourceType.orderbook.asks.highbid;
            updateType.Bid = sourceType.orderbook.bids.highbid;
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = sourceType.volume_24hours;
        }

        public void Update(DataObjects.Dtos.Exmo.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.sell_price, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.buy_price, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.vol, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.HuobiPro.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = sourceType.ask[0];
            updateType.Bid = sourceType.bid[0];
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = sourceType.vol;
        }

        public void Update(DataObjects.Dtos.TrustDex.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.lowestAsk, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.highestBid, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.baseVolume, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.Exx.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.sell, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.buy, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.vol, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.Gatecoin.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = sourceType.createDateTime;
            updateType.Ask = sourceType.ask;
            updateType.Bid = sourceType.bid;
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = sourceType.volume;
        }

        public void Update(DataObjects.Dtos.Gemini.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(sourceType.volume["timestamp"])).ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.ask, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.bid, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.volume.First().Value, NumberStyles.Float);
        }

        public void Update(DataObjects.Dtos.Poloniex.TickerDto sourceType, PricingItem updateType)
        {
            updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            updateType.Ask = decimal.Parse(sourceType.lowestAsk, NumberStyles.Float);
            updateType.Bid = decimal.Parse(sourceType.highestBid, NumberStyles.Float);
            updateType.Price = (updateType.Ask + updateType.Bid) / 2;
            updateType.Volume = decimal.Parse(sourceType.high24hr, NumberStyles.Float);
        }
    }
}