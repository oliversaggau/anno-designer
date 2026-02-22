using System;
using System.Windows;
using System.Windows.Media;
using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Helper;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Gamedata;

namespace AnnoDesigner.Import.Model
{
    internal class GameObject : BaseObject
    {
        private const string X = "x0";
        private const string Y = "z0";
        private const string Width = "x";
        private const string Height = "z";

        public GameObject(BuildingInfo template, double rotation, Point2D<float> position)
            : base(template, rotation, position)
        {
            this.BuildBlocker = new Rectangle<double>(template.BuildBlocker[X], template.BuildBlocker[Y], template.BuildBlocker[Width], template.BuildBlocker[Height]);
        }

        public Rectangle<double> BuildBlocker { get; }

        public override AnnoObject CreateObject()
        {
            AnnoObject result = Template.ToAnnoObject();
            int rotationDegrees = Round(Rotation * 180 / Math.PI);
            result.Color = Color ?? ColorPresetsHelper.Instance.GetPredefinedColor(result) ?? Colors.Red;
            result.Direction = (GridDirection)(Round(Rotation / Math.PI * 2) % 4);
            result.Rotation = Rotation;

            // the position in-game is cx/cy, but Anno-Designer uses top-left coordinates,
            // so we need to convert all coordinates below
            double x = Position.X;
            double y = Position.Y;

            // to convert the center to the top-left corner we can't just use the width/height div 2
            // because some buildings are off-center, so instead we need to use the BuildBlocker X/Y
            // values, which are the offsets between the center and the top-left corner the game told
            // us and because we need to use the same offset while drawing (because we need to rotate
            // around the center), we need to store the center point for later use by Anno-Designer
            result.RotationCenter = new Point(BuildBlocker.Width + BuildBlocker.X, -BuildBlocker.Y); // do NOT round here

            if (rotationDegrees % 90 != 0)
            {
                // in order to calculate the offsets for diagonal rotations, we first need to calculate
                // the scaled size in the diagonal grid because we need to scale everything accordingly
                double scaleX = MathHelper.GetDiagonalSize(BuildBlocker.Width) / BuildBlocker.Width;
                double scaleY = MathHelper.GetDiagonalSize(BuildBlocker.Height) / BuildBlocker.Height;

                x -= result.RotationCenter.X * scaleX;
                y -= result.RotationCenter.Y * scaleY;
                result.Position = new Point(x, y); // do NOT round here

                // leave the size as-is (in orthogonal units), in case the building is later rotated in
                // Anno-Designer (scaled width/height will be calculated again by LayoutObject if needed)
                result.Size = new Size((int)BuildBlocker.Width, (int)BuildBlocker.Height);
            }
            else
            {
                x -= result.RotationCenter.X;
                y -= result.RotationCenter.Y;
                result.Position = new Point(x, y); // do NOT round here
                result.Size = new Size((int)BuildBlocker.Width, (int)BuildBlocker.Height);
            }

            result.Label = Label;
            return result;
        }
    }
}
