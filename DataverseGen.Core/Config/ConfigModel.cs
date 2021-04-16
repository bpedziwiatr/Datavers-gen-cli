using System;
using System.Runtime.Serialization;

namespace DataverseGen.Core.Config
{
    [Serializable]
    [DataContract]
    public class ConfigModel
    {
        [DataMember]
        public string ConnectionString { get; set; }

        [DataMember]
        public string Entities { get; set; }
        [DataMember]
        public bool IsSingleOutputScriban { get; set; }

        [DataMember]
        public string Namespace { get; set; }

        [DataMember]
        public string OutDirectory { get; set; }

        [DataMember]
        public string TemplateEngine { get; set; }
        [DataMember]
        public string TemplateName { get; set; }
    }
}