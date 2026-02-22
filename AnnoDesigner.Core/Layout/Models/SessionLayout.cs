using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AnnoDesigner.Core.Layout.Models
{
    /// <summary>
    /// Container with session information and all islands.
    /// </summary>
    [DataContract]
    public class SessionLayout
    {
        [DataMember(Order = 0)]
        public string Name { get; set; }

        [DataMember(Order = 99)]
        public List<IslandLayout> Islands { get; set; }
    }
}
