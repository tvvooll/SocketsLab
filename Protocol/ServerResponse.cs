using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol
{
    [Serializable]
    public class ServerResponse
    {
        public int RightDigits { get; set; }
        public int DigitsInPlaces { get; set; }
        public bool IsSuccessful => DigitsInPlaces == 4;
    }
}
