using System;
using System.Collections.Generic;

namespace FastHashtable
{
    public static class ByteArrayComparer
    {
        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(byte[] bytes)
        {
            return GetHashCode(bytes, 0, bytes.Length);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static int GetHashCode(byte[] bytes, int offset, int count)
        {
            if (IntPtr.Size == 4)
            {
                return unchecked((int)FarmHash.Hash32(bytes, offset, count));
            }
            else
            {
                return unchecked((int)FarmHash.Hash64(bytes, offset, count));
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals(byte[] xs, byte[] ys)
        {
            return Equals(xs, 0, xs.Length, ys, 0, ys.Length);
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals(byte[] xs, int xsOffset, int xsCount, byte[] ys, int ysOffset, int ysCount)
        {
            if (IntPtr.Size == 4)
            {
                return Equals32(xs, xsOffset, xsCount, ys, ysOffset, ysCount);
            }
            else
            {
                return Equals64(xs, xsOffset, xsCount, ys, ysOffset, ysCount);
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals32(byte[] xs, int xsOffset, int xsCount, byte[] ys, int ysOffset, int ysCount)
        {
            if (xs == null && ys == null) return true;
            if (xs == null || ys == null || xsCount != ysCount)
            {
                return false;
            }
            if (xsCount == 0 && ysCount == 0) return true;

            fixed (byte* p1 = &xs[xsOffset])
            fixed (byte* p2 = &ys[ysOffset])
            {
                switch (xsCount)
                {
                    case 0:
                        return true;
                    case 1:
                        return *p1 == *p2;
                    case 2:
                        return *(ushort*)p1 == *(ushort*)p2;
                    case 3:
                        if (*(byte*)p1 != *(byte*)p2) return false;
                        return *(ushort*)(p1 + 1) == *(ushort*)(p2 + 1);
                    case 4:
                        return *(uint*)p1 == *(uint*)p2;
                    default:
                        {
                            var x1 = p1;
                            var x2 = p2;

                            byte* xEnd = p1 + xsCount - 4;
                            byte* yEnd = p2 + ysCount - 4;

                            while (x1 < xEnd)
                            {
                                if (*(uint*)x1 != *(uint*)x2)
                                {
                                    return false;
                                }

                                x1 += 8;
                                x2 += 8;
                            }

                            return *(uint*)xEnd == *(uint*)yEnd;
                        }
                }
            }
        }

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals64(byte[] xs, int xsOffset, int xsCount, byte[] ys, int ysOffset, int ysCount)
        {
            if (xs == null && ys == null) return true;
            if (xs == null || ys == null || xsCount != ysCount)
            {
                return false;
            }

            fixed (byte* p1 = &xs[xsOffset])
            fixed (byte* p2 = &ys[ysOffset])
            {
                switch (xsCount)
                {
                    case 0:
                        return true;
                    case 1:
                        return *p1 == *p2;
                    case 2:
                        return *(ushort*)p1 == *(ushort*)p2;
                    case 3:
                        if (*(byte*)p1 != *(byte*)p2) return false;
                        return *(ushort*)(p1 + 1) == *(ushort*)(p2 + 1);
                    case 4:
                        return *(uint*)p1 == *(uint*)p2;
                    case 5:
                        if (*(byte*)p1 != *(byte*)p2) return false;
                        return *(uint*)(p1 + 1) == *(uint*)(p2 + 1);
                    case 6:
                        if (*(ushort*)p1 != *(ushort*)p2) return false;
                        return *(uint*)(p1 + 2) == *(uint*)(p2 + 2);
                    case 7:
                        if (*(byte*)p1 != *(byte*)p2) return false;
                        if (*(short*)(p1 + 1) != *(short*)(p2 + 1)) return false;
                        return *(uint*)(p1 + 3) == *(uint*)(p2 + 3);
                    case 8:
                        return *(ulong*)p1 == *(ulong*)p2;
                    default:
                        {
                            var x1 = p1;
                            var x2 = p2;

                            byte* xEnd = p1 + xsCount - 8;
                            byte* yEnd = p2 + ysCount - 8;

                            while (x1 < xEnd)
                            {
                                if (*(ulong*)x1 != *(ulong*)x2)
                                {
                                    return false;
                                }

                                x1 += 8;
                                x2 += 8;
                            }

                            return *(ulong*)xEnd == *(ulong*)yEnd;
                        }
                }
            }
        }
    }

    public class ByteArrayEqualityComaprer : IEqualityComparer<byte[]>
    {
        public static readonly IEqualityComparer<byte[]> Default = new ByteArrayEqualityComaprer();

        public bool Equals(byte[] x, byte[] y)
        {
            return ByteArrayComparer.Equals(x, y);
        }

        public int GetHashCode(byte[] obj)
        {
            return ByteArrayComparer.GetHashCode(obj);
        }
    }

    public class ByteArraySegmentEqualityComaprer : IEqualityComparer<ArraySegment<byte>>
    {
        public static readonly IEqualityComparer<byte[]> Default = new ByteArrayEqualityComaprer();

        public bool Equals(ArraySegment<byte> x, ArraySegment<byte> y)
        {
            return ByteArrayComparer.Equals(x.Array, x.Offset, x.Count, y.Array, y.Offset, y.Count);
        }

        public int GetHashCode(ArraySegment<byte> obj)
        {
            return ByteArrayComparer.GetHashCode(obj.Array, obj.Offset, obj.Count);
        }
    }
}
