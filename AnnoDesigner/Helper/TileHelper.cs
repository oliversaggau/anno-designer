using System;
using System.Windows;
using System.Windows.Media;

namespace AnnoDesigner.Helper
{
    internal static class TileHelper
    {
        public static PathGeometry CreateGeometry(Rect rect, byte quadrants)
        {
            var TL = rect.TopLeft;
            var TR = rect.TopRight;
            var BR = rect.BottomRight;
            var BL = rect.BottomLeft;

            if ((quadrants & 0x80) == 0x80)
            {
                // unofficial quadrants not used by the game
                var TC = new Point(rect.X + rect.Width / 2, rect.Y);
                var BC = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height);
                var LC = new Point(rect.X, rect.Y + rect.Height / 2);
                var RC = new Point(rect.X + rect.Width, rect.Y + rect.Height / 2);

                return (quadrants & 0b1111) switch
                {
                    // quad missing two corner triangles (2 bits)
                    0b0011 => CreatePentagon(TL, BL, BC, RC, TC), // Top-Right and Bottom-Right corners missing
                    0b1100 => CreatePentagon(BR, TR, TC, LC, BC), // Top-Left and Bottom-Left corners missing
                    // quad missing a single corner triangle (3 bits)
                    0b1110 => CreatePentagon(TL, LC, BC, BR, TR), // Bottom-Left corner missing
                    0b1101 => CreatePentagon(TL, BL, BC, RC, TR), // Bottom-Right corner missing
                    0b1011 => CreatePentagon(TL, BL, BR, RC, TC), // Top-Right corner missing
                    0b0111 => CreatePentagon(LC, BL, BR, TR, TC), // Top-Left corner missing
                    // combination not allowed
                    _ => throw new NotSupportedException()
                };
            }
            else
            {
                // official quadrants used by the game
                var C = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

                return (quadrants & 0b1111) switch
                {
                    // single sub-tile triangle (1 bit)
                    0b0001 => CreateTriangle(TL, C, BL),
                    0b0010 => CreateTriangle(BL, C, BR),
                    0b0100 => CreateTriangle(BR, C, TR),
                    0b1000 => CreateTriangle(TR, C, TL),
                    // half-quad - diagonal triangle (2 bits)
                    0b0011 => CreateTriangle(TL, BL, BR),
                    0b0110 => CreateTriangle(BL, BR, TR),
                    0b1001 => CreateTriangle(BL, TL, TR),
                    0b1100 => CreateTriangle(TL, TR, BR),
                    // quad missing a single sub-tile triangle (3 bits)
                    0b0111 => CreatePentagon(TL, BL, BR, TR, C),
                    0b1011 => CreatePentagon(TL, BL, BR, C, TR),
                    0b1101 => CreatePentagon(TL, BL, C, BR, TR),
                    0b1110 => CreatePentagon(TL, C, BL, BR, TR),
                    // full quad (all 4 bits)
                    0b1111 => CreateQuad(TL, TR, BR, BL),
                    // combination not allowed
                    _ => throw new NotSupportedException()
                };
            }
        }

        public static bool IsRect(byte quadrants)
        {
            return (quadrants & 0b1111) == 0b1111;
        }

        #region Private Helper Methods

        private static PathGeometry CreateTriangle(Point p1, Point p2, Point p3)
        {
            var figure = CreateFigure(p1, p2, p3);
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        private static PathGeometry CreatePentagon(Point p1, Point p2, Point p3, Point p4, Point p5)
        {
            var figure = CreateFigure(p1, p2, p3, p4, p5);
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        private static PathGeometry CreateQuad(Point p1, Point p2, Point p3, Point p4)
        {
            var figure = CreateFigure(p1, p2, p3, p4);
            var geometry = new PathGeometry();
            geometry.Figures.Add(figure);
            return geometry;
        }

        private static PathFigure CreateFigure(Point p1, params Point[] points)
        {
            var figure = new PathFigure { StartPoint = p1, IsClosed = true };
            foreach (var p in points) figure.Segments.Add(new LineSegment(p, true));
            return figure;
        }

        #endregion
    }
}
