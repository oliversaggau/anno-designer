using System.Windows;
using System.Windows.Media;
using AnnoDesigner.Models;

namespace AnnoDesigner.Extensions
{
    public static class DrawingContextExtensions
    {
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
