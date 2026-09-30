using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using System.Text;

namespace LubaEngine.Types
{
    public readonly struct SBVector2
    {
        public const int Shift = 8;
        public const int One = 1 << Shift;  
        public readonly int x, y;         

        public SBVector2(int subX, int subY) { x = subX; y = subY; }

        public static SBVector2 FromPixels(int px, int py) => new(px << Shift, py << Shift);

        public int PixelX => x >> Shift;
        public int PixelY => y >> Shift;

        public int SubX => x & (One - 1);
        public int SubY => y & (One - 1);

        public static SBVector2 operator +(SBVector2 a, SBVector2 b) => new(a.x + b.x, a.y + b.y);
        public static SBVector2 operator -(SBVector2 a, SBVector2 b) => new(a.x - b.x, a.y - b.y);
        public static SBVector2 operator -(SBVector2 a) => new(-a.x, -a.y);
        public static SBVector2 operator *(SBVector2 a, int s) => new(a.x * s, a.y * s);

        public Vector2 ToPixelVector() => new(PixelX, PixelY);

        public override string ToString() => $"({PixelX}.{SubX:X2}, {PixelY}.{SubY:X2})";
    }
}
