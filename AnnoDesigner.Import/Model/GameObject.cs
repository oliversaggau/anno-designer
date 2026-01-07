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

        public GameObject(BuildingInfo template, double direction, Point2D<float> position)
            : base(template, position)
        {
            this.BuildBlocker = new Rectangle<double>(template.BuildBlocker[X], template.BuildBlocker[Y], template.BuildBlocker[Width], template.BuildBlocker[Height]);
            this.Direction = direction;
        }

        public Rectangle<double> BuildBlocker { get; }

        public double Direction { get; }

        public override AnnoObject CreateObject()
        {
            AnnoObject result = Template.ToAnnoObject();
            int rotationDegrees = Round(Direction * 180 / Math.PI);
            result.Color = Color ?? ColorPresetsHelper.Instance.GetPredefinedColor(result) ?? Colors.Red;
            result.Direction = (GridDirection)(Round(Direction / Math.PI * 2) % 4);
            result.Rotation = Direction;

            // the position in-game is cx/cy, but Anno-Designer uses top-left coordinates,
            // so we need to convert all coordinates below
            double x = Position.X;
            double y = Position.Y;

            // to convert the center to the top-left corner we can't just use the width/height div 2
            // because some buildings are off-center, so instead we need to use the BuildBlocker X/Y
            // values, which are the offsets between the center and the top-left corner the game told
            // us and because we need to use the same offset while drawing (because we need to rotate
            // around the center), we need to store the center point for later use by Anno-Designer
            result.RotationCenter = new Point(-BuildBlocker.X, -BuildBlocker.Y); // do NOT round here

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
                switch (result.Direction)
                {
                    case GridDirection.Up: // 0 degrees
                        x -= BuildBlocker.X + BuildBlocker.Width;
                        y -= BuildBlocker.Y + BuildBlocker.Height;
                        break;
                    case GridDirection.Right: // 90 degrees
                        x -= BuildBlocker.Y + BuildBlocker.Height;
                        y -= BuildBlocker.X + BuildBlocker.Width;
                        break;
                    case GridDirection.Down: // 180 degrees
                        x -= -BuildBlocker.X;
                        y -= -BuildBlocker.Y;
                        break;
                    case GridDirection.Left: // 270 degrees
                        x -= -BuildBlocker.Y;
                        y -= -BuildBlocker.X;
                        break;
                }

                result.Position = new Point(Round(x), Round(y));

                switch (result.Direction)
                {
                    case GridDirection.Left:
                    case GridDirection.Right:
                        result.Size = new Size((int)BuildBlocker.Height, (int)BuildBlocker.Width); // rotated (flip width and height)
                        break;
                    case GridDirection.Up:
                    case GridDirection.Down:
                        result.Size = new Size((int)BuildBlocker.Width, (int)BuildBlocker.Height); // normal
                        break;
                }
            }

            result.Label = Label;
            return result;
        }
    }
}
