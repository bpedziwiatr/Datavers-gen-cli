using System;
using System.Runtime.Serialization;

namespace DataverseGen.Core.Config
{
    [Serializable]
    [DataContract]
    public class ConfigModel
    {
        [DataMember]
        public string Entities { get; set; }
        [DataMember]
        public string ConnectionString { get; set; }
        [DataMember]
        public string Namespace { get; set; }
        [DataMember]
        public string OutDirectory { get; set; }
    }
}