using System;
using System.Collections.Generic;
using System.Text;

namespace LubaEngine
{
    public static class Utils {
        public static class Text {
            public static string ToHexBytesNoSpaces(uint v) =>
             $"{(v >> 24) & 0xFF:X2}{(v >> 16) & 0xFF:X2}{(v >> 8) & 0xFF:X2}{v & 0xFF:X2}";
            public static string ToHexBytes(uint v) =>
                $"{(v >> 24) & 0xFF:X2} {(v >> 16) & 0xFF:X2} {(v >> 8) & 0xFF:X2} {v & 0xFF:X2}";
        }
    }
}
