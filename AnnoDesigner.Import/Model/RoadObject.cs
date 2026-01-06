using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Gamedata;

namespace AnnoDesigner.Import.Model
{
    internal class RoadObject : TileObject
    {
        public RoadObject(BuildingInfo template, Point2D<float> position)
            : base(template, position)
        {
        }

        public override AnnoObject CreateObject()
        {
            AnnoObject result = base.CreateObject();
            if (Color.HasValue) result.Color = Color.Value;
            result.Road = true;
            return result;
        }

        protected override AnnoObject BuildTemplate()
        {
            return Template != null ? Template.ToAnnoObject() : new AnnoObject
            {
                Color = new SerializableColor(255, 169, 169, 169),
                Identifier = "Road",
                Template = "Road",
            };
        }
    }
}
