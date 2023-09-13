namespace DataverseGen.Core.ConnectionString.Validators;

public class CertificateBaseConnectionStringValidator : BaseConnectionStringValidator,
	IConnectionStringValidator
{
	public CertificateBaseConnectionStringValidator(IDictionary<string, string> connectionStringTokens) : base(
		connectionStringTokens) { }

	public bool Validate()
	{
		CheckIfThumbprintIsPresent();
		CheckIfClientIdIsPresent();

		return true;
	}

	private void CheckIfClientIdIsPresent()
	{
		CheckIfTokenIsPresentWithValue(ConnectionStringConst.ClientId);
	}

	private void CheckIfThumbprintIsPresent()
	{
		CheckIfTokenIsPresentWithValue(ConnectionStringConst.Thumbprint);
	}
}