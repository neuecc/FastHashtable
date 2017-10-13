using System;
using System.Collections.Generic;

namespace FastHashtable
{
    public sealed class StringKeyFixedSizeHashable<TValue> : ThreadSafeFixedSizeHashtable<string, TValue>
    {
        public StringKeyFixedSizeHashable(IEnumerable<KeyValuePair<string, TValue>> values) : base(values)
        {
        }

        public StringKeyFixedSizeHashable(IEnumerable<KeyValuePair<string, TValue>> values, float loadFactor) : base(values, loadFactor)
        {
        }

        public override bool KeyEquals(string key1, string key2)
        {
            return key1 == key2;
        }

        public override int KeyGetHashCode(string key)
        {
            if (key == null) return 0;
            return key.GetHashCode();
        }
    }

    public sealed class StringKeyHashtable<TValue> : ThreadSafeHashtable<string, TValue>
    {
        public StringKeyHashtable(int capacity = 4, float loadFactor = 0.75F)
            : base(capacity, loadFactor)
        {
        }

        public override bool KeyEquals(string key1, string key2)
        {
            return key1 == key2;
        }

        public override int KeyGetHashCode(string key)
        {
            if (key == null) return 0;
            return key.GetHashCode();
        }
    }
}
