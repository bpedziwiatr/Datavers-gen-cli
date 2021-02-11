using System;

namespace DataverseGen.Core.Metadata
{
    [Serializable]
    public class Context
    {
        public string Namespace { get; set; }

        public MappingEntity[] Entities { get; set; }
    }
}
