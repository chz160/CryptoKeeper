using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare;
using CryptoKeeper.Domain.Mappers.Interfaces;
using Newtonsoft.Json;

namespace CryptoKeeper.Domain.Mappers.CryptoCompare
{
    //Pretty sure that not all the these fields are correct. It appears that depending on the Flags column the field order is different.
    public class TickerDtoMapper : IUpdateMapper<string[], TickerDto>
    {
        public void Update(string[] sourceType, TickerDto updateType)
        {
            if (sourceType != null && sourceType.Any() && updateType != null)
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
                        //unpackedTrade[property] = valuesArray[currentField];
                    }
                    else if (Convert.ToBoolean(maskInt & _fields[key]))
                    {
                        var myType = typeof(TickerDto);
                        var myPropInfo = myType.GetProperty(key);
                        myPropInfo.SetValue(updateType, decimal.Parse(sourceType[currentField], NumberStyles.Float), null);
                        //unpackedCurrent[property] = parseFloat(valuesArray[currentField]);
                    }
                    else
                    {

                    }
                    currentField++;
                }

                //File.AppendAllLines(@"C:\temp\socketdata.txt", new List<string> { JsonConvert.SerializeObject(updateType) }, Encoding.UTF8);
                

                //updateType.MessageType = sourceType[0];
                //updateType.Market = sourceType[1];
                //updateType.FromSymbol = sourceType[2];
                //updateType.ToSymbol = sourceType[3];
                //updateType.Flags = sourceType[4];
                ////price data moves depending on field.
                //if (recordType == RecordType.Ce9)
                //{
                //    updateType.Price = decimal.Parse(sourceType[5], NumberStyles.Float); 
                //}
                //else if (recordType == RecordType.Ce8)
                //{
                //    updateType.Price = decimal.Parse(sourceType[6], NumberStyles.Float);
                //}
                //updateType.Bid = sourceType[6];
                //updateType.Offer = sourceType.Length > 7 ? sourceType[7] : null;
                //updateType.LastUpdate = sourceType.Length > 8 ? sourceType[8] : null;
                //updateType.Avg = sourceType.Length > 9 ? sourceType[9] : null;
                //updateType.LastVolume = sourceType.Length > 10 ? sourceType[10] : null;
                //updateType.LastVolumeTo = sourceType.Length > 11 ? sourceType[11] : null;
                //updateType.LastTradeId = sourceType.Length > 12 ? sourceType[12] : null;
                //updateType.VolumeHour = sourceType.Length > 13 ? sourceType[13] : null;
                //updateType.VolumeHourTo = sourceType.Length > 14 ? sourceType[14] : null;
                //updateType.Volume24Hour = sourceType.Length > 15 ? sourceType[15] : null;
                //updateType.Volume24HourTo = sourceType.Length > 16 ? sourceType[16] : null;
                //updateType.OpenHour = sourceType.Length > 17 ? sourceType[17] : null;
                //updateType.HighHour = sourceType.Length > 18 ? sourceType[18] : null;
                //updateType.LowHour = sourceType.Length > 19 ? sourceType[19] : null;
                //updateType.Open24Hour = sourceType.Length > 20 ? sourceType[20] : null;
                //updateType.High24Hour = sourceType.Length > 21 ? sourceType[21] : null;
                //updateType.Low24Hour = sourceType.Length > 22 ? sourceType[22] : null;
                //updateType.LastMarket = sourceType.Length > 23 ? sourceType[23] : null;
                //if (recordType == RecordType.Ce8 || recordType == RecordType.C88)
                //{
                //    updateType.Timestamp = long.Parse(sourceType[5]);
                //}
                //else if (recordType == RecordType.Ce9)
                //{
                //    updateType.Timestamp = long.Parse(sourceType[6]);
                //}
                //else
                //{
                //    updateType.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                //}
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
