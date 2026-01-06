using System.Collections.Generic;
using System.Numerics;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Gamedata;

namespace AnnoDesigner.Import.Model
{
    internal class Island
    {
        public Island(string cityName, string template, Point2D<int> position, GridDirection rotation, Size<int> size)
        {
            this.CityName = cityName;
            this.Colors = new Dictionary<long, SerializableColor>();
            this.Objects = new List<AnnoObject>();
            this.Template = template;

            this.Position = position;
            this.Rotation = rotation;
            this.Size = size;
        }

        public string CityName { get; }
        public string Template { get; }

        public Point2D<int> Position { get; }
        public GridDirection Rotation { get; }
        public Size<int> Size { get; }

        public Dictionary<long, SerializableColor> Colors { get; }
        public List<AnnoObject> Objects { get; }

        public Point2D<float> ToLocalCoordinates<T>(Point2D<T> world) where T : struct, INumber<T>
        {
            float X = this.Size.Width - (float.CreateChecked(world.X) - this.Position.X); // flip horizontally
            float Y = float.CreateChecked(world.Y) - this.Position.Y;
            return new Point2D<float>(X, Y);
        }
    }
}
