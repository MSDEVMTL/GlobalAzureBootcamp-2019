﻿# Global Azure Bootcamp 2019 - Montréal, Ottawa & Halifax, Canada

![gablogo][gablogo]

All around the world user groups and communities want to learn about Azure and Cloud Computing!

On **April 27, 2019**, all communities will come together once again in the sixth great [Global Azure Bootcamp](https://global.azurebootcamp.net/) event! 

In this repository you will find included all materials used during the demos so you can try them at home.


## Scenario 2019
Life has been going very well at *Contoso* it's been a while since they start their journey on Azure. Things have been so well that today *Contoso* has many developers across the many different locations and using different platforms.  They want to build an new application that could be secure and accessible from everywhere to manage their images. They would also like to have the application to be easily maintainable and to follow the best practices.

In an effort to improve their efficiency and the quality of their application they decided to agree on the following:

* Use cross-platform tools as much as possible. 
* Continuous Integration and Continuous Deployment solution (CI/CD).
* A way to reproduce the environment quickly in any new environment.
* They would like to be the application "portable".
* Use Artificial Intellegent is an excellent way to bonify their application and distinguish themselves from the competition.
* Get rid of their old process.

The team has been assigned a couple of days to find tools & put new processes in place to check all the action items of the list above. Excited, the team will actively start working on this tomorrow and documenting their progress.

## Overview of the day

### [Lab 0 - What you need to install (must do >>before<< the bootcamp!)](./Lab0/README.md) 


Time        | Duration | Tittle                                      | Description  
:----:      | :------: | :-------                                    | :----------- 
08h30-9h00  | 30 mins  | [Arrival/ Coffee](./Locations/README.md)    | Grab a seat, get your coffee, set up your environment on your laptop...
09h00-9h15  | 15 mins  | Intro                                       | Intro, agenda
09h15-10h15 | 60 mins  | [Lab 1 - CI/CD ](./Lab1/README.md)          | Create an .Net Core App, put it in GitHub/ or Azure DevOps Repo. Create an Azure DevOps CI/CD, and deploy. a webapp.
10h15-11h00 | 45 mins  | [Lab 2 - ARM template](./Lab2/README.md)    | Create an ARM template, also add a storage to the mix, modify the CICD to deploy.
11h00-11h45 | 45 mins  | [Lab 3 - Container 1](./Lab3/README.md)     | Package the Application in a Docker container. Test it locally and deploy it manually to Azure Container. Services.
11h45-12h30 | 45 mins  | [Lab 4 - Containers 2](./Lab4/README.md)    | Create a new CI/CD pipeline to automate the container creation and deployment. 
12h30-13h15 | 45 mins  | Lunch                                       | Lunch
13h15-14h00 | 45 mins  | [Lab 5 - AI](./Lab5/README.md)              | Add an Azure Cognitive Services Vision to the application to validate an image. 
14h00-14h45 | 45 mins  | [Lab 6 - Serverless](./Lab7/readme.md)      | Create an Azure Functions (blob trigger) to process all images in a blob storage and use the Vision API for filtering. 
14h45-15h30 | 45 mins  | [Lab 7 - Security](./Lab7/README.md)        | Move all the keys and sensitive information to a KeyVault. 
15h30-16h00 | 30 mins  | Ending                                      | Questions/ Networking/ Giveaway

## Technology Stack required for the Global Azure Bootcamp 2019

All Platforms (Windows, MacOS, Linux)

* [Visual Studio Code](https://code.visualstudio.com/?WT.mc_id=globalazure-github-frbouche&wt.mc_id=vscom_downloads)
* [Azure CLI](https://docs.microsoft.com/en-us/cli/azure/install-azure-cli?WT.mc_id=globalazure-github-frbouche&view=azure-cli-latest)
* [Git](https://git-scm.com/downloads)
* [Docker](https://www.docker.com/get-started)

## What you will need before the bootcamp?

Before the bootcamp, we encourage you to make sure you have all the requirement software so you can concentrate on learning and not running installations.  Make sure you get all the source code and install all the required software.  See **[Lab 0)](./Lab0/README.md)**.

### Azure Subscription
If you don't own an Azure subscription already, you can create your free account today. It comes with 200$ credit, so you can experience almost everything without spending a dime. 

Make sure to have your account up and ready before the bootcamp.

[Create your free Azure account today](https://azure.microsoft.com/en-us/free/?WT.mc_id=globalazure-github-frbouche)


[gablogo]: ./medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
