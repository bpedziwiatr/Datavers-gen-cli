namespace DataverseGen.Core.ConnectionString.Validators;

internal static class UrlValidator
{
	public static bool ValidUrl(string url)
	{
		return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
			&&
			(
				uriResult.Scheme == Uri.UriSchemeHttp
				|| uriResult.Scheme == Uri.UriSchemeHttps
			);
	}
}