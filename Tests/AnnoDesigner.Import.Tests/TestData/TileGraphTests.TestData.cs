using System.Collections.Generic;

using static AnnoDesigner.Import.Model.TileGraph;

namespace AnnoDesigner.Import.Tests
{
    public partial class TileGraphTests
    {
        public static class TestData
        {
            public static IEnumerable<object[]> GetIntersections()
            {
                foreach (var testCase in GetIntersectionCases())
                {
                    yield return new object[] { testCase };
                }
            }

            private static List<TestCase> GetIntersectionCases()
            {
                int id = 1;
                return new List<TestCase>
                {
                    /* ============================================ */
                    /* 2-DEGREE INTERSECTIONS (28 cases)            */
                    /* ============================================ */

                    /* Orthogonal Opposite (2 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "N+S" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "E+W" },

                    /* Orthogonal Adjacent - L-shapes (4 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "N+E" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "E+S" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "S+W" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "W+N" },

                    /* Diagonal Opposite (2 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NE+SW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "SE+NW" },

                    /* Diagonal Adjacent (4 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "NE+SE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "SE+SW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "SW+NW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "NW+NE" },

                    /* Mixed: Orthogonal + Diagonal Adjacent (8 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+NE", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+NE", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+SE", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+SE", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+SW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+NW", AdjacentModifications = { ["W"] = 0b0110 } },

                    /* Mixed: Orthogonal + Diagonal Opposite (8 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+SE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+SW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+SW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+NW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+NW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+NE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+NE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+SE" },

                    /* ============================================ */
                    /* 3-DEGREE INTERSECTIONS (56 cases)            */
                    /* ============================================ */

                    /* 3 Orthogonal (4 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "N+E+S" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "E+S+W" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "S+W+N" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "W+N+E" },

                    /* 3 Diagonal (4 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NE+SE+SW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "SE+SW+NW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "SW+NW+NE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NW+NE+SE" },

                    /* 2 Orthogonal Adjacent + 1 Diagonal (16 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+NE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+SE", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+SW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+NE", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+SE", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+SW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+NW" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+NE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+SE", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+SW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+NW", AdjacentModifications = { ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+NE", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+SE" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1001 } },

                    /* 2 Orthogonal Opposite + 1 Diagonal (8 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+S+NE", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+S+SE", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+S+SW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+S+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+W+NE", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+W+SE", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+W+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+W+NW", AdjacentModifications = { ["W"] = 0b0110 } },

                    /* 1 Orthogonal + 2 Diagonal Adjacent (8 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "N+NE+NW", AdjacentModifications = { ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+NE+SE", AdjacentModifications = { ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+SE+SW", AdjacentModifications = { ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+SW+NW", AdjacentModifications = { ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+NE+SE", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+SE+SW", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+SW+NW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "W+NW+NE", AdjacentModifications = { ["W"] = 0b0110 } },

                    /* 1 Orthogonal + 2 Diagonal Opposite (8 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+SW", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+SE+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+SW", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+SE+NW", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+NE+SW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+SE+NW", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+NE+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+SE+NW", AdjacentModifications = { ["W"] = 0b0110 } },

                    /* 2 Diagonal Adjacent + 1 Orthogonal non-adjacent (8 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "NE+SE+W" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "SE+SW+N" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "SW+NW+E" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "NW+NE+S" },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "NE+S+SE", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "SE+SW+W", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "E+NE+NW", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+NW+SW", AdjacentModifications = { ["N"] = 0b1001 } },

                    /* ============================================ */
                    /* 4-DEGREE INTERSECTIONS (70 cases)            */
                    /* ============================================ */

                    /* 4 Orthogonal (1 case) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyOrthogonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W" },

                    /* 4 Diagonal (1 case) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NE+SE+SW+NW" },

                    /* 3 Orthogonal + 1 Diagonal (16 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+NE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+SE", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+SW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+W+NE", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+W+SE", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+W+SW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "E+S+W+NW", AdjacentModifications = { ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+N+NE", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+N+SE", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+N+SW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "S+W+N+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+E+NE", AdjacentModifications = { ["E"] = 0b0011, ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+E+SE", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+E+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "W+N+E+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1001 } },

                    /* 1 Orthogonal + 3 Diagonal (16 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+SE+SW", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+SE+NW", AdjacentModifications = { ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+SW+NW", AdjacentModifications = { ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+SE+SW", AdjacentModifications = { ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+SE+NW", AdjacentModifications = { ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+SW+NW", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+SE+SW+NW", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+NE+SE+SW", AdjacentModifications = { ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+NE+SE+NW", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+NE+SW+NW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+SE+SW+NW", AdjacentModifications = { ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+NE+SE+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+NE+SE+NW", AdjacentModifications = { ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+NE+SW+NW", AdjacentModifications = { ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+SE+SW+NW", AdjacentModifications = { ["W"] = 0b0100 } },

                    /* 2 Orthogonal Adjacent + 2 Diagonal (24 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+NE+SE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+NE+SW", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "N+E+NE+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+SE+SW", AdjacentModifications = { ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+SE+NW", AdjacentModifications = { ["N"] = 0b1001, ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+SW+NW", AdjacentModifications = { ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+S+NE+SE", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+NE+SW", AdjacentModifications = { ["E"] = 0b0011, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "E+S+NE+NW", AdjacentModifications = { ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+S+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+SE+NW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+S+SW+NW", AdjacentModifications = { ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+W+NE+SE", AdjacentModifications = { ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+W+NE+SW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "S+W+NE+NW", AdjacentModifications = { ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+W+SE+SW", AdjacentModifications = { ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+W+SE+NW", AdjacentModifications = { ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+W+SW+NW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+N+NE+SE", AdjacentModifications = { ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+N+NE+SW", AdjacentModifications = { ["W"] = 0b1100, ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "W+N+NE+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+N+SE+SW", AdjacentModifications = { ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+N+SE+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+N+SW+NW", AdjacentModifications = { ["W"] = 0b0100, ["N"] = 0b1001 } },

                    /* 2 Orthogonal Opposite + 2 Diagonal (12 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+S+NE+SE", AdjacentModifications = { ["N"] = 0b1100, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+NE+SW", AdjacentModifications = { ["N"] = 0b1100, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "N+S+NE+NW", AdjacentModifications = { ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+S+SE+SW", AdjacentModifications = { ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+SE+NW", AdjacentModifications = { ["N"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+S+SW+NW", AdjacentModifications = { ["N"] = 0b1001, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+W+NE+SE", AdjacentModifications = { ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+W+NE+SW", AdjacentModifications = { ["E"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "E+W+NE+NW", AdjacentModifications = { ["E"] = 0b0011, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+W+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+W+SE+NW", AdjacentModifications = { ["E"] = 0b1001, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+W+SW+NW", AdjacentModifications = { ["W"] = 0b0100 } },

                    /* ============================================ */
                    /* 5-DEGREE INTERSECTIONS (56 cases)            */
                    /* ============================================ */

                    /* 4 Orthogonal + 1 Diagonal (4 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+W+NE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+W+SE", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+W+SW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1100), Description = "N+E+S+W+NW", AdjacentModifications = { ["N"] = 0b1001, ["W"] = 0b0110 } },

                    /* 1 Orthogonal + 4 Diagonal (4 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+SE+SW+NW", AdjacentModifications = { ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+NE+SE+SW+NW", AdjacentModifications = { ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+NE+SE+SW+NW", AdjacentModifications = { ["W"] = 0b0100 } },

                    /* 3 Orthogonal + 2 Diagonal (28 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+S+NE+SE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+NE+SW", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0011, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "N+E+S+NE+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+S+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+SE+NW", AdjacentModifications = { ["N"] = 0b1001, ["E"] = 0b1001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+S+SW+NW", AdjacentModifications = { ["N"] = 0b1001, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+S+W+NE+SE", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+NE+SW", AdjacentModifications = { ["E"] = 0b0011, ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "E+S+W+NE+NW", AdjacentModifications = { ["E"] = 0b0011, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+S+W+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["W"] = 0b1100, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+SE+NW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "E+S+W+SW+NW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+W+N+NE+SE", AdjacentModifications = { ["S"] = 0b0110, ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+W+N+NE+SW", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b1100, ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "S+W+N+NE+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+W+N+SE+SW", AdjacentModifications = { ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+W+N+SE+NW", AdjacentModifications = { ["S"] = 0b0110, ["W"] = 0b0110, ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "S+W+N+SW+NW", AdjacentModifications = { ["S"] = 0b0011, ["N"] = 0b1001, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+N+E+NE+SE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+N+E+NE+SW", AdjacentModifications = { ["W"] = 0b1100, ["N"] = 0b1100, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "W+N+E+NE+NW", AdjacentModifications = { ["N"] = 0b1000, ["W"] = 0b0110, ["E"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+N+E+SE+SW", AdjacentModifications = { ["W"] = 0b1100, ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+N+E+SE+NW", AdjacentModifications = { ["W"] = 0b0110, ["N"] = 0b1001, ["E"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "W+N+E+SW+NW", AdjacentModifications = { ["W"] = 0b0100, ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NE+SE+SW", AdjacentModifications = { ["E"] = 0b0001, ["N"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+S+SE+SW", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+SE+SW+W", AdjacentModifications = { ["N"] = 0b1100, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+SE+SW+W", AdjacentModifications = { ["E"] = 0b0001, ["W"] = 0b1100 } },

                    /* 2 Orthogonal + 3 Diagonal (20 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+NE+SE+SW", AdjacentModifications = { ["N"] = 0b1100, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+NE+SE+NW", AdjacentModifications = { ["N"] = 0b1000, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+NE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NE+S+SE+SW+W", AdjacentModifications = { ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NE+NW+SE", AdjacentModifications = { ["E"] = 0b0001, ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+NW+S+SE", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NE+NW+SW", AdjacentModifications = { ["E"] = 0b0011, ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NW+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["N"] = 0b1001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+NW+S+SW", AdjacentModifications = { ["E"] = 0b0011, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NW+S+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+NW+SE+W", AdjacentModifications = { ["N"] = 0b1000, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+NW+SE+W", AdjacentModifications = { ["E"] = 0b0001, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NE+NW+S+SE+W", AdjacentModifications = { ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+NW+SW+W", AdjacentModifications = { ["N"] = 0b1000, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NE+NW+SW+W", AdjacentModifications = { ["E"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NW+SE+SW+W", AdjacentModifications = { ["N"] = 0b1001, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+NW+SE+SW+W", AdjacentModifications = { ["E"] = 0b1001, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NE+NW+S+SW+W", AdjacentModifications = { ["S"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "NW+S+SE+SW+W", AdjacentModifications = { ["S"] = 0b0010, ["W"] = 0b0100 } },

                    /* ============================================ */
                    /* 6-DEGREE INTERSECTIONS (28 cases)            */
                    /* ============================================ */

                    /* 4 Orthogonal + 2 Diagonal (6 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+S+W+NE+SE", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+NE+SW", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0011, ["S"] = 0b0011, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b0111), Description = "N+E+S+W+NE+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0011, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+S+W+SE+SW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+SE+NW", AdjacentModifications = { ["N"] = 0b1001, ["E"] = 0b1001, ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = Special(0b1110), Description = "N+E+S+W+SW+NW", AdjacentModifications = { ["N"] = 0b1001, ["S"] = 0b0011, ["W"] = 0b0100 } },

                    /* 2 Orthogonal + 4 Diagonal (6 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+S+NE+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+W+NE+SE+SW+NW", AdjacentModifications = { ["E"] = 0b0001, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+NE+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0001 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+NE+SE+SW+NW", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+W+NE+SE+SW+NW", AdjacentModifications = { ["S"] = 0b0010, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+N+NE+SE+SW+NW", AdjacentModifications = { ["W"] = 0b0100, ["N"] = 0b1000 } },

                    /* 3 Orthogonal + 3 Diagonal (16 cases) */
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+NE+SE+SW", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+NE+SE+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0001, ["S"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+NE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0011, ["S"] = 0b0011 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1001, ["E"] = 0b1001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+NE+SE+SW", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+NE+SE+NW", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+NE+SW+NW", AdjacentModifications = { ["E"] = 0b0011, ["S"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+SE+SW+NW", AdjacentModifications = { ["E"] = 0b1001, ["S"] = 0b0010, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NE+SE+SW+W", AdjacentModifications = { ["E"] = 0b0001, ["N"] = 0b1100, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+S+SE+SW+W", AdjacentModifications = { ["N"] = 0b1100, ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NE+NW+SE+W", AdjacentModifications = { ["E"] = 0b0001, ["N"] = 0b1000, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+NW+S+SE+W", AdjacentModifications = { ["N"] = 0b1000, ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NE+NW+SW+W", AdjacentModifications = { ["E"] = 0b0011, ["N"] = 0b1000, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+N+NW+SE+SW+W", AdjacentModifications = { ["E"] = 0b1001, ["N"] = 0b1001, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NE+NW+S+SW+W", AdjacentModifications = { ["N"] = 0b1000, ["S"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+NW+S+SE+SW+W", AdjacentModifications = { ["N"] = 0b1001, ["S"] = 0b0010, ["W"] = 0b0100 } },

                    /* ============================================ */
                    /* 7-DEGREE INTERSECTIONS (8 cases)             */
                    /* ============================================ */

                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+NE+SE+SW", AdjacentModifications = { ["N"] = 0b1100, ["E"] = 0b0001, ["S"] = 0b0010, ["W"] = 0b1100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+NE+SE+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0001, ["S"] = 0b0110, ["W"] = 0b0110 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+NE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0011, ["S"] = 0b0011, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1001, ["E"] = 0b1001, ["S"] = 0b0010, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+NE+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0001, ["S"] = 0b0010 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "E+S+W+NE+SE+SW+NW", AdjacentModifications = { ["E"] = 0b0001, ["S"] = 0b0010, ["W"] = 0b0100 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "S+W+N+NE+SE+SW+NW", AdjacentModifications = { ["S"] = 0b0010, ["W"] = 0b0100, ["N"] = 0b1000 } },
                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "W+N+E+NE+SE+SW+NW", AdjacentModifications = { ["W"] = 0b0100, ["N"] = 0b1000, ["E"] = 0b0001 } },

                    /* ============================================ */
                    /* 8-DEGREE INTERSECTION (1 case)               */
                    /* ============================================ */

                    new TestCase { CaseId = id++, ExpectedTile = TileSelection.AnyDiagonal, ExpectedQuadrants = 0b1111, Description = "N+E+S+W+NE+SE+SW+NW", AdjacentModifications = { ["N"] = 0b1000, ["E"] = 0b0001, ["S"] = 0b0010, ["W"] = 0b0100 } },
                };
            }
        }
    }
}
