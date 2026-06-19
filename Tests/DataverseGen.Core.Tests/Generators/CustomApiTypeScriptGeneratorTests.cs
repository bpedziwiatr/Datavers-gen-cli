using DataverseGen.Core.CustomApi;
using DataverseGen.Core.Generators;

namespace DataverseGen.Core.Tests.Generators;

[TestClass]
public class CustomApiTypeScriptGeneratorTests
{
	[TestMethod]
	public void WriteCustomApis_CreatesTypeScriptRequestClass()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiTypeScriptGenerator.WriteCustomApis(tempRoot, new[]
			{
				new CustomApiModel
				{
					RequestClassName = "ProcessStatusRequest",
					OperationName = "newupdatestatus",
					RequestParameters = new[]
					{
						new CustomApiParameterModel
						{
							PropertyName = "new_customer",
							RequestPropertyName = "new_customer",
							ConstructorParameterName = "customerid",
							TypeScriptType = "string[]",
							WebApiTypeName = "Collection(Edm.String)",
							WebApiStructuralProperty = "WebApiRequestStructuralProperty.Collection"
						}
					}
				}
			});

			string filePath = Path.Combine(tempRoot, "customapi", "processstatusrequest.ts");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "export default class ProcessStatusRequest implements IWebApiRequest");
			StringAssert.Contains(content, "private new_customer: string[];");
			StringAssert.Contains(content, "constructor(private customerid: string[])");
			StringAssert.Contains(content, "this.new_customer = customerid;");
			StringAssert.Contains(content, "operationName = \"newupdatestatus\"");
			StringAssert.Contains(content, "typeName: \"Collection(Edm.String)\"");
			StringAssert.Contains(content, "structuralProperty: WebApiRequestStructuralProperty.Collection");
			StringAssert.Contains(content, "new_customer: this.new_customer");
		}
		finally
		{
			if (Directory.Exists(tempRoot))
			{
				Directory.Delete(tempRoot, true);
			}
		}
	}
}
