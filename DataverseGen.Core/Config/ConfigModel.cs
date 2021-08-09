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
        public string Namespace { get; set; }

        [DataMember]
        public string OutDirectory { get; set; }

        [DataMember]
        public TemplateEngineModel TemplateEngine { get; set; }

        [DataMember]
        public string TemplateName { get; set; }
    }
    [Serializable]
    [DataContract]
    public class TemplateEngineModel
    {
        [DataMember]
        public bool IsSingleOutput { get; set; }

        /// <summary>
        /// scriban/t4
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// TS,C#
        /// </summary>
        [DataMember]
        public string Type { get; set; }
    }
}