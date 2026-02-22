using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Numerics;
using AnnoDesigner.Core.Helper;
using AnnoDesigner.Core.Layout.Models;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Gamedata;
using FileDBSerializing;
using RDAExplorer;

namespace IslandOutlinesExtractor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), "outlines.zip");
            string[] inputFiles = null;

            do
            {
                Console.Write("Please enter the path to the extracted .a7m files (files may be in sub-directories): ");
                string input = Console.ReadLine();
                if (input == "quit") return;

                if (Directory.Exists(input))
                {
                    inputFiles = Directory.GetFiles(input, "*.a7m", SearchOption.AllDirectories);

                    if (inputFiles.Length == 0)
                    {
                        Console.WriteLine("No .a7m files found, please try again or enter 'quit' to exit.");
                        Console.WriteLine();
                        inputFiles = null;
                    }
                }
                else
                {
                    Console.WriteLine("Path does not exists, please try again or enter 'quit' to exit.");
                    Console.WriteLine();
                }
            } while (inputFiles == null);

            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                using (var archive = new ZipArchive(outputStream, ZipArchiveMode.Create, true))
                {
                    foreach (string path in inputFiles)
                    {
                        string islandName = Path.GetFileNameWithoutExtension(path);
                        ZipArchiveEntry archiveEntry = archive.CreateEntry(islandName + ".ad", CompressionLevel.SmallestSize);
                        IFileDBDocument gamedata = LoadIslandGamedata(path);
                        ParseIsland(islandName, gamedata, archiveEntry);
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Saved {inputFiles.Length} outlines in {outputPath}");
        }

        private static IFileDBDocument LoadIslandGamedata(string path)
        {
            using (RDAReader reader = new RDAReader() { FileName = path })
            {
                return reader.File("gamedata.data").GetFileDBDocument();
            }
        }

        private static void ParseIsland(string name, IFileDBDocument gamedata, ZipArchiveEntry output)
        {
            Tag gameSessionManager = gamedata.Tag("GameSessionManager");
            Tag irrigationManager = gameSessionManager.Tag("IrrigationManager");
            Tag worldManager = gameSessionManager.Tag("WorldManager");

            Grid2D<UInt16> areaGrid = ParseBlocks<UInt16>(gameSessionManager.Tag("AreaIDs"));
            Grid2D<bool> irrigationGrid = ParseBlocks<byte>(irrigationManager.Tag("m_StaticTileGrid"), areaGrid.Width, areaGrid.Height).ToBoolean(value => value != 0);
            Grid2D<bool> water = ParseBits(worldManager.Tag("Water"), invert: true);
            Grid2D<bool> river = ParseBits(worldManager.Tag("RiverGrid"));

            Grid2D<bool> islandGrid = new Grid2D<bool>(areaGrid.Width, areaGrid.Height);
            Grid2D<bool> harbourGrid = new Grid2D<bool>(areaGrid.Width, areaGrid.Height);

            for (int y = 0; y < areaGrid.Height; y++)
            {
                for (int x = 0; x < areaGrid.Width; x++)
                {
                    UInt16 value = areaGrid[x, y];

                    if (value == 0)
                    {
                        continue; // empty area
                    }
                    else if (value == 1)
                    {
                        continue; // non-buildable area
                    }
                    else if (value == 0x2001)
                    {
                        if (water[x, y])
                        {
                            harbourGrid[x, y] = true;
                        }
                        else if (!river[x, y])
                        {
                            islandGrid[x, y] = true;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException($"Area ID 0x{value:X4} not implemented!");
                    }
                }
            }

            Grid2D<bool> islandOutline = islandGrid.ToOutline();
            Grid2D<bool> coastlineGrid = islandOutline.Intersect(harbourGrid);
            Grid2D<bool> harbourOutline = harbourGrid.Subtract(islandOutline).ToOutline();
            Grid2D<bool> irrigationOutline = irrigationGrid.ToOutline().Subtract(islandOutline);

            LayoutFile layout = new LayoutFile
            {
                FileVersion = 4,
                LayoutVersion = new Version("1.0.0.0"),
                Objects = new List<AnnoObject>(),
                Modified = DateTime.Now,
            };

            for (int y = 0; y < areaGrid.Height; y++)
            {
                for (int x = 0; x < areaGrid.Width; x++)
                {
                    if (coastlineGrid[x, y])
                    {
                        layout.Objects.Add(CreateBlocker(x, y, new SerializableColor(255, 30, 144, 255)));
                    }
                    else if (islandOutline[x, y])
                    {
                        layout.Objects.Add(CreateBlocker(x, y, new SerializableColor(255, 0, 0, 0)));
                    }
                    else if (irrigationOutline[x, y])
                    {
                        layout.Objects.Add(CreateBlocker(x, y, new SerializableColor(255, 128, 128, 0)));
                    }
                    else if (harbourOutline[x, y])
                    {
                        layout.Objects.Add(CreateBlocker(x, y, new SerializableColor(255, 192, 192, 192)));
                    }
                }
            }

            using (Stream outputStream = output.Open())
            {
                SerializationHelper.SaveToStream(layout, outputStream);
            }
        }

        private static Grid2D<bool> ParseBits(Tag grid, bool invert = false)
        {
            int width = grid.Attribute("x").ToNumber<int>();
            int height = grid.Attribute("y").ToNumber<int>();
            bool[] values = new bool[width * height];

            BitArray bits = new BitArray(grid.Attribute("bits").Content);
            if (invert) bits = bits.Not();
            bits.CopyTo(values, 0);

            return new Grid2D<bool>(values, width, height);
        }

        private static Grid2D<T> ParseBlocks<T>(Tag grid, int? fallbackWidth = null, int? fallbackHeight = null) where T : struct, INumber<T>
        {
            int gridWidth = grid.Attribute("x")?.ToNumber<int>() ?? fallbackWidth ?? throw new NullReferenceException();
            int gridHeight = grid.Attribute("y")?.ToNumber<int>() ?? fallbackHeight ?? throw new NullReferenceException();
            bool isSparseEnabled = grid.Attribute("SparseEnabled").ToBoolean();
            Grid2D<T> result = new Grid2D<T>(gridWidth, gridHeight);

            if (isSparseEnabled)
            {
                UInt16? blockWidth = null;
                UInt16? blockHeight = null;

                foreach (Tag block in grid.Tags("block"))
                {
                    byte? mode = block.Attribute("mode")?.ToNumber<byte>();

                    if (mode == 1) // start
                    {
                        blockWidth = block.Attribute("x").ToNumber<UInt16>();
                        blockHeight = block.Attribute("y").ToNumber<UInt16>();
                    }
                    else if (mode == 0) // end
                    {
                        blockWidth = null;
                        blockHeight = null;
                    }
                    else if (mode == 2) // default
                    {
                        Debug.Assert(blockWidth.HasValue && blockHeight.HasValue, "Block not initialized!");
                        Grid2D<T> section = new Grid2D<T>(blockWidth.Value, blockHeight.Value);
                        T value = block.Attribute("default").ToNumber<T>();
                        section.Fill(value);

                        UInt16 destinationX = block.Attribute("x")?.ToNumber<UInt16>() ?? 0;
                        UInt16 destinationY = block.Attribute("y")?.ToNumber<UInt16>() ?? 0;
                        result.Copy(section, destinationX, destinationY);
                    }
                    else if (mode == null) // values
                    {
                        Debug.Assert(blockWidth.HasValue && blockHeight.HasValue, "Block not initialized!");
                        T[] values = block.Attribute("values").ToNumbers<T>().ToArray();
                        Grid2D<T> section = new Grid2D<T>(values, blockWidth.Value, blockHeight.Value);

                        UInt16 destinationX = block.Attribute("x")?.ToNumber<UInt16>() ?? 0;
                        UInt16 destinationY = block.Attribute("y")?.ToNumber<UInt16>() ?? 0;
                        result.Copy(section, destinationX, destinationY);
                    }
                    else
                    {
                        throw new NotImplementedException($"Mode {mode} not implemented!");
                    }
                }
            }
            else
            {
                throw new NotImplementedException("Non-sparse mode not implemented!");
            }

            return result;
        }

        #region Private Helper Methods

        private static AnnoObject CreateBlocker(int x, int y, SerializableColor color)
        {
            return new AnnoObject
            {
                Template = "Blocker",
                Identifier = "BlockTile_1x1",
                Position = new System.Windows.Point(x, y),
                Size = new System.Windows.Size(1, 1),
                Borderless = true,
                Color = color,
            };
        }

        #endregion
    }
}
