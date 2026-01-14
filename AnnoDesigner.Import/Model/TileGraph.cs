using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AnnoDesigner.Core.Models;
using AnnoDesigner.Gamedata;

namespace AnnoDesigner.Import.Model
{
    internal class TileGraph
    {
        public class Edge
        {
            public int Length { get; private set; }
            public Tile Start { get; private set; }
            public Tile End { get; private set; }

            public IEnumerable<Tile> Tiles
            {
                get
                {
                    for (Tile tile = Start; tile != null; tile = tile.Next)
                    {
                        yield return tile;
                    }
                }
            }

            public void Add(Tile tile)
            {
                if (Start == null)
                {
                    this.Start = tile;
                    this.End = tile;
                }
                else
                {
                    this.End.Next = tile;
                    tile.Previous = End;
                    this.End = tile;
                }

                Length++;
            }
        }

        public class Tile
        {
            private readonly TileGraph graph;

            public Tile(int guid, TileGraph graph, Edge edge, Point2D<float> position, double rotation, byte quadrants)
            {
                this.RotationDegrees = (int)Math.Round(rotation * 180 / Math.PI);
                this.IsDiagonal = (RotationDegrees + 45) % 90 == 0;
                this.graph = graph;
                this.Edge = edge;

                this.Guid = guid;
                this.Position = position;
                this.Rotation = rotation;
                this.Quadrants = quadrants;
            }

            public Edge Edge { get; }
            public Tile Previous { get; set; }
            public Tile Next { get; set; }

            public bool IsStart => Edge.Start == this;
            public bool IsEnd => Edge.End == this;

            public bool IsDiagonal { get; }
            public bool IsOrthogonal => !IsDiagonal;
            public int RotationDegrees { get; }

#if DEBUG
            public SerializableColor? Color { get; set; }
            public string Label { get; set; }
#endif

            public int Guid { get; set; }
            public Point2D<float> Position { get; }
            public double Rotation { get; set; }
            public byte Quadrants { get; set; }

            public int GetNormalizedAngle()
            {
                int result = this.RotationDegrees;
                if (this.IsStart) result = (result + 180) % 360;
                if (result > 180) result -= 360;
                return result;
            }

            public int GetRelativeAngle(Tile other)
            {
                int result = other.GetNormalizedAngle() - this.GetNormalizedAngle();
                if (result > 180) result -= 360;
                if (result < -180) result += 360;
                return result;
            }

            public static bool MergeAll(List<Tile> diagonals, List<Tile> orthogonals)
            {
                if (diagonals.Count == 1)
                {
                    Tile diagonalTile = diagonals.Single();
                    if (diagonalTile.Merge(orthogonals)) return true;
                }
                else if (diagonals.Count == 2 && orthogonals.Count == 1)
                {
                    Tile orthogonalTile = orthogonals.Single();
                    Tile matchingDiagonal = diagonals.FirstOrDefault(diagonalTile => Math.Abs(diagonalTile.GetRelativeAngle(orthogonalTile)) == 45)
                        ?? diagonals.FirstOrDefault(diagonalTile => Math.Abs(diagonalTile.GetRelativeAngle(orthogonalTile)) == 135);

                    if (matchingDiagonal == null) throw new InvalidOperationException("Unhandled diagonal/orthogonal merge");
                    Tile[] otherDiagonals = diagonals.Where(diagonalTile => diagonalTile != matchingDiagonal).ToArray();
                    if (matchingDiagonal.Merge(orthogonalTile, otherDiagonals)) return true;
                }
                else if (diagonals.Count >= 2 && orthogonals.Count >= 2)
                {
                    if (IsCorner(orthogonals))
                    {
                        var cornerDiagonals = diagonals
                            .Where(diagonalTile => orthogonals.Any(orthogonalTile => Math.Abs(diagonalTile.GetRelativeAngle(orthogonalTile)) == 45))
                            .ToList();

                        if (cornerDiagonals.Count >= 2)
                        {
#if DEBUG
                            Debug.WriteLine($"    => Diagonals on corner case with {cornerDiagonals.Count} diagonals");
#endif

                            foreach (Tile orthogonalTile in orthogonals)
                            {
                                orthogonalTile.Skip(); // Skip all orthogonal tiles
                            }

                            foreach (Tile diagonalTile in diagonals.Skip(1))
                            {
                                diagonalTile.Skip(); // Skip all but one diagonal tiles
                            }
                        }
                        else
                        {
#if DEBUG
                            Debug.WriteLine($"    => Diagonal corner case with {orthogonals.Count} orthogonals and {diagonals.Count} diagonals");
#endif

                            foreach (Tile diagonalTile in diagonals)
                            {
                                diagonalTile.ModifyAdjacent(orthogonals);
                                diagonalTile.Skip();
                            }

                            foreach (Tile orthogonalTile in orthogonals.Skip(1))
                            {
                                orthogonalTile.Skip(); // Skip all but one orthogonal tile
                            }
                        }

                        return true;
                    }
                }

                return false;
            }

            public bool Merge(Tile other, params Tile[] others)
            {
                if (this.Quadrants == 0b0000) return false;
                List<Tile> diagonals = new List<Tile>();
                List<Tile> orthogonals = new List<Tile>();
                if (other.IsDiagonal) diagonals.Add(other);
                else orthogonals.Add(other);

                if (others.Length > 0)
                {
                    diagonals.AddRange(others.Where(t => t.IsDiagonal));
                    orthogonals.AddRange(others.Where(t => t.IsOrthogonal));
                }

                return Merge(diagonals, orthogonals);
            }

            public bool Merge(IEnumerable<Tile> others)
            {
                if (this.Quadrants == 0b0000) return false;
                List<Tile> diagonals = others.Where(t => t.IsDiagonal).ToList();
                List<Tile> orthogonals = others.Where(t => t.IsOrthogonal).ToList();
                return Merge(diagonals, orthogonals);
            }

            private void Merge(byte quadrants)
            {
                this.Quadrants = (byte)(0x80 | quadrants);
            }

            public void Skip()
            {
                this.Quadrants = 0;
            }

            #region Private Helper Method

            private List<Tile> GetAdjacentTiles()
            {
                List<Tile> result = new List<Tile>();
                Tile adjacentTile = this.IsStart ? this.Next : this.Previous;
                if (adjacentTile == null) return result;
                result.Add(adjacentTile);

                if (graph.TilesByPosition.TryGetValue(adjacentTile.Position, out var tiles)) result.AddRange(tiles);
                return result.Where(tile => tile.Quadrants != 0).ToList();
            }

            private bool Merge(List<Tile> diagonals, List<Tile> orthogonals)
            {
                if (this.IsDiagonal)
                {
                    bool result = false;

#if DEBUG
                    Debug.WriteLine($"  Merge: this.RotationDegrees={this.RotationDegrees}, isStart={this.IsStart}, isEnd={this.IsEnd}");
                    Debug.WriteLine($"  Merge: diagonals.Count={diagonals.Count}, orthogonals.Count={orthogonals.Count}");
#endif

                    if (Math.Abs(this.RotationDegrees) == 45 && orthogonals.Count == 2)
                    {
                        if (this.IsInnerCorner(orthogonals))
                        {
#if DEBUG
                            Debug.WriteLine($"    => Diagonal corner case with {orthogonals.Count} orthogonals and 1 diagonals");
#endif

                            this.ModifyAdjacent(orthogonals);
                            this.Skip();

                            // all orthogonals processed, return
                            return true;
                        }
                    }

                    foreach (Tile other in diagonals)
                    {
                        int relativeAngle = this.GetRelativeAngle(other);

#if DEBUG
                        Debug.WriteLine($"    Processing diagonal: other.RotationDegrees={other.RotationDegrees}, relativeAngle={relativeAngle}");
#endif

                        if (Math.Abs(relativeAngle) == 90)
                        {
#if DEBUG
                            Debug.WriteLine($"    => Diagonal merged with 90° angle");
                            this.Color = new SerializableColor(255, 255, 200, 100); // Light Orange
#endif

                            this.Merge((relativeAngle, this.IsStart) switch
                            {
                                (-90, false) => 0b0111, // checked
                                ( 90, true)  => 0b1011, // checked
                                (-90, true)  => 0b1101, // checked
                                ( 90, false) => 0b1110, // checked
                                _ => throw new InvalidOperationException($"Unhandled diagonal/diagonal merge: relativeAngle={relativeAngle}, isStart={this.IsStart}")
                            });
                        }
#if DEBUG
                        else
                        {
                            Debug.WriteLine($"    => Diagonal skipped (relativeAngle={relativeAngle} != ±90)");
                        }
#endif

                        other.Skip();
                        result = true;
                    }

                    foreach (Tile other in orthogonals)
                    {
                        int relativeAngle = this.GetRelativeAngle(other);

#if DEBUG
                        Debug.WriteLine($"    Processing orthogonal: other.RotationDegrees={other.RotationDegrees}, relativeAngle={relativeAngle}, this.RotationDegrees={Math.Abs(this.RotationDegrees)}");
#endif

                        if (Math.Abs(this.RotationDegrees) == 45 && diagonals.Count == 0)
                        {
#if DEBUG
                            Debug.WriteLine($"    => Orthogonal case 1");
                            this.Color = new SerializableColor(255, 255, 165, 0); // Orange
#endif

                            if (this.IsStart) this.Merge(0b0011);
                            else this.Merge(0b1100);

                            other.ModifyAdjacent(this, diagonals);
                            other.Skip();
                            result = true;
                        }
                        else if (diagonals.Count > 0)
                        {
#if DEBUG
                            Debug.WriteLine($"    => Orthogonal case 2");
#endif

                            if (Math.Abs(relativeAngle) == 135)
                            {
#if DEBUG
                                this.Color = new SerializableColor(255, 255, 200, 100); // Light Orange
#endif

                                int orthogonalNormalizedAngle = other.GetNormalizedAngle();
                                this.Merge((orthogonalNormalizedAngle, relativeAngle) switch
                                {
                                    ( 90, -135) => 0b1011, // checked
                                    (  0, -135) => 0b1011, // checked
                                    (-90,  135) => 0b1101, // checked
                                    (180, -135) => 0b1110, // checked
                                    _ => throw new InvalidOperationException($"Unhandled diagonal/orthogonal merge: relativeAngle={relativeAngle}, orthogonalNormalizedAngle={orthogonalNormalizedAngle}")
                                });
                            }

                            other.ModifyAdjacent(this, diagonals);
                            other.Skip();
                            result = true;
                        }
                    }

#if DEBUG
                    Debug.WriteLine($"  Merge result: {result}");
#endif
                    return result;
                }

                return false;
            }

            private void ModifyAdjacent(Tile other, IEnumerable<Tile> others)
            {
                List<Tile> diagonals = new List<Tile>();
                List<Tile> orthogonals = new List<Tile>();
                if (other.IsDiagonal) diagonals.Add(other);
                else orthogonals.Add(other);

                diagonals.AddRange(others.Where(t => t.IsDiagonal));
                orthogonals.AddRange(others.Where(t => t.IsOrthogonal));
                ModifyAdjacent(diagonals, orthogonals);
            }

            private void ModifyAdjacent(IEnumerable<Tile> others)
            {
                List<Tile> diagonals = others.Where(t => t.IsDiagonal).ToList();
                List<Tile> orthogonals = others.Where(t => t.IsOrthogonal).ToList();
                ModifyAdjacent(diagonals, orthogonals);
            }

            private void ModifyAdjacent(List<Tile> diagonals, List<Tile> orthogonals)
            {
                List<Tile> adjacentTiles = GetAdjacentTiles();
                if (adjacentTiles.Count == 0) return;

                if (this.IsDiagonal)
                {
                    if (diagonals.Count == 0 && orthogonals.Count >= 2)
                    {
                        if (!IsCorner(orthogonals)) return;
                        foreach (Tile adjacentTile in adjacentTiles)
                        {
                            if (this.IsStart) adjacentTile.Merge(0b0011);
                            else adjacentTile.Merge(0b1100);

#if DEBUG
                            adjacentTile.Color = new SerializableColor(255, 0, 255, 255); // Cyan
#endif
                        }
                    }
                }
                else
                {
                    if (diagonals.Count > 0 && orthogonals.Count == 0)
                    {
                        Tile diagonalTile = diagonals.First();
                        int relativeAngle = diagonalTile.GetRelativeAngle(this);
                        if (Math.Abs(relativeAngle) != 45) return;

                        int numDiagonals = 1; // diagonalTile
                        int orthogonalNormalizedAngle = this.GetNormalizedAngle();
                        numDiagonals += diagonals.Count(other => other.RotationDegrees != diagonalTile.RotationDegrees);

                        foreach (Tile adjacentTile in adjacentTiles)
                        {
                            adjacentTile.Quadrants = (orthogonalNormalizedAngle, relativeAngle, numDiagonals) switch
                            {
	                            // Orthogonal comes from the left (towards the current location in a ±0° angle)
	                            (0,  45, 1) => 0b1100, // checked
	                            (0,  45, 2) => 0b0001,
	                            (0, -45, 1) => 0b0110,
	                            (0, -45, 2) => 0b0100, // checked

	                            // Orthogonal comes from above (towards the current location in a 90° angle)
	                            (90,  45, 1) => 0b1001, // checked
	                            (90,  45, 2) => 0b0001,
	                            (90, -45, 1) => 0b1100, // checked
	                            (90, -45, 2) => 0b1000, // checked

	                            // Orthogonal comes from the right (towards the current location in a ±180° angle)
	                            (180 or -180,  45, 1) => 0b0011, // checked
	                            (180 or -180,  45, 2) => 0b0001, // checked
	                            (180 or -180, -45, 1) => 0b1001,
	                            (180 or -180, -45, 2) => 0b1000,

	                            // Orthogonal comes from below (towards the current location in a -90° angle)
	                            (-90,  45, 1) => 0b0110, // checked
	                            (-90,  45, 2) => 0b0010, // checked
	                            (-90, -45, 1) => 0b0011, // checked
	                            (-90, -45, 2) => 0b0001,

	                            _ => 0b1111
                            };

#if DEBUG
                            adjacentTile.Color = (orthogonalNormalizedAngle, relativeAngle) switch
                            {
                                (0,  45) => new SerializableColor(255, 255, 0, 0),               // Red
                                (0, -45) => new SerializableColor(255, 200, 50, 50),             // Light red
                                (90,  45) => new SerializableColor(255, 0, 255, 0),              // Green
                                (90, -45) => new SerializableColor(255, 0, 150, 0),              // Dark Green
                                (180 or -180,  45) => new SerializableColor(255, 0, 0, 255),     // Blue
                                (180 or -180, -45) => new SerializableColor(255, 100, 100, 255), // Light Blue
                                (-90,  45) => new SerializableColor(255, 255, 255, 0),           // Yellow
                                (-90, -45) => new SerializableColor(255, 255, 165, 0),           // Orange
                                _ => new SerializableColor(255, 255, 0, 255)                     // Magenta
                            };
#endif
                        }
                    }
                }
            }

            private static bool IsCorner(List<Tile> orthogonals)
            {
                for (int i = 0; i < orthogonals.Count; i++)
                {
                    for (int j = i + 1; j < orthogonals.Count; j++)
                    {
                        int relativeAngle = orthogonals[i].GetRelativeAngle(orthogonals[j]);
                        if (Math.Abs(relativeAngle) == 90 || Math.Abs(relativeAngle) == 270) return true;
                    }
                }

                return false;
            }

            private bool IsInnerCorner(List<Tile> orthogonals)
            {
                if (!this.IsDiagonal || orthogonals.Count != 2 || !IsCorner(orthogonals)) return false;
                int[] relativeAngles = orthogonals.Select(this.GetRelativeAngle).ToArray();
                if (relativeAngles.Any(angle => Math.Abs(angle) != 45)) return false;
                return Math.Sign(relativeAngles[0]) != Math.Sign(relativeAngles[1]);
            }

            #endregion
        }

        private readonly List<Edge> Edges = new List<Edge>();
        private readonly Dictionary<Point2D<float>, List<Tile>> TilesByPosition = new Dictionary<Point2D<float>, List<Tile>>();

        public void AddEdge(int guid, Line2D<float> edge)
        {
            List<Point2D<float>> points = edge.Rasterize().ToList();
            AddEdge(guid, points, edge.Angle);
        }

        // Pass 1: Create Edges and group all Tiles by position
        private void AddEdge(int guid, List<Point2D<float>> points, double angle)
        {
            TileGraph.Edge edge = new TileGraph.Edge();

            for (int i = 0; i < points.Count; i++)
            {
                Point2D<float> point = new Point2D<float>(points[i].X, points[i].Y);
                Tile tile = new Tile(guid, this, edge, point, angle, 0b1111);

#if DEBUG
                tile.Label = point.ToString();
#endif

                edge.Add(tile);
                AddTile(point, tile);
            }

            Edges.Add(edge);
        }

        private void AddTile(Point2D<float> position, Tile tile)
        {
            if (!TilesByPosition.ContainsKey(position)) TilesByPosition[position] = new List<Tile>();
            TilesByPosition[position].Add(tile);
        }

        public IEnumerable<Tile> Merge()
        {
            // Pass 2: Merge tiles at intersections
            foreach ((Point2D<float> position, List<Tile> tiles) in TilesByPosition)
            {
                if (tiles.Count > 1)
                {
#if DEBUG
                    Debug.WriteLine($"Intersection at ({position.X}, {position.Y}): {tiles.Count} tiles");
                    foreach (var tile in tiles)
                    {
                        double angle = tile.Rotation * 180 / Math.PI;
                        Debug.WriteLine($"  - angle={angle:F1}°, isDiag={tile.IsDiagonal}, isOrth={tile.IsOrthogonal}, isStart={tile.IsStart}, isEnd={tile.IsEnd}");
                    }
#endif

                    if (tiles.All(p => p.IsOrthogonal))
                    {
                        for (int i = 1; i < tiles.Count; i++)
                        {
                            tiles[i].Skip(); // Skip all but one orthogonal tile
                        }

                        continue;
                    }

                    if (tiles.Count == 2)
                    {
                        if (tiles.All(p => p.IsDiagonal))
                        {
                            var ordered = tiles.OrderBy(p => p.IsStart).ToArray();
                            if (ordered[0].Merge(ordered[1])) continue;
                        }
                        else
                        {
                            Tile diagonalTile = tiles.FirstOrDefault(p => p.IsDiagonal);
                            Tile orthogonalTile = tiles.FirstOrDefault(p => p.IsOrthogonal);

                            if (diagonalTile != null && orthogonalTile != null)
                            {
                                if (diagonalTile.Merge(orthogonalTile)) continue;
                            }
                        }
                    }

                    var diagonalTiles = tiles.Where(p => p.IsDiagonal).ToList();
                    var orthogonalTiles = tiles.Where(p => p.IsOrthogonal).ToList();
                    if (Tile.MergeAll(diagonalTiles, orthogonalTiles)) continue;

#if DEBUG
                    Debug.WriteLine($"  => UNHANDLED CASE");
                    foreach (var tile in tiles)
                    {
                        if (tile.IsStart || tile.IsEnd)
                        {
                            tile.Color = new SerializableColor(255, 200, 0, 200); // Dark Magenta (unhandled intersection)
                        }
                    }
#endif
                }
            }

            // Pass 3: Yield return all non-zero tiles
            foreach (var edge in Edges)
            {
                foreach (var tile in edge.Tiles)
                {
                    if (tile.Quadrants == 0b0000) continue;
                    yield return tile;
                }
            }
        }
    }
}
