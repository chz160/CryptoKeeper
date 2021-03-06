﻿using System.Collections.Generic;
using CryptoKeeper.Domain.DataObjects.Dtos;
using CryptoKeeper.Domain.Mappers.Interfaces;
using CryptoKeeper.Entities.Pricing.Models;

namespace CryptoKeeper.Domain.Mappers
{
    public class WithdrawalFeeMapper : 
        IUpdateMapper<DataObjects.Dtos.BitTrex.CurrencyDto, WithdrawalFee>,
        IUpdateMapper<KeyValuePair<string, DataObjects.Dtos.Poloniex.CurrencyDto>, WithdrawalFee>,
        IUpdateMapper<DataObjects.Dtos.Bleutrade.CurrencyDto, WithdrawalFee>
    {
        public void Update(DataObjects.Dtos.BitTrex.CurrencyDto sourceType, WithdrawalFee updateType)
        {
            if (sourceType != null && updateType != null)
            {
                updateType.Symbol = sourceType.Currency;
                updateType.Fee = sourceType.TxFee;
            }
        }

        public void Update(KeyValuePair<string, DataObjects.Dtos.Poloniex.CurrencyDto> sourceType, WithdrawalFee updateType)
        {
            if (sourceType.Value != null && updateType != null)
            {
                updateType.Symbol = sourceType.Key;
                updateType.Fee = decimal.Parse(sourceType.Value.TxFee);
            }
        }

        public void Update(DataObjects.Dtos.Bleutrade.CurrencyDto sourceType, WithdrawalFee updateType)
        {
            if (sourceType != null && updateType != null)
            {
                updateType.Symbol = sourceType.Currency;
                updateType.Fee = decimal.Parse(sourceType.TxFee);
            }
        }
    }
}