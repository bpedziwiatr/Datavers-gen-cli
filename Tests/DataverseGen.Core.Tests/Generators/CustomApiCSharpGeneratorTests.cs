using DataverseGen.Core.CustomApi;
using DataverseGen.Core.Generators;

namespace DataverseGen.Core.Tests.Generators;

[TestClass]
public class CustomApiCSharpGeneratorTests
{
	[TestMethod]
	public void WriteCustomApis_CreatesCSharpRequestClass()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiCSharpGenerator.WriteCustomApis(tempRoot,
				"MyApp.CustomApis",
				new[]
				{
					new CustomApiModel
					{
						RequestClassName = "ProcessStatusRequest",
						OperationName = "newupdatestatus",
						RequestParameters = new[]
						{
							new CustomApiParameterModel
							{
								DisplayName = "Customer Id",
								UniqueName = "customerid",
								PropertyName = "customerid",
								RequestPropertyName = "customerid",
								ConstructorParameterName = "customerid",
								CSharpTypeName = "string"
							}
						}
					}
				});

			string filePath = Path.Combine(tempRoot, "customapi", "processstatusrequest.cs");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "namespace MyApp.CustomApis;");
			StringAssert.Contains(content, "public sealed class ProcessStatusRequest : OrganizationRequest");
			StringAssert.Contains(content, "public const string RequestNameValue = \"newupdatestatus\";");
			StringAssert.Contains(content, "public string CustomerId");
			StringAssert.Contains(content, "RequestName = RequestNameValue;");
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
	public void WriteCustomApis_IncludesBoundParameterAndMultipleProperties()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiCSharpGenerator.WriteCustomApis(tempRoot,
				"MyApp.CustomApis",
				new[]
				{
					new CustomApiModel
					{
						RequestClassName = "ProcessStatusRequest",
						OperationName = "newupdatestatus",
						BoundEntityLogicalName = "account",
						RequestParameters = new[]
						{
							new CustomApiParameterModel
							{
								DisplayName = "Flag",
								UniqueName = "flag",
								PropertyName = "flag",
								RequestPropertyName = "flag",
								ConstructorParameterName = "flag",
								CSharpTypeName = "bool"
							},
							new CustomApiParameterModel
							{
								DisplayName = "Count",
								UniqueName = "count",
								PropertyName = "count",
								RequestPropertyName = "count",
								ConstructorParameterName = "count",
								CSharpTypeName = "int"
							}
						}
					}
				});

			string filePath = Path.Combine(tempRoot, "customapi", "processstatusrequest.cs");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "public EntityReference Entity");
			StringAssert.Contains(content, "public ProcessStatusRequest(bool flag, int count, EntityReference entity)");
			StringAssert.Contains(content, "public bool Flag");
			StringAssert.Contains(content, "public int Count");
			StringAssert.Contains(content, "Parameters[\"entity\"] = value;");
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
	public void WriteCustomApis_CreatesParameterlessRequestClass()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiCSharpGenerator.WriteCustomApis(tempRoot,
				"MyApp.CustomApis",
				new[]
				{
					new CustomApiModel
					{
						RequestClassName = "PingRequest",
						OperationName = "newping"
					}
				});

			string filePath = Path.Combine(tempRoot, "customapi", "pingrequest.cs");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "public PingRequest()");
			StringAssert.Contains(content, "RequestName = RequestNameValue;");
			Assert.IsFalse(content.Contains("public EntityReference Entity"));
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
	public void WriteCustomApis_EscapesCSharpKeywordParameters()
	{
		string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
		Directory.CreateDirectory(tempRoot);

		try
		{
			CustomApiCSharpGenerator.WriteCustomApis(tempRoot,
				"MyApp.CustomApis",
				new[]
				{
					new CustomApiModel
					{
						RequestClassName = "KeywordRequest",
						OperationName = "newkeyword",
						RequestParameters = new[]
						{
							new CustomApiParameterModel
							{
								DisplayName = "Private",
								UniqueName = "private",
								PropertyName = "private",
								RequestPropertyName = "private",
								ConstructorParameterName = "private",
								CSharpTypeName = "string"
							}
						}
					}
				});

			string filePath = Path.Combine(tempRoot, "customapi", "keywordrequest.cs");
			string content = File.ReadAllText(filePath);

			StringAssert.Contains(content, "public string Private");
			StringAssert.Contains(content, "public KeywordRequest(string @private)");
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
			CustomApiCSharpGenerator.WriteCustomApis(tempRoot, "MyApp.CustomApis", (IEnumerable<CustomApiModel>)null!);

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
}
