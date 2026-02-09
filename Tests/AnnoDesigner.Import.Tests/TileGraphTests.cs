using System;
using System.Collections.Generic;
using System.Linq;
using AnnoDesigner.Gamedata;
using AnnoDesigner.Import.Model;
using Xunit;

namespace AnnoDesigner.Import.Tests
{
    public partial class TileGraphTests
    {
        private readonly TileGraph graph;

        public TileGraphTests()
        {
            this.graph = new TileGraph();
        }

        [Theory]
        [MemberData(nameof(TestData.GetIntersections), MemberType = typeof(TestData))]
        public void Intersections_Merge(TestCase test)
        {
            var directions = test.Directions;
            var tiles = CreateTiles(directions);
            if (tiles.Count != directions.Count) Assert.Fail($"Failed to create tiles! (expected={directions.Count}, actual={tiles.Count})");

            var tile = TileGraph.SelectTile(tiles);
            Assert.NotNull(tile);

            var direction = tile.GetDirection();
            Assert.True(Verify(direction, test.ExpectedTile), $"Tile mismatch! (expected={test.ExpectedTile}, actual={direction})");

            byte quadrants = tile.CalculateQuadrants(tiles);
            Assert.True(quadrants == test.ExpectedQuadrants, $"Quadrants mismatch! (expected=0b{Convert.ToString(test.ExpectedQuadrants, 2)}, actual=0b{Convert.ToString(quadrants, 2)})");

            var adjacentModifications = tile.ModifyAdjacent(tiles);
            Verify(adjacentModifications, test.AdjacentModifications);
        }

        #region Private Helper Methods

        private List<TileGraph.Tile> CreateTiles(List<TileGraph.Direction> directions)
        {
            var center = new Point2D<float>(50.5f, 50.5f);
            const float orthogonalStep = 5.0f;
            const float diagonalStep = 4.0f;
            const int guid = 0;

            var directionVectors = new Dictionary<TileGraph.Direction, (float dx, float dy)>
            {
                { TileGraph.Direction.N,  (0, -orthogonalStep) },
                { TileGraph.Direction.NE, (diagonalStep, -diagonalStep) },
                { TileGraph.Direction.E,  (orthogonalStep, 0) },
                { TileGraph.Direction.SE, (diagonalStep, diagonalStep) },
                { TileGraph.Direction.S,  (0, orthogonalStep) },
                { TileGraph.Direction.SW, (-diagonalStep, diagonalStep) },
                { TileGraph.Direction.W,  (-orthogonalStep, 0) },
                { TileGraph.Direction.NW, (-diagonalStep, -diagonalStep) }
            };

            // our test data has some slightly different ordering
            var sortedDirections = directions.OrderBy(d => ((int)d + 90) % 360).ToList();

            foreach (var direction in sortedDirections)
            {
                var (dx, dy) = directionVectors[direction];
                var outer = new Point2D<float>(center.X + dx, center.Y + dy);
                graph.AddEdge(guid, new Line2D<float>(outer, center));
            }

            return graph.GetTilesAtPosition(center);
        }

        private bool Verify(TileGraph.Direction actual, TileSelection expected)
        {
            if (expected == TileSelection.AnyOrthogonal)
            {
                return actual == TileGraph.Direction.N || actual == TileGraph.Direction.E || actual == TileGraph.Direction.S || actual == TileGraph.Direction.W;
            }
            else if (expected == TileSelection.AnyDiagonal)
            {
                return actual == TileGraph.Direction.NE || actual == TileGraph.Direction.SE || actual == TileGraph.Direction.SW || actual == TileGraph.Direction.NW;
            }
            else
            {
                return false;
            }
        }

        private bool Verify(Dictionary<TileGraph.Tile, byte> actual, Dictionary<string, byte> expected)
        {
            int actualCount = actual?.Count ?? 0;
            int expectedCount = expected?.Count ?? 0;
            if (actualCount == 0 && expectedCount == 0) return true;

            if (actualCount != expectedCount)
            {
                Assert.Fail($"Number of adjacent modifications do not match! (expected={expectedCount}, actual={actualCount})");
                return false;
            }

            return Verify(
                actual.ToDictionary(kvp => kvp.Key.GetDirection(), kvp => kvp.Value),
                expected.ToDictionary(kvp => Enum.Parse<TileGraph.Direction>(kvp.Key.ToUpperInvariant()), kvp => kvp.Value));
        }

        private bool Verify(Dictionary<TileGraph.Direction, byte> actual, Dictionary<TileGraph.Direction, byte> expected)
        {
            foreach (var kvp in expected)
            {
                TileGraph.Direction direction = kvp.Key;
                byte expectedQuadrants = kvp.Value;

                if (!actual.TryGetValue(direction, out byte actualQuadrants))
                {
                    Assert.Fail($"Adjacent modification for direction {direction} missing!");
                    return false;
                }

                if (expectedQuadrants != actualQuadrants)
                {
                    Assert.Fail($"Adjacent modification mismatch for {direction}! (expected=0b{Convert.ToString(expectedQuadrants, 2)}, actual=0b{Convert.ToString(actualQuadrants, 2)})");
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}
