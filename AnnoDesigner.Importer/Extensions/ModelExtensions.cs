using System;
using System.Collections.Generic;
using System.Linq;
using FileDBSerializing;

namespace AnnoDesigner.Importer
{
    internal static class ModelExtensions
    {
        internal static bool ContainsAll(this Rectangle<int> rectangle, IEnumerable<Tag> gameObjects)
        {
            foreach (Tag gameObject in gameObjects)
            {
                var attribute = gameObject.Attribute("Position");
                if (attribute == null) throw new ArgumentException("Invalid game object!");

                var position = attribute.ToPoint3D<float>();
                if (!rectangle.Contains((int)position.X, (int)position.Y)) return false;
            }

            return true;
        }

        internal static float Match(this IEnumerable<Tag> gameObjects, Rectangle<int> rectangle)
        {
            int count = 0;

            foreach (Tag gameObject in gameObjects)
            {
                var attribute = gameObject.Attribute("Position");
                if (attribute == null) throw new ArgumentException("Invalid game object!");

                var position = attribute.ToPoint3D<float>();
                if (rectangle.Contains((int)position.X, (int)position.Y)) count++;
            }

            return (float)count / gameObjects.Count();
        }
    }
}
