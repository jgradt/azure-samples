# Http Trigger Function with RBAC

An Azure Function with an Http Trigger that writes entries into a Storage Blob.  This uses Azure Identity to retrieve access 
credentials to write to the blob.  A System Identity and Role Based Access Control are used to assign and control access to 
the storage account.

Create a resource group:

```powershell
$rg="<your-resource-group-name>"
az group create -g $rg
```

Deploy ARM template:

```powershell
$appname="<your-app-name>"
$deployment=$(az group deployment create -g $rg --template-file deploy.json --parameters appName=$appname)
```

If you don't have the Azure Function Core Tools installed:

```powershell
npm install -g azure-functions-core-tools
```

Publish function code:

```powershell
func azure functionapp publish $appname
```

Get deployment outputs

```powershell
# Powershell
$principalid=$($deployment | convertfrom-json).properties.outputs.principalId.value
$blobStorageid=$($deployment | convertfrom-json).properties.outputs.blobStorage.value
```
OR
```bash
# bash
principalid=$(echo $deployment | jq -r 'properties.outputs.principalId.value')
blobStorageid=$(echo $deployment | jq -r 'properties.outputs.blobStorage.value')
```

Create RBAC assignments

```powershell
az role assignment create --assignee $principalid --role "Storage Blob Data Contributor" --scope $blobStorageid
```


## Additional References:
* https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/identity/Azure.Identity