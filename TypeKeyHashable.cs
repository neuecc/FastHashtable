using System;

namespace FastHashtable
{
    public sealed class TypeKeyHashable<TValue> : ThreadSafeHashtable<Type, TValue>
    {
        public TypeKeyHashable(int capacity = 4, float loadFactor = 0.75F)
            : base(capacity, loadFactor)
        {
        }

        public override int KeyGetHashCode(Type key)
        {
            if (key == null) return 0;
            return key.GetHashCode();
        }

        public override bool KeyEquals(Type key1, Type key2)
        {
            return key1 == key2;
        }
    }
}
