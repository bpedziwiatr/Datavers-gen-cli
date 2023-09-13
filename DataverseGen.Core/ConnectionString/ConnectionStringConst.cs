namespace DataverseGen.Core.ConnectionString;

public static class ConnectionStringConst
{
	public const string Password = "Password";

	public static readonly HashSet<string> Auth = new()
	{
		"AuthenticationType",
		"AuthType"
	};

	public static readonly HashSet<string> ClientId = new()
	{
		"ClientId",
		"clientid",
		"AppId",
		"appid",
		"ApplicationId",
		"applicationid"
	};

	public static readonly HashSet<string> ClientSecret = new()
	{
		"ClientSecret",
		"clientsecret",
		"Secret",
		"secret"
	};

	public static readonly HashSet<string> IntegratedSecurity = new()
	{
		"Integrated Security",
		"integrated security"
	};

	public static readonly HashSet<string> RedirectUri = new()
	{
		"RedirectUri",
		"redirecturi",
		"ReplyUrl",
		"replyurl"
	};

	public static readonly HashSet<string> TokenCacheStorePath = new()
	{
		"TokenCacheStorePath",
		"Tokencachestorepath"
	};

	public static readonly HashSet<string> Url = new()
	{
		"ServiceUri",
		"serviceuri",
		"Service Uri",
		"service uri",
		"Url",
		"url",
		"Server",
		"server"
	};

	public static readonly HashSet<string> UserName = new()
	{
		"UserName",
		"Username",
		"username",
		"User Name",
		"UserId",
		"userid",
		"User Id"
	};

	internal static readonly HashSet<string> Thumbprint = new()
	{
		"Thumbprint",
		"thumbprint",
		"CertThumbprint",
		"certthumbprint"
	};

	public static string GetToken(
		IDictionary<string, string> tokens,
		HashSet<string> tokenNames)
	{
		string firstValidToken = tokenNames.FirstOrDefault(tokens.ContainsKey);

		return firstValidToken
			?? throw new Exception(
				$"token not found {string.Join(", ", tokenNames)} in {string.Join(", ", tokens.Keys)}");
	}

	public static ConnectionType Map(string authType)
	{
		return authType switch
		{
			"OAuth" => ConnectionType.OAuth,
			"AD" => ConnectionType.AD,
			"IFD" => ConnectionType.IFD,
			"Certificate" => ConnectionType.Certificate,
			"ClientSecret" => ConnectionType.ClientSecret,
			"Office365" => ConnectionType.Office365,
			_ => throw new NotImplementedException($"not implemented authType:{authType}")
		};
	}
}