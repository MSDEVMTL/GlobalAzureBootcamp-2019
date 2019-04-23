![gablogo][gablogo]

# Lab 5 - Azure Cognitive Services

# Goal

Our goal is to know if a picture has a dog or not.

To do this, we will add Azure Cognitive Services (Vision) to our project. The web application will be modified to read one image (provided) from Azure blob storage and display the information returned by the Vision API. The image (a dog or a cat) will be uploaded to the Blob storage using the Azure Portal or the Azure Storage Explorer.

# Let's code!

- [Lab 5 - Azure Cognitive Services](#lab-5---azure-cognitive-services)
- [Goal](#goal)
- [Let's code!](#lets-code)
- [Add Azure components](#add-azure-components)
  - [Modify the ARM template to add the Computer Vision service](#modify-the-arm-template-to-add-the-computer-vision-service)
  - [Deploy the ARM templates to Azure](#deploy-the-arm-templates-to-azure)
    - [Azure CLI - incremental mode](#azure-cli---incremental-mode)
  - [Getting config info for the web app](#getting-config-info-for-the-web-app)
    - [Getting config info from deployment](#getting-config-info-from-deployment)
- [Upload the dog image into your storage account](#upload-the-dog-image-into-your-storage-account)
- [Let's add computer vision to our web application](#lets-add-computer-vision-to-our-web-application)
  - [Adding the Computer Vision API library to the application](#adding-the-computer-vision-api-library-to-the-application)
  - [Adding the Blob storage library to the application](#adding-the-blob-storage-library-to-the-application)
  - [Using the API in code](#using-the-api-in-code)
    - [Setting up the resources keys](#setting-up-the-resources-keys)
    - [Add code to make use of the resource keys](#add-code-to-make-use-of-the-resource-keys)
    - [Creating classes to consume the storage and computer vision API](#creating-classes-to-consume-the-storage-and-computer-vision-api)
      - [BlobStorageManager](#blobstoragemanager)
      - [ImageAnalyzer](#imageanalyzer)
      - [Wiring all this into the Startup](#wiring-all-this-into-the-startup)
  - [Creating the controller and view](#creating-the-controller-and-view)
    - [Better options validation](#better-options-validation)
  - [Folder structure](#folder-structure)
- [Annex](#annex)
  - [A](#a)
    - [Getting the API keys for the API](#getting-the-api-keys-for-the-api)
    - [Getting the blob storage connection string](#getting-the-blob-storage-connection-string)
- [Reference](#reference)
- [End](#end)

# Add Azure components

Let's add Cognitive Services to our deployment (ARM) template and deploy it.

## Modify the ARM template to add the Computer Vision service

In [Lab 2](../Lab2/README.md) we created an ARM template to deploy the backbone of our application (app service plan, web app (mvc) and storage account).

We will start from that template and add the Cognitive Services resource to it

Open the existing [ARM template](../Lab2/deployment/gab2019.json) and [ARM template parameters](../Lab2/deployment/gab2019.parameters.json) from lab2.

**1 - Add a 'csVisionName' variable to name the Cognitive Services resource**

We will create a unique name for the vision resources utilizing the 'suffix' variable created previously.

The variable section should now look like this:

```json
"variables": {
    "suffix": "[uniqueString(resourceGroup().id, resourceGroup().location)]",
    "storageName": "[concat('stg', variables('suffix'))]",
    "csVisionName": "[concat('vision', variables('suffix'))]"
}
```

**2 - Add the Cognitive Services resource:**

Firstly, under the resources array in the ARM template (gab2019.json) add:

```json
{
    "type": "Microsoft.CognitiveServices/accounts",
    "apiVersion": "2016-02-01-preview",
    "name": "[variables('csVisionName')]",
    "location": "[resourceGroup().location]",
    "sku": {
        "name": "F0"
    },
    "kind": "ComputerVision",
    "dependsOn": [],
    "properties": {},
    "scale": null
}
```

This will tell Azure that we want an instance of Cognitive Services.

**3 - Add the Cognitive Services access keys in the web application configuration**

In the web app resource / parameters section of the ARM template (gab2019.json), add:

```json
"properties": {
    "name": "[parameters('webAppName')]",
    "siteConfig": {
        "appSettings": [
        {
           "name": "ComputerVision:Endpoint",
           "value": "[reference(variables('csVisionName'), '2017-04-18').endpoint]"
        },
        {
           "name": "ComputerVision:ApiKey",
           "value": "[listKeys(variables('csVisionName'), '2017-04-18').key1]"
        }]
    },
    "serverFarmId": "[resourceId('Microsoft.Web/serverfarms/', parameters('appSvcPlanName'))]"
},
```

The site config will add app settings to the web application in Azure.

The Cognitive Services endpoint and key will be set into our MVC Web App (just like we do with the storage account)

**4 - Add output variables**

In the output section of the template, we will add outputs to make it easier to get the keys and the connection string we will need to run our app locally. Replace the `outputs` section with the code below:

```json
"outputs": {
    "CognitiveServices-endpoint": {
        "type": "string",
        "value": "[reference(variables('csVisionName'), '2017-04-18').endpoint]"
    },
    "CognitiveServices-key1": {
        "type": "string",
        "value": "[listKeys(variables('csVisionName'), '2017-04-18').key1]"
    },
    "Storage-connectionString": {
        "type": "string",
        "value": "[Concat('DefaultEndpointsProtocol=https;AccountName=',variables('StorageAccountName'),';AccountKey=',listKeys(resourceId('Microsoft.Storage/storageAccounts', variables('StorageAccountName')), providers('Microsoft.Storage', 'storageAccounts').apiVersions[0]).keys[0].value)]"
    }
}
```

This will add output variables to the resource group deployment.

At this point the ARM template for lab 5 is ready.

-   [ARM template](deployment/gab2019.json)
-   [ARM template parameters](deployment/gab2019.parameters.json)

## Deploy the ARM templates to Azure

Now that we have an ARM template that includes Cognitive Services, we want to deploy it.

In [Lab 2](../Lab2/README.md) we saw a few methods to deploy the initial components of the ARM template, we will revisit the Azure CLI for this lab.

### Azure CLI - incremental mode

Azure CLI allows for two different deployment modes; "Complete" and "Incremental".

The Incremental mode tells Azure to add the missing components while leaving the existing ones alone (components that exist in Azure but not in the template will also be kept).

The script below will deploy the ARM template resources.

```powershell
az group deployment create \
--name Gab2019Deployment1 \
--mode Incremental \
--resource-group GAB2019RG \
--template-file gab2019.json \
--parameters GAB2019.parameters.json
```

Once done, the Cognitive Services resource would be deployed and the keys should be configured in the web application.

## Getting config info for the web app

The ARM template deployment takes care of setting the config information in the web app, so there is no need to deploy the values in the app settings (or to commit these values to git).

We, however, will need the config information to run the web application locally. Copy these values into a text editor once the ARM templace has finished deploying or follow the steps below to get them from the portal. Refer yourself to Annex A to learn other ways to get all this information.

### Getting config info from deployment

Since we added output variables to the ARM template, it makes it easier to get the information we need for Cognitive Services and Azure Storage.

To find the output values;

-   navigate to the Azure portal
-   locate your resource group
-   in the deployments section
    -   click on the latest deployment
        -   click on the outputs section
        -   the output variable values will be there ready to be used (all in one place!)

![deployment-output][deployment-output]

# Upload the dog image into your storage account

We will upload an image to blob storage to serve as test data.

-   create a container in your storage blob called `images`
-   upload the dog image found in the data directory into this container
    -   Use either the portal or the storage explorer application (see [Lab 0](../Lab0/README.md) for details)

**Make sure to set the access level to public so the computer vision can read from it**

To do that in Azure Storage Explorer;

-   right click on your container
-   select Set Public Access Level

![public-access-blob-se][public-access-blob-se]

To do the same in the Azure Portal;

-   navigate to your storage account
-   click on the Blobs under Blob service
-   select your blob (in our case images)
-   In the overview blade, click the `Change access level`
-   Select `Blob (anonymous read access for blobs only) as the public access level`

![public-access-blob-portal-overview][public-access-blob-portal-overview]

# Let's add computer vision to our web application

Azure components are now deployed and test data is ready. It s now time to add Computer Vision functionality to the web appication.

## Adding the Computer Vision API library to the application

We need to install the Computer Vision client library NuGet package.

To do so;

-   open the terminal in your Visual Studio Code (`` ctrl+` ``)
-   type `dotnet add package Microsoft.Azure.CognitiveServices.Vision.ComputerVision`

## Adding the Blob storage library to the application

We need to install the Blob storage library NuGet Package.

To do so;

-   again in your terminal
-   type `dotnet add package Microsoft.Azure.Storage.Blob`

## Using the API in code

### Setting up the resources keys

To use the computer vision API, first copy the API key you copied earlier into the file appsettings.json, under the `ComputerVision:ApiKey` section.

In the `ComputerVision:ApiEndPoint` section, enter the computer vision API endpoint. It should like like this: `https://eastus.api.cognitive.microsoft.com`, where `eastus` is the region into which you created your computer vision API resource into.

Now for the storage connection string, copy the connection string you copied earlier into the section `ConnectionStrings:ApplicationStorage`.

*Note*: The `Key1:Key2`, for instance `ComputerVision:ApiEndPoint`, refers to ASP.NET Core way of setting application settings in appsettings.json. The `Key1:Key2` refers to the hierarchy of JSON keys. For instance, your appsettings.json should look like this:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "ApplicationStorage": ""
  },
  "ComputerVision": {
    "ApiKey": "",
    "ApiEndPoint": ""
  }
}
```

### Add code to make use of the resource keys

To be able to use these keys into our application, create two classes in the application named `StorageAccountOptions` and `ComputerVisionOptions`. We will be using the Options pattern available in ASP.NET Core. In that class, you will need to copy the structure of your appsettings into code. It should look like the following:

```csharp
public class StorageAccountOptions
{
    [Required]
    public string ConnectionString { get; set; }
}
```

```csharp
public class ComputerVisionOptions
{
    /// <summary>
    /// Your subscription key
    /// </summary>
    [Required]
    public string ApiKey { get; set; }

    /// <summary>
    /// The endpoint of the region in which your created your ComputerVision resource. i.e. https://westcentralus.api.cognitive.microsoft.com
    /// </summary>
    [Required]
    public string ApiEndPoint { get; set; }
}
```

Note that we have also created classes for the sub-sections.

### Creating classes to consume the storage and computer vision API

Under the root of your application, create a folder called `Services` and create 2 classes (2 classes): `BlobStorageManager` and `ImageAnalyzer`

The `BlobStorageManager` is used to consume the blob storage and the `ImageAnalyzer` is used to consume the computer vision API.

#### BlobStorageManager

In order to consume the blob storage, copy the following code into the class `BlobStorageManager`

```csharp
private readonly CloudStorageAccount _storageAccount;

public BlobStorageManager(IOptions<StorageAccountOptions> options)
{
    if (options == null) { throw new ArgumentNullException(nameof(options)); }
    _storageAccount = CreateCloudStorageAccount(options.Value);
}

private CloudStorageAccount CreateCloudStorageAccount(StorageAccountOptions options)
{
    if (!CloudStorageAccount.TryParse(options.ConnectionString, out CloudStorageAccount storageAccount))
    {
        throw new Exception("Invalid storage account connecting string. Please verify the connection string and try again");
    }
    return storageAccount;
}

public IEnumerable<IListBlobItem> GetFiles(string containerName)
{
    var cloudBlobClient = _storageAccount.CreateCloudBlobClient();
    var container = cloudBlobClient.GetContainerReference(containerName);
    var blobs = container.ListBlobs();
    return blobs;
}
```

The method `GetFiles` will be used to list all the files in your blob container (in our cases we will use the _images_ container created earlier).

The namespaces for this class should have:

```csharp
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
```

Create an interface that will be used for the dependency injection called `IBlobStorageManager`. Copy the following code into this interface

```csharp
IEnumerable<IListBlobItem> GetFiles(string containerName);
```

Make sure to inherit from this interface. Your class definition should look like

```csharp
public class BlobStorageManager : IBlobStorageManager
```

#### ImageAnalyzer

To consume the computer vision API, copy the following code into the class `ImageAnalyzer`

```csharp
private readonly ComputerVisionClient _computerVision;

private static readonly List<VisualFeatureTypes> Features =
    new List<VisualFeatureTypes>
    {
        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
        VisualFeatureTypes.Tags
    };

public ImageAnalyzer(ComputerVisionClient computerVision)
{
    _computerVision = computerVision ?? throw new ArgumentNullException(nameof(computerVision));
}

public Task<ImageAnalysis> AnalyzeAsync(string imageUrl)
{
    if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
    {
        throw new Exception($"Invalid remoteImageUrl: {imageUrl}");
    }
    var analysisResults = _computerVision.AnalyzeImageAsync(imageUrl, Features);
    return analysisResults;
}
```

As you can see, we are asking the API to return use the Categories of the image, its description, the image type information, the image tags and the image faces collection. You can also ask it to return the Color information (by adding `VisualFeatureTypes.Color`) and Objects information (by adding `VisualFeatureTypes.Objects`).

The namespaces for this class should have:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
```

Create an interface that will be used for the dependency injection called `IImageAnalyzer`. Copy the following code into this interface

```csharp
Task<ImageAnalysis> AnalyzeAsync(string imageUrl);
```

Make sure to inherit from this interface. Your class definition should look like

```csharp
public class ImageAnalyzer : IImageAnalyzer
```

#### Wiring all this into the Startup

In your `Startup` class, under the `ConfigureServices` method, add the following:

```csharp
// Blob Storage
services.AddOptions<StorageAccountOptions>()
    .Configure(options => options.ConnectionString = Configuration.GetConnectionString("ApplicationStorage"))
    .ValidateDataAnnotations();
services.AddSingleton<IBlobStorageManager, BlobStorageManager>();

// Computer Vision
services.AddOptions<ComputerVisionOptions>()
    .Bind(Configuration.GetSection("ComputerVision"))
    .ValidateDataAnnotations();
services.AddSingleton(serviceProvider =>
{
    var options = serviceProvider.GetRequiredService<IOptions<ComputerVisionOptions>>().Value;
    return new ComputerVisionClient(new ApiKeyServiceClientCredentials(options.ApiKey)) { Endpoint = options.ApiEndPoint };
});
services.AddSingleton<IImageAnalyzer, ImageAnalyzer>();
```

This will map our settings to our Options objects and add our service class into our dependency injection container for later consumption.

Make sure to add the following the following namespace to your startup class:

```csharp
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
```

## Creating the controller and view

In the controller folder, create a class named `AnalyzerController`. This is the controller that will be called by our application to deal with the image analysis.

In that class, copy the following code:

```csharp
private readonly IBlobStorageManager _blobStorageManager;
private readonly IImageAnalyzer _imageAnalyzer;

public AnalyzerController(IBlogStorageManager blobStorageManager, IImageAnalyzer imageAnalyzer)
{
    _blobStorageManager = blobStorageManager ?? throw new ArgumentNullException(nameof(blobStorageManager));
    _imageAnalyzer = imageAnalyzer ?? throw new ArgumentNullException(nameof(imageAnalyzer));
}

public IActionResult Index()
{
    var files = _blobStorageManager.GetFiles("images").Select(item => item.Uri).ToList();
    ViewBag.Files = files;
    return View();
}

public async Task<IActionResult> Analyze(string imageUrl)
{
    var results = await _imageAnalyzer.AnalyzeAsync(imageUrl);
    ViewData["Title"] = "Image analysis results";
    ViewData["ImageUrl"] = imageUrl;
    return View("Results", results);
}
```

The `Index` method will list all the images from our _images_ container and the `Analyze` method will take the URL of our image in our container and analyze it with the computer vision API.

The namespaces in this class should include:

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GABDemo.Services;
```

In the Views folder, create a new folder called Analyzer. In that folder add 2 Razor Views called `Index` and `Results`.

In the `Index` view, copy the following code:

```html
<table class="table">
    <thead>
        <tr>
            <th scope="col">#</th>
            <th scope="col">Image</th>
            <th scope="col">Actions</th>
        </tr>
    </thead>
    <tbody>
        @if (ViewBag.Files != null) { int count = 1; foreach (Uri file in ViewBag.Files) {
        <tr>
            <th scope="row">@count</th>
            <td>@file</td>
            <td>
                <a asp-action="Analyze" asp-route-imageUrl="@file.AbsoluteUri">Analyze</a>
            </td>
        </tr>
        count++; } }
    </tbody>
</table>
```

This code displays all the files in our _images_ container.

In the `Results` view, the most important part is having the model defined. At the top of the file, add the following:

```csharp
@model Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.ImageAnalysis
```

With this, you can access `Model` in your view and display the results information the way you want it. Here's an example of how it can be displayed:

```html
<h1>Categories</h1>
<table class="table">
    <thead>
        <tr>
            <th scope="col">Name</th>
            <th scope="col">Score</th>
            <th scope="col">Details</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var category in Model.Categories) {
        <tr>
            <td>@category.Name</td>
            <td>@category.Score</td>
            <td>
                @if (category.Detail != null) { if (category.Detail.Celebrities != null && category.Detail.Celebrities.Any()) { foreach (var
                celebrity in category.Detail.Celebrities) { @celebrity<br />
                } } if (category.Detail.Landmarks != null && category.Detail.Landmarks.Any()) { foreach (var landmark in
                category.Detail.Landmarks) { @landmark<br />
                } } }
            </td>
        </tr>
        }
    </tbody>
</table>

<h1>Description</h1>
<h2>Captions</h2>
<table class="table">
    <thead>
        <tr>
            <th scope="col">Text</th>
            <th scope="col">Confidence</th>
        </tr>
    </thead>
    @foreach (var caption in Model.Description.Captions) {
    <tr>
        <td>@caption.Text</td>
        <td>@caption.Confidence</td>
    </tr>
    }
</table>

<h2>Tags</h2>
@string.Join(", ", Model.Description.Tags) @if (Model.Adult != null) {
<h2>Adult content</h2>
<table class="table">
    <thead>
        <tr>
            <th scope="col">Adult score</th>
            <th scope="col">Is adult content?</th>
            <th scope="col">Is racy content?</th>
            <th scope="col">Racy score</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>@Model.Adult.AdultScore</td>
            <td>@Model.Adult.IsAdultContent</td>
            <td>@Model.Adult.IsRacyContent</td>
            <td>@Model.Adult.RacyScore</td>
        </tr>
    </tbody>
</table>
}

<h2>Image Type</h2>
Clipart confidence level: @Model.ImageType.ClipArtType<br />
Line drawing confidence level: @Model.ImageType.LineDrawingType<br />

@if (Model.Faces != null && Model.Faces.Count > 0) {
<h2>Faces</h2>
<table class="table">
    <thead>
        <tr>
            <th scope="col">Age</th>
            <th scope="col">Gender (if applicable)</th>
            <th scope="col">Face rectangle</th>
        </tr>
    </thead>
    <tbody>
        @foreach (var faceDescription in @Model.Faces) {
        <tr>
            <td>@faceDescription.Age</td>
            <td>@(faceDescription.Gender != null ? faceDescription.Gender.Value.ToString() : "N/A")</td>
            <td>
                Height: @faceDescription.FaceRectangle.Height<br />
                Width: @faceDescription.FaceRectangle.Width<br />
                Left: @faceDescription.FaceRectangle.Left<br />
                Top: @faceDescription.FaceRectangle.Width<br />
            </td>
        </tr>
        }
    </tbody>
</table>
}

<div class="row">
    <div class="col-sm">
        <h2>Image Metadata</h2>
        Width: @Model.Metadata.Width<br />
        Height: @Model.Metadata.Height<br />
        Format: @Model.Metadata.Format<br />
    </div>
    <div class="col-sm">
        <h2>Image</h2>
        <img src="@ViewData["ImageUrl"]" class="img-thumbnail" />
    </div>
</div>
```

### Better options validation

If your application throws an `OptionsValidationException` and that you want a better understanding of the error, add a class named `OptionsValidationExceptionFilterAttribute` at the root of your project and replace the class by the following code:

```csharp
/// <summary>
/// This filter allows for displaying more explicit options validation exception.
/// Implements the <see cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute" />
public class OptionsValidationExceptionFilterAttribute : ExceptionFilterAttribute
{
    /// <summary>
    /// This method is called when an exception occurs and wrap <see cref="Microsoft.Extensions.Options.OptionsValidationException"/>
    /// into another <see cref="Exception"/> to make the message easier to understand.
    /// </summary>
    /// <param name="context">The provided exception context.</param>
    /// <inheritdoc />
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is OptionsValidationException validationEx)
        {
            context.Exception = new Exception(validationEx.Failures.First(), validationEx);
        }
    }
}
```

Then, in `Startup.cs`, replace `.AddMvc()` by `.AddMvc(options => options.Filters.Add<OptionsValidationExceptionFilterAttribute>())`.

Re-run your application and the error message should be clearer. If it is a generic error message, make sure that the `ASPNETCORE_ENVIRONMENT` is set to `Development`.

## Folder structure

If you followed the steps above, your folder structure should look like this

![folder-structure][folder-structure]

# Annex

## A
### Getting the API keys for the API

To get your computer vision keys;

-   navigate to the Azure portal
-   locate your computer vision resource
-   go into the keys section
    -   find the key value under key1
    -   click the Copy button to copy the key

![computer-vision-keys][computer-vision-keys]

### Getting the blob storage connection string

To get your storage key;

-   navigate to the Azure portal
-   locate your storage account
-   in the Settings section of the storage account overview
    -   select Access keys
        -   here you can view your account access keys and the complete connection string for each key
    -   find the Connection string value under key1
    -   click the Copy button to copy the connection string

![blob-connectionstring][blob-connectionstring]

# Reference

[Quickstart: Analyze an image using the Computer Vision SDK and C#](https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/quickstarts-sdk/csharp-analyze-sdk?WT.mc_id=globalazure-github-frbouche)

[Develop with blobs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows?WT.mc_id=globalazure-github-frbouche)

[Options pattern in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?WT.mc_id=globalazure-github-frbouche&view=aspnetcore-2.2)

# End

[Previous Lab](../Lab4/README.md)
[Next Lab](../Lab6/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png 'Global Azure Bootcamp 2019'
[computer-vision-keys]: medias/lab5-keys.PNG 'Computer Vision API Keys'
[blob-connectionstring]: medias/portal-connectionstring-blob.png 'Storage Account connection string'
[deployment-output]: medias/lab5-deployment-output.png 'ARM template deployment outputs'
[public-access-blob-se]: medias/public-access-blob-se.png 'Public access blob through storage explorer'
[public-access-blob-portal-overview]: medias/public-access-blob-portal-overview.png 'Public access blob through the azure portal'
[folder-structure]: medias/folder-structure-gab.png 'Folder structure'
