using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Core.Presets.Models;
using AnnoDesigner.Importer.Model;
using FileDBSerializing;
using RDAExplorer;

namespace AnnoDesigner.Importer
{
    public static class Anno117
    {
        private const string Header = "(A8) Anno 117";
        private const int GUID_PROFILE_HUMAN = 41;

        public class SavegameReader
        {
            public LayoutFile ImportLayout(string path, BuildingPresets presets)
            {
                RDAReader reader = new RDAReader() { FileName = path };
                IFileDBDocument gamedata = reader.File("data.a7s").GetFileDBDocumentInflated(); // interestingly the actual data file inside the .a8s is still named .a7s

                Tag metaGameManager = gamedata.Tag("MetaGameManager");
                Tag gameSessions = metaGameManager.Tag("GameSessions");

                LayoutFile layout = new LayoutFile
                {
                    FileVersion = 5,
                    LayoutVersion = new Version("1.0.0.0"),
                    Modified = File.GetLastWriteTime(path),
                    Sessions = new List<SessionLayout>(),
                };

#if DEBUG
                HashSet<int> missingPresets = new HashSet<int>();
#endif

                foreach (Tag session in gameSessions.Tags())
                {
                    int sessionGuid = session.Tag("SessionDesc").Attribute("SessionGUID").ToNumber<int>();
                    IFileDBDocument sessionData = session.Tag("SessionData").Attribute("BinaryData").ToFileDBDocument();
                    Tag gameSessionManager = sessionData.Tag("GameSessionManager");

                    Tag areaManagers = gameSessionManager.Tag("AreaManagers");
                    Dictionary<UInt16, Tag> areaInfos = gameSessionManager.Tag("AreaInfo").ToDictionary<UInt16>();

                    List<Tag> mapTemplates = gameSessionManager.Tag("MapTemplate").Tags("TemplateElement")
                        .SelectMany(element => element.Tags("Element"))
                        .Where(element => element.Attribute("MapFilePath") != null)
                        .ToList();

                    SessionLayout sessionLayout = new SessionLayout
                    {
                        Name = sessionGuid.ToString(), // TODO get session name by guid
                        Islands = new List<IslandLayout>(),
                    };

                    foreach (UInt16 areaId in areaInfos.Keys)
                    {
                        Tag areaInfo = areaInfos[areaId];
                        Tag areaManager = areaManagers.Tag("AreaManager_" + areaId);
                        int islandOwnerGuid = areaInfo.Attribute("OwnerProfile")?.ToNumber<int>() ?? 0;
                        if (islandOwnerGuid != GUID_PROFILE_HUMAN) continue;

                        string cityName = areaInfo.Attribute("CityName")?.ToUnicode();
                        if (cityName == null) cityName = areaInfo.Attribute("CityNameGuid").ToNumber<long>().ToString(); // TODO get city name by guid

                        Debug.WriteLine($"Processing island '{cityName}'...");
                        IEnumerable<Tag> polygonObjects = areaManager.Tag("AreaPolygonObjectManager").Tag("Polygons").Tags();
                        IEnumerable<Tag> gameObjects = areaManager.Tag("AreaObjectManager").Tag("GameObject").Tag("Objects").Tags();
                        Island island = CreateIsland(cityName, gameObjects, mapTemplates);

                        Tag streetGraph = areaManager.Tag("AreaStreetManager").Tag("Graph");
                        Tag aqueductGraph = areaManager.Tag("AreaAqueductManager").Tag("Graph");
                        Tag canalGraph = areaManager.Tag("AreaCanalManager").Tag("Graph");
                        Tag hedgeGraph = areaManager.Tag("AreaHedgeManager").Tag("Graph");
                        Tag wallGraph = areaManager.Tag("AreaWallManager").Tag("Graph");

                        foreach (Tag gameObject in gameObjects)
                        {
                            long id = gameObject.Attribute("ID").ToNumber<long>();
                            int guid = gameObject.Attribute("Guid").ToNumber<int>();
                            int state = gameObject.Attribute("StateBits")?.ToNumber<int>() ?? 0;
                            var template = FindBuildingByGuid(presets, guid);

                            if (template != null)
                            {
                                var position = gameObject.Attribute("Position").ToPoint3D<float>();
                                float direction = gameObject.Attribute("Direction")?.ToNumber<float>() ?? 0;
                                GameObject building = new GameObject(template, direction, island.ToLocalCoordinates<float>(position));

#if DEBUG
                                building.Label = id.ToString();
#endif

                                // TODO find out values for different states (destroyed, blue-print, etc.) and change the color accordingly
                                if ((state & 102) != 0) building.Color = new SerializableColor(255, 224, 239, 255); // blue-print
                                else if (island.Colors.TryGetValue(id, out var color)) building.Color = color;
                                island.Objects.Add(building.CreateObject());
                            }
#if DEBUG
                            else if (missingPresets.Add(guid))
                            {
                                Debug.WriteLine($"Building {guid} not found!");
                            }
#endif
                        }

                        foreach (Tag polygonObject in polygonObjects)
                        {
                            long ownerId = polygonObject.Tag("ModuleOwner").Attribute("ObjectID").ToNumber<long>();
                            if (ownerId == 0) continue;

                            Tag tilesGrid = polygonObject.Tag("SubTilesGrid");
                            int guid = polygonObject.Attribute("GUID").ToNumber<int>();
                            var template = FindBuildingByGuid(presets, guid);
                            var color = island.Colors.TryGetValue(ownerId);

                            if (template != null)
                            {
                                ProcessTilesGrid(tilesGrid, (value, position) =>
                                {
                                    /*
                                     * 0x6 (0110) for a ◢ tile
                                     * 0xC (1100) for a ◥ tile
                                     * 0x3 (0011) for a ◣ tile
                                     * 0x9 (1001) for a ◤ tile
                                     * 0xF (1111) for a ■ tile
                                     */

                                    if (value == 0xF)
                                    {
                                        TileObject tile = new TileObject(template, island.ToLocalCoordinates(position));
                                        if (color.HasValue) tile.Color = color.Value;
                                        island.Objects.Add(tile.CreateObject());
                                    }
                                    else
                                    {
                                        // TODO sub-triangle handling
                                    }
                                });
                            }
#if DEBUG
                            else if (missingPresets.Add(guid))
                            {
                                Debug.WriteLine($"Building {guid} not found!");
                            }
#endif
                        }

                        if (streetGraph != null)
                        {
                            ProcessGraph(streetGraph, (guid, position) =>
                            {
                                var template = FindBuildingByGuid(presets, guid);

#if DEBUG
                                if (template == null && missingPresets.Add(guid))
                                {
                                    Debug.WriteLine($"Street object {guid} not found!");
                                }
#endif

                                RoadObject road = new RoadObject(template, island.ToLocalCoordinates(position));
                                island.Objects.Add(road.CreateObject());
                            });
                        }

                        if (aqueductGraph != null)
                        {
                            ProcessGraph(aqueductGraph, (guid, position) =>
                            {
                                var template = FindBuildingByGuid(presets, guid);

#if DEBUG
                                if (template == null && missingPresets.Add(guid))
                                {
                                    Debug.WriteLine($"Aqueduct object {guid} not found!");
                                }
#endif

                                TileObject tile = new TileObject(template, island.ToLocalCoordinates(position));
                                island.Objects.Add(tile.CreateObject());
                            });
                        }

                        sessionLayout.Islands.Add(new IslandLayout
                        {
                            Name = cityName,
                            Objects = island.Objects,
                        });
                    }

                    layout.Sessions.Add(sessionLayout);
                }

                return layout;
            }

            #region Graph Section

            private static void ProcessGraph(Tag graph, Action<int, Point2D<int>> action)
            {
                IEnumerable<Point2D<int>> nodes = graph.Tag("Nodes").Attributes().Select(node => node.ToPoint2D<int>()); // TODO can we simply ignore the nodes?
                IEnumerable<Tag> edges = graph.Tag("Edges").Tags();

                foreach (Tag edge in edges)
                {
                    int guid = edge.Attribute("guid").ToNumber<int>();
                    ProcessEdge(guid, edge.Tag("Edge"), action);
                }
            }

            private static void ProcessEdge(int guid, Tag edge, Action<int, Point2D<int>> action)
            {
                // for some reason the positions in a graph in Anno 117 are scaled by a factor of 2 so we need to scale them down
                Point2D<int> start = edge.Attribute("PosMin").ToPoint2D<int>().Scale(0.5f).Round(MidpointRounding.ToPositiveInfinity);
                Point2D<int> end = edge.Attribute("PosMax").ToPoint2D<int>().Scale(0.5f).Round(MidpointRounding.ToPositiveInfinity);
                ProcessEdge(guid, new Line2D<int>(start, end), action);
            }

            private static void ProcessEdge(int guid, Line2D<int> edge, Action<int, Point2D<int>> action)
            {
                foreach (Point2D<int> point in edge.Rasterize())
                {
                    action(guid, new Point2D<int>(point.X, point.Y - 1)); // TODO check why we need Y-1 to fix incorrect position
                }
            }

            #endregion

            #region Tiles Grid Section

            private static void ProcessTilesGrid(Tag tilesGrid, Action<byte, Point2D<int>> action)
            {
                Tag grid = tilesGrid.Tag("Grid").Tags().Single();
                Point2D<int> origin = tilesGrid.Attribute("GridOriginWS").ToPoint2D<int>();
                ProcessTilesGrid(grid, origin, action);
            }

            private static void ProcessTilesGrid(Tag grid, Point2D<int> origin, Action<byte, Point2D<int>> action)
            {
                int rows = grid.Attribute("y").ToNumber<int>();
                byte[] bits = grid.Attribute("bits").ToNibbles().ToArray();
                int columns = bits.Length / rows; // actually we should use grid.Attribute("x") which as far as I can tell is the number of bits per rows, but sometimes for some reason the value is incorrect
                ProcessTilesGrid(bits, columns, rows, origin, action);
            }

            private static void ProcessTilesGrid(byte[] bits, int width, int height, Point2D<int> origin, Action<byte, Point2D<int>> action)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        byte value = bits[y * width + x];
                        action(value, new Point2D<int>(origin.X + x + 1, origin.Y + y)); // TODO check why we need X+1 to fix incorrect position
                    }
                }
            }

            #endregion

            #region Presets Section

            private static BuildingInfo FindBuildingByGuid(BuildingPresets presets, int guid)
            {
                switch (guid)
                {
                    case 81354: guid = 19723; break; // Aqueduct Roman Aqueduct MaxGround
                    case 82038: guid = 29525; break; // Aqueduct Roman Celtic Aqueduct MaxGround
                }

                return presets.Buildings.FirstOrDefault(b => b.Header.Equals(Anno117.Header) && b.Guid == guid);
            }

            #endregion
        }

        #region Map Templates/Islands Section

        // extracted from .a7minfo files
        private static readonly Dictionary<string, Size<int>> IslandSizes = new Dictionary<string, Size<int>>
        {
            { "roman_island_extralarge_01", new Size<int>(416, 416) },
            { "roman_island_extralarge_02", new Size<int>(416, 416) },
            { "roman_island_extralarge_03", new Size<int>(416, 392) },
            { "roman_island_extralarge_04", new Size<int>(416, 416) },
            { "roman_island_large_01", new Size<int>(416, 416) },
            { "roman_island_large_02", new Size<int>(336, 416) },
            { "roman_island_large_03", new Size<int>(384, 416) },
            { "roman_island_large_04", new Size<int>(416, 376) },
            { "roman_island_large_05", new Size<int>(376, 376) },
            { "roman_island_large_06", new Size<int>(384, 384) },
            { "roman_island_large_07", new Size<int>(408, 384) },
            { "roman_island_large_09", new Size<int>(416, 360) },
            { "roman_island_medium_01", new Size<int>(272, 320) },
            { "roman_island_medium_02", new Size<int>(256, 256) },
            { "roman_island_medium_03", new Size<int>(264, 320) },
            { "roman_island_medium_04", new Size<int>(320, 320) },
            { "roman_island_medium_05", new Size<int>(256, 256) },
            { "roman_island_medium_06", new Size<int>(312, 296) },
            { "roman_island_medium_07", new Size<int>(320, 320) },
            { "roman_island_medium_08", new Size<int>(304, 312) },
            { "roman_island_small_01", new Size<int>(256, 240) },
            { "roman_island_small_02", new Size<int>(192, 168) },
            { "roman_island_small_03", new Size<int>(160, 192) },
            { "roman_island_small_04", new Size<int>(248, 240) },
            { "roman_island_small_05", new Size<int>(240, 224) },
            { "roman_island_small_06", new Size<int>(240, 232) },
            { "roman_island_small_07", new Size<int>(240, 248) },
            { "celtic_island_large_01", new Size<int>(344, 408) },
            { "celtic_island_large_02", new Size<int>(352, 408) },
            { "celtic_island_large_03", new Size<int>(352, 344) },
            { "celtic_island_large_04", new Size<int>(336, 392) },
            { "celtic_island_large_05", new Size<int>(360, 408) },
            { "celtic_island_large_06", new Size<int>(376, 384) },
            { "celtic_island_large_07", new Size<int>(304, 320) },
            { "celtic_island_large_08", new Size<int>(368, 352) },
            { "celtic_island_medium_01", new Size<int>(208, 208) },
            { "celtic_island_medium_02", new Size<int>(256, 256) },
            { "celtic_island_medium_03", new Size<int>(248, 248) },
            { "celtic_island_medium_04", new Size<int>(232, 240) },
            { "celtic_island_medium_05", new Size<int>(296, 288) },
            { "celtic_island_medium_06", new Size<int>(304, 272) },
            { "celtic_island_medium_07", new Size<int>(208, 248) },
            { "celtic_island_small_01", new Size<int>(256, 256) },
            { "celtic_island_small_02", new Size<int>(232, 216) },
            { "celtic_island_small_03", new Size<int>(184, 240) },
            { "celtic_island_small_04", new Size<int>(224, 216) },
            { "celtic_island_small_05", new Size<int>(176, 232) },
            { "celtic_island_small_06", new Size<int>(152, 128) },
            { "celtic_island_small_07", new Size<int>(192, 192) },
        };

        /// <summary>
        /// Searchs within the <paramref name="templateElements"/> for the island that contains the <paramref name="gameObjects"/> based on their position.
        /// </summary>
        private static Island CreateIsland(string cityName, IEnumerable<Tag> gameObjects, IEnumerable<Tag> templateElements)
        {
            foreach (Tag element in templateElements)
            {
                string islandTemplate = Path.GetFileNameWithoutExtension(element.Attribute("MapFilePath").ToUnicode());
                Point2D<int> islandPosition = element.Attribute("Position").ToPoint2D<int>();

                if (IslandSizes.TryGetValue(islandTemplate, out Size<int> islandSize))
                {
                    Rectangle<int> islandRectangle = new Rectangle<int>(islandPosition.X, islandPosition.Y, islandSize.Width, islandSize.Height);
                    GridDirection islandRotation = (GridDirection)element.Attribute("Rotation90").ToNumber<byte>();

                    if (gameObjects.Match(islandRectangle) >= 0.9f) // TODO actually for Anno 1800 the islandRectangle.ContainsAll(gameObjects) method worked fine, for Anno 117 some buildings are outside of bounds sometimes (maybe IslandSizes are not correct?)
                    {
                        var island = new Island(cityName, islandTemplate, islandPosition, islandRotation, islandSize);
                        List<Tag> ownerObjects = gameObjects.Where(o => o.Tag("ModuleOwner") != null)
                            .DistinctBy(o => o.Attribute("ID").ToNumber<long>())
                            .ToList();

                        for (int i = 0; i < ownerObjects.Count; i++)
                        {
                            Tag moduleOwner = ownerObjects[i].Tag("ModuleOwner");
                            long ownerId = moduleOwner.Parent.Attribute("ID").ToNumber<long>();
                            var color = Modules.Colors[i % Modules.Colors.Count];
                            island.Colors[ownerId] = color;

                            if (moduleOwner.Attribute("BinArray") != null)
                            {
                                List<long> modules = moduleOwner.Attribute("BinArray").ToNumbers<long>()
                                    .SkipLast(1) // BinArray has 1 element more than owner has modules, unclear what this element is though so skip it for now
                                    .ToList();

                                long moduleId = modules[0];
                                island.Colors[moduleId] = color;

                                for (int j = 1; j < modules.Count; j++)
                                {
                                    moduleId += modules[j] + 1;
                                    island.Colors[moduleId] = color;
                                }
                            }
                        }

                        return island;
                    }
                }
            }

            throw new Exception("No matching island found!");
        }

        #endregion
    }
}
