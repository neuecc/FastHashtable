using System;
using System.Linq;
using System.Collections.Generic;

namespace FastHashtable
{
    public abstract class ThreadSafeFixedSizeHashtable<TKey, TValue>
    {
        readonly Entry[][] buckets; // immutable array(faster than linkedlist)
        readonly int indexFor;

        public ThreadSafeFixedSizeHashtable(IEnumerable<KeyValuePair<TKey, TValue>> values)
            : this(values, 0.75f)
        {
        }

        public ThreadSafeFixedSizeHashtable(IEnumerable<KeyValuePair<TKey, TValue>> values, float loadFactor)
        {
            var array = values.ToArray();

            var tableSize = CalculateCapacity(array.Length, loadFactor);
            this.buckets = new Entry[tableSize][];
            this.indexFor = buckets.Length - 1;

            foreach (var item in array)
            {
                if (!TryAddInternal(item.Key, item.Value))
                {
                    throw new ArgumentException("Key was already exists. Key:" + item.Key);
                }
            }
        }

        public abstract int KeyGetHashCode(TKey key);
        public abstract bool KeyEquals(TKey key1, TKey key2);

        bool TryAddInternal(TKey key, TValue value)
        {
            var h = KeyGetHashCode(key);
            var entry = new Entry { Key = key, Value = value };

            var array = buckets[h & (indexFor)];
            if (array == null)
            {
                buckets[h & (indexFor)] = new[] { entry };
            }
            else
            {
                // check duplicate
                for (int i = 0; i < array.Length; i++)
                {
                    var e = array[i].Key;
                    if (Equals(key, e))
                    {
                        return false;
                    }
                }

                var newArray = new Entry[array.Length + 1];
                Array.Copy(array, newArray, array.Length);
                array = newArray;
                array[array.Length - 1] = entry;
                buckets[h & (indexFor)] = array;
            }

            return true;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var table = buckets;
            var hash = KeyGetHashCode(key);
            var entry = table[hash & indexFor];

            if (entry == null) goto NOT_FOUND;

            {
                var v = entry[0];
                if (KeyEquals(key, v.Key))
                {
                    value = v.Value;
                    return true;
                }
            }

            for (int i = 1; i < entry.Length; i++)
            {
                var v = entry[i];
                if (KeyEquals(key, v.Key))
                {
                    value = v.Value;
                    return true;
                }
            }

            NOT_FOUND:
            value = default(TValue);
            return false;
        }

        static int CalculateCapacity(int collectionSize, float loadFactor)
        {
            var size = (int)(((float)collectionSize) / loadFactor);

            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            size += 1;

            if (size < 8)
            {
                size = 8;
            }
            return size;
        }

        struct Entry
        {
            public TKey Key;
            public TValue Value;

            // for debugging
            public override string ToString()
            {
                return "(" + Key + ", " + Value + ")";
            }
        }
    }

    public abstract class ThreadSafeFixedSizeHashtable<TKeyForSave, TKeyForGet, TValue>
    {
        readonly Entry[][] buckets; // immutable array(faster than linkedlist)
        readonly int indexFor;

        public ThreadSafeFixedSizeHashtable(IEnumerable<KeyValuePair<TKeyForSave, TValue>> values)
            : this(values, 0.75f)
        {
        }

        public ThreadSafeFixedSizeHashtable(IEnumerable<KeyValuePair<TKeyForSave, TValue>> values, float loadFactor)
        {
            var array = values.ToArray();

            var tableSize = CalculateCapacity(array.Length, loadFactor);
            this.buckets = new Entry[tableSize][];
            this.indexFor = buckets.Length - 1;

            foreach (var item in array)
            {
                if (!TryAddInternal(item.Key, item.Value))
                {
                    throw new ArgumentException("Key was already exists. Key:" + item.Key);
                }
            }
        }

        public abstract int GetHashCode(TKeyForSave key);
        public abstract int GetHashCode(TKeyForGet key);
        public abstract bool KeyEquals(TKeyForGet key1, TKeyForSave key2);

        bool TryAddInternal(TKeyForSave key, TValue value)
        {
            var h = GetHashCode(key);
            var entry = new Entry { Key = key, Value = value };

            var array = buckets[h & (indexFor)];
            if (array == null)
            {
                buckets[h & (indexFor)] = new[] { entry };
            }
            else
            {
                // check duplicate
                for (int i = 0; i < array.Length; i++)
                {
                    var e = array[i].Key;
                    if (Equals(key, e))
                    {
                        return false;
                    }
                }

                var newArray = new Entry[array.Length + 1];
                Array.Copy(array, newArray, array.Length);
                array = newArray;
                array[array.Length - 1] = entry;
                buckets[h & (indexFor)] = array;
            }

            return true;
        }

        public bool TryGetValue(TKeyForGet key, out TValue value)
        {
            var table = buckets;
            var hash = GetHashCode(key);
            var entry = table[hash & indexFor];

            if (entry == null) goto NOT_FOUND;

            {
                var v = entry[0];
                if (KeyEquals(key, v.Key))
                {
                    value = v.Value;
                    return true;
                }
            }

            for (int i = 1; i < entry.Length; i++)
            {
                var v = entry[i];
                if (KeyEquals(key, v.Key))
                {
                    value = v.Value;
                    return true;
                }
            }

            NOT_FOUND:
            value = default(TValue);
            return false;
        }

        static int CalculateCapacity(int collectionSize, float loadFactor)
        {
            var size = (int)(((float)collectionSize) / loadFactor);

            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            size += 1;

            if (size < 8)
            {
                size = 8;
            }
            return size;
        }

        struct Entry
        {
            public TKeyForSave Key;
            public TValue Value;

            // for debugging
            public override string ToString()
            {
                return "(" + Key + ", " + Value + ")";
            }
        }
    }
}