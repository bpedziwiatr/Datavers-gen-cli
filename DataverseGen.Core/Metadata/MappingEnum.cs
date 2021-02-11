using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DataverseGen.Core.Extensions;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Metadata
{
    [Serializable]
    public class MappingEnum
    {
        public string DisplayName { get; set; }
        public MapperEnumItem[] Items { get; set; }

        public static MappingEnum Parse(object attribute )
        {
            switch (attribute)
            {
                case EnumAttributeMetadata attributeMetadata:
                    return Parse(attributeMetadata);
                case BooleanAttributeMetadata booleanAttributeMetadata:
                    return Parse(booleanAttributeMetadata);
                default:
                    return null;
            }
        }

        public static MappingEnum Parse(EnumAttributeMetadata pickList)
        {
            MappingEnum enm = new MappingEnum
            {
                DisplayName = MetadataNamingExtensions.GetProperVariableName(MetadataNamingExtensions.GetProperVariableName(pickList.SchemaName)),
                Items =
                    pickList.OptionSet.Options
                    .Where(p=>p.Label.UserLocalizedLabel != null)
                    .Select(
                        o => new MapperEnumItem
                        {
                            Attribute = new CrmPicklistAttribute
                            {
                                DisplayName = o.Label.UserLocalizedLabel.Label,
                                Value = o.Value ?? 1
                            },
                            Name = MetadataNamingExtensions.GetProperVariableName(o.Label.UserLocalizedLabel.Label)
                        }
                    ).ToArray()
            };

            RenameDuplicates(enm);

            return enm;
        }

        public static MappingEnum Parse(BooleanAttributeMetadata twoOption)
        {
            MappingEnum enm = new MappingEnum
            {
                DisplayName = MetadataNamingExtensions.GetProperVariableName(MetadataNamingExtensions.GetProperVariableName(twoOption.SchemaName)),
                Items = new MapperEnumItem[2]
            };
            enm.Items[0] = MapBoolOption(twoOption.OptionSet.TrueOption);
            enm.Items[1] = MapBoolOption(twoOption.OptionSet.FalseOption);
            RenameDuplicates(enm);

            return enm;
        }
        private static void RenameDuplicates(MappingEnum enm)
        {
            Dictionary<string, int> duplicates = new Dictionary<string, int>();
            foreach (MapperEnumItem i in enm.Items)
                if (duplicates.ContainsKey(i.Name))
                {
                    duplicates[i.Name] = duplicates[i.Name] + 1;
                    i.Name += "_" + duplicates[i.Name];
                }
                else
                    duplicates[i.Name] = 1;
        }

        private static MapperEnumItem MapBoolOption(OptionMetadata option)
        {
            Debug.Assert(option.Value != null, "option.Value != null");
            MapperEnumItem results = new MapperEnumItem()
            {
                Attribute = new CrmPicklistAttribute()
                {
                    DisplayName = option.Label.UserLocalizedLabel.Label,
                    Value = (int)option.Value
                },
                Name = MetadataNamingExtensions.GetProperVariableName(option.Label.UserLocalizedLabel.Label)
            };
            return results;
        }

    }

    [Serializable]
    public class MapperEnumItem
    {
        public CrmPicklistAttribute Attribute { get; set; }
        public string Name { get; set; }
        public int Value => Attribute.Value;
    }
}
