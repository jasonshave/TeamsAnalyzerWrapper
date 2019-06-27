# Teams Analyzer (API wrapper)

Teams Analyzer is a network assessment tool for measuring network performance and detecting issues with connectivity to Microsoft Teams. Specifically, the system is designed to simiulate a real Teams audio call, measure the network performance, and report the results to your cloud tenant at [teamsanalyzer.com](https://teamsanalyzer.com).

>Using [Microsoft's guidance on Docs](https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-first-function-vs-code), this example was built with Visual Studio Code.

>NOTE: This solution uses [Azure Functions V2 dependency injection](https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection) to provide an `IHttpClientFactory` in accordance with the new methods for avoiding port exhaustion/etc.

## Purpose

Some customers are using Power BI to visualize their data from the teamsanalyzer.com API. Power BI only supports injecting a header value containing the subscription key (Ocp-Apim-Subscription-Key) value in the desktop version and since the key is required to authenticate to the API, it becomes difficult to use Power BI's live feed feature(s).

This set of wrapper functions were built to help customers get up and running quickly with a set of Azure Functions which call each of the reporting API's while hiding the subscription key in Azure Key Vault or within the function's application settings.

## Security

Each Azure Function in this solution uses the "function keys" security model as described [in the Microsoft Docs website](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-http-webhook#trigger---usage).

>NOTE: Following the steps in this guide will make it possible for anyone to execute your reporting API's if they have the function key. Since the article describes how to wrap the API calls with a pre-populated Teams Analyzer Subscription Key, there will be no other means of authentication. While this is true, someone would need your domain name and potentially a valid node ID as well (depending on the API call).

**WARNING:**
>Do not change the security of the function to anonymous since anyone who tries your function URL will be able to execute calls against your tenant.

## Configuration

This repository can be cloned and opened/deployed to your Azure tenant with minimal configuration. You have two options to choose from; using the "Application Settings" of the Azure function directly, or more securely, in Azure Key Vault.

### Using `local.settings.json` file

Starting out with a local development environment you likely won't have Key Vault set up yet and you will need some place to store secrets. You can use the `local.settings.json` file to store these as this file is ignored by Git. Running your functions locally will use the local file, whereas when you deploy to Azure, the **Application Settings** will take affect.

### Using Azure Functions Application Settings in Azure

1. Obtain your teamsanalyzer.com reporting API key from dev.teamsanalyzer.com.

2. Once you've deployed this solution and associated functions to Azure, [follow the instructions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-how-to-use-azure-function-app-settings) on how to add a new setting.

3. The setting should have a name of "TeamsAnalyzerSubscriptionKey" so that it matches the constant defined in each function. For example:

```c#
private static string key = Environment.GetEnvironmentVariable("TeamsAnalyzerSubscriptionKey");
```

4. The value for the setting will be your reporting API subscription key.

### Using Azure Key Vault (recommended)

Since Azure Key Vault is 'cheap as chips' we highly recommend storing secrets there. This way you can use [Access Control (IAM)](https://docs.microsoft.com/en-ca/azure/role-based-access-control/check-access) to apply RBAC policies to control access to the secrets separate from the Azure Functions themselves.

First, we need to create a managed service identity for the function to authenticate to the key vault:

1. From the Azure portal, locate your function and click on the tab labeled **Platform features**.

2. Click the **Identity** link and enable the System-assigned Managed Service Identity. This will create an entity within your Azure AD environment we can use to set the permission on the key vault.

Now we need to assign the identity permission to the secret:

1. Follow the [quickstart](https://docs.microsoft.com/en-us/azure/key-vault/quick-create-portal) if you haven't created a Key Vault already.

2. Create a secret called `TeamsAnalyzerSubscriptionKey` and paste in your teamsanalyzer.com reporting API subscription key.

3. Once the secret is created you'll need the FQDN to reference it in your Azure Function so click on the secret, then click on the GUID representing the version. This should bring you to the current version of the secret where you'll see a URI for the secret identifier.

4. Click the copy button to copy the secret FQDN.

5. Navigate back to your Key Vault so we can add permissions.

6. Click on **Access Policies** then click **Add new**.

7. Click **Select principal** and search for the name of your Azure Function app.

8. Under **Key Permissions** and under the section **Key Management Operations** choose only `Get` and click OK to complete the operation.

The last step is to point the Azure Function at the secret:

1. Going back to your Azure Function, click on **Configuration** and this will take you to the application settings for the function.

2. Click the **New application setting** button:
   - For the **name**, type: `TeamsAnalyzerSubscriptionKey`
   - For the **value**, type: `@Microsoft.KeyVault(SecretUri=**YOUR_SECRET_FQDN_GOES_HERE**)`

## Usage in Power BI

>More details to follow in this section soon. However, the query URL will need to be updated for anyone using the existing `.pibx` file. The URL to get data will be the path to your Function App.

## Potential issues

1. Each call you make to your own Azure Function will also make a call to Azure Key Vault. There are [limits to how many calls](https://docs.microsoft.com/en-us/azure/key-vault/key-vault-ovw-throttling) you can make in succession so you may want to consider throttling your functions if you encounter this issue.
