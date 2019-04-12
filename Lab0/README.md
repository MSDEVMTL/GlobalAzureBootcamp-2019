![gablogo][gablogo]

# Lab 0 - What you will need before the bootcamp

To get the best of the Global Azure bootcamp 2019, we **strongly** suggest that you prepare your station with all the required software. Nothing complicated and all the tools that we are going to use during the bootcamp supports macOS, Linux, and Windows - so you can hit the ground running, no matter the platform.

# .NET Core

We will be creating a Web application in .NET Core. Therefore you will need the .NET Core Framework installed on your local machine. To validate if you already have it execute the following command in a terminal.

    dotnet --version

If you have a version equal or greater to 2.1 your are fine. Otherwise we invite you to download a more recent version. In a web browser, navigate to [https://dotnet.microsoft.com/download](https://dotnet.microsoft.com/download). Select your OS between Windows, Linux, or MacOS and click the button Download .NET Core SDK

![dotnetcore][dotnetcore]

Once the installer is downloaded follow the instruction in run it.  When the install is complete re-execute the comment to validate that you have a version equal or greater to 2.1

# The Tools

## Visual Studio Code

![code][code]

Visual Studio Code is a lightweight source code editor developed by Microsoft for Windows, Linux and macOS. It includes support for debugging, embedded Git control, syntax highlighting, intelligent code completion, snippets, and code refactoring.

It's not a requirement but all labs will assume that you have it installed. To install Visual Studio Code, navigate to [code.visualstudio.com](https://code.visualstudio.com/?wt.mc_id=vscom_downloads), and select your platform.

![code-select][code-select]

### Extension

One of the strengths of Visual Studio Code is all its extensions.  During the following Labs, we will use some of them to make things easier.

Installing an extension is very simple and it's done directly from VSCode. Follow these instruction:

* Open VSCode and from the left menu and select the Extensions (Ctrl+Shift+X).
* Type `Azure Resource Manager Snippets` into the search bar at the top of the Extensions Marketplace panel that just opened.
* From the search result, select `Azure Resource Manager Snippets`.
* Click on the little *Install* green button on the side of the extension to proceed with installation.

You just installed your first extension.  Now, make sure to install at least these:

![code-extensions][code-extensions]

- **Azure Resource Manager Snippets**: This extension adds snippets to Visual Studio Code for creating Azure Resource Manager Templates.
- **Azure Functions**: Create, debug, manage, and deploy Azure Functions directly from VS Code.
- **Docker**: Adds syntax highlighting, commands, hover tips, and linting for Dockerfile and docker-compose files.
- **Docker Explorer**: Manage Docker Containers, Docker Images, Docker Hub and Azure Container Registry

![installdockerextension][installdockerextension]

Once all the extensions are installed, click the *Refresh* button to restart VS Code.

## Azure CLI

The Azure CLI is a command-line tool providing a great experience for managing Azure resources. The CLI is designed to make scripting easy, query data, support long-running operations, and more. To install it just follow the instruction based on your platform on the site: [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?WT.mc_id=globalazure-github-frbouche&view=azure-cli-latest).

![azurecli][azurecli]

To validate that you have it type the following commend in a terminal/ console

    az --version

THe result should be `azure-cli (2.0.55)` or newest.

## Git

The Git is a decentralized source control manager, that stand apart from nearly every other SCM out there is its branching model.

Git allows and encourages you to have multiple local branches that can be entirely independent of each other. The creation, merging, and deletion of those lines of development takes seconds.

To install this great tool go to the official web page [Git](https://git-scm.com/downloads) and download the version matching your current platform.

![gitdownload][gitdownload]

This will install the console version of git. This is good enough to be functional. However if you are more visual or if you prefer graphical tool GitKraken is an excellent choice. You can download it from here: [Git Kraken](https://www.gitkraken.com/invite/saVBBaq4)

![gitkraken][gitkraken]

## Azure Storage Explorer (optional)

In Lab 5 and Lab 6 we will be uploading files to Azure Storage Explorer. You can definitely do that directly from the Azure portal. However, great tools that provide much more flexibility and ease is the Azure Storage Explorer.

![azureexplorer][azureexplorer]

Upload, download, and manage blobs, files, queues, tables, and Cosmos DB entities. Gain easy access to manage your virtual machine disks. Work with either Azure Resource Manager or classic storage accounts, plus manage and configure cross-origin resource sharing (CORS) rules. In short, a great tool to have on your machine.

It can be download from [here](https://azure.microsoft.com/en-ca/features/storage-explorer) and it's compatible with MacOS, Windows, and Linux.

Once Azure Storage Explorer is installed, you will need to add your account. The first time Azure Storage Explorer opens it will ask you to sign-in. (To add more account click on the plug icon in the left menubar)

![azexplorer-AddAccount][azexplorer-AddAccount]

You are now ready for the Labs.

# Accounts

## Azure Subscription

Having an Azure subscription is mandatory to be able to do the Global Azure Bootcamp. If you don't own an Azure subscription already, you can create your **free** account today. It comes with 200$ credit, so you can experience almost everything without spending a dime. 

Make sure to have your account up and ready before the bootcamp.

[Create your free Azure account today](https://azure.microsoft.com/en-us/free?WT.mc_id=globalazure-github-frbouche)

![freeAzure][freeAzure]

## [Azure DevOps]

For the GLobal Azure Bootcamp, you will need an Azure DevOps account. It's free! Get 10 free parallel jobs and unlimited minutes for your open source projects with Pipelines.

[Create your free Azure DevOps account today!](https://azure.microsoft.com/en-ca/services/devops?WT.mc_id=globalazure-github-frbouche)

![azureDevOps][azureDevOps]

## GitHub (optional)

GitHub is a development platform inspired by the way you work. From open source to business, you can host and review code, manage projects, and build software.  During the Global Azure Bottcamp you could host your code in a GitHub Repository if you prefer it to Azure DevOps. 

[Create your free account today](https://github.com/)


# Need help?

Contact our volunteers:

- Frank Boucher : Twitter [@fboucheros](https://twitter.com/fboucheros)
- Alain Vezina : Twitter [@avezina](https://twitter.com/avezina)

[Next Lab](../Lab1/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[code]: medias/code-screenshot.png "Visual Studio Code screenshot"
[code-select]: medias/code-select.jpg
[code-extensions]: medias/code-extensions.jpg
[azurecli]: medias/azurecli.jpg
[gitdownload]: medias/gitdownload.jpg
[gitkraken]: medias/gk-merge-edit.gif
[azureexplorer]: medias/AzureBlobStorage.png
[azexplorer-AddAccount]: medias/azexplorer-AddAccount.png
[installdockerextension]: medias/installdockerextension.png
[freeAzure]: medias/freeAzure.png
[azureDevOps]: medias/azureDevOps.png
[dotnetcore]: medias/dotnetcore.png
