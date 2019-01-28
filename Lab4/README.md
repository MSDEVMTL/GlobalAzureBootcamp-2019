![gablogo][gablogo]

# Lab 4 - Docker Container part 2

## Goal

In the previous lab, you deployed the container manually but it would be awesome if we could automate the process so each time you'd checkin a code update, Azure DevOps would compile the code, deploy it to a container and push it to Azure Container Registry (ACR) automatically.  Like in the previous lab, the Web App will be updated using a Webhook to our container in ACR. Magical! To do so, we'll create a new pipeline.

## Let's code!

Head to dev.azure.com, click on the **Pipelines** menu and the **New Pipeline** button.

Click on the **Use the Visual Designer** link.

Select **Azure Repos Git** and the team project you created earlier.

Select the **Docker Container template**.

Select your **subscription** and your **Azure Container Registry**.  

Locate the **Dockerfile**.

Click on the **Push an Image** step.

Select your **subscription** and your **Azure Container Registry**.  Check **Include Latest Tag**.

Click on **Save**.  You’ll be prompted for a location in your repo, accept the defaults and click **Save**.

Click on the **Triggers** tab and check **Enable continuous integration**.

Create a **Webhook** from you Web App to your container following the steps you already did in the previous lab.

Back in **Code**, , make a simple change to a page, commit and push the change.  The pipeline will pick the change, build the code, create a container and deploy it to ACR.  The Web App will pick the change using the Webhook and pull the new container.

## Reference

## End
[Previous Lab](../Lab3/README.md)
[Next Lab](../Lab5/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
