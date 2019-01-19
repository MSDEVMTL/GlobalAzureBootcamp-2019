![gablogo][gablogo]

# Lab 1 - Continouus Integration and Continuous Deployment (CI/CD) 

## Goal

In this lab you will create an application in .Net Core, push it to a remote repository and create a Continouus Integration and Continuous Deployment (CI/CD) with the Azure DevOps Pipeline to deploy the website to Azure. 

## Let's code!

### Build The Application

Let's create a new web application using the .Net Core. Open a terminal, and navigate to a "dev" folder (ex: C:\Dev).

    cd C:\dev

Let's scafold an application named GABCDemo using the following command:

    dotnet new mvc -o GABCDemo

Go into the new folder where your application as just been created.

    cd GABCDemo

Open the solution into Visual Studio Code with:

    code .

### Initialize Local Repository

To create a Git repository you can from Visual Studio Code open a terminal (Ctrl + \`) or return to the terminal windows already open. You should be in the folder `C:\Dev\GABCDemo`. Type

    git Init

This command will initialize a local repository. Now let's add alll the code files to the repo.

    git add -A -m "initil commit"

Now Git will track the evolution of our code. Git is a decentralized code repository system, therefore there is usually many repository where you can push and pull. However, before we can push our code to a remote repository we have other task to do. Will come back to it later.


### Creat The Azure WebApp

The next step is to create a placeholder for our website.  We will create an empty shell of a web application in Azure with these three Azure CLI commands. You can execute them locally or from the Cloud Shell. (Don't forget to validate that you are in the good subscription)

    az group create --name gabcdemogroup --location eastus

    az appservice plan create --name gabcdemoplan --resource-group gabcdemogroup --sku FREE

    az webapp create --name gabcdemo --resource-group gabcdemogroup --plan gabcdemoplan

The first command will create a Resource group. Then inside of this group we create a service plan, and finally we create a webapp to the mix. 

To validate that everything has been created, open a internet browser and navigate to the Azure Portal (portal.azure.com). From the left menu select *Resource Groups*. Click on the group *gabcdemogroup* that we just created, then click on the web App *gabcdemo*

![resourceGroup][resourceGroup]

In the top section of the blade you will found the URL of the web site, click on it. You should see a meesage saying:"Your App Service app is up and running".  That perfect, our website shell is ready.

### Create an Azure DevOps project 

Navigate to [Dev.Azure.com](http://Dev.Azure.com) and if you don't already have an account create one it's free! Once you are logged-in, create a new project by clicking the New project blue button in the top right corner.

![createNewProject][createNewProject]

You will need to provide a unique name and a few simple information. 

### Get the Remote Repository Url

Here you have two options you can use the repository provided in the Azure DevOps project you just created, or create a another one in GitHub.

#### Option 1: Using Azure DevOps Repos

On Azure DevOps portal, from the left menu select *Repos*. Copy the command under the `or push an existing repository from command line`.

[ADD AN IMAGE]

#### Option 2: Using GitHub

[SOON]

### Add a Remote Repository

Return to the Terminal/ Console and paste the command.

    git remote add origin https:..... 
    git push origin --all

THe first line is to add the remote repository and name it "origin". The second line is to push (upload) the content of the local repository to origin (the remote one). You will need to enter your credential.

> Note: You may need to add some creds from the Setting in Azure DevOps [NEED TO TEST AND DOCUMENT THIS]

### Continuous Integration

The goal is to have the code to get to compile at every commit. From the Azure DevOps' left menubar, select *Pipelines*, and click the create new button. The first step is to identify where our code is, as you can see Azure DevOps is flexible and accept code from diffrent sources. Select the source you use at the present step.

![NewPipeline_step1][NewPipeline_step1]

Select the exact repository.

![NewPipeline_step2][NewPipeline_step2]

This third step displays the YAML code that defines your pipeline. At this point, the file is not complete, but it's enough to build, we will come back to it later. Click the *Add* button to add the `azure-pipelines.yml` file at the root level of your repository.

![NewPipeline_step3][NewPipeline_step3]

The build pipeline is ready click the *Run* button to execute it for the first time. Now at every commit, the build will be triggered. To see the status of your build just on to into the build section from the left menubar.

![buildSuccess][buildSuccess]

### Continous Deployment

Great, our code gets to compile at every commit. It would be nice if the code could also be automatically deployed into our dev environment. To acheive that we need to create a *Release Pipeline*.
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

From the left menubar, click on e the *Pipeles*,  select *Release*, and clienk the *New Release* blue button. Select the template that matches your application. For this post *Azure App Service deployment* is the one we need. 

![New Release, select a template][NewRelease_step1]

The next thing to will be to rename the environment for something else than *Stage 1*, I named mine "to Azure" but it could be **dev**, **prod** or anything that make sense for you.  Click on the *Add an Artifact* button.

![Release Pipeline][ReleasePipeline]

You will now specify to the pipeline where to pick the artifacts it will deploy. In this case, we want the "output" of our latest build. And I renamed the Source alias as **Drop**.

![AddArtifact][AddArtifact]

To get our continuous deployment (CD) we need to enable that trigger by clicking on the little lightning bolt and enabled it.

![TriggerRelease][TriggerRelease]

The last step to configure the Release pipeline is to specify a destination.  By clicking on the "1 job, 1 task" in the middle of the screen (with the little red exclamation point in a circle), that will open the window where we will do that.

Select the subscription you would like to use, and then click on the `Authaurize` button on the right. Once it's done go change the `App Service Name`. Click on it and wait 2-3 seconds you should see the app we created with our Azure CLI display.  Select it, and voila!

![SetupReleaseDetails][SetupReleaseDetails]

### Testing time

To test if our CI/CD works we will do a simple code change. First we need to get the latest version of the code. Remember that YAML file was added directly from the web so we don't have it locally. 

#### Git pull

To get the lastes version of the code we need to do a `git pull`. You can execute the command directly from the terminal. However, Visual Studio is also well integrated with Git so let's try that way.

![git-pull][git-pull]

From the left menu, select the Source Control icon (third one from the top), that will open the source control tab.  Click on the elipse [...] button and select pull.

Now that you have the last version of the code, go in the Home controller and change one of the message. Don't forget to save your work.  

The last steps you need to do is a git commit, and a git push. Once more you can do it dirrectly from the terminal or from visual studio.

That last push should have trigger our build pipeline. go back into Azure DevOps to see if it's In progress. Once the build is done the Release pipeline should start the deployment.

Once the deployment is done, refresh your web browser where the web page was showinf the "Your App Service app is up and running" mesage previously...


## Reference

- [Build, test, and deploy .NET Core apps in Azure Pipelines](https://docs.microsoft.com/azure/devops/pipelines/languages/dotnet-core)

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
[git-pull]: medais/git-pull.png
[resourceGroup]: medias/resourceGroup.png