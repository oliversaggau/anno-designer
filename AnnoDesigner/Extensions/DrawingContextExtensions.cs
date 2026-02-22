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

        public static void PushTransform(this DrawingContext context, LayoutObject obj, Point rotationCenter)
        {
            context.PushTransform(new RotateTransform(-obj.RotationDegrees, rotationCenter.X, rotationCenter.Y));
        }

        public static void PopTransform(this DrawingContext context, LayoutObject obj)
        {
            context.Pop(); // RotateTransform
        }

        public static void PushTextTransform(this DrawingContext context, LayoutObject obj, ref Rect objRect)
        {
            if (obj.IsDiagonal)
            {
                // for diagonal objects we want icon and text to be rotated, but only ±45° so mirror the others
                if (obj.RotationDegrees != 45 && obj.RotationDegrees != 315)
                {
                    context.PushTransform(new ScaleTransform(-1, -1, objRect.X + objRect.Width / 2, objRect.Y + objRect.Height / 2));
                }
            }
            else
            {
                // for non-diagonal objects we want icon and text to NOT be rotated, so we apply the reverse rotation here (around the actual object center)
                // and we also need to apply the transform to the objRect which is later used for calculating the position/size of the the text
                var transform = new RotateTransform(obj.RotationDegrees, objRect.X + objRect.Width / 2, objRect.Y + objRect.Height / 2);
                objRect = transform.TransformBounds(objRect);
                context.PushTransform(transform);
            }
        }

        public static void PopTextTransform(this DrawingContext context, LayoutObject obj)
        {
            if (obj.IsDiagonal)
            {
                if (obj.RotationDegrees != 45 && obj.RotationDegrees != 315)
                {
                    context.Pop(); // ScaleTransform
                }
            }
            else
            {
                context.Pop(); // RotateTransform
            }
        }
    }
}
