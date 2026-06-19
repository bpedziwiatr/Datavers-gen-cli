using System.Text;
using DataverseGen.Core.CustomApi;
using DataverseGen.Core.Extensions;

namespace DataverseGen.Core.Generators;

public static class CustomApiCSharpGenerator
{
	public static void WriteCustomApis(string outPath, string namespaceName, IEnumerable<CustomApiModel> customApis)
	{
		if (customApis == null)
		{
			return;
		}

		string customApiDirectory = Path.Combine(outPath, "customapi");
		Directory.CreateDirectory(customApiDirectory);

		foreach (CustomApiModel customApi in customApis)
		{
			string filePath = Path.Combine(customApiDirectory, $"{customApi.RequestClassName.ToLowerInvariant()}.cs");
			File.WriteAllText(filePath, RenderRequestClass(namespaceName, customApi), Encoding.UTF8);
		}
	}

	internal static string RenderRequestClass(string namespaceName, CustomApiModel customApi)
	{
		StringBuilder sb = new();
		string className = customApi.RequestClassName;
		List<CustomApiParameterModel> constructorParameters = customApi.RequestParameters.ToList();
		CustomApiParameterModel boundParameter = GetBoundParameter(customApi);

		sb.AppendLine("using System;");
		sb.AppendLine("using Microsoft.Xrm.Sdk;");
		sb.AppendLine();
		sb.AppendLine($"namespace {namespaceName};");
		sb.AppendLine();
		sb.AppendLine($"public sealed class {className} : OrganizationRequest");
		sb.AppendLine("{");
		sb.AppendLine($"\tpublic const string RequestNameValue = \"{EscapeString(customApi.OperationName)}\";");
		sb.AppendLine();

		if (constructorParameters.Count > 0 || boundParameter != null)
		{
			sb.Append($"\tpublic {className}(");
			sb.Append(string.Join(", ", constructorParameters.Select(p => $"{GetCSharpType(p)} {GetParameterName(p)}")));

			if (boundParameter != null)
			{
				if (constructorParameters.Count > 0)
				{
					sb.Append(", ");
				}

				sb.Append($"EntityReference {GetParameterName(boundParameter)}");
			}

			sb.AppendLine(")");
			sb.AppendLine("\t{");

			sb.AppendLine("\t\tRequestName = RequestNameValue;");

			foreach (CustomApiParameterModel parameter in constructorParameters)
			{
				sb.AppendLine($"\t\t{GetPropertyName(parameter)} = {GetParameterName(parameter)};");
			}

			if (boundParameter != null)
			{
				sb.AppendLine($"\t\t{GetPropertyName(boundParameter)} = {GetParameterName(boundParameter)};");
			}

			sb.AppendLine("\t}");
			sb.AppendLine();
		}
		else
		{
			sb.AppendLine($"\tpublic {className}()");
			sb.AppendLine("\t{");
			sb.AppendLine("\t\tRequestName = RequestNameValue;");
			sb.AppendLine("\t}");
			sb.AppendLine();
		}

		if (boundParameter != null)
		{
			sb.AppendLine($"\tpublic EntityReference {GetPropertyName(boundParameter)}");
			sb.AppendLine("\t{");
			sb.AppendLine($"\t\tget => Parameters.ContainsKey(\"{EscapeString(boundParameter.RequestPropertyName)}\")");
			sb.AppendLine($"\t\t\t? (EntityReference)Parameters[\"{EscapeString(boundParameter.RequestPropertyName)}\"]");
			sb.AppendLine("\t\t\t: default;");
			sb.AppendLine($"\t\tset => Parameters[\"{EscapeString(boundParameter.RequestPropertyName)}\"] = value;");
			sb.AppendLine("\t}");
			sb.AppendLine();
		}

		foreach (CustomApiParameterModel parameter in constructorParameters)
		{
			sb.AppendLine($"\tpublic {GetCSharpType(parameter)} {GetPropertyName(parameter)}");
			sb.AppendLine("\t{");
			sb.AppendLine($"\t\tget => Parameters.ContainsKey(\"{EscapeString(parameter.RequestPropertyName)}\")");
			sb.AppendLine($"\t\t\t? ({GetCSharpType(parameter)})Parameters[\"{EscapeString(parameter.RequestPropertyName)}\"]");
			sb.AppendLine("\t\t\t: default;");
			sb.AppendLine($"\t\tset => Parameters[\"{EscapeString(parameter.RequestPropertyName)}\"] = value;");
			sb.AppendLine("\t}");
			sb.AppendLine();
		}

		sb.AppendLine("}");

		return sb.ToString();
	}

	private static CustomApiParameterModel GetBoundParameter(CustomApiModel customApi)
	{
		if (string.IsNullOrWhiteSpace(customApi.BoundEntityLogicalName))
		{
			return null;
		}

		return new CustomApiParameterModel
		{
			DisplayName = "Entity",
			UniqueName = "entity",
			PropertyName = "entity",
			RequestPropertyName = "entity",
			ConstructorParameterName = "entity",
			TypeScriptType = "any",
			CSharpTypeName = "EntityReference",
			WebApiTypeName = $"mscrm.{customApi.BoundEntityLogicalName}",
			WebApiStructuralProperty = "WebApiRequestStructuralProperty.EntityType"
		};
	}

	private static string GetPropertyName(CustomApiParameterModel parameter)
	{
		string source = string.IsNullOrWhiteSpace(parameter.DisplayName)
			? parameter.UniqueName
			: parameter.DisplayName;

		string sanitized = MetadataNamingExtensions.GetProperVariableName(source);
		if (sanitized.StartsWith("_", StringComparison.InvariantCulture) &&
			sanitized.Length > 1 &&
			char.IsDigit(sanitized[1]))
		{
			return sanitized;
		}

		return ToPascalCase(sanitized);
	}

	private static string GetParameterName(CustomApiParameterModel parameter)
	{
		string propertyName = GetPropertyName(parameter);

		if (string.IsNullOrWhiteSpace(propertyName))
		{
			return "value";
		}

		string parameterName = char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
		return IsCSharpKeyword(parameterName)
			? $"@{parameterName}"
			: parameterName;
	}

	private static string GetCSharpType(CustomApiParameterModel parameter)
	{
		return string.IsNullOrWhiteSpace(parameter.CSharpTypeName)
			? "string"
			: parameter.CSharpTypeName;
	}

	private static string EscapeString(string value)
	{
		return string.IsNullOrEmpty(value)
			? string.Empty
			: value.Replace("\\", "\\\\").Replace("\"", "\\\"");
	}

	private static string ToPascalCase(string value)
	{
		if (string.IsNullOrWhiteSpace(value))
		{
			return string.Empty;
		}

		char[] separators = [' ', '_', '-', '.', '/', '\\'];
		string[] parts = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
		StringBuilder builder = new();

		for (int i = 0; i < parts.Length; i++)
		{
			string part = parts[i].Trim();

			if (part.Length == 0)
			{
				continue;
			}

			builder.Append(char.ToUpperInvariant(part[0]));
			if (part.Length > 1)
			{
				builder.Append(part[1..]);
			}
		}

		return builder.ToString();
	}

	private static bool IsCSharpKeyword(string value)
	{
		return value.Equals("public", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("private", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("single", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("new", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("partial", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("to", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("error", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("readonly", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("case", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("object", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("global", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("true", StringComparison.InvariantCultureIgnoreCase)
			|| value.Equals("false", StringComparison.InvariantCultureIgnoreCase);
	}
}
