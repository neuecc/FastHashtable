using System;
using System.Collections.Generic;

namespace FastHashtable
{
    public sealed class ByteArrayHashtable<TValue> : ThreadSafeFixedSizeHashtable<byte[], ArraySegment<byte>, TValue>
    {
        public ByteArrayHashtable(IEnumerable<KeyValuePair<byte[], TValue>> values) : base(values)
        {
        }

        public ByteArrayHashtable(IEnumerable<KeyValuePair<byte[], TValue>> values, float loadFactor) : base(values, loadFactor)
        {
        }

        public override int GetHashCode(byte[] key)
        {
            if (key == null || key.Length == 0) return 0;
            return ByteArrayComparer.GetHashCode(key);
        }

        public override int GetHashCode(ArraySegment<byte> key)
        {
            if (key.Array == null || key.Count == 0) return 0;
            return ByteArrayComparer.GetHashCode(key.Array, key.Offset, key.Count);
        }

        public override bool KeyEquals(ArraySegment<byte> key1, byte[] key2)
        {
            if (key1.Array == null && key2 == null) return true;
            if (key1.Array == null || key2 == null) return false;
            return ByteArrayComparer.Equals(key1.Array, key1.Offset, key1.Count, key2, 0, key2.Length);
        }
    }
}
