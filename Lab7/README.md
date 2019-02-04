![gablogo][gablogo]

# Lab 7 - Security - KeyVault

## Goal

Add a KeyVault to the ARM tempalte. migrate the API and storage keys to the keyvault. Use the keyvault instead of the keys in the ARM template and application.

## Let's code!

1. [Create an identity for the App](#1-Create-an-identity-for-the-App)
1. [Create a KeyVault to store secrets](#2-Create-a-keyVault-to-store-secrets)
1. [Give the app access to the KeyVault's secrets](#3-Give-the-app-access-to-the-KeyVault's-secrets)
1. [Move storage account key to KeyVault](#4-Move-storage-account-key-to-KeyVault)
1. [Modify the app to get the Storage Account Keys from KeyVault](#5-Modify-the-app-to-get-the-Storage-Account-Keys-from-KeyVault)
#. [Reference](#Reference)

## 1. Create an identity for the App

In order to access different azure services our app will need an identity.

1. Login to the [azure portal](https://portal.azure.com)
2. Use the search bar to get to the "Active Directory" service

![](medias/search-active-directory.png)

3. On the left side select "App registrations" then click on "New application registration

![](medias/select-app-registration.png)

4. Select a name and assign a Sign-on URL (can be anything and doesn't need to point to a valid website)

![](medias/create-form.png)

5. Create a key for you app, click on settings, then keys, then add a new key and click save

![](medias/create-key.png)

6. Now copy the Application ID, object Id and the newly create key value in a notepad, we'll need them later

![](medias/app-values.png)

## 2. Create a KeyVault to store secrets

1. Open the ARM template we created in [Lab2](../Lab2/README.md)
1. Add a new resource for KeyVault

```json
{
    "type": "Microsoft.KeyVault/vaults",
    "name": "[parameters('keyVaultName')]",
    "apiVersion": "2016-10-01",
    "location": "[resourceGroup().location]",
    "properties": {
        "accessPolicies": [],
        "enabledForDiskEncryption": false,
        "sku": {
            "name": "standard",
            "family": "A"
        },
        "enabledForTemplateDeployment": true,
        "tenantId": "[subscription().tenantId]",
        "enabledForDeployment": true
    },
    "resources": [],
    "dependsOn": []
}
```
> TIP: note that we are using template expressions like `[resourceGroup().location]` and `[subscription().tenantId]` to fill out automatically some values
3. Add a new parameter named `keyVaultName` and select a default value

```json
"keyVaultName": {
    "defaultValue": "gabc-key-vault",
    "type": "string"
}
```

4. Run the script to deploy a KeyVault

## 3. Give the app access to the KeyVault's secrets
In order to retrieve a secret from KeyVault you need to give explicit permission.
1. Modify the ARM template to add an access policy that will give access to our app
```json
"accessPolicies": [
{
    "tenantId" : "[subscription().tenantId]",
    "objectId": "[parameters('applicationObjectId')]",
    "permissions": {
        "secrets": [
            "get",
            "list"
        ]
    }
}]
```
3. Add a new parameter named `applicationObjectId` and set the default value to the `ObjectId` we got from Step 1

```json
"applicationObjectId": {
    "defaultValue": "<YOUR OBJECT ID VALUE HERE>",
    "type": "string"
}
```

## 4. Move storage account key to KeyVault
Wouldn't it be great if the ARM template could provision the Storage Account and save the dynamically generated access keys to keyVault? Let's see how to do it!
1. Modify the ARM template to add a new resource
```json
{
    "type": "Microsoft.KeyVault/vaults/secrets",
    "name": "[concat(parameters('keyVaultName'), '/StorageAccount--ConnectionString')]",
    "apiVersion": "2016-10-01",
    "location": "[resourceGroup().location]",
    "properties": {
        "value": "[listKeys(resourceId('Microsoft.Storage/storageAccounts', parameters('storageAccountName')), providers('Microsoft.Storage', 'storageAccounts').apiVersions[0]).keys[0].value]"
    },
    "resources": [],
    "dependsOn": []
}
```

This resource will create a new secret in key vault by retrieving automatically the value from the storage account, once it's provisionned

## 5. Modify the app to get the Storage Account Keys from KeyVault
Alright now that everything is provisionned correctly in azure, it's time to modify the web application and get the secret from KeyVault instead of using an hardcoded value.

1. 

## Reference

// TODO [listKeys]
// TODO keyvault
// ASP .net configuration KeyVault Provider

## End

[Previous Lab](../Lab6/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
