using System.Text;
using DataverseGen.Core.CustomApi;

namespace DataverseGen.Core.Generators;

public static class CustomApiTypeScriptGenerator
{
	public static void WriteCustomApis(string outPath, IEnumerable<CustomApiModel> customApis)
	{
		if (customApis == null)
		{
			return;
		}

		string customApiDirectory = Path.Combine(outPath, "customapi");
		Directory.CreateDirectory(customApiDirectory);

		foreach (CustomApiModel customApi in customApis)
		{
			string filePath = Path.Combine(customApiDirectory, $"{customApi.RequestClassName.ToLowerInvariant()}.ts");
			File.WriteAllText(filePath, RenderRequestClass(customApi), Encoding.UTF8);
		}
	}

	internal static string RenderRequestClass(CustomApiModel customApi)
	{
		StringBuilder sb = new();
		string className = customApi.RequestClassName;
		List<CustomApiParameterModel> constructorParameters = customApi.RequestParameters.ToList();
		CustomApiParameterModel boundParameter = GetBoundParameter(customApi);

		sb.AppendLine($"export default class {className} implements IWebApiRequest {{");
		sb.AppendLine("\tprivate operationName: string;");

		if (boundParameter != null)
		{
			sb.AppendLine($"\tprivate {boundParameter.RequestPropertyName}: {boundParameter.TypeScriptType};");
		}

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\tprivate {parameter.RequestPropertyName}: {parameter.TypeScriptType};");
		}

		sb.AppendLine();
		sb.Append($"\tconstructor(");
		sb.Append(string.Join(", ", constructorParameters.Select(p => $"private {p.ConstructorParameterName}: {p.TypeScriptType}")));
		if (boundParameter != null)
		{
			if (constructorParameters.Count > 0)
			{
				sb.Append(", ");
			}
			sb.Append($"{boundParameter.ConstructorParameterName}: {boundParameter.TypeScriptType}");
		}
		sb.AppendLine(") {");
		sb.AppendLine($"\t\tthis.operationName = \"{EscapeString(customApi.OperationName)}\";");

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\t\tthis.{parameter.RequestPropertyName} = {parameter.ConstructorParameterName};");
		}

		if (boundParameter != null)
		{
			sb.AppendLine($"\t\tthis.{boundParameter.RequestPropertyName} = {boundParameter.ConstructorParameterName};");
		}

		sb.AppendLine("\t}");
		sb.AppendLine("\tgetInternalRequest(): IWebApiInternalRequest {");
		sb.AppendLine("\t\treturn {");

		if (boundParameter != null)
		{
			sb.AppendLine($"\t\t\t{boundParameter.RequestPropertyName}: this.{boundParameter.RequestPropertyName},");
		}

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\t\t\t{parameter.RequestPropertyName}: this.{parameter.RequestPropertyName},");
		}

		sb.AppendLine("\t\t\tgetMetadata: () => ({");
		sb.AppendLine($"\t\t\t\tboundParameter: {GetBoundParameterName(boundParameter)},");
		sb.AppendLine("\t\t\t\tparameterTypes: {");

		if (boundParameter != null)
		{
			sb.AppendLine($"\t\t\t\t\t{boundParameter.RequestPropertyName}: {{");
			sb.AppendLine($"\t\t\t\t\t\ttypeName: \"{boundParameter.WebApiTypeName}\",");
			sb.AppendLine($"\t\t\t\t\t\tstructuralProperty: {boundParameter.WebApiStructuralProperty}");
			sb.AppendLine("\t\t\t\t\t},");
		}

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\t\t\t\t\t{parameter.RequestPropertyName}: {{");
			sb.AppendLine($"\t\t\t\t\t\ttypeName: \"{parameter.WebApiTypeName}\",");
			sb.AppendLine($"\t\t\t\t\t\tstructuralProperty: {parameter.WebApiStructuralProperty}");
			sb.AppendLine("\t\t\t\t\t},");
		}

		sb.AppendLine("\t\t\t\t},");
		sb.AppendLine($"\t\t\t\toperationType: {GetOperationType(customApi)},");
		sb.AppendLine("\t\t\t\toperationName: this.operationName");
		sb.AppendLine("\t\t\t})");
		sb.AppendLine("\t\t} as IWebApiInternalRequest;");
		sb.AppendLine("\t}");
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
			PropertyName = "entity",
			RequestPropertyName = "entity",
			ConstructorParameterName = "entity",
			TypeScriptType = "any",
			WebApiTypeName = $"mscrm.{customApi.BoundEntityLogicalName}",
			WebApiStructuralProperty = "WebApiRequestStructuralProperty.EntityType"
		};
	}

	private static string GetBoundParameterName(CustomApiParameterModel boundParameter)
	{
		return boundParameter == null
			? "null"
			: $"\"{EscapeString(boundParameter.RequestPropertyName)}\"";
	}

	private static string GetOperationType(CustomApiModel customApi)
	{
		return customApi.IsFunction
			? "WebApiRequestOperationType.Function"
			: "WebApiRequestOperationType.Action";
	}

	private static string EscapeString(string value)
	{
		return string.IsNullOrEmpty(value)
			? string.Empty
			: value.Replace("\\", "\\\\").Replace("\"", "\\\"");
	}
}
