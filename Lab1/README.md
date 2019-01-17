![gablogo][gablogo]

# Lab 1 - Continouus Integration and Continuous Deployment (CI/CD) 

## Goal

In this lab you will create an application in .Net Core, push it to a remote repository and create a Continouus Integration and Continuous Deployment (CI/CD) with the Azure DevOps Pipeline to deploy the website to Azure. 

## Let's code!

### The Application

Let's create a new web application using the .Net Core. Open a terminal, and navigate to a "dev" folder (ex: C:\Dev).

    cd C:\dev

Let's scafold an application named GABCDemo using the following command:

    dotnet new mvc -o GABCDemo

Go into the new folder where your application as just been created.

    cd GABCDemo

Open the solution into Visual Studio Code with:

    code .

### Local Repository

To create a Git repository you can from Visual Studio Code open a terminal (Ctrl + \`) or return to the terminal windows already open. You should be in the folder `C:\Dev\GABCDemo`. Type

    git Init

This command will initialize a local repository. Now let's add alll the code files to the repo.

    git add -A

Now Git will track the evolution of our code. Git is a decentralized code repository system, therefore there is usually many repository where you can push and pull. However, before we can push our code to a remote repository we have other task to do. Will come back to it later.


### The Azure WebApp

The next step is to create a placeholder for our website.  We will create an empty shell of a web application in Azure with these three Azure CLI commands. You can execute them locally or from the Cloud Shell. (Don't forget to validate that you are in the good subscription)

    az group create --name gabcdemogroup --location eastus

    az appservice plan create --name gabcdemoplan --resource-group gabcdemogroup --sku FREE

    az webapp create --name gabcdemo --resource-group gabcdemogroup --plan gabcdemoplan

The first command will create a Resource group. Then inside of this group we create a service plan, and finally we create a webapp to the mix. 


### Create an Azure DevOps project 

Navigate to [Dev.Azure.com](http://Dev.Azure.com) and if you don't already have an account create one it's free! Once you are logged-in, create a new project by clicking the New project blue button in the top right corner.

![createNewProject][createNewProject]

You will need to provide a unique name and a few simple information. 



## Reference

## End
[Previous Lab](../Lab0/README.md)
[Next Lab](../Lab2/README.md)

[gablogo]: ../medias/GlobalAzureBootcamp2019.png "Global Azure Bootcamp 2019"
[createNewProject]: medias/createNewProject.png