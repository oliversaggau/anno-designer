using System.Windows;
using System.Windows.Media;
using AnnoDesigner.Helper;
using AnnoDesigner.Models;

namespace AnnoDesigner.Extensions
{
    public static class DrawingContextExtensions
    {
        public static void DrawObjectShape(this DrawingContext context, LayoutObject obj, Brush brush, Pen pen, Rect rectangle)
        {
            if (obj.WrappedAnnoObject.TileQuadrants.HasValue)
            {
                var geometry = TileHelper.CreateGeometry(rectangle, obj.WrappedAnnoObject.TileQuadrants.Value);
                if (geometry != null) context.DrawGeometry(brush, pen, geometry);
            }
            else
            {
                context.DrawRectangle(brush, pen, rectangle);
            }
        }

        public static void Push(this DrawingContext context, LayoutObject obj, Point rotationCenter)
        {
            if (obj.IsDiagonal)
            {
                context.PushTransform(new RotateTransform(-obj.RotationDegrees, rotationCenter.X, rotationCenter.Y));
            }
        }

        public static void Pop(this DrawingContext context, LayoutObject obj)
        {
            if (obj.IsDiagonal)
            {
                context.Pop(); // RotateTransform
            }
        }
    }
}
