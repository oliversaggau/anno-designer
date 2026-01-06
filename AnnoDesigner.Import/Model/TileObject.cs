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
        public TileObject(BuildingInfo template, Point2D<float> position)
            : base(template, position)
        {
        }

        public override AnnoObject CreateObject()
        {
            AnnoObject result = BuildTemplate();
            result.Color = Color ?? ColorPresetsHelper.Instance.GetPredefinedColor(result) ?? Colors.Red;
            result.Position = new Point(Round(Position.X), Round(Position.Y));
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
