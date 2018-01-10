using System.Collections;
using System.Collections.Generic;

namespace CryptoKeeper.Domain.DataObjects.Dtos.CryptoCompare
{
    public class HistoMinuteList : IList<HistoMinuteItem>
    {
        public HistoMinuteList(HistoMinuteDto histoMinuteDto)
        {
            Data = histoMinuteDto.Data;
        }

        public List<HistoMinuteItem> Data { get; set; }
        
        public IEnumerator<HistoMinuteItem> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(HistoMinuteItem item)
        {
            Data.Add(item);
        }

        public void Clear()
        {
            Data.Clear();
        }

        public bool Contains(HistoMinuteItem item)
        {
            return Data.Contains(item);
        }

        public void CopyTo(HistoMinuteItem[] array, int arrayIndex)
        {
            Data.CopyTo(array, arrayIndex);
        }

        public bool Remove(HistoMinuteItem item)
        {
            return Data.Remove(item);
        }

        public int Count => Data.Count;
        public bool IsReadOnly { get; }
        public int IndexOf(HistoMinuteItem item)
        {
            return Data.IndexOf(item);
        }

        public void Insert(int index, HistoMinuteItem item)
        {
            Data.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            Data.RemoveAt(index);
        }

        public HistoMinuteItem this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        public void AddRange(IEnumerable<HistoMinuteItem> collection)
        {
            Data.AddRange(collection);
        }
        public bool IsPriming { get; set; }
    }
}
