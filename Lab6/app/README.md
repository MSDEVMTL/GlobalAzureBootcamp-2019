# Steps (needs cleaning and better scenario)
1. ARM template to add function app
   1. In the lab6/deployment/gab2019.parameters.json file (should look like the file from previous lab) after the `csVisionName` parameter add this snippet: 
   `funcAppName": {
      "value": "gab2019-funcApp"
    }`
   2. In the lab6/deployment/gab2019.json
      1. Add this snippet in the parameters (**needs explanation**): 
        `"funcAppName": {
            "type": "string",
            "metadata": {
                "description": "The  name of the Function App"
            }
        }`
        2. Add this snippet at the end of the resources array(**needs explanation**):
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
    }`
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
   2. Copy the Code inside the function:
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