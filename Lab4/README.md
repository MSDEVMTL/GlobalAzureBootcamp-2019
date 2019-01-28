![gablogo][gablogo]

# Lab 4 - Docker Container part 2

## Goal

In the previous lab, you deployed the container manually but it would be awesome if we could automate the process so each time you'd checkin a code update, Azure DevOps would compile the code, deploy it to a container and push it to Azure Container Registry (ACR) automatically.  Like in the previous lab, the Web App will be updated using a Webhook to our container in ACR. Magical! To do so, we'll create a new pipeline.

## Let's code!

Head to dev.azure.com, click on the **Pipelines** menu and the **New Pipeline** button.
![01][01]

Click on the **Use the Visual Designer** link.
![02][02]

Select **Azure Repos Git** and the team project you created earlier.
![03][03]

Select the **Docker Container template**.
![04][04]

Select your **subscription** and your **Azure Container Registry**.  
![05][05]

Locate the **Dockerfile**.
![06][06]

Click on the **Push an Image** step.
![07][07]

Select your **subscription** and your **Azure Container Registry**.  Check **Include Latest Tag**.
![08][08]

Click on **Save**.  You’ll be prompted for a location in your repo, accept the defaults and click **Save**.
![09][09]

Click on the **Triggers** tab and check **Enable continuous integration**.
![10][10]

Create a **Webhook** from you Web App to your container following the steps you already did in the previous lab.

Back in **Code**, , make a simple change to a page, commit and push the change.  The pipeline will pick the change, build the code, create a container and deploy it to ACR.  The Web App will pick the change using the Webhook and pull the new container.

## Reference

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
