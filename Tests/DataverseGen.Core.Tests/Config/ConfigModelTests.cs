using DataverseGen.Core.Config;

namespace DataverseGen.Core.Tests.Config;

[TestClass]
public class ConfigModelTests
{
	[TestMethod]
	public void ConfigModel_DefaultValues_AreFalse()
	{
		var model = new ConfigModel();

		Assert.IsFalse(model.ThrowOnEntityNotFound);
		Assert.IsFalse(model.EnableConnectionStringValidation);
	}

	[TestMethod]
	public void ConfigModel_AllPropertiesAreSettable()
	{
		var model = new ConfigModel
		{
			ConnectionString = "AuthType=AD;Url=https://contoso.crm.dynamics.com",
			Entities = new[] { "account", "contact" },
			CustomApis = new[] { "newupdatestatus" },
			Namespace = "MyApp.DataModel",
			OutDirectory = @"C:\output",
			TemplateName = "scriban",
			TemplateDirectoryName = "Templates",
			ThrowOnEntityNotFound = true,
			EnableConnectionStringValidation = true,
			TemplateEngine = new TemplateEngineModel
			{
				IsSingleOutput = true,
				Name = "scriban",
				Type = "C#"
			}
		};

		Assert.AreEqual("AuthType=AD;Url=https://contoso.crm.dynamics.com", model.ConnectionString);
		CollectionAssert.AreEqual(new[] { "account", "contact" }, model.Entities);
		CollectionAssert.AreEqual(new[] { "newupdatestatus" }, model.CustomApis);
		Assert.AreEqual("MyApp.DataModel", model.Namespace);
		Assert.AreEqual(@"C:\output", model.OutDirectory);
		Assert.AreEqual("scriban", model.TemplateName);
		Assert.AreEqual("Templates", model.TemplateDirectoryName);
		Assert.IsTrue(model.ThrowOnEntityNotFound);
		Assert.IsTrue(model.EnableConnectionStringValidation);
	}

	[TestMethod]
	public void TemplateEngineModel_AllPropertiesAreSettable()
	{
		var model = new TemplateEngineModel
		{
			IsSingleOutput = true,
			Name = "scriban",
			Type = "TypeScript"
		};

		Assert.IsTrue(model.IsSingleOutput);
		Assert.AreEqual("scriban", model.Name);
		Assert.AreEqual("TypeScript", model.Type);
	}

	[TestMethod]
	public void TemplateEngineModel_DefaultValues_AreFalseOrNull()
	{
		var model = new TemplateEngineModel();

		Assert.IsFalse(model.IsSingleOutput);
		Assert.IsNull(model.Name);
		Assert.IsNull(model.Type);
	}

	[TestMethod]
	public void ConfigValidation_AllowsCustomApisForCsOutput()
	{
		var model = new ConfigModel
		{
			CustomApis = new[] { "newupdatestatus" },
			TemplateEngine = new TemplateEngineModel
			{
				Type = "C#"
			}
		};

		ConfigValidation.Validate(model);
	}

	[TestMethod]
	public void ConfigValidation_ThrowsWhenConfigIsNull()
	{
		Assert.ThrowsException<ArgumentNullException>(() => ConfigValidation.Validate(null));
	}

	[TestMethod]
	public void ConfigValidation_ThrowsWhenTemplateEngineIsMissing()
	{
		var model = new ConfigModel();

		Assert.ThrowsException<InvalidOperationException>(() => ConfigValidation.Validate(model));
	}

	[TestMethod]
	public void ConfigValidation_AllowsCustomApisForTsOutput()
	{
		var model = new ConfigModel
		{
			CustomApis = new[] { "newupdatestatus" },
			TemplateEngine = new TemplateEngineModel
			{
				Type = "ts"
			}
		};

		ConfigValidation.Validate(model);
	}
}
