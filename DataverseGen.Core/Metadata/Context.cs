using System;

namespace DataverseGen.Core.Metadata
{
    [Serializable]
    public class Context
    {
        public Context()
        {
            Info = new GeneratorInfo();
        }

        public MappingEntity[] Entities { get; set; }
        public GeneratorInfo Info { get; set; }
        public string Namespace { get; set; }
    }
}