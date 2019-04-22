![gablogo][gablogo]

# Lab 1 - Continuous Integration and Continuous Deployment (CI/CD)

## Goal

In this lab you will create an application in .Net Core, push it to a remote repository and create a Continuous Integration and Continuous Deployment (CI/CD) with the Azure DevOps Pipeline to deploy the website to Azure.

![CICD](https://www.edureka.co/blog/content/ver.1531719070/uploads/2018/07/Asset-33-1.png)

# Let's code!

## Build The Application

First, create a folder named `gab2019`. This will be the root folder for today's solution, all subsequent labs during the day will add content and sub-folder to this one. Then let's create a new web application using the .Net Core. Open a terminal, and navigate to your new root folder "gab2019" folder (ex: C:\Dev\gab2019\).

    cd C:\dev\gab2019\

Let's scaffold an application named GABDemo using the following command:

    dotnet new mvc -o GABDemo

Open the solution into Visual Studio Code with:

    code .

## Initialize Local Repository

To create a Git repository you can from Visual Studio Code open a terminal (Ctrl + \`) or return to the terminal windows already open. Make sure your are in the folder `C:\dev\gab2019\` and the following command:

    git init

This command will initialize a local repository at the root of your solution folder. Now let's add all the code files to the repo.

    git add .

This command will add all modified files under your workspace to the index of git.

    git commit -m "Initial commit"

This command will create a new commit containing the current contents of the index and the given log message describing the changes.

Now, Git will track the evolution of our code. Git is a decentralized code repository system, therefore there is usually many repository where you can push and pull. However, before we can push our code to a remote repository we have other task to do. Will come back to it later.

## Create The Azure WebApp

The next step is to create a placeholder for our website.  We will create an empty shell of a web application in Azure with these three Azure CLI commands. You can execute them locally, in a terminal or from the Azure Cloud Shell. The Cloud Shell is a terminal... in Azure! You can access it via the URL [https://shell.azure.com/](https://shell.azure.com).

![CloudShell][CloudShell]

Login to your Azure account

    az login
    
Don't forget to validate that you are in the good subscription. if you are not sure try `az account show` to display information about the current subscription.

> Note: You might experience an error if you webapp name is not unique. (Conflict: Website with given name <web app name> already exists.) Make sure to use a unique name. ex: gab2019<your_initial><2digits_random_number> gab2019AV47

    az group create --name gabdemogroup --location eastus

    az appservice plan create --name gabdemoplan --resource-group gabdemogroup --sku FREE

    az webapp create --name gabdemo --resource-group gabdemogroup --plan gabdemoplan

The first command will create a Resource group. Then inside of this group we create a service plan, and finally we create a webapp to the mix.

To validate that everything has been created, open a internet browser and navigate to the Azure Portal (portal.azure.com). From the left menu select *Resource Groups*. Click on the group *gabdemogroup* that we just created, then click on the web App *gabdemo*

![resourceGroup][resourceGroup]

In the top section of the blade you will found the URL of the web site, click on it. You should see a message saying:"Your App Service app is up and running".  That perfect, our website shell is ready.

## Create an Azure DevOps project

Navigate to [Dev.Azure.com](http://Dev.Azure.com) and if you don't already have an account [create one it's free!](../Lab0/README.md#azure-devops) Once you are logged-in, create a new project by clicking the New project blue button in the top right corner.

![createNewProject][createNewProject]

You will need to provide a unique name and a few simple information. 

## Get the Best Azure DevOps Experience

To get the best of the Azure DevOps portal and from the Azure Pipeline, turn ON a few (or all) option(s) in the Settings. To follow this Lab, the option **New YAML pipeline creation experience** should be on. Do enable it, click in the top-right corner on your avatar, and select the **Preview features** option. Then turn on the feature(s)

![PreviewFeatures][PreviewFeatures]

## Get the Remote Repository Url

Here you have two options you can use the repository provided in the Azure DevOps project you just created, or create a another one in GitHub.

### Option 1: Using Azure DevOps Repos

On Azure DevOps portal, from the left menu select *Repos*. Copy the command under the `or push an existing repository from command line`.

![gitremoteadd][gitremoteadd]

### Option 2: Using GitHub

Azure Pipeline support many different repository. One that is very popular is GitHub. If you don't already have an account [create one it's free!](../Lab0/README.md#github-optional) Once you are logged-in, create a new repository by expending the "+" in the top right corner, then clicking the **New repository** button.

![NewGitHubRepoHere][NewGitHubRepoHere]

Enter the name of your project (ex: Gab2019), and click the **Create repository** green button.

![GithubRepoDetails][GithubRepoDetails]

Now from this page grab the code under `…or push an existing repository from the command line`

![AddGitHubRemote][AddGitHubRemote]

## Add a Remote Repository

Return to the Terminal/ Console and paste the command.

    git remote add origin [URL_FOUND_IN_THE_PREVIOUS_STEP]
    git push origin --all

THe first line is to add the remote repository and name it "origin". The second line is to push (upload) the content of the local repository to origin (the remote one). You will need to enter your credential.

## Continuous Integration

The goal is to have the code to get to compile at every commit. From the Azure DevOps' left menubar, select *Pipelines*, and click the create new button. The first step is to identify where our code is, as you can see Azure DevOps is flexible and accept code from different sources. Select the source you use at the precedent step (option 1: Azure Repos or option 2: GitHub).

> **Note:** If you are using GitHub you will need to Authorize Azure DevOps to your GitHub repository by clicking the Authorize button.

![NewPipeline_step1][NewPipeline_step1]

Select the exact repository.

![NewPipeline_step2][NewPipeline_step2]

This third step is to configure our pipeline. You can start from a template, an existing configuration file or an empty one.  For now, let's take the recommended one (Azure DevOps detected our project was .NET Core) and select the template ASP.NET Core.

![SelectTemplateASPNETCore][SelectTemplateASPNETCore]

This will displays the *YAML code* that defines your pipeline. At this point, the file is not complete, we need to specify where is our project. On the line 16 add `./GABDemo/` (assuming your project is in folder GABDemo).

![UpdateYaml][UpdateYaml]

Click the *Save and run* button to add the `azure-pipelines.yml` file at the root level of your repository and execute it for the first time. Now at every commit, the build will be triggered. To see the status of your build just on to into the build section from the left menubar.

![buildSuccess][buildSuccess]

## Continuous Deployment

Great, our code gets to compile at every commit. It would be nice if the code could also be automatically deployed into our dev environment. To achieve that we need to create a *Release Pipeline*.
And our pipeline will need artifacts. We will edit the `azure-pipelines.yml` to add two new tasks. You can do this directly in the online repository or just from your local machine; remember the file is at the root.  Add these commands:

    - task: DotNetCoreCLI@2
      displayName: 'dotnet publish $(buildConfiguration)'
      inputs:
        command: publish 
        publishWebProjects: True
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
        zipAfterPublish: True

    - task: PublishBuildArtifacts@1
      displayName: 'publish artifacts'

Those two tasks are to publish our application (package it), and make it available in our Artifact folder. To learn more about the type of command available and see example have a look the  excellent documentation at: https://docs.microsoft.com/azure/devops/pipelines/languages/dotnet-core.  Once you are done, save and commit (and push if it was local).

From the left menubar, click on e the *Pipelines*,  select *Release*, and client the *New Release* blue button. Select the template that matches your application. For this lab *Azure App Service deployment* is the one we need. 

![New Release, select a template][NewRelease_step1]

The next thing to will be to rename the environment for something else than *Stage 1*, I named mine "to Azure" but it could be **dev**, **prod** or anything that make sense for you.  Click on the *Add an Artifact* button.

![Release Pipeline][ReleasePipeline]

You will now specify to the pipeline where to pick the artifacts it will deploy. In this case, we want the "output" of our latest build. And I renamed the Source alias as **Drop**.

To get our continuous deployment (CD) we need to enable that trigger by clicking on the little lightning bolt and enabled it.

![TriggerRelease][TriggerRelease]

The last step to configure the Release pipeline is to specify a destination.  By clicking on the "1 job, 1 task" in the middle of the screen (with the little red exclamation point in a circle), that will open the window where we will do that.

Select the subscription you would like to use, and then click on the `Authorize` button on the right. Once it's done go change the `App Service Name`. Click on it and wait 2-3 seconds you should see the app we created with our Azure CLI display.  Select it, and voila!

![SetupReleaseDetails][SetupReleaseDetails]

## Testing time

To test if our CI/CD works we will do a simple code change. First we need to get the latest version of the code. Remember that YAML file was added directly from the web so we don't have it locally.

### Git pull

To get the latest version of the code we need to do a `git pull`. You can execute the command directly from the terminal. However, Visual Studio is also well integrated with Git so let's try that way.

![git-pull][git-pull]

From the left menu, select the Source Control icon (third one from the top), that will open the source control tab.  Click on the ellipsis [...] button and select pull.

Now that you have the last version of the code, go in the Home controller and change one of the message. Don't forget to save your work.  

The last steps you need to do is a git commit, and a git push. Once more you can do it directly from the terminal or from visual studio.

That last push should have trigger our build pipeline. go back into Azure DevOps to see if it's In progress. Once the build is done the Release pipeline should start the deployment.

Once the deployment is done, refresh your web browser where the web page was shown "Your App Service app is up and running" message previously...

## Reference

- [Build, test, and deploy .NET Core apps in Azure Pipelines](https://docs.microsoft.com/azure/devops/pipelines/languages/dotnet-core?WT.mc_id=globalazure-github-frbouche)

## End

[Previous Lab](../Lab0/README.md)
[Next Lab](../Lab2/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[createNewProject]: medias/createNewProject.png
[NewPipeline_step1]: medias/NewPipeline_step1.png
[NewPipeline_step2]: medias/NewPipeline_step2.png
[NewPipeline_step3]: medias/NewPipeline_step3.png
[buildSuccess]: medias/buildSuccess.png

[AddArtifact]: medias/AddArtifact.png
[NewRelease_step1]: medias/NewRelease_step1.png
[ReleasePipeline]: medias/ReleasePipeline.png
[SetupReleaseDetails]: medias/SetupReleaseDetails.png
[TriggerRelease]: medias/TriggerRelease.png
[git-pull]: medias/git-pull.png
[resourceGroup]: medias/resourceGroup.png
[gitremoteadd]: medias/gitremoteadd.png
[NewGitHubRepoHere]: medias/NewGitHubRepoHere.png
[GithubRepoDetails]: medias/GithubRepoDetails.png
[AddGitHubRemote]: medias/AddGitHubRemote.png
[SelectTemplateASPNETCore]: medias/SelectTemplateASPNETCore.png
[UpdateYaml]: medias/UpdateYaml.png
[PreviewFeatures]: medias/PreviewFeatures.png
[CloudShell]: medias/CloudShell.png

