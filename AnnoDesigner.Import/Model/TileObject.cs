using System.Windows;
using System.Windows.Media;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Helper;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Gamedata;

namespace AnnoDesigner.Import.Model
{
    internal class TileObject : BaseObject
    {
        /*
         * For example
         * 0x6 (0110) for a ◢ tile
         * 0xC (1100) for a ◥ tile
         * 0x3 (0011) for a ◣ tile
         * 0x9 (1001) for a ◤ tile
         * 0xF (1111) for a ■ tile
         */
        private readonly byte quadrants;

        public TileObject(BuildingInfo template, Point2D<float> position, byte quadrants)
            : this(template, position, 0.0, quadrants)
        {
        }

        protected TileObject(BuildingInfo template, Point2D<float> position, double rotation, byte quadrants)
            : base(template, rotation, position)
        {
            this.quadrants = quadrants;
        }

        public override AnnoObject CreateObject()
        {
            AnnoObject result = BuildTemplate();
            result.Color = Color ?? ColorPresetsHelper.Instance.GetPredefinedColor(result) ?? Colors.Red;
            result.Position = new Point(Round(Position.X), Round(Position.Y));
            result.RotationCenter = new Point(0, 0);
            result.Rotation = Rotation;
            result.TileQuadrants = quadrants;
            result.Size = new Size(1, 1);
            result.Label = Label;
            return result;
        }

        protected virtual AnnoObject BuildTemplate()
        {
            return Template.ToAnnoObject();
        }
    }
}
