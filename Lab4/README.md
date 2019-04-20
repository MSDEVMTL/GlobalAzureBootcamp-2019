![gablogo][gablogo]

# Lab 4 - Docker Container part 2

## Goal

In the previous lab, you deployed the container manually but it would be awesome if we could automate the process so each time you'd checkin a code update, Azure DevOps would compile the code, deploy it to a container and push it to Azure Container Registry (ACR) automatically.  Like in the previous lab, the Web App will be updated using a Webhook. 

Magical! To do so, let's start by creating a new pipeline.

## Let's code!

### Create a new pipeline

Head to dev.azure.com, click on the **Pipelines** menu and the **New Pipeline** button.

![01][01]


Click on the **Use the Visual Designer** link.

![02][02]


Select **Azure Repos Git** and the team project you created earlier.

![03][03]


Select the **Docker Container template**.  This template has 2 steps that needs to be configured: Build and Push.

![04][04]


### Configure the Build step

Let's configure the Build step.  Select your **subscription** and your **Azure Container Registry**.  

![05][05]


Locate the **Dockerfile** in your project.

![06][06]


### Configure the Push step

Let's now configure the Push step that will push the container to ACR.  Click on the **Push an Image** step.

![07][07]


Select your **subscription** and your **Azure Container Registry**.  Check **Include Latest Tag**.

![08][08]


Click on **Save**.  You’ll be prompted for a location in your repo, accept the defaults and click **Save**.

![09][09]


### Enable continuous integration

Click on the **Triggers** tab and check **Enable continuous integration**.

![10][10]


### Configurate the Web App

In the Web App created in the previous lab, change the container settings to pint to the container located in the Azure Container Registry that was just built by the pipeline.  The startup file is **GABCDemo.dll**.  Set **Continuous Deployment** to On.


### Test the Web App

Launch a browser and test the app.


### Commit a change

Back in **Code**, , make a simple change to a page, commit and push the change.  The pipeline will pick the change, build the code, create a container and deploy it to ACR.  The Web App will pick the change using the Webhook and pull the new container.


## Reference

[Deploy to an Azure Web App for Containers](https://docs.microsoft.com/en-us/azure/devops/pipelines/apps/cd/deploy-docker-webapp?WT.mc_id=globalazure-github-frbouche&view=azdevops)

## End
[Previous Lab](../Lab3/README.md)
[Next Lab](../Lab5/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[01]: medias/Lab4-01.png
[02]: medias/Lab4-02.png
[03]: medias/Lab4-03.png
[04]: medias/Lab4-04.png
[05]: medias/Lab4-05.png
[06]: medias/Lab4-06.png
[07]: medias/Lab4-07.png
[08]: medias/Lab4-08.png
[09]: medias/Lab4-09.png
[10]: medias/Lab4-10.png
