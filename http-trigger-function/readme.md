# Http Trigger Function

An Azure Function with an Http Trigger that writes entries into a Storage Table.

Create a resource group:

```bash
$rg="<your-resource-group-name>"
az group create -g $rg
```

Deploy ARM template:

```bash
$appname="<your-app-name>"
az group deployment create -g $rg --template-file deploy.json --parameters appName=$appname
```

If you don't have the Azure Function Core Tools installed:

```bash
npm install -g azure-functions-core-tools
```

Publish function code:

```bash
func azure functionapp publish $appname
```

## Additional References:
* https://docs.microsoft.com/en-us/azure/cosmos-db/tutorial-develop-table-dotnet
* https://github.com/Azure/azure-quickstart-templates/blob/master/101-function-app-create-dedicated/azuredeploy.json