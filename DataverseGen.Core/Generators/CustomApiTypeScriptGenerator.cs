using System.Text;
using DataverseGen.Core.CustomApi;
using DataverseGen.Core.Extensions;

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
		CustomApiParameterModel? boundParameter = GetBoundParameter(customApi);

		if (boundParameter != null)
		{
			constructorParameters.Insert(0, boundParameter);
		}

		sb.AppendLine($"export default class {className} implements IWebApiRequest {{");
		sb.AppendLine("\tprivate operationName: string;");

		foreach (CustomApiParameterModel parameter in constructorParameters)
		{
			sb.AppendLine($"\tprivate {parameter.PropertyName}: {parameter.TypeScriptType};");
		}

		sb.AppendLine();
		sb.Append($"\tconstructor(");
		sb.Append(string.Join(", ", constructorParameters.Select(p => $"private {p.PropertyName}: {p.TypeScriptType}")));
		sb.AppendLine(") {");
		sb.AppendLine($"\t\tthis.operationName = \"{EscapeString(customApi.OperationName)}\";");

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\t\tthis.{parameter.PropertyName} = {parameter.PropertyName};");
		}

		sb.AppendLine("\t}");
		sb.AppendLine("\tgetInternalRequest(): IWebApiInternalRequest {");
		sb.AppendLine("\t\treturn {");

		if (boundParameter != null)
		{
			sb.AppendLine($"\t\t\t{boundParameter.PropertyName}: this.{boundParameter.PropertyName},");
		}

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\t\t\t{parameter.PropertyName}: this.{parameter.PropertyName},");
		}

		sb.AppendLine("\t\t\tgetMetadata: () => ({");
		sb.AppendLine($"\t\t\t\tboundParameter: {GetBoundParameterName(boundParameter)},");
		sb.AppendLine("\t\t\t\tparameterTypes: {");

		if (boundParameter != null)
		{
			sb.AppendLine($"\t\t\t\t\t{boundParameter.PropertyName}: {{");
			sb.AppendLine($"\t\t\t\t\t\ttypeName: \"{boundParameter.WebApiTypeName}\",");
			sb.AppendLine($"\t\t\t\t\t\tstructuralProperty: {boundParameter.WebApiStructuralProperty}");
			sb.AppendLine("\t\t\t\t\t},");
		}

		foreach (CustomApiParameterModel parameter in customApi.RequestParameters)
		{
			sb.AppendLine($"\t\t\t\t\t{parameter.PropertyName}: {{");
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

	private static CustomApiParameterModel? GetBoundParameter(CustomApiModel customApi)
	{
		if (string.IsNullOrWhiteSpace(customApi.BoundEntityLogicalName))
		{
			return null;
		}

		return new CustomApiParameterModel
		{
			PropertyName = "entity",
			TypeScriptType = "any",
			WebApiTypeName = $"mscrm.{customApi.BoundEntityLogicalName}",
			WebApiStructuralProperty = "WebApiRequestStructuralProperty.EntityType"
		};
	}

	private static string GetBoundParameterName(CustomApiParameterModel? boundParameter)
	{
		return boundParameter == null
			? "null"
			: $"\"{EscapeString(boundParameter.PropertyName)}\"";
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
