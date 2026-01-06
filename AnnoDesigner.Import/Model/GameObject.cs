using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
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
            int rotation = Round(Direction * 180 / Math.PI);
            result.Color = Color ?? ColorPresetsHelper.Instance.GetPredefinedColor(result) ?? Colors.Red;
            result.Direction = (GridDirection)(Round(Direction / Math.PI * 2) % 4);

            if (rotation % 90 != 0)
            {
                Debug.WriteLine("Diagonal building"); // TODO
            }

            double x = Position.X;
            double y = Position.Y;

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

            result.Label = Label;
            return result;
        }
    }
}
