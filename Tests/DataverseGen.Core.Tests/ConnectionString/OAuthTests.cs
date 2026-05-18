using DataverseGen.Core.ConnectionString;

namespace DataverseGen.Core.Tests.ConnectionString;

[TestClass]
public class OAuthTests
{
	private const string ValidOAuth =
		@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;Password=passcode;" +
		@"Url=https://contosotest.crm.dynamics.com;" +
		@"AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;" +
		@"RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;" +
		@"TokenCacheStorePath=c:\MyTokenCache;LoginPrompt=Auto";

	[TestMethod]
	[DataRow(@"AuthType=OAuth;
  Username=jsmith@contoso.onmicrosoft.com;
  Password=passcode;
  Url=https://contosotest.crm.dynamics.com;
  AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
  RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;
  TokenCacheStorePath=c:\MyTokenCache;
  LoginPrompt=Auto")]
	[DataRow(@"AuthType=OAuth;
  Username=jsmith@contoso.onmicrosoft.com;
  Integrated Security=true;
  Url=https://contosotest.crm.dynamics.com;
  AppId=51f81489-12ee-4a9e-aaae-a2591f45987d;
  RedirectUri=app://58145B91-0C36-4500-8554-080854F2AC97;
  TokenCacheStorePath=c:\MyTokenCache\msal_cache.data;
  LoginPrompt=Auto")]
	public void OAuth_ConnectionString_Success(string connectionString)
	{
		new ConnectionStringValidator(connectionString)
		   .Validate();
	}

	[TestMethod]
	public void OAuth_MissingUrl_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;Password=passcode;" +
			@"AppId=abc;RedirectUri=app://abc;TokenCacheStorePath=c:\cache;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}

	[TestMethod]
	public void OAuth_MissingUsername_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Password=passcode;Url=https://contoso.crm.dynamics.com;" +
			@"AppId=abc;RedirectUri=app://abc;TokenCacheStorePath=c:\cache;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}

	[TestMethod]
	public void OAuth_NoPasswordAndNoIntegratedSecurity_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;" +
			@"Url=https://contoso.crm.dynamics.com;" +
			@"AppId=abc;RedirectUri=app://abc;TokenCacheStorePath=c:\cache;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}

	[TestMethod]
	public void OAuth_IntegratedSecurityFalseWithoutPassword_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;Integrated Security=false;" +
			@"Url=https://contoso.crm.dynamics.com;" +
			@"AppId=abc;RedirectUri=app://abc;TokenCacheStorePath=c:\cache;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}

	[TestMethod]
	public void OAuth_MissingAppId_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;Password=passcode;" +
			@"Url=https://contoso.crm.dynamics.com;" +
			@"RedirectUri=app://abc;TokenCacheStorePath=c:\cache;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}

	[TestMethod]
	public void OAuth_MissingRedirectUri_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;Password=passcode;" +
			@"Url=https://contoso.crm.dynamics.com;" +
			@"AppId=abc;TokenCacheStorePath=c:\cache;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}

	[TestMethod]
	public void OAuth_MissingTokenCacheStorePath_Throws()
	{
		const string cs =
			@"AuthType=OAuth;Username=jsmith@contoso.onmicrosoft.com;Password=passcode;" +
			@"Url=https://contoso.crm.dynamics.com;" +
			@"AppId=abc;RedirectUri=app://abc;";
		Assert.ThrowsException<Exception>(() => new ConnectionStringValidator(cs).Validate());
	}
}