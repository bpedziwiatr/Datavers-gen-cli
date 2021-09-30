using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using DataverseGen.Core.Metadata;
using Microsoft.Xrm.Sdk.Metadata;

namespace DataverseGen.Core.Extensions
{
    public static class MetadataNamingExtensions
    {
        private static string Clean(string text)
        {
            string result = "";
            if (string.IsNullOrEmpty(text)) return result;
            text = text.Trim();
            text = Normalize(text);

            if (!string.IsNullOrEmpty(text))
            {
                StringBuilder sb = new StringBuilder();
                int start = 0;
                if (!char.IsLetter(text[0]))
                {
                    sb.Append("_");
                }

                for (int i = start; i < text.Length; i++)
                {
                    if ((char.IsDigit(text[i]) || char.IsLetter(text[i]) || text[i] == '_') && !string.IsNullOrEmpty(text[i].ToString()))
                    {
                        sb.Append(text[i]);
                    }
                }

                result = sb.ToString();
            }

            result = ReplaceKeywords(result);

            result = Regex.Replace(result, "[^A-Za-z0-9_]", "");

            return result;
        }

        private static string Normalize(string regularString)
        {
            string normalizedString = regularString.Normalize(NormalizationForm.FormD);

            StringBuilder sb = new StringBuilder(normalizedString);

            for (int i = 0; i < sb.Length; i++)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(sb[i]) == UnicodeCategory.NonSpacingMark)
                    sb.Remove(i, 1);
            }
            regularString = sb.ToString();

            return regularString.Replace("æ", "");
        }


        private static string ReplaceKeywords(string keyword)
        {
            return keyword.Equals("public", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("private", StringComparison.InvariantCultureIgnoreCase)
                // || name.Equals("event", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("single", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("new", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("partial", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("to", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("error", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("readonly", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("case", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("object", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("global", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("true", StringComparison.InvariantCultureIgnoreCase)
                || keyword.Equals("false", StringComparison.InvariantCultureIgnoreCase)
                ? "__" + keyword
                : keyword;
        }


        private static string CapitalizeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return "";

            return word.Substring(0, 1).ToUpper() + word.Substring(1);
        }

        private static string DecapitalizeWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return "";

            return word.Substring(0, 1).ToLower() + word.Substring(1);
        }

        private static string Capitalize(string name, bool capitalizeFirstWord)
        {
            string[] parts = name.Split(' ', '_');

            for (int i = 0; i < parts.Count(); i++)
                parts[i] = i != 0 || capitalizeFirstWord ? CapitalizeWord(parts[i]) : DecapitalizeWord(parts[i]);

            return string.Join("_", parts);
        }

        //internal static string GetUniqueName(MappingEntity entity, string requestedName)
        //{
        //    if (entity.DisplayName != requestedName)
        //    {
        //        if(entity.Fields != null)
        //        {
        //            if (!entity.Fields.Any(x => requestedName.Equals(x.DisplayName)))
        //                return requestedName;
        //        }
        //    }
        //    for (int i = 1; i < 999; i++)
        //    {
        //        var newName = requestedName + i;
        //        if (!entity.Fields.Any(x => x.DisplayName == newName))
        //            return newName;
        //    }
        //    return requestedName;
        //}

        public static string GetProperEntityName(this EntityMetadata entityMetadata)
        {
                return Clean(Capitalize(entityMetadata.SchemaName, true));
        }
        public static string GetProperHybridName(this EntityMetadata entityMetadata )
        {
            if (!entityMetadata.LogicalName.Contains("_")) 
                return Clean(Capitalize(entityMetadata.SchemaName, true));
            Console.WriteLine($@"{entityMetadata.SchemaName} {entityMetadata.LogicalName}");
            return entityMetadata.SchemaName;

        }
        public static string GetProperHybridFieldName(this MappingField mappingField)
        {
            return mappingField.Attribute != null && mappingField.Attribute.LogicalName.Contains("_")
                ? mappingField.Attribute.LogicalName
                : mappingField.DisplayName;
        }

        public static string GetProperVariableName(this AttributeMetadata attribute)
        {
            switch (attribute.LogicalName)
            {
                // Normally we want to use the SchemaName as it has the capitalized names (Which is what CrmSvcUtil.exe does).  
                // HOWEVER, If you look at the 'annual' attributes on the annualfiscalcalendar you see it has schema name of Period1  
                // So if the logicalname & schema name don't match use the logical name and try to capitalize it 
                // EXCEPT,  when it's RequiredAttendees/From/To/Cc/Bcc/SecondHalf/FirstHalf  (i have no idea how CrmSvcUtil knows to make those upper case)
                case "requiredattendees":
                    return "RequiredAttendees";
                case "from":
                    return "From";
                case "to":
                    return "To";
                case "cc":
                    return "Cc";
                case "bcc":
                    return "Bcc";
                case "firsthalf":
                    return "FirstHalf";
                case "secondhalf":
                    return "SecondHalf";
                case "firsthalf_base":
                    return "FirstHalf_Base";
                case "secondhalf_base":
                    return "SecondHalf_Base";
                case "attributes":
                    return "Attributes1";
                case "id":
                    return "__Id";  // LocalConfigStore has an attribute named Id, the template will already add and Id
                case "entitytypecode":
                    return "__EntityTypeCode";  // PrincipalSyncAttributeMap has a field called EntityTypeCode, the template will already have a property called EntityTypeCode which refers to the entity's type code.
            }

            return Clean(attribute.LogicalName != null && attribute.LogicalName.Equals(attribute.SchemaName, StringComparison.InvariantCultureIgnoreCase) 
                ? attribute.SchemaName 
                : Capitalize(attribute.LogicalName, true));
        }
        public static string GetProperVariableName(string p)
        {
            if (string.IsNullOrWhiteSpace(p))
                return "Empty";
            return p == "Closed (deprecated)" ? "Closed" : Clean(p);
        
        }

        public static string GetPluralName(this string name)
        {
            if (name.EndsWith("y"))
                return name.Substring(0, name.Length - 1) + "ies";

            if (name.EndsWith("s"))
                return name;

            return name + "s";
        }

        public static string GetEntityPropertyPrivateName(this string name)
        {
            return "_" + Clean(Capitalize(name, false));
        }

        public static string XmlEscape(this string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return node.InnerXml;
        }
    }
}
