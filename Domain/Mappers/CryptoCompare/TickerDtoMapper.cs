using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.Mappers.Interfaces;

namespace CryptoKeeper.Domain.Mappers.CryptoCompare
{
    public class TickerDtoMapper : IUpdateMapper<SocketDataWrapperDto, TickerDto>
    {
        public void Update(SocketDataWrapperDto sourceType, TickerDto updateType)
        {
            var data = sourceType.Data;
            if (data != null && data.Any() && updateType != null)
            {
                var length = data.Length;
                var mask = data[length - 1];
                var maskInt = int.Parse(mask, NumberStyles.HexNumber);
                var currentField = 0;
                var currentFieldOfPreviousData = 0;
                foreach (var key in _fields.Keys)
                {
                    if (currentField == length - 1) break;
                    var myType = typeof(TickerDto);
                    var myPropInfo = myType.GetProperty(key);
                    if (_fields[key] == 0)
                    {
                        myPropInfo.SetValue(updateType, data[currentField], null);
                        currentField++;
                    }
                    else if (Convert.ToBoolean(maskInt & _fields[key]))
                    {
                        if (key == "LastUpdate")
                        {
                            myPropInfo.SetValue(updateType, long.Parse(data[currentField]), null);
                        }
                        else
                        {
                            myPropInfo.SetValue(updateType, decimal.Parse(data[currentField], NumberStyles.Float), null);
                        }
                        if (sourceType.PreviousData != null &&
                            sourceType.PreviousData.Length >= currentFieldOfPreviousData + 1)
                        {
                            sourceType.PreviousData[currentFieldOfPreviousData] = data[currentField];
                        }
                        currentField++;
                    }
                    else // for fields that are not in the update, pull from previous data
                    {
                        if (sourceType.PreviousData != null && sourceType.PreviousData.Length >= currentFieldOfPreviousData + 1)
                        {
                            if (key == "LastUpdate")
                            {
                                myPropInfo.SetValue(updateType, long.Parse(sourceType.PreviousData[currentFieldOfPreviousData]), null);
                            }
                            else
                            {
                                myPropInfo.SetValue(updateType, decimal.Parse(sourceType.PreviousData[currentFieldOfPreviousData], NumberStyles.Float), null);
                            }
                        }
                    }
                    currentFieldOfPreviousData++;
                }
                //File.AppendAllLines(@"C:\temp\socketdata.txt", new List<string> { JsonConvert.SerializeObject(updateType) }, Encoding.UTF8);
            }
        }
        
        private Dictionary<string, int> _fields = new Dictionary<string, int>
        {
            {"MessageType",0x0},      // hex for binary 0, it is a special case of fields that are always there
            {"Market", 0x0},          // hex for binary 0, it is a special case of fields that are always there
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
