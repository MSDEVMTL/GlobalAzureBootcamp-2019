![gablogo][gablogo]

# Lab 5 - Azure Cognitive Services

## Goal

We will add a Azure Cognitive Services Vision to our ARM template. Modify the code of the web application to read one image (provided) from a Azure blob storage, and display the information returned by the Vision API. The image ( a dog or a cat ) will be uploaded to the Blob storage using the Azure Portal or the Azure Storage Explorer.

## Let's code!

### Modify the ARM template to add the computer vision

Under the resources array, add the following:

```
{
"type": "Microsoft.CognitiveServices/accounts",
"sku": {
    "name": "F0"
},
"kind": "ComputerVision",
"name": "[parameters('accounts_cs_vision_name')]",
"apiVersion": "2016-02-01-preview",
"location": "eastus",
"scale": null,
"properties": {},
"dependsOn": []
}
```

### Getting the API keys for the API

Once your ARM templated has successfully been deployed, go in the Azure Portal and search for your computer vision resource name in the search bar. Once in the computer vision resource blade, go in the keys section and copy the KEY 1 into a text editor. We will be using it in the next sections.

![computer-vision-keys][computer-vision-keys]

### Getting the blob storage connection string

To get your storage key, navigate to the Azure portal, then locate your storage account.

In the Settings section of the storage account overview, select Access keys. Here, you can view your account access keys and the complete connection string for each key.

Find the Connection string value under key1, and select the Copy button to copy the connection string.

Copy this value into a text editor. We will be using it in the next sections.

![blob-connectionstring][blob-connectionstring]

### Adding Computer Vision API to the application

Install the Computer Vision client library NuGet package. To do so, open the terminal in your Visual Studio Code (```ctrl+` ```) and type ```dotnet add package Microsoft.Azure.CognitiveServices.Vision.ComputerVision```

### Adding Blob storage library to the application

Install the Blob storage library Nuget Package. To do so, again in your terminal type ```dotnet add package Microsoft.Azure.Storage.Blob```

### Upload the dog image into your storage account

Create a container in your storage blob called ```images```, then upload the dog image found in the data directory into this container. Use either the portal or the storage explorer application.

**Make sure to set the access level to public so the computer vision can read from it**

To do that in Azure Storage Explorer, right click on your container and select Set Public Access Level

![public-access-blob-se][public-access-blob-se]

To do the same in the Azure Portal, navigate to your storage account, then click on the Blobs under Blob service. Select your blob (in our case images), then click on Access policy.
Select Blob (anonymous read access for blobs only) as the public access level

![public-access-blob-portal][public-access-blob-portal]

### Using the API in code

#### Setting up the resources keys

To use the computer vision api, first copy the api key you copied earlier into the file appsettings.json, under the ```Keys:ComputerVision:ApiKey``` section. In the ```Keys:ComputerVision:ApiEndPoint``` section, enter the computer vision api endpoint. It should like like this: ```https://eastus.api.cognitive.microsoft.com```, where ```eastus``` is the region into which you created your computer vision api resource into.

Now for the storage connection string, copy the connection string you copied earlier into the section ```Keys:Storage:ConnectionString```.

To be able to use these keys into our application, create a class in the application named ```KeysOptions```. We will be using the Options pattern available in ASP.NET Core. In that class, you will need to copy the structure of your appsettings into code. It should look like the following:

```csharp
public class KeysOptions
{
    public ComputerVisionOptions ComputerVision { get; set; }
    public StorageAccountOptions Storage { get; set; }
}

public class StorageAccountOptions
{
    public string ConnectionString { get; set; }
}

public class ComputerVisionOptions
{
    /// <summary>
    /// Your subscription key
    /// </summary>
    public string ApiKey { get; set; }

    /// <summary>
    /// The endpoint of the region in which your created your ComputerVision resource. i.e. https://westcentralus.api.cognitive.microsoft.com
    /// </summary>
    public string ApiEndPoint { get; set; }
}
 ```

 Note that we have also created classes for the sub-sections.

#### Creating classes to consume the storage and computer vision api

Under the root of your application, create a folder called ```Services``` and create 2 classes (2 classes): ```BlobStorageManager``` and ```ImageAnalyzer```

The ```BlobStorageManager``` is used to consume the blob storage and the ```ImageAnalyzer``` is used to consume the computer vision api.

##### BlobStorageManager

In order to consume the blob storage, copy the following code into the class ```BlobStorageManager```

```csharp
private readonly CloudStorageAccount _storageAccount;

public BlobStorageManager(string connectionString)
{
    if (!CloudStorageAccount.TryParse(connectionString, out _storageAccount))
    {
        throw new Exception(
            "Invalid storage account connecting string. Please verify the connection string and try again");
    }
}

public IEnumerable<IListBlobItem> GetFiles(string containerName)
{
    var cloudBlobClient = _storageAccount.CreateCloudBlobClient();

    var container = cloudBlobClient.GetContainerReference(containerName);
    foreach (var file in container.ListBlobs())
    {
        yield return file;
    }
}
```

The method ```GetFiles``` will be used to list all the files in your blob container (in our cases we will use the _images_ container created earlier).

The namespaces for this class should have:

```csharp
using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
```

##### ImageAnalyzer

To consume the computer vision api, copy the following code into the class ```ImageAnalyzer```

```csharp
private readonly ComputerVisionClient _computerVision;

private static readonly List<VisualFeatureTypes> Features =
    new List<VisualFeatureTypes>
    {
        VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
        VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
        VisualFeatureTypes.Tags
    };

public ImageAnalyzer(string apiKey, string apiEndpoint)
{
    _computerVision = new ComputerVisionClient(new ApiKeyServiceClientCredentials(apiKey));
    _computerVision.Endpoint = apiEndpoint;
}

public async Task<ImageAnalysis> AnalyzeAsync(string imageUrl)
{
    if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
    {
        throw new Exception("Invalid remoteImageUrl: {imageUrl}");
    }

    ImageAnalysis analysis = await _computerVision.AnalyzeImageAsync(imageUrl, Features);
    return analysis;
}
```

As you can see, we are asking the API to return use the Categories of the image, its description, the image type information, the image tags and the image faces collection. You can also ask it to return the Color information (by adding ```VisualFeatureTypes.Color```) and Objects information (by adding ```VisualFeatureTypes.Objects```).

The namespaces for this class should have:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
```

##### Wiring all this into the Startup

In your ```Startup``` class, under the ```ConfigureServices``` method, add the following:

```csharp
services.Configure<KeysOptions>(Configuration.GetSection("Keys"))
        .PostConfigure<KeysOptions>(options =>
{
    if (string.IsNullOrEmpty(options.ComputerVision.ApiKey))
    {
        throw new Exception("Computer Vision API Key is missing");
    }

    if (string.IsNullOrEmpty(options.ComputerVision.ApiEndPoint))
    {
        throw new Exception("Computer Vision API Key is missing");
    }
});
```

This will tell ASP.NET Core to map our appsettings structure to an object structure.

#### Creating the controller and view

 In the controller folder, create a class named ```AnalyzerController```. This is the controller that will be called by our application to deal with the image analysis.

 In that class, copy the following code:

 ```csharp
private readonly KeysOptions _keys;

public AnalyzerController(IOptions<KeysOptions> apiKeysOptions)
{
    _keys = apiKeysOptions.Value;
}

public IActionResult Index()
{
    var manager = new BlobStorageManager(_keys.Storage.ConnectionString);
    var files = manager.GetFiles("images").Select(_ => _.Uri).ToList();

    ViewBag.Files = files;

    return View();
}

public async Task<IActionResult> Analyze(string imageUrl)
{
    var imageAnalyzer = new ImageAnalyzer(_keys.ComputerVision.ApiKey, _keys.ComputerVision.ApiEndPoint);
    var results = await imageAnalyzer.AnalyzeAsync(imageUrl);
    ViewData["Title"] = "Image analysis results";

    return View("Results",results);
}
 ```

 The ```Index``` method will list all the images from our _images_ container and the ```Analyze``` method will take the url of our image in our container and analyze it with the computer vision api.

 The namespaces in this class should include:

 ```csharp
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GABDemo.Services;
using Microsoft.Extensions.Options;
 ```

 In the Views folder, create a new folder called Analyzer. In that folder add 2 Razor Views called ```Index``` and ```Results```. 

 In the ```Index``` view, copy the following code:

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
        @if (ViewBag.Files != null)
        {
            int count = 1;
            foreach (Uri file in ViewBag.Files)
            {
                <tr>
                    <th scope="row">@count</th>
                    <td>@file</td>
                    <td>
                        <a asp-action="Analyze" asp-route-imageUrl="@file.AbsoluteUri">Analyze</a>
                    </td>
                </tr>
                count++;
            }
        }
    </tbody>
</table>
 ```

This code displays all the files in our _images_ container.

In the ```Results``` view, the most important part is having the model defined. At the top of the file, add the following:

```csharp
@model Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models.ImageAnalysis
```

With this, you can access ```Model``` in your view and display the results information the way you want it. Here's an example of how it can be displayed:

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
        @foreach (var category in Model.Categories)
        {
            <tr>
                <td>@category.Name</td>
                <td>@category.Score</td>
                <td>
                    @if (category.Detail != null)
                    {
                        if (category.Detail.Celebrities != null && category.Detail.Celebrities.Any())
                        {
                            foreach (var celebrity in category.Detail.Celebrities)
                            {
                                @celebrity<br />
                            }
                        }

                        if (category.Detail.Landmarks != null && category.Detail.Landmarks.Any())
                        {
                            foreach (var landmark in category.Detail.Landmarks)
                            {
                                @landmark<br />
                            }
                        }
                    }
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
    @foreach (var caption in Model.Description.Captions)
    {
        <tr>
            <td>@caption.Text</td>
            <td>@caption.Confidence</td>
        </tr>
    }
</table>

<h2>Tags</h2>
@string.Join(", ", Model.Description.Tags)

@if (Model.Adult != null)
{
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

@if (Model.Faces != null && Model.Faces.Count > 0)
{
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
            @foreach (var faceDescription in @Model.Faces)
            {
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

<h2>Image Metadata</h2>
Width: @Model.Metadata.Width<br />
Height: @Model.Metadata.Height<br />
Format: @Model.Metadata.Format<br />
```

## Reference

[Quickstart: Analyze an image using the Computer Vision SDK and C#](https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/quickstarts-sdk/csharp-analyze-sdk)

[Develop with blobs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows)

[Options pattern in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-2.2)

## End
[Previous Lab](../Lab4/README.md)
[Next Lab](../Lab6/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[computer-vision-keys]: medias/lab5-keys.PNG "Computer Vision API Keys"
[blob-connectionstring]: medias/portal-connectionstring-blob.png "Storage Account connection string"
[public-access-blob-se]: medias/public-access-blob-se.png "Public access blob through storage explorer"
[public-access-blob-portal]: medias/public-access-blob-portal.png "Public access blob through the azure portal"