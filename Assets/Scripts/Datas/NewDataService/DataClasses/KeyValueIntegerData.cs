using System;

namespace Mathy.Services.Data
{
    public class KeyValueIntegerData
    {
        public KeyValuePairKeys Key { get; set; } = KeyValuePairKeys.none;
        public int Value { get; set; } = 0;
        public DateTime Date { get; set; }
    }
}
