using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers.CryptoCompare
{
    /// Pretty sure that not all the these fields are correct. 
    /// Not really sure I have the bitwise operator stuff correct. 
    /// Attempted to mimic the ticker logic in CryptoCompares 
    /// example project on GitHub. Fields don't always line up right.
    public class TickerDtoMapper : IUpdateMapper<string[], TickerDto>
    {
        public void Update(string[] sourceType, TickerDto updateType)
        {
            if (sourceType != null && sourceType.Any() && updateType != null)
            {
                try
                {
                    var length = sourceType.Length;
                    var mask = sourceType[length - 1];
                    var maskInt = int.Parse(mask, NumberStyles.HexNumber);
                    var currentField = 0;
                    foreach (var key in _fields.Keys)
                    {
                        if (currentField == length - 1) break;
                        if (_fields[key] == 0)
                        {
                            var myType = typeof(TickerDto);
                            var myPropInfo = myType.GetProperty(key);
                            myPropInfo.SetValue(updateType, sourceType[currentField], null);
                        }
                        else if (Convert.ToBoolean(maskInt & _fields[key]))
                        {
                            var myType = typeof(TickerDto);
                            var myPropInfo = myType.GetProperty(key);
                            myPropInfo.SetValue(updateType, decimal.Parse(sourceType[currentField], NumberStyles.Float), null);
                        }
                        else
                        {
                            //Not really sure what to do here.
                        }
                        currentField++;
                    }
                }
                catch (Exception ex)
                {
                    //Not really sure what to do here.
                }
                //File.AppendAllLines(@"C:\temp\socketdata.txt", new List<string> { JsonConvert.SerializeObject(updateType) }, Encoding.UTF8);
            }
        }
        
        private Dictionary<string, int> _fields = new Dictionary<string, int>
        {
            {"MessageType",00},             // hex for binary 0, it is a special case of fields that are always there
            {"Market", 0x0},           // hex for binary 0, it is a special case of fields that are always there
            {"FromSymbol",0x0},       // hex for binary 0, it is a special case of fields that are always there
            {"ToSymbol",0x0},         // hex for binary 0, it is a special case of fields that are always there
            {"Flags",0x0},            // hex for binary 0, it is a special case of fields that are always there
            {"Price",0x1},            // hex for binary 1
            {"Bid",0x2},              // hex for binary 10
            {"Offer",0x4},            // hex for binary 100
            {"LastUpdate",0x8},       // hex for binary 1000
            {"Avg",0x10},             // hex for binary 10000
            {"LastVolume",0x20},      // hex for binary 100000
            {"LastVolumeTo",0x40},    // hex for binary 1000000
            {"LastTradeId",0x80},     // hex for binary 10000000
            {"VolumeHour",0x100},     // hex for binary 100000000
            {"VolumeHourTo",0x200},   // hex for binary 1000000000
            {"Volume24Hour",0x400},   // hex for binary 10000000000
            {"Volume24HourTo",0x800}, // hex for binary 100000000000
            {"OpenHour",0x1000},      // hex for binary 1000000000000
            {"HighHour",0x2000},      // hex for binary 10000000000000
            {"LowHour",0x4000},       // hex for binary 100000000000000
            {"Open24Hour",0x8000},    // hex for binary 1000000000000000
            {"High24Hour",0x10000},   // hex for binary 10000000000000000
            {"Low24Hour",0x20000},    // hex for binary 100000000000000000
            {"LastMarket",0x40000}    // hex for binary 1000000000000000000, this is a special case and will only appear on CCCAGG messages
        };
    }
}
