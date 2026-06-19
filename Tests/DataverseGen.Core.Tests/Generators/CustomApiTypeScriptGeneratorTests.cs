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

	[TestMethod]
	public void WriteCustomApis_ReturnsWhenEnumerableIsNull()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiTypeScriptGenerator.WriteCustomApis(tempRoot, (IEnumerable<CustomApiModel>)null!);

			Assert.IsFalse(Directory.Exists(Path.Combine(tempRoot, "customapi")));
		}
		finally
		{
			if (Directory.Exists(tempRoot))
			{
				Directory.Delete(tempRoot, true);
			}
		}
	}

	[TestMethod]
	public void WriteCustomApis_CreatesFunctionRequestClassWithoutParameters()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiTypeScriptGenerator.WriteCustomApis(tempRoot, new[]
			{
				new CustomApiModel
				{
					RequestClassName = "GetStatusRequest",
					OperationName = "newgetstatus",
					IsFunction = true
				}
			});

			string filePath = Path.Combine(tempRoot, "customapi", "getstatusrequest.ts");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "constructor()");
			StringAssert.Contains(content, "boundParameter: null");
			StringAssert.Contains(content, "operationType: WebApiRequestOperationType.Function");
		}
		finally
		{
			if (Directory.Exists(tempRoot))
			{
				Directory.Delete(tempRoot, true);
			}
		}
	}

	[TestMethod]
	public void WriteCustomApis_IncludesBoundParameter()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiTypeScriptGenerator.WriteCustomApis(tempRoot, new[]
			{
				new CustomApiModel
				{
					RequestClassName = "BoundRequest",
					OperationName = "newbound",
					BoundEntityLogicalName = "account"
				}
			});

			string filePath = Path.Combine(tempRoot, "customapi", "boundrequest.ts");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "private entity: any;");
			StringAssert.Contains(content, "constructor(entity: any)");
			StringAssert.Contains(content, "boundParameter: \"entity\"");
			StringAssert.Contains(content, "entity: this.entity");
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
