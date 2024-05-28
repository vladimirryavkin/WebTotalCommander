namespace WebTotalComander.Repository.Services;

public class AzureSettings
{
    public string AzureConnection { get; }
    public string AzureContainer { get; }
    public AzureSettings(string azureConnection, string azureContainer)
    {
        AzureConnection = azureConnection;
        AzureContainer = azureContainer;
    }
}
