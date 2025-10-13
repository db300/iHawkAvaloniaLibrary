using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iHawkSkiaSharpCommonLibrary
{
    internal class Common
    {
        internal static uint ReadUInt32(ReadOnlySpan<byte> span, ref int offset)
        {
            var v = BinaryPrimitives.ReadUInt32BigEndian(span.Slice(offset, 4));
            offset += 4;
            return v;
        }

        internal static ushort ReadUInt16(ReadOnlySpan<byte> span, ref int offset)
        {
            var v = BinaryPrimitives.ReadUInt16BigEndian(span.Slice(offset, 2));
            offset += 2;
            return v;
        }

        internal static short ReadInt16(ReadOnlySpan<byte> span, ref int offset)
        {
            var v = BinaryPrimitives.ReadInt16BigEndian(span.Slice(offset, 2));
            offset += 2;
            return v;
        }

        internal static long ReadInt64(ReadOnlySpan<byte> span, ref int offset)
        {
            var v = BinaryPrimitives.ReadInt64BigEndian(span.Slice(offset, 8));
            offset += 8;
            return v;
        }
    }
}
