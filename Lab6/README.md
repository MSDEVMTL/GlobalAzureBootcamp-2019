![gablogo][gablogo]

# Lab 6 - Serverless - Azure Function

## Goal

Create an Azure Functions (blob trigger) to process all new images into a blob storage. The Function will use the Vision API to keep only the dogs pictures.

## Let's code!

In the previous Lab we learn how to use the Cognitive Services to add great functionalities to our application very easily. 

In many real scenarios we can't or we don't have access to the "main" application. In those situations (and many others) Azure Functions are fantastic. In this Lab we will leverage the Serverless offer of Azure and create an Azure Function.

The function will be triggered every times a new file is created in the Azure Blob Storage. It will then call the same Cognitive Services created in [Lab 5](https://github.com/MSDEVMTL/GlobalAzureBootcamp-2019/tree/master/Lab5#modify-the-arm-template-to-add-the-computer-vision-service). The Function will then examine the result and delete any file that is NOT a dog.

It's a good practice to keep these kind of component in a different life cycle. Therefore, we will create another CI/CD Pipeline to build and deploy this project.

> To keep the level of complexity of this Lab accessible for all we will reused the same ARM template. However, in a real environment it would be suggested to have three different template. 
> 1. A template creating the resources required for the webApplication
> 1. A template creating the Azure Function and the Cognitive Services
> 1. A main template that use nested call to references the previous one.
> Deploying the main template will deploy the entire solution. Deploy one or the other sub-template will only deploy a subsets of resources.
> You can learn more by reading the documentation: [Using linked and nested templates when deploying Azure resources](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-linked-templates)

# Modify the ARM template to add the Azure Function

Just like we did in Lab 5, open the existing [ARM template](../Lab2/deployment/gab2019.json) from lab2. We will start by creating a new variable to set the name of our Function App. 

In the array of variable add a new variable named `funcAppName`: 

    "funcAppName": "[concat(parameters('webAppName'), '-funcApp')]"

This will reuse the name received in parameter `webAppName` and add `-funcApp` to it. We will use this as the Function App name.

Now let's add the Function App. Under the resources array in the ARM template add the following snippet we will explain it after:

    {
      "apiVersion": "2015-08-01",
      "name": "[variables('funcAppName')]",
      "type": "Microsoft.Web/sites",
      "kind": "functionapp",
      "location": "[resourceGroup().location]",
      "properties": {
         "serverFarmId": "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]",
         "siteConfig": {
            "alwaysOn": false,
            "appSettings": [
                { "name": "FUNCTIONS_EXTENSION_VERSION", "value": "~2" }
            ]
         }
      },
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]",
        "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
      ],
      "resources": [
         {
            "apiVersion": "2015-08-01",
            "name": "appsettings",
            "type": "config",
            "dependsOn": [
              "[resourceId('Microsoft.Web/sites', variables('funcAppName'))]",
              "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
            ],
            "properties": {
              "FUNCTIONS_EXTENSION_VERSION":"~2",
              "AzureWebJobsStorage": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('StorageAccountName'),'2015-05-01-preview').key1)]",
              "AzureWebJobsDashboard": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('StorageAccountName'),'2015-05-01-preview').key1)]",
              "ComputerVision:Endpoint":"[reference(parameters('csVisionName'), '2017-04-18').endpoint]",
              "ComputerVision:ApiKey":"[listKeys(parameters('csVisionName'), '2017-04-18').key1]"
            }
         },
         {
          "apiVersion": "2018-02-01",
          "type": "config",
          "name": "connectionstrings",
          "dependsOn": [
            "[resourceId('Microsoft.Web/sites', variables('funcAppName'))]",
            "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
          ],
          "properties": {
            "AzureWebJobsStorage": {
              "value": "[Concat('DefaultEndpointsProtocol=https;AccountName=',variables('StorageAccountName'),';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts', variables('StorageAccountName')), providers('Microsoft.Storage', 'storageAccounts').apiVersions[0]).keys[0].value)]",
              "type": "Custom"
            }
          }
        }
      ]
    }


---


# TODOS
1. ARM template to add function app
   ~~1. In the lab6/deployment/gab2019.parameters.json file (should look like the file from previous lab) after the `csVisionName` parameter add this snippet: 
   `funcAppName": {
      "value": "gab2019-funcApp"
    }`~~
   1. ~~In the lab6/deployment/gab2019.json
      1. Add this snippet in the parameters (**needs explanation**): 
        `"funcAppName": {
            "type": "string",
            "metadata": {
                "description": "The  name of the Function App"
            }
        }`~~
        1. ~~Add this snippet at the end of the resources array(**needs explanation**):
        `{
      "apiVersion": "2015-08-01",
      "name": "[parameters('funcAppName')]",
      "type": "Microsoft.Web/sites",
      "kind": "functionapp",
      "location": "[resourceGroup().location]",
      "properties": {
         "serverFarmId": "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]",
         "siteConfig": {
            "alwaysOn": false,
            "appSettings": [
                { "name": "FUNCTIONS_EXTENSION_VERSION", "value": "~2" }
            ]
         }
      },
      "dependsOn": [
        "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]",
        "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
      ],
      "resources": [
         {
            "apiVersion": "2015-08-01",
            "name": "appsettings",
            "type": "config",
            "dependsOn": [
              "[resourceId('Microsoft.Web/sites', parameters('funcAppName'))]",
              "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
            ],
            "properties": {
              "FUNCTIONS_EXTENSION_VERSION":"~2",
              "AzureWebJobsStorage": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('StorageAccountName'),'2015-05-01-preview').key1)]",
              "AzureWebJobsDashboard": "[concat('DefaultEndpointsProtocol=https;AccountName=', variables('storageAccountName'), ';AccountKey=', listKeys(variables('StorageAccountName'),'2015-05-01-preview').key1)]",
              "ComputerVision:Endpoint":"[reference(parameters('csVisionName'), '2017-04-18').endpoint]",
              "ComputerVision:ApiKey":"[listKeys(parameters('csVisionName'), '2017-04-18').key1]"
            }
         },
         {
          "apiVersion": "2018-02-01",
          "type": "config",
          "name": "connectionstrings",
          "dependsOn": [
            "[resourceId('Microsoft.Web/sites', parameters('funcAppName'))]",
            "[resourceId('Microsoft.Storage/storageAccounts', variables('storageAccountName'))]"
          ],
          "properties": {
            "AzureWebJobsStorage": {
              "value": "[Concat('DefaultEndpointsProtocol=https;AccountName=',variables('StorageAccountName'),';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts', variables('StorageAccountName')), providers('Microsoft.Storage', 'storageAccounts').apiVersions[0]).keys[0].value)]",
              "type": "Custom"
            }
          }
        }
      ]
    }`~~
2. in ADO create new build pipeline using yaml and **target lab6/app/build.yaml**, it should trigger on every commit (CI)
   1. yaml should target the csproj of the app (may need adjustment depending on location)
3. in ADO create release pipeline using generated artifact from yaml, it should trigger when build is done (CD)
4. in DogImage.cs
   1. Function Signature explanation:
      1. `FunctionName` is the name displayed in the portal
      2. `[BlobTrigger("images/{name}"` Setup the function for which type of trigger (Blob) what container to look into (images) and what the blob name will be ({name})
      3. `Connection = "AzureWebJobsStorage"` what connection string to use
      4. `CloudBlockBlob myBlob, string name, ILogger log` binded parameters to use in the function
   **more info on triggers here: https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-expressions-patterns**
   1. Copy the Code inside the function:
    `var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
            var visionAPI =  new ComputerVisionClient(new ApiKeyServiceClientCredentials(config["ComputerVision:ApiKey"])) { Endpoint = config["ComputerVision:Endpoint"] };
            var path = $"{myBlob.Uri.ToString()}{myBlob.GetSharedAccessSignature(sasConstraints)}";`
            
            var results = await visionAPI.AnalyzeImageAsync(path, Features);
            if(IsDog(results))
            {
                return;
            }
            
            await myBlob.DeleteIfExistsAsync();`
5. Commit code to repo... should trigger a build and then a release
6. Use provided assets to add all images in the container (storage explorer) (**Provide assets**)
7. check what is left of assets that were dogs
8. *Extra credit*: move non-dog assets to another container using the available function overload


## Reference

## End
[Previous Lab](../Lab5/README.md)
[Next Lab](../Lab7/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"