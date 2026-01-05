using System;
using System.Numerics;

namespace AnnoDesigner.Gamedata
{
    internal static class SupportExtensions
    {
        internal static Point2D<int> Round<T>(this Point2D<T> input)
            where T : struct, IFloatingPoint<T>
        {
            int X = int.CreateChecked(T.Round(input.X));
            int Y = int.CreateChecked(T.Round(input.Y));
            return new Point2D<int>(X, Y);
        }

        internal static Point2D<int> Round<T>(this Point2D<T> input, MidpointRounding mode)
            where T : struct, IFloatingPoint<T>
        {
            int X = int.CreateChecked(T.Round(input.X, mode));
            int Y = int.CreateChecked(T.Round(input.Y, mode));
            return new Point2D<int>(X, Y);
        }

        internal static Point2D<R> Scale<T, R>(this Point2D<T> input, R factor)
            where T : struct, INumber<T>
            where R : struct, INumber<R>
        {
            R X = R.CreateChecked(input.X) * factor;
            R Y = R.CreateChecked(input.Y) * factor;
            return new Point2D<R>(X, Y);
        }
    }
}
