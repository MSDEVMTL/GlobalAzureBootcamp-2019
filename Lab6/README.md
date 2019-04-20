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
> You can learn more by reading the documentation: [Using linked and nested templates when deploying Azure resources](https://docs.microsoft.com/en-us/azure/azure-resource-manager/resource-group-linked-templates?WT.mc_id=globalazure-github-frbouche)

# Modify the ARM template to add the Azure Function

Just like we did in Lab 5, open the existing [ARM template](../Lab2/deployment/gab2019.json) from lab2. We will start by creating a new variable to set the name of our Function App. 

In the array of variable add a new variable named `funcAppName`: 

    "funcAppName": "[concat(parameters('webAppName'), '-funcApp')]"

This will reuse the name received in parameter `webAppName` and add `-funcApp` to it. We will use this as the Function App name.

Now let's add the Function App. Under the resources array in the ARM template add the following snippet we will explain it after:

``` json

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
```

Now let's explain what we just added. 

- The first section defines our Azure Function name, type, and a few basic properties. 
- The second section `dependsOn` specify that the Function need to be created after the storage account and the service plan.
- In the last section we define a list of `resources` contained inside the Function: `appsettings`, `connectionstrings`. Those resources are very important since they will keep the information like the connection to the ComputerVision Service, and Storage.
  
# Create the Azure Function App

From the windows explorer create a new sub-folder `gab2019-FuncApp`. For example in Lab 1 if you created a folder `C:\dev\gab2019\` it will look like this.

    C:
    └───dev
        └───gab2019
            ├───deployment
            ├───GABCDemo
            └───GABCDemo-FuncApp

Open a new instance of VSCode. Click on the Azure Logo from the Left menu-bar, we will use the Azure Function extension. The first icon (1) is to create a Function App. The second icon (2) will create one function inside a function App. A Function App can contain many different functions. The third icon (3) is to deploy from VSCode. It's very useful when you are deploying in a dev environment or just for a quick test. However, we wont use this last button since we will deploy using our Azure Pipeline. 

The key icon (4) is to explore Azure Function already deployed in Azure. Once the VSCode extension is authenticated to your subscription, a key icon will be display by subscription.

![CreateAzureFunction][CreateAzureFunction]

Let's create the Function App. Click on the first icon (the folder with a lightning bolt), and navigate to the folder `gab2019-FuncApp` created previously.  When prompt to *select a language for your function project*, select **C#**. 

> Wait... It can take a few second while things are getting prepared.

You now be ask to *Select a template for your project's first function*; Select **Skip for now**. Then Select to open it in the current VSCode window.

![CreateFunctionApp][CreateFunctionApp]

We now need to create a Function inside our Function App. Click again on the Azure logo to use the Azure Function Extension. This time click on the second icon (the lightning bolt with a little plus sign).

When ask *Select a template for your function*; Select **Blob Trigger**. Then You will need to provide a function name (ex: DogDetector), and a namespace (ex: GABC.Function).

We need to specify where we will keep our connections. When asked to *Select setting from "local.settings.json"*, type **AzureWebJobsStorage**.

The last step is to specify the path where our Azure Function trigger will be watching. Since we want the function to be trigger every time a new file is created in the container *images*, enter `images`

![FunctionCreated][FunctionCreated]

Let's have a quick look at the Function signature:
1. `FunctionName`: is the name displayed in the portal.
1. `[BlobTrigger("images/{name}"`: Setup the function for which type of trigger (Blob) what container to look into (images) and what the blob name will be ({name})
1. `Connection = "AzureWebJobsStorage"`: This is the connection string to use.
1. `Stream myBlob, string name, ILogger log`: Binded parameters to use in the function.

Refer to the documentation to learn more about the [Azure Functions binding expression patterns](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-expressions-patterns?WT.mc_id=globalazure-github-frbouche)

> Note in Lab 1 we should add a gitignore file at the root or move to the root the one created by dotnet new.  Then we should remove the other gitignore file from lab 5 and 6.

## Add the code inside the Azure Function

Before we add some code inside the Azure Function let's add some requirement features. Let's first start by adding some package we will need. Look into the file `GABCDemo-FuncApp.csproj`. Currently you should have only one `<PackageReference>`, referencing the Microsoft.NET.Sdk.Functions. Let's add a few more.

Open the terminal ( Ctrl + \` ) if it's not already done. Be sure you are in the `gab2019-FuncApp` folder, and enter the following command.

  dotnet add package Newtonsoft.Json

If you look again the file `GABCDemo-FuncApp.csproj`, you will see that the package is now referenced. Repeat the previous command for the package `Microsoft.Azure.WebJobs.Extensions.Storage`, `Microsoft.Azure.CognitiveServices.Vision.ComputerVision` , and `Microsoft.Azure.WebJobs.Script.ExtensionsMetadataGenerator`

We will need a few new refences. At the top of the file, add these using lines:

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
```

First inside the class `DogDetector`, and before the method `Run` paste this code.

``` csharp
// Feature we want to work with when getting analysis back
private static readonly List<VisualFeatureTypes> Features = new List<VisualFeatureTypes>
{
    VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
    VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
    VisualFeatureTypes.Tags
};

// We must provide SAS token in order to have the API read the image located at the provided URL since our container is private
private static SharedAccessBlobPolicy sasConstraints = new SharedAccessBlobPolicy
{
    SharedAccessExpiryTime = DateTimeOffset.UtcNow.AddMinutes(10),
    Permissions = SharedAccessBlobPermissions.Read | SharedAccessBlobPermissions.List
};
```

Now let's replace the code of the main method `Run`.

``` csharp
[FunctionName("DogDetector")]
public static async Task Run([BlobTrigger("images/{name}", Connection = "AzureWebJobsStorage")]CloudBlockBlob myBlob, string name, ILogger log)
{        
    var config = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .Build();
    var visionAPI =  new ComputerVisionClient(new ApiKeyServiceClientCredentials(config["ComputerVision:ApiKey"])) { Endpoint = config["ComputerVision:Endpoint"] };
    var path = $"{myBlob.Uri.ToString()}{myBlob.GetSharedAccessSignature(sasConstraints)}";
    
    var results = await visionAPI.AnalyzeImageAsync(path, Features);
    if(IsDog(results))
    {
        return;
    }
    
    await myBlob.DeleteIfExistsAsync();
}
```
As you can see we changed the signature. `Run` is now an `async Task`. We also changed the type of myBlob from `Stream` to `CloudBlockBlob`, this will give us access to more useful properties in our current scenario.

The code, it will retrieve the connectionString from the configuration, instantiate the Vision API, and call AnalyzeImageAsync passing the info about the image. It will finally check if `IsDog` is true and delete the image if not. 

The only piece missing is that `IsDog` method, so let's add it. Paste the following code inside the class `DogDetector`.

``` csharp
private static bool IsDog(ImageAnalysis image)
{
    return image.Categories.Any(x => x.Name == "animal_dog") || image.Tags.Any(x => x.Name == "dog");
}
```

## Preparation for a New Azure Pipeline

Often when Azure function are used in a solution we want them to have their onw life cycle. It could be because they are use with a legacy application that doesn't really changes, or it cold be because we want to be able to deployment separately.  

You can close this instance of VSCode, or just return in the other instance (the one open at the root `c:\Dev\gab2019`).

We could use only one Yaml file and have multiple jobs define in it. However, for this lab we will use two different files.

Let's create a new `build-AzFunc.yml` file at the root `gab2019` (beside the azure-pipeline.yml). Copy the following code and have a look to see if you understand what's happening.

``` yaml
trigger:
- master


pool:
  name: Hosted VS2017
  demands:
  - msbuild
  - visualstudio
  - vstest
  
steps:
- task: NuGetToolInstaller@0
  displayName: 'Use NuGet 4.4.1'
  inputs:
    versionSpec: 4.4.1

- task: NuGetCommand@2
  displayName: 'NuGet restore'
  inputs:
    restoreSolution: '**\GABCDemo-FuncApp\*.csproj'

- task: VSBuild@1
  displayName: 'Build solution'
  inputs:
    solution: '**\GABCDemo-FuncApp\*.csproj'
    msbuildArgs: '/p:DeployOnBuild=true /p:DeployDefaultTarget=WebPublish /p:WebPublishMethod=FileSystem /p:publishUrl="$(Agent.TempDirectory)\WebAppContent\\"'
    platform: '$(BuildPlatform)'
    configuration: '$(BuildConfiguration)'

- task: ArchiveFiles@2
  displayName: 'Archive Files'
  inputs:
    rootFolderOrFile: '$(Agent.TempDirectory)\WebAppContent'
    includeRootFolder: false

- task: VSTest@2
  displayName: 'Test Assemblies'
  inputs:
    testAssemblyVer2: |
     **\$(BuildConfiguration)\*test*.dll
     !**\obj\**
    platform: '$(BuildPlatform)'
    configuration: '$(BuildConfiguration)'

- task: PublishSymbols@2
  displayName: 'Publish symbols path'
  inputs:
    SearchPattern: '**\bin\**\*.pdb'
    PublishSymbols: false
  continueOnError: true

- task: PublishBuildArtifacts@1
  displayName: 'Publish Artifact'
  inputs:
    PathtoPublish: '$(build.artifactstagingdirectory)'
  condition: succeededOrFailed()
```

Now, we have everything we need to create the new Azure Pipeline. You can close this instance of VSCode, or just return in the other instance (the one open at the root `c:\Dev\gab2019`).

Let's commit, and push all our changes. That should trigger the existing Azure Pipeline, but that doesn't matter. We will create a new one.

## Create a New Azure BuildPipeline


In the Azure DevOps portal, click on the Pipeline icon in the left menu-bar, and select the *New Build pipeline* option.

![secondPipeline][secondPipeline]

This time we will need to use the classic editor, otherwise the azure-pipeline.yml file will be selected automatically.

![UseClassic][UseClassic]

Select your repository, and click the *Continue* button. If you are using Azure Repos, everything should be fine. If you are using GitHub, just do the selection like previously. 

Now it's time to select the *YAML* option in the template list.

![yaml][yaml]

To configure the template, click on the [...] button. Then select the `build-AzFunc.yml` file and click the *OK* blue button. 

![SelectYamlFile][SelectYamlFile]

You can now click the *Save & queue* button to start the build.


## Create a New Azure Release Pipeline

In the Azure DevOps portal, click on the Pipeline icon in the left menu-bar, and select Release. Click *New Release pipeline* option. This time you will need to create two artifacts, one for each build pipeline.

![TwoArtifacts][TwoArtifacts]

> !! IN PROGRESS !! 

## Time to test our work



1. Use provided assets to add all images in the container (storage explorer) (**Provide assets**)
2. check what is left of assets that were dogs
3. *Extra credit*: move non-dog assets to another container using the available function overload


## Reference

- [Automate resource deployment for your function app in Azure Functions](https://docs.microsoft.com/en-us/azure/azure-functions/functions-infrastructure-as-code?WT.mc_id=globalazure-github-frbouche)
- [Azure Functions binding expression patterns](https://docs.microsoft.com/en-us/azure/azure-functions/functions-bindings-expressions-patterns?WT.mc_id=globalazure-github-frbouche)

## End
[Previous Lab](../Lab5/README.md)
[Next Lab](../Lab7/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[CreateAzureFunction]: medias/CreateAzureFunction.png 'Azure Function Extension explained'
[CreateFunctionApp]: medias/CreateFunctionApp.gif 'Create Azure Function App'
[FunctionCreated]: medias/FunctionCreated.png 'Generated Function'
[secondPipeline]: medias/secondPipeline.png 'Create a second Azure Pipeline'
[UseClassic]: medias/UseClassic.png 'Use the classic editor instead'
[yaml]: medias/yaml.png 'Use the classic editor instead'
[SelectYamlFile]: medias/SelectYamlFile.png 'Select the YAML file'
[TwoArtifacts]: medias/TwoArtifacts.png 'Create two artifacts'
