using System;
using System.Collections.Generic;
using System.Linq;
using AnnoDesigner.Import.Model;
using Xunit.Abstractions;

namespace AnnoDesigner.Import.Tests
{
    public partial class TileGraphTests
    {
        public enum TileSelection
        {
            AnyOrthogonal, // Any orthogonal tile (N, E, S, W)
            AnyDiagonal,   // Any diagonal tile (NE, SE, SW, NW)
        }

        public class TestCase : IXunitSerializable
        {
            public TestCase()
            {
                this.AdjacentModifications = new Dictionary<string, byte>();
            }

            public int CaseId { get; set; }
            public string Description { get; set; }
            public TileSelection ExpectedTile { get; set; }
            public byte ExpectedQuadrants { get; set; }
            public Dictionary<string, byte> AdjacentModifications { get; set; }

            internal List<TileGraph.Direction> Directions
            {
                get
                {
                    if (string.IsNullOrEmpty(Description)) return new List<TileGraph.Direction>();
                    return Description.Split('+').Select(d => Enum.Parse<TileGraph.Direction>(d.ToUpperInvariant())).ToList();
                }
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue(nameof(CaseId), this.CaseId);
                info.AddValue(nameof(Description), this.Description);
                info.AddValue(nameof(ExpectedTile), this.ExpectedTile);
                info.AddValue(nameof(ExpectedQuadrants), this.ExpectedQuadrants);

                if (AdjacentModifications != null && AdjacentModifications.Count > 0)
                {
                    info.AddValue(nameof(AdjacentModifications), string.Join("|", AdjacentModifications.Select(kvp => $"{kvp.Key}:{kvp.Value}")));
                }
                else
                {
                    info.AddValue(nameof(AdjacentModifications), string.Empty);
                }
            }

            public void Deserialize(IXunitSerializationInfo info)
            {
                this.CaseId = info.GetValue<int>(nameof(CaseId));
                this.Description = info.GetValue<string>(nameof(Description));
                this.ExpectedTile = info.GetValue<TileSelection>(nameof(ExpectedTile));
                this.ExpectedQuadrants = info.GetValue<byte>(nameof(ExpectedQuadrants));
                var adjacentModifications = info.GetValue<string>(nameof(AdjacentModifications));

                if (!string.IsNullOrEmpty(adjacentModifications))
                {
                    this.AdjacentModifications = adjacentModifications.Split('|').Select(kvp => kvp.Split(':'))
                        .ToDictionary(kvp => kvp[0], kvp => byte.Parse(kvp[1]));
                }
                else
                {
                    this.AdjacentModifications = new Dictionary<string, byte>();
                }
            }

            public override string ToString()
            {
                return $"Case {CaseId:D3}: {Description}";
            }
        }
    }
}
