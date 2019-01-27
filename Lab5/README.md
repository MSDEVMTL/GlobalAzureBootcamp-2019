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

## Reference

[Quickstart: Analyze an image using the Computer Vision SDK and C#](https://docs.microsoft.com/en-us/azure/cognitive-services/Computer-vision/quickstarts-sdk/csharp-analyze-sdk)

[Develop with blobs](https://docs.microsoft.com/en-us/azure/storage/blobs/storage-quickstart-blobs-dotnet?tabs=windows)

## End
[Previous Lab](../Lab4/README.md)
[Next Lab](../Lab6/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[computer-vision-keys]: medias/lab5-keys.PNG "Computer Vision API Keys"
[blob-connectionstring]: medias/portal-connectionstring-blob.png "Storage Account connection string"
[public-access-blob-se]: medias/public-access-blob-se.png "Public access blob through storage explorer"
[public-access-blob-portal]: medias/public-access-blob-portal.png "Public access blob through the azure portal"