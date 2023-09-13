namespace DataverseGen.Core.ConnectionString.Validators;

public class ClientSecretConnectionStringValidator : BaseConnectionStringValidator,
	IConnectionStringValidator
{
	public ClientSecretConnectionStringValidator(IDictionary<string, string> connectionStringTokens)
		: base(connectionStringTokens) { }

	public bool Validate()
	{
		CheckIfUrlIsPresentAndValid();
		CheckIfClientIdIsPresent();
		CheckIfClientSecretIdIsPresent();

		return true;
	}

	private void CheckIfClientIdIsPresent()
	{
		CheckIfTokenIsPresentWithValue(ConnectionStringConst.ClientId);
	}

	private void CheckIfClientSecretIdIsPresent()
	{
		CheckIfTokenIsPresentWithValue(ConnectionStringConst.ClientSecret);
	}
}