using System;
using System.Collections.Generic;
using System.Text;

namespace CryptoKeeper.Domain.DataObjects.Dtos.Coinroom
{
    public class ResponseDto
    {
        public List<string> real { get; set; }
        public List<string> crypto { get; set; }
    }
}
